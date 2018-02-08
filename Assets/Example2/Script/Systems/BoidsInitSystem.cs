using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Example2
{
    public class BoidsInitSystem : IInitializeSystem
    {
        private const float SQR_3 = 1 / 3.0f;
        
        private readonly GameContext context;

        internal BoidsInitSystem(Contexts contexts)
        {
            context = contexts.game;
        }

        public void Initialize()
        {
            var config = context.worldEntity.config.value;
            
            if (config.BoidNumber == 0) return;

            var item = (int) Mathf.Pow(config.BoidNumber, SQR_3);

            var xMax = Mathf.Max(item, 1);
            var yMax = Mathf.Max(item, 1);
            var zMax = config.BoidNumber / (yMax * xMax) + 1;

            var count = 0;
            for (var x = 0; x < xMax; ++x)
            for (var y = 0; y < yMax; ++y)
            for (var z = 0; z < zMax; ++z)
            {
                if (count >= config.BoidNumber) break;

                var go = GameObject.Instantiate(config.BoidPrefab,
                    new Vector3(x - xMax * 0.5f, y - yMax * 0.5f, z - zMax * 0.5f),
                    config.BoidParent.rotation,
                    config.BoidParent);
                go.name = $"Boid_{count}";

                var renderer = go.GetComponent<Renderer>();
                var enity = context.CreateEntity();
                enity.isBoid = true;
                enity.AddId(go.GetInstanceID());
                enity.AddPosition(go.transform.position);
                enity.AddRotation(go.transform.rotation);
                enity.AddBounds(renderer.bounds);
                enity.AddRenderer(renderer);
                enity.AddTransform(go.transform);
                enity.AddSpeed(Random.Range(config.MinSpeed, config.MaxSpeed));
                
                count++;
            }
        }
    }
}