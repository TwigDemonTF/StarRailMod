// using System.Collections.Generic;
// using LBoL.Base;
// using LBoL.ConfigData;
// using LBoL.Core;
// using LBoL.Core.Battle;
// using LBoL.Core.Battle.BattleActions;
// using LBoL.Core.Cards;
// using LBoL.Core.StatusEffects;
// using LBoLEntitySideloader;
// using LBoLEntitySideloader.Attributes;
// using LBoLEntitySideloader.Entities;
// using LBoLEntitySideloader.Resource;

// namespace StarRailMod {
//     public sealed class ExampleCardDef : CardTemplate
//     {
//         public override IdContainer GetId()
//         {
//             return nameof(ExampleCard);
//         }

//         public override CardImages LoadCardImages()
//         {
//             var imgs = new CardImages(BepinexPlugin.embeddedSource);
//             imgs.AutoLoad(this, extension: ".png");
//             return imgs;
//         }

//         public override LocalizationOption LoadLocalization()
//         {
//             #pragma warning disable
//                 var loc = new GlobalLocalization(BepinexPlugin.embeddedSource);
//                 loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
//                 return loc;
//             #pragma warning restore
//         }

//         public override CardConfig MakeConfig()
//         {
//             var cardConfig = new CardConfig(
//                 Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
//                 Id: "",
//                 Order: 10,
//                 AutoPerform: true,
//                 Perform: new string[0][],
//                 GunName: "Simple1",
//                 GunNameBurst: "Simple1",
//                 DebugLevel: 0,
//                 Revealable: false,
//                 IsPooled: true,
//                 FindInBattle: true,
//                 HideMesuem: false,
//                 IsUpgradable: true,
//                 Rarity: Rarity.Rare,
//                 Type: CardType.Friend,
//                 TargetType: TargetType.SingleEnemy,
//                 Colors: new List<ManaColor>() { ManaColor.Black },
//                 IsXCost: false,
//                 Cost: new ManaGroup() { Black = 3 },
//                 UpgradedCost: null,
//                 MoneyCost: null,
//                 Damage: 70,
//                 UpgradedDamage: 100,
//                 Block: null,
//                 UpgradedBlock: null,
//                 Shield: null,
//                 UpgradedShield: null,
//                 Value1: null,
//                 UpgradedValue1: null,
//                 Value2: null,
//                 UpgradedValue2: null,
//                 Mana: null,
//                 UpgradedMana: null,
//                 Scry: null,
//                 UpgradedScry: null,
//                 ToolPlayableTimes: null,

//                 Loyalty: 1,
//                 UpgradedLoyalty: 1,
//                 PassiveCost: 1,
//                 UpgradedPassiveCost: 2,
//                 ActiveCost: -3,
//                 UpgradedActiveCost: -3,
//                 UltimateCost: -9,
//                 UpgradedUltimateCost: -9,

//                 Keywords: Keyword.None,
//                 UpgradedKeywords: Keyword.None,
//                 EmptyDescription: false,
//                 RelativeKeyword: Keyword.None,
//                 UpgradedRelativeKeyword: Keyword.None,

//                 RelativeEffects: new List<string>() { },
//                 UpgradedRelativeEffects: new List<string>() { },
//                 RelativeCards: new List<string>() { },
//                 UpgradedRelativeCards: new List<string>() { },
//                 Owner: "",
//                 ImageId: null,
//                 UpgradeImageId: null,
//                 Unfinished: false,
//                 Illustrator: "Default",
//                 SubIllustrator: new List<string>() { "" }
//              );
//             return cardConfig;
//         }
//     }

//     [EntityLogic(typeof(ExampleCardDef))]
//     public sealed class ExampleCard : Card {
//         protected override IEnumerable<BattleAction> Actions(UnitSelector unitSelector, ManaGroup consumingMana, Interaction precondition) {
        
//             yield return BuffAction<Firepower>(5);
//         }
//     }
// }