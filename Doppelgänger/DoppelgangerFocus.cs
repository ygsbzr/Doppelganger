using System.Collections;
using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;

namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerFocus()
        {
            GameObject baldurShell = null;
            GameObject dustL;
            GameObject dustR;
            GameObject linesAnim;
            GameObject focusEffects = _hero.FindGameObjectInChildren("Focus Effects");
            PlayMakerFSM spellControl = _hero.LocateMyFSM("Spell Control");
            
            IEnumerator StartFocus()
            {
                float initialWaitTime = spellControl.GetAction<Wait>("Focus Start", 17).time.Value;
                yield return new WaitForSeconds(initialWaitTime);

                if (_pd.equippedCharm_28)
                    _anim.Play("Slug Down");
                else
                    _anim.PlayReversed("Focus End");

                float crawlSpeed;
                if (_pd.equippedCharm_28 && _pd.equippedCharm_7)
                    crawlSpeed = 12;
                else if (_pd.equippedCharm_28)
                    crawlSpeed = 6;
                else
                    crawlSpeed = 0;
                    
                _rb.velocity = Vector2.right * -transform.localScale.x * crawlSpeed;

                tk2dSpriteAnimationClip animClip = _anim.GetClipByName("Focus End");
                float waitTime = animClip.frames.Length * (1.0f / animClip.fps);

                yield return new WaitForSeconds(waitTime);

                StartCoroutine(Focus());
            }

            IEnumerator Focus()
            {
                string focusAnimation;
                if (_pd.equippedCharm_28)
                {
                    focusAnimation = "Slug Walk";
                    if (_pd.equippedCharm_5 && _pd.equippedCharm_17)
                        focusAnimation += " BS";
                    else if (_pd.equippedCharm_5)
                        focusAnimation += " B";
                    else if (_pd.equippedCharm_17)
                        focusAnimation += " S";
                    if (_pd.equippedCharm_7)
                        focusAnimation += " Quick";    
                }
                else
                    focusAnimation = "Focus";

                Log("Focus Animation: " + focusAnimation);
                _anim.Play(focusAnimation);

                GameObject dustLObj = focusEffects.FindGameObjectInChildren("Dust L");
                dustL = Instantiate(dustLObj, transform);
                dustL.SetActive(true);
                
                GameObject dustRObj = focusEffects.FindGameObjectInChildren("Dust R");
                dustR = Instantiate(dustRObj, transform);
                dustR.SetActive(true);

                GameObject linesAnimObj = focusEffects.FindGameObjectInChildren("Lines Anim");    
                linesAnim = Instantiate(linesAnimObj, transform);
                linesAnim.SetActive(true);
                linesAnim.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);
                
                //var focusStartClip = (AudioClip) spellControl.GetAction<AudioPlay>("Focus Start", 6).oneShotClip.Value;
                //_audio.PlayOneShot(focusStartClip);

                if (_pd.equippedCharm_5)
                {
                    GameObject baldurObj = _hero.transform.Find("Charm Effects").Find("Blocker Shield").gameObject;
                    baldurShell = Instantiate(baldurObj, transform);
                    baldurShell.SetActive(true);
                    baldurShell.layer = 11;
                    baldurShell.AddComponent<BaldurShell>();
                    baldurShell.SendMessage("BaldurClose");
                    _hm.IsInvincible = true;
                }
                
                float healTime = 0.85f;
                if (_pd.equippedCharm_7 && _pd.equippedCharm_34)
                    healTime = 0.95f;
                else if (_pd.equippedCharm_7)
                    healTime = 0.65f;
                else if (_pd.equippedCharm_34)
                    healTime = 1.4f;
                
                yield return new WaitForSeconds(healTime);
                
                StartCoroutine(FocusGet());
            }
            
            IEnumerator FocusGet()
            {
                string burstAnimation;
                if (_pd.equippedCharm_28)
                {
                    burstAnimation = "Slug Burst";
                    if (_pd.equippedCharm_5 && _pd.equippedCharm_17)
                        burstAnimation += " BS";
                    else if (_pd.equippedCharm_5)
                        burstAnimation += " B";
                    else if (_pd.equippedCharm_17)
                        burstAnimation += " S";
                }
                else
                {
                    burstAnimation = "Focus Get Once";
                }
                
                Log("Burst Animation: " + burstAnimation);

                _anim.Play(burstAnimation);
                _rb.velocity = Vector2.zero;
                
                Destroy(dustL);
                Destroy(dustR);
                Destroy(linesAnim);

                var healClip = (AudioClip) spellControl.GetAction<AudioPlayerOneShotSingle>("Focus Heal", 3).audioClip.Value;
                _audio.PlayOneShot(healClip);
                
                GameObject healAnimObj = focusEffects.FindGameObjectInChildren("Heal Anim");
                GameObject healAnim = Instantiate(healAnimObj, transform);
                healAnim.SetActive(true);
                
                GameObject chargeAudioObj = focusEffects.FindGameObjectInChildren("Charge Audio");
                GameObject chargeAudio = Instantiate(chargeAudioObj, transform);
                chargeAudio.SetActive(true);
                chargeAudio.GetComponent<AudioSource>().Play();

                _hm.hp += _pd.equippedCharm_34 ? 100 : 50;
                if (_hm.hp > _maxHealth) _hm.hp = _maxHealth;
                yield return new WaitForSeconds(_anim.PlayAnimGetTime(burstAnimation));

                StartCoroutine(FocusEnd());
            }

            IEnumerator FocusEnd()
            {
                if (_pd.equippedCharm_5)
                {
                    baldurShell.SendMessage("BaldurOpen");
                    _hm.IsInvincible = false;
                }
                _anim.Play(_pd.equippedCharm_28 ? "Slug Up" : "Focus End");

                yield return new WaitForSeconds(_anim.PlayAnimGetTime(_pd.equippedCharm_28 ? "Slug Up" : "Focus End"));

                if (_pd.equippedCharm_5) Destroy(baldurShell);
                
                StartCoroutine(IdleAndChooseNextAttack());
            }
            
            StartCoroutine(StartFocus());
        }

        private void BaldurShellBreak()
        {
            StopAllCoroutines();
            
            IEnumerator Break()
            {
                _anim.Play("Recoil");
                _hm.IsInvincible = false;
                _rb.velocity = Vector2.zero;

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Recoil"));
                
                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Break());
        }
    }
}