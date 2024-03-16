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

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Game_Assets.Scripts.Runtime.Unity_Timeline
{
    /// <summary>
    /// The track that we will be able to manipulate in modify our DOTween transform tween
    /// </summary>
    [TrackColor(148/255f,222/255f,89/255f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(DOTweenClip))]
    public class DOTweenTrack : TrackAsset
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            //playable.GetDuration() returns some super strange results when ClipCaps.Extrapolation is set
            //in order to make extrapolation work intuitively with DOTWeen we need clip.duration, so we override
            //CreatePlayable and pass the current clip to our behavior.
            //Thank my dude here https://forum.unity.com/threads/trying-to-get-percentage-of-the-way-through-playable.503672/#post-3281262
            //additional info here https://forum.unity.com/threads/timeline-adds-1-million-to-playable-getduration-when-extrapolation-is-set-to-anything-but-none.1324440/
            var playable = (ScriptPlayable<DOTweenBehavior>)base.CreatePlayable(graph, gameObject, clip);
            // grab the track so reference so that we can initialize our DOTween behavior with it's values
            var trackBinding = gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this) as Transform;
            if (trackBinding == null)
            {
                return playable;
            }
            playable.GetBehaviour().Initialize(clip, trackBinding.position, trackBinding.eulerAngles, trackBinding.localScale);
            return playable;
        }
        
        
    }
}