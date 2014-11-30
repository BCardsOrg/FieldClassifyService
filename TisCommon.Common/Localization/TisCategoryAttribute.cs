using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TiS.Core.TisCommon.Localization
{
	public class TisCategoryAttribute : CategoryAttribute
	{
		public TisCategoryAttribute(string category, string strFileName)
			: base(category)
		{
			TisStr.AddLanguageFile(strFileName);
		}

		protected override string GetLocalizedString(string value)
		{
			return TisStr.LoadText(value, value);
		}
	}
}
