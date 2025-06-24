using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Vehicles;

[CompilerGenerated]
public class ParkedVehiclesSystem : GameSystemBase
{
	private struct ParkingLocation
	{
		public Game.Net.ParkingLane m_ParkingLane;

		public ParkingLaneData m_ParkingLaneData;

		public TrackTypes m_TrackTypes;

		public SpawnLocationType m_SpawnLocationType;

		public Transform m_OwnerTransform;

		public Curve m_Curve;

		public Entity m_Lane;

		public float2 m_MaxSize;

		public float m_CurvePos;
	}

	private struct DeletedVehicleData
	{
		public Entity m_Entity;

		public Entity m_SecondaryPrefab;

		public Transform m_Transform;
	}

	[BurstCompile]
	private struct FindParkingLocationsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_PrefabParkingLaneData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> m_SpawnLocationElements;

		[ReadOnly]
		public BufferLookup<LaneOverlap> m_LaneOverlaps;

		[ReadOnly]
		public Entity m_Entity;

		public NativeList<ParkingLocation> m_Locations;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<SpawnLocationElement> val = default(DynamicBuffer<SpawnLocationElement>);
			if (!m_SpawnLocationElements.TryGetBuffer(m_Entity, ref val))
			{
				return;
			}
			PrefabRef prefabRef = default(PrefabRef);
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
			for (int i = 0; i < val.Length; i++)
			{
				SpawnLocationElement spawnLocationElement = val[i];
				switch (spawnLocationElement.m_Type)
				{
				case SpawnLocationType.SpawnLocation:
					if (m_PrefabRefData.TryGetComponent(spawnLocationElement.m_SpawnLocation, ref prefabRef) && m_PrefabSpawnLocationData.TryGetComponent(prefabRef.m_Prefab, ref spawnLocationData))
					{
						CheckSpawnLocation(spawnLocationElement.m_SpawnLocation, spawnLocationData);
					}
					break;
				case SpawnLocationType.ParkingLane:
					if (m_ParkingLaneData.TryGetComponent(spawnLocationElement.m_SpawnLocation, ref parkingLane))
					{
						CheckParkingLane(spawnLocationElement.m_SpawnLocation, parkingLane);
					}
					break;
				}
			}
		}

		private void CheckSpawnLocation(Entity entity, SpawnLocationData spawnLocationData)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if (((spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None && spawnLocationData.m_ConnectionType == RouteConnectionType.Air) || spawnLocationData.m_ConnectionType == RouteConnectionType.Track)
			{
				Transform ownerTransform = m_TransformData[entity];
				ref NativeList<ParkingLocation> reference = ref m_Locations;
				ParkingLocation parkingLocation = new ParkingLocation
				{
					m_ParkingLaneData = new ParkingLaneData
					{
						m_RoadTypes = spawnLocationData.m_RoadTypes
					},
					m_TrackTypes = spawnLocationData.m_TrackTypes,
					m_SpawnLocationType = SpawnLocationType.SpawnLocation,
					m_OwnerTransform = ownerTransform,
					m_Lane = entity,
					m_MaxSize = float2.op_Implicit(float.MaxValue),
					m_CurvePos = 0f
				};
				reference.Add(ref parkingLocation);
			}
		}

		private void CheckParkingLane(Entity lane, Game.Net.ParkingLane parkingLane)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			if ((parkingLane.m_Flags & (ParkingLaneFlags.VirtualLane | ParkingLaneFlags.SpecialVehicles)) != ParkingLaneFlags.SpecialVehicles)
			{
				return;
			}
			Curve curve = m_CurveData[lane];
			PrefabRef prefabRef = m_PrefabRefData[lane];
			DynamicBuffer<LaneOverlap> val = m_LaneOverlaps[lane];
			ParkingLaneData parkingLaneData = m_PrefabParkingLaneData[prefabRef.m_Prefab];
			float2 parkingSize = VehicleUtils.GetParkingSize(parkingLaneData);
			Transform ownerTransform = default(Transform);
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(lane, ref owner))
			{
				m_TransformData.TryGetComponent(owner.m_Owner, ref ownerTransform);
			}
			if (parkingLaneData.m_SlotInterval == 0f)
			{
				return;
			}
			int parkingSlotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, parkingLaneData);
			float parkingSlotInterval = NetUtils.GetParkingSlotInterval(curve, parkingLane, parkingLaneData, parkingSlotCount);
			float3 val2 = curve.m_Bezier.a;
			float2 val3 = float2.op_Implicit(0f);
			float num = 0f;
			float num2 = math.max((parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane)) switch
			{
				ParkingLaneFlags.StartingLane => curve.m_Length - (float)parkingSlotCount * parkingSlotInterval, 
				ParkingLaneFlags.EndingLane => 0f, 
				_ => (curve.m_Length - (float)parkingSlotCount * parkingSlotInterval) * 0.5f, 
			}, 0f);
			int i = -1;
			float2 val4 = float2.op_Implicit(2f);
			int num3 = 0;
			if (num3 < val.Length)
			{
				LaneOverlap laneOverlap = val[num3++];
				val4 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
			}
			for (int j = 1; j <= 16; j++)
			{
				float num4 = (float)j * 0.0625f;
				float3 val5 = MathUtils.Position(curve.m_Bezier, num4);
				for (num += math.distance(val2, val5); num >= num2 || (j == 16 && i < parkingSlotCount); i++)
				{
					val3.y = math.select(num4, math.lerp(val3.x, num4, num2 / num), num2 < num);
					bool flag = false;
					if (val4.x < val3.y)
					{
						flag = true;
						if (val4.y <= val3.y)
						{
							val4 = float2.op_Implicit(2f);
							while (num3 < val.Length)
							{
								LaneOverlap laneOverlap2 = val[num3++];
								float2 val6 = new float2((float)(int)laneOverlap2.m_ThisStart, (float)(int)laneOverlap2.m_ThisEnd) * 0.003921569f;
								if (val6.y > val3.y)
								{
									val4 = val6;
									break;
								}
							}
						}
					}
					if (!flag && i >= 0 && i < parkingSlotCount)
					{
						float curvePos = math.lerp(val3.x, val3.y, 0.5f);
						ref NativeList<ParkingLocation> reference = ref m_Locations;
						ParkingLocation parkingLocation = new ParkingLocation
						{
							m_ParkingLane = parkingLane,
							m_ParkingLaneData = parkingLaneData,
							m_SpawnLocationType = SpawnLocationType.ParkingLane,
							m_OwnerTransform = ownerTransform,
							m_Curve = curve,
							m_Lane = lane,
							m_MaxSize = parkingSize,
							m_CurvePos = curvePos
						};
						reference.Add(ref parkingLocation);
					}
					num -= num2;
					val3.x = val3.y;
					num2 = parkingSlotInterval;
				}
				val2 = val5;
			}
		}
	}

	[BurstCompile]
	private struct CollectDeletedVehiclesJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			m_DeletedVehicleMap.Capacity = num;
			Controller controller = default(Controller);
			DynamicBuffer<LayoutElement> layoutElements = default(DynamicBuffer<LayoutElement>);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val2 = m_Chunks[j];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<Controller> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Controller>(ref m_ControllerType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					Transform transform = nativeArray2[k];
					PrefabRef prefabRef = nativeArray4[k];
					DeletedVehicleData deletedVehicleData = new DeletedVehicleData
					{
						m_Entity = val3,
						m_Transform = transform
					};
					if (!EntitiesExtensions.HasEnabledComponent<PrefabData>(m_PrefabData, prefabRef.m_Prefab) || (CollectionUtils.TryGet<Controller>(nativeArray3, k, ref controller) && controller.m_Controller != val3))
					{
						continue;
					}
					if (CollectionUtils.TryGet<LayoutElement>(bufferAccessor, k, ref layoutElements))
					{
						deletedVehicleData.m_SecondaryPrefab = GetSecondaryPrefab(prefabRef.m_Prefab, layoutElements, ref m_PrefabRefData, ref m_PrefabData, out var validLayout);
						if (!validLayout)
						{
							continue;
						}
					}
					m_DeletedVehicleMap.Add(prefabRef.m_Prefab, deletedVehicleData);
				}
			}
		}
	}

	[BurstCompile]
	private struct DuplicateVehiclesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Helicopter> m_HelicopterData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<Train> m_TrainData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> m_PrefabMovingObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<TrainData> m_PrefabTrainData;

		[ReadOnly]
		public ComponentLookup<TrainObjectData> m_PrefabTrainObjectData;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		public NativeList<ParkingLocation> m_Locations;

		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0626: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			Transform parentTransform = m_TransformData[m_Entity];
			Transform inverseParentTransform = ObjectUtils.InverseTransform(m_TransformData[m_Temp.m_Original]);
			DynamicBuffer<OwnedVehicle> val = m_OwnedVehicles[m_Temp.m_Original];
			NativeList<LayoutElement> val2 = default(NativeList<LayoutElement>);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			MovingObjectData movingObjectData = default(MovingObjectData);
			DynamicBuffer<LayoutElement> val3 = default(DynamicBuffer<LayoutElement>);
			TrainData trainData = default(TrainData);
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			TrainObjectData trainObjectData = default(TrainObjectData);
			TrainObjectData trainObjectData2 = default(TrainObjectData);
			Train train = default(Train);
			for (int i = 0; i < val.Length; i++)
			{
				OwnedVehicle ownedVehicle = val[i];
				bool flag = m_ParkedCarData.HasComponent(ownedVehicle.m_Vehicle);
				bool flag2 = m_ParkedTrainData.HasComponent(ownedVehicle.m_Vehicle);
				if (!flag && !flag2)
				{
					continue;
				}
				PrefabRef prefabRef = m_PrefabRefData[ownedVehicle.m_Vehicle];
				if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) || !m_PrefabMovingObjectData.TryGetComponent(prefabRef.m_Prefab, ref movingObjectData) || !m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab))
				{
					continue;
				}
				Entity secondaryPrefab = Entity.Null;
				if (m_LayoutElements.TryGetBuffer(ownedVehicle.m_Vehicle, ref val3))
				{
					secondaryPrefab = GetSecondaryPrefab(prefabRef.m_Prefab, val3, ref m_PrefabRefData, ref m_PrefabData, out var validLayout);
					if (!validLayout)
					{
						continue;
					}
				}
				NativeArray<LayoutElement> val4 = default(NativeArray<LayoutElement>);
				Transform transform = ObjectUtils.WorldToLocal(inverseParentTransform, m_TransformData[ownedVehicle.m_Vehicle]);
				transform = ObjectUtils.LocalToWorld(parentTransform, transform);
				bool flag3 = (m_Temp.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0;
				Entity lane = Entity.Null;
				float curvePosition = 0f;
				if (!flag3)
				{
					RoadTypes roadType = RoadTypes.None;
					TrackTypes trackType = TrackTypes.None;
					if (m_HelicopterData.HasComponent(ownedVehicle.m_Vehicle))
					{
						roadType = RoadTypes.Helicopter;
					}
					else if (m_PrefabTrainData.TryGetComponent(prefabRef.m_Prefab, ref trainData))
					{
						trackType = trainData.m_TrackType;
					}
					else
					{
						roadType = RoadTypes.Car;
					}
					SelectParkingSpace(objectGeometryData, roadType, trackType, ref transform, out lane, out curvePosition);
				}
				Entity val5 = FindDeletedVehicle(prefabRef.m_Prefab, secondaryPrefab, transform, m_DeletedVehicleMap);
				if (((m_LayoutElements.TryGetBuffer(val5, ref layout) && layout.Length != 0) || (val3.IsCreated && val3.Length != 0)) && !AreEqual(val5, ownedVehicle.m_Vehicle, layout, val3))
				{
					val5 = Entity.Null;
				}
				if ((flag && m_ParkedCarData.HasComponent(val5)) || (flag2 && m_ParkedTrainData.HasComponent(val5)))
				{
					if (layout.IsCreated && layout.Length != 0)
					{
						val4 = layout.AsNativeArray();
						for (int j = 0; j < layout.Length; j++)
						{
							Entity vehicle = layout[j].m_Vehicle;
							((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(vehicle);
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(vehicle);
						}
					}
					else
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val5);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val5);
					}
				}
				else if (val3.IsCreated && val3.Length != 0)
				{
					if (!val2.IsCreated)
					{
						val2._002Ector(val3.Length, AllocatorHandle.op_Implicit((Allocator)2));
					}
					for (int k = 0; k < val3.Length; k++)
					{
						Entity vehicle2 = val3[k].m_Vehicle;
						PrefabRef prefabRef2 = m_PrefabRefData[vehicle2];
						Entity val6;
						if (vehicle2 == ownedVehicle.m_Vehicle)
						{
							val6 = ((!m_PrefabTrainObjectData.TryGetComponent(prefabRef2.m_Prefab, ref trainObjectData)) ? ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(movingObjectData.m_StoppedArchetype) : ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(trainObjectData.m_StoppedControllerArchetype));
							val5 = val6;
						}
						else
						{
							MovingObjectData movingObjectData2 = m_PrefabMovingObjectData[prefabRef2.m_Prefab];
							val6 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(movingObjectData2.m_StoppedArchetype);
						}
						LayoutElement layoutElement = new LayoutElement(val6);
						val2.Add(ref layoutElement);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val6, prefabRef2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val6, default(Animation));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val6, default(InterpolatedTransform));
					}
					val4 = val2.AsArray();
				}
				else
				{
					val5 = ((!m_PrefabTrainObjectData.TryGetComponent(prefabRef.m_Prefab, ref trainObjectData2)) ? ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(movingObjectData.m_StoppedArchetype) : ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(trainObjectData2.m_StoppedControllerArchetype));
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val5, prefabRef);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val5, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val5, default(InterpolatedTransform));
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val5, new Owner(m_Entity));
				Temp temp = default(Temp);
				if (!flag3 && lane == Entity.Null)
				{
					temp.m_Flags = TempFlags.Delete | TempFlags.Hidden;
				}
				else
				{
					temp.m_Flags = m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate);
				}
				if (flag3 && m_UnspawnedData.HasComponent(ownedVehicle.m_Vehicle))
				{
					temp.m_Flags |= TempFlags.Hidden;
				}
				if (val4.IsCreated)
				{
					for (int l = 0; l < val4.Length; l++)
					{
						Entity vehicle3 = val4[l].m_Vehicle;
						temp.m_Original = val3[l].m_Vehicle;
						if (flag)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(vehicle3, new ParkedCar(lane, curvePosition));
						}
						if (flag2)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedTrain>(vehicle3, new ParkedTrain(lane));
							if (m_TrainData.TryGetComponent(temp.m_Original, ref train))
							{
								train.m_Flags &= TrainFlags.Reversed;
								((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Train>(vehicle3, train);
							}
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(vehicle3, temp);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(vehicle3, transform);
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(vehicle3, m_PseudoRandomSeedData[temp.m_Original]);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(temp.m_Original, default(Hidden));
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(temp.m_Original, default(BatchesUpdated));
					}
				}
				else
				{
					temp.m_Original = ownedVehicle.m_Vehicle;
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val5, new ParkedCar(lane, curvePosition));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val5, temp);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val5, transform);
					((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PseudoRandomSeed>(val5, m_PseudoRandomSeedData[temp.m_Original]);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Hidden>(temp.m_Original, default(Hidden));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(temp.m_Original, default(BatchesUpdated));
				}
				if (!val2.IsCreated || val2.Length == 0)
				{
					continue;
				}
				for (int m = 0; m < val4.Length; m++)
				{
					if (m_ControllerData.HasComponent(val3[m].m_Vehicle))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Controller>(val4[m].m_Vehicle, new Controller(val5));
					}
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetBuffer<LayoutElement>(val5).CopyFrom(val4);
				val2.Clear();
			}
			if (val2.IsCreated)
			{
				val2.Dispose();
			}
		}

		private bool AreEqual(Entity entity1, Entity entity2, DynamicBuffer<LayoutElement> layout1, DynamicBuffer<LayoutElement> layout2)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (!layout1.IsCreated || !layout2.IsCreated)
			{
				return false;
			}
			if (layout1.Length != layout2.Length)
			{
				return false;
			}
			for (int i = 0; i < layout1.Length; i++)
			{
				Entity vehicle = layout1[i].m_Vehicle;
				Entity vehicle2 = layout2[i].m_Vehicle;
				if (vehicle == entity1 != (vehicle2 == entity2))
				{
					return false;
				}
				if (m_PrefabRefData[vehicle].m_Prefab != m_PrefabRefData[vehicle2].m_Prefab)
				{
					return false;
				}
			}
			return true;
		}

		private bool SelectParkingSpace(ObjectGeometryData objectGeometryData, RoadTypes roadType, TrackTypes trackType, ref Transform transform, out Entity lane, out float curvePosition)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			float offset;
			float2 parkingSize = VehicleUtils.GetParkingSize(objectGeometryData, out offset);
			Transform transform2 = default(Transform);
			float num = float.MaxValue;
			int num2 = -1;
			for (int i = 0; i < m_Locations.Length; i++)
			{
				ParkingLocation parkingLocation = m_Locations[i];
				if (!math.any(parkingSize > parkingLocation.m_MaxSize) && ((parkingLocation.m_ParkingLaneData.m_RoadTypes & roadType) != RoadTypes.None || (parkingLocation.m_TrackTypes & trackType) != TrackTypes.None))
				{
					Transform transform3 = ((parkingLocation.m_SpawnLocationType != SpawnLocationType.ParkingLane) ? parkingLocation.m_OwnerTransform : VehicleUtils.CalculateParkingSpaceTarget(parkingLocation.m_ParkingLane, parkingLocation.m_ParkingLaneData, objectGeometryData, parkingLocation.m_Curve, parkingLocation.m_OwnerTransform, parkingLocation.m_CurvePos));
					float num3 = math.distancesq(transform.m_Position, transform3.m_Position);
					if (num3 < num)
					{
						transform2 = transform3;
						num = num3;
						num2 = i;
					}
				}
			}
			if (num2 != -1)
			{
				ParkingLocation parkingLocation2 = m_Locations[num2];
				transform = transform2;
				lane = parkingLocation2.m_Lane;
				curvePosition = parkingLocation2.m_CurvePos;
				if (parkingLocation2.m_SpawnLocationType == SpawnLocationType.ParkingLane && parkingLocation2.m_ParkingLaneData.m_SlotAngle <= 0.25f)
				{
					if (offset > 0f)
					{
						Bounds1 val = default(Bounds1);
						((Bounds1)(ref val))._002Ector(curvePosition, 1f);
						MathUtils.ClampLength(parkingLocation2.m_Curve.m_Bezier, ref val, offset);
						curvePosition = val.max;
					}
					else if (offset < 0f)
					{
						Bounds1 val2 = default(Bounds1);
						((Bounds1)(ref val2))._002Ector(0f, curvePosition);
						MathUtils.ClampLengthInverse(parkingLocation2.m_Curve.m_Bezier, ref val2, 0f - offset);
						curvePosition = val2.min;
					}
					transform = VehicleUtils.CalculateParkingSpaceTarget(parkingLocation2.m_ParkingLane, parkingLocation2.m_ParkingLaneData, objectGeometryData, parkingLocation2.m_Curve, parkingLocation2.m_OwnerTransform, curvePosition);
				}
				m_Locations.RemoveAtSwapBack(num2);
				return true;
			}
			transform = default(Transform);
			lane = Entity.Null;
			curvePosition = 0f;
			return false;
		}
	}

	[BurstCompile]
	private struct SpawnPoliceCarsJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> m_PrefabPoliceStationData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public PoliceCarSelectData m_PoliceCarSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<PoliceStationData>(m_Entity, out PoliceStationData data, ref m_PrefabRefData, ref m_PrefabPoliceStationData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null && UpgradeUtils.TryGetCombinedComponent<PoliceStationData>(m_Temp.m_Original, out PoliceStationData data2, ref m_PrefabRefData, ref m_PrefabPoliceStationData, ref m_InstalledUpgrades))
			{
				data.m_PatrolCarCapacity -= data2.m_PatrolCarCapacity;
				data.m_PoliceHelicopterCapacity -= data2.m_PoliceHelicopterCapacity;
			}
			for (int i = 0; i < data.m_PatrolCarCapacity; i++)
			{
				CreateVehicle(ref random, data, RoadTypes.Car);
			}
			for (int j = 0; j < data.m_PoliceHelicopterCapacity; j++)
			{
				CreateVehicle(ref random, data, RoadTypes.Helicopter);
			}
		}

		private void CreateVehicle(ref Random random, PoliceStationData policeStationData, RoadTypes roadType)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			PolicePurpose purposeMask = policeStationData.m_PurposeMask;
			Entity val = m_PoliceCarSelectData.SelectVehicle(ref random, ref purposeMask, roadType);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_PoliceCarSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, ref purposeMask, roadType, parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PoliceCar>(val2, new PoliceCar(PoliceCarFlags.Empty | PoliceCarFlags.Disabled, 0, policeStationData.m_PurposeMask & purposeMask));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	[BurstCompile]
	private struct SpawnFireEnginesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<FireStationData> m_PrefabFireStationData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public FireEngineSelectData m_FireEngineSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<FireStationData>(m_Entity, out FireStationData data, ref m_PrefabRefData, ref m_PrefabFireStationData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null && UpgradeUtils.TryGetCombinedComponent<FireStationData>(m_Temp.m_Original, out FireStationData data2, ref m_PrefabRefData, ref m_PrefabFireStationData, ref m_InstalledUpgrades))
			{
				data.m_FireEngineCapacity -= data2.m_FireEngineCapacity;
				data.m_FireHelicopterCapacity -= data2.m_FireHelicopterCapacity;
				data.m_DisasterResponseCapacity -= data2.m_DisasterResponseCapacity;
			}
			for (int i = 0; i < data.m_FireEngineCapacity; i++)
			{
				CreateVehicle(ref random, ref data, RoadTypes.Car);
			}
			for (int j = 0; j < data.m_FireHelicopterCapacity; j++)
			{
				CreateVehicle(ref random, ref data, RoadTypes.Helicopter);
			}
		}

		private void CreateVehicle(ref Random random, ref FireStationData fireStationData, RoadTypes roadType)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			float2 extinguishingCapacity = default(float2);
			((float2)(ref extinguishingCapacity))._002Ector(float.Epsilon, float.MaxValue);
			Entity val = m_FireEngineSelectData.SelectVehicle(ref random, ref extinguishingCapacity, roadType);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_FireEngineSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, ref extinguishingCapacity, roadType, parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			FireEngineFlags fireEngineFlags = FireEngineFlags.Disabled;
			if (fireStationData.m_DisasterResponseCapacity > 0)
			{
				fireEngineFlags |= FireEngineFlags.DisasterResponse;
				fireStationData.m_DisasterResponseCapacity--;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<FireEngine>(val2, new FireEngine(fireEngineFlags, 0, extinguishingCapacity.y, fireStationData.m_VehicleEfficiency));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	[BurstCompile]
	private struct SpawnHealthcareVehiclesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<HospitalData> m_PrefabHospitalData;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> m_PrefabDeathcareFacilityData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public HealthcareVehicleSelectData m_HealthcareVehicleSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<HospitalData>(m_Entity, out HospitalData data, ref m_PrefabRefData, ref m_PrefabHospitalData, ref m_InstalledUpgrades);
			UpgradeUtils.TryGetCombinedComponent<DeathcareFacilityData>(m_Entity, out DeathcareFacilityData data2, ref m_PrefabRefData, ref m_PrefabDeathcareFacilityData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null)
			{
				if (UpgradeUtils.TryGetCombinedComponent<HospitalData>(m_Temp.m_Original, out HospitalData data3, ref m_PrefabRefData, ref m_PrefabHospitalData, ref m_InstalledUpgrades))
				{
					data.m_AmbulanceCapacity -= data3.m_AmbulanceCapacity;
					data.m_MedicalHelicopterCapacity -= data3.m_MedicalHelicopterCapacity;
				}
				if (UpgradeUtils.TryGetCombinedComponent<DeathcareFacilityData>(m_Temp.m_Original, out DeathcareFacilityData data4, ref m_PrefabRefData, ref m_PrefabDeathcareFacilityData, ref m_InstalledUpgrades))
				{
					data2.m_HearseCapacity -= data4.m_HearseCapacity;
				}
			}
			for (int i = 0; i < data.m_AmbulanceCapacity; i++)
			{
				CreateVehicle(ref random, HealthcareRequestType.Ambulance, RoadTypes.Car);
			}
			for (int j = 0; j < data2.m_HearseCapacity; j++)
			{
				CreateVehicle(ref random, HealthcareRequestType.Hearse, RoadTypes.Car);
			}
			for (int k = 0; k < data.m_MedicalHelicopterCapacity; k++)
			{
				CreateVehicle(ref random, HealthcareRequestType.Ambulance, RoadTypes.Helicopter);
			}
		}

		private void CreateVehicle(ref Random random, HealthcareRequestType healthcareType, RoadTypes roadType)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_HealthcareVehicleSelectData.SelectVehicle(ref random, healthcareType, roadType);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_HealthcareVehicleSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, healthcareType, roadType, parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			if (healthcareType == HealthcareRequestType.Ambulance)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Ambulance>(val2, new Ambulance(Entity.Null, Entity.Null, AmbulanceFlags.Disabled));
			}
			else
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Hearse>(val2, new Hearse(Entity.Null, HearseFlags.Disabled));
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	[BurstCompile]
	private struct SpawnTransportVehiclesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		[ReadOnly]
		public ComponentLookup<PrisonData> m_PrefabPrisonData;

		[ReadOnly]
		public ComponentLookup<EmergencyShelterData> m_PrefabEmergencyShelterData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public TransportVehicleSelectData m_TransportVehicleSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public ParallelWriter m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<TransportDepotData>(m_Entity, out TransportDepotData data, ref m_PrefabRefData, ref m_PrefabTransportDepotData, ref m_InstalledUpgrades);
			UpgradeUtils.TryGetCombinedComponent<PrisonData>(m_Entity, out PrisonData data2, ref m_PrefabRefData, ref m_PrefabPrisonData, ref m_InstalledUpgrades);
			UpgradeUtils.TryGetCombinedComponent<EmergencyShelterData>(m_Entity, out EmergencyShelterData data3, ref m_PrefabRefData, ref m_PrefabEmergencyShelterData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null)
			{
				if (UpgradeUtils.TryGetCombinedComponent<TransportDepotData>(m_Temp.m_Original, out TransportDepotData data4, ref m_PrefabRefData, ref m_PrefabTransportDepotData, ref m_InstalledUpgrades))
				{
					data.m_VehicleCapacity -= data4.m_VehicleCapacity;
				}
				if (UpgradeUtils.TryGetCombinedComponent<PrisonData>(m_Temp.m_Original, out PrisonData data5, ref m_PrefabRefData, ref m_PrefabPrisonData, ref m_InstalledUpgrades))
				{
					data2.m_PrisonVanCapacity -= data5.m_PrisonVanCapacity;
				}
				if (UpgradeUtils.TryGetCombinedComponent<EmergencyShelterData>(m_Temp.m_Original, out EmergencyShelterData data6, ref m_PrefabRefData, ref m_PrefabEmergencyShelterData, ref m_InstalledUpgrades))
				{
					data3.m_VehicleCapacity -= data6.m_VehicleCapacity;
				}
			}
			NativeList<LayoutElement> layoutBuffer = default(NativeList<LayoutElement>);
			RoadTypes roadType = RoadTypes.None;
			TrackTypes trackType = TrackTypes.None;
			bool flag = false;
			switch (data.m_TransportType)
			{
			case TransportType.Bus:
				roadType = RoadTypes.Car;
				break;
			case TransportType.Taxi:
				roadType = RoadTypes.Car;
				break;
			case TransportType.Train:
				trackType = TrackTypes.Train;
				flag = true;
				break;
			case TransportType.Tram:
				trackType = TrackTypes.Tram;
				break;
			case TransportType.Subway:
				trackType = TrackTypes.Subway;
				break;
			default:
				data.m_VehicleCapacity = 0;
				break;
			}
			for (int i = 0; i < data.m_VehicleCapacity; i++)
			{
				PublicTransportPurpose publicTransportPurpose = (PublicTransportPurpose)0;
				Resource cargoResource = Resource.NoResource;
				if (flag && ((Random)(ref random)).NextBool())
				{
					cargoResource = Resource.Food;
				}
				else
				{
					publicTransportPurpose = ((data.m_TransportType != TransportType.Taxi) ? PublicTransportPurpose.TransportLine : ((PublicTransportPurpose)0));
				}
				CreateVehicle(ref random, data.m_TransportType, data.m_EnergyTypes, data.m_SizeClass, publicTransportPurpose, cargoResource, roadType, trackType, (PublicTransportFlags)0u, ref layoutBuffer);
			}
			for (int j = 0; j < data2.m_PrisonVanCapacity; j++)
			{
				CreateVehicle(ref random, TransportType.Bus, EnergyTypes.FuelAndElectricity, SizeClass.Large, PublicTransportPurpose.PrisonerTransport, Resource.NoResource, RoadTypes.Car, TrackTypes.None, PublicTransportFlags.PrisonerTransport, ref layoutBuffer);
			}
			for (int k = 0; k < data3.m_VehicleCapacity; k++)
			{
				CreateVehicle(ref random, TransportType.Bus, EnergyTypes.FuelAndElectricity, SizeClass.Large, PublicTransportPurpose.Evacuation, Resource.NoResource, RoadTypes.Car, TrackTypes.None, PublicTransportFlags.Evacuating, ref layoutBuffer);
			}
			if (layoutBuffer.IsCreated)
			{
				layoutBuffer.Dispose();
			}
		}

		private void CreateVehicle(ref Random random, TransportType transportType, EnergyTypes energyTypes, SizeClass sizeClass, PublicTransportPurpose publicTransportPurpose, Resource cargoResource, RoadTypes roadType, TrackTypes trackType, PublicTransportFlags publicTransportFlags, ref NativeList<LayoutElement> layoutBuffer)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			int2 passengerCapacity = int2.op_Implicit(0);
			int2 cargoCapacity = int2.op_Implicit(0);
			if (cargoResource != Resource.NoResource)
			{
				((int2)(ref cargoCapacity))._002Ector(1, int.MaxValue);
			}
			else
			{
				((int2)(ref passengerCapacity))._002Ector(1, int.MaxValue);
			}
			m_TransportVehicleSelectData.SelectVehicle(ref random, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResource, out var primaryPrefab, out var secondaryPrefab, ref passengerCapacity, ref cargoCapacity);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(primaryPrefab, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, trackType, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val = FindDeletedVehicle(primaryPrefab, secondaryPrefab, transform, m_DeletedVehicleMap);
			NativeArray<LayoutElement> val2 = default(NativeArray<LayoutElement>);
			if (val != Entity.Null)
			{
				DynamicBuffer<LayoutElement> val3 = default(DynamicBuffer<LayoutElement>);
				if (m_LayoutElements.TryGetBuffer(val, ref val3) && val3.Length != 0)
				{
					val2 = val3.AsNativeArray();
					for (int i = 0; i < val3.Length; i++)
					{
						Entity vehicle = val3[i].m_Vehicle;
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(0, vehicle);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(0, vehicle, transform);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(0, vehicle);
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Deleted>(0, val);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(0, val, transform);
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(0, val);
				}
			}
			else
			{
				val = m_TransportVehicleSelectData.CreateVehicle(m_CommandBuffer, 0, ref random, transform, m_Entity, primaryPrefab, secondaryPrefab, transportType, energyTypes, sizeClass, publicTransportPurpose, cargoResource, ref passengerCapacity, ref cargoCapacity, parked: true, ref layoutBuffer);
				if (layoutBuffer.IsCreated && layoutBuffer.Length != 0)
				{
					val2 = layoutBuffer.AsArray();
					if (m_IsTemp)
					{
						for (int j = 0; j < layoutBuffer.Length; j++)
						{
							Entity vehicle2 = layoutBuffer[j].m_Vehicle;
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Animation>(0, vehicle2, default(Animation));
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(0, vehicle2, default(InterpolatedTransform));
						}
					}
				}
				else if (m_IsTemp)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Animation>(0, val, default(Animation));
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(0, val, default(InterpolatedTransform));
				}
			}
			if (transportType == TransportType.Taxi)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Taxi>(0, val, new Taxi(TaxiFlags.Disabled));
			}
			if (publicTransportPurpose != 0)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PublicTransport>(0, val, new PublicTransport
				{
					m_State = (PublicTransportFlags.Disabled | publicTransportFlags)
				});
			}
			if (cargoResource != Resource.NoResource)
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CargoTransport>(0, val, new CargoTransport
				{
					m_State = CargoTransportFlags.Disabled
				});
			}
			if (val2.IsCreated)
			{
				for (int k = 0; k < val2.Length; k++)
				{
					Entity vehicle3 = val2[k].m_Vehicle;
					if (roadType != RoadTypes.None)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(0, vehicle3, new ParkedCar(lane, curvePosition));
					}
					if (trackType != TrackTypes.None)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedTrain>(0, vehicle3, new ParkedTrain(lane));
					}
				}
			}
			else
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ParkedCar>(0, val, new ParkedCar(lane, curvePosition));
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(0, val, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				if (val2.IsCreated)
				{
					for (int l = 0; l < val2.Length; l++)
					{
						Entity vehicle4 = val2[l].m_Vehicle;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(0, vehicle4, temp);
					}
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Temp>(0, val, temp);
				}
			}
			if (layoutBuffer.IsCreated)
			{
				layoutBuffer.Clear();
			}
		}
	}

	[BurstCompile]
	private struct SpawnPostVansJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> m_PrefabPostFacilityData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public PostVanSelectData m_PostVanSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<PostFacilityData>(m_Entity, out PostFacilityData data, ref m_PrefabRefData, ref m_PrefabPostFacilityData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null && UpgradeUtils.TryGetCombinedComponent<PostFacilityData>(m_Temp.m_Original, out PostFacilityData data2, ref m_PrefabRefData, ref m_PrefabPostFacilityData, ref m_InstalledUpgrades))
			{
				data.m_PostVanCapacity -= data2.m_PostVanCapacity;
			}
			for (int i = 0; i < data.m_PostVanCapacity; i++)
			{
				CreateVehicle(ref random, RoadTypes.Car);
			}
		}

		private void CreateVehicle(ref Random random, RoadTypes roadType)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			int2 mailCapacity = default(int2);
			((int2)(ref mailCapacity))._002Ector(1, int.MaxValue);
			Entity val = m_PostVanSelectData.SelectVehicle(ref random, ref mailCapacity);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_PostVanSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PostVan>(val2, new PostVan(PostVanFlags.Disabled, 0, 0));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	[BurstCompile]
	private struct SpawnMaintenanceVehiclesJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> m_PrefabMaintenanceDepotData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public MaintenanceVehicleSelectData m_MaintenanceVehicleSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<MaintenanceDepotData>(m_Entity, out MaintenanceDepotData data, ref m_PrefabRefData, ref m_PrefabMaintenanceDepotData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null && UpgradeUtils.TryGetCombinedComponent<MaintenanceDepotData>(m_Temp.m_Original, out MaintenanceDepotData data2, ref m_PrefabRefData, ref m_PrefabMaintenanceDepotData, ref m_InstalledUpgrades))
			{
				data.m_VehicleCapacity -= data2.m_VehicleCapacity;
			}
			for (int i = 0; i < data.m_VehicleCapacity; i++)
			{
				CreateVehicle(ref random, data, RoadTypes.Car);
			}
		}

		private void CreateVehicle(ref Random random, MaintenanceDepotData maintenanceDepotData, RoadTypes roadType)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_MaintenanceVehicleSelectData.SelectVehicle(ref random, MaintenanceType.None, maintenanceDepotData.m_MaintenanceType, GetMaxVehicleSize(m_Locations, roadType));
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_MaintenanceVehicleSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, MaintenanceType.None, maintenanceDepotData.m_MaintenanceType, float4.op_Implicit(float.MaxValue), parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<MaintenanceVehicle>(val2, new MaintenanceVehicle(MaintenanceVehicleFlags.Disabled, 0, maintenanceDepotData.m_VehicleEfficiency));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	[BurstCompile]
	private struct SpawnGarbageTrucksJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> m_PseudoRandomSeedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> m_PrefabGarbageFacilityData;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public Temp m_Temp;

		[ReadOnly]
		public bool m_IsTemp;

		[ReadOnly]
		public GarbageTruckSelectData m_GarbageTruckSelectData;

		public NativeList<ParkingLocation> m_Locations;

		[NativeDisableContainerSafetyRestriction]
		public NativeParallelMultiHashMap<Entity, DeletedVehicleData> m_DeletedVehicleMap;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_PseudoRandomSeedData[m_Entity].GetRandom(PseudoRandomSeed.kParkedCars);
			UpgradeUtils.TryGetCombinedComponent<GarbageFacilityData>(m_Entity, out GarbageFacilityData data, ref m_PrefabRefData, ref m_PrefabGarbageFacilityData, ref m_InstalledUpgrades);
			if (m_Temp.m_Original != Entity.Null && UpgradeUtils.TryGetCombinedComponent<GarbageFacilityData>(m_Temp.m_Original, out GarbageFacilityData data2, ref m_PrefabRefData, ref m_PrefabGarbageFacilityData, ref m_InstalledUpgrades))
			{
				data.m_VehicleCapacity -= data2.m_VehicleCapacity;
			}
			for (int i = 0; i < data.m_VehicleCapacity; i++)
			{
				CreateVehicle(ref random, data, RoadTypes.Car);
			}
		}

		private void CreateVehicle(ref Random random, GarbageFacilityData garbageFacilityData, RoadTypes roadType)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			int2 garbageCapacity = default(int2);
			((int2)(ref garbageCapacity))._002Ector(1, int.MaxValue);
			Entity val = m_GarbageTruckSelectData.SelectVehicle(ref random, ref garbageCapacity);
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			if (!m_PrefabObjectGeometryData.TryGetComponent(val, ref objectGeometryData) || !SelectParkingSpace(ref random, m_Locations, objectGeometryData, roadType, TrackTypes.None, out var transform, out var lane, out var curvePosition))
			{
				return;
			}
			Entity val2 = FindDeletedVehicle(val, Entity.Null, transform, m_DeletedVehicleMap);
			if (val2 != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Deleted>(val2);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Transform>(val2, transform);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2);
			}
			else
			{
				val2 = m_GarbageTruckSelectData.CreateVehicle(m_CommandBuffer, ref random, transform, m_Entity, val, ref garbageCapacity, parked: true);
				if (m_IsTemp)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Animation>(val2, default(Animation));
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(val2, default(InterpolatedTransform));
				}
			}
			GarbageTruckFlags garbageTruckFlags = GarbageTruckFlags.Disabled;
			if (garbageFacilityData.m_IndustrialWasteOnly)
			{
				garbageTruckFlags |= GarbageTruckFlags.IndustrialWasteOnly;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<GarbageTruck>(val2, new GarbageTruck(garbageTruckFlags, 0));
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<ParkedCar>(val2, new ParkedCar(lane, curvePosition));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Owner>(val2, new Owner(m_Entity));
			if (m_IsTemp)
			{
				Temp temp = new Temp
				{
					m_Flags = (m_Temp.m_Flags & (TempFlags.Create | TempFlags.Delete | TempFlags.Select | TempFlags.Modify | TempFlags.Hidden | TempFlags.Duplicate))
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Temp>(val2, temp);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SpawnLocationElement> __Game_Buildings_SpawnLocationElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Controller> __Game_Vehicles_Controller_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PseudoRandomSeed> __Game_Common_PseudoRandomSeed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Helicopter> __Game_Vehicles_Helicopter_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Train> __Game_Vehicles_Train_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> __Game_Prefabs_MovingObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainData> __Game_Prefabs_TrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainObjectData> __Game_Prefabs_TrainObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PoliceStationData> __Game_Prefabs_PoliceStationData_RO_ComponentLookup;

		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RW_BufferLookup;

		[ReadOnly]
		public ComponentLookup<FireStationData> __Game_Prefabs_FireStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HospitalData> __Game_Prefabs_HospitalData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<DeathcareFacilityData> __Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> __Game_Prefabs_TransportDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrisonData> __Game_Prefabs_PrisonData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EmergencyShelterData> __Game_Prefabs_EmergencyShelterData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PostFacilityData> __Game_Prefabs_PostFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> __Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarbageFacilityData> __Game_Prefabs_GarbageFacilityData_RO_ComponentLookup;

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
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Buildings_SpawnLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SpawnLocationElement>(true);
			__Game_Net_LaneOverlap_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneOverlap>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Vehicles_Controller_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Controller>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Common_PseudoRandomSeed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PseudoRandomSeed>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Helicopter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Helicopter>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Vehicles_Train_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Train>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Prefabs_MovingObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_TrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainData>(true);
			__Game_Prefabs_TrainObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainObjectData>(true);
			__Game_Vehicles_OwnedVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Prefabs_PoliceStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PoliceStationData>(true);
			__Game_Buildings_InstalledUpgrade_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(false);
			__Game_Prefabs_FireStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FireStationData>(true);
			__Game_Prefabs_HospitalData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HospitalData>(true);
			__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<DeathcareFacilityData>(true);
			__Game_Prefabs_TransportDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportDepotData>(true);
			__Game_Prefabs_PrisonData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrisonData>(true);
			__Game_Prefabs_EmergencyShelterData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EmergencyShelterData>(true);
			__Game_Prefabs_PostFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PostFacilityData>(true);
			__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MaintenanceDepotData>(true);
			__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarbageFacilityData>(true);
		}
	}

	private ModificationBarrier4B m_ModificationBarrier;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_DeletedVehicleQuery;

	private EntityQuery m_PoliceCarQuery;

	private EntityQuery m_FireEngineQuery;

	private EntityQuery m_HealthcareVehicleQuery;

	private EntityQuery m_TransportVehicleQuery;

	private EntityQuery m_PostVanQuery;

	private EntityQuery m_MaintenanceVehicleQuery;

	private EntityQuery m_GarbageTruckQuery;

	private PoliceCarSelectData m_PoliceCarSelectData;

	private FireEngineSelectData m_FireEngineSelectData;

	private HealthcareVehicleSelectData m_HealthcareVehicleSelectData;

	private TransportVehicleSelectData m_TransportVehicleSelectData;

	private PostVanSelectData m_PostVanSelectData;

	private MaintenanceVehicleSelectData m_MaintenanceVehicleSelectData;

	private GarbageTruckSelectData m_GarbageTruckSelectData;

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
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4B>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<OwnedVehicle>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Temp>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Applied>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>()
		};
		array[0] = val;
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_DeletedVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_PoliceCarQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PoliceCarSelectData.GetEntityQueryDesc() });
		m_FireEngineQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { FireEngineSelectData.GetEntityQueryDesc() });
		m_HealthcareVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { HealthcareVehicleSelectData.GetEntityQueryDesc() });
		m_TransportVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { TransportVehicleSelectData.GetEntityQueryDesc() });
		m_PostVanQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { PostVanSelectData.GetEntityQueryDesc() });
		m_MaintenanceVehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { MaintenanceVehicleSelectData.GetEntityQueryDesc() });
		m_GarbageTruckQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)new EntityQueryDesc[1] { GarbageTruckSelectData.GetEntityQueryDesc() });
		m_PoliceCarSelectData = new PoliceCarSelectData((SystemBase)(object)this);
		m_FireEngineSelectData = new FireEngineSelectData((SystemBase)(object)this);
		m_HealthcareVehicleSelectData = new HealthcareVehicleSelectData((SystemBase)(object)this);
		m_TransportVehicleSelectData = new TransportVehicleSelectData((SystemBase)(object)this);
		m_PostVanSelectData = new PostVanSelectData((SystemBase)(object)this);
		m_MaintenanceVehicleSelectData = new MaintenanceVehicleSelectData((SystemBase)(object)this);
		m_GarbageTruckSelectData = new GarbageTruckSelectData((SystemBase)(object)this);
		((ComponentSystemBase)this).RequireForUpdate(m_BuildingQuery);
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		((ComponentSystemBase)this).Enabled = mode.IsGame();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_BuildingQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Temp> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).CompleteDependencyBeforeRO<PrefabRef>();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).CompleteDependencyBeforeRO<Temp>();
		try
		{
			NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap = default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>);
			JobHandle deletedVehiclesDeps = default(JobHandle);
			JobHandle dependency = ((SystemBase)this).Dependency;
			JobHandle val2 = default(JobHandle);
			Temp temp = default(Temp);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val3 = val[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<Temp>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val4 = nativeArray[j];
					bool flag = CollectionUtils.TryGet<Temp>(nativeArray2, j, ref temp);
					if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Select | TempFlags.Duplicate)) != 0)
					{
						continue;
					}
					NativeList<ParkingLocation> parkingLocations = default(NativeList<ParkingLocation>);
					JobHandle parkingLocationDeps = default(JobHandle);
					if (flag && temp.m_Original != Entity.Null)
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						DuplicateVehicles(val4, temp, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PoliceStation>(val4))
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						if (flag)
						{
							CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						}
						SpawnPoliceCars(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.FireStation>(val4))
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						if (flag)
						{
							CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						}
						SpawnFireEngines(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Hospital>(val4))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.DeathcareFacility>(val4))
						{
							goto IL_0207;
						}
					}
					FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
					if (flag)
					{
						CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
					}
					SpawnHealthcareVehicles(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					goto IL_0207;
					IL_026f:
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.PostFacility>(val4))
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						if (flag)
						{
							CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						}
						SpawnPostVans(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.MaintenanceDepot>(val4))
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						if (flag)
						{
							CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						}
						SpawnMaintenanceVehicles(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.GarbageFacility>(val4))
					{
						FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
						if (flag)
						{
							CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
						}
						SpawnGarbageTrucks(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					}
					if (parkingLocations.IsCreated)
					{
						parkingLocations.Dispose(parkingLocationDeps);
						val2 = JobHandle.CombineDependencies(val2, parkingLocationDeps);
					}
					continue;
					IL_0207:
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.TransportDepot>(val4))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Prison>(val4))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.EmergencyShelter>(val4))
							{
								goto IL_026f;
							}
						}
					}
					FindParkingLocations(val4, ref parkingLocations, dependency, ref parkingLocationDeps);
					if (flag)
					{
						CollectDeletedVehicles(ref deletedVehicleMap, dependency, ref deletedVehiclesDeps);
					}
					SpawnTransportVehicles(val4, temp, flag, parkingLocations, deletedVehicleMap, ref parkingLocationDeps, ref deletedVehiclesDeps);
					goto IL_026f;
				}
			}
			if (deletedVehicleMap.IsCreated)
			{
				deletedVehicleMap.Dispose(deletedVehiclesDeps);
			}
			((SystemBase)this).Dependency = val2;
		}
		finally
		{
			val.Dispose();
		}
	}

	private void FindParkingLocations(Entity entity, ref NativeList<ParkingLocation> parkingLocations, JobHandle inputDeps, ref JobHandle parkingLocationDeps)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if (!parkingLocations.IsCreated)
		{
			parkingLocations = new NativeList<ParkingLocation>(100, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val = IJobExtensions.Schedule<FindParkingLocationsJob>(new FindParkingLocationsJob
			{
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocationElements = InternalCompilerInterface.GetBufferLookup<SpawnLocationElement>(ref __TypeHandle.__Game_Buildings_SpawnLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneOverlaps = InternalCompilerInterface.GetBufferLookup<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Entity = entity,
				m_Locations = parkingLocations
			}, inputDeps);
			parkingLocationDeps = val;
		}
	}

	private void CollectDeletedVehicles(ref NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, JobHandle inputDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (!deletedVehicleMap.IsCreated)
		{
			deletedVehicleMap = new NativeParallelMultiHashMap<Entity, DeletedVehicleData>(0, AllocatorHandle.op_Implicit((Allocator)3));
			JobHandle val = default(JobHandle);
			CollectDeletedVehiclesJob collectDeletedVehiclesJob = new CollectDeletedVehiclesJob
			{
				m_Chunks = ((EntityQuery)(ref m_DeletedVehicleQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerType = InternalCompilerInterface.GetComponentTypeHandle<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedVehicleMap = deletedVehicleMap
			};
			JobHandle val2 = IJobExtensions.Schedule<CollectDeletedVehiclesJob>(collectDeletedVehiclesJob, JobHandle.CombineDependencies(inputDeps, val));
			collectDeletedVehiclesJob.m_Chunks.Dispose(val2);
			deletedVehiclesDeps = val2;
		}
	}

	private void DuplicateVehicles(Entity entity, Temp temp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<DuplicateVehiclesJob>(new DuplicateVehiclesJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HelicopterData = InternalCompilerInterface.GetComponentLookup<Helicopter>(ref __TypeHandle.__Game_Vehicles_Helicopter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainData = InternalCompilerInterface.GetComponentLookup<Train>(ref __TypeHandle.__Game_Vehicles_Train_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMovingObjectData = InternalCompilerInterface.GetComponentLookup<MovingObjectData>(ref __TypeHandle.__Game_Prefabs_MovingObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainData = InternalCompilerInterface.GetComponentLookup<TrainData>(ref __TypeHandle.__Game_Prefabs_TrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrainObjectData = InternalCompilerInterface.GetComponentLookup<TrainObjectData>(ref __TypeHandle.__Game_Prefabs_TrainObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = deletedVehicleMap,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps));
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		deletedVehiclesDeps = val;
	}

	private void SpawnPoliceCars(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		m_PoliceCarSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_PoliceCarQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnPoliceCarsJob>(new SpawnPoliceCarsJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPoliceStationData = InternalCompilerInterface.GetComponentLookup<PoliceStationData>(ref __TypeHandle.__Game_Prefabs_PoliceStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_PoliceCarSelectData = m_PoliceCarSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_PoliceCarSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private void SpawnFireEngines(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		m_FireEngineSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_FireEngineQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnFireEnginesJob>(new SpawnFireEnginesJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabFireStationData = InternalCompilerInterface.GetComponentLookup<FireStationData>(ref __TypeHandle.__Game_Prefabs_FireStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_FireEngineSelectData = m_FireEngineSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_FireEngineSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private void SpawnHealthcareVehicles(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		m_HealthcareVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_HealthcareVehicleQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnHealthcareVehiclesJob>(new SpawnHealthcareVehiclesJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabHospitalData = InternalCompilerInterface.GetComponentLookup<HospitalData>(ref __TypeHandle.__Game_Prefabs_HospitalData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabDeathcareFacilityData = InternalCompilerInterface.GetComponentLookup<DeathcareFacilityData>(ref __TypeHandle.__Game_Prefabs_DeathcareFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_HealthcareVehicleSelectData = m_HealthcareVehicleSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_HealthcareVehicleSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private void SpawnTransportVehicles(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		m_TransportVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_TransportVehicleQuery, (Allocator)3, out var jobHandle);
		SpawnTransportVehiclesJob spawnTransportVehiclesJob = new SpawnTransportVehiclesJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTransportDepotData = InternalCompilerInterface.GetComponentLookup<TransportDepotData>(ref __TypeHandle.__Game_Prefabs_TransportDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPrisonData = InternalCompilerInterface.GetComponentLookup<PrisonData>(ref __TypeHandle.__Game_Prefabs_PrisonData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEmergencyShelterData = InternalCompilerInterface.GetComponentLookup<EmergencyShelterData>(ref __TypeHandle.__Game_Prefabs_EmergencyShelterData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_TransportVehicleSelectData = m_TransportVehicleSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>))
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		spawnTransportVehiclesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = IJobExtensions.Schedule<SpawnTransportVehiclesJob>(spawnTransportVehiclesJob, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_TransportVehicleSelectData.PostUpdate(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		parkingLocationDeps = val2;
		if (isTemp)
		{
			deletedVehiclesDeps = val2;
		}
	}

	private void SpawnPostVans(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		m_PostVanSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_PostVanQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnPostVansJob>(new SpawnPostVansJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPostFacilityData = InternalCompilerInterface.GetComponentLookup<PostFacilityData>(ref __TypeHandle.__Game_Prefabs_PostFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_PostVanSelectData = m_PostVanSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_PostVanSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private void SpawnMaintenanceVehicles(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		m_MaintenanceVehicleSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_MaintenanceVehicleQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnMaintenanceVehiclesJob>(new SpawnMaintenanceVehiclesJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabMaintenanceDepotData = InternalCompilerInterface.GetComponentLookup<MaintenanceDepotData>(ref __TypeHandle.__Game_Prefabs_MaintenanceDepotData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_MaintenanceVehicleSelectData = m_MaintenanceVehicleSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_MaintenanceVehicleSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private void SpawnGarbageTrucks(Entity entity, Temp temp, bool isTemp, NativeList<ParkingLocation> parkingLocations, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedVehicleMap, ref JobHandle parkingLocationDeps, ref JobHandle deletedVehiclesDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		m_GarbageTruckSelectData.PreUpdate((SystemBase)(object)this, m_CityConfigurationSystem, m_GarbageTruckQuery, (Allocator)3, out var jobHandle);
		JobHandle val = IJobExtensions.Schedule<SpawnGarbageTrucksJob>(new SpawnGarbageTrucksJob
		{
			m_PseudoRandomSeedData = InternalCompilerInterface.GetComponentLookup<PseudoRandomSeed>(ref __TypeHandle.__Game_Common_PseudoRandomSeed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGarbageFacilityData = InternalCompilerInterface.GetComponentLookup<GarbageFacilityData>(ref __TypeHandle.__Game_Prefabs_GarbageFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Entity = entity,
			m_Temp = temp,
			m_IsTemp = isTemp,
			m_GarbageTruckSelectData = m_GarbageTruckSelectData,
			m_Locations = parkingLocations,
			m_DeletedVehicleMap = (isTemp ? deletedVehicleMap : default(NativeParallelMultiHashMap<Entity, DeletedVehicleData>)),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, isTemp ? JobHandle.CombineDependencies(parkingLocationDeps, deletedVehiclesDeps, jobHandle) : JobHandle.CombineDependencies(parkingLocationDeps, jobHandle));
		m_GarbageTruckSelectData.PostUpdate(val);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val);
		parkingLocationDeps = val;
		if (isTemp)
		{
			deletedVehiclesDeps = val;
		}
	}

	private static Entity GetSecondaryPrefab(Entity primaryPrefab, DynamicBuffer<LayoutElement> layoutElements, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<PrefabData> prefabDatas, out bool validLayout)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		validLayout = true;
		PrefabRef prefabRef = default(PrefabRef);
		for (int i = 0; i < layoutElements.Length; i++)
		{
			Entity vehicle = layoutElements[i].m_Vehicle;
			if (prefabRefs.TryGetComponent(vehicle, ref prefabRef) && prefabRef.m_Prefab != primaryPrefab)
			{
				if (val == Entity.Null)
				{
					val = prefabRef.m_Prefab;
				}
				validLayout &= EntitiesExtensions.HasEnabledComponent<PrefabData>(prefabDatas, prefabRef.m_Prefab);
			}
		}
		return val;
	}

	private static float4 GetMaxVehicleSize(NativeList<ParkingLocation> locations, RoadTypes roadType)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float4 val = float4.op_Implicit(0f);
		for (int i = 0; i < locations.Length; i++)
		{
			ParkingLocation parkingLocation = locations[i];
			val = math.select(val, ((float2)(ref parkingLocation.m_MaxSize)).xyxy, (((float2)(ref parkingLocation.m_MaxSize)).xxyy > ((float4)(ref val)).xxww) & ((parkingLocation.m_ParkingLaneData.m_RoadTypes & roadType) != 0));
		}
		return val;
	}

	private static bool SelectParkingSpace(ref Random random, NativeList<ParkingLocation> locations, ObjectGeometryData objectGeometryData, RoadTypes roadType, TrackTypes trackType, out Transform transform, out Entity lane, out float curvePosition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		float offset;
		float2 parkingSize = VehicleUtils.GetParkingSize(objectGeometryData, out offset);
		int num = 0;
		int num2 = -1;
		for (int i = 0; i < locations.Length; i++)
		{
			ParkingLocation parkingLocation = locations[i];
			if (!math.any(parkingSize > parkingLocation.m_MaxSize) && ((parkingLocation.m_ParkingLaneData.m_RoadTypes & roadType) != RoadTypes.None || (parkingLocation.m_TrackTypes & trackType) != TrackTypes.None))
			{
				int num3 = 100;
				num += num3;
				if (((Random)(ref random)).NextInt(num) < num3)
				{
					num2 = i;
				}
			}
		}
		if (num2 != -1)
		{
			ParkingLocation parkingLocation2 = locations[num2];
			lane = parkingLocation2.m_Lane;
			curvePosition = parkingLocation2.m_CurvePos;
			if (parkingLocation2.m_SpawnLocationType == SpawnLocationType.ParkingLane)
			{
				if (parkingLocation2.m_ParkingLaneData.m_SlotAngle <= 0.25f)
				{
					if (offset > 0f)
					{
						Bounds1 val = default(Bounds1);
						((Bounds1)(ref val))._002Ector(curvePosition, 1f);
						MathUtils.ClampLength(parkingLocation2.m_Curve.m_Bezier, ref val, offset);
						curvePosition = val.max;
					}
					else if (offset < 0f)
					{
						Bounds1 val2 = default(Bounds1);
						((Bounds1)(ref val2))._002Ector(0f, curvePosition);
						MathUtils.ClampLengthInverse(parkingLocation2.m_Curve.m_Bezier, ref val2, 0f - offset);
						curvePosition = val2.min;
					}
				}
				transform = VehicleUtils.CalculateParkingSpaceTarget(parkingLocation2.m_ParkingLane, parkingLocation2.m_ParkingLaneData, objectGeometryData, parkingLocation2.m_Curve, parkingLocation2.m_OwnerTransform, curvePosition);
			}
			else
			{
				transform = parkingLocation2.m_OwnerTransform;
			}
			locations.RemoveAtSwapBack(num2);
			return true;
		}
		transform = default(Transform);
		lane = Entity.Null;
		curvePosition = 0f;
		return false;
	}

	private static Entity FindDeletedVehicle(Entity primaryPrefab, Entity secondaryPrefab, Transform transform, NativeParallelMultiHashMap<Entity, DeletedVehicleData> deletedMap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		DeletedVehicleData deletedVehicleData = default(DeletedVehicleData);
		NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
		if (deletedMap.IsCreated && deletedMap.TryGetFirstValue(primaryPrefab, ref deletedVehicleData, ref val2))
		{
			float num = float.MaxValue;
			NativeParallelMultiHashMapIterator<Entity> val3 = default(NativeParallelMultiHashMapIterator<Entity>);
			do
			{
				if (!(deletedVehicleData.m_SecondaryPrefab != secondaryPrefab))
				{
					float num2 = math.distance(deletedVehicleData.m_Transform.m_Position, transform.m_Position);
					if (num2 < num)
					{
						val = deletedVehicleData.m_Entity;
						num = num2;
						val3 = val2;
					}
				}
			}
			while (deletedMap.TryGetNextValue(ref deletedVehicleData, ref val2));
			if (val != Entity.Null)
			{
				deletedMap.Remove(val3);
			}
		}
		return val;
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
	public ParkedVehiclesSystem()
	{
	}
}
