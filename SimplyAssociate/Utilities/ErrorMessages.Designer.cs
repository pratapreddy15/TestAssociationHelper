﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.SimplyAssociate.Utilities {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.SimplyAssociate.Utilities.ErrorMessages", typeof(ErrorMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The output file of project is not available at the project output locaiton. Please try building the project first..
        /// </summary>
        internal static string ACTIVEPROJECT_OUTPUTFILE_NOTFOUND {
            get {
                return ResourceManager.GetString("ACTIVEPROJECT_OUTPUTFILE_NOTFOUND", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find the active\\opened solution or error might have occurred while opening the solution..
        /// </summary>
        internal static string ACTIVESOLUTION_NOTAVAILABLE {
            get {
                return ResourceManager.GetString("ACTIVESOLUTION_NOTAVAILABLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot perform Build\Rebuild\Clean on Solution or Project because test associations are in progress. You must either stop the test associations or wait until all tests are associated..
        /// </summary>
        internal static string CANNOTBUILD_TESTASSOCIATIONS_INPROGRESS {
            get {
                return ResourceManager.GetString("CANNOTBUILD_TESTASSOCIATIONS_INPROGRESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot start associating test(s) not because solution build is in progress. Please wait until the building of solution is completed..
        /// </summary>
        internal static string CANNOTSTART_TESTASSOCIATION_BUILD_INPROGRESS {
            get {
                return ResourceManager.GetString("CANNOTSTART_TESTASSOCIATION_BUILD_INPROGRESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The test class has invalid attributes because it contains TestClass as well as CodedUITest. Please specify only one attribute for a single test class..
        /// </summary>
        internal static string CLASS_HAS_TESTCLASS_CODEDUITEST_ATTRIBUTES {
            get {
                return ResourceManager.GetString("CLASS_HAS_TESTCLASS_CODEDUITEST_ATTRIBUTES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The CodeElement COM object is not a valid function or class element. The passed CodeElement object is of kind {0}..
        /// </summary>
        internal static string CODEELEMENT_UNKNOWNKIND {
            get {
                return ResourceManager.GetString("CODEELEMENT_UNKNOWNKIND", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to read the test method(s) inside the test class. Please make sure the cursor is inside the test class..
        /// </summary>
        internal static string CURSOR_NOTINSIDE_TESTCLASS {
            get {
                return ResourceManager.GetString("CURSOR_NOTINSIDE_TESTCLASS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to get the test method at the cursor position. Please make sure you are trying to associate the method with TestMethod attribute..
        /// </summary>
        internal static string CURSOR_NOTINSIDE_TESTMETHOD {
            get {
                return ResourceManager.GetString("CURSOR_NOTINSIDE_TESTMETHOD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Visual Studio Core Automation wrapper [EnvDTE.DTE] is not initialized..
        /// </summary>
        internal static string ENVDTE_ISNULL {
            get {
                return ResourceManager.GetString("ENVDTE_ISNULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to build the test association data for associating test method {0} with test case having id {1}..
        /// </summary>
        internal static string NEW_TESTASSOCIATION_IS_NULL {
            get {
                return ResourceManager.GetString("NEW_TESTASSOCIATION_IS_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Team Project is not initialized..
        /// </summary>
        internal static string TEAMPROJECT_ISNULL {
            get {
                return ResourceManager.GetString("TEAMPROJECT_ISNULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not locate the team project at the mapped solution with path {0}..
        /// </summary>
        internal static string TEAMPROJECT_NOTFOUND_ATMAPPEDSOLUTION {
            get {
                return ResourceManager.GetString("TEAMPROJECT_NOTFOUND_ATMAPPEDSOLUTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection has {0} projects. The index {1} specified is greater than the count of projects contained..
        /// </summary>
        internal static string TESTPROJECTCOLLECTION_ISEMPTY {
            get {
                return ResourceManager.GetString("TESTPROJECTCOLLECTION_ISEMPTY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one test case found in TFS with Id {0}..
        /// </summary>
        internal static string TFS_MORETHANONE_TESTCASE_FOUND {
            get {
                return ResourceManager.GetString("TFS_MORETHANONE_TESTCASE_FOUND", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Work Item not found in TFS for Test Method. Please make sure that correct WorkItem Id attribute is applied to Test Method {0}..
        /// </summary>
        internal static string TFS_WORKITEM_NOTFOUND_FOR_TESTMETHOD {
            get {
                return ResourceManager.GetString("TFS_WORKITEM_NOTFOUND_FOR_TESTMETHOD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Visual Studio Extension Package is NULL..
        /// </summary>
        internal static string VS_EXTENSIONPACKAGE_ISNULL {
            get {
                return ResourceManager.GetString("VS_EXTENSIONPACKAGE_ISNULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The solution events are not initialized and is NULL..
        /// </summary>
        internal static string VS_SOLUTIONEVENTS_ISNULL {
            get {
                return ResourceManager.GetString("VS_SOLUTIONEVENTS_ISNULL", resourceCulture);
            }
        }
    }
}
