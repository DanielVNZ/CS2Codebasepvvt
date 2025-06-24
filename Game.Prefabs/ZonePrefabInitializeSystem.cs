using System.Runtime.CompilerServices;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ZonePrefabInitializeSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ZoneData> __Game_Prefabs_ZoneData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle;

		public BufferTypeHandle<ProcessEstimate> __Game_Zones_ProcessEstimate_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_ZoneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneData>(false);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingPropertyData>(true);
			__Game_Zones_ProcessEstimate_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ProcessEstimate>(false);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
		}
	}

	private EntityQuery m_PrefabGroup;

	private EntityQuery m_ProcessGroup;

	private EntityQuery m_EconomyParameterGroup;

	private PrefabSystem m_PrefabSystem;

	private ResourceSystem m_ResourceSystem;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ZoneData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[0];
		array[0] = val;
		m_PrefabGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ProcessGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<IndustrialProcessData>(),
			ComponentType.ReadOnly<IndustrialCompanyData>(),
			ComponentType.ReadOnly<WorkplaceData>(),
			ComponentType.Exclude<StorageCompanyData>()
		});
		m_EconomyParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_ProcessGroup);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterGroup);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_PrefabGroup)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabGroup)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ZoneData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<BuildingPropertyData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<ProcessEstimate> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<ProcessEstimate>(ref __TypeHandle.__Game_Zones_ProcessEstimate_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		EconomyParameterData economyParameters = ((EntityQuery)(ref m_EconomyParameterGroup)).GetSingleton<EconomyParameterData>();
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_ProcessGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<IndustrialProcessData> val3 = ((EntityQuery)(ref m_ProcessGroup)).ToComponentDataArray<IndustrialProcessData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<WorkplaceData> val4 = ((EntityQuery)(ref m_ProcessGroup)).ToComponentDataArray<WorkplaceData>(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentLookup<ResourceData> resourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val5 = val[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val5)).GetNativeArray(entityTypeHandle);
			NativeArray<ZoneData> nativeArray2 = ((ArchetypeChunk)(ref val5)).GetNativeArray<ZoneData>(ref componentTypeHandle);
			NativeArray<BuildingPropertyData> nativeArray3 = ((ArchetypeChunk)(ref val5)).GetNativeArray<BuildingPropertyData>(ref componentTypeHandle2);
			BufferAccessor<ProcessEstimate> bufferAccessor = ((ArchetypeChunk)(ref val5)).GetBufferAccessor<ProcessEstimate>(ref bufferTypeHandle);
			if (nativeArray2.Length <= 0)
			{
				continue;
			}
			for (int j = 0; j < ((ArchetypeChunk)(ref val5)).Count; j++)
			{
				DynamicBuffer<ProcessEstimate> val6 = bufferAccessor[j];
				bool office = m_PrefabSystem.GetPrefab<ZonePrefab>(nativeArray[j]).m_Office;
				if (office)
				{
					ZoneData zoneData = nativeArray2[j];
					zoneData.m_ZoneFlags |= ZoneFlags.Office;
					nativeArray2[j] = zoneData;
				}
				if (nativeArray2[j].m_AreaType != AreaType.Industrial || office)
				{
					continue;
				}
				float num = 1f;
				if (nativeArray3.Length > 0)
				{
					num = nativeArray3[j].m_SpaceMultiplier;
				}
				for (int k = 0; k < EconomyUtils.ResourceCount; k++)
				{
					val6.Add(default(ProcessEstimate));
				}
				for (int l = 0; l < val3.Length; l++)
				{
					IndustrialProcessData industrialProcessData = val3[l];
					int num2 = Mathf.RoundToInt(num * industrialProcessData.m_MaxWorkersPerCell * 100f);
					WorkplaceData workplaceData = val4[l];
					Workplaces workplaces = EconomyUtils.CalculateNumberOfWorkplaces(num2, workplaceData.m_Complexity, 1);
					float num3 = 0f;
					float num4 = 1f;
					for (int m = 0; m < 5; m++)
					{
						float num5 = (float)workplaces[m] * EconomyUtils.GetWorkerWorkforce(50, m);
						if (m < 2)
						{
							num3 += num5;
						}
						else
						{
							num4 += num5;
						}
					}
					int resourceIndex = EconomyUtils.GetResourceIndex(industrialProcessData.m_Output.m_Resource);
					BuildingData buildingData = new BuildingData
					{
						m_LotSize = new int2(10, 10)
					};
					EconomyUtils.BuildPseudoTradeCost(5000f, industrialProcessData, ref resourceDatas, prefabs);
					float num6 = 1f * (float)EconomyUtils.GetCompanyProductionPerDay(1f, num2, new SpawnableBuildingData
					{
						m_Level = 1
					}.m_Level, isIndustrial: true, workplaceData, industrialProcessData, prefabs, ref resourceDatas, ref economyParameters) / (float)EconomyUtils.kCompanyUpdatesPerDay;
					ProcessEstimate processEstimate = new ProcessEstimate
					{
						m_ProductionPerCell = 0.01f * num6,
						m_WorkerProductionPerCell = 0.01f * num6 / (num * industrialProcessData.m_MaxWorkersPerCell),
						m_LowEducationWeight = num3 / (num3 + num4),
						m_ProcessEntity = val2[l]
					};
					val6[resourceIndex] = processEstimate;
				}
			}
		}
		val3.Dispose();
		val4.Dispose();
		val.Dispose();
		val2.Dispose();
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
	public ZonePrefabInitializeSystem()
	{
	}
}
