using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Example3
{
    public class Manager1 : BaseManager
    {
        private const string KERNEL_NAME           = "CSBoidMove";
        private const string PARAM_BOID_SIZE       = "boidsNumber";
        private const string PARAM_0               = "g_params0";
        private const string PARAM_1               = "g_params1";
        private const string PARAM_2               = "g_params2";
        private const string PARAM_IN_BOID_BUFF    = "g_boidIn";
        private const string PARAM_OUT_BOID_BUFF   = "g_boidOut";
        private const string PARAM_CONSUME_BUFF    = "g_consume";

        [SerializeField] private GameObject boidPrefab;
        
        private int kernelIndex;
        
        private bool isEvenFrame = true;
        private BoidData[] data;
        private ComputeBuffer boidsEvenFrameBuffer;
        private ComputeBuffer boidsOddFrameBuffer;
        private ComputeBuffer consumeBuffer;
        private int[] consumeIds;

        private readonly List<Transform> boidsTr = new List<Transform>();

        private ComputeBuffer OutBoidsBuffer => !isEvenFrame ? boidsEvenFrameBuffer : boidsOddFrameBuffer;
        private ComputeBuffer InBoidsBuffer  =>  isEvenFrame ? boidsEvenFrameBuffer : boidsOddFrameBuffer;
        private ComputeBuffer ConsumeBuffer  =>  consumeBuffer;

        // Use this for initialization
        protected override void Init()
        {
            kernelIndex = shader.FindKernel(KERNEL_NAME);
            InitBoidsBuffer();
        }

        // Update is called once per frame
        protected override void UpdateFrame()
        {
            ComputeStepFrame();
            
            OutBoidsBuffer.GetData(data);
            for (var i = 0; i < boidsNumber; i++)
            {
                var tr = boidsTr[i];
                var d = data[i];
   
                tr.localRotation = d.rotation;
                tr.localPosition = d.position;
            }
            isEvenFrame = !isEvenFrame;
            
        }

        private void OnDestroy()
        {
            boidsEvenFrameBuffer.Release();
            boidsOddFrameBuffer.Release();
            consumeBuffer.Release();
        }

        private void InitBoidsBuffer()
        {
            if (boidsNumber == 0) return;

            boidsEvenFrameBuffer = new ComputeBuffer(boidsNumber, sizeof(float) * 8);
            boidsOddFrameBuffer = new ComputeBuffer(boidsNumber, sizeof(float) * 8);
            consumeBuffer = new ComputeBuffer(boidsNumber, sizeof(int), ComputeBufferType.Append);
           
            var item = (int) Mathf.Pow(boidsNumber, SQR_3);

            var xMax = Mathf.Max(item, 1);
            var yMax = Mathf.Max(item, 1);
            var zMax = boidsNumber / (yMax * xMax) + 1;

            var count = 0;
            data = new BoidData[boidsNumber];
            consumeIds = new int[boidsNumber];
            for (var x = 0; x < xMax; ++x)
            for (var y = 0; y < yMax; ++y)
            for (var z = 0; z < zMax; ++z)
            {
                if (count >= boidsNumber) break;

                var boidData = new BoidData
                {
                    rotation = transform.rotation,
                    position = new Vector3(x - xMax * 0.5f, y - yMax * 0.5f, z - zMax * 0.5f),
                    speed = Random.Range(minSpeed, maxSpeed)
                };
                data[count] = boidData;
                consumeIds[count] = count;

                var go = Instantiate(boidPrefab, boidData.position, transform.rotation, transform);
                go.name = "Boid_" + count;

                boidsTr.Add(go.transform);

                count++;
            }

            var buffer = InBoidsBuffer;
            buffer.SetData(data);
            buffer.SetCounterValue((uint) boidsNumber);
  
            consumeBuffer.SetData(consumeIds);
            consumeBuffer.SetCounterValue((uint) boidsNumber);
        }

        private void ComputeStepFrame()
        {
            shader.SetInt(PARAM_BOID_SIZE, boidsNumber);
            shader.SetFloats(PARAM_0, Goal.x, Goal.y, Goal.z, Time.deltaTime);
            shader.SetFloats(PARAM_1, minFlockRadius, maxFlockRadius, minSpeed, maxSpeed);
            shader.SetFloats(PARAM_2, centerFactor, avoidFactor, directionFactor);

            ConsumeBuffer.SetCounterValue((uint)boidsNumber);
            InBoidsBuffer.SetCounterValue((uint)boidsNumber);
            OutBoidsBuffer.SetCounterValue(0);

            // Do Boid Pass            
            shader.SetBuffer(kernelIndex, PARAM_IN_BOID_BUFF,  InBoidsBuffer);
            shader.SetBuffer(kernelIndex, PARAM_OUT_BOID_BUFF, OutBoidsBuffer);
            shader.SetBuffer(kernelIndex, PARAM_CONSUME_BUFF,  ConsumeBuffer);
            shader.Dispatch(kernelIndex, boidsNumber / 10 + (boidsNumber % 10 != 0 ? 1 : 0), 1, 1);
        }

        private struct BoidData
        {
            public Quaternion rotation;
            public Vector3 position;
            public float speed;

            public override string ToString()
            {
                return $"[{rotation}, {position}, {speed}]";
            }
        }
    }
}