using System.Linq;
using System.Reflection;
using Satchel;
using UnityEngine;

namespace Doppelgänger
{
    public class BattleControl : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(GameObject.Find("Battle Control"));
            foreach (GameObject go in FindObjectsOfType<GameObject>().Where(go => go.layer == 11)) Destroy(go);
            CreateDoppelganger();
        }

        private void CreateDoppelganger()
        {
            GameObject doppelganger = new GameObject(
                "Doppelgänger",
                typeof(AudioSource),
                typeof(BoxCollider2D),
                typeof(DamageHero),
                typeof(EnemyDreamnailReaction),
                typeof(EnemyHitEffectsUninfected),
                typeof(ExtraDamageable),
                typeof(HealthManager),
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(NonBouncer),
                typeof(Rigidbody2D),
                typeof(SpriteFlash),
                typeof(tk2dSprite),
                typeof(tk2dSpriteAnimator),
                typeof(Doppelganger)
            )
            {
                layer = 11,
            };

            GameObject hero = HeroController.instance.gameObject;
            
            Log("Assigning Fields");
            var collider = doppelganger.GetComponent<BoxCollider2D>();
            var heroCollider = hero.GetComponent<BoxCollider2D>();

            GameObject spells = new GameObject("Spells") {layer = 11};
            spells.transform.SetParent(doppelganger.transform);
            
            GameObject nailArts = new GameObject("Nail Arts") {layer = 11};
            nailArts.transform.SetParent(doppelganger.transform);
            
            collider.size = heroCollider.size;
            collider.offset = heroCollider.offset;
            collider.enabled = false;

            Bounds bounds = collider.bounds;
            Bounds heroBounds = heroCollider.bounds;
            bounds.min = heroBounds.min;
            bounds.max = heroBounds.max;

            var hitEffects = doppelganger.GetComponent<EnemyHitEffectsUninfected>();
            
            var enemyDnReaction = doppelganger.GetComponent<EnemyDreamnailReaction>();
            enemyDnReaction.SetConvoTitle("KNIGHT_DREAM");

            var hm = doppelganger.GetComponent<HealthManager>();

            var mFilter = doppelganger.GetComponent<MeshFilter>();
            
            Mesh mesh = mFilter.mesh;
            Mesh heroMesh = hero.GetComponent<MeshFilter>().sharedMesh;

            mesh.vertices = heroMesh.vertices;
            mesh.normals = heroMesh.normals;
            mesh.uv = heroMesh.uv;
            mesh.triangles = heroMesh.triangles;
            mesh.tangents = heroMesh.tangents;

            var mRend = doppelganger.GetComponent<MeshRenderer>();
            mRend.material = new Material(hero.GetComponent<MeshRenderer>().material);

            var nb = doppelganger.GetComponent<NonBouncer>();
            nb.active = false;

            var rb = doppelganger.GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0.0f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.isKinematic = false;

            var anim = doppelganger.GetComponent<tk2dSpriteAnimator>();
            anim.Library = hero.GetComponent<tk2dSpriteAnimator>().Library;

            GameObject zoteTurret = GameObject.Find("Zote Turret");
            
            var zoteHitEffects = zoteTurret.GetComponent<EnemyHitEffectsUninfected>();
            foreach (FieldInfo fi in typeof(EnemyHitEffectsUninfected).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                fi.SetValue(hitEffects, fi.GetValue(zoteHitEffects));
            }
            
            var zoteHealth = zoteTurret.GetComponent<HealthManager>();
            foreach (FieldInfo fi in typeof(HealthManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.Name.Contains("Prefab")))
            {
                fi.SetValue(hm, fi.GetValue(zoteHealth));
            }

            doppelganger.transform.SetPosition2D(107.4f, 6.4f);

            
            Log("Finished Creating Doppelganger");
        }

        private void Log(object message) => Modding.Logger.Log("[Battle Control] " + message);
    }
}