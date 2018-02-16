using UnityEngine;

namespace Example3
{
    public class GpuSort
    {
        private const string KERNEL_NAME_SORT_SCAN = "CSRadixSortScan";
        private const string KERNEL_NAME_SORT_STORE = "CSRadixSortStore";
        private const string KERNEL_NAME_SORT_INDEX = "CSRadixSortIndex";
        private const string KERNEL_NAME_STORE_SUM = "CSBlockSumStore";
        private const string KERNEL_NAME_SCAN_SUM = "CSBlockSumScan";
        private const string KERNEL_NAME_ADD_SUM = "CSBlockSumAdd";

        private const string PARAM_GRID = "g_gridDimen";
        private const string PARAM_DATA_IN = "g_in";
        private const string PARAM_DATA_OUT = "g_out";
        private const string PARAM_DATA_INT3_IN = "g_int4In";
        private const string PARAM_DATA_INT3_OUT = "g_int4Out";
        private const string PARAM_SIZE = "g_size";
        private const string PARAM_SUM_SIZE = "g_sumSize";
        private const string PARAM_MASK = "g_mask";

        private readonly ComputeShader shader;
        private readonly int kernelIdSortScan;
        private readonly int kernelIdSortStore;
        private readonly int kernelIdSortIndex;
        private readonly int kernelIdStoreSum;
        private readonly int kernelIdAddSum;
        private readonly int totalThreadsInBlock;
        private readonly int kernelIdScanSum;

        public GpuSort(ComputeShader s)
        {
            shader = s;
            kernelIdSortScan = shader.FindKernel(KERNEL_NAME_SORT_SCAN);
            kernelIdSortStore = shader.FindKernel(KERNEL_NAME_SORT_STORE);
            kernelIdSortIndex = shader.FindKernel(KERNEL_NAME_SORT_INDEX);
            kernelIdStoreSum = shader.FindKernel(KERNEL_NAME_STORE_SUM);
            kernelIdScanSum = shader.FindKernel(KERNEL_NAME_SCAN_SUM);
            kernelIdAddSum = shader.FindKernel(KERNEL_NAME_ADD_SUM);
            totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIdSortScan);
        }

        public void Sort(ComputeBuffer dataBuffer, int length)
        {
            var gridTotal = Utils.TrimToBlock(length, totalThreadsInBlock);
            var indexBuffer = new ComputeBuffer(length + gridTotal, sizeof(int) * 4);
            for (var i = 0; i < sizeof(int) * 8; i++)
            {
                var mask = 1 << i;

                shader.SetInt(PARAM_MASK, mask);
                shader.SetInt(PARAM_SIZE, length);
                shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

                shader.SetBuffer(kernelIdSortScan, PARAM_DATA_IN, dataBuffer);
                shader.SetBuffer(kernelIdSortScan, PARAM_DATA_INT3_OUT, indexBuffer);
                shader.Dispatch(kernelIdSortScan, gridTotal, 1, 1);

                HandleSumBuffer(indexBuffer, length);

                shader.SetInt(PARAM_MASK, mask);
                shader.SetInt(PARAM_SIZE, length);
                shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

                shader.SetBuffer(kernelIdSortIndex, PARAM_DATA_INT3_IN, indexBuffer);
                shader.SetBuffer(kernelIdSortIndex, PARAM_DATA_INT3_OUT, indexBuffer);
                shader.Dispatch(kernelIdSortIndex, gridTotal, 1, 1);

                // Store
                shader.SetInt(PARAM_MASK, mask);
                shader.SetInt(PARAM_SIZE, length);
                shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

                shader.SetBuffer(kernelIdSortStore, PARAM_DATA_INT3_IN, indexBuffer);
                shader.SetBuffer(kernelIdSortStore, PARAM_DATA_OUT, dataBuffer);
                shader.Dispatch(kernelIdSortStore, gridTotal, 1, 1);
            }
            indexBuffer.Release();
        }

        private void HandleSumBuffer(ComputeBuffer indexBuffer, int length)
        {
            if (length < totalThreadsInBlock) return;

            var size = Utils.TrimToBlock(length, totalThreadsInBlock);
            var gridTotal = Utils.TrimToBlock(size, totalThreadsInBlock);
            var sumBuffer = new ComputeBuffer(size + gridTotal, sizeof(int) * 4);

            StoreBlockSum(indexBuffer, length, gridTotal, sumBuffer);
            ScanBlockSum(size, gridTotal, sumBuffer);

            HandleSumBuffer(sumBuffer, size);
            AddBlockSum(indexBuffer, length, gridTotal, sumBuffer);
            sumBuffer.Release();
        }

        private void AddBlockSum(ComputeBuffer indexBuffer, int length, int gridTotal, ComputeBuffer sumBuffer)
        {
            shader.SetInt(PARAM_SUM_SIZE, sumBuffer.count);
            shader.SetInt(PARAM_SIZE, length);
            shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

            shader.SetBuffer(kernelIdAddSum, PARAM_DATA_INT3_IN, sumBuffer);
            shader.SetBuffer(kernelIdAddSum, PARAM_DATA_INT3_OUT, indexBuffer);
            shader.Dispatch(kernelIdAddSum, gridTotal, 1, 1);
        }

        private void ScanBlockSum(int size, int gridTotal, ComputeBuffer sumBuffer)
        {
            shader.SetInt(PARAM_SIZE, size);
            shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

            shader.SetBuffer(kernelIdScanSum, PARAM_DATA_INT3_IN, sumBuffer);
            shader.SetBuffer(kernelIdScanSum, PARAM_DATA_INT3_OUT, sumBuffer);
            shader.Dispatch(kernelIdScanSum, gridTotal, 1, 1);
        }

        private void StoreBlockSum(ComputeBuffer indexBuffer, int size, int gridTotal, ComputeBuffer sumBuffer)
        {
            shader.SetInt(PARAM_SIZE, size);
            shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

            shader.SetBuffer(kernelIdStoreSum, PARAM_DATA_INT3_IN, indexBuffer);
            shader.SetBuffer(kernelIdStoreSum, PARAM_DATA_INT3_OUT, sumBuffer);
            shader.Dispatch(kernelIdStoreSum, gridTotal, 1, 1);
        }
    }
}