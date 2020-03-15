using System.Collections;
using UnityEngine;

namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerRun()
        {
            bool isDashing = false;
            
            IEnumerator Run()
            {
                _anim.Play(_pd.equippedCharm_37 ? "Sprint" : "Run");
                float runSpeed;
                if (_pd.equippedCharm_37)
                {
                    if (_pd.equippedCharm_31)
                        runSpeed = _hc.RUN_SPEED_CH_COMBO;
                    else
                        runSpeed = _hc.RUN_SPEED_CH;
                }
                else
                    runSpeed = _hc.RUN_SPEED;

                _rb.velocity = new Vector2(runSpeed * -transform.localScale.x, 0);

                float stopThreshold = 2.0f;
                
                yield return new WaitWhile(() =>
                {
                    if (_canShadowDash) StartCoroutine(Dash());

                    float heroX = _hero.transform.position.x;
                    float posX = transform.position.x;
                    float diff = Mathf.Abs(heroX - posX);
                    return diff > stopThreshold;
                });

                while (isDashing) yield return null;

                StartCoroutine(IdleAndChooseNextAttack());
            }

            GameObject shadowDashParticles;
            IEnumerator Dash()
            {
                string animation = PlayerData.instance.equippedCharm_16 ? "Shadow Dash Sharp" : "Shadow Dash";
                
                _anim.Play(animation);
                _audio.PlayOneShot(_pd.equippedCharm_16 ? _hc.sharpShadowClip : _hc.shadowDashClip);
                _hm.enabled = false;
                _nb.active = true;

                _canShadowDash = false;
                isDashing = true;
                
                float xVel = (_pd.equippedCharm_16 ? _hc.DASH_SPEED_SHARP : _hc.DASH_SPEED) * -transform.localScale.x;
                _rb.velocity = new Vector2(xVel, 0);
                Instantiate(_hc.shadowdashBurstPrefab, transform);
                shadowDashParticles = Instantiate(_hc.shadowdashParticlesPrefab, transform);
                shadowDashParticles.SetActive(true);
                var ps = shadowDashParticles.GetComponent<ParticleSystem>();
                ps.Play();

                _collider.enabled = false;
                _hm.enabled = true;
                _nb.active = false;
                _rb.gravityScale = 0.0f;
                
                yield return new WaitForSeconds(HeroController.instance.SHADOW_DASH_TIME);

                StartCoroutine(DashEnd());
            }
            
            IEnumerator DashEnd()
            {
                _anim.Play("Dash To Idle");
                _collider.enabled = true;
                _rb.gravityScale = 1.0f;
                _rb.velocity = Vector2.zero;
                
                StartCoroutine(DashTimer());

                Destroy(shadowDashParticles);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Dash To Idle"));

                isDashing = false;
                
                _anim.Play(_pd.equippedCharm_37 ? "Sprint" : "Run");
                float runSpeed;
                if (_pd.equippedCharm_37)
                {
                    if (_pd.equippedCharm_31)
                        runSpeed = _hc.RUN_SPEED_CH_COMBO;
                    else
                        runSpeed = _hc.RUN_SPEED_CH;
                }
                else
                    runSpeed = _hc.RUN_SPEED;

                _rb.velocity = new Vector2(runSpeed * -transform.localScale.x, 0);
            }

            StartCoroutine(Run());
        }
    }
}