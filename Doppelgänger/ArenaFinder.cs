using System.Collections;
using IL.HutongGames.PlayMaker.Actions;
using ModCommon;
using UnityEngine;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Doppelgänger
{
    public class ArenaFinder : MonoBehaviour
    {
        private void Start()
        {
            USceneManager.activeSceneChanged += OnSceneChange;
        }

        private void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GG_Workshop")
            {
                SetStatue();
            }

            //nextScene.PrintHierarchy();
            
            if (nextScene.name != "GG_Mighty_Zote") return;
            if (prevScene.name != "GG_Workshop") return;
            
            AddComponent();
        }

        private void SetStatue()
        {
            GameObject statue = Instantiate(GameObject.Find("GG_Statue_TraitorLord"));
            statue.transform.SetPosition2D(250f, statue.transform.GetPositionY());
            BossScene scene = ScriptableObject.CreateInstance<BossScene>();
            scene.sceneName = "GG_Mighty_Zote";
            BossStatue bossStatue = statue.GetComponent<BossStatue>();
            bossStatue.bossScene = scene;
            
            BossStatue.Completion completion = new BossStatue.Completion
            {
                completedTier1 = true,
                completedTier2 = true,
                completedTier3 = true,
                seenTier3Unlock = true,
                isUnlocked = true,
                hasBeenSeen = true,
                usingAltVersion = false,
            };

            bossStatue.StatueState = completion;
            BossStatue.BossUIDetails details = new BossStatue.BossUIDetails();
            details.nameKey = details.nameSheet = "KNIGHT_NAME";
            details.descriptionKey = details.descriptionSheet = "KNIGHT_DESC";
            bossStatue.bossDetails = details;
            foreach (SpriteRenderer sr in bossStatue.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.sprite = null;
            }

            GameObject knightStatue = GameObject.Find("GG_Statue_Knight");
            GameObject statueV1 = knightStatue.FindGameObjectInChildren("Knight_v01");
            GameObject statueV2 = knightStatue.FindGameObjectInChildren("Knight_v02");
            GameObject statueV3 = knightStatue.FindGameObjectInChildren("Knight_v03");
            Destroy(statueV1.FindGameObjectInChildren("Interact"));
            Destroy(statueV2.FindGameObjectInChildren("Interact"));
            Destroy(statueV3.FindGameObjectInChildren("Interact"));
        }

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<BattleControl>();
        }
    }
}