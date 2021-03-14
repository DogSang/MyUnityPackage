using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangs.Audio
{
    public interface IAudioMgrInitializer
    {
        void InitAudioMgr(AudioMgr mgr);
    }
}