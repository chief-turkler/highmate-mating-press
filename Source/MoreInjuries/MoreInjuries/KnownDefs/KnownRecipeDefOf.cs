﻿using RimWorld;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace MoreInjuries.KnownDefs;

[DefOf]
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Cannot encapsulate DefOf fields in properties, required for reflection. Name must match XML def name.")]
public static class KnownRecipeDefOf
{
    public static RecipeDef ExtractWholeBloodBag = null!;
}