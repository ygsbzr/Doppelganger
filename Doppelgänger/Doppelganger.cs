using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
using Bounds = UnityEngine.Bounds;
using Random = UnityEngine.Random;
using Modding;
namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        internal static Doppelganger Instance { get; private set; }

        private tk2dSpriteAnimator _anim;
        private AudioSource _audio;
        private HeroController _hc;
        private GameObject _hero;
        private HeroAudioController _ac;
        private BoxCollider2D _collider;
        private HealthManager _hm;
        private AudioSource _music;
        private GameObject _musicObj;
        private GameObject _nailArts;
        private NonBouncer _nb;
        private PlayerData _pd;
        private Rigidbody2D _rb;
        private PlayMakerFSM _sc;
        private GameObject _spells;
        
        private bool _canShadowDash = true;
        private Color _furyColor = new Color32(255, 50, 113, 255); 
        private int _furyThreshold;
        private bool _furyActivated;
        private int _maxHealth;

        private void Awake()
        {
            _anim = GetComponent<tk2dSpriteAnimator>();
            _audio = GetComponent<AudioSource>();
            _collider = GetComponent<BoxCollider2D>();
            _hm = GetComponent<HealthManager>();
            _nb = GetComponent<NonBouncer>();
            _rb = GetComponent<Rigidbody2D>();
            
            _hc = HeroController.instance;
            _ac = ReflectionHelper.GetField<HeroController, HeroAudioController>(_hc, "audioCtrl");
            _hero = _hc.gameObject;
            _sc = _hc.spellControl;
            _pd = PlayerData.instance;
            
            _musicObj = new GameObject("Doppelganger Music");
            
            _maxHealth = 2000 + (_pd.maxHealth + _pd.healthBlue) * 200;
            _hm.hp = _maxHealth;
            
            #if DEBUG
            //_hm.hp = 1000;
            #endif
            
            _furyThreshold = _hm.hp / 10;
            
            _hm.OnDeath += OnDeath;
            HeroController.instance.OnDeath += OnHeroDeath;
            On.HealthManager.TakeDamage += OnTakeDamage;
            On.MusicCue.GetChannelInfo += MusicCueGetChannelInfo;
        }
        
        private IEnumerator Start()
        {
            while (HeroController.instance == null) yield return null;
                
            _nailArts = gameObject.FindGameObjectInChildren("Nail Arts");
            _spells = gameObject.FindGameObjectInChildren("Spells");
            
            _moves = new List<Action>
            {
                DoppelgangerDash,
                DoppelgangerFocus,
                DoppelgangerJump,
                DoppelgangerSlash,
                DoppelgangerNailArt,
                DoppelgangerSpell,
            };
            _repeats = new Dictionary<Action, int>
            {
                [DoppelgangerDash] = 0,
                [DoppelgangerFocus] = 0,
                [DoppelgangerJump] = 0,
                [DoppelgangerSlash] = 0,
                [DoppelgangerNailArt] = 0,
                [DoppelgangerSpell] = 0,
            };

            _maxRepeats = new Dictionary<Action, int>
            {
                [DoppelgangerDash] = 1,
                [DoppelgangerFocus] = 3,
                [DoppelgangerJump] = 3,
                [DoppelgangerSlash] = 3,
                [DoppelgangerNailArt] = 3,
                [DoppelgangerSpell] = 1,
            };

            if (_pd.equippedCharm_40 && _pd.grimmChildLevel < 5)
            {
                Log("Finding Grimmchild Clone");
                yield return new WaitWhile(() => !GameObject.Find("Grimmchild(Clone)"));
                
                /*Log("Creating Grimmchild Clone");
                GameObject gcObj = GameObject.Find("Grimmchild(Clone)");
                Log("Found Grimmchild");
                GameObject grimmChild = Instantiate(gcObj, transform.position, Quaternion.identity);
                grimmChild.SetActive(true);
                grimmChild.name = "Doppelgänger Grimmchild";
                grimmChild.layer = 11;
                grimmChild.AddComponent<Grimmchild>().Owner = gameObject;
                float posX = transform.position.x + 2.0f;
                float posY = transform.position.y - 0.25f;
                grimmChild.transform.SetPosition2D(posX, posY);*/

                GameObject gcClone = new GameObject(
                    "Doppelgänger Grimmchild",
                    typeof(AudioSource),
                    typeof(BoxCollider2D),
                    typeof(DamageHero),
                    typeof(EnemyHitEffectsUninfected),
                    typeof(MeshFilter),
                    typeof(MeshRenderer),
                    typeof(Rigidbody2D),
                    typeof(SpriteFlash),
                    typeof(tk2dSprite),
                    typeof(tk2dSpriteAnimator),
                    typeof(Grimmchild)
                )
                {
                    layer = 11,
                };

                GameObject gc = GameObject.Find("Grimmchild(Clone)");

                var collider = gcClone.GetComponent<BoxCollider2D>();
                var gcCollider = gc.GetComponent<BoxCollider2D>();
                
                collider.size = gcCollider.size;
                collider.offset = gcCollider.offset;
                collider.enabled = false;
                
                Bounds bounds = collider.bounds;
                Bounds gcBounds = gcCollider.bounds;
                bounds.min = gcBounds.min;
                bounds.max = gcBounds.max;
                
                var hitEffects = gcClone.GetComponent<EnemyHitEffectsUninfected>();

                gcClone.GetComponent<Grimmchild>().Owner = gameObject;
                
                var hm = gcClone.GetComponent<HealthManager>();
                
                var mFilter = gcClone.GetComponent<MeshFilter>();
            
                Mesh mesh = mFilter.mesh;
                Mesh gcMesh = gc.GetComponent<MeshFilter>().sharedMesh;

                mesh.vertices = gcMesh.vertices;
                mesh.normals = gcMesh.normals;
                mesh.uv = gcMesh.uv;
                mesh.triangles = gcMesh.triangles;
                mesh.tangents = gcMesh.tangents;

                var mRend = gcClone.GetComponent<MeshRenderer>();
                mRend.material = new Material(gc.GetComponent<MeshRenderer>().material);

                var rb = gcClone.GetComponent<Rigidbody2D>();
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.gravityScale = 0.0f;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                rb.isKinematic = false;

                var anim = gcClone.GetComponent<tk2dSpriteAnimator>();
                anim.Library = gc.GetComponent<tk2dSpriteAnimator>().Library;

                float posX = transform.position.x + 2.0f;
                float posY = transform.position.y - 0.25f;
                gcClone.transform.SetPosition2D(posX, posY);
            }

            if (_pd.equippedCharm_38)
            {
                GameObject markoth = Doppelgänger.PreloadedGameObjects["Markoth"];
                PlayMakerFSM shieldAttack = markoth.LocateMyFSM("Shield Attack");
                GameObject dreamShieldObj = shieldAttack.GetAction<CreateObject>("Init", 1).gameObject.Value;
                GameObject dreamShield = Instantiate(dreamShieldObj, transform);
                dreamShield.transform.localScale = _pd.equippedCharm_30 ? new Vector3(0.85f, 0.85f, 0.85f) : new Vector3(0.75f, 0.75f, 0.75f); 
                dreamShield.SetActive(true);
            }
            
            if (_pd.equippedCharm_39)
            {
                yield return new WaitWhile(() => !GameObject.Find("Weaverling(Clone)"));
                var weaverlingObj = GameObject.Find("Weaverling(Clone)");
                GameObject weaverling = Instantiate(weaverlingObj, transform);
                weaverling.SetActive(true);
            }

            StartCoroutine(DoppelgangerIntro());
        }

        private void OnDeath()
        {
            StopAllCoroutines();
            
            IEnumerator DeathAnimation()
            {
                _collider.enabled = false;
                _music.Stop();
                _rb.gravityScale = 0.0f;
                _rb.velocity = Vector2.zero;
                _anim.Play("Death");
                
                GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");

                GameObject heroDeathGO = _hero.FindGameObjectInChildren("Hero Death");
                PlayMakerFSM heroDeathAnim = heroDeathGO.LocateMyFSM("Hero Death Anim");

                var heroDeathClip = (AudioClip) heroDeathAnim.GetAction<AudioPlayerOneShotSingle>("Start", 4).audioClip.Value;
                var heroDeath = new GameObject("Hero Death", typeof(AudioSource));
                var heroDeathAudio = heroDeath.GetComponent<AudioSource>();
                heroDeathAudio.PlayOneShot(heroDeathClip);
                
                var heroDmgClip = (AudioClip) heroDeathAnim.GetAction<AudioPlayerOneShotSingle>("Start", 5).audioClip.Value;
                var heroDamage = new GameObject("Hero Damage", typeof(AudioSource));
                var heroDamageAudio = heroDamage.GetComponent<AudioSource>();
                heroDamageAudio.PlayOneShot(heroDmgClip);

                var heroDeathExtraDetailsClip = (AudioClip) heroDeathAnim.GetAction<AudioPlayerOneShotSingle>("Initiate", 2).audioClip.Value;
                var heroDeathExtraDetails = new GameObject("Hero Death Extra Details", typeof(AudioSource));
                var heroDeathExtraDetailsAudio = heroDeathExtraDetails.GetComponent<AudioSource>();
                heroDeathExtraDetailsAudio.PlayOneShot(heroDeathExtraDetailsClip);
                
                foreach (GameObject go in FindObjectsOfType<GameObject>()
                    .Where(go => (go.layer == 11 || go.layer == 22) && go.name != "Doppelgänger"))
                {
                    Destroy(go);
                }

                GameObject particleWaveObj = heroDeathGO.FindGameObjectInChildren("Particle Wave");
                GameObject particleWave = Instantiate(particleWaveObj, transform);
                ParticleSystem particleWavePS = particleWave.GetComponent<ParticleSystem>();
                particleWavePS.Play();
                
                GameObject shadeParticlesObj = heroDeathGO.FindGameObjectInChildren("Shade Particles");
                GameObject shadeParticles = Instantiate(shadeParticlesObj, transform);
                ParticleSystem shadeParticlesPS = shadeParticles.GetComponent<ParticleSystem>();
                shadeParticlesPS.Play();

                GameObject permaDeathLoopingFXObj = heroDeathGO.FindGameObjectInChildren("perma_death_looping_effects");
                GameObject permaDeathLoopingFX = Instantiate(permaDeathLoopingFXObj, transform);
                permaDeathLoopingFX.SetActive(true);
                GameObject blackParticleBurstObj = permaDeathLoopingFX.FindGameObjectInChildren("black particle burst");
                GameObject blackParticleBurst = Instantiate(blackParticleBurstObj, transform);
                var blackParticleBurstPS = blackParticleBurst.GetComponent<ParticleSystem>();
                blackParticleBurstPS.Play();

                yield return new WaitForSeconds(1.0f / 12);

                yield return new WaitForSeconds(_anim.PlayAnimGetTime("Death") - 1.0f / 12);
                
                StartCoroutine(CreateDeathObject());
            }

            IEnumerator CreateDeathObject()
            {
                yield return null;

                GameObject doppelgangerDeath = new GameObject(
                    "Doppelganger Death",
                    typeof(AudioSource),
                    typeof(DoppelgangerDeath));

                doppelgangerDeath.transform.SetPosition2D(transform.position.x, transform.position.y);
                
                Destroy(gameObject);
            }

            StartCoroutine(DeathAnimation());
        }

        private void OnHeroDeath()
        {
            _music.Stop();
        }
        
        private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if (self.gameObject.name == "Doppelgänger")
            {
                GetComponent<EnemyHitEffectsUninfected>().RecieveHitEffect(hitInstance.Direction);
                GetComponent<SpriteFlash>().flashFocusHeal();
            }

            orig(self, hitInstance);
            
            _furyActivated = _pd.equippedCharm_6 && _hm.hp < _furyThreshold;
        }

        private MusicCue.MusicChannelInfo MusicCueGetChannelInfo(On.MusicCue.orig_GetChannelInfo orig, MusicCue self, MusicChannels channels)
        {
            if (_musicObj.GetComponent<AudioController>() == null)
            {
                _musicObj.SetActive(true);
                _musicObj.transform.position = new Vector2(75f, 15f);
                _music = _musicObj.AddComponent<AudioSource>();
               _music.clip = Doppelgänger.Audio["DoppelgangerMusic"];
               _music.loop = true;
               _music.bypassReverbZones = _music.bypassEffects = true;
               _music.volume = 0f;
               _musicObj.AddComponent<AudioController>();
            }
            return orig(self, channels);
        }

        private Action _previousMove;
        private Action _nextMove;
        private List<Action> _moves;
        private Dictionary<Action, int> _repeats;
        private Dictionary<Action, int> _maxRepeats;
        private IEnumerator IdleAndChooseNextAttack()
        {
            _anim.Play("Idle");
            _rb.velocity = Vector2.zero;

            //if (_previousMove != DryyaTurn && _previousMove != DryyaWalk && _previousMove != DryyaEvade)
            {
                float waitMin = 0.0f;
                float waitMax = 0.1f;
                float waitTime = Random.Range(waitMin, waitMax);
            
                yield return new WaitForSeconds(waitTime); 
            }

            yield return null;

            if (_nextMove != null) _previousMove = _nextMove;
            int index = Random.Range(0, _moves.Count);
            _nextMove = _moves[index];
            
            // Re-select move based on specified conditions
            while (_repeats[_nextMove] >= _maxRepeats[_nextMove] ||
                   _nextMove == DoppelgangerDash && !_canShadowDash || 
                   _nextMove == DoppelgangerFocus && _hm.hp >= _maxHealth)
            {
                index = Random.Range(0, _moves.Count);
                _nextMove = _moves[index];
            }

            Vector2 pos = transform.position;
            Vector2 heroPos = HeroController.instance.transform.position;

            // Run if Knight is out of range
            float runThreshold = _pd.equippedCharm_35 ? 10.0f : 5.0f;
            if (Mathf.Abs(heroPos.x - pos.x) > runThreshold && 
                _nextMove != DoppelgangerJump && 
                _nextMove != DoppelgangerSpell && 
                _nextMove != DoppelgangerDash && 
                _nextMove != DoppelgangerFocus)
            {
                _nextMove = DoppelgangerRun;
            }

            if (_nextMove == DoppelgangerSlash && Mathf.Abs(heroPos.y - pos.y) > Mathf.Abs(heroPos.x - pos.x))
            {
                if (heroPos.y - pos.y > 0)
                    _nextMove = DoppelgangerUpSlash;
                else
                    _nextMove = DoppelgangerDownSlash;
            }
            
            if (_nextMove == DoppelgangerSlash && _previousMove == DoppelgangerSlash)
                _nextMove = DoppelgangerAltSlash;
            else if (_nextMove == DoppelgangerAltSlash && _previousMove == DoppelgangerAltSlash)
                _nextMove = DoppelgangerSlash;
            
            // Turn if facing opposite of direction to Knight
            if (heroPos.x - pos.x < 0 && transform.localScale.x == -1 || heroPos.x - pos.x > 0 && transform.localScale.x == 1)
            {
                _nextMove = DoppelgangerTurn;
            }

            // Increment or reset move repeats dictionary
            if (_moves.Contains(_nextMove))
            {
                foreach (Action move in _moves)
                {
                    if (move == _nextMove)
                    {
                        _repeats[move]++;
                    }        
                    else
                    {
                        _repeats[move] = 0;
                    }
                }
            }
            
            
            Log("Next Move: " + _nextMove.Method.Name);
            _nextMove.Invoke();
        }

        private IEnumerator DashTimer()
        {
            yield return new WaitForSeconds(_hc.SHADOW_DASH_COOLDOWN);
            _canShadowDash = true;
        }

        private void DoppelgangerTurn()
        {
            IEnumerator Turn()
            {
                _anim.Play("TurnToIdle");

                // TurnToIdle is weird and has idle frames for most of the animation, so manually set time here.
                yield return new WaitForSeconds(2 * (1.0f / _anim.ClipFps));
                Flip();

                StartCoroutine(IdleAndChooseNextAttack());
            }

            StartCoroutine(Turn());
        }

        private void Flip()
        {
            Vector2 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        
        private bool TouchingGround()
        {
            Bounds bounds = _collider.bounds;
            Vector3 min = new Vector2(bounds.min.x, bounds.center.y);
            Vector3 center = bounds.center;
            Vector3 max = new Vector2(bounds.max.x, bounds.center.y);

            float distance = bounds.extents.y + 0.16f;

            RaycastHit2D minRay = Physics2D.Raycast(min, Vector2.down, distance, 256);
            RaycastHit2D centerRay = Physics2D.Raycast(center, Vector2.down, distance, 256);
            RaycastHit2D maxRay = Physics2D.Raycast(max, Vector2.down, distance, 256);

            return minRay.collider != null || centerRay.collider != null || maxRay.collider != null;
        }
        
        private void OnDestroy()
        {
            _ac.StopAllSounds();
            _hm.OnDeath -= OnDeath;
            HeroController.instance.OnDeath -= OnHeroDeath;
            On.HealthManager.TakeDamage -= OnTakeDamage;
            On.MusicCue.GetChannelInfo -= MusicCueGetChannelInfo;
        }
        
        private void Log(object message) => Modding.Logger.Log("[Doppelganger] " + message);
    }
}