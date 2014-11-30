using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace TiS.Core.TisCommon.Reflection
{
    internal delegate object[] GetFieldValuesDelegate(object obj);
    internal delegate void SetFieldValuesDelegate(object obj, object[] values);

    internal delegate object GetFieldValueDelegate(object obj);
    internal delegate void SetFieldValueDelegate(object obj, object value);

    internal delegate object GetPropertyValueDelegate(object obj);
    internal delegate void SetPropertyValueDelegate(object obj, object value);

    internal delegate object CreateInstanceDelegate();

    public delegate object InvokeMethodDelegate(object target, object[] paramters);

    #region ILReflector

    public class ILReflector
	{
        private CreateInstanceDelegate m_createInstanceMethod;

		private Type m_reflectedType;
		private Type m_refinedType;

        private GetFieldValuesDelegate m_getAllInvoker;
        private SetFieldValuesDelegate m_setAllInvoker;

        private Dictionary<FieldInfo, GetFieldValueDelegate> m_getFieldValueInvokers =
            new Dictionary<FieldInfo, GetFieldValueDelegate>();

        private Dictionary<FieldInfo, SetFieldValueDelegate> m_setFieldValueInvokers = new
            Dictionary<FieldInfo, SetFieldValueDelegate>();

        private Dictionary<PropertyInfo, GetPropertyValueDelegate> m_getPropertyValueInvokers =
            new Dictionary<PropertyInfo, GetPropertyValueDelegate>();

        private Dictionary<PropertyInfo, SetPropertyValueDelegate> m_setPropertyValueInvokers =
            new Dictionary<PropertyInfo, SetPropertyValueDelegate>();

        public ILReflector(
            Type reflectedType, 
            FieldInfo[] fields,
            PropertyInfo[] properties)
		{
			this.m_reflectedType = reflectedType;
			this.m_refinedType = 
                reflectedType.IsValueType ? reflectedType.MakeByRefType() : reflectedType;

            InitializeType();

            m_getAllInvoker = CreateGetFieldValuesMethod(fields);
            m_setAllInvoker = CreateSetFieldValuesMethod(fields);

            this.m_createInstanceMethod = CreateCreateInstanceMethod();

            for (int i = 0; i < fields.Length; i++)
            {
                m_getFieldValueInvokers.Add(fields[i], CreateGetFieldValueMethod(fields[i]));
                m_setFieldValueInvokers.Add(fields[i], CreateSetFieldValueMethod(fields[i]));
            }

            for (int i = 0; i < properties.Length; i++)
            {
                m_getPropertyValueInvokers.Add(properties[i], CreateGetPropertyValueMethod(properties[i]));
                m_setPropertyValueInvokers.Add(properties[i], CreateSetPropertyValueMethod(properties[i]));
            }
        }

        #region Create instance

        public bool CanCreateInstance
        {
            get
            {
                return this.m_createInstanceMethod != null;
            }
        }

        public object CreateInstance()
        {
            if (CanCreateInstance)
            {
                return m_createInstanceMethod();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Field value

        public object GetFieldValue(object target, FieldInfo fieldInfo)
        {
            GetFieldValueDelegate getInvoker;

            if (m_getFieldValueInvokers.TryGetValue(fieldInfo, out getInvoker))
            {
                return getInvoker(target);
            }
            else
            {
                return fieldInfo.GetValue(target);
            }
        }

        public object[] GetFieldValues(object target)
        {
            return m_getAllInvoker(target);
        }

        public void SetFieldValue(object target, FieldInfo fieldInfo, object fieldValue)
        {
            SetFieldValueDelegate setInvoker;

            if (m_setFieldValueInvokers.TryGetValue(fieldInfo, out setInvoker))
            {
                setInvoker(target, fieldValue);
            }
            else
            {
                fieldInfo.SetValue(target, fieldValue);
            }
        }

        public void SetFieldValues(object target, object[] values)
        {
            m_setAllInvoker(target, values);
        }

        #endregion

        #region Property value

        public object GetPropertyValue(object target, PropertyInfo propertyInfo)
        {
            // Target parameter is an object, so ValueType parameters (f.e. structure) got boxed before it is passed to this method. 
            // GetValue gets the value on the boxed copy of the ValueType parameter, which is discarded after the call returned.
            // Assigning target parameter to tempTarget results in boxing of ValueType prior to calling GetValue.
            // By this the boxed object is held before it is discarded.           

            object tempTarget = target;

            object propertValue = null;

            if (propertyInfo != null)
            {
                GetPropertyValueDelegate getInvoker;

                if (m_getPropertyValueInvokers.TryGetValue(propertyInfo, out getInvoker) && getInvoker != null)
                {
                    propertValue = getInvoker(tempTarget);
                }
                else
                {
                    propertValue = propertyInfo.GetValue(tempTarget, null);
                }
            }

            return propertValue;
        }

        public object SetPropertyValue(object target, PropertyInfo propertyInfo, object value)
        {
            // Target parameter is an object, so ValueType parameters (f.e. structure) got boxed before it is passed to this method. 
            // SetValue sets the value on the boxed copy of the ValueType parameter, which is discarded after the call returned.
            // Assigning target parameter to tempTarget results in boxing of ValueType prior to calling SetValue.
            // By this the boxed object is held before it is discarded.             
            // The result is set to the boxed copy that was actually changed.

            object tempTarget = target;

            SetPropertyValueDelegate setInvoker;

            if (m_setPropertyValueInvokers.TryGetValue(propertyInfo, out setInvoker) && setInvoker != null)
            {
                setInvoker(tempTarget, value);
            }
            else
            {
                propertyInfo.SetValue(tempTarget, value, null);
            }

            return tempTarget;
        }

        #endregion

        private void InitializeType()
        {
            if (m_refinedType.TypeInitializer != null)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(m_refinedType.TypeHandle);
            }
        }

        private CreateInstanceDelegate CreateCreateInstanceMethod()
		{
            if (m_reflectedType.IsInterface || m_reflectedType.IsAbstract)
			{
				return null;
			}

            DynamicMethod method = ILUtil.CreateDynamicMethod(m_reflectedType, typeof(object), null);

			ILGenerator il = method.GetILGenerator();

            if (m_reflectedType.IsValueType)
			{
                LocalBuilder tmpLocal = il.DeclareLocal(m_reflectedType);
				il.Emit(OpCodes.Ldloca, tmpLocal);
                il.Emit(OpCodes.Initobj, m_reflectedType);
				il.Emit(OpCodes.Ldloc, tmpLocal);
                il.Emit(OpCodes.Box, m_reflectedType);
			}
			else
			{
                ConstructorInfo constructor = ReflectionUtil.GetDefaultConstructor(m_reflectedType);

				if (constructor == null)
				{
                    return null;
				}

				il.Emit(OpCodes.Newobj, constructor);
			}

			il.Emit(OpCodes.Ret);

			return (CreateInstanceDelegate) method.CreateDelegate(typeof(CreateInstanceDelegate));
		}

        private GetFieldValuesDelegate CreateGetFieldValuesMethod(FieldInfo[] fields)
		{
			Type[] methodArguments = new Type[] {typeof(object)};

            DynamicMethod method = ILUtil.CreateDynamicMethod(m_reflectedType, typeof(object[]), methodArguments);

			ILGenerator il = method.GetILGenerator();

			LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
			LocalBuilder dataLocal = il.DeclareLocal(typeof(object[]));

			il.Emit(OpCodes.Ldarg_0);
			ILUtil. EmitCastToReference(il, m_reflectedType);
			il.Emit(OpCodes.Stloc, thisLocal);

			il.Emit(OpCodes.Ldc_I4, fields.Length);
			il.Emit(OpCodes.Newarr, typeof(object));
			il.Emit(OpCodes.Stloc, dataLocal);

			for (int i = 0; i < fields.Length; i++)
			{
                FieldInfo field = fields[i];

				il.Emit(OpCodes.Ldloc, dataLocal);
				il.Emit(OpCodes.Ldc_I4, i);

				// get the value
				il.Emit(OpCodes.Ldloc, thisLocal);
                il.Emit(OpCodes.Ldfld, field);
                ILUtil.BoxIfNeeded(il, field.FieldType);

				il.Emit(OpCodes.Stelem_Ref);
			}

			il.Emit(OpCodes.Ldloc, dataLocal.LocalIndex);
			il.Emit(OpCodes.Ret);

			return (GetFieldValuesDelegate) method.CreateDelegate(typeof(GetFieldValuesDelegate));
		}

        private SetFieldValuesDelegate CreateSetFieldValuesMethod(FieldInfo[] fields)
		{
			Type[] methodArguments = new Type[] {typeof(object), typeof(object[])};
            DynamicMethod method = ILUtil.CreateDynamicMethod(m_reflectedType, null, methodArguments);

			ILGenerator il = method.GetILGenerator();

			LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
			il.Emit(OpCodes.Ldarg_0);
			ILUtil.EmitCastToReference(il, m_reflectedType);
			il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);

			for (int i = 0; i < fields.Length; i++)
			{
				Type valueType = fields[i].FieldType;

				// load 'this'
				il.Emit(OpCodes.Ldloc, thisLocal);

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, i);
				il.Emit(OpCodes.Ldelem_Ref);

                ILUtil.PrepareFieldForSet(il, valueType);

                il.Emit(OpCodes.Stfld, fields[i]);
            }

			il.Emit(OpCodes.Ret);

			return (SetFieldValuesDelegate) method.CreateDelegate(typeof(SetFieldValuesDelegate));
		}


        private GetFieldValueDelegate CreateGetFieldValueMethod(FieldInfo field)
        {
            DynamicMethod method =
                ILUtil.CreateDynamicMethod(m_reflectedType, typeof(object), new Type[] { typeof(object) });

            ILGenerator il = method.GetILGenerator();

            LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
            LocalBuilder dataLocal = il.DeclareLocal(typeof(object));

            il.Emit(OpCodes.Ldarg_0);
            ILUtil.EmitCastToReference(il, m_reflectedType);
            il.Emit(OpCodes.Stloc, thisLocal);

            ConstructorInfo ci = ReflectionUtil.GetDefaultConstructor(typeof(object));

            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc, dataLocal);

//              get the value
            il.Emit(OpCodes.Ldloc, thisLocal);
            il.Emit(OpCodes.Ldfld, field);
            ILUtil.BoxIfNeeded(il, field.FieldType);

            il.Emit(OpCodes.Stloc, dataLocal);
            il.Emit(OpCodes.Ldloc, dataLocal);

            il.Emit(OpCodes.Ret);

            return (GetFieldValueDelegate)method.CreateDelegate(typeof(GetFieldValueDelegate));
        }

        private SetFieldValueDelegate CreateSetFieldValueMethod(FieldInfo field)
        {
            DynamicMethod method =
                ILUtil.CreateDynamicMethod(m_reflectedType, null, new Type[] { typeof(object), typeof(object) });

            ILGenerator il = method.GetILGenerator();

            LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
            il.Emit(OpCodes.Ldarg_0);
            ILUtil.EmitCastToReference(il, m_reflectedType);
            il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);

            Type valueType = field.FieldType;

            // load 'this'
            il.Emit(OpCodes.Ldloc, thisLocal);

            il.Emit(OpCodes.Ldarg_1);

            ILUtil.PrepareFieldForSet(il, valueType);

            il.Emit(OpCodes.Stfld, field);

            il.Emit(OpCodes.Ret);

            return (SetFieldValueDelegate)method.CreateDelegate(typeof(SetFieldValueDelegate));
        }

        private GetPropertyValueDelegate CreateGetPropertyValueMethod(PropertyInfo pi)
        {
            MethodInfo methodInfo = pi.GetGetMethod();

            if (methodInfo == null)
            {
                return null;
            }

            DynamicMethod method =
                ILUtil.CreateDynamicMethod(m_reflectedType, typeof(object), new Type[] { typeof(object) });

            ILGenerator il = method.GetILGenerator();

            LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
            il.Emit(OpCodes.Ldarg_0);
            ILUtil.EmitCastToReference(il, m_reflectedType);
            il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);
            il.Emit(OpCodes.Ldloc, thisLocal);

            il.EmitCall(OpCodes.Callvirt, methodInfo, null);

            ILUtil.BoxIfNeeded(il, pi.PropertyType);

            il.Emit(OpCodes.Ret);

            return (GetPropertyValueDelegate)method.CreateDelegate(typeof(GetPropertyValueDelegate));
        }

        private SetPropertyValueDelegate CreateSetPropertyValueMethod(PropertyInfo pi)
        {
            MethodInfo methodInfo = pi.GetSetMethod(true);

            if (methodInfo == null)
            {
                return null;
            }

            DynamicMethod method =
                ILUtil.CreateDynamicMethod(m_reflectedType, typeof(void), new Type[] { typeof(object), typeof(object) });

            ILGenerator il = method.GetILGenerator();

            LocalBuilder thisLocal = il.DeclareLocal(m_refinedType);
            il.Emit(OpCodes.Ldarg_0);
            ILUtil.EmitCastToReference(il, m_reflectedType);
            il.Emit(OpCodes.Stloc, thisLocal.LocalIndex);
            il.Emit(OpCodes.Ldloc, thisLocal);

            il.Emit(OpCodes.Ldarg_1);

            ILUtil.UnboxIfNeeded(il, methodInfo.GetParameters()[0].ParameterType);

            il.Emit(OpCodes.Call, methodInfo);

            il.Emit(OpCodes.Ret);

            return (SetPropertyValueDelegate)method.CreateDelegate(typeof(SetPropertyValueDelegate));
        }

        #region Method invoker

        public static InvokeMethodDelegate CreateMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod method = ILUtil.CreateDynamicMethod(
                methodInfo.DeclaringType, 
                typeof(object), 
                new Type[] { typeof(object), typeof(object[]) });

            ILGenerator il = method.GetILGenerator();

            ParameterInfo[] parameters = methodInfo.GetParameters();

            Type[] paramTypes = new Type[parameters.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    paramTypes[i] = parameters[i].ParameterType.GetElementType();
                }
                else
                {
                    paramTypes[i] = parameters[i].ParameterType;
                }
            }

            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);

                if (i >= -1 && i <= 8)
                {
                    il.Emit(ILUtil.GetOpCodeByInt(i));
                }
                else
                {
                    if (i > -129 && i < 128)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (SByte)i);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, i);
                    }
                }

                il.Emit(OpCodes.Ldelem_Ref);

//                ILUtil.EmitCastToReference(il, paramTypes[i]);

                if (paramTypes[i].IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, paramTypes[i]);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, paramTypes[i]);
                }

                il.Emit(OpCodes.Stloc, locals[i]);
            }

            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, locals[i]);
                }
            }

            if (methodInfo.IsStatic)
            {
                il.EmitCall(OpCodes.Call, methodInfo, null);
            }
            else
            {
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            }

            if (methodInfo.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                ILUtil.BoxIfNeeded(il, methodInfo.ReturnType);
            }

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (parameters[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);

                    if (i >= -1 && i <= 8)
                    {
                        il.Emit(ILUtil.GetOpCodeByInt(i));
                    }
                    else
                    {
                        if (i > -129 && i < 128)
                        {
                            il.Emit(OpCodes.Ldc_I4_S, (SByte)i);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldc_I4, i);
                        }
                    }

                    il.Emit(OpCodes.Ldloc, locals[i]);

                    if (locals[i].LocalType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    }

                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);

            return (InvokeMethodDelegate)method.CreateDelegate(typeof(InvokeMethodDelegate));
        }

        #endregion
    }

    #endregion
}

