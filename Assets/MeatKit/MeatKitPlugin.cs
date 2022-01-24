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
    public static AssetBundle bundle;
    public static ConfigEntry<bool> cfgShowLPC;
    public static ConfigEntry<bool> cfgSolidifyHPText;
    public static ConfigEntry<bool> cfgShowHPBackground;
    public static ConfigEntry<float> cfgHPBackgroundOpacity;
    public static ConfigEntry<bool> cfgShowTokens;
    public static ConfigEntry<bool> cfgShowHolds;
    
    private static InPlay instance;
    
    private LeaderboardPlayerCountPatch lpc;
    private bool lpcModGone = false;
    private float lpcModSearchTimeEnd;

    private void SceneChanged(Scene from, Scene to)
    {
        //Logger.LogInfo(string.Format("scene chg: {0} --> {1}", from.name, to.name));
        Logger.LogInfo("_GameManager present: " + (GameObject.Find("_GameManager") != null));
        Logger.LogInfo("TNH_Manager object present: " + (FindObjectOfType<FistVR.TNH_Manager>() != null));
        if(GameObject.Find("_GameManager") != null || FindObjectOfType<FistVR.TNH_Manager>() != null)
        {
            Logger.LogInfo("We are in a TNH game!");
            instance = new GameObject().AddComponent<InPlay>();
        }
        else
        {
            Logger.LogInfo("We are NOT in a TNH game!");
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
        cfgShowHPBackground = Config.Bind("Health Counter",
                                          "Background enabled",
                                          true,
                                          "Apply a background to the health text.");
        cfgHPBackgroundOpacity = Config.Bind("Health Counter",
                                             "Background opacity",
                                             0.74f,
                                             "Set opacity of health text's background (if enabled).");
        cfgSolidifyHPText = Config.Bind("Health Counter",
                                        "Solidify HP text",
                                        true,
                                        "Set opacity of HP text to full and give it a shadow.");
        cfgShowLPC = Config.Bind("Game Info",
                                 "Show player count in online leaderboards",
                                 true,
                                 "Shows the number of players in the currently selected TNH leaderboard.");
        cfgShowTokens = Config.Bind("Game Info",
                                    "Show Tokens",
                                    true,
                                    "Shows how many tokens the player has by their radar hand.");
        cfgShowHolds = Config.Bind("Game Info",
                                   "Show Holds",
                                   true,
                                   "Shows how many holds the player has completed by their radar hand.");

        // patch leaderboard code
        if (cfgShowLPC.Value)
            lpc = new LeaderboardPlayerCountPatch();

        // give 120 seconds to search for old mod
        lpcModSearchTimeEnd = Time.realtimeSinceStartup + 120;
    }
    // DO NOT EDIT.
    private void LoadAssets() {}

    /// <summary>
    /// Its only purpose: to kill the deprecated TNH Leaderboard Player Count mod.
    /// </summary>
    private void Update()
    {
        if (lpcModGone)
            return;

        foreach (var plugin in Chainloader.PluginInfos)
        {
            if (plugin.Key == "me.muskit.tnhLeaderboardPlayerCount")
            {
                Logger.LogWarning("TNH Leaderboard Player Count mod detected. Destroying it to avoid interference.");
                Destroy(plugin.Value.Instance);
                lpcModGone = true;
            }
        }

        if (Time.realtimeSinceStartup >= lpcModSearchTimeEnd)
        {
            Logger.LogInfo("Stopping search for TNH Leaderboard Player Count mod after 120 seconds.");
            lpcModGone = true;
        }
    }
}
#endif