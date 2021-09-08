using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using Orcas.Core.Tools;

namespace Orcas.Resources
{
    
    public class SoundLoader
    {
        /// <summary>
        /// 音效的依托
        /// </summary>
        private static GameObject SoundObj;
        //上一次播放的音效
        private static string lastLoopSound;
        
        //上一次播放的背景音乐
        private static string lastBGMSound;
        /// <summary>
        /// 播放
        /// </summary>
        private static AudioSource MusicAudio;
        /// <summary>
        /// 循环播放的音效
        /// </summary>
        private static Dictionary<string, AudioSource> LoopSounds;

        /// <summary>
        /// 背景音乐
        /// </summary>
        private static Dictionary<string, AudioSource> BGMSounds;
        
        //音频文件目录
        public static string AudioFilePath = "External/Audio/";

        private static Dictionary<string, AudioClip> loadedAudioClips = new Dictionary<string, AudioClip>();

        private static HashSet<string> preloadClipNames = new HashSet<string>();

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            SoundObj = new GameObject("SoundLoader");
            GameObject.DontDestroyOnLoad(SoundObj);
            if (MusicAudio == null) MusicAudio = SoundObj.AddComponent<AudioSource>();
            if (LoopSounds == null) LoopSounds = new Dictionary<string, AudioSource>();
            if (BGMSounds == null) BGMSounds = new Dictionary<string, AudioSource>();
            lastLoopSound = "";
        }

