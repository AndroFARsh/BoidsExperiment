using Entitas;
using Example2.ParalelSpatialHash;
using Example2.SpatialHash;
using Example2.UnityPhysics;
using Smooth.Pools;
using Smooth.Slinq;
using UnityEngine;

namespace Example2
{
    public class Manager : MonoBehaviour, IConfig
    {
        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private int boidNumber = 100;
        
        [Space(20)] 
        [SerializeField] private BoidSystemType boidSystemType = BoidSystemType.Physics;
        [Range(0.1f, 5)]
        [SerializeField] private float cellSize = 1;
        [Range(1, 5)]
        [SerializeField] private int splitFrame = 1;
        
        [Space(20)] 
        [SerializeField] private float goalRadius = 10;
        [SerializeField] private float minSpeed = 2;
        [SerializeField] private float maxSpeed = 5;

        [Space(20)] 
        [SerializeField] private float maxFlockRadius = 5;
        [SerializeField] private float minFlockRadius = 2;

        [Space(20)] 
        [SerializeField] private float directionFactor = 1;
        [SerializeField] private float centerFactor = 1;
        [SerializeField] private float avoidFactor = 5;

        [Space(20)] 
        [SerializeField] private bool run;
        
        public int BoidNumber => boidNumber;
        public Transform BoidParent => transform;
        public GameObject BoidPrefab => boidPrefab;
        public float GoalRadius => goalRadius;
        public float MaxSpeed => maxSpeed;
        public float MinSpeed => minSpeed;
        public float MaxFlockRadius => maxFlockRadius;
        public float MinFlockRadius => minFlockRadius;
        public float DirectionFactor => directionFactor;
        public float CenterFactor => centerFactor;
        public float AvoidFactor => avoidFactor;
        public float CellFactor => 1 / cellSize ;
        public BoidSystemType BoidSystemType => boidSystemType;
        public int MaxFrameSplit => splitFrame;

        public bool Run => run;

        private Systems systems;

        // Use this for initialization
        private void Start()
        {
            var contexts = Contexts.sharedInstance;

            contexts.game.isWorld = true;
            contexts.game.worldEntity.AddConfig(this);

            var indexer = new SpatialHashIndexer(contexts);
            
            systems = new Feature()
                .Add(new BoidsInitSystem(contexts))
                .Add(new GoalInitSystem(contexts))

                .Add(new PhysicsBoidsMoveSystems(contexts, LayerMask.GetMask("Boid")))
                .Add(new SpatialHashBoidsSystems(contexts, indexer))
                .Add(new ParalelSpatialHashBoidsSystems(contexts, indexer))

                .Add(new BoidsUpdateViewSystem(contexts));

            systems.Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
            systems.Execute();
            systems.Cleanup();
        }

        private void OnDrawGizmosSelected()
        {
            var contexts = Contexts.sharedInstance;
            if (contexts.game.isWorld && contexts.game.worldEntity.hasGoal)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(contexts.game.worldEntity.goal.value, 0.5f);

                contexts.game.GetGroup(GameMatcher.Boid)
                    .Slinq()
                    .Where(e => e.hasBounds)
                    .Select(e => e.bounds.value)
                    .ForEach(DrawBounds, Color.magenta);
            }
        }

        private static void DrawBounds(Bounds bounds, Color color)
        {
            Gizmos.color = color;

            var center = bounds.center;
            var extents = bounds.extents;
            var points = ListPool<Vector3>.Instance.Borrow();
            points.Add(center + new Vector3( extents.x,  extents.y,  extents.z));
            points.Add(center + new Vector3( extents.x,  extents.y, -extents.z));
            points.Add(center + new Vector3( extents.x, -extents.y,  extents.z));
            points.Add(center + new Vector3( extents.x, -extents.y, -extents.z));
            points.Add(center + new Vector3(-extents.x,  extents.y,  extents.z));
            points.Add(center + new Vector3(-extents.x,  extents.y, -extents.z));
            points.Add(center + new Vector3(-extents.x, -extents.y,  extents.z));
            points.Add(center + new Vector3(-extents.x, -extents.y, -extents.z));
            
            Gizmos.DrawLine(points[7], points[5]);
            Gizmos.DrawLine(points[3], points[1]);
            Gizmos.DrawLine(points[6], points[4]);
            Gizmos.DrawLine(points[2], points[0]);
            
            Gizmos.DrawLine(points[5], points[1]);
            Gizmos.DrawLine(points[7], points[3]);
            Gizmos.DrawLine(points[4], points[0]);
            Gizmos.DrawLine(points[6], points[2]);
            
            Gizmos.DrawLine(points[5], points[4]);
            Gizmos.DrawLine(points[1], points[0]);
            Gizmos.DrawLine(points[7], points[6]);
            Gizmos.DrawLine(points[3], points[2]);
            
            ListPool<Vector3>.Instance.Release(points);
        }
    }
}