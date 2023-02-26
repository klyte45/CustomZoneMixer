using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CustomZoneMixer.Data;
using HarmonyLib;
using Kwytto.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static CustomZoneMixer.Data.CustomZoneData;
using static TerrainManager;

namespace CustomZoneMixer.Overrides
{

    public class CustomZoneMixerOverrides : Redirector, IRedirectable
    {

        public void Awake()
        {
            AddRedirect(typeof(UnlockManager).GetMethod("InitializeProperties"), null, GetType().GetMethod("AddZonesUnlockData"));
            AddRedirect(typeof(ZoneBlock).GetMethod("SimulationStep"), null, null, typeof(CustomZoneMixerOverrides).GetMethod("SimulationStepTranspiller", RedirectorUtils.allFlags));
            AddRedirect(typeof(ZoneBlock).GetMethod("CheckBlock", RedirectorUtils.allFlags), null, null, typeof(CustomZoneMixerOverrides).GetMethod("CheckBlockTranspiller", RedirectorUtils.allFlags));
            AddRedirect(typeof(TerrainPatch).GetMethod("Refresh", RedirectorUtils.allFlags), null, null, typeof(CustomZoneMixerOverrides).GetMethod("TranspilePatchRefresh", RedirectorUtils.allFlags));
            AddRedirect(typeof(Building).GetMethod("CheckZoning", RedirectorUtils.allFlags, null, new Type[] { typeof(ItemClass.Zone), typeof(ItemClass.Zone), typeof(uint).MakeByRefType(), typeof(bool).MakeByRefType(), typeof(ZoneBlock).MakeByRefType() }, null), null, null, typeof(CustomZoneMixerOverrides).GetMethod("TranspileCheckZoning", RedirectorUtils.allFlags));
            AddRedirect(typeof(GeneratedScrollPanel).GetMethod("SpawnEntry", RedirectorUtils.allFlags), typeof(CustomZoneMixerOverrides).GetMethod("GenPanelSpawnEntryPre", RedirectorUtils.allFlags));
            CustomZoneMixerOverrides.FixZonePanel();

            if (ModInstance.DebugMode)
            {
                AddRedirect(typeof(BuildingManager).GetMethod("ReleaseBuilding"), typeof(CustomZoneMixerOverrides).GetMethod("LogStacktrace"));
            }

            foreach (Type zoneType in Get81TilesFakeZoneBlockTypes())
            {
                LogUtils.DoWarnLog("Patching 81 tiles");
                AddRedirect(zoneType.GetMethod("SimulationStep"), null, null, typeof(CustomZoneMixerOverrides).GetMethod("SimulationStepTranspiller", RedirectorUtils.allFlags));
            }
            foreach (Type zoneType in GetBuildingThemesZoneBlockDetourTypes())
            {
                LogUtils.DoWarnLog("Patching BuildingThemes");
                AddRedirect(zoneType.GetMethod("SimulationStep"), null, null, typeof(CustomZoneMixerOverrides).GetMethod("SimulationStepTranspiller", RedirectorUtils.allFlags));
            }
        }

        public static List<Type> Get81TilesFakeZoneBlockTypes()
        {
            return Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
                pi.assemblyCount > 0
                && pi.GetAssemblies().Where(x => "EightyOne" == x.GetName().Name).Where(x => x.GetType("EightyOne.Zones.FakeZoneBlock") != null).Count() > 0
             ).SelectMany(pi => pi.GetAssemblies().Where(x => "EightyOne" == x.GetName().Name).Select(x => x.GetType("EightyOne.Zones.FakeZoneBlock"))).ToList();
        }
        public static List<Type> GetBuildingThemesZoneBlockDetourTypes()
        {
            return Singleton<PluginManager>.instance.GetPluginsInfo().Where((PluginManager.PluginInfo pi) =>
                pi.assemblyCount > 0
                && pi.GetAssemblies().Where(x => "BuildingThemes" == x.GetName().Name).Where(x => x.GetType("BuildingThemes.Detour.ZoneBlockDetour") != null).Count() > 0
             ).SelectMany(pi => pi.GetAssemblies().Where(x => "BuildingThemes" == x.GetName().Name).Select(x => x.GetType("BuildingThemes.Detour.ZoneBlockDetour"))).ToList();
        }

        public static void FixZonePanel()
        {

            var enumTypes = ColossalFramework.Utils.GetOrderedEnumData<ItemClass.Zone>().Where(x => (int)x.enumValue < 9 || (int)x.enumValue > 14).ToList();

            if (!CZMController.m_ghostMode)
            {
                enumTypes.AddRange(new List<PositionData<ItemClass.Zone>>(){
                        new PositionData<ItemClass.Zone>
                    {
                        index = 90,
                        enumName = "Z1",
                        enumValue = (ItemClass.Zone)8
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 91,
                        enumName = "Z2",
                        enumValue = (ItemClass.Zone)9
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 92,
                        enumName = "Z3",
                        enumValue = (ItemClass.Zone)10
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 93,
                        enumName = "Z4",
                        enumValue = (ItemClass.Zone)11
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 94,
                        enumName = "Z5",
                        enumValue = (ItemClass.Zone)12
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 95,
                        enumName = "Z6",
                        enumValue = (ItemClass.Zone)13
                    },
                     new PositionData<ItemClass.Zone>
                     {
                        index = 96,
                        enumName = "Z7",
                        enumValue = (ItemClass.Zone)14
                    }
                    });

            }
            typeof(ZoningPanel).GetField("kZones", RedirectorUtils.allFlags).SetValue(null, enumTypes.ToArray());           

        }
        public static void GenPanelSpawnEntryPre(ref string tooltip, ref string thumbnail, ref UITextureAtlas atlas, ref UIComponent tooltipBox)
        {
            if (thumbnail?.StartsWith("ZoningZ") ?? false)
            {
                atlas = TextureAtlasUtils.DefaultTextureAtlas;
                tooltip = tooltip?.Replace("&sprite|Z", "&sprite|ZoningZ");
                if (tooltipBox != null)
                {
                    tooltipBox.height = 320;
                }
            }
        }

        public static void LogStacktrace() => LogUtils.DoWarnLog($"RELEASE BUILDING: {Environment.StackTrace}");

        public static void LogReturn(int instrId) => LogUtils.DoLog($"Exited at instruction {instrId}");

        public static void LogBreak(int instrId) => LogUtils.DoLog($"Breaked at instruction {instrId}");

        public static void AddZonesUnlockData(UnlockManager __instance) => __instance.m_properties.m_ZoneMilestones = new MilestoneInfo[0x10].Select((x, i) => __instance.m_properties.m_ZoneMilestones.ElementAtOrDefault(i) ?? __instance.m_properties.m_ZoneMilestones[0]).ToArray();



        public static ItemClass.Zone GetBlockZoneOverride(ref ZoneBlock block, int x, int z, ItemClass.Zone zone1, ItemClass.Zone zone2)
        {
            ItemClass.Zone targetZone = block.GetZone(x, z);
            switch ((int)targetZone)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    if (CZMController.m_ghostMode)
                    {
                        ItemClass.Zone newValue = CustomZoneData.Instance[targetZone].HasZone(zone1) ? zone1 : CustomZoneData.Instance[targetZone].HasZone(zone2) ? zone2 : CustomZoneData.Instance[targetZone].GetLowerestZone();
                        block.SetZone(x, z, newValue);
                        block.RefreshZoning(0);
                        return newValue;
                    }
                    else
                    {
                        return CustomZoneData.Instance[targetZone].HasZone(zone1) ? zone1 : CustomZoneData.Instance[targetZone].HasZone(zone2) ? zone2 : targetZone;
                    }
                default:
                    return targetZone;
            }
        }

        public static bool GetBlockZoneSanitize(ref ZoneBlock block, int x, int z)
        {
            ItemClass.Zone targetZone = block.GetZone(x, z);
            switch ((int)targetZone)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    if (CZMController.m_ghostMode)
                    {
                        ItemClass.Zone newValue = CustomZoneData.Instance[targetZone].GetLowerestZone();
                        block.SetZone(x, z, newValue);
                        return true;
                    }
                    break;
            }
            return false;
        }

        public static int GetCurrentDemandFor(ref ItemClass.Zone zone, byte district)
        {
            DistrictManager instance2 = DistrictManager.instance;
            ZoneManager instance = Singleton<ZoneManager>.instance;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow: return GetDistrictLResDemand(district, instance2, instance);
                case ItemClass.Zone.ResidentialHigh: return GetDistrictHResDemand(district, instance2, instance);
                case ItemClass.Zone.CommercialLow: return GetDistrictLComDemand(district, instance2, instance);
                case ItemClass.Zone.CommercialHigh: return GetDistrictHComDemand(district, instance2, instance);
                case ItemClass.Zone.Industrial: return GetDistrictIndtDemand(district, instance2, instance);
                case ItemClass.Zone.Office: return GetDistrictOffcDemand(district, instance2, instance);
                case (ItemClass.Zone)8: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)9: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)10: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)11: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)12: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)13: return GetHighestDemand(ref zone, district, instance2, instance);
                case (ItemClass.Zone)14: return GetHighestDemand(ref zone, district, instance2, instance);
                default: return 0;
            };
        }

        private static int GetDistrictLComDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualCommercialDemand + instance2.m_districts.m_buffer[district].CalculateCommercialLowDemandOffset();
        private static int GetDistrictLResDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualResidentialDemand + instance2.m_districts.m_buffer[district].CalculateResidentialLowDemandOffset();
        private static int GetDistrictHComDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualCommercialDemand + instance2.m_districts.m_buffer[district].CalculateCommercialHighDemandOffset();
        private static int GetDistrictHResDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualResidentialDemand + instance2.m_districts.m_buffer[district].CalculateResidentialHighDemandOffset();
        private static int GetDistrictOffcDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualWorkplaceDemand + instance2.m_districts.m_buffer[district].CalculateOfficeDemandOffset();
        private static int GetDistrictIndtDemand(byte district, DistrictManager instance2, ZoneManager instance) => instance.m_actualWorkplaceDemand + instance2.m_districts.m_buffer[district].CalculateIndustrialDemandOffset();

        public static readonly ItemClass.Zone[] ZONES_TO_CHECK = new ItemClass.Zone[]
        {
            ItemClass.Zone.ResidentialLow       ,
            ItemClass.Zone.ResidentialHigh      ,
            ItemClass.Zone.CommercialLow        ,
            ItemClass.Zone.CommercialHigh       ,
            ItemClass.Zone.Industrial           ,
            ItemClass.Zone.Office
        };

        private static readonly Func<byte, DistrictManager, ZoneManager, int>[] m_zonesDemandFunctions = new Func<byte, DistrictManager, ZoneManager, int>[]
        {
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictLResDemand),
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictHResDemand),
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictLComDemand),
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictHComDemand),
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictIndtDemand),
        new Func<byte, DistrictManager, ZoneManager, int>(GetDistrictOffcDemand),
        };

        private static int GetHighestDemand(ref ItemClass.Zone zone, byte district, DistrictManager instance2, ZoneManager instance)
        {
            ZoneItem zi = CustomZoneData.Instance[zone];
            int[] demands = new int[ZONES_TO_CHECK.Length];
            for (int i = 0; i < ZONES_TO_CHECK.Length; i++)
            {
                if (zi.HasZone(ZONES_TO_CHECK[i]))
                {
                    demands[i] = m_zonesDemandFunctions[i](district, instance2, instance);
                }
            }
            int maxVal = demands.Max();
            int maxIdx = Array.IndexOf(demands, maxVal);
            zone = (ItemClass.Zone)(2 + maxIdx);
            return maxVal;
        }

        internal static IEnumerable<CodeInstruction> SimulationStepTranspiller(IEnumerable<CodeInstruction> instructions)
        {
            var inst = new List<CodeInstruction>(instructions);
            int idxFound = 0;
            if (ModInstance.DebugMode)
            {
                var detourLogPoints = new List<Tuple<int, CodeInstruction[]>>();
                for (int i = idxFound + 1; i < inst.Count - 2; i++)
                {
                    if (inst[i].opcode == OpCodes.Ret)
                    {
                        detourLogPoints.Add(Tuple.New(i, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldc_I4, i),
                        new CodeInstruction(OpCodes.Call, typeof(CustomZoneMixerOverrides).GetMethod("LogReturn")),
                    }));
                    }
                    else if (inst[i].opcode == OpCodes.Break)
                    {
                        detourLogPoints.Add(Tuple.New(i, new CodeInstruction[] {
                        new CodeInstruction(OpCodes.Ldc_I4, i),
                        new CodeInstruction(OpCodes.Call, typeof(CustomZoneMixerOverrides).GetMethod("LogBreak")),
                    }));
                    }
                }
                detourLogPoints.Sort((a, b) => a.First - b.First);
                detourLogPoints.ForEach(x => inst.InsertRange(x.First, x.Second));
            }

            int zoneField = -1;
            for (int i = 1; i < inst.Count; i++)
            {
                if (zoneField == -1 && inst[i].opcode == OpCodes.Stloc_S && (inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalType == typeof(ItemClass.Zone))
                {
                    LogUtils.DoLog($"inst[{i}].operand = {inst[i].operand} {inst[i].operand?.GetType()} {(inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalIndex} {(inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalType}");
                    zoneField = (inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalIndex;
                }

                if (inst[i - 1].opcode == OpCodes.Callvirt && inst[i - 1].operand is MethodInfo mi && mi.Name == "GetDistrict" && inst[i].opcode == OpCodes.Stloc_S)
                {


                    LogUtils.DoLog($"inst[{i}].operand = {inst[i].operand} ({inst[i].operand?.GetType()}) {(inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalIndex}");
                    int locIdx = (inst[i].operand as System.Reflection.Emit.LocalBuilder).LocalIndex;
                    int deltaResult = 9 - locIdx;

                    Label postSwitch = default;
                    bool switchStartFound = false;
                    bool defaultPartEnded = false;

                    for (int j = i + 1; j < inst.Count - 2; j++)
                    {
                        if (switchStartFound)
                        {
                            if (defaultPartEnded)
                            {
                                if (inst[j].opcode == OpCodes.Br || inst[j].opcode == OpCodes.Br_S)
                                {
                                    postSwitch = (Label)inst[j].operand;
                                    break;
                                }
                            }
                            else
                            {
                                if (inst[j].opcode == OpCodes.Br || inst[j].opcode == OpCodes.Br_S || inst[j].opcode == OpCodes.Ret)
                                {
                                    defaultPartEnded = true;
                                }
                            }
                        }
                        else
                        {
                            if (inst[j].opcode == OpCodes.Switch)
                            {
                                switchStartFound = true;
                            }
                        }
                    }


                    inst.InsertRange(i + 1, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Ldloca_S, zoneField ),
                        new CodeInstruction(OpCodes.Ldloc_S, locIdx ),
                        new CodeInstruction(OpCodes.Call, typeof(CustomZoneMixerOverrides).GetMethod("GetCurrentDemandFor") ),
                        new CodeInstruction(OpCodes.Stloc_S, 10 - deltaResult),
                        //new CodeInstruction(OpCodes.Ldstr, "LOGGING ZONE! district: {0} - demand: {1} - zone: {2}" ),
                        //new CodeInstruction(OpCodes.Ldc_I4_3),
                        //new CodeInstruction(OpCodes.Newarr, typeof(object)),
                        //new CodeInstruction(OpCodes.Dup),
                        //new CodeInstruction(OpCodes.Ldc_I4_0),
                        //new CodeInstruction(OpCodes.Ldloc_S, 9 ),
                        //new CodeInstruction(OpCodes.Box,typeof(byte) ),
                        //new CodeInstruction(OpCodes.Stelem_Ref ),
                        //new CodeInstruction(OpCodes.Dup),
                        //new CodeInstruction(OpCodes.Ldc_I4_1),
                        //new CodeInstruction(OpCodes.Ldloc_S, 10 ),
                        //new CodeInstruction(OpCodes.Box,typeof(int) ),
                        //new CodeInstruction(OpCodes.Stelem_Ref ),
                        //new CodeInstruction(OpCodes.Dup),
                        //new CodeInstruction(OpCodes.Ldc_I4_2),
                        //new CodeInstruction(OpCodes.Ldloc_S, 6 ),
                        //new CodeInstruction(OpCodes.Box,typeof(ItemClass.Zone) ),
                        //new CodeInstruction(OpCodes.Stelem_Ref ),
                        //new CodeInstruction(OpCodes.Call, typeof(LogUtils).GetMethod("DoLog") ),
                        new CodeInstruction(OpCodes.Br, postSwitch)
                    });
                    idxFound = i;
                    break;
                }
            }

            LogUtils.PrintMethodIL(inst);
            return inst;
        }

        public static IEnumerable<CodeInstruction> CheckBlockTranspiller(IEnumerable<CodeInstruction> instructions)
        {
            var inst = new List<CodeInstruction>(instructions);
            MethodInfo getBlockZoneOverride = typeof(CustomZoneMixerOverrides).GetMethod("GetBlockZoneOverride");
            for (int i = 2; i < inst.Count - 1; i++)
            {
                if (inst[i].opcode == OpCodes.Call && inst[i].operand == GET_BLOCK_ZONE)
                {

                    inst[i].operand = getBlockZoneOverride;
                    inst.InsertRange(i, new List<CodeInstruction>() {
                        new CodeInstruction(OpCodes.Ldarg_S,5),
                        new CodeInstruction(OpCodes.Ldarg_S,5),
                    });
                    i += 4;
                }
            }
            LogUtils.PrintMethodIL(inst);
            return inst;
        }



        public static float ConvertZoneToFloat(byte zone)
        {
            switch (zone & 0xf)
            {
                case 8:
                    return 1 + (zone & 0xf0);
                case 9:
                    return 3 + (zone & 0xf0);
                case 10:
                    return 2 + (zone & 0xf0);
                case 11:
                    return 4 + (zone & 0xf0);
                case 12:
                    return 7 + (zone & 0xf0);
                case 13:
                    return 5 + (zone & 0xf0);
                case 14:
                    return 6 + (zone & 0xf0);
                default:
                    return zone;
            }
        }


        public static float ConvertAngleToFloat(ref ZoneCell zone)
        {
            switch (zone.m_zone & 0xf)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    return (zone.m_angle + 128) & 0xFF;
                default:
                    return zone.m_angle;
            }
        }

        public static IEnumerable<CodeInstruction> TranspilePatchRefresh(IEnumerable<CodeInstruction> instructions)
        {
            var inst = new List<CodeInstruction>(instructions);
            for (int i = 1; i < inst.Count; i++)
            {
                if (inst[i - 1].operand == ZONE_FIELD && inst[i].opcode == OpCodes.Conv_R4)
                {
                    inst[i] = new CodeInstruction(OpCodes.Call, typeof(CustomZoneMixerOverrides).GetMethod("ConvertZoneToFloat"));
                }
                if (inst[i - 1].operand == ANGLE_FIELD && inst[i].opcode == OpCodes.Conv_R4)
                {
                    inst[i] = new CodeInstruction(OpCodes.Call, typeof(CustomZoneMixerOverrides).GetMethod("ConvertAngleToFloat"));
                    inst.RemoveAt(i - 1);
                }
            }
            LogUtils.PrintMethodIL(inst);
            return inst;
        }
        public static IEnumerable<CodeInstruction> TranspileCheckZoning(IEnumerable<CodeInstruction> instructions)
        {
            var inst = new List<CodeInstruction>(instructions);
            MethodInfo getBlockZoneOverride = typeof(CustomZoneMixerOverrides).GetMethod("GetBlockZoneOverride");
            for (int i = 5; i < inst.Count; i++)
            {
                if (inst[i].operand == GET_BLOCK_ZONE && inst[i].opcode == OpCodes.Call)
                {
                    inst[i].operand = getBlockZoneOverride;
                    inst.InsertRange(i, new List<CodeInstruction>() {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_2),
                    });
                    i += 4;
                }
            }
            LogUtils.PrintMethodIL(inst);
            return inst;
        }
        public static FieldInfo ZONE_FIELD = typeof(ZoneCell).GetField("m_zone");
        public static FieldInfo ANGLE_FIELD = typeof(ZoneCell).GetField("m_angle");
        public static MethodInfo GET_BLOCK_ZONE = typeof(ZoneBlock).GetMethod("GetZone");
    }

}
