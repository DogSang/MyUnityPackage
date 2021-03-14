using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Sangs.UGUITools
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformSizeChangeEvent : MonoBehaviour
    {
        public UnityEvent UnityEvent_OnSizeChange;
        public event System.Action<Vector2> Event_OnSizeChange;

        public RectTransform tf => transform as RectTransform;
        private Vector2 vLastSize;

        public Vector2 CurSize
        {
            get { return tf.sizeDelta; }
        }

        private void Awake()
        {
            vLastSize = tf.sizeDelta;
        }

        private void Update()
        {
            Vector2 curSize = tf.sizeDelta;

            if (vLastSize != curSize)
            {
                vLastSize = curSize;

                OnSizeChange(curSize);
            }
        }

        protected virtual void OnSizeChange(Vector2 curSize)
        {
            Event_OnSizeChange?.Invoke(curSize);
            UnityEvent_OnSizeChange?.Invoke();
        }
    }
}