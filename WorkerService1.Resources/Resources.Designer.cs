﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkerService1.Resources {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WorkerService1.Resources.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to appSettings.
        /// </summary>
        public static string ConfigKeyAppSettings {
            get {
                return ResourceManager.GetString("ConfigKeyAppSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to powerTradePublishingDirectory.
        /// </summary>
        public static string ConfigKeyPowerTradePublishingDirectory {
            get {
                return ResourceManager.GetString("ConfigKeyPowerTradePublishingDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to powerTradePublishingIntervalMinutes.
        /// </summary>
        public static string ConfigKeyPowerTradePublishingIntervalMinutes {
            get {
                return ResourceManager.GetString("ConfigKeyPowerTradePublishingIntervalMinutes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error initialising service.
        /// </summary>
        public static string LogMessageInitialisationError {
            get {
                return ResourceManager.GetString("LogMessageInitialisationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value for configuration setting: powerTradePublishingDirectory.
        /// </summary>
        public static string LogMessageInvalidPublishDirectory {
            get {
                return ResourceManager.GetString("LogMessageInvalidPublishDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value for configuration setting: powerTradePublishingIntervalMinutes.
        /// </summary>
        public static string LogMessageInvalidPublishInterval {
            get {
                return ResourceManager.GetString("LogMessageInvalidPublishInterval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error retrieving power trades..
        /// </summary>
        public static string LogMessagePowerServiceError {
            get {
                return ResourceManager.GetString("LogMessagePowerServiceError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PowerTrades.
        /// </summary>
        public static string ServiceName {
            get {
                return ResourceManager.GetString("ServiceName", resourceCulture);
            }
        }
    }
}