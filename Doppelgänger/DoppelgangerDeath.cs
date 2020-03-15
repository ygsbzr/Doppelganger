using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doppelgänger
{
    public class DoppelgangerDeath : MonoBehaviour
    {
        public GameObject Doppelganger;
        
        private readonly HeroController _hc = HeroController.instance;
        
        private IEnumerator Start()
        {
            GameObject heroDeath = _hc.transform.Find("Hero Death").gameObject;
            PlayMakerFSM heroDeathAnim = heroDeath.LocateMyFSM("Hero Death Anim");
            GameObject headObj = heroDeathAnim.GetAction<CreateObject>("Head Left", 0).gameObject.Value;
            GameObject head = Instantiate(headObj, transform.position, Quaternion.identity);
            float angle = heroDeathAnim.GetAction<SetVelocityAsAngle>("Head Left", 2).angle.Value;
            float speed = heroDeathAnim.GetAction<SetVelocityAsAngle>("Head Left", 2).speed.Value;
            float yVel = speed * (float) Math.Sin(angle * Mathf.Deg2Rad);
            float xVelValue = speed * (float) Math.Cos(angle * Mathf.Deg2Rad);
            List<float> xVels = new List<float>{-xVelValue, xVelValue};
            int index = Random.Range(0, xVels.Count);
            float xVel = xVels[index];
            head.GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);
            head.SetActive(true);

            yield return new WaitForSeconds(3.0f);
            var bossSceneController = GameObject.Find("Boss Scene Controller");
            var bsc = bossSceneController.GetComponent<BossSceneController>();
            GameObject transition = Instantiate(bsc.transitionPrefab);
            PlayMakerFSM transitionsFSM = transition.LocateMyFSM("Transitions");
            transitionsFSM.SetState("Out Statue");
            yield return new WaitForSeconds(1.0f);
            bsc.DoDreamReturn();
        }

        private void Log(object message) => Modding.Logger.Log("[Doppelganger Death] " + message);
    }
}