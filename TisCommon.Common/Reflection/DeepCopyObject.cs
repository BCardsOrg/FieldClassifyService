using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TiS.Core.TisCommon.Reflection
{
	public class DeepCopyObject
	{
		public static bool CopyObjectData(object source, object target, string excludedProperties, BindingFlags memberAccess)
		{
			bool copyDone = false;

			string[] excluded = null;
			if (!string.IsNullOrEmpty(excludedProperties))
			{
				excluded = excludedProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			}

			MemberInfo[] miT = target.GetType().GetMembers(memberAccess);
			foreach (MemberInfo Field in miT)
			{
				string name = Field.Name;

				// Skip over excluded properties
				if (string.IsNullOrEmpty(excludedProperties) == false
					&& excluded.Contains(name))
				{
					continue;
				}


				if (Field.MemberType == MemberTypes.Field)
				{
					FieldInfo sourcefield = source.GetType().GetField(name);
					if (sourcefield == null) { continue; }

					object SourceValue = sourcefield.GetValue(source);
					((FieldInfo)Field).SetValue(target, SourceValue);
					copyDone = true;
				}
				else if (Field.MemberType == MemberTypes.Property)
				{
					PropertyInfo piTarget = Field as PropertyInfo;
					PropertyInfo sourceField = source.GetType().GetProperty(name, memberAccess);
					if (sourceField == null) { continue; }

					if (piTarget.CanWrite && sourceField.CanRead)
					{
						object targetValue = piTarget.GetValue(target, null);
						object sourceValue = sourceField.GetValue(source, null);

						if (sourceValue == null) { continue; }

						if (sourceField.PropertyType.IsArray
							&& piTarget.PropertyType.IsArray
							&& sourceValue != null)
						{
							CopyArray(source, target, memberAccess, piTarget, sourceField, sourceValue);
							copyDone = true;
						}
						else
						{
							CopySingleData(source, target, memberAccess, piTarget, sourceField, targetValue, sourceValue);
							copyDone = true;
						}
					}
				}
			}

			return copyDone;
		}

		private static void CopySingleData(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object targetValue, object sourceValue)
		{
			//instantiate target if needed
			if (targetValue == null
				&& piTarget.PropertyType.IsValueType == false
				&& piTarget.PropertyType != typeof(string))
			{
				if (piTarget.PropertyType.IsArray)
				{
					targetValue = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
				}
				else
				{
					targetValue = Activator.CreateInstance(piTarget.PropertyType);
				}
			}


			if (piTarget.PropertyType.IsValueType == false
				&& piTarget.PropertyType != typeof(string))
			{
				if (CopyObjectData(sourceValue, targetValue, "", memberAccess))
				{
					piTarget.SetValue(target, targetValue, null);
				}
				else
				{
					piTarget.SetValue(target, sourceValue, null);
				}
			}
			else
			{
				if (piTarget.PropertyType.FullName == sourceField.PropertyType.FullName)
				{
					object tempSourceValue = sourceField.GetValue(source, null);
					piTarget.SetValue(target, tempSourceValue, null);
				}
				else
				{
					CopyObjectData(piTarget, target, "", memberAccess);
				}
			}
		}

		private static void CopyArray(object source, object target, BindingFlags memberAccess, PropertyInfo piTarget, PropertyInfo sourceField, object sourceValue)
		{
			int sourceLength = (int)sourceValue.GetType().InvokeMember("Length", BindingFlags.GetProperty, null, sourceValue, null);
			Array targetArray = Array.CreateInstance(piTarget.PropertyType.GetElementType(), sourceLength);
			Array array = (Array)sourceField.GetValue(source, null);

			for (int i = 0; i < array.Length; i++)
			{
				object o = array.GetValue(i);
				object tempTarget = Activator.CreateInstance(piTarget.PropertyType.GetElementType());
				CopyObjectData(o, tempTarget, "", memberAccess);
				targetArray.SetValue(tempTarget, i);
			}
			piTarget.SetValue(target, targetArray, null);
		}
	}

}
