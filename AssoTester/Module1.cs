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
using System.Windows.Input;

namespace AssoTester
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("AssoTester_Module");

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

        protected override bool Initialize()
        {
            //var mapItem = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(x => x.Name == "AMFM Editor");
            //QueuedTask.Run(() =>
            //{
            //    var map = mapItem.GetMap();

            //    var layers = map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
            //    foreach (var layer in layers)
            //    {
            //        if (layer is FeatureLayer)
            //        {
            //            var bfLayer = (FeatureLayer)layer;
            //            var table = bfLayer.GetTable();
            //            if (bfLayer.Name == "Stormwater Culvert Path Boundary") RowDeletedEvent.Subscribe(OnRowDeleted, table);
            //        }
            //    }
            //});
            //EditCompletedEvent.Subscribe(OnEditCompleted);
            return base.Initialize();
        }
        
        private Task OnEditCompleted(EditCompletedEventArgs args)
        {
            
            return Task.CompletedTask;
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
