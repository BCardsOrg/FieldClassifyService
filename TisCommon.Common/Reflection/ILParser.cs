using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TiS.Core.TisCommon.Reflection
{
    #region ILMethodParser

    // Parses MSIL instructions
    internal class ILMethodParser
    {
        private const int DOUBLE_BYTE_OPCODES_PREFIX = 0xFE;
        private List<ILInstruction> m_instructions = new List<ILInstruction>();

        public ILMethodParser(MethodInfo methodToParse)
        {
            Parse(methodToParse);
        }

        public List<ILInstruction> Instructions
        {
            get
            {
                return m_instructions;
            }
        }

        public void Parse(MethodInfo methodToParse)
        {
            MethodBody methodBody = methodToParse.GetMethodBody();

            if (methodBody != null)
            {
                byte[] methodBodyBytes = methodBody.GetILAsByteArray();

                int currentPosition = 0;

                while (currentPosition < methodBodyBytes.Length)
                {
                    OpCode opCode = GetOperandOpCode(methodBodyBytes, ref currentPosition);

                    int operandLength = GetOperandLength(opCode, methodBodyBytes, currentPosition);

                    byte[] operandBytes = new byte[operandLength];

                    Array.Copy(methodBodyBytes, currentPosition, operandBytes, 0, operandLength);

                    currentPosition += operandLength;

                    m_instructions.Add(new ILInstruction(methodToParse, opCode, operandBytes));
                }
            }
        }

        private OpCode GetOperandOpCode(byte[] methodBodyBytes, ref int position)
        {
            byte methodBodyByte = methodBodyBytes[position++];

            if (methodBodyByte != DOUBLE_BYTE_OPCODES_PREFIX)
            {
                return ILUtil.SingleByteOpCodes[methodBodyByte];
            }
            else
            {
                return ILUtil.DoubleByteOpCodes[methodBodyBytes[position++]];
            }
        }

        private int GetOperandLength(OpCode opCode, byte[] methodBodyBytes, int position)
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    return 0;

                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return 1;

                case OperandType.InlineVar:
                    return 2;

                case OperandType.InlineBrTarget:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.ShortInlineR:
                    return 4;

                case OperandType.InlineI8:
                case OperandType.InlineR:
                    return 8;

                case OperandType.InlineSwitch:
                    uint numberOfCases = BitConverter.ToUInt32(methodBodyBytes, position);
                    return (int)(4 * (numberOfCases + 1));

                default:
                    throw new TisException("Unexpected operand type : [{0}]", opCode.OperandType);
            }
        }
    }

    #endregion

    #region ILInstruction

    internal class ILInstruction
    {
        private OpCode m_opCode;
        private ILOperand m_operand;

        public ILInstruction(MethodInfo method, OpCode opCode, byte[] rawBytes)
        {
            m_opCode = opCode;
            m_operand = new ILOperand(method, rawBytes);
        }

        public OpCode OpCode
        {
            get 
            { 
                return m_opCode; 
            }
        }

        public ILOperand Operand
        {
            get 
            { 
                return m_operand; 
            }
        }
    }

    #endregion

    #region ILOperand

    internal class ILOperand
    {
        private MethodInfo m_methodInfo;
        private int m_methodToken = -1;
        private Type m_declaringType;

        internal ILOperand(MethodInfo methodInfo, byte[] operandBytes)
        {
            m_methodInfo   = methodInfo;

            m_declaringType = methodInfo.DeclaringType;

            if (operandBytes.Length == sizeof(int))
            {
                m_methodToken = BitConverter.ToInt32(operandBytes, 0);
            }
        }

        public FieldInfo ResolveField()
        {
            if (m_methodToken > -1)
            {
                return m_declaringType.Module.ResolveField(
                                    m_methodToken,
                                    m_declaringType.GetGenericArguments(),
                                    m_methodInfo.GetGenericArguments());
            }
            else
            {
                return null;
            }
        }

        public MethodInfo ResolveMethod()
        {
            if (m_methodToken > -1)
            {
                return (MethodInfo)m_declaringType.Module.ResolveMethod(
                                    m_methodToken,
                                    m_declaringType.GetGenericArguments(),
                                    m_methodInfo.GetGenericArguments());
            }
            else
            {
                return null;
            }
        }

    }

    #endregion
}
