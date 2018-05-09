using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace Example3
{
    public class Manager4 : BaseManager
    {
        private const string MOVE_BOID = "MoveBoids";
        private const string SORT_HASH = "SortBoidsHash";
        
        private const string KERNEL_NAME_MOVE = "CSBoidMove";

        private const string PARAM_0 = "g_params0";
        private const string PARAM_1 = "g_params1";
        private const string PARAM_2 = "g_params2";
        private const string PARAM_UINT = "g_params_uint";
        private const string PARAM_IN_BOID_BUFF = "g_boidIn";
        private const string PARAM_OUT_BOID_BUFF = "g_boidOut";
        private const string PARAM_OUT_KEY_BUFF = "g_keyOut";
        private const string PARAM_IN_KEY_BUFF = "g_keyIn";

        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private ComputeShader gpuSortShader;
        [SerializeField] private float cellSize = 2;

        private int kernelIndexMove;
        private int totalThreadsInBlock;

        private BoidData[] data;
        private ComputeBuffer inBoidsBuffer;
        private ComputeBuffer outBoidsBuffer;
        private ComputeBuffer inKeysBuffer;
        private ComputeBuffer outKeysBuffer;
        
        private GpuSort gpuSort;
        private readonly List<Transform> boidsTr = new List<Transform>();

        // Use this for initialization
        protected override void Init()
        {
            gpuSort = new GpuSort(gpuSortShader);
            kernelIndexMove = shader.FindKernel(KERNEL_NAME_MOVE);
            totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIndexMove);
            InitBoidsBuffer();
        }

        // Update is called once per frame
        protected override void UpdateFrame()
        {
            ComputeStepFrame();

            outBoidsBuffer.GetData(data);
            for (var i = 0; i < boidsNumber; i++)
            {
                var tr = boidsTr[i];
                var d = data[i];

                tr.localRotation = d.rotation;
                tr.localPosition = d.position;
            }

            Swap(ref inBoidsBuffer, ref outBoidsBuffer);
            Swap(ref inKeysBuffer, ref outKeysBuffer);
        }

        private void OnDestroy()
        {
            outBoidsBuffer.Release();
            inBoidsBuffer.Release();
            outKeysBuffer.Release();
            inKeysBuffer.Release();
        }

        private void InitBoidsBuffer()
        {
            if (boidsNumber == 0) return;

            inKeysBuffer = new ComputeBuffer(boidsNumber, sizeof(int) * 2);
            outKeysBuffer = new ComputeBuffer(boidsNumber, sizeof(int) * 2);
            inBoidsBuffer = new ComputeBuffer(boidsNumber, sizeof(float) * 8);
            outBoidsBuffer = new ComputeBuffer(boidsNumber, sizeof(float) * 8);

            var item = (int) Mathf.Pow(boidsNumber, SQR_3);

            var xMax = Mathf.Max(item, 1);
            var yMax = Mathf.Max(item, 1);
            var zMax = boidsNumber / (yMax * xMax) + 1;

            data = new BoidData[boidsNumber];
            
            var count = -1;
            for (var x = 0; x < xMax; ++x)
            for (var y = 0; y < yMax; ++y)
            for (var z = 0; z < zMax; ++z)
            {
                ++count;
                if (count >= boidsNumber) break;

                var boidData = new BoidData
                {
                    rotation = transform.rotation,
                    position = new Vector3(x - xMax * 0.5f, y - yMax * 0.5f, z - zMax * 0.5f),
                    speed = Random.Range(minSpeed, maxSpeed)
                };
                data[count] = boidData;

                var go = Instantiate(boidPrefab, boidData.position, transform.rotation, transform);
                go.name = "Boid_" + count;

                boidsTr.Add(go.transform);
            }

            inBoidsBuffer.SetData(data);
        }

        private void ComputeStepFrame()
        {
            if (boidsNumber == 0) return;
            var grid = Utils.TrimToBlock(boidsNumber, totalThreadsInBlock);
            
            Profiler.BeginSample(MOVE_BOID);
            shader.SetInts(PARAM_UINT, grid, 1, 1, boidsNumber);

            shader.SetFloats(PARAM_0, Goal.x, Goal.y, Goal.z, Time.deltaTime);
            shader.SetFloats(PARAM_1, minFlockRadius, maxFlockRadius, minSpeed, maxSpeed);
            shader.SetFloats(PARAM_2, centerFactor, avoidFactor, directionFactor, 1.0f / cellSize);

            // boid move step
            shader.SetBuffer(kernelIndexMove, PARAM_IN_KEY_BUFF, inKeysBuffer);
            shader.SetBuffer(kernelIndexMove, PARAM_OUT_KEY_BUFF, outKeysBuffer);
            shader.SetBuffer(kernelIndexMove, PARAM_IN_BOID_BUFF, inBoidsBuffer);
            shader.SetBuffer(kernelIndexMove, PARAM_OUT_BOID_BUFF, outBoidsBuffer);
            shader.Dispatch(kernelIndexMove, grid, 1, 1);
            Profiler.EndSample();
           
            Profiler.BeginSample(SORT_HASH);
            gpuSort.Sort(outKeysBuffer, boidsNumber);
            Profiler.EndSample();
        }

        private struct BoidData
        {
            public Quaternion rotation;
            public Vector3 position;

            public float speed;

            public override string ToString()
            {
                return $"p:{position}; s:{speed}";
            }
        }
    }
}