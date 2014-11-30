using System;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using System.IO;

namespace TiS.Core.TisCommon.Evaluator
{
    internal class StrongTypedRuleBuilder
    {
        private long m_UniqueID;

        protected VariableDeclarationList m_GlobalVariablesSchema;
        protected object m_PredefinedFunctions;

        protected FunctionsReflector m_FunctionsReflector;

        private const string STRONG_TYPED_CLASSES_NAMESPACE = "TiSStrongTypedRulesClasses";
        private const string STRONG_TYPED_CLASSES_INITIALS = "TiSStrongTypedRules";
        private const string RUN_TRUE_METHOD_PARAMETER_NAME = "TiS_TagsValueForRuleCalculation_Tis";

        private string m_sTempFilesPath;
        private string m_TestVBRulesFileFullPath;
        private bool m_bKeepTempFiles;

        internal StrongTypedRuleBuilder(
            VariableDeclarationList TagsSchema,
            object PredefinedFunctions,
            string sTempFilesPath,
            bool bKeepTempFiles)
        {
            if (TagsSchema == null || PredefinedFunctions == null)
                throw new ArgumentNullException("Error! StrongTypedRuleBuilder:StrongTypedRuleBuilder can not get null arguments");

            m_sTempFilesPath = sTempFilesPath;

            // TO_DO: Change... Because when running from IIS it cant find the TestVBRule.vb when it is without full path.
            m_TestVBRulesFileFullPath = Path.GetDirectoryName(m_sTempFilesPath);
            m_TestVBRulesFileFullPath = Path.GetDirectoryName(m_TestVBRulesFileFullPath);
            m_TestVBRulesFileFullPath = Path.Combine(m_TestVBRulesFileFullPath, @"bin\TestVBRule.vb");

            m_GlobalVariablesSchema = TagsSchema;
            m_PredefinedFunctions = PredefinedFunctions;
            m_UniqueID = 0;
            m_FunctionsReflector = new FunctionsReflector(PredefinedFunctions);
            m_bKeepTempFiles = bKeepTempFiles;
        }

        public StrongTypeRulesBase CreateNewRule(Rule RuleToCreate)
        {
            StrongTypeRulesBase[] CompiledRules = CreateNewRules(
                new Rule[] { RuleToCreate });

            if (CompiledRules.Length != 1)
            {
                throw new TisException(
                    "CreateNewRules should return 1 result, {0} returned",
                    CompiledRules.Length);
            }

            return CompiledRules[0];
        }

        public StrongTypeRulesBase[] CreateNewRules(Rule[] RulesToCreate)
        {
            Log.WriteDetailedDebug("{0}: Start compiling rules {1}",
                GetType().FullName,
                RulesToString(RulesToCreate));

            #region Namespaces creation and import relevant namespaces

            CodeCompileUnit CompileUnit = new CodeCompileUnit();

            CodeNamespace StrongTypedNamespace = new CodeNamespace(STRONG_TYPED_CLASSES_NAMESPACE);

            StrongTypedNamespace.Imports.Add(new CodeNamespaceImport("System"));

            // a reference to the current module is required since the compiled code inherits
            //	from a base class which is defined in this module
            string BaseClassNamespace = typeof(StrongTypeRulesBase).Namespace;

            StrongTypedNamespace.Imports.Add(new CodeNamespaceImport(BaseClassNamespace));

            CompileUnit.Namespaces.Add(StrongTypedNamespace);

            #endregion

            #region Create & Add Dom classes

            // Create Type names for rules
            string[] TypeNames = CreateTypeNames(RulesToCreate);

            // Create & Add Rules classes to namespace
            for (int i = 0; i < RulesToCreate.Length; i++)
            {
                Rule oRule = RulesToCreate[i];
                string sUniqueRuleName = TypeNames[i];

                CodeTypeDeclaration oRuleDomClass = CreateRuleDomClass(
                    oRule,
                    sUniqueRuleName);

                StrongTypedNamespace.Types.Add(oRuleDomClass);
            }

            #endregion

            #region Prepare CompilerParams

            CompilerParameters CompilerParams = new CompilerParameters();

            // scan each meta tag in the collection
            foreach (VariableDeclaration oVariableDeclaration in m_GlobalVariablesSchema)
            {
                // add reference to the assembly that contains that meta tag
                // TODO: add only assemblies that does not occure more than once
                Type VariableType = oVariableDeclaration.Type;

                CompilerParams.ReferencedAssemblies.Add(VariableType.Module.FullyQualifiedName);
            }

            #endregion

            #region Convert the generated CompileUnit to source code in memory (string)


            VBCodeProvider VBProvider = new VBCodeProvider();
            ICodeGenerator CodeGen = VBProvider.CreateGenerator();

            StringBuilder ClassSourceCode = new StringBuilder();
            StringWriter StringWrite = new StringWriter(ClassSourceCode);

            CodeGeneratorOptions CodeGenOptions = new CodeGeneratorOptions();
            CodeGenOptions.BracingStyle = "C";
            CodeGenOptions.ElseOnClosing = false;
            CodeGen.GenerateCodeFromCompileUnit(CompileUnit,
                                                    StringWrite,
                                                    CodeGenOptions);
            #endregion

            #region DEBUG Mode Only - Save the resulted generated source code to disk

#if (DEBUG_RULE_BUILDER)
            try
            {
                String sourceFile = m_TestVBRulesFileFullPath;

                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(sourceFile));

                IndentedTextWriter tw = new IndentedTextWriter(
                    new StreamWriter(sourceFile, false), "    ");

                CodeGen.GenerateCodeFromCompileUnit(CompileUnit,
                    tw,
                    CodeGenOptions);

                tw.Close();
            }
            catch
            {
            }

#endif
            #endregion

