/// <summary>
/// Heap 类是一个泛型堆结构，用于按优先级管理一组数据。
/// 该类支持最大堆和最小堆两种模式，通过比较函数确定元素的排列顺序。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. Insert：向堆中插入一个元素，并确保堆的顺序。
/// 2. DeleteHead：删除堆顶元素（即最大或最小元素）。
/// 3. GetHead：获取堆顶元素（但不删除）。
/// 4. Adjust：调整堆中指定元素的位置，以确保堆的顺序。
/// </remarks>
using System;
using System.Collections.Generic;
using System.Text;

namespace LDataStruct
{
    /// <summary>
    /// 表示堆的类型，MinHeap 表示最小堆，MaxHeap 表示最大堆。
    /// </summary>
    public enum HeapType
    {
        MinHeap,
        MaxHeap
    }

    /// <summary>
    /// 泛型堆类，支持最小堆和最大堆的操作。
    /// 堆是一种二叉树结构，用于快速查找最大值或最小值。
    /// </summary>
    /// <typeparam name="T">堆中存储的元素类型，该类型必须实现 IComparable 接口。</typeparam>
    public class Heap<T> : IDisposable where T : IComparable
    {
        private bool _disposed = false;  // 标识对象是否已被释放，防止重复释放
        protected List<T> itemArray;  // 堆中的元素列表，存储堆的所有元素
        private int capacity;  // 堆的初始容量
        protected int count;  // 堆中当前元素的数量
        public int Count => count;  // 获取堆中当前元素的数量
        private readonly Func<T, T, bool> _comparerFun;  // 比较函数，决定堆的排列顺序（最大堆或最小堆）

        /// <summary>
        /// 构造函数，根据堆的类型初始化堆，并设置容量。
        /// </summary>
        /// <param name="capacity">堆的初始容量。</param>
        /// <param name="heapType">堆的类型，MinHeap 或 MaxHeap。</param>
        public Heap(int capacity, HeapType heapType)
        {
            // 根据堆的类型设置比较函数，最小堆使用小于比较，最大堆使用大于比较
            if (heapType == HeapType.MinHeap)
                _comparerFun = MinComparerFunc;
            else
                _comparerFun = MaxComparerFunc;

            Init(capacity);  // 初始化堆
        }

        /// <summary>
        /// 检查堆是否满足最小堆或最大堆的属性。
        /// </summary>
        /// <returns>返回 true 表示堆有效，false 表示堆无效。</returns>
        public bool CheckHeapValid()
        {
            if (IsEmpty() || Count == 1)
                return true;  // 空堆或仅有一个元素的堆一定满足条件

            for (int i = 2; i <= Count; i++)
            {
                if (_comparerFun(itemArray[i / 2], itemArray[i]))
                    return false;  // 如果父节点不满足堆属性，则堆无效
            }

            return true;
        }

        /// <summary>
        /// 最小堆的比较函数，用于判断父节点是否大于子节点。
        /// </summary>
        private bool MinComparerFunc(T t1, T t2)
        {
            return t1.CompareTo(t2) > 0;
        }

        /// <summary>
        /// 最大堆的比较函数，用于判断父节点是否小于子节点。
        /// </summary>
        private bool MaxComparerFunc(T t1, T t2)
        {
            return t1.CompareTo(t2) < 0;
        }

        /// <summary>
        /// 初始化堆的容量，并准备存储元素。
        /// </summary>
        /// <param name="initCapacity">堆的初始容量。</param>
        void Init(int initCapacity)
        {
            if (initCapacity <= 0)
            {
                throw new IndexOutOfRangeException();
            }

            capacity = initCapacity;
            // 从下标为1开始存放数据（堆的第一个元素不使用下标0）
            itemArray = new List<T>(initCapacity + 1) { default };
            count = 0;
        }

        /// <summary>
        /// 判断堆是否已满。
        /// </summary>
        /// <returns>返回 true 表示堆已满，false 表示堆未满。</returns>
        public bool IsFull()
        {
            return count == capacity;
        }

        /// <summary>
        /// 判断堆是否为空。
        /// </summary>
        /// <returns>返回 true 表示堆为空，false 表示堆不为空。</returns>
        public bool IsEmpty()
        {
            return count == 0;
        }

        /// <summary>
        /// 向堆中插入一个元素，并调整堆的顺序，确保满足堆属性。
        /// </summary>
        /// <param name="item">要插入的元素。</param>
        public void Insert(T item)
        {
            // i指向插入堆后的最后一个元素位置
            itemArray.Add(item);
            count += 1;
            Pop(count);  // 调整堆的顺序，使其满足堆属性
        }

