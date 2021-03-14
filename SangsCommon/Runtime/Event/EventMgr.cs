using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Sangs
{
    public class EventMgr
    {
        public enum EMEventType
        {
            Nomarl,
            Once,
        }
        public struct STEventData
        {
            public string strTag;
            public EMEventType emEventType;
            public System.Action<string> onEvent;
        }

        private Dictionary<string, STEventData> dicEventData;
        public EventMgr()
        {
            dicEventData = new Dictionary<string, STEventData>();
        }

        public string AddListener(STEventData eventData)
        {
            string guid = GetGUID();
            if (!dicEventData.ContainsKey(guid))
            {
                dicEventData.Add(guid, eventData);
            }
            return guid;
        }
        public void RemoveListener(string guid)
        {
            if (dicEventData.ContainsKey(guid))
                dicEventData.Remove(guid);
        }

        public void OnEvent(string tag, string value = null)
        {
            //C# LINQ 查询语句
            KeyValuePair<string, STEventData>[] arrTargetEvent = (from data in dicEventData
                                                                  where data.Value.strTag == tag
                                                                  select data).ToArray();
            //保存需要清除的eventData的index
            List<int> listIndex = new List<int>();
            for (int i = 0; i < arrTargetEvent.Length; i++)
            {
                arrTargetEvent[i].Value.onEvent?.Invoke(value);

                if (arrTargetEvent[i].Value.emEventType == EMEventType.Once)
                    listIndex.Add(i);
            }

            listIndex.ForEach((index) => RemoveListener(arrTargetEvent[index].Key));
        }

        private static string GetGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }

}
