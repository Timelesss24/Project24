using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using MEC;

namespace Core
{
    /// AnimationSystem 클래스는 Unity의 Animator 및 AnimationClip을 활용하여 
    /// 다양한 애니메이션 로직들을 처리하는 시스템입니다.
    public class AnimationSystem
    {
        PlayableGraph _playableGraph; // Unity의 PlayableGraph는 애니메이션, 오디오 및 기타 콘텐츠의 플레이블 트리를 관리합니다.
        AnimatorControllerPlayable _animatorControllerPlayable; // AnimatorControllerPlayable: AnimatorController를 Playable 형태로 제어.
        
        AnimationMixerPlayable _topLevelMixer; // 최상위 레벨의 애니메이션 믹서를 나타냅니다.
        AnimationMixerPlayable _locomotionMixer; // 캐릭터의 이동 애니메이션(로코모션)을 제어하는 믹서입니다.
        
        AnimationPlayableOutput _playableOutput; // Animator에 의해 애니메이션이 재생되도록 출력 경로를 설정함.
        
        AnimationClipPlayable _oneShotPlayable; // 단발성(One-shot) 애니메이션을 제어하기 위한 Playable입니다.
      
        CoroutineHandle _blendInHandle; // 애니메이션 블렌드 인(Blend-in) 코루틴 Handle입니다.
        CoroutineHandle _blendOutHandle; // 애니메이션 블렌드 아웃(Blend-out) 코루틴 Handle입니다.
        
        // Dictionary를 사용하여 Animator의 파라미터를 저장.
        // Animator 설정 상태를 임시적으로 유지하거나 복원하는 데 사용됨.
        Dictionary<string, object> _savedParameters = new ();
        
        // AnimationLayerMixerPlayable: Layer별 애니메이션 블렌딩과 제어를 제공.
        // 여러 레이어의 애니메이션을 혼합하여 복합적인 동작을 구현할 수 있음.
        List<AnimationLayerMixerPlayable> _layerMixerList = new();
     
        public AnimationSystem(Animator animator)
        {
            // 새 PlayableGraph를 생성하고 이름을 "AnimationSystem"으로 지정합니다.
            _playableGraph = PlayableGraph.Create("AnimationSystem");
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            SetupAnimations(animator);

            // PlayableGraph를 재생합니다.
            _playableGraph.Play();
            
        }
        
        void SetupAnimations(Animator animator)
        {
            // AnimatorControllerPlayable을 생성.
            _animatorControllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);
            
            // 최상위 애니메이션 믹서를 생성합니다. 
            // 이 믹서는 여러 입력(애니메이션, 하위 믹서 등)을 혼합하여 최종 출력으로 보냅니다.
            _topLevelMixer = AnimationMixerPlayable.Create(_playableGraph, 2);

            // 이동(Locomotion)을 처리할 믹서를 생성합니다.
            _locomotionMixer = AnimationMixerPlayable.Create(_playableGraph, 2);

            // 첫 번째 입력값을 활성화(가중치 = 1), 두 번째 입력값을 비활성화(가중치 = 0).
            _locomotionMixer.SetInputWeight(0, 1f);
            _locomotionMixer.SetInputWeight(1, 0f);
            
            // graph에서 _animatorControllerPlayable 출력을 _locomotionMixer 첫 번째 입력으로 연결.
            _playableGraph.Connect(_animatorControllerPlayable, 0, _locomotionMixer, 0);

            // _locomotionMixer 출력을 _topLevelMixer 첫 번째 입력으로 연결.
            _playableGraph.Connect(_locomotionMixer, 0, _topLevelMixer, 0);

             // 첫 번째 입력값을 활성화(가중치 = 1), 두 번째 입력값을 비활성화(가중치 = 0).
            _topLevelMixer.SetInputWeight(0, 1f);
            _topLevelMixer.SetInputWeight(1, 0f);
            