        /// <summary>
        /// 删除堆顶元素（最小堆中为最小值，最大堆中为最大值）。
        /// </summary>
        /// <returns>返回被删除的堆顶元素。</returns>
        public T DeleteHead()
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();
            T deleteItem = itemArray[1];
            if (count > 1)
                itemArray[1] = itemArray[count];
            itemArray.RemoveAt(count);
            count -= 1;
            if (count > 1)
                Sink(1);  // 调整堆顺序
            return deleteItem;
        }

        /// <summary>
        /// 获取堆顶元素（但不删除）。
        /// </summary>
        /// <returns>返回堆顶元素。</returns>
        public T GetHead()
        {
            if (IsEmpty())
            {
                throw new IndexOutOfRangeException("Heap is empty!");
            }

            return itemArray[1];  // 堆顶元素存储在下标为1的位置
        }

        /// <summary>
        /// 调整堆中指定元素的位置，确保堆的顺序。
        /// </summary>
        /// <param name="item">需要调整的元素。</param>
        public void Adjust(T item)
        {
            // 获取指定元素的索引
            int index = GetItemIndex(item);
            if (index == -1)
                return;

            // 如果元素与父节点相比不符合堆属性，则调整其位置
            if (_comparerFun(item, itemArray[index / 2]))
            {
                Sink(index);  // 下沉操作
            }
            else
            {
                Pop(index);  // 上浮操作
            }
        }

        /// <summary>
        /// 获取指定元素在堆中的索引位置。
        /// </summary>
        /// <param name="item">需要查找的元素。</param>
        /// <returns>返回元素的索引，如果不存在则返回 -1。</returns>
        public int GetItemIndex(T item)
        {
            int index = -1;
            for (int i = 1; i <= count; i++)
            {
                if (itemArray[i].CompareTo(item) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 上浮操作，将元素调整到堆中的正确位置。
        /// </summary>
        private void Pop(int index)
        {
            T targetItem = itemArray[index];
            while (index > 1 && _comparerFun(itemArray[index / 2], targetItem))
            {
                var parentIndex = index / 2;
                itemArray[index] = itemArray[parentIndex];
                index = parentIndex;
            }

            itemArray[index] = targetItem;
        }

        /// <summary>
        /// 下沉操作，将元素调整到堆中的正确位置。
        /// </summary>
        /// <param name="index">需要调整的元素的索引。</param>
        protected void Sink(int index)
        {
            T targetItem = itemArray[index];
            int parent = index;
            // 节点的左子节点为 2 * index，右子节点为 2 * index + 1
            while (parent * 2 <= count)
            {
                var child = parent * 2;
                // 根据堆的类型，选择左右子节点中符合条件的节点
                if (child != count && _comparerFun(itemArray[child], itemArray[child + 1]))
                    child++;
                if (_comparerFun(itemArray[child], targetItem))
                    break;
                itemArray[parent] = itemArray[child];
                parent = child;
            }

            itemArray[parent] = targetItem;
        }

        /// <summary>
        /// 删除指定的元素，并调整堆的顺序。
        /// </summary>
        /// <param name="item">需要删除的元素。</param>
        /// <returns>返回是否成功删除元素。</returns>
        public bool Delete(T item)
        {
            if (IsEmpty())
                throw new InvalidOperationException("Heap is empty");

            int index = GetItemIndex(item);
            if (index == -1)
                return false;

            if (count == 1)
            {
                itemArray.RemoveAt(1);
                count--;
                return true;
            }

            if (index == count)
            {
                itemArray.RemoveAt(count);
                count--;
                return true;
            }

            T lastItem = itemArray[count];
            itemArray[count] = itemArray[index];
            itemArray[index] = lastItem;
            itemArray.RemoveAt(count);
            count--;

            if (_comparerFun(lastItem, item))
            {
                Sink(index);  // 下沉
            }
            else
            {
                Pop(index);  // 上浮
            }

            return true;
        }

        /// <summary>
        /// 清空堆中的所有元素，并释放内存。
        /// </summary>
        public void Clear()
        {
            itemArray.Clear();
            itemArray.Add(default);
            count = 0;
        }

        /// <summary>
        /// 释放堆的资源，防止内存泄漏。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 获得所有元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T GetItemAt(int index)
        {
            if (index < 1 || index > count)
            {
                throw new IndexOutOfRangeException();
            }
            return itemArray[index];  // 直接返回对应的堆元素
        }



        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Clear();
                itemArray = null;
            }
            _disposed = true;
        }

        ~Heap()
        {
            Dispose(false);
        }
    }
}
