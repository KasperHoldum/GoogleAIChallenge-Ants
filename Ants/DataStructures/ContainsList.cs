using System;
using System.Collections;
using System.Collections.Generic;

namespace Ants.DataStructures
{
    public class ContainsList<T> : ICollection<T>
    {
        private readonly Dictionary<T, bool> containsList = new Dictionary<T,bool>(); 

        public void Add(T t)
        {
            containsList[t] = true;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            return containsList.Remove(item);
        }

        public int Count
        {
            get { return containsList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(T t)
        {
            containsList.Remove(t);
        }

        public void Clear()
        {
            containsList.Clear();
        }

        public bool Contains(T t)
        {
            return containsList.ContainsKey(t);
        }

        public bool this[T t]
        {
            get { return containsList.ContainsKey(t); }
            set
            {
                if (value)
                {
                    containsList[t] = true;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return containsList.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
