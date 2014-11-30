using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Transactions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Transactions
{

    public static class TransactedFile
    {
        #region Transacted file management 

        public static SafeFileHandle Open(
                 string path,
                 FileMode mode,
                 FileAccess access,
                 FileShare share)
        {

            using (KtmTransactionHandle ktmTx = KtmTransactionHandle.CreateKtmTransactionHandle())
            {
                // Translate the managed flags to unmanaged flags.
                NativeMethods.FileMode internalMode = TranslateFileMode(mode);

                NativeMethods.FileShare internalShare = TranslateFileShare(share);

                NativeMethods.FileAccess internalAccess = TranslateFileAccess(access);

                // Create the transacted file using P/Invoke.
                SafeFileHandle hFile = NativeMethods.CreateFileTransacted(
                    path,
                    internalAccess,
                    internalShare,
                    IntPtr.Zero,
                    internalMode,
                    0,
                    IntPtr.Zero,
                    ktmTx,
                    IntPtr.Zero,
                    IntPtr.Zero);
                {

                    // Throw an exception if an error occured.
                    if (hFile.IsInvalid)
                    {
                        hFile = NativeMethods.CreateFile(
                            path,
                            internalAccess,
                            internalShare,
                            IntPtr.Zero,
                            internalMode,
                            0,
                            IntPtr.Zero);

                        if (hFile.IsInvalid)
                        {
                            NativeMethods.HandleCOMError(Marshal.GetLastWin32Error());
                        }
                    }

                    return hFile;
                }
            }
        }

        public static void DeleteFile(string path)
        {
			// We do not need to do nothing if file not exist
			if (!File.Exists(path))
				return;

            using (KtmTransactionHandle ktmTx = KtmTransactionHandle.CreateKtmTransactionHandle())
            {
                  // Delete the transacted file using P/Invoke.
                bool result = NativeMethods.DeleteFileTransacted(path, ktmTx);

                // Throw an exception if an error occured.
                if (!result)
                {
                    result = NativeMethods.DeleteFile(path);

                    if (!result)
                    {
                        NativeMethods.HandleCOMError(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        public static void Move(string existingFileName, string newFileName )
        {
            using (KtmTransactionHandle ktmTx = KtmTransactionHandle.CreateKtmTransactionHandle())
            {
                NativeMethods.MoveFileFlags moveFlags = NativeMethods.MoveFileFlags.MOVEFILE_COPY_ALLOWED | NativeMethods.MoveFileFlags.MOVEFILE_REPLACE_EXISTING;

                bool result = NativeMethods.MoveFileTransacted(existingFileName, newFileName, IntPtr.Zero, IntPtr.Zero, moveFlags, ktmTx);

                // Throw an exception if an error occured.
                if (result == false )
                {
                    result = NativeMethods.MoveFileEx(existingFileName, newFileName, moveFlags);

                    if (result == false)
                    {
                        NativeMethods.HandleCOMError(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        public static void DeleteDir(string path)
        {
            using (KtmTransactionHandle ktmTx = KtmTransactionHandle.CreateKtmTransactionHandle())
            {
                bool result = NativeMethods.RemoveDirectoryTransacted(path, ktmTx);

                // Throw an exception if an error occured.
                if (!result)
                {
                    result = NativeMethods.RemoveDirectory(path);

                    if (!result)
                    {
                        NativeMethods.HandleCOMError(Marshal.GetLastWin32Error());
                    }
                }
            }
        }

        public static void CreateDir(string path)
        {
            List<string> createdDirs = new List<string>();

            CreateDir(path, ref createdDirs);
        }

        public static void CreateDir(
            string path, 
            ref List<string> createdDirs)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            using (KtmTransactionHandle ktmTx = KtmTransactionHandle.CreateKtmTransactionHandle())
            {
                CreateDirectory(directoryInfo, ktmTx, ref createdDirs);
            }
        }

        private static void CreateDirectory(
            DirectoryInfo dirInfo,
            KtmTransactionHandle ktmTx,
            ref List<string> createdDirs)
        {
            if (createdDirs.Exists(dir => StringUtil.CompareIgnoreCase(dirInfo.FullName, dir)))
            {
                return;
            }

            if (dirInfo.Parent != null && !dirInfo.Parent.Exists)
            {
                CreateDirectory(dirInfo.Parent, ktmTx, ref createdDirs);
            }

            CreateDirectoryTransacted(dirInfo.FullName, ktmTx);

            createdDirs.Add(dirInfo.FullName);
        }

        private static bool CreateDirectoryTransacted(
            string path, 
            KtmTransactionHandle ktmTx)
        {
            bool result =  NativeMethods.CreateDirectoryTransacted(null, path, IntPtr.Zero, ktmTx);

            if (!result)
            {
                result = NativeMethods.CreateDirectory(null, path, IntPtr.Zero);

                if (!result)
                {
                    Log.WriteDebug("Failed to create directory : [{0}] ... possibly already exists. Error : [{1}]", 
                        path, 
                        Marshal.GetLastWin32Error().ToString());
                }
            }

            return result;
        }

        #endregion

        #region Translate managed enums to unmanaged

        // Translate managed FileMode member to unmanaged file mode flag.
        private static NativeMethods.FileMode TranslateFileMode(FileMode mode)
        {
            if (mode != FileMode.Append)
            {
                return (NativeMethods.FileMode)(int)mode;
            }
            else
            {
                return (NativeMethods.FileMode)(int)FileMode.OpenOrCreate;
            }
        }

        // Translate managed FileAcess member to unmanaged file access flag.
        private static NativeMethods.FileAccess TranslateFileAccess(FileAccess access)
        {
            return access == FileAccess.Read ?
                NativeMethods.FileAccess.GENERIC_READ :
                NativeMethods.FileAccess.GENERIC_WRITE;
        }

        // FileShare members map directly to their unmanaged counterparts.
        private static NativeMethods.FileShare TranslateFileShare(FileShare share)
        {
            return (NativeMethods.FileShare)(int)share;
        }

        
        #endregion

   
    }
  
}
