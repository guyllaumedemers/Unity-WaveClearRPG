using System;
using System.Collections;
using UnityEngine;

namespace AStar
{
    public class Heap<T> where T : IHeapItem<T> {
        private readonly T[] _items;
        private int _currentItemCount;
    
        public Heap(int maxHeapSize) {
            _items = new T[maxHeapSize];
        }
    
        public int Count => _currentItemCount;

        public void Add(T item) {
            item.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = item;
            SortUp(item);
            _currentItemCount++;
        }

        public T RemoveFirst() {
            var firstItem = _items[0];
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        public void UpdateItem(T item) {
            SortUp(item);
        }

        public bool Contains(T item) {
            return Equals(_items[item.HeapIndex], item);
        }

        private void SortDown(T item) {
            while (true) {
                var childIndexLeft = item.HeapIndex * 2 + 1;
                var childIndexRight = item.HeapIndex * 2 + 2;
                var swapIndex = 0;

                if (childIndexLeft < _currentItemCount) {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < _currentItemCount)
                        if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
                            swapIndex = childIndexRight;

                    if (item.CompareTo(_items[swapIndex]) < 0)
                        Swap(item, _items[swapIndex]);
                    else
                        return;
                }
                else {
                    return;
                }
            }
        }
    
        private void SortUp(T item) {
            var parentIndex = (item.HeapIndex - 1) / 2;

            while (true) {
                var parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void Swap(T itemA, T itemB) {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            var itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T> {
        int HeapIndex { get; set; }
    }
}