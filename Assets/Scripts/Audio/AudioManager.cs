using System;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;

        private Sound _intro;
        private Sound _loop;

        /*
        #region SingleTon
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            // Needed if we want the audio manager to persist through scenes
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            // Add audio source components
            foreach (Sound s in sounds)
            {
                s.SetSource(gameObject.AddComponent<AudioSource>());
            }
        }
        

        #endregion
        */

        private void Awake()
        {
            foreach (Sound s in sounds)
            {
                s.SetSource(gameObject.AddComponent<AudioSource>());
            }        
        }

        public void Play(string soundName)
        {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound " + soundName + " not found!");
                return;
            }

            if (s.IsPlaying()) return;
            s.Play();
        }

        public void Stop(string soundName)
        {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound " + soundName + " not found!");
                return;
            }
            if (!s.IsPlaying()) return;
            s.Stop();
        }

        public void SetMusic(string introName, string loopName)
        {
            if (_intro != null && introName == _intro.name && _loop != null && loopName == _loop.name)
            {
                Debug.Log(introName + "/" + loopName + " already playing, ignoring.");
                return;
            }

            if (_intro != null)
                _intro.Stop();
            if (_loop != null)
                _loop.Stop();

            _intro = Array.Find(sounds, sound => sound.name == introName);
            if (_intro == null)
            {
                Debug.LogWarning("Sound " + introName + " not found!");
                return;
            }

            _loop = Array.Find(sounds, sound => sound.name == loopName);
            if (_intro == null)
            {
                Debug.LogWarning("Sound " + loopName + " not found!");
                return;
            }

            var introDuration = _intro.clip.length;
            var startTime = AudioSettings.dspTime + 0.2;
            _intro.PlayScheduled(startTime);
            _loop.PlayScheduled(startTime + introDuration);
        }

        public bool IsPlaying(String soundName)
        {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s != null) return s.IsPlaying();
            Debug.LogWarning("Sound " + soundName + " not found!");
            return false;
        }
    }
}