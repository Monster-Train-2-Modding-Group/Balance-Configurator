using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using ShinyShoe;
using System.Text;
using TrainworksReloaded.Base;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using UnityEngine;
using static BalanceData;

namespace BalanceConfigurator.Plugin
{
    class ConfigDescriptionBuilder
    {
        public string English { get; set; } = "";
        public string French { get; set; } = "";
        public string German { get; set; } = "";
        public string Russian { get; set; } = "";
        public string Portuguese { get; set; } = "";
        public string Chinese { get; set; } = "";
        public string Spanish { get; set; } = "";
        public string ChineseTraditional { get; set; } = "";
        public string Korean { get; set; } = "";
        public string Japanese { get; set; } = "";

        public override string ToString()
        {
            StringBuilder builder = new();
            if (!string.IsNullOrEmpty(English)) builder.AppendLine(English);
            if (!string.IsNullOrEmpty(French)) builder.AppendLine(French);
            if (!string.IsNullOrEmpty(German)) builder.AppendLine(German);
            if (!string.IsNullOrEmpty(Russian)) builder.AppendLine(Russian);
            if (!string.IsNullOrEmpty(Portuguese)) builder.AppendLine(Portuguese);
            if (!string.IsNullOrEmpty(Chinese)) builder.AppendLine(Chinese);
            if (!string.IsNullOrEmpty(Spanish)) builder.AppendLine(Spanish);
            if (!string.IsNullOrEmpty(ChineseTraditional)) builder.AppendLine(ChineseTraditional);
            if (!string.IsNullOrEmpty(Korean)) builder.AppendLine(Korean);
            if (!string.IsNullOrEmpty(Japanese)) builder.AppendLine(Japanese);
            return builder.ToString().TrimEnd();
        }
    }

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);

        ConfigEntry<int>? startingGold;
        ConfigEntry<int>? maxHandSize;
        ConfigEntry<int>? startOfTurnEnergy;
        ConfigEntry<int>? startOfDeploymentPhaseEnergy;
        ConfigEntry<int>? maxEnergy;
        ConfigEntry<int>? initialDragonsHoardCap;
        ConfigEntry<int>? maxDragonsHoard;
        ConfigEntry<int>? startOfTurnCards;
        ConfigEntry<int>? unitUpgradeSlots;
        ConfigEntry<int>? spellUpgradeSlots;
        ConfigEntry<int>? equipmentUpgradeSlots;
        ConfigEntry<int>? numSpawnPointsPerFloor;
        ConfigEntry<int>? characterCapacityPerFloor;
        ConfigEntry<int>? maxMutatorCount;
        ConfigEntry<int>? championUpgradesShown;
        ConfigEntry<float>? mainClanXpFactor;
        ConfigEntry<float>? subClanXpFactor;
        ConfigEntry<int>? alternateChampionUnlockLevel;

        //cardRarityTicketValues
        ConfigEntry<int>? cardRarityTicketCommon;
        ConfigEntry<int>? cardRarityTicketUncommon;
        ConfigEntry<int>? cardRarityTicketRare;
        ConfigEntry<int>? cardRarityTicketChampion;

        //enhancerRarityTicketValues
        ConfigEntry<int>? enhancerRarityTicketCommon;
        ConfigEntry<int>? enhancerRarityTicketUncommon;
        ConfigEntry<int>? enhancerRarityTicketRare;
        ConfigEntry<int>? enhancerRarityTicketChampion;

        //relicRarityTicketValues
        ConfigEntry<int>? relicRarityTicketCommon;
        ConfigEntry<int>? relicRarityTicketUncommon;
        ConfigEntry<int>? relicRarityTicketRare;
        ConfigEntry<int>? relicRarityTicketChampion;

        // goldForSkippingRewards
        ConfigEntry<int>? goldSkipCardDraft;
        ConfigEntry<int>? goldSkipRelicDraft;
        ConfigEntry<int>? goldSkipChampionUpgrade;
        ConfigEntry<int>? goldSkipIndividualRelic;
        ConfigEntry<int>? goldSkipIndividualCard;
        ConfigEntry<int>? goldSkipPurge;
        ConfigEntry<int>? goldSkipLevelUpUnit;

        // new entries
        ConfigEntry<int>? chanceOfOptionalOutpostDialogue;
        ConfigEntry<float>? fastDialogueMultiplier;

        // draft costs
        ConfigEntry<int>? draftCostCommonCardMin;
        ConfigEntry<int>? draftCostCommonCardMax;
        ConfigEntry<int>? draftCostUncommonCardMin;
        ConfigEntry<int>? draftCostUncommonCardMax;
        ConfigEntry<int>? draftCostRareCardMin;
        ConfigEntry<int>? draftCostRareCardMax;

        ConfigEntry<int>? draftCostCommonRelicMin;
        ConfigEntry<int>? draftCostCommonRelicMax;
        ConfigEntry<int>? draftCostUncommonRelicMin;
        ConfigEntry<int>? draftCostUncommonRelicMax;
        ConfigEntry<int>? draftCostRareRelicMin;
        ConfigEntry<int>? draftCostRareRelicMax;

        ConfigEntry<int>? draftCostCommonEnhancerMin;
        ConfigEntry<int>? draftCostCommonEnhancerMax;
        ConfigEntry<int>? draftCostUncommonEnhancerMin;
        ConfigEntry<int>? draftCostUncommonEnhancerMax;
        ConfigEntry<int>? draftCostRareEnhancerMin;
        ConfigEntry<int>? draftCostRareEnhancerMax;

        ConfigEntry<int>? runHistoryMaxEntries;


        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            FormConfiguration();

            // The action gets called later after all mods are initialized, json parsed, and injected into the game data.
            Railend.ConfigurePostAction(
                c =>
                {
                    // GameDataClient has all of the relevant Provider Objects, We get the instance from the SimpleInjector Container object.
                    var client = c.GetInstance<GameDataClient>();

                    // Get a provider (SaveManager) from GameDataClient.
                    if (!client.TryGetProvider<SaveManager>(out var saveManager))
                    {
                        Logger.LogError("Failed to get SaveManager instance please report this https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/issues");
                        return;
                    }

                    // Do the magic
                    var allGameData = saveManager.GetAllGameData();
                    var balanceData = allGameData.GetBalanceData();
                    ReconfigureBalance(balanceData);
                }
            );


            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void FormConfiguration()
        {
            // Gold
            startingGold = Config.Bind<int>("Gold", "Starting Gold", 50,
                new ConfigDescriptionBuilder
                {
                    English = "Starting Gold in a run.",
                    Chinese = "修改一轮游戏的初始金钱。"
                }.ToString());


            // Hand Size
            startOfTurnCards = Config.Bind<int>("Hand Size", "Initial Hand Size", 5,
                new ConfigDescriptionBuilder
                {
                    English = "Starting hand size.",
                    Chinese = "修改每回合的初始抽牌数量。"
                }.ToString());
            maxHandSize = Config.Bind<int>("Hand Size", "Max Hand Size", 10,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum hand size possible.",
                    Chinese = "修改手牌数量上限。"
                }.ToString());


            // Ember
            startOfTurnEnergy = Config.Bind<int>("Ember", "Starting Ember", 3,
                new ConfigDescriptionBuilder
                {
                    English = "Starting ember gained per turn in a run.",
                    Chinese = "修改每回合获得的初始余烬数量。"
                }.ToString());
            startOfDeploymentPhaseEnergy = Config.Bind<int>("Ember", "Deployment Phase Ember", 1,
                new ConfigDescriptionBuilder
                {
                    English = "Extra ember given during the Deployment Phase.",
                    Chinese = "修改初始部署余烬数量。"
                }.ToString());
            maxEnergy = Config.Bind<int>("Ember", "Maximum Ember", 99,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum ember that is possible during a turn.",
                    Chinese = "修改余烬数量上限。"
                }.ToString());
            

            // Dragons Hoard
            initialDragonsHoardCap = Config.Bind<int>("Dragons Hoard", "Initial Dragons Hoard Cap", 8,
                new ConfigDescriptionBuilder
                {
                    English = "Initial cap on Dragons Hoard.",
                    Chinese = "修改龙族宝藏的初始数量上限。"
                }.ToString());
            maxDragonsHoard = Config.Bind<int>("Dragons Hoard", "Maximum Dragons Hoard Cap", 13,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cap on Dragons Hoard that's possible in given run.",
                    Chinese = "修改龙族宝藏的最大数量上限。"
                }.ToString());

            // Upgrades and Slots
            unitUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Unit Upgrade Slots", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of unit upgrade slots available.",
                    Chinese = "修改单位的初始升级栏位数量。"
                }.ToString());
            spellUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Spell Upgrade Slots", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of spell upgrade slots available.",
                    Chinese = "修改法术的初始升级栏位数量。"
                }.ToString());
            equipmentUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Equipment Upgrade Slots", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of equipment upgrade slots available (Currently equipment can't be upgraded).",
                    Chinese = "修改装备的初始升级栏位数量（目前装备无法升级）。"
                }.ToString());
            championUpgradesShown = Config.Bind<int>("Upgrades and Slots", "Champion Upgrades Shown", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Number of champion upgrades shown in the Light Forge.",
                    Chinese = "修改光之锻炉出现的勇者升级数量。"
                }.ToString());

            // Floor Balance
            numSpawnPointsPerFloor = Config.Bind<int>("Floor Balance", "Num Spawn Points Per Floor", 7,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum number of spawn points (characters) that can be present on a side of the floor.",
                    Chinese = "修改每层每方势力的单位数量上限。"
                }.ToString());
            characterCapacityPerFloor = Config.Bind<int>("Floor Balance", "Character Capacity Per Floor", 5,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of capacity per floor.",
                    Chinese = "修改每层的初始容量值。"
                }.ToString());

            // Mutators
            maxMutatorCount = Config.Bind<int>("Mutators", "Max Mutator Count", 3,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum amount of mutators that can be applied to a run.",
                    Chinese = "修改一轮游戏的转变器数量上限。"
                }.ToString());

            // XP and Unlocks
            mainClanXpFactor = Config.Bind<float>("XP and Unlocks", "Main Clan Xp Factor", 1.0f,
                new ConfigDescriptionBuilder
                {
                    English = "Scales the base amount of XP amount given to the primary clan at the end of a run.",
                    Chinese = "修改在一轮游戏结束之后，主氏族获得经验的倍率。"
                }.ToString());
            subClanXpFactor = Config.Bind<float>("XP and Unlocks", "Sub Clan Xp Factor", 0.5f,
                new ConfigDescriptionBuilder
                {
                    English = "Scales the base amount of XP amount given to the allied clan at the end of a run.",
                    Chinese = "修改在一轮游戏结束之后，盟友氏族获得经验的倍率。"
                }.ToString());
            alternateChampionUnlockLevel = Config.Bind<int>("XP and Unlocks", "Alternate Champion Unlock Level", 5,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "The level at which the alternate champion is available.",
                    Chinese = "修改备选勇者的解锁等级。"
                }.ToString(), new AcceptableValueRange<int>(0, 10)));


            //cardRarityTicketValues
            cardRarityTicketCommon = Config.Bind<int>("Card Rarity Ticket Counts", "Common", 940,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a common card when drafting a card. (Rings 1 & 2 only, non banner drafts)",
                    Chinese = "修改自选卡牌时，普通卡牌出现的权重。出现的几率为该权重占权重总和中的比例。（仅前2层区域的除战旗以外的自选卡牌）"
                }.ToString());
            cardRarityTicketUncommon = Config.Bind<int>("Card Rarity Ticket Counts", "Uncommon", 55,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a uncommon card when drafting a card.",
                    Chinese = "修改自选卡牌时，高级卡牌出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            cardRarityTicketRare = Config.Bind<int>("Card Rarity Ticket Counts", "Rare", 5,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a rare card when drafting a card.",
                    Chinese = "修改自选卡牌时，稀有卡牌出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            cardRarityTicketChampion = Config.Bind<int>("Card Rarity Ticket Counts", "Champion", 0,
                new ConfigDescriptionBuilder
                {
                    English = "(Unused) number of tickets that will produce a champion card in a card draft.",
                    Chinese = "（未使用）修改自选卡牌时，勇者卡牌出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());


            //enhancerRarityTicketValues
            enhancerRarityTicketCommon = Config.Bind<int>("Enhancer (Shop Upgrade) Rarity Ticket Counts", "Common", 70,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a common enhancer when finding an enhancer.",
                    Chinese = "修改商店升级时，普通升级石出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            enhancerRarityTicketUncommon = Config.Bind<int>("Enhancer (Shop Upgrade) Rarity Ticket Counts", "Uncommon", 30,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a uncommon enhancer when finding an enhancer.",
                    Chinese = "修改商店升级时，高级升级石出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            enhancerRarityTicketRare = Config.Bind<int>("Enhancer (Shop Upgrade) Rarity Ticket Counts", "Rare", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a rare enhancer when finding an enhancer.",
                    Chinese = "修改商店升级时，稀有升级石出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            enhancerRarityTicketChampion = Config.Bind<int>("Enhancer (Shop Upgrade) Rarity Ticket Counts", "Champion", 0,
                new ConfigDescriptionBuilder
                {
                    English = "(Unused) number of tickets that will produce a champion enhancer when finding an enhancer.",
                    Chinese = "（未使用）修改商店升级时，勇者升级石出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());


            //relicRarityTicketValues
            relicRarityTicketCommon = Config.Bind<int>("Relic Rarity Ticket Counts", "Common", 80,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a common relic when drafting a relic.",
                    Chinese = "修改自选神器时，普通神器出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            relicRarityTicketUncommon = Config.Bind<int>("Relic Rarity Ticket Counts", "Uncommon", 20,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a uncommon relic when drafting a relic.",
                    Chinese = "修改自选神器时，高级神器出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            relicRarityTicketRare = Config.Bind<int>("Relic Rarity Ticket Counts", "Rare", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Number of tickets that will produce a rare relic when drafting a relic.",
                    Chinese = "修改自选神器时，稀有神器出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());
            relicRarityTicketChampion = Config.Bind<int>("Relic Rarity Ticket Counts", "Champion", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Unused no chance of finding a champion relic in a relic draft.",
                    Chinese = "（未使用）修改自选神器时，勇者神器出现的权重。出现的几率为该权重占权重总和中的比例。"
                }.ToString());


            // goldForSkippingRewards
            goldSkipCardDraft = Config.Bind<int>("Gold For Skipping Rewards", "Card Draft", 10,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a Card Draft.",
                    Chinese = "修改跳过自选卡牌时获得的金钱。"
                }.ToString());
            goldSkipRelicDraft = Config.Bind<int>("Gold For Skipping Rewards", "Relic Draft", 25,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a Relic Draft.",
                    Chinese = "修改跳过自选神器时获得的金钱。"
                }.ToString());
            goldSkipChampionUpgrade = Config.Bind<int>("Gold For Skipping Rewards", "Champion Upgrade Draft", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a Champion Upgrade.",
                    Chinese = "修改跳过勇者升级时获得的金钱。"
                }.ToString());
            goldSkipIndividualRelic = Config.Bind<int>("Gold For Skipping Rewards", "Individual Relic", 25,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a singular Relic Draft.",
                    Chinese = "修改跳过单件神器时获得的金钱。"
                }.ToString());
            goldSkipIndividualCard = Config.Bind<int>("Gold For Skipping Rewards", "Individual Card", 10,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a singular Card Draft.",
                    Chinese = "修改跳过单张卡牌时获得的金钱。"
                }.ToString());
            goldSkipPurge = Config.Bind<int>("Gold For Skipping Rewards", "Purge", 0,
                new ConfigDescriptionBuilder
                {
                    English = "Gold for skipping a Purge.",
                    Chinese = "修改跳过移除卡牌时获得的金钱。"
                }.ToString());
            goldSkipLevelUpUnit = Config.Bind<int>("Gold For Skipping Rewards", "Level Up Unit", 0,
                new ConfigDescriptionBuilder
                {
                    English = "(Unused) Gold for skipping a Level Up Unit.",
                    Chinese = "（未使用）修改跳过升级单位时获得的金钱。"
                }.ToString());


            chanceOfOptionalOutpostDialogue = Config.Bind<int>("Dialogues", "Chance of Optional Output Dialogue", 30,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Percentage chance of getting an optional outpost dialogue after a run.",
                    Chinese = "修改在一轮游戏结束之后，契约前哨站可选对话的出现概率百分比 。"
                }.ToString(), new AcceptableValueRange<int>(0, 100)));
            fastDialogueMultiplier = Config.Bind<float>("Dialogues", "Fast Dialog Multiplier", 2.0f,
                new ConfigDescriptionBuilder
                {
                    English = "Multiplier for fast dialogues.",
                    Chinese = "修改对话的速度倍率。"
                }.ToString());


            draftCostCommonCardMin = Config.Bind<int>("Card Draft Costs", "Minimum Cost Common Card", 20,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a common card.",
                    Chinese = "修改普通卡牌的最低价格。"
                }.ToString());
            draftCostCommonCardMax = Config.Bind<int>("Card Draft Costs", "Maximum Cost Common Card", 20,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an common card.",
                    Chinese = "修改普通卡牌的最高价格。"
                }.ToString());
            draftCostUncommonCardMin = Config.Bind<int>("Card Draft Costs", "Minimum Cost Uncommon Card", 40,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a uncommon card.",
                    Chinese = "修改高级卡牌的最低价格。"
                }.ToString());
            draftCostUncommonCardMax = Config.Bind<int>("Card Draft Costs", "Maximum Cost Uncommon Card", 40,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an uncommon card.",
                    Chinese = "修改高级卡牌的最高价格。"
                }.ToString());
            draftCostRareCardMin = Config.Bind<int>("Card Draft Costs", "Minimum Cost Rare Card", 90,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a rare card.",
                    Chinese = "修改稀有卡牌的最低价格。"
                }.ToString());
            draftCostRareCardMax = Config.Bind<int>("Card Draft Costs", "Maximum Cost Rare Card", 90,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of a rare card.",
                    Chinese = "修改稀有卡牌的最高价格。"
                }.ToString());


            draftCostCommonRelicMin = Config.Bind<int>("Relic Draft Costs", "Minimum Cost Common Relic", 125,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a common artifact.",
                    Chinese = "修改普通神器的最低价格。"
                }.ToString());
            draftCostCommonRelicMax = Config.Bind<int>("Relic Draft Costs", "Maximum Cost Common Relic", 175,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an common artifact.",
                    Chinese = "修改普通神器的最高价格。"
                }.ToString());
            draftCostUncommonRelicMin = Config.Bind<int>("Relic Draft Costs", "Minimum Cost Uncommon Relic", 125,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a uncommon artifact.",
                    Chinese = "修改高级神器的最低价格。"
                }.ToString());
            draftCostUncommonRelicMax = Config.Bind<int>("Relic Draft Costs", "Maximum Cost Uncommon Relic", 175,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an uncommon artifact.",
                    Chinese = "修改高级神器的最高价格。"
                }.ToString());
            draftCostRareRelicMin = Config.Bind<int>("Relic Draft Costs", "Minimum Cost Rare Relic", 999,
                new ConfigDescriptionBuilder
                {
                    English = "(Unused) Minimum cost of a rare shop artifact.",
                    Chinese = "（未使用）修改稀有神器的最低价格。"
                }.ToString());
            draftCostRareRelicMax = Config.Bind<int>("Relic Draft Costs", "Maximum Cost Rare Relic", 999,
                new ConfigDescriptionBuilder
                {
                    English = "(Unused) Maximum cost of a rare artifact.",
                    Chinese = "（未使用）修改稀有神器的最高价格。"
                }.ToString());


            draftCostCommonEnhancerMin = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Minimum Cost Common Enhancer", 15,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a common shop upgrade.",
                    Chinese = "修改普通升级石的最低价格。"
                }.ToString());
            draftCostCommonEnhancerMax = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Maximum Cost Common Enhancer", 25,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an common shop upgrade.",
                    Chinese = "修改普通升级石的最高价格。"
                }.ToString());
            draftCostUncommonEnhancerMin = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Minimum Cost Uncommon Enhancer", 35,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a uncommon shop upgrade.",
                    Chinese = "修改高级升级石的最低价格。"
                }.ToString());
            draftCostUncommonEnhancerMax = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Maximum Cost Uncommon Enhancer", 35,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of an uncommon shop upgrade.",
                    Chinese = "修改高级升级石的最高价格。"
                }.ToString());
            draftCostRareEnhancerMin = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Minimum Cost Rare Enhancer", 80,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a rare shop upgrade.",
                    Chinese = "修改稀有升级石的最低价格。"
                }.ToString());
            draftCostRareEnhancerMax = Config.Bind<int>("Enhancer (Shop Upgrade) Draft Costs", "Maximum Cost Rare Enhancer", 110,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of a rare shop upgrade.",
                    Chinese = "修改稀有升级石的最高价格。"
                }.ToString());

            runHistoryMaxEntries = Config.Bind<int>("Run History Max Entries", "Run History", 100,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum number of entries allowed in Run History."
                }.ToString());
            PatchRunHistory1.RunHistoryMaxEntries = runHistoryMaxEntries.Value;
        }

        private void ReconfigureBalance(BalanceData balanceData)
        {
            var cardTicketValues     =  GetRarityTicket(cardRarityTicketCommon!,        cardRarityTicketUncommon!,      cardRarityTicketRare!,      cardRarityTicketChampion!);
            var enhancerTicketValues =  GetRarityTicket(enhancerRarityTicketCommon!,    enhancerRarityTicketUncommon!,  enhancerRarityTicketRare!,  enhancerRarityTicketChampion!);
            var relicTicketValues    =  GetRarityTicket(relicRarityTicketCommon!,       relicRarityTicketUncommon!,     relicRarityTicketRare!,     relicRarityTicketChampion!);
            var skipRewardValues     =  GetGoldForSkippingRewards();

            SafeSetField<BalanceData>(balanceData, "cardRarityTicketValues",          cardTicketValues);
            SafeSetField<BalanceData>(balanceData, "enhancerRarityTicketValues",      enhancerTicketValues);
            SafeSetField<BalanceData>(balanceData, "relicRarityTicketValues",         relicTicketValues);
            SafeSetField<BalanceData>(balanceData, "goldForSkippingRewards",          skipRewardValues);
            SafeSetField<BalanceData>(balanceData, "startingGold",                    startingGold!.Value);
            SafeSetField<BalanceData>(balanceData, "maxHandSize",                     maxHandSize!.Value);
            SafeSetField<BalanceData>(balanceData, "startOfTurnEnergy",               startOfTurnEnergy!.Value);
            SafeSetField<BalanceData>(balanceData, "startOfDeploymentPhaseEnergy",    startOfDeploymentPhaseEnergy!.Value);
            SafeSetField<BalanceData>(balanceData, "maxEnergy",                       maxEnergy!.Value);
            SafeSetField<BalanceData>(balanceData, "initialDragonsHoardCap",          initialDragonsHoardCap!.Value);
            SafeSetField<BalanceData>(balanceData, "maxDragonsHoard",                 maxDragonsHoard!.Value);
            SafeSetField<BalanceData>(balanceData, "startOfTurnCards",                startOfTurnCards!.Value);
            SafeSetField<BalanceData>(balanceData, "unitUpgradeSlots",                unitUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "spellUpgradeSlots",               spellUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "equipmentUpgradeSlots",           equipmentUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "numSpawnPointsPerFloor",          numSpawnPointsPerFloor!.Value);
            SafeSetField<BalanceData>(balanceData, "characterCapacityPerFloor",       characterCapacityPerFloor!.Value);
            SafeSetField<BalanceData>(balanceData, "maxMutatorCount",                 maxMutatorCount!.Value);
            SafeSetField<BalanceData>(balanceData, "championUpgradesShown",           championUpgradesShown!.Value);
            SafeSetField<BalanceData>(balanceData, "mainClanXpFactor",                mainClanXpFactor!.Value);
            SafeSetField<BalanceData>(balanceData, "subClanXpFactor",                 subClanXpFactor!.Value);
            SafeSetField<BalanceData>(balanceData, "alternateChampionUnlockLevel",    alternateChampionUnlockLevel!.Value);
            SafeSetField<BalanceData>(balanceData, "chanceOfOptionalOutpostDialogue", chanceOfOptionalOutpostDialogue!.Value);
            SafeSetField<BalanceData>(balanceData, "fastDialogueMultiplier",          fastDialogueMultiplier!.Value);


            DraftCost[]? draftCosts = SafeGetField(balanceData, "draftCosts") as DraftCost[];
            if (draftCosts == null)
                return;

            SafeSetField<DraftCost>(draftCosts[0], "costRange", new Vector2Int(draftCostCommonCardMin!.Value,       draftCostCommonCardMax!.Value));
            SafeSetField<DraftCost>(draftCosts[1], "costRange", new Vector2Int(draftCostUncommonCardMin!.Value,     draftCostUncommonCardMax!.Value));
            SafeSetField<DraftCost>(draftCosts[2], "costRange", new Vector2Int(draftCostRareCardMin!.Value,         draftCostRareCardMax!.Value));
            SafeSetField<DraftCost>(draftCosts[3], "costRange", new Vector2Int(draftCostCommonRelicMin!.Value,      draftCostCommonRelicMax!.Value));
            SafeSetField<DraftCost>(draftCosts[4], "costRange", new Vector2Int(draftCostUncommonRelicMin!.Value,    draftCostUncommonRelicMax!.Value));
            SafeSetField<DraftCost>(draftCosts[5], "costRange", new Vector2Int(draftCostRareRelicMin!.Value,        draftCostRareRelicMax!.Value));
            SafeSetField<DraftCost>(draftCosts[6], "costRange", new Vector2Int(draftCostCommonEnhancerMin!.Value,   draftCostCommonEnhancerMax!.Value));
            SafeSetField<DraftCost>(draftCosts[7], "costRange", new Vector2Int(draftCostUncommonEnhancerMin!.Value, draftCostUncommonEnhancerMax!.Value));
            SafeSetField<DraftCost>(draftCosts[8], "costRange", new Vector2Int(draftCostRareEnhancerMin!.Value,     draftCostRareEnhancerMax!.Value));
        }

        /// <summary>
        /// Function to use reflection to set a field on BalanceData without throwing an exception if it fails.
        /// </summary>
        /// <param name="balanceData"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        private void SafeSetField<T>(T? data, string field, object? obj)
        {
            if (data == null)
            {
                Logger.LogError($"Internal Error data is null");
                throw new ArgumentNullException(nameof(data));
            }
            else if (obj == null)
            {
                Logger.LogWarning($"Not setting {typeof(T).Name} field {field} because the value to set is null (value specified may be invalid or field not present)");
                return;
            }
            try
            {
                Logger.LogDebug($"Setting {typeof(T).Name} field {field} to {obj}");
                AccessTools.Field(data.GetType(), field).SetValue(data, obj);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not set {typeof(T).Name} field {field} because of an exception {ex.Message}");
            }
        }

        /// Function to use reflection to set a field on BalanceData without throwing an exception if it fails.
        /// </summary>
        /// <param name="balanceData"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        private object? SafeGetField(BalanceData balanceData, string field)
        {
            try
            {
                Logger.LogDebug($"Getting BalanceData field {field}");
                return AccessTools.Field(balanceData.GetType(), field).GetValue(balanceData);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not get BalanceData field {field} because of an exception {ex.Message}");
            }
            return null;
        }

        private List<RarityTicket>? GetRarityTicket(ConfigEntry<int> common, ConfigEntry<int> uncommon, ConfigEntry<int> rare, ConfigEntry<int> champion)
        {
            return [
                new RarityTicket {rarityType = CollectableRarity.Common,    ticketValue = common.Value},
                new RarityTicket {rarityType = CollectableRarity.Uncommon,  ticketValue = uncommon.Value},
                new RarityTicket {rarityType = CollectableRarity.Rare,      ticketValue = rare.Value},
                new RarityTicket {rarityType = CollectableRarity.Champion,  ticketValue = champion.Value},
            ];
        }

        private GoldForSkippingRewardsData? GetGoldForSkippingRewards()
        {
            return new GoldForSkippingRewardsData
            {
                cardDraft            = goldSkipCardDraft!.Value,
                blessingDraft        = goldSkipRelicDraft!.Value,
                championUpgradeDraft = goldSkipChampionUpgrade!.Value,
                individualBlessing   = goldSkipIndividualRelic!.Value,
                individualCard       = goldSkipIndividualCard!.Value,
                purge                = goldSkipPurge!.Value,
                levelUpUnit          = goldSkipLevelUpUnit!.Value
            };
        }
    }

    // Patch to increase Run History limit
    [HarmonyPatch(typeof(SteamPlatformServices), nameof(SteamPlatformServices.CreateEmptyRunHistory))]
    class PatchRunHistory1
    {
        public static int RunHistoryMaxEntries;
        public static void Postfix(ref IRunHistoryData __result)
        {
            __result = new RunHistoryDataJson(RunHistoryMaxEntries);
        }
    }

    // This patch is here just in case. SteamPlatformServices patch should be good enough but if for some reason not on Steam then catch that too.
    [HarmonyPatch(typeof(StandalonePlatformServices), nameof(StandalonePlatformServices.CreateEmptyRunHistory))]
    class PatchRunHistory2
    {
        public static void Postfix(ref IRunHistoryData __result)
        {
            __result = new RunHistoryDataJson(PatchRunHistory1.RunHistoryMaxEntries);
        }
    }
}
