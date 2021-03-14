using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.Audio.AudioResourcesLoader
{
    public static class AudioResourcesLoader
    {
        #region 音频路径信息
        //音频文件的初始信息
        private static Dictionary<string, string> dicAudioResPath;
        public static void InitAudioResPath(this AudioMgr mgr, Dictionary<string, string> dic)
        {
            dicAudioResPath = dic;
        }
        #endregion

        #region load接口
        public static AudioClip LoadAudio(string audioName)
        {
            if (dicAudioResPath == null || !dicAudioResPath.ContainsKey(audioName)) return null;
            return Resources.Load<AudioClip>(dicAudioResPath[audioName]);
        }
        #endregion

        #region 扩展播放接口
        public static AudioPlayer PlayAudio(this AudioMgr mgr, string audioName, System.Action<AudioClip> onAudioOver = null)
        {
            if (dicAudioResPath == null || !dicAudioResPath.ContainsKey(audioName))
            {
                onAudioOver?.Invoke(null);
                return null;
            }
            else
                return mgr.PlayAudio(LoadAudio(audioName), onAudioOver);
        }
        public static AudioPlayer PlayAudio(this AudioMgr mgr, string audioName, Vector3 pos, System.Action<AudioClip> onAudioOver = null)
        {
            return AudioMgr.Ins.PlayAudio(audioName, onAudioOver)?.SetPos(pos);
        }
        public static AudioPlayer PlayAudio(this AudioMgr mgr, string audioName, Transform target, System.Action<AudioClip> onAudioOver = null)
        {
            return AudioMgr.Ins.PlayAudio(audioName, onAudioOver)?.SetTarget(target);
        }

        #endregion
    }
}
