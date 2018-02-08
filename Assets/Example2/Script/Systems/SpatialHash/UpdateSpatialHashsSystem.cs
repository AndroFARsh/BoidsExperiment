using System;
using System.Collections.Generic;
using Entitas;
using Example2.Utils;
using Smooth.Pools;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Example2.SpatialHash
{
    internal class UpdateSpatialHashsSystem : ReactiveSystem<GameEntity>
    {
        private const float TOLERANCE = 0.05f;
        private const int BIG_ENOUGH_INT = 16 * 1024;
        private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT + 0.0000;
        
        private readonly GameContext context;
        
        internal UpdateSpatialHashsSystem(Contexts contexts) : base(contexts.game)
        {
            context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.Bounds));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasRenderer;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var config = context.worldEntity.config.value;

            var cellFactor = config.CellFactor;
            for (var i = 0; i < @entities.Count; i++)
                Update(@entities[i], cellFactor);
        }

        private static void Update(GameEntity boid, float cellFactor)
        {
            var hashes = HashSetPool<int>.Instance.Borrow();
            
            Calculate(boid.bounds.value, cellFactor, ref hashes);
            
            if (!boid.hasSpatialHashes)
            {
                boid.AddSpatialHashes(hashes);
            }
            else
            {
                Profiler.BeginSample("Compare Old and New Hashes");
                var oldHashes = boid.spatialHashes.value;
                if (!oldHashes.SetEquals(hashes))
                {
                    Profiler.BeginSample("Replace Hashes");
                    boid.ReplaceSpatialHashes(hashes);
                    HashSetPool<int>.Instance.Release(oldHashes);
                    Profiler.EndSample();
                }
                else
                {
                    HashSetPool<int>.Instance.Release(hashes);
                }
                Profiler.EndSample();
            }
        }

        private static void Calculate(Bounds bounds, float cellFactor, ref HashSet<int> set)
        {
            Profiler.BeginSample("Calculate Hashes");
            var min = bounds.min;
            var max = bounds.max;

            var minHash = Hash(min, cellFactor);
            var maxHash = Hash(max, cellFactor);

            var delta = new Vector3(Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y), Mathf.Abs(min.z - max.z));
            
            if (minHash == maxHash)
            {
                set.Add(minHash);
            } else if ((Math.Abs(delta.x) > TOLERANCE && Math.Abs(delta.y) < TOLERANCE && Math.Abs(delta.z) < TOLERANCE) ||
                (Math.Abs(delta.x) < TOLERANCE && Math.Abs(delta.y) > TOLERANCE && Math.Abs(delta.z) < TOLERANCE) ||
                (Math.Abs(delta.x) < TOLERANCE && Math.Abs(delta.y) < TOLERANCE && Math.Abs(delta.z) > TOLERANCE))
            {
                set.Add(minHash);
                set.Add(maxHash);
            }
            else if (Math.Abs(delta.x) < TOLERANCE && Math.Abs(delta.y) > TOLERANCE && Math.Abs(delta.z) > TOLERANCE) 
            {
                set.Add(minHash);
                set.Add(Hash(new Vector3(min.x, max.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, min.y, max.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, max.y, max.z), cellFactor));
            } 
            else if (Math.Abs(delta.x) > TOLERANCE && Math.Abs(delta.y) < TOLERANCE && Math.Abs(delta.z) > TOLERANCE) 
            {
                set.Add(minHash);
                set.Add(Hash(new Vector3(max.x, min.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, min.y, max.z), cellFactor));
                set.Add(Hash(new Vector3(max.x, min.y, max.z), cellFactor));
            }
            else if (Math.Abs(delta.x) > TOLERANCE && Math.Abs(delta.y) > TOLERANCE && Math.Abs(delta.z) < TOLERANCE)
            {
                set.Add(minHash);
                set.Add(Hash(new Vector3(max.x, min.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, max.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(max.x, max.y, min.z), cellFactor));
            }
            else
            {
                set.Add(minHash);
                
                set.Add(Hash(new Vector3(min.x, max.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, min.y, max.z), cellFactor));
                set.Add(Hash(new Vector3(min.x, max.y, max.z), cellFactor));
                
                set.Add(Hash(new Vector3(max.x, min.y, min.z), cellFactor));
                set.Add(Hash(new Vector3(max.x, min.y, max.z), cellFactor));
                set.Add(Hash(new Vector3(max.x, max.y, min.z), cellFactor));
                
                set.Add(maxHash);
            }
            Profiler.EndSample();
        }
        
        private static int FastFloor(float f)
        {
            return (int)(f + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
        }

        private static int Hash(Vector3 pos, float cellFactor)
        {
            return (FastFloor(pos.x * cellFactor) * 73856093) ^ 
                   (FastFloor(pos.y * cellFactor) * 19349663) ^ 
                   (FastFloor(pos.z * cellFactor) * 83492791);
        }
    }
}