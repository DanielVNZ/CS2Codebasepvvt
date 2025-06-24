using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class TrafficRoutesSystem : GameSystemBase
{
	private struct LivePathEntityData
	{
		public Entity m_Entity;

		public int m_SegmentCount;

		public bool m_HasNewSegments;
	}

	[BurstCompile]
	private struct FillTargetMapJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocations;

		[ReadOnly]
		public BufferLookup<Renter> m_Renters;

		[ReadOnly]
		public BufferLookup<AggregateElement> m_AggregateElements;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> m_ConnectedRoutes;

		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public int m_SelectedIndex;

		public NativeHashSet<Entity> m_TargetMap;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			m_TargetMap.Add(m_SelectedEntity);
			AddSubLanes(m_SelectedEntity);
			AddSubNets(m_SelectedEntity);
			AddSubAreas(m_SelectedEntity);
			AddSubObjects(m_SelectedEntity);
			DynamicBuffer<SpawnLocationElement> val = default(DynamicBuffer<SpawnLocationElement>);
			if (m_SpawnLocations.TryGetBuffer(m_SelectedEntity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					SpawnLocationElement spawnLocationElement = val[i];
					m_TargetMap.Add(spawnLocationElement.m_SpawnLocation);
				}
				Attached attached = default(Attached);
				if (m_AttachedData.TryGetComponent(m_SelectedEntity, ref attached))
				{
					AddSubLanes(attached.m_Parent);
					AddSubNets(attached.m_Parent);
					AddSubAreas(attached.m_Parent);
					AddSubObjects(attached.m_Parent);
				}
			}
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			if (m_Renters.TryGetBuffer(m_SelectedEntity, ref val2))
			{
				for (int j = 0; j < val2.Length; j++)
				{
					Renter renter = val2[j];
					m_TargetMap.Add(renter.m_Renter);
				}
			}
			DynamicBuffer<AggregateElement> val3 = default(DynamicBuffer<AggregateElement>);
			if (m_AggregateElements.TryGetBuffer(m_SelectedEntity, ref val3))
			{
				if (m_SelectedIndex >= 0 && m_SelectedIndex < val3.Length)
				{
					AddSubLanes(val3[m_SelectedIndex].m_Edge);
				}
				else
				{
					for (int k = 0; k < val3.Length; k++)
					{
						AddSubLanes(val3[k].m_Edge);
					}
				}
			}
			DynamicBuffer<ConnectedRoute> val4 = default(DynamicBuffer<ConnectedRoute>);
			if (m_ConnectedRoutes.TryGetBuffer(m_SelectedEntity, ref val4))
			{
				for (int l = 0; l < val4.Length; l++)
				{
					ConnectedRoute connectedRoute = val4[l];
					m_TargetMap.Add(connectedRoute.m_Waypoint);
				}
			}
			Owner owner = default(Owner);
			if (m_OutsideConnectionData.HasComponent(m_SelectedEntity) && m_OwnerData.TryGetComponent(m_SelectedEntity, ref owner))
			{
				AddSubLanes(owner.m_Owner);
			}
		}

		private void AddSubObjects(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Game.Objects.SubObject subObject = val[i];
					AddSubLanes(subObject.m_SubObject);
					AddSubNets(subObject.m_SubObject);
					AddSubAreas(subObject.m_SubObject);
					AddSubObjects(subObject.m_SubObject);
				}
			}
		}

		private void AddSubNets(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubNet> val = default(DynamicBuffer<Game.Net.SubNet>);
			if (m_SubNets.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					AddSubLanes(val[i].m_SubNet);
				}
			}
		}

		private void AddSubAreas(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Areas.SubArea> val = default(DynamicBuffer<Game.Areas.SubArea>);
			if (m_SubAreas.TryGetBuffer(entity, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Game.Areas.SubArea subArea = val[i];
					AddSubLanes(subArea.m_Area);
					AddSubAreas(subArea.m_Area);
				}
			}
		}

		private void AddSubLanes(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (!m_SubLanes.TryGetBuffer(entity, ref val))
			{
				return;
			}
			for (int i = 0; i < val.Length; i++)
			{
				Game.Net.SubLane subLane = val[i];
				if (subLane.m_PathMethods != 0)
				{
					m_TargetMap.Add(subLane.m_SubLane);
				}
			}
		}
	}

	[BurstCompile]
	private struct FindPathSourcesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> m_HumanCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> m_AircraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> m_TrainCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrames;

		[ReadOnly]
		public BufferTypeHandle<CarNavigationLane> m_CarNavigationLaneType;

		[ReadOnly]
		public BufferTypeHandle<WatercraftNavigationLane> m_WatercraftNavigationLaneType;

		[ReadOnly]
		public BufferTypeHandle<AircraftNavigationLane> m_AircraftNavigationLaneType;

		[ReadOnly]
		public BufferTypeHandle<TrainNavigationLane> m_TrainNavigationLaneType;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public NativeHashSet<Entity> m_TargetMap;

		public ParallelWriter<Entity> m_PathSourceQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Target> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<CurrentVehicle> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			bool flag = nativeArray4.Length != 0 && !((ArchetypeChunk)(ref chunk)).Has<TransformFrame>(ref m_TransformFrames);
			NativeArray<HumanCurrentLane> val = default(NativeArray<HumanCurrentLane>);
			NativeArray<CarCurrentLane> val2 = default(NativeArray<CarCurrentLane>);
			NativeArray<WatercraftCurrentLane> val3 = default(NativeArray<WatercraftCurrentLane>);
			NativeArray<AircraftCurrentLane> val4 = default(NativeArray<AircraftCurrentLane>);
			NativeArray<TrainCurrentLane> val5 = default(NativeArray<TrainCurrentLane>);
			NativeArray<Controller> val6 = default(NativeArray<Controller>);
			BufferAccessor<CarNavigationLane> val7 = default(BufferAccessor<CarNavigationLane>);
			BufferAccessor<WatercraftNavigationLane> val8 = default(BufferAccessor<WatercraftNavigationLane>);
			BufferAccessor<AircraftNavigationLane> val9 = default(BufferAccessor<AircraftNavigationLane>);
			BufferAccessor<TrainNavigationLane> val10 = default(BufferAccessor<TrainNavigationLane>);
			val = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HumanCurrentLane>(ref m_HumanCurrentLaneType);
			if (val.Length == 0 && nativeArray4.Length == 0)
			{
				val2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
				val6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Controller>(ref m_ControllerType);
				if (val2.Length != 0)
				{
					val7 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<CarNavigationLane>(ref m_CarNavigationLaneType);
				}
				else
				{
					val3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
					if (val3.Length != 0)
					{
						val8 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<WatercraftNavigationLane>(ref m_WatercraftNavigationLaneType);
					}
					else
					{
						val4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
						if (val4.Length != 0)
						{
							val9 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<AircraftNavigationLane>(ref m_AircraftNavigationLaneType);
						}
						else
						{
							val5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_TrainCurrentLaneType);
							if (val5.Length != 0)
							{
								val10 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TrainNavigationLane>(ref m_TrainNavigationLaneType);
							}
						}
					}
				}
			}
			PathOwner pathOwner = default(PathOwner);
			DynamicBuffer<PathElement> val11 = default(DynamicBuffer<PathElement>);
			Target target = default(Target);
			HumanCurrentLane humanCurrentLane = default(HumanCurrentLane);
			CarCurrentLane carCurrentLane = default(CarCurrentLane);
			DynamicBuffer<CarNavigationLane> val12 = default(DynamicBuffer<CarNavigationLane>);
			WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
			DynamicBuffer<WatercraftNavigationLane> val13 = default(DynamicBuffer<WatercraftNavigationLane>);
			AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
			DynamicBuffer<AircraftNavigationLane> val14 = default(DynamicBuffer<AircraftNavigationLane>);
			TrainCurrentLane trainCurrentLane = default(TrainCurrentLane);
			DynamicBuffer<TrainNavigationLane> val15 = default(DynamicBuffer<TrainNavigationLane>);
			Controller controller = default(Controller);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (CollectionUtils.TryGet<PathOwner>(nativeArray3, i, ref pathOwner) && CollectionUtils.TryGet<PathElement>(bufferAccessor, i, ref val11))
				{
					int num = pathOwner.m_ElementIndex;
					while (num < val11.Length)
					{
						PathElement pathElement = val11[num];
						if ((pathElement.m_Flags & PathElementFlags.Action) != 0 || !m_TargetMap.Contains(pathElement.m_Target))
						{
							num++;
							continue;
						}
						goto IL_040f;
					}
				}
				if (!CollectionUtils.TryGet<Target>(nativeArray2, i, ref target) || !m_TargetMap.Contains(target.m_Target))
				{
					if (CollectionUtils.TryGet<HumanCurrentLane>(val, i, ref humanCurrentLane))
					{
						if (!m_TargetMap.Contains(humanCurrentLane.m_Lane))
						{
							continue;
						}
					}
					else if (CollectionUtils.TryGet<CarCurrentLane>(val2, i, ref carCurrentLane))
					{
						if (!m_TargetMap.Contains(carCurrentLane.m_Lane))
						{
							if (!CollectionUtils.TryGet<CarNavigationLane>(val7, i, ref val12))
							{
								continue;
							}
							int num2 = 0;
							while (num2 < val12.Length)
							{
								CarNavigationLane carNavigationLane = val12[num2];
								if (!m_TargetMap.Contains(carNavigationLane.m_Lane))
								{
									num2++;
									continue;
								}
								goto IL_040f;
							}
							continue;
						}
					}
					else if (CollectionUtils.TryGet<WatercraftCurrentLane>(val3, i, ref watercraftCurrentLane))
					{
						if (!m_TargetMap.Contains(watercraftCurrentLane.m_Lane))
						{
							if (!CollectionUtils.TryGet<WatercraftNavigationLane>(val8, i, ref val13))
							{
								continue;
							}
							int num3 = 0;
							while (num3 < val13.Length)
							{
								WatercraftNavigationLane watercraftNavigationLane = val13[num3];
								if (!m_TargetMap.Contains(watercraftNavigationLane.m_Lane))
								{
									num3++;
									continue;
								}
								goto IL_040f;
							}
							continue;
						}
					}
					else if (CollectionUtils.TryGet<AircraftCurrentLane>(val4, i, ref aircraftCurrentLane))
					{
						if (!m_TargetMap.Contains(aircraftCurrentLane.m_Lane))
						{
							if (!CollectionUtils.TryGet<AircraftNavigationLane>(val9, i, ref val14))
							{
								continue;
							}
							int num4 = 0;
							while (num4 < val14.Length)
							{
								AircraftNavigationLane aircraftNavigationLane = val14[num4];
								if (!m_TargetMap.Contains(aircraftNavigationLane.m_Lane))
								{
									num4++;
									continue;
								}
								goto IL_040f;
							}
							continue;
						}
					}
					else
					{
						if (!CollectionUtils.TryGet<TrainCurrentLane>(val5, i, ref trainCurrentLane))
						{
							continue;
						}
						if (!m_TargetMap.Contains(trainCurrentLane.m_Front.m_Lane) && !m_TargetMap.Contains(trainCurrentLane.m_Rear.m_Lane))
						{
							if (!CollectionUtils.TryGet<TrainNavigationLane>(val10, i, ref val15))
							{
								continue;
							}
							int num5 = 0;
							while (num5 < val15.Length)
							{
								TrainNavigationLane trainNavigationLane = val15[num5];
								if (!m_TargetMap.Contains(trainNavigationLane.m_Lane))
								{
									num5++;
									continue;
								}
								goto IL_040f;
							}
							continue;
						}
					}
				}
				goto IL_040f;
				IL_040f:
				if (flag)
				{
					CurrentVehicle currentVehicle = nativeArray4[i];
					if (!m_PublicTransportData.HasComponent(currentVehicle.m_Vehicle))
					{
						continue;
					}
				}
				if (CollectionUtils.TryGet<Controller>(val6, i, ref controller) && controller.m_Controller != Entity.Null)
				{
					m_PathSourceQueue.Enqueue(controller.m_Controller);
				}
				else
				{
					m_PathSourceQueue.Enqueue(nativeArray[i]);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLivePathsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_LivePathChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public BufferTypeHandle<RouteSegment> m_RouteSegmentType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<PathSource> m_PathSourceData;

		[ReadOnly]
		public ComponentLookup<Human> m_HumanData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Watercraft> m_WatercraftData;

		[ReadOnly]
		public ComponentLookup<Aircraft> m_AircraftData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> m_CurrentTransportData;

		[ReadOnly]
		public ComponentLookup<RouteData> m_PrefabRouteData;

		[ReadOnly]
		public BufferLookup<Passenger> m_Passengers;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public int m_UpdateFrameIndex;

		[ReadOnly]
		public int m_SourceCountLimit;

		[ReadOnly]
		public RouteConfigurationData m_RouteConfigurationData;

		[NativeDisableContainerSafetyRestriction]
		public NativeQueue<Entity> m_PathSourceQueue;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			NativeHashMap<Entity, LivePathEntityData> livePathEntities = default(NativeHashMap<Entity, LivePathEntityData>);
			livePathEntities._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashMap<Entity, bool> pathSourceFound = default(NativeHashMap<Entity, bool>);
			pathSourceFound._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < m_LivePathChunks.Length; i++)
			{
				ArchetypeChunk val = m_LivePathChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<RouteSegment> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					DynamicBuffer<RouteSegment> val2 = bufferAccessor[j];
					livePathEntities[nativeArray2[j].m_Prefab] = new LivePathEntityData
					{
						m_Entity = nativeArray[j],
						m_SegmentCount = val2.Length
					};
					for (int k = 0; k < val2.Length; k++)
					{
						Entity segment = val2[k].m_Segment;
						pathSourceFound[m_PathSourceData[segment].m_Entity] = false;
					}
				}
			}
			Entity val3 = m_SelectedEntity;
			CurrentTransport currentTransport = default(CurrentTransport);
			if (m_CurrentTransportData.TryGetComponent(val3, ref currentTransport))
			{
				val3 = currentTransport.m_CurrentTransport;
			}
			Controller controller = default(Controller);
			if (m_ControllerData.TryGetComponent(val3, ref controller) && controller.m_Controller != Entity.Null)
			{
				val3 = controller.m_Controller;
			}
			AddLivePath(val3, livePathEntities, pathSourceFound);
			CurrentVehicle currentVehicle = default(CurrentVehicle);
			if (m_CurrentVehicleData.TryGetComponent(val3, ref currentVehicle))
			{
				if (m_ControllerData.TryGetComponent(currentVehicle.m_Vehicle, ref controller) && controller.m_Controller != Entity.Null)
				{
					AddLivePath(controller.m_Controller, livePathEntities, pathSourceFound);
				}
				else
				{
					AddLivePath(currentVehicle.m_Vehicle, livePathEntities, pathSourceFound);
				}
			}
			DynamicBuffer<LayoutElement> val4 = default(DynamicBuffer<LayoutElement>);
			DynamicBuffer<Passenger> val6 = default(DynamicBuffer<Passenger>);
			if (m_LayoutElements.TryGetBuffer(val3, ref val4) && val4.Length != 0)
			{
				DynamicBuffer<Passenger> val5 = default(DynamicBuffer<Passenger>);
				for (int l = 0; l < val4.Length; l++)
				{
					if (m_Passengers.TryGetBuffer(val4[l].m_Vehicle, ref val5))
					{
						for (int m = 0; m < val5.Length; m++)
						{
							AddLivePath(val5[m].m_Passenger, livePathEntities, pathSourceFound);
						}
					}
				}
			}
			else if (m_Passengers.TryGetBuffer(val3, ref val6))
			{
				for (int n = 0; n < val6.Length; n++)
				{
					AddLivePath(val6[n].m_Passenger, livePathEntities, pathSourceFound);
				}
			}
			DynamicBuffer<HouseholdCitizen> val7 = default(DynamicBuffer<HouseholdCitizen>);
			if (m_HouseholdCitizens.TryGetBuffer(val3, ref val7))
			{
				for (int num = 0; num < val7.Length; num++)
				{
					val3 = val7[num].m_Citizen;
					if (m_CurrentTransportData.TryGetComponent(val3, ref currentTransport))
					{
						val3 = currentTransport.m_CurrentTransport;
					}
					AddLivePath(val3, livePathEntities, pathSourceFound);
					if (m_CurrentVehicleData.TryGetComponent(val3, ref currentVehicle))
					{
						if (m_ControllerData.TryGetComponent(currentVehicle.m_Vehicle, ref controller) && controller.m_Controller != Entity.Null)
						{
							AddLivePath(controller.m_Controller, livePathEntities, pathSourceFound);
						}
						else
						{
							AddLivePath(currentVehicle.m_Vehicle, livePathEntities, pathSourceFound);
						}
					}
				}
			}
			if (m_PathSourceQueue.IsCreated)
			{
				Entity sourceEntity = default(Entity);
				while (m_PathSourceQueue.TryDequeue(ref sourceEntity))
				{
					AddLivePath(sourceEntity, livePathEntities, pathSourceFound);
				}
			}
			for (int num2 = 0; num2 < m_LivePathChunks.Length; num2++)
			{
				ArchetypeChunk val8 = m_LivePathChunks[num2];
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val8)).GetNativeArray(m_EntityType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val8)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<RouteSegment> bufferAccessor2 = ((ArchetypeChunk)(ref val8)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
				for (int num3 = 0; num3 < bufferAccessor2.Length; num3++)
				{
					Entity val9 = nativeArray3[num3];
					DynamicBuffer<RouteSegment> val10 = bufferAccessor2[num3];
					int num4 = 0;
					for (int num5 = 0; num5 < val10.Length; num5++)
					{
						RouteSegment routeSegment = val10[num5];
						PathSource pathSource = m_PathSourceData[routeSegment.m_Segment];
						if (!pathSourceFound[pathSource.m_Entity] && GetUpdateFrameIndex(pathSource.m_Entity) == m_UpdateFrameIndex)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(routeSegment.m_Segment);
						}
						else
						{
							val10[num4++] = routeSegment;
						}
					}
					bool flag = livePathEntities[nativeArray4[num3].m_Prefab].m_HasNewSegments;
					if (num4 < val10.Length)
					{
						val10.RemoveRange(num4, val10.Length - num4);
						if (num4 == 0 && !flag)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val9);
						}
					}
					if (flag)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val9);
					}
				}
			}
			livePathEntities.Dispose();
			pathSourceFound.Dispose();
		}

		private int GetUpdateFrameIndex(Entity sourceEntity)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdateFrameIndex == -1)
			{
				return m_UpdateFrameIndex;
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(sourceEntity))
			{
				return m_UpdateFrameIndex;
			}
			EntityStorageInfo val = ((EntityStorageInfoLookup)(ref m_EntityLookup))[sourceEntity];
			if (!((ArchetypeChunk)(ref val.Chunk)).Has<UpdateFrame>(m_UpdateFrameType))
			{
				return m_UpdateFrameIndex;
			}
			return (int)((ArchetypeChunk)(ref val.Chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
		}

		private void AddLivePath(Entity sourceEntity, NativeHashMap<Entity, LivePathEntityData> livePathEntities, NativeHashMap<Entity, bool> pathSourceFound)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PathElements.HasBuffer(sourceEntity))
			{
				return;
			}
			bool flag = default(bool);
			if (pathSourceFound.TryGetValue(sourceEntity, ref flag))
			{
				if (!flag)
				{
					pathSourceFound[sourceEntity] = true;
				}
				return;
			}
			Entity val = (m_HumanData.HasComponent(sourceEntity) ? m_RouteConfigurationData.m_HumanPathVisualization : (m_WatercraftData.HasComponent(sourceEntity) ? m_RouteConfigurationData.m_WatercraftPathVisualization : (m_AircraftData.HasComponent(sourceEntity) ? m_RouteConfigurationData.m_AircraftPathVisualization : ((!m_TrainData.HasComponent(sourceEntity)) ? m_RouteConfigurationData.m_CarPathVisualization : m_RouteConfigurationData.m_TrainPathVisualization))));
			RouteData routeData = m_PrefabRouteData[val];
			LivePathEntityData livePathEntityData = default(LivePathEntityData);
			if (!livePathEntities.TryGetValue(val, ref livePathEntityData))
			{
				livePathEntityData.m_Entity = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(routeData.m_RouteArchetype);
				livePathEntityData.m_SegmentCount = 1;
				livePathEntityData.m_HasNewSegments = true;
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(livePathEntityData.m_Entity, new PrefabRef(val));
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Game.Routes.Color>(livePathEntityData.m_Entity, new Game.Routes.Color(routeData.m_Color));
				livePathEntities[val] = livePathEntityData;
			}
			else
			{
				if (livePathEntityData.m_SegmentCount++ >= m_SourceCountLimit)
				{
					return;
				}
				livePathEntityData.m_HasNewSegments = true;
				livePathEntities[val] = livePathEntityData;
			}
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(routeData.m_SegmentArchetype);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val2, new PrefabRef(val));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Owner>(val2, new Owner(livePathEntityData.m_Entity));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PathSource>(val2, new PathSource
			{
				m_Entity = sourceEntity
			});
			((EntityCommandBuffer)(ref m_CommandBuffer)).AppendToBuffer<RouteSegment>(livePathEntityData.m_Entity, new RouteSegment(val2));
			pathSourceFound.Add(sourceEntity, true);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<AggregateElement> __Game_Net_AggregateElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedRoute> __Game_Routes_ConnectedRoute_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CarNavigationLane> __Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<WatercraftNavigationLane> __Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<AircraftNavigationLane> __Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TrainNavigationLane> __Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public BufferTypeHandle<RouteSegment> __Game_Routes_RouteSegment_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<PathSource> __Game_Routes_PathSource_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Human> __Game_Creatures_Human_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Watercraft> __Game_Vehicles_Watercraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Aircraft> __Game_Vehicles_Aircraft_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RouteData> __Game_Prefabs_RouteData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Passenger> __Game_Vehicles_Passenger_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

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
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Net_AggregateElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<AggregateElement>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Routes_ConnectedRoute_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedRoute>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Pathfind_PathOwner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HumanCurrentLane>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainCurrentLane>(true);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Pathfind_PathElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(true);
			__Game_Objects_TransformFrame_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(true);
			__Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CarNavigationLane>(true);
			__Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<WatercraftNavigationLane>(true);
			__Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AircraftNavigationLane>(true);
			__Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TrainNavigationLane>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Routes_RouteSegment_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteSegment>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Routes_PathSource_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathSource>(true);
			__Game_Creatures_Human_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Human>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Vehicles_Watercraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Watercraft>(true);
			__Game_Vehicles_Aircraft_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Aircraft>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentTransport>(true);
			__Game_Prefabs_RouteData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteData>(true);
			__Game_Vehicles_Passenger_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Passenger>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
		}
	}

	private ModificationBarrier2 m_ModificationBarrier;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_LivePathQuery;

	private EntityQuery m_PathSourceQuery;

	private EntityQuery m_RouteConfigQuery;

	private int m_UpdateFrameIndex;

	private TypeHandle __TypeHandle;

	public bool routesVisible { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier2>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_LivePathQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<LivePath>(),
			ComponentType.ReadOnly<Route>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UpdateFrame>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PathOwner>(),
			ComponentType.ReadOnly<TrainCurrentLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_PathSourceQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_RouteConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RouteConfigurationData>() });
		m_UpdateFrameIndex = -1;
		routesVisible = false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Entity val = (routesVisible ? m_ToolSystem.selected : Entity.Null);
		if (val == Entity.Null && ((EntityQuery)(ref m_LivePathQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		JobHandle val2 = default(JobHandle);
		NativeList<ArchetypeChunk> livePathChunks = ((EntityQuery)(ref m_LivePathQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
		NativeQueue<Entity> pathSourceQueue = default(NativeQueue<Entity>);
		JobHandle val3 = ((SystemBase)this).Dependency;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Building>(val))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Aggregate>(val))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(val))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.Edge>(val))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(val))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(val))
							{
								m_UpdateFrameIndex = -1;
								goto IL_04d2;
							}
						}
					}
				}
			}
		}
		NativeHashSet<Entity> targetMap = default(NativeHashSet<Entity>);
		targetMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)3));
		pathSourceQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		if (++m_UpdateFrameIndex == 16)
		{
			m_UpdateFrameIndex = 0;
		}
		((EntityQuery)(ref m_PathSourceQuery)).ResetFilter();
		((EntityQuery)(ref m_PathSourceQuery)).AddSharedComponentFilter<UpdateFrame>(new UpdateFrame((uint)m_UpdateFrameIndex));
		FillTargetMapJob fillTargetMapJob = new FillTargetMapJob
		{
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocations = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AggregateElements = InternalCompilerInterface.GetBufferLookup<AggregateElement>(ref __TypeHandle.__Game_Net_AggregateElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedRoutes = InternalCompilerInterface.GetBufferLookup<ConnectedRoute>(ref __TypeHandle.__Game_Routes_ConnectedRoute_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectedEntity = val,
			m_SelectedIndex = m_ToolSystem.selectedIndex,
			m_TargetMap = targetMap
		};
		FindPathSourcesJob obj = new FindPathSourcesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrames = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<CarNavigationLane>(ref __TypeHandle.__Game_Vehicles_CarNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<WatercraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_WatercraftNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<AircraftNavigationLane>(ref __TypeHandle.__Game_Vehicles_AircraftNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrainNavigationLaneType = InternalCompilerInterface.GetBufferTypeHandle<TrainNavigationLane>(ref __TypeHandle.__Game_Vehicles_TrainNavigationLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetMap = targetMap,
			m_PathSourceQueue = pathSourceQueue.AsParallelWriter()
		};
		JobHandle val4 = IJobExtensions.Schedule<FillTargetMapJob>(fillTargetMapJob, val3);
		JobHandle val5 = JobChunkExtensions.ScheduleParallel<FindPathSourcesJob>(obj, m_PathSourceQuery, val4);
		targetMap.Dispose(val5);
		val3 = val5;
		goto IL_04d2;
		IL_04d2:
		JobHandle val6 = IJobExtensions.Schedule<UpdateLivePathsJob>(new UpdateLivePathsJob
		{
			m_LivePathChunks = livePathChunks,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteSegmentType = InternalCompilerInterface.GetBufferTypeHandle<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathSourceData = InternalCompilerInterface.GetComponentLookup<PathSource>(ref __TypeHandle.__Game_Routes_PathSource_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanData = InternalCompilerInterface.GetComponentLookup<Human>(ref __TypeHandle.__Game_Creatures_Human_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftData = InternalCompilerInterface.GetComponentLookup<Watercraft>(ref __TypeHandle.__Game_Vehicles_Watercraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftData = InternalCompilerInterface.GetComponentLookup<Aircraft>(ref __TypeHandle.__Game_Vehicles_Aircraft_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportData = InternalCompilerInterface.GetComponentLookup<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRouteData = InternalCompilerInterface.GetComponentLookup<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Passengers = InternalCompilerInterface.GetBufferLookup<Passenger>(ref __TypeHandle.__Game_Vehicles_Passenger_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SelectedEntity = val,
			m_UpdateFrameIndex = m_UpdateFrameIndex,
			m_SourceCountLimit = 200,
			m_RouteConfigurationData = ((EntityQuery)(ref m_RouteConfigQuery)).GetSingleton<RouteConfigurationData>(),
			m_PathSourceQueue = pathSourceQueue,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(val2, val3));
		livePathChunks.Dispose(val6);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val6);
		if (pathSourceQueue.IsCreated)
		{
			pathSourceQueue.Dispose(val6);
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
	public TrafficRoutesSystem()
	{
	}
}
