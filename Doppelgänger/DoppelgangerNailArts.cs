using System.Collections;
using GlobalEnums;
using UnityEngine;
using Satchel;
namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerNailArt()
        {
            IEnumerator NailArtCharge()
            {
                _ac.PlaySound(HeroSounds.NAIL_ART_CHARGE);
                GameObject naChargeEffect = Instantiate(_hc.artChargeEffect, transform);
                naChargeEffect.SetActive(true);
                naChargeEffect.layer = 22;
                naChargeEffect.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);

                yield return new WaitForSeconds(_pd.equippedCharm_26
                    ? _hc.NAIL_CHARGE_TIME_CHARM
                    : _hc.NAIL_CHARGE_TIME_DEFAULT);

                Destroy(naChargeEffect);
                
                _ac.StopSound(HeroSounds.NAIL_ART_CHARGE);
                _ac.PlaySound(HeroSounds.NAIL_ART_READY);
                GameObject chargedEffect = Instantiate(_hc.artChargedEffect, transform);
                chargedEffect.SetActive(true);
                chargedEffect.layer = 22;
                chargedEffect.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(0);

                yield return new WaitForSeconds(0.1f);
            
                _ac.StopSound(HeroSounds.NAIL_ART_READY);
                Destroy(chargedEffect);

                float heroX = _hero.transform.position.x;
                float posX = transform.position.x;
                if (heroX - posX > 0 && transform.localScale.x > 0 || heroX - posX < 0 && transform.localScale.x < 0)
                    StartCoroutine(Turn());
                else
                    ChooseNailArt();
            }

            void ChooseNailArt()
            {
                Vector2 heroPos = _hero.transform.position;
                Vector2 pos = transform.position;
                float dsThreshold = 5.0f;
                   
                if (Mathf.Abs(heroPos.y - pos.y) > Mathf.Abs(heroPos.x - pos.x))
                    DoppelgangerGreatSlash();
                else
                {
                    if (Mathf.Abs(heroPos.x - pos.x) > dsThreshold)
                        DoppelgangerDashSlash();
                    else
                        DoppelgangerCycloneSlash();
                }
            }

            IEnumerator Turn()
            {
                _anim.Play("TurnToIdle");

                // TurnToIdle is weird and has idle frames for most of the animation, so manually set time here.
                yield return new WaitForSeconds(2 * (1.0f / _anim.ClipFps));
                Flip();

                ChooseNailArt();
            }
            
            StartCoroutine(NailArtCharge());
        }
        
        private void DoppelgangerCycloneSlash()
        {
            GameObject cycloneSlash;
            GameObject hitLDamager;
            GameObject hitRDamager;
            IEnumerator CycloneStart()
            {
                _anim.Play("NA Cyclone Start");
                
                GameObject cycloneObj = _hc.transform.Find("Attacks").Find("Cyclone Slash").gameObject;
                cycloneSlash = Instantiate(cycloneObj, _nailArts.transform);
                cycloneSlash.SetActive(true);
                cycloneSlash.layer = 22;
                cycloneSlash.GetComponent<tk2dSprite>().color = _furyActivated ? _furyColor : Color.white;
                cycloneSlash.LocateMyFSM("Control Collider").SetState("Init");
                GameObject hitL = cycloneSlash.FindGameObjectInChildren("Hit L");
                GameObject hitR = cycloneSlash.FindGameObjectInChildren("Hit R");

                hitLDamager = Instantiate(new GameObject("Hit L"), hitL.transform);
                hitLDamager.layer = 11;
                hitLDamager.AddComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var hitLDmgPoly = hitLDamager.AddComponent<PolygonCollider2D>();
                hitLDmgPoly.isTrigger = true;
                var hitLPoly = hitL.GetComponent<PolygonCollider2D>();
                hitLDmgPoly.points = hitLPoly.points;
                hitLDamager.AddComponent<TinkEffect>();
                hitLDamager.AddComponent<TinkSound>();
                
                hitRDamager = Instantiate(new GameObject("Hit R"), hitR.transform);
                hitRDamager.layer = 11;
                hitRDamager.AddComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var hitRDmgPoly = hitRDamager.AddComponent<PolygonCollider2D>();
                hitRDmgPoly.isTrigger = true;
                var hitRPoly = hitR.GetComponent<PolygonCollider2D>();
                hitRDmgPoly.points = hitRPoly.points;
                hitRDamager.AddComponent<TinkEffect>();
                hitRDamager.AddComponent<TinkSound>();

                Destroy(hitLPoly);
                Destroy(hitRPoly);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("NA Cyclone Start"));

                StartCoroutine(CycloneSlash());
            }

            IEnumerator CycloneSlash()
            {
                _anim.Play("NA Cyclone");
                
                yield return new WaitForSeconds(1.0f);

                StartCoroutine(CycloneEnd());
            }

            IEnumerator CycloneEnd()
            {
                _anim.Play("NA Cyclone End");

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("NA Cyclone End"));

                Destroy(cycloneSlash);
                Destroy(hitLDamager);
                Destroy(hitRDamager);

                StartCoroutine(IdleAndChooseNextAttack());
            }
            
            StartCoroutine(CycloneStart());
        }

        private void DoppelgangerDashSlash()
        {
            IEnumerator Dash()
            {
                _anim.Play("Dash");
                _ac.PlaySound(HeroSounds.DASH);
                _rb.velocity = Vector2.right * _hc.DASH_SPEED * -transform.localScale.x;
                
                yield return new WaitForSeconds(_hc.DASH_TIME);
                
                StartCoroutine(DashSlash());
            }

            IEnumerator DashSlash()
            {
                _anim.Play("NA Dash Slash");
                _ac.StopSound(HeroSounds.DASH);
                _rb.velocity = Vector2.zero;

                GameObject dsObj = _hc.transform.Find("Attacks").Find("Dash Slash").gameObject;
                var dashSlash = Instantiate(dsObj, _nailArts.transform);
                dashSlash.SetActive(true);
                dashSlash.layer = 22;
                dashSlash.GetComponent<tk2dSprite>().color = _furyActivated ? _furyColor : Color.white;
                dashSlash.LocateMyFSM("Control Collider").SetState("Init");

                GameObject dsCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], dashSlash.transform);
                dsCollider.SetActive(true);
                dsCollider.layer = 22;
                Vector2[] points = dashSlash.GetComponent<PolygonCollider2D>().points;
                dsCollider.GetComponent<PolygonCollider2D>().points = points;
                dsCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 3 : 2;
                var anim = dashSlash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(dsCollider, lifetime);
                
                yield return new WaitForSeconds(_anim.PlayAnimGetTime("NA Dash Slash"));

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Dash());
        }
        
        private void DoppelgangerGreatSlash()
        {
            IEnumerator GreatSlash()
            {
                _anim.Play("NA Big Slash");
                GameObject gsObj = _hc.transform.Find("Attacks").Find("Great Slash").gameObject;
                GameObject ds = Instantiate(_hc.transform.Find("Attacks").Find("Dash Slash").gameObject, transform.position, Quaternion.identity);
                ds.SetActive(true);
                var greatSlash = Instantiate(gsObj, _nailArts.transform);//.position, Quaternion.identity);
                greatSlash.SetActive(true);
                greatSlash.layer = 22;
                greatSlash.GetComponent<tk2dSprite>().color = _furyActivated ? _furyColor : Color.white;
                greatSlash.LocateMyFSM("Control Collider").SetState("Init");

                GameObject gsCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], greatSlash.transform);
                gsCollider.SetActive(true);
                gsCollider.layer = 22;
                Vector2[] points = greatSlash.GetComponent<PolygonCollider2D>().points;
                gsCollider.GetComponent<PolygonCollider2D>().points = points;
                gsCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 3 : 2;
                var anim = greatSlash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(gsCollider, lifetime);

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("NA Big Slash"));

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(GreatSlash());
        }
    }
}