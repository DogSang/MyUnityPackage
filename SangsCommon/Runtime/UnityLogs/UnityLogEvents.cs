using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sangs.UnityLogs
{
    public class UnityLogEvents
    {
        public event Action<STLogMessage> EventLogMessage;

        public event Action<STLogMessage> EventLogNormal;
        public event Action<STLogMessage> EventLogWarning;
        public event Action<STLogMessage> EventLogError;

        public UnityLogEvents()
        {
            Application.logMessageReceived += OnLogMessage;
        }
        public void Destory()
        {
            Application.logMessageReceived -= OnLogMessage;
        }
        public void ClearEvents()
        {
            EventLogMessage = null;

            EventLogNormal = null;
            EventLogWarning = null;
            EventLogError = null;
        }

        protected virtual void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            STLogMessage msg = GetLogMessage(condition, stackTrace, type);

            EventLogMessage?.Invoke(msg);

            switch (type)
            {
                case LogType.Log:
                    EventLogNormal?.Invoke(msg);
                    break;
                case LogType.Warning:
                    EventLogWarning?.Invoke(msg);
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    EventLogError?.Invoke(msg);
                    break;
                default:
                    break;
            }
        }

        protected virtual STLogMessage GetLogMessage(string condition, string stackTrace, LogType type)
        {
            STLogMessage msg = new STLogMessage();
            msg.type = type;
            msg.stackTrace = stackTrace;
            msg.condition = condition;
            msg.date = System.DateTime.Now;

            return msg;
        }
    }
}