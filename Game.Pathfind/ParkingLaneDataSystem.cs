using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class ParkingLaneDataSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneDataJob : IJobChunk
	{
		private struct CountVehiclesIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_Lane;

			public Bounds3 m_Bounds;

			public int m_Result;

			public ComponentLookup<ParkedCar> m_ParkedCarData;

			public ComponentLookup<Controller> m_ControllerData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				Controller controller = default(Controller);
				ParkedCar parkedCar = default(ParkedCar);
				if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && (!m_ControllerData.TryGetComponent(entity, ref controller) || !(controller.m_Controller != entity)) && m_ParkedCarData.TryGetComponent(entity, ref parkedCar) && parkedCar.m_Lane == m_Lane)
				{
					m_Result++;
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> m_LaneOverlapType;

		public ComponentTypeHandle<Game.Net.ParkingLane> m_ParkingLaneType;

		public ComponentTypeHandle<Game.Net.ConnectionLane> m_ConnectionLaneType;

		public ComponentTypeHandle<GarageLane> m_GarageLaneType;

		public ComponentTypeHandle<Game.Objects.SpawnLocation> m_SpawnLocationType;

		public BufferTypeHandle<LaneObject> m_LaneObjectType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ParkingFacility> m_ParkingFacilityData;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Unspawned> m_UnspawnedData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingFacilityData> m_PrefabParkingFacilityData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_PrefabBuildingPropertyData;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> m_PrefabWorkplaceData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> m_PrefabSpawnLocationData;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public BufferLookup<BuildingModifier> m_BuildingModifiers;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<ActivityLocationElement> m_ActivityLocations;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_MovingObjectSearchTree;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Game.Net.ParkingLane> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.ParkingLane>(ref m_ParkingLaneType);
			if (nativeArray.Length != 0)
			{
				NativeArray<Curve> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Lane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_LaneType);
				NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<LaneObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneObject>(ref m_LaneObjectType);
				BufferAccessor<LaneOverlap> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneOverlap>(ref m_LaneOverlapType);
				ushort taxiFee = 0;
				if (m_City != Entity.Null && CityUtils.CheckOption(m_CityData[m_City], CityOption.PaidTaxiStart))
				{
					float value = 0f;
					CityUtils.ApplyModifier(ref value, m_CityModifiers[m_City], CityModifierType.TaxiStartingFee);
					taxiFee = (ushort)math.clamp(Mathf.RoundToInt(value), 0, 65535);
				}
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Curve curve = nativeArray2[i];
					Owner owner = nativeArray3[i];
					Lane laneData = nativeArray4[i];
					Game.Net.ParkingLane parkingLane = nativeArray[i];
					PrefabRef prefabRef = nativeArray5[i];
					DynamicBuffer<LaneObject> laneObjects = bufferAccessor[i];
					DynamicBuffer<LaneOverlap> laneOverlaps = bufferAccessor2[i];
					ParkingLaneData parkingLaneData = m_ParkingLaneData[prefabRef.m_Prefab];
					Bounds1 blockedRange = GetBlockedRange(owner, laneData);
					parkingLane.m_Flags &= ~(ParkingLaneFlags.ParkingDisabled | ParkingLaneFlags.AllowEnter | ParkingLaneFlags.AllowExit);
					NativeSortExtension.Sort<LaneObject>(laneObjects.AsNativeArray());
					parkingLane.m_FreeSpace = CalculateFreeSpace(curve, parkingLane, parkingLaneData, laneObjects, laneOverlaps, blockedRange);
					GetParkingStats(owner, parkingLane, out parkingLane.m_AccessRestriction, out var _, out parkingLane.m_ParkingFee, out parkingLane.m_ComfortFactor, out var disabled, out var allowEnter, out var allowExit);
					parkingLane.m_TaxiFee = taxiFee;
					if (disabled)
					{
						parkingLane.m_Flags |= ParkingLaneFlags.ParkingDisabled;
					}
					if (allowEnter)
					{
						parkingLane.m_Flags |= ParkingLaneFlags.AllowEnter;
					}
					if (allowExit)
					{
						parkingLane.m_Flags |= ParkingLaneFlags.AllowExit;
					}
					nativeArray[i] = parkingLane;
				}
				return;
			}
			NativeArray<GarageLane> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<GarageLane>(ref m_GarageLaneType);
			if (nativeArray6.Length != 0)
			{
				NativeArray<Entity> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				NativeArray<Curve> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<Owner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Game.Net.ConnectionLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.ConnectionLane>(ref m_ConnectionLaneType);
				for (int j = 0; j < nativeArray6.Length; j++)
				{
					Entity entity = nativeArray7[j];
					Curve curve2 = nativeArray8[j];
					Owner owner2 = nativeArray9[j];
					GarageLane garageLane = nativeArray6[j];
					Game.Net.ConnectionLane connectionLane = nativeArray10[j];
					connectionLane.m_Flags &= ~(ConnectionLaneFlags.Disabled | ConnectionLaneFlags.AllowEnter | ConnectionLaneFlags.AllowExit);
					GetParkingStats(owner2, default(Game.Net.ParkingLane), out connectionLane.m_AccessRestriction, out garageLane.m_VehicleCapacity, out garageLane.m_ParkingFee, out garageLane.m_ComfortFactor, out var disabled2, out var allowEnter2, out var allowExit2);
					garageLane.m_VehicleCount = CountVehicles(entity, owner2, curve2, connectionLane);
					if (disabled2)
					{
						connectionLane.m_Flags |= ConnectionLaneFlags.Disabled;
					}
					if (allowEnter2)
					{
						connectionLane.m_Flags |= ConnectionLaneFlags.AllowEnter;
					}
					if (allowExit2)
					{
						connectionLane.m_Flags |= ConnectionLaneFlags.AllowExit;
					}
					nativeArray6[j] = garageLane;
					nativeArray10[j] = connectionLane;
				}
				return;
			}
			NativeArray<Game.Objects.SpawnLocation> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Objects.SpawnLocation>(ref m_SpawnLocationType);
			if (nativeArray11.Length == 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Transform> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<PrefabRef> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			SpawnLocationData spawnLocationData = default(SpawnLocationData);
			for (int k = 0; k < nativeArray11.Length; k++)
			{
				PrefabRef prefabRef2 = nativeArray14[k];
				if (m_PrefabSpawnLocationData.TryGetComponent(prefabRef2.m_Prefab, ref spawnLocationData) && (((spawnLocationData.m_RoadTypes & RoadTypes.Helicopter) != RoadTypes.None && spawnLocationData.m_ConnectionType == RouteConnectionType.Air) || spawnLocationData.m_ConnectionType == RouteConnectionType.Track))
				{
					Entity entity2 = nativeArray12[k];
					Game.Objects.SpawnLocation spawnLocation = nativeArray11[k];
					Transform transform = nativeArray13[k];
					if (CountVehicles(entity2, transform, spawnLocation, spawnLocationData) != 0)
					{
						spawnLocation.m_Flags |= SpawnLocationFlags.ParkedVehicle;
					}
					else
					{
						spawnLocation.m_Flags &= ~SpawnLocationFlags.ParkedVehicle;
					}
					nativeArray11[k] = spawnLocation;
				}
			}
		}

		private Bounds1 GetBlockedRange(Owner owner, Lane laneData)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(2f, -1f);
			if (m_SubLanes.HasBuffer(owner.m_Owner))
			{
				DynamicBuffer<Game.Net.SubLane> val2 = m_SubLanes[owner.m_Owner];
				for (int i = 0; i < val2.Length; i++)
				{
					Entity subLane = val2[i].m_SubLane;
					Lane lane = m_LaneData[subLane];
					if (laneData.m_StartNode.EqualsIgnoreCurvePos(lane.m_MiddleNode) && m_CarLaneData.HasComponent(subLane))
					{
						Game.Net.CarLane carLane = m_CarLaneData[subLane];
						if (carLane.m_BlockageEnd >= carLane.m_BlockageStart)
						{
							Bounds1 blockageBounds = carLane.blockageBounds;
							blockageBounds.min = math.select(blockageBounds.min - 0.01f, 0f, blockageBounds.min <= 0.51f);
							blockageBounds.max = math.select(blockageBounds.max + 0.01f, 1f, blockageBounds.max >= 0.49f);
							val |= blockageBounds;
						}
					}
				}
			}
			return val;
		}

		private void GetParkingStats(Owner owner, Game.Net.ParkingLane parkingLane, out Entity restriction, out ushort garageCapacity, out ushort fee, out ushort comfort, out bool disabled, out bool allowEnter, out bool allowExit)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			restriction = Entity.Null;
			garageCapacity = 0;
			fee = 0;
			comfort = 0;
			disabled = false;
			allowEnter = false;
			allowExit = false;
			Owner owner2 = owner;
			Owner owner3 = default(Owner);
			while (m_OwnerData.TryGetComponent(owner2.m_Owner, ref owner3))
			{
				owner2 = owner3;
			}
			if (m_BuildingData.HasComponent(owner2.m_Owner))
			{
				ParkingFacilityData parkingFacilityData = default(ParkingFacilityData);
				bool flag = false;
				PrefabRef prefabRef = m_PrefabRefData[owner2.m_Owner];
				if (m_PrefabParkingFacilityData.HasComponent(prefabRef.m_Prefab))
				{
					parkingFacilityData = m_PrefabParkingFacilityData[prefabRef.m_Prefab];
					flag = true;
				}
				DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
				if (m_InstalledUpgrades.TryGetBuffer(owner2.m_Owner, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						InstalledUpgrade installedUpgrade = val[i];
						if (!BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive))
						{
							PrefabRef prefabRef2 = m_PrefabRefData[installedUpgrade.m_Upgrade];
							if (m_PrefabParkingFacilityData.HasComponent(prefabRef2.m_Prefab))
							{
								parkingFacilityData.Combine(m_PrefabParkingFacilityData[prefabRef2.m_Prefab]);
								flag = true;
							}
						}
					}
				}
				Entity val2 = owner2.m_Owner;
				Attachment attachment = default(Attachment);
				if (m_AttachmentData.TryGetComponent(val2, ref attachment) && attachment.m_Attached != Entity.Null)
				{
					val2 = attachment.m_Attached;
				}
				PrefabRef prefabRef3 = default(PrefabRef);
				BuildingData buildingData = default(BuildingData);
				if (m_PrefabRefData.TryGetComponent(val2, ref prefabRef3) && m_PrefabBuildingData.TryGetComponent(prefabRef3.m_Prefab, ref buildingData))
				{
					if (m_RoadData.HasComponent(owner.m_Owner))
					{
						buildingData.m_Flags &= ~(Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar);
					}
					restriction = val2;
					allowEnter = (buildingData.m_Flags & (Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar)) == 0;
					allowExit = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RestrictedParking) == 0;
				}
				PrefabRef prefabRef4 = default(PrefabRef);
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef4) && m_PrefabGeometryData.TryGetComponent(prefabRef4.m_Prefab, ref netGeometryData) && (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) != 0)
				{
					restriction = Entity.Null;
					allowEnter = false;
					allowExit = false;
				}
				if (!flag)
				{
					parkingFacilityData.m_GarageMarkerCapacity = 2;
					parkingFacilityData.m_ComfortFactor = 0.8f;
					BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
					WorkplaceData workplaceData = default(WorkplaceData);
					if (m_PrefabBuildingPropertyData.TryGetComponent(prefabRef.m_Prefab, ref buildingPropertyData))
					{
						parkingFacilityData.m_GarageMarkerCapacity = math.max(1, Mathf.RoundToInt(buildingPropertyData.m_SpaceMultiplier));
					}
					else if (m_PrefabWorkplaceData.TryGetComponent(prefabRef.m_Prefab, ref workplaceData))
					{
						parkingFacilityData.m_GarageMarkerCapacity = math.max(2, workplaceData.m_MaxWorkers / 20);
					}
				}
				Game.Buildings.ParkingFacility parkingFacility = default(Game.Buildings.ParkingFacility);
				if (m_ParkingFacilityData.TryGetComponent(owner2.m_Owner, ref parkingFacility))
				{
					disabled = (parkingFacility.m_Flags & ParkingFacilityFlags.ParkingSpacesActive) == 0 && (parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) == 0;
					parkingFacilityData.m_ComfortFactor = parkingFacility.m_ComfortFactor;
				}
				garageCapacity = (ushort)math.clamp(parkingFacilityData.m_GarageMarkerCapacity, 0, 65535);
				comfort = (ushort)math.clamp(Mathf.RoundToInt(parkingFacilityData.m_ComfortFactor * 65535f), 0, 65535);
				fee = GetBuildingParkingFee(owner2.m_Owner);
			}
			else if (m_BorderDistrictData.HasComponent(owner.m_Owner))
			{
				BorderDistrict borderDistrict = m_BorderDistrictData[owner.m_Owner];
				if ((parkingLane.m_Flags & ParkingLaneFlags.RightSide) != 0)
				{
					fee = GetDistrictParkingFee(borderDistrict.m_Right);
				}
				else
				{
					fee = GetDistrictParkingFee(borderDistrict.m_Left);
				}
			}
		}

		private ushort GetDistrictParkingFee(Entity district)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (m_DistrictData.HasComponent(district) && AreaUtils.CheckOption(m_DistrictData[district], DistrictOption.PaidParking))
			{
				float value = 0f;
				DynamicBuffer<DistrictModifier> modifiers = m_DistrictModifiers[district];
				AreaUtils.ApplyModifier(ref value, modifiers, DistrictModifierType.ParkingFee);
				return (ushort)math.clamp(Mathf.RoundToInt(value), 0, 65535);
			}
			return 0;
		}

		private ushort GetBuildingParkingFee(Entity building)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (m_BuildingData.HasComponent(building) && BuildingUtils.CheckOption(m_BuildingData[building], BuildingOption.PaidParking))
			{
				float value = 0f;
				DynamicBuffer<BuildingModifier> modifiers = m_BuildingModifiers[building];
				BuildingUtils.ApplyModifier(ref value, modifiers, BuildingModifierType.ParkingFee);
				return (ushort)math.clamp(Mathf.RoundToInt(value), 0, 65535);
			}
			return 0;
		}

		private ushort CountVehicles(Entity entity, Owner owner, Curve curve, Game.Net.ConnectionLane connectionLane)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			CountVehiclesIterator countVehiclesIterator = new CountVehiclesIterator
			{
				m_Lane = entity,
				m_Bounds = VehicleUtils.GetConnectionParkingBounds(connectionLane, curve.m_Bezier),
				m_ParkedCarData = m_ParkedCarData,
				m_ControllerData = m_ControllerData
			};
			Owner owner2 = owner;
			while (m_OwnerData.HasComponent(owner2.m_Owner))
			{
				owner2 = m_OwnerData[owner2.m_Owner];
			}
			if (m_BuildingData.HasComponent(owner2.m_Owner))
			{
				PrefabRef prefabRef = m_PrefabRefData[owner2.m_Owner];
				DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
				if (m_ActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val))
				{
					Transform transform = m_TransformData[owner2.m_Owner];
					ActivityMask activityMask = new ActivityMask(ActivityType.GarageSpot);
					for (int i = 0; i < val.Length; i++)
					{
						ActivityLocationElement activityLocationElement = val[i];
						if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask.m_Mask) != 0)
						{
							float3 val2 = ObjectUtils.LocalToWorld(transform, activityLocationElement.m_Position);
							countVehiclesIterator.m_Bounds.min = math.min(countVehiclesIterator.m_Bounds.min, val2 - 1f);
							countVehiclesIterator.m_Bounds.max = math.max(countVehiclesIterator.m_Bounds.max, val2 + 1f);
						}
					}
				}
			}
			m_MovingObjectSearchTree.Iterate<CountVehiclesIterator>(ref countVehiclesIterator, 0);
			return (ushort)math.clamp(countVehiclesIterator.m_Result, 0, 65535);
		}

		private ushort CountVehicles(Entity entity, Transform transform, Game.Objects.SpawnLocation spawnLocation, SpawnLocationData spawnLocationData)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			switch (spawnLocationData.m_ConnectionType)
			{
			case RouteConnectionType.Air:
			{
				CountVehiclesIterator countVehiclesIterator = new CountVehiclesIterator
				{
					m_Lane = entity,
					m_Bounds = new Bounds3(transform.m_Position - 1f, transform.m_Position + 1f),
					m_ParkedCarData = m_ParkedCarData,
					m_ControllerData = m_ControllerData
				};
				m_MovingObjectSearchTree.Iterate<CountVehiclesIterator>(ref countVehiclesIterator, 0);
				return (ushort)math.clamp(countVehiclesIterator.m_Result, 0, 65535);
			}
			case RouteConnectionType.Track:
			{
				int num = 0;
				DynamicBuffer<LaneObject> val = default(DynamicBuffer<LaneObject>);
				if (m_LaneObjects.TryGetBuffer(spawnLocation.m_ConnectedLane1, ref val))
				{
					ParkedTrain parkedTrain = default(ParkedTrain);
					for (int i = 0; i < val.Length; i++)
					{
						LaneObject laneObject = val[i];
						if (m_ParkedTrainData.TryGetComponent(laneObject.m_LaneObject, ref parkedTrain) && parkedTrain.m_ParkingLocation == entity)
						{
							num++;
						}
					}
				}
				return (ushort)math.clamp(num, 0, 65535);
			}
			default:
				return 0;
			}
		}

		private float CalculateFreeSpace(Curve curve, Game.Net.ParkingLane parkingLane, ParkingLaneData parkingLaneData, DynamicBuffer<LaneObject> laneObjects, DynamicBuffer<LaneOverlap> laneOverlaps, Bounds1 blockedRange)
		{
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
			{
				return 0f;
			}
			if (parkingLaneData.m_SlotInterval != 0f)
			{
				int parkingSlotCount = NetUtils.GetParkingSlotCount(curve, parkingLane, parkingLaneData);
				float parkingSlotInterval = NetUtils.GetParkingSlotInterval(curve, parkingLane, parkingLaneData, parkingSlotCount);
				float3 val = curve.m_Bezier.a;
				float2 val2 = float2.op_Implicit(0f);
				float num = 0f;
				float num2 = math.max((parkingLane.m_Flags & (ParkingLaneFlags.StartingLane | ParkingLaneFlags.EndingLane)) switch
				{
					ParkingLaneFlags.StartingLane => curve.m_Length - (float)parkingSlotCount * parkingSlotInterval, 
					ParkingLaneFlags.EndingLane => 0f, 
					_ => (curve.m_Length - (float)parkingSlotCount * parkingSlotInterval) * 0.5f, 
				}, 0f);
				int i = -1;
				float num3 = 2f;
				int num4 = 0;
				while (num4 < laneObjects.Length)
				{
					LaneObject laneObject = laneObjects[num4++];
					if (m_ParkedCarData.HasComponent(laneObject.m_LaneObject) && !m_UnspawnedData.HasComponent(laneObject.m_LaneObject))
					{
						num3 = laneObject.m_CurvePosition.x;
						break;
					}
				}
				float2 val3 = float2.op_Implicit(2f);
				int num5 = 0;
				if (num5 < laneOverlaps.Length)
				{
					LaneOverlap laneOverlap = laneOverlaps[num5++];
					val3 = new float2((float)(int)laneOverlap.m_ThisStart, (float)(int)laneOverlap.m_ThisEnd) * 0.003921569f;
				}
				for (int j = 1; j <= 16; j++)
				{
					float num6 = (float)j * 0.0625f;
					float3 val4 = MathUtils.Position(curve.m_Bezier, num6);
					for (num += math.distance(val, val4); num >= num2 || (j == 16 && i < parkingSlotCount); i++)
					{
						val2.y = math.select(num6, math.lerp(val2.x, num6, num2 / num), num2 < num);
						bool flag = false;
						if (num3 <= val2.y)
						{
							num3 = 2f;
							flag = true;
							while (num4 < laneObjects.Length)
							{
								LaneObject laneObject2 = laneObjects[num4++];
								if (m_ParkedCarData.HasComponent(laneObject2.m_LaneObject) && !m_UnspawnedData.HasComponent(laneObject2.m_LaneObject) && laneObject2.m_CurvePosition.x > val2.y)
								{
									num3 = laneObject2.m_CurvePosition.x;
									break;
								}
							}
						}
						if (val3.x < val2.y)
						{
							flag = true;
							if (val3.y <= val2.y)
							{
								val3 = float2.op_Implicit(2f);
								while (num5 < laneOverlaps.Length)
								{
									LaneOverlap laneOverlap2 = laneOverlaps[num5++];
									float2 val5 = new float2((float)(int)laneOverlap2.m_ThisStart, (float)(int)laneOverlap2.m_ThisEnd) * 0.003921569f;
									if (val5.y > val2.y)
									{
										val3 = val5;
										break;
									}
								}
							}
						}
						if (!flag && i >= 0 && i < parkingSlotCount && (val2.x > blockedRange.max || val2.y < blockedRange.min))
						{
							return parkingLaneData.m_MaxCarLength;
						}
						num -= num2;
						val2.x = val2.y;
						num2 = parkingSlotInterval;
					}
					val = val4;
				}
				return 0f;
			}
			float num7 = 0f;
			float2 val6 = float2.op_Implicit(math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.StartingLane) == 0));
			float3 val7 = curve.m_Bezier.a;
			float num8 = 2f;
			float2 val8 = float2.op_Implicit(0f);
			int num9 = 0;
			while (num9 < laneObjects.Length)
			{
				LaneObject laneObject3 = laneObjects[num9++];
				if (m_ParkedCarData.HasComponent(laneObject3.m_LaneObject) && !m_UnspawnedData.HasComponent(laneObject3.m_LaneObject))
				{
					num8 = laneObject3.m_CurvePosition.x;
					val8 = VehicleUtils.GetParkingOffsets(laneObject3.m_LaneObject, ref m_PrefabRefData, ref m_ObjectGeometryData) + 1f;
					break;
				}
			}
			float2 val9 = float2.op_Implicit(2f);
			int num10 = 0;
			if (num10 < laneOverlaps.Length)
			{
				LaneOverlap laneOverlap3 = laneOverlaps[num10++];
				val9 = new float2((float)(int)laneOverlap3.m_ThisStart, (float)(int)laneOverlap3.m_ThisEnd) * 0.003921569f;
			}
			float3 val10 = default(float3);
			float3 val11 = default(float3);
			if (blockedRange.max >= blockedRange.min)
			{
				val10 = MathUtils.Position(curve.m_Bezier, MathUtils.Center(blockedRange));
				val11.x = math.distance(MathUtils.Position(curve.m_Bezier, blockedRange.min), val10);
				val11.y = math.distance(MathUtils.Position(curve.m_Bezier, blockedRange.max), val10);
			}
			float num11;
			while (num8 != 2f || val9.x != 2f)
			{
				float2 val12;
				float x;
				if (num8 <= val9.x)
				{
					val12 = float2.op_Implicit(num8);
					val6.y = val8.x;
					x = val8.y;
					num8 = 2f;
					while (num9 < laneObjects.Length)
					{
						LaneObject laneObject4 = laneObjects[num9++];
						if (m_ParkedCarData.HasComponent(laneObject4.m_LaneObject) && !m_UnspawnedData.HasComponent(laneObject4.m_LaneObject))
						{
							num8 = laneObject4.m_CurvePosition.x;
							val8 = VehicleUtils.GetParkingOffsets(laneObject4.m_LaneObject, ref m_PrefabRefData, ref m_ObjectGeometryData) + 1f;
							break;
						}
					}
				}
				else
				{
					val12 = val9;
					val6.y = 0.5f;
					x = 0.5f;
					val9 = float2.op_Implicit(2f);
					while (num10 < laneOverlaps.Length)
					{
						LaneOverlap laneOverlap4 = laneOverlaps[num10++];
						float2 val13 = new float2((float)(int)laneOverlap4.m_ThisStart, (float)(int)laneOverlap4.m_ThisEnd) * 0.003921569f;
						if (val13.x <= val12.y)
						{
							val12.y = math.max(val12.y, val13.y);
							continue;
						}
						val9 = val13;
						break;
					}
				}
				float3 val14 = MathUtils.Position(curve.m_Bezier, val12.x);
				num11 = math.distance(val7, val14) - math.csum(val6);
				if (blockedRange.max >= blockedRange.min)
				{
					float num12 = math.distance(val7, val10) - val6.x - val11.x;
					float num13 = math.distance(val14, val10) - val6.y - val11.y;
					num11 = math.min(num11, math.max(num12, num13));
				}
				num7 = math.max(num7, num11);
				val6.x = x;
				val7 = MathUtils.Position(curve.m_Bezier, val12.y);
			}
			val6.y = math.select(0f, 0.5f, (parkingLane.m_Flags & ParkingLaneFlags.EndingLane) == 0);
			num11 = math.distance(val7, curve.m_Bezier.d) - math.csum(val6);
			if (blockedRange.max >= blockedRange.min)
			{
				float num14 = math.distance(val7, val10) - val6.x - val11.x;
				float num15 = math.distance(curve.m_Bezier.d, val10) - val6.y - val11.y;
				num11 = math.min(num11, math.max(num14, num15));
			}
			num7 = math.max(num7, num11);
			return math.select(num7, math.min(num7, parkingLaneData.m_MaxCarLength), parkingLaneData.m_MaxCarLength != 0f);
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
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LaneOverlap> __Game_Net_LaneOverlap_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Net.ParkingLane> __Game_Net_ParkingLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<GarageLane> __Game_Net_GarageLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RW_ComponentTypeHandle;

		public BufferTypeHandle<LaneObject> __Game_Net_LaneObject_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<District> __Game_Areas_District_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ParkingFacility> __Game_Buildings_ParkingFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.City.City> __Game_City_City_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Unspawned> __Game_Objects_Unspawned_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingFacilityData> __Game_Prefabs_ParkingFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnLocationData> __Game_Prefabs_SpawnLocationData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<BuildingModifier> __Game_Buildings_BuildingModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_LaneOverlap_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneOverlap>(true);
			__Game_Net_ParkingLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ParkingLane>(false);
			__Game_Net_ConnectionLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ConnectionLane>(false);
			__Game_Net_GarageLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarageLane>(false);
			__Game_Objects_SpawnLocation_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.SpawnLocation>(false);
			__Game_Net_LaneObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneObject>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_Areas_District_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<District>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ParkingFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ParkingFacility>(true);
			__Game_City_City_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.City>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Unspawned_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Unspawned>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_ParkingFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingFacilityData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_SpawnLocationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnLocationData>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_Buildings_BuildingModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BuildingModifier>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Prefabs_ActivityLocationElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ActivityLocationElement>(true);
		}
	}

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_LaneQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<GarageLane>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PathfindUpdated>() };
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<GarageLane>(),
			ComponentType.ReadOnly<Game.Objects.SpawnLocation>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_LaneQuery);
	}

	[Preserve]
	protected override void OnUpdate()
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
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.ScheduleParallel<UpdateLaneDataJob>(new UpdateLaneDataJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneOverlapType = InternalCompilerInterface.GetBufferTypeHandle<LaneOverlap>(ref __TypeHandle.__Game_Net_LaneOverlap_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneType = InternalCompilerInterface.GetComponentTypeHandle<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnLocationType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectType = InternalCompilerInterface.GetBufferTypeHandle<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictData = InternalCompilerInterface.GetComponentLookup<District>(ref __TypeHandle.__Game_Areas_District_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ParkingFacility>(ref __TypeHandle.__Game_Buildings_ParkingFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityData = InternalCompilerInterface.GetComponentLookup<Game.City.City>(ref __TypeHandle.__Game_City_City_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedData = InternalCompilerInterface.GetComponentLookup<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabParkingFacilityData = InternalCompilerInterface.GetComponentLookup<ParkingFacilityData>(ref __TypeHandle.__Game_Prefabs_ParkingFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingPropertyData = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkplaceData = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnLocationData = InternalCompilerInterface.GetComponentLookup<SpawnLocationData>(ref __TypeHandle.__Game_Prefabs_SpawnLocationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingModifiers = InternalCompilerInterface.GetBufferLookup<BuildingModifier>(ref __TypeHandle.__Game_Buildings_BuildingModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ActivityLocations = InternalCompilerInterface.GetBufferLookup<ActivityLocationElement>(ref __TypeHandle.__Game_Prefabs_ActivityLocationElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_MovingObjectSearchTree = m_ObjectSearchSystem.GetMovingSearchTree(readOnly: true, out dependencies)
		}, m_LaneQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		m_ObjectSearchSystem.AddMovingSearchTreeReader(val);
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
	public ParkingLaneDataSystem()
	{
	}
}
