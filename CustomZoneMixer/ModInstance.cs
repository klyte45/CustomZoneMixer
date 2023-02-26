using CustomZoneMixer.Localization;
using Kwytto.Interfaces;
using System.Globalization;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("2.0.0.*")]
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
    }
}
