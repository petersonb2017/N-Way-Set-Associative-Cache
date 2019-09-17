// Author: Bailey Peterson
// Created Date: 08/23/2017

namespace NSACache
{
    // This set associative cache is built from an array that contains the nodes  
    // of linked lists that make up each set. The elements in each set are 
    // arranged in descending order from most rescently used, to least recently
    // used. Object are placed in sets based on the output of a hash function
    // that is performed on their key.
    public class NSACache<K, T>
    {
        public readonly CacheNode<K, T>[] setHeads;
        public delegate KeyValuePair<K, T> evictionAlgorithm(CacheNode<K, T> head);
        public evictionAlgorithm alg;
        readonly int setNum;
        readonly int cacheCapacity;
        readonly int setSize;

        // Initializer using the algorithms that are provided in the class.
        public NSACache(int cacheCapacity, int setNum, string alg)
        {
            this.setNum = setNum;
            this.cacheCapacity = cacheCapacity;

            // Every set is the same size, so the setSize is the quotient of
            // cacheCapacity / setNum.
            setSize = cacheCapacity / setNum;
            //cachedObjectsArray = new KeyValuePair<K, T>[setNum, setSize];
            setHeads = new CacheNode<K, T>[setNum];

            // The default algorithm is LRU.
            if (alg.ToLower() == "mru")
            {
                this.alg = mruAlgorithm;
            }
            else
            {
                this.alg = lruAlgorithm;
            }

            // The following loop instantiates the head node in each set.
            for (int i = 0; i < setNum; i++)
            {
                setHeads[i] = new CacheNode<K, T>();
            }
        }

        // Initializer using custom algorithms that are provided by the user.
        public NSACache(int cacheCapacity, int setNum, evictionAlgorithm customAlg)
        {
            this.setNum = setNum;
            this.cacheCapacity = cacheCapacity;
            setHeads = new CacheNode<K, T>[setNum];
            alg = customAlg;
            for (int i = 0; i < setNum; i++)
            {
                setHeads[i] = new CacheNode<K, T>();
            }
            setSize = cacheCapacity / setNum;
        }

        public T get(K key)
        {
            int placementSet = hashKey(key);
            CacheNode<K, T> head = setHeads[placementSet];
            CacheNode<K, T> currentNode = head;

            // The following loop checks the nodes in the set that the key 
            // hashes to. On a hit, the node matching the key is set as the new
            // head and removed from its former placement.
            while (currentNode.key != null)
            {
                if (currentNode.key.Equals(key))
                {
                    head.prev = currentNode;
                    setHeads[placementSet] = currentNode;
                    currentNode.removeNode();
                    return currentNode.value;
                }
                currentNode = currentNode.next;
            }

            // On a miss, the cache returns null.
            return default(T);
        }

        public bool isInCache(K key, T value)
        {
            int setIndex = hashKey(key);
            CacheNode<K, T> head = setHeads[setIndex];
            CacheNode<K, T> currentNode = head;

            // The following loop checks the nodes in the set that the key 
            // hashes to.
            while (currentNode.key != null)
            {
                if (currentNode.value.Equals(value))
                {
                    return true;
                }
                currentNode = currentNode.next;
            }
            return false;
        }

        public void put(K key, T value)
        {
            CacheNode<K, T> newNode = new CacheNode<K, T>(key, value);
            int placementSet = hashKey(key);
            CacheNode<K, T> head = setHeads[placementSet];

            // Check to see if the cache already contains that value.
            if (isInCache(key, value)) return;

            // If the set is at capacity a node is evicted, otherwise the new
            // node is set as the head node.
            if (head.count() >= setSize)
            {
                KeyValuePair<K, T> pairToEvict = alg(head);
                evictNode(pairToEvict, newNode);
            }
            else
            {
                head.prev = newNode;
                newNode.next = head;
                setHeads[placementSet] = newNode;
            }
        }

        // This hash function utilizes the standard C# GetHashCode method
        // modulo the number of sets. 
        public int hashKey(K key)
        {
            int hashInt = key.GetHashCode() % setNum;
            if (hashInt < 0) hashInt = setNum + hashInt;
            return hashInt;
        }

        public void evictNode(KeyValuePair<K, T> pairToEvict, CacheNode<K, T> newNode)
        {
            int evictionSetIndex = hashKey(pairToEvict.key);
            T evictedValue = pairToEvict.value;
            CacheNode<K, T> head = setHeads[evictionSetIndex];
            CacheNode<K, T> currentNode = head;
            while (currentNode.value != null)
            {
                if (currentNode.value.Equals(evictedValue))
                {
                    head.prev = newNode;
                    newNode.next = head;
                    currentNode.removeNode();
                    setHeads[evictionSetIndex] = newNode;
                }
                currentNode = currentNode.next;
            }

        }

        KeyValuePair<K, T> lruAlgorithm(CacheNode<K, T> head)
        {
            CacheNode<K, T> currentNode = head;
            while (currentNode.next.key != null)
            {
                currentNode = currentNode.next;
            }
            var pair = new KeyValuePair<K, T>(currentNode.key, currentNode.value);
            return pair;
        }

        KeyValuePair<K, T> mruAlgorithm(CacheNode<K, T> head)
        {
            var pair = new KeyValuePair<K, T>(head.key, head.value);
            return pair;
        }
    }

    public class KeyValuePair<K, T>
    {
        public K key;
        public T value;

        public KeyValuePair(K key, T value)
        {
            this.key = key;
            this.value = value;
        }

    }
}
