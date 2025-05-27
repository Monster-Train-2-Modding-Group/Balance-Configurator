using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using I2.Loc;
using Microsoft.Extensions.Configuration;
using ShinyShoe.Logging;
using SimpleInjector;
using TrainworksReloaded.Base;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static BalanceData;

namespace BalanceConfigurator.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            var builder = Railhead.GetBuilder();
            // Give configuration for Trainworks Reloaded to consume
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    // The framework will parse the json file to an IConfiguration object that we can retrieve later
                    // Technically only json files following our official schema should be used here, so avoid conflicts in terms of properties with the official schema
                    // https://github.com/Monster-Train-2-Modding-Group/Trainworks-Reloaded/blob/main/schemas/base.json
                    c.AddMergedJsonFile(
                        "json/settings.json"
                    );
                }
            );

            // The action gets called later after all mods are initialized, json parsed, and injected into the game data.
            // Here I just want the configuration I passed in earlier so I can parse it myself.
            Railend.ConfigurePostAction(
                c =>
                {
                    // PluginAtlas contains a mapping of plugin GUID to a PluginDefinition which contains the parsed configuration, asset directories, and Assembly object from the DLL.
                    var atlas = c.GetInstance<PluginAtlas>();
                    var definition = atlas.PluginDefinitions[MyPluginInfo.PLUGIN_GUID];
                    
                    // GameDataClient has all of the relevant Provider Objects, We get the instance from the SimpleInjector Container object.
                    var client = c.GetInstance<GameDataClient>();

                    // Get a provider (SaveManager) from GameDataClient.
                    if (!client.TryGetProvider<SaveManager>(out var saveManager))
                    {
                        Logger.LogError("Failed to get SaveManager instance please report this https://github.com/Monster-Train-2-Modding-Group/Balance-Configurator/issues");
                        return;
                    }

                    // Do the magic
                    var allGameData = saveManager.GetAllGameData().GetBalanceData();
                    ReconfigureBalance(allGameData, definition.Configuration.GetSection("settings"));
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void ReconfigureBalance(BalanceData balanceData, IConfiguration configuration)
        {
            var cardTicketValues     = ParseRarityTicketValues(configuration,       "cardRarityTicketValues");
            var enhancerTicketValues = ParseRarityTicketValues(configuration,       "enhancerRarityTicketValues");
            var relicTicketValues    = ParseRarityTicketValues(configuration,       "relicRarityTicketValues");
            var skipRewardValues     = ParseGoldForSkippingRewards(configuration,   "goldForSkippingRewards");

            SafeSetField(balanceData, "cardRarityTicketValues",         cardTicketValues);
            SafeSetField(balanceData, "enhancerRarityTicketValues",     enhancerTicketValues);
            SafeSetField(balanceData, "relicRarityTicketValues",        relicTicketValues);
            SafeSetField(balanceData, "goldForSkippingRewards",         skipRewardValues);
            SafeSetField(balanceData, "startingGold",                   configuration.GetSection("startingGold").ParseInt());
            SafeSetField(balanceData, "maxHandSize",                    configuration.GetSection("maxHandSize").ParseInt());
            SafeSetField(balanceData, "startOfTurnEnergy",              configuration.GetSection("startOfTurnEnergy").ParseInt());
            SafeSetField(balanceData, "startOfDeploymentPhaseEnergy",   configuration.GetSection("startOfDeploymentPhaseEnergy").ParseInt());
            SafeSetField(balanceData, "maxEnergy",                      configuration.GetSection("maxEnergy").ParseInt());
            SafeSetField(balanceData, "initialDragonsHoardCap",         configuration.GetSection("initialDragonsHoardCap").ParseInt());
            SafeSetField(balanceData, "maxDragonsHoard",                configuration.GetSection("maxDragonsHoard").ParseInt());
            SafeSetField(balanceData, "startOfTurnCards",               configuration.GetSection("startOfTurnCards").ParseInt());
            SafeSetField(balanceData, "unitUpgradeSlots",               configuration.GetSection("unitUpgradeSlots").ParseInt());
            SafeSetField(balanceData, "spellUpgradeSlots",              configuration.GetSection("spellUpgradeSlots").ParseInt());
            SafeSetField(balanceData, "equipmentUpgradeSlots",          configuration.GetSection("equipmentUpgradeSlots").ParseInt());
            SafeSetField(balanceData, "numSpawnPointsPerFloor",         configuration.GetSection("numSpawnPointsPerFloor").ParseInt());
            SafeSetField(balanceData, "characterCapacityPerFloor",      configuration.GetSection("characterCapacityPerFloor").ParseInt());
            SafeSetField(balanceData, "maxMutatorCount",                configuration.GetSection("maxMutatorCount").ParseInt());
            SafeSetField(balanceData, "championUpgradesShown",          configuration.GetSection("championUpgradesShown").ParseInt());
            SafeSetField(balanceData, "mainClanXpFactor",               configuration.GetSection("mainClanXpFactor").ParseInt());
            SafeSetField(balanceData, "subClanXpFactor",                configuration.GetSection("subClanXpFactor").ParseInt());
            SafeSetField(balanceData, "alternateChampionUnlockLevel",   configuration.GetSection("alternateChampionUnlockLevel").ParseInt());
        }

        /// <summary>
        /// Function to use reflection to set a field on BalanceData without throwing an exception if it fails.
        /// </summary>
        /// <param name="balanceData"></param>
        /// <param name="field"></param>
        /// <param name="obj"></param>
        private void SafeSetField(BalanceData balanceData, string field, object? obj)
        {
            if (obj == null)
            {
                Logger.LogWarning($"Not setting BalanceData field {field} because the value to set is null");
                return;
            }
            try
            {
                Logger.LogDebug($"Setting BalanceData field {field} to {obj}");
                AccessTools.Field(balanceData.GetType(), field).SetValue(balanceData, obj);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Could not set BalanceData field {field} because of an exception {ex.Message}");
            }
        }

        private List<RarityTicket>? ParseRarityTicketValues(IConfiguration settings, string context)
        {
            var configuration = settings.GetSection(context);
            if (!configuration.Exists())
            {
                Logger.LogError($"Field {context} does not exist ignoring");
                return null;
            }
                
            var common   =  configuration.GetSection("common").ParseInt();
            var uncommon =  configuration.GetSection("uncommon").ParseInt();
            var rare     =  configuration.GetSection("rare").ParseInt();
            var champion =  configuration.GetSection("champion").ParseInt();
            if (common == null || uncommon == null || rare == null || champion == null)
            {
                Logger.LogError($"Field {context} does not have all of the rarities specified ignoring.");
                return null;
            }

            return [
                new RarityTicket {rarityType = CollectableRarity.Common, ticketValue = common.Value},
                new RarityTicket {rarityType = CollectableRarity.Uncommon, ticketValue = uncommon.Value},
                new RarityTicket {rarityType = CollectableRarity.Rare, ticketValue = rare.Value},
                new RarityTicket {rarityType = CollectableRarity.Champion, ticketValue = champion.Value},
                ];
        }

        private GoldForSkippingRewardsData? ParseGoldForSkippingRewards(IConfiguration settings, string context)
        {
            var configuration = settings.GetSection(context);
            if (!configuration.Exists())
            {
                Logger.LogError($"Field {context} does not exist ignoring");
                return null;
            }

            var cardDraft            = configuration.GetSection("cardDraft").ParseInt();
		    var blessingDraft        = configuration.GetSection("blessingDraft").ParseInt();
            var championUpgradeDraft = configuration.GetSection("championUpgradeDraft").ParseInt();
            var individualBlessing   = configuration.GetSection("individualBlessing").ParseInt();
            var individualCard       = configuration.GetSection("individualCard").ParseInt();
            var purge                = configuration.GetSection("purge").ParseInt();
            var levelUpUnit          = configuration.GetSection("levelUpUnit").ParseInt();

            if (cardDraft == null || blessingDraft == null || championUpgradeDraft == null || individualBlessing == null || individualCard == null || purge == null || levelUpUnit == null)
            {
                Logger.LogError($"Field {context} does not have all of the values specified ignoring.");
                return null;
            }

            return new GoldForSkippingRewardsData
            {
                cardDraft = cardDraft.Value,
                blessingDraft = blessingDraft.Value,
                championUpgradeDraft = championUpgradeDraft.Value,
                individualBlessing = individualBlessing.Value,
                individualCard = individualCard.Value,
                purge = purge.Value,
                levelUpUnit = levelUpUnit.Value
            };
        }
    }
}
