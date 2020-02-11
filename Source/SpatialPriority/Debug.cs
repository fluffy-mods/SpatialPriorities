// Debug.cs
// Copyright Karel Kroeze, 2018-2018

using System.Diagnostics;

namespace SpatialPriority
{
    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void Log( string message )
        {
            Verse.Log.Message( $"SpatialPriority :: {message}" );
        }
    }
}