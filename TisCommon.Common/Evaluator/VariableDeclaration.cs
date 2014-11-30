using System;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.Evaluator
{
    [ComVisible(false)]
    public class VariableDeclaration
    {
        private Type m_oType;
        private string m_sName;

        public VariableDeclaration(string sName, Type oVarType)
        {
            m_sName = sName;
            m_oType = oVarType;
        }

        public Type Type
        {
            get { return m_oType; }
        }

        public string Name
        {
            get { return m_sName; }
        }

        // this implementation is based on the fact that MetaTagSchema inherits
        //	EQUALS directly from Object
        public override bool Equals(object ObjectToCompareTo)
        {
            // Add the specific comparision with the specific value that
            //	this class implements
            VariableDeclaration oOtherObject = (VariableDeclaration)ObjectToCompareTo;

            // check the base class ( Name ) equality 
            if (!Object.Equals(this.m_sName, oOtherObject.Name))
                return false;

            // compare reference type, prevents NullArgumentException
            //	in case on of the compared objects is null
            if (!Object.Equals(this.Type, oOtherObject.Type))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return m_sName.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name + "\t\t" + (this.Type).ToString();
        }

        // override the default ==
        public static bool operator ==(VariableDeclaration o1, VariableDeclaration o2)
        {
            return Object.Equals(o1, o2);
        }

        public static bool operator !=(VariableDeclaration o1, VariableDeclaration o2)
        {
            return !(o1 == o2);
        }

    }
}
