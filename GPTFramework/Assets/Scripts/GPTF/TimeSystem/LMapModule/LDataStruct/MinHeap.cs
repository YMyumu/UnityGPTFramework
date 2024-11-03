/// <summary>
/// MinHeap 类继承自 Heap 类，用于表示一个最小堆。
/// 最小堆的特点是每个父节点的值都小于或等于其子节点的值。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. MinHeap 的初始化，自动设置堆的类型为最小堆（HeapType.MinHeap）。
/// 2. 使用 IComparable 接口来确保元素可比较。
/// </remarks>
using System;

namespace LDataStruct
{
    public class MinHeap<T> : Heap<T> where T : IComparable
    {
        /// <summary>
        /// 构造函数，初始化一个最小堆，初始容量为 10。
        /// </summary>
        public MinHeap() : base(10, HeapType.MinHeap)
        {
        }
    }
}
