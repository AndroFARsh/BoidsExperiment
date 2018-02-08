using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Profiling;

namespace Example2
{
    public class BoidsUpdateViewSystem : ReactiveSystem<GameEntity>
    {
        internal BoidsUpdateViewSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.Rotation, GameMatcher.Position));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasRenderer;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            for (var i = 0; i < @entities.Count; i++)
                Update(@entities[i]);
        }

        private static void Update(GameEntity boid)
        {
            Profiler.BeginSample("Update");
            var rotation = boid.rotation.value;
            var transform = boid.transform.value;
            var position = boid.position.value;
            
            Profiler.BeginSample("Update Rotation");
            transform.rotation = rotation;
            Profiler.EndSample();
            
            Profiler.BeginSample("Update Position");
            transform.localPosition = position;
            Profiler.EndSample();
            
            Profiler.BeginSample("Update Bounds");
            var renderer = boid.renderer.value;
            boid.ReplaceBounds(renderer.bounds);
            Profiler.EndSample();
            Profiler.EndSample();
        }
    }
}