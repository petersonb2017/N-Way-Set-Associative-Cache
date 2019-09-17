// Author: Bailey Peterson
// Created Date: 08/23/2017

namespace NSACache
{
    // This class is a standard doubly-linked list that stores the cached object
    // data and key.
    public class CacheNode<K, T>
    {
        public CacheNode<K, T> prev;
        public CacheNode<K, T> next;
        public K key;
        public T value;

        public CacheNode()
        {
            key = default(K);
            value = default(T);
        }

        public CacheNode(K key, T data)
		{
            this.key = key;
            this.value = data;
		}

        public int count()
        {
            int counter = 0;
            CacheNode<K, T> currentNode = this;
            while (currentNode != null){
                counter++;
                currentNode = currentNode.next;
            }

            // This -1 comes from the fact that there is a non-null but
            // unoccupied node being pointed to by the last node in the list
            // that should be left out.
            return counter - 1 ;
        }

        public void removeNode(){
            if (prev != null){
                prev.next = next;
            }
            if(next != null){
                next.prev = prev;
            }
        }

    }
}
