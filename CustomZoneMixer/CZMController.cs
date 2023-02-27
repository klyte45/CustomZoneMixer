using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using CustomZoneMixer.Localization;
using CustomZoneMixer.Overrides;
using Kwytto.Interfaces;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ColossalFramework.UI.UITextureAtlas;
using static Kwytto.LiteUI.KwyttoDialog;
using Path = System.IO.Path;

namespace CustomZoneMixer
{
    public class CZMController : BaseController<ModInstance, CZMController>
    {
        public static bool m_ghostMode;

        private static bool m_dirtyZonePanel = false;

        private readonly List<GameObject> refGOs = new List<GameObject>();
        protected override void StartActions()
        {

            if (m_ghostMode)
            {

                for (ushort a = 1; a < BuildingManager.instance.m_buildings.m_buffer.Length; a++)
                {
                    if (BuildingManager.instance.m_buildings.m_buffer[a].Info.m_buildingAI is PrivateBuildingAI)
                    {
                        Vector3 position = BuildingManager.instance.m_buildings.m_buffer[a].m_position;
                        int num = Mathf.Max((int)((position.x - 35f) / 64f + 135f), 0);
                        int num2 = Mathf.Max((int)((position.z - 35f) / 64f + 135f), 0);
                        int num3 = Mathf.Min((int)((position.x + 35f) / 64f + 135f), 269);
                        int num4 = Mathf.Min((int)((position.z + 35f) / 64f + 135f), 269);
                        Array16<Building> buildings = Singleton<BuildingManager>.instance.m_buildings;
                        ushort[] buildingGrid = Singleton<BuildingManager>.instance.m_buildingGrid;
                        for (int i = num2; i <= num4; i++)
                        {
                            for (int j = num; j <= num3; j++)
                            {
                                ushort num5 = buildingGrid[i * 270 + j];
                                int num6 = 0;
                                while (num5 != 0)
                                {
                                    ushort nextGridBuilding = buildings.m_buffer[num5].m_nextGridBuilding;
                                    Building.Flags flags = buildings.m_buffer[num5].m_flags;
                                    if ((flags & (Building.Flags.Created | Building.Flags.Deleted | Building.Flags.Demolishing | Building.Flags.Collapsed)) == Building.Flags.Created)
                                    {
                                        BuildingInfo info = buildings.m_buffer[num5].Info;
                                        if (info != null && info.m_placementStyle == ItemClass.Placement.Automatic)
                                        {
                                            ItemClass.Zone zone = info.m_class.GetZone();
                                            ItemClass.Zone secondaryZone = info.m_class.GetSecondaryZone();
                                            if (zone != ItemClass.Zone.None && VectorUtils.LengthSqrXZ(buildings.m_buffer[num5].m_position - position) <= 1225f)
                                            {
                                                buildings.m_buffer[num5].CheckZoning(zone, secondaryZone, true);
                                            }
                                        }
                                    }
                                    num5 = nextGridBuilding;
                                    if (++num6 >= 49152)
                                    {
                                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }

                for (ushort i = 1; i < ZoneManager.instance.m_blocks.m_buffer.Length; i++)
                {
                    bool changed = false;
                    for (int x = 0; x < 4; x++)
                    {
                        for (int z = 0; z < 8; z++)
                        {
                            changed = CustomZoneMixerOverrides.GetBlockZoneSanitize(ref ZoneManager.instance.m_blocks.m_buffer[i], x, z) | changed;
                        }
                    }
                    if (changed) { ZoneManager.instance.m_blocks.m_buffer[i].RefreshZoning(i); }
                }

                KwyttoDialog.ShowModal(new BindProperties()
                {
                    title = Str.ZM_GHOST_MODE_MODAL_TITLE,
                    message = Str.ZM_GHOST_MODE_MODAL_MESSAGE,
                    buttons = KwyttoDialog.basicOkButtonBar
                });
            }
            else
            {
                if (Singleton<ZoneManager>.instance.m_properties.m_zoneColors.Length < 15)
                {
                    Singleton<ZoneManager>.instance.m_properties.m_zoneColors = Singleton<ZoneManager>.instance.m_properties.m_zoneColors.Take(8).Concat(new Color[]
                       {
                            ColorExtensions.FromRGB("FF0000"),//Z1
                            ColorExtensions.FromRGB("AA4444"),//Z2
                            ColorExtensions.FromRGB("FF00FF"),//Z3
                            ColorExtensions.FromRGB("AA44AA"),//Z4
                            ColorExtensions.FromRGB("0000FF"),//Z5
                            ColorExtensions.FromRGB("00FF00"),//Z6
                            ColorExtensions.FromRGB("FF8800"),//Z7
                       }).ToArray();
                }
                if (Singleton<ZoneManager>.instance.m_zoneNotUsed.Length < 15)
                {
                    Singleton<ZoneManager>.instance.m_zoneNotUsed = Singleton<ZoneManager>.instance.m_zoneNotUsed.Take(8).Concat(new ZoneTypeGuide[]
                       {
                            new ZoneTypeGuide(),//Z1
                            new ZoneTypeGuide(),//Z2
                            new ZoneTypeGuide(),//Z3
                            new ZoneTypeGuide(),//Z4
                            new ZoneTypeGuide(),//Z5
                            new ZoneTypeGuide(),//Z6
                            new ZoneTypeGuide(),//Z7
                       }).ToArray();
                }
                var newSprites = new List<SpriteInfo>();
                string[] imagesFiles = KFileUtils.GetAllFilesEmbeddedAtFolder("UI.Images", ".png");
                foreach (string file in imagesFiles)
                {
                    if (!file.StartsWith("%")) continue;
                    Texture2D tex = KResourceLoader.LoadTextureMod(Path.GetFileNameWithoutExtension(file));
                    if (tex != null)
                    {
                        newSprites.AddRange(TextureAtlasUtils.CreateSpriteInfo(new Dictionary<string, Tuple<RectOffset, bool>>(), file, tex));
                    }
                }
                LogUtils.DoLog($"ADDING {newSprites.Count} sprites!");
                TextureAtlasUtils.RegenerateDefaultTextureAtlas(newSprites);

                newSprites.Clear();
                imagesFiles = KFileUtils.GetAllFilesEmbeddedAtFolder("UI.Images.InfoTooltip", ".png");
                foreach (string file in imagesFiles)
                {
                    if (!file.StartsWith("%")) continue;
                    Texture2D tex = KResourceLoader.LoadTextureMod(Path.GetFileNameWithoutExtension(file), "Images.InfoTooltip");
                    if (tex != null)
                    {
                        newSprites.AddRange(TextureAtlasUtils.CreateSpriteInfo(new Dictionary<string, Tuple<RectOffset, bool>>(), file, tex));
                    }
                }
                LogUtils.DoLog($"ADDING {newSprites.Count} tooltip sprites!");
                TextureAtlasUtils.RegenerateTextureAtlas(UIView.GetAView().FindUIComponent<UIPanel>("InfoTooltip").GetComponentInChildren<UISprite>().atlas, newSprites);
                LogUtils.FlushBuffer();

                GameObject.FindObjectOfType<ZoningPanel>().RefreshPanel();

                refGOs.Add(GameObjectUtils.CreateElement<CZMGUI>(UIView.GetAView().gameObject.transform, "CZMGUI").gameObject);
            }
        }

        public void OnDestroy()
        {
            foreach (GameObject go in refGOs)
            {
                Destroy(go);
            }
        }

        public static void SetDirty() => m_dirtyZonePanel = true;

        private void FixedUpdate()
        {
            if (m_dirtyZonePanel)
            {
                GameObject.FindObjectOfType<ZoningPanel>()?.RefreshPanel();
                m_dirtyZonePanel = false;
            }
        }

        public static string FOLDER_PATH => ModInstance.ModSettingsRootFolder;
        public static readonly string DEFAULT_CONFIG_FILE = $"{FOLDER_PATH}{Path.DirectorySeparatorChar}DefaultConfiguration.xml";
    }
}
