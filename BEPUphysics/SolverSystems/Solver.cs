﻿using System;
using System.Threading;
using System.Collections.ObjectModel;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.Threading;
using BEPUphysics.Constraints;
using BEPUphysics.DataStructures;

namespace BEPUphysics.SolverSystems
{
    ///<summary>
    /// Iteratively solves solver updateables, converging to a solution for simulated joints and collision pair contact constraints.
    ///</summary>
    public class Solver : MultithreadedProcessingStage
    {
        RawList<SolverUpdateable> solverUpdateables = new RawList<SolverUpdateable>();
        internal int iterationLimit = 10;
        ///<summary>
        /// Gets or sets the maximum number of iterations the solver will attempt to use to solve the simulation's constraints.
        ///</summary>
        public int IterationLimit { get { return iterationLimit; } set { iterationLimit = Math.Max(value, 0); } }
        ///<summary>
        /// Gets the list of solver updateables in the solver.
        ///</summary>
        public ReadOnlyList<SolverUpdateable> SolverUpdateables
        {
            get
            {
                return new ReadOnlyList<SolverUpdateable>(solverUpdateables);
            }
        }
        protected internal TimeStepSettings timeStepSettings;
        ///<summary>
        /// Gets or sets the time step settings used by the solver.
        ///</summary>
        public TimeStepSettings TimeStepSettings
        {
            get
            {
                return timeStepSettings;
            }
            set
            {
                timeStepSettings = value;
            }
        }

        ///<summary>
        /// Gets or sets the deactivation manager used by the solver.
        /// When constraints are added and removed, the deactivation manager
        /// gains and loses simulation island connections that affect simulation islands
        /// and activity states.
        ///</summary>
        public DeactivationManager DeactivationManager { get; set; }

        ///<summary>
        /// Constructs a Solver.
        ///</summary>
        ///<param name="timeStepSettings">Time step settings used by the solver.</param>
        ///<param name="deactivationManager">Deactivation manager used by the solver.</param>
        public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager)
        {
            TimeStepSettings = timeStepSettings;
            DeactivationManager = deactivationManager;
            multithreadedPrestepDelegate = MultithreadedPrestep;
            multithreadedIterationDelegate = MultithreadedIteration;
            Enabled = true;
        }
        ///<summary>
        /// Constructs a Solver.
        ///</summary>
        ///<param name="timeStepSettings">Time step settings used by the solver.</param>
        ///<param name="deactivationManager">Deactivation manager used by the solver.</param>
        /// <param name="threadManager">Thread manager used by the solver.</param>
        public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager, IThreadManager threadManager)
            : this(timeStepSettings, deactivationManager)
        {
            ThreadManager = threadManager;
            AllowMultithreading = true;
        }

        ///<summary>
        /// Adds a solver updateable to the solver.
        ///</summary>
        ///<param name="item">Updateable to add.</param>
        ///<exception cref="ArgumentException">Thrown when the item already belongs to a solver.</exception>
        public void Add(SolverUpdateable item)
        {
            if (item.Solver == null)
            {
                item.Solver = this;
                solverUpdateables.Add(item);
                DeactivationManager.Add(item);
                item.OnAdditionToSolver(this);
            }
            else
                throw new ArgumentException("Solver updateable already belongs to something; it can't be added.", "item");
        }
        ///<summary>
        /// Removes a solver updateable from the solver.
        ///</summary>
        ///<param name="item">Updateable to remove.</param>
        ///<exception cref="ArgumentException">Thrown when the item does not belong to the solver.</exception>
        public void Remove(SolverUpdateable item)
        {
            if (item.Solver == this)
            {
                item.Solver = null;
                solverUpdateables.FastRemove(item);
                DeactivationManager.Remove(item);
                item.OnRemovalFromSolver(this);
            }
            else
                throw new ArgumentException("Solver updateable doesn't belong to this solver; it can't be removed.", "item");
        }

        ///<summary>
        /// Adds a solver updateable to the solver.
        /// "Flux" addition means that the solver updateable being added will be contained in the flux list if active,
        /// and so should not be added to the main updateables list.
        ///</summary>
        ///<param name="item">Updateable to add.</param>
        ///<exception cref="ArgumentException">Thrown when the item already belongs to a solver.</exception>
        public void FluxAdd(SolverUpdateable item)
        {
            if (item.Solver == null)
            {
                item.Solver = this;
                DeactivationManager.Add(item);
                item.OnAdditionToSolver(this);
            }
            else
                throw new ArgumentException("Solver updateable already belongs to something; it can't be added.", "item");
        }
        ///<summary>
        /// Removes a solver updateable from the solver.
        /// "Flux" removal means that the solver updateable being removed will be contained in the flux list if active,
        /// and so should not be removed from the main updateables list (it would not be present).
        ///</summary>
        ///<param name="item">Updateable to remove.</param>
        ///<returns>Whether or not a split occurred during the removal process.</returns>
        ///<exception cref="ArgumentException">Thrown when the item does not belong to the solver.</exception>
        public bool FluxRemove(SolverUpdateable item)
        {
            if (item.Solver == this)
            {
                item.Solver = null;
                bool split = DeactivationManager.Remove(item);
                item.OnRemovalFromSolver(this);
                return split;
            }
            else
                throw new ArgumentException("Solver updateable doesn't belong to this solver; it can't be removed.", "item");
        }

        RawList<SolverUpdateable> fluxUpdateables = new RawList<SolverUpdateable>();
        /// <summary>
        /// Gets or sets the list of 'flux' updateables.  Flux updateables are those with sufficiently short life spans such that
        /// adding them to and removing them from the primary updateables list would be expensive.  Instead, a list of flux updateables
        /// is recreated each frame and updated alongside the normal updateables.
        /// </summary>
        public RawList<SolverUpdateable> FluxUpdateables
        {
            get
            {
                return fluxUpdateables;
            }
            set
            {
                fluxUpdateables = value;
            }
        }

        /// <summary>
        /// Gets the updateable at the given index.
        /// </summary>
        /// <param name="i">Index to use.</param>
        /// <returns>Solver updateable at the index.</returns>
        SolverUpdateable GetUpdateable(long i)
        {
            return i >= fluxUpdateables.count ? solverUpdateables.Elements[i - fluxUpdateables.count] : fluxUpdateables.Elements[i];
        }

        Action<int> multithreadedPrestepDelegate;
        void MultithreadedPrestep(int i)
        {
            var updateable = GetUpdateable(i);
            updateable.UpdateSolverActivity();
            if (updateable.isActiveInSolver)
            {
                updateable.SolverSettings.currentIterations = 0;
                updateable.SolverSettings.iterationsAtZeroImpulse = 0;
                updateable.Update(timeStepSettings.TimeStepDuration);

                updateable.EnterLock();
                try
                {
                    updateable.ExclusiveUpdate();
                }
                finally
                {
                    updateable.ExitLock();
                }
            }
        }

        /// <summary>
        /// Gets or sets the permutation index used by the solver.  If the simulation is restarting from a given frame,
        /// setting this index to be consistent is required for deterministic results.
        /// </summary>
        public int PermutationIndex
        {
            get
            {
                return primeIndex;
            }
            set
            {
                primeIndex = value % primes.Length;
            }
        }

        int primeIndex;
        static long[] primes = {
                                    472882049, 492876847,
                                    492876863, 512927357,
                                    512927377, 533000389,
                                    533000401, 553105243,
                                    553105253, 573259391,
                                    573259433, 593441843,
                                    593441861, 613651349,
                                    613651369, 633910099,
                                    633910111, 654188383,
                                    654188429, 674506081,
                                    674506111, 694847533,
                                    694847539, 715225739,
                                    715225741, 735632791,
                                    735632797, 756065159,
                                    756065179, 776531401,
                                    776531419, 797003413,
                                    797003437, 817504243,
                                    817504253, 838041641,
                                    838041647, 858599503,
                                    858599509, 879190747,
                                    879190841, 899809343,
                                    899809363, 920419813,
                                    920419823, 941083981,
                                    941083987, 961748927,
                                    961748941, 982451653
                               };
        long prime;
        void ComputeIterationCoefficient()
        {
            prime = primes[primeIndex = (primeIndex + 1) % primes.Length];
        }

        Action<int> multithreadedIterationDelegate;
        void MultithreadedIteration(int i)
        {
            //'i' is currently an index into an implicit array of solver updateables that goes from 0 to solverUpdateables.count * iterationLimit.
            //It includes iterationLimit copies of each updateable.
            //Permute the entire set with duplicates.
            var updateable = GetUpdateable((i * prime) % totalUpdateableCount);


            SolverSettings solverSettings = updateable.solverSettings;
            //Updateables only ever go from active to inactive during iterations,
            //so it's safe to check for activity before we do hard (synchronized) work.
            if (updateable.isActiveInSolver)
            {
                int incrementedIterations = -1;
                updateable.EnterLock();
                //This duplicate test protects against the possibility that the updateable went inactive between the first check and the lock.
                if (updateable.isActiveInSolver)
                {
                    if (updateable.SolveIteration() < solverSettings.minimumImpulse)
                    {
                        solverSettings.iterationsAtZeroImpulse++;
                        if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations)
                            updateable.isActiveInSolver = false;
                    }
                    else
                    {
                        solverSettings.iterationsAtZeroImpulse = 0;
                    }

                    //Increment the iteration count.
                    incrementedIterations = solverSettings.currentIterations++;
                }
                updateable.ExitLock();
                //Since the updateables only ever go from active to inactive, it's safe to check outside of the lock.
                //Keeping this if statement out of the lock allows other waiters to get to work a few nanoseconds faster.
                if (incrementedIterations > iterationLimit ||
                    incrementedIterations > solverSettings.maximumIterations)
                {
                    updateable.isActiveInSolver = false;
                }

            }



        }

        int totalUpdateableCount;
        protected override void UpdateMultithreaded()
        {
            totalUpdateableCount = fluxUpdateables.count + solverUpdateables.count;
            ThreadManager.ForLoop(0, totalUpdateableCount, multithreadedPrestepDelegate);
            ComputeIterationCoefficient();
            ThreadManager.ForLoop(0, iterationLimit * totalUpdateableCount, multithreadedIterationDelegate);
        }

        protected override void UpdateSingleThreaded()
        {

            totalUpdateableCount = fluxUpdateables.count + solverUpdateables.count;
            for (int i = 0; i < totalUpdateableCount; i++)
            {
                UnsafePrestep(GetUpdateable(i));
            }

            int totalCount = iterationLimit * totalUpdateableCount;
            ComputeIterationCoefficient();
            for (int i = 0; i < totalCount; i++)
            {
                UnsafeSolveIteration(GetUpdateable((i * prime) % totalUpdateableCount));
            }


        }

        protected internal void UnsafePrestep(SolverUpdateable updateable)
        {
            updateable.UpdateSolverActivity();
            if (updateable.isActiveInSolver)
            {
                SolverSettings solverSettings = updateable.solverSettings;
                solverSettings.currentIterations = 0;
                solverSettings.iterationsAtZeroImpulse = 0;
                updateable.Update(timeStepSettings.TimeStepDuration);
                updateable.ExclusiveUpdate();
            }
        }

        protected internal void UnsafeSolveIteration(SolverUpdateable updateable)
        {
            if (updateable.isActiveInSolver)
            {
                SolverSettings solverSettings = updateable.solverSettings;


                solverSettings.currentIterations++;
                if (solverSettings.currentIterations <= iterationLimit &&
                    solverSettings.currentIterations <= solverSettings.maximumIterations)
                {
                    if (updateable.SolveIteration() < solverSettings.minimumImpulse)
                    {
                        solverSettings.iterationsAtZeroImpulse++;
                        if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations)
                            updateable.isActiveInSolver = false;

                    }
                    else
                    {
                        solverSettings.iterationsAtZeroImpulse = 0;
                    }
                }
                else
                {
                    updateable.isActiveInSolver = false;
                }

                //if (++solverSettings.currentIterations > iterationLimit ||
                //    solverSettings.currentIterations > solverSettings.maximumIterations ||
                //    (updateable.SolveIteration() < solverSettings.minimumImpulse &&
                //    ++solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterations))
                //{
                //    updateable.isActiveInSolver = false;
                //}
                //else //If it's greater than the minimum impulse, reset the count.
                //    solverSettings.iterationsAtZeroImpulse = 0;
            }
        }


    }
}
