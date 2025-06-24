using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Simulation;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class ResolvePrefabsSystem : GameSystemBase
{
	private struct ComponentModification
	{
		public Entity m_Entity;

		public PrefabComponents m_Add;

		public PrefabComponents m_Remove;

		public ComponentModification(Entity entity, PrefabComponents add, PrefabComponents remove)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Add = add;
			m_Remove = remove;
		}
	}

	[BurstCompile]
	private struct FillLoadedPrefabsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		[ReadOnly]
		public ComponentTypeHandle<PlacedSignatureBuildingData> m_PlacedSignatureType;

		[NativeDisableParallelForRestriction]
		public NativeArray<Entity> m_PrefabArray;

		[NativeDisableParallelForRestriction]
		public NativeArray<PrefabComponents> m_PrefabComponents;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			PrefabComponents prefabComponents = (PrefabComponents)0u;
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<Locked>(ref m_LockedType);
			if (((ArchetypeChunk)(ref chunk)).Has<PlacedSignatureBuildingData>(ref m_PlacedSignatureType))
			{
				prefabComponents |= PrefabComponents.PlacedSignatureBuilding;
			}
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				int index = nativeArray2[i].m_Index;
				index = math.select(index, m_PrefabArray.Length + index, index < 0);
				m_PrefabArray[index] = nativeArray[i];
				PrefabComponents prefabComponents2 = prefabComponents;
				SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
				if (((SafeBitRef)(ref enableBit)).IsValid && ((EnabledMask)(ref enabledMask))[i])
				{
					prefabComponents2 |= PrefabComponents.Locked;
				}
				m_PrefabComponents[index] = prefabComponents2;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CheckActualPrefabsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		[ReadOnly]
		public ComponentTypeHandle<SignatureBuildingData> m_SignatureBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<PlacedSignatureBuildingData> m_PlacedSignatureBuildingType;

		[ReadOnly]
		public BufferTypeHandle<LoadedIndex> m_LoadedIndexType;

		[ReadOnly]
		public Context m_Context;

		[ReadOnly]
		public NativeArray<PrefabComponents> m_PrefabComponents;

		public ComponentTypeHandle<Locked> m_LockedType;

		[NativeDisableParallelForRestriction]
		public NativeArray<Entity> m_PrefabArray;

		public ParallelWriter<ComponentModification> m_ComponentModifications;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Invalid comparison between Unknown and I4
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			BufferAccessor<LoadedIndex> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LoadedIndex>(ref m_LoadedIndexType);
			PrefabComponents prefabComponents = (PrefabComponents)0u;
			if (((ArchetypeChunk)(ref chunk)).Has<SignatureBuildingData>(ref m_SignatureBuildingType))
			{
				prefabComponents |= PrefabComponents.PlacedSignatureBuilding;
			}
			PrefabComponents prefabComponents2 = (PrefabComponents)0u;
			EnabledMask enabledMask = ((ArchetypeChunk)(ref chunk)).GetEnabledMask<Locked>(ref m_LockedType);
			if (((ArchetypeChunk)(ref chunk)).Has<PlacedSignatureBuildingData>(ref m_PlacedSignatureBuildingType))
			{
				prefabComponents2 |= PrefabComponents.PlacedSignatureBuilding;
			}
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<LoadedIndex> val2 = bufferAccessor[i];
				PrefabComponents prefabComponents3 = PrefabComponents.Locked;
				for (int j = 0; j < val2.Length; j++)
				{
					int index = val2[j].m_Index;
					index = math.select(index, m_PrefabArray.Length + index, index < 0);
					m_PrefabArray[index] = val;
					if ((int)((Context)(ref m_Context)).purpose == 2)
					{
						prefabComponents3 = m_PrefabComponents[index];
					}
				}
				SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
				if (((SafeBitRef)(ref enableBit)).IsValid)
				{
					((EnabledMask)(ref enabledMask))[i] = (prefabComponents3 & PrefabComponents.Locked) != 0;
					prefabComponents3 = (PrefabComponents)((uint)prefabComponents3 & 0xFFFFFFFEu);
				}
				prefabComponents3 &= prefabComponents;
				if (prefabComponents3 != prefabComponents2)
				{
					m_ComponentModifications.Enqueue(new ComponentModification(val, prefabComponents3 & ~prefabComponents2, prefabComponents2 & ~prefabComponents3));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CopyBudgetDataJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		[ReadOnly]
		public BufferTypeHandle<LoadedIndex> m_LoadedIndexType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CollectedCityServiceBudgetData> m_Budgets;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CollectedCityServiceFeeData> m_Fees;

		[NativeDisableParallelForRestriction]
		public BufferLookup<CollectedCityServiceUpkeepData> m_Upkeeps;

		[ReadOnly]
		public NativeArray<Entity> m_PrefabArray;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			BufferAccessor<LoadedIndex> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LoadedIndex>(ref m_LoadedIndexType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				DynamicBuffer<LoadedIndex> val2 = bufferAccessor[i];
				for (int j = 0; j < val2.Length; j++)
				{
					int index = val2[j].m_Index;
					index = math.select(index, m_PrefabArray.Length + index, index < 0);
					Entity val3 = m_PrefabArray[index];
					if (m_Budgets.HasComponent(val) && m_Budgets.HasComponent(val3))
					{
						m_Budgets[val] = m_Budgets[val3];
					}
					if (m_Fees.HasBuffer(val) && m_Fees.HasBuffer(val3))
					{
						DynamicBuffer<CollectedCityServiceFeeData> val4 = m_Fees[val];
						DynamicBuffer<CollectedCityServiceFeeData> val5 = m_Fees[val3];
						for (int k = 0; k < val5.Length; k++)
						{
							for (int l = 0; l < val4.Length; l++)
							{
								if (val4[l].m_PlayerResource == val5[k].m_PlayerResource)
								{
									val4[l] = val5[k];
								}
							}
						}
					}
					if (!m_Upkeeps.HasBuffer(val) || !m_Upkeeps.HasBuffer(val3))
					{
						continue;
					}
					DynamicBuffer<CollectedCityServiceUpkeepData> val6 = m_Upkeeps[val];
					DynamicBuffer<CollectedCityServiceUpkeepData> val7 = m_Upkeeps[val3];
					for (int m = 0; m < val7.Length; m++)
					{
						for (int n = 0; n < val6.Length; n++)
						{
							if (val6[n].m_Resource == val7[m].m_Resource)
							{
								val6[n] = val7[m];
							}
						}
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FillZoneTypeArrayJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		[ReadOnly]
		public ComponentTypeHandle<ZoneData> m_ZoneDataType;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public NativeArray<Entity> m_PrefabArray;

		[NativeDisableParallelForRestriction]
		public NativeArray<ZoneType> m_ZoneTypeArray;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			NativeArray<ZoneData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ZoneData>(ref m_ZoneDataType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				int index = nativeArray2[i].m_Index;
				index = math.select(index, m_PrefabArray.Length + index, index < 0);
				Entity val2 = m_PrefabArray[index];
				ZoneType zoneType = nativeArray3[i].m_ZoneType;
				ZoneType zoneType2 = m_ZoneData[val2].m_ZoneType;
				if (val == val2)
				{
					zoneType2 = ZoneType.None;
				}
				m_ZoneTypeArray[(int)zoneType.m_Index] = zoneType2;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FixZoneTypeJob : IJobChunk
	{
		[ReadOnly]
		public NativeArray<ZoneType> m_ZoneTypeArray;

		public BufferTypeHandle<Cell> m_CellType;

		public BufferTypeHandle<VacantLot> m_VacantLotType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Cell> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Cell>(ref m_CellType);
			BufferAccessor<VacantLot> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<VacantLot>(ref m_VacantLotType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Cell> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Cell cell = val[j];
					cell.m_Zone = m_ZoneTypeArray[(int)cell.m_Zone.m_Index];
					val[j] = cell;
				}
			}
			for (int k = 0; k < bufferAccessor2.Length; k++)
			{
				DynamicBuffer<VacantLot> val2 = bufferAccessor2[k];
				for (int l = 0; l < val2.Length; l++)
				{
					VacantLot vacantLot = val2[l];
					vacantLot.m_Type = m_ZoneTypeArray[(int)vacantLot.m_Type.m_Index];
					val2[l] = vacantLot;
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PlacedSignatureBuildingData> __Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LoadedIndex> __Game_Prefabs_LoadedIndex_RO_BufferTypeHandle;

		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RW_ComponentTypeHandle;

		public ComponentLookup<CollectedCityServiceBudgetData> __Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup;

		public BufferLookup<CollectedCityServiceFeeData> __Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup;

		public BufferLookup<CollectedCityServiceUpkeepData> __Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		public BufferTypeHandle<Cell> __Game_Zones_Cell_RW_BufferTypeHandle;

		public BufferTypeHandle<VacantLot> __Game_Zones_VacantLot_RW_BufferTypeHandle;

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
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
			__Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlacedSignatureBuildingData>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SignatureBuildingData>(true);
			__Game_Prefabs_LoadedIndex_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LoadedIndex>(true);
			__Game_Prefabs_Locked_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(false);
			__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CollectedCityServiceBudgetData>(false);
			__Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceFeeData>(false);
			__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CollectedCityServiceUpkeepData>(false);
			__Game_Prefabs_ZoneData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Zones_Cell_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Cell>(false);
			__Game_Zones_VacantLot_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<VacantLot>(false);
		}
	}

	private LoadGameSystem m_LoadGameSystem;

	private PrefabSystem m_PrefabSystem;

	private UpdateSystem m_UpdateSystem;

	private CheckPrefabReferencesSystem m_CheckPrefabReferencesSystem;

	private EntityQuery m_ActualPrefabQuery;

	private EntityQuery m_EnabledLoadedPrefabQuery;

	private EntityQuery m_AllLoadedPrefabQuery;

	private EntityQuery m_LoadedZonePrefabQuery;

	private EntityQuery m_LoadedZoneCellQuery;

	private EntityQuery m_ActualBudgetQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateSystem>();
		m_CheckPrefabReferencesSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CheckPrefabReferencesSystem>();
		EntityQueryBuilder val = new EntityQueryBuilder(AllocatorHandle.op_Implicit((Allocator)2));
		val = ((EntityQueryBuilder)(ref val)).WithAll<PrefabData, LoadedIndex>();
		val = ((EntityQueryBuilder)(ref val)).WithOptions((EntityQueryOptions)8);
		m_ActualPrefabQuery = ((EntityQueryBuilder)(ref val)).Build((SystemBase)(object)this);
		m_EnabledLoadedPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<LoadedIndex>()
		});
		val = new EntityQueryBuilder(AllocatorHandle.op_Implicit((Allocator)2));
		val = ((EntityQueryBuilder)(ref val)).WithAll<PrefabData>();
		val = ((EntityQueryBuilder)(ref val)).WithNone<LoadedIndex>();
		val = ((EntityQueryBuilder)(ref val)).WithOptions((EntityQueryOptions)8);
		m_AllLoadedPrefabQuery = ((EntityQueryBuilder)(ref val)).Build((SystemBase)(object)this);
		m_LoadedZonePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.Exclude<LoadedIndex>()
		});
		m_LoadedZoneCellQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Cell>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val2 = new EntityQueryDesc();
		val2.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<LoadedIndex>()
		};
		val2.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<CollectedCityServiceBudgetData>(),
			ComponentType.ReadOnly<CollectedCityServiceFeeData>(),
			ComponentType.ReadOnly<CollectedCityServiceUpkeepData>()
		};
		array[0] = val2;
		m_ActualBudgetQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		int num = ((EntityQuery)(ref m_EnabledLoadedPrefabQuery)).CalculateEntityCount();
		NativeArray<Entity> val = default(NativeArray<Entity>);
		val._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		NativeArray<PrefabComponents> prefabComponents = default(NativeArray<PrefabComponents>);
		prefabComponents._002Ector(num, (Allocator)3, (NativeArrayOptions)1);
		NativeArray<ZoneType> zoneTypeArray = default(NativeArray<ZoneType>);
		zoneTypeArray._002Ector(340, (Allocator)3, (NativeArrayOptions)1);
		NativeQueue<ComponentModification> val2 = default(NativeQueue<ComponentModification>);
		val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		FillLoadedPrefabsJob fillLoadedPrefabsJob = new FillLoadedPrefabsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlacedSignatureType = InternalCompilerInterface.GetComponentTypeHandle<PlacedSignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabArray = val,
			m_PrefabComponents = prefabComponents
		};
		CheckActualPrefabsJob checkActualPrefabsJob = new CheckActualPrefabsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SignatureBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlacedSignatureBuildingType = InternalCompilerInterface.GetComponentTypeHandle<PlacedSignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_PlacedSignatureBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LoadedIndexType = InternalCompilerInterface.GetBufferTypeHandle<LoadedIndex>(ref __TypeHandle.__Game_Prefabs_LoadedIndex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Context = m_LoadGameSystem.context,
			m_PrefabComponents = prefabComponents,
			m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabArray = val,
			m_ComponentModifications = val2.AsParallelWriter()
		};
		CopyBudgetDataJob copyBudgetDataJob = new CopyBudgetDataJob
		{
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LoadedIndexType = InternalCompilerInterface.GetBufferTypeHandle<LoadedIndex>(ref __TypeHandle.__Game_Prefabs_LoadedIndex_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabArray = val,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Budgets = InternalCompilerInterface.GetComponentLookup<CollectedCityServiceBudgetData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceBudgetData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Fees = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceFeeData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceFeeData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Upkeeps = InternalCompilerInterface.GetBufferLookup<CollectedCityServiceUpkeepData>(ref __TypeHandle.__Game_Simulation_CollectedCityServiceUpkeepData_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		FillZoneTypeArrayJob fillZoneTypeArrayJob = new FillZoneTypeArrayJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDataType = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneDataType = InternalCompilerInterface.GetComponentTypeHandle<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabArray = val,
			m_ZoneTypeArray = zoneTypeArray
		};
		FixZoneTypeJob obj = new FixZoneTypeJob
		{
			m_ZoneTypeArray = zoneTypeArray,
			m_CellType = InternalCompilerInterface.GetBufferTypeHandle<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VacantLotType = InternalCompilerInterface.GetBufferTypeHandle<VacantLot>(ref __TypeHandle.__Game_Zones_VacantLot_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<FillLoadedPrefabsJob>(fillLoadedPrefabsJob, m_EnabledLoadedPrefabQuery, ((SystemBase)this).Dependency);
		JobHandle.ScheduleBatchedJobs();
		m_PrefabSystem.UpdateLoadedIndices();
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<CopyBudgetDataJob>(copyBudgetDataJob, m_ActualBudgetQuery, val3);
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<CheckActualPrefabsJob>(checkActualPrefabsJob, m_ActualPrefabQuery, val4);
		JobHandle val6 = JobChunkExtensions.ScheduleParallel<FillZoneTypeArrayJob>(fillZoneTypeArrayJob, m_LoadedZonePrefabQuery, val5);
		JobHandle dependencies = JobChunkExtensions.ScheduleParallel<FixZoneTypeJob>(obj, m_LoadedZoneCellQuery, val6);
		((JobHandle)(ref val6)).Complete();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentEnabled<PrefabData>(m_ActualPrefabQuery, false);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentEnabled<PrefabData>(m_EnabledLoadedPrefabQuery, false);
		m_CheckPrefabReferencesSystem.BeginPrefabCheck(val, isLoading: true, dependencies);
		m_UpdateSystem.Update(SystemUpdatePhase.PrefabReferences);
		m_CheckPrefabReferencesSystem.EndPrefabCheck(out var dependencies2);
		((JobHandle)(ref dependencies2)).Complete();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentEnabled<PrefabData>(m_ActualPrefabQuery, true);
		NativeArray<Entity> val7 = ((EntityQuery)(ref m_EnabledLoadedPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentType type = ComponentType.ReadWrite<PlacedSignatureBuildingData>();
		ComponentModification componentModification = default(ComponentModification);
		while (val2.TryDequeue(ref componentModification))
		{
			AddOrRemoveComponent(componentModification, PrefabComponents.PlacedSignatureBuilding, type);
		}
		if (val7.Length != 0)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<LoadedIndex>(val7);
			for (int i = 0; i < val7.Length; i++)
			{
				Entity val8 = val7[i];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val8);
				PrefabID loadedObsoleteID = m_PrefabSystem.GetLoadedObsoleteID(componentData.m_Index);
				componentData.m_Index = -1 - i;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PrefabData>(val8, componentData);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentEnabled<PrefabData>(val8, false);
				m_PrefabSystem.AddObsoleteID(val8, loadedObsoleteID);
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).DestroyEntity(m_AllLoadedPrefabQuery);
		val.Dispose();
		prefabComponents.Dispose();
		zoneTypeArray.Dispose();
		val2.Dispose();
		val7.Dispose();
	}

	private void AddOrRemoveComponent(ComponentModification componentModification, PrefabComponents mask, ComponentType type)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if ((componentModification.m_Remove & mask) != 0)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent(componentModification.m_Entity, type);
		}
		else if ((componentModification.m_Add & mask) != 0)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent(componentModification.m_Entity, type);
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
	public ResolvePrefabsSystem()
	{
	}
}
