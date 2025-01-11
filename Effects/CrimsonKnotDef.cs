using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;

namespace StarRailMod.Status
{
    public sealed class CrimsonKnotDef : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(CrimsonKnot);
        }

        public override LocalizationOption LoadLocalization()
        {
            #pragma warning disable
            var loc = new GlobalLocalization(BepinexPlugin.embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
            #pragma warning restore
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("CrimsonKnot.png", BepinexPlugin.embeddedSource, null, 1, null);
        }

        public override StatusEffectConfig MakeConfig()
        {
            return new StatusEffectConfig(
                            Index: BepinexPlugin.sequenceTable.Next(typeof(StatusEffectConfig)),
                            Id: "",
                            Order: 10,
                            Type: StatusEffectType.Negative,
                            IsVerbose: false,
                            IsStackable: true,
                            StackActionTriggerLevel: null,
                            HasLevel: true,
                            LevelStackType: StackType.Add,
                            HasDuration: false,
                            DurationStackType: StackType.Add,
                            DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                            HasCount: false,
                            CountStackType: StackType.Keep,
                            LimitStackType: StackType.Keep,
                            ShowPlusByLimit: false,
                            Keywords: Keyword.None,
                            RelativeEffects: new List<string>() { },
                            VFX: "Default",
                            VFXloop: "Default",
                            SFX: "Default"
                );
        }
    }


    [EntityLogic(typeof(CrimsonKnotDef))]
    public sealed class CrimsonKnot : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            base.OnAdded(unit);
        }
    }
}