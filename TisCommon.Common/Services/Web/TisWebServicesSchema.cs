
namespace TiS.Core.TisCommon.Services
{
    public partial class TisServicesSchema
    {
        #region Web services

        // BasicConfigurationServer
        public static readonly ITisServiceInfo BasicConfigurationServer = CreateWebServiceInfo(
            "BasicConfigurationServer",
            null,
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.TisCommon.Configuration.BasicConfigurationServer,TiS.Core.TisCommon.Server",
            true);

        // ApplicationPathProvider
        public static readonly ITisServiceInfo ApplicationPathProvider = CreateWebServiceInfo(
            "ApplicationPathProvider",
            null,
            "TiS.Core.Domain.Services.Web.ApplicationPathProviderCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.TisCommon.Configuration.ApplicationPathProvider,TiS.Core.TisCommon.Server",
            true);

        // DbAccessServer
        public static readonly ITisServiceInfo DbAccessServer = CreateWebServiceInfo(
            "DbAccessServer",
			"TiS.Core.Application.DbAccess.IDbAccessWebService,TiS.Core.Application.Common",
			"TiS.Core.Application.DbAccess.DbAccessWebServiceCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.DbAccess.DbAccessWebService,TiS.Core.Application.Server");

        // DomainManagementServer
        public static readonly ITisServiceInfo DomainManagementServer = CreateWebServiceInfo(
            "DomainManagementServer",
            "TiS.Core.Domain.Management.IManagementService,TiS.Core.Domain.Common",
            "TiS.Core.Domain.Services.Web.DomainManagementServerCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Management.ManagementService,TiS.Core.Domain.Server");

