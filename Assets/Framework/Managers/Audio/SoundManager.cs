﻿using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils;

namespace Framework.Audio
{
    /// <summary>
    /// 게임 전역에서 오디오(BGM/SFX)를 관리하는 매니저
    /// </summary>
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        [Header("BGM Settings")]
        [SerializeField] private AudioSource bgmSource; // 배경 음악용 오디오 소스

        [FormerlySerializedAs("bgmVolume")]
        [Range(0f, 1f)] public float BGMVolume = 1f; // 배경음악(BGM)의 볼륨

        [SerializeField] private List<AudioClip> bgmClips; // 배경 음악 클립 리스트

        [Header("SFX Settings")]
        [SerializeField]
        [Range(0f, 1f)]
        private float sfxVolume = 1f; // 효과음 볼륨

        [SerializeField][Range(0f, 1f)] private float sfxPitchVariance; // 효과음 피치 변동 범위

        [FormerlySerializedAs("soundSourcePrefab")]
        public SoundSource SoundSourcePrefab; // 효과음 재생을 위한 사운드 소스 프리팹

        [SerializeField] private bool isMuted; // 음소거 설정 여부

        private Coroutine _fadeCoroutine; // 현재 활성화된 페이드 코루틴


        // 추가
        private Queue<SoundSource> soundSourcePool = new Queue<SoundSource>();
        [SerializeField] private AudioClip clickSound;
        public AudioClip ClickSound => clickSound;


        protected void Start()
        {
            bgmSource = gameObject.GetOrAdd<AudioSource>();

            ApplyVolumeSettings(); // 초기 볼륨 설정

            //PlayBGM(GameStateManager.Instance.CurrentState.ToString().Replace("Scene", ""));
        }

        protected void Update()
        {
            //BGMSetting();
        }

        /// <summary>
        /// 볼륨 설정 적용
        /// </summary>
        private void ApplyVolumeSettings()
        {
            bgmSource.volume = isMuted ? 0f : BGMVolume;
            bgmSource.loop = true;
            //isMuted | isSfxMuted? 0f : sfxVolume;
        }

        public void BGMSetting()
        {
            GameObject Boss = GameObject.FindGameObjectWithTag("Boss");
            if (Boss != null && Boss.activeSelf)
            {
                ChangeBGMWithFade("Last Gaint", 2.5f);
                //StartCoroutine(ChangeBGMCoroutine("Last Gaint", 5.2f));
                //PlayBGM("Last Gaint");
            }
            else
            {
                ChangeBGMWithFade($"{GameStateManager.Instance.CurrentState.ToString().Replace("Scene", "")}BGM", 1.2f);
                //StartCoroutine(ChangeBGMCoroutine($"{GameStateManager.Instance.CurrentState.ToString().Replace("Scene", "")}BGM", 4.2f));
            }
        }

        /// <summary>
        /// BGM 재생
        /// </summary>
        /// <param name="clipName">재생할 BGM 클립 이름</param>
        public void PlayBGM(string clipName)
        {
            if (isMuted) return;

            // 이름으로 BGM 찾기
            AudioClip clip = bgmClips.Find(c => c.name.Contains(clipName));

            if (clip != null && bgmSource.clip != clip)
            {
                bgmSource.Stop(); // 현재 배경 음악 정지
                bgmSource.clip = clip; // 새로운 배경 음악 설정
                bgmSource.Play(); // 배경 음악 재생
            }
        }

        /// <summary>
        /// BGM 중지
        /// </summary>
        public void StopBGM()
        {
            bgmSource.Stop();
        }

        /// <summary>
        /// SFX 재생 
        /// </summary>
        /// <param name="clip">재생할 효과음 클립</param>
        public static void PlaySfx(AudioClip clip)
        {
            if (Instance.isMuted || !clip) return;

            SoundSource soundSource = Instance.GetSoundSourceFromPool(); // 풀에서 SoundSource 가져오기
            soundSource.Play(clip, Instance.sfxVolume, Instance.sfxPitchVariance); // 효과음 재생
        }

        /// <summary>
        /// 풀에서 SoundSource 가져오기. 없으면 새로 생성.
        /// </summary>
        private SoundSource GetSoundSourceFromPool()
        {
            if (soundSourcePool.Count > 0)
            {
                SoundSource source = soundSourcePool.Dequeue();
                source.gameObject.SetActive(true);
                return source;
            }
            else
            {
                return Instantiate(SoundSourcePrefab, transform);
            }
        }

