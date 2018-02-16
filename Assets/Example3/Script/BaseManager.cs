using UnityEngine;

namespace Example3
{
    public abstract class BaseManager : MonoBehaviour
    {
        protected const float SQR_3 = 1.0f / 3.0f;
        
        [Header("In play mode pres 'R' to run/pause")] 
        [SerializeField] protected bool run;

        [Space(20)] 
        [SerializeField] protected float goalRadius = 10;
        
        [SerializeField] protected int boidsNumber = 100;

        [Space(20)] 
        [SerializeField] protected float maxFlockRadius = 5;
        [SerializeField] protected float minFlockRadius = 2;

        [Space(20)] 
        [SerializeField] protected float minSpeed = 2;
        [SerializeField] protected float maxSpeed = 5;

        [Space(20)] 
        [SerializeField] protected float directionFactor = 1;
        [SerializeField] protected float centerFactor = 1;
        [SerializeField] protected float avoidFactor = 2;
       
        [Space(20)]
        [SerializeField] protected ComputeShader shader;
        
        protected Vector3 Goal { get; private set; }

        protected Vector3Int CalcGridDimention(int kernelIndex, int length)
        {
            uint x;
            uint y;
            uint z;
            shader.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);

            var totalThreads = x * y * z;
            var numGroups = length % totalThreads != 0
                ? length / totalThreads + 1
                : length / totalThreads;

            int dimZ, dimY, dimX;
            dimZ = dimY = (int) Mathf.Max(Mathf.Pow(numGroups, SQR_3), 1);
            dimX = Mathf.CeilToInt(numGroups / (float) (dimY * dimZ));
            return new Vector3Int
            {
                x = dimX,
                y = dimY,
                z = dimZ
            };
        }

        private void Start()
        {
            UpdateGoal();

            Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) run = !run;
            
            if (!run) return;

            if (Random.Range(0, 20) < 1) UpdateGoal();

            UpdateFrame();
        }

        private void UpdateGoal()
        {
            Goal = new Vector3
            {
                x = Random.Range(-goalRadius, goalRadius),
                y = Random.Range(-goalRadius, goalRadius),
                z = Random.Range(-goalRadius, goalRadius)
            };
        }

        private void OnDrawGizmosSelected()
        {
            if (!run) return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Goal, 0.1f);
        }


        protected abstract void Init();

        protected abstract void UpdateFrame();
    }
}