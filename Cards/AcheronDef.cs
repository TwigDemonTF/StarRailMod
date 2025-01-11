using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks.Triggers;
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
using StarRailMod.Status;

namespace StarRailMod
{
    public sealed class AcheronDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(Acheron);
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
                IsPooled: true,
                FindInBattle: true,
                HideMesuem: false,
                IsUpgradable: true,
                Rarity: Rarity.Rare,
                Type: CardType.Friend,
                TargetType: TargetType.SingleEnemy,
                Colors: new List<ManaColor>() { ManaColor.Black },
                IsXCost: false,
                Cost: new ManaGroup() { Black = 3 }, // Hybrid = 1, HybridColor = {HybridTypeNumber}
                UpgradedCost: null,
                MoneyCost: null,
                Damage: 70,
                UpgradedDamage: 100,
                Block: 0,
                UpgradedBlock: 0,
                Shield: 0,
                UpgradedShield: 0,
                Value1: 0,
                UpgradedValue1: 0,
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
                UltimateCost: -9,
                UpgradedUltimateCost: -9,

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


        [EntityLogic(typeof(AcheronDef))]
        public sealed class Acheron : Card {
            public override IEnumerable<BattleAction> OnTurnEndingInHand() {
                return this.GetPassiveActions();
            }

            public override IEnumerable<BattleAction> GetPassiveActions() {
                base.NotifyActivating();
                // Add loyalty
                base.Loyalty += base.PassiveCost;
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
                yield return BuffAction<SlashedDream>(1);
                yield return DebuffAction<CrimsonKnot>(base.Battle.RandomAliveEnemy, 1);
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
                    
                    StatusEffect SlashedDreamEffect = base.Battle.Player.GetStatusEffect<SlashedDream>(); 

                    // Activate active effects
                    int PlayerMaxHealth = base.Battle.Player.MaxHp;
                    int HealthToHeal = (int)Math.Floor(PlayerMaxHealth * (SlashedDreamEffect.Level / 100f * 5f));

                    yield return HealAction(HealthToHeal);
                    yield return DefenseAction(0, 15, BlockShieldType.Normal, true);
                    yield return new RemoveStatusEffectAction(SlashedDreamEffect);
                    yield break;
                }
                else {
                    base.Loyalty += base.UltimateCost;
                    base.UltimateUsed = true;
                    yield return PerformAction.Effect(base.Battle.Player, "Wave1s", 0f, "BirdSing", 0f, PerformAction.EffectBehavior.PlayOneShot, 0f);

                    if (Battle.BattleShouldEnd)
                        yield break;

                    if (unitSelector.SelectedEnemy.HasStatusEffect<CrimsonKnot>()) {
                        int CrimsonKnotLevel = unitSelector.SelectedEnemy.GetStatusEffect<CrimsonKnot>().Level;
                        StatusEffect crimsonKnotStatusEffect = unitSelector.SelectedEnemy.GetStatusEffect<CrimsonKnot>();
                        float DamageIncreasepercentage = 0f;

                        if (CrimsonKnotLevel >= 9)
                            DamageIncreasepercentage = 1.90f;
                        else if (CrimsonKnotLevel >= 6)
                            DamageIncreasepercentage = 1.60f;
                        else if (CrimsonKnotLevel >= 3)
                            DamageIncreasepercentage = 1.30f;

                        yield return new RemoveStatusEffectAction(crimsonKnotStatusEffect, true);

                        DamageInfo CrimsonKnotDamage = new DamageInfo {
                            Damage = 70 * DamageIncreasepercentage,
                        };

                        yield return new DamageAction(base.Battle.Player, unitSelector.SelectedEnemy, CrimsonKnotDamage, GunName, GunType.Single); 
                    }
                    else
                        yield return new DamageAction(base.Battle.Player, unitSelector.SelectedEnemy, Damage, GunName, GunType.Single);

                    if (Battle.BattleShouldEnd)
                        yield break;
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