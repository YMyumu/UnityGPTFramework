using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


using ConfigModule;
using ResourceModule;
using PoolModule;
using DelayedTaskModule;

namespace AudioModule
{
    public class AudioManager : MonoSingleton<AudioManager>, IInitable
    {
        public AudioMixer audioMixer;  // 引用 AudioMixer
        private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, Audios> audioDic = new Dictionary<string, Audios>();
        private Transform _transform;
        private List<AudioSource> _playingAudioSources = new List<AudioSource>();

        public void Init()
        {
            _transform = this.transform;

            // 加载配置表
            LoadAudioConfig();

            // 初始化音频组
            InitializeMixerGroups();
        }

        private void Start()
        {

        }

        /// <summary>
        /// 加载音频配置
        /// </summary>
        private void LoadAudioConfig()
        {
            var audiosCfg = ConfigManager.Instance.GetConfig<AudiosCfg>();  // 从配置模块加载
            if (audiosCfg != null)
            {
                foreach (var audio in audiosCfg.cfg)
                {
                    audioDic[audio.Name] = audio;
                }
            }
            else
            {
                LogManager.LogError("无法加载 Audios 配置");
            }
        }

        /// <summary>
        /// 初始化 MixerGroup
        /// </summary>
        private void InitializeMixerGroups()
        {
            audioMixer = ResourceManager.Instance.LoadResource<AudioMixer>("Audio/GameAudioMixer.mixer");
            // 从配置中加载音频组
            var mixerGroupDataCfg = ConfigManager.Instance.GetConfig<MixerGroupDataCfg>();
            if (mixerGroupDataCfg != null)
            {
                foreach (var groupConfig in mixerGroupDataCfg.cfg)
                {
                    var groups = audioMixer.FindMatchingGroups(groupConfig.GroupName);
                    if (groups.Length > 0)
                    {
                        audioMixer.SetFloat($"{groupConfig.GroupName}Volume", Mathf.Log10(groupConfig.InitialVolume) * 20);
                    }
                }
            }
        }

        /// <summary>
        /// 播放指定音频，使用对象池管理 AudioSource
        /// </summary>
        public AudioSource PlayAudio(string key, Transform audioParent = null)
        {
            // 从配置字典中获取音频配置信息
            if (audioDic.TryGetValue(key, out Audios conf))
            {
                // 使用资源加载模块加载音频文件
                AudioClip clip = ResourceManager.Instance.LoadResource<AudioClip>(conf.Path);
                if (clip)
                {
                    // 从对象池中获取 AudioSource 实例
                    GameObject objAudioSource = UnityObjectPoolFactory.Instance.GetObject("AudioSourceTemplate");
                    AudioSource audioSource = objAudioSource != null ? objAudioSource.GetComponent<AudioSource>() : null;

                    if (audioSource)
                    {
                        // 设置父对象
                        if (audioParent)
                            audioSource.transform.SetParent(audioParent, false);
                        else
                            audioSource.transform.SetParent(_transform, false);

                        audioSource.transform.localPosition = Vector3.zero;
                        audioSource.gameObject.SetActive(true);
                        audioSource.clip = clip;
                        audioSource.loop = conf.IsLoop;

                        audioSource.spatialBlend = audioParent ? 1f : 0f;  // 有父对象为3D音频，无父对象为2D音频


                        // 设置 AudioSource 的 MixerGroup
                        var groups = audioMixer.FindMatchingGroups(conf.MixerName);
                        audioSource.outputAudioMixerGroup = groups.Length > 0 ? groups[0] : null;

                        // 播放音频
                        audioSource.Play();
                        _playingAudioSources.Add(audioSource);

                        // 如果不是循环音频，使用延迟任务回收
                        if (!conf.IsLoop)
                        {
                            DelayedTaskScheduler.Instance.AddDelayedTask(
                                clip.length, () => RecycleAudioSource(audioSource));
                        }

                        return audioSource;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 回收 AudioSource 到对象池
        /// </summary>
        private void RecycleAudioSource(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
            UnityObjectPoolFactory.Instance.RecycleObject("AudioSourceTemplate", audioSource.gameObject);  // 回收到对象池
            _playingAudioSources.Remove(audioSource);
        }

        /// <summary>
        /// 停止所有正在播放的音频
        /// </summary>
        public void StopAllAudio()
        {
            foreach (var audioSource in _playingAudioSources.ToArray())
            {
                RecycleAudioSource(audioSource);
            }
        }

        /// <summary>
        /// 设置音频组音量
        /// </summary>
        public void SetGroupVolume(string groupName, float volume)
        {
            audioMixer.SetFloat($"{groupName}Volume", Mathf.Log10(volume) * 20);
        }

        /// <summary>
        /// 获取音频组的当前音量
        /// </summary>
        public float GetGroupVolume(string groupName)
        {
            if (audioMixer.GetFloat($"{groupName}Volume", out float volume))
            {
                return Mathf.Pow(10, volume / 20);
            }
            return 1.0f;  // 默认返回1.0
        }


    }
}
