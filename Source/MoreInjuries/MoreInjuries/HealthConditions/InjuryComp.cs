﻿using System.Collections.Generic;
using Verse;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.Fractures;
using MoreInjuries.Debug;
using MoreInjuries.HealthConditions.Paralysis;
using MoreInjuries.HealthConditions.IntestinalSpill;
using MoreInjuries.HealthConditions.HemorrhagicStroke;
using MoreInjuries.HealthConditions.AdrenalineRush;
using MoreInjuries.HealthConditions.HydrostaticShock;
using MoreInjuries.HealthConditions.LungCollapse;
using MoreInjuries.HealthConditions.InhalationInjury;
using MoreInjuries.HealthConditions.SpallingInjury;
using MoreInjuries.HealthConditions.EmpShutdown;

namespace MoreInjuries.HealthConditions;

public class InjuryComp : ThingComp
{
    private DamageInfo _damageInfo;

    private readonly InjuryWorker[] _pipeline;

    public InjuryComp()
    {
        _pipeline =
        [
            new ParalysisWorker(this),
            new IntestinalSpillWorker(this),
            new HemorrhagicStrokeWorker(this),
            new AdrenalineWorker(this),
            new HydrostaticShockWorker(this),
            new FractureWorker(this),
            new LungCollapseWorker(this),
            new InhalationInjuryWorker(this),
            new SpallingWorker(this),
            new ChokingWorker(this),
            new EmpBionicsWorker(this)
        ];
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        foreach (InjuryWorker component in _pipeline)
        {
            if (component is ICompFloatMenuOptionsHandler { IsEnabled: true } handler)
            {
                foreach (FloatMenuOption option in handler.CompFloatMenuOptions(selectedPawn))
                {
                    yield return option;
                }
            }
        }
    }

    public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        foreach (InjuryWorker component in _pipeline)
        {
            if (component is IPostPostApplyDamageHandler { IsEnabled: true } handler)
            {
                handler.PostPostApplyDamage(in dinfo);
            }
        }

        base.PostPostApplyDamage(dinfo, totalDamageDealt);
    }

    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        Patch_Thing_TakeDamage.IsActive = true;
        _damageInfo = dinfo;

        foreach (InjuryWorker component in _pipeline)
        {
            if (component is IPostPostApplyDamageHandler { IsEnabled: true } handler)
            {
                handler.PostPostApplyDamage(in dinfo);
            }
        }

        base.PostPreApplyDamage(ref dinfo, out absorbed);
    }

    public void PostDamageFull(DamageWorker.DamageResult damage)
    {
        DebugAssert.NotNull(damage, "damage is null in PostDamageFull");

        foreach (InjuryWorker component in _pipeline)
        {
            if (component is IPostTakeDamageHandler { IsEnabled: true } handler)
            {
                handler.PostTakeDamage(damage, in _damageInfo);
            }
        }
    }
}