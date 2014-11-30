using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Security
{
	//public static class TisClaimOperationStr
	//{
	//    public const string Write = "Write";
	//    public const string Read = "Read";
	//    public const string Run = "Run";
	//    public const string Configuration = "Configuration";
	//    public const string Debug = "Debug";
	//    public const string All = "All";
	//    public const string None = "None";
	//}

	public enum TisClaimOperation
	{
		Write,
		Read,
		Run,
		Configuration,
		Debug,
		All,
		None
	}

	public class ClaimOperationEx
	{
		public TisClaimOperation ClaimOperation { get; set; }
		public string ClaimOperationStr { get; set; }
	}

	public static class TisClaimOperations
	{
		public static ClaimOperationEx Write = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.Write, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_WRITE };
		public static ClaimOperationEx Read = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.Read, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_READ };
		public static ClaimOperationEx Run = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.Run, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_EXECUTE };
		public static ClaimOperationEx Configuration = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.Configuration, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_CONFIGURE };
		public static ClaimOperationEx Debug = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.Debug, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_DEBUG };
		public static ClaimOperationEx None = new ClaimOperationEx() { ClaimOperation = TisClaimOperation.None, ClaimOperationStr = PermissionsConst.TIS_PERMISSION_NONE };

		public static Dictionary<TisClaimOperation, string> All { get; private set; }

		static TisClaimOperations()
		{
			All = new Dictionary<TisClaimOperation, string>();

			All.Add(TisClaimOperation.Write, PermissionsConst.TIS_PERMISSION_WRITE);
			All.Add(TisClaimOperation.Read, PermissionsConst.TIS_PERMISSION_READ);
			All.Add(TisClaimOperation.Run, PermissionsConst.TIS_PERMISSION_EXECUTE);
			All.Add(TisClaimOperation.Configuration, PermissionsConst.TIS_PERMISSION_CONFIGURE);
			All.Add(TisClaimOperation.Debug, PermissionsConst.TIS_PERMISSION_DEBUG);
			All.Add(TisClaimOperation.None, PermissionsConst.TIS_PERMISSION_NONE);
		}

		static public TisClaimOperation GetClaimOperation(string operationKey)
		{
			try
			{
				var claimOp = (from c in All where c.Value == operationKey select c.Key).First();
				return claimOp;
			}
			catch (Exception e)
			{
				throw new TisException(e, "Claim operation not exist for operationKey={0}", operationKey);
			}
		}
	}
}
