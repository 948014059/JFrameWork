using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    public class AudioManager : BaseSingleTon<AudioManager>
    {
        private AudioSource BGM;
        private int maxSoundNum = 5;
        private List<AudioSource> audioSourceList = new List<AudioSource>();
        private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();

        public void Init()
        {
            GameObject AudioManager = new GameObject("AudioManager");
            BGM = AudioManager.AddComponent<AudioSource>();
            for (int i = 0; i < maxSoundNum; i++)
            {
                GameObject audioSource = new GameObject("audioSource" + i);
                audioSourceList.Add(audioSource.AddComponent<AudioSource>());
                audioSource.transform.parent = BGM.transform;
            }
        }


        private AudioClip GetAudioClip(string path)
        {
            AudioClip audioClip = null;
            if (audioClipDict.ContainsKey(path))
            {
                audioClip = audioClipDict[path];
            }
            else
            {
                audioClip = ResourcesManager.GetAssets<AudioClip>(path);
                audioClipDict.Add(path, audioClip);
            }
            return audioClip;
        }

        public void PlayBGM(string bgmPath)
        {

            AudioClip audioClip = GetAudioClip(bgmPath);
            BGM.clip = audioClip;
            BGM.loop = true;
            BGM.Play();
        }

        public void PauseBGM()
        {
            BGM.Pause();
        }

        public void UnPauseBGM()
        {
            BGM.UnPause();
        }

        public void StopBGM()
        {
            BGM.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            BGM.volume = volume;
        }


        public void PlaySound(string soundPath, bool isLoop = false)
        {
            AudioClip audioClip = GetAudioClip(soundPath);
            foreach (AudioSource audioSource in audioSourceList)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = audioClip;
                    audioSource.clip.name = soundPath;
                    audioSource.loop = isLoop;
                    audioSource.Play();
                    return;
                }
            }
        }

        public void StopSound(string soundPath)
        {
            foreach (AudioSource audioSource in audioSourceList)
            {
                if (audioSource.isPlaying &&
                    audioSource.clip && audioSource.clip.name == soundPath)
                {
                    audioSource.Stop();
                    return;
                }
            }
        }

        public void SetSoundVolume(float volume)
        {
            foreach (AudioSource audioSource in audioSourceList)
            {
                audioSource.volume = volume;
            }
        }


    }

}
