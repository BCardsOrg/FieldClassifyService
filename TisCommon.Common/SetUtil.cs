using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace TiS.Core.TisCommon
{
    #region SetUtil

    public class SetUtil<T> 
	{
        public static T[] GetDifference(ICollection<T> oSet1, ICollection<T> oSet2, IEqualityComparer<T> comparer = null)
		{
            Set<T> set1 = new Set<T>(oSet1, comparer);
            Set<T> set2 = new Set<T>(oSet2, comparer);

            Set<T> xor = set1.Xor(set2);

            return xor.GetAsArray();
		}

        public static T[] GetUnion(ICollection<T> oSet1, ICollection<T> oSet2, IEqualityComparer<T> comparer = null)
		{
            Set<T> set1 = new Set<T>(oSet1, comparer);
            Set<T> set2 = new Set<T>(oSet2, comparer);

            Set<T> union = set1.Union(set2);

            return union.GetAsArray();
		}

        public static T[] GetIntersection(ICollection<T> oSet1, ICollection<T> oSet2, IEqualityComparer<T> comparer = null)
		{
            Set<T> set1 = new Set<T>(oSet1, comparer);
            Set<T> set2 = new Set<T>(oSet2, comparer);

            Set<T> intersect = set1.Intersect(set2);

            return intersect.GetAsArray();
		}
    }

    #endregion


    #region Set

    public class Set<T> : HashSet<T>
    {
        public Set(IEqualityComparer<T> comparer = null)
            : base(comparer)
        {
        }

        public Set(IEnumerable<T> collection, IEqualityComparer<T> comparer = null)
            : base(collection, comparer)
        {
        }

        public void Add(IEnumerable<T> collection)
        {
            foreach (T member in collection)
            {
                base.Add(member);
            }
        }

        public bool ContainsAll(IEnumerable<T> collection)
        {
            foreach (T member in collection)
            {
                if (!base.Contains(member))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsEmpty
        {
            get 
            { 
                return base.Count == 0; 
            }
        }

        public void Remove(IEnumerable<T> collection)
        {
            foreach (T member in collection)
            {
                base.Remove(member);
            }
        }

        public Set<T> Union(Set<T> set)
        {
            Set<T> union = (Set<T>)this.Clone();

            if (set != null)
            {
                union.UnionWith(set);
            }

            return union;
        }

        public static Set<T> Union(Set<T> set1, Set<T> set2)
        {
            if (set1 == null && set2 == null)
            {
                return null;
            }

            if (set1 == null)
            {
                return (Set<T>)set2.Clone();
            }

            if (set2 == null)
            {
                return (Set<T>)set1.Clone();
            }

            return set1.Union(set2);
        }

        public Set<T> Intersect(Set<T> set)
        {
            Set<T> intersection = (Set<T>)this.Clone();

            if (set != null)
            {
                intersection.IntersectWith(set);
            }
            else
            {
                intersection.Clear();
            }

            return intersection;
        }

        public static Set<T> Intersect(Set<T> set1, Set<T> set2)
        {
            if (set1 == null && set2 == null)
            {
                return null;
            }

            if (set1 == null)
            {
                return set2.Intersect(set1);
            }

            return set1.Intersect(set2);
        }

        public virtual Set<T> Minus(Set<T> set1)
        {
            Set<T> minus = (Set<T>)this.Clone();

            if (set1 != null)
            {
                minus.ExceptWith(set1);
            }

            return minus;
        }

        public static Set<T> Minus(Set<T> set1, Set<T> set2)
        {
            if (set1 == null)
            {
                return null;
            }

            return set1.Minus(set2);
        }

        public Set<T> Xor(Set<T> set)
        {
            Set<T> xor = (Set<T>)this.Clone();

            if (set != null)
            {
                xor.SymmetricExceptWith(set);
            }

            return xor;
        }

        public static Set<T> Xor(Set<T> set1, Set<T> set2)
        {
            if (set1 == null && set2 == null)
            {
                return null;
            }

            if (set1 == null)
            {
                return (Set<T>)set2.Clone();
            }

            if (set2 == null)
            {
                return (Set<T>)set1.Clone();
            }

            return set1.Xor(set2);
        }

        public Set<T> Clone()
        {
            Set<T> newSet = (Set<T>)Activator.CreateInstance(this.GetType(), new object[] {this.Comparer});

            newSet.Add(this);

            return newSet;
        }

        public T[] GetAsArray()
        {
            T[] array = new T[Count];

            this.CopyTo(array, 0);

            return array;
        }
    }

    #endregion
}
