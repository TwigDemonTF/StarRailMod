using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.StatusEffects.Basic;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;

namespace StarRailMod {
    public class AcheronDef : CardTemplate
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
            var cardConfig = new CardConfig(
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
                Rarity: Rarity.Mythic,
                Type: CardType.Friend,
                TargetType: TargetType.SingleEnemy,
                Colors: new List<ManaColor>() { ManaColor.Black },
                IsXCost: false,
                Cost: new ManaGroup() { Black = 3 },
                UpgradedCost: null,
                MoneyCost: null,
                Damage: 70,
                UpgradedDamage: 100,
                Block: null,
                UpgradedBlock: null,
                Shield: null,
                UpgradedShield: null,
                Value1: null,
                UpgradedValue1: null,
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

                RelativeEffects: new List<string>() { },
                UpgradedRelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { },
                UpgradedRelativeCards: new List<string>() { },
                Owner: "",
                ImageId: null,
                UpgradeImageId: null,
                Unfinished: false,
                Illustrator: "Default",
                SubIllustrator: new List<string>() { "" }
             );
            return cardConfig;

        }


        [EntityLogic(typeof(AcheronDef))]
        public class Acheron : Card {
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

                    if (BattleShouldEnd) {
                        yield break;
                    }
                    num = i;
                }

                bool flag2 = base.Loyalty <= 0;

                if (flag2) {
                    yield return new RemoveCardAction(this);
                }
                //Trigger effects
                yield return BuffAction<SlashedDream>(1);
                Unit target = base.Battle.RandomAliveEnemy;
                yield return DebuffAction<CrimsonKnot>(target, 1);
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

                    yield return new DamageAction(base.Battle.Player, base.Battle.AllAliveEnemies, Damage, GunName, GunType.Single);

                    bool flag2 = Battle.BattleShouldEnd;
                    if (flag2)
                        yield break;

                    // Activate active effects
                }
                else {
                    base.Loyalty += base.UltimateCost;
                    base.UltimateUsed = true;
                    yield return PerformAction.Effect(base.Battle.Player, "Wave1s", 0f, "BirdSing", 0f, PerformAction.EffectBehavior.PlayOneShot, 0f);
                    
                    // Activate ultimate effects
                    DamageInfo AllDamage = new DamageInfo {
                        Damage = 40,
                    };

                    yield return new DamageAction(base.Battle.Player, base.Battle.AllAliveEnemies, AllDamage, GunName, GunType.Single);

                    flag = Battle.BattleShouldEnd;
                    if (flag)
                        yield break;

                    bool enemyHasCrimsonKnot = unitSelector.SelectedEnemy.HasStatusEffect<CrimsonKnot>();
                    if (enemyHasCrimsonKnot) {
                        int CrimsonKnotLevel = unitSelector.SelectedEnemy.GetStatusEffect<CrimsonKnot>().Level;

                        float DamageIncreasepercentage = 0f;

                        switch (CrimsonKnotLevel)
                        {
                            case 1: DamageIncreasepercentage = 1.30f; break;
                            case 2: DamageIncreasepercentage = 1.60f; break;
                            case 3: DamageIncreasepercentage = 1.90f; break;
                        }

                        DamageInfo CrimsonKnotDamage = new DamageInfo {
                            Damage = 70 * DamageIncreasepercentage,
                        };
                        yield return new DamageAction(base.Battle.Player, unitSelector.SelectedEnemy, CrimsonKnotDamage, GunName, GunType.Single);
                    }
                    else
                        yield return new DamageAction(base.Battle.Player, unitSelector.SelectedEnemy, Damage, GunName, GunType.Single);

                    flag = Battle.BattleShouldEnd;
                    if (flag)
                        yield break;

                    foreach (Unit enemy in base.Battle.AllAliveEnemies) {
                        if (enemy.HasStatusEffect<CrimsonKnot>()) {
                            StatusEffect effect = enemy.GetStatusEffect<CrimsonKnot>();
                            yield return new RemoveStatusEffectAction(effect, true, 0.1f);
                        }                    
                    }
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