using System.Collections.Generic;
using UnityEngine;

namespace Example1
{
	public class Manager : Singleton<Manager>
	{
		private const float SQR_3 = 1 / 3.0f;

		public GameObject boidPrefab;
		public int boidNumber;

		[Space(20)] 
		public float goalRadius = 10;
		public float minSpeed = 2;
		public float maxSpeed = 5;
	
		[Space(20)] 
		public float maxFlockRadius = 5;
		public float minFlockRadius = 2;

		[Space(20)] 
		public float directionFactor = 1;
		public float centerFactor = 1;
		public float avoidFactor = 5;

		[Space(20)] 
		public bool run;
		
		private HashSet<BoidData> boids = new HashSet<BoidData>();
		public Vector3 Goal { get; private set; }

		public BoidData FindBoidById(Transform transform)
		{
			foreach (var boid in boids)
			{
				if (boid.transform == transform)
					return boid;
			}
			return default(BoidData);
		}

		// Use this for initialization
		void Start()
		{
			if (boidNumber == 0) return;

			var item = (int) Mathf.Pow(boidNumber, SQR_3);

			var xMax = Mathf.Max(item, 1);
			var yMax = Mathf.Max(item, 1);
			var zMax = boidNumber / (yMax * xMax) + 1;

			var count = 0;
			for (var x = 0; x < xMax; ++x)
			for (var y = 0; y < yMax; ++y)
			for (var z = 0; z < zMax; ++z)
			{
				if (count >= boidNumber) break;

				var go = Instantiate(boidPrefab,
					new Vector3(x - xMax * 0.5f, y - yMax * 0.5f, z - zMax * 0.5f),
					transform.rotation,
					transform);
				go.name = $"Boid_{count}";
				boids.Add(go);
				count++;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (!run) return;
			
			if (Random.Range(0, 6) < 1) UpdateGoal();
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

		public struct BoidData
		{
			public int id => transform.GetInstanceID();
			public readonly Transform transform;
			public readonly float speed;

			public Vector3 position => transform.position;
			public static float maxFlockRadius => Instance.maxFlockRadius;
			public static float minFlockRadius => Instance.minFlockRadius;

			public static float directionFactor => Instance.directionFactor;
			public static float centerFactor => Instance.centerFactor;
			public static float avoidFactor => Instance.avoidFactor;

			public override int GetHashCode()
			{
				return transform.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return transform.Equals(((BoidData)obj).transform);
			}

			private BoidData(Transform tr)
			{
				transform = tr;
				speed = Random.Range(Instance.minSpeed, Instance.maxSpeed);
			}

			public static implicit operator BoidData(GameObject go) => new BoidData(go.transform);
			
			public static implicit operator BoidData(Transform tr) => new BoidData(tr);
		}
	}
}