            // PlayableGraph와 Animator를 연결하기 위한 AnimationPlayableOutput을 생성합니다.
            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);
            // PlayableOutput의 소스 플레이어블을 최상위 Mixer로 설정합니다.
            playableOutput.SetSourcePlayable(_topLevelMixer);
        }
        
        /// <summary>
        /// 지정된 애니메이션 클립(AnimationClip)을 단발성(One-shot)으로 재생합니다.
        /// 현재 재생 중인 One-shot 애니메이션이 동일한 경우, 메서드는 아무 작업도 수행하지 않습니다.
        /// </summary>
        /// <param name="clip">One-shot으로 재생될 AnimationClip</param>
        public void PlayOneShot(AnimationClip clip)
        {
            if (clip == null)
            {
                // 최종적으로 clip이 null인 경우에는 애니메이션을 실행할 수 없으므로 종료.
                Debug.Log("no clip found");
                return;
            }
            
            // 기존의 One-shot 애니메이션을 중단합니다.
            InterruptOnShot();

            // 새로운 AnimationClipPlayable을 생성하여 One-shot 애니메이션으로 설정합니다.
            _oneShotPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
            
            // 최상위 믹서의 두 번째 입력에 One-shot 애니메이션을 연결합니다.
            _topLevelMixer.ConnectInput(1, _oneShotPlayable, 0);
            //_topLevelMixer.SetInputWeight(1, 0f);

            // 블렌드 지속 시간을 계산합니다.
            // 블렌드 시간은 클립 길이의 10%로 설정하되,
            // 최소 0.1초, 최대 클립 길이의 절반을 넘지 않도록 제한합니다.
            float blendDuration = Mathf.Max(0.1f, Mathf.Min(clip.length * 0.1f, clip.length / 2));

            // BlendIn 호출: 블렌드 효과로 자연스럽게 One-shot 애니메이션을 페이드 인합니다.
            BlendIn(blendDuration);

            // BlendOut 호출: 애니메이션 종료 시점에서 자연스럽게 페이드 아웃합니다.
            // 이 작업은 애니메이션 지속 시간에서 블렌드 시간만큼 뺀 지점에서 시작됩니다.
            BlendOut(blendDuration, clip.length - blendDuration);
        }

        /// <summary>
        /// One-shot 애니메이션으로의 페이드 인(Blend-in)을 처리합니다.
        /// 지정된 지속 시간 동안 부드럽게 기본 애니메이션에서 One-shot 애니메이션으로 전환됩니다.
        /// </summary>
        /// <param name="duration">페이드 인 효과가 발생하는 시간(초)</param>
        void BlendIn(float duration)
        {
            // Coroutine을 실행하여 블렌드 작업을 수행합니다.
            // blendTime에 따라 TopLevelMixer의 입력(Idle ↔ One-shot) 가중치가 점진적으로 조정됩니다.
            _blendInHandle = Timing.RunCoroutine(Blend(duration, blendTime =>
            {
                // blendTime에 따라 가중치를 선형 보간(Linear Interpolation)합니다.
                // Idle 애니메이션 가중치는 점차 1에서 0으로 감소하며,
                // One-shot 애니메이션 가중치는 0에서 1로 증가합니다.
                float weight = Mathf.Lerp(1f, 0f, blendTime);
                _topLevelMixer.SetInputWeight(0, weight);
                _topLevelMixer.SetInputWeight(1, 1f - weight);
            }));
        }

        /// <summary>
        /// One-shot 애니메이션에서 기본 애니메이션으로의 페이드 아웃(Blend-out)을 처리합니다.
        /// 지정된 지연 시간 후, 주어진 지속 시간 동안 부드럽게 전환됩니다.
        /// </summary>
        /// <param name="duration">페이드 아웃 효과가 발생하는 시간(초)</param>
        /// <param name="delay">페이드 아웃이 시작되기 전 대기 시간(초)</param>
        void BlendOut(float duration, float delay)
        {
            // Coroutine을 실행하여 페이드 아웃 작업을 수행합니다.
            // delay가 적용된 후 blendTime에 따라 가중치가 조정되고, 완료 후 DisconnectOnShot을 호출합니다.
            _blendOutHandle = Timing.RunCoroutine(Blend(duration, blendTime =>
            {
                // blendTime에 따라 입력 가중치를 선형 보간합니다.
                // Idle 애니메이션 가중치는 0에서 1로 증가하며,
                // One-shot 애니메이션 가중치는 1에서 0으로 감소합니다.
                float weight = Mathf.Lerp(0f, 1f, blendTime);
                _topLevelMixer.SetInputWeight(0, weight);
                _topLevelMixer.SetInputWeight(1, 1f - weight);
                
            }, delay, DisconnectOnShot));
        }

        /// <summary>
        /// 지정된 시간, 딜레이, 및 콜백을 기반으로 블렌딩을 처리하는 코루틴 메서드입니다.
        /// 주어진 <paramref name="blendCallback"/>을 호출하여 블렌딩 상태를 업데이트하며,
        /// 완료되면 <paramref name="finishedCallback"/>이 호출됩니다.
        /// </summary>
        /// <param name="duration">블렌딩에 소요되는 총 시간(초)</param>
        /// <param name="blendCallback">블렌드 진행 상황(0에서 1 사이 값)에 따라 호출되는 콜백</param>
        /// <param name="delay">블렌드 시작 전 대기 시간(초, 기본값 0)</param>
        /// <param name="finishedCallback">블렌드 작업 완료 후 호출되는 콜백</param>
        /// <returns>블렌딩 진행 상황(0에서 1 사이 값)을 반환하는 IEnumerator</returns>
        IEnumerator<float> Blend(float duration, Action<float> blendCallback, float delay = 0f,
            Action finishedCallback = null)
        {
            // 딜레이 값이 0 이상인 경우, 지정된 시간만큼 대기합니다.
            if (delay > 0f) yield return Timing.WaitForSeconds(delay);

            // 블렌딩 진행 상태를 나타내는 변수입니다(0 ~ 1).
            float blendTime = 0f;

            // blendTime이 1에 도달할 때까지 루프를 반복합니다.
            while (blendTime < 1f)
            {
                // Time.deltaTime을 사용해 블렌드 진행 상태를 계산합니다.
                // duration에 따라 blendTime 증가 속도가 결정됩니다.
                blendTime += Time.deltaTime / duration;

                // 현재 블렌드 상태(0 ~ 1)를 나타내는 값을 콜백 함수에 전달합니다.
                blendCallback(blendTime);

                // 코루틴 진행 상황을 반환합니다.
                yield return blendTime;
            }

            // 블렌딩이 완료되었음을 나타내기 위해 최종적으로 blendTime을 1로 설정합니다.
            blendCallback(1f);
            // 블렌딩 완료 후 실행할 작업이 정의되어 있는 경우, 해당 작업을 실행합니다.
            finishedCallback?.Invoke();
        }

        /// <summary>
        /// 실행 중인 One-shot 애니메이션을 강제로 중단하고 기본 애니메이션 상태로 되돌립니다.
        /// 진행 중인 블렌드 코루틴을 종료하고, 가중치를 초기화하며, One-shot 재생 자원을 해제합니다.
        /// </summary>
        void InterruptOnShot()
        {
            // 진행 중인 BlendIn/BlendOut 코루틴을 강제 종료합니다.
            Timing.KillCoroutines(_blendInHandle);
            Timing.KillCoroutines(_blendOutHandle);

            // 최상위 믹서를 기본 상태(Idle 애니메이션)로 복구합니다.
            // Idle 애니메이션(0번 입력)의 가중치를 1로 설정합니다.
            // One-shot 애니메이션(1번 입력)의 가중치를 0으로 설정합니다.
            _topLevelMixer.SetInputWeight(0, 1f);
            _topLevelMixer.SetInputWeight(1, 0f);

            // 만약 One-shot 애니메이션이 유효하면 연결을 해제합니다.
            if (_oneShotPlayable.IsValid())
            {
                DisconnectOnShot();
            }
        }

        void DisconnectOnShot()
        {
            _topLevelMixer.DisconnectInput(1);
            _playableGraph.DestroyPlayable(_oneShotPlayable);
        }
        
        public void Destroy()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }

        class ClipInfo
        {
            public AnimationClip Clip;
            
        }
    }
}