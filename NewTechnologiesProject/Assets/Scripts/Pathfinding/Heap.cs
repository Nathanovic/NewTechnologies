using UnityEngine;
using System.Collections;
using System;

//heap: verzameling van dingen waarvan het makkelijk is om het item met de waarde te vinden
public class Heap<T> where T : IHeapItem<T>{

	private T[] items;
	int currentItemCount;

	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];
	}

	public void Add(T item){
		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;
		SortUp (item);
		currentItemCount++;
	}

	public T RemoveFirst(){
		T firstItem = items [0];
		currentItemCount--;
		items [0] = items [currentItemCount];
		items [0].HeapIndex = 0;
		SortDown (items [0]);
		return firstItem;
	}

	public void UpdateItem(T item){
		SortUp (item);
	}

	public int Count{
		get{ 
			return currentItemCount;
		}
	}

	public bool Contains(T item){
		return Equals (items[item.HeapIndex], item);
	}

	private void SortDown(T item){
		while (true) {
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = childIndexLeft + 1;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					//choose the child with the lowest FCost:
					if (items [childIndexLeft].CompareTo (items [childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				//if the lowest FCost-child has lower FCost than item, swap item with child
				if (item.CompareTo (items [swapIndex]) < 0) {
					Swap (item, items [swapIndex]);
				} else {
					return;				
				}
			} 
			else {
				return;
			}
		}
	}

	private void SortUp(T item){
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true) {
			T parentItem = items [parentIndex];
			//if parentItem has a lower FCost than item, swap item with its parent
			if (item.CompareTo (parentItem) > 0) {//parentItem has a higher priority than item = lower FCost
				Swap (item, parentItem);
			} 
			else {//my heapIndex is correct!
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	private void Swap(T itemA, T itemB){
		int itemAHeapIndex = itemA.HeapIndex;
		int itemBHeapIndex = itemB.HeapIndex;

		items [itemAHeapIndex] = itemB;
		items [itemBHeapIndex] = itemA;

		itemA.HeapIndex = itemBHeapIndex;
		itemB.HeapIndex = itemAHeapIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	/// <value>The index of the heap</value>
	int HeapIndex {
		get;
		set;
	}
}