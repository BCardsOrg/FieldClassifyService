using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TiS.Core.TisCommon.Configuration
{
 

    public interface IBasicConfigurationProvider : IBasicConfigurationProviderHelper
    {
        BasicConfigurationData Load();

        void Save(BasicConfigurationData data);

    }


    public class BasicConfigurationProvider : BasicConfigurationProviderHelper, IBasicConfigurationProvider
    {
        private const string SECTION_NAME = "eFlowPlatformConfig";

        #region IBasicConfigurationProvider Members

        public BasicConfigurationProvider()
        {           
        }

        public BasicConfigurationData Load()
        {
            try
            {
                BasicConfigurationData data = (BasicConfigurationData)Storage.Load( SECTION_NAME);

                if (data == null)
                {
                    data = new BasicConfigurationData();
                }

                return data;
            }
            catch (Exception exc)
            {
                Log.WriteError("Failed loading configuration : {0}", exc);

                return null;
            }
        }

        public void Save(BasicConfigurationData data)
        {
            Storage.Save(data, SECTION_NAME);
        }


        #endregion
    }

}
