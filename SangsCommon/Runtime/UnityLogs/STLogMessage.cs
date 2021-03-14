using UnityEngine;
using System;

namespace Sangs.UnityLogs
{
    public struct STLogMessage
    {
        public string condition;
        public string stackTrace;
        public LogType type;
        public DateTime date;

        public string GetStackTraceRichText()
        {
            if (string.IsNullOrEmpty(stackTrace)) return null;
            string[] arrStr = stackTrace.Split('\n');
            string strRichText = "";
            for (int i = 0; i < arrStr.Length; i++)
            {
                if (string.IsNullOrEmpty(arrStr[i])) continue;

                int startIndex = arrStr[i].LastIndexOf("(at");
                if (startIndex < 0) continue;

                int endIndex = arrStr[i].LastIndexOf(')');

                string tmp = arrStr[i];
                tmp = tmp.Insert(endIndex, "</color>");
                tmp = tmp.Insert(startIndex + 3, "<color=#00FF00>");

                tmp += '\n';

                strRichText += tmp;
            }

            return strRichText;
        }
    }
}