        /// <summary>
        /// 사용 완료한 SoundSource를 풀에 반환.
        /// </summary>
        public void ReturnSoundSource(SoundSource source)
        {
            source.gameObject.SetActive(false);
            soundSourcePool.Enqueue(source);
        }

        /// <summary>
        /// 오디오 음소거 설정
        /// </summary>
        /// <param name="mute">음소거 여부</param>
        public void SetMute(bool mute)
        {
            isMuted = mute;
            ApplyVolumeSettings();
        }

        /// <summary>
        /// 배경음악 볼륨 설정
        /// </summary>
        /// <param name="volume">볼륨 크기</param>
        public void SetBGMVolume(float volume)
        {
            BGMVolume = Mathf.Clamp(volume, 0f, 1f);
            ApplyVolumeSettings();
        }

        /// <summary>
        /// 효과음 볼륨 설정
        /// </summary>
        /// <param name="volume">볼륨 크기</param>
        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp(volume, 0f, 1f);
            ApplyVolumeSettings();
        }

        /// <summary>
        /// 배경음악 페이드아웃
        /// </summary>
        /// <param name="fadeDuration">페이드 아웃 시간</param>
        private IEnumerator FadeOutBGM(float fadeDuration)
        {
            // 현재 실행 중인 페이드 코루틴 중단 (중복 실행 방지)
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            float startVolume = bgmSource.volume;

            while (bgmSource.volume > 0)
            {
                bgmSource.volume =
                    Mathf.MoveTowards(bgmSource.volume, 0f, (startVolume / fadeDuration) * Time.deltaTime);
                yield return null;
            }
            bgmSource.Stop();
            bgmSource.volume = 0; // 완전히 0으로 설정
        }

        /// <summary>
        /// 배경음악 페이드인
        /// </summary>
        /// <param name="clipName">재생할 클립 이름</param>
        /// <param name="fadeDuration">페이드 인 시간</param>
        private IEnumerator FadeInBGM(string clipName, float fadeDuration)
        {
            // 새로운 클립 가져오기
            AudioClip newClip = bgmClips.Find(c => c.name == clipName);

            if (!newClip)
            {
                Debug.LogError($"SoundManager: BGM '{clipName}' not found.");
                yield break;
            }

            if (bgmSource.clip != newClip | !bgmSource.isPlaying)
            {
                bgmSource.clip = newClip;
                bgmSource.volume = 0;
                bgmSource.Play();
            }

            while (bgmSource.volume < BGMVolume)
            {
                bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, BGMVolume,
                    (BGMVolume / fadeDuration) * Time.deltaTime);
                yield return null;
            }

            bgmSource.volume = BGMVolume; // 최종 볼륨 고정
        }

        /// <summary>
        /// BGM 변경 및 페이드 처리 (외부 메서드 호출)
        /// </summary>
        /// <param name="clipName">변경할 BGM 이름</param>
        /// <param name="fadeDuration">페이드 시간</param>
        public void ChangeBGMWithFade(string clipName, float fadeDuration)
        {
            // 기존 코루틴 중단 후 새로운 코루틴 시작
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(ChangeBGMCoroutine(clipName, fadeDuration));
        }

        /// <summary>
        /// 페이드 아웃 후 페이드 인
        /// </summary>
        /// <param name="clipName">재생할 클립 이름</param>
        /// <param name="fadeDuration">페이드 시간</param>
        private IEnumerator ChangeBGMCoroutine(string clipName, float fadeDuration)
        {
            float initialVolume = bgmSource.volume;
            // 클립이 같고 이미 재생 중이라면 페이드아웃 생략
            if (bgmSource.isPlaying && bgmSource.clip && (bgmSource.clip.name == clipName))
            {
                bgmSource.volume = initialVolume; // 현재 볼륨 유지
            }
            else
            {
                yield return FadeOutBGM(fadeDuration); // 현재 재생 중인 음악 페이드아웃
            }
            yield return FadeInBGM(clipName, fadeDuration); // 새로운 음악 페이드인
        }

        /// <summary>
        /// 독립적으로 페이드 아웃 시작
        /// </summary>
        /// <param name="fadeDuration">페이드 아웃 시간</param>
        public void StartFadeOut(float fadeDuration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeOutBGM(fadeDuration));
        }

        /// <summary>
        /// 독립적으로 페이드 인 시작
        /// </summary>
        /// <param name="clipName">재생할 클립 이름</param>
        /// <param name="fadeDuration">페이드 인 시간</param>
        public void StartFadeIn(string clipName, float fadeDuration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeInBGM(clipName, fadeDuration));
        }
    }
}