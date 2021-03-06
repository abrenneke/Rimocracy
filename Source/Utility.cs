﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rimocracy
{
    enum LogLevel { Message = 0, Warning, Error };

    public static class Utility
    {
        static bool? simpleSlaveryInstalled = null;

        public static RimocracyComp RimocracyComp => Find.World?.GetComponent<RimocracyComp>();

        public static bool PoliticsEnabled => RimocracyComp != null && RimocracyComp.IsEnabled;

        public static bool IsSimpleSlaveryInstalled =>
            (bool)(simpleSlaveryInstalled ?? (simpleSlaveryInstalled = RimocracyDefOf.Enslaved != null));

        public static IEnumerable<Pawn> Citizens =>
            PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(p => p.IsCitizen());

        public static int CitizensCount => Citizens.Count();

        public static string NationName => Find.FactionManager.OfPlayer.Name;

        public static IEnumerable<LeaderTitleDef> ApplicableLeaderTitles => DefDatabase<LeaderTitleDef>.AllDefs.Where(def => def.IsApplicable);

        public static string LeaderTitle => RimocracyComp?.LeaderTitleDef?.GetTitle(RimocracyComp.Leader) ?? "leader";

        public static bool IsCitizen(this Pawn pawn) =>
            pawn != null
            && !pawn.Dead
            && pawn.IsFreeColonist
            && pawn.ageTracker.AgeBiologicalYears >= Settings.CitizenshipAge
            && (!IsSimpleSlaveryInstalled || !pawn.health.hediffSet.hediffs.Any(hediff => hediff.def == RimocracyDefOf.Enslaved));

        public static bool CanBeLeader(this Pawn p) => p.IsCitizen() && !p.GetDisabledWorkTypes(true).Contains(RimocracyDefOf.Governing);

        public static bool IsLeader(this Pawn p) => PoliticsEnabled && RimocracyComp.Leader == p;

        public static string ListString(List<string> list)
        {
            if (list.NullOrEmpty())
                return "";
            if (list.Count == 2)
                return list[0] + " and " + list[1];
            string res = list[0];
            for (int i = 1; i < list.Count - 1; i++)
                res += ", " + list[i];
            res += " and " + list.Last();
            return res;
        }

        internal static void Log(string message, LogLevel logLevel = LogLevel.Message)
        {
            message = "[Rimocracy] " + message;
            switch (logLevel)
            {
                case LogLevel.Message:
                    if (Settings.DebugLogging)
                        Verse.Log.Message(message);
                    break;

                case LogLevel.Warning:
                    Verse.Log.Warning(message);
                    break;

                case LogLevel.Error:
                    Verse.Log.Error(message);
                    break;
            }
        }
    }
}
