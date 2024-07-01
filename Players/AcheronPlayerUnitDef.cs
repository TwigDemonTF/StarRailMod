using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.UIhelpers;
using LBoLEntitySideloader.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
using StarRailMod;


namespace PvB
{
    public sealed class AcheronPlayerUnitDef : PlayerUnitTemplate
    {
        public static DirectorySource dir = new DirectorySource(PluginInfo.GUID, "Acheron");

        public static string name = nameof(AcheronPlayerUnit);

        public override IdContainer GetId() => nameof(AcheronPlayerUnit);

        public override LocalizationOption LoadLocalization()
        {
        #pragma warning disable
            {
            var loc = new GlobalLocalization(StarRailMod.BepinexPlugin.embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "PlayerUnitEn.yaml");
            return loc;
            }
        #pragma warning restore
        }


        public override PlayerImages LoadPlayerImages()
        {
            var sprites = new PlayerImages();

            var asyncLoading = ResourceLoader.LoadSpriteAsync("AcheronSprite.png", dir);

            sprites.SetStartPanelStand(asyncLoading);
            sprites.SetWinStand(asyncLoading);
            sprites.SetDeckStand(asyncLoading);

            return sprites;
        }

        public override PlayerUnitConfig MakeConfig()
        {
            var config = new PlayerUnitConfig(
            Id: "",
            ShowOrder: 6,
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#e58c27",
            IsSelectable: true,
            MaxHp: 100,
            InitialMana: new LBoL.Base.ManaGroup() { Red = 2, Black = 2 },
            InitialMoney: 100,
            InitialPower: 0,
            UltimateSkillA: "SakuyaUltU",
            UltimateSkillB: "SakuyaUltU",
            ExhibitA: "",
            ExhibitB: "",
            DeckA: new List<string> { "Acheron" },
            DeckB: new List<string> { "Acheron" },
            DifficultyA: 3,
            DifficultyB: 3
            );
            return config;
        }


        [EntityLogic(typeof(AcheronPlayerUnitDef))]
        public sealed class AcheronPlayerUnit : PlayerUnit { 

        }

    }

    public sealed class AcheronPlayerUnitModelDef : UnitModelTemplate
    {


        public override IdContainer GetId() => new AcheronPlayerUnitDef().UniqueId;

        public override LocalizationOption LoadLocalization() => new DirectLocalization(new Dictionary<string, object>() { { "Default", "Acheron" }, { "Short", "Acheron" } });

        public override ModelOption LoadModelOptions()
        {
            // ppu = sprite size, smaller is bigger
            return new ModelOption(ResourceLoader.LoadSpriteAsync("AcheronGameSprite.png", AcheronPlayerUnitDef.dir, ppu: 56));
        }

        // ppu = sprite size, smaller is bigger
        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("AcheronGameSprite.png", AcheronPlayerUnitDef.dir, ppu: 550);


        public override UnitModelConfig MakeConfig()
        {

            var config = UnitModelConfig.FromName("Reimu").Copy();
            config.Flip = true;
            config.Type = 0;
            config.Offset = new Vector2(0, 0.04f);
            return config;

        }
    }
}