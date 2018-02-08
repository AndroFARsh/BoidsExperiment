    using UnityEngine;

namespace Example1
{
    public class Boid : MonoBehaviour
    {
        private readonly Collider[] temp = new Collider[10];

        // Update is called once per frame
        void Update()
        {
            var manager = Manager.Instance;
            if (!manager.run) return;
            
            var boidData = manager.FindBoidById(transform);
            

            var direction = manager.Goal - boidData.position;
            var center = Vector3.zero;
            var avoid = Vector3.zero;
            var speed = 0.0f;

            var c = 0;
            var count = Physics.OverlapSphereNonAlloc(transform.position, Manager.BoidData.maxFlockRadius, temp);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var otherBoidData = manager.FindBoidById(temp[i].transform);
                    if (otherBoidData.id == 0 || otherBoidData.id == boidData.id) continue;

                    c++;
                    
                    speed += otherBoidData.speed;
                    center += otherBoidData.position;

                    var distance = Vector3.Distance(boidData.position, otherBoidData.position);
                    if (distance > 0 && distance < Manager.BoidData.minFlockRadius)
                        avoid += (boidData.position - otherBoidData.position).normalized / distance;
                }
               
                if (c > 0)
                {
                    center = center / c - boidData.position;
                    speed /= c;
                }
                
                avoid = avoid.normalized;
                direction = direction * Manager.BoidData.directionFactor +
                            center * Manager.BoidData.centerFactor +
                            avoid * Manager.BoidData.avoidFactor;
            }
            else
            {
                speed = boidData.speed;
            }

            boidData.transform.rotation = Quaternion.Slerp(boidData.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * speed);
            boidData.transform.Translate(0, 0, Time.deltaTime * speed);
        }
    }
}