		// MonitorCountersService
		public static readonly ITisServiceInfo MonitorCountersService = CreateWebServiceInfo(
			"MonitorCountersService",
			"TiS.Core.Domain.Counters.IMonitorCountersService,TiS.Core.Domain.Common",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Domain.Counters.MonitorCountersService,TiS.Core.Domain.Server");

		// TaskManagerServer
        public static readonly ITisServiceInfo TaskManagerServer = CreateWebServiceInfo(
            "TaskManagerServer",
            "TiS.Core.Domain.DistributedTask.ITaskManagerService,TiS.Core.Domain.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.DistributedTask.TaskManagerService,TiS.Core.Domain.Server");

        // SetupStorageServer
        public static readonly ITisServiceInfo SetupStorageServer = CreateWebServiceInfo(
            "SetupStorageServer",
            "TiS.Core.TisCommon.Storage.ITransactionalStorage,TiS.Core.TisCommon.Common",
            "TiS.Core.Domain.Services.Web.SetupStorageServerCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Storage.SetupStorageService,TiS.Core.Domain.Server");

        // DynamicStorageServer
        public static readonly ITisServiceInfo DynamicStorageServer = CreateWebServiceInfo(
            "DynamicStorageServer",
            "TiS.Core.TisCommon.Storage.ITransactionalStorage, TiS.Core.TisCommon.Common",
            "TiS.Core.Application.Storage.DynamicStorageServerCreator, TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Storage.DynamicStorageService, TiS.Core.Application.Server");

        // SecurityPolicyProviderServer
        public static readonly ITisServiceInfo SecurityPolicyProviderServer = CreateWebServiceInfo(
            "SecurityPolicyProviderServer",
            "TiS.Core.TisCommon.Security.ITisSecurityPolicyProviderService,TiS.Core.TisCommon.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.TisCommon.Security.TisSecurityPolicyProviderService,TiS.Core.TisCommon.Server");

        // BasicConfigurationServerService
        public static readonly ITisServiceInfo BasicConfigurationServerService = CreateWebServiceInfo(
            "BasicConfigurationServerService",
            "TiS.Core.TisCommon.Configuration.IBasicConfigurationServerService,TiS.Core.TisCommon.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.TisCommon.Configuration.BasicConfigurationServerService,TiS.Core.TisCommon.Server");

        // SetupFlowsetService
        public static readonly ITisServiceInfo SetupFlowsetService = CreateWebServiceInfo(
            "SetupFlowsetService",
            "TiS.Core.Application.Setup.ISetupFlowsetService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.SetupFlowsetServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Setup.SetupFlowsetService,TiS.Core.Application.Server");

        // CollectionNameCounterService
        public static readonly ITisServiceInfo CollectionNameCounterService = CreateWebServiceInfo(
            "CollectionNameCounterService",
            "TiS.Core.Application.Dynamic.Counter.ICollectionNameCounterService,TiS.Core.Application.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Dynamic.Counter.CollectionNameCounterService,TiS.Core.Application.Server");

        // GroupIdCounterService
        public static readonly ITisServiceInfo GroupIdCounterService = CreateWebServiceInfo(
            "GroupIdCounterService",
            "TiS.Core.Application.Dynamic.Counter.IGroupIdCounterService,TiS.Core.Application.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Dynamic.Counter.GroupIdCounterService,TiS.Core.Application.Server");

        // LicenseServer
        public static readonly ITisServiceInfo LicenseServer = CreateWebServiceInfo(
            "LicenseServer",
            "TiS.Core.Domain.License.ILicenseService,TiS.Core.Domain.Common",
            "TiS.Core.Domain.Services.Web.LicenseServerCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.License.LicenseService,TiS.Core.Domain.Server");

        #region Statistics

        // StatisticsConfigurationService
        public static readonly ITisServiceInfo StatisticsConfigurationService = CreateWebServiceInfo(
            "StatisticsConfigurationService",
            "TiS.Core.Application.Statistics.IStatisticsConfigurationService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.StatisticsConfigurationServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Statistics.StatisticsConfigurationService,TiS.Core.Application.Server");

        // StatisticsService
        public static readonly ITisServiceInfo StatisticsService = CreateWebServiceInfo(
            "StatisticsService",
            "TiS.Core.Application.Statistics.IStatisticsService,TiS.Core.Application.Common",
			"TiS.Core.Application.Statistics.StatisticsServerCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Statistics.StatisticsService,TiS.Core.Application.Server");

        #endregion

        //ApplicationConfigurationService
        public static readonly ITisServiceInfo ApplicationConfigurationService = CreateWebServiceInfo(
          "ApplicationConfigurationService",
          "TiS.Core.TisCommon.Configuration.ISpecificConfigurationService,TiS.Core.TisCommon.Common",
          "TiS.Core.Domain.Services.Web.ApplicationConfigurationServiceCreator,TiS.Core.Domain.Server",
          TisServicesConst.SERVER_REQUIRED,
          "TiS.Core.Domain.Configuration.ApplicationConfigurationService,TiS.Core.Domain.Server");

        //StationConfigurationService
        public static readonly ITisServiceInfo StationConfigurationService = CreateWebServiceInfo(
          "StationConfigurationService",
          "TiS.Core.TisCommon.Configuration.ISpecificConfigurationService,TiS.Core.TisCommon.Common",
          "TiS.Core.Application.Services.StationConfigurationCreator,TiS.Core.Application.Server",
          TisServicesConst.SERVER_REQUIRED,
          "TiS.Core.Application.Configuration.StationConfigurationService,TiS.Core.Application.Server");

        #region Workflow

        // WorkflowCounterService
        public static readonly ITisServiceInfo WorkflowCounterService = CreateWebServiceInfo(
            "WorkflowCounterService",
            "TiS.Core.Application.Workflow.IWorkflowCounterService,TiS.Core.Application.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowCounterService,TiS.Core.Application.Server");

        // WorkflowOperationsService
        public static readonly ITisServiceInfo WorkflowOperationsService = CreateWebServiceInfo(
            "WorkflowOperationsService",
            "TiS.Core.Application.Workflow.IWorkflowOperationsService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.WorkflowOperationsServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowOperationsService,TiS.Core.Application.Server");

        public static readonly ITisServiceInfo ControllerService = CreateWebServiceInfo(
            "ControllerService",
            "TiS.Core.Application.Controller.IControllerService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.ControllerServiceCreator, TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Controller.ControllerService,TiS.Core.Application.Server");

        public static readonly ITisServiceInfo InvoiceReaderService = CreateWebServiceInfo(
            "InvoiceReaderService",
            "TiS.Core.Application.InvoiceReader.IInvoiceReaderService,TiS.Core.Application.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator, TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.InvoiceReader.InvoiceReaderService,TiS.Core.Application.Server");

        // WorkflowUnitsQueryService
        public static readonly ITisServiceInfo WorkflowUnitsQueryService = CreateWebServiceInfo(
            "WorkflowUnitsQueryService",
            "TiS.Core.Application.Workflow.IWorkflowUnitsQueryService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.WorkflowUnitsQueryServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowUnitsQueryService,TiS.Core.Application.Server");

        // WorkflowSessionService
        public static readonly ITisServiceInfo WorkflowSessionService = CreateWebServiceInfo(
            "WorkflowSessionService",
            "TiS.Core.Application.Workflow.IWorkflowSessionService,TiS.Core.Application.Common",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowSessionService,TiS.Core.Application.Server");

        // WorkflowSchemaService
        public static readonly ITisServiceInfo WorkflowSchemaService = CreateWebServiceInfo(
            "WorkflowSchemaService",
            "TiS.Core.Application.Workflow.IWorkflowSchemaService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.WorkflowSchemaServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowSchemaService,TiS.Core.Application.Server");

        // WorkflowUnitsControlService
        public static readonly ITisServiceInfo WorkflowUnitsControlService = CreateWebServiceInfo(
            "WorkflowUnitsControlService",
            "TiS.Core.Application.Workflow.IWorkflowUnitsControlService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.WorkflowUnitsControlServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowUnitsControlService,TiS.Core.Application.Server");

        // DynamicPriorityService
        public static readonly ITisServiceInfo DynamicPriorityService = CreateWebServiceInfo(
            "DynamicPriorityService",
            "TiS.Core.Application.Workflow.Priority.IDynamicPriorityService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.DynamicPriorityServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Workflow.Priority.DynamicPriorityService,TiS.Core.Application.Server");

        // DynamicService
        public static readonly ITisServiceInfo DynamicService = CreateWebServiceInfo(
            "DynamicService",
            "TiS.Core.Application.Dynamic.IDynamicService,TiS.Core.Application.Common",
            "TiS.Core.Application.Services.DynamicServiceCreator,TiS.Core.Application.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicService,TiS.Core.Application.Server");

        #endregion

		#region Learning

		// LearningTemplatesDBService
		public static readonly ITisServiceInfo LearningTemplatesDBService = CreateWebServiceInfo(
			"LearningTemplatesDBService",
			"TiS.Core.Application.LearningDatabase.DataAccess.ILearningTemplatesDB,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.LearningTemplatesDBCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.DataAccess.LearningTemplatesDB,TiS.Core.Application.Server");

		// LearningCollectPagesDBService
		public static readonly ITisServiceInfo LearningCollectPagesDBService = CreateWebServiceInfo(
			"LearningCollectPagesDBService",
			"TiS.Core.Application.LearningDatabase.DataAccess.ICollectPagesDB,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.CollectPagesDBCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.DataAccess.CollectPagesDB,TiS.Core.Application.Server");

		// LearningSetupDBService
		public static readonly ITisServiceInfo LearningSetupDBService = CreateWebServiceInfo(
			"LearningSetupDBService",
			"TiS.Core.Application.LearningDatabase.DataAccess.ILearningSetupDB,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.LearningSetupDBCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.DataAccess.LearningSetupDB,TiS.Core.Application.Server");

		// LearningFormDescriptionDBService
		public static readonly ITisServiceInfo LearningFormDescriptionDBService = CreateWebServiceInfo(
			"LearningFormDescriptionDBService",
			"TiS.Core.Application.LearningDatabase.DataAccess.IFormDescriptionDB,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.FormDescriptionDBCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.DataAccess.FormDescriptionDB,TiS.Core.Application.Server");

		// LearningFieldDescriptionDBService
		public static readonly ITisServiceInfo LearningFieldDescriptionDBService = CreateWebServiceInfo(
			"LearningFieldDescriptionDBService",
			"TiS.Core.Application.LearningDatabase.DataAccess.IFieldDescriptionDB,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.FieldDescriptionDBCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.DataAccess.FieldDescriptionDB,TiS.Core.Application.Server");

		// LearningConfiguration
		public static readonly ITisServiceInfo LearningConfiguration = CreateWebServiceInfo(
			"LearningConfiguration",
			"TiS.Core.Application.LearningDatabase.ILearningConfiguration,TiS.Core.Application.Common",
			"TiS.Core.Application.LearningDatabase.Services.LearningConfigurationCreator,TiS.Core.Application.Server",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.LearningDatabase.Services.LearningConfiguration,TiS.Core.Application.Server");

		#endregion

        //ClaimsServiceWindows
		public static readonly ITisServiceInfo ClaimsServiceWindows = CreateWebServiceInfo(
          TisServicesConst.CLAIMS_SERVICE_WINDOWS,
          "TiS.Core.Domain.Security.IClaimsService,TiS.Core.Domain.Common",
		  "TiS.Core.Application.Security.ClaimsServiceCreator,TiS.Core.Application.Server",
		  TisServicesConst.SERVER_REQUIRED,
		  "TiS.Core.Application.Security.ClaimsService,TiS.Core.Application.Server",
		  false,
		  false,
          true,
		  "ClaimsService_WS2007Fed_Windows"
		);

        //ClaimsServiceUser
        public static readonly ITisServiceInfo ClaimsServiceUser = CreateWebServiceInfo(
          TisServicesConst.CLAIMS_SERVICE_USER,
          "TiS.Core.Domain.Security.IClaimsService,TiS.Core.Domain.Common",
          "TiS.Core.Application.Security.ClaimsServiceCreator,TiS.Core.Application.Server",
          TisServicesConst.SERVER_REQUIRED,
          "TiS.Core.Application.Security.ClaimsService,TiS.Core.Application.Server",
          false,
          false,
          true,
          "ClaimsService_WS2007Fed_User"
        );

        // ResourceDisposerService
        public static readonly ITisServiceInfo ResourceDisposerService = CreateWebServiceInfo(
            "ResourceDisposerService",
            "TiS.Core.Domain.ResourceDisposer.IResourceDisposerService,TiS.Core.Domain.Server",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.ResourceDisposer.ResourceDisposerService,TiS.Core.Domain.Server",
            false,
            true,
            true);

		// MonitorDataToStorageService
		public static readonly ITisServiceInfo MonitorDataToStorageService = CreateWebServiceInfo(
			"MonitorDataToStorageService",
			"TiS.Core.Domain.Counters.IMonitorDataToStorageService,TiS.Core.Domain.Server",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.SERVER_REQUIRED,
			"TiS.Core.Application.Counters.MonitorDataToStorageService,TiS.Core.Application.Server",
			true,
			true);

        //Controller Alert Generator Service
        public static readonly ITisServiceInfo AlertGeneratorService = CreateWebServiceInfo(
          "AlertGeneratorService",
          "TiS.Core.Application.Alerts.IAlertGeneratorService,TiS.Core.Application.Common",
          "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
          TisServicesConst.SERVER_REQUIRED,
          "TiS.Core.Application.Services.AlertGeneratorService,TiS.Core.Application.Server",
          true,
          true);

		// ManagementDatabaseCreationService
        public static readonly ITisServiceInfo ManagementDatabaseCreationService = CreateWebServiceInfo(
            "ManagementDatabaseCreationService",
            "TiS.Core.Domain.Management.IManagementDatabaseCreationService,TiS.Core.Domain.Server",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Management.ManagementDatabaseCreationService,TiS.Core.Domain.Server",
            true,
            true);

        //// LicenseFeatureVerificationService //Removed so that customer can't disable service, instead hitches a ride on the ResourceDisposer
        //public static readonly ITisServiceInfo LicenseFeatureVerificationService = CreateWebServiceInfo(
        //    "LicenseFeatureVerificationService",
        //    "TiS.Core.Domain.LicenseFeatureVerification.ILicenseFeatureVerificationService,TiS.Core.Domain.Server",
        //    "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
        //    TisServicesConst.SERVER_REQUIRED,
        //    "TiS.Core.Domain.LicenseFeatureVerification.LicenseFeatureVerificationService,TiS.Core.Domain.Server",
        //    false,
        //    true,
        //    true);

        #endregion
    }
}
