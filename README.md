# BalanceConfigurator

[![GitHub Release](https://img.shields.io/github/v/release/Monster-Train-2-Modding-Group/Balance-Configurator?color=4CAF50&label=latest)](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/releases)
[![Trainworks Reloaded](https://img.shields.io/badge/framework-Trainworks--Reloaded-blue?logo=github)](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded)
[![License](https://img.shields.io/github/license/Monster-Train-2-Modding-Group/Balance-Configurator?color=lightgrey)](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/LICENSE)
[![Donate](https://img.shields.io/badge/Ko--Fi-brandonandzeus-F16061?color=F16061&logo=ko-fi&style=flat&labelColor=?color=4E4E4E&logoColor=FFFFFF)](https://ko-fi.com/brandonandzeus)

Support language: English | [简体中文](https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/blob/main/README_zh.md)

![icon](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/icon.png)

Non-cosmetic mod that allows one to edit fields from BalanceData, and as an example of a mod using Trainworks Reloaded that has user editable configuration.

![starting](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/starting.png)

![relic_draft](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/relic_reward.png)
<sub>The reward for skipping an artifact was adjusted from 25 to 75 gold</sub>

![champion_upgrade](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/champion_upgrades.png)
<sub>All three champion upgrades are available! (The UI doesn't handle this well)</sub>

![deployment](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/deployment.png)
<sub>You can change ember amount and the extra deployment ember</sub>

![battle](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/battle.png)
<sub>You can change your hand size and the maximum hand size. Here its 10. Along with 5 ember</sub>

![card_draft](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/card_draft.png)
<sub>Reward for skipping card drafts can also be adjusted</sub>

![bog_wurm](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/bogwurm.jpg)
<sub>Additional QoL stats for displaying the capacities per floor if bogwurm's growth pyre is used</sub>

## Features

 Ability to edit particular starting details of runs among other things:

* Ticket values for each rarity type for cards, enhancers (shop upgrades), and artifacts
  * Example: for enhancers the rarity tickets are Common = 70, Uncommon = 30 meaning 70% chance for common, and 30% chance for uncommon.
* Starting gold
* Maximum hand size
* Start of turn ember
* Extra ember for deployment phase
* Max ember
* Initial cap for Dragons Hoard
* Max cap for Dragons Hoard
* Start of turn hand size
* Unit Upgrade slots
* Spell Upgrade Slots
* Equipment Upgrade Slots (*not useful*)
* Maximum spawn points per floor
* Initial Unit capacity per floor
* Maximum number of mutators
* Number of champion upgrades shown
* Main class Xp factor (controls experience gained)
* Subclass Xp factor
* Alternate champion unlock level
* Gold for skipping rewards.
* (New 1.2.0) Chance of optional outpost dialogue after a run
* (New 1.2.0) Fast dialogue multiplier
* (New 1.2.0) Costs of items in shops by rarity (Cards, Enhancers, and Artifacts)
* (New 1.3.0) Maximum number of entries present in Run History
* (New 1.4.0) Story priority ticket counts
* (New 1.4.0) Enable Shattered Halo forcing rares for Banner Drafts
* (New 1.4.0) Number of options for Clan Banner Drafts
* (New 1.4.0) Eliminate Run Rarity Floors (Allow Common Drafts to be found past ring 2)
* (New 1.4.0) Eliminate Run Rarity Floor For Equipment Merchant (Equipment Merchant sells common equipment/rooms)
* (New 1.4.0) Enable Card Mastery for all run types (Can master cards from Dimensional Challenges, Mutators, and Community Challenges)
* (New 2.2.0) Options for default Deck Sort / Persistent Deck Sort.
* (New 2.3.0) Option to sort deck by deployment phase hand.
* (New 3.0.0) Option to modify boss relics costs in Soul Savior mode.
* (New 3.0.0) Option to modify maximum card upgrade slots
* (New 3.0.0) Option to modify various Soul Savior options (# of souls for 2-3 souls at start, rerolls).
* (New 3.1.0) Option to start an endless run from a run with mutators or malicka's challenge.
* (New 3.2.0) Option to enable lore tooltip display for Artifacts, Cards, and Characters.
* (New 3.3.0) Option to display the room capacity increases per floor when using Bogwurm's Growth Pyre.

## Installation / Initial Setup

It is **highly** recommended to use a mod manager (Thunderstore Mod Manager, Gale Mod Manager, or r2modman) to install this mod. If installing manually, then [Trainworks Reloaded](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded) must be installed as well.

If you don't know which mod manager to choose, then Gale Mod Manager or r2modman are preferred. Gale gives a more modern interface while r2modman UI's is similar to Thunderstore Mod Manager without the advertisements. For more information on how to install mods visit the [wiki tutorial page](https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded/wiki/Installing-Mods)

Once installed via mod manager or manually. You will need to run the game once with mods enabled (there should be a button in the mod manager to run modded) to generate the configuration. Close the game afterward.


### Config editing via your mod manager
The file to edit is `BalanceConfigurator.Plugin.cfg`. The default values for each configuration option are those used within the game.


#### Thunderstore Mod Manager

Hit the "Edit Config" button on the left panel, once you chosen Monster Train 2 and your modding profile. You will be presented with several configuration files.

Under `BepInEx/config/BalanceConfigurator.Plugin.cfg` select `Edit Config` and you will be presented with this screen.

![tmm](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/tmm.png)

You can click on a Section to view the configuration options relevant to the section or your simply scroll down to view all of the configuration options.

#### r2modman

After selecting Monster Train 2 and your profile. in the left panel, under `Other` select `Config editor`. You will be presented with all of the configuration files.

Select `BepInEx/config/BalanceConfigurator.Plugin.cfg`. Then Press `Edit Config` and you will be presented with this screen

![r2modman](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/r2modman.png)

You can click on a Section to view the configuration options relevant to the section or your simply scroll down to view all of the configuration options.

#### Gale Mod Manager

After selecting Monster Train 2 and your profile. in the left panel, click the page button with a gear int he bottom right hand corner (3rd button down) You will be presented with the mod names with dropdowns.

Expand `BalanceConfigurator.Plugin` You will be presented with each configuration section upon expansion. Click on the relevant configuration section to see th options available to that section.

Afterwards you can edit the configuration options

![gale](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/gale.png)

#### Manual editing of config

If installed manually it should be placed where you installed BepinEx. Normally such an install would be placed in the same directory as the game's executable.

Relative to the BepInEx folder the file will be placed in the `config` directory.

For instance in my computer the file is located at `C:\Program Files (x86)\Steam\steamapps\common\Monster Train 2\BepInEx\config\BalanceConfigurator.Plugin.cfg`

The file can be edited in any text editor, I suggest your favorite text editor. Note that the configuration options are available in Chinese so if you see any weird characters your text editor doesn't support additional languages.

![manual](https://raw.githubusercontent.com/Monster-Train-2-Modding-Group/Balance-Configurator/main/screenshots/manual.png)

## Warning

As with all non cosmetic mods and as a warning **do not** do anything online such as community challenges or the daily challenge with modded content. Getting an unfair advantage and setting a high score will surely get you banned ([Steam forums post - Don't anger ShinySteve!](https://steamcommunity.com/app/2742830/discussions/0/599653789035669752/)).

The default configuration in the mod (see usage below) is the same as how the game was configured.

The only ***probably safe*** fields you can modify without giving an yourself an unfair advantage in online play are

- Main class Xp factor
- Subclass Xp factor
- Alternate champion unlock level
- Chance of optional outpost dialogue after a run
- Fast dialogue multiplier
- Maximum number of entries present in Run History
- Card Mastery for other run types
- Deck sort options
- Soul savior specific fields
- lore options

### **Important**

F8 bug reports will be disabled with this mod enabled (you can still open the menu, but submitting a report won't send it anywhere). This feature was asked for directly from Shiny Shoe and should be present in most mods. If you find a bug in the game and suspect that it isn't due to your use of mods, file a report  at [Monster Train 2 Bugs and Technical issues :: Steam Community](https://steamcommunity.com/app/2742830/discussions/1/) or on the Shiny Shoe discord.

## Lastly

We are not responsible for any unintended behavior / damage that results from using this mod. Use responsibly.
