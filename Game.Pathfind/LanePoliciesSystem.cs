using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Policies;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class LanePoliciesSystem : GameSystemBase
{
	private enum LaneCheckMask
	{
		ParkingUnknown = 1,
		CarUnknown = 2,
		PedestrianUnknown = 4
	}

	[BurstCompile]
	private struct CheckDistrictLanesJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<BorderDistrict> m_BorderDistrictType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> m_SubLaneType;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public NativeParallelHashMap<Entity, LaneCheckMask> m_CheckDistricts;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<BorderDistrict> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BorderDistrict>(ref m_BorderDistrictType);
			BufferAccessor<Game.Net.SubLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.SubLane>(ref m_SubLaneType);
			LaneCheckMask laneCheckMask2 = default(LaneCheckMask);
			LaneCheckMask laneCheckMask3 = default(LaneCheckMask);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				BorderDistrict borderDistrict = nativeArray[i];
				LaneCheckMask laneCheckMask = (LaneCheckMask)0;
				if (m_CheckDistricts.TryGetValue(borderDistrict.m_Left, ref laneCheckMask2))
				{
					laneCheckMask |= laneCheckMask2;
				}
				if (m_CheckDistricts.TryGetValue(borderDistrict.m_Right, ref laneCheckMask3))
				{
					laneCheckMask |= laneCheckMask3;
				}
				if (laneCheckMask == (LaneCheckMask)0)
				{
					continue;
				}
				DynamicBuffer<Game.Net.SubLane> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					Entity subLane = val[j].m_SubLane;
					if ((laneCheckMask & LaneCheckMask.ParkingUnknown) != 0 && m_ParkingLaneData.HasComponent(subLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, subLane);
					}
					if ((laneCheckMask & LaneCheckMask.CarUnknown) != 0 && m_CarLaneData.HasComponent(subLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, subLane);
					}
					if ((laneCheckMask & LaneCheckMask.PedestrianUnknown) != 0 && m_PedestrianLaneData.HasComponent(subLane))
					{
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(unfilteredChunkIndex, subLane);
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
	private struct CheckBuildingLanesJob : IJobParallelFor
	{
		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public NativeArray<Entity> m_CheckBuildings;

		public ParallelWriter m_CommandBuffer;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_CheckBuildings[index];
			if (m_SubLanes.HasBuffer(val))
			{
				CheckParkingLanes(index, m_SubLanes[val]);
			}
			if (m_SubNets.HasBuffer(val))
			{
				CheckParkingLanes(index, m_SubNets[val]);
			}
			if (m_SubObjects.HasBuffer(val))
			{
				CheckParkingLanes(index, m_SubObjects[val]);
			}
		}

		private void CheckParkingLanes(int jobIndex, DynamicBuffer<Game.Objects.SubObject> subObjects)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subObjects.Length; i++)
			{
				Entity subObject = subObjects[i].m_SubObject;
				if (m_SubLanes.HasBuffer(subObject))
				{
					CheckParkingLanes(jobIndex, m_SubLanes[subObject]);
				}
				if (m_SubObjects.HasBuffer(subObject))
				{
					CheckParkingLanes(jobIndex, m_SubObjects[subObject]);
				}
			}
		}

		private void CheckParkingLanes(int jobIndex, DynamicBuffer<Game.Net.SubNet> subNets)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subNets.Length; i++)
			{
				Entity subNet = subNets[i].m_SubNet;
				if (m_SubLanes.HasBuffer(subNet))
				{
					CheckParkingLanes(jobIndex, m_SubLanes[subNet]);
				}
			}
		}

		private void CheckParkingLanes(int jobIndex, DynamicBuffer<Game.Net.SubLane> subLanes)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < subLanes.Length; i++)
			{
				Entity subLane = subLanes[i].m_SubLane;
				if (m_ParkingLaneData.HasComponent(subLane) || m_GarageLaneData.HasComponent(subLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, subLane);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

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
			__Game_Areas_BorderDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BorderDistrict>(true);
			__Game_Net_SubLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.SubLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_PolicyModifyQuery;

	private EntityQuery m_LaneOwnerQuery;

	private EntityQuery m_CarLaneQuery;

	private EntityQuery m_ParkingLaneQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_PolicyModifyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Modify>() });
		m_LaneOwnerQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<BorderDistrict>(),
			ComponentType.ReadOnly<Game.Net.SubLane>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CarLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ParkingLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PolicyModifyQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Modify> val = ((EntityQuery)(ref m_PolicyModifyQuery)).ToComponentDataArray<Modify>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, LaneCheckMask> checkDistricts = default(NativeParallelHashMap<Entity, LaneCheckMask>);
		NativeList<Entity> val2 = default(NativeList<Entity>);
		LaneCheckMask laneCheckMask = (LaneCheckMask)0;
		CityOptionData optionData = default(CityOptionData);
		DynamicBuffer<CityModifierData> val3 = default(DynamicBuffer<CityModifierData>);
		DistrictOptionData optionData2 = default(DistrictOptionData);
		DynamicBuffer<DistrictModifierData> val4 = default(DynamicBuffer<DistrictModifierData>);
		BuildingOptionData optionData3 = default(BuildingOptionData);
		DynamicBuffer<BuildingModifierData> val5 = default(DynamicBuffer<BuildingModifierData>);
		for (int i = 0; i < val.Length; i++)
		{
			Modify modify = val[i];
			LaneCheckMask laneCheckMask2 = (LaneCheckMask)0;
			bool flag = false;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.City.City>(modify.m_Entity))
			{
				if (EntitiesExtensions.TryGetComponent<CityOptionData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, ref optionData))
				{
					if (CityUtils.HasOption(optionData, CityOption.UnlimitedHighwaySpeed))
					{
						laneCheckMask |= LaneCheckMask.CarUnknown;
					}
					if (CityUtils.HasOption(optionData, CityOption.PaidTaxiStart))
					{
						laneCheckMask |= LaneCheckMask.ParkingUnknown;
					}
				}
				if (EntitiesExtensions.TryGetBuffer<CityModifierData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, true, ref val3))
				{
					for (int j = 0; j < val3.Length; j++)
					{
						if (val3[j].m_Type == CityModifierType.TaxiStartingFee)
						{
							laneCheckMask |= LaneCheckMask.ParkingUnknown;
						}
					}
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<District>(modify.m_Entity))
			{
				if (EntitiesExtensions.TryGetComponent<DistrictOptionData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, ref optionData2))
				{
					if (AreaUtils.HasOption(optionData2, DistrictOption.PaidParking))
					{
						laneCheckMask2 |= LaneCheckMask.ParkingUnknown;
					}
					if (AreaUtils.HasOption(optionData2, DistrictOption.ForbidCombustionEngines))
					{
						laneCheckMask2 |= LaneCheckMask.CarUnknown;
					}
					if (AreaUtils.HasOption(optionData2, DistrictOption.ForbidTransitTraffic))
					{
						laneCheckMask2 |= (LaneCheckMask)6;
					}
					if (AreaUtils.HasOption(optionData2, DistrictOption.ForbidHeavyTraffic))
					{
						laneCheckMask2 |= LaneCheckMask.CarUnknown;
					}
				}
				if (EntitiesExtensions.TryGetBuffer<DistrictModifierData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, true, ref val4))
				{
					for (int k = 0; k < val4.Length; k++)
					{
						switch (val4[k].m_Type)
						{
						case DistrictModifierType.ParkingFee:
							laneCheckMask2 |= LaneCheckMask.ParkingUnknown;
							break;
						case DistrictModifierType.StreetSpeedLimit:
							laneCheckMask2 |= LaneCheckMask.CarUnknown;
							break;
						}
					}
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Building>(modify.m_Entity))
			{
				if (EntitiesExtensions.TryGetComponent<BuildingOptionData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, ref optionData3) && BuildingUtils.HasOption(optionData3, BuildingOption.PaidParking))
				{
					flag = true;
				}
				if (EntitiesExtensions.TryGetBuffer<BuildingModifierData>(((ComponentSystemBase)this).EntityManager, modify.m_Policy, true, ref val5))
				{
					for (int l = 0; l < val5.Length; l++)
					{
						if (val5[l].m_Type == BuildingModifierType.ParkingFee)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (laneCheckMask2 != 0)
			{
				if (!checkDistricts.IsCreated)
				{
					checkDistricts._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)3));
				}
				if (!checkDistricts.TryAdd(modify.m_Entity, laneCheckMask2))
				{
					checkDistricts[modify.m_Entity] = checkDistricts[modify.m_Entity] | laneCheckMask2;
				}
			}
			if (flag)
			{
				if (!val2.IsCreated)
				{
					val2._002Ector(val.Length, AllocatorHandle.op_Implicit((Allocator)3));
				}
				val2.Add(ref modify.m_Entity);
			}
		}
		val.Dispose();
		JobHandle val6 = ((SystemBase)this).Dependency;
		if (laneCheckMask != 0)
		{
			EntityCommandBuffer val7 = m_ModificationBarrier.CreateCommandBuffer();
			if ((laneCheckMask & LaneCheckMask.CarUnknown) != 0)
			{
				((EntityCommandBuffer)(ref val7)).AddComponent<PathfindUpdated>(m_CarLaneQuery, (EntityQueryCaptureMode)1);
			}
			if ((laneCheckMask & LaneCheckMask.ParkingUnknown) != 0)
			{
				((EntityCommandBuffer)(ref val7)).AddComponent<PathfindUpdated>(m_ParkingLaneQuery, (EntityQueryCaptureMode)1);
			}
		}
		EntityCommandBuffer val8;
		if (checkDistricts.IsCreated)
		{
			CheckDistrictLanesJob checkDistrictLanesJob = new CheckDistrictLanesJob
			{
				m_BorderDistrictType = InternalCompilerInterface.GetComponentTypeHandle<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubLaneType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CheckDistricts = checkDistricts
			};
			val8 = m_ModificationBarrier.CreateCommandBuffer();
			checkDistrictLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val8)).AsParallelWriter();
			JobHandle val9 = JobChunkExtensions.ScheduleParallel<CheckDistrictLanesJob>(checkDistrictLanesJob, m_LaneOwnerQuery, ((SystemBase)this).Dependency);
			checkDistricts.Dispose(val9);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val9);
			val6 = JobHandle.CombineDependencies(val6, val9);
		}
		if (val2.IsCreated)
		{
			CheckBuildingLanesJob checkBuildingLanesJob = new CheckBuildingLanesJob
			{
				m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CheckBuildings = val2.AsArray()
			};
			val8 = m_ModificationBarrier.CreateCommandBuffer();
			checkBuildingLanesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val8)).AsParallelWriter();
			JobHandle val10 = IJobParallelForExtensions.Schedule<CheckBuildingLanesJob>(checkBuildingLanesJob, val2.Length, 1, ((SystemBase)this).Dependency);
			val2.Dispose(val10);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val10);
			val6 = JobHandle.CombineDependencies(val6, val10);
		}
		((SystemBase)this).Dependency = val6;
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
	public LanePoliciesSystem()
	{
	}
}
