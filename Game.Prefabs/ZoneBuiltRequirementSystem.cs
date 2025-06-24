using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Serialization;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ZoneBuiltRequirementSystem : GameSystemBase, IPreDeserialize
{
	private struct ZoneBuiltData
	{
		public Entity m_Theme;

		public Entity m_Zone;

		public int m_Squares;

		public int m_Count;

		public AreaType m_Type;

		public byte m_Level;
	}

	[BurstCompile]
	private struct UpdateZoneBuiltDataJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_BuildingChunks;

		public NativeParallelHashMap<ZoneBuiltDataKey, ZoneBuiltDataValue> m_ZoneBuiltData;

		public NativeQueue<ZoneBuiltLevelUpdate> m_ZoneBuiltLevelQueue;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingData;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_BuildingChunks.Length; i++)
			{
				ArchetypeChunk val = m_BuildingChunks[i];
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool flag = ((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					PrefabRef prefabRef = nativeArray[j];
					if (m_SpawnableBuildingData.HasComponent(prefabRef.m_Prefab))
					{
						SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingData[prefabRef.m_Prefab];
						BuildingData buildingData = m_BuildingData[prefabRef.m_Prefab];
						ZoneBuiltDataKey zoneBuiltDataKey = new ZoneBuiltDataKey
						{
							m_Zone = spawnableBuildingData.m_ZonePrefab,
							m_Level = spawnableBuildingData.m_Level
						};
						if (!m_ZoneBuiltData.ContainsKey(zoneBuiltDataKey))
						{
							m_ZoneBuiltData[zoneBuiltDataKey] = new ZoneBuiltDataValue(0, 0);
						}
						int num2;
						int num = (num2 = ((!flag) ? 1 : (-1))) * buildingData.m_LotSize.x * buildingData.m_LotSize.y;
						m_ZoneBuiltData[zoneBuiltDataKey] = new ZoneBuiltDataValue
						{
							m_Count = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey].m_Count + num2),
							m_Squares = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey].m_Squares + num)
						};
					}
				}
			}
			ZoneBuiltLevelUpdate zoneBuiltLevelUpdate = default(ZoneBuiltLevelUpdate);
			while (m_ZoneBuiltLevelQueue.TryDequeue(ref zoneBuiltLevelUpdate))
			{
				ZoneBuiltDataKey zoneBuiltDataKey2 = new ZoneBuiltDataKey
				{
					m_Zone = zoneBuiltLevelUpdate.m_Zone,
					m_Level = zoneBuiltLevelUpdate.m_FromLevel
				};
				ZoneBuiltDataKey zoneBuiltDataKey3 = new ZoneBuiltDataKey
				{
					m_Zone = zoneBuiltLevelUpdate.m_Zone,
					m_Level = zoneBuiltLevelUpdate.m_ToLevel
				};
				if (!m_ZoneBuiltData.ContainsKey(zoneBuiltDataKey2))
				{
					m_ZoneBuiltData[zoneBuiltDataKey2] = new ZoneBuiltDataValue(0, 0);
				}
				if (!m_ZoneBuiltData.ContainsKey(zoneBuiltDataKey3))
				{
					m_ZoneBuiltData[zoneBuiltDataKey3] = new ZoneBuiltDataValue(0, 0);
				}
				m_ZoneBuiltData[zoneBuiltDataKey2] = new ZoneBuiltDataValue
				{
					m_Count = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey2].m_Count - 1),
					m_Squares = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey2].m_Squares - zoneBuiltLevelUpdate.m_Squares)
				};
				m_ZoneBuiltData[zoneBuiltDataKey3] = new ZoneBuiltDataValue
				{
					m_Count = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey3].m_Count + 1),
					m_Squares = math.max(0, m_ZoneBuiltData[zoneBuiltDataKey3].m_Squares + zoneBuiltLevelUpdate.m_Squares)
				};
			}
		}
	}

	[BurstCompile]
	private struct ZoneBuiltRequirementJob : IJobChunk
	{
		[ReadOnly]
		public NativeParallelHashMap<ZoneBuiltDataKey, ZoneBuiltDataValue> m_ZoneBuiltData;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneData;

		[ReadOnly]
		public EntityArchetype m_UnlockEventArchetype;

		[ReadOnly]
		public ComponentLookup<ThemeData> m_ThemeData;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> m_ObjectRequirementElements;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ZoneBuiltRequirementData> m_ZoneBuiltRequirementType;

		public ComponentTypeHandle<UnlockRequirementData> m_UnlockRequirementType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ZoneBuiltRequirementData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ZoneBuiltRequirementData>(ref m_ZoneBuiltRequirementType);
			NativeArray<UnlockRequirementData> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<UnlockRequirementData>(ref m_UnlockRequirementType);
			ChunkEntityEnumerator val = default(ChunkEntityEnumerator);
			((ChunkEntityEnumerator)(ref val))._002Ector(useEnabledMask, chunkEnabledMask, ((ArchetypeChunk)(ref chunk)).Count);
			int num = default(int);
			while (((ChunkEntityEnumerator)(ref val)).NextEntityIndex(ref num))
			{
				ZoneBuiltRequirementData zoneBuiltRequirement = nativeArray2[num];
				UnlockRequirementData unlockRequirement = nativeArray3[num];
				if (ShouldUnlock(zoneBuiltRequirement, ref unlockRequirement))
				{
					Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex, m_UnlockEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Unlock>(unfilteredChunkIndex, val2, new Unlock(nativeArray[num]));
				}
				nativeArray3[num] = unlockRequirement;
			}
		}

		private bool ShouldUnlock(ZoneBuiltRequirementData zoneBuiltRequirement, ref UnlockRequirementData unlockRequirement)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			if (zoneBuiltRequirement.m_RequiredZone != Entity.Null)
			{
				Enumerator<ZoneBuiltDataKey, ZoneBuiltDataValue> enumerator = m_ZoneBuiltData.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValue<ZoneBuiltDataKey, ZoneBuiltDataValue> current = enumerator.Current;
						if (current.Key.m_Zone == zoneBuiltRequirement.m_RequiredZone && current.Key.m_Level >= zoneBuiltRequirement.m_MinimumLevel)
						{
							num += current.Value.m_Squares;
							num2 += current.Value.m_Count;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			else if (zoneBuiltRequirement.m_RequiredTheme != Entity.Null)
			{
				Enumerator<ZoneBuiltDataKey, ZoneBuiltDataValue> enumerator = m_ZoneBuiltData.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValue<ZoneBuiltDataKey, ZoneBuiltDataValue> current2 = enumerator.Current;
						if (!m_ObjectRequirementElements.HasBuffer(current2.Key.m_Zone))
						{
							continue;
						}
						DynamicBuffer<ObjectRequirementElement> val = m_ObjectRequirementElements[current2.Key.m_Zone];
						for (int i = 0; i < val.Length; i++)
						{
							ObjectRequirementElement objectRequirementElement = val[i];
							if (m_ThemeData.HasComponent(objectRequirementElement.m_Requirement) && objectRequirementElement.m_Requirement == zoneBuiltRequirement.m_RequiredTheme)
							{
								num += current2.Value.m_Squares;
								num2 += current2.Value.m_Count;
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			else
			{
				Enumerator<ZoneBuiltDataKey, ZoneBuiltDataValue> enumerator = m_ZoneBuiltData.GetEnumerator();
				try
				{
					ZoneData zoneData = default(ZoneData);
					while (enumerator.MoveNext())
					{
						KeyValue<ZoneBuiltDataKey, ZoneBuiltDataValue> current3 = enumerator.Current;
						if (m_ZoneData.TryGetComponent(current3.Key.m_Zone, ref zoneData) && zoneData.m_AreaType == zoneBuiltRequirement.m_RequiredType && current3.Key.m_Level >= zoneBuiltRequirement.m_MinimumLevel)
						{
							num += current3.Value.m_Squares;
							num2 += current3.Value.m_Count;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
				}
			}
			if (num < zoneBuiltRequirement.m_MinimumSquares || zoneBuiltRequirement.m_MinimumCount == 0)
			{
				unlockRequirement.m_Progress = math.min(num, zoneBuiltRequirement.m_MinimumSquares);
			}
			else
			{
				unlockRequirement.m_Progress = math.min(num2, zoneBuiltRequirement.m_MinimumCount);
			}
			if (num >= zoneBuiltRequirement.m_MinimumSquares)
			{
				return num2 >= zoneBuiltRequirement.m_MinimumCount;
			}
			return false;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ZoneBuiltRequirementData> __Game_Prefabs_ZoneBuiltRequirementData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<UnlockRequirementData> __Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ThemeData> __Game_Prefabs_ThemeData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ObjectRequirementElement> __Game_Prefabs_ObjectRequirementElement_RO_BufferLookup;

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
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_ZoneBuiltRequirementData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneBuiltRequirementData>(true);
			__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UnlockRequirementData>(false);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_ThemeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ThemeData>(true);
			__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ObjectRequirementElement>(true);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_UpdatedBuildingsQuery;

	private EntityQuery m_AllBuildingsQuery;

	private EntityQuery m_RequirementQuery;

	private EntityArchetype m_UnlockEventArchetype;

	private NativeParallelHashMap<ZoneBuiltDataKey, ZoneBuiltDataValue> m_ZoneBuiltData;

	private NativeQueue<ZoneBuiltLevelUpdate> m_ZoneBuiltLevelQueue;

	private JobHandle m_WriteDeps;

	private JobHandle m_QueueWriteDeps;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	public NativeQueue<ZoneBuiltLevelUpdate> GetZoneBuiltLevelQueue(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_QueueWriteDeps;
		return m_ZoneBuiltLevelQueue;
	}

	public void AddWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_QueueWriteDeps = JobHandle.CombineDependencies(jobHandle, m_QueueWriteDeps);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AllBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Temp>()
		});
		m_RequirementQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ZoneBuiltRequirementData>(),
			ComponentType.ReadWrite<UnlockRequirementData>(),
			ComponentType.ReadOnly<Locked>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_UnlockEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<Unlock>()
		});
		m_ZoneBuiltData = new NativeParallelHashMap<ZoneBuiltDataKey, ZoneBuiltDataValue>(20, AllocatorHandle.op_Implicit((Allocator)4));
		m_ZoneBuiltLevelQueue = new NativeQueue<ZoneBuiltLevelUpdate>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ZoneBuiltData.Dispose();
		m_ZoneBuiltLevelQueue.Dispose();
		base.OnDestroy();
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllBuildingsQuery : m_UpdatedBuildingsQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter || !m_ZoneBuiltLevelQueue.IsEmpty())
		{
			JobHandle val2 = default(JobHandle);
			NativeList<ArchetypeChunk> buildingChunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			JobHandle val3 = IJobExtensions.Schedule<UpdateZoneBuiltDataJob>(new UpdateZoneBuiltDataJob
			{
				m_BuildingChunks = buildingChunks,
				m_ZoneBuiltData = m_ZoneBuiltData,
				m_ZoneBuiltLevelQueue = m_ZoneBuiltLevelQueue,
				m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2, m_QueueWriteDeps));
			buildingChunks.Dispose(val3);
			m_WriteDeps = val3;
			if (!((EntityQuery)(ref m_RequirementQuery)).IsEmptyIgnoreFilter)
			{
				ZoneBuiltRequirementJob zoneBuiltRequirementJob = new ZoneBuiltRequirementJob
				{
					m_ZoneBuiltData = m_ZoneBuiltData,
					m_UnlockEventArchetype = m_UnlockEventArchetype
				};
				EntityCommandBuffer val4 = m_ModificationBarrier.CreateCommandBuffer();
				zoneBuiltRequirementJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
				zoneBuiltRequirementJob.m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
				zoneBuiltRequirementJob.m_ZoneBuiltRequirementType = InternalCompilerInterface.GetComponentTypeHandle<ZoneBuiltRequirementData>(ref __TypeHandle.__Game_Prefabs_ZoneBuiltRequirementData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				zoneBuiltRequirementJob.m_UnlockRequirementType = InternalCompilerInterface.GetComponentTypeHandle<UnlockRequirementData>(ref __TypeHandle.__Game_Prefabs_UnlockRequirementData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
				zoneBuiltRequirementJob.m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				zoneBuiltRequirementJob.m_ThemeData = InternalCompilerInterface.GetComponentLookup<ThemeData>(ref __TypeHandle.__Game_Prefabs_ThemeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				zoneBuiltRequirementJob.m_ObjectRequirementElements = InternalCompilerInterface.GetBufferLookup<ObjectRequirementElement>(ref __TypeHandle.__Game_Prefabs_ObjectRequirementElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
				JobHandle val5 = JobChunkExtensions.ScheduleParallel<ZoneBuiltRequirementJob>(zoneBuiltRequirementJob, m_RequirementQuery, val3);
				((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val5);
				((SystemBase)this).Dependency = val5;
			}
			else
			{
				((SystemBase)this).Dependency = val3;
			}
		}
	}

	public void PreDeserialize(Context context)
	{
		((JobHandle)(ref m_WriteDeps)).Complete();
		m_ZoneBuiltData.Clear();
		m_Loaded = true;
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
	public ZoneBuiltRequirementSystem()
	{
	}
}
