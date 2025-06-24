using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Creatures;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TrafficSpawnerAISystem : GameSystemBase
{
	[BurstCompile]
	private struct TrafficSpawnerTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TrafficSpawner> m_TrafficSpawnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> m_CreatureDataType;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> m_ResidentDataType;

		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<TrafficSpawnerData> m_PrefabTrafficSpawnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> m_PrefabDeliveryTruckData;

		[ReadOnly]
		public ComponentLookup<ObjectData> m_PrefabObjectData;

		[ReadOnly]
		public ComponentLookup<RandomTrafficRequest> m_RandomTrafficRequestData;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocationElements;

		[ReadOnly]
		public float m_Loading;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_VehicleRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public DeliveryTruckSelectData m_DeliveryTruckSelectData;

		[ReadOnly]
		public PersonalCarSelectData m_PersonalCarSelectData;

		[ReadOnly]
		public TransportVehicleSelectData m_TransportVehicleSelectData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_CreaturePrefabChunks;

		[ReadOnly]
		public ComponentTypeSet m_CurrentLaneTypesRelative;

		public ParallelWriter m_CommandBuffer;

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
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.TrafficSpawner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TrafficSpawner>(ref m_TrafficSpawnerType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<ServiceDispatch> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Game.Buildings.TrafficSpawner trafficSpawner = nativeArray2[i];
				PrefabRef prefabRef = nativeArray3[i];
				DynamicBuffer<ServiceDispatch> dispatches = bufferAccessor[i];
				Tick(unfilteredChunkIndex, entity, ref random, trafficSpawner, prefabRef, dispatches);
			}
		}

		private void Tick(int jobIndex, Entity entity, ref Random random, Game.Buildings.TrafficSpawner trafficSpawner, PrefabRef prefabRef, DynamicBuffer<ServiceDispatch> dispatches)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			TrafficSpawnerData prefabTrafficSpawnerData = m_PrefabTrafficSpawnerData[prefabRef.m_Prefab];
			float num = prefabTrafficSpawnerData.m_SpawnRate * 4.266667f;
			float num2 = ((Random)(ref random)).NextFloat(num * 0.5f, num * 1.5f);
			if (MathUtils.RoundToIntRandom(ref random, num2) > 0 && !m_RandomTrafficRequestData.HasComponent(trafficSpawner.m_TrafficRequest))
			{
				RequestVehicle(jobIndex, ref random, entity, prefabTrafficSpawnerData);
			}
			for (int i = 0; i < dispatches.Length; i++)
			{
				Entity request = dispatches[i].m_Request;
				if (m_RandomTrafficRequestData.HasComponent(request))
				{
					int num3 = ((!(m_Loading < 0.9f)) ? 1 : (((prefabTrafficSpawnerData.m_RoadType & RoadTypes.Airplane) != RoadTypes.None) ? ((Random)(ref random)).NextInt(2) : (((prefabTrafficSpawnerData.m_TrackType & TrackTypes.Train) == 0) ? 2 : 0)));
					for (int j = 0; j < num3; j++)
					{
						SpawnVehicle(jobIndex, ref random, entity, request, prefabTrafficSpawnerData);
					}
					dispatches.RemoveAt(i--);
				}
				else if (!m_ServiceRequestData.HasComponent(request))
				{
					dispatches.RemoveAt(i--);
				}
			}
		}

		private void RequestVehicle(int jobIndex, ref Random random, Entity entity, TrafficSpawnerData prefabTrafficSpawnerData)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			SizeClass sizeClass = SizeClass.Small;
			RandomTrafficRequestFlags randomTrafficRequestFlags = (RandomTrafficRequestFlags)0;
			if ((prefabTrafficSpawnerData.m_RoadType & RoadTypes.Car) != RoadTypes.None)
			{
				int num = ((Random)(ref random)).NextInt(100);
				if (num < 20)
				{
					sizeClass = SizeClass.Large;
					randomTrafficRequestFlags |= RandomTrafficRequestFlags.DeliveryTruck;
				}
				else if (num < 25)
				{
					sizeClass = SizeClass.Large;
					randomTrafficRequestFlags |= RandomTrafficRequestFlags.TransportVehicle;
				}
			}
			else
			{
				sizeClass = SizeClass.Large;
				randomTrafficRequestFlags |= RandomTrafficRequestFlags.TransportVehicle;
			}
			if (prefabTrafficSpawnerData.m_NoSlowVehicles)
			{
				randomTrafficRequestFlags |= RandomTrafficRequestFlags.NoSlowVehicles;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_VehicleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RandomTrafficRequest>(jobIndex, val, new RandomTrafficRequest(entity, prefabTrafficSpawnerData.m_RoadType, prefabTrafficSpawnerData.m_TrackType, EnergyTypes.FuelAndElectricity, sizeClass, randomTrafficRequestFlags));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(16u));
		}

		private void SpawnVehicle(int jobIndex, ref Random random, Entity entity, Entity request, TrafficSpawnerData prefabTrafficSpawnerData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			RandomTrafficRequest randomTrafficRequest = default(RandomTrafficRequest);
			PathInformation pathInformation = default(PathInformation);
			if (!m_RandomTrafficRequestData.TryGetComponent(request, ref randomTrafficRequest) || !m_PathInformationData.TryGetComponent(request, ref pathInformation) || !m_PrefabRefData.HasComponent(pathInformation.m_Destination))
			{
				return;
			}
			uint delay = ((Random)(ref random)).NextUInt(256u);
			Entity val = entity;
			Transform transform = m_TransformData[entity];
			int num = 0;
			DynamicBuffer<PathElement> sourceElements = default(DynamicBuffer<PathElement>);
			m_PathElements.TryGetBuffer(request, ref sourceElements);
			if (m_Loading < 0.9f)
			{
				delay = 0u;
				val = Entity.Null;
				if (sourceElements.IsCreated && sourceElements.Length >= 5)
				{
					num = ((Random)(ref random)).NextInt(2, sourceElements.Length * 3 / 4);
					PathElement pathElement = sourceElements[num];
					Curve curve = default(Curve);
					if (m_CurveData.TryGetComponent(pathElement.m_Target, ref curve))
					{
						float3 val2 = MathUtils.Tangent(curve.m_Bezier, pathElement.m_TargetDelta.x);
						val2 = math.select(val2, -val2, pathElement.m_TargetDelta.y < pathElement.m_TargetDelta.x);
						transform.m_Position = MathUtils.Position(curve.m_Bezier, pathElement.m_TargetDelta.x);
						transform.m_Rotation = quaternion.LookRotationSafe(val2, math.up());
					}
				}
			}
			Entity val3 = Entity.Null;
			if ((randomTrafficRequest.m_Flags & RandomTrafficRequestFlags.DeliveryTruck) != 0)
			{
				Resource randomResource = GetRandomResource(ref random);
				m_DeliveryTruckSelectData.GetCapacityRange(Resource.NoResource, out var _, out var max);
				int amount = ((Random)(ref random)).NextInt(1, max + max / 10 + 1);
				int returnAmount = 0;
				DeliveryTruckFlags deliveryTruckFlags = DeliveryTruckFlags.DummyTraffic;
				if (((Random)(ref random)).NextInt(100) < 75)
				{
					deliveryTruckFlags |= DeliveryTruckFlags.Loaded;
				}
				if (m_DeliveryTruckSelectData.TrySelectItem(ref random, randomResource, amount, out var item))
				{
					val3 = m_DeliveryTruckSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, ref m_PrefabDeliveryTruckData, ref m_PrefabObjectData, item, randomResource, Resource.NoResource, ref amount, ref returnAmount, transform, val, deliveryTruckFlags, delay);
				}
				int maxCount = 1;
				if (CreatePassengers(jobIndex, val3, item.m_Prefab1, transform, driver: true, ref maxCount, ref random) > 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Passenger>(jobIndex, val3);
				}
			}
			else if ((randomTrafficRequest.m_Flags & RandomTrafficRequestFlags.TransportVehicle) != 0)
			{
				TransportType transportType = TransportType.None;
				PublicTransportPurpose publicTransportPurpose = (PublicTransportPurpose)0;
				Resource resource = Resource.NoResource;
				int2 passengerCapacity = int2.op_Implicit(0);
				int2 cargoCapacity = int2.op_Implicit(0);
				if ((randomTrafficRequest.m_RoadType & RoadTypes.Car) != RoadTypes.None)
				{
					transportType = TransportType.Bus;
					publicTransportPurpose = PublicTransportPurpose.TransportLine;
					((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
				}
				else if ((randomTrafficRequest.m_RoadType & RoadTypes.Airplane) != RoadTypes.None)
				{
					transportType = TransportType.Airplane;
					if (((Random)(ref random)).NextInt(100) < 25)
					{
						resource = Resource.Food;
						((int2)(ref cargoCapacity))._002Ector(1, int.MaxValue);
					}
					else
					{
						publicTransportPurpose = PublicTransportPurpose.TransportLine;
						((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
					}
				}
				else if ((randomTrafficRequest.m_RoadType & RoadTypes.Watercraft) != RoadTypes.None)
				{
					transportType = TransportType.Ship;
					if (((Random)(ref random)).NextInt(100) < 50)
					{
						resource = Resource.Food;
						((int2)(ref cargoCapacity))._002Ector(1, int.MaxValue);
					}
					else
					{
						publicTransportPurpose = PublicTransportPurpose.TransportLine;
						((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
					}
				}
				else if ((randomTrafficRequest.m_TrackType & TrackTypes.Train) != TrackTypes.None)
				{
					transportType = TransportType.Train;
					if (((Random)(ref random)).NextInt(100) < 50)
					{
						resource = Resource.Food;
						((int2)(ref cargoCapacity))._002Ector(1, int.MaxValue);
					}
					else
					{
						publicTransportPurpose = PublicTransportPurpose.TransportLine;
						((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
					}
				}
				val3 = m_TransportVehicleSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, transform, val, Entity.Null, Entity.Null, transportType, randomTrafficRequest.m_EnergyTypes, randomTrafficRequest.m_SizeClass, publicTransportPurpose, resource, ref passengerCapacity, ref cargoCapacity, parked: false);
				if (val3 != Entity.Null)
				{
					if (publicTransportPurpose != 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.PublicTransport>(jobIndex, val3, new Game.Vehicles.PublicTransport
						{
							m_State = PublicTransportFlags.DummyTraffic
						});
					}
					if (resource != Resource.NoResource)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Vehicles.CargoTransport>(jobIndex, val3, new Game.Vehicles.CargoTransport
						{
							m_State = CargoTransportFlags.DummyTraffic
						});
						DynamicBuffer<LoadingResources> val4 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<LoadingResources>(jobIndex, val3);
						int num2 = ((Random)(ref random)).NextInt(1, math.min(5, cargoCapacity.y + 1));
						int num3 = ((Random)(ref random)).NextInt(num2, cargoCapacity.y + cargoCapacity.y / 10 + 1);
						int num4 = 0;
						for (int i = 0; i < num2; i++)
						{
							int num5 = ((Random)(ref random)).NextInt(1, 100000);
							num4 += num5;
							val4.Add(new LoadingResources
							{
								m_Resource = GetRandomResource(ref random),
								m_Amount = num5
							});
						}
						for (int j = 0; j < num2; j++)
						{
							LoadingResources loadingResources = val4[j];
							int amount2 = loadingResources.m_Amount;
							loadingResources.m_Amount = (int)(((long)amount2 * (long)num3 + (num4 >> 1)) / num4);
							num4 -= amount2;
							num3 -= loadingResources.m_Amount;
							val4[j] = loadingResources;
						}
					}
				}
			}
			else
			{
				int maxCount2 = ((Random)(ref random)).NextInt(1, 6);
				int num6 = ((Random)(ref random)).NextInt(1, 6);
				if (((Random)(ref random)).NextInt(20) == 0)
				{
					maxCount2 += 5;
					num6 += 5;
				}
				else if (((Random)(ref random)).NextInt(10) == 0)
				{
					num6 += 5;
					if (((Random)(ref random)).NextInt(10) == 0)
					{
						num6 += 5;
					}
				}
				bool noSlowVehicles = prefabTrafficSpawnerData.m_NoSlowVehicles | ((randomTrafficRequest.m_Flags & RandomTrafficRequestFlags.NoSlowVehicles) != 0);
				val3 = m_PersonalCarSelectData.CreateVehicle(m_CommandBuffer, jobIndex, ref random, maxCount2, num6, avoidTrailers: false, noSlowVehicles, transform, val, Entity.Null, PersonalCarFlags.DummyTraffic, stopped: false, delay, out var trailer, out var vehiclePrefab, out var trailerPrefab);
				CreatePassengers(jobIndex, val3, vehiclePrefab, transform, driver: true, ref maxCount2, ref random);
				CreatePassengers(jobIndex, trailer, trailerPrefab, transform, driver: false, ref maxCount2, ref random);
			}
			if (val3 == Entity.Null)
			{
				return;
			}
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Target>(jobIndex, val3, new Target(pathInformation.m_Destination));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val3, new Owner(entity));
			Entity val5 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val5, new HandleRequest(request, val3, completed: true));
			if (val == Entity.Null)
			{
				if ((randomTrafficRequest.m_RoadType & RoadTypes.Car) != RoadTypes.None)
				{
					CarCurrentLane carCurrentLane = default(CarCurrentLane);
					carCurrentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.ResetSpeed;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(jobIndex, val3, carCurrentLane);
				}
				else if ((randomTrafficRequest.m_RoadType & RoadTypes.Airplane) != RoadTypes.None)
				{
					AircraftCurrentLane aircraftCurrentLane = default(AircraftCurrentLane);
					aircraftCurrentLane.m_LaneFlags |= AircraftLaneFlags.ResetSpeed | AircraftLaneFlags.Flying;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AircraftCurrentLane>(jobIndex, val3, aircraftCurrentLane);
				}
				else if ((randomTrafficRequest.m_RoadType & RoadTypes.Watercraft) != RoadTypes.None)
				{
					WatercraftCurrentLane watercraftCurrentLane = default(WatercraftCurrentLane);
					watercraftCurrentLane.m_LaneFlags |= WatercraftLaneFlags.ResetSpeed;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<WatercraftCurrentLane>(jobIndex, val3, watercraftCurrentLane);
				}
			}
			if (sourceElements.IsCreated && sourceElements.Length != 0)
			{
				DynamicBuffer<PathElement> targetElements = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<PathElement>(jobIndex, val3);
				PathUtils.CopyPath(sourceElements, default(PathOwner), 0, targetElements);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathOwner>(jobIndex, val3, new PathOwner(num, PathFlags.Updated));
				if ((randomTrafficRequest.m_Flags & RandomTrafficRequestFlags.DeliveryTruck) != 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathInformation>(jobIndex, val3, pathInformation);
				}
			}
		}

		private int CreatePassengers(int jobIndex, Entity vehicleEntity, Entity vehiclePrefab, Transform transform, bool driver, ref int maxCount, ref Random random)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
			if (maxCount > 0 && m_ActivityLocationElements.TryGetBuffer(vehiclePrefab, ref val))
			{
				ActivityMask activityMask = new ActivityMask(ActivityType.Driving);
				int num2 = 0;
				int num3 = -1;
				float num4 = float.MinValue;
				for (int i = 0; i < val.Length; i++)
				{
					ActivityLocationElement activityLocationElement = val[i];
					if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
					{
						num2++;
						bool flag = ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertLefthandTraffic) != 0 && m_LeftHandTraffic) || ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertRighthandTraffic) != 0 && !m_LeftHandTraffic);
						activityLocationElement.m_Position.x = math.select(activityLocationElement.m_Position.x, 0f - activityLocationElement.m_Position.x, flag);
						if ((!(math.abs(activityLocationElement.m_Position.x) >= 0.5f) || activityLocationElement.m_Position.x >= 0f == m_LeftHandTraffic) && activityLocationElement.m_Position.z > num4)
						{
							num3 = i;
							num4 = activityLocationElement.m_Position.z;
						}
					}
				}
				int num5 = 100;
				if (driver && num3 != -1)
				{
					maxCount--;
					num2--;
				}
				if (num2 > maxCount)
				{
					num5 = maxCount * 100 / num2;
				}
				Relative relative = default(Relative);
				for (int j = 0; j < val.Length; j++)
				{
					ActivityLocationElement activityLocationElement2 = val[j];
					if ((activityLocationElement2.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0 && ((driver && j == num3) || ((Random)(ref random)).NextInt(100) >= num5))
					{
						relative.m_Position = activityLocationElement2.m_Position;
						relative.m_Rotation = activityLocationElement2.m_Rotation;
						relative.m_BoneIndex = new int3(0, -1, -1);
						Citizen citizenData = default(Citizen);
						if (((Random)(ref random)).NextBool())
						{
							citizenData.m_State |= CitizenFlags.Male;
						}
						if (driver)
						{
							citizenData.SetAge(CitizenAge.Adult);
						}
						else
						{
							citizenData.SetAge((CitizenAge)((Random)(ref random)).NextInt(4));
						}
						citizenData.m_PseudoRandom = (ushort)(((Random)(ref random)).NextUInt() % 65536);
						CreatureData creatureData;
						PseudoRandomSeed randomSeed;
						Entity val2 = ObjectEmergeSystem.SelectResidentPrefab(citizenData, m_CreaturePrefabChunks, m_EntityType, ref m_CreatureDataType, ref m_ResidentDataType, out creatureData, out randomSeed);
						ObjectData objectData = m_PrefabObjectData[val2];
						PrefabRef prefabRef = new PrefabRef
						{
							m_Prefab = val2
						};
						Game.Creatures.Resident resident = default(Game.Creatures.Resident);
						resident.m_Flags |= ResidentFlags.InVehicle | ResidentFlags.DummyTraffic;
						CurrentVehicle currentVehicle = new CurrentVehicle
						{
							m_Vehicle = vehicleEntity
						};
						currentVehicle.m_Flags |= CreatureVehicleFlags.Ready;
						if (driver && j == num3)
						{
							currentVehicle.m_Flags |= CreatureVehicleFlags.Leader | CreatureVehicleFlags.Driver;
						}
						Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, objectData.m_Archetype);
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(jobIndex, val3, ref m_CurrentLaneTypesRelative);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(jobIndex, val3, transform);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val3, prefabRef);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Game.Creatures.Resident>(jobIndex, val3, resident);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(jobIndex, val3, randomSeed);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CurrentVehicle>(jobIndex, val3, currentVehicle);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Relative>(jobIndex, val3, relative);
						num++;
					}
				}
			}
			return num;
		}

		private Resource GetRandomResource(ref Random random)
		{
			return ((Random)(ref random)).NextInt(31) switch
			{
				0 => Resource.Grain, 
				1 => Resource.ConvenienceFood, 
				2 => Resource.Food, 
				3 => Resource.Vegetables, 
				4 => Resource.Meals, 
				5 => Resource.Wood, 
				6 => Resource.Timber, 
				7 => Resource.Paper, 
				8 => Resource.Furniture, 
				9 => Resource.Vehicles, 
				10 => Resource.UnsortedMail, 
				11 => Resource.Oil, 
				12 => Resource.Petrochemicals, 
				13 => Resource.Ore, 
				14 => Resource.Plastics, 
				15 => Resource.Metals, 
				16 => Resource.Electronics, 
				17 => Resource.Coal, 
				18 => Resource.Stone, 
				19 => Resource.Livestock, 
				20 => Resource.Cotton, 
				21 => Resource.Steel, 
				22 => Resource.Minerals, 
				23 => Resource.Concrete, 
				24 => Resource.Machinery, 
				25 => Resource.Chemicals, 
				26 => Resource.Pharmaceuticals, 
				27 => Resource.Beverages, 
				28 => Resource.Textiles, 
				29 => Resource.Garbage, 
				30 => Resource.Fish, 
				_ => Resource.NoResource, 
			};
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
		public ComponentTypeHandle<Game.Buildings.TrafficSpawner> __Game_Buildings_TrafficSpawner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CreatureData> __Game_Prefabs_CreatureData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ResidentData> __Game_Prefabs_ResidentData_RO_ComponentTypeHandle;

		public BufferTypeHandle<ServiceDispatch> __Game_Simulation_ServiceDispatch_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<TrafficSpawnerData> __Game_Prefabs_TrafficSpawnerData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectData> __Game_Prefabs_ObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RandomTrafficRequest> __Game_Simulation_RandomTrafficRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<PathElement> __Game_Pathfind_PathElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> __Game_Prefabs_ActivityLocationElement_RO_BufferLookup;

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
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_TrafficSpawner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TrafficSpawner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_CreatureData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CreatureData>(true);
			__Game_Prefabs_ResidentData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResidentData>(true);
			__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ServiceDispatch>(false);
			__Game_Prefabs_TrafficSpawnerData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficSpawnerData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeliveryTruckData>(true);
			__Game_Prefabs_ObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectData>(true);
			__Game_Simulation_RandomTrafficRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RandomTrafficRequest>(true);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Pathfind_PathElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PathElement>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
		}
	}

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_PersonalCarQuery;

	private EntityQuery m_TransportVehicleQuery;

	private EntityQuery m_CreaturePrefabQuery;

	private SimulationSystem m_SimulationSystem;

	private ClimateSystem m_ClimateSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private VehicleCapacitySystem m_VehicleCapacitySystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityArchetype m_TrafficRequestArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private ComponentTypeSet m_CurrentLaneTypesRelative;

	private PersonalCarSelectData m_PersonalCarSelectData;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		if (phase == SystemUpdatePhase.LoadSimulation)
		{
			return 16;
		}
		return 256;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		if (phase == SystemUpdatePhase.LoadSimulation)
		{
			return 2;
		}
		return 32;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_VehicleCapacitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VehicleCapacitySystem>();
		m_PersonalCarSelectData = new PersonalCarSelectData((SystemBase)(object)this);
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Buildings.TrafficSpawner>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PersonalCarQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PersonalCarSelectData.GetEntityQueryDesc() });
		m_TransportVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		m_CreaturePrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CreatureData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_TrafficRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<RandomTrafficRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Event>()
		});
		m_CurrentLaneTypesRelative = new ComponentTypeSet((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<HumanNavigation>(),
			ComponentType.ReadWrite<HumanCurrentLane>(),
			ComponentType.ReadWrite<Blocker>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		m_PersonalCarSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_PersonalCarQuery, (Allocator)3, out var jobHandle);
		m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_TransportVehicleQuery, (Allocator)3, out var jobHandle2);
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> creaturePrefabChunks = ((EntityQuery)(ref m_CreaturePrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		TrafficSpawnerTickJob trafficSpawnerTickJob = new TrafficSpawnerTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficSpawnerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TrafficSpawner>(ref __TypeHandle.__Game_Buildings_TrafficSpawner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureDataType = InternalCompilerInterface.GetComponentTypeHandle<CreatureData>(ref __TypeHandle.__Game_Prefabs_CreatureData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentDataType = InternalCompilerInterface.GetComponentTypeHandle<ResidentData>(ref __TypeHandle.__Game_Prefabs_ResidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceDispatchType = InternalCompilerInterface.GetBufferTypeHandle<ServiceDispatch>(ref __TypeHandle.__Game_Simulation_ServiceDispatch_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrafficSpawnerData = InternalCompilerInterface.GetComponentLookup<TrafficSpawnerData>(ref __TypeHandle.__Game_Prefabs_TrafficSpawnerData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeliveryTruckData = InternalCompilerInterface.GetComponentLookup<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectData = InternalCompilerInterface.GetComponentLookup<ObjectData>(ref __TypeHandle.__Game_Prefabs_ObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RandomTrafficRequestData = InternalCompilerInterface.GetComponentLookup<RandomTrafficRequest>(ref __TypeHandle.__Game_Simulation_RandomTrafficRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathElements = InternalCompilerInterface.GetBufferLookup<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocationElements = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Loading = m_SimulationSystem.loadingProgress,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_RandomSeed = RandomSeed.Next(),
			m_VehicleRequestArchetype = m_TrafficRequestArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_DeliveryTruckSelectData = m_VehicleCapacitySystem.GetDeliveryTruckSelectData(),
			m_PersonalCarSelectData = m_PersonalCarSelectData,
			m_TransportVehicleSelectData = m_TransportVehicleSelectData,
			m_CreaturePrefabChunks = creaturePrefabChunks,
			m_CurrentLaneTypesRelative = m_CurrentLaneTypesRelative
		};
		EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
		trafficSpawnerTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		JobHandle val3 = JobChunkExtensions.ScheduleParallel<TrafficSpawnerTickJob>(trafficSpawnerTickJob, m_BuildingQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, jobHandle, jobHandle2, val));
		m_PersonalCarSelectData.PostUpdate(val3);
		m_TransportVehicleSelectData.PostUpdate(val3);
		m_EndFrameBarrier.AddJobHandleForProducer(val3);
		creaturePrefabChunks.Dispose(val3);
		((SystemBase)this).Dependency = val3;
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
	public TrafficSpawnerAISystem()
	{
	}
}
