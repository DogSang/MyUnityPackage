using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.Audio
{
    public class BGMAudioPlayer
    {
        private System.Func<AudioSource> funcGetAudioSource;
        public System.Action<AudioSource> actionRemoveAudioSource;

        /// <summary>
        /// bgm的播放器
        /// </summary>
        private AudioSource mainPlayer;
        /// <summary>
        /// 用于切换的播放器
        /// </summary>
        private List<AudioSource> listSwitchPlayer;

        /// <summary>
        /// 当前用于切换的闹钟的key
        /// </summary>
        private string strClockKey;

        public BGMAudioPlayer(System.Func<AudioSource> getAudioSource, System.Action<AudioSource> removeAudioSource)
        {
            funcGetAudioSource = getAudioSource;
            actionRemoveAudioSource = removeAudioSource;

            listSwitchPlayer = new List<AudioSource>();
        }

        public void PlayBgm(AudioClip bgmClip, float time)
        {
            //当前主播放器放入切换列表
            if (mainPlayer != null && mainPlayer.clip != null && mainPlayer.isPlaying)
                listSwitchPlayer.Add(mainPlayer);

            mainPlayer = GetBgmPlayer(bgmClip);

            //清除当前的渐变
            ClockMgr.Ins.RemoveClock(strClockKey);

            if (time <= 0)
            {
                //秒切
                RemoveSwitchList();
            }
            else
            {
                //过度
                //初始音量
                float[] arrFirstVolume = new float[listSwitchPlayer.Count];
                for (int i = 0; i < listSwitchPlayer.Count; i++)
                    arrFirstVolume[i] = listSwitchPlayer[i].volume;

                strClockKey = ClockMgr.Ins.AddClock(new ClockData(time, () =>
                {
                    //完成
                    RemoveSwitchList();
                }, (lerp) =>
                {
                    //主bgm渐变增大
                    mainPlayer.volume = Mathf.Lerp(0, 1, lerp);
                    //处理渐变音量
                    for (int i = 0; i < listSwitchPlayer.Count; i++)
                        listSwitchPlayer[i].volume = Mathf.Lerp(arrFirstVolume[i], 0, lerp);
                }));
            }
        }

        private void RemoveSwitchList()
        {
            foreach (var item in listSwitchPlayer)
            {
                item.Stop();
                item.clip = null;
                actionRemoveAudioSource?.Invoke(item);
            }
            listSwitchPlayer.Clear();
        }

        private AudioSource GetBgmPlayer(AudioClip clip)
        {
            AudioSource bgmPlayer = funcGetAudioSource?.Invoke();
            if (bgmPlayer == null) return null;

            bgmPlayer.clip = clip;
            bgmPlayer.volume = 1;//TODO 后期要做音量设置
            bgmPlayer.loop = true;
            bgmPlayer.spatialBlend = 0;
            bgmPlayer.Play();

            return bgmPlayer;
        }
    }
}