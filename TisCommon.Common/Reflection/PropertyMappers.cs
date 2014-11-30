using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TiS.Core.TisCommon.Reflection
{
    #region FieldMemoryOffsetToNameMap

    internal delegate void FieldMemoryOffsetToNameMapDelegate(Dictionary<int, string> map);

    // Maps all fields in the "typeToMap".
    internal class FieldMemoryOffsetToNameMap
    {
        // Map : field position within "typeToMap" -> field name
        private Dictionary<int, string> m_memoryOffsetToNameMap = new Dictionary<int, string>();
        private FieldNameToPropertyNameMap m_fieldNameToPropertyNameMap;

        public FieldMemoryOffsetToNameMap(
            Type typeToMap, 
            MethodInfo propertySetterWithNotification)
        {
            m_fieldNameToPropertyNameMap = 
                new FieldNameToPropertyNameMap(typeToMap, propertySetterWithNotification);

            FieldMemoryOffsetToNameMapDelegate createDelegate = Create(typeToMap);

            createDelegate(m_memoryOffsetToNameMap);
        }

        public string this[int offset]
        {
            get
            {
                return m_memoryOffsetToNameMap[offset];
            }
        }

        // Can not be coded directly. 
        // "FieldInfo field" should be loaded by ref for calculating its position within "typeToMap" 
        // and it's impossible to pass "FieldInfo field" by ref as "foreach itteration variable" when coding directly.
        private FieldMemoryOffsetToNameMapDelegate Create(Type typeToMap)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(
                "Create",
                typeof(void),
                new Type[] { typeof(Dictionary<int, string>) },
                typeToMap,
                false);

            ILGenerator IL = dynamicMethod.GetILGenerator();

            ConstructorInfo constructor = typeToMap.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new TisException("No parameterless constructor in type : [{0}]", typeToMap);
            }

            MethodInfo offsetMethod = typeof(ILMemoryOffsetCalculator).GetMethod("GetOffset", BindingFlags.Static | BindingFlags.Public);

            MethodInfo addMethod = typeof(Dictionary<int, string>).GetMethod("Add");

            IL.DeclareLocal(typeToMap);

            // Create a dummy instance of the type
            IL.Emit(OpCodes.Newobj, constructor);
            IL.Emit(OpCodes.Stloc_0);

            foreach (FieldInfo field in typeToMap.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (m_fieldNameToPropertyNameMap.Contains(field.Name))
                {
                    MethodInfo typedOffsetMethod = offsetMethod.MakeGenericMethod(field.FieldType);

                    // first param (static)
                    IL.Emit(OpCodes.Ldarg_0);
                    IL.Emit(OpCodes.Ldloc_0);
                    IL.Emit(OpCodes.Ldloc_0);
                    IL.Emit(OpCodes.Ldflda, field); // field by ref
                    IL.Emit(OpCodes.Call, typedOffsetMethod);
                    IL.Emit(OpCodes.Ldstr, m_fieldNameToPropertyNameMap[field.Name]);
                    IL.Emit(OpCodes.Call, addMethod);
                }
            }

            IL.Emit(OpCodes.Ret);

            return (FieldMemoryOffsetToNameMapDelegate)dynamicMethod.CreateDelegate(typeof(FieldMemoryOffsetToNameMapDelegate));
        }
    }

    #endregion

    #region FieldNameToPropertyInfoMap

    // Maps all fields in the "typeToMap".
    internal class FieldNameToPropertyNameMap
    {
        // Map : field name -> property name
        private Dictionary<string, string> m_fieldNameToPropertyNameMap = new Dictionary<string, string>();

        private MethodInfo m_propertySetterWithNotification;

        public FieldNameToPropertyNameMap(
            Type typeToMap, 
            MethodInfo propertySetterWithNotification)
        {
            m_propertySetterWithNotification = propertySetterWithNotification;

            Create(typeToMap);
        }

        public string this[string fieldName]
        {
            get 
            { 
                return m_fieldNameToPropertyNameMap[fieldName]; 
            }
        }

        public bool Contains(string fieldName)
        {
            return m_fieldNameToPropertyNameMap.ContainsKey(fieldName);
        }

        private void Create(Type typeToMap)
        {
            foreach (PropertyInfo property in typeToMap.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.CanWrite)  
                {
                    bool foundFieldForProperty = false;

                    FieldInfo foundField = null;

                    MethodInfo propertySetter = property.GetSetMethod(true);

                    // Parse property setter with notification 
                    ILMethodParser methodParser = new ILMethodParser(propertySetter);

                    foreach (ILInstruction currentInstruction in methodParser.Instructions)
                    {
                        int currentIndex = methodParser.Instructions.IndexOf(currentInstruction);

                        if (currentIndex > 0)
                        {
                            ILInstruction previousInstruction = methodParser.Instructions[currentIndex - 1];

                            foundFieldForProperty = 
                                SearchFieldByRef(currentInstruction, previousInstruction, ref foundField);

                            if (foundFieldForProperty)
                            {
                                break;
                            }
                        }
                    }

                    if (foundField != null && foundFieldForProperty)
                    {
                        m_fieldNameToPropertyNameMap.Add(foundField.Name, property.Name);
                    }
                }
            }
        }

        private bool SearchFieldByRef(
            ILInstruction currentInstruction, 
            ILInstruction previousInstruction,
            ref FieldInfo foundField)
        {
            if (foundField != null && currentInstruction.OpCode == OpCodes.Call)
            {
                // Validate the found field is a parameter for property setter with notification.
                MethodInfo currentMethod = currentInstruction.Operand.ResolveMethod();

                if (currentMethod != null &&
                    currentMethod.MetadataToken == m_propertySetterWithNotification.MakeGenericMethod(foundField.FieldType).MetadataToken)
                {
                    return true;
                }
            }

            // Opcodes search pattern for field loaded by ref : OpCodes.Ldarg_0 (this) & OpCodes.Ldflda (by ref)
            if (currentInstruction.OpCode == OpCodes.Ldflda &&
                previousInstruction.OpCode == OpCodes.Ldarg_0)
            {
                foundField = currentInstruction.Operand.ResolveField();
            }

            return false;
        }
    }

    #endregion
}