        public static void PlaySound(string name, Vector3 worldPos, float volume)
        {
            if (PlayerPrefsManager.GetInt(PlayerPrefHelper.SoundSwitch, 1) == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(name)) return;

            if (SoundObj == null) Init();
            
            AudioClip clip = LoadAudioClip(name);
            if (clip == null)
            {
                return;
            }
            
            GameObject clipObj = new GameObject("clipObj_" + name);
            clipObj.transform.SetParent(SoundObj.transform, false);
            clipObj.transform.position = worldPos;
            AudioSource audioSource = clipObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            GameObject.Destroy(clipObj, clip.length + 0.1f);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public static void PlaySound(string name)
        {
            PlaySound(name, Vector3.zero, 1f);
        }

        public static void PlayLoopSound(string name)
        {
            PlayLoopSound(name, Vector3.zero, 1f);
        }

        public static void PlayLoopSound(string name, Vector3 pos, float volume)
        {
            if (PlayerPrefsManager.GetInt(PlayerPrefHelper.SoundSwitch, 1) == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(name)) return;
            if (SoundObj == null) Init();
    
            if (LoopSounds.ContainsKey(name) == true)
            {
                if (LoopSounds[name].isPlaying == false)
                {
                    LoopSounds[name].Play();
                    lastLoopSound = name;
                    return;
                }
                else
                {
                    return;
                }
            }
            
            AudioClip clip = LoadAudioClip(name);
            if (clip == null)
            {
                return;
            }
            lastLoopSound = name;
            GameObject clipObj = new GameObject("clipObj_" + name);
            clipObj.transform.SetParent(SoundObj.transform, false);
            clipObj.transform.position = pos;
            AudioSource audioSource = clipObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
            LoopSounds[name] = audioSource;
        }

        public static void PlayBGM(string name)
        {
            PlayBGM(name, Vector3.one, 1f);
        }
        
        public static void PlayBGM(string name, Vector3 pos, float volume)
        {
            if (PlayerPrefsManager.GetInt(PlayerPrefHelper.BGMSwitch, 1) == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(name)) return;
            if (SoundObj == null) Init();
    
            if (BGMSounds.ContainsKey(name) == true)
            {
                if (BGMSounds[name].isPlaying == false)
                {
                    BGMSounds[name].Play();
                    lastBGMSound = name;
                    return;
                }
                else
                {
                    return;
                }
            }
            
            AudioClip clip = LoadAudioClip(name);
            if (clip == null)
            {
                return;
            }
            lastBGMSound = name;
            GameObject clipObj = new GameObject("clipObj_" + name);
            clipObj.transform.SetParent(SoundObj.transform, false);
            clipObj.transform.position = Vector3.zero;
            AudioSource audioSource = clipObj.AddComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
            BGMSounds[name] = audioSource;
        }

        public static void DestroyLoopSound(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (SoundObj == null) Init();

            if (LoopSounds.ContainsKey(name) == true)
            {
                AudioSource audioSource = LoopSounds[name];
                LoopSounds.Remove(name);
                if (audioSource != null)
                {
                    audioSource.Stop();
                    GameObject.Destroy(audioSource.gameObject);
                }
            }
        }
        
        public static void DestroyBGM(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (SoundObj == null) Init();

            if (BGMSounds.ContainsKey(name) == true)
            {
                AudioSource audioSource = BGMSounds[name];
                BGMSounds.Remove(name);
                if (audioSource != null)
                {
                    audioSource.Stop();
                    GameObject.Destroy(audioSource.gameObject);
                }
            }
        }

        public static void DestroyAllLoopSound()
        {
            if (LoopSounds != null)
            {
                foreach (var sound in LoopSounds)
                {
                    DestroyLoopSound(sound.Key);
                }
            }
        }
        
        public static void DestroyAllBGM()
        {
            if (BGMSounds != null)
            {
                foreach (var sound in BGMSounds)
                {
                    DestroyLoopSound(sound.Key);
                }
            }
        }

        public static void DestroyAllLoopSoundAndBGM()
        {
            DestroyAllLoopSound();
            DestroyAllBGM();
        }

        public static void SwitchBGM(bool on)
        {
            if (SoundObj == null) Init();
            if (on == true && PlayerPrefsManager.GetInt(PlayerPrefHelper.BGMSwitch, 1) == 0)
            {
                PlayerPrefsManager.SetInt(PlayerPrefHelper.BGMSwitch, 1);
                foreach (var audioSrouce in BGMSounds)
                {
                    if (audioSrouce.Key == lastBGMSound)
                    {
                        audioSrouce.Value.Play();
                    }
                }
            }
            else
            {
                PlayerPrefsManager.SetInt(PlayerPrefHelper.BGMSwitch, 0);
                
                foreach (var audioSrouce in BGMSounds)
                {
                    if (audioSrouce.Value != null)
                    {
                        audioSrouce.Value.Stop();
                    }
                }
            }
        }

        public static void SwitchSound(bool on)
        {
            if (SoundObj == null) Init();
            if (on == true && PlayerPrefsManager.GetInt(PlayerPrefHelper.SoundSwitch, 1) == 0)
            {
                PlayerPrefsManager.SetInt(PlayerPrefHelper.SoundSwitch, 1);
    
                if (MusicAudio != null)
                {
                    MusicAudio.Play();
                }
                foreach (var audioSrouce in LoopSounds)
                {
                    if (audioSrouce.Key == lastLoopSound)
                    {
                        audioSrouce.Value.Play();
                    }
                }
            }
            else
            {
                PlayerPrefsManager.SetInt(PlayerPrefHelper.SoundSwitch, 0);
    
                if (MusicAudio != null)
                {
                    MusicAudio.Stop();
                }
                foreach (var audioSrouce in LoopSounds)
                {
                    if (audioSrouce.Value != null)
                    {
                        audioSrouce.Value.Stop();
                    }
                }
            }
        }

        public static void AddPreloadAudioClipsName(string name)
        {
            preloadClipNames.Add(name);
        }

        public static void RemovePreloadAudioClipsName(string name)
        {
            preloadClipNames.Remove(name);
        }

        public static void ClearPreloadAudioClipsNames()
        {
            preloadClipNames.Clear();
        }

        public static void PreloadAudioClips()
        {
            foreach (string clipName in preloadClipNames)
            {
                if (!loadedAudioClips.ContainsKey(clipName))
                {
                    LoadAudioClip(clipName);
                }
            }
        }

        private static AudioClip LoadAudioClip(string clipName)
        {
            try
            {
                if (loadedAudioClips.ContainsKey(clipName) && loadedAudioClips[clipName] != null)
                {                
                    return loadedAudioClips[clipName];
                }            
                string path = AudioFilePath + clipName;
                AudioClip clip = ResourceLoader.LoadAudioClip(path);
                if (clip != null)
                {
                    loadedAudioClips.Add(clipName, clip);
                }
                
                return clip;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void ClearAllLoadedAudioClips()
        {
            loadedAudioClips.Clear();
        }

        public static void RemoveLoadedAudioClip(string name)
        {
            if (loadedAudioClips.ContainsKey(name))
            {
                loadedAudioClips.Remove(name);
            }
        }
    }
}

