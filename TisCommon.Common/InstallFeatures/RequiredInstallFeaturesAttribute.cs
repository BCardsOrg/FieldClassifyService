using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.InstallFeatures
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	[ComVisible(false)]
	public class RequiredInstallFeaturesAttribute : Attribute
	{
		public RequiredInstallFeaturesAttribute (string[] requiredInstallFeatures)
		{
            RequiredInstallFeatures = requiredInstallFeatures;
		}

        public string[] RequiredInstallFeatures { get; private set; }
	}
}
