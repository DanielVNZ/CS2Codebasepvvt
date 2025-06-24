using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class FireAndRescueInfoviewUISystem : InfoviewUISystemBase
{
	[BurstCompile]
	private struct FireHazardJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Building> m_BuildingType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> m_DamagedType;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> m_UnderConstructionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public EventHelpers.FireHazardData m_FireHazardData;

		public NativeArray<float> m_Result;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Building> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<CurrentDistrict> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
			NativeArray<Damaged> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Damaged>(ref m_DamagedType);
			NativeArray<UnderConstruction> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnderConstruction>(ref m_UnderConstructionType);
			float num = 0f;
			float num2 = 0f;
			Damaged damaged = default(Damaged);
			UnderConstruction underConstruction = default(UnderConstruction);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				PrefabRef prefabRef = nativeArray2[i];
				Building building = nativeArray3[i];
				CurrentDistrict currentDistrict = nativeArray4[i];
				CollectionUtils.TryGet<Damaged>(nativeArray5, i, ref damaged);
				if (!CollectionUtils.TryGet<UnderConstruction>(nativeArray6, i, ref underConstruction))
				{
					underConstruction = new UnderConstruction
					{
						m_Progress = byte.MaxValue
					};
				}
				if (m_FireHazardData.GetFireHazard(prefabRef, building, currentDistrict, damaged, underConstruction, out var _, out var riskFactor))
				{
					num += riskFactor;
					num2 += 1f;
				}
			}
			ref NativeArray<float> reference = ref m_Result;
			reference[0] = reference[0] + num;
			reference = ref m_Result;
			reference[1] = reference[1] + num2;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Building> __Game_Buildings_Building_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Damaged> __Game_Objects_Damaged_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UnderConstruction> __Game_Objects_UnderConstruction_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

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
			__Game_Buildings_Building_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(true);
			__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(true);
			__Game_Objects_Damaged_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Damaged>(true);
			__Game_Objects_UnderConstruction_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnderConstruction>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private const string kGroup = "fireAndRescueInfo";

	private LocalEffectSystem m_LocalEffectSystem;

	private ClimateSystem m_ClimateSystem;

	private FireHazardSystem m_FireHazardSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_FlammableQuery;

	private EntityQuery m_FireStationsModifiedQuery;

	private EntityQuery m_FireConfigQuery;

	private NativeArray<float> m_Results;

	private EventHelpers.FireHazardData m_FireHazardData;

	private ValueBinding<IndicatorValue> m_AverageFireHazard;

	private TypeHandle __TypeHandle;

	protected override bool Active
	{
		get
		{
			if (!base.Active)
			{
				return ((EventBindingBase)m_AverageFireHazard).active;
			}
			return true;
		}
	}

	protected override bool Modified => !((EntityQuery)(ref m_FireStationsModifiedQuery)).IsEmptyIgnoreFilter;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LocalEffectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LocalEffectSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_FireHazardSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<FireHazardSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Tree>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Buildings.FireStation>(),
			ComponentType.ReadOnly<OnFire>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_FlammableQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.FireStation>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		array2[0] = val;
		m_FireStationsModifiedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_FireConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireConfigurationData>() });
		AddBinding((IBinding)(object)(m_AverageFireHazard = new ValueBinding<IndicatorValue>("fireAndRescueInfo", "averageFireHazard", default(IndicatorValue), (IWriter<IndicatorValue>)(object)new ValueWriter<IndicatorValue>(), (EqualityComparer<IndicatorValue>)null)));
		m_Results = new NativeArray<float>(2, (Allocator)4, (NativeArrayOptions)1);
		m_FireHazardData = new EventHelpers.FireHazardData((SystemBase)(object)this);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_Results.Dispose();
	}

	protected override void PerformUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		LocalEffectSystem.ReadData readData = m_LocalEffectSystem.GetReadData(out dependencies);
		FireConfigurationPrefab prefab = m_PrefabSystem.GetPrefab<FireConfigurationPrefab>(((EntityQuery)(ref m_FireConfigQuery)).GetSingletonEntity());
		if (m_Results.IsCreated)
		{
			JobHandle dependency = ((SystemBase)this).Dependency;
			((JobHandle)(ref dependency)).Complete();
			float num = m_Results[0];
			float num2 = m_Results[1];
			float current = ((num2 > 0f) ? (num / num2) : 0f);
			m_AverageFireHazard.Update(new IndicatorValue(0f, 100f, current));
			m_Results[0] = 0f;
			m_Results[1] = 0f;
		}
		m_FireHazardData.Update((SystemBase)(object)this, readData, prefab, m_ClimateSystem.temperature, m_FireHazardSystem.noRainDays);
		JobHandle val = JobChunkExtensions.Schedule<FireHazardJob>(new FireHazardJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedType = InternalCompilerInterface.GetComponentTypeHandle<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnderConstructionType = InternalCompilerInterface.GetComponentTypeHandle<UnderConstruction>(ref __TypeHandle.__Game_Objects_UnderConstruction_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_FireHazardData = m_FireHazardData,
			m_Result = m_Results
		}, m_FlammableQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_LocalEffectSystem.AddLocalEffectReader(val);
		((SystemBase)this).Dependency = val;
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
	public FireAndRescueInfoviewUISystem()
	{
	}
}
