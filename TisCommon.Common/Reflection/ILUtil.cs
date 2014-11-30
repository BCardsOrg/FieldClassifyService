using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;

namespace TiS.Core.TisCommon.Reflection
{
	public class ILUtil
	{
        private static Dictionary<Type, OpCode> m_typeToOpcode;
        private static Dictionary<int, OpCode> m_intToOpcode;
        private static OpCode[] m_singleByteOpCodes = new OpCode[256];
        private static OpCode[] m_doubleByteOpCodes = new OpCode[256];

        static ILUtil()
        {
            FillOpcodesMap();
            CategorizeOpcodes();
        }

        private ILUtil()
		{
		}

        public static OpCode[] SingleByteOpCodes
        {
            get
            {
                return m_singleByteOpCodes;
            }
        }

        public static OpCode[] DoubleByteOpCodes
        {
            get
            {
                return m_doubleByteOpCodes;
            }
        }

        public static bool GetOpCodeByType(Type type, out OpCode opCode)
        {
            return m_typeToOpcode.TryGetValue(type, out opCode);
        }

        public static OpCode GetOpCodeByInt(int value)
        {
            OpCode opCode;

            m_intToOpcode.TryGetValue(value, out opCode);

            return opCode;
        }

        public static void BoxIfNeeded(ILGenerator il, Type type)
		{
			if (type.IsValueType)
			{
				il.Emit(OpCodes.Box, type);
			}
		}

        public static void UnboxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
        }

        public static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        public static void PrepareFieldForSet(ILGenerator il, Type propertyType)
        {
            if (propertyType.IsValueType)
            {
                Label notNullLabel = il.DefineLabel();
                Label nullDoneLabel = il.DefineLabel();
                LocalBuilder localNew = il.DeclareLocal(propertyType);

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brtrue_S, notNullLabel);

                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldloca, localNew);
                il.Emit(OpCodes.Initobj, propertyType);
                il.Emit(OpCodes.Ldloc, localNew);
                il.Emit(OpCodes.Br_S, nullDoneLabel);

                il.MarkLabel(notNullLabel);

                il.Emit(OpCodes.Unbox, propertyType);

                OpCode specificOpCode;

                if (ILUtil.GetOpCodeByType(propertyType, out specificOpCode))
                {
                    il.Emit(specificOpCode);
                }
                else
                {
                    il.Emit(OpCodes.Ldobj, propertyType);
                }

                il.MarkLabel(nullDoneLabel);
            }
            else
            {
                if (propertyType != typeof(object))
                {
                    il.Emit(OpCodes.Castclass, propertyType);
                }
            }
        }

        public static DynamicMethod CreateDynamicMethod(Type reflectedType, Type returnType, Type[] argumentTypes)
        {
            return new DynamicMethod(string.Empty, returnType, argumentTypes, reflectedType.Module, true);
        }

        private static void FillOpcodesMap()
        {
            m_typeToOpcode = new Dictionary<Type, OpCode>();

            m_typeToOpcode[typeof(bool)] = OpCodes.Ldind_I1;
            m_typeToOpcode[typeof(sbyte)] = OpCodes.Ldind_I1;
            m_typeToOpcode[typeof(byte)] = OpCodes.Ldind_U1;

            m_typeToOpcode[typeof(char)] = OpCodes.Ldind_U2;
            m_typeToOpcode[typeof(short)] = OpCodes.Ldind_I2;
            m_typeToOpcode[typeof(ushort)] = OpCodes.Ldind_U2;

            m_typeToOpcode[typeof(int)] = OpCodes.Ldind_I4;
            m_typeToOpcode[typeof(uint)] = OpCodes.Ldind_U4;

            m_typeToOpcode[typeof(long)] = OpCodes.Ldind_I8;
            m_typeToOpcode[typeof(ulong)] = OpCodes.Ldind_I8;

            m_typeToOpcode[typeof(float)] = OpCodes.Ldind_R4;
            m_typeToOpcode[typeof(double)] = OpCodes.Ldind_R8;


            m_intToOpcode = new Dictionary<int, OpCode>();

            m_intToOpcode[-1] = OpCodes.Ldc_I4_M1;
            m_intToOpcode[0]  = OpCodes.Ldc_I4_0;
            m_intToOpcode[1]  = OpCodes.Ldc_I4_1;
            m_intToOpcode[2]  = OpCodes.Ldc_I4_2;
            m_intToOpcode[3]  = OpCodes.Ldc_I4_3;
            m_intToOpcode[4]  = OpCodes.Ldc_I4_4;
            m_intToOpcode[5]  = OpCodes.Ldc_I4_5;
            m_intToOpcode[6]  = OpCodes.Ldc_I4_6;
            m_intToOpcode[7]  = OpCodes.Ldc_I4_7;
            m_intToOpcode[8]  = OpCodes.Ldc_I4_8;
        }

        private static void CategorizeOpcodes()
        {
            foreach (FieldInfo fi in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode opCode = (OpCode)fi.GetValue(null);

                UInt16 opCodeValue = (UInt16)opCode.Value;

                if (opCodeValue < 0x100)
                {
                    m_singleByteOpCodes[opCodeValue] = opCode;
                }
                else
                {
                    if ((opCodeValue & 0xFF00) == 0xFE00)
                    {
                        m_doubleByteOpCodes[opCodeValue & 0xFF] = opCode;
                    }
                    else
                    {
                        throw new TisException("Unexpected opcode : [{0}]", opCode);
                    }
                }
            }
        }

    }
}