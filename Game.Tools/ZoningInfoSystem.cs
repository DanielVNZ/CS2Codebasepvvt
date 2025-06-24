using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ZoningInfoSystem : GameSystemBase, IZoningInfoSystem
{
	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	private EntityQuery m_ZoningPreferenceGroup;

	private EntityQuery m_ProcessQuery;

	private NativeList<ZoneEvaluationUtils.ZoningEvaluationResult> m_EvaluationResults;

	private ZoneToolSystem m_ZoneToolSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private ToolRaycastSystem m_ToolRaycastSystem;

	private TypeHandle __TypeHandle;

	public NativeList<ZoneEvaluationUtils.ZoningEvaluationResult> evaluationResults => m_EvaluationResults;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ZoneToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneToolSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_ToolRaycastSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolRaycastSystem>();
		m_ProcessQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialProcessData>() });
		m_ZoningPreferenceGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ZonePreferenceData>() });
		m_EvaluationResults = new NativeList<ZoneEvaluationUtils.ZoningEvaluationResult>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_ProcessQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_EvaluationResults.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		m_EvaluationResults.Clear();
		Block block = default(Block);
		Owner owner = default(Owner);
		if (m_ToolRaycastSystem.GetRaycastResult(out var result) && EntitiesExtensions.TryGetComponent<Block>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref block) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, result.m_Owner, ref owner))
		{
			JobHandle dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			BufferLookup<ResourceAvailability> bufferLookup = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
			ComponentLookup<LandValue> componentLookup = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			NativeArray<ZonePreferenceData> val = ((EntityQuery)(ref m_ZoningPreferenceGroup)).ToComponentDataArray<ZonePreferenceData>(AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle deps;
			NativeArray<int> industrialResourceDemands = m_IndustrialDemandSystem.GetIndustrialResourceDemands(out deps);
			ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
			ComponentLookup<ResourceData> componentLookup2 = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
			ZonePreferenceData preferences = val[0];
			Entity owner2 = owner.m_Owner;
			AreaType areaType = m_ZoneToolSystem.prefab.m_AreaType;
			JobHandle dependencies;
			NativeArray<GroundPollution> map = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies);
			JobHandle dependencies2;
			NativeArray<AirPollution> map2 = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies2);
			JobHandle dependencies3;
			NativeArray<NoisePollution> map3 = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies3);
			((JobHandle)(ref deps)).Complete();
			((JobHandle)(ref dependencies)).Complete();
			((JobHandle)(ref dependencies2)).Complete();
			((JobHandle)(ref dependencies3)).Complete();
			float num = GroundPollutionSystem.GetPollution(block.m_Position, map).m_Pollution;
			num += (float)AirPollutionSystem.GetPollution(block.m_Position, map2).m_Pollution;
			num += (float)NoisePollutionSystem.GetPollution(block.m_Position, map3).m_Pollution;
			float num2 = componentLookup[owner2].m_LandValue;
			Entity entity = m_PrefabSystem.GetEntity(m_ZoneToolSystem.prefab);
			EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
			DynamicBuffer<ProcessEstimate> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ProcessEstimate>(entity, true);
			entityManager = ((ComponentSystemBase)this).World.EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ZonePropertiesData>(entity))
			{
				entityManager = ((ComponentSystemBase)this).World.EntityManager;
				ZonePropertiesData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ZonePropertiesData>(entity);
				float num3 = ((areaType != AreaType.Residential) ? componentData.m_SpaceMultiplier : (componentData.m_ScaleResidentials ? componentData.m_ResidentialProperties : (componentData.m_ResidentialProperties / 8f)));
				num2 /= num3;
			}
			JobHandle val2 = default(JobHandle);
			NativeList<IndustrialProcessData> processes = ((EntityQuery)(ref m_ProcessQuery)).ToComponentDataListAsync<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			((JobHandle)(ref val2)).Complete();
			ZoneEvaluationUtils.GetFactors(areaType, m_ZoneToolSystem.prefab.m_Office, bufferLookup[owner2], result.m_Hit.m_CurvePosition, ref preferences, m_EvaluationResults, industrialResourceDemands, num, num2, buffer, processes, prefabs, componentLookup2);
			processes.Dispose();
			val.Dispose();
			NativeSortExtension.Sort<ZoneEvaluationUtils.ZoningEvaluationResult>(m_EvaluationResults);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ZoningInfoSystem()
	{
	}
}
