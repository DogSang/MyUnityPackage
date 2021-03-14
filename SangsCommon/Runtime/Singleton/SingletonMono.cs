using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs
{
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance;
        public static T Ins
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    _instance.OnCreate();
                }
                return _instance;
            }
            protected set { _instance = value; }
        }

        protected virtual void OnCreate()
        {
            
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}