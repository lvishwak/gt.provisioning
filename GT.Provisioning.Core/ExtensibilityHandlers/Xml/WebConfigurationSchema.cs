﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1586.0.
// 
namespace GT.Provisioning.Core.ExtensibilityHandlers.XML {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class WebProviderConfiguration {
        
        private WebProviderConfigurationWeb[] websField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("web", IsNullable=false)]
        public WebProviderConfigurationWeb[] webs {
            get {
                return this.websField;
            }
            set {
                this.websField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class WebProviderConfigurationWeb {
        
        private WebProviderConfigurationWebRoleAssignment[] roleAssignmentsField;
        
        private int languageField;
        
        private string titleField;
        
        private string descriptionField;
        
        private string urlField;
        
        private bool useSamePermissionsAsParentSiteField;
        
        private bool inheritNavigationField;
        
        private string baseTemplateField;
        
        private string pnPTemplateField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RoleAssignment", IsNullable=false)]
        public WebProviderConfigurationWebRoleAssignment[] RoleAssignments {
            get {
                return this.roleAssignmentsField;
            }
            set {
                this.roleAssignmentsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Language {
            get {
                return this.languageField;
            }
            set {
                this.languageField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Title {
            get {
                return this.titleField;
            }
            set {
                this.titleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Url {
            get {
                return this.urlField;
            }
            set {
                this.urlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool UseSamePermissionsAsParentSite {
            get {
                return this.useSamePermissionsAsParentSiteField;
            }
            set {
                this.useSamePermissionsAsParentSiteField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool InheritNavigation {
            get {
                return this.inheritNavigationField;
            }
            set {
                this.inheritNavigationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BaseTemplate {
            get {
                return this.baseTemplateField;
            }
            set {
                this.baseTemplateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string PnPTemplate {
            get {
                return this.pnPTemplateField;
            }
            set {
                this.pnPTemplateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class WebProviderConfigurationWebRoleAssignment {
        
        private string principalField;
        
        private string roleDefinitionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Principal {
            get {
                return this.principalField;
            }
            set {
                this.principalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RoleDefinition {
            get {
                return this.roleDefinitionField;
            }
            set {
                this.roleDefinitionField = value;
            }
        }
    }
}