﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MoreInjuries.HealthConditions.HearingLoss;

public class HearingLossComp : ThingComp
{
    public float GetHearingDamageMultiplier(Pawn pawn)
    {
        float result = 1f;
        // check if anything is worn that covers the ears
        if (pawn.apparel is { WornApparel.Count: > 0 })
        {
            // first, we need to get the ears
            IEnumerable<BodyPartRecord> ears = pawn.health.hediffSet.GetNotMissingParts().Where(bodyPart => bodyPart.def.defName is BodyPartNameDefOf.Ear);
            // does it cover the ears?
            if (ears.FirstOrDefault() is BodyPartRecord ear && pawn.apparel.WornApparel.Any(clothing => clothing.def.apparel.CoversBodyPart(ear)))
            {
                result /= 5f;
            }
        }

        if (IsIndoors(pawn))
        {
            // loud noises are more damaging indoors (e.g. gunshots)
            result *= 3;
        }
        return result;
    }

    public override void Notify_UsedWeapon(Pawn pawn)
    {
        // early exit if the pawn is not a human
        if (pawn.def != ThingDefOf.Human)
        {
            return;
        }
        // early exit if the pawn is not equipped with a gun
        if (pawn.equipment?.Primary?.TryGetComp<CompEquippable>()?.PrimaryVerb.verbProps is not VerbProperties { range: > 0 } gunProperties)
        {
            return;
        }
        float radius = gunProperties.muzzleFlashScale;
        // when shot indoors, the gunshot sound is damaging over a larger area
        if (IsIndoors(pawn))
        {
            radius *= 1.25f;
        }
        // get all pawns in the vicinity of the shooter and apply hearing damage
        IEnumerable<IntVec3> cellsInVicinity = GenRadial.RadialCellsAround(pawn.Position, radius, useCenter: true);
        foreach (IntVec3 cell in cellsInVicinity)
        {
            IEnumerable<Pawn> pawnsInCell = cell.GetThingList(pawn.Map).OfType<Pawn>();
            foreach (Pawn otherPawn in pawnsInCell)
            {
                float hearingDamageMultiplier = GetHearingDamageMultiplier(otherPawn);
                if (Rand.Chance(hearingDamageMultiplier / 10f))
                {
                    if (!otherPawn.health.hediffSet.TryGetHediff(HearingLossDefOf.HearingLoss, out Hediff? hearingLoss))
                    {
                        hearingLoss = HediffMaker.MakeHediff(HearingLossDefOf.HearingLoss, otherPawn);
                        otherPawn.health.AddHediff(hearingLoss);
                    }
                    hearingLoss.Severity += hearingDamageMultiplier / 100f;
                }
            }
        }
    }

    private static bool IsIndoors(Pawn pawn) => !pawn.Position.UsesOutdoorTemperature(pawn.Map);
}