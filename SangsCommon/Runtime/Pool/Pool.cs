using System.Collections.Generic;
using System.Collections;
using System;

namespace Sangs
{
    public class Pool<T> : IEnumerable
    {
        public event Action<T> EventOnCreateMember;
        public event Action<T> EventOnGetMember;
        public event Action<T> EventOnRecycleMember;

        protected T prefab;
        protected Func<T, T> delCreateMember;
        protected Action<T, bool> delSetMemberAction;
        protected Action<T> delDestroyMember;

        protected List<T> activeMembers = new List<T>();
        protected Stack<T> unActiveMembers = new Stack<T>();

        public Pool(T prefab, Func<T, T> delCreate, Action<T, bool> delSetActive, Action<T> delDestroy = null)
        {
            this.prefab = prefab;
            delCreateMember = delCreate;
            delSetMemberAction = delSetActive;
            delDestroyMember = delDestroy;
        }

        public T GetMember()
        {
            T member;
            if (unActiveMembers.Count > 0)
            {
                member = unActiveMembers.Pop();
            }
            else
            {
                member = CreateMember();
            }

            activeMembers.Add(member);

            delSetMemberAction?.Invoke(member, true);
            EventOnGetMember?.Invoke(member);
            return member;
        }

        protected T CreateMember()
        {
            T member;
            if (delCreateMember != null)
                member = delCreateMember(prefab);
            else
                member = default(T);

            EventOnCreateMember?.Invoke(member);
            return member;
        }

        public void RecycleMember(T member)
        {
            if (activeMembers.Contains(member))
                activeMembers.Remove(member);
            if (unActiveMembers.Contains(member))
                return;
            else
                unActiveMembers.Push(member);

            delSetMemberAction?.Invoke(member, false);
            EventOnRecycleMember?.Invoke(member);
        }

        public void RecycleAll()
        {
            T[] arrTmp = activeMembers.ToArray();
            for (var i = 0; i < arrTmp.Length; i++)
            {
                RecycleMember(arrTmp[i]);
            }
        }

        public void Clean()
        {
            activeMembers.ForEach((member) => delDestroyMember?.Invoke(member));
            foreach (var member in unActiveMembers)
            {
                delDestroyMember?.Invoke(member);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
        public PoolEnumerator<T> GetEnumerator()
        {
            return new PoolEnumerator<T>(activeMembers, unActiveMembers);
        }
    }
}
