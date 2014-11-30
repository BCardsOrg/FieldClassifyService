using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Transactions;
using Microsoft.Win32.SafeHandles;

namespace TiS.Core.TisCommon.Transactions
{
    [Guid("79427A2B-F895-40e0-BE79-B57DC82ED231"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKernelTransaction
    {
        int GetHandle(out IntPtr pHandle);
    }

    public static class NativeMethods
    {
        // Enumerations that capture Win32 values.
        [Flags]
        public enum FileShare
        {
            FILE_SHARE_NONE = 0x00,
            FILE_SHARE_READ = 0x01,
            FILE_SHARE_WRITE = 0x02,
            FILE_SHARE_DELETE = 0x04
        }

        public enum FileMode
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }

        public enum FileAccess
        {
            GENERIC_READ = unchecked((int)0x80000000),
            GENERIC_WRITE = 0x40000000
        }

        [Flags]
        public enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        // Win32 Error codes.
        internal const int ERROR_SUCCESS = 0;
        internal const int ERROR_FILE_NOT_FOUND = 2;
        internal const int ERROR_NO_MORE_FILES = 18;
        internal const int ERROR_RECOVERY_NOT_NEEDED = 6821;

        // Win32 APIs.

          [DllImport("KERNEL32.dll", EntryPoint = "CreateDirectoryTransacted", 
            CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CreateDirectoryTransacted(
             [In][Optional] string lpTemplateDirectory,
             [In] string lpNewDirectory,
             [In][Optional]IntPtr lpSecurityAttributes,
             [In] KtmTransactionHandle hTransaction);

          [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
          internal static extern bool CreateDirectory(
               [In][Optional] string lpTemplateDirectory,
               [In] string lpNewDirectory,
               [In][Optional]IntPtr lpSecurityAttributes);

        [DllImport("KERNEL32.dll", EntryPoint = "RemoveDirectoryTransacted", 
            CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RemoveDirectoryTransacted(
             [In][Optional] string lpDirectory,
             [In] KtmTransactionHandle hTransaction);

        [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RemoveDirectory(
             [In][Optional] string lpDirectory);

        [DllImport("KERNEL32.dll", EntryPoint = "CreateFileTransacted", 
            CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFileHandle CreateFileTransacted(
            [In] string lpFileName,
            [In] NativeMethods.FileAccess dwDesiredAccess,
            [In] NativeMethods.FileShare dwShareMode,
            [In] IntPtr lpSecurityAttributes,
            [In] NativeMethods.FileMode dwCreationDisposition,
            [In] int dwFlagsAndAttributes,
            [In] IntPtr hTemplateFile,
            [In] KtmTransactionHandle hTransaction,
            [In] IntPtr pusMiniVersion,
            [In] IntPtr pExtendedParameter);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            [In] string lpFileName,
            [In] NativeMethods.FileAccess dwDesiredAccess,
            [In] NativeMethods.FileShare dwShareMode,
            [In] IntPtr lpSecurityAttributes,
            [In] NativeMethods.FileMode dwCreationDisposition,
            [In] int dwFlagsAndAttributes,
            [In] IntPtr hTemplateFile
              );

        [DllImport("KERNEL32.dll", EntryPoint = "DeleteFileTransacted",
          CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteFileTransacted(
            [In] string lpFileName,
            [In] KtmTransactionHandle hTransaction);


        [DllImport("KERNEL32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteFile(
            [In] string lpFileName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", EntryPoint = "MoveFileTransacted",
            CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool MoveFileTransacted(
            [In] string lpExistingFileName, 
            [In] string lpNewFileName, 
            [In] IntPtr lpProgressRoutine, 
            [In] IntPtr lpData, 
            [In] MoveFileFlags dwFlags, [In] KtmTransactionHandle hTransaction);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool MoveFileEx(
            [In] string lpExistingFileName,
            [In] string lpNewFileName,
            [In] MoveFileFlags dwFlags);

        [DllImport("KERNEL32.dll", 
         CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(
            [In] IntPtr handle);
     
        internal static void HandleCOMError(int error)
        {
            throw new System.ComponentModel.Win32Exception(error);
        }
    }

}
