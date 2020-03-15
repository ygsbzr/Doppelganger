using System.Collections;
using UnityEngine;

namespace Doppelgänger
{
    internal partial class Doppelganger : MonoBehaviour
    {
        private IEnumerator DoppelgangerIntro()
        {
            _anim.Play("Wake Up Ground");

            yield return new WaitForSeconds(_anim.PlayAnimGetTime("Wake Up Ground"));

            _collider.enabled = true;
            _music.Play();
            _rb.gravityScale = 1.0f;

            StartCoroutine(IdleAndChooseNextAttack());
        }
    }
}