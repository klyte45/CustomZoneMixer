﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CustomZoneMixer.Localization {
    using System;
    
    
    /// <summary>
    ///   Uma classe de recurso de tipo de alta segurança, para pesquisar cadeias de caracteres localizadas etc.
    /// </summary>
    // Essa classe foi gerada automaticamente pela classe StronglyTypedResourceBuilder
    // através de uma ferramenta como ResGen ou Visual Studio.
    // Para adicionar ou remover um associado, edite o arquivo .ResX e execute ResGen novamente
    // com a opção /str, ou recrie o projeto do VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Str {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Str() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CustomZoneMixer.Localization.Str", typeof(Str).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Substitui a propriedade CurrentUICulture do thread atual para todas as
        ///   pesquisas de recursos que usam essa classe de recurso de tipo de alta segurança.
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
        ///   Consulta uma cadeia de caracteres localizada semelhante a Copy to clipboard.
        /// </summary>
        internal static string czm_copyToClipboard {
            get {
                return ResourceManager.GetString("czm_copyToClipboard", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Demand at district.
        /// </summary>
        internal static string czm_demandAtDistrict {
            get {
                return ResourceManager.GetString("czm_demandAtDistrict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Margin to consider as same
        ///(all zones with &lt;color=yellow&gt;density &gt; max - margin&lt;/color&gt; will be randomized).
        /// </summary>
        internal static string czm_thresoldConsiderEquals {
            get {
                return ResourceManager.GetString("czm_thresoldConsiderEquals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Minimum demand to use High Commercial
        ///(if both Com. densities selected).
        /// </summary>
        internal static string czm_thresoldForComHigh {
            get {
                return ResourceManager.GetString("czm_thresoldForComHigh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Minimum demand to use High Residential
        ///(if both Res. densities selected).
        /// </summary>
        internal static string czm_thresoldForResHigh {
            get {
                return ResourceManager.GetString("czm_thresoldForResHigh", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Adds 7 extra zones to the game, that can be customizable..
        /// </summary>
        internal static string root_modDescription {
            get {
                return ResourceManager.GetString("root_modDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Zone #{0}.
        /// </summary>
        internal static string ZM_DEFAULT_ZONE_TITLE {
            get {
                return ResourceManager.GetString("ZM_DEFAULT_ZONE_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a The file with default configurations (at &lt;color=#FFFF00&gt;{0}&lt;/color&gt;) seems to be corrupted. This error was thrown while trying to load it:
        ///
        ///&lt;color=#FF0000&gt;{1} - {2}
        ///
        ///{3}&lt;/color&gt;
        ///
        ///If you think this is a mod problem, please take a print of this window and post it at mod page..
        /// </summary>
        internal static string ZM_ERROR_LOADING_DEFAULTS_MESSAGE {
            get {
                return ResourceManager.GetString("ZM_ERROR_LOADING_DEFAULTS_MESSAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Error loading defaults.
        /// </summary>
        internal static string ZM_ERROR_LOADING_DEFAULTS_TITLE {
            get {
                return ResourceManager.GetString("ZM_ERROR_LOADING_DEFAULTS_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Export as Default.
        /// </summary>
        internal static string ZM_EXPORT_DEFAULT_BTN {
            get {
                return ResourceManager.GetString("ZM_EXPORT_DEFAULT_BTN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a All extra zones were successfully removed from this city. Now, it&apos;s safe to save this city and play it without this mod. Rules applied:
        ///
        ///&lt;color=#FFFF00&gt;
        ///-All zones with a building were painted with the building zone default type
        ///-All marked but not built zones were marked as one of their allowed zones in precedence order Res -&gt; Com -&gt; Ind -&gt; Off (low density first too)&lt;/color&gt;
        ///
        ///
        ///
        ///&lt;color=#FF0000&gt;NOTE THAT IS NOT SAFE TO CONTINUE PLAYING THIS CITY BEFORE SAVING IT, DISABLING THIS MOD, REBOOTING THE GAME A [o restante da cadeia de caracteres foi truncado]&quot;;.
        /// </summary>
        internal static string ZM_GHOST_MODE_MODAL_MESSAGE {
            get {
                return ResourceManager.GetString("ZM_GHOST_MODE_MODAL_MESSAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Removal applied!.
        /// </summary>
        internal static string ZM_GHOST_MODE_MODAL_TITLE {
            get {
                return ResourceManager.GetString("ZM_GHOST_MODE_MODAL_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Clean this mod from a savegame. (See tooltip here!).
        /// </summary>
        internal static string ZM_GHOST_MODE_OPTION {
            get {
                return ResourceManager.GetString("ZM_GHOST_MODE_OPTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Load a city once with this enabled to be able to play that city without this mod.
        ///After loading, you can save the city, close the game, disable this mod and load the savegame again to be safe for playing.
        /// </summary>
        internal static string ZM_GHOST_MODE_OPTION_TOOLTIP {
            get {
                return ResourceManager.GetString("ZM_GHOST_MODE_OPTION_TOOLTIP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Go to mod page at Workshop.
        /// </summary>
        internal static string ZM_GO_TO_MOD_PAGE_BUTTON {
            get {
                return ResourceManager.GetString("ZM_GO_TO_MOD_PAGE_BUTTON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Import Defaults.
        /// </summary>
        internal static string ZM_IMPORT_DEFAULT_BTN {
            get {
                return ResourceManager.GetString("ZM_IMPORT_DEFAULT_BTN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Okay....
        /// </summary>
        internal static string ZM_OK_BUTTON {
            get {
                return ResourceManager.GetString("ZM_OK_BUTTON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Open folder in explorer.
        /// </summary>
        internal static string ZM_OPEN_FOLDER_ON_EXPLORER_BUTTON {
            get {
                return ResourceManager.GetString("ZM_OPEN_FOLDER_ON_EXPLORER_BUTTON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Use mod defaults.
        /// </summary>
        internal static string ZM_RESET_BTN {
            get {
                return ResourceManager.GetString("ZM_RESET_BTN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a First configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z1 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Second configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z2 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Third configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z3 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Fourth configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z4 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Fifth configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z5 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Sixth configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z6 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z6", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Seventh configurable zone from Custom Zone Mixer Mod.
        ///Currently supports:
        ///.
        /// </summary>
        internal static string ZM_ZONE_DESC_Z7 {
            get {
                return ResourceManager.GetString("ZM_ZONE_DESC_Z7", resourceCulture);
            }
        }
    }
}