            #region Compile the source code

            ICodeCompiler VBCompiler = VBProvider.CreateCompiler();

            #region Set compiler references to needed modules: System32.dll, original object, base class

            CompilerParams.ReferencedAssemblies.Add("System.dll");

            // Since the run-time compiled unit should know the the type it
            //	was derived from, during the compilation process, an explicit reference
            //	to the module that contains the class (which must be in memory since
            //	the class was just sent to this function) must be add to the compilation
            //	unit.
            string VariableValuesModuleName = typeof(VariableValueList).Module.FullyQualifiedName;
            CompilerParams.ReferencedAssemblies.Add(VariableValuesModuleName);

            string UserDefinedFunctionsModuleName = (m_PredefinedFunctions.GetType()).Module.FullyQualifiedName;

            CompilerParams.ReferencedAssemblies.Add(UserDefinedFunctionsModuleName);

            #endregion


            CompilerParams.GenerateExecutable = false;

            CompilerParams.GenerateInMemory = true;

            if (StringUtil.IsStringInitialized(m_sTempFilesPath))
            {
                CompilerParams.TempFiles = new TempFileCollection(m_sTempFilesPath);
                CompilerParams.TempFiles.KeepFiles = m_bKeepTempFiles;
            }

            CompilerResults CompilerRes = VBCompiler.CompileAssemblyFromDom(CompilerParams,
                CompileUnit);

            Assembly oCompiledAssembly = null;

            try
            {
                // Check if we have compilation errors
                if (CompilerRes.Errors.Count > 0)
                {
                    throw new TisException("Errors in compilation");
                }

                if (CompilerRes.PathToAssembly != null)
                {
                    // For the case that assembly not generated in memory
                    oCompiledAssembly = Assembly.LoadFrom(
                        CompilerRes.PathToAssembly);
                }
                else
                {
                    // We can still have a problem 
                    // (Maybe there's no compilation errors because 
                    // the compiler not even started)
                    oCompiledAssembly = CompilerRes.CompiledAssembly;
                }
            }
            catch (Exception oOrigExc)
            {
                Log.WriteException(oOrigExc);

                RuleCompilationException oExc = new RuleCompilationException(
                    CompilerRes.Output,
                    RulesToString(RulesToCreate));

                // Add errors
                foreach (CompilerError oErr in CompilerRes.Errors)
                {
                    oExc.AddErrorInfo(
                        oErr.IsWarning,
                        oErr.ErrorText,
                        oErr.ErrorNumber,
                        oErr.Column,
                        oErr.FileName,
                        oErr.Line);
                }

                Log.WriteException(oExc);

                throw oExc;
            }

            #endregion

            StrongTypeRulesBase[] RuleObjects = GetRulesFromAssembly(
                oCompiledAssembly,
                TypeNames);

            return RuleObjects;
        }

        //
        //	Private
        //

        #region Rules TypeName creation

        private string[] CreateTypeNames(Rule[] Rules)
        {
            string[] TypeNames = new string[Rules.Length];

            for (int i = 0; i < Rules.Length; i++)
            {
                TypeNames[i] = CreateTypeName(Rules[i]);
            }

            return TypeNames;
        }

