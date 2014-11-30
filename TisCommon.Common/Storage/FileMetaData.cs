using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

namespace TiS.Core.TisCommon.Storage
{
    [DataContract(Namespace = "http://www.topimagesystems.com/eFlow")]
    public class FileMetaData
    {
        private string m_fileName;

        [DataMember]
        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }
    }

}
