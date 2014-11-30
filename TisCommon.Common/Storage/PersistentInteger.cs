using System;
using System.IO;
using System.Text;

namespace TiS.Core.TisCommon.Storage
{
	public class PersistentInteger
	{
		private string		 m_sBLOBName;
		private IStorageService m_oBLOBStorage;

		private int			 m_nValue;

		public PersistentInteger(
			IStorageService oBLOBStorage,
			string		 sBLOBName, 
			int			 nInitialValue)
		{
			m_oBLOBStorage = oBLOBStorage;
			m_sBLOBName    = sBLOBName;

			m_nValue = nInitialValue;
				
			if(BackupExists())
			{
				RevertFromBackup();
			}

			try
			{
				Load();
			}
			catch(Exception)
			{
				m_nValue = 0;

				Log.Write(
					Log.Severity.ERROR,
					System.Reflection.MethodInfo.GetCurrentMethod(),
					"Failed loading value, resetting to 0");
			}

		}

		public int Value
		{
			get 
			{
				return m_nValue;
			}
			set
			{
				// Save the value
				TransactStoreValue(value);
					
				// Set cached value
				m_nValue = value;
			}
		}

		//
		//	Private methods
		//

		private void TransactStoreValue(int nValue)
		{
			try
			{
				// Create backup
				CreateBackupIfNeeded();
					
				// Save the value
				Save(nValue);
					
				// Delete backup
				DeleteBackup();
			}
			catch(Exception exc)
			{
                Log.WriteException(exc);

				RevertFromBackupIfNeeded();

				throw;
			}
		}

		private void CreateBackupIfNeeded()
		{
			if(m_oBLOBStorage.IsStorageExist( m_sBLOBName))
			{
				m_oBLOBStorage.RenameStorage(m_sBLOBName, GetBackupBLOBName());
			}
		}

		private void RevertFromBackupIfNeeded()
		{
			if(BackupExists())
			{
				RevertFromBackup();
			}
		}

		private void RevertFromBackup()
		{
			if(m_oBLOBStorage.IsStorageExist(m_sBLOBName))
			{
				m_oBLOBStorage.DeleteStorage(new string[] { m_sBLOBName } );
			}

            m_oBLOBStorage.RenameStorage(GetBackupBLOBName(), m_sBLOBName);
		}

		private bool BackupExists()
		{
			return m_oBLOBStorage.IsStorageExist(GetBackupBLOBName());
		}

		private string GetBackupBLOBName()
		{
			return m_sBLOBName + "~";
		}

		private void DeleteBackup()
		{
			if(BackupExists())
			{
				m_oBLOBStorage.DeleteStorage(
					new string[] { GetBackupBLOBName() } );
			}
		}

        private void Load()
        {
            // Check if BLOB exists
            if (m_oBLOBStorage.IsStorageExist( m_sBLOBName))
            {
                // Open BLOB
                using (Stream oStream = m_oBLOBStorage.ReadStream( m_sBLOBName))
                {
                    StreamReader oReader = new StreamReader(oStream);

                    // Read first line
                    string sLine = oReader.ReadLine();

                    if (sLine == null)
                    {
                        throw new TisException("Invalid version file format");
                    }

                    try
                    {
                        // Parse to int
                        m_nValue = int.Parse(sLine);
                    }
                    catch (Exception oExc)
                    {
                        Log.Write(
                            Log.Severity.ERROR,
                            System.Reflection.MethodInfo.GetCurrentMethod(),
                            "Failed converting value [{0}] to int, {1}",
                            sLine,
                            oExc.Message);

                        //m_nValue = TryGetVersionFromFilePrefix();

                        //if (m_nValue < 0)
                        //{
                        //    throw;
                        //}
                    }
                }
            }
        }

		private void Save(int nValue)
		{
            // TODO !!!!

            //// Open BLOB
            //using(Stream oStream = m_oBLOBStorage.OpenWriteStorage("", m_sBLOBName))
            //{
            //    using (StreamWriter oWriter = new StreamWriter(oStream))
            //    {
            //        // Write value
            //        oWriter.WriteLine(nValue.ToString());

            //        // Flush & close Writer
            //        oWriter.Flush();
            //        oWriter.Close();
            //    }
            //}
		}

	}
}
