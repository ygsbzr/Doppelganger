using UnityEngine;

namespace Doppelgänger
{
    public class Fireball : MonoBehaviour
    {
        public float xDir;
        
        private const float FireballSpeed = 45;
        
        private tk2dSpriteAnimator _anim;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _anim = GetComponent<tk2dSpriteAnimator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _anim.PlayFromFrame(0);
            _rb.velocity = Vector2.right * FireballSpeed * xDir;
            var dmg = gameObject.AddComponent<DamageHero>();
            if (PlayerData.instance.equippedCharm_19)
            {
                dmg.damageDealt = 2;
                transform.localScale = new Vector3(xDir * 1.8f * 1.3f, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                dmg.damageDealt = 1;
                transform.localScale = new Vector3(xDir * 1.8f, transform.localScale.y, transform.localScale.z);
            }
            Destroy(gameObject, 2);
        }
        
        private void Log(object message) => Modding.Logger.Log("[Fireball] " + message);
    }
}