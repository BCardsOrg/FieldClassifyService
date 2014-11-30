using System;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;

namespace TiS.Core.TisCommon.Customizations
{
	[DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class TisInvokeParams : ITisInvokeParams
	{
        private string m_sModulePath;
        [DataMember]
        private string m_sInvokeType;
        [DataMember]
        private string m_sModuleName;
        [DataMember]
        private string m_sClassName;
        [DataMember]
        private string m_sMethodName;
        [DataMember]
        private string m_sEventString;

        protected const string TIS_VBA_MODULE_NAME = "eFLOW";

		internal const char EVENT_NAME_DELIMITER = ':';

        public const char LEGACY_INVOKE_TYPE_DOTNET = '+';
        public const char LEGACY_INVOKE_TYPE_WIN32DLL = '&';
        public const char LEGACY_INVOKE_TYPE_VBA = '@';

		public TisInvokeParams ()
		{
		}

        public TisInvokeParams(string sEventString)
        {
            EventString = sEventString;
        }

        public TisInvokeParams(
			string sInvokeType,
			string sEventString)
		{
		    m_sInvokeType = sInvokeType;

			EventString   = sEventString;
		}

		public TisInvokeParams (
			string sInvokeType,
			string sModuleName,
			string sClassName,
			string sMethodName) : base ()
		{
			m_sInvokeType = sInvokeType;
			m_sModuleName = sModuleName;
			m_sClassName  = sClassName;
			m_sMethodName = sMethodName;
		}

		#region ITisInvokeParams Members

		public string InvokeType
		{
			get
			{
				return m_sInvokeType;
			}
			set
			{
				m_sInvokeType = value;
			}
		}

		public string ModulePath
		{
			get
			{
				return m_sModulePath;
			}
		}

        public string ModuleName
        {
            get
            {
                return m_sModuleName;
            }
            set
            {
                m_sModuleName = value;
            }
        }

		public string ClassName
		{
			get
			{
				return m_sClassName;
			}
			set
			{
				m_sClassName = value;
			}
		}

		public string MethodName
		{
			get
			{
				return m_sMethodName;
			}
			set
			{
				m_sMethodName = value;
			}
		}

		public string EventString
		{
			get
			{
				if (m_sEventString == String.Empty)
				{
					m_sEventString = ComposeEventString();
				}

				return m_sEventString;
			}
			set
			{
                ParseEventString(value);
            }
		}

		#endregion

		private string ComposeEventString ()
		{
			string sEventString = String.Empty;

			if (ModuleName != String.Empty && MethodName != String.Empty)
			{
                sEventString = ModuleName + EVENT_NAME_DELIMITER;

				if (ClassName != String.Empty)
				{
					sEventString += ClassName + EVENT_NAME_DELIMITER;
				}

				sEventString += MethodName;
			}

			return sEventString;
		}

        public static string ParseLegacyEventString(
            string sLegacyEventString,
            string sInvokeType,
            out string sModuleName,
            out string sClassName,
            out string sMethodName,
            out string sModulePath)
        {
            sModuleName = String.Empty;
            sClassName = String.Empty;
            sMethodName = String.Empty;
            sModulePath = String.Empty;

            if (sLegacyEventString != null && sLegacyEventString != String.Empty)
            {
                if (sLegacyEventString[0] == LEGACY_INVOKE_TYPE_DOTNET)
                {
                    sInvokeType = TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET;
                    sLegacyEventString = sLegacyEventString.TrimStart(new char[] { LEGACY_INVOKE_TYPE_DOTNET });
                }
                else
                {
                    if (sLegacyEventString[0] == LEGACY_INVOKE_TYPE_VBA)
                    {
                        sInvokeType = TisCustomizationConsts.TIS_INVOKE_TYPE_VBA;
                        sLegacyEventString = sLegacyEventString.TrimStart(new char[] { LEGACY_INVOKE_TYPE_VBA });
                    }
                    else
                    {
                        if (sLegacyEventString[0] == LEGACY_INVOKE_TYPE_WIN32DLL)
                        {
                            sInvokeType = TisCustomizationConsts.TIS_INVOKE_TYPE_WIN32DLL;
                            sLegacyEventString = sLegacyEventString.TrimStart(new char[] { LEGACY_INVOKE_TYPE_WIN32DLL });
                        }
                    }
                }

                try
                {
                    if (sInvokeType != String.Empty)
                    {
                        string[] StringParts = sLegacyEventString.Split(new char[] { EVENT_NAME_DELIMITER });

                        if (StringParts.Length >= 3)
                        {
                            if (sInvokeType == TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET)
                            {
                                sMethodName = StringParts[StringParts.Length - 1];
                                sClassName = StringParts[StringParts.Length - 2];

                                if (StringParts.Length == 3)
                                {
                                    sModuleName = StringParts[StringParts.Length - 3];
                                }
                                else
                                {
                                    sModuleName = Path.GetFileNameWithoutExtension(StringParts[StringParts.Length - 3]);
                                    sModulePath = StringParts[StringParts.Length - 4] + EVENT_NAME_DELIMITER + Path.GetDirectoryName(StringParts[StringParts.Length - 3]);
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Exception tisExc = 
                        new TisException("Invalid invoke type [{0}]. Details : Event string [{1}], InnerException [{2}].", sInvokeType, sLegacyEventString, exc.Message);

                    Log.WriteException(tisExc);

                    throw tisExc;
                }
            }

            return sInvokeType;
        }

        private void ParseEventString(string sLegacyEventString)
		{
            m_sInvokeType = ParseLegacyEventString(
                sLegacyEventString,
                m_sInvokeType,
                out m_sModuleName,
                out m_sClassName,
                out m_sMethodName,
                out m_sModulePath);

            m_sEventString = ComposeEventString();
		}
	}

	public class TisDNInvokeParams : TisInvokeParams
	{
		public TisDNInvokeParams ()
		{
		}

		public TisDNInvokeParams (string sEventString) : base(TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET, sEventString)
		{
		}

		public TisDNInvokeParams (
			string sModuleName,
			string sClassName,
			string sMethodName) : base (TisCustomizationConsts.TIS_INVOKE_TYPE_DOTNET, sModuleName, sClassName, sMethodName)
	   {
	   }

		public TisDNInvokeParams (MethodInfo oMethodInfo) : this (
			oMethodInfo.ReflectedType.Assembly.GetName().Name,
			oMethodInfo.ReflectedType.FullName,
			oMethodInfo.Name)
		{
		}
	}

	public class TisVBAInvokeParams : TisInvokeParams
	{
		public TisVBAInvokeParams ()
		{
		}

		public TisVBAInvokeParams (string sEventString) : base(TisCustomizationConsts.TIS_INVOKE_TYPE_VBA, sEventString)
		{
		}

		public TisVBAInvokeParams (
			string sModuleName,
			string sClassName,
			string sMethodName) : base (TisCustomizationConsts.TIS_INVOKE_TYPE_VBA, sModuleName, sClassName, sMethodName)
     	{
	    }

		public TisVBAInvokeParams (
            string sClassName,
			string sMethodName) : base (TisCustomizationConsts.TIS_INVOKE_TYPE_VBA, TIS_VBA_MODULE_NAME, sClassName, sMethodName)
		{
		}
	}

	public class TisWin32DLLInvokeParams : TisInvokeParams
	{
		public TisWin32DLLInvokeParams ()
		{
		}

		public TisWin32DLLInvokeParams (string sEventString) : base(TisCustomizationConsts.TIS_INVOKE_TYPE_WIN32DLL, sEventString)
    	{
	    }

		public TisWin32DLLInvokeParams (
			string sModuleName, 
			string sMethodName) : base (TisCustomizationConsts.TIS_INVOKE_TYPE_WIN32DLL, sModuleName, String.Empty, sMethodName)
		{
		}
	}
}
