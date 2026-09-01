using System.Reflection;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace SettlementVisitTracker;

[StaticConstructorOnStartup]
public static class SettlementVisitTrackerStartup
{
    static SettlementVisitTrackerStartup()
    {
        var harmony = new Harmony("com.uranv.settlementvisittracker");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Settlement), nameof(Settlement.GetInspectString))]
internal static class SettlementInspectStringPatch
{
    public static void Postfix(Settlement __instance, ref string __result)
    {
        var lastVisit = __instance.GetComponent<WorldObjectComp_LastVisitTracker>()?.GetLastVisitInspectString();
        if (lastVisit.NullOrEmpty())
        {
            return;
        }

        if (!__result.NullOrEmpty())
        {
            __result += "\n";
        }
        __result += lastVisit;
    }
}

[HarmonyPatch(typeof(CaravanArrivalAction_VisitSettlement), nameof(CaravanArrivalAction_VisitSettlement.Arrived))]
internal static class CaravanVisitSettlementPatch
{
    private static readonly FieldInfo SettlementField =
        AccessTools.Field(typeof(CaravanArrivalAction_VisitSettlement), "settlement");

    public static void Postfix(CaravanArrivalAction_VisitSettlement __instance, Caravan caravan)
    {
        if (caravan == null || !caravan.IsPlayerControlled || SettlementField == null)
        {
            return;
        }

        var settlement = SettlementField.GetValue(__instance) as Settlement;
        settlement?.GetComponent<WorldObjectComp_LastVisitTracker>()?.RecordVisit();
    }
}

[HarmonyPatch(typeof(TransportersArrivalAction_FormCaravan), nameof(TransportersArrivalAction_FormCaravan.Arrived))]
internal static class TransportersVisitSettlementPatch
{
    private static readonly FieldInfo SettlementField =
        AccessTools.Field(typeof(TransportersArrivalAction_VisitSettlement), "settlement");

    public static void Postfix(TransportersArrivalAction_FormCaravan __instance, PlanetTile tile)
    {
        if (!(__instance is TransportersArrivalAction_VisitSettlement) || SettlementField == null)
        {
            return;
        }

        var settlement = SettlementField.GetValue(__instance) as Settlement;
        if (settlement?.Tile == tile)
        {
            settlement.GetComponent<WorldObjectComp_LastVisitTracker>()?.RecordVisit();
        }
    }
}
