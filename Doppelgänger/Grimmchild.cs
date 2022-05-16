using System.Collections;
using System.Reflection;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using AddComponent = IL.HutongGames.PlayMaker.Actions.AddComponent;
using Satchel;
namespace Doppelgänger
{
    public class Grimmchild : MonoBehaviour
    {
        public GameObject Owner;

        private const float FlyDistStop = 3.123348f;
        private const float FlySpeed = 7.123348f;
        private const float MaxSpeed = 15.0f;
        
        private tk2dSpriteAnimator _anim;
        private GameObject _hero = HeroController.instance.gameObject;
        private HealthManager _hm;
        private PlayMakerFSM _control;
        private Rigidbody2D _rb;
        private Vector3 _offset;

        private bool _awake;

        private void Awake()
        {    
            Log("Grimmchild Awake");
            /*PlayMakerFSM oldControl = gameObject.LocateMyFSM("Control");
            _control = gameObject.AddComponent<PlayMakerFSM>();
            Log("Getting Fields");
            foreach (FieldInfo fi in typeof(PlayMakerFSM).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fi.SetValue(_control, fi.GetValue(oldControl));
            }

            _control.FsmName = "Clone Control";
            Log("First PFSM: " + gameObject.GetComponents<PlayMakerFSM>()[0].FsmName);
            Log("Destroy old Control");
            Destroy(gameObject.GetComponents<PlayMakerFSM>()[0]);*/

            gameObject.AddComponent<NonBouncer>().active = true;
            foreach (Transform childTransform in transform)
            {
                GameObject child = childTransform.gameObject;
                child.AddComponent<NonBouncer>().active = true;
            }
            _hm = gameObject.AddComponent<HealthManager>();
            _hm.hp = 10000;
            
            _rb = GetComponent<Rigidbody2D>();

            Destroy(gameObject.FindGameObjectInChildren("Enemy Range"));

            _anim = GetComponent<tk2dSpriteAnimator>();
            _hm = GetComponent<HealthManager>();
            _rb = GetComponent<Rigidbody2D>();
            
            On.HealthManager.TakeDamage += OnTakeDamage;
        }

        private IEnumerator Start()
        {
            Log("Grimmchild Start");
            
            yield return new WaitWhile(() => Owner == null);

            Log("Number of Clips: " + _anim.Library.clips.Length);
            foreach (var animation in _anim.Library.clips)
            {
                Log("Animation Name: " + animation.name);
            }

            _offset = new Vector3(2, 1.880373f, 0);

            StartCoroutine(GrimmchildIntro());

            /*Log("Grimmchild Start");
            _control.SetState("Rest Start");
            Log("Chainging Tele Position");
            _control.GetAction<GetPosition>("Tele", 0).Owner = Owner;
            Log("Changing Antic Target");
            _control.GetAction<FaceObject>("Antic", 1).objectB.Value = _hero;
            Log("Changing Shoot Target");
            _control.GetAction<FireAtTarget>("Shoot", 7).target.Value = _hero;
            Log("Changing Follow Target");
            _control.GetAction<DistanceFlySmooth>("Follow", 11).target.Value = Owner;

            Log("Getting Grimmball");
            GameObject grimmBall = _control.GetAction<SpawnObjectFromGlobalPool>("Shoot", 4).gameObject.Value;
            grimmBall.layer = 22;
            grimmBall.AddComponent<DamageHero>();
            //Log("Destroying Enemy Damager");
            //Destroy(grimmBall.FindGameObjectInChildren("Enemy Damager"));
            //Log("Setting Control State");
            //grimmBall.LocateMyFSM("Control").SetState("Init");
            Log("Changing Shoot GO");
            _control.GetAction<SpawnObjectFromGlobalPool>("Shoot", 4).gameObject.Value = grimmBall;
            Log("Printing GC Hierarchy");
            grimmBall.PrintSceneHierarchyTree();*/
        }

        private void Update()
        {
            if (!_awake) return;
            
            Vector3 flyToTarget = Owner.transform.position + _offset * -Owner.transform.localScale.x;
            if (Vector2.Distance(flyToTarget, transform.position) < FlyDistStop)
                _rb.velocity = Vector2.zero;
            else
                _rb.velocity = Vector2.ClampMagnitude((flyToTarget - transform.position) * FlySpeed, MaxSpeed);

            if (transform.localScale.x < 0 && Owner.transform.localScale.x < 0 ||
                transform.localScale.x > 0 && Owner.transform.localScale.x > 0)
            {
                StartCoroutine(GrimmchildTurn());
            }
        }
        
        private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (self.gameObject.name == "Doppelgänger Grimmchild")
            {
                Owner.GetComponent<HealthManager>().Hit(hitInstance);
            }
            
            orig(self, hitInstance);
        }

        private IEnumerator GrimmchildIntro()
        {
            _anim.Play("Sleep 4");
            yield return new WaitForSeconds(2.0f);

            _anim.Play("Wake 4");
            _awake = true;

            yield return new WaitForSeconds(_anim.PlayAnimGetTime("Wake 4"));
            
            StartCoroutine(GrimmchildIdle());
        }

        private IEnumerator GrimmchildIdle()
        {
            _anim.Play("Idle 4");

            yield return null;
        }

        private IEnumerator GrimmchildTurn()
        {
            _anim.Play("TurnToIdle 4");
            
            yield return new WaitForSeconds(_anim.PlayAnimGetTime("TurnToIdle 4"));
            Flip();
        }
        
        private void Flip()
        {
            Vector2 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        
        private void Log(object message) => Modding.Logger.Log("[Grimmchild] " + message);
    }
}