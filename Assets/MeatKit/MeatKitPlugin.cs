#if H3VR_IMPORTED
using HarmonyLib;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
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
    public static Font fontAgencyFB;
    public static Font fontBombardier;

    public static GameObject playerCamera;

    // BepInEx configuration
    //--- Health Counter ---//
    public static ConfigEntry<bool> cfgSolidifyHPText;
    public static ConfigEntry<bool> cfgShowHPBackground;
    public static ConfigEntry<float> cfgHPBackgroundOpacity;
    //--- Take and Hold Info ---//
    public static ConfigEntry<bool> cfgShowLPC;
    public static ConfigEntry<bool> cfgShowTokens;
    public static ConfigEntry<bool> cfgShowHolds;
    public static ConfigEntry<bool> cfgShowNumbersAtShop;
    public static ConfigEntry<bool> cfgShowInfoOnGameOver;
    public static ConfigEntry<bool> cfgShowWaves;
    //--- Misc. ---//
    public static ConfigEntry<HealthExpireIndicationType> cfgHealthCrystalIndicator;

    // Take and Hold modifications
    private static InPlay instance;
    
    // Searching for old leaderboards player count mod to disable
    private bool lpcModGone = false;
    private float lpcModSearchTimeEnd;

    private Harmony harmony;

    private void SceneChanged(Scene from, Scene to)
    {
        if (GameObject.Find("_GameManager") != null || FindObjectOfType<FistVR.TNH_Manager>() != null)
        {
            Logger.LogInfo("We are in a TNH game!");
            instance = new GameObject().AddComponent<InPlay>();
        }
        else
        {
            Logger.LogInfo("We are NOT in a TNH game!");
            Destroy(instance);
        }

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

        // apply health readability globally
        var healthCounter = FindObjectOfType<FistVR.FVRHealthBar>();
        if (healthCounter != null)
        {
            if (cfgShowHPBackground.Value || cfgSolidifyHPText.Value)
                HPReadability.ImproveHPTextReadability(healthCounter.transform.GetChild(0).gameObject);
        }

        // grab Agency FB from game if it's not set
        if(fontAgencyFB == null)
        {
            if (healthCounter != null)
            {
                fontAgencyFB = healthCounter.transform.GetChild(0).GetChild(0).GetComponent<Text>().font;
            }
            else
            {
                var query = FindObjectsOfType<Text>();
                foreach (Text itm in query)
                {
                    if (itm.font.name == "AGENCYR")
                    {
                        fontAgencyFB = itm.font;
                        break;
                    }
                }
            }
        }
    }

    public MeatKitPlugin(): base()
    {
        harmony = new Harmony("muskit.TNHQualityOfLifeImprovements");
    }

    private void Awake()
    {
        // MeatKit requirement
        LoadAssets();

        // get Agency FB from system (BAD IDEA, NOT EVERYONE WILL HAVE IT)
        //fontAgencyFB = Font.CreateDynamicFontFromOSFont("Agency FB", 16);

        // load asset bundle
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "tnh_qol_improvements"));
        SceneManager.activeSceneChanged += SceneChanged;

        fontBombardier = MeatKitPlugin.bundle.LoadAsset<Font>("Bombardier");

        // setup configuration
        //--- Health Counter ---//
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
        //--- Take and Hold Info ---//
        cfgShowLPC = Config.Bind("Take and Hold Info",
                                 "Show player count in online leaderboards",
                                 true,
                                 "Shows the number of players in the currently selected TNH leaderboard.");
        cfgShowTokens = Config.Bind("Take and Hold Info",
                                    "Show Tokens",
                                    true,
                                    "Shows how many tokens the player has by their radar hand.");
        cfgShowHolds = Config.Bind("Take and Hold Info",
                                   "Show Holds",
                                   true,
                                   "Shows how many holds the player has completed by their radar hand.");
        cfgShowWaves = Config.Bind("Take and Hold Info",
                                   "Show Waves",
                                   true,
                                   "Shows how many waves the player has completed on the current hold by their radar hand.");
        cfgShowInfoOnGameOver = Config.Bind("Take and Hold Info",
                                            "Show Extra Info at Game Over",
                                            true,
                                            "Show enabled extra game information at the game over area.");
        cfgShowNumbersAtShop = Config.Bind("Take and Hold Info",
                                           "Show Numbers for Tokens at Item Station",
                                           true,
                                           "At the item station, add a numberical representation to costs and player's tokens.");
        //--- Misc. ---//
        cfgHealthCrystalIndicator = Config.Bind("Misc.",
                                                "Show expiration of Health Crystals",
                                                HealthExpireIndicationType.Flashing,
                                                "Add a visual indication on the Health Crystal's despawn timer.");

        // give 120 seconds to search for old mod, which we want to kill
        lpcModSearchTimeEnd = Time.time + 120;

        RunPatches();
    }
    // DO NOT EDIT.
    private void LoadAssets() {}

    private void RunPatches()
    {
        if (harmony == null)
            return;

        // patch KillAll code (only acts w/ health crystals)
        if (cfgHealthCrystalIndicator.Value != HealthExpireIndicationType.None)
            TimedHealthCrystalPatch.Patch(harmony);

        // patch leaderboard code
        if (cfgShowLPC.Value)
            LeaderboardPlayerCountPatch.Patch(harmony);

        // for counting wins/loses for TNHInfo.holdCounter
        if (cfgShowHolds.Value)
            HoldCounterPatch.Patch(harmony);

        // stick stats to hand after game over
        if (cfgShowHolds.Value || cfgShowTokens.Value)
            InPlay.Patch(harmony);
        
        // show numerical representation of shop values
        if (cfgShowNumbersAtShop.Value)
        {
            // costs
            ShopCostPatch.Patch(harmony);
            // player tokens
            ShopTokenPatch.Patch(harmony);
        }
    }

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