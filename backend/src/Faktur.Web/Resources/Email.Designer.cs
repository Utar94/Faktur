﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Faktur.Web.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Email {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Email() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Faktur.Web.Resources.Email", typeof(Email).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cordially,.
        /// </summary>
        public static string Cordially {
            get {
                return ResourceManager.GetString("Cordially", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Logitar Team.
        /// </summary>
        public static string FakturTeam {
            get {
                return ResourceManager.GetString("FakturTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In this case, please click on the link below to reset it:.
        /// </summary>
        public static string PasswordRecovery_ClickLink {
            get {
                return ResourceManager.GetString("PasswordRecovery_ClickLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hello {name}!.
        /// </summary>
        public static string PasswordRecovery_Hello {
            get {
                return ResourceManager.GetString("PasswordRecovery_Hello", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If we&apos;ve been mistaken, we suggest you to delete this message..
        /// </summary>
        public static string PasswordRecovery_OtherwiseDelete {
            get {
                return ResourceManager.GetString("PasswordRecovery_OtherwiseDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It seems you have lost your password....
        /// </summary>
        public static string PasswordRecovery_PasswordLost {
            get {
                return ResourceManager.GetString("PasswordRecovery_PasswordLost", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reset your password.
        /// </summary>
        public static string PasswordRecovery_Subject {
            get {
                return ResourceManager.GetString("PasswordRecovery_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please click on the link below to activate your account:.
        /// </summary>
        public static string SignUp_ClickLink {
            get {
                return ResourceManager.GetString("SignUp_ClickLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Activate your account.
        /// </summary>
        public static string SignUp_Subject {
            get {
                return ResourceManager.GetString("SignUp_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Welcome to our application, {name}!.
        /// </summary>
        public static string SignUp_Welcome {
            get {
                return ResourceManager.GetString("SignUp_Welcome", resourceCulture);
            }
        }
    }
}