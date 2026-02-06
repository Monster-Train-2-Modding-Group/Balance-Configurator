using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Malee;
using ShinyShoe;
using System.Text;
using TrainworksReloaded.Base;
using TrainworksReloaded.Core;
using UnityEngine;
using static BalanceData;
using static DeckScreen;
using static PoolRewardData;

namespace BalanceConfigurator.Plugin
{
    enum ConfigSortOption
    {
        DoNotOverrideDeckSort,
        Default,
        Name,
        CardType,
        Ember,
        Upgrades,
        Rarity
    }

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
        ConfigEntry<int>? unitMaxUpgradeSlots;
        ConfigEntry<int>? spellMaxUpgradeSlots;
        ConfigEntry<int>? equipmentMaxUpgradeSlots;
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
        ConfigEntry<int>? draftCostChampionRelicMin;
        ConfigEntry<int>? draftCostChampionRelicMax;

        ConfigEntry<int>? draftCostCommonEnhancerMin;
        ConfigEntry<int>? draftCostCommonEnhancerMax;
        ConfigEntry<int>? draftCostUncommonEnhancerMin;
        ConfigEntry<int>? draftCostUncommonEnhancerMax;
        ConfigEntry<int>? draftCostRareEnhancerMin;
        ConfigEntry<int>? draftCostRareEnhancerMax;

        // Event ticket counts
        ConfigEntry<int>? balatroEvent;
        ConfigEntry<int>? boneDogTitan;
        ConfigEntry<int>? classPotionsTitan;
        ConfigEntry<int>? classSpikes;
        ConfigEntry<int>? classTomes;
        ConfigEntry<int>? clippedWings;
        ConfigEntry<int>? copier;
        ConfigEntry<int>? corruptedAngels;
        ConfigEntry<int>? cultOfTheLambEvent;
        ConfigEntry<int>? danteReturns;
        ConfigEntry<int>? garfieldBoxes;
        ConfigEntry<int>? inkboundEvent;
        ConfigEntry<int>? inscryptionEvent;
        ConfigEntry<int>? lazarusLeagueLab;
        ConfigEntry<int>? mothersRemnant;
        ConfigEntry<int>? moreOrLessArtifacts;
        ConfigEntry<int>? nonclassCards;
        ConfigEntry<int>? pathOfTheAngel;
        ConfigEntry<int>? penanceFields;
        ConfigEntry<int>? purgeChampion;
        ConfigEntry<int>? purifyingFlame;
        ConfigEntry<int>? pyreHeartUpgrade;
        ConfigEntry<int>? theOldOrder;
        ConfigEntry<int>? titanDominion;
        ConfigEntry<int>? titanEntropy;
        ConfigEntry<int>? titanSavagery;
        ConfigEntry<int>? trainRoomInstall;

        // Banner Drafts
        ConfigEntry<bool>? shatteredHaloAffectsBanners;
        ConfigEntry<uint>? numberOfBannerDraftCards;

        // Soul Savior
        ConfigEntry<int>? unlocksForRunStartSelectButton2;
        ConfigEntry<int>? unlocksForRunStartSelectButton3;
        ConfigEntry<int>? draftRerollCount;
        ConfigEntry<int>? draftRerollUnlocksForBonus1;
        ConfigEntry<int>? draftRerollUnlocksForBonus2;
        ConfigEntry<int>? draftRerollUnlocksForBonus3;

        // Miscellaneous
        ConfigEntry<bool>? eliminateRunRarityFloor;
        ConfigEntry<bool>? eliminateRarityFloorArmsShop;
        //ConfigEntry<bool>? allowPurgingChampionAtUnstableVortex;
        ConfigEntry<bool>? allowCardMasteryForAllRunTypes;
        ConfigEntry<ConfigSortOption>? deckSortDefaultOption;
        ConfigEntry<bool>? persistentDeckSort;
        ConfigEntry<bool>? defaultDeployableSort;

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
                    ReconfigureBalance(allGameData);
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
            unitUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Unit Initial Upgrade Slots", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of unit upgrade slots available.",
                    Chinese = "修改单位的初始升级栏位数量。"
                }.ToString());
            unitMaxUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Unit Max Upgrade Slots", 4,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum amount of unit upgrade slots.",
                    Chinese = ""
                }.ToString());
            spellUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Spell Initial Upgrade Slots", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of spell upgrade slots available.",
                    Chinese = "修改法术的初始升级栏位数量。"
                }.ToString());
            spellMaxUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Spell Max Upgrade Slots", 4,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum amount of spell upgrade slots.",
                    Chinese = ""
                }.ToString());
            equipmentUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Equipment Initial Upgrade Slots", 2,
                new ConfigDescriptionBuilder
                {
                    English = "Initial amount of equipment upgrade slots available.",
                    Chinese = "修改装备的初始升级栏位数量（目前装备无法升级）。"
                }.ToString());
            equipmentMaxUpgradeSlots = Config.Bind<int>("Upgrades and Slots", "Equipment Max Upgrade Slots", 4,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum amount of equipment upgrade slots.",
                    Chinese = ""
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
                    Chinese = "修改在一轮游戏结束之后，契约前哨站可选对话的出现概率百分比。"
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
                    Chinese = ""
                }.ToString());
            draftCostChampionRelicMin = Config.Bind<int>("Relic Draft Costs", "Minimum Cost Champion Relic", 600,
                new ConfigDescriptionBuilder
                {
                    English = "Minimum cost of a champion artifact (boss relics in soul savior mode).",
                    Chinese = "（未使用）修改稀有神器的最低价格。"
                }.ToString());
            draftCostChampionRelicMax = Config.Bind<int>("Relic Draft Costs", "Maximum Cost Champion Relic", 600,
                new ConfigDescriptionBuilder
                {
                    English = "Maximum cost of a rare artifact (boss relics in soul savior mode).",
                    Chinese = ""
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
                    English = "Maximum number of entries allowed in Run History.",
                    Chinese = "修改历史记录的条目数量上限。"
                }.ToString());
            PatchRunHistory1.RunHistoryMaxEntries = runHistoryMaxEntries.Value;

            deckSortDefaultOption = Config.Bind<ConfigSortOption>("Deck Options", "Default Sort Option", ConfigSortOption.DoNotOverrideDeckSort,
                new ConfigDescriptionBuilder
                {
                    English = "Default sort option for the Deck. The default does not override the default sorting method of the deck.",
                    Chinese = "修改卡组默认采用哪种排序方式。不会修改游戏本体“默认”选项对应的排序方式。可供选择的选项分别为：该功能不生效、默认、名字、卡牌类型、余烬费用、升级、稀有度。"
                }.ToString());
            PatchDeckScreen.OverrideSortMode = deckSortDefaultOption.Value;

            persistentDeckSort = Config.Bind<bool>("Deck Options", "Persistent Deck Sort", false,
                new ConfigDescriptionBuilder
                {
                    English = "The selected deck sort option persists, the game remembers the last selected deck sort option and will store it in \"Default Sort Option\"",
                    Chinese = "修改游戏本体“默认”选项对应的卡组排序方式。如果启用，则修改为上次选择的卡组排序方式。"
                }.ToString());
            PatchDeckScreenPersistSort.config = deckSortDefaultOption;
            PatchDeckScreenPersistSort.SortOptionShouldPersist = persistentDeckSort.Value;

            defaultDeployableSort = Config.Bind<bool>("Deck Options", "Change Default Sort to Deployment Hand", false,
                new ConfigDescriptionBuilder
                {
                    English = "The \"Default\" (in game) sort option now sorts the deck by Deployment Hand (total attack by each priority) then alphabetically",
                    Chinese = "修改游戏本体“默认”选项对应的卡组排序方式。如果启用，则将修改为可部署卡牌在前。"
                }.ToString());
            DeckScreenHandleDeployableSort.DefaultIsDeployableSort = defaultDeployableSort.Value;

            // story ticket counts
            ConfigDescription genericDescription = new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = $"Number of tickets that will produce this event.",
                    Chinese = "修改这个事件出现的权重。"
                }.ToString(), new AcceptableValueRange<int>(0, 100));

            cultOfTheLambEvent  = Config.Bind<int>("Story Ticket Counts", "Cult of the Lamb",        7, genericDescription);
            pathOfTheAngel      = Config.Bind<int>("Story Ticket Counts", "Path of the Angel",      10, genericDescription);
            penanceFields       = Config.Bind<int>("Story Ticket Counts", "Penitent Suits",         10, genericDescription);
            
            boneDogTitan        = Config.Bind<int>("Story Ticket Counts", "Bone Dog",               10, genericDescription);
            moreOrLessArtifacts = Config.Bind<int>("Story Ticket Counts", "Historian Records",      10, genericDescription);
            pyreHeartUpgrade    = Config.Bind<int>("Story Ticket Counts", "Divine Shards",          10, genericDescription);
            trainRoomInstall    = Config.Bind<int>("Story Ticket Counts", "Heph Blueprints",        10, genericDescription);
            clippedWings        = Config.Bind<int>("Story Ticket Counts", "Clipped Wings",          10, genericDescription);
            danteReturns        = Config.Bind<int>("Story Ticket Counts", "Dante Returns",          10, genericDescription);
            classPotionsTitan   = Config.Bind<int>("Story Ticket Counts", "Gremlin Looter",         10, genericDescription);
            lazarusLeagueLab    = Config.Bind<int>("Story Ticket Counts", "Plague Doctor",          10, genericDescription);
            corruptedAngels     = Config.Bind<int>("Story Ticket Counts", "Archus",                 10, genericDescription);
            classTomes          = Config.Bind<int>("Story Ticket Counts", "Library",                15, genericDescription);
            inscryptionEvent    = Config.Bind<int>("Story Ticket Counts", "Inscryption",            15, genericDescription);
            classSpikes         = Config.Bind<int>("Story Ticket Counts", "Railspikes",             15, genericDescription);
            titanDominion       = Config.Bind<int>("Story Ticket Counts", "Fountain of Dominion",   15, genericDescription);
            titanSavagery       = Config.Bind<int>("Story Ticket Counts", "Savagery Statue",        15, genericDescription);
            titanEntropy        = Config.Bind<int>("Story Ticket Counts", "Entropy Crystals",       15, genericDescription);
            inkboundEvent       = Config.Bind<int>("Story Ticket Counts", "Inkbound",               15, genericDescription);
            nonclassCards       = Config.Bind<int>("Story Ticket Counts", "Wreckage Remains",       20, genericDescription);
            copier              = Config.Bind<int>("Story Ticket Counts", "Mysterious Mirror",      25, genericDescription);
            garfieldBoxes       = Config.Bind<int>("Story Ticket Counts", "Wondrous Boxes",         25, genericDescription);
            balatroEvent        = Config.Bind<int>("Story Ticket Counts", "Balatro",                25, genericDescription);
            mothersRemnant      = Config.Bind<int>("Story Ticket Counts", "Mothers Remnant",        10, genericDescription);
            theOldOrder         = Config.Bind<int>("Story Ticket Counts", "The Old Order",          10, genericDescription);
            purifyingFlame      = Config.Bind<int>("Story Ticket Counts", "Purifying Flame",        10, genericDescription);
            purgeChampion       = Config.Bind<int>("Story Ticket Counts", "Purge Champion",         10, genericDescription);

            // Banner Drafts
            shatteredHaloAffectsBanners = Config.Bind<bool>("Unit Banner Drafts", "Shattered Halo Applies To Banner Drafts", false,
                new ConfigDescriptionBuilder
                {
                    English = "Force Shattered Halo to apply to Banner Drafts as well as Card Drafts.",
                    Chinese = "修改破碎光环是否也对自选战旗生效。"
                }.ToString());

            numberOfBannerDraftCards = Config.Bind<uint>("Unit Banner Drafts", "Number of Cards Offered For Unit Banner Drafts", 2,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Number of unit cards offered for a clan banner map node.",
                    Chinese = "修改自选战旗出现的单位数量。"
                }.ToString(), new AcceptableValueRange<uint>(1u, 3u)));

            // Card Drafts
            eliminateRunRarityFloor = Config.Bind<bool>("Card Drafts", "Allow Common Cards After Ring 2", false,
                new ConfigDescriptionBuilder
                {
                    English = "Eliminates the rarity floor for card drafts for battle rewards after ring 2. Highly recommended to adjust the Card Rarity Ticket Counts and lower the common rarity ticket count if you enable this.",
                    Chinese = "修改从3层开始是否会出现普通卡牌。如果启用，强烈建议修改各稀有度卡牌的出现权重，尤其是普通卡牌的出现权重。"
                }.ToString());

            // Arms Shop
            eliminateRarityFloorArmsShop = Config.Bind<bool>("Arms shop", "Allow Sale of Common Cards", false,
                new ConfigDescriptionBuilder
                {
                    English = "Allows common rarity equipment and room cards to be sold at the arms shop.",
                    Chinese = "修改武器商人是否会售卖普通稀有度的装备或房间卡。"
                }.ToString());

            allowCardMasteryForAllRunTypes = Config.Bind<bool>("Card Mastery", "Allow Cards to be Mastered From Any Run Type", false,
                new ConfigDescriptionBuilder
                {
                    English = "Allows cards to be mastered in Dimensional Challenges, Custom Runs, and Community Challenges.",
                    Chinese = "修改位面挑战、自定义游戏和社区挑战是否也会精通卡牌。"
                }.ToString());
            PatchCardMastery.OverrideCardMasteryRuns = allowCardMasteryForAllRunTypes.Value;

            unlocksForRunStartSelectButton2 = Config.Bind<int>("Soul Savior Options", "Upgrade Selection 1", 11,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Soul Unlocks Required For 2 At Run Start.",
                    Chinese = ""
                }.ToString(), new AcceptableValueRange<int>(2, 33)));

            unlocksForRunStartSelectButton3 = Config.Bind<int>("Soul Savior Options", "Upgrade Selection 2", 33,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Soul Unlocks Required For 3 At Run Start.",
                    Chinese = ""
                }.ToString(), new AcceptableValueRange<int>(3, 33)));

            draftRerollCount = Config.Bind<int>("Soul Savior Options", "Draft Reroll Count", 1,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Base number of rerolls for soul drafting.",
                    Chinese = ""
                }.ToString()));

            draftRerollUnlocksForBonus1 = Config.Bind<int>("Soul Savior Options", "Upgrade Rerolls 1", 5,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Number of souls unlocked to get 1 extra reroll.",
                    Chinese = ""
                }.ToString()));

            draftRerollUnlocksForBonus2 = Config.Bind<int>("Soul Savior Options", "Upgrade Rerolls 2", 15,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Number of souls unlocked to get 2 extra rerolls.",
                    Chinese = ""
                }.ToString()));

            draftRerollUnlocksForBonus3 = Config.Bind<int>("Soul Savior Options", "Upgrade Rerolls 3", 25,
                new ConfigDescription(new ConfigDescriptionBuilder
                {
                    English = "Number of souls unlocked to get 3 extra rerolls.",
                    Chinese = ""
                }.ToString()));
        }

        private void ReconfigureBalance(AllGameData allGameData)
        {
            var balanceData = allGameData.GetBalanceData();

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
            SafeSetField<BalanceData>(balanceData, "unitMaxUpgradeSlots",             unitMaxUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "spellUpgradeSlots",               spellUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "spellMaxUpgradeSlots",            spellMaxUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "equipmentUpgradeSlots",           equipmentUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "equipmentMaxUpgradeSlots",        equipmentMaxUpgradeSlots!.Value);
            SafeSetField<BalanceData>(balanceData, "numSpawnPointsPerFloor",          numSpawnPointsPerFloor!.Value);
            SafeSetField<BalanceData>(balanceData, "characterCapacityPerFloor",       characterCapacityPerFloor!.Value);
            SafeSetField<BalanceData>(balanceData, "maxMutatorCount",                 maxMutatorCount!.Value);
            SafeSetField<BalanceData>(balanceData, "championUpgradesShown",           championUpgradesShown!.Value);
            SafeSetField<BalanceData>(balanceData, "mainClanXpFactor",                mainClanXpFactor!.Value);
            SafeSetField<BalanceData>(balanceData, "subClanXpFactor",                 subClanXpFactor!.Value);
            SafeSetField<BalanceData>(balanceData, "alternateChampionUnlockLevel",    alternateChampionUnlockLevel!.Value);
            SafeSetField<BalanceData>(balanceData, "chanceOfOptionalOutpostDialogue", chanceOfOptionalOutpostDialogue!.Value);
            SafeSetField<BalanceData>(balanceData, "fastDialogueMultiplier",          fastDialogueMultiplier!.Value);


            DraftCost[]? draftCosts = SafeGetField<BalanceData>(balanceData, "draftCosts") as DraftCost[];
            if (draftCosts != null)
            {
                SafeSetField<DraftCost>(draftCosts[0], "costRange", new Vector2Int(draftCostCommonCardMin!.Value, draftCostCommonCardMax!.Value));
                SafeSetField<DraftCost>(draftCosts[1], "costRange", new Vector2Int(draftCostUncommonCardMin!.Value, draftCostUncommonCardMax!.Value));
                SafeSetField<DraftCost>(draftCosts[2], "costRange", new Vector2Int(draftCostRareCardMin!.Value, draftCostRareCardMax!.Value));
                SafeSetField<DraftCost>(draftCosts[3], "costRange", new Vector2Int(draftCostCommonRelicMin!.Value, draftCostCommonRelicMax!.Value));
                SafeSetField<DraftCost>(draftCosts[4], "costRange", new Vector2Int(draftCostUncommonRelicMin!.Value, draftCostUncommonRelicMax!.Value));
                SafeSetField<DraftCost>(draftCosts[5], "costRange", new Vector2Int(draftCostRareRelicMin!.Value, draftCostRareRelicMax!.Value));
                SafeSetField<DraftCost>(draftCosts[6], "costRange", new Vector2Int(draftCostCommonEnhancerMin!.Value, draftCostCommonEnhancerMax!.Value));
                SafeSetField<DraftCost>(draftCosts[7], "costRange", new Vector2Int(draftCostUncommonEnhancerMin!.Value, draftCostUncommonEnhancerMax!.Value));
                SafeSetField<DraftCost>(draftCosts[8], "costRange", new Vector2Int(draftCostRareEnhancerMin!.Value, draftCostRareEnhancerMax!.Value));
                SafeSetField<DraftCost>(draftCosts[9], "costRange", new Vector2Int(draftCostChampionRelicMin!.Value, draftCostChampionRelicMax!.Value));
            }

            BalanceData.RegionRunSoulOptionsData regionRunSoulOptionsData = new()
            {
                unlocksForRunStartSelectButton2 = unlocksForRunStartSelectButton2!.Value,
                unlocksForRunStartSelectButton3 = unlocksForRunStartSelectButton3!.Value,
                draftRerollCount = draftRerollCount!.Value,
                draftRerollUnlocksForBonus1 = draftRerollUnlocksForBonus1!.Value,
                draftRerollUnlocksForBonus2 = draftRerollUnlocksForBonus2!.Value,
                draftRerollUnlocksForBonus3 = draftRerollUnlocksForBonus3!.Value
            };
            SafeSetField<BalanceData>(balanceData, "regionRunSoulOptions", regionRunSoulOptionsData);

            IReadOnlyDictionary<string, ConfigEntry<int>?> storyConfig = new Dictionary<string, ConfigEntry<int>?> {
                ["CultOfTheLambEvent"] = cultOfTheLambEvent,
                ["PenanceFields"] = penanceFields,
                ["PathOfTheAngel"] = pathOfTheAngel,
                ["BoneDogTitan"] = boneDogTitan,
                ["MoreOrLessArtifacts"] = moreOrLessArtifacts,
                ["PyreHeartUpgrade"] = pyreHeartUpgrade,
                ["TrainRoomInstall"] = trainRoomInstall,
                ["ClippedWings"] = clippedWings,
                ["DanteReturns"] = danteReturns,
                ["ClassPotionsTitan"] = classPotionsTitan,
                ["LazarusLeagueLab"] = lazarusLeagueLab,
                ["CorruptedAngels"] = corruptedAngels,
                ["ClassTomes"] = classTomes,
                ["InscryptionEvent"] = inscryptionEvent,
                ["ClassSpikes"] = classSpikes,
                ["TitanDominion"] = titanDominion,
                ["TitanSavagery"] = titanSavagery,
                ["TitanEntropy"] = titanEntropy,
                ["InkboundEvent"] = inkboundEvent,
                ["NonclassCards"] = nonclassCards,
                ["Copier"] = copier,
                ["GarfieldBoxes"] = garfieldBoxes,
                ["BalatroEvent"] = balatroEvent,
                ["PurifyingFlame"] = purifyingFlame,
                ["TheOldOrder"] = theOldOrder,
                ["MothersRemnant1"] = mothersRemnant,
                ["PurgeChampion"] = purgeChampion,
            };

            foreach (var story_option in storyConfig)
            {
                var storyEvent = allGameData.FindStoryEventDataByName(story_option.Key);
                if (storyEvent == null)
                    continue;
                SafeSetField<StoryEventData>(storyEvent, "priorityTicketCount", story_option.Value!.Value);
            }

            // Change Options for Clan Banner Drafts For the Map Nodes
            // This chunk of code required due to custom clans.
            var clanBannerContainer = allGameData.FindMapNodeData("79d643b3-08e5-4114-8d04-feb8723bd49f") as RandomMapDataContainer;
            if (clanBannerContainer != null)
            {
                var list = SafeGetField<RandomMapDataContainer>(clanBannerContainer, "mapNodeDataList") as ReorderableArray<MapNodeData>;
                if (list != null)
                {
                    foreach (var mapNodeData in list)
                    {
                        var clanBanner = mapNodeData as RewardNodeData;
                        if (clanBanner == null || !clanBanner.GetIsBannerNode())
                            continue;
                        // Do not use GetRewards as its not the same data as of the Echoes update.
                        var rewards = SafeGetField<RewardNodeData>(clanBanner, "rewards") as List<RewardData>;
                        if (rewards == null)
                            continue;
                        foreach (var reward in rewards)
                        {
                            if (reward is not DraftRewardData draftRewardData)
                            {
                                continue;
                            }
                            SafeSetField<DraftRewardData>(draftRewardData, "ignoreRelicRarityOverride", !shatteredHaloAffectsBanners!.Value);
                            SafeSetField<DraftRewardData>(draftRewardData, "draftOptionsCount", numberOfBannerDraftCards!.Value);
                        }
                    }
                }
            }

            // Change Options for Arkion Reward.
            var clanRewardBoss = allGameData.FindRewardDataByName("CardDraftLevelUpUnitMainOrAllied") as DraftRewardData;
            SafeSetField<DraftRewardData>(clanRewardBoss, "ignoreRelicRarityOverride", !shatteredHaloAffectsBanners!.Value);

            // Miscellaneous
            var cardDraftMain = allGameData.FindRewardDataByName("CardDraftMainClassReward") as DraftRewardData;
            SafeSetField<DraftRewardData>(cardDraftMain, "useRunRarityFloors", !eliminateRunRarityFloor!.Value);
            var cardDraftSub = allGameData.FindRewardDataByName("CardDraftSubClassReward") as DraftRewardData;
            SafeSetField<DraftRewardData>(cardDraftSub, "useRunRarityFloors", !eliminateRunRarityFloor!.Value);

            var equipmentMerchant = allGameData.FindMapNodeData(/*EquipmentMerchant*/"e2c67b52-4d52-48b5-b20a-c6f4c12e44fa") as MerchantData;
            var equipmentReward = equipmentMerchant?.GetReward(0).RewardData as CardPoolRewardData;
            var roomReward = equipmentMerchant?.GetReward(1).RewardData as CardPoolRewardData;
            if (eliminateRarityFloorArmsShop!.Value)
            {
                SafeSetField<CardPoolRewardData>(equipmentReward, "rarityFilter", GrantableRarity.Common | GrantableRarity.Uncommon | GrantableRarity.Rare);
                SafeSetField<CardPoolRewardData>(roomReward, "rarityFilter", GrantableRarity.Common | GrantableRarity.Uncommon | GrantableRarity.Rare);
            }
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
        private object? SafeGetField<T>(T? data, string field)
        {
            if (data == null)
            {
                Logger.LogError($"Internal Error data is null");
                throw new ArgumentNullException(nameof(data));
            }
            try
            {
                Logger.LogDebug($"Getting {typeof(T).Name} field {field}");
                return AccessTools.Field(data.GetType(), field).GetValue(data);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not get {typeof(T).Name} field {field} because of an exception {ex.Message}");
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

    [HarmonyPatch(typeof(RunTypeUtil), nameof(RunTypeUtil.AllowCardMastery))]
    class PatchCardMastery
    {
        public static bool OverrideCardMasteryRuns = false;
        public static bool Prefix(ref bool __result)
        {
            if (OverrideCardMasteryRuns)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DeckScreen), "InitSortDropdown")]
    class PatchDeckScreen
    {
        public static ConfigSortOption OverrideSortMode = ConfigSortOption.DoNotOverrideDeckSort;
        private readonly static Dictionary<ConfigSortOption, DeckScreen.SortOrder> SortOrderDictionary = new()
        {
            [ConfigSortOption.Default] = DeckScreen.SortOrder.Default,
            [ConfigSortOption.Name] = DeckScreen.SortOrder.Name,
            [ConfigSortOption.CardType] = DeckScreen.SortOrder.CardType,
            [ConfigSortOption.Ember] = DeckScreen.SortOrder.EmberCost,
            [ConfigSortOption.Upgrades] = DeckScreen.SortOrder.Upgrades,
            [ConfigSortOption.Rarity] = DeckScreen.SortOrder.Rarity
        };

        public static void Prefix(ref DeckScreen.SortOrder ___sortOrder)
        {
            if (OverrideSortMode == ConfigSortOption.DoNotOverrideDeckSort)
                return;

            ___sortOrder = SortOrderDictionary[OverrideSortMode];
        }
    }

    [HarmonyPatch(typeof(DeckScreen), "ChangeSort")]
    class PatchDeckScreenPersistSort
    {
        public static bool SortOptionShouldPersist = false;
        public static ConfigEntry<ConfigSortOption>? config;
        private readonly static Dictionary<DeckScreen.SortOrder, ConfigSortOption> SortOrderDictionary = new()
        {
            [DeckScreen.SortOrder.Default] = ConfigSortOption.Default,
            [DeckScreen.SortOrder.Name] = ConfigSortOption.Name,
            [DeckScreen.SortOrder.CardType] = ConfigSortOption.CardType,
            [DeckScreen.SortOrder.EmberCost] = ConfigSortOption.Ember,
            [DeckScreen.SortOrder.Upgrades] = ConfigSortOption.Upgrades,
            [DeckScreen.SortOrder.Rarity] = ConfigSortOption.Rarity
        };
        public static void Postfix(DeckScreen.SortOrder ___sortOrder)
        {
            if (SortOptionShouldPersist)
            {
                PatchDeckScreen.OverrideSortMode = config!.Value = SortOrderDictionary[___sortOrder];
            }
        }
    }

    [HarmonyPatch(typeof(DeckScreen), "SortCards")]
    class DeckScreenHandleDeployableSort
    {
        public static bool DefaultIsDeployableSort = false;
        public static void Postfix(ref List<DeckScreen.CardInfo> ___cardInfos, RelicManager ___relicManager, SortOrder ___sortOrder)
        {
            if (!DefaultIsDeployableSort)
                return;
            if (___relicManager != null && ___relicManager.GetRelicEffect<ISortDeckRelicEffect>() != null)
                return;
            if (___sortOrder != SortOrder.Default)
                return;

            IOrderedEnumerable<CardInfo> source = ___cardInfos.OrderBy(c =>
                c.purged ? -1 :
                c.cardState.IsChampionCard() ? 0 :
                IsBannerCard(c.cardState) ? 1 :
                IsDeployableCard(c.cardState) ? 2 : 3
            );
            source = source.ThenBy((CardInfo c) => c.cardState.IsCurrentlyDisabled())
             .ThenBy(c => c.cardState.IsChampionCard() ? c.cardState.GetTitle() : null)
             .ThenByDescending(c => IsBannerCard(c.cardState) ? c.cardState.GetTotalAttackDamage() : -1)
             .ThenByDescending(c => IsDeployableCard(c.cardState) ? c.cardState.GetTotalAttackDamage() : -1);

            ___cardInfos = source.ToList();
        }

        public static bool IsBannerCard(CardState card)
        {
            CharacterData? spawnCharacterData = card.GetSpawnCharacterData();
            if (spawnCharacterData != null)
            {
                foreach (SubtypeData subtype in spawnCharacterData.GetSubtypes())
                {
                    if (subtype.Key == "SubtypesData_BannerUnit")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsDeployableCard(CardState card)
        {
            foreach (CardUpgradeState cardUpgrade in card.GetCardStateModifiers().GetCardUpgrades())
            {
                foreach (CardTraitData traitDataUpgrade in cardUpgrade.GetTraitDataUpgrades())
                {
                    if (traitDataUpgrade.GetDrawInDeploymentPhase())
                    {
                        return true;
                    }
                }
            }
            foreach (CardTraitState traitState in card.GetTraitStates())
            {
                if (traitState.DrawOnDeploymentPhase)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