        private string CreateTypeName(Rule oRule)
        {
            ++m_UniqueID;

            string sTypeName = String.Format(
                "{0}_{1}",
                STRONG_TYPED_CLASSES_INITIALS,
                m_UniqueID);

            return sTypeName;
        }

        private string GetRuleFullTypeName(string sRuleTypeName)
        {
            return String.Format(
                "{0}.{1}",
                STRONG_TYPED_CLASSES_NAMESPACE,
                sRuleTypeName);
        }

        #endregion

        #region Rule->CodeDom

        private CodeTypeDeclaration CreateRuleDomClass(
            Rule oRule,
            string sUniqueRuleName)
        {
            CodeTypeDeclaration oDomClass =
                new CodeTypeDeclaration(sUniqueRuleName);

            oDomClass.IsClass = true;

            // <ClassName> : <ObjectType>,....
            // Since we want to serialize the protected members as well, we need to derive
            //	from the class we would like to serialize, to have access to the 
            //	memebers - note that private memebers can not be accessed using this
            //	logic.
            oDomClass.BaseTypes.Add(typeof(StrongTypeRulesBase));

            oDomClass.Attributes = MemberAttributes.Public;

            #region Add Ctor

            // Add the constructor

            CodeConstructor ObjectConstructor = new CodeConstructor();
            ObjectConstructor.Attributes = MemberAttributes.Public;
            ObjectConstructor.Parameters.Add(new CodeParameterDeclarationExpression("System.Object", "UserDefinedFunctions"));
            CodeArgumentReferenceExpression ObjectArgument = new CodeArgumentReferenceExpression("UserDefinedFunctions");
            ObjectConstructor.BaseConstructorArgs.Add(ObjectArgument);


            oDomClass.Members.Add(ObjectConstructor);

            #endregion

            #region Add properties defined in m_GlobalVariablesSchema and getters

            // scan each meta tag in the collection
            foreach (VariableDeclaration oVariableDeclaration in m_GlobalVariablesSchema)
            {
                // add reference to the assembly that contains that meta tag
                // TODO: add only assemblies that does not occure more than once
                Type VariableType = oVariableDeclaration.Type;
                string VariableName = oVariableDeclaration.Name;

                string VariableFieldName = "m_" + VariableName;

                // Add a private field named m_..... (oVariableDeclaration name)
                CodeMemberField VariableFields = new CodeMemberField(VariableType.ToString(), VariableFieldName);
                VariableFields.Attributes = MemberAttributes.Private;
                oDomClass.Members.Add(VariableFields);

                CodeMemberProperty VariableProperty = new CodeMemberProperty();
                // Since we would like that the user function will access the meta tags
                //	values by their names, we set the property name as the name of the meta tag
                VariableProperty.Name = VariableName;
                VariableProperty.Attributes = MemberAttributes.Public;
                VariableProperty.HasGet = true;
                VariableProperty.HasSet = false;
                VariableProperty.Type = new CodeTypeReference(VariableType);

                // add the getter for the specific field
                CodeFieldReferenceExpression oRefExpression = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), VariableFieldName);

