using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace CustomFMODFunctions
{
    public class AudioInstanceHandler
    {
        public static EventInstance CreateAndPlaySFXInstance(EventInstance instance, EventReference reference)
        {
            if(instance.isValid())
                return instance;
            
            instance = RuntimeManager.CreateInstance(reference);
            instance.start();
            return instance;
        }
        
        public static void StopAndReleaseSFXInstance(EventInstance instance)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
             
        public static bool CheckIfPlayingSFX(EventInstance instance)
        {
            if (!instance.isValid())
                return false;
            
            PLAYBACK_STATE state;
            instance.getPlaybackState(out state);
            return state == PLAYBACK_STATE.PLAYING;
        }
    }
}

