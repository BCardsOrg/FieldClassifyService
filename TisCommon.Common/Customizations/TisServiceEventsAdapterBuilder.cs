using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using TiS.Core.TisCommon.Services;
using TiS.Core.TisCommon.Reflection;

namespace TiS.Core.TisCommon.Customizations
{
	internal class TisServiceEventsAdapterBuilder
	{
        private const string ASSEMBLY_NAME = "TiS.Core.ServiceEventsInterceptor";

        private static AssemblyBuilder m_oAssemblyBuilder;
        private TisServiceEventsAdapter m_oEventInterceptor;

        private static object m_locker = new object();

        private static TisServiceEventsAdapterBuilder m_oServiceEventsAdapterBuilder;

        static TisServiceEventsAdapterBuilder()
        {
            AssemblyName oAssemblyName = new AssemblyName();

            oAssemblyName.Name = ASSEMBLY_NAME;

            m_oAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                oAssemblyName,
                AssemblyBuilderAccess.RunAndSave);

            m_oServiceEventsAdapterBuilder = new TisServiceEventsAdapterBuilder();
        }

        public TisServiceEventsAdapterBuilder()
        {
            m_oEventInterceptor = new TisServiceEventsAdapter();
        }

        public static void AddService(
            ITisServicesHost servicesHost,
            string sAppName,
            object oServiceInstance,
            ITisServiceInfo oServiceInfo,
            string sEventSourceBindingKey)
        {
            if (ShouldCreateAdapter(oServiceInstance, sEventSourceBindingKey))
            {
                m_oServiceEventsAdapterBuilder.AddServiceImpl(
                    servicesHost,
                    sAppName,
                    oServiceInstance,
                    oServiceInfo,
                    sEventSourceBindingKey);
            }
        }

        public static void ReleaseService(
            ITisServiceInfo serviceInfo,
            object serviceInstance)
        {
            List<TisEventParams> SupportedEvents = m_oServiceEventsAdapterBuilder.GetSupportedEvents(serviceInfo);

            if (SupportedEvents.Count > 0)
            {
                FieldInfo[] fields = ReflectionUtil.GetTypeFieldsForILReflector(serviceInstance.GetType(), BindingFlags.Default, false);

                foreach (TisEventParams oEventParams in SupportedEvents)
                {
                    var field = (from e in fields where e.Name == oEventParams.Name select e).FirstOrDefault();

                    if (field != null)
                    {
                        oEventParams.RemoveEventHandler(serviceInstance, field);
                        break;
                    }
                }
            }
        }

        private void AddServiceImpl(
            ITisServicesHost servicesHost,
            string applicationName,
            object oServiceInstance,
            ITisServiceInfo oServiceInfo,
            string sEventSourceBindingKey)
        {
            if (!StringUtil.CompareIgnoreCase(sEventSourceBindingKey, TisServicesConst.UNNAMED_INSTANCE))
            {
                List<TisEventParams> SupportedEvents = GetSupportedEvents(oServiceInfo);

                if (SupportedEvents.Count != 0)
                {
                    ITisEventsManager oEventsMngr = (ITisEventsManager)servicesHost.GetService(applicationName, "EventsManager");

                    CreateServiceEventInterceptorTypeIfNeeded(
                            applicationName,
                            oServiceInfo,
                            SupportedEvents);

                    CreateServiceEventHandlers(
                        oServiceInstance,
                        oServiceInfo,
                        sEventSourceBindingKey,
                        SupportedEvents,
                        oEventsMngr);
                }
            }
        }

        private static bool ShouldCreateAdapter(
            object oServiceInstance,
            string sEventSourceBindingKey)
        {
            return !StringUtil.CompareIgnoreCase(sEventSourceBindingKey, TisServicesConst.UNNAMED_INSTANCE) &&
                    oServiceInstance is ITisSupportEvents;
        }

