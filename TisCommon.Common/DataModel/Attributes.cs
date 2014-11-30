using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TiS.Core.TisCommon.DataModel
{
    // List
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ListAttribute : Attribute
    {
        private Type m_oElementType;

        public ListAttribute(Type oElementType)
        {
            m_oElementType = oElementType;
        }

        public Type ElementType
        {
            get { return m_oElementType; }
        }
    }

    // ChildrenList
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ChildrenListAttribute : ListAttribute
    {
        public ChildrenListAttribute(Type oElementType)
            : base(oElementType)
        {
        }
    }

    // OwnedChildrenList
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class OwnedChildrenListAttribute : ChildrenListAttribute
    {
        public OwnedChildrenListAttribute(Type oElementType)
            : base(oElementType)
        {
        }
    }

    // LinkedChildrenList
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class LinkedChildrenListAttribute : ChildrenListAttribute
    {
        public LinkedChildrenListAttribute(Type oElementType)
            : base(oElementType)
        {
        }
    }

    // Parent
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ParentAttribute : Attribute
    {
        private bool m_bMandatory;

        public ParentAttribute()
        {
            m_bMandatory = false;
        }

        public ParentAttribute(bool bMandatory)
        {
            m_bMandatory = bMandatory;
        }

        public bool Mandatory
        {
            get { return m_bMandatory; }
        }
    }

    // OwnerParent
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class OwnerParentAttribute : ParentAttribute
    {
        public OwnerParentAttribute(bool bMandatory)
            : base(bMandatory)
        {
        }

        public OwnerParentAttribute()
        {
        }
    }

    // Child
	//[ComVisible(false)]
	//[AttributeUsage(AttributeTargets.Field, Inherited = true)]
	//public class ChildAttribute : Attribute
	//{
	//}

    // AggregatedChild
    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class AggregatedChildAttribute : Attribute
    {
        private string m_sParamsPrefix = String.Empty;
        private string m_sParamsPostfix = String.Empty;

        public AggregatedChildAttribute()
        {
        }

        public AggregatedChildAttribute(string sParamsPrefix)
            : this(sParamsPrefix, String.Empty)
        {
        }

        public AggregatedChildAttribute(
            string sParamsPrefix,
            string sParamsPostfix)
        {
            m_sParamsPrefix = sParamsPrefix;
            m_sParamsPostfix = sParamsPostfix;
        }

        public string ParamsPrefix
        {
            get { return m_sParamsPrefix; }
        }

        public string ParamsPostfix
        {
            get { return m_sParamsPostfix; }
        }

    }

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Field)]
    public class UniqueParentLinkAttribute : Attribute
    {
    }

    [ComVisible(false)]
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        private bool m_bIsParameter = true;

        public ParameterAttribute()
        {
        }

        public ParameterAttribute(bool bIsParameter)
        {
            m_bIsParameter = bIsParameter;
        }

        public bool IsParameter
        {
            get { return m_bIsParameter; }
        }
    }
}
