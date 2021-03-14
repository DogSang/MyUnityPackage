using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sangs.Audio
{
    public class AudioMgr
    {
        #region 单例
        protected static AudioMgr _instance;
        public static AudioMgr Ins
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioMgr();
                    _instance.Init();
                }
                return _instance;
            }
        }
        #endregion

        #region 初始化
        private AudioMgr()
        {
            //加载场景后清空当前音效
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (a, b) =>
            {
                //清空Player
                AudioPlayer[] arrPlayer = listActivePlayer?.ToArray();
                if(arrPlayer!=null)
                    for (int i = 0; i < arrPlayer.Length; i++)
                    {
                        arrPlayer[i]?.Destroy();
                    }

                //清空音频播放器
                poolAudioSource?.RecycleAll();
            };

            poolAudioSource = new PoolComponent<AudioSource>();
            //切场景不删除，只清空clip
            poolAudioSource.EventOnCreateMember += (AudioSource player) => MonoBehaviour.DontDestroyOnLoad(player.gameObject);
            //获取时音量重置
            poolAudioSource.EventOnGetMember += (AudioSource player) => player.volume = 1;

            bgmPlayer = new BGMAudioPlayer(poolAudioSource.GetMember, poolAudioSource.RecycleMember);
        }

        private bool bHaveInit = false;
        public void Init()
        {
            if (bHaveInit) return;
            bHaveInit = true;

            //获取项目中所有继承了初始化接口的类型
            foreach (var typeInitializer in CSharpTools.GetType(typeof(IAudioMgrInitializer)))
            {
                try
                {
                    IAudioMgrInitializer initializer = Activator.CreateInstance(typeInitializer) as IAudioMgrInitializer;
                    initializer?.InitAudioMgr(this);
                }
#if !UNITY_EDITOR
                catch (System.Exception) { }
#else
                catch (System.Exception e)
                {
                    Debug.Log("AudioMgr Init Error : " + e);
                }
#endif
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 开始播放音频的事件
        /// </summary>
        public event System.Action<AudioPlayer> EventPlayAudioStart;
        /// <summary>
        /// 结束音频的事件
        /// </summary>
        public event System.Action<AudioClip> EventPlayAudioEnd;
        #endregion

        #region BGM
        private BGMAudioPlayer bgmPlayer;

        public void PlayBgm(AudioClip bgmClip) => bgmPlayer.PlayBgm(bgmClip, 0);

        public void SwitchBgm(AudioClip bgmClip, float switchTime) => bgmPlayer.PlayBgm(bgmClip, switchTime);

        #endregion

        #region 播放接口
        public AudioPlayer PlayAudio(AudioClip clip, System.Action<AudioClip> onAudioOver = null)
        {
            if (clip != null)
                return PlayAudio(GetAudioPlayer(), clip, onAudioOver);
            else
            {
                onAudioOver?.Invoke(clip);
                return null;
            }
        }
        public AudioPlayer PlayAudio(AudioClip clip, Vector3 pos, System.Action<AudioClip> onAudioOver = null) => PlayAudio(clip, onAudioOver)?.SetPos(pos);
        public AudioPlayer PlayAudio(AudioClip clip, Transform point, System.Action<AudioClip> onAudioOver = null) => PlayAudio(clip, onAudioOver)?.SetTarget(point);

        public AudioPlayer PlayAudio(AudioPlayer player, AudioClip clip, System.Action<AudioClip> onAudioOver = null)
        {
            //先执行开始播音频事件，因为播放音频有可能秒结束导致先结束事件再播放事件
            if (player != null)
            {
                player.PlayAudio(clip, onAudioOver);
                EventPlayAudioStart?.Invoke(player);
            }
            else
                onAudioOver?.Invoke(clip);
            return player;
        }
        #endregion

        #region Pool
        //TODO 如果性能压力大，短时间多次播放短音频的情况，需要把AudioPlayer改成内存池
        private List<AudioPlayer> listActivePlayer = new List<AudioPlayer>();
        private PoolComponent<AudioSource> poolAudioSource;

        /// <summary>
        /// player每次都是新建的对象，不会为空不需要重新初始化
        /// </summary>
        /// <returns></returns>
        public AudioPlayer GetAudioPlayer()
        {
            //创建player对象
            AudioPlayer player = new AudioPlayer(poolAudioSource.GetMember(), RemoveAudioPlayer);
            //注册player对象
            listActivePlayer.Add(player);
            return player;
        }

        /// <summary>
        /// 私有，创建audioPlayer时注册事件，需要注销在audioplayer调用destroy
        /// </summary>
        /// <param name="player"></param>
        private void RemoveAudioPlayer(AudioPlayer player)
        {
            if (player == null) return;

            //移除player脚本
            listActivePlayer.Remove(player);

            if (player.audioSource != null)
            {
                //清空clip
                player.audioSource.clip = null;
                //播放器添加进堆
                poolAudioSource.RecycleMember(player.audioSource);
            }

            //结束事件
            EventPlayAudioEnd?.Invoke(player?.audioSource?.clip);
        }
        #endregion
    }
}