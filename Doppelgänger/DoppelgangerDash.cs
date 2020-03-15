using System.Collections;
using UnityEngine;

namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerDash()
        {
            GameObject shadowDashParticles;
            
            IEnumerator Dash()
            {
                string animation = PlayerData.instance.equippedCharm_16 ? "Shadow Dash Sharp" : "Shadow Dash";
                
                _anim.Play(animation);
                _audio.PlayOneShot(_pd.equippedCharm_16 ? _hc.sharpShadowClip : _hc.shadowDashClip);

                float xVel = (_pd.equippedCharm_16 ? _hc.DASH_SPEED_SHARP : _hc.DASH_SPEED) * -transform.localScale.x;
                _rb.velocity = new Vector2(xVel, 0);
                Instantiate(_hc.shadowdashBurstPrefab, transform);
                shadowDashParticles = Instantiate(_hc.shadowdashParticlesPrefab);
                shadowDashParticles.SetActive(true);
                var ps = shadowDashParticles.GetComponent<ParticleSystem>();
                ps.Play();

                _canShadowDash = false;
                _hm.enabled = false;
                _nb.enabled = true;
                _collider.enabled = _pd.equippedCharm_16;
                _rb.gravityScale = 0.0f;
                
                yield return new WaitForSeconds(HeroController.instance.SHADOW_DASH_TIME);

                StartCoroutine(DashEnd());
            }
            
            IEnumerator DashEnd()
            {
                _anim.Play("Dash To Idle");
                _hm.enabled = true;
                _nb.enabled = false;
                _collider.enabled = true;
                _rb.gravityScale = 1.0f;
                _rb.velocity = Vector2.zero;
                
                StartCoroutine(DashTimer());

                Destroy(shadowDashParticles);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Dash To Idle"));
                    
                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Dash());
        }
    }
}