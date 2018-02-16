using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using Smooth.Pools;
using UnityEngine;
using UnityEngine.Profiling;

namespace Example2.ParalelSpatialHash
{
    internal class ParalelUpdateSpatialHashsSystem : ReactiveSystem<GameEntity>
    {
        private const float TOLERANCE = 0.05f;
        private const int BIG_ENOUGH_INT = 16 * 1024;
        private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT + 0.0000;

        private readonly GameContext context;

        internal ParalelUpdateSpatialHashsSystem(Contexts contexts) : base(contexts.game)
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
            Profiler.BeginSample("UpdateSpatials");
            foreach (var result in entities.AsParallel().Select(e => Update(e, cellFactor)))
            {
                if (!result.Item3)
                    result.Item1.AddSpatialHashes(result.Item2);
                else if (result.Item4)
                {
                    var oldSet = result.Item1.spatialHashes.value;
                    result.Item1.ReplaceSpatialHashes(result.Item2);
                    HashSetPool<int>.Instance.Release(oldSet);
                }
            }
            Profiler.EndSample();
        }

        private static Tuple<GameEntity, HashSet<int>, bool, bool> Update(GameEntity boid, float cellFactor)
        {   
            var hashes = HashSetPool<int>.Instance.Borrow();

            Calculate(boid.bounds.value, cellFactor, ref hashes);

            if (!boid.hasSpatialHashes)
                return Tuple.Create(boid, hashes, false, false);

            var oldHashes = boid.spatialHashes.value;
            if (!oldHashes.SetEquals(hashes))
            {
                return Tuple.Create(boid, hashes, true, true);
            }
            HashSetPool<int>.Instance.Release(hashes);
            return Tuple.Create(boid, (HashSet<int>)null, true, false);
        }

        private static void Calculate(Bounds bounds, float cellFactor, ref HashSet<int> set)
        {
            var min = bounds.min;
            var max = bounds.max;

            var minHash = Hash(min, cellFactor);
            var maxHash = Hash(max, cellFactor);

            var delta = new Vector3(Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y), Mathf.Abs(min.z - max.z));

            if (minHash == maxHash)
            {
                set.Add(minHash);
            }
            else if ((Math.Abs(delta.x) > TOLERANCE && Math.Abs(delta.y) < TOLERANCE &&
                      Math.Abs(delta.z) < TOLERANCE) ||
                     (Math.Abs(delta.x) < TOLERANCE && Math.Abs(delta.y) > TOLERANCE &&
                      Math.Abs(delta.z) < TOLERANCE) ||
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
        }

        private static int FastFloor(float f)
        {
            return (int) (f + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
        }

        private static int Hash(Vector3 pos, float cellFactor)
        {
            return (FastFloor(pos.x * cellFactor) * 73856093) ^
                   (FastFloor(pos.y * cellFactor) * 19349663) ^
                   (FastFloor(pos.z * cellFactor) * 83492791);
        }
    }
}