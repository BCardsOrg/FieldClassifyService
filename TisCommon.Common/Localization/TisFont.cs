using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Design;
using TiS.Core.TisCommon;
using TiS.Core.TisCommon.Configuration;

namespace TiS.Core.TisCommon.Localization
{
    /// <summary>
    /// When this control is placed on a form, it modifies the font name and font size of the form 
    /// according to the "FontName" and "FontRatio" values
    /// in the "Globals" section of the eFLOW.ini.
    /// </summary>
    public partial class TisFont : Component, ISupportInitialize
    {
        const bool DEFAULT_FORCE_PARENT_FONT = true ;
        const bool DEFAULT_FORCE_PARENT_BIDI = true ;

        private string m_FontName ;
        private byte m_FontCharset ;
        private int m_FontRatio ;
        
        private RightToLeft m_BidiMode;
        private bool m_ForceParentFont;
        private bool m_ForceParentBidi;
        private bool m_ApplyNowToAll;

    
        //private Form m_OwnerForm;

        /// <summary>
        /// Constructor
        /// </summary>
        public TisFont() 
        {
            InitializeComponent();

            // Set the default "force parent font to children"
            // property.
            m_ForceParentFont = DEFAULT_FORCE_PARENT_FONT;
            m_ForceParentBidi = DEFAULT_FORCE_PARENT_BIDI;

            m_FontName = SystemFonts.DefaultFont.Name;
            m_FontCharset = SystemFonts.DefaultFont.GdiCharSet;
            m_FontRatio = 100;

            // Fixed value
            m_ApplyNowToAll = false;

            // Read the ini parameters 
            ReadIniParams();
        }

        /// <summary>
        /// Constructor that adds <see cref="TisFont"/> component to its container
        /// </summary>
        /// <param name="oContainer">Component's container</param>
        public TisFont(IContainer oContainer)
        {
            // Set the default "force parent font to children"
            // property.
            m_ForceParentFont = DEFAULT_FORCE_PARENT_FONT;
            m_ForceParentBidi = DEFAULT_FORCE_PARENT_BIDI;

            m_FontName = SystemFonts.DefaultFont.Name;
            m_FontCharset = SystemFonts.DefaultFont.GdiCharSet;
            m_FontRatio = 100;

            oContainer.Add(this);

            // Read the ini parameters (in case they were modified)
            ReadIniParams();

        }

        #region Read/write
        /// <summary>
        /// If set to True, this font name and ratio
        /// is applied to all the controls of the form.
        /// </summary>
        public bool ForceParentFont
        {
            get { return m_ForceParentFont; }
            set { m_ForceParentFont = value; }
        }

        /// <summary>
        /// If set to True, the specified Bidi mode
        /// is applied to all the controls of the form.
        /// </summary>
        public bool ForceParentBidi
        {
            get { return m_ForceParentBidi; }
            set { m_ForceParentBidi = value; }
        }

        public bool ApplyNowToAll
        {
            get { return m_ApplyNowToAll; }
            set 
            {
                SetApplyNowToAll(value); 
            }
        }

        #endregion

        #region Read only properties

        [BrowsableAttribute(false)]
        public string FontName
        {
            get { return m_FontName; }
        }
        [BrowsableAttribute(false)]
        public int FontRatio
        {
            get { return m_FontRatio; }
        }
        [BrowsableAttribute(false)]
        public byte FontCharset
        {
            get { return m_FontCharset; }
        }

        #endregion

        private void ReadIniParams() 
        {
            // Ini file data, for extracting the parameters.
            const string INI_FILE_NAME         = "eFLOW.INI" ;
            const string GLOBAL_SECTION        = "Globals" ;
            const string FONT_NAME_KEYNAME     = "FontName" ;
            const string FONT_RATIO_KEYNAME    = "FontRatio" ;
            const string FONT_CHARSET_KEYNAME  = "FontCharset" ;
            const string BIDI_MODE_KEYNAME     = "BidiMode" ;

            GlobalConfigurationService globalSrv = new GlobalConfigurationService();
            string iniFilePath = globalSrv.GetConfigFilesPath();

            IniFile iniFile = new IniFile(Path.Combine(iniFilePath, INI_FILE_NAME));

            try
            {
                 // Get the font name
                 m_FontName = iniFile.ReadValue
                             ( GLOBAL_SECTION, FONT_NAME_KEYNAME, SystemFonts.DefaultFont.Name ) ;

                 // Get the font ratio
                 m_FontRatio = iniFile.ReadInteger
                             ( GLOBAL_SECTION, FONT_RATIO_KEYNAME, 100 ) ;

                 // Get the charset
                 int intRead = iniFile.ReadInteger
                             (GLOBAL_SECTION, FONT_CHARSET_KEYNAME, SystemFonts.DefaultFont.GdiCharSet);

                 if (( intRead >= 0) && ( intRead <= 255))
                 {
                    m_FontCharset = (byte)intRead ;
                 }   
                 else
                 {
                     m_FontCharset = 1;
                 }   
                 // Get the Bidi mode
                 intRead= iniFile.ReadInteger
                             ( GLOBAL_SECTION, BIDI_MODE_KEYNAME, (int)RightToLeft.No ) ;

                 if (( intRead >= 0) && (intRead <= 2)) 
                 {
                    m_BidiMode = (RightToLeft)intRead;
                 }
                 else
                 {
                    m_BidiMode = RightToLeft.No; // LeftToRight by default
                 }
            }
            catch (Exception oExc)
            {
                Log.Write(Log.Severity.ERROR,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Error : {0}",
                    oExc.Message);
            }
        }

        #region Apply font to control

        private void ApplyFontToControl(Control oControl)
        {
            // Modify the font of the control if the
            // parent font property is false, or if it is a form.
            Type CurrentType = oControl.GetType();

            if (oControl is Form ||
               (oControl.Font != oControl.Parent.Font))
            {

                try
                {
                    float fontSize = oControl.Font.Size;

                    // Do not change font size in design mode
                    if (!this.DesignMode)
                    {
                        fontSize = fontSize * m_FontRatio / 100;
                    }
                    if (String.IsNullOrEmpty(m_FontName))
                    {
                        m_FontName = oControl.Font.Name;
                    }

                    oControl.Font = new Font(m_FontName,
                        fontSize,
                        oControl.Font.Style,
                        oControl.Font.Unit,
                        m_FontCharset);
                }
                catch (Exception oExc)
                {
                    Log.Write(Log.Severity.WARNING,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Error : {0}",
                        oExc.Message);
                }
            }
        }

        private void ApplyBidiToControl(Control oControl)
        {
            RightToLeft OriginalBidiMode = oControl.RightToLeft;

            Type CurrentType = oControl.GetType();

            // Modify the Bidi mode of the control if the
            // ParentBidiMode property is false, or if it is a form.
            if (oControl is Form ||
                 oControl.Parent.RightToLeft == RightToLeft.No)
            {
                // Do not change the Bidi mode in design time
                if (!this.DesignMode)
                {
                    try
                    {
                        oControl.RightToLeft = m_BidiMode;
                    }
                    catch (Exception oExc)
                    {
                        Log.Write(Log.Severity.WARNING,
                            System.Reflection.MethodInfo.GetCurrentMethod(),
                            "Error : {0}",
                            oExc.Message);
                    }
                }
            }
        }

        private void ApplyToChildren(Control oControl)
        {
            // Loop over child controls
            foreach (Control CurrentControl in oControl.Controls)
            {
                // Change the font of the child (unless it is specified
                // that it should receive the parent font automaticaly)
                if (ForceParentFont)
                {
                    ApplyFontToControl(CurrentControl);
                }

                // Change the BidiMode of the child (unless it is specified
                // that it should receive the parent font automaticaly)
                if (ForceParentBidi)
                {
                    ApplyBidiToControl(CurrentControl);
                }
                // Recursion over grand-children too (and so on)
                // if it may be a parent too.
                if (CurrentControl.HasChildren )
                {
                    ApplyToChildren(CurrentControl);
                }
                else if (CurrentControl is ToolStrip )
                {
                    ApplyToToolStripItems(((ToolStrip)CurrentControl).Items);
                }
            }
        }
        
        #endregion

        #region Apply font to toolstrip 

        private void ApplyFontToToolStripItem( ToolStripItem item)
        {
            try
            {
                float fontSize = item.Font.Size;

                // Do not change font size in design mode
                if (!this.DesignMode)
                {
                    fontSize = fontSize * m_FontRatio / 100;
                }
                if (String.IsNullOrEmpty(m_FontName))
                {
                    m_FontName = item.Font.Name;
                }

                item.Font = new Font(m_FontName,
                    fontSize,
                    item.Font.Style,
                    item.Font.Unit,
                    m_FontCharset);
            }
            catch (Exception oExc)
            {
                Log.Write(Log.Severity.WARNING,
                    System.Reflection.MethodInfo.GetCurrentMethod(),
                    "Error : {0}",
                    oExc.Message);
            }
        }

        private void ApplyBidiToToolStripItem(ToolStripItem item)
        {
            // Do not change the Bidi mode in design time
            if (!this.DesignMode)
            {
                try
                {
                    item.RightToLeft = m_BidiMode;
                }
                catch (Exception oExc)
                {
                    Log.Write(Log.Severity.WARNING,
                        System.Reflection.MethodInfo.GetCurrentMethod(),
                        "Error : {0}",
                        oExc.Message);
                }
            }
        }

        private void ApplyToToolStripItems(ToolStripItemCollection toolStripItems)
        {
            // Loop over child controls
            foreach (ToolStripItem item in toolStripItems )
            {
                // Change the font of the child (unless it is specified
                // that it should receive the parent font automaticaly)
                if (ForceParentFont)
                {
                    ApplyFontToToolStripItem(item);
                }

                // Change the BidiMode of the child (unless it is specified
                // that it should receive the parent font automaticaly)
                if (ForceParentBidi)
                {
                    ApplyBidiToToolStripItem(item);
                }
                
                //Recursion over grand-children too (and so on)
                //if it may be a parent too.
                ToolStripMenuItem menuItem = item as ToolStripMenuItem ;

                if (  menuItem != null  && menuItem.HasDropDownItems )
                {
                    ApplyToToolStripItems(menuItem.DropDownItems);
                }
            }
        }

        #endregion

        private void Apply()
        {
            // Ignore if the component was not placed on a form
            if ( hostingForm == null ) 
            {
                return ;
            }

            // Apply the specified (eFLOW.ini) font to the form
            ApplyFontToControl(hostingForm);

            // Apply the specified (eFLOW.ini) BidiMode to the form
            ApplyBidiToControl(hostingForm);

            // Force all the children to have the same font
            // and/or BidiMode unless specified otherwise.
            if (ForceParentFont || ForceParentBidi)
            {
                ApplyToChildren(hostingForm);
            }
        }

        private void SetApplyNowToAll( bool ApplyNow )
        {
            if (ApplyNow)
            {
                Apply();
            }
        }

        #region ISupportInitialize Members

        /// <summary>
        /// <see cref="ISupportInitialize"/> interface member implementation
        /// </summary>
        public void BeginInit()
        {
            
        }

        /// <summary>
        /// <see cref="ISupportInitialize"/> interface member implementation. Sets font values for the hosting form.
        /// </summary>
        public void EndInit()
        {
            if ( !this.DesignMode )
                Apply();
        }
    
        #endregion

        Form hostingForm = null;
        [BrowsableAttribute(false)]
        public Form HostingForm
        {
            // Used to populate InitializeComponent at design time
            get
            {
                if ((hostingForm == null) && this.DesignMode)
                {
                    // Access designer host and obtain reference to root component
                    IDesignerHost designer =
                        this.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (designer != null)
                    {
                        hostingForm = designer.RootComponent as Form;
                    }
                }
                return hostingForm;
            }
            set 
            {
                hostingForm = value;
            }
        }
    }
}