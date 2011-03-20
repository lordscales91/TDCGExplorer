using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    //
    //  Priority queue based on an index into a set of keys. The queue is
    //  maintained as a 2-way heap.
    //
    //  The priority in this implementation is the lowest valued key
    class IndexedPriorityQueueLow
    {
        List<float> keys;
        List<int> heap;
        List<int> invHeap;
        int size, maxSize;

        void Swap(int a, int b)
        {
            int tmp = heap[a];
            heap[a] = heap[b];
            heap[b] = tmp;

            //change the handles too
            invHeap[heap[a]] = a;
            invHeap[heap[b]] = b;
        }
        
        void ReorderUpwards(int nd)
        {
            //move up the heap swapping the elements until the heap is ordered
            while (nd > 1 && keys[heap[nd / 2]] > keys[heap[nd]])
            {
                Swap(nd / 2, nd);
                nd /= 2;
            }
        }
        
        void ReorderDownwards(int nd, int heapSize)
        {
            //move down the heap from node nd swapping the elements until
            //the heap is reordered
            while (2 * nd <= heapSize)
            {
                int child = 2 * nd;

                //set child to smaller of nd's two children
                if (child < heapSize && keys[heap[child]] > keys[heap[child + 1]])
                {
                    ++child;
                }

                //if this nd is larger than its child, swap
                if (keys[heap[nd]] > keys[heap[child]])
                {
                    Swap(child, nd);

                    //move the current node down the tree
                    nd = child;
                }
                else
                {
                    break;
                }
            }
        }

        public IndexedPriorityQueueLow(List<float> keys, int maxSize)
        {
            this.keys = keys;
            this.maxSize = maxSize;
            this.size = 0;
            
            this.heap = new List<int>();
            for (int i = 0; i < maxSize + 1; ++i)
                heap.Add(0);

            this.invHeap = new List<int>();
            for (int i = 0; i < maxSize + 1; ++i)
                invHeap.Add(0);
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        //to insert an item into the queue it gets added to the end of the heap
        //and then the heap is reordered from the bottom up.
        public void Enqueue(int index)
        {
            ++size;
            heap[size] = index;
            invHeap[index] = size;
            ReorderUpwards(size);
        }

        //to get the min item the first element is exchanged with the lowest
        //in the heap and then the heap is reordered from the top down. 
        public int Dequeue()
        {
            Swap(1, size);
            ReorderDownwards(1, size - 1);
            return heap[size--];
        }

        //if the value of one of the client key's changes then call this with 
        //the key's index to adjust the queue accordingly
        public void ChangePriority(int index)
        {
            ReorderUpwards(invHeap[index]);
        }
    }
}
