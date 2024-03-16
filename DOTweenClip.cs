// MIT License

// Copyright (c) 2024 Breakstep Studios

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Game_Assets.Scripts.Runtime.Unity_Timeline
{
    /// <summary>
    /// The clip that contains the data that we will use when animating our DOTween
    /// </summary>
    [Serializable]
    public class DOTweenClip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// If true will allow us to tween our current position to a target position.
        /// </summary>
        [Tooltip("If true will allow us to tween our current position to a target position.")]
        public bool tweenPosition;
        /// <summary>
        /// The position we will tween to, if tweenPosition is enabled.
        /// </summary>
        [Tooltip("The position we will tween to, if tweenPosition is enabled.")]
        [ShowIf("tweenPosition")]
        public Vector3 targetPosition;

        /// <summary>
        /// If true will allow us to tween our current rotation to a target rotation.
        /// </summary>
        [Tooltip("If true will allow us to tween our current rotation to a target rotation.")]
        public bool tweenRotation;

        /// <summary>
        /// The rotation we will tween to, if tweenRotation is enabled. 
        /// </summary>
        [Tooltip("The rotation we will tween to, if tweenRotation is enabled. ")]
        [ShowIf("tweenRotation")]
        public Vector3 targetRotation;

        /// <summary>
        /// If true will allow us to tween our current scale to a target scale.
        /// </summary>
        [Tooltip("If true will allow us to tween our current scale to a target scale.")]
        public bool tweenScale;

        /// <summary>
        /// The scale we will tween to, if tweenScale is enabled.
        /// </summary>
        [Tooltip("The scale we will tween to, if tweenScale is enabled.")]
        [ShowIf("tweenScale")]
        public Vector3 targetScale;
        
        /// <summary>
        /// The ease that we will use for both our tweenPosition and tweenRotation
        /// </summary>
        [Tooltip("The ease that we will use for both our tweenPosition and tweenRotation")]
        public Ease ease;

        /// <summary>
        /// If true the target values will not be reset when the clip ends, otherwise target will have values reset to those of when the clip began.
        /// </summary>
        [Tooltip("If true the target values will not be reset when the clip ends, otherwise target will have values reset to those of when the clip began.")]
        public bool retainTargetValues;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DOTweenBehavior>.Create(graph);
            var tween = playable.GetBehaviour();
            tween.tweenPosition = tweenPosition;
            tween.targetPosition = targetPosition;
            tween.tweenRotation = tweenRotation;
            tween.targetRotation = targetRotation;
            tween.tweenScale = tweenScale;
            tween.targetScale = targetScale;
            tween.ease = ease;
            tween.retainTargetValues = retainTargetValues;
            return playable;
        }

        public ClipCaps clipCaps => ClipCaps.Extrapolation;
    }
}