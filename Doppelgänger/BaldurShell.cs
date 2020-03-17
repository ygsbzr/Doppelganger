using System.Collections;
using HutongGames.PlayMaker.Actions;
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
            _shellAnim.GetComponent<MeshRenderer>().enabled = true;
            _anim = _shellAnim.GetComponent<tk2dSpriteAnimator>();
        }

        private IEnumerator Start()
        {
            yield return null;

            GameObject pusherObj = gameObject.FindGameObjectInChildren("Pusher");
            GameObject pusher = Instantiate(pusherObj, transform);
            pusher.SetActive(true);
            pusher.layer = 11;
            Destroy(pusher.LocateMyFSM("Deactivate"));

            GameObject hitLObj = pusher.FindGameObjectInChildren("Hit L");
            GameObject hitL = Instantiate(hitLObj, pusher.transform);
            hitL.SetActive(true);
            hitL.AddComponent<PusherHit>();
            Log("Hit L Active Self: " + hitL.activeSelf);
            Log("Hit L Active Hierarchy: " + hitL.activeInHierarchy);

            GameObject hitRObj = pusher.FindGameObjectInChildren("Hit R");
            GameObject hitR = Instantiate(hitRObj, pusher.transform);
            hitR.SetActive(true);
            hitR.AddComponent<PusherHit>();
            
            GameObject hitUObj = pusher.FindGameObjectInChildren("Hit U");
            GameObject hitU = Instantiate(hitUObj, pusher.transform);
            hitU.SetActive(true);
            hitU.AddComponent<PusherHit>();
            
            _shellAnim.layer = 11;
            
            gameObject.PrintSceneHierarchyTree();
        }

        private void BaldurClose()
        {
            _anim.Play("Appear");
        }
        
        private void BaldurHit()
        {
            _anim.Play("Impact");
         
            gameObject.transform.parent.gameObject.SendMessage("BaldurShellBreak");
            
            BaldurOpen();
        }

        private void BaldurOpen()
        {
            IEnumerator Open()
            {
                _anim.Play("Disappear");

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Disappear"));

                Destroy(gameObject);
            }

            StartCoroutine(Open());
        }
        
        private void OnTriggerEnter2D(Collider2D collider)
        {
            Log("Trigger Enter: " + collider.gameObject.tag);
            if (collider.gameObject.CompareTag("Nail Attack") || collider.gameObject.CompareTag("Hero Spell"))
            {
                BaldurHit();
            }
        }

        private void Log(object message) => Modding.Logger.Log("[Baldur Shell] " + message);
    }
}