using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.UnityLogs
{
    public enum EMLogLevel
    {
        Normal,
        Warnig,
        Error,
        All,
    }
    public interface IUnityLogView
    {
        void OnAddLog(EMLogLevel logLevel, STLogMessage msg);
        void ClearLog(EMLogLevel logLevel);
    }

    public class UnityLogListener
    {
        protected UnityLogEvents events;
        private Dictionary<EMLogLevel, List<STLogMessage>> dicLogMsg;

        public IUnityLogView logView { get; private set; }

        public UnityLogListener(IUnityLogView view = null)
        {
            Init();
            logView=view;
        }
        public UnityLogListener(UnityLogEvents events, IUnityLogView view = null)
        {
            this.events = events;
            logView=view;
            Init();
        }

        protected virtual void Init()
        {
            dicLogMsg = new Dictionary<EMLogLevel, List<STLogMessage>>()
            {
                {EMLogLevel.Normal,new List<STLogMessage>()},
                {EMLogLevel.Warnig,new List<STLogMessage>()},
                {EMLogLevel.Error,new List<STLogMessage>()},
            };

            if (events == null)
                events = new UnityLogEvents();

            events.EventLogNormal += OnNormalLog;
            events.EventLogWarning += OnNormalWarning;
            events.EventLogError += OnNormalError;
        }

        public void Destroy()
        {
            if (events != null)
            {
                events.EventLogNormal -= OnNormalLog;
                events.EventLogWarning -= OnNormalWarning;
                events.EventLogError -= OnNormalError;
            }
            ClearMsg();
        }

        public void ForeachMsgList(EMLogLevel logLevel, System.Action<STLogMessage> action)
        {
            if (action != null)
            {
                if (logLevel == EMLogLevel.All)
                {
                    List<STLogMessage> listTmp = new List<STLogMessage>();
                    foreach (var list in dicLogMsg.Values)
                    {
                        listTmp.AddRange(list);
                    }
                    listTmp.Sort((msg1, msg2) => msg1.date > msg2.date ? 1 : (msg1.date < msg2.date ? -1 : 0));
                    listTmp.ForEach(action);
                }
                else if (dicLogMsg != null && dicLogMsg.ContainsKey(logLevel))
                    dicLogMsg[logLevel]?.ForEach(action);
            }
        }

        protected virtual void OnNormalLog(STLogMessage msg)
        {
            AddMsg(EMLogLevel.Normal, msg);
        }
        protected virtual void OnNormalWarning(STLogMessage msg)
        {
            AddMsg(EMLogLevel.Warnig, msg);
        }
        protected virtual void OnNormalError(STLogMessage msg)
        {
            AddMsg(EMLogLevel.Error, msg);
        }

        protected virtual void AddMsg(EMLogLevel logLevel, STLogMessage msg)
        {
            if (dicLogMsg != null && dicLogMsg.ContainsKey(logLevel))
                dicLogMsg[logLevel]?.Add(msg);

            logView?.OnAddLog(logLevel, msg);
        }

        public virtual void ClearMsg()
        {
            int num = (int)EMLogLevel.All;
            for (int i = 0; i < num; i++)
            {
                ClearMsg((EMLogLevel)i);
            }
        }
        public virtual void ClearMsg(EMLogLevel logLevel)
        {
            if (dicLogMsg != null && dicLogMsg.ContainsKey(logLevel))
                dicLogMsg[logLevel]?.Clear();

            logView?.ClearLog(logLevel);
        }
    }
}
