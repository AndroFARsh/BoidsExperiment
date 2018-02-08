using Entitas;
using Example2.Utils;
using Smooth.Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Example2.SpatialHash
{
    internal class SpatialHashBoidsMoveSystem : IExecuteSystem
    {
        
        private readonly SpatialHashIndexer indexer;
        private readonly GameEntity world;
        private readonly IGroup<GameEntity> group;

        internal SpatialHashBoidsMoveSystem(Contexts contexts, SpatialHashIndexer spatialIndexer)
        {
            indexer = spatialIndexer;
            world = contexts.game.worldEntity;
            group = contexts.game.GetGroup(GameMatcher.Boid);
        }

        public void Execute()
        {
            var config = world.config.value;
            var frames = config.MaxFrameSplit;
            var frame = world.hasFrame ? world.frame.value : 0;
            var goal = world.goal.value;
            for (var i = 0; i < @group.count; i++)
            {
                if (i % frames != frame)
                    SimpleUpdate(@group.GetEntities()[i]);
                else
                    FullUpdate(@group.GetEntities()[i], goal, config);
            }
                
            frame++;
            if (frame >= frames)
                frame = 0;
            
            if (world.hasFrame)
                world.ReplaceFrame(frame);    
            else 
                world.AddFrame(frame);
        }
        
        private static void SimpleUpdate(GameEntity boid)
        {
            var rotation = boid.rotation.value;
            var position = boid.position.value;
            var speed = boid.speed.value;

            position += rotation * Vector3.forward * Time.deltaTime * speed;
            
            boid.ReplacePosition(position);
        }

        private void FullUpdate(GameEntity boid, Vector3 goal, IConfig config)
        {
            var hashes = boid.spatialHashes.value;
            var rotation = boid.rotation.value;
            var position = boid.position.value;
            var direction = (goal - position).ApproximateNormalize();
            var center = Vector3.zero;
            var avoid = Vector3.zero;
            var speed = 0.0f;

            var count = 0;

            var distinct = HashSetPool<GameEntity>.Instance.Borrow();
            distinct.Add(boid);
            var hashesEn = hashes.GetEnumerator();
            while (hashesEn.MoveNext())
            {
                var boidsEn = indexer.FindGameEntitiesForHash(hashesEn.Current).GetEnumerator();
                while (boidsEn.MoveNext())
                {
                    var otherBoid = boidsEn.Current;
                    if (distinct.Contains(otherBoid)) continue;

                    distinct.Add(otherBoid);
                    count++;

                    var otherPosition = otherBoid.position.value;

                    speed += otherBoid.speed.value;
                    center += otherPosition;

                    var distance = position.ApproximateDistanceTo(otherPosition);
                    if (distance > 0 && distance < config.MinFlockRadius)
                        avoid += ((position - otherPosition).ApproximateNormalize() / distance);
                }
                boidsEn.Dispose();
            }
            hashesEn.Dispose();
            HashSetPool<GameEntity>.Instance.Release(distinct);
            
            if (count != 0)
            {
                center = (center / count - position).ApproximateNormalize();
                avoid = (avoid / count).ApproximateNormalize();
            }

            speed = count > 0 ? speed / count : Random.Range(config.MinSpeed, config.MaxSpeed);
            speed = Mathf.Clamp(speed, config.MinSpeed, config.MaxSpeed);
            direction = direction * config.DirectionFactor +
                        center * config.CenterFactor +
                        avoid * config.AvoidFactor;


            var newRotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);
            var newPosition = position + newRotation * Vector3.forward * Time.deltaTime * speed;
            
            boid.ReplaceRotation(newRotation);
            boid.ReplacePosition(newPosition);
            boid.ReplaceSpeed(speed);
        }
    }
}