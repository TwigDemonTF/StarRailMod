using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks.Triggers;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using static StarRailMod.Cards.KirliaDef;

namespace StarRailMod.Cards
{
    public sealed class RaltsDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(Ralts);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(BepinexPlugin.embeddedSource);
            imgs.AutoLoad(this, extension: ".png");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            #pragma warning disable
                var loc = new GlobalLocalization(BepinexPlugin.embeddedSource);
                loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
                return loc;
            #pragma warning restore
        }

        public override CardConfig MakeConfig()
        {
            return new CardConfig(
                Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
                Id: "",
                Order: 10,
                AutoPerform: true,
                Perform: new string[0][],
                GunName: "Simple1",
                GunNameBurst: "Simple1",
                DebugLevel: 0,
                Revealable: false,
                IsPooled: false,
                FindInBattle: true,
                HideMesuem: false,
                IsUpgradable: true,
                Rarity: Rarity.Rare,
                Type: CardType.Friend,
                TargetType: TargetType.SingleEnemy,
                Colors: new List<ManaColor>() { ManaColor.Any },
                IsXCost: false,
                Cost: new ManaGroup() { Any = 1 }, // Hybrid = 1, HybridColor = {HybridTypeNumber}
                UpgradedCost: null,
                MoneyCost: null,
                Damage: 20,
                UpgradedDamage: 20,
                Block: 0,
                UpgradedBlock: 0,
                Shield: 10,
                UpgradedShield: 10,
                Value1: 10,
                UpgradedValue1: 15,
                Value2: null,
                UpgradedValue2: null,
                Mana: null,
                UpgradedMana: null,
                Scry: null,
                UpgradedScry: null,
                ToolPlayableTimes: null,

                Loyalty: 1,
                UpgradedLoyalty: 1,
                PassiveCost: 1,
                UpgradedPassiveCost: 2,
                ActiveCost: -3,
                UpgradedActiveCost: -3,
                UltimateCost: -5,
                UpgradedUltimateCost: -5,

                Keywords: Keyword.Retain | Keyword.Exile | Keyword.FriendCard | Keyword.Loyalty,
                UpgradedKeywords: Keyword.Retain | Keyword.Exile | Keyword.FriendCard | Keyword.Loyalty,
                EmptyDescription: false,
                RelativeKeyword: Keyword.None,
                UpgradedRelativeKeyword: Keyword.None,

                RelativeEffects: new List<string>() { "CrimsonKnot" },
                UpgradedRelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { },
                UpgradedRelativeCards: new List<string>() { },
                Owner: "AcheronPlayerUnit",
                ImageId: null,
                UpgradeImageId: null,
                Unfinished: false,
                Illustrator: "Default",
                SubIllustrator: new List<string>() { "ALT" }
             );
        }


        [EntityLogic(typeof(RaltsDef))]
        public sealed class Ralts : Card {
            public override IEnumerable<BattleAction> OnTurnEndingInHand() {
                return this.GetPassiveActions();
            }

            public override IEnumerable<BattleAction> GetPassiveActions() {
                base.NotifyActivating();
                int num;
                
                for (int i = 0; i < base.Battle.FriendPassiveTimes; i = num + 1) {
                    bool BattleShouldEnd = base.Battle.BattleShouldEnd;

                    if (BattleShouldEnd) 
                        yield break;
                    num = i;
                }

                bool flag2 = base.Loyalty <= 0;

                if (flag2)
                    yield return new RemoveCardAction(this);

                // * Trigger passive effects
                yield return BuffAction<Firepower>(1);
                yield return BuffAction<Spirit>(1);
                if (base.Loyalty >= 5)
                    yield break;
                base.Loyalty += base.PassiveCost;
                yield break;
            }

            public override IEnumerable<BattleAction> SummonActions(UnitSelector unitSelector, ManaGroup consumingMana, Interaction precondition) {
                foreach (BattleAction battleAction in base.SummonActions(unitSelector, consumingMana, precondition)) {
                    yield return battleAction;
                }

                yield break;
            }

            protected override IEnumerable<BattleAction> Actions(UnitSelector unitSelector, ManaGroup consumingMana, Interaction precondition) {
                bool flag = precondition == null || ((MiniSelectCardInteraction) precondition).SelectedCard.FriendToken == FriendToken.Active;
                if (flag) {
                    base.Loyalty += base.ActiveCost;
                    yield return PerformAction.Effect(base.Battle.Player, "Wave1s", 0f, "BirdSing", 0f, PerformAction.EffectBehavior.PlayOneShot, 0f);
                    
                    // Activate active effects
                    DamageInfo ActiveDamage = new DamageInfo{
                    Damage = Value1
                    };
                    yield return new DamageAction(base.Battle.Player, base.Battle.AllAliveEnemies, ActiveDamage, GunName, GunType.Single);
                    yield break;
                }
                else {
                    base.Loyalty += base.UltimateCost;
                    base.UltimateUsed = true;
                    yield return PerformAction.Effect(base.Battle.Player, "Wave1s", 0f, "BirdSing", 0f, PerformAction.EffectBehavior.PlayOneShot, 0f);

                    // add Ult effects
                    yield return new DamageAction(base.Battle.Player, unitSelector.SelectedEnemy, Damage, GunName, GunType.Single);
                    if (base.Battle.BattleShouldEnd) 
                        yield break;
                    yield return DefenseAction(true);

                    // *  Create kirlia as a card
                    Card kirliaCard = Library.CreateCard<Kirlia>();
                    // Upgrade the card is Ralts is upgraded
                    if (base.IsUpgraded)
                        kirliaCard.IsUpgraded = true;
                    
                    yield return new AddCardsToHandAction(new Card[] { kirliaCard });
                }
                yield break;
            }
        
                public override IEnumerable<BattleAction> AfterUseAction() {
                bool flag = !base.Summoned || base.Battle.BattleShouldEnd;
                if (flag) 
                    yield break;

                bool flag2 = base.Loyalty <= 0 || base.UltimateUsed;
                if (flag2) {
                    yield return new RemoveCardAction(this);
                    yield break;
                }

                yield return new MoveCardAction(this, CardZone.Hand);
                yield break;
            }
        }
    }
}