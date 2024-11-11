/// <summary>
/// MaxHeap 类继承自 Heap 类，用于表示一个最大堆。
/// 最大堆的特点是每个父节点的值都大于或等于其子节点的值。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. MaxHeap 的初始化，自动设置堆的类型为最大堆（HeapType.MaxHeap）。
/// 2. 使用 IComparable 接口来确保元素可比较。
/// </remarks>
using System;

namespace LDataStruct
{
    public class MaxHeap<T> : Heap<T> where T : IComparable
    {
        /// <summary>
        /// 构造函数，初始化一个最大堆，初始容量为 10。
        /// </summary>
        public MaxHeap() : base(10, HeapType.MaxHeap)
        {
        }
    }
}
