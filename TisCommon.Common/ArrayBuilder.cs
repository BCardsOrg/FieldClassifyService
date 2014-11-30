using System;
using System.Collections;
using System.Collections.Generic;

namespace TiS.Core.TisCommon
{
	public delegate bool ArrayElementFilter(object oElement);

	public class ArrayBuilder
	{
		private ArrayList m_Array;
		private Type      m_oElementType;

		public ArrayBuilder()
		{
			m_Array = new ArrayList();
		}

        public ArrayBuilder(IEnumerable oObjects)
			:this()
		{
			AddRange(oObjects);
		}

		public ArrayBuilder(
            IEnumerable oObjects, 
			ArrayElementFilter	oFilter)
			:this()
		{
			AddRange(oObjects, oFilter);
		}

		public ArrayBuilder(Type oElementType)
			:this()
		{
			m_oElementType = oElementType;
		}

		public void Add(object o)
		{
			m_Array.Add(o);
		}

		public void AddIfNotExists(object o)
		{
			if(m_Array.IndexOf(o) < 0)
			{
				m_Array.Add(o);
			}
		}

        public void AddRange(IEnumerable oObjects)
		{
            foreach (object obj in oObjects)
            {
                m_Array.Add(obj);
            }
		}

		public void AddRange(
            IEnumerable oObjects, 
			ArrayElementFilter	oFilter)
		{
			// If no filter
			if(oFilter == null)
			{
				// Add all
				AddRange(oObjects);
			}
			else
			{
				// If filter defined
				foreach(object oObj in oObjects)
				{
					// Add only objects that pass filter
					if(oFilter(oObj))
					{
						Add(oObj);
					}
				}
			}
		}
		
		public int Count
		{
			get 
			{
				return m_Array.Count;
			}
		}

        public bool Contains(object o)
        {
            return m_Array.IndexOf(o) > -1;
        }

        public Array GetArray(Type oElType)
		{
			Array oArray = Array.CreateInstance(oElType, m_Array.Count);

			m_Array.CopyTo(oArray);

			return oArray;
		}
		
		public Array GetArray()
		{
			Type oElType = typeof(object);

			if(m_oElementType != null)
			{
				oElType = m_oElementType;
			}
			else
			{
				// If at least one element is found,
				// Use it's type to set type of the whole array 
				if(m_Array.Count>0)
				{
					oElType = m_Array[0].GetType();
				}
			}

			return GetArray(oElType);
		}

        public byte[] GetArrayBytes(Type oElType)
        {
            List<byte> arrayBytes = new List<byte>();

            Array array = GetArray(oElType);

            TypeCode typeCode = Type.GetTypeCode(oElType);

            foreach (object element in array)
            {
                byte[] bytes = GetBytes(typeCode, element);

                arrayBytes.AddRange(bytes);
            }

            return arrayBytes.ToArray();
        }

        public byte[] GetArrayBytes()
        {
            Type oElType = typeof(object);

            if (m_oElementType != null)
            {
                oElType = m_oElementType;
            }
            else
            {
                if (m_Array.Count > 0)
                {
                    oElType = m_Array[0].GetType();
                }
            }

            return GetArrayBytes(oElType);
        }

		public ICollection Elements
		{
			get { return m_Array; }
		}

		public static Array ChangeArrayType(Array oArray, Type oTargetElType)
		{
			ArrayBuilder oArrayBuiler = 
				new ArrayBuilder(oArray);
			
			return oArrayBuiler.GetArray(oTargetElType);
		}

		public static Array CreateArray(
            IEnumerable oObjects,
			Type				oTargetElType,
			ArrayElementFilter	oFilter)
		{
			ArrayBuilder oArrayBuiler = new ArrayBuilder(
				oObjects, 
				oFilter);
			
			return oArrayBuiler.GetArray(oTargetElType);
		}

		public static Array ArrayAddElements(
			Array oSrcArray,
			Array oElementsToAdd,
			Type  oElementType)
		{
			ArrayBuilder oBuilder = new ArrayBuilder(oSrcArray);

			oBuilder.AddRange(oElementsToAdd);

			return oBuilder.GetArray(oElementType);
		}

		public static Array ArrayRemoveElement(			
			Array  oSrcArray,
			object oElement,
			Type   oElementType)
		{
			int nIndexToRemove = Array.IndexOf(oSrcArray, oElement);

			if(nIndexToRemove >= 0)
			{
				return ArrayRemoveElements(
					oSrcArray,
					nIndexToRemove,
					1,
					oElementType);
			}

			return oSrcArray;
		}

		public static Array ArrayRemoveElements(			
			Array oSrcArray,
			int   nStartIndex,
			int   nCount,
			Type  oElementType)
		{
			if(nStartIndex >= oSrcArray.Length)
			{
				throw new TisException(
					"Invalid nStartIndex={0} specified, array length ={1}",
					nStartIndex,
					oSrcArray.Length);
			}

			int nCountToRemove = nCount;

			if(nStartIndex + nCount > oSrcArray.Length)
			{
				nCountToRemove = oSrcArray.Length - nStartIndex;
			}
			
			int nPreRemovedAreaSize  = nStartIndex;
			int nPostRemovedAreaSize = oSrcArray.Length - nPreRemovedAreaSize - nCountToRemove;

			Array oNewArray = Array.CreateInstance(
				oElementType, 
				oSrcArray.Length - nCountToRemove);

			// Copy elements before removed area
			Array.Copy(
				oSrcArray, 
				0, // src index
				oNewArray,
				0, // dst index
				nPreRemovedAreaSize // Count
				);

			// Copy elements after removed area
			Array.Copy(
				oSrcArray, 
				nStartIndex + nCountToRemove, // src index
				oNewArray,
				nStartIndex, // Dst index
				nPostRemovedAreaSize // Count
				);
			
			return oNewArray;
		}

        private byte[] GetBytes(TypeCode typeCode, object value)
        {
            byte[] bytes = null;

            switch (typeCode)
            {
                case TypeCode.Int16:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToInt16(value));
                        break;
                    }
                case TypeCode.Int32:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToInt32(value));
                        break;
                    }
                case TypeCode.Int64:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToInt64(value));
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToUInt16(value));
                        break;
                    }

                case TypeCode.UInt32:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToUInt32(value));
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        bytes = BitConverter.GetBytes(Convert.ToUInt64(value));
                        break;
                    }
            }

            // BitConverter.IsLittleEndian is true even though Windows is big endian
            Array.Reverse(bytes);

            return bytes;
        }
    }
}