        private void CreateServiceEventInterceptorTypeIfNeeded(
            string sAppName,
            ITisServiceInfo oServiceInfo,
            List<TisEventParams> SupportedEvents)
        {
            lock (m_locker)
            {
                if (m_oAssemblyBuilder.GetType(oServiceInfo.ServiceName, false, false) == null)
                {
                    ModuleBuilder oModuleBuilder = m_oAssemblyBuilder.GetDynamicModule(sAppName);

                    if (oModuleBuilder == null)
                    {
                        oModuleBuilder = m_oAssemblyBuilder.DefineDynamicModule(
                            sAppName
#if TEST_INTERCEPTION_ADAPTER
                    ,
                    ASSEMBLY_NAME + ".dll"
#endif
);
                    }

                    BuildEventInterceptorType(
                        oModuleBuilder,
                        oServiceInfo.ServiceName,
                        SupportedEvents);

#if TEST_INTERCEPTION_ADAPTER
                    m_oAssemblyBuilder.Save(ASSEMBLY_NAME + ".dll");
#endif
                }
            }
        }
    

        private void CreateServiceEventHandlers(
            object oServiceInstance,
            ITisServiceInfo oServiceInfo,
            string oEventBindingKey,
            List<TisEventParams> SupportedEvents,
            ITisEventsManager oEventsMngr)
        {
            Type oEventInterceptorDynamicType =                             //Todo : IgnoreCase does not work properly
                m_oAssemblyBuilder.GetType(oServiceInfo.ServiceName, false, false);

            object oEventInterceptorDynamicInstance;
            Delegate oEventInterceptorDynamicDelegate;

            foreach (ITisEventParams oEventParams in SupportedEvents)
            {
                oEventInterceptorDynamicInstance = Activator.CreateInstance(
                    oEventInterceptorDynamicType,
                    new object[] { m_oEventInterceptor.EventInterceptorDelegate, 
                                       oEventsMngr, 
                                       oServiceInfo, 
                                       oEventBindingKey, 
                                       oEventParams.Name,
                                        });

                oEventInterceptorDynamicDelegate = Delegate.CreateDelegate(
                    oEventParams.EventHandlerType,
                    oEventInterceptorDynamicInstance,
                    oEventParams.Name);

                oEventParams.AddEventHandler(oServiceInstance, oEventInterceptorDynamicDelegate);
            }
        }

        private Type BuildEventInterceptorType(
            ModuleBuilder oModuleBuilder,
            string sTypeName,
            List<TisEventParams> SupportedEvents)
        {
            TypeBuilder oTypeBuilder = oModuleBuilder.DefineType(
                sTypeName,
                TypeAttributes.Public);

            FieldBuilder oEventInterceptorDelegateBuilder = oTypeBuilder.DefineField(
                "m_oEventInterceptorDelegate",
                typeof(EventInterceptorDelegate),
                FieldAttributes.Private);

            FieldBuilder oEventsMngrBuilder = oTypeBuilder.DefineField(
                "m_oEventsMngr",
                typeof(ITisEventsManager),
                FieldAttributes.Private);

            FieldBuilder oEventSourceBuilder = oTypeBuilder.DefineField(
                "m_oEventSource",
                typeof(object),
                FieldAttributes.Private);

            FieldBuilder oEventBindingKeyBuilder = oTypeBuilder.DefineField(
                "m_oEventBindingKey",
                typeof(object),
                FieldAttributes.Private);

            FieldBuilder oEventNameBuilder = oTypeBuilder.DefineField(
                "m_oEventName",
                typeof(string),
                FieldAttributes.Private);

            FieldBuilder[] TypeFields =
                new FieldBuilder[] {oEventInterceptorDelegateBuilder, 
                                    oEventsMngrBuilder, 
                                    oEventSourceBuilder, 
                                    oEventBindingKeyBuilder,
                                    oEventNameBuilder};

            MethodInfo oEventInterceptorDelegateInvokeMethod =
                typeof(EventInterceptorDelegate).GetMethod("Invoke");

            BuildTypeConstructor(
                oTypeBuilder,
                TypeFields);

            foreach (ITisEventParams oEventParams in SupportedEvents)
            {
                BuildEventMethod(
                    oTypeBuilder,
                    TypeFields,
                    oEventInterceptorDelegateInvokeMethod,
                    oEventParams);
            }

            return oTypeBuilder.CreateType();
        }

        private void BuildTypeConstructor(
            TypeBuilder oTypeBuilder,
            FieldBuilder[] TypeFields)
        {
            ArrayBuilder oArrayBuilder = new ArrayBuilder(typeof(Type));

            for (int i = 0; i < TypeFields.Length; i++)
            {
                oArrayBuilder.Add(TypeFields[i].FieldType);
            }

            Type[] ConstructorArgs = (Type[])oArrayBuilder.GetArray();

            ConstructorBuilder oCtorBuilder = oTypeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                ConstructorArgs);

            ILGenerator oILGenerator = oCtorBuilder.GetILGenerator();

            oILGenerator.Emit(OpCodes.Ldarg_0);

            Type oObjectType = Type.GetType("System.Object");
            ConstructorInfo oBaseObjectCtor = oObjectType.GetConstructor(new Type[0]);

            oILGenerator.Emit(OpCodes.Call, oBaseObjectCtor);

            for (int i = 0; i < TypeFields.Length; i++)
            {
                oILGenerator.Emit(OpCodes.Ldarg_0);
                oILGenerator.Emit(OpCodes.Ldarg_S, i + 1);
                oILGenerator.Emit(OpCodes.Stfld, TypeFields[i]);
            }

            oILGenerator.Emit(OpCodes.Ret);
        }

        private void BuildEventMethod(
            TypeBuilder oTypeBuilder,
            FieldBuilder[] TypeFields,
            MethodInfo oEventInterceptorDelegateInvokeMethod,
            ITisEventParams oEventParams)
        {
            Type oBaseReturnType = GetBaseType(oEventParams.MethodSignature.ReturnInfo.ReturnType);

            MethodBuilder oMethodBuilder = oTypeBuilder.DefineMethod(
                oEventParams.Name,
                MethodAttributes.Public,
                oBaseReturnType,
                oEventParams.MethodSignature.ParamTypes);

            ILGenerator oILGenerator = oMethodBuilder.GetILGenerator();

            LocalBuilder oLocalObjArray = PackInOutParams(oILGenerator, oEventParams.MethodSignature);

            for (int i = 0; i < TypeFields.Length; i++)
            {
                oILGenerator.Emit(OpCodes.Ldarg_0);
                oILGenerator.Emit(OpCodes.Ldfld, TypeFields[i]);
            }

            oILGenerator.Emit(OpCodes.Ldloca_S, oLocalObjArray);

            oILGenerator.Emit(OpCodes.Callvirt, oEventInterceptorDelegateInvokeMethod);


            if (!IsVoidMethod(oBaseReturnType))
            {
                LocalBuilder oResult = oILGenerator.DeclareLocal(oBaseReturnType);

                oILGenerator.Emit(OpCodes.Stloc, oResult);

                PrepareReturnValue(oILGenerator, oBaseReturnType, oResult);
            }
            else
            {
                oILGenerator.Emit(OpCodes.Pop);
            }

            PrepareOutParams(oILGenerator, oEventParams.MethodSignature, oLocalObjArray);

            if (!IsVoidMethod(oBaseReturnType))
            {
                LocalBuilder oResult = oILGenerator.DeclareLocal(oBaseReturnType);

                oILGenerator.Emit(OpCodes.Ldloc, oResult);
            }

            oILGenerator.Emit(OpCodes.Ret);
        }

        private bool IsVoidMethod(Type oReturnType)
        {
            return GetBaseType(oReturnType) == typeof(void);
        }

        private LocalBuilder PackInOutParams(
            ILGenerator oILGenerator,
            ITisMethodSignature oMethodSignature)
        {
            LocalBuilder oLocalObjArray = oILGenerator.DeclareLocal(typeof(object[]));

            oILGenerator.Emit(OpCodes.Ldc_I4, oMethodSignature.Params.Length);
            oILGenerator.Emit(OpCodes.Newarr, typeof(object));
            oILGenerator.Emit(OpCodes.Stloc, oLocalObjArray);

            Type oBaseType;

            for (int i = 0; i < oMethodSignature.Params.Length; i++)
            {
                oILGenerator.Emit(OpCodes.Ldloc, oLocalObjArray);
                oILGenerator.Emit(OpCodes.Ldc_I4, i);
                oILGenerator.Emit(OpCodes.Ldarg, i + 1);

                if (oMethodSignature.Params[i].IsOut)
                {
                    oILGenerator.Emit(OpCodes.Ldind_Ref);
                }

                oBaseType = GetBaseType(oMethodSignature.ParamTypes[i]);

                if (oBaseType.IsValueType)
                {
                    oILGenerator.Emit(OpCodes.Box, oBaseType);
                }

                oILGenerator.Emit(OpCodes.Stelem_Ref);
            }

            return oLocalObjArray;
        }

        private void PrepareOutParams(
            ILGenerator oILGenerator,
            ITisMethodSignature oMethodSignature,
            LocalBuilder oLocalObjArray)
        {
            Type oBaseType;

            for (int i = 0; i < oMethodSignature.Params.Length; i++)
            {
                if (oMethodSignature.Params[i].IsOut)
                {
                    oILGenerator.Emit(OpCodes.Ldarg, i + 1);
                    oILGenerator.Emit(OpCodes.Ldloc, oLocalObjArray);
                    oILGenerator.Emit(OpCodes.Ldc_I4, i);
                    oILGenerator.Emit(OpCodes.Ldelem_Ref);

                    oBaseType = GetBaseType(oMethodSignature.ParamTypes[i]);

                    if (oBaseType.IsValueType)
                    {
                        oILGenerator.Emit(OpCodes.Unbox, oBaseType);
                        oILGenerator.Emit(OpCodes.Ldobj, oBaseType);
                        oILGenerator.Emit(OpCodes.Stobj, oBaseType);
                    }
                    else
                    {
                        oILGenerator.Emit(OpCodes.Castclass, oBaseType);
                        oILGenerator.Emit(OpCodes.Stind_Ref);
                    }
                }
            }
        }

        private void PrepareReturnValue(
            ILGenerator oILGenerator,
            Type oReturnType,
            LocalBuilder oLocal)
        {
            Type oBaseType = GetBaseType(oReturnType);

            oILGenerator.Emit(OpCodes.Ldloc, oLocal);

            if (oBaseType.IsValueType)
            {
                oILGenerator.Emit(OpCodes.Unbox, oBaseType);
            }
            else
            {
                oILGenerator.Emit(OpCodes.Castclass, oBaseType);
            }

            oILGenerator.Emit(OpCodes.Stloc, oLocal);
        }

        private Type GetBaseType(Type oRefType)
        {
            string sBaseTypeName = oRefType.FullName.TrimEnd(new char[] { '&' });
            string sBaseTypeAssemblyName = oRefType.Assembly.GetName().Name;

            Type oBaseType = Type.GetType(sBaseTypeName);

            if (oBaseType == null)
            {
                oBaseType = Type.GetType(sBaseTypeName + "," + sBaseTypeAssemblyName);

                if (oBaseType == null)
                {
                    oBaseType = Assembly.LoadWithPartialName(sBaseTypeAssemblyName).GetType(sBaseTypeName);

                    if (oBaseType == null)
                    {
                        throw new TisException("Failed to obtain type for {0}", oRefType.FullName);
                    }
                }
            }

            return oBaseType;
        }

        private List<TisEventParams> GetSupportedEvents(ITisServiceInfo oServiceInfo)
        {
            return oServiceInfo.SupportedEvents;
        }
    }
}
