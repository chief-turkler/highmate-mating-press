﻿using System.Collections.Generic;
using Verse;

namespace MoreInjuries.Initialization;

public class FixMisplacedBionicsModExtension : DefModExtension
{
    public required List<BodyPartDef> BodyParts { get; set; }
}