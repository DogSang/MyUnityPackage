using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs
{
    public class SingletonScript<T> where T : SingletonScript<T>, new()
    {
        private static T _instance;
        public static T Ins
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.OnAfterCreate();
                }
                return _instance;
            }
            protected set => _instance = value;
        }

        protected virtual void OnAfterCreate() { }
    }
}