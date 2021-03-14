using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs
{
    public class PoolEnumerator<T> : IEnumerator
    {
        public struct STMember
        {
            public T member;
            public bool bActive;
        }

        private List<STMember> list;
        private int nIndex = -1;

        public PoolEnumerator(List<T> activeMumbers, Stack<T> unActiveMembers)
        {
            list = new List<STMember>();

            activeMumbers?.ForEach((tmp) => list.Add(new STMember() { member = tmp, bActive = true }));
            foreach (var item in unActiveMembers)
            {
                list.Add(new STMember() { member = item, bActive = false });
            }
        }

        object IEnumerator.Current { get { return Current; } }
        public STMember Current
        {
            get
            {
                try
                {
                    return list[nIndex];
                }
                catch (System.IndexOutOfRangeException)
                {
                    throw new System.InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            nIndex++;
            return nIndex < list.Count;
        }
        public void Reset()
        {
            nIndex = -1;
        }
    }
}