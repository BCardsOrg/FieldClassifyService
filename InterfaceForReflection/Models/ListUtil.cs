﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models
{
     public static class ListUtil
    {
        public static void AdjustSize<T>(this IList<T> list, int newSize, Func<T> addFun)
        {
            if (list.Count > newSize)
            {
                // Need to remove elements

                while (list.Count > newSize)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else
            {
                // Need to add elements

                while (list.Count < newSize)
                {
                    list.Add(addFun());
                }
            }

        }

        public static void AdjustSize<T>(this IList<T> list, int newSize, T defaultValue)
        {
            if (list.Count > newSize)
            {
                // Need to remove elements

                while (list.Count > newSize)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
            else
            {
                // Need to add elements

                while (list.Count < newSize)
                {
                    list.Add(defaultValue);
                }
            }

        }

        public static int IndexOf<T>(this IList<T> items, Func<T, bool> predicate) 
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items) {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        public static double STD(this IEnumerable<double> source)
        {
            var len = source.Count();

            if ( len == 0 )
            {
                return 0;
            }

            var avg = source.Average();

            return Math.Sqrt(source.Sum(x => Math.Pow(x - avg, 2)) / (double)len);
        }

        public static double STD<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return source.Select(x => selector(x)).STD();
        }
    }

     public class ConcurrentList<T> : IList<T>, IList
     {
         private readonly List<T> underlyingList = new List<T>();
         private readonly object syncRoot = new object();
         private readonly ConcurrentQueue<T> underlyingQueue;
         private bool requiresSync;
         private bool isDirty;

         public ConcurrentList()
         {
             underlyingQueue = new ConcurrentQueue<T>();
         }

         public ConcurrentList(IEnumerable<T> items)
         {
             underlyingQueue = new ConcurrentQueue<T>(items);
         }

         private void UpdateLists()
         {
             if (!isDirty)
                 return;
             lock (syncRoot)
             {
                 requiresSync = true;
                 T temp;
                 while (underlyingQueue.TryDequeue(out temp))
                     underlyingList.Add(temp);
                 requiresSync = false;
             }
         }

         public IEnumerator<T> GetEnumerator()
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.GetEnumerator();
             }
         }

         IEnumerator IEnumerable.GetEnumerator()
         {
             return GetEnumerator();
         }

         public void Add(T item)
         {
             if (requiresSync)
                 lock (syncRoot)
                     underlyingQueue.Enqueue(item);
             else
                 underlyingQueue.Enqueue(item);
             isDirty = true;
         }

         public int Add(object value)
         {
             if (requiresSync)
                 lock (syncRoot)
                     underlyingQueue.Enqueue((T)value);
             else
                 underlyingQueue.Enqueue((T)value);
             isDirty = true;
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.IndexOf((T)value);
             }
         }

         public bool Contains(object value)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.Contains((T)value);
             }
         }

         public int IndexOf(object value)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.IndexOf((T)value);
             }
         }

         public void Insert(int index, object value)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.Insert(index, (T)value);
             }
         }

         public void Remove(object value)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.Remove((T)value);
             }
         }

         public void RemoveAt(int index)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.RemoveAt(index);
             }
         }

         T IList<T>.this[int index]
         {
             get
             {
                 lock (syncRoot)
                 {
                     UpdateLists();
                     return underlyingList[index];
                 }
             }
             set
             {
                 lock (syncRoot)
                 {
                     UpdateLists();
                     underlyingList[index] = value;
                 }
             }
         }

         object IList.this[int index]
         {
             get { return ((IList<T>)this)[index]; }
             set { ((IList<T>)this)[index] = (T)value; }
         }

         public bool IsReadOnly
         {
             get { return false; }
         }

         public bool IsFixedSize
         {
             get { return false; }
         }

         public void Clear()
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.Clear();
             }
         }

         public bool Contains(T item)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.Contains(item);
             }
         }

         public void CopyTo(T[] array, int arrayIndex)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.CopyTo(array, arrayIndex);
             }
         }

         public bool Remove(T item)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.Remove(item);
             }
         }

         public void CopyTo(Array array, int index)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.CopyTo((T[])array, index);
             }
         }

         public int Count
         {
             get
             {
                 lock (syncRoot)
                 {
                     UpdateLists();
                     return underlyingList.Count;
                 }
             }
         }

         public object SyncRoot
         {
             get { return syncRoot; }
         }

         public bool IsSynchronized
         {
             get { return true; }
         }

         public int IndexOf(T item)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 return underlyingList.IndexOf(item);
             }
         }

         public void Insert(int index, T item)
         {
             lock (syncRoot)
             {
                 UpdateLists();
                 underlyingList.Insert(index, item);
             }
         }
     }
}
