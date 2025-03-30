// namespace Core
// {
//     public class temp
//     {
//         using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Animations;
// using UnityEngine.Playables;
//
// namespace Core
// {
//     public class AnimationSystem : MonoBehaviour
//     {
//         PlayableGraph _playableGraph;
//         AnimatorControllerPlayable _animatorControllerPlayable;
//
//         AnimationMixerPlayable _topLevelMixer;
//         AnimationMixerPlayable _baseMixer;
//         AnimationMixerPlayable _latestActionMixer;
//
//         AnimationPlayableOutput _playableOutput;
//
//         Dictionary<string, object> _savedParameters = new();
//         List<AnimationLayerMixerPlayable> _layerMixerList = new();
//         List<Coroutine> _blendCoroutines = new();
//
//         AnimationMixerPlayable _pendingCleanupTarget;
//
//         void Start()
//         {
//             var animator = GetComponent<Animator>();
//             Initialize(animator);
//         }
//
//         public void Initialize(Animator animator)
//         {
//             _playableGraph = PlayableGraph.Create("AnimationSystem");
//             _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
//
//             SetupAnimations(animator);
//             _playableGraph.Play();
//         }
//
//         void SetupAnimations(Animator animator)
//         {
//             _animatorControllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, animator.runtimeAnimatorController);
//
//             _baseMixer = AnimationMixerPlayable.Create(_playableGraph, 1);
//             _baseMixer.SetInputWeight(0, 1f);
//             _playableGraph.Connect(_animatorControllerPlayable, 0, _baseMixer, 0);
//
//             _topLevelMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
//             _topLevelMixer.SetInputWeight(0, 1f);
//             _topLevelMixer.SetInputWeight(1, 0f);
//             _topLevelMixer.ConnectInput(0, _baseMixer, 0);
//
//             _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);
//             _playableOutput.SetSourcePlayable(_topLevelMixer);
//         }
//
//         public void PlayOneShot(AnimationClip clip)
//         {
//             if (clip == null)
//             {
//                 Debug.Log("no clip found");
//                 return;
//             }
//
//             // 반드시 먼저 연결 끊기 (오류 방지)
//             if (_topLevelMixer.GetInputCount() > 0 && _topLevelMixer.GetInput(0).IsValid())
//                 _topLevelMixer.DisconnectInput(0);
//
//             Playable previous = _latestActionMixer.IsValid() ? (Playable)_latestActionMixer : _baseMixer;
//
//             var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
//
//             var newActionMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
//             newActionMixer.ConnectInput(0, previous, 0);
//             newActionMixer.ConnectInput(1, clipPlayable, 0);
//             newActionMixer.SetInputWeight(0, 1f);
//             newActionMixer.SetInputWeight(1, 0f);
//
//             _topLevelMixer.ConnectInput(0, newActionMixer, 0);
//             _topLevelMixer.SetInputWeight(0, 1);
//             _topLevelMixer.SetInputWeight(1, 0);
//
//             _pendingCleanupTarget = _latestActionMixer;
//             _latestActionMixer = newActionMixer;
//
//             float blendDuration = Mathf.Max(0.1f, Mathf.Min(clip.length * 0.1f, clip.length / 2));
//
//             Coroutine blendIn = StartCoroutine(Blend(blendDuration, blendTime =>
//             {
//                 float weight = Mathf.Lerp(1f, 0f, blendTime);
//                 newActionMixer.SetInputWeight(0, weight);
//                 newActionMixer.SetInputWeight(1, 1f - weight);
//             }));
//
//             Coroutine blendOut = StartCoroutine(Blend(blendDuration, blendTime =>
//             {
//                 float weight = Mathf.Lerp(0f, 1f, blendTime);
//                 newActionMixer.SetInputWeight(0, weight);
//                 newActionMixer.SetInputWeight(1, 1f - weight);
//             }, clip.length - blendDuration, () =>
//             {
//                 if (_latestActionMixer.Equals(newActionMixer))
//                 {
//                     CleanupOldMixers(newActionMixer);
//                 }
//             }));
//
//             _blendCoroutines.Add(blendIn);
//             _blendCoroutines.Add(blendOut);
//         }
//
//         void CleanupOldMixers(AnimationMixerPlayable keep)
//         {
//             void CleanupRecursive(Playable current)
//             {
//                 if (!current.IsValid()
//                     || current.Equals(_animatorControllerPlayable)
//                     || current.Equals(_baseMixer)
//                     || current.Equals(keep))
//                     return;
//
//                 for (int i = 0; i < current.GetInputCount(); i++)
//                 {
//                     var input = current.GetInput(i);
//                     CleanupRecursive(input);
//                 }
//
//                 _playableGraph.DestroyPlayable(current);
//             }
//
//             var input = keep.GetInput(0);
//             if (input.IsValid() && !input.Equals(_baseMixer))
//             {
//                 CleanupRecursive(input);
//             }
//
//             // ✅ 기존 입력이 _baseMixer일 수도 있으므로 체크
//             if (_topLevelMixer.GetInput(0).IsValid())
//             {
//                 _topLevelMixer.DisconnectInput(0);
//             }
//
//             _topLevelMixer.ConnectInput(0, _baseMixer, 0);
//             _topLevelMixer.SetInputWeight(0, 1f);
//             _topLevelMixer.SetInputWeight(1, 0f);
//             _latestActionMixer = default;
//         }
//
//         IEnumerator Blend(float duration, Action<float> blendCallback, float delay = 0f, Action finishedCallback = null)
//         {
//             if (delay > 0f) yield return new WaitForSeconds(delay);
//
//             float blendTime = 0f;
//             while (blendTime < 1f)
//             {
//                 blendTime += Time.deltaTime / duration;
//                 blendCallback(blendTime);
//                 yield return null;
//             }
//
//             blendCallback(1f);
//             finishedCallback?.Invoke();
//         }
//
//         void OnDestroy()
//         {
//             foreach (var coroutine in _blendCoroutines)
//             {
//                 if (coroutine != null)
//                     StopCoroutine(coroutine);
//             }
//             _blendCoroutines.Clear();
//
//             if (_playableGraph.IsValid())
//             {
//                 _playableGraph.Destroy();
//             }
//         }
//
//         class ClipInfo
//         {
//             public AnimationClip Clip;
//         }
//     }
// }
//
//     }
// }