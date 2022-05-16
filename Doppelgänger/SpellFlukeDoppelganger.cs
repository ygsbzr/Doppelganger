using System.Collections;
using GlobalEnums;
using UnityEngine;
using Random = UnityEngine.Random;
using Satchel;
namespace Doppelgänger
{
    public class SpellFlukeDoppelganger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.gameObject.name.Contains("Knight")) return;

            Splat();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));

            Splat();
        }

        private void Splat()
        {
            GameObject splatObj = gameObject.FindGameObjectInChildren("Splat");
            GameObject splat = Instantiate(splatObj, transform.position, Quaternion.identity);
            splat.SetActive(true);
            Destroy(gameObject);
        }
        
        private void Log(object message) => Modding.Logger.Log("[Spell Fluke Doppelganger] " + message);
    }    
}