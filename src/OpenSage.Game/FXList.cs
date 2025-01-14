﻿using System.Collections.Generic;
using System.Numerics;
using OpenSage.Data.Ini;
using OpenSage.Logic.Object;
using OpenSage.Mathematics;
using OpenSage.Terrain;

namespace OpenSage
{
    public sealed class FXList : BaseAsset
    {
        internal static FXList Parse(IniParser parser)
        {
            return parser.ParseNamedBlock(
                (x, name) => x.SetNameAndInstanceId("FXList", name),
                FieldParseTable);
        }

        private static readonly IniParseTable<FXList> FieldParseTable = new IniParseTable<FXList>
        {
            { "AttachedModel", (parser, x) => x.Items.Add(AttachedModelFXNugget.Parse(parser)) },
            { "BuffNugget", (parser, x) => x.Items.Add(BuffNugget.Parse(parser)) },
            { "CameraShakerVolume", (parser, x) => x.Items.Add(CameraShakerVolumeFXNugget.Parse(parser)) },
            { "CursorParticleSystem", (parser, x) => x.Items.Add(CursorParticleSystemFXNugget.Parse(parser)) },
            { "DynamicDecal", (parser, x) => x.Items.Add(DynamicDecalFXNugget.Parse(parser)) },
            { "EvaEvent", (parser, x) => x.Items.Add(EvaEventFXNugget.Parse(parser)) },
            { "FXListAtBonePos", (parser, x) => x.Items.Add(FXListAtBonePosFXListItem.Parse(parser)) },
            { "LightPulse", (parser, x) => x.Items.Add(LightPulseFXListItem.Parse(parser)) },
            { "ParticleSysBone", (parser, x) => x.Items.Add(FXParticleSysBoneNugget.Parse(parser)) },
            { "ParticleSystem", (parser, x) => x.Items.Add(ParticleSystemFXListItem.Parse(parser)) },
            { "Sound", (parser, x) => x.Items.Add(SoundFXListItem.Parse(parser)) },
            { "TerrainScorch", (parser, x) => x.Items.Add(TerrainScorchFXListItem.Parse(parser)) },
            { "TintDrawable", (parser, x) => x.Items.Add(TintDrawableFXNugget.Parse(parser)) },
            { "Tracer", (parser, x) => x.Items.Add(TracerFXListItem.Parse(parser)) },
            { "ViewShake", (parser, x) => x.Items.Add(ViewShakeFXListItem.Parse(parser)) },

            { "PlayEvenIfShrouded", (parser, x) => x.PlayEvenIfShrouded = parser.ParseBoolean() },

            {
                "CullingInfo",
                (parser, x) =>
                {
                    x.CullTracking = parser.ParseAttributeFloat("TrackingSeconds");
                    x.CullTrackingMin = parser.ParseAttributeInteger("StartCullingAbove");
                    x.CullTrackingMax = parser.ParseAttributeInteger("CullAllAbove");
                }
            }
        };

        public List<FXListItem> Items { get; } = new List<FXListItem>();

        public bool PlayEvenIfShrouded { get; private set; }

        public float CullTracking { get; private set; }
        public int CullTrackingMin { get; private set; }
        public int CullTrackingMax { get; private set; }
    }

    public abstract class FXListItem
    {
        internal static readonly IniParseTable<FXListItem> FXListItemFieldParseTable = new IniParseTable<FXListItem>
        {
            { "ExcludedSourceModelConditions", (parser, x) => x.ExcludedSourceModelConditions = parser.ParseEnumBitArray<ModelConditionFlag>() },
            { "RequiredSourceModelConditions", (parser, x) => x.RequiredSourceModelConditions = parser.ParseEnumBitArray<ModelConditionFlag>() },
            { "ObjectFilter", (parser, x) => x.ObjectFilter = ObjectFilter.Parse(parser) },
            { "SourceObjectFilter", (parser, x) => x.SourceObjectFilter = ObjectFilter.Parse(parser) },
            { "StopIfNuggetPlayed", (parser, x) => x.StopIfNuggetPlayed = parser.ParseBoolean() }
        };

        public BitArray<ModelConditionFlag> ExcludedSourceModelConditions { get; private set; }
        public BitArray<ModelConditionFlag> RequiredSourceModelConditions { get; private set; }

        // TODO: What is the difference between ObjectFilter and SourceObjectFilter?
        // BFME I's fxlist.ini uses both.
        public ObjectFilter ObjectFilter { get; private set; }
        public ObjectFilter SourceObjectFilter { get; private set; }

        public bool StopIfNuggetPlayed { get; private set; }
    }

    public sealed class ParticleSystemFXListItem : FXListItem
    {
        internal static ParticleSystemFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<ParticleSystemFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<ParticleSystemFXListItem>
        {
            { "AttachToObject", (parser, x) => x.AttachToObject = parser.ParseBoolean() },
            { "AttachToBone", (parser, x) => x.AttachToBone = parser.ParseBoneName() },
            { "Count", (parser, x) => x.Count = parser.ParseInteger() },
            { "CreateAtGroundHeight", (parser, x) => x.CreateAtGroundHeight = parser.ParseBoolean() },
            { "Height", (parser, x) => x.Height = parser.ParseRandomVariable() },
            { "InitialDelay", (parser, x) => x.InitialDelay = parser.ParseRandomVariable() },
            { "Name", (parser, x) => x.Name = parser.ParseAssetReference() },
            { "Offset", (parser, x) => x.Offset = parser.ParseVector3() },
            { "OrientToObject", (parser, x) => x.OrientToObject = parser.ParseBoolean() },
            { "Radius", (parser, x) => x.Radius = parser.ParseRandomVariable() },
            { "Ricochet", (parser, x) => x.Ricochet = parser.ParseBoolean() },
            { "RotateY", (parser, x) => x.RotateY = parser.ParseInteger() },
            { "UseCallersRadius", (parser, x) => x.UseCallersRadius = parser.ParseBoolean() },
            { "CreateBoneOverride", (parser, x) => x.CreateBoneOverride = parser.ParseBoneName() },
            { "TargetBoneOverride", (parser, x) => x.TargetBoneOverride = parser.ParseBoneName() },
            { "UseTargetOffset", (parser, x) => x.UseTargetOffset = parser.ParseBoolean() },
            { "TargetOffset", (parser, x) => x.TargetOffset = parser.ParseVector3() },
            { "TargetCoeff", (parser, x) => x.TargetCoeff = parser.ParseInteger() },
            { "SystemLife", (parser, x) => x.SystemLife = parser.ParseInteger() },
            { "SetTargetMatrix", (parser, x) => x.SetTargetMatrix = parser.ParseBoolean() },
            { "OnlyIfOnLand", (parser, x) => x.OnlyIfOnLand = parser.ParseBoolean() },
            { "OnlyIfOnWater", (parser, x) => x.OnlyIfOnWater = parser.ParseBoolean() },
            { "Weather", (parser, x) => x.Weather = parser.ParseEnum<WeatherType>() }
        });

        public bool AttachToObject { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public string AttachToBone { get; private set; }

        public int Count { get; private set; } = 1;
        public bool CreateAtGroundHeight { get; private set; }
        public RandomVariable Height { get; private set; }
        public RandomVariable InitialDelay { get; private set; }
        public string Name { get; private set; }
        public Vector3 Offset { get; private set; }
        public bool OrientToObject { get; private set; }
        public RandomVariable Radius { get; private set; }
        public bool Ricochet { get; private set; }
        public int RotateY { get; private set; }
        public bool UseCallersRadius { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public string CreateBoneOverride { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public string TargetBoneOverride { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public bool UseTargetOffset { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public Vector3 TargetOffset { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public int TargetCoeff { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public int SystemLife { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public bool SetTargetMatrix { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public bool OnlyIfOnLand { get; private set; }

        [AddedIn(SageGame.Bfme)]
        public bool OnlyIfOnWater { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public WeatherType Weather { get; private set; }
    }

    public sealed class SoundFXListItem : FXListItem
    {
        internal static SoundFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<SoundFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<SoundFXListItem>
        {
            { "Name", (parser, x) => x.Name = parser.ParseAssetReference() }
        });

        public string Name { get; private set; }
    }

    public sealed class LightPulseFXListItem : FXListItem
    {
        internal static LightPulseFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<LightPulseFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<LightPulseFXListItem>
        {
            { "Color", (parser, x) => x.Color = parser.ParseColorRgb() },
            { "Radius", (parser, x) => x.Radius = parser.ParseInteger() },
            { "RadiusAsPercentOfObjectSize", (parser, x) => x.RadiusAsPercentOfObjectSize = parser.ParsePercentage() },
            { "IncreaseTime", (parser, x) => x.IncreaseTime = parser.ParseInteger() },
            { "DecreaseTime", (parser, x) => x.DecreaseTime = parser.ParseInteger() }
        });

        public ColorRgb Color { get; private set; }
        public int Radius { get; private set; }
        public Percentage RadiusAsPercentOfObjectSize { get; private set; }
        public int IncreaseTime { get; private set; }
        public int DecreaseTime { get; private set; }
    }

    public sealed class ViewShakeFXListItem : FXListItem
    {
        internal static ViewShakeFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<ViewShakeFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<ViewShakeFXListItem>
        {
            { "Type", (parser, x) => x.Type = parser.ParseEnum<ViewShakeType>() }
        });

        public ViewShakeType Type { get; private set; }
    }

    public sealed class FXListAtBonePosFXListItem : FXListItem
    {
        internal static FXListAtBonePosFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<FXListAtBonePosFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<FXListAtBonePosFXListItem>
        {
            { "FX", (parser, x) => x.FX = parser.ParseAssetReference() },
            { "BoneName", (parser, x) => x.BoneName = parser.ParseAssetReference() },
            { "OrientToBone", (parser, x) => x.OrientToBone = parser.ParseBoolean() },
            { "Weather", (parser, x) => x.Weather = parser.ParseAssetReference() }
        });

        public string FX { get; private set; }
        public string BoneName { get; private set; }
        public bool OrientToBone { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public string Weather { get; private set; }
    }

    public sealed class TerrainScorchFXListItem : FXListItem
    {
        internal static TerrainScorchFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<TerrainScorchFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<TerrainScorchFXListItem>
        {
            { "Type", (parser, x) => x.Type = parser.ParseEnum<TerrainScorchType>() },
            { "Radius", (parser, x) => x.Radius = parser.ParseInteger() },
            { "Weather", (parser, x) => x.Weather = parser.ParseEnum<WeatherType>() },
            { "RandomRange", (parser, x) => x.RandomRange = parser.ParseVector2() }
        });

        public TerrainScorchType Type { get; private set; }
        public int Radius { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public WeatherType Weather { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public Vector2 RandomRange { get; private set; }
    }

    public sealed class TracerFXListItem : FXListItem
    {
        internal static TracerFXListItem Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<TracerFXListItem> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<TracerFXListItem>
        {
            { "Color", (parser, x) => x.Color = parser.ParseColorRgb() },
            { "DecayAt", (parser, x) => x.DecayAt = parser.ParseFloat() },
            { "Length", (parser, x) => x.Length = parser.ParseFloat() },
            { "Probability", (parser, x) => x.Probability = parser.ParseFloat() },
            { "Speed", (parser, x) => x.Speed = parser.ParseInteger() },
            { "Width", (parser, x) => x.Width = parser.ParseFloat() },
        });

        public ColorRgb Color { get; private set; }
        public float DecayAt { get; private set; }
        public float Length { get; private set; }
        public float Probability { get; private set; }
        public int Speed { get; private set; }
        public float Width { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class BuffNugget : FXListItem
    {
        internal static BuffNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<BuffNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<BuffNugget>
        {
            { "BuffType", (parser, x) => x.BuffType = parser.ParseIdentifier() },
            { "BuffThingTemplate", (parser, x) => x.BuffThingTemplate = parser.ParseAssetReference() },
            { "BuffInfantryTemplate", (parser, x) => x.BuffInfantryTemplate = parser.ParseAssetReference() },
            { "BuffCavalryTemplate", (parser, x) => x.BuffCavalryTemplate = parser.ParseAssetReference() },
            { "BuffTrollTemplate", (parser, x) => x.BuffTrollTemplate = parser.ParseAssetReference() },
            { "BuffOrcTemplate", (parser, x) => x.BuffOrcTemplate = parser.ParseAssetReference() },
            { "IsComplexBuff", (parser, x) => x.IsComplexBuff = parser.ParseBoolean() },
            { "BuffLifeTime", (parser, x) => x.BuffLifeTime = parser.ParseLong() },
            { "Extrusion", (parser, x) => x.Extrusion = parser.ParseFloat() },
            { "Color", (parser, x) => x.Color = parser.ParseColorRgb() },
            { "BuffShipTemplate", (parser, x) => x.BuffShipTemplate = parser.ParseAssetReference() },
            { "BuffMonsterTemplate", (parser, x) => x.BuffMonsterTemplate = parser.ParseAssetReference() }
        });

        public string BuffType { get; private set; }
        public string BuffThingTemplate { get; private set; }
        public string BuffInfantryTemplate { get; private set; }
        public string BuffCavalryTemplate { get; private set; }
        public string BuffTrollTemplate { get; private set; }
        public string BuffOrcTemplate { get; private set; }
        public bool IsComplexBuff { get; private set; }
        public long BuffLifeTime { get; private set; }
        public float Extrusion { get; private set; }
        public ColorRgb Color { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public string BuffShipTemplate { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public string BuffMonsterTemplate { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class CameraShakerVolumeFXNugget : FXListItem
    {
        internal static CameraShakerVolumeFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<CameraShakerVolumeFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<CameraShakerVolumeFXNugget>
        {
            { "Radius", (parser, x) => x.Radius = parser.ParseInteger() },
            { "Duration_Seconds", (parser, x) => x.Duration = parser.ParseFloat() },
            { "Amplitude_Degrees", (parser, x) => x.Amplitude = parser.ParseFloat() }
        });

        public int Radius { get; private set; }
        public float Duration { get; private set; }
        public float Amplitude { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class DynamicDecalFXNugget : FXListItem
    {
        internal static DynamicDecalFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<DynamicDecalFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<DynamicDecalFXNugget>
        {
            { "DecalName", (parser, x) => x.DecalName = parser.ParseAssetReference() },
            { "Size", (parser, x) => x.Size = parser.ParseInteger() },
            { "Color", (parser, x) => x.Color = parser.ParseColorRgb() },
            { "Offset", (parser, x) => x.Offset = parser.ParseVector2() },

            { "OpacityStart", (parser, x) => x.OpacityStart = parser.ParseInteger() },
            { "OpacityFadeTimeOne", (parser, x) => x.OpacityFadeTimeOne = parser.ParseInteger() },
            { "OpacityPeak", (parser, x) => x.OpacityPeak = parser.ParseInteger() },
            { "OpacityPeakTime", (parser, x) => x.OpacityPeakTime = parser.ParseInteger() },
            { "OpacityFadeTimeTwo", (parser, x) => x.OpacityFadeTimeTwo = parser.ParseInteger() },
            { "OpacityEnd", (parser, x) => x.OpacityEnd = parser.ParseInteger() },

            { "StartingDelay", (parser, x) => x.StartingDelay = parser.ParseInteger() },
            { "Lifetime", (parser, x) => x.Lifetime = parser.ParseInteger() },
            { "Shader", (parser, x) => x.Shader = parser.ParseEnum<ShaderType>() }
        });

        public string DecalName { get; private set; }
        public int Size { get; private set; }
        public ColorRgb Color { get; private set; }
        public Vector2 Offset { get; private set; }

        public int OpacityStart { get; private set; }
        public int OpacityFadeTimeOne { get; private set; }
        public int OpacityPeak { get; private set; }
        public int OpacityPeakTime { get; private set; }
        public int OpacityFadeTimeTwo { get; private set; }
        public int OpacityEnd { get; private set; }

        public int StartingDelay { get; private set; }
        public int Lifetime { get; private set; }

        [AddedIn(SageGame.Bfme2)]
        public ShaderType Shader { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class EvaEventFXNugget : FXListItem
    {
        internal static EvaEventFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<EvaEventFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<EvaEventFXNugget>
        {
            { "EvaEventOwner", (parser, x) => x.EvaEventOwner = parser.ParseAssetReference() },
            { "EvaEventAlly", (parser, x) => x.EvaEventAlly = parser.ParseAssetReference() }
        });

        public string EvaEventOwner { get; private set; }
        public string EvaEventAlly { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class TintDrawableFXNugget : FXListItem
    {
        internal static TintDrawableFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<TintDrawableFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<TintDrawableFXNugget>
        {
            { "Color", (parser, x) => x.Color = parser.ParseColorRgb() },
            { "PreColorTime", (parser, x) => x.PreColorTime = parser.ParseInteger() },
            { "PostColorTime", (parser, x) => x.PostColorTime = parser.ParseInteger() },
            { "SustainedColorTime", (parser, x) => x.SustainedColorTime = parser.ParseInteger() },
            { "Frequency", (parser, x) => x.Frequency = parser.ParseFloat() },
            { "Amplitude", (parser, x) => x.Amplitude = parser.ParseFloat() },
        });

        public ColorRgb Color { get; private set; }
        public int PreColorTime { get; private set; }
        public int PostColorTime { get; private set; }
        public int SustainedColorTime { get; private set; }
        public float Frequency { get; private set; }
        public float Amplitude { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class FXParticleSysBoneNugget : FXListItem
    {
        internal static FXParticleSysBoneNugget Parse(IniParser parser)
        {
            return new FXParticleSysBoneNugget
            {
                Bone = parser.ParseBoneName(),
                Particle = parser.ParseAssetReference(),
                FollowBone = parser.ParseAttributeBoolean("FollowBone")
            };
        }

        public string Bone { get; private set; }
        public string Particle { get; private set; }
        public bool FollowBone { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class AttachedModelFXNugget : FXListItem
    {
        internal static AttachedModelFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<AttachedModelFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<AttachedModelFXNugget>
        {
            { "Modelname", (parser, x) => x.ModelName = parser.ParseAssetReference() }
        });

        public string ModelName { get; private set; }
    }

    [AddedIn(SageGame.Bfme)]
    public sealed class CursorParticleSystemFXNugget : FXListItem
    {
        internal static CursorParticleSystemFXNugget Parse(IniParser parser) => parser.ParseBlock(FieldParseTable);

        private static readonly IniParseTable<CursorParticleSystemFXNugget> FieldParseTable = FXListItemFieldParseTable.Concat(new IniParseTable<CursorParticleSystemFXNugget>
        {
            { "Anim2DTemplateName", (parser, x) => x.Anim2DTemplateName = parser.ParseAssetReference() },
            { "BurstCount", (parser, x) => x.BurstCount = parser.ParseInteger() },
            { "ParticleLife", (parser, x) => x.ParticleLife = parser.ParseRandomVariable() },
            { "SystemLife", (parser, x) => x.SystemLife = parser.ParseRandomVariable() },
            { "DriftVelX", (parser, x) => x.DriftVelX = parser.ParseRandomVariable() },
            { "DriftVelY", (parser, x) => x.DriftVelY = parser.ParseRandomVariable() },
        });

        public string Anim2DTemplateName { get; private set; }
        public int BurstCount { get; private set; }
        public RandomVariable ParticleLife { get; private set; }
        public RandomVariable SystemLife { get; private set; }
        public RandomVariable DriftVelX { get; private set; }
        public RandomVariable DriftVelY { get; private set; }
    }

    public enum ViewShakeType
    {
        [IniEnum("SUBTLE")]
        Subtle,

        [IniEnum("NORMAL")]
        Normal,

        [IniEnum("STRONG")]
        Strong,

        [IniEnum("SEVERE")]
        Severe,

        [IniEnum("CINE_EXTREME"), AddedIn(SageGame.Bfme)]
        CineExtreme
    }

    public enum TerrainScorchType
    {
        [IniEnum("RANDOM")]
        Random,

        [IniEnum("SCORCH_4"), AddedIn(SageGame.CncGeneralsZeroHour)]
        Scorch4,
    }

    [AddedIn(SageGame.Bfme)]
    public enum FxType
    {
        [IniEnum("MAGIC")]
        Magic,

        [IniEnum("SWORD_SLASH")]
        SwordSlash,

        [IniEnum("EVIL_ARROW_PIERCE")]
        EvilArrowPierce,

        [IniEnum("CLUBBING")]
        Clubbing,

        [IniEnum("SMALL_ROCK")]
        SmallRock,

        [IniEnum("BIG_ROCK"), AddedIn(SageGame.Bfme2)]
        BigRock,

        [IniEnum("FLAME"), AddedIn(SageGame.Bfme2)]
        Flame,

        [IniEnum("ELECTRIC"), AddedIn(SageGame.Bfme)]
        Electric,

        [IniEnum("BALROG_SWORD"), AddedIn(SageGame.Bfme)]
        BalrogSword,

        [IniEnum("GOOD_ARROW_PIERCE"), AddedIn(SageGame.Bfme)]
        GoodArrowPierce,

        [IniEnum("REFLECTED"), AddedIn(SageGame.Bfme)]
        Reflected,

        [IniEnum("GIMLI_LEAP"), AddedIn(SageGame.Bfme)]
        GimliLeap,

        [IniEnum("WITCH_KING_MORGUL_BLADE"), AddedIn(SageGame.Bfme)]
        WitchKingMorgulBlade,

        [IniEnum("STRUCTURAL"), AddedIn(SageGame.Bfme)]
        Structural,

        [IniEnum("BALROG_WHIP"), AddedIn(SageGame.Bfme)]
        BalrogWhip,

        [IniEnum("POISON"), AddedIn(SageGame.Bfme2)]
        Poison,

        [IniEnum("BOLT"), AddedIn(SageGame.Bfme2)]
        Bolt,

        [IniEnum("TORNADO"), AddedIn(SageGame.Bfme2)]
        Tornado,

        [IniEnum("FIRE1"), AddedIn(SageGame.Bfme2)]
        Fire1,

        [IniEnum("FIRE2"), AddedIn(SageGame.Bfme2)]
        Fire2,

        [IniEnum("FIRE3"), AddedIn(SageGame.Bfme2)]
        Fire3,

        [IniEnum("FLOOD_HORSE"), AddedIn(SageGame.Bfme2)]
        FloodHorse,

        [IniEnum("UNDEFINED"), AddedIn(SageGame.Bfme2)]
        Undefined,

        [IniEnum("NECRO1"), AddedIn(SageGame.Bfme2Rotwk)]
        Necro1,

        [IniEnum("NECRO2"), AddedIn(SageGame.Bfme2Rotwk)]
        Necro2,
    }

    [AddedIn(SageGame.Bfme2)]
    public enum ShaderType
    {
        [IniEnum("ADDITIVE")]
        Additive
    }
}
