using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Satchel;
namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerSpell()
        {
            Vector2 heroPos = _hero.transform.position;
            Vector2 pos = transform.position;
            float fireballThreshold = 5.0f;

            if (Mathf.Abs(heroPos.y - pos.y) > Mathf.Abs(heroPos.x - pos.x))
                DoppelgangerScream();
            else
            {
                if (Mathf.Abs(heroPos.x - pos.x) > fireballThreshold)
                    DoppelgangerFireball();
                else
                    DoppelgangerQuake();
            }
        }
        
        private void DoppelgangerFireball()
        {
            IEnumerator FireballAntic()
            {
                _anim.Play("Fireball Antic");
                _rb.velocity = Vector2.zero;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Fireball Antic"));

                StartCoroutine(Fireball());
            }

            IEnumerator Fireball()
            {
                _anim.Play("Fireball2 Cast");

                GameObject fireballParent = _sc.GetAction<SpawnObjectFromGlobalPool>("Fireball 2", 3).gameObject.Value;
                PlayMakerFSM fireballCast = fireballParent.LocateMyFSM("Fireball Cast");
                AudioClip castClip;
                if (_pd.equippedCharm_11)
                {
                    castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Fluke R", 0).audioClip.Value;
                    if (_pd.equippedCharm_10)
                    {
                        GameObject dungFlukeObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Dung R", 0).gameObject.Value;
                        GameObject dungFluke = Instantiate(dungFlukeObj, transform.position, Quaternion.identity);
                        dungFluke.SetActive(true);
                        dungFluke.transform.rotation = Quaternion.Euler(0, 0, 26 * -transform.localScale.x);
                        dungFluke.layer = 22;
                        PlayMakerFSM dungFlukeControl = dungFluke.LocateMyFSM("Control");
                        var blowClip = (AudioClip) dungFlukeControl.GetAction<AudioPlayerOneShotSingle>("Blow", 4).audioClip.Value;
                        Destroy(dungFluke.LocateMyFSM("Control"));
                        dungFluke.AddComponent<DamageHero>();
                        dungFluke.AddComponent<DungFluke>().blowClip = blowClip;
                        dungFluke.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(5, 15) * -transform.localScale.x, Random.Range(0, 20));
                        Destroy(dungFluke.LocateMyFSM("Control"));
                        Destroy(dungFluke.FindGameObjectInChildren("Damager"));
                    }
                    else
                    {
                        GameObject flukeObj = fireballCast.GetAction<FlingObjectsFromGlobalPool>("Flukes", 0).gameObject.Value;
                        for (int i = 0; i <= 15; i++)
                        {
                            GameObject fluke = Instantiate(flukeObj, transform.position, Quaternion.identity);
                            fluke.SetActive(true);
                            fluke.layer = 22;
                            fluke.AddComponent<DamageHero>();
                            fluke.AddComponent<SpellFlukeDoppelganger>();
                            fluke.GetComponent<AudioSource>().Play();
                            fluke.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(5, 15) * -transform.localScale.x, Random.Range(0, 20));
                            fluke.GetComponent<SpriteFlash>().flashFocusHeal();
                            Destroy(fluke.GetComponent<SpellFluke>());
                            Destroy(fluke.FindGameObjectInChildren("Damager"));
                        }    
                    }
                }
                else
                {
                    castClip = (AudioClip) fireballCast.GetAction<AudioPlayerOneShotSingle>("Cast Right", 3).audioClip.Value;
                    GameObject fireballObj = fireballCast.GetAction<SpawnObjectFromGlobalPool>("Cast Right", 4).gameObject.Value;
                    GameObject fireball = Instantiate(fireballObj, transform.position, Quaternion.identity);
                    fireball.SetActive(true);
                    fireball.layer = 22;
                    // Instantiating fireball and setting properties here is weird, so create a component for it.
                    fireball.AddComponent<Fireball>().xDir = -transform.localScale.x;
                }

                _audio.PlayOneShot(castClip);
                
                _rb.velocity = _sc.GetAction<SetVelocity2d>("Fireball Recoil", 9).vector.Value;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Fireball2 Cast"));

                _rb.velocity = Vector2.zero;
                
                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(FireballAntic());
        }

        private void DoppelgangerQuake()
        {
            IEnumerator QuakeAntic()
            {
                _anim.Play("Quake Antic");
                AudioClip quakeAnticClip = (AudioClip) _sc.GetAction<AudioPlay>("Quake Antic", 0).oneShotClip.Value;
                _audio.PlayOneShot(quakeAnticClip);
                _rb.gravityScale = 0.0f;
                _rb.velocity = Vector2.up * 11;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Quake Antic"));

                StartCoroutine(QuakeFall());
            }

            IEnumerator QuakeFall()
            {
                _anim.Play("Quake Fall 2");
                _hm.enabled = false;
                _nb.active = true;
                _rb.velocity = Vector2.down * 50.0f;

                yield return new WaitWhile(() => !TouchingGround());

                StartCoroutine(QuakeLand());
            }

            IEnumerator QuakeLand()
            {
                _anim.Play("Quake Land 2");
                AudioClip quakeLandClip = (AudioClip) _sc.GetAction<AudioPlay>("Q2 Land", 1).oneShotClip.Value;
                _audio.PlayOneShot(quakeLandClip);
                _rb.velocity = Vector2.zero;

                GameObject qSlamObj = _hero.transform.Find("Spells").Find("Q Slam 2").gameObject;
                GameObject quakeSlam = Instantiate(qSlamObj, _spells.transform
                );
                quakeSlam.SetActive(true);
                quakeSlam.layer = 22;
                GameObject hitL = quakeSlam.FindGameObjectInChildren("Hit L");
                hitL.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                GameObject hitR = quakeSlam.FindGameObjectInChildren("Hit R");
                hitR.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;

                GameObject qPillarObj = _hero.transform.Find("Spells").Find("Q Pillar").gameObject;
                GameObject quakePillar = Instantiate(qPillarObj, _spells.transform);
                quakePillar.SetActive(true);

                GameObject qMegaObj = _hero.transform.Find("Spells").Find("Q Mega").gameObject;
                GameObject qMega = Instantiate(qMegaObj, _spells.transform);
                qMega.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
                qMega.SetActive(true);
                GameObject qMegaHitL = qMega.FindGameObjectInChildren("Hit L");
                qMegaHitL.layer = 22;
                Destroy(qMegaHitL.LocateMyFSM("damages_enemy"));
                qMegaHitL.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                GameObject qMegaHitR = qMega.FindGameObjectInChildren("Hit R");
                qMegaHitR.layer = 22;
                Destroy(qMegaHitR.LocateMyFSM("damages_enemy"));
                qMegaHitR.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Quake Land 2"));

                StartCoroutine(QuakePillar());
            }

            IEnumerator QuakePillar()
            {
                _hm.enabled = true;
                _nb.active = false;
                _rb.gravityScale = 1.0f;

                yield return null;

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(QuakeAntic());
        }

        private void DoppelgangerScream()
        {
            IEnumerator ScreamAntic()
            {
                _anim.Play("Scream Start");
                AudioClip screamAnticClip = (AudioClip) _sc.GetAction<AudioPlay>("Scream Antic2", 1).oneShotClip.Value;
                _audio.PlayOneShot(screamAnticClip);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Scream Start"));
                
                StartCoroutine(Scream());
            }

            IEnumerator Scream()
            {
                _anim.Play("Scream 2 Get");

                GameObject scrHeadsObj = _hero.transform.Find("Spells").Find("Scr Heads 2").gameObject;
                GameObject screamHeads = Instantiate(scrHeadsObj, _spells.transform);//.position, Quaternion.identity);
                screamHeads.SetActive(true);
                Destroy(screamHeads.LocateMyFSM("Deactivate on Hit"));
                
                GameObject hitL = screamHeads.FindGameObjectInChildren("Hit L");
                Destroy(hitL.LocateMyFSM("damages_enemy"));
                
                GameObject hitR = screamHeads.FindGameObjectInChildren("Hit R");
                Destroy(hitR.LocateMyFSM("damages_enemy"));
                
                GameObject hitU = screamHeads.FindGameObjectInChildren("Hit U");
                Destroy(hitU.LocateMyFSM("damages_enemy"));

                var hitLDamager = Instantiate(new GameObject("Hit L"), hitL.transform);
                hitLDamager.layer = 22;
                var hitLDmgPoly = hitLDamager.AddComponent<PolygonCollider2D>();
                hitLDmgPoly.isTrigger = true;
                var hitLPoly = hitL.GetComponent<PolygonCollider2D>(); 
                hitLDmgPoly.points = hitLPoly.points;
                hitLDamager.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;

                var hitRDamager = Instantiate(new GameObject("Hit R"), hitR.transform);
                hitRDamager.layer = 22;
                var hitRDmgPoly = hitRDamager.AddComponent<PolygonCollider2D>();
                hitRDmgPoly.isTrigger = true;
                var hitRPoly = hitR.GetComponent<PolygonCollider2D>();
                hitRDmgPoly.points = hitRPoly.points;
                hitRDamager.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                
                var hitUDamager = Instantiate(new GameObject("Hit U"), hitU.transform);
                hitUDamager.layer = 22;
                var hitUDmgPoly = hitUDamager.AddComponent<PolygonCollider2D>();
                hitUDmgPoly.isTrigger = true;
                var hitUPoly = hitU.GetComponent<PolygonCollider2D>();
                hitUDmgPoly.points = hitUPoly.points;
                hitUDamager.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;

                Destroy(hitLPoly);
                Destroy(hitRPoly);
                Destroy(hitUPoly);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Scream 2 Get"));

                Destroy(hitLDamager);
                Destroy(hitRDamager);
                Destroy(hitUDamager);
                
                StartCoroutine(ScreamEnd());
            }

            IEnumerator ScreamEnd()
            {
                _anim.Play("Scream End");

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Scream End"));

                StartCoroutine(IdleAndChooseNextAttack());
            }
            
            StartCoroutine(ScreamAntic());
        }
    }
}