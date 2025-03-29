using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Core
{
    public class AnimationSystem : MonoBehaviour
    {
        PlayableGraph _playableGraph;
        AnimatorControllerPlayable _animatorControllerPlayable;

        AnimationMixerPlayable _topLevelMixer;
        AnimationMixerPlayable _baseMixer;
        AnimationMixerPlayable _latestActionMixer;

        AnimationPlayableOutput _playableOutput;

        readonly List<Coroutine> _blendCoroutines = new();
        List<AnimationMixerPlayable> _oneShotMixers = new();
        
        public bool IsSynced => _oneShotMixers.Count != 0; 

        void Start()
        {
            var animator = GetComponent<Animator>();
            Initialize(animator);
        }

        void Initialize(Animator animator)
        {
            _playableGraph = PlayableGraph.Create("AnimationSystem");
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // 기본 AnimatorController를 Playable로 변환
            _animatorControllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);

            // baseMixer는 항상 존재하며, 이동/Idle 등을 포함
            _baseMixer = AnimationMixerPlayable.Create(_playableGraph, 1);
            _baseMixer.SetInputWeight(0, 1f);
            _playableGraph.Connect(_animatorControllerPlayable, 0, _baseMixer, 0);

            // topLevelMixer는 출력으로 향하는 최상위 Mixer
            _topLevelMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
            _topLevelMixer.ConnectInput(0, _baseMixer, 0);
            _topLevelMixer.SetInputWeight(0, 1f);
            _topLevelMixer.SetInputWeight(1, 0f);

            _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);
            _playableOutput.SetSourcePlayable(_topLevelMixer);

            _playableGraph.Play();
        }

        public void PlayOneShot(AnimationClip clip, Action onComplte = null)
        {
            if (clip == null)
            {
                Debug.Log("no clip found");
                return;
            }

            // 이전 믹서를 기준으로 새 믹서를 연결
            Playable previous = _oneShotMixers.Count > 0 ? (Playable)_oneShotMixers[^1] : _baseMixer;

            if (_topLevelMixer.GetInputCount() > 0 && _topLevelMixer.GetInput(0).IsValid())
                _topLevelMixer.DisconnectInput(0);

            var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);

            var oneShotMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
            oneShotMixer.ConnectInput(0, previous, 0);
            oneShotMixer.ConnectInput(1, clipPlayable, 0);
            oneShotMixer.SetInputWeight(0, 1f);
            oneShotMixer.SetInputWeight(1, 0f);

            _topLevelMixer.ConnectInput(0, oneShotMixer, 0);
            _topLevelMixer.SetInputWeight(0, 1);
            _topLevelMixer.SetInputWeight(1, 0);

            _oneShotMixers.Add(oneShotMixer);

            float blendDuration = Mathf.Max(0.1f, Mathf.Min(clip.length * 0.1f, clip.length / 2));

            Coroutine blendIn = StartCoroutine(Blend(blendDuration, t => {
                float weight = Mathf.Lerp(1f, 0f, t);
                oneShotMixer.SetInputWeight(0, weight);
                oneShotMixer.SetInputWeight(1, 1f - weight);
            }));

            Coroutine blendOut = StartCoroutine(Blend(blendDuration, t => {
                float weight = Mathf.Lerp(0f, 1f, t);
                oneShotMixer.SetInputWeight(0, weight);
                oneShotMixer.SetInputWeight(1, 1f - weight);
            }, clip.length - blendDuration, () => {
               
                _playableGraph.DestroyPlayable(clipPlayable);
                
                // 마지막 원샷 믹서가 사라지면 baseMixer 다시 연결
                if (!_oneShotMixers[^1].Equals(oneShotMixer)) return;
                foreach (var mixer in _oneShotMixers)
                {
                    _playableGraph.DestroyPlayable(mixer);
                }
                _oneShotMixers = new();

                if (_topLevelMixer.GetInputCount() > 0 && _topLevelMixer.GetInput(0).IsValid())
                    _topLevelMixer.DisconnectInput(0);

                _topLevelMixer.ConnectInput(0, _baseMixer, 0);
                _topLevelMixer.SetInputWeight(0, 1f);
                
                onComplte?.Invoke();
            }));

            _blendCoroutines.Add(blendIn);
            _blendCoroutines.Add(blendOut);
        }

        IEnumerator Blend(float duration, Action<float> blendCallback, float delay = 0f, Action finishedCallback = null)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime / duration;
                blendCallback(time);
                yield return null;
            }

            blendCallback(1f);
            finishedCallback?.Invoke();
        }

        void OnDestroy()
        {
            foreach (var coroutine in _blendCoroutines)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
            }
            _blendCoroutines.Clear();

            if (_playableGraph.IsValid())
                _playableGraph.Destroy();
        }
    }
}