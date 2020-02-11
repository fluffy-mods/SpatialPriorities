// Extensions.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SpatialPriority
{
    public static class Extensions
    {
        public static int GetPriority( this WorkGiver workgiver, Pawn pawn )
        {
            // TODO: get priority from WorkTab if loaded.
            return pawn.workSettings.GetPriority( workgiver.def.workType );
        }

        public static string StringJoin( this IEnumerable<string> strings, string glue )
        {
            return string.Join( glue, strings.ToArray() );
        }
    }
}