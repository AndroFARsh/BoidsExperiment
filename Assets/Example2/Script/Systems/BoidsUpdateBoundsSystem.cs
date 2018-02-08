using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Example2
{
    public class BoidsUpdateBoundsSystem : ReactiveSystem<GameEntity>
    {
        public BoidsUpdateBoundsSystem(Contexts contexts) : base(contexts.game)
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
            var renderer = boid.renderer.value;
            boid.ReplaceBounds(renderer.bounds);
        }
    }
}