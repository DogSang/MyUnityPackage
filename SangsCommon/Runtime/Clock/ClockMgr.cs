using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sangs
{
    public class ClockMgr : SingletonMono<ClockMgr>
    {
        private const float RefreshTime = 0.1f;

        private Dictionary<string, ClockData> dicClockData;

        protected override void OnCreate()
        {
            dicClockData = new Dictionary<string, ClockData>();

            StartCoroutine(IEUpdate());
        }

        public string AddClock(ClockData clockData)
        {
            string strGUID = Guid.NewGuid().ToString();
            //理论上GUID不会重复
            dicClockData.Add(strGUID, clockData);
            return strGUID;
        }
        public void RemoveClock(string key)
        {
            if (dicClockData.ContainsKey(key)) dicClockData.Remove(key);
        }

        private IEnumerator IEUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(RefreshTime);
                List<string> listWillDestroyKey = new List<string>();

                foreach (var item in dicClockData)
                {
                    if (item.Value == null || item.Value.CheckClock(RefreshTime))
                    {
                        //没有闹钟信息或者刚刚结束
                        listWillDestroyKey.Add(item.Key);
                    }
                }

                //去空跟已经完成的
                foreach (var item in listWillDestroyKey)
                {
                    dicClockData.Remove(item);
                }
            }
        }

    }

    public class ClockData
    {
        private Action acitonOnClock;
        private Action<float> actionOnUpdateLerp;
        private float fTime;

        private float fCurTime;

        public ClockData(float time, Action onClock, Action<float> onUpdateLerp = null)
        {
            fTime = time;
            acitonOnClock = onClock;
            actionOnUpdateLerp = onUpdateLerp;

            fCurTime = 0;
        }

        public bool CheckClock(float deltaTime)
        {
            fCurTime += deltaTime;

            //处理时间异常的情况
            float fLerp = fTime <= 0 ? 1 : Mathf.Min(fCurTime / fTime, 1);

            //达标后会调用一次 lerp 为 1 的 update
            actionOnUpdateLerp?.Invoke(fLerp);

            if (fLerp >= 1)
            {
                acitonOnClock?.Invoke();
                return true;
            }
            else
                return false;
        }
    }
}