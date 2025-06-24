using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class PollutionSection : InfoSectionBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> __Game_Prefabs_PollutionModifierData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_PollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionData>(true);
			__Game_Prefabs_PollutionModifierData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionModifierData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
		}
	}

	private EntityQuery m_UIConfigQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1774369403_0;

	private EntityQuery __query_1774369403_1;

	protected override string group => "PollutionSection";

	protected override bool displayForDestroyedObjects => true;

	private PollutionThreshold groundPollutionKey { get; set; }

	private PollutionThreshold airPollutionKey { get; set; }

	private PollutionThreshold noisePollutionKey { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UIConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIPollutionConfigurationData>() });
	}

	protected override void Reset()
	{
		groundPollutionKey = PollutionThreshold.None;
		airPollutionKey = PollutionThreshold.None;
		noisePollutionKey = PollutionThreshold.None;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
		{
			PollutionData pollution = GetPollution();
			if (!(pollution.m_GroundPollution > 0f) && !(pollution.m_AirPollution > 0f))
			{
				return pollution.m_NoisePollution > 0f;
			}
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		PollutionData pollution = GetPollution();
		UIPollutionConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<UIPollutionConfigurationPrefab>(m_UIConfigQuery);
		groundPollutionKey = PollutionUIUtils.GetPollutionKey(singletonPrefab.m_GroundPollution, pollution.m_GroundPollution);
		airPollutionKey = PollutionUIUtils.GetPollutionKey(singletonPrefab.m_AirPollution, pollution.m_AirPollution);
		noisePollutionKey = PollutionUIUtils.GetPollutionKey(singletonPrefab.m_NoisePollution, pollution.m_NoisePollution);
	}

	private PollutionData GetPollution()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).CompleteDependency();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		bool destroyed = ((EntityManager)(ref entityManager)).HasComponent<Destroyed>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool abandoned = ((EntityManager)(ref entityManager)).HasComponent<Abandoned>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool isPark = ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(selectedEntity);
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		float efficiency = (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref buffer) ? BuildingUtils.GetEfficiency(buffer) : 1f);
		DynamicBuffer<Renter> renters = default(DynamicBuffer<Renter>);
		EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref renters);
		DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
		EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref installedUpgrades);
		PollutionParameterData singleton = ((EntityQuery)(ref __query_1774369403_0)).GetSingleton<PollutionParameterData>();
		DynamicBuffer<CityModifier> singletonBuffer = ((EntityQuery)(ref __query_1774369403_1)).GetSingletonBuffer<CityModifier>(true);
		ComponentLookup<PrefabRef> prefabRefs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<BuildingData> buildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<SpawnableBuildingData> spawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PollutionData> pollutionDatas = InternalCompilerInterface.GetComponentLookup<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<PollutionModifierData> pollutionModifierDatas = InternalCompilerInterface.GetComponentLookup<PollutionModifierData>(ref __TypeHandle.__Game_Prefabs_PollutionModifierData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<ZoneData> zoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<Employee> employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<HouseholdCitizen> householdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		return BuildingPollutionAddSystem.GetBuildingPollution(selectedPrefab, destroyed, abandoned, isPark, efficiency, renters, installedUpgrades, singleton, singletonBuffer, ref prefabRefs, ref buildingDatas, ref spawnableDatas, ref pollutionDatas, ref pollutionModifierDatas, ref zoneDatas, ref employees, ref householdCitizens, ref citizens);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("groundPollutionKey");
		writer.Write((int)groundPollutionKey);
		writer.PropertyName("airPollutionKey");
		writer.Write((int)airPollutionKey);
		writer.PropertyName("noisePollutionKey");
		writer.Write((int)noisePollutionKey);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<PollutionParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1774369403_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAllRW<CityModifier>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1774369403_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public PollutionSection()
	{
	}
}
