using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Example2
{
    public class GoalInitSystem : IInitializeSystem, IExecuteSystem
    {
        private readonly GameContext context;
        private readonly GameEntity world;

        internal GoalInitSystem(Contexts contexts)
        {
            context = contexts.game;
            world = contexts.game.worldEntity;
        }

        public void Initialize()
        {
            var config = context.worldEntity.config.value;
            world.AddGoal(RandomPoint(config.GoalRadius));
        }

        public void Execute()
        {
            var config = context.worldEntity.config.value;
            if (!config.Run) return;
            
            if (Random.Range(0, 10) < 1)
                world.ReplaceGoal(RandomPoint(config.GoalRadius));
        }
        
        private static Vector3 RandomPoint(float goalRadius) => new Vector3
        {
            x = Random.Range(-goalRadius, goalRadius),
            y = Random.Range(-goalRadius, goalRadius),
            z = Random.Range(-goalRadius, goalRadius)
        };
    }
}