﻿using RimWorld;
using Verse;

namespace MoreInjuries;

public class InjuriesCompProps : CompProperties
{
    public HediffDef Concussion;
    public HediffDef Shock;
    public NeedDef polak;

    public InjuriesCompProps()
    {
        compClass = typeof(InjuriesComp);
    }

    public InjuriesCompProps(Type compClass) : base(compClass)
    {
        this.compClass = compClass;
    }
}
