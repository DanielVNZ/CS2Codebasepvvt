using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.City;
using Game.Objects;
using Game.Prefabs;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class LevelSection : InfoSectionBase
{
	[BurstCompile]
	private struct CalculateMaxLevelJob : IJobChunk
	{
		[ReadOnly]
		public int2 m_LotSize;

		[ReadOnly]
		public Entity m_ZonePrefabEntity;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> m_BuildingDataTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingDataTypeHandle;

		public NativeArray<int> m_Result;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<BuildingData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BuildingData>(ref m_BuildingDataTypeHandle);
			NativeArray<SpawnableBuildingData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingDataTypeHandle);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				SpawnableBuildingData spawnableBuildingData = nativeArray2[i];
				if (spawnableBuildingData.m_Level > m_Result[0] && spawnableBuildingData.m_ZonePrefab == m_ZonePrefabEntity)
				{
					BuildingData buildingData = nativeArray[i];
					if (((int2)(ref buildingData.m_LotSize)).Equals(m_LotSize))
					{
						m_Result[0] = spawnableBuildingData.m_Level;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_BuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
		}
	}

	private EntityQuery m_SpawnableBuildingQuery;

	private EntityQuery m_CityQuery;

	private NativeArray<int> m_Result;

	private TypeHandle __TypeHandle;

	protected override string group => "LevelSection";

	private int level { get; set; }

	private int maxLevel { get; set; }

	private bool isUnderConstruction { get; set; }

	private float progress { get; set; }

	private Entity zone { get; set; }

	protected override bool displayForUnderConstruction => true;

	protected override void Reset()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		level = 0;
		maxLevel = 0;
		isUnderConstruction = false;
		progress = 0f;
		zone = Entity.Null;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SpawnableBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<SpawnableBuildingData>()
		});
		m_CityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CityModifier>() });
		m_Result = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_Result.Dispose();
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(selectedPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Abandoned>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Renter>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<BuildingData>(selectedPrefab))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						return ((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(selectedPrefab);
					}
				}
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.visible = Visible();
		if (base.visible)
		{
			UnderConstruction underConstruction = default(UnderConstruction);
			if (EntitiesExtensions.TryGetComponent<UnderConstruction>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref underConstruction) && underConstruction.m_NewPrefab == Entity.Null)
			{
				isUnderConstruction = true;
				progress = Math.Min((int)underConstruction.m_Progress, 100);
				return;
			}
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			BuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<BuildingData>(selectedPrefab);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SpawnableBuildingData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(selectedPrefab);
			JobHandle val = JobChunkExtensions.Schedule<CalculateMaxLevelJob>(new CalculateMaxLevelJob
			{
				m_LotSize = componentData.m_LotSize,
				m_ZonePrefabEntity = componentData2.m_ZonePrefab,
				m_BuildingDataTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingDataTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_Result = m_Result
			}, m_SpawnableBuildingQuery, ((SystemBase)this).Dependency);
			((JobHandle)(ref val)).Complete();
		}
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		SpawnableBuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(selectedPrefab);
		zone = componentData.m_ZonePrefab;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		ZoneData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(zone);
		if (isUnderConstruction)
		{
			base.tooltipKeys.Add("UnderConstruction");
			m_InfoUISystem.tooltipTags.Add(TooltipTags.UnderConstruction);
			return;
		}
		switch (componentData2.m_AreaType)
		{
		case AreaType.Residential:
			base.tooltipKeys.Add("Residential");
			break;
		case AreaType.Commercial:
			base.tooltipKeys.Add("Commercial");
			break;
		case AreaType.Industrial:
			base.tooltipKeys.Add(((componentData2.m_ZoneFlags & ZoneFlags.Office) != 0) ? "Office" : "Industrial");
			break;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		BuildingPropertyData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingPropertyData>(selectedPrefab);
		level = componentData.m_Level;
		maxLevel = math.max(m_Result[0], level);
		progress = 0f;
		if (componentData.m_Level < maxLevel)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			int condition = ((EntityManager)(ref entityManager)).GetComponentData<BuildingCondition>(selectedEntity).m_Condition;
			AreaType areaType = componentData2.m_AreaType;
			int currentlevel = level;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			int levelingCost = BuildingUtils.GetLevelingCost(areaType, componentData3, currentlevel, ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(((EntityQuery)(ref m_CityQuery)).GetSingletonEntity(), true));
			progress = ((levelingCost > 0) ? ((float)condition / (float)levelingCost * 100f) : 100f);
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("zone");
		writer.Write(m_PrefabSystem.GetPrefabName(zone));
		writer.PropertyName("level");
		writer.Write(level);
		writer.PropertyName("maxLevel");
		writer.Write(maxLevel);
		writer.PropertyName("isUnderConstruction");
		writer.Write(isUnderConstruction);
		writer.PropertyName("progress");
		writer.Write(progress);
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
	public LevelSection()
	{
	}
}
