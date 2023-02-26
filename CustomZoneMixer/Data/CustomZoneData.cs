using ColossalFramework;
using ColossalFramework.Globalization;
using CustomZoneMixer.Localization;
using CustomZoneMixer.Overrides;
using ICities;
using Kwytto.Data;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static Kwytto.LiteUI.KwyttoDialog;

namespace CustomZoneMixer.Data
{

    public class CustomZoneData : DataExtensionBase<CustomZoneData>
    {
        public static event Action EventAllChanged;
        public static event Action<int> EventOneChanged;
        internal static readonly Func<Locale, Dictionary<Locale.Key, string>> m_localeStringsDictionary = ReflectionUtils.GetGetFieldDelegate<Locale, Dictionary<Locale.Key, string>>(typeof(Locale).GetField("m_LocalizedStrings", RedirectorUtils.allFlags));
        internal static readonly Func<LocaleManager, Locale> m_localeManagerLocale = ReflectionUtils.GetGetFieldDelegate<LocaleManager, Locale>(typeof(LocaleManager).GetField("m_Locale", RedirectorUtils.allFlags));

        private ZoneItem m_z1 = new ZoneItem(1, 0b00010100);
        private ZoneItem m_z2 = new ZoneItem(2, 0b00101000);
        private ZoneItem m_z3 = new ZoneItem(3, 0b00001100);
        private ZoneItem m_z4 = new ZoneItem(4, 0b00110000);
        private ZoneItem m_z5 = new ZoneItem(5, 0b10110000);
        private ZoneItem m_z6 = new ZoneItem(6, 0b11000000);
        private ZoneItem m_z7 = new ZoneItem(7, 0b01110000);

        public void Reset()
        {
            Z1 = new ZoneItem(1, 0b00010100);
            Z2 = new ZoneItem(2, 0b00101000);
            Z3 = new ZoneItem(3, 0b00001100);
            Z4 = new ZoneItem(4, 0b00110000);
            Z5 = new ZoneItem(5, 0b10110000);
            Z6 = new ZoneItem(6, 0b11000000);
            Z7 = new ZoneItem(7, 0b01110000);
            EventAllChanged?.Invoke();
        }

        public void SaveAsDefault() => File.WriteAllBytes(CZMController.DEFAULT_CONFIG_FILE, Serialize());
        public override CustomZoneData LoadDefaults(ISerializableData serializableData)
        {
            if (File.Exists(CZMController.DEFAULT_CONFIG_FILE))
            {
                try
                {
                    var result = new CustomZoneData();
                    if (result.Deserialize(typeof(CustomZoneData), File.ReadAllBytes(CZMController.DEFAULT_CONFIG_FILE)) is CustomZoneData defaultData)
                    {
                        Z1 = defaultData.Z1;
                        Z2 = defaultData.Z2;
                        Z3 = defaultData.Z3;
                        Z4 = defaultData.Z4;
                        Z5 = defaultData.Z5;
                        Z6 = defaultData.Z6;
                        Z7 = defaultData.Z7;
                        EventAllChanged?.Invoke();
                        return result;
                    }
                }
                catch (Exception e)
                {
                    LogUtils.DoErrorLog($"EXCEPTION WHILE LOADING: {e.GetType()} - {e.Message}\n {e.StackTrace}");
                    var message = string.Format(Str.ZM_ERROR_LOADING_DEFAULTS_MESSAGE, CZMController.DEFAULT_CONFIG_FILE, e.GetType(), e.Message, e.StackTrace);
                    KwyttoDialog.ShowModal(new BindProperties()
                    {
                        title = Str.ZM_ERROR_LOADING_DEFAULTS_TITLE,
                        message = message,
                        buttons = new ButtonDefinition[]
                        {
                            new ButtonDefinition()
                            {
                                title= Str.czm_copyToClipboard,
                                onClick= ()=>{
                                    Clipboard.text = message;
                                    return false;
                                }
                            },
                            new ButtonDefinition()
                            {
                                title= Str.ZM_OPEN_FOLDER_ON_EXPLORER_BUTTON,
                                onClick= () =>
                                {
                                    ColossalFramework.Utils.OpenInFileBrowser(CZMController.FOLDER_PATH);
                                    return false;
                                }
                            },
                            new ButtonDefinition()
                            {
                                title= Str.ZM_GO_TO_MOD_PAGE_BUTTON,
                                onClick= () =>
                                {
                                    ColossalFramework.Utils.OpenUrlThreaded("https://steamcommunity.com/sharedfiles/filedetails/?id=" + ModInstance.ModId);
                                    return false;
                                }
                            },
                            new ButtonDefinition()
                            {
                                title= Str.ZM_OK_BUTTON,
                                onClick= ()=>true,
                                style = ButtonStyle.White
                            },
                        }
                    });

                }
            }
            return null;
        }

