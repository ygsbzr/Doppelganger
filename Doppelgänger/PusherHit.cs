using System.Collections;
using ModCommon;
using UnityEngine;

namespace Doppelgänger
{
    public class PusherHit : MonoBehaviour
    {
        private GameObject _baldurShell;

        private IEnumerator Start()
        {
            yield return new WaitWhile(() => gameObject == null);
            
            Log("PusherHit Start");
            GameObject go = gameObject;
            go.layer = 11;
            Destroy(go.LocateMyFSM("push_enemy"));
            go.AddComponent<DamageHero>();
            go.AddComponent<TinkEffect>();
            go.AddComponent<TinkSound>();
            GameObject pusher = go.transform.parent.gameObject;
            _baldurShell = pusher.transform.parent.gameObject;
            
            _baldurShell.PrintSceneHierarchyTree();
        }
        
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Nail Attack") || collider.gameObject.CompareTag("Hero Spell"))
            {
                _baldurShell.SendMessage("BaldurHit");
            }
        }

        private void Log(object message) => Modding.Logger.Log("[Pusher Hit] " + message);
    }
}