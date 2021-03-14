using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sangs.UGUITools
{
    public class UIGif : MonoBehaviour
    {
        public Sprite[] arrSprite;
        public Image imgTarget;

        [Header("每次切换图片间隔的时间")]
        public float fSwicthSpriteTime = 0.1f;
        [Header("每次切换图片间隔的时间")]
        public float fWaitTime_EveryLoop = 0.3f;

        private int _curIndex;
        public int CurSpriteIndex
        {
            get { return _curIndex; }
            set
            {
                if (value < 0)
                {
                    _curIndex = 0;
                }
                else
                {
                    _curIndex = value % arrSprite.Length;
                }
                imgTarget.sprite = arrSprite[_curIndex];
            }
        }

        private void Awake()
        {
            if (imgTarget == null) imgTarget = GetComponentInChildren<Image>();
            if (imgTarget == null)
            {
                Destroy(this);
                return;
            }

            if (arrSprite == null || arrSprite.Length == 0)
            {
                Destroy(this);
                return;
            }
            else if (arrSprite.Length == 1)
            {
                CurSpriteIndex = 0;
                Destroy(this);
                return;
            }

            CurSpriteIndex = 0;
        }

        private void OnEnable()
        {
            StopCoroutine("IE_Gif");
            StartCoroutine("IE_Gif");
        }
        private void OnDisable()
        {
            StopCoroutine("IE_Gif");
        }

        private IEnumerator IE_Gif()
        {
            while (true)
            {
                yield return new WaitForSeconds(fSwicthSpriteTime);
                CurSpriteIndex++;

                //一次循环完毕
                if (CurSpriteIndex == 0)
                    yield return new WaitForSeconds(fWaitTime_EveryLoop);
            }
        }
    }
}