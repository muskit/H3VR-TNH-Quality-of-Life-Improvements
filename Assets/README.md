# TNH Quality of Life Improvements
Ever got frustrated checking your HP against a bright ceiling in TNH?  
Have you forgotten how many Holds you're playing for, so you don't know if you should spend all your tokens?  
And... wait, which hold are you on again?  
**This mod adds quality of life improvements to the *Take and Hold* experience that help with these questions, and then some.**

## Features
* Better health counter visibility
* Health counter fade when aiming around it
* Token, hold, and wave counter on radar hand
* Player count for online leaderboards; see how you stack up!
  * Disabled if [*TakeAndHoldTweaker*](https://h3vr.thunderstore.io/package/devyndamonster/TakeAndHoldTweaker/) is installed
* Numerical representation of tokens at item stations
* Expiration indication for health crystals (configurable to multiple types)
* ...and possibly more!

Toggle and customize these features in your mod manager's *Config editor*.

**This mod will not disqualify you from Steam or TNHTweaker leaderboards.**

For any issues/ideas, please create an issue at the GitHub repository (linked on Thunderstore page).

## Changelog
1.2.4
* [TNH] Fix errors occurring in Atlas-based maps
* Update MeatKit (now on Unity 5.6.7f1!)

1.2.3
* Added button in wrist menu to toggle HP counter (thanks PutterMyBancakes for the suggestion!)
* [TNH] Made the search time for deprecated Leaderboard mod based on day of year as an input of the sin()

1.2.2
* When aiming around the HP counter, its opacity can now change to a player setting
* Increased size of hide-HP aiming region
* [TNH] Decreased search time for deprecated Leaderboard mod

1.2.1
* [TNH] Changed leaderboard player count message for unavailability with TNHTweaker

**1.2.0**
* HP counter text can now be hidden completely
* Added HP counter fading when pointing a firearm towards it, allowing better visiblity
* Shrunk borders of health counter's background

1.1.3
* [TNH] Fixed wrist stats still trying to look at the camera in the game over area, resulting in weird rotations

1.1.2
* [TNH] Wrist stats can now tilt towards the camera, making it less awkward to read

1.1.1
* Fixed wave counter text not showing up during a hold

**1.1.0**
* [TNH] Added win/lose count on hold counter
* [TNH] Added enemy waves counter (substitutes token counter during hold if enabled)
* [TNH] Added token numerical representation to shop
* [TNH] Extra info from this mod now shows in game over
* Added expiration indicators to Health Crystals
* Health readability now applies outside of Take and Hold

1.0.1
* Fixed the in-play improvements only applying to Classic Hallways map (whoops!!)
* Added option to enable/disable showing player count of online leaderboards
* Added option to enable/disable HP text opacity/shadow change
  * (Surprisingly, the HP text normally doesn't have full opacity)
* Searching for the deprecated TNH Leaderboards Player Count mod to kill now stops after 120s


**1.0.0**
* Initial release!

**NOTE: [*TNH Leaderboard Player Count*](https://h3vr.thunderstore.io/package/muskit/TNH_Leaderboard_Player_Count/) has been merged with this mod. If installed, please remove that mod as it lacks features and is no longer supported.**
