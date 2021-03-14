using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.Json
{
    public interface IJsonData { }

    public static class JsonDataTools
    {
        /// <summary>
        /// 转换成json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this IJsonData data)
        {
            return data == null ? null : JsonUtility.ToJson(data);
        }
        /// <summary>
        /// 尝试给当前对象用json数据赋值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="json"></param>
        /// <returns>如果是struct需要用返回值覆盖，class不需要</returns>
        public static IJsonData Pause(this IJsonData data, string json)
        {
            if (!string.IsNullOrEmpty(json))
                JsonUtility.FromJsonOverwrite(json, data);
            return data;
        }
        /// <summary>
        /// 创建并返回一个copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Copy<T>(this IJsonData data) where T : IJsonData
        {
            return JsonUtility.FromJson<T>(data.ToJson());
        }
    }
}