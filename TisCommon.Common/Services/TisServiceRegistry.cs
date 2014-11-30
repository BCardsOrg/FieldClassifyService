using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.Services
{
    #region TisServiceRegistry

    public class TisServiceRegistry : ITisServiceRegistry
	{
		private bool m_bReadOnly = true;

        protected Dictionary<string, ITisServiceInfo> m_oInstalledServices = new Dictionary<string, ITisServiceInfo>();

		protected string m_sServicesSchemaType = String.Empty;

		//
		//	Public
		//

		public TisServiceRegistry()
		{
		}

        public TisServiceRegistry(string schemaTypeName)
            : this(Type.GetType(schemaTypeName))
        {
        }

        public TisServiceRegistry(Type schemaType)
		{
			ServicesSchemaCLRType = schemaType;
		}

		#region Implementation of ITisServiceRegistry

		public virtual void InstallService(
			string   sServiceName,
			string   sServiceCreatorType, 
			string	 sServiceImplType,
            string[] NodeRoles,
            bool freeUse)
		{
			InternalInstallService(
				sServiceName,
				sServiceCreatorType,
				sServiceImplType,
				NodeRoles,
                false,
                freeUse
				); // From Schema
		}

		public virtual void UninstallService(string sServiceName)
		{
			ITisServiceInfo oServiceInfo = 
				GetInstalledServiceInfo(sServiceName);

			if(oServiceInfo == null)
			{
				throw new TisException(
					"Service [{0}] not installed, can't uninstall", 
					sServiceName);
			}

			if(oServiceInfo.FromSchema)
			{
				throw new TisException(
					"Service [{0}] comes from schema, can't uninstall", 
					sServiceName);
			}

			InternalUninstallService(sServiceName);
		}

		public ITisServiceInfo GetInstalledServiceInfo(
			string sServiceName)
		{
            ITisServiceInfo oServiceInfo;

            m_oInstalledServices.TryGetValue(sServiceName, out oServiceInfo);

			return oServiceInfo;
		}

		public bool IsServiceInstalled(string sServiceName)
		{
			return GetInstalledServiceInfo(sServiceName) != null;
		}

		public bool ReadOnly
		{
			get
			{
				return m_bReadOnly;
			}
			set
			{
				m_bReadOnly = value;
			}
		}

		public ITisServiceInfo[] InstalledServices
		{
			get
			{
				ITisServiceInfo[] Services = 
					new ITisServiceInfo[m_oInstalledServices.Count];

				m_oInstalledServices.Values.CopyTo(Services, 0);

				return Services;
			}
		}

		public int NumberOfInstalledServices 
		{ 
			get { return m_oInstalledServices.Count; }
		}

		public virtual string ServicesSchema 
		{ 
			get 
			{ 
				return m_sServicesSchemaType; 
			}
			set
			{
				InternalSetServicesSchema(value);
			}
		}

		public Type ServicesSchemaCLRType
		{
			get
			{
				return SchemaTypeFromString(ServicesSchema);
			}
			set
			{
				ServicesSchema = SchemaTypeToString(value);
			}
		}

		#endregion

        protected void InternalInstallService(
            ITisServiceInfo serviceInfo,
            bool bFromSchema)
        {
            ((TisServiceInfo)serviceInfo).SetFromSchema(bFromSchema);

            // Add it to map
            m_oInstalledServices[serviceInfo.ServiceName] = serviceInfo;
        }

		protected void InternalInstallService(
			string   sServiceName,
			string   sServiceCreatorType, 
			string	 sServiceImplType,
			string[] NodeRoles,
			bool	 bFromSchema,
            bool freeUse)
		{
            ServicesUsingMode servicesUsingMode = freeUse ? ServicesUsingMode.Free : ServicesUsingMode.Restricted;

			// Create new service info record
			TisServiceInfo oServiceInfo = new TisServiceInfo(
				sServiceName,
				sServiceCreatorType, 
				sServiceImplType,
				NodeRoles,
                servicesUsingMode);

			oServiceInfo.SetFromSchema(bFromSchema);

			// Add it to map
			m_oInstalledServices[sServiceName] = oServiceInfo;
		}

		protected void InternalUninstallService(
			string sServiceName)
		{
			m_oInstalledServices.Remove(sServiceName);
		}

		protected void InternalSetServicesSchema(string sServicesSchema)
		{
			m_sServicesSchemaType = sServicesSchema;

			LoadServicesFromSchema();
		}

        internal void Clear()
        {
            m_oInstalledServices.Clear();
        }

		private void LoadServicesFromSchema()
		{
			Type oSchemaType = ServicesSchemaCLRType;
			
			UninstallSchemaServices();

			if(oSchemaType == null)
			{
				return;
			}

            ITisServicesSchemaCollection servicesCollection = 
                Activator.CreateInstance(oSchemaType) as ITisServicesSchemaCollection;

            if (servicesCollection == null)
            {
                Log.Write(
                            Log.Severity.ERROR,
                            System.Reflection.MethodInfo.GetCurrentMethod(),
                            "Type [{0}] does not implement interface IServicesSchemaCollection Schema will not be created",
                            oSchemaType.FullName);
            }

            List<ITisServiceInfo> allServices = servicesCollection.GetServices();

            IEnumerable<ITisServiceInfo> servicesToInstall =
                from servicesInfo in allServices
                where servicesInfo != null && !IsServiceInstalled(servicesInfo.ServiceName)
                select servicesInfo;


            foreach (ITisServiceInfo serviceInfo in servicesToInstall)
			{
                InternalInstallService(serviceInfo, true);
			}
		}

		private void UninstallSchemaServices()
		{
            IEnumerable<string> oServicesToUninstall =
                from oService in m_oInstalledServices.Values
                where oService.FromSchema
                select oService.ServiceName;

			foreach(string sServiceName in oServicesToUninstall)
			{
				InternalUninstallService(sServiceName);
			}
		}

		private string SchemaTypeToString(Type oSchemaType)
		{
			return TisServicesUtil.GetFullTypeString(oSchemaType);
		}

		private Type SchemaTypeFromString(string sSchemaType)
		{
			return Type.GetType(sSchemaType);
		}

	}

    #endregion
}
