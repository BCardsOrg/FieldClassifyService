using System;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Storage
{
    public static class DataSetStorageUtil
    {
        public static bool LoadDataSet(
            IStorageService storage,
            string storageName,
            DataSet dataSet)
        {
            using (Stream stream = OpenStream(storage, storageName))
            {
                // Create reader
                using (StreamReader oReader = new StreamReader(stream))
                {
                    try
                    {
                        // Read data
                        dataSet.ReadXml(oReader);
                    }
                    catch (Exception exc)
                    {
                        Log.WriteException(exc);

                        return false;
                    }
                }
            }

            return true;
        }

        private static Stream OpenStream(IStorageService storage, string storageName )
        {
            try
            {
                byte[] Data = storage.ReadStorage(storageName);

                Stream stream = new MemoryStream(Data);

                return stream;
            }
            catch (StorageDoesNotExistException exc)
            {
                Log.WriteException(exc);

                return null;
            }
            catch (Exception exc)
            {
                Log.WriteWarning("Failed to read BLOB [{0}] directly, streamed transfer is used. Details : [{1}]", storageName, exc.Message);

                Stream stream = storage.ReadStream(storageName);

                return stream;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="storageName"></param>
        /// <param name="dataSet">Used locally only</param>
        public static void StoreDataSet(
            IStorageService storage,
            string storageName,
            DataSet dataSet)
        {
            try
            {
                storage.WriteDataSet(dataSet, storageName);
            }
            catch (Exception exc)
            {
                Log.WriteException(exc);
            }
         }
    }
}
