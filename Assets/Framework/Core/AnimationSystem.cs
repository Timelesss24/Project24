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

        AnimationPlayableOutput _playableOutput;

        readonly List<Coroutine> _blendCoroutines = new();
        List<AnimationMixerPlayable> _oneShotMixers = new();

        readonly Dictionary<AnimationMixerPlayable, Coroutine> _mixerToBlendOut = new();

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

            _animatorControllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);

            _baseMixer = AnimationMixerPlayable.Create(_playableGraph, 1);
            _baseMixer.SetInputWeight(0, 1f);
            _playableGraph.Connect(_animatorControllerPlayable, 0, _baseMixer, 0);

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

            // ▶ Blend In
            Coroutine blendIn = StartCoroutine(Blend(blendDuration, t =>
            {
                float weight = Mathf.Lerp(1f, 0f, t);
                oneShotMixer.SetInputWeight(0, weight);
                oneShotMixer.SetInputWeight(1, 1f - weight);
            }, 0f, () =>
            {
                // ✅ 블렌드 인 끝나면 이전 믹서 weight = 0, 코루틴 정지
                for (int i = 0; i < _oneShotMixers.Count - 1; i++)
                {
                    var prev = _oneShotMixers[i];
                    if (prev.IsValid())
                    {
                        prev.SetInputWeight(0, 1f);
                        prev.SetInputWeight(1, 0f);
                    }

                    if (_mixerToBlendOut.TryGetValue(prev, out var co))
                    {
                        if (co != null)
                            StopCoroutine(co);
                    }
                }
            }));

            // ▶ Blend Out
            Coroutine blendOut = StartCoroutine(Blend(blendDuration, t =>
            {
                float weight = Mathf.Lerp(0f, 1f, t);
                oneShotMixer.SetInputWeight(0, weight);
                oneShotMixer.SetInputWeight(1, 1f - weight);
            }, clip.length - blendDuration, () =>
            {
                _playableGraph.DestroyPlayable(clipPlayable);

                // 마지막 mixer일 때만 정리
                if (!_oneShotMixers[^1].Equals(oneShotMixer)) return;

                foreach (var mixer in _oneShotMixers)
                {
                    _playableGraph.DestroyPlayable(mixer);
                }
                _oneShotMixers.Clear();
                _mixerToBlendOut.Clear();

                if (_topLevelMixer.IsValid() && _topLevelMixer.GetInputCount() > 0)
                {
                    var input = _topLevelMixer.GetInput(0);
                    if (input.IsValid())
                    {
                        _topLevelMixer.DisconnectInput(0);
                    }
                }

                _topLevelMixer.ConnectInput(0, _baseMixer, 0);
                _topLevelMixer.SetInputWeight(0, 1f);

                onComplte?.Invoke();
            }));

            _blendCoroutines.Add(blendIn);
            _blendCoroutines.Add(blendOut);
            _mixerToBlendOut[oneShotMixer] = blendOut;
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