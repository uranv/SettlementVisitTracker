using RimWorld;
using RimWorld.Planet;
using Verse;

namespace SettlementVisitTracker;

public class WorldObjectComp_LastVisitTracker : WorldObjectComp
{
    private int _lastVisitTick = -1;

    public bool HasBeenVisited => _lastVisitTick >= 0;

    public void RecordVisit()
    {
        if (Find.TickManager != null)
        {
            _lastVisitTick = Find.TickManager.TicksGame;
        }
    }

    public string GetLastVisitInspectString()
    {
        if (!(parent is Settlement settlement) || settlement.Faction == Faction.OfPlayer)
        {
            return null;
        }

        if (!HasBeenVisited)
        {
            return "SettlementVisitTracker_LastVisitNever".Translate().Resolve();
        }

        int ticksSinceLastVisit = Find.TickManager.TicksGame - _lastVisitTick;
        return "SettlementVisitTracker_LastVisit".Translate(
            (ticksSinceLastVisit < 0 ? 0 : ticksSinceLastVisit).ToStringTicksToPeriodVague()).Resolve();
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref _lastVisitTick, "lastVisitTick", -1);
    }
}