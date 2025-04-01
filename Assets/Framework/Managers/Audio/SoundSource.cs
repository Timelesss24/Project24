using Framework.Audio;
using Unity.VisualScripting;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource; // 오디오 소스 컴포넌트

    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        if (_audioSource == null)
            _audioSource = gameObject.GetOrAddComponent<AudioSource>(); // 오디오 소스 가져오기 (초기화)

        CancelInvoke(); // 기존에 예약된 Invoke 취소
        _audioSource.clip = clip; // 재생할 오디오 클립 설정
        _audioSource.volume = soundEffectVolume; // 볼륨 설정
        _audioSource.Play(); // 오디오 재생
        
        // 피치(음 높낮이) 설정 (랜덤 변동 적용)
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        // 클립 재생 길이 + 0.5초 후 Disable 호출 (풀에 반환)
        Invoke(nameof(Disable), clip.length + 0.5f);
    }

    public void Disable()
    {
        _audioSource.Stop();
        // Destroy 대신 풀로 반환
        SoundManager.Instance.ReturnSoundSource(this);
    }
}