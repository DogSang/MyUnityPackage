using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Sangs.Audio
{
    /// <summary>
    /// 临时对象，生命周期从播放音频开始，一直到音频结束
    /// </summary>
    public class AudioPlayer
    {
        /// <summary>
        /// 音频长度的加成系数，避免提前回收音频播放器
        /// </summary>
        private const float fAudipTimeCoefficient = 1.01f;

        /// <summary>
        /// 音频播放器
        /// </summary>
        public AudioSource audioSource { get; private set; }

        /// <summary>
        /// 结束销毁的委托，构造函数注册
        /// </summary>
        private System.Action<AudioPlayer> actionDestory;

        /// <summary>
        /// 跟随信息的key
        /// </summary>
        private string strFollowerKey;

        /// <summary>
        /// 是否已经被删除
        /// </summary>
        public bool bDestory { get; private set; }

        public AudioPlayer(AudioSource source, System.Action<AudioPlayer> onDestroy = null)
        {
            actionDestory = onDestroy;
            audioSource = source;

            bDestory = false;
        }

        /// <summary>
        /// 设置播放器坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public AudioPlayer SetPos(Vector3 pos)
        {
            if (audioSource != null)
                audioSource.transform.position = pos;

            return this;
        }
        /// <summary>
        /// 设置跟随目标
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public AudioPlayer SetTarget(Transform target)
        {
            if (audioSource != null && target != null)
                strFollowerKey = PosFollowerMgr.AddFollowerData(new PosFollowerMgr.STFollowerData()
                {
                    tfFollower = audioSource.transform,
                    tfTarget = target,
                });

            return this;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="onAudioOver"></param>
        public async void PlayAudio(AudioClip clip, System.Action<AudioClip> onAudioOver)
        {
            if (bDestory)
            {
                //提前删除没有回调
                //onAudioOver?.Invoke(clip);
                return;
            }

            //TODO 这里直接用异步等待回收，可能需要改成update检测
            await PlayerAudioAsync(clip);

            //TODO 需要停止异步，这里暂时不处理
            if (bDestory) return;

            //TODO 有可能需要添加一个public的event，这样可以自由的添加音频结束的事件
            onAudioOver?.Invoke((clip));

            Destroy();
        }

        /// <summary>
        /// 异步播放并等待音频结束
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        private async Task<AudioClip> PlayerAudioAsync(AudioClip clip)
        {
            if (audioSource != null && clip != null && !bDestory)
            {
                audioSource.clip = clip;
                audioSource.Play();
                await Task.Delay((int)(clip.length * 1000 * fAudipTimeCoefficient));
                audioSource?.Stop();
            }
            return clip;
        }

        public void Destroy()
        {
            if (bDestory) return;
            bDestory = true;

            actionDestory?.Invoke(this);
            audioSource = null;
        }
    }
}