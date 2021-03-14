using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sangs.UGUITools;

namespace Sangs.UnityLogs
{
    public class UILogListenerItem : MonoBehaviour, IPointerClickHandler
    {
        public EMLogLevel logLevel { get; private set; }
        public STLogMessage curMsg { get; private set; }

        [SerializeField]
        private Text text = null;

        private Image imgBg;
        private bool bIsOpen = false;

        protected string ConditionText
        {
            get
            {
                string strText = curMsg.date.ToString() + "  ";
                switch (logLevel)
                {
                    case EMLogLevel.Normal:
                        strText += logLevel;
                        break;
                    case EMLogLevel.Warnig:
                        strText += "<color=#FFFF00>" + logLevel + "</color>";
                        break;
                    case EMLogLevel.Error:
                        strText += "<color=#FF0000>" + logLevel + "</color>";
                        break;
                }
                strText += "  " + curMsg.condition;
                return strText;
            }
        }

        private void Awake()
        {
            imgBg = GetComponent<Image>();

            RectTransformSizeChangeEvent sizeChangeEvent = GetComponentInChildren<RectTransformSizeChangeEvent>();
            sizeChangeEvent.Event_OnSizeChange += (size) => (transform as RectTransform).sizeDelta = size + Vector2.one * 20;
        }

        public virtual UILogListenerItem SetMsg(EMLogLevel logLevel, STLogMessage msg, bool bOpen = false)
        {
            this.logLevel = logLevel;
            curMsg = msg;

            //onclick里面会再自动翻一次面
            SetOpenStage(bOpen);

            return this;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        protected virtual void OnClick()
        {
            SetOpenStage(!bIsOpen);
        }

        public void SetOpenStage(bool bOpen)
        {
            bIsOpen = bOpen;

            if (text != null)
                text.text = bOpen ? ConditionText + "\n" + curMsg.GetStackTraceRichText() : ConditionText;

            if (imgBg != null)
                imgBg.color = bOpen ? new Color(0, 0, 1, 0.1f) : new Color(1, 1, 1, 0.1f);
        }
    }
}