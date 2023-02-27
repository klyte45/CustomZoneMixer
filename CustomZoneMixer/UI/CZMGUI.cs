extern alias UUI;

using ColossalFramework.UI;
using CustomZoneMixer.Data;
using CustomZoneMixer.Localization;
using CustomZoneMixer.Overrides;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System.Linq;
using UnityEngine;

namespace CustomZoneMixer
{
    internal class CZMGUI : GUIRootWindowBase
    {
        public static CZMGUI Instance { get; private set; }
        protected override float FontSizeMultiplier => .9f;

        public void Awake()
        {
            Instance = this;
            Init($"{ModInstance.Instance.GeneralName}", new Rect(128, 128, 440, 610), resizable: false, minSize: new Vector2(440, 610), maxSize: new Vector2(440, 610));

            Visible = false;
        }
        protected override bool showOverModals => false;

        protected override bool requireModal => false;


        private UITextureAtlas refAtlas;

        private UITextureAtlas RefAtlas
        {
            get
            {
                if (refAtlas is null)
                {
                    refAtlas = FindObjectOfType<ZoningPanel>().GetComponentInChildren<UIScrollablePanel>().atlas;
                }
                return refAtlas;
            }
        }

        private string[] m_districtsLabel = new string[] { "<ALL>" };
        private byte[] m_districtsIds = new byte[] { 0 };
        private int m_currentDistrictSel = 0;

        private readonly Texture2D m_refresh = KResourceLoader.LoadTextureKwytto(Kwytto.UI.CommonsSpriteNames.K45_Reload);

        private GUIStyle m_centerLabel;

        private void InitStyles()
        {
            if (m_centerLabel is null)
            {
                m_centerLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            }
        }

        protected override void DrawWindow(Vector2 size)
        {
            InitStyles();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Str.ZM_EXPORT_DEFAULT_BTN)) CustomZoneData.Instance.SaveAsDefault();
                if (GUILayout.Button(Str.ZM_IMPORT_DEFAULT_BTN)) CustomZoneData.LoadDefaults();
                if (GUILayout.Button(Str.ZM_RESET_BTN)) CustomZoneData.Instance.Reset();
            }
            for (int idx = 1; idx <= 7; idx++)
            {
                var i = idx;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Button(UIView.GetAView().defaultAtlas["ZoningZ" + i].texture, GUI.skin.label, GUILayout.Height(30), GUILayout.Width(30));
                    CustomZoneData.Instance[i].ZoneName = GUITextField.TextField("K45_CZM_ZoneName" + i, CustomZoneData.Instance[i].ZoneName);

                    foreach (ItemClass.Zone zone in CustomZoneMixerOverrides.ZONES_TO_CHECK)
                    {
                        AddCheckboxZone(i, zone);
                    }
                }
            }
            GUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label(Str.czm_thresoldConsiderEquals, m_centerLabel);
                using (new GUILayout.HorizontalScope())
                {
                    if (GUIKwyttoCommons.DrawIntSlider(size.x, Str.czm_thresoldConsiderEquals, (int)CustomZoneData.Instance.SimilarityThresold, out var newThresold, 0, 50))
                    {
                        CustomZoneData.Instance.SimilarityThresold = (uint)newThresold;
                    }
                }
            }
            GUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label(Str.czm_thresoldForComHigh, m_centerLabel);
                using (new GUILayout.HorizontalScope())
                {
                    if (GUIKwyttoCommons.DrawIntSlider(size.x, Str.czm_thresoldForComHigh, (int)CustomZoneData.Instance.CommercialDensityElevationFactor, out var newThresold, 10, 90))
                    {
                        CustomZoneData.Instance.CommercialDensityElevationFactor = (uint)newThresold;
                    }
                }
            }
            GUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label(Str.czm_thresoldForResHigh, m_centerLabel);
                using (new GUILayout.HorizontalScope())
                {
                    if (GUIKwyttoCommons.DrawIntSlider(size.x, Str.czm_thresoldForResHigh, (int)CustomZoneData.Instance.ResidentialDensityElevationFactor, out var newThresold, 10, 90))
                    {
                        CustomZoneData.Instance.ResidentialDensityElevationFactor = (uint)newThresold;
                    }
                }
            }
            GUILayout.FlexibleSpace();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(Str.czm_demandAtDistrict);
                m_currentDistrictSel = GUIComboBox.Box(m_currentDistrictSel, m_districtsLabel, "K45_CZM_DistrictDemand", this);
                GUIKwyttoCommons.SquareTextureButton2(m_refresh, "S", () =>
                {
                    var districts = DistrictUtils.GetValidDistricts().OrderBy(x => x.Value == 0 ? "" : x.Key).ToList();
                    m_districtsLabel = districts.Select(x => x.Key).ToArray();
                    m_districtsIds = districts.Select(x => x.Value).ToArray();
                    m_currentDistrictSel = 0;
                }, size: 20);
            }
            using (new GUILayout.HorizontalScope())
            {
                foreach (ItemClass.Zone zone in CustomZoneMixerOverrides.ZONES_TO_CHECK)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Button(RefAtlas[$"Zoning{zone}{(enabled ? "" : "Disabled")}"].texture, m_centerLabel, GUILayout.Height(30), GUILayout.Width(30));
                            GUILayout.FlexibleSpace();
                        }
                        var z = zone;
                        GUILayout.Label("" + CustomZoneMixerOverrides.GetCurrentDemandFor(ref z, m_districtsIds[m_currentDistrictSel]), m_centerLabel);
                    }
                }
            }
        }

        private void AddCheckboxZone(int idx, ItemClass.Zone zone)
        {
            var enabled = CustomZoneData.Instance[idx].HasZone(zone);
            if (GUILayout.Button(RefAtlas[$"Zoning{zone}{(enabled ? "" : "Disabled")}"].texture, GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (enabled)
                {
                    CustomZoneData.Instance[idx].RemoveFromZone(zone);
                }
                else
                {
                    CustomZoneData.Instance[idx].AddToZone(zone);
                }
            }
        }

        protected override void OnWindowDestroyed()
        {
            Instance = null;
        }
    }
}
