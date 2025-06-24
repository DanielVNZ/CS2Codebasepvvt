using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine.Scripting;

namespace Game.Buildings;

[CompilerGenerated]
public class CityServiceEfficiencySystem : GameSystemBase
{
	[BurstCompile]
	private struct BuildingStateEfficiencyJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		public BufferTypeHandle<Efficiency> m_EfficiencyType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> m_ServiceObjectDatas;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> m_ServiceUpkeepDatas;

		[ReadOnly]
		public DynamicBuffer<ServiceBudgetData> m_ServiceBudgets;

		public AnimationCurve1 m_ServiceBudgetEfficiencyFactor;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			BufferAccessor<Efficiency> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_EfficiencyType);
			ServiceObjectData serviceObjectData = default(ServiceObjectData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if (m_ServiceObjectDatas.TryGetComponent((Entity)nativeArray[i], ref serviceObjectData))
				{
					float efficiency;
					if (HasMoneyUpkeep((Entity)nativeArray[i]) || (bufferAccessor.Length != 0 && HasMoneyUpkeep(bufferAccessor[i])))
					{
						int serviceBudget = GetServiceBudget(serviceObjectData.m_Service);
						efficiency = ((AnimationCurve1)(ref m_ServiceBudgetEfficiencyFactor)).Evaluate((float)serviceBudget / 100f);
					}
					else
					{
						efficiency = 1f;
					}
					BuildingUtils.SetEfficiencyFactor(bufferAccessor2[i], EfficiencyFactor.ServiceBudget, efficiency);
				}
			}
		}

		private bool HasMoneyUpkeep(Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceUpkeepData> val = default(DynamicBuffer<ServiceUpkeepData>);
			if (m_ServiceUpkeepDatas.TryGetBuffer(prefab, ref val))
			{
				Enumerator<ServiceUpkeepData> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.m_Upkeep.m_Resource == Resource.Money)
						{
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			return false;
		}

		private bool HasMoneyUpkeep(DynamicBuffer<InstalledUpgrade> installedUpgrades)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<InstalledUpgrade> enumerator = installedUpgrades.GetEnumerator();
			try
			{
				PrefabRef prefabRef = default(PrefabRef);
				while (enumerator.MoveNext())
				{
					InstalledUpgrade current = enumerator.Current;
					if (m_Prefabs.TryGetComponent((Entity)current, ref prefabRef) && HasMoneyUpkeep((Entity)prefabRef))
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return false;
		}

		private int GetServiceBudget(Entity service)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<ServiceBudgetData> enumerator = m_ServiceBudgets.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ServiceBudgetData current = enumerator.Current;
					if (current.m_Service == service)
					{
						return current.m_Budget;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return 100;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceObjectData> __Game_Prefabs_ServiceObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceUpkeepData> __Game_Prefabs_ServiceUpkeepData_RO_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Buildings_Efficiency_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ServiceObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceObjectData>(true);
			__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceUpkeepData>(true);
		}
	}

	private EntityQuery m_UpdatedBudgetQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_ChangedBuildingQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_339138653_0;

	private EntityQuery __query_339138653_1;

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
		//IL_0040: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdatedBudgetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ServiceBudgetData>(),
			ComponentType.ReadOnly<Updated>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<Efficiency>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_ChangedBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<CityServiceUpkeep>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<Efficiency>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_UpdatedBudgetQuery, m_ChangedBuildingQuery });
		((ComponentSystemBase)this).RequireForUpdate<ServiceBudgetData>();
		((ComponentSystemBase)this).RequireForUpdate<BuildingEfficiencyParameterData>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		BuildingEfficiencyParameterData singleton = ((EntityQuery)(ref __query_339138653_0)).GetSingleton<BuildingEfficiencyParameterData>();
		BuildingStateEfficiencyJob buildingStateEfficiencyJob = new BuildingStateEfficiencyJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceObjectDatas = InternalCompilerInterface.GetComponentLookup<ServiceObjectData>(ref __TypeHandle.__Game_Prefabs_ServiceObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpkeepDatas = InternalCompilerInterface.GetBufferLookup<ServiceUpkeepData>(ref __TypeHandle.__Game_Prefabs_ServiceUpkeepData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceBudgets = ((EntityQuery)(ref __query_339138653_1)).GetSingletonBuffer<ServiceBudgetData>(true),
			m_ServiceBudgetEfficiencyFactor = singleton.m_ServiceBudgetEfficiencyFactor
		};
		EntityQuery val = ((!((EntityQuery)(ref m_UpdatedBudgetQuery)).IsEmptyIgnoreFilter) ? m_BuildingQuery : m_ChangedBuildingQuery);
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<BuildingStateEfficiencyJob>(buildingStateEfficiencyJob, val, ((SystemBase)this).Dependency);
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<BuildingEfficiencyParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_339138653_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		val2 = ((EntityQueryBuilder)(ref val)).WithAllRW<ServiceBudgetData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_339138653_1 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public CityServiceEfficiencySystem()
	{
	}
}
