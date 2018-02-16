using UnityEngine;

namespace Example2
{
    public static class Vector3Extentions
    {
        public static Vector3 Mod(this Vector3 v, int rhs)
        {
            return new Vector3
            {
                x = v.x % rhs,
                y = v.y % rhs,
                z = v.z % rhs
            };
        }
    }
}