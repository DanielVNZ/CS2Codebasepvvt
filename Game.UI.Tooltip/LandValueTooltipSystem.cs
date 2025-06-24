using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Debug;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class LandValueTooltipSystem : TooltipSystemBase
{
	[BurstCompile]
	private struct LandValueTooltipJob : IJob
	{
		[ReadOnly]
		public NativeArray<LandValueCell> m_LandValueMap;

		[ReadOnly]
		public NativeArray<TerrainAttractiveness> m_AttractiveMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_GroundPollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public AttractivenessParameterData m_AttractivenessParameterData;

		[ReadOnly]
		public float m_TerrainHeight;

		[ReadOnly]
		public float3 m_RaycastPosition;

		public NativeValue<float> m_LandValueResult;

		public NativeValue<float> m_TerrainAttractiveResult;

		public NativeValue<float> m_AirPollutionResult;

		public NativeValue<float> m_NoisePollutionResult;

		public NativeValue<float> m_GroundPollutionResult;

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int cellIndex = LandValueSystem.GetCellIndex(m_RaycastPosition);
			m_LandValueResult.value = m_LandValueMap[cellIndex].m_LandValue;
			TerrainAttractiveness attractiveness = TerrainAttractivenessSystem.GetAttractiveness(m_RaycastPosition, m_AttractiveMap);
			m_TerrainAttractiveResult.value = TerrainAttractivenessSystem.EvaluateAttractiveness(m_TerrainHeight, attractiveness, m_AttractivenessParameterData);
			m_GroundPollutionResult.value = GroundPollutionSystem.GetPollution(m_RaycastPosition, m_GroundPollutionMap).m_Pollution;
			m_AirPollutionResult.value = AirPollutionSystem.GetPollution(m_RaycastPosition, m_AirPollutionMap).m_Pollution;
			m_NoisePollutionResult.value = NoisePollutionSystem.GetPollution(m_RaycastPosition, m_NoisePollutionMap).m_Pollution;
		}
	}

	private RaycastSystem m_RaycastSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private ToolSystem m_ToolSystem;

	private TerrainToolSystem m_TerrainToolSystem;

	private LandValueSystem m_LandValueSystem;

	private LandValueDebugSystem m_LandValueDebugSystem;

	private TerrainAttractivenessSystem m_TerrainAttractivenessSystem;

	private TerrainSystem m_TerrainSystem;

	private PrefabSystem m_PrefabSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private EntityQuery m_AttractivenessParameterQuery;

	private EntityQuery m_LandValueParameterQuery;

	private FloatTooltip m_LandValueTooltip;

	private FloatTooltip m_TerrainAttractiveTooltip;

	private FloatTooltip m_AirPollutionTooltip;

	private FloatTooltip m_GroundPollutionTooltip;

	private FloatTooltip m_NoisePollutionTooltip;

	private NativeValue<float> m_LandValueResult;

	private NativeValue<float> m_TerrainAttractiveResult;

	private NativeValue<float> m_AirPollutionResult;

	private NativeValue<float> m_NoisePollutionResult;

	private NativeValue<float> m_GroundPollutionResult;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RaycastSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TerrainToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainToolSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_LandValueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LandValueSystem>();
		m_LandValueDebugSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LandValueDebugSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_TerrainAttractivenessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainAttractivenessSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_AttractivenessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
		m_LandValueParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LandValueParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_AttractivenessParameterQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_LandValueParameterQuery);
		m_LandValueTooltip = new FloatTooltip
		{
			path = "LandValue",
			icon = "Media/Game/Icons/LandValue.svg",
			label = LocalizedString.Id("Infoviews.INFOVIEW[LandValue]"),
			unit = "money"
		};
		m_TerrainAttractiveTooltip = new FloatTooltip
		{
			path = "TerrainAttractive",
			icon = "Media/Game/Icons/Tourism.svg",
			label = LocalizedString.Id("Properties.CITY_MODIFIER[Attractiveness]"),
			unit = "integer"
		};
		m_AirPollutionTooltip = new FloatTooltip
		{
			path = "AirPollution",
			icon = "Media/Game/Icons/AirPollution.svg",
			label = LocalizedString.Id("Infoviews.INFOVIEW[AirPollution]"),
			unit = "integer"
		};
		m_GroundPollutionTooltip = new FloatTooltip
		{
			path = "GroundPollution",
			icon = "Media/Game/Icons/GroundPollution.svg",
			label = LocalizedString.Id("Infoviews.INFOVIEW[GroundPollution]"),
			unit = "integer"
		};
		m_NoisePollutionTooltip = new FloatTooltip
		{
			path = "NoisePollution",
			icon = "Media/Game/Icons/NoisePollution.svg",
			label = LocalizedString.Id("Infoviews.INFOVIEW[NoisePollution]"),
			unit = "integer"
		};
		m_LandValueResult = new NativeValue<float>((Allocator)4);
		m_TerrainAttractiveResult = new NativeValue<float>((Allocator)4);
		m_NoisePollutionResult = new NativeValue<float>((Allocator)4);
		m_AirPollutionResult = new NativeValue<float>((Allocator)4);
		m_GroundPollutionResult = new NativeValue<float>((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_LandValueResult.Dispose();
		m_TerrainAttractiveResult.Dispose();
		m_NoisePollutionResult.Dispose();
		m_AirPollutionResult.Dispose();
		m_GroundPollutionResult.Dispose();
		base.OnDestroy();
	}

	private bool IsInfomodeActivated()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (m_PrefabSystem.TryGetPrefab<InfoviewPrefab>(((EntityQuery)(ref m_LandValueParameterQuery)).GetSingleton<LandValueParameterData>().m_LandValueInfoViewPrefab, out var prefab))
		{
			return (Object)(object)m_ToolSystem.activeInfoview == (Object)(object)prefab;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		if (IsInfomodeActivated() || ((ComponentSystemBase)m_LandValueDebugSystem).Enabled)
		{
			((SystemBase)this).CompleteDependency();
			m_LandValueTooltip.value = m_LandValueResult.value;
			AddMouseTooltip(m_LandValueTooltip);
			if (((ComponentSystemBase)m_LandValueDebugSystem).Enabled)
			{
				if (m_TerrainAttractiveResult.value > 0f)
				{
					m_TerrainAttractiveTooltip.value = m_TerrainAttractiveResult.value;
					AddMouseTooltip(m_TerrainAttractiveTooltip);
				}
				if (m_AirPollutionResult.value > 0f)
				{
					m_AirPollutionTooltip.value = m_AirPollutionResult.value;
					AddMouseTooltip(m_AirPollutionTooltip);
				}
				if (m_GroundPollutionResult.value > 0f)
				{
					m_GroundPollutionTooltip.value = m_GroundPollutionResult.value;
					AddMouseTooltip(m_GroundPollutionTooltip);
				}
				if (m_NoisePollutionResult.value > 0f)
				{
					m_NoisePollutionTooltip.value = m_NoisePollutionResult.value;
					AddMouseTooltip(m_NoisePollutionTooltip);
				}
			}
			m_LandValueResult.value = 0f;
			m_TerrainAttractiveResult.value = 0f;
			m_AirPollutionResult.value = 0f;
			m_GroundPollutionResult.value = 0f;
			m_NoisePollutionResult.value = 0f;
			if (m_CameraUpdateSystem.TryGetViewer(out var viewer))
			{
				RaycastInput input = new RaycastInput
				{
					m_Line = ToolRaycastSystem.CalculateRaycastLine(viewer.camera),
					m_TypeMask = (TypeMask.Terrain | TypeMask.Water),
					m_CollisionMask = (CollisionMask.OnGround | CollisionMask.Overground)
				};
				m_RaycastSystem.AddInput(this, input);
				NativeArray<RaycastResult> result = m_RaycastSystem.GetResult(this);
				if (result.Length != 0)
				{
					TerrainHeightData data = m_TerrainSystem.GetHeightData();
					JobHandle dependencies;
					JobHandle dependencies2;
					JobHandle dependencies3;
					JobHandle dependencies4;
					JobHandle dependencies5;
					LandValueTooltipJob landValueTooltipJob = new LandValueTooltipJob
					{
						m_LandValueMap = m_LandValueSystem.GetMap(readOnly: true, out dependencies),
						m_AttractiveMap = m_TerrainAttractivenessSystem.GetMap(readOnly: true, out dependencies2),
						m_GroundPollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies3),
						m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies4),
						m_NoisePollutionMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies5),
						m_TerrainHeight = TerrainUtils.SampleHeight(ref data, result[0].m_Hit.m_HitPosition),
						m_AttractivenessParameterData = ((EntityQuery)(ref m_AttractivenessParameterQuery)).GetSingleton<AttractivenessParameterData>(),
						m_LandValueResult = m_LandValueResult,
						m_NoisePollutionResult = m_NoisePollutionResult,
						m_AirPollutionResult = m_AirPollutionResult,
						m_GroundPollutionResult = m_GroundPollutionResult,
						m_TerrainAttractiveResult = m_TerrainAttractiveResult,
						m_RaycastPosition = result[0].m_Hit.m_HitPosition
					};
					((SystemBase)this).Dependency = IJobExtensions.Schedule<LandValueTooltipJob>(landValueTooltipJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, JobHandle.CombineDependencies(dependencies2, dependencies, JobHandle.CombineDependencies(dependencies3, dependencies4, dependencies5))));
					m_LandValueSystem.AddReader(((SystemBase)this).Dependency);
					m_TerrainAttractivenessSystem.AddReader(((SystemBase)this).Dependency);
					m_GroundPollutionSystem.AddReader(((SystemBase)this).Dependency);
					m_AirPollutionSystem.AddReader(((SystemBase)this).Dependency);
					m_NoisePollutionSystem.AddReader(((SystemBase)this).Dependency);
				}
			}
		}
		else
		{
			m_LandValueResult.value = 0f;
			m_TerrainAttractiveResult.value = 0f;
			m_AirPollutionResult.value = 0f;
			m_GroundPollutionResult.value = 0f;
			m_NoisePollutionResult.value = 0f;
		}
	}

	[Preserve]
	public LandValueTooltipSystem()
	{
	}
}
