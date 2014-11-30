using System;
using System.Collections.Specialized;
using System.Collections;

namespace TiS.Core.TisCommon.Evaluator
{
    public class VariableDeclarationList : NameObjectCollectionBase
    {

        public VariableDeclarationList()
            : base()
        {
        }

        public void Add(VariableDeclaration oVariableDeclaration)
        {
            BaseAdd(oVariableDeclaration.Name, oVariableDeclaration);
        }

        public void Add(string sName, Type oVarType)
        {
            Add(new VariableDeclaration(sName, oVarType));
        }

        // it is possible that the name of the meta tag is the same, but the
        //	type is different - the ContainMetaTag will return false in this case
        public bool Contains(string sName)
        {
            return this[sName] != null;
        }

        // returns null if no entry is found
        public VariableDeclaration this[string sName]
        {
            get
            {
                return (VariableDeclaration)BaseGet(sName);
            }

        }

        public new IEnumerator GetEnumerator()
        {
            return BaseGetAllValues().GetEnumerator();
        }


    }
}
