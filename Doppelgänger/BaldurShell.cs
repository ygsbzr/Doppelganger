using System.Collections;
using ModCommon;
using UnityEngine;

namespace Doppelgänger
{
    public class BaldurShell : MonoBehaviour
    {
        private tk2dSpriteAnimator _anim;
        private GameObject _shellAnim;

        private void Awake()
        {
            Destroy(gameObject.LocateMyFSM("Control"));
            
            _shellAnim = gameObject.FindGameObjectInChildren("Shell Anim");
            _anim = _shellAnim.GetComponent<tk2dSpriteAnimator>();
        }

        private IEnumerator Start()
        {
            yield return null;
            
            _anim.Play("Appear");
        }
        
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == 8 || collider.gameObject.layer == 11 || collider.gameObject.layer == 22)
            {
                Log("Blocker Hit");
                _anim.Play("Impact");
            }
        }

        private void Log(object message) => Modding.Logger.Log("[Baldur Shell] " + message);
    }
}