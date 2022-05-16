using System.Collections;
using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Satchel;
namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerJump()
        {
            bool willQuake = Random.Range(0, 2) == 0;
            bool quaked = false;
            bool canDoubleJump = true;
            bool willDoubleJump = Random.Range(0, 2) == 0;
            float diveThreshold = 2.0f;

            IEnumerator Jump()
            {
                _ac.PlaySound(HeroSounds.JUMP);
                _anim.Play("Airborne");
                float value = Random.Range(20, 25);
                _rb.velocity = new Vector2(value / 2 * -transform.localScale.x, value);

                Instantiate(_hc.jumpEffectPrefab, transform);

                yield return null;
                
                StartCoroutine(Airborne());
            }

            IEnumerator Airborne()
            {
                yield return new WaitWhile(() =>
                {
                    if (willQuake && !quaked)
                    {
                        float heroX = _hero.transform.position.x;
                        float posX = transform.position.x;
                        float distance = Mathf.Abs(heroX - posX);
                        if (distance < diveThreshold)
                        {
                            quaked = true;
                            _repeats[DoppelgangerSpell]++;
                            StartCoroutine(AirborneQuakeAntic());
                        }
                    }

                    return _rb.velocity.y > 0;
                });

                if (quaked) yield break;

                if (willDoubleJump && canDoubleJump)
                    StartCoroutine(DoubleJump());
                else
                    StartCoroutine(Fall());
            }

            IEnumerator Fall()
            {
                _anim.Play("Fall");
                
                yield return new WaitWhile(() =>
                {
                    if (willQuake && !quaked)
                    {
                        float heroX = _hero.transform.position.x;
                        float posX = transform.position.x;
                        float distance = Mathf.Abs(heroX - posX);
                        if (distance < diveThreshold)
                        {
                            quaked = true;
                            StartCoroutine(AirborneQuakeAntic());
                        }
                    }

                    return !TouchingGround();
                });

                if (quaked) yield break;

                StartCoroutine(Land());
            }

            IEnumerator DoubleJump()
            {
                canDoubleJump = false;
                
                _anim.Play("Double Jump");
                _audio.PlayOneShot(_hc.doubleJumpClip);
                float heroX = _hero.transform.position.x;
                float posX = transform.position.x;
                float diff = heroX - posX;
                if (diff > 0 && transform.localScale.x == 1 || diff < 0 && transform.localScale.x == -1) Flip();
                float value = Random.Range(20, 25);
                _rb.velocity = new Vector2(value / 2 * -transform.localScale.x, value);
                
                Instantiate(_hc.dJumpWingsPrefab, transform).SetActive(true);
                Instantiate(_hc.dJumpFlashPrefab, transform).SetActive(true);
                Instantiate(_hc.dJumpFeathers, transform).Play();

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Double Jump"));

                StartCoroutine(Airborne());
            }
            
            IEnumerator Land()
            {
                _ac.PlaySound(HeroSounds.SOFT_LANDING);
                
                Instantiate(_hc.softLandingEffectPrefab, transform);

                yield return null;

                StartCoroutine(IdleAndChooseNextAttack());
            }
            
            IEnumerator AirborneQuakeAntic()
            {
                _anim.Play("Quake Antic");
                _rb.gravityScale = 0.0f;
                AudioClip quakeAnticClip = (AudioClip) _sc.GetAction<AudioPlay>("Quake Antic", 0).oneShotClip.Value;
                _audio.PlayOneShot(quakeAnticClip);
                _rb.velocity = Vector2.zero;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Quake Antic"));

                StartCoroutine(AirborneQuakeFall());
            }

            IEnumerator AirborneQuakeFall()
            {
                _anim.Play("Quake Fall 2");
                _hm.enabled = false;
                _nb.active = true;
                _rb.velocity = Vector2.down * 50.0f;

                yield return new WaitWhile(() => !TouchingGround());

                StartCoroutine(AirborneQuakeLand());
            }

            IEnumerator AirborneQuakeLand()
            {
                _anim.Play("Quake Land 2");
                AudioClip quakeLandClip = (AudioClip) _sc.GetAction<AudioPlay>("Q2 Land", 1).oneShotClip.Value;
                _audio.PlayOneShot(quakeLandClip);
                _rb.velocity = Vector2.zero;

                GameObject qSlamObj = _hero.transform.Find("Spells").Find("Q Slam 2").gameObject;
                GameObject quakeSlam = Instantiate(qSlamObj, _spells.transform);
                quakeSlam.SetActive(true);
                quakeSlam.layer = 22;
                GameObject hitL = quakeSlam.FindGameObjectInChildren("Hit L");
                Destroy(hitL.LocateMyFSM("damages_enemy"));
                hitL.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                GameObject hitR = quakeSlam.FindGameObjectInChildren("Hit R");
                Destroy(hitR.LocateMyFSM("damages_enemy"));
                hitR.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Quake Land 2"));

                StartCoroutine(AirborneQuakePillar());
            }

            IEnumerator AirborneQuakePillar()
            {
                _hm.enabled = true;
                _nb.active = false;
                _rb.gravityScale = 1.0f;

                GameObject qPillarObj = _hero.transform.Find("Spells").Find("Q Pillar").gameObject;
                GameObject quakePillar = Instantiate(qPillarObj, _spells.transform);
                
                GameObject qMegaObj = _hero.transform.Find("Spells").Find("Q Mega").gameObject;
                GameObject qMega = Instantiate(qMegaObj, _spells.transform);
                GameObject hitL = qMega.FindGameObjectInChildren("Hit L");
                hitL.layer = 22;
                hitL.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                GameObject hitR = qMega.FindGameObjectInChildren("Hit R");
                hitR.layer = 22;
                hitR.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_19 ? 2 : 1;
                
                quakePillar.SetActive(true);

                yield return null;

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Jump());
        }
    }
}