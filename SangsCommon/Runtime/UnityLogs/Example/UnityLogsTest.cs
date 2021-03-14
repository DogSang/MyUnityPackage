using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sangs.UnityLogs;

namespace Sangs.UnityLogs.Test
{
    public class UnityLogsTest : MonoBehaviour
    {
        // public string strText
        // {
        //     get => text.text;
        //     set => text.text = value;
        // }
        // public Text text;
        // private void Awake()
        // {
        //     new UnityLogEvents().EventLogMessage += (msg) =>
        //     {
        //         strText += $"\n{msg.type}\n{msg.condition}\n{msg.GetStackTraceRichText()}\n";
        //     };
        // }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Log"))
            {
                Debug.Log("Log");
            }
            if (GUI.Button(new Rect(0, 75, 100, 50), "LogWarning"))
            {
                Debug.LogWarning("LogWarning");
            }
            if (GUI.Button(new Rect(0, 150, 100, 50), "LogError"))
            {
                Debug.LogError("LogError");
            }
            if (GUI.Button(new Rect(0, 225, 100, 50), "LogAssertion"))
            {
                Debug.LogAssertion("LogAssertion");
            }
            if (GUI.Button(new Rect(0, 300, 100, 50), "LogException"))
            {
                Debug.LogException(new System.Exception("TestException"));
            }
            if (GUI.Button(new Rect(0, 375, 100, 50), "index out of range"))
            {
                int[] arrTmp = new int[1];
                arrTmp[1] = 0;
            }
        }
    }

}