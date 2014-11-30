using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
 
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ScalingFactors
    {

        private string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAccountNoField;

        private string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAmountDueField;

        private string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfDueDateField;

        private string IsSpecificKeyWordkwfAccountNoField;

        private string IsSpecificKeyWordkwfAmountDueField;

        private string IsSpecificKeyWordkwfDueDateField;

        private string IsSpecificKeyWordkwfCompanyNameField;

        private string IsSpecificKeyWordlastnameField;

        private string IsSpecificKeyWordfirstnameField;

        private string IsSpecificKeyWordcontainsvaluefirstnameField;

        private string IsDateField;

        private string IsAmountField;

        private string FeaturehasMinusField;

        private string IsZipCodeField;

        private string IsKeyWordField;

        private string AllWordsCapitalField;

        private string hasCurrencyField;

        private string FeatureWordOnLeftField;

        private string FeatureWordOnRightField;

        private string FeatureWordOnTopField;

        private string FeatureWordOnBottomField;

        private string FeatureNumberOfWordsField;

        private string FeatureNumberOfLinesField;

        private string FeatureCoulmnAtEndField;

        private string NoAlphaField;

        private string NumberOfDigitsField;

        private string RelativeDistTopField;

        private string RelativeDistLeftField;

        private string NumberOfCharsField;

        private string NumberOfAlphaField;

        private string NumberOfPunctioationField;

        private string AllCapitalField;

        private string CapitalaziedField;

        private string NumberOfCapitalField;

        private string IsNumberField;

        private string NoNumbersField;

        private string NoPunctuationField;

        private string AlphaDensityInWordField;

        private string CapitalDensityInWordField;

        private string NumericDensityInWordField;

        private string NumberOfLinesField;

        private string IsAmountOldField;

        private string FeatureCandidateOnRightisAmountField;

        private string FeatureCandidateOnTopisAmountField;

        private string FeatureCandidateOnRightisonlyalphaField;

        private string FeatureDistnceFromSpecialCandidateDistancefCompanyNameField;

        private string FeatureDistnceFromSpecialCandidateDistancefirstnamelastnameField;

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAmountDue
        {
            get
            {
                return this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAmountDueField;
            }
            set
            {
                this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAmountDueField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfDueDate
        {
            get
            {
                return this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfDueDateField;
            }
            set
            {
                this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfDueDateField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAccountNo
           {
               get
               {
                   return this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAccountNoField;
               }
               set
               {
                   this.FeatureCandidateOnLeftorTopisSpcificKeyWordkwfAccountNoField = value;
               }
           }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordkwfAccountNo
        {
            get
            {
                return this.IsSpecificKeyWordkwfAccountNoField;
            }
            set
            {
                this.IsSpecificKeyWordkwfAccountNoField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordkwfAmountDue
        {
            get
            {
                return this.IsSpecificKeyWordkwfAmountDueField;
            }
            set
            {
                this.IsSpecificKeyWordkwfAmountDueField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordkwfDueDate
        {
            get
            {
                return this.IsSpecificKeyWordkwfDueDateField;
            }
            set
            {
                this.IsSpecificKeyWordkwfDueDateField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordkwfCompanyName
        {
            get
            {
                return this.IsSpecificKeyWordkwfCompanyNameField;
            }
            set
            {
                this.IsSpecificKeyWordkwfCompanyNameField = value;
            }
        }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordfirstname
           {
               get
               {
                   return this.IsSpecificKeyWordfirstnameField;
               }
               set
               {
                   this.IsSpecificKeyWordfirstnameField = value;
               }
           }

           [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordlastname
           {
               get
               {
                   return this.IsSpecificKeyWordlastnameField;
               }
               set
               {
                   this.IsSpecificKeyWordlastnameField = value;
               }
           }

     

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
           public string IsSpecificKeyWordcontainsvaluefirstname
        {
            get
            {
                return this.IsSpecificKeyWordcontainsvaluefirstnameField;
            }
            set
            {
                this.IsSpecificKeyWordcontainsvaluefirstnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsDate
        {
            get
            {
                return this.IsDateField;
            }
            set
            {
                this.IsDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsAmount
        {
            get
            {
                return this.IsAmountField;
            }
            set
            {
                this.IsAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeaturehasMinus
        {
            get
            {
                return this.FeaturehasMinusField;
            }
            set
            {
                this.FeaturehasMinusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsZipCode
        {
            get
            {
                return this.IsZipCodeField;
            }
            set
            {
                this.IsZipCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsKeyWord
        {
            get
            {
                return this.IsKeyWordField;
            }
            set
            {
                this.IsKeyWordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AllWordsCapital
        {
            get
            {
                return this.AllWordsCapitalField;
            }
            set
            {
                this.AllWordsCapitalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string hasCurrency
        {
            get
            {
                return this.hasCurrencyField;
            }
            set
            {
                this.hasCurrencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureWordOnLeft
        {
            get
            {
                return this.FeatureWordOnLeftField;
            }
            set
            {
                this.FeatureWordOnLeftField = value;
            }
        }

       

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureWordOnRight
        {
            get
            {
                return this.FeatureWordOnRightField;
            }
            set
            {
                this.FeatureWordOnRightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureWordOnTop
        {
            get
            {
                return this.FeatureWordOnTopField;
            }
            set
            {
                this.FeatureWordOnTopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureWordOnBottom
        {
            get
            {
                return this.FeatureWordOnBottomField;
            }
            set
            {
                this.FeatureWordOnBottomField = value;
            }
        }

      

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureNumberOfWords
        {
            get
            {
                return this.FeatureNumberOfWordsField;
            }
            set
            {
                this.FeatureNumberOfWordsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureNumberOfLines
        {
            get
            {
                return this.FeatureNumberOfLinesField;
            }
            set
            {
                this.FeatureNumberOfLinesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureCoulmnAtEnd
        {
            get
            {
                return this.FeatureCoulmnAtEndField;
            }
            set
            {
                this.FeatureCoulmnAtEndField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NoAlpha
        {
            get
            {
                return this.NoAlphaField;
            }
            set
            {
                this.NoAlphaField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfDigits
        {
            get
            {
                return this.NumberOfDigitsField;
            }
            set
            {
                this.NumberOfDigitsField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RelativeDistTop
        {
            get
            {
                return this.RelativeDistTopField;
            }
            set
            {
                this.RelativeDistTopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RelativeDistLeft
        {
            get
            {
                return this.RelativeDistLeftField;
            }
            set
            {
                this.RelativeDistLeftField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfChars
        {
            get
            {
                return this.NumberOfCharsField;
            }
            set
            {
                this.NumberOfCharsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfAlpha
        {
            get
            {
                return this.NumberOfAlphaField;
            }
            set
            {
                this.NumberOfAlphaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfPunctioation
        {
            get
            {
                return this.NumberOfPunctioationField;
            }
            set
            {
                this.NumberOfPunctioationField = value;
            }
        }

   

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AllCapital
        {
            get
            {
                return this.AllCapitalField;
            }
            set
            {
                this.AllCapitalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Capitalazied
        {
            get
            {
                return this.CapitalaziedField;
            }
            set
            {
                this.CapitalaziedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfCapital
        {
            get
            {
                return this.NumberOfCapitalField;
            }
            set
            {
                this.NumberOfCapitalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsNumber
        {
            get
            {
                return this.IsNumberField;
            }
            set
            {
                this.IsNumberField = value;
            }
        }

      

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NoNumbers
        {
            get
            {
                return this.NoNumbersField;
            }
            set
            {
                this.NoNumbersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NoPunctuation
        {
            get
            {
                return this.NoPunctuationField;
            }
            set
            {
                this.NoPunctuationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AlphaDensityInWord
        {
            get
            {
                return this.AlphaDensityInWordField;
            }
            set
            {
                this.AlphaDensityInWordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CapitalDensityInWord
        {
            get
            {
                return this.CapitalDensityInWordField;
            }
            set
            {
                this.CapitalDensityInWordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumericDensityInWord
        {
            get
            {
                return this.NumericDensityInWordField;
            }
            set
            {
                this.NumericDensityInWordField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NumberOfLines
        {
            get
            {
                return this.NumberOfLinesField;
            }
            set
            {
                this.NumberOfLinesField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsAmountOld
        {
            get
            {
                return this.IsAmountOldField;
            }
            set
            {
                this.IsAmountOldField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureCandidateOnRightisAmount
        {
            get
            {
                return this.FeatureCandidateOnRightisAmountField;
            }
            set
            {
                this.FeatureCandidateOnRightisAmountField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureCandidateOnTopisAmount
        {
            get
            {
                return this.FeatureCandidateOnTopisAmountField;
            }
            set
            {
                this.FeatureCandidateOnTopisAmountField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureCandidateOnRightisonlyalpha
        {
            get
            {
                return this.FeatureCandidateOnRightisonlyalphaField;
            }
            set
            {
                this.FeatureCandidateOnRightisonlyalphaField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureDistnceFromSpecialCandidateDistancefCompanyName
        {
            get
            {
                return this.FeatureDistnceFromSpecialCandidateDistancefCompanyNameField;
            }
            set
            {
                this.FeatureDistnceFromSpecialCandidateDistancefCompanyNameField = value;
            }
        }


        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FeatureDistnceFromSpecialCandidateDistancefirstnamelastname
        {
            get
            {
                return this.FeatureDistnceFromSpecialCandidateDistancefirstnamelastnameField;
            }
            set
            {
                this.FeatureDistnceFromSpecialCandidateDistancefirstnamelastnameField = value;
            }
        }

      

       
    }

  

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class TuneScales
    {

        private ScalingFactors[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ScalingFactors")]
        public ScalingFactors[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class FeatureScalesStorage
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FeatureScalesStorageField[] Items;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class FeatureScalesStorageField
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string name;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double scale;

    
    }

  

}
