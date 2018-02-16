using System;
using System.Collections.Generic;
using System.IO;
using Smooth.Collections;
using Smooth.Pools;
using Smooth.Slinq;
using UnityEngine;

namespace Example3
{
    public static class LogUtils
    {
        public static void PrintArray<T>(IList<T> data, string title = "", Func<T, string> toString = null,
            int start = 0, int end = -1)
        {
            end = end < start || end < data.Count ? data.Count - 1 : end;
            using (var disposable = StringBuilderPool.Instance.BorrowDisposable())
            {
                Debug.Log(data.SlinqWithIndex()
                    .Where(v => v.Item2 >= start && v.Item2 <= end)
                    .Select(v => v.Item1)
                    .Aggregate(disposable.value.Append(title).Append(" ["),
                        (builder, i) => builder.Append(toString != null ? toString(i) : i.ToString()).Append(", "))
                    .Append(']').ToString());
            }
        }

        public static void PrintArrayToFile<T>(string file, IList<T> data, string title = "",
            Func<T, string> toString = null, int start = 0, int end = -1)
        {
            end = end < start || end < data.Count ? data.Count - 1 : end;
            //if (File.Exists(file))
            
            var sw = File.AppendText(file); 
            sw.Write(title);
            sw.Write(" [");
            data.SlinqWithIndex()
                .Where(v => v.Item2 >= start && v.Item2 <= end)
                .Select(v => v.Item1)
                .ForEach(value =>
                {
                    sw.Write(toString != null ? toString(value) : value.ToString());
                    sw.Write(", ");
                });
            sw.Write("]\n");
            sw.Close();
        }
    }
}