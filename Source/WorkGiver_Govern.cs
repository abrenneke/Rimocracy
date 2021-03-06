﻿using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Rimocracy
{
    class WorkGiver_Govern : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => pawn.Map.listerBuildings.allBuildingsColonist;

        // Don't start Govern job if governance is already 99%+
        public override bool ShouldSkip(Pawn pawn, bool forced = false) => !pawn.IsLeader() || Utility.RimocracyComp.Governance >= 0.99f;

        // Prefer more efficient stations
        public override float GetPriority(Pawn pawn, TargetInfo t) => t.Thing.GetStatValue(RimocracyDefOf.GovernEfficiencyFactor);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
            => t.def.StatBaseDefined(RimocracyDefOf.GovernEfficiencyFactor) && pawn.CanReserve(t, ignoreOtherReservations: forced);

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) => JobMaker.MakeJob(RimocracyDefOf.Govern, t);
    }
}
