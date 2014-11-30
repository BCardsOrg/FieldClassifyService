using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Customizations.MethodBrowsers
{
	public class PEImageHelper
	{
		private const uint GENERIC_READ          = 0x80000000;
		private const int FILE_SHARE_READ        = 0x00000001;
		private const int OPEN_EXISTING          = 3;
		private const int FILE_ATTRIBUTE_NORMAL  = 0x00000080;
		private const int PAGE_READONLY          = 2;
		private const int FILE_MAP_READ          = 4;
		private const uint IMAGE_NT_SIGNATURE    = 0x00004550;
	    private const uint INVALID_HANDLE_VALUE  = 0xFFFFFFFF;

		internal enum PEInvokeType {Win32DLL, DNAssembly, Unknown};

        public static bool GetPEImage(
			string sPEName, 
			out uint nFileHandle, 
			out uint nFileMapping, 
			out IntPtr PEBaseAddr)
		{
			nFileMapping = 0;
			PEBaseAddr = IntPtr.Zero;

			nFileHandle = CreateFile (
				sPEName,
				GENERIC_READ,
				FILE_SHARE_READ,
				IntPtr.Zero,
				OPEN_EXISTING,
				FILE_ATTRIBUTE_NORMAL,
				0);

			if (nFileHandle == INVALID_HANDLE_VALUE)
			{
				return false; 
			}

			nFileMapping = CreateFileMapping (
				nFileHandle,
				IntPtr.Zero,
				PAGE_READONLY,
				0,
				0,
				String.Empty);


			if (nFileMapping == 0)
			{
				return false; 
			}

			PEBaseAddr = MapViewOfFile (
				nFileMapping,
				FILE_MAP_READ,
				0,
				0,
				0);

			return PEBaseAddr == IntPtr.Zero ? false : true;
		}

        public static void FreePEImage(
			uint nFileHandle, 
			uint nFileMapping, 
			IntPtr PEBaseAddr)
		{
			if (PEBaseAddr != IntPtr.Zero)
			{
				UnmapViewOfFile (PEBaseAddr);
			}

			if (nFileMapping > 0)
			{
				CloseHandle (nFileMapping);
			}

			if (nFileHandle > 0)
			{
				CloseHandle (nFileHandle);
			}
		}

        public static bool GetImageNTHeaders(
			IntPtr PEBaseAddr, 
			out IntPtr ImageNTHeadersAddr, 
			out _IMAGE_NT_HEADERS ImageNTHeaders)
		{
			ImageNTHeaders = new _IMAGE_NT_HEADERS ();
			ImageNTHeadersAddr = IntPtr.Zero;

            _IMAGE_DOS_HEADER ImageDOSHeader = (_IMAGE_DOS_HEADER) Marshal.PtrToStructure (PEBaseAddr, typeof (_IMAGE_DOS_HEADER));

			ImageNTHeadersAddr = new IntPtr (PEBaseAddr.ToInt32 () + ImageDOSHeader._lfanew);

			if (IsBadReadPtr (
				ImageNTHeadersAddr, 
				(uint) Marshal.SizeOf (typeof (_IMAGE_NT_HEADERS))))
			{
				return false; 
			}

			ImageNTHeaders = (_IMAGE_NT_HEADERS) Marshal.PtrToStructure (
				ImageNTHeadersAddr,
				typeof (_IMAGE_NT_HEADERS));

			return ImageNTHeaders.Signature == IMAGE_NT_SIGNATURE ? true : false;
		}

        public static bool GetExportDirectory(
			IntPtr PEBaseAddr, 
			IntPtr ImageNTHeadersAddr, 
			_IMAGE_NT_HEADERS ImageNTHeaders, 
			out _IMAGE_EXPORT_DIRECTORY ImageExportDir)
		{
			ImageExportDir = new _IMAGE_EXPORT_DIRECTORY ();
			IntPtr LastRvaSection = IntPtr.Zero;

			IntPtr ImageExportDirPtr = ImageRvaToVa (
				ImageNTHeadersAddr, 
				PEBaseAddr,
                ImageNTHeaders.OptionalHeader.ExportDirectory.VirtualAddress, 
				LastRvaSection);

			if (IsBadReadPtr (
				ImageExportDirPtr, 
				(uint) Marshal.SizeOf (typeof (_IMAGE_EXPORT_DIRECTORY))))
			{
				return false; 
			}

			ImageExportDir = (_IMAGE_EXPORT_DIRECTORY) Marshal.PtrToStructure (
				ImageExportDirPtr,
				typeof (_IMAGE_EXPORT_DIRECTORY));

			return true;
		}

        public static bool GetCor20Header(
			IntPtr PEBaseAddr, 
			IntPtr ImageNTHeadersAddr, 
			_IMAGE_NT_HEADERS ImageNTHeaders, 
			out _IMAGE_COR20_HEADER ImageCor20Header)
		{
			ImageCor20Header = new _IMAGE_COR20_HEADER ();
			IntPtr LastRvaSection = IntPtr.Zero;

			IntPtr ImageCor20HeaderPtr = ImageRvaToVa (
				ImageNTHeadersAddr, 
				PEBaseAddr, 
				ImageNTHeaders.OptionalHeader.ComDescriptorDirectory.VirtualAddress, 
				LastRvaSection);

			if (IsBadReadPtr (
				ImageCor20HeaderPtr, 
				(uint) Marshal.SizeOf (typeof (_IMAGE_COR20_HEADER))))
			{
				return false; 
			}

			ImageCor20Header = (_IMAGE_COR20_HEADER) Marshal.PtrToStructure (
				ImageCor20HeaderPtr,
				typeof (_IMAGE_COR20_HEADER));

			return true;
		}

        public static string[] GetExportedMethodsNames(string sPEName)
		{
			uint nFileHandle;
			uint nFileMapping;
			IntPtr PEBaseAddr;
			string[] ExportedMethodsNames;

			if (!PEImageHelper.GetPEImage (sPEName, out nFileHandle, out nFileMapping, out PEBaseAddr))
			{
				throw new TisException ("Bad PE image");
			}

			try
			{
				IntPtr ImageNTHeadersAddr;
				PEImageHelper._IMAGE_NT_HEADERS ImageNTHeaders;

				if (!PEImageHelper.GetImageNTHeaders (PEBaseAddr, out ImageNTHeadersAddr, out ImageNTHeaders))
				{
					throw new TisException ("Bad PE image");
				}

				GetExportedMethodsNames (PEBaseAddr, ImageNTHeadersAddr, ImageNTHeaders, out ExportedMethodsNames);
			}
			finally
			{
				PEImageHelper.FreePEImage (nFileHandle, nFileMapping, PEBaseAddr);
			}

			return ExportedMethodsNames; 
		}

        public static bool GetExportedMethodsNames(
			IntPtr PEBaseAddr, 
			IntPtr ImageNTHeadersAddr, 
			_IMAGE_NT_HEADERS ImageNTHeaders, 
			out string[] ExportedMethodsNames)
		{
			ExportedMethodsNames = new string[] {};

			_IMAGE_EXPORT_DIRECTORY ImageExportDir = new _IMAGE_EXPORT_DIRECTORY ();

			IntPtr LastRvaSection = IntPtr.Zero;

			IntPtr ImageExportDirPtr = ImageRvaToVa (
				ImageNTHeadersAddr, 
				PEBaseAddr, 
				ImageNTHeaders.OptionalHeader.ExportDirectory.VirtualAddress, 
				LastRvaSection);

			if (IsBadReadPtr (
				ImageExportDirPtr, 
				(uint) Marshal.SizeOf (typeof (_IMAGE_EXPORT_DIRECTORY))))
			{
				return false; 
			}

			ImageExportDir = (_IMAGE_EXPORT_DIRECTORY) Marshal.PtrToStructure (
				ImageExportDirPtr,
				typeof (_IMAGE_EXPORT_DIRECTORY));


			IntPtr TempPtr = ImageRvaToVa (
				ImageNTHeadersAddr, 
				PEBaseAddr, 
				(uint)ImageExportDir.AddressOfNames, 
				LastRvaSection);

			if (TempPtr == IntPtr.Zero)
			{
				return false;
			}

			ArrayList oMethodsNames = new ArrayList ();

			IntPtr StringPtr;
			string sMethodName;

			for (int i = 0; i < ImageExportDir.NumberOfNames; i++)
			{
				if (TempPtr == IntPtr.Zero)
				{
					continue;
				}

				StringPtr = ImageRvaToVa (
                    ImageNTHeadersAddr, 
					PEBaseAddr, 
					(uint) Marshal.ReadInt32 (TempPtr), 
					LastRvaSection);

				sMethodName = Marshal.PtrToStringAnsi (StringPtr);

				if (sMethodName != String.Empty)
				{
					oMethodsNames.Add (sMethodName);
				}

				TempPtr = new IntPtr (TempPtr.ToInt32 () + Marshal.SizeOf (typeof (IntPtr)));
			}

			ExportedMethodsNames = (string[]) oMethodsNames.ToArray (typeof (string));

			return true;
		}

        internal static PEInvokeType GetPEInvokeType(string sPEName)
        {
            bool bIsDNAssembly = false;
            bool bIsWin32DLL = false;
            uint nFileHandle;
            uint nFileMapping;
            IntPtr PEBaseAddr;

            if (!PEImageHelper.GetPEImage(sPEName, out nFileHandle, out nFileMapping, out PEBaseAddr))
            {
                throw new TisException("Bad PE image");
            }

            try
            {
                IntPtr ImageNTHeadersAddr;
                PEImageHelper._IMAGE_NT_HEADERS ImageNTHeaders;
                PEImageHelper._IMAGE_COR20_HEADER ImageCor20Header;

                if (!PEImageHelper.GetImageNTHeaders(PEBaseAddr, out ImageNTHeadersAddr, out ImageNTHeaders))
                {
                    throw new TisException("Bad PE image");
                }

                bIsDNAssembly = PEImageHelper.GetCor20Header(PEBaseAddr, ImageNTHeadersAddr, ImageNTHeaders, out ImageCor20Header);

                if (bIsDNAssembly)
                {
                    return PEInvokeType.DNAssembly;
                }

                PEImageHelper._IMAGE_EXPORT_DIRECTORY ImageExportDir;
                bIsWin32DLL = PEImageHelper.GetExportDirectory(PEBaseAddr, ImageNTHeadersAddr, ImageNTHeaders, out ImageExportDir);

                if (bIsWin32DLL)
                {
                    return PEInvokeType.Win32DLL;
                }
            }
            finally
            {
                PEImageHelper.FreePEImage(nFileHandle, nFileMapping, PEBaseAddr);
            }

            return PEInvokeType.Unknown;
        }

		public static bool IsDNAssembly (string sPEName)
		{
			return GetPEInvokeType (sPEName) == PEInvokeType.DNAssembly ? true : false;
		}

        public static bool IsWin32DLL(string sPEName)
		{
			return GetPEInvokeType (sPEName) == PEInvokeType.Win32DLL ? true : false;
		}

		#region Unmanaged prototypes

		[DllImport ("Kernel32.DLL")]
		private static extern uint CreateFile (
			string sFileName, 
			uint nDesiredAccess, 
			uint nShareMode,
			IntPtr SecurityAttributes,
			uint nCreationDisposition,
			uint nFlagsAndAttributes,
			uint nTemplateFile); 

		[DllImport ("Kernel32.DLL")]
		private static extern uint CreateFileMapping (
			uint FileHandle,
			IntPtr Attributes,
			uint nProtect,
			uint nMaximumSizeHigh,
            uint nMaximumSizeLow,
			string sName);

		[DllImport ("Kernel32.DLL")]
		private static extern IntPtr MapViewOfFile (
			uint nFileMappingObject,
			uint nDesiredAccess, 
			uint nFileOffsetHigh,
			uint nFileOffsetLow,
			uint nNumberOfBytesToMap);

		[DllImport ("Kernel32.DLL")]
		private static extern bool IsBadReadPtr (
			IntPtr lp,
			uint ucb);

		[DllImport ("Dbghelp.DLL")]
		private static extern IntPtr ImageRvaToVa (
			IntPtr NtHeaders,
			IntPtr Base,
			uint Rva,
			IntPtr LastRvaSection);

		[DllImport ("Kernel32.DLL")]
		private static extern bool UnmapViewOfFile (
			IntPtr BaseAddress);

		[DllImport ("Kernel32.DLL")]
		private static extern bool CloseHandle (
			uint ObjectHandle);

        [ComVisible(false)]
        [StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_DOS_HEADER
		{
			public ushort e_magic;
			public ushort e_cblp;
			public ushort e_cp;
			public ushort e_crlc;
			public ushort e_cparhdr;
			public ushort e_minalloc;
			public ushort e_maxalloc;
			public ushort e_ss;
			public ushort e_sp;
			public ushort e_csum;
			public ushort e_ip;
			public ushort e_cs;
			public ushort e_lfarlc;
			public ushort e_ovno;
			[MarshalAs (UnmanagedType.ByValArray, SizeConst=4)]
			public ushort[] e_res;
			public ushort e_oemid;
			public ushort e_oeminfo;
			[MarshalAs (UnmanagedType.ByValArray, SizeConst=10)]
			public ushort[] e_res2;
			public int _lfanew;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_NT_HEADERS
		{
			public uint	Signature;
			public _IMAGE_FILE_HEADER FileHeader;
			public _IMAGE_OPTIONAL_HEADER OptionalHeader;

		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_FILE_HEADER
		{
			public ushort Machine;
			public ushort NumberOfSections;
			public uint	TimeDateStamp;
			public uint	PointerToSymbolTable;
			public uint	NumberOfSymbols;
			public ushort SizeOfOptionalHeader;
			public ushort Characteristics;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_OPTIONAL_HEADER
		{
			public ushort Magic;
			public byte MajorLinkerVersion;
			public byte MinorLinkerVersion;
			public uint	SizeOfCode;
			public uint	SizeOfInitializedData;
			public uint	SizeOfUninitializedData;
			public uint	AddressOfEntryPoint;
			public uint	BaseOfCode;
			public uint	BaseOfData;
			public uint	ImageBase;
			public uint	SectionAlignment;
			public uint	FileAlignment;
			public ushort MajorOperatingSystemVersion;
			public ushort MinorOperatingSystemVersion;
			public ushort MajorImageVersion;
			public ushort MinorImageVersion;
			public ushort MajorSubsystemVersion;
			public ushort MinorSubsystemVersion;
			public uint	Win32VersionValue;
			public uint	SizeOfImage;
			public uint	SizeOfHeaders;
			public uint	CheckSum;
			public ushort Subsystem;
			public ushort DllCharacteristics;
			public uint	SizeOfStackReserve;
			public uint	SizeOfStackCommit;
			public uint	SizeOfHeapReserve;
			public uint	SizeOfHeapCommit;
			public uint	LoaderFlags;
			public uint	NumberOfRvaAndSizes;
//			[MarshalAs (UnmanagedType.ByValArray, SizeConst=16, ArraySubType = UnmanagedType.ByValArray)]
//            public _IMAGE_DATA_DIRECTORY[] DataDirectory;  
            public _IMAGE_DATA_DIRECTORY ExportDirectory;  
			public _IMAGE_DATA_DIRECTORY ImportDirectory;  
			public _IMAGE_DATA_DIRECTORY ResourceDirectory;  
			public _IMAGE_DATA_DIRECTORY ExceptionDirectory;  
			public _IMAGE_DATA_DIRECTORY SecurityDirectory;  
			public _IMAGE_DATA_DIRECTORY BaserelocDirectory;  
			public _IMAGE_DATA_DIRECTORY DebugDirectory;  
			public _IMAGE_DATA_DIRECTORY CopyrightDirectory;  
			public _IMAGE_DATA_DIRECTORY GlobalptrDirectory;  
			public _IMAGE_DATA_DIRECTORY TLSDirectory;  
			public _IMAGE_DATA_DIRECTORY LoadConfigDirectory;  
			public _IMAGE_DATA_DIRECTORY BoundImportDirectory;  
			public _IMAGE_DATA_DIRECTORY IATDirectory;  
			public _IMAGE_DATA_DIRECTORY DelayImportDirectory;  
			public _IMAGE_DATA_DIRECTORY ComDescriptorDirectory;  
			public _IMAGE_DATA_DIRECTORY Unknown1Directory;  
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_DATA_DIRECTORY
		{
			public uint	VirtualAddress;
			public uint	Size;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_EXPORT_DIRECTORY
		{
			public uint	Characteristics;
			public uint	TimeDateStamp;
			public ushort MajorVersion;
			public ushort MinorVersion;
			public uint	Name;
			public uint	Base;
			public uint	NumberOfFunctions;
			public uint	NumberOfNames;
			public IntPtr AddressOfFunctions;
			public IntPtr AddressOfNames;
			public IntPtr AddressOfNameOrdinals;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct _IMAGE_COR20_HEADER
		{
			public ulong cb;
			public ushort MajorRuntimeVersion;
			public ushort MinorRuntimeVersion;
			public _IMAGE_DATA_DIRECTORY MetaData;
			public ulong Flags;
			public ulong EntryPointToken;
			public _IMAGE_DATA_DIRECTORY Resources;
			public _IMAGE_DATA_DIRECTORY StrongNameSignature;
			public _IMAGE_DATA_DIRECTORY CodeManagerTable;
			public _IMAGE_DATA_DIRECTORY VTableFixups;
			public _IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
			public _IMAGE_DATA_DIRECTORY ManagedNativeHeader;
		}

		#endregion
	}
}
