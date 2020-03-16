using System.Collections;
using UnityEngine;

namespace Doppelgänger
{
    public static class Tk2DSpriteAnimatorExtensions
    {
        public static void PlayReversed(this tk2dSpriteAnimator anim, string clipName, float fps = 12)
        {
            IEnumerator PlayReversed()
            {
                anim.Stop();

                tk2dSpriteAnimationClip clip = anim.GetClipByName(clipName);
                tk2dSpriteAnimationFrame[] frames = clip.frames;
                int numFrames = frames.Length;
                tk2dSpriteCollectionData collectionData = clip.GetFrame(0).spriteCollection;
                
                for (int frame = numFrames - 1; frame >= 0; frame--)
                {
                    int spriteId = frames[frame].spriteId;
                    anim.SetSprite(collectionData, spriteId);
                    yield return new WaitForSeconds(1.0f / fps);
                }
            }

            anim.StartCoroutine(PlayReversed());
        }
        
        private static void Log(object message) => Modding.Logger.Log("[Tk2DSpriteAnimator Extensions] " + message);
    }    
}