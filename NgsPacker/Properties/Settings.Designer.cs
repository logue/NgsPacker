﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NgsPacker.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Language {
            get {
                return ((string)(this["Language"]));
            }
            set {
                this["Language"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool NotifyComplete {
            get {
                return ((bool)(this["NotifyComplete"]));
            }
            set {
                this["NotifyComplete"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".acb\r\n.bti\r\n.cml\r\n.cmp\r\n.crbp\r\n.crc\r\n.dds\r\n.emi\r\n.evt\r\n.fcl\r\n.fig\r\n.flte\r\n.ikn\r\n." +
            "ini\r\n.lat\r\n.light\r\n.lua\r\n.lve\r\n.mus\r\n.pgd\r\n.pha\r\n.phi\r\n.prm\r\n.rgn\r\n.sib\r\n.skit\r\n" +
            ".snd\r\n.trm\r\n.trn\r\n.txl\r\n.wdsn\r\n.wtr\r\noxyresource.crc")]
        public string WhiteList {
            get {
                return ((string)(this["WhiteList"]));
            }
            set {
                this["WhiteList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ThemeDark {
            get {
                return ((bool)(this["ThemeDark"]));
            }
            set {
                this["ThemeDark"] = value;
            }
        }
    }
}
