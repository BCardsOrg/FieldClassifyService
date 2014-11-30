using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class VariableValue
    {
        private string m_sName;
        private object m_oValue;

        public string Name
        {
            get { return m_sName; }
        }


        public object Value
        {
            get { return m_oValue; }
            set { m_oValue = value; }
        }

        public VariableValue(string sName, object oValue)
        {
            m_sName = sName;
            m_oValue = oValue;
        }

        // this implementation is based on the fact that MetaTagSchema inherits
        //	EQUALS directly from Object
        public override bool Equals(object ObjectToCompareTo)
        {

            // check the base class ( Name ) equality 
            if (!base.Equals(ObjectToCompareTo))
                return false;

            // Add the specific comparision with the specific value that
            //	this class implements
            VariableValue OtherObject = (VariableValue)ObjectToCompareTo;

            // compare reference type, prevents NullArgumentException
            //	in case on of the compared objects is null
            if (!Object.Equals(this.Value, OtherObject.Value))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name + "\t\t" + (this.Value).ToString();
        }

        // override the default ==
        public static bool operator ==(VariableValue o1, VariableValue o2)
        {
            return Object.Equals(o1, o2);
        }

        public static bool operator !=(VariableValue o1, VariableValue o2)
        {
            return !(o1 == o2);
        }

        #region strong typed value accessors

        public int Int
        {
            get
            {
                if (Value == null)
                    throw new InvalidCastException("Value is 'null'");

                var type = Value.GetType();

                if (type == typeof(string))
                {
                    int result;
                    if (int.TryParse((string)Value, out result) == true)
                        return result;
                    else
                        throw new InvalidCastException("Expected int value");
                }
                else
                    if (type == typeof(Int64))
                    {
                        return (this.Value as IConvertible).ToInt32(CultureInfo.InvariantCulture);
                    }
                return (int)(this.Value);
            }
        }

        public bool Bool
        {
            get
            {
                if (Value == null)
                    throw new InvalidCastException("Value is 'null'");
                bool result;
                var type = Value.GetType();
                if (type == typeof(bool))
                    return (bool)(this.Value);
                else if (type == typeof(string) &&
                   bool.TryParse((string)Value, out result) == true)
                    return result;
                else
                    throw new InvalidCastException("Expected bool value");
            }
        }

        public string String
        {
            get
            {
                if (Value == null)
                    return null;
                var type = Value.GetType();
                if (type == typeof(string))
                    return (string)(this.Value);
                else
                    return Value.ToString();

            }
        }

        public float Float
        {
            get
            {
                if (Value == null)
                    throw new InvalidCastException("Value is 'null'");
                float result;
                var type = Value.GetType();
                if (type == typeof(float))
                    return (float)(this.Value);
                else if (type == typeof(string) &&
                   float.TryParse((string)Value, out result) == true)
                    return result;
                else
                    throw new InvalidCastException("Expected float value");
            }
        }

        public double GetDouble
        {
            get
            {
                if (Value == null)
                    throw new InvalidCastException("Value is 'null'");
                double result;
                var type = Value.GetType();
                if (type == typeof(double))
                    return (double)(this.Value);
                else if (type == typeof(string) &&
                   double.TryParse((string)Value, out result) == true)
                    return result;
                else
                    throw new InvalidCastException("Expected double value");
            }
        }

        public DateTime Date
        {
            get
            {
                if (Value == null)
                    throw new InvalidCastException("Value is 'null'");
                DateTimeConverter converter = new DateTimeConverter();

                if (Value.GetType() == typeof(DateTime))
                {
                    return (DateTime)Value;
                }
                else if (converter.CanConvertFrom(Value.GetType()) == true)
                {
                    return (DateTime)converter.ConvertFrom(null, CultureInfo.InvariantCulture, Value);
                }
                else
                {
                    throw new InvalidCastException("Expected DateTime value");
                }
            }
        }
        #endregion
        public static explicit operator DateTime(VariableValue val)
        {
            return val.Date;
        }
        public static explicit operator int(VariableValue val)
        {
            return val.Int;
        }
        public static explicit operator double(VariableValue val)
        {
            return val.GetDouble;
        }
        public static explicit operator float(VariableValue val)
        {
            return val.Float;
        }
        public static explicit operator string(VariableValue val)
        {
            return val.String;
        }
        public static explicit operator bool(VariableValue val)
        {
            return val.Bool;
        }

    }
}
