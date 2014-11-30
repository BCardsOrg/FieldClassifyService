using System;
using System.Runtime.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Storage
{
    #region StorageDoesNotExistException

    [Serializable]
    [System.Runtime.InteropServices.ComVisible(false)]
    public class StorageDoesNotExistException : TisException
    {
        public StorageDoesNotExistException()
            : base("Storage does not exist")
        {

        }
        public StorageDoesNotExistException(string message)
            : base(message)
        {

        }
        public StorageDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        public StorageDoesNotExistException(
            string path,
            string blobName)
            : base(String.Format(CultureInfo.InvariantCulture,
            "Storage [{0}] does not exist at path [{1}]",
            blobName,
            path))
        {
        }

        protected StorageDoesNotExistException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }


    }

    #endregion

}
