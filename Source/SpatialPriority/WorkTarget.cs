// JobGiver_Work_Spatial.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace SpatialPriority
{
    public class WorkTarget
    {
        private Thing _thing;
        private IntVec3 _cell;
        private Map _map;

        private WorkTarget( Pawn pawn )
        {
            Pawn = pawn;
        }

        public WorkTarget( Pawn pawn, Thing thing, WorkGiver_Scanner workgiver ): this( pawn )
        {
            WorkGiver = workgiver;
            _thing = thing;
            _map = thing.MapHeld;
        }

        public WorkTarget( Pawn pawn, IntVec3 cell, Map map, WorkGiver_Scanner workgiver ): this( pawn )
        {
            WorkGiver = workgiver;
            _cell = cell;
            _map = map;
        }

        public WorkGiver_Scanner WorkGiver { get; }
        public Pawn Pawn { get; }

        private static FieldInfo _wantedPlantDefFieldInfo = AccessTools.Field( typeof( WorkGiver_Grower ), "wantedPlantDef" );
        public void FixWorkGiver_Grower()
        {
            if ( WorkGiver is WorkGiver_Grower grower )
            {
                Debug.Log( $"plant to grow: {Traverse.Create( grower ).Field( "wantedPlantDef" ).GetValue<ThingDef>()?.defName ?? "NULL"} :: " +
                    $"calculated plant to grow: {WorkGiver_Grower.CalculateWantedPlantDef( _cell, _map )?.defName ?? "NULL"}" );
                _wantedPlantDefFieldInfo.SetValue( null, null );
            }
        }

        private Job _job;
        public Job Job
        {
            get
            {
                if ( _job == null )
                {
                    _job = WorkGiver.NonScanJob( Pawn );
                    if ( _job == null )
                    {
                        if (_thing == null)
                        {
                            FixWorkGiver_Grower();
                            _job = WorkGiver.JobOnCell(Pawn, _cell);
                        }
                        else
                        {
                            _job = WorkGiver.JobOnThing(Pawn, _thing);
                        }
                    }
                }
                return _job;
            }
        }

        public bool Valid 
        {
            get
            {
                try
                {
                    if ( _thing != null && _thing.IsForbidden( Pawn ) )
                        return false;
                    if ( _cell != IntVec3.Invalid && _cell.IsForbidden( Pawn ) )
                        return false;
                    if ( !WorkGiver.AllowUnreachable && !_map.reachability.CanReach( Pawn.Position, LocalTarget, PathEndMode, TraverseParms ) )
                        return false;
                    if ( !HasJob )
                        return false;
                    return true;
                }
                catch (Exception err )
                {
                    Log.Warning( $"Exception in WorkTarget.Valid: {err.Message}" +
                                   $"\n  for {Pawn.LabelShort}" +
                                   $"\n  doing {WorkGiver.def.defName}" +
                                   $"\n  {err.StackTrace}" );
                    return false;
                }
            }
        }

        public PathEndMode PathEndMode => WorkGiver.PathEndMode;

        public TraverseParms TraverseParms => TraverseParms.For( Pawn, WorkGiver.MaxPathDanger( Pawn ) );

        public bool HasJob
        {
            get
            {
                if ( _thing != null )
                    return WorkGiver.HasJobOnThing( Pawn, _thing );

                FixWorkGiver_Grower();
                return WorkGiver.HasJobOnCell( Pawn, _cell );
            }
        }

        public LocalTargetInfo LocalTarget
        {
            get
            {
                if (_thing == null)
                    return _cell;
                return _thing;
            }
        }

        public TargetInfo Target
        {
            get
            {
                if ( _thing == null )
                    return new TargetInfo( _cell, _map );
                return _thing;
            }
        }

        public float Distance => ( Target.Cell - Pawn.Position ).LengthHorizontalSquared;

        public SpatialPriority SpatialPriority => Manager.PriorityGrid( Target.Map )[Target.Cell];

        public float Priority => WorkGiver.Prioritized ? WorkGiver.GetPriority( Pawn, Target ) : 0f;


        public static WorkTarget Closest( IEnumerable<WorkTarget> targets, Pawn pawn, float maxSearchDistance = 9999f )
        {
            WorkTarget best = null;
            float bestDistance = float.MaxValue;
            float bestPriority = float.MinValue;
            float maxDistance = maxSearchDistance * maxSearchDistance;

            foreach ( var target in targets )
            {
                var distance = target.Distance;
                var priority = target.Priority;
                var prioritized = target.WorkGiver.Prioritized;
                if ( distance < maxDistance && 
                    // higher priority    
                    ( prioritized && priority > bestPriority || 
                    // not prioritized or same priority, and smaller distance
                    ( !prioritized || priority == bestPriority ) && distance < bestDistance ) 
                    && target.Valid )
                {
                    best = target;
                    bestDistance = distance;
                    bestPriority = priority;
                }
            }

            return best;
        }
    }
}