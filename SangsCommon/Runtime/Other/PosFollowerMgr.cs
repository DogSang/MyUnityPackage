using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs
{
    public class PosFollowerMgr : SingletonMono<PosFollowerMgr>
    {
        public struct STFollowerData
        {
            /// <summary>
            /// 跟随者
            /// </summary>
            public Transform tfFollower;
            /// <summary>
            /// 跟随的目标
            /// </summary>
            public Transform tfTarget;

            //TODO 后续加跟随类型，指向目标，或者跟随，或者其他的
        }

        public Dictionary<string, STFollowerData> dicFollowerData;

        protected override void OnCreate()
        {
            base.OnCreate();
            dicFollowerData = new Dictionary<string, STFollowerData>();

            //没10秒调用一次自动去空
            InvokeRepeating("RemoveEmptyData", 10, 10);
        }

        /// <summary>
        /// 注册跟随信息
        /// </summary>
        /// <param name="followerData"></param>
        /// <returns></returns>
        public static string AddFollowerData(STFollowerData followerData)
        {
            if (CheckFollowerDataIsEmpty(followerData)) return null;

            string key = System.Guid.NewGuid().ToString();
            Ins.dicFollowerData.Add(key, followerData);
            return key;
        }
        /// <summary>
        /// 注销跟随信息
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveFollowerData(string key)
        {
            if (Ins.dicFollowerData.ContainsKey(key))
                Ins.dicFollowerData.Remove(key);
        }

        private void LateUpdate()
        {
            foreach (var item in dicFollowerData.Values)
            {
                Execute(item);
            }
        }

        /// <summary>
        /// 执行跟随信息
        /// </summary>
        /// <param name="followerData"></param>
        protected void Execute(STFollowerData followerData)
        {
            if (followerData.tfFollower == null) return;

            if (followerData.tfTarget != null)
                followerData.tfFollower.position = followerData.tfTarget.position;
        }

        /// <summary>
        /// 去空方法，create后定时触发
        /// </summary>
        private void RemoveEmptyData()
        {
            List<string> listTmp = new List<string>();
            listTmp.AddRange(dicFollowerData.Keys);
            for (int i = 0; i < listTmp.Count; i++)
            {
                if (dicFollowerData.ContainsKey(listTmp[i]) && CheckFollowerDataIsEmpty(dicFollowerData[listTmp[i]]))
                    dicFollowerData.Remove(listTmp[i]);
            }
        }

        /// <summary>
        /// 检查是否为空
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckFollowerDataIsEmpty(STFollowerData data)
        {
            if (data.tfFollower == null || data.tfTarget == null) return true;

            return false;
        }
    }
}