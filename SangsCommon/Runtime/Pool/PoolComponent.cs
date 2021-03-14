using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sangs
{
    public class PoolComponent<T> : Pool<T> where T : Component
    {
        public PoolComponent() : base(null, OnCreateMember, OnSetMemberActive, OnDestroyMember) { }
        public PoolComponent(GameObject prefab) : base(prefab != null ? prefab.GetComponent<T>() : null, OnCreateMember, OnSetMemberActive, OnDestroyMember) { }
        public PoolComponent(GameObject prefab, Func<T, T> delCreate, Action<T, bool> delSetActive, Action<T> delDestroy) :
            base(prefab != null ? prefab.GetComponent<T>() : null, OnCreateMember, OnSetMemberActive, OnDestroyMember)
        { }

        private static T OnCreateMember(T prefab)
        {
            if (prefab == null)
            {
                GameObject obj = new GameObject("PoolMember:" + typeof(T).ToString());
                T member = obj.GetComponent<T>();
                if (member == null)
                    member = obj.AddComponent<T>();
                return member;
            }
            else
            {
                return MonoBehaviour.Instantiate(prefab.gameObject).GetComponent<T>();
            }
        }
        private static void OnSetMemberActive(T member, bool active)
        {
            if (member != null)
            {
                member.gameObject.SetActive(active);
            }
        }
        private static void OnDestroyMember(T member)
        {
            if (member != null)
                MonoBehaviour.Destroy(member.gameObject);
        }
    }
}