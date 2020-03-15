using System.Collections;
using UnityEngine;

namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private void DoppelgangerSlash()
        {
            IEnumerator Slash()
            {
                _anim.Play("Slash");
                
                GameObject slash = Instantiate(_hc.slashPrefab, transform);
                slash.SetActive(true);
                slash.layer = 22;
                var nailSlash = slash.GetComponent<NailSlash>();
                nailSlash.SetFury(_furyActivated);
                nailSlash.SetMantis(_pd.equippedCharm_13);
                if (_pd.equippedCharm_13 && _pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f, nailSlash.scale.z);
                else if (_pd.equippedCharm_13)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f, nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                else if (_pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f, nailSlash.scale.y * 1.15f, nailSlash.scale.z);
                nailSlash.StartSlash();

                if (_pd.equippedCharm_35)
                {
                    GameObject elegyBeam = transform.localScale.x > 0
                        ? _furyActivated ? _hc.grubberFlyBeamPrefabL_fury : _hc.grubberFlyBeamPrefabL
                        : _furyActivated ? _hc.grubberFlyBeamPrefabR_fury : _hc.grubberFlyBeamPrefabR;
                    GameObject beam = Instantiate(elegyBeam, transform.position, Quaternion.identity);
                    beam.SetActive(true);
                    beam.layer = 22;
                    Destroy(beam.LocateMyFSM("damages_enemy"));
                    beam.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_25 && _pd.equippedCharm_6 && _furyActivated ? 2 : 1;
                }
                
                GameObject slashCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], slash.transform);
                slashCollider.SetActive(true);
                slashCollider.layer = 22;
                Vector2[] points = slash.GetComponent<PolygonCollider2D>().points;
                slashCollider.GetComponent<PolygonCollider2D>().points = points;
                slashCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var anim = slash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(slashCollider, lifetime);

                yield return new WaitForSeconds(_pd.equippedCharm_32 ? _hc.ATTACK_DURATION_CH : _hc.ATTACK_DURATION);

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Slash());
        }

        private void DoppelgangerAltSlash()
        {
            IEnumerator AltSlash()
            {
                _anim.Play("SlashAlt");
                
                GameObject altSlash = Instantiate(_hc.slashAltPrefab, transform);
                altSlash.SetActive(true);
                altSlash.layer = 22;
                var nailSlash = altSlash.GetComponent<NailSlash>();
                nailSlash.SetFury(_furyActivated);
                nailSlash.SetMantis(_pd.equippedCharm_13);
                if (_pd.equippedCharm_13 && _pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f, nailSlash.scale.z);
                else if (_pd.equippedCharm_13)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f, nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                else if (_pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f, nailSlash.scale.y * 1.15f, nailSlash.scale.z);

                nailSlash.StartSlash();
                
                if (_pd.equippedCharm_35)
                {
                    GameObject elegyBeam = transform.localScale.x > 0
                        ? _furyActivated ? _hc.grubberFlyBeamPrefabL_fury : _hc.grubberFlyBeamPrefabL
                        : _furyActivated ? _hc.grubberFlyBeamPrefabR_fury : _hc.grubberFlyBeamPrefabR;
                    GameObject beam = Instantiate(elegyBeam, transform.position, Quaternion.identity);
                    beam.SetActive(true);
                    beam.layer = 22;
                    Destroy(beam.LocateMyFSM("damages_enemy"));
                    beam.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_25 && _pd.equippedCharm_6 && _furyActivated ? 2 : 1;
                }
                
                GameObject altSlashCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], altSlash.transform);
                altSlashCollider.SetActive(true);
                altSlashCollider.layer = 22;
                Vector2[] points = altSlash.GetComponent<PolygonCollider2D>().points;
                altSlashCollider.GetComponent<PolygonCollider2D>().points = points;
                altSlashCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var anim = altSlash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(altSlashCollider, lifetime);

                yield return new WaitForSeconds(_pd.equippedCharm_32 ? _hc.ATTACK_DURATION_CH : _hc.ATTACK_DURATION);

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(AltSlash());
        }

        private void DoppelgangerUpSlash()
        {
            IEnumerator UpSlash()
            {
                _anim.Play("UpSlash");
                
                GameObject upSlash = Instantiate(_hc.upSlashPrefab, transform);
                upSlash.SetActive(true);
                upSlash.layer = 22;
                var nailSlash = upSlash.GetComponent<NailSlash>();
                nailSlash.SetFury(_furyActivated);
                nailSlash.SetMantis(_pd.equippedCharm_13);
                if (_pd.equippedCharm_13 && _pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f, nailSlash.scale.z);
                else if (_pd.equippedCharm_13)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f, nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                else if (_pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f, nailSlash.scale.y * 1.15f, nailSlash.scale.z);
                nailSlash.StartSlash();
                
                if (_pd.equippedCharm_35)
                {
                    GameObject elegyBeam = _furyActivated ? _hc.grubberFlyBeamPrefabU_fury : _hc.grubberFlyBeamPrefabU;
                    GameObject beam = Instantiate(elegyBeam, transform.position, Quaternion.Euler(0, 0, -90));
                    beam.SetActive(true);
                    beam.layer = 22;
                    Destroy(beam.LocateMyFSM("damages_enemy"));
                    beam.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_25 && _pd.equippedCharm_6 && _furyActivated ? 2 : 1;
                }
                
                GameObject upSlashCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], upSlash.transform);
                upSlashCollider.SetActive(true);
                upSlashCollider.layer = 22;
                Vector2[] points = upSlash.GetComponent<PolygonCollider2D>().points;
                upSlashCollider.GetComponent<PolygonCollider2D>().points = points;
                upSlashCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var anim = upSlash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(upSlashCollider, lifetime);

                yield return new WaitForSeconds(_pd.equippedCharm_32 ? _hc.ATTACK_DURATION_CH : _hc.ATTACK_DURATION);

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(UpSlash());
        }

        private void DoppelgangerDownSlash()
        {
            IEnumerator DownSlash()
            {
                _anim.Play("DownSlash");
                
                GameObject downSlash = Instantiate(_hc.downSlashPrefab, transform);
                downSlash.SetActive(true);
                downSlash.layer = 22;
                var nailSlash = downSlash.GetComponent<NailSlash>();
                nailSlash.SetFury(_furyActivated);
                nailSlash.SetMantis(_pd.equippedCharm_13);
                if (_pd.equippedCharm_13 && _pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.4f, nailSlash.scale.y * 1.4f, nailSlash.scale.z);
                else if (_pd.equippedCharm_13)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.25f, nailSlash.scale.y * 1.25f, nailSlash.scale.z);
                else if (_pd.equippedCharm_18)
                    nailSlash.transform.localScale = new Vector3(nailSlash.scale.x * 1.15f, nailSlash.scale.y * 1.15f, nailSlash.scale.z);
                nailSlash.StartSlash();

                if (_pd.equippedCharm_35)
                {
                    GameObject elegyBeam = _furyActivated ? _hc.grubberFlyBeamPrefabD_fury : _hc.grubberFlyBeamPrefabD;
                    GameObject beam = Instantiate(elegyBeam, transform.position, Quaternion.Euler(0, 0, 90));
                    beam.SetActive(true);
                    beam.layer = 22;
                    Destroy(beam.LocateMyFSM("damages_enemy"));
                    beam.AddComponent<DamageHero>().damageDealt = _pd.equippedCharm_25 && _pd.equippedCharm_6 && _furyActivated ? 2 : 1;
                }
                
                GameObject downSlashCollider = Instantiate(Doppelgänger.PreloadedGameObjects["Slash"], downSlash.transform);
                downSlashCollider.SetActive(true);
                downSlashCollider.layer = 22;
                Vector2[] points = downSlash.GetComponent<PolygonCollider2D>().points;
                downSlashCollider.GetComponent<PolygonCollider2D>().points = points;
                downSlashCollider.GetComponent<DamageHero>().damageDealt = _furyActivated ? 2 : 1;
                var anim = downSlash.GetComponent<tk2dSpriteAnimator>();
                float lifetime = anim.DefaultClip.frames.Length / anim.ClipFps;
                Destroy(downSlashCollider, lifetime);
                
                yield return new WaitForSeconds(_pd.equippedCharm_32 ? _hc.ATTACK_DURATION_CH : _hc.ATTACK_DURATION);

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(DownSlash());
        }
    }
}