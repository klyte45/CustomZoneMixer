using ColossalFramework.UI;
using CustomZoneMixer.Localization;
using CustomZoneMixer.Overrides;
using Kwytto.Interfaces;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.0.0")]
namespace CustomZoneMixer
{
    public class ModInstance : BasicIUserMod<ModInstance, CZMController>
    {
        public override string SimpleName => "Custom Zone Mixer";

        public override string SafeName => "CustomZoneMixer"; 

        public override string Acronym => "CZM";
         
        public override Color ModColor => new Color32(0x66, 0x26, 0x24, 0xFF);

        public override string Description => Str.root_modDescription;
        protected override void SetLocaleCulture(CultureInfo culture)
        {
            Str.Culture = culture;
        }

        private IUUIButtonContainerPlaceholder[] cachedUUI;
        public override IUUIButtonContainerPlaceholder[] UUIButtons => cachedUUI ?? (cachedUUI = new IUUIButtonContainerPlaceholder[]
        {
            new UUIWindowButtonContainerPlaceholder(
             buttonName: $"{SimpleName}",
             tooltip: $"{SimpleName}",
             iconPath: "ModIcon_btn",
             windowGetter: ()=>CZMGUI.Instance,
             shallShowButton: ()=>true
             ),
        }.Where(x => x != null).ToArray());

        public override void TopSettingsUI(UIHelper ext)
        {
            base.TopSettingsUI(ext);
            var ghostModeChk = ext.AddCheckbox(Str.ZM_GHOST_MODE_OPTION, CZMController.m_ghostMode, (x) =>
            {
                CZMController.m_ghostMode = x;
                CustomZoneMixerOverrides.FixZonePanel();
            }) as UICheckBox;
            ghostModeChk.tooltip = Str.ZM_GHOST_MODE_OPTION_TOOLTIP;
            if (SimulationManager.exists && SimulationManager.instance.m_metaData != null)
            {
                ghostModeChk.Disable();
            }
        }
    }
}