        [XmlElement] public ZoneItem Z1 { get => m_z1; set => SetZ(ref m_z1, value ?? m_z1); }
        [XmlElement] public ZoneItem Z2 { get => m_z2; set => SetZ(ref m_z2, value ?? m_z2); }
        [XmlElement] public ZoneItem Z3 { get => m_z3; set => SetZ(ref m_z3, value ?? m_z3); }
        [XmlElement] public ZoneItem Z4 { get => m_z4; set => SetZ(ref m_z4, value ?? m_z4); }
        [XmlElement] public ZoneItem Z5 { get => m_z5; set => SetZ(ref m_z5, value ?? m_z5); }
        [XmlElement] public ZoneItem Z6 { get => m_z6; set => SetZ(ref m_z6, value ?? m_z6); }
        [XmlElement] public ZoneItem Z7 { get => m_z7; set => SetZ(ref m_z7, value ?? m_z7); }

        public ZoneItem this[int idx]
        {
            get
            {
                switch (idx & 7)
                {
                    case 1: return Z1;
                    case 2: return Z2;
                    case 3: return Z3;
                    case 4: return Z4;
                    case 5: return Z5;
                    case 6: return Z6;
                    case 7: return Z7;
                    default: return default;
                }
            }
        }

        public ZoneItem this[ItemClass.Zone idx] => this[(int)(idx - 7)];

        public override string SaveId => "K45_ZM_CustomZoneData";

        private void SetZ(ref ZoneItem field, ZoneItem value)
        {
            field = value;
            field.UpdateZoneName();
            field.UpdateZoneConfig();
        }
        public class ZoneItem
        {
            public ZoneItem() { }
            public ZoneItem(int zoneNumber, byte config)
            {
                m_zoneNumber = zoneNumber;
                ZoneConfig = config;
                ZoneName = null;
            }

            [XmlAttribute("zoneNumber")]
            public int m_zoneNumber;

            private string m_zoneName;
            private byte m_zoneConfig;

            [XmlAttribute("name")]
            public string ZoneName
            {
                get => m_zoneName.IsNullOrWhiteSpace() ? string.Format(Str.ZM_DEFAULT_ZONE_TITLE, m_zoneNumber) : m_zoneName;

                set
                {
                    m_zoneName = value;
                    UpdateZoneName();
                    EventOneChanged?.Invoke(m_zoneNumber);
                }
            }


            private static void SetLocaleEntry(Locale.Key key, string value) => m_localeStringsDictionary(m_localeManagerLocale(LocaleManager.instance))[key] = value;

            public void UpdateZoneName()
            { 
                SetLocaleEntry(new ColossalFramework.Globalization.Locale.Key
                {
                    m_Identifier = "ZONING_TITLE",
                    m_Key = "Z" + m_zoneNumber
                }, m_zoneName.IsNullOrWhiteSpace() ? string.Format(Str.ZM_DEFAULT_ZONE_TITLE, m_zoneNumber) : m_zoneName);
            }

            [XmlAttribute("zoneConfig")]
            public byte ZoneConfig
            {
                get => m_zoneConfig;
                set
                {
                    m_zoneConfig = value;
                    UpdateZoneConfig();
                    EventOneChanged?.Invoke(m_zoneNumber);
                }
            }

            public void UpdateZoneConfig()
            {
                SetLocaleEntry(new ColossalFramework.Globalization.Locale.Key
                {
                    m_Identifier = "ZONING_DESC",
                    m_Key = "Z" + m_zoneNumber
                }, string.Format(Str.ResourceManager.GetString("ZM_ZONE_DESC_Z" + m_zoneNumber, Str.Culture) + GetGenerationString()));
            }

            public void AddToZone(ItemClass.Zone zone) => ZoneConfig |= (byte)(1 << (int)zone);
            public void RemoveFromZone(ItemClass.Zone zone) => ZoneConfig &= (byte)~(1 << (int)zone);
            public bool HasZone(ItemClass.Zone zone) => (ZoneConfig & (1 << (int)zone)) != 0;

            private string GetGenerationString() => string.Join("\n", CustomZoneMixerOverrides.ZONES_TO_CHECK.Where(x => HasZone(x)).Select(x => $"\t• {Locale.Get("ZONEDBUILDING_TITLE", x.ToString())}").ToArray());

            public ItemClass.Zone GetLowerestZone()
            {
                int i = 2;
                int j = m_zoneConfig >> 2;
                while (j > 0 && (j & 1) == 0)
                {
                    i++;
                    j >>= 1;
                }
                return (ItemClass.Zone)i;
            }
        }
    }

}
