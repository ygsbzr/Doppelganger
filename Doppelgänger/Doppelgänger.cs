using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IL.HutongGames.PlayMaker.Actions;
using ModCommon;
using Modding;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Doppelgänger
{
    public class Doppelgänger : Mod<SaveSettings>, ITogglableMod
    {
        public static Doppelgänger Instance { get; private set; }
        
        public static readonly Dictionary<string, GameObject> PreloadedGameObjects = new Dictionary<string, GameObject>();
        public static Dictionary<string, AudioClip> Audio = new Dictionary<string, AudioClip>();
        
        public override string GetVersion()
        {
            return "0.0.1";
        }
        
        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hive_Knight", "Battle Scene/Globs/Hive Knight Glob"),
                ("GG_Hive_Knight", "Battle Scene/Hive Knight/Slash 1"),
                ("GG_Ghost_Markoth", "Warrior/Ghost Warrior Markoth"),
            };
        }
        
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            //LoadAssets();
            LoadResources();
            
            PreloadedGameObjects.Add("Glob", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Globs/Hive Knight Glob"]);
            PreloadedGameObjects.Add("Slash", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Hive Knight/Slash 1"]);
            PreloadedGameObjects.Add("Markoth", preloadedObjects["GG_Ghost_Markoth"]["Warrior/Ghost Warrior Markoth"]);

            Instance = this;

            Unload();

            ModHooks.Instance.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook += AddComponent;
            ModHooks.Instance.LanguageGetHook += LangGet;
            
            Log("Initialized.");
        }

        private void LoadAssets()
        {
            AssetBundle doppelgangerAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "doppelganger"));
            Audio.Add("DoppelgangerMusic", doppelgangerAssetBundle.LoadAsset<AudioClip>("DoppelgangerMusic"));
        }

        private void LoadResources()
        {
            foreach (string resource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (resource.EndsWith(".wav"))
                {
                    Stream audioStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
                    if (audioStream != null)
                    {
                        byte[] buffer = new byte[audioStream.Length];
                        audioStream.Read(buffer, 0, buffer.Length);
                        audioStream.Dispose();
                        string restemp = resource.Replace("Doppelgänger.Assets.", "").Replace(".wav", "");
                        Audio.Add(restemp, WavUtility.ToAudioClip(buffer));
                    }

                    Modding.Logger.Log("Created Audio from " + resource);
                }
            }
        }
        
        public void Unload()
        {
            ModHooks.Instance.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook -= AddComponent;
            ModHooks.Instance.LanguageGetHook -= LangGet;
            
            // ReSharper disable once Unity.NoNullPropogation
            var finder = GameManager.instance?.gameObject.GetComponent<ArenaFinder>();
            if (finder == null) return;
            UObject.Destroy(finder);
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<ArenaFinder>();
        }

        private string LangGet(string key, string sheetTitle)
        {
            switch (key)
            {
                case "KNIGHT_NAME": return "The Knight";
                case "KNIGHT_DESC": return "All-powerful god of soul and shade.";
                case "KNIGHT_DREAM_1": return "...";
                default: return Language.Language.GetInternal(key, sheetTitle);
            }
        }
    }
}