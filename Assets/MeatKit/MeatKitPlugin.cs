#if H3VR_IMPORTED
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

using TNHQoLImprovements;

/*
 * SUPER LARGE WARNING ABOUT THIS CLASS
 * This class can be used to add custom behaviour to your generated BepInEx plugin.
 * Please note, however, that all of the things in here already are REQUIRED and CANNOT BE CHANGED.
 * There are LARGE TEXT WARNINGS above such items so you don't forget.
 * You may add to this class so long as you do not modify anything with those notices (lest you want build errors)
 *
 * The class name and BepInPlugin attribute are modified at build-time to reflect your build settings.
 * BepInDependency attributes will automatically be generated if they're required by a build item, otherwise
 * may add it yourself here.
 */

// DO NOT REMOVE OR CHANGE ANY OF THESE ATTRIBUTES
[BepInPlugin("MeatKit", "MeatKit Plugin", "1.0.0")]
[BepInProcess("h3vr.exe")]

// DO NOT CHANGE THE NAME OF THIS CLASS.
public class MeatKitPlugin : BaseUnityPlugin
{
    // DO NOT CHANGE OR REMOVE THIS FIELD.
#pragma warning disable 414
    private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#pragma warning restore 414

    private bool lpcKilled = false;

    public static AssetBundle bundle;

    public static ConfigEntry<bool> showHPBackground;
    public static ConfigEntry<float> hpBackgroundOpacity;
    public static ConfigEntry<bool> showTokens;
    public static ConfigEntry<bool> showHolds;

    private static InPlay instance;
    private LeaderboardPlayerCountPatch lpc;

    private void SceneChanged(Scene from, Scene to)
    {
        //Logger.LogInfo(string.Format("scene chg: {0} --> {1}", from.name, to.name));
        if(GameObject.Find("_NewTAHReticle") != null)
        {
            instance = new GameObject().AddComponent<InPlay>();
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Awake()
    {
        // load asset bundle
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "tnh_qol_improvements"));
        SceneManager.activeSceneChanged += SceneChanged;
        LoadAssets();

        // setup configuration
        showHPBackground = Config.Bind("Health Counter",
                                       "Background enabled",
                                       true,
                                       "Apply a background to the health text.");
        hpBackgroundOpacity = Config.Bind("Health Counter",
                                          "Background opacity",
                                          0.74f,
                                          "Set opacity of health text's background (if enabled).");
        showTokens = Config.Bind("Game Info",
                                 "Show Tokens",
                                 true,
                                 "Shows how many tokens the player has by their radar hand.");
        showHolds = Config.Bind("Game Info",
                                "Show Holds",
                                true,
                                "Shows how many holds the player has completed by their radar hand.");

        // patch the leaderboard
        lpc = new LeaderboardPlayerCountPatch();
    }
    // DO NOT EDIT.
    private void LoadAssets() {}

    /// <summary>
    /// Its only purpose: to kill TNH Leaderboard Player Count
    /// </summary>
    private void Update()
    {
        if (lpcKilled)
            return;

        foreach (var plugin in Chainloader.PluginInfos)
        {
            if (plugin.Key == "me.muskit.tnhLeaderboardPlayerCount")
            {
                Logger.LogWarning("TNH Leaderboard Player Count mod detected. Destroying it to avoid interference.");
                Destroy(plugin.Value.Instance);
                lpcKilled = true;
            }
        }
    }
}
#endif