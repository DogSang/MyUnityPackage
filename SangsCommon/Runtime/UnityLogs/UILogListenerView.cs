using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sangs.UnityLogs
{
    public class UILogListenerView : MonoBehaviour, IUnityLogView
    {
        protected PoolComponent<UILogListenerItem> poolLogItem;
        [SerializeField]
        private UILogListenerItem prefabItem = null;

        private UnityLogListener listener;

        [SerializeField]
        private bool bItemNormalOpen = false;

        private void Awake()
        {
            if (prefabItem != null)
            {
                poolLogItem = new PoolComponent<UILogListenerItem>(prefabItem.gameObject);
                prefabItem.gameObject.SetActive(false);

                poolLogItem.EventOnCreateMember += (item) =>
                {
                    item.transform.SetParent(prefabItem.transform.parent);
                    item.transform.localScale = Vector3.one;
                    item.transform.localPosition = Vector3.zero;
                };
            }

            listener = new UnityLogListener(this);
        }

        public void OnAddLog(EMLogLevel logLevel, STLogMessage msg)
        {
            var item = poolLogItem?.GetMember();
            if (item != null)
            {
                item?.SetMsg(logLevel, msg, bItemNormalOpen);
                item.transform.SetSiblingIndex(item.transform.parent.childCount - 1);
            }
        }
        public void ClearLog(EMLogLevel logLevel)
        {
            foreach (var member in poolLogItem)
            {
                if (member.bActive && member.member.logLevel == logLevel)
                    poolLogItem.RecycleMember(member.member);
            }
        }

        private void OnDestroy()
        {
            listener?.Destroy();
        }
    }
}