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


using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Game_Assets.Scripts.Runtime.Unity_Timeline
{
    /// <summary>
    /// The playable that will actually drive the animation behavior using <see cref="DOTween"/>
    /// </summary>
    public class DOTweenBehavior : PlayableBehaviour
    {
        public bool tweenPosition;
        public Vector3 targetPosition;
        public bool tweenRotation;
        public Vector3 targetRotation;
        public bool tweenScale;
        public Vector3 targetScale;
        public Ease ease;
        public bool retainTargetValues;
        
        /// <summary>
        /// The binding source we are animating
        /// </summary>
        private Transform transform;
        /// <summary>
        /// True if our first frame has been processed, false otherwise
        /// </summary>
        private bool firstFrameProcessed;
        /// <summary>
        /// The dotween sequence that contains our position, rotation and scale tweens where applicable
        /// </summary>
        private Sequence sequence;
        /// <summary>
        /// The position of our binding source will start at when clip.progress = 0 
        /// </summary>
        private Vector3 startPosition;
        /// <summary>
        /// The rotation of our binding source will start at when clip.progress = 0 
        /// </summary>
        private Vector3 startRotation;
        /// <summary>
        /// The scale of our binding source will start at when clip.progress = 0 
        /// </summary>
        private Vector3 startScale;
        /// <summary>
        /// The reference to the clip that is representing this behavior in timeline
        /// </summary>
        private TimelineClip clip;
        /// <summary>
        /// The position of our track binding before our track starts
        /// </summary>
        private Vector3 trackBindingStartPosition;
        /// <summary>
        /// The rotation of our track binding before our track starts
        /// </summary>
        private Vector3 trackBindingStartRotation;
        /// <summary>
        /// The scale of our track binding before our track starts
        /// </summary>
        private Vector3 trackBindingStartScale;

        /// <summary>
        /// Initializes our behavior with necessary values
        /// </summary>
        /// <param name="clip">The reference to the clip that is representing this behavior in timeline</param>
        /// <param name="trackBindingStartPosition">The position of our track binding before our track starts</param>
        /// <param name="trackBindingStartRotation">The rotation of our track binding before our track starts</param>
        /// <param name="trackBindingStartScale">The scale of our track binding before our track starts</param>
        public void Initialize(TimelineClip clip, Vector3 trackBindingStartPosition, Vector3 trackBindingStartRotation, Vector3 trackBindingStartScale)
        {
            this.clip = clip;
            this.trackBindingStartPosition = trackBindingStartPosition;
            this.trackBindingStartRotation = trackBindingStartRotation;
            this.trackBindingStartScale = trackBindingStartScale;
        }
        
        public override void OnGraphStop(Playable playable)
        {
            //only reset binding source back to track start positions in editor mode when we are no longer viewing timeline
            //this ensures that we don't retain timeline values when we don't want to
            if (Application.isPlaying)
            {
                return;
            }
            
            if (transform == null)
            {
                return;
            }
            
            transform.position = trackBindingStartPosition;
            transform.eulerAngles = trackBindingStartRotation;
            transform.localScale = trackBindingStartScale;
            base.OnGraphStop(playable);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            transform = playerData as Transform;
            if (transform == null)
            {
                return;
            }

            //if the first frame hasn't been processed, we know we are starting a new clip play / scrub
            //so make sure to save any defaults before we start animating things, so we can easily rest them later
            if (!firstFrameProcessed)
            {
                startPosition = transform.position;
                startRotation = transform.eulerAngles;
                startScale = transform.localScale;
                firstFrameProcessed = true;
            }
            
            //kill pre-existing tween if any and create a new one
            sequence?.Kill();
            sequence = DOTween.Sequence();
            
            //if tweenPosition is enabled, append it to our sequence in order to tween it
            var myPosition = startPosition;
            if (tweenPosition)
            {
                sequence.Append(DOTween.To(
                    () => myPosition,
                    x => myPosition = x, 
                    targetPosition,
                    (float)clip.duration));
            }

            //if tweenRotation is enabled, append it to our sequence in order to tween it
            var myRotation = startRotation;
            if (tweenRotation)
            {
                sequence.Insert(0,DOTween.To(
                    () => myRotation,
                    x => myRotation = x,
                    targetRotation,
                    (float)clip.duration));
            }

            //if tweenScale is enabled, append it to our sequence in order to tween it
            var myScale = startScale;
            if (tweenScale)
            {
                sequence.Insert(0,DOTween.To(
                    () => myScale,
                    x => myScale = x,
                    targetScale,
                    (float)clip.duration));
            }
            
            //set any global sequence params
            sequence.SetEase(ease);
            
            //move to the point in time of our sequence and set our transform accordingly
            sequence.Goto((float)playable.GetTime());
            if (tweenPosition)
            {
                transform.position = myPosition;
            }
            if (tweenRotation)
            {
                transform.eulerAngles = myRotation;
            }
            if (tweenRotation)
            {
                transform.localScale = myScale;
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //clip is no longer being played / scrubbed so reset all our defaults back
            //see https://youtu.be/LSrcQJHDUT4?t=422
            firstFrameProcessed = false;

            //if we are retaining target values, don't reset our binding source values back to start position when clip ends
            if (retainTargetValues)
            {
                return;
            }
            
            if (transform == null)
            {
                return;
            }
            
            if (tweenPosition)
            {
                transform.position = startPosition;
            }
            if (tweenRotation)
            {
                transform.eulerAngles = startRotation;
            }
            if (tweenScale)
            {
                transform.localScale = startScale;
            }
            base.OnBehaviourPause(playable, info);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            sequence?.Kill();
        }
    }
}