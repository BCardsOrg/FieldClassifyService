using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiS.Core.TisCommon.Services;

namespace TiS.Core.TisCommon.Services
{
	public partial class TisServicesSchema : TisInternalServicesSchema
	{
        public static readonly string CSM = "CSM";

        #region Local services

		#region Domain

		// BasicConfiguration
		public static readonly ITisServiceInfo BasicConfiguration = CreateServiceInfo(
			"BasicConfiguration",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.TisCommon.Configuration.BasicConfiguration,TiS.Core.TisCommon.Common");

		// TaskManager
		public static readonly ITisServiceInfo TaskManager = CreateServiceInfo(
			"TaskManager",
			"TiS.Core.Domain.Services.TaskManagerCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.DistributedTask.TaskManagerClient,TiS.Core.Domain.Client");

        // LocalProcessMonitorService
        public static readonly ITisServiceInfo LocalProcessMonitorService = CreateServiceInfo(
            "LocalProcessMonitorService",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Management.LocalProcessMonitorService,TiS.Core.Domain.Server");

        // LivelinessService
        public static readonly ITisServiceInfo LivelinessService = CreateWebServiceInfo(
            "LivelinessService",
            "TiS.Core.Domain.Management.ILivelinessService,TiS.Core.Domain.Common",
            "TiS.Core.Domain.Services.Web.LivelinessServiceCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Management.LivelinessService,TiS.Core.Domain.Server");

		// ManagementDataCache
		public static readonly ITisServiceInfo DomainManagementDataCache = CreateServiceInfo(
			"DomainManagementDataCache",
			"TiS.Core.Domain.Services.ManagementDataCacheCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Management.ManagementDataCache,TiS.Core.Domain.Client");

		// DomainManagementEvents
		public static readonly ITisServiceInfo DomainManagementEvents = CreateServiceInfo(
			"DomainManagementEvents",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Management.ManagementEvents,TiS.Core.Domain.Common");

		// DomainClientProvider
		public static readonly ITisServiceInfo DomainClientProvider = CreateServiceInfo(
			"DomainClientProvider",
			"TiS.Core.Domain.Services.DomainClientCreatorBase,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Services.DomainClientProvider,TiS.Core.Domain.Client");

		// NodeClient
		public static readonly ITisServiceInfo NodeClientServices = CreateServiceInfo(
			"NodeClientServices",
			"TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Nodes.NodeClientServices,TiS.Core.Domain.Client");

		// NodesManager
		public static readonly ITisServiceInfo NodesManager = CreateServiceInfo(
			"NodesManager",
			"TiS.Core.Domain.Services.DomainClientCreatorBase,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Nodes.NodesManager,TiS.Core.Domain.Client");

		// StationClientServices
		public static readonly ITisServiceInfo StationClientServices = CreateServiceInfo(
			"StationClientServices",
			"TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Stations.StationClientServices,TiS.Core.Domain.Client");

		// StationsManager
		public static readonly ITisServiceInfo StationsManager = CreateServiceInfo(
			"StationsManager",
			"TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Stations.StationsManager,TiS.Core.Domain.Client");

		// ApplicationClientServices
		public static readonly ITisServiceInfo ApplicationClientServices = CreateServiceInfo(
			"ApplicationClientServices",
			"TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Applications.ApplicationClientServices,TiS.Core.Domain.Client");

		// ApplicationsManager
		public static readonly ITisServiceInfo ApplicationsManager = CreateServiceInfo(
			"ApplicationManager",
            "TiS.Core.Domain.Services.DomainClientCreatorBase,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Applications.ApplicationManager,TiS.Core.Domain.Client");

        // ApplicationServerInstallersManager
        public static readonly ITisServiceInfo ApplicationServerInstallersManager = CreateServiceInfo(
            "ApplicationServerInstallersManager",
            "TiS.Core.Domain.Services.Web.ApplicationInstallersManagerCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Applications.ApplicationInstallersManager,TiS.Core.Domain.Common");

        // ApplicationClientInstallersManager
        public static readonly ITisServiceInfo ApplicationClientInstallersManager = CreateServiceInfo(
            "ApplicationClientInstallersManager",
            "TiS.Core.Domain.Services.ApplicationInstallersManagerCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Applications.ApplicationInstallersManager,TiS.Core.Domain.Common");

		// DomainClientServices
		public static readonly ITisServiceInfo DomainClientServices = CreateServiceInfo(
			"DomainClientServices",
			"TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Domain.DomainClientServices,TiS.Core.Domain.Client");

		// StandardArchiveReader
		public static readonly ITisServiceInfo StandardArchiveReader = CreateServiceInfo(
			"StandardArchiveReader",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Applications.Archive.StandardArchiveReader,TiS.Core.Domain.Common");

		// LegacyArchiveReader
		public static readonly ITisServiceInfo LegacyArchiveReader = CreateServiceInfo(
			"LegacyArchiveReader",
			"TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.Applications.Archive.LegacyArchiveReader,TiS.Core.AppArchiveServices");

		// SecurityManager
		public static readonly ITisServiceInfo SecurityManager = CreateServiceInfo(
			"SecurityManager",
            "TiS.Core.Application.Services.SecurityCreator,TiS.Core.Application.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.TisCommon.Security.TisSecurityMngr,TiS.Core.TisCommon.Common");

		// SecurityManagerServer
		public static readonly ITisServiceInfo SecurityManagerServer = CreateServiceInfo(
			"SecurityManagerServer",
			"TiS.Core.Domain.Services.Web.SecurityCreator,TiS.Core.Domain.Server",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.TisCommon.Security.TisSecurityMngr,TiS.Core.TisCommon.Common");

		// SetupObjectStorage
		public static readonly ITisServiceInfo SetupStorageBinaryObjects = CreateServiceInfo(
			"SetupStorageBinaryObjects",
			"TiS.Core.Domain.Services.SetupStorageBinaryObjectsCreator,TiS.Core.Domain.Common",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.TisCommon.Storage.ObjectStorage.ObjectStorage,TiS.Core.TisCommon.Common");

		// ProcessManager
		public static readonly ITisServiceInfo ProcessManager = CreateServiceInfo(
			"ProcessManager",
			"TiS.Core.Domain.Services.ProcessManagerCreator,TiS.Core.Domain.Client",
			TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.TisCommon.Process.ProcessManager,TiS.Core.TisCommon.Common");

        // LicenseClient
        public static readonly ITisServiceInfo LicenseManagerClient = CreateServiceInfo(
            "LicenseManagerClient",
            "TiS.Core.Application.Services.LicenseManagerClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
			"TiS.Core.Domain.License.LicenseManagerClient,TiS.Core.Domain.Client");

		// LicenseManager
        public static readonly ITisServiceInfo LicenseManager = CreateServiceInfo(
            "LicenseManager",
            "TiS.Core.Domain.Services.LicenseManagerCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.License.LicenseManager,TiS.Core.Domain.Client");

        // PersistentDataCache
        public static readonly ITisServiceInfo PersistentDataCache = CreateServiceInfo(
            "PersistentDataCache",
            "TiS.Core.Domain.Services.Web.PersistentDataCacheCreator,TiS.Core.Domain.Server",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Cache.Persistent.PersistentDataCache,TiS.Core.Domain.Server");

        // CountersDisposer
        public static readonly ITisServiceInfo CountersDisposer = CreateServiceInfo(
            "CountersDisposer",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.SERVER_REQUIRED,
            "TiS.Core.Domain.Counters.CountersDisposer,TiS.Core.Domain.Server");

        #region Validation

        // ValidationPolicy
        public static readonly ITisServiceInfo ValidationPolicy = CreateServiceInfo(
            "ValidationPolicy",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.TisValidationPolicy,TiS.Core.TisCommon.Common");

        // ValidationManager
        public static readonly ITisServiceInfo ValidationManager = CreateServiceInfo(
            "ValidationManager",
            "TiS.Core.Domain.Services.ValidationManagerCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.TisValidationManager,TiS.Core.TisCommon.Common");

        // InternalValidationManager
        public static readonly ITisServiceInfo InternalValidationManager = CreateServiceInfo(
            "InternalValidationManager",
            "TiS.Core.Domain.Services.InternalValidationManagerCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.InternalValidationManager,TiS.Core.TisCommon.Common");

        // ValidationContextManager
        public static readonly ITisServiceInfo ValidationContextManager = CreateServiceInfo(
            "ValidationContextManager",
            "TiS.Core.Domain.Services.ValidationContextManagerCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.TisValidationContextManager,TiS.Core.TisCommon.Common");

        // ValidatorManager
        public static readonly ITisServiceInfo ValidatorManager = CreateServiceInfo(
            "ValidatorManager",
            "TiS.Core.Domain.Services.ValidatorManagerCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.TisValidatorManager,TiS.Core.TisCommon.Common");

        // Validator
        public static readonly ITisServiceInfo Validator = CreateServiceInfo(
            "Validator",
            "TiS.Core.Domain.Services.ValidatorCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Validation.TisValidator,TiS.Core.TisCommon.Common");

        #endregion

        #region Events

        // EventsManager
        public static readonly ITisServiceInfo EventsManager = CreateServiceInfo(
            "EventsManager",
            "TiS.Core.Application.Services.EventsCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Customizations.TisEventsManager,TiS.Core.TisCommon.Common");

        // EventsManagerServer
        public static readonly ITisServiceInfo EventsManagerServer = CreateServiceInfo(
            "EventsManagerServer",
            "TiS.Core.Domain.Services.Web.EventsCreator,TiS.Core.Domain.Server",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Customizations.TisEventsManager,TiS.Core.TisCommon.Common");

        // EventsProviderManager
        public static readonly ITisServiceInfo EventsProviderManager = CreateServiceInfo(
            "EventsProviderManager",
            "TiS.Core.Domain.Services.EventsProviderManagerCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Customizations.TisEventsProviderManager,TiS.Core.TisCommon.Common");

        // EventsFirerManager
        public static readonly ITisServiceInfo EventsFirerManager = CreateServiceInfo(
            "EventsFirerManager",
            "TiS.Core.Domain.Services.EventsFirerManagerCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Customizations.TisEventsFirerManager,TiS.Core.TisCommon.Common");

        // DOTNETInvokeType
        public static readonly ITisServiceInfo EventsDOTNETInvokeType = CreateServiceInfo(
            "EventsDOTNETInvokeType",
            "TiS.Core.Domain.Services.EventsDOTNETInvokeTypeCreator,TiS.Core.Domain.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Customizations.TisDNInvokeType,TiS.Core.TisCommon.Common");

        #endregion

        #region Preconfigured

        // PreconfiguredStationsGroup
        public static readonly ITisServiceInfo PreconfiguredStationsGroup = CreateServiceInfo(
            "PreconfiguredStationsGroup",
            "TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Stations.Preconfigured.PreconfiguredStationsGroup,TiS.Core.Domain.Client");

        // PreconfiguredStation
        public static readonly ITisServiceInfo PreconfiguredStation = CreateServiceInfo(
            "PreconfiguredStation",
            "TiS.Core.Domain.Services.DomainClientCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Stations.Preconfigured.PreconfiguredStation,TiS.Core.Domain.Client");

        #endregion

        #endregion

        #region Application

        // DataEntryLayoutsConfigurationEditor
        public static readonly ITisServiceInfo DataEntryLayoutsConfigurationEditor = CreateServiceInfo(
            "DataEntryLayoutsConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Applications.LayoutDesigner.PlugIn.LayoutDesignerService,TiS.LayoutDesigner.PlugIn.Common.CsmWrapper");

        public static readonly ITisServiceInfo SetupConfigurationEditorDriver = CreateServiceInfo(
            "SetupConfigurationEditorDriver",
            "TiS.Core.Application.Services.TisSetupConfigurationEditorDriverCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.TisSetupConfigurationEditorDriver,TiS.Core.Application.Client");

        // FieldContentsConvertorConfigurationEditor
        public static readonly ITisServiceInfo FieldContentsConvertorConfigurationEditor = CreateServiceInfo(
            "FieldContentsConvertorConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.Converter.ConvertorSetupConfiguration,TiS.Core.Application.Client");

        // SetupStorageClient
        public static readonly ITisServiceInfo SetupStorageClient = CreateServiceInfo(
            "SetupStorageClient",
            "TiS.Core.Domain.Services.SetupStorageClientCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Storage.SetupStorageClient,TiS.Core.Domain.Client");

        // SetupStorageClientCache
        public static readonly ITisServiceInfo SetupStorageClientCache = CreateServiceInfo(
            "SetupStorageClientCache",
            "TiS.Core.Application.Services.SetupStorageClientCacheCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.SetupStorageClientCache,TiS.Core.Application.Client");

        // DynamicStorageClient
        public static readonly ITisServiceInfo DynamicStorageClient = CreateServiceInfo(
            "DynamicStorageClient",
            "TiS.Core.Domain.Services.DynamicStorageClientCreator,TiS.Core.Domain.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Storage.DynamicStorageClient,TiS.Core.Domain.Client");

        // ClientUserAccessData
        public static readonly ITisServiceInfo ClientUserAccessData = CreateServiceInfo(
            "ClientUserAccessData",
            "TiS.Core.Application.Services.ClientUserAccessDataCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Security.ClientUserAccessData,TiS.Core.Application.Client");

        // ClientStationPermissionValidator
        public static readonly ITisServiceInfo ClientStationPermissionValidator = CreateServiceInfo(
            "ClientStationPermissionValidator",
            "TiS.Core.Application.Services.StationPermissionValidatorCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Security.StationPermissionValidator,TiS.Core.Application.Client");

        // ServerStationPermissionValidator
        public static readonly ITisServiceInfo ServerStationPermissionValidator = CreateServiceInfo(
            "ServerStationPermissionValidator",
            "TiS.Core.Application.Services.StationPermissionValidatorCreator,TiS.Core.Application.Server",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Security.StationPermissionValidator,TiS.Core.Application.Server");

        // ApplicationServicesProvider
        public static readonly ITisServiceInfo ApplicationServicesProvider = CreateServiceInfo(
            "ApplicationServicesProvider",
            "TiS.Core.Application.Services.ApplicationServicesProviderCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Services.TisApplicationServicesProvider,TiS.Core.TisCommon.Common");

        // LegacyLiveliness
        public static readonly ITisServiceInfo LegacyLiveliness = CreateServiceInfo(
            "LegacyLiveliness",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Liveliness.TisLivelinessTestClient,TiS.Core.TisCommon.Client");

        // SetupClient
        public static readonly ITisServiceInfo SetupClient = CreateServiceInfo(
            "SetupClient",
            "TiS.Core.Application.Services.SetupClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.SetupClient,TiS.Core.Application.Client");

        // SetupFlowsetCache
        public static readonly ITisServiceInfo SetupFlowsetCache = CreateServiceInfo(
            "SetupFlowsetCache",
            "TiS.Core.Application.Services.SetupFlowsetCacheCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.SetupFlowsetCache,TiS.Core.Application.Common");

        // EntityReflection
        public static readonly ITisServiceInfo EntityReflection = CreateServiceInfo(
            "EntityReflection",
            "TiS.Core.Application.Services.EntityReflectionCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Common.Entity.TisEntityReflection,TiS.Core.Application.Common");

        // SetupAttachmentsSynchronizer
        public static readonly ITisServiceInfo SetupAttachmentsSynchronizer = CreateServiceInfo(
            "SetupAttachmentsSynchronizer",
            "TiS.Core.Application.Services.SetupAttachmentsSynchronizerCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Attachment.Synchronizer.SetupAttachmentsSynchronizer,TiS.Core.Application.Common");

        //// SetupAttachmentsSynchronizer
        //public static readonly ITisServiceInfo SetupAttachmentsSynchronizer = CreateServiceInfo(
        //    "SetupAttachmentsSynchronizer",
        //    "TiS.Core.Application.Services.SetupAttachmentsSynchronizerCreator,TiS.Core.Application.Client",
        //    TisServicesConst.NO_ROLES_REQUIRED,
        //    "TiS.Core.TisCommon.Attachment.Synchronizer.AttachmentsSynchronizer,TiS.Core.TisCommon.Common");

        // SetupAttachmentsFileManager
        public static readonly ITisServiceInfo SetupAttachmentsFileManager = CreateServiceInfo(
            "SetupAttachmentsFileManager",
            "TiS.Core.Application.Services.SetupAttachmentsFileManagerCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Attachment.File.TisAttachedFileManager,TiS.Core.Application.Common");

        // SetupAttachmentsFileManagerServer
        public static readonly ITisServiceInfo SetupAttachmentsFileManagerServer = CreateServiceInfo(
            "SetupAttachmentsFileManagerServer",
            "TiS.Core.Application.Services.SetupAttachmentsFileManagerCreator,TiS.Core.Application.Server",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Attachment.File.TisAttachedFileManager,TiS.Core.Application.Common");

        //StationConfiguration
        public static readonly ITisServiceInfo StationConfiguration = CreateServiceInfo(
          "StationConfiguration",
          "TiS.Core.Application.Services.StationConfigurationCreator,TiS.Core.Application.Common",
          TisServicesConst.NO_ROLES_REQUIRED,
          "TiS.Core.Aplication.Configuration.StationConfiguration,TiS.Core.Application.Common");

        //ApplicationConfiguration
        public static readonly ITisServiceInfo ApplicationConfiguration = CreateServiceInfo(
          "ApplicationConfiguration",
          "TiS.Core.Domain.Services.ApplicationConfigurationCreator,TiS.Core.Domain.Common",
          TisServicesConst.NO_ROLES_REQUIRED,
          "TiS.Core.Domain.Configuration.ApplicationConfiguration,TiS.Core.Domain.Common");

        // LocalPathLocator
        public static readonly ITisServiceInfo LocalPathLocator = CreateServiceInfo(
            "LocalPathLocator",
            "TiS.Core.Application.Services.LocalPathLocatorCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.FilePath.LocalPathLocator,TiS.Core.TisCommon.Common");

        // SetupLocalPathProvider
        public static readonly ITisServiceInfo SetupLocalPathProvider = CreateServiceInfo(
            "SetupLocalPathProvider",
            "TiS.Core.Application.Services.SetupLocalPathProviderCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Setup.SetupLocalPathProvider,TiS.Core.Application.Common");

        // LocalPathProvider
        public static readonly ITisServiceInfo LocalPathProvider = CreateServiceInfo(
            "LocalPathProvider",
            "TiS.Core.Application.Services.LocalPathProviderCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.FilePath.LocalPathProvider,TiS.Core.TisCommon.Common");

        // EventControl
        public static readonly ITisServiceInfo EventControl = CreateServiceInfo(
            "EventControl",
            "TiS.Core.Application.Services.EventControlCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.TisEventControl,TiS.Core.Application.Client");

        // FieldRuleEvaluator
        public static readonly ITisServiceInfo FieldRuleEvaluator = CreateServiceInfo(
            "FieldRuleEvaluator",
            "TiS.Core.Application.Services.FieldRuleEvaluatorCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.FieldRuleEvaluator,TiS.Core.Application.Client");

        // CollectionDataSplitter
        public static readonly ITisServiceInfo CollectionDataSplitter = CreateServiceInfo(
            "CollectionDataSplitter",
            "TiS.Core.Application.Services.CollectionDataSplitterCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.SplitMerge.CollectionSplitMergeHandler,TiS.Core.Application.Client");

        // CollectionDataMerger
        public static readonly ITisServiceInfo CollectionDataMerger = CreateServiceInfo(
            "CollectionDataMerger",
            "TiS.Core.Application.Services.CollectionDataMergerCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.SplitMerge.CollectionSplitMergeHandler,TiS.Core.Application.Client");

        // UnitStorageProvider
        public static readonly ITisServiceInfo UnitStorageProvider = CreateServiceInfo(
            "UnitStorageProvider",
            "TiS.Core.Application.Services.UnitStorageProviderCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.Storage.DefaultUnitStorageProvider,TiS.Core.Application.Common");

        // WFDirectReader
        public static readonly ITisServiceInfo WFDirectReader = CreateServiceInfo(
            "WFDirectReader",
            "TiS.Core.Application.Services.WFDirectReaderCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.WFDirectReader,TiS.Core.Application.Client");

        // WorkflowClient
        public static readonly ITisServiceInfo WorkflowClient = CreateServiceInfo(
            "WorkflowClient",
            "TiS.Core.Application.Services.WorkflowClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowClient,TiS.Core.Application.Client");

        // MetaTagsExtractor
        public static readonly ITisServiceInfo MetaTagsExtractor = CreateServiceInfo(
            "MetaTagsExtractor",
            "TiS.Core.Application.Services.MetaTagsExtractorCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DefaultMetaTagsExtractor,TiS.Core.Application.Common");

        // UnitsSerializer
        public static readonly ITisServiceInfo UnitsSerializer = CreateServiceInfo(
            "UnitsSerializer",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.WFUnitsSerializer,TiS.Core.Application.Common");

        // CollectionNameCounterClient
        public static readonly ITisServiceInfo CollectionNameCounterClient = CreateServiceInfo(
            "CollectionNameCounterClient",
            "TiS.Core.Application.Services.CollectionNameCounterClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.Counter.CounterClient,TiS.Core.Application.Client");

        // CollectionNameProvider
        public static readonly ITisServiceInfo CollectionNameProvider = CreateServiceInfo(
            "CollectionNameProvider",
            "TiS.Core.Application.Services.CollectionNameProviderCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.Collection.CollectionNameProvider,TiS.Core.Application.Client");

        // GroupIdCounterClient
        public static readonly ITisServiceInfo GroupIdCounterClient = CreateServiceInfo(
            "GroupIdCounterClient",
            "TiS.Core.Application.Services.GroupIdCounterClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.Counter.CounterClient,TiS.Core.Application.Client");

        // DynamicCountersProvider
        public static readonly ITisServiceInfo DynamicCountersProvider = CreateServiceInfo(
            "DynamicCountersProvider",
            "TiS.Core.Application.Services.DynamicCountersProviderCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicCountersProvider,TiS.Core.Application.Client");

        // DynamicCounters
        public static readonly ITisServiceInfo DynamicCounters = CreateServiceInfo(
            "DynamicCounters",
            "TiS.Core.Application.Services.DynamicCountersCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicCounters,TiS.Core.Application.Client");

        // DynamicControl
        public static readonly ITisServiceInfo DynamicControl = CreateServiceInfo(
            "DynamicControl",
            "TiS.Core.Application.Services.DynamicClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicClient,TiS.Core.Application.Client");

        // DynamicGlobal
        public static readonly ITisServiceInfo DynamicGlobal = CreateServiceInfo(
            "DynamicGlobal",
            "TiS.Core.Application.Services.DynamicGlobalCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.TisDynamicGlobal,TiS.Core.Application.Client");

        // DynamicQuery
        public static readonly ITisServiceInfo DynamicQuery = CreateServiceInfo(
            "DynamicQuery",
            "TiS.Core.Application.Services.DynamicQueryCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.TisDynamicQuery,TiS.Core.Application.Client");

        // DynamicManage
        public static readonly ITisServiceInfo DynamicManage = CreateServiceInfo(
            "DynamicManage",
            "TiS.Core.Application.Services.DynamicManageCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicManage, TiS.Core.Application.Client");

        // DynamicDirectReader
        public static readonly ITisServiceInfo DynamicDirectReader = CreateServiceInfo(
            "DynamicDirectReader",
            "TiS.Core.Application.Services.DynamicDirectReaderCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.DynamicDirectReader, TiS.Core.Application.Client");

        // DynamicPiorityClient
        public static readonly ITisServiceInfo DynamicPiorityClient = CreateServiceInfo(
            "DynamicPriorityClient",
            "TiS.Core.Application.Services.DynamicPriorityClientCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.DynamicPriorityClient,TiS.Core.Application.Client");

        // DynamicImportExport
        public static readonly ITisServiceInfo DynamicImportExport = CreateServiceInfo(
            "DynamicImportExport",
            "TiS.Core.Application.Services.DynamicImportExportCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.ExportImport.DynamicImportExport, TiS.Core.Application.Client");

        // StationStatistics
        public static readonly ITisServiceInfo StationStatistics = CreateServiceInfo(
            "StationStatistics",
            "TiS.Core.Application.Services.StationStatisticsCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Statistics.StationStatistics,TiS.Core.Application.Client");

        // Statistics
        public static readonly ITisServiceInfo Statistics = CreateServiceInfo(
            "Statistics",
            "TiS.Core.Application.Services.StatisticsCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Statistics.TisStatistics,TiS.Core.Application.Client");

        // StatisticsSetup
        public static readonly ITisServiceInfo StatisticsSetup = CreateServiceInfo(
            "StatisticsSetup",
            "TiS.Core.Application.Services.StatisticsSetupCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Statistics.Configuration.StatisticsSetup,TiS.Core.Application.Client");

        // ApplicationStorageService
        public static readonly ITisServiceInfo ApplicationStorageService = CreateServiceInfo(
            "ApplicationStorageService",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Domain.Storage.ApplicationStorageService,TiS.Core.Domain.Server");

        // DefaultAttachmentManipulatorService
        public static readonly ITisServiceInfo DefaultAttachmentManipulatorService = CreateServiceInfo(
            "DefaultAttachmentManipulatorService",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Attachment.DefaultAttachmentManipulator,TiS.Core.TisCommon.Common");

        // DrdAttachmentManipulatorService
        public static readonly ITisServiceInfo DrdAttachmentManipulatorService = CreateServiceInfo(
            "DrdAttachmentManipulatorService",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.RecognitionServices.Helpers.FileUtils,RecognitionServices");

        // WFUnitQueueEvaluator
        public static readonly ITisServiceInfo WFUnitQueueEvaluator = CreateServiceInfo(
            "WFUnitQueueEvaluator",
            "TiS.Core.Application.Services.WFUnitQueueEvaluatorCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.RoutingRule.WFUnitQueueEvaluator,TiS.Core.Application.Client");

        // WFQueueSchema
        public static readonly ITisServiceInfo WFQueueSchema = CreateServiceInfo(
            "WFQueueSchema",
            "TiS.Core.Application.Services.WFQueueSchemaCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.WFQueueSchema,TiS.Core.Application.Common");

        // WFRoutingRuleEvaluator
        public static readonly ITisServiceInfo WFRoutingRuleEvaluator = CreateServiceInfo(
            "WFRoutingRuleEvaluator",
            "TiS.Core.Application.Services.WFRoutingRuleEvaluatorCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.TisCommon.Evaluator.RuleEvaluator,TiS.Core.TisCommon.Common");

        // CollectionStorageProvider
        public static readonly ITisServiceInfo CollectionStorageProvider = CreateServiceInfo(
            "CollectionStorageProvider",
            "TiS.Core.Application.Services.CollectionStorageProviderCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.Collection.CollectionStorageProvider,TiS.Core.Application.Common");

        // DynamicPriorityProvider
        public static readonly ITisServiceInfo DynamicPriorityProvider = CreateServiceInfo(
            "DynamicPriorityProvider",
            "TiS.Core.Application.Services.DynamicPriorityProviderCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Dynamic.Priority.DynamicPriorityProvider,TiS.Core.Application.Common");

        #region RecognitionServices

        // FullPageOCRService
        public static readonly ITisServiceInfo FullPageOCRService = CreateServiceInfo(
            "FullPageOCRService",
            "TiS.Core.Application.Services.FullPageOCRServiceCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.RecognitionServices.FullPageOCRService.FullPageOCRService,TiS.FullPageOCRService");

        // FieldEngineOCRService
        public static readonly ITisServiceInfo FieldEngineOCRService = CreateServiceInfo(
            "FieldEngineOCRService",
            "TiS.Core.Application.Services.FieldEngineOCRServiceCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.RecognitionServices.FieldEngineOCRService.FieldEngineOCRService,TiS.FieldEngineOCRService");

        #endregion

        #region Validation

        // TisValidationContext
        public static readonly ITisServiceInfo TisValidationContext = CreateServiceInfo(
            "TisValidationContext",
            "TiS.Core.Application.Services.TisValidationContextCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Validation.TisValidationContext,TiS.Core.Application.Client");

        // TisFormValidations
        public static readonly ITisServiceInfo TisFormValidations = CreateServiceInfo(
            "TisFormValidation",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Validation.TisFormValidation,TiS.Core.Application.Client");

        // TisFieldGroupValidations
        public static readonly ITisServiceInfo TisFieldGroupValidations = CreateServiceInfo(
            "TisFieldGroupValidations",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Validation.TisFieldGroupValidation,TiS.Core.Application.Client");

        // TisFieldValidations
        public static readonly ITisServiceInfo TisFieldValidations = CreateServiceInfo(
            "TisFieldValidations",
            "TiS.Core.Application.Services.TisFieldValidationCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Validation.TisFieldValidation,TiS.Core.Application.Client");

        #endregion

        #region Events

        public static readonly ITisServiceInfo ITisFieldGroupEvents = CreateServiceInfo(
            "ITisFieldGroupEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisFieldGroupEvents, TiS.Core.Application.Client"
            );

        public static readonly ITisServiceInfo ITisFieldEvents = CreateServiceInfo(
            "ITisFieldEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisFieldEvents, TiS.Core.Application.Client"
            );

        public static readonly ITisServiceInfo ITisFormRuleEvents = CreateServiceInfo(
            "ITisFormRuleEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisFormRuleEvents, TiS.Core.Application.Client"
            );

        public static readonly ITisServiceInfo ITisFieldGroupRuleEvents = CreateServiceInfo(
            "ITisFieldGroupRuleEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisFieldGroupRuleEvents, TiS.Core.Application.Client"
            );

        public static readonly ITisServiceInfo ITisFieldRuleEvents = CreateServiceInfo(
            "ITisFieldRuleEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisFieldRuleEvents, TiS.Core.Application.Client"
            );

        public static readonly ITisServiceInfo ITisLookupTableEvents = CreateServiceInfo(
            "ITisLookupTableEvents",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.Setup.TisLookupTableEvents, TiS.Core.Application.Client"
            );

        // StationDeclarationManager
        public static readonly ITisServiceInfo StationDeclarationManager = CreateServiceInfo(
            "StationDeclarationManager",
            "TiS.Core.Application.Services.StationDeclarationManagerCreator,TiS.Core.Application.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisStationDeclarationManager,TiS.Core.Application.Client");

        // ControllerStationDeclaration
        public static readonly ITisServiceInfo ControllerStationDeclaration = CreateServiceInfo(
            "ControllerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisControllerStationDeclaration,TiS.Core.Application.Client");

        // CustomStationDeclaration
        public static readonly ITisServiceInfo CustomStationDeclaration = CreateServiceInfo(
            "CustomStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisCustomStationDeclaration,TiS.Core.Application.Client");

        // FilePortalStationDeclaration
        public static readonly ITisServiceInfo FilePortalStationDeclaration = CreateServiceInfo(
            "FilePortalStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.Portals.TisFilePortalStationDeclaration,TiS.StationDeclaration.Portals");

        // ScanPortalStationDeclaration
        public static readonly ITisServiceInfo ScanPortalStationDeclaration = CreateServiceInfo(
            "ScanPortalStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.Portals.TisScanPortalStationDeclaration,TiS.StationDeclaration.Portals");

        // Completion4StationDeclaration
        public static readonly ITisServiceInfo Completion4StationDeclaration = CreateServiceInfo(
            "Completion4StationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisCompletion4StationDeclaration,TiS.Core.Application.Client");

        // ExceptionStationDeclaration
        public static readonly ITisServiceInfo ExceptionStationDeclaration = CreateServiceInfo(
            "ExceptionStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisExceptionStationDeclaration,TiS.Core.Application.Client");

        // ExportStationDeclaration
        public static readonly ITisServiceInfo ExportStationDeclaration = CreateServiceInfo(
            "ExportStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.Export.TisExportStationDeclaration,TiS.ExportPlugIn");

        // ERPExportStationDeclaration
        public static readonly ITisServiceInfo ERPExportStationDeclaration = CreateServiceInfo(
            "ERPExportStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.ERPExport.TisERPExportStationDeclaration,TiS.StationDeclaration.ERPExport");

        // FreeBuildStationDeclaration
        public static readonly ITisServiceInfo FreeBuildStationDeclaration = CreateServiceInfo(
            "FreeBuildStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisFreeBuildStationDeclaration,TiS.Core.Application.Client");

        // FreeCollectStationDeclaration
        public static readonly ITisServiceInfo FreeCollectStationDeclaration = CreateServiceInfo(
            "FreeCollectStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisFreeCollectStationDeclaration,TiS.Core.Application.Client");

        // OCRAnalyzerStationDeclaration
        public static readonly ITisServiceInfo OCRAnalyzerStationDeclaration = CreateServiceInfo(
            "OCRAnalyzerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisOCRAnalyzerStationDeclaration,TiS.Core.Application.Common");

        // DesignerStationDeclaration
        public static readonly ITisServiceInfo DesignerStationDeclaration = CreateServiceInfo(
            "DesignerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisDesignerStationDeclaration,TiS.Core.Application.Common");

        // FreedomDesignerStationDeclaration
        public static readonly ITisServiceInfo FreedomDesignerStationDeclaration = CreateServiceInfo(
            "FreedomDesignerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisFreedomDesignerStationDeclaration,TiS.Core.Application.Common");

        // FreedomAnalyzerStationDeclaration
        public static readonly ITisServiceInfo FreedomAnalyzerStationDeclaration = CreateServiceInfo(
            "FreedomAnalyzerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisFreedomAnalyzerStationDeclaration,TiS.Core.Application.Common");

        // RecognitionAnalyzerStationDeclaration
        public static readonly ITisServiceInfo RecognitionAnalyzerStationDeclaration = CreateServiceInfo(
            "RecognitionAnalyzerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisRecognitionAnalyzerStationDeclaration,TiS.Core.Application.Common");

        // SmartDesignerStationDeclaration
        public static readonly ITisServiceInfo SmartDesignerStationDeclaration = CreateServiceInfo(
            "SmartDesignerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisSmartDesignerStationDeclaration,TiS.Core.Application.Common");

        // SimpleAutoStationDeclaration
        public static readonly ITisServiceInfo SimpleAutoStationDeclaration = CreateServiceInfo(
            "SimpleAutoStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisSimpleAutoStationDeclaration,TiS.Core.Application.Client");

        // TileStationDeclaration
        public static readonly ITisServiceInfo TileStationDeclaration = CreateServiceInfo(
            "TileStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Events.StationDeclarations.TisTileStationDeclaration,TiS.Core.Application.Client");

        // CollectionOrganizerStationDeclaration
        public static readonly ITisServiceInfo CollectionOrganizerStationDeclaration = CreateServiceInfo(
            "CollectionOrganizerStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.CollectionOrganizer.TisCollectionOrganizerStationDeclaration,TiS.StationDeclaration.CollectionOrganizer");

        // RecognitionStationDeclaration
        public static readonly ITisServiceInfo RecognitionStationDeclaration = CreateServiceInfo(
            "RecognitionStationDeclaration",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.StationDeclaration.Recognition.TisRecognitionStationDeclaration,TiS.StationDeclaration.Recognition");


        #region Lookups

        // TisSqlLookupSetupEditor
        public static readonly ITisServiceInfo TisSqlLookupSetupEditor = CreateServiceInfo(
            "TisSqlLookupSetupEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Design.DbAccess.Setup.TisSqlLookupSetupEditor,TiS.Design.DbAccessSetupEditor");

        // TisLegacyLookupSetupEditor
        public static readonly ITisServiceInfo TisLegacyLookupSetupEditor = CreateServiceInfo(
            "TisLegacyLookupSetupEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Design.DbAccess.Setup.TisLegacyLookupSetupEditor,TiS.Design.DbAccessSetupEditor");

        // TisFuzzyLookupSetupEditor
        public static readonly ITisServiceInfo TisFuzzyLookupSetupEditor = CreateServiceInfo(
            "TisFuzzyLookupSetupEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Design.DbAccess.Setup.TisFuzzyLookupSetupEditor,TiS.Design.DbAccessSetupEditor");

        // TisExorbyteLookupSetupEditor
        public static readonly ITisServiceInfo TisExorbyteLookupSetupEditor = CreateServiceInfo(
            "TisExorbyteLookupSetupEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Design.DbAccess.Setup.TisExorbyteLookupSetupEditor,TiS.Design.DbAccessSetupEditor");

        // LookupSetupEditorsManager
        public static readonly ITisServiceInfo LookupSetupEditorsManager = CreateServiceInfo(
            "LookupSetupEditorsManager",
            "TiS.Design.DbAccess.Setup.TisLookupSetupEditorsManagerCreator,TiS.Design.DbAccessSetupEditor",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Design.DbAccess.Setup.TisLookupSetupEditorsManager,TiS.Design.DbAccessSetupEditor");

        // TisLookupManager
        public static readonly ITisServiceInfo TisLookupManager = CreateServiceInfo(
            "TisLookupManager",
            "TiS.Core.Application.Services.TisLookupManagerCreator,TiS.Core.Application.Client",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.DbAccess.TisLookupManager,TiS.Core.Application.Client");

        #endregion

        // WorkflowInstaller
        public static readonly ITisServiceInfo WorkflowInstaller = CreateServiceInfo(
            "WorkflowInstaller",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Workflow.WorkflowInstaller,TiS.Core.Application.Server");

        // ApplicationServerInstaller
        public static readonly ITisServiceInfo ApplicationServerInstaller = CreateServiceInfo(
            "ApplicationServerInstaller",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Core.Application.Application.ApplicationServerInstaller,TiS.Core.Application.Server");

        #endregion

        #region Stations configuration

        public static readonly ITisServiceInfo CompletionConfigurationEditor = CreateServiceInfo(
            "DataEntry_CompletionConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "DataEntryConfiguration.DesignerAdapter.DataEntryConfigurationAdapter,DataEntryConfiguration.DesignerAdapter");

        public static readonly ITisServiceInfo ExceptionsConfigurationEditor = CreateServiceInfo(
            "DataEntry_ExceptionConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "DataEntryConfiguration.DesignerAdapter.DataEntryConfigurationAdapter,DataEntryConfiguration.DesignerAdapter");

        public static readonly ITisServiceInfo CollectionOrganizerConfigurationEditor = CreateServiceInfo(
            "efCollectionOrganizer_ConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.CollectionOrganizer.Configuration.ConfigurationEditorRunner,TiS.CollectionOrganizer.Configuration");

        public static readonly ITisServiceInfo FilePortalConfigurationEditor = CreateServiceInfo(
            "Input_FileConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Applications.Input.Configuration.InputConfigurationEditor,Input.Configuration");

        public static readonly ITisServiceInfo ScanPortalConfigurationEditor = CreateServiceInfo(
            "Input_ScanConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Applications.Input.Configuration.InputConfigurationEditor,Input.Configuration");

        public static readonly ITisServiceInfo ExportConfigurationEditor = CreateServiceInfo(
            "efProcessShell_ExportConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Applications.ExportPlugIn.Configuration.ConfigurationEditorRunner,TiS.ExportPlugIn");

        public static readonly ITisServiceInfo TileConfigurationEditor = CreateServiceInfo(
            "efTile_ConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Applications.Tile.eFlow.TisTileConfigurationEditor,efTile");

        public static readonly ITisServiceInfo RecognitionConfigurationEditor = CreateServiceInfo(
            "efProcessShell_RecognitionConfigurationEditor",
            "TiS.Core.TisCommon.Services.TisUniversalServiceCreator,TiS.Core.TisCommon.Common",
            TisServicesConst.NO_ROLES_REQUIRED,
            "TiS.Recognition.Configuration.TisRecognitionConfigurationEditor,TiS.Recognition.Configuration");

        #endregion

        #endregion

        #endregion

        private readonly ITisServiceInfo[] Services = 
        {
            #region Local services

            #region Domain

            BasicConfiguration,
            TaskManager,
            DomainManagementDataCache,
            DomainManagementEvents,
            DomainClientServices,
            DomainClientProvider,
            NodesManager,
            NodeClientServices,
            StationsManager,
            StationClientServices,
            ApplicationsManager,
            ApplicationClientServices,
            SecurityManager,
            SecurityManagerServer,
            SetupStorageBinaryObjects,
            StandardArchiveReader,
            ProcessManager,
            LicenseManagerClient,
            LicenseManager,
            PersistentDataCache,
            CountersDisposer,

            #region Preconfigured

            PreconfiguredStationsGroup,
            PreconfiguredStation,

            #endregion

            #region Configuration         
            ApplicationConfiguration,
            StationConfiguration,

            #endregion

            #endregion

            #region Validation

            ValidationManager,
            Validator,
            ValidationPolicy,
            ValidationManager,
            InternalValidationManager,
            ValidationContextManager,
            ValidatorManager,
            Validator,

            #endregion

            #region Events

            EventsManager,
            EventsManagerServer,
            EventsProviderManager,
            EventsFirerManager,
            EventsDOTNETInvokeType,

            #endregion

            #endregion

            #region Application

            #region Common

            ClientUserAccessData,
            ClientStationPermissionValidator,
            ServerStationPermissionValidator,
            ApplicationServicesProvider,
            LocalPathLocator,
            LegacyLiveliness,
            EntityReflection,
            LocalPathProvider,
            FieldRuleEvaluator,
            ApplicationClientInstallersManager,
            ApplicationStorageService,
            DefaultAttachmentManipulatorService,
			DrdAttachmentManipulatorService,

            #endregion

            #region Setup

            DataEntryLayoutsConfigurationEditor,
            FieldContentsConvertorConfigurationEditor,
            SetupConfigurationEditorDriver,
            SetupLocalPathProvider,
            SetupStorageClient,
            SetupStorageClientCache,
            SetupClient,
            SetupFlowsetCache,
            SetupAttachmentsSynchronizer,
            SetupAttachmentsFileManager,
            SetupAttachmentsFileManagerServer,

            #endregion

            #region Dynamic

            DynamicStorageClient,
            CollectionDataSplitter,
            CollectionDataMerger,
            MetaTagsExtractor,
            CollectionNameCounterClient,
            CollectionNameProvider,
            GroupIdCounterClient,
            DynamicCountersProvider,
            DynamicCounters,
            DynamicControl,
            DynamicGlobal,
            DynamicQuery,
            DynamicManage,
            DynamicDirectReader,
            DynamicPiorityClient,
            DynamicImportExport,
            CollectionStorageProvider,
            DynamicPriorityProvider,

            #endregion

            #region Statistics

            StationStatistics,
            Statistics,
            StatisticsSetup,

            #endregion

            #region RecognitionServices

            FullPageOCRService,
			FieldEngineOCRService,

            #endregion

            #region Validation

            TisValidationContext,
            TisFormValidations,
            TisFieldGroupValidations,
            TisFieldValidations,

            #endregion

            #region Events

            ITisFieldGroupEvents,
            ITisFieldEvents,
            ITisFormRuleEvents,
            ITisFieldGroupRuleEvents,
            ITisFieldRuleEvents,
            ITisLookupTableEvents,
            EventControl,
            StationDeclarationManager,
            ControllerStationDeclaration,
            CustomStationDeclaration,
            FilePortalStationDeclaration,
            ScanPortalStationDeclaration,
            Completion4StationDeclaration,
            ExceptionStationDeclaration,
            ExportStationDeclaration,
			ERPExportStationDeclaration,
            FreeBuildStationDeclaration,
            FreeCollectStationDeclaration,
            OCRAnalyzerStationDeclaration,
            DesignerStationDeclaration,
            FreedomDesignerStationDeclaration,
            FreedomAnalyzerStationDeclaration,
            RecognitionAnalyzerStationDeclaration,
            SmartDesignerStationDeclaration,
            RecognitionStationDeclaration,
            SimpleAutoStationDeclaration,
            TileStationDeclaration,
            CollectionOrganizerStationDeclaration,

            #endregion

            #region Lookups

            TisLookupManager,
            LookupSetupEditorsManager,
			TisLegacyLookupSetupEditor,
			TisSqlLookupSetupEditor,
			TisFuzzyLookupSetupEditor,
			TisExorbyteLookupSetupEditor,

            #endregion

            #region Station configuration

            CompletionConfigurationEditor,
            ExceptionsConfigurationEditor,
			CollectionOrganizerConfigurationEditor,
			FilePortalConfigurationEditor,
			ScanPortalConfigurationEditor,
			ExportConfigurationEditor,
			TileConfigurationEditor,
            RecognitionConfigurationEditor,

            #endregion

            #region Workflow

            WorkflowClient,
            WFUnitQueueEvaluator,
            WFQueueSchema,
            WFRoutingRuleEvaluator,

            #endregion

            #endregion

            #region Web services

            BasicConfigurationServer,
            BasicConfigurationServerService,
            ApplicationPathProvider,
            ApplicationServerInstallersManager,
            DomainManagementServer,
            LivelinessService,
            DbAccessServer,
            TaskManagerServer,
            SetupStorageServer,
            LicenseServer,
            DynamicStorageServer,
            SecurityPolicyProviderServer,
            SetupFlowsetService,
            CollectionNameCounterService,
            GroupIdCounterService,
            DynamicPriorityService,
            DynamicService,
            ApplicationConfigurationService,
            StationConfigurationService,
			ClaimsServiceWindows,
			ClaimsServiceUser,
			MonitorCountersService,

            LocalProcessMonitorService,
            ResourceDisposerService,
            //LicenseFeatureVerificationService,
			MonitorDataToStorageService,

            WorkflowInstaller,
            ApplicationServerInstaller,

            #region Statistics

            StatisticsConfigurationService,
            StatisticsService,

            #endregion

            #region Workflow

            UnitStorageProvider,
            WFDirectReader,
            UnitsSerializer,
            WorkflowCounterService,
            WorkflowOperationsService,
            WorkflowUnitsQueryService,
            WorkflowSessionService,
            WorkflowSchemaService,
            WorkflowUnitsControlService,

            ControllerService,
            AlertGeneratorService,
            InvoiceReaderService,

            #endregion

			#region Learning
			
			LearningTemplatesDBService,
			LearningCollectPagesDBService,
			LearningSetupDBService,
			LearningFormDescriptionDBService,
			LearningFieldDescriptionDBService,
			LearningConfiguration,
			
			#endregion

            #endregion

        };

		#region ITisServicesSchemaCollection Members

		public override List<ITisServiceInfo> GetServices()
		{
			List<ITisServiceInfo> services = new List<ITisServiceInfo>(Services);

			return services;
		}

		#endregion
	}
}
