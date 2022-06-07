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

using FistVR;
using TNHQoLImprovements;
using Sodalite.Api;
using Sodalite;

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
    public static ConfigEntry<bool> cfgHPHiddenWhenAiming;
    public static ConfigEntry<float> cfgHPAimOpacity;
    public static ConfigEntry<bool> cfgShowHPBackground;
    public static ConfigEntry<float> cfgHPBackgroundOpacity;
    public static ConfigEntry<HPTextType> cfgHPTextType;
    //--- Take and Hold Info ---//
    public static ConfigEntry<bool> cfgShowLPC;
    public static ConfigEntry<bool> cfgInfoFollowCamera;
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
    private float lpcSearchTime;
    private bool lpcStopSearching = false;
    private float lpcModSearchTimeEnd;

    public static FVRHealthBar hpDisplay;
    
    // toggle HP visibility from wrist menu
    private bool hpDisplayEnabled = true;
    private WristMenuButton wmbHPToggle;

    private Harmony harmony;

    private void SceneChanged(Scene from, Scene to)
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        hpDisplay = FindObjectOfType<FVRHealthBar>();
        GetFonts();

        // apply health counter tweaks globally
        if (hpDisplay != null)
        {
            HPReadability.ImproveHPTextReadability(hpDisplay.transform.GetChild(0).gameObject);

            if (cfgHPHiddenWhenAiming.Value)
                hpDisplay.gameObject.AddComponent<HPHideWhenAiming>();

            WristMenuAPI.Buttons.Add(wmbHPToggle);
        }
        else
        {
            WristMenuAPI.Buttons.Remove(wmbHPToggle);
        }

        // TNH patches
        if (GameObject.Find("_GameManager") != null || FindObjectOfType<TNH_Manager>() != null)
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
    
    // called on scene change, find fonts from game if they're not set
    private void GetFonts()
    {
        // Agency FB
        if (fontAgencyFB == null)
        {
            if (hpDisplay != null)
            {
                fontAgencyFB = hpDisplay.transform.GetChild(0).GetChild(0).GetComponent<Text>().font;
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
        lpcSearchTime = 30f + 30f * Mathf.Sin(System.DateTime.Today.DayOfYear / 365f);
    }

    private void Awake()
    {
        // MeatKit requirement
        LoadAssets();

        // get Agency FB from system (BAD IDEA, NOT EVERYONE WILL HAVE IT; MAY SET TO DEFAULT FONT)
        //fontAgencyFB = Font.CreateDynamicFontFromOSFont("Agency FB", 16);

        // load asset bundle
        bundle = AssetBundle.LoadFromFile(Path.Combine(BasePath, "tnh_qol_improvements"));
        SceneManager.activeSceneChanged += SceneChanged;

        fontBombardier = bundle.LoadAsset<Font>("Bombardier");

        // setup configuration
        //--- Health Counter ---//
        cfgHPHiddenWhenAiming = Config.Bind("Health Counter",
                                            "Hide HP Counter When Aiming",
                                            true,
                                            "While aiming around the health counter in view, hide it.");
        cfgHPAimOpacity = cfgHPBackgroundOpacity = Config.Bind("Health Counter",
                          "Aiming opacity",
                          0f,
                          "Opacity of Health Counter when aiming around it (if Hide HP is enabled).");
        cfgShowHPBackground = Config.Bind("Health Counter",
                                          "Background enabled",
                                          false,
                                          "Apply a background to the Health Counter.");
        cfgHPBackgroundOpacity = Config.Bind("Health Counter",
                                             "Background opacity",
                                             0.74f,
                                             "Set opacity of health text's background (if enabled).");
        cfgHPTextType = Config.Bind("Health Counter",
                                    "HP Text Type",
                                    HPTextType.Solidify,
                                    "Solidify: Set text to full opacity and give it a drop shadow\n" +
                                    "Untouched: Leave text untouched\n" +
                                    "Hidden: Hide health counter completely (will hide background if enabled)");
        //--- Take and Hold Info ---//
        cfgShowLPC = Config.Bind("Take and Hold Info",
                                 "Show Player Count in Online Leaderboards",
                                 true,
                                 "Shows the number of players in the currently selected TNH leaderboard.");
        cfgInfoFollowCamera = Config.Bind("Take and Hold Info",
                                          "Tilt Wrist Stats Towards Camera",
                                          true,
                                          "Tilt the extra wrist statistics from this mod towards the player's camera, allowing for easier readability.");
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

        // calculate end time to search for my deprecated Leaderboard
        lpcModSearchTimeEnd = Time.time + lpcSearchTime;

        wmbHPToggle = new WristMenuButton("Toggle HP Display", ToggleHPVisibility);

        RunPatches();
    }

    // DO NOT EDIT.
    private void LoadAssets() {}

    private void RunPatches()
    {
        if (harmony == null)
        {
            Logger.LogError("Could not run patches; Harmony didn't initialize correctly!");
            return;
        }

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

    private void ToggleHPVisibility(object sender, ButtonClickEventArgs args)
    {
        hpDisplayEnabled = !hpDisplayEnabled;
        if (hpDisplay != null)
            hpDisplay.gameObject.SetActive(hpDisplayEnabled);
    }

    /// <summary>
    /// Its only purpose: to kill the deprecated TNH Leaderboard Player Count mod.
    /// </summary>
    private void Update()
    {
        if (lpcStopSearching)
            return;

        foreach (var plugin in Chainloader.PluginInfos)
        {
            if (plugin.Key == "me.muskit.tnhLeaderboardPlayerCount")
            {
                Logger.LogWarning("TNH Leaderboard Player Count mod detected. Destroying it to avoid interference.");
                Destroy(plugin.Value.Instance);
                lpcStopSearching = true;
            }
        }

        if (Time.realtimeSinceStartup >= lpcModSearchTimeEnd)
        {
            Logger.LogInfo(string.Format("Stopping search for TNH Leaderboard Player Count mod after {0} seconds.", lpcSearchTime));
            lpcStopSearching = true;
        }
    }
}
#endif