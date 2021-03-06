﻿// ------------------------------------------------------------------------------
//<autogenerated>
//        This code was generated by Microsoft Visual Studio Team System 2005.
//
//        Changes to this file may cause incorrect behavior and will be lost if
//        the code is regenerated.
//</autogenerated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TiS.Recognition.Common;
using Recognition.Locator;

namespace RecognitionCommonTests
{
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class BaseAccessor {
    
    protected Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject m_privateObject;
    
    protected BaseAccessor(object target, Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) {
        m_privateObject = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(target, type);
    }
    
    protected BaseAccessor(Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) : 
            this(null, type) {
    }
    
    internal virtual object Target {
        get {
            return m_privateObject.Target;
        }
    }
    
    public override string ToString() {
        return this.Target.ToString();
    }
    
    public override bool Equals(object obj) {
        if (typeof(BaseAccessor).IsInstanceOfType(obj)) {
            obj = ((BaseAccessor)(obj)).Target;
        }
        return this.Target.Equals(obj);
    }
    
    public override int GetHashCode() {
        return this.Target.GetHashCode();
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class TiS_Recognition_Common_TPageAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::TiS.Recognition.Common.TPage));
    
    internal TiS_Recognition_Common_TPageAccessor(global::TiS.Recognition.Common.TPage target) : 
            base(target, m_privateType) {
    }
    
    internal global::TiS.Recognition.Common.TOCRRect m_oRect {
        get {
            global::TiS.Recognition.Common.TOCRRect ret = ((global::TiS.Recognition.Common.TOCRRect)(m_privateObject.GetField("m_oRect")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_oRect", value);
        }
    }
    
    internal int m_iDeskewX {
        get {
            int ret = ((int)(m_privateObject.GetField("m_iDeskewX")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_iDeskewX", value);
        }
    }
    
    internal int m_iDeskewY {
        get {
            int ret = ((int)(m_privateObject.GetField("m_iDeskewY")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_iDeskewY", value);
        }
    }
    
    internal int m_iResolution {
        get {
            int ret = ((int)(m_privateObject.GetField("m_iResolution")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_iResolution", value);
        }
    }
    
    internal int m_iImageWidth {
        get {
            int ret = ((int)(m_privateObject.GetField("m_iImageWidth")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_iImageWidth", value);
        }
    }
    
    internal int m_iImageHeight {
        get {
            int ret = ((int)(m_privateObject.GetField("m_iImageHeight")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_iImageHeight", value);
        }
    }
    
    internal void AddWord(global::TiS.Recognition.Common.TWord oNewWord) {
        object[] args = new object[] {
                oNewWord};
        m_privateObject.Invoke("AddWord", new System.Type[] {
                    typeof(global::TiS.Recognition.Common.TWord)}, args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Recognition_Locator_LocatorHelperAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Recognition.Locator", "Recognition.Locator.LocatorHelper");
    
    internal Recognition_Locator_LocatorHelperAccessor() : 
            base(m_privateType) {
    }
    
    internal static int AvrageWordHeight {
        get {
            int ret = ((int)(m_privateType.GetStaticField("AvrageWordHeight")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("AvrageWordHeight", value);
        }
    }
    
    internal static int WordWidth {
        get {
            int ret = ((int)(m_privateType.GetStaticField("WordWidth")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("WordWidth", value);
        }
    }
    
    internal static int DivisionUpperLimit {
        get {
            int ret = ((int)(m_privateType.GetStaticField("DivisionUpperLimit")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("DivisionUpperLimit", value);
        }
    }
    
    internal static global::System.Drawing.Size GetBestSubDivision(global::System.Drawing.Size boundingSize) {
        object[] args = new object[] {
                boundingSize};
        global::System.Drawing.Size ret = ((global::System.Drawing.Size)(m_privateType.InvokeStatic("GetBestSubDivision", new System.Type[] {
                    typeof(global::System.Drawing.Size)}, args)));
        return ret;
    }
    
    internal static int GetBestSubDivision(int intervalLength, int preferedLength, int divisionUpperLimit) {
        object[] args = new object[] {
                intervalLength,
                preferedLength,
                divisionUpperLimit};
        int ret = ((int)(m_privateType.InvokeStatic("GetBestSubDivision", new System.Type[] {
                    typeof(int),
                    typeof(int),
                    typeof(int)}, args)));
        return ret;
    }
    
    internal static object CreatePrivate() {
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject priv_obj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("Recognition.Locator", "Recognition.Locator.LocatorHelper", new object[0]);
        return priv_obj.Target;
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Recognition_Locator_DataLineIndexerAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Recognition.Locator", "Recognition.Locator.DataLineIndexer");
    
    internal Recognition_Locator_DataLineIndexerAccessor(object target) : 
            base(target, m_privateType) {
    }
    
    internal global::TiS.Recognition.Common.TLine m_Line {
        get {
            global::TiS.Recognition.Common.TLine ret = ((global::TiS.Recognition.Common.TLine)(m_privateObject.GetField("m_Line")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_Line", value);
        }
    }
    
    internal global::System.Drawing.Size SubDivisionSize {
        get {
            global::System.Drawing.Size ret = ((global::System.Drawing.Size)(m_privateObject.GetField("SubDivisionSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("SubDivisionSize", value);
        }
    }
    
    internal System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>> m_Words {
        get {
            System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>> ret = ((System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>>)(m_privateObject.GetField("m_Words")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_Words", value);
        }
    }
    
    internal global::TiS.Recognition.Common.TLine Line {
        get {
            global::TiS.Recognition.Common.TLine ret = ((global::TiS.Recognition.Common.TLine)(m_privateObject.GetProperty("Line")));
            return ret;
        }
    }
    
    internal static object CreatePrivate(global::TiS.Recognition.Common.TLine line) {
        object[] args = new object[] {
                line};
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject priv_obj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("Recognition.Locator", "Recognition.Locator.DataLineIndexer", new System.Type[] {
                    typeof(global::TiS.Recognition.Common.TLine)}, args);
        return priv_obj.Target;
    }
    
    internal void IndexLine() {
        object[] args = new object[0];
        m_privateObject.Invoke("IndexLine", new System.Type[0], args);
    }
}
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class Recognition_Locator_DataLocatorAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::Recognition.Locator.DataLocator));
    
    internal Recognition_Locator_DataLocatorAccessor(global::Recognition.Locator.DataLocator target) : 
            base(target, m_privateType) {
    }
    
    internal global::TiS.Recognition.Common.TPage m_Page {
        get {
            global::TiS.Recognition.Common.TPage ret = ((global::TiS.Recognition.Common.TPage)(m_privateObject.GetField("m_Page")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_Page", value);
        }
    }
    
    internal global::System.Drawing.Size SubDivisionSize {
        get {
            global::System.Drawing.Size ret = ((global::System.Drawing.Size)(m_privateObject.GetField("SubDivisionSize")));
            return ret;
        }
        set {
            m_privateObject.SetField("SubDivisionSize", value);
        }
    }
    
    internal System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>> m_Lines {
        get {
            System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>> ret = ((System.Collections.Generic.Dictionary<System.Drawing.Rectangle, System.Collections.Generic.IList<TiS.Recognition.Common.IOCRData>>)(m_privateObject.GetField("m_Lines")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_Lines", value);
        }
    }
    
    internal System.Collections.Generic.Dictionary<TiS.Recognition.Common.TLine, Recognition.Locator.DataLineIndexer> m_LineWords {
        get {
            System.Collections.Generic.Dictionary<TiS.Recognition.Common.TLine, Recognition.Locator.DataLineIndexer> ret = ((System.Collections.Generic.Dictionary<TiS.Recognition.Common.TLine, Recognition.Locator.DataLineIndexer>)(m_privateObject.GetField("m_LineWords")));
            return ret;
        }
        set {
            m_privateObject.SetField("m_LineWords", value);
        }
    }
    
    internal void IndexPage() {
        object[] args = new object[0];
        m_privateObject.Invoke("IndexPage", new System.Type[0], args);
    }
}
}
