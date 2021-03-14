using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.UGUITools
{
    public class CurvedTextData : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        private string _ReadMe = "Overflow设置必须全部为Overflow\n" +
            "因为功能没做完没处理自动换行的情况\n" + "修改完参数需要刷新一下";

        [Header("是否启用曲面")]
        public bool bCurved = true;
        [Header("曲面中心点到文本的距离，控制曲面弧度")]
        public float fCurvedCenterDis = 100;
        [Header("字的距离的系数，越小越文本越紧密")]
        public float fDisCoefficient = 1;
    }
}