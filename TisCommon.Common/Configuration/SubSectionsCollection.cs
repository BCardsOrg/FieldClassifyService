using System;
using System.Collections;

namespace TiS.Core.TisCommon.Configuration
{
	/// <summary>
	/// SubSectionsCollection.
	/// 
	/// Hold list of sub sections names 
	/// </summary>
	public class SubSectionsCollection : ISubSectionsCollection
	{
		private ArrayList m_oSections = new ArrayList();


		public SubSectionsCollection(string [] oSubSectionsNames)
		{
			InitSections(oSubSectionsNames);
		}

		private void InitSections(string [] oSubSectionsNames)
		{
			if (oSubSectionsNames != null)
			{
				for (int i=0; i < oSubSectionsNames.Length; i++)
				{					
					m_oSections.Add(oSubSectionsNames[i]);
				}
			}
		}

		public string GetByIndex(int nIndex)
		{
			return this[nIndex];
		}

		public string this[int nIndex]
		{
			get
			{
				if (nIndex >-1 && nIndex < m_oSections.Count)
				{
					return (string)m_oSections[nIndex];
				}
				else
				{
					throw new TisException("SubSectionsCollection , array index out of range .");
				}
			}
		}


		public int Count 
		{ 
			get
			{
				return m_oSections.Count;
			}
		}		
	}
}
