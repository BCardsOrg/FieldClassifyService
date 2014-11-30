using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TiS.Core.TisCommon.Reflection
{
    internal delegate int MemoryOffsetCalculatorDelegate<T>(object fieldContainer, ref T field);

    public static class ILMemoryOffsetCalculator
    {
        private static Type m_memoryOffsetCalculatorType = null;
        private static Dictionary<Type, Delegate> m_memoryOffsetCalculatorDelegateCache = new Dictionary<Type, Delegate>();

        static ILMemoryOffsetCalculator()
        {
            m_memoryOffsetCalculatorType = Emit();
        }

        // Calculates "field" (passed "by ref") position within "fieldContainer" i.e 
        // offset between their memory locations (untyped pointers).
        // Pinning the "fieldContainer" in memory makes it ineligible for GC.
        public static int GetOffset<T>(object fieldContainer, ref T field)
        {
            int memoryOffset = -1;

            if (m_memoryOffsetCalculatorType != null)
            {
                // for possible use with Dynamic method (needed cache) :
                // MemoryOffsetCalculatorDelegate<T> typedMemoryOffsetCalculatorDelegate = EmitMethod<T>();

                Delegate memoryOffsetCalculatorDelegate;

                if (!m_memoryOffsetCalculatorDelegateCache.TryGetValue(typeof(T), out memoryOffsetCalculatorDelegate))
                {
                    memoryOffsetCalculatorDelegate = (MemoryOffsetCalculatorDelegate<T>)Delegate.CreateDelegate(
                       typeof(MemoryOffsetCalculatorDelegate<T>),
                       null,
                       m_memoryOffsetCalculatorType.GetMethod("GetOffset").MakeGenericMethod(typeof(T)));

                    m_memoryOffsetCalculatorDelegateCache.Add(typeof(T), memoryOffsetCalculatorDelegate);
                }

                memoryOffset = ((MemoryOffsetCalculatorDelegate<T>)memoryOffsetCalculatorDelegate)(fieldContainer, ref field);
            }

            return memoryOffset;
        }

        #region Emit

        private static Type Emit()
        {
            AssemblyName assemblyName = new AssemblyName("TisMemoryOffsetCalculator");

            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);

            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder =
                moduleBuilder.DefineType("MemoryOffsetCalculator", TypeAttributes.Public);

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod("GetOffset", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig);

            GenericTypeParameterBuilder[] genericParameters = methodBuilder.DefineGenericParameters("T");
            genericParameters[0].SetGenericParameterAttributes(GenericParameterAttributes.None);

            methodBuilder.SetReturnType(typeof(int));
            methodBuilder.SetParameters(typeof(object), genericParameters[0].MakeByRefType());

            methodBuilder.DefineParameter(1, ParameterAttributes.None, "fieldContainer");
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "field");

            ILGenerator IL = methodBuilder.GetILGenerator();

            EmitOffsetCalculatorMethod(methodBuilder.GetILGenerator(), genericParameters[0]);

            return typeBuilder.CreateType();
        }

        private static MemoryOffsetCalculatorDelegate<T> EmitMethod<T>()
        {
            DynamicMethod dynamicMethod = new DynamicMethod(
                "GetOffset",
                typeof(int),
                new Type[] { typeof(object), typeof(T).MakeByRefType()},
                typeof(ILMemoryOffsetCalculator),
                false);

            EmitOffsetCalculatorMethod(dynamicMethod.GetILGenerator(), typeof(T));

            return (MemoryOffsetCalculatorDelegate<T>)dynamicMethod.CreateDelegate(typeof(MemoryOffsetCalculatorDelegate<T>));
        }

        // Code to be emitted :
        /*
          public static unsafe int GetOffset<T>(object fieldContainer, ref T field)
          {
             object pinned pinnedObjPtr = null;
             pinnedObjPtr = fieldContainer;
             byte* untypedPinnedObjPtr = (byte*) pinnedObjPtr;
             fixed (T* fieldPtr = field)
             {
                byte* untypedFieldPtr = fieldRef;
                return (int) (untypedFieldPtr - untypedPinnedObjPtr);
             }
          }
        */
        private static void EmitOffsetCalculatorMethod(
            ILGenerator IL, 
            Type pinnedFieldType)
        {
            IL.DeclareLocal(typeof(Int32));                               // Result
            IL.DeclareLocal(typeof(Byte*));                               // field pointer
            IL.DeclareLocal(pinnedFieldType.MakeByRefType(), true);       // pinned field
            IL.DeclareLocal(typeof(Byte*));                               // field container 
            IL.DeclareLocal(typeof(object), true);                        // pinned field container

            // pin field container
            IL.Emit(OpCodes.Ldnull);      
            IL.Emit(OpCodes.Stloc_S, 4);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Stloc_S, 4);

            // reference to field container
            IL.Emit(OpCodes.Ldloc_S, 4);  
            IL.Emit(OpCodes.Stloc_3);

            // pin field
            IL.Emit(OpCodes.Ldarg_1);     
            IL.Emit(OpCodes.Stloc_2);

            // reference to field
            IL.Emit(OpCodes.Ldloc_2);      
            IL.Emit(OpCodes.Stloc_1);

            //calculate offset
            IL.Emit(OpCodes.Ldloc_1);
            IL.Emit(OpCodes.Ldloc_3);
            IL.Emit(OpCodes.Sub);
            IL.Emit(OpCodes.Stloc_0);
            IL.Emit(OpCodes.Ldloc_0);
            IL.Emit(OpCodes.Ret);
        }

        #endregion
    }
}
