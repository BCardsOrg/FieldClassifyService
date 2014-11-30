using System;

namespace TiS.Core.TisCommon
{
	public delegate bool ObjectFilterDelegate(object oObj);

	public enum BoolFilter 
	{ 
		True, 
		False, 
		Unspecified 
	}

	public class PrimitiveFilter
	{
		public static bool Pass(
			BoolFilter enFilter,
			bool bVal)
		{
			if(enFilter == BoolFilter.Unspecified)
			{
				return true;
			}

			if( bVal && (enFilter == BoolFilter.True) )
			{
				return true;
			}

			if( !bVal && (enFilter == BoolFilter.False) )
			{
				return true;
			}

			return false;
		}

	}
}