                if (VariableType == typeof(string))
                {
                    CodeMethodInvokeExpression IsStringEmptyExpression = new CodeMethodInvokeExpression(
                        oRefExpression,
                        "Equals",
                        new CodeExpression[] { new CodePrimitiveExpression(String.Empty) });

                    CodeMethodReturnStatement oReturnTrue =
                        new CodeMethodReturnStatement(new CodePrimitiveExpression(null));

                    CodeMethodReturnStatement oReturnFalse =
                        new CodeMethodReturnStatement(oRefExpression);

                    CodeConditionStatement IfStringEmpty = new CodeConditionStatement(
                        // The condition to test.
                        IsStringEmptyExpression,
                        // The statements to execute if the condition evaluates to true.
                        new CodeStatement[] { oReturnTrue },
                        // The statements to execute if the condition evalues to false.
                        new CodeStatement[] { oReturnFalse });

                    // add the getter for the specific field
                    VariableProperty.GetStatements.Add(IfStringEmpty);
                }
                else
                {
                    VariableProperty.GetStatements.Add(new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), VariableFieldName)));
                }


                oDomClass.Members.Add(VariableProperty);

            }

            #endregion

            #region Add the specific user functions as defined in m_PredefinedFunctions

            MethodInfo[] PredefinedFunctions = m_FunctionsReflector.GetMethodsInfo();

            foreach (MethodInfo mi in PredefinedFunctions)
            {
                CodeMemberMethod OriginInvoker = new CodeMemberMethod();
                OriginInvoker.Name = mi.Name;
                OriginInvoker.Attributes = MemberAttributes.Private;
                OriginInvoker.ReturnType = new CodeTypeReference(mi.ReturnType);


                ParameterInfo[] Params = mi.GetParameters();

                int index = 0;

                // Array of CodeExpression
                ArrayBuilder oCodeInvokeParams = new ArrayBuilder();

                oCodeInvokeParams.Add(new CodePrimitiveExpression(mi.Name));

                // Each functions is allowed to have maximum of one VariableValueList
                //	parameters
                int nVariableValuesFunctionParameters = 0;

                foreach (ParameterInfo pi in Params)
                {
                    Type ParameterType = pi.ParameterType;

                    String OriginalParameterName = pi.Name;
                    String ParameterName = pi.Name;

                    ++index;

                    // in the created method, ignore the VariableValueList, which is 
                    //	contained as a memebr of the object
                    if (ParameterType != typeof(VariableValueList))
                    {
                        oCodeInvokeParams.Add(new CodeVariableReferenceExpression(ParameterName));

                        CodeParameterDeclarationExpression OriginalParameter = new
                            CodeParameterDeclarationExpression(ParameterType.ToString(), pi.Name);

                        OriginInvoker.Parameters.Add(OriginalParameter);
                    }
                    else
                        ++nVariableValuesFunctionParameters;
                }

                // ... more than single VariableValueList parameter
                if (nVariableValuesFunctionParameters > 1)
                {
                    string ExceptionMessage = "Error! StringTypedRule:StrongTypedRuleBuilder method " +
                        "m_PredefinedFunctions:" + mi.Name + " contains more than single" +
                        " parameter of type VariableValueList ";

                    throw new ArgumentException(ExceptionMessage, mi.Name);
                }

                oDomClass.Members.Add(OriginInvoker);

                CodeMethodInvokeExpression UserDefinedInvoke = new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "InvokeUserDefinedFunction",
                    (CodeExpression[])oCodeInvokeParams.GetArray());

                //					new CodeExpression[] {	OriginalFunctionParametersName } );

                // if the return type of the function is not void, than a "return" 
                //	clause should be added before the invocation of the function
                if (mi.ReturnType != typeof(void))
                {
                    // return ...
                    CodeMethodReturnStatement ReturnStatement = new CodeMethodReturnStatement(UserDefinedInvoke);
                    OriginInvoker.Statements.Add(ReturnStatement);
                }
                else
                    OriginInvoker.Statements.Add(UserDefinedInvoke);


            } // end of foreach( MethodInfo mi

            #endregion

            #region Add the RuleTrue override of the virtual function that actually computes the rule

            CodeMemberMethod RuleTrueMethod = new CodeMemberMethod();
            RuleTrueMethod.Name = "RuleTrue";
            RuleTrueMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            RuleTrueMethod.ReturnType = new CodeTypeReference(typeof(System.Boolean));

            // ... ( VariableValueList RUN_TRUE_METHOD_PARAMETER_NAME )
            CodeParameterDeclarationExpression RuleTrueParameter = new
                CodeParameterDeclarationExpression(typeof(VariableValueList).ToString(),
                RUN_TRUE_METHOD_PARAMETER_NAME);

            RuleTrueMethod.Parameters.Add(RuleTrueParameter);

            // base.RuleTrue( RUN_TRUE_METHOD_PARAMETER_NAME )
            CodeMethodInvokeExpression InvokeBaseRuleTrue = new CodeMethodInvokeExpression(
                new CodeBaseReferenceExpression(),
                // Method name and method parameter arguments
                "RuleTrue",
                new CodeExpression[] { new CodeArgumentReferenceExpression(RUN_TRUE_METHOD_PARAMETER_NAME) });


            RuleTrueMethod.Statements.Add(InvokeBaseRuleTrue);


            // Add the IfExists check on the VariableValueList, and if exists assign it to the
            //	relevant private meta tag
            // if( m_TiSVariableValues.Exists("VariableName") == true )
            //	m_VariableName = <VariableValueType>( m_TiSVariableValues.GetValue("VariableName") )
            foreach (VariableDeclaration oVariableDeclaration in m_GlobalVariablesSchema)
            {
                string VariableName = oVariableDeclaration.Name;
                Type VariableType = oVariableDeclaration.Type;

                // m_TiSVariableValues.Contains("VariableName") 
                CodeMethodInvokeExpression ExistVariable = new CodeMethodInvokeExpression(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                    "m_TiSVariableValues"),
                    "Contains",
                    new CodeExpression[] { new CodePrimitiveExpression(VariableName) });

               
                 //m_TiSVariableValues["VariableName"]
                CodeIndexerExpression VariableGetValue = new CodeIndexerExpression(new
                    CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "m_TiSVariableValues"),
                    new CodePrimitiveExpression(VariableName));
                // m_TiSVariableValues.GetValue("VariableName")
                //CodeMethodInvokeExpression VariableGetValue = new CodeMethodInvokeExpression(
                //    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                //    "m_TiSVariableValues"),
                //    "GetValue",
                //    new CodeExpression[] { new CodePrimitiveExpression(VariableName) });

                // <VariableValueType>
                CodeCastExpression VariableGetSpecificValue = new CodeCastExpression(
                    VariableType,
                    VariableGetValue);

                string PrivateFieldName = "m_" + VariableName;

                // m_VariableName = <Me ...
                CodeAssignStatement VariableAssignment = new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                    PrivateFieldName),
                    VariableGetSpecificValue);

                // if ...
                CodeConditionStatement CheckVariableValueExistence = new CodeConditionStatement(
                    // The condition to test.
                    ExistVariable,
                    // The statements to execute if the condition evaluates to true.
                    new CodeStatement[] { VariableAssignment },
                    // The statements to execute if the condition evalues to false.
                    new CodeStatement[] { });

                RuleTrueMethod.Statements.Add(CheckVariableValueExistence);

            }

            string BracketsWrapped = "( " + oRule.Text + " ) ";

            CodeSnippetExpression ActualUserRule = new CodeSnippetExpression(BracketsWrapped);

            CodeMethodReturnStatement ReturnedRule = new CodeMethodReturnStatement(ActualUserRule);

            // return ( User Function )
            RuleTrueMethod.Statements.Add(ReturnedRule);

            oDomClass.Members.Add(RuleTrueMethod);

            #endregion


            return oDomClass;
        }

        #endregion

        #region GetRule(s)FromAssembly

        private StrongTypeRulesBase GetRuleFromAssembly(
            Assembly oAssembly,
            string sRuleTypeName)
        {
            string sClassTypeFullName = GetRuleFullTypeName(sRuleTypeName);

            Type oRuleType = oAssembly.GetType(sClassTypeFullName);

            if (oRuleType == null)
            {
                throw new TisException(
                    "Error! can not get type [{0}] from compiled assembly [{1}]",
                    sClassTypeFullName,
                    oAssembly.CodeBase);
            }

            object[] Args = { m_PredefinedFunctions };

            StrongTypeRulesBase oRuleObj = (StrongTypeRulesBase)(Activator.CreateInstance(
                oRuleType,
                Args));

            return oRuleObj;
        }

        private StrongTypeRulesBase[] GetRulesFromAssembly(
            Assembly oAssembly,
            string[] RulesTypeNames)
        {
            StrongTypeRulesBase[] RuleObjects =
                new StrongTypeRulesBase[RulesTypeNames.Length];

            for (int i = 0; i < RulesTypeNames.Length; i++)
            {
                RuleObjects[i] = GetRuleFromAssembly(
                    oAssembly,
                    RulesTypeNames[i]);
            }

            return RuleObjects;
        }

        #endregion

        #region Utilities

        private string RulesToString(Rule[] Rules)
        {
            StringBuilder oRules = new StringBuilder();

            foreach (Rule oRule in Rules)
            {
                if (oRules.Length > 0)
                {
                    oRules.Append(",");
                }

                oRules.Append("[");
                oRules.Append(oRule.Text);
                oRules.Append("]");
            }

            return oRules.ToString();
        }

        #endregion

        //		private static string CreateTempName(string sExt)
        //		{
        //			Guid oGuid = Guid.NewGuid();
        //
        //			return String.Format(
        //				"{0}.{1}",
        //				HexUtils.ToHexString(oGuid.ToByteArray()),
        //				sExt
        //				);
        //		}

    } // end of CreateNewRule
}
