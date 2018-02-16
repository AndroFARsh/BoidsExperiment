using System.Runtime.InteropServices;
using UnityEngine;

namespace Example2.Utils
{
    public static class VMath
    {
        public static Vector3 ApproximateNormalize(this Vector3 v)
        {
            var num = ApproximateMagnitude(v);
            if (num > 9.99999974737875E-06)
                return v / num;
           
            return Vector3.zero;
        }
        
        public static float ApproximateMagnitude(this Vector3 v)
        {
            return ASqrt1(v.x * v.x + v.y * v.y + v.z * v.z);
        }
           
        public static float ApproximateDistanceTo(this Vector3 from, Vector3 to) => ApproximateDistance(from, to);

        public static float ApproximateDistance(Vector3 v1, Vector3 v2)
        {
            var dx = v1.x - v2.x;
            var dy = v1.y - v2.y;
            var dz = v1.z - v2.z;

            return ASqrt1(dx * dx + dy * dy + dz * dz);
        }
        
        // More acurate than ASqrt1 but slover
        public static float ASqrt2(float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * z;
            u.f = z;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * z;
        }

        // Less acurate than ASqrt2 but faster 
        public static float ASqrt1(float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp -= 1 << 23; /* Subtract 2^m. */
            u.tmp >>= 1; /* Divide by 2. */
            u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
            return u.f;
        }

        public static float NextFloat(this System.Random rng, float min, float max)
        {
            // Perform arithmetic in double type to avoid overflowing
            var range = min - max;
            var sample = rng.NextDouble();
            var scaled = sample * range + min;
            return (float) scaled;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }
    }
}