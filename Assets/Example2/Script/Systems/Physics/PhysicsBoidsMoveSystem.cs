using System;
using Entitas;
using Example2.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Example2.UnityPhysics
{
    internal class PhysicsBoidsMoveSystem : IExecuteSystem
    {
        private static readonly Collider[] temp = new Collider[30];

        private readonly int layerMask;
        private readonly GameContext context;
        private readonly GameEntity world;
        private readonly IGroup<GameEntity> group;

        internal PhysicsBoidsMoveSystem(Contexts contexts, int lMask)
        {
            layerMask = lMask;
            world = contexts.game.worldEntity;
            context = contexts.game;
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
            var rotation = boid.rotation.value;
            var position = boid.position.value;
            var direction = (goal - position).ApproximateNormalize();
            var center = Vector3.zero;
            var avoid = Vector3.zero;
            var speed = 0.0f;

            var number = 0;
            var count = Physics.OverlapSphereNonAlloc(position, config.MaxFlockRadius, temp, layerMask);

            for (var i = 0; i < count; i++)
            {
                var otherBoid = context.GetEntityWithId(temp[i].gameObject.GetInstanceID());
                if (otherBoid == null || otherBoid.id.value == boid.id.value) continue;

                number++;

                var otherPosition = otherBoid.position.value;

                speed += otherBoid.speed.value;
                center += otherPosition;

                var distance = position.ApproximateDistanceTo(otherPosition);
                if (distance > 0 && distance < config.MinFlockRadius)
                    avoid += ((position - otherPosition).ApproximateNormalize() / distance);
            }

            if (number != 0)
            {
                center = (center / number - position).ApproximateNormalize();
                avoid = (avoid / number).ApproximateNormalize();
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