using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Security
{
	public enum TisClaimResource
	{
		None,
		Flow,
		Workflow,
		Security,
		Statistic,
		Setup,
		All,
		Manager
	}

	public class ClaimResourceEx
	{
		public TisClaimResource ClaimResource { get; set; }
		public string ClaimResourceStr { get; set; }
		public string ClaimResourcePersistKey { get; set; }
	}

	public static class TisClaimResources
	{
		public static ClaimResourceEx Workflow = new ClaimResourceEx() { ClaimResource = TisClaimResource.Workflow, ClaimResourceStr = "Workflow", ClaimResourcePersistKey = "Service$...$Workflow" };
		public static ClaimResourceEx Security = new ClaimResourceEx() { ClaimResource = TisClaimResource.Security, ClaimResourceStr = "Security", ClaimResourcePersistKey = "Service$...$Security" };
		public static ClaimResourceEx Setup = new ClaimResourceEx() { ClaimResource = TisClaimResource.Setup, ClaimResourceStr = "Setup", ClaimResourcePersistKey = "Service$...$Setup" };
		public static ClaimResourceEx Statistic = new ClaimResourceEx() { ClaimResource = TisClaimResource.Statistic, ClaimResourceStr = "Statistic", ClaimResourcePersistKey = "Service$...$Statistic" };

		public static Dictionary<TisClaimResource, string> All { get; private set; }

		static TisClaimResources()
		{
			All = new Dictionary<TisClaimResource, string>();
			All.Add(Workflow.ClaimResource, "Service$...$Workflow");
			All.Add(Security.ClaimResource, "Service$...$Security");
			All.Add(Setup.ClaimResource, @"Service$TiS.Core.Application.Setup.SetupClient,TiS.Core.Application.Client$SetupClient");
			All.Add(Statistic.ClaimResource, @"Service$TiS.Core.Application.Statistics.TisStatistics,TiS.Core.Application.Client$Statistics");
		}

		static public string GetPersistKey(TisClaimResource claim)
		{
			return All[claim];
		}

		static public TisClaimResource GetClaimResource( string persistKey )
		{
			try
			{
				var claimRs = (from c in All where c.Value == persistKey select c.Key).First();
				return claimRs;
			}
			catch (Exception e)
			{
				throw new TisException(e, "Claim not exist for persistKey={0}", persistKey ) ;
			}
		}

		public static bool TryGetClaimResource(string persistKey, out TisClaimResource claimResource)
		{
			foreach (var keyStr in All)
			{
				if ( keyStr.Value == persistKey )
				{
					claimResource = keyStr.Key;
					return true;
				}
			}
			claimResource = TisClaimResource.None;
			return false;
		}
	}



}
