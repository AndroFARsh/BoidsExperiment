using Smooth.Slinq;
using UnityEngine;

namespace Example3
{
    public enum FillArrayType
    {
        Random,
        RandomNonNegative,
        Reversed,
        DubleReversed,
        Sorted
    }

    public static class Utils
    {
        private static readonly System.Random randNum = new System.Random();

        public static void FillArray(int[] array, FillArrayType type, int max = 100)
        {
            Slinqable
                .Range(0, array.Length)
                .Aggregate(array, (ints, i) =>
                {
                    switch (type)
                    {
                        case FillArrayType.Random:
                            ints[i] = randNum.Next(-max, max);
                            break;
                        case FillArrayType.RandomNonNegative:
                            ints[i] = randNum.Next(0, max);
                            break;
                        case FillArrayType.Reversed:
                            ints[i] = ints.Length - i;
                            break;
                        case FillArrayType.DubleReversed:
                            ints[i] = ints.Length - (i % 2 == 0 ? i : i - 1);
                            break;
                        case FillArrayType.Sorted:
                        default:
                            ints[i] = i;
                            break;
                    }
                    return ints;
                });
        }

        public static Quaternion Normalize(this Quaternion q)
        {
            var length = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaternion(q.x / length, q.y / length, q.z / length, q.w / length);
        }

        public static int To1D(Vector3Int index, Vector3Int dimen)
        {
            return index.z * dimen.x * dimen.y + index.y * dimen.x + index.x;
        }

        public static Vector3Int To3D(int index, Vector3Int dimen)
        {
            var z = index / (dimen.x * dimen.y);
            index -= z * dimen.x * dimen.y;
            var y = index / dimen.x;
            var x = index % dimen.x;
            return new Vector3Int(x, y, z);
        }
        
        public static int TrimToBlock(int length, int block)
        {
            return (length % block != 0) ? ((length / block) + 1) : (length / block);
        }
    }
}