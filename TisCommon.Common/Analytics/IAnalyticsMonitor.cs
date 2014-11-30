using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Analytics
{
    public interface IAnalyticsMonitor
    {
        /// <summary>
        /// Track a feature.
        /// </summary>
        /// <param name="moduleName">The module name that tracks the feature. 
        /// Ex in the client: CollectionOrganizer, Completion, FilePortal..
        /// Ex in the server: License, Workflow..</param>
        /// <param name="category">Logical group inside the module.
        /// Ex for CollectionOrganizer: SdkOperations, UIOperations.
        /// Ex for Completion: LayoutOperations, ImageOperations.</param>
        /// <param name="featureName">The feature that needs to be trakced.
        /// Ex from Completion_ImageOperations: ZoomIn, ZoomOut...</param>
        void TrackFeature(string moduleName, string category, string featureName);

        /// <summary>
        /// Track a feature as occured more the once.
        /// Example: If the user performed 5 times zoom-in, you can call 5 times TrackFeature method
        /// or 1 time TrackFeatureValue with trackedValue = 5.
        /// </summary>
        /// <param name="moduleName">The module name that tracks the feature. 
        /// Ex in the client: CollectionOrganizer, Completion, FilePortal..
        /// Ex in the server: License, Workflow..</param>
        /// <param name="category">Logical group inside the module.
        /// Ex for CollectionOrganizer: SdkOperations, UIOperations.
        /// Ex for Completion: LayoutOperations, ImageOperations.</param>
        /// <param name="featureName">The feature that needs to be trakced.
        /// Ex from Completion_ImageOperations: ZoomIn, ZoomOut...</param>
        void TrackFeatureValue(string moduleName, string category, string featureName, long trackedValue);

        /// <summary>
        /// Track a feature as started, for measuring the time the feature was executed.
        /// </summary>
        /// <param name="moduleName">The module name that tracks the feature. 
        /// Ex in the client: CollectionOrganizer, Completion, FilePortal..
        /// Ex in the server: License, Workflow..</param>
        /// <param name="category">Logical group inside the module.
        /// Ex for CollectionOrganizer: SdkOperations, UIOperations.
        /// Ex for Completion: LayoutOperations, ImageOperations.</param>
        /// <param name="featureName">The feature that needs to be trakced.
        /// Ex from Completion_ImageOperations: ZoomIn, ZoomOut...</param>
        void TrackFeatureStart(string moduleName, string category, string featureName);

        /// <summary>
        /// Track a feature as stoped, for measuring the time the feature was executed.
        /// </summary>
        /// <param name="moduleName">The module name that tracks the feature. 
        /// Ex in the client: CollectionOrganizer, Completion, FilePortal..
        /// Ex in the server: License, Workflow..</param>
        /// <param name="category">Logical group inside the module.
        /// Ex for CollectionOrganizer: SdkOperations, UIOperations.
        /// Ex for Completion: LayoutOperations, ImageOperations.</param>
        /// <param name="featureName">The feature that needs to be trakced.
        /// Ex from Completion_ImageOperations: ZoomIn, ZoomOut...</param>
        void TrackFeatureStop(string moduleName, string category, string featureName);

        /// <summary>
        /// Cancel tracking a feature time. 
        /// NOTE: This method should be called after TrackFeatureStart in case you want to cancel the feature time measurement.
        /// </summary>
        /// <param name="moduleName">The module name that tracks the feature. 
        /// Ex in the client: CollectionOrganizer, Completion, FilePortal..
        /// Ex in the server: License, Workflow..</param>
        /// <param name="category">Logical group inside the module.
        /// Ex for CollectionOrganizer: SdkOperations, UIOperations.
        /// Ex for Completion: LayoutOperations, ImageOperations.</param>
        /// <param name="featureName">The feature that needs to be trakced.
        /// Ex from Completion_ImageOperations: ZoomIn, ZoomOut...</param>
        void TrackFeatureCancel(string moduleName, string category, string featureName);

        /// <summary>
        /// Sends a message.
        /// </summary>
        void SendLog(string logMessage);

        /// <summary>
        /// Sends installation information.
        /// </summary>
        void SetInstallationInfo(string installationId, IDictionary<string, string> properties);

        /// <summary>
        /// Send exception if occured.
        /// </summary>
        void TrackException(Exception exception, string contextMessage);
    }
}
