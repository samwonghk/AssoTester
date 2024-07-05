using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.UtilityNetwork;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Events;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssoTester.Tools
{
    internal class DeleteWithAsso : Button
    {
        protected override void OnClick()
        {
            QueuedTask.Run(() => {
                var selections = MapView.Active.Map.GetSelection();
                foreach (var selection in selections.ToDictionary())
                {
                    if (selection.Key is FeatureLayer featureLayer)
                    {
                        var featureClass = featureLayer.GetFeatureClass();
                        var eventToken = RowDeletedEvent.Subscribe(OnRowDeleted, featureClass);

                        if (featureClass is not null)
                        {
                            var ctrlDatasets = featureClass.GetControllerDatasets();
                            foreach (var ctrlDataset in ctrlDatasets)
                            {
                                if (ctrlDataset is UtilityNetwork un)
                                {
                                    EditOperation editOperation = new EditOperation();
                                    editOperation.Name = "Delete with Associations";
                                    editOperation.EditOperationType = EditOperationType.Long;
                                    var qf = new QueryFilter() { ObjectIDs = selection.Value };
                                    var rows = featureLayer.Search(qf);

                                    while (rows.MoveNext())
                                    {
                                        var currentFeature = rows.Current;
                                        editOperation.Delete(currentFeature);
                                    }

                                    var result = editOperation.Execute();
                                    MessageBox.Show(result.ToString());
                                    if (!result) MessageBox.Show(editOperation.ErrorMessage);
                                }
                            }
                        }
                        RowDeletedEvent.Unsubscribe(eventToken);
                    }
                }
            });
        }
        private void OnRowDeleted(RowChangedEventArgs args)
        {

            var featureClass = args.Row.GetTable() as FeatureClass;
            if (featureClass is not null)
            {
                var ctrlDatasets = featureClass.GetControllerDatasets();
                foreach (var ctrlDataset in ctrlDatasets)
                {
                    if (ctrlDataset is UtilityNetwork un)
                    {
                        var currentFeature = args.Row;
                        var element = un.CreateElement(currentFeature);
                        var assocs = un.GetAssociations(element);
                        foreach (var assoc in assocs)
                        {
                            un.DeleteAssociation(assoc);
                        }

                    }
                }
            }
        }
    }
}
