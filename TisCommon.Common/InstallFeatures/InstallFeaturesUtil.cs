using System;
using TiS.Core.TisCommon.Reflection;
using System.Collections.Generic;

namespace TiS.Core.TisCommon.InstallFeatures
{
	public class InstallFeaturesUtil
	{
		public static List<string> GetRequiredInstallFeatures(Type oType)
		{
            List<string> requiredInstallFeatures = new List<string>();

			RequiredInstallFeaturesAttribute installFeaturesAttribute =
				(RequiredInstallFeaturesAttribute)ReflectionUtil.GetAttribute(
				oType, 
				typeof(RequiredInstallFeaturesAttribute));

			if (installFeaturesAttribute != null)
			{
				requiredInstallFeatures.AddRange(installFeaturesAttribute.RequiredInstallFeatures);
			}

            return requiredInstallFeatures;
		}
	}
}
