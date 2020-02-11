// JobGiver_Work_Spatial.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace SpatialPriority
{
    public class JobGiver_Work_Spatial : JobGiver_Work
    {
        private MethodInfo _giverTryGiveJobPrioritized_MI = AccessTools.Method( typeof( JobGiver_Work ), "GiverTryGiveJobPrioritized" );
        public Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
        {
            return (Job) _giverTryGiveJobPrioritized_MI.Invoke( this, new object[] {pawn, giver, cell} );
        }

        private MethodInfo _pawnCanUseWorkGiver_MI = AccessTools.Method( typeof( JobGiver_Work ), "PawnCanUseWorkGiver" );
        public bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
        {
            return (bool)_pawnCanUseWorkGiver_MI.Invoke( this, new object[] {pawn, giver} );
        }

        public override ThinkResult TryIssueJobPackage( Pawn pawn, JobIssueParams jobParams )
        {
            // prioritized work given (right click menu)
            var prioritizedJob = PrioritizedJob( pawn );
            if ( prioritizedJob.Job != null )
                return prioritizedJob;

            /**
             * For spatial priorities to have a meaningful effect, we need to at least look at all workgivers in the same worktype, 
             * with the same priority, as having the same natural priority. In other words, we need to evaluate them at the same time.
             * 
             * Step 1; list workgivers in order.
             * Step 2; group workgivers in the same type and with the same priority together.
             * Step 3; get all potential work things for the whole group.
             *  - Note that that includes workgivers that implement PotentialWorkThings, PotentialWorkCells, _and_ ThingRequests to select their targets.
             * Step 4; subset potential work things/cells by spatial priority, iterating over priority levels.
             * Repeat 2 - 4 until a job is found. 
             */

            // step 1; get workgivers.
            var workgivers = emergency
                ? pawn.workSettings.WorkGiversInOrderEmergency
                : pawn.workSettings.WorkGiversInOrderNormal;

             if (!workgivers.Any())
                return ThinkResult.NoJob;

            // step 2; create batches (lazy, only create next batch if first failed to give a job).
            var batch = new List<WorkGiver_Scanner>();
            var batchPriority = workgivers.First().GetPriority( pawn );
            var batchWorkType = workgivers.First().def.workType;
            var batchDebug = false;
            for ( int i = 0; i < workgivers.Count; i++ )
            {
                var workgiver = workgivers[i];

                if (batchDebug)
                    Debug.Log( $"Workgiver: {workgiver.def.defName}, pawn: {pawn.LabelShort}, {i+1}/{workgivers.Count}" );

                if ( !PawnCanUseWorkGiver( pawn, workgiver ) )
                    continue;

                // add workgivers to batch;
                if ( CanBatch( workgiver, pawn, batchPriority, batchWorkType ) )
                {
                    batch.Add( workgiver as WorkGiver_Scanner );
                    continue;
                }

                // step 3; get potential targets for everything in the batch, grouped by spatial priority
                var job = GetJobFromBatch( pawn, batch, batchDebug );
                if ( job.Job != null )
                    return job;

                // if current is not a scanner, handle it alone.
                if ( !( workgiver is WorkGiver_Scanner ) )
                {
                    var job2 = workgiver.NonScanJob( pawn );
                    if ( job2 != null )
                        return new ThinkResult( job2, this, workgiver.def.tagToGive );
                }
                else
                {
                    // start next batch
                    batch = new List<WorkGiver_Scanner>();
                    batch.Add( workgiver as WorkGiver_Scanner );
                    batchPriority = workgiver.GetPriority( pawn );
                    batchWorkType = workgiver.def.workType;
                    batchDebug = false;
                }
            }

            // process last batch, if any.
            if ( !batch.NullOrEmpty() )
                return GetJobFromBatch( pawn, batch, batchDebug );

            // or just nothing.
            return ThinkResult.NoJob;
        }

        private ThinkResult GetJobFromBatch( Pawn pawn, List<WorkGiver_Scanner> batch, bool debug = false)
        {
            var priorityBatches = batch
                .SelectMany( wg => PotentialTargets( wg, pawn ) )
                .GroupBy( wt => new
                {
                    spatialPriority = wt.SpatialPriority,
                    workGiver = wt.WorkGiver.def,
                } )
                .OrderByDescending( g => g.Key.spatialPriority )
                .ThenByDescending( g => g.Key.workGiver.priorityInType );

            if ( debug )
                Debug.Log( "batch: " + batch.Select( wg => wg.def.defName ).StringJoin( ", " ) );

            // step 4; iterate over priorities until a job is found
            foreach ( var priorityBatch in priorityBatches )
            {
                if ( debug )
                    Debug.Log( $"priority {priorityBatch.Key.spatialPriority} " +
                               $"| {priorityBatch.Key.workGiver.priorityInType}:\n  " +
                               $"{priorityBatch.Select( wt => $"{wt.WorkGiver.def.defName} :: {wt.LocalTarget} :: {( wt.Valid ? "Valid" : "Invalid" )} :: {( wt.Job != null ? "HasJob" : "NO JOB" )} :: {wt.WorkGiver.GetPriority( pawn, wt.Target )}" ).StringJoin( "\n  " )}" );

                var target = WorkTarget.Closest( priorityBatch, pawn );
                if ( target != null )
                {
                    // we found a target!
                    pawn.mindState.lastGivenWorkType = target.WorkGiver.def.workType;
                    var job = target.Job;
                    if ( job == null )
                    {
                        Log.Error( string.Concat( target.WorkGiver, " provided target ", target.Target,
                            " but yielded no actual job for pawn ", pawn,
                            ". The CanGiveJob and JobOnX methods may not be synchronized." ) );
                    }
                    else
                    {
                        return new ThinkResult( job, this, target.WorkGiver.def.tagToGive );
                    }
                }
            }
            return ThinkResult.NoJob;
        }

        internal IEnumerable<WorkTarget> PotentialTargets( WorkGiver_Scanner workgiver, Pawn pawn )
        {
            if ( workgiver == null )
            {
                Debug.Log( $"Trying to get potential targets fron non-scanner" );
                return EmptyTargetList;
            }

            var potentialTargets = new List<WorkTarget>();
            if ( workgiver.def.scanThings )
                potentialTargets.AddRange( PotentialThings( workgiver, pawn ) );
            if ( workgiver.def.scanCells )
                potentialTargets.AddRange( PotentialCells( workgiver, pawn ) );
            return potentialTargets;
        }

        internal IEnumerable<WorkTarget> PotentialThings(WorkGiver_Scanner workgiver, Pawn pawn )
        {
            var potentialThings = workgiver.PotentialWorkThingsGlobal( pawn );
            if ( potentialThings == null )
            {
                potentialThings = pawn.Map.listerThings.ThingsMatching( workgiver.PotentialWorkThingRequest );
            }
            return potentialThings.Select( t => new WorkTarget( pawn, t, workgiver ) );
        }

        internal IEnumerable<WorkTarget> PotentialCells( WorkGiver_Scanner workgiver, Pawn pawn )
        {
            var potentialCells = workgiver.PotentialWorkCellsGlobal( pawn );
            if ( potentialCells?.Any() ?? false )
                return potentialCells.Select( c => new WorkTarget( pawn, c, pawn.Map, workgiver ) );
            return EmptyTargetList;
        }

        private static List<WorkTarget> EmptyTargetList = new List<WorkTarget>();

        internal bool CanBatch( WorkGiver workgiver, Pawn pawn, int priority, WorkTypeDef worktype )
        {
            // TODO: expose setting for grouping across workTypes.
            return workgiver is WorkGiver_Scanner && workgiver.GetPriority( pawn ) == priority && workgiver.def.workType == worktype;
        }

        internal ThinkResult PrioritizedJob( Pawn pawn )
        {
            if (emergency && pawn.mindState.priorityWork.IsPrioritized)
            {
                List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkType.workGiversByPriority;
                for (int i = 0; i < workGiversByPriority.Count; i++)
                {
                    WorkGiver worker = workGiversByPriority[i].Worker;
                    Job job = GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
                    if (job != null)
                    {
                        job.playerForced = true;
                        return new ThinkResult(job, this, workGiversByPriority[i].tagToGive);
                    }
                }
                pawn.mindState.priorityWork.Clear();
            }

            return ThinkResult.NoJob;
        }
    }
}