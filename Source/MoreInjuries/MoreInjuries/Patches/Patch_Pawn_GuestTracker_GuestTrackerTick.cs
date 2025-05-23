﻿using HarmonyLib;
using MoreInjuries.HealthConditions.HeavyBleeding.Transfusions;
using MoreInjuries.KnownDefs;
using RimWorld;
using Verse;

namespace MoreInjuries.Patches;

[HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.GuestTrackerTick))]
public static class Patch_Pawn_GuestTracker_GuestTrackerTick
{
    private static AccessTools.FieldRef<Pawn_GuestTracker, Pawn>? s_guestTrackerPawn;

    public static AccessTools.FieldRef<Pawn_GuestTracker, Pawn> GuestTrackerPawnRef => s_guestTrackerPawn
        ??= AccessTools.FieldRefAccess<Pawn_GuestTracker, Pawn>("pawn");

    internal static void Postfix(Pawn_GuestTracker __instance)
    {
        if (GuestTrackerPawnRef.Invoke(__instance) is Pawn pawn 
            && pawn.IsHashIntervalTick(15000)
            && KnownResearchProjectDefOf.BasicFirstAid.IsFinished
            && Recipe_ExtractBloodBag.CanSafelyBeQueued(pawn)
            && __instance.GuestStatus is GuestStatus.Prisoner
            && __instance.IsInteractionEnabled(KnownPrisonerInteractionModeDefOf.BloodBagFarm))
        {
            HealthCardUtility.CreateSurgeryBill(pawn, KnownRecipeDefOf.ExtractWholeBloodBag, part: null!, sendMessages: false);
        }
    }
}
