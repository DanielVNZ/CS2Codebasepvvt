using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Vehicles;

[CompilerGenerated]
public class ReferencesSystem : GameSystemBase
{
	[BurstCompile]
	public struct InitializeCurrentLaneJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_LaneData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<CarCurrentLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Transform transform = nativeArray[i];
				CarCurrentLane carCurrentLane = nativeArray2[i];
				if (m_CarLaneData.HasComponent(carCurrentLane.m_Lane))
				{
					Game.Net.CarLane carLane = m_CarLaneData[carCurrentLane.m_Lane];
					Entity owner = m_OwnerData[carCurrentLane.m_Lane].m_Owner;
					DynamicBuffer<Game.Net.SubLane> val = m_LaneData[owner];
					carCurrentLane.m_Lane = Entity.Null;
					float3 curvePosition = carCurrentLane.m_CurvePosition;
					float num = float.MaxValue;
					for (int j = 0; j < val.Length; j++)
					{
						Entity subLane = val[j].m_SubLane;
						if (!m_CarLaneData.HasComponent(subLane) || m_MasterLaneData.HasComponent(subLane))
						{
							continue;
						}
						Game.Net.CarLane carLane2 = m_CarLaneData[subLane];
						if (carLane2.m_CarriagewayGroup == carLane.m_CarriagewayGroup)
						{
							float3 val2 = math.select(curvePosition, 1f - ((float3)(ref curvePosition)).zyx, ((carLane.m_Flags ^ carLane2.m_Flags) & Game.Net.CarLaneFlags.Invert) != 0);
							float num2 = math.lengthsq(MathUtils.Position(m_CurveData[subLane].m_Bezier, val2.x) - transform.m_Position);
							if (num2 < num)
							{
								carCurrentLane.m_Lane = subLane;
								carCurrentLane.m_CurvePosition = val2;
								num = num2;
							}
						}
					}
				}
				nativeArray2[i] = carCurrentLane;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLayoutReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public ComponentLookup<Controller> m_ControllerData;

		public BufferLookup<LayoutElement> m_Layouts;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				if (m_Layouts.HasBuffer(val))
				{
					DynamicBuffer<LayoutElement> val2 = m_Layouts[val];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity vehicle = val2[j].m_Vehicle;
						if (vehicle != val && vehicle != Entity.Null && !m_DeletedData.HasComponent(vehicle) && m_ControllerData.HasComponent(vehicle))
						{
							Controller controller = m_ControllerData[vehicle];
							if (controller.m_Controller == val)
							{
								controller.m_Controller = Entity.Null;
								m_ControllerData[vehicle] = controller;
							}
						}
					}
				}
				if (m_ControllerData.HasComponent(val))
				{
					Controller controller2 = m_ControllerData[val];
					if (controller2.m_Controller != val && controller2.m_Controller != Entity.Null && !m_DeletedData.HasComponent(controller2.m_Controller) && m_Layouts.HasBuffer(controller2.m_Controller))
					{
						CollectionUtils.RemoveValue<LayoutElement>(m_Layouts[controller2.m_Controller], new LayoutElement(val));
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
	private struct UpdateVehicleReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Target> m_TargetType;

		[ReadOnly]
		public ComponentTypeHandle<PersonalCar> m_PersonalCarType;

		[ReadOnly]
		public ComponentTypeHandle<DeliveryTruck> m_DeliveryTruckType;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> m_CarCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> m_CarTrailerLaneType;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> m_WatercraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> m_AircraftCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> m_TrainCurrentLaneType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> m_ParkedCarType;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> m_ParkedTrainType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Moving> m_MovingType;

		[ReadOnly]
		public ComponentTypeHandle<Odometer> m_OdometerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> m_CurrentRouteType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<BlockedLane> m_BlockedLaneType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> m_SpawnLocationData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public ComponentLookup<MasterLane> m_MasterLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> m_PublicTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> m_CargoTransportVehicleData;

		[ReadOnly]
		public ComponentLookup<TaxiData> m_TaxiData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		public ComponentLookup<CarKeeper> m_CarKeepers;

		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepots;

		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		public BufferLookup<GuestVehicle> m_GuestVehicles;

		public BufferLookup<LaneObject> m_LaneObjects;

		public BufferLookup<RouteVehicle> m_RouteVehicles;

		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_SearchTree;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0998: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0def: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e27: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f26: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
			//IL_1012: Unknown result type (might be due to invalid IL or missing references)
			//IL_1017: Unknown result type (might be due to invalid IL or missing references)
			//IL_102c: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1045: Unknown result type (might be due to invalid IL or missing references)
			//IL_104c: Unknown result type (might be due to invalid IL or missing references)
			//IL_103a: Unknown result type (might be due to invalid IL or missing references)
			//IL_103c: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0734: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_107f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1060: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_1121: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_1093: Unknown result type (might be due to invalid IL or missing references)
			//IL_106e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1070: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_10fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_1134: Unknown result type (might be due to invalid IL or missing references)
			//IL_1139: Unknown result type (might be due to invalid IL or missing references)
			//IL_113f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_115e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1172: Unknown result type (might be due to invalid IL or missing references)
			//IL_1177: Unknown result type (might be due to invalid IL or missing references)
			//IL_117c: Unknown result type (might be due to invalid IL or missing references)
			//IL_118f: Unknown result type (might be due to invalid IL or missing references)
			//IL_119e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				DynamicBuffer<OwnedVehicle> val = default(DynamicBuffer<OwnedVehicle>);
				for (int i = 0; i < nativeArray4.Length; i++)
				{
					Entity vehicle = nativeArray[i];
					Owner owner = nativeArray4[i];
					if (m_OwnedVehicles.TryGetBuffer(owner.m_Owner, ref val))
					{
						CollectionUtils.TryAddUniqueValue<OwnedVehicle>(val, new OwnedVehicle(vehicle));
					}
				}
				NativeArray<PersonalCar> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCar>(ref m_PersonalCarType);
				for (int j = 0; j < nativeArray5.Length; j++)
				{
					Entity car = nativeArray[j];
					PersonalCar personalCar = nativeArray5[j];
					if (m_CarKeepers.HasComponent(personalCar.m_Keeper))
					{
						CarKeeper carKeeper = m_CarKeepers[personalCar.m_Keeper];
						carKeeper.m_Car = car;
						m_CarKeepers[personalCar.m_Keeper] = carKeeper;
					}
				}
				if (((ArchetypeChunk)(ref chunk)).Has<DeliveryTruck>(ref m_DeliveryTruckType))
				{
					NativeArray<Target> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
					for (int k = 0; k < nativeArray6.Length; k++)
					{
						Entity vehicle2 = nativeArray[k];
						Target target = nativeArray6[k];
						if (m_GuestVehicles.HasBuffer(target.m_Target))
						{
							m_GuestVehicles[target.m_Target].Add(new GuestVehicle(vehicle2));
						}
					}
				}
				NativeArray<CarCurrentLane> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
				for (int l = 0; l < nativeArray7.Length; l++)
				{
					Entity val2 = nativeArray[l];
					CarCurrentLane carCurrentLane = nativeArray7[l];
					if (m_LaneObjects.HasBuffer(carCurrentLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[carCurrentLane.m_Lane], val2, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
					}
					else
					{
						Transform transform = nativeArray2[l];
						PrefabRef prefabRef = nativeArray3[l];
						ObjectGeometryData geometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
						Bounds3 bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
						m_SearchTree.Add(val2, new QuadTreeBoundsXZ(bounds));
					}
					if (m_LaneObjects.HasBuffer(carCurrentLane.m_ChangeLane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[carCurrentLane.m_ChangeLane], val2, ((float3)(ref carCurrentLane.m_CurvePosition)).xy);
					}
				}
				NativeArray<CarTrailerLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerLane>(ref m_CarTrailerLaneType);
				for (int m = 0; m < nativeArray8.Length; m++)
				{
					Entity val3 = nativeArray[m];
					CarTrailerLane carTrailerLane = nativeArray8[m];
					if (m_LaneObjects.HasBuffer(carTrailerLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[carTrailerLane.m_Lane], val3, ((float2)(ref carTrailerLane.m_CurvePosition)).xy);
					}
					else
					{
						Transform transform2 = nativeArray2[m];
						PrefabRef prefabRef2 = nativeArray3[m];
						ObjectGeometryData geometryData2 = m_ObjectGeometryData[prefabRef2.m_Prefab];
						Bounds3 bounds2 = ObjectUtils.CalculateBounds(transform2.m_Position, transform2.m_Rotation, geometryData2);
						m_SearchTree.Add(val3, new QuadTreeBoundsXZ(bounds2));
					}
					if (m_LaneObjects.HasBuffer(carTrailerLane.m_NextLane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[carTrailerLane.m_NextLane], val3, ((float2)(ref carTrailerLane.m_NextPosition)).xy);
					}
				}
				NativeArray<WatercraftCurrentLane> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
				for (int n = 0; n < nativeArray9.Length; n++)
				{
					Entity val4 = nativeArray[n];
					WatercraftCurrentLane watercraftCurrentLane = nativeArray9[n];
					if (m_LaneObjects.HasBuffer(watercraftCurrentLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[watercraftCurrentLane.m_Lane], val4, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
					}
					else
					{
						Transform transform3 = nativeArray2[n];
						PrefabRef prefabRef3 = nativeArray3[n];
						ObjectGeometryData geometryData3 = m_ObjectGeometryData[prefabRef3.m_Prefab];
						Bounds3 bounds3 = ObjectUtils.CalculateBounds(transform3.m_Position, transform3.m_Rotation, geometryData3);
						m_SearchTree.Add(val4, new QuadTreeBoundsXZ(bounds3));
					}
					if (m_LaneObjects.HasBuffer(watercraftCurrentLane.m_ChangeLane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[watercraftCurrentLane.m_ChangeLane], val4, ((float3)(ref watercraftCurrentLane.m_CurvePosition)).xy);
					}
				}
				NativeArray<AircraftCurrentLane> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
				for (int num = 0; num < nativeArray10.Length; num++)
				{
					Entity val5 = nativeArray[num];
					AircraftCurrentLane aircraftCurrentLane = nativeArray10[num];
					if (m_LaneObjects.HasBuffer(aircraftCurrentLane.m_Lane))
					{
						NetUtils.AddLaneObject(m_LaneObjects[aircraftCurrentLane.m_Lane], val5, ((float3)(ref aircraftCurrentLane.m_CurvePosition)).xy);
					}
					if (!m_LaneObjects.HasBuffer(aircraftCurrentLane.m_Lane) || (aircraftCurrentLane.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
					{
						Transform transform4 = nativeArray2[num];
						PrefabRef prefabRef4 = nativeArray3[num];
						ObjectGeometryData geometryData4 = m_ObjectGeometryData[prefabRef4.m_Prefab];
						Bounds3 bounds4 = ObjectUtils.CalculateBounds(transform4.m_Position, transform4.m_Rotation, geometryData4);
						m_SearchTree.Add(val5, new QuadTreeBoundsXZ(bounds4));
					}
				}
				NativeArray<TrainCurrentLane> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_TrainCurrentLaneType);
				DynamicBuffer<LaneObject> buffer = default(DynamicBuffer<LaneObject>);
				for (int num2 = 0; num2 < nativeArray11.Length; num2++)
				{
					Entity laneObject = nativeArray[num2];
					TrainCurrentLane currentLane = nativeArray11[num2];
					TrainNavigationHelpers.GetCurvePositions(ref currentLane, out var pos, out var pos2);
					if (m_LaneObjects.TryGetBuffer(currentLane.m_Front.m_Lane, ref buffer))
					{
						NetUtils.AddLaneObject(buffer, laneObject, pos);
					}
					if (currentLane.m_Rear.m_Lane != currentLane.m_Front.m_Lane && m_LaneObjects.TryGetBuffer(currentLane.m_Rear.m_Lane, ref buffer))
					{
						NetUtils.AddLaneObject(buffer, laneObject, pos2);
					}
				}
				NativeArray<ParkedCar> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedCar>(ref m_ParkedCarType);
				DynamicBuffer<LaneObject> buffer2 = default(DynamicBuffer<LaneObject>);
				for (int num3 = 0; num3 < nativeArray12.Length; num3++)
				{
					Entity val6 = nativeArray[num3];
					ParkedCar parkedCar = nativeArray12[num3];
					if (m_LaneObjects.TryGetBuffer(parkedCar.m_Lane, ref buffer2))
					{
						NetUtils.AddLaneObject(buffer2, val6, float2.op_Implicit(parkedCar.m_CurvePosition));
						continue;
					}
					Transform transform5 = nativeArray2[num3];
					PrefabRef prefabRef5 = nativeArray3[num3];
					ObjectGeometryData geometryData5 = m_ObjectGeometryData[prefabRef5.m_Prefab];
					Bounds3 bounds5 = ObjectUtils.CalculateBounds(transform5.m_Position, transform5.m_Rotation, geometryData5);
					m_SearchTree.Add(val6, new QuadTreeBoundsXZ(bounds5));
				}
				NativeArray<ParkedTrain> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedTrain>(ref m_ParkedTrainType);
				DynamicBuffer<LaneObject> buffer3 = default(DynamicBuffer<LaneObject>);
				for (int num4 = 0; num4 < nativeArray13.Length; num4++)
				{
					Entity laneObject2 = nativeArray[num4];
					ParkedTrain parkedTrain = nativeArray13[num4];
					TrainNavigationHelpers.GetCurvePositions(ref parkedTrain, out var pos3, out var pos4);
					if (m_LaneObjects.TryGetBuffer(parkedTrain.m_FrontLane, ref buffer3))
					{
						NetUtils.AddLaneObject(buffer3, laneObject2, pos3);
					}
					if (parkedTrain.m_RearLane != parkedTrain.m_FrontLane && m_LaneObjects.TryGetBuffer(parkedTrain.m_RearLane, ref buffer3))
					{
						NetUtils.AddLaneObject(buffer3, laneObject2, pos4);
					}
				}
				NativeArray<CurrentRoute> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
				DynamicBuffer<RouteVehicle> val7 = default(DynamicBuffer<RouteVehicle>);
				for (int num5 = 0; num5 < nativeArray14.Length; num5++)
				{
					Entity vehicle3 = nativeArray[num5];
					CurrentRoute currentRoute = nativeArray14[num5];
					if (m_RouteVehicles.TryGetBuffer(currentRoute.m_Route, ref val7))
					{
						CollectionUtils.TryAddUniqueValue<RouteVehicle>(val7, new RouteVehicle(vehicle3));
					}
				}
				return;
			}
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Moving>(ref m_MovingType);
			NativeArray<Owner> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			if (nativeArray15.Length != 0)
			{
				NativeArray<Odometer> nativeArray16 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Odometer>(ref m_OdometerType);
				NativeArray<PrefabRef> nativeArray17 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				DynamicBuffer<OwnedVehicle> val8 = default(DynamicBuffer<OwnedVehicle>);
				PublicTransportVehicleData publicTransportVehicleData = default(PublicTransportVehicleData);
				CargoTransportVehicleData cargoTransportVehicleData = default(CargoTransportVehicleData);
				TaxiData taxiData = default(TaxiData);
				for (int num6 = 0; num6 < nativeArray15.Length; num6++)
				{
					Entity vehicle4 = nativeArray[num6];
					Owner owner2 = nativeArray15[num6];
					if (m_OwnedVehicles.TryGetBuffer(owner2.m_Owner, ref val8))
					{
						CollectionUtils.RemoveValue<OwnedVehicle>(val8, new OwnedVehicle(vehicle4));
					}
					if (nativeArray16.Length == 0 || !m_TransportDepots.HasComponent(owner2.m_Owner))
					{
						continue;
					}
					Odometer odometer = nativeArray16[num6];
					PrefabRef prefabRef6 = nativeArray17[num6];
					Game.Buildings.TransportDepot transportDepot = m_TransportDepots[owner2.m_Owner];
					if (m_PublicTransportVehicleData.TryGetComponent(prefabRef6.m_Prefab, ref publicTransportVehicleData))
					{
						if (publicTransportVehicleData.m_MaintenanceRange > 0.1f)
						{
							transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / publicTransportVehicleData.m_MaintenanceRange);
							m_TransportDepots[owner2.m_Owner] = transportDepot;
						}
					}
					else if (m_CargoTransportVehicleData.TryGetComponent(prefabRef6.m_Prefab, ref cargoTransportVehicleData))
					{
						if (cargoTransportVehicleData.m_MaintenanceRange > 0.1f)
						{
							transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / cargoTransportVehicleData.m_MaintenanceRange);
							m_TransportDepots[owner2.m_Owner] = transportDepot;
						}
					}
					else if (m_TaxiData.TryGetComponent(prefabRef6.m_Prefab, ref taxiData) && taxiData.m_MaintenanceRange > 0.1f)
					{
						transportDepot.m_MaintenanceRequirement += math.saturate(odometer.m_Distance / taxiData.m_MaintenanceRange);
						m_TransportDepots[owner2.m_Owner] = transportDepot;
					}
				}
			}
			NativeArray<PersonalCar> nativeArray18 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PersonalCar>(ref m_PersonalCarType);
			for (int num7 = 0; num7 < nativeArray18.Length; num7++)
			{
				Entity val9 = nativeArray[num7];
				PersonalCar personalCar2 = nativeArray18[num7];
				if (m_CarKeepers.HasComponent(personalCar2.m_Keeper))
				{
					CarKeeper carKeeper2 = m_CarKeepers[personalCar2.m_Keeper];
					if (carKeeper2.m_Car == val9)
					{
						carKeeper2.m_Car = Entity.Null;
						m_CarKeepers[personalCar2.m_Keeper] = carKeeper2;
					}
				}
			}
			if (((ArchetypeChunk)(ref chunk)).Has<DeliveryTruck>(ref m_DeliveryTruckType))
			{
				NativeArray<Target> nativeArray19 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
				for (int num8 = 0; num8 < nativeArray19.Length; num8++)
				{
					Entity vehicle5 = nativeArray[num8];
					Target target2 = nativeArray19[num8];
					if (m_GuestVehicles.HasBuffer(target2.m_Target))
					{
						CollectionUtils.RemoveValue<GuestVehicle>(m_GuestVehicles[target2.m_Target], new GuestVehicle(vehicle5));
					}
				}
			}
			NativeArray<CarCurrentLane> nativeArray20 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CarCurrentLaneType);
			for (int num9 = 0; num9 < nativeArray20.Length; num9++)
			{
				Entity val10 = nativeArray[num9];
				CarCurrentLane carCurrentLane2 = nativeArray20[num9];
				if (m_LaneObjects.HasBuffer(carCurrentLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[carCurrentLane2.m_Lane], val10);
					if (!flag && m_CarLaneData.HasComponent(carCurrentLane2.m_Lane))
					{
						AddLaneComponent<PathfindUpdated>(carCurrentLane2.m_Lane, default(PathfindUpdated));
					}
				}
				else
				{
					m_SearchTree.TryRemove(val10);
				}
				if (m_LaneObjects.HasBuffer(carCurrentLane2.m_ChangeLane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[carCurrentLane2.m_ChangeLane], val10);
					if (!flag && m_CarLaneData.HasComponent(carCurrentLane2.m_ChangeLane))
					{
						AddLaneComponent<PathfindUpdated>(carCurrentLane2.m_ChangeLane, default(PathfindUpdated));
					}
				}
			}
			NativeArray<CarTrailerLane> nativeArray21 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarTrailerLane>(ref m_CarTrailerLaneType);
			for (int num10 = 0; num10 < nativeArray21.Length; num10++)
			{
				Entity val11 = nativeArray[num10];
				CarTrailerLane carTrailerLane2 = nativeArray21[num10];
				if (m_LaneObjects.HasBuffer(carTrailerLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[carTrailerLane2.m_Lane], val11);
					if (!flag && m_CarLaneData.HasComponent(carTrailerLane2.m_Lane))
					{
						AddLaneComponent<PathfindUpdated>(carTrailerLane2.m_Lane, default(PathfindUpdated));
					}
				}
				else
				{
					m_SearchTree.TryRemove(val11);
				}
				if (m_LaneObjects.HasBuffer(carTrailerLane2.m_NextLane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[carTrailerLane2.m_NextLane], val11);
					if (!flag && m_CarLaneData.HasComponent(carTrailerLane2.m_NextLane))
					{
						AddLaneComponent<PathfindUpdated>(carTrailerLane2.m_NextLane, default(PathfindUpdated));
					}
				}
			}
			NativeArray<WatercraftCurrentLane> nativeArray22 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WatercraftCurrentLane>(ref m_WatercraftCurrentLaneType);
			for (int num11 = 0; num11 < nativeArray22.Length; num11++)
			{
				Entity val12 = nativeArray[num11];
				WatercraftCurrentLane watercraftCurrentLane2 = nativeArray22[num11];
				if (m_LaneObjects.HasBuffer(watercraftCurrentLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[watercraftCurrentLane2.m_Lane], val12);
				}
				else
				{
					m_SearchTree.TryRemove(val12);
				}
				if (m_LaneObjects.HasBuffer(watercraftCurrentLane2.m_ChangeLane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[watercraftCurrentLane2.m_ChangeLane], val12);
				}
			}
			NativeArray<AircraftCurrentLane> nativeArray23 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<AircraftCurrentLane>(ref m_AircraftCurrentLaneType);
			for (int num12 = 0; num12 < nativeArray23.Length; num12++)
			{
				Entity val13 = nativeArray[num12];
				AircraftCurrentLane aircraftCurrentLane2 = nativeArray23[num12];
				if (m_LaneObjects.HasBuffer(aircraftCurrentLane2.m_Lane))
				{
					NetUtils.RemoveLaneObject(m_LaneObjects[aircraftCurrentLane2.m_Lane], val13);
				}
				if (!m_LaneObjects.HasBuffer(aircraftCurrentLane2.m_Lane) || (aircraftCurrentLane2.m_LaneFlags & AircraftLaneFlags.Flying) != 0)
				{
					m_SearchTree.TryRemove(val13);
				}
			}
			NativeArray<TrainCurrentLane> nativeArray24 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrainCurrentLane>(ref m_TrainCurrentLaneType);
			DynamicBuffer<LaneObject> buffer4 = default(DynamicBuffer<LaneObject>);
			for (int num13 = 0; num13 < nativeArray24.Length; num13++)
			{
				Entity laneObject3 = nativeArray[num13];
				TrainCurrentLane trainCurrentLane = nativeArray24[num13];
				if (m_LaneObjects.TryGetBuffer(trainCurrentLane.m_Front.m_Lane, ref buffer4))
				{
					NetUtils.RemoveLaneObject(buffer4, laneObject3);
				}
				if (trainCurrentLane.m_Rear.m_Lane != trainCurrentLane.m_Front.m_Lane && m_LaneObjects.TryGetBuffer(trainCurrentLane.m_Rear.m_Lane, ref buffer4))
				{
					NetUtils.RemoveLaneObject(buffer4, laneObject3);
				}
			}
			NativeArray<ParkedCar> nativeArray25 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedCar>(ref m_ParkedCarType);
			DynamicBuffer<LaneObject> buffer5 = default(DynamicBuffer<LaneObject>);
			for (int num14 = 0; num14 < nativeArray25.Length; num14++)
			{
				Entity val14 = nativeArray[num14];
				ParkedCar parkedCar2 = nativeArray25[num14];
				if (m_LaneObjects.TryGetBuffer(parkedCar2.m_Lane, ref buffer5))
				{
					NetUtils.RemoveLaneObject(buffer5, val14);
					if (m_ParkingLaneData.HasComponent(parkedCar2.m_Lane) || m_GarageLaneData.HasComponent(parkedCar2.m_Lane))
					{
						AddLaneComponent<PathfindUpdated>(parkedCar2.m_Lane, default(PathfindUpdated));
					}
				}
				else
				{
					m_SearchTree.TryRemove(val14);
					if (m_SpawnLocationData.HasComponent(parkedCar2.m_Lane))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(parkedCar2.m_Lane, default(PathfindUpdated));
					}
				}
			}
			NativeArray<ParkedTrain> nativeArray26 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ParkedTrain>(ref m_ParkedTrainType);
			DynamicBuffer<LaneObject> buffer6 = default(DynamicBuffer<LaneObject>);
			for (int num15 = 0; num15 < nativeArray26.Length; num15++)
			{
				Entity laneObject4 = nativeArray[num15];
				ParkedTrain parkedTrain2 = nativeArray26[num15];
				if (m_LaneObjects.TryGetBuffer(parkedTrain2.m_FrontLane, ref buffer6))
				{
					NetUtils.RemoveLaneObject(buffer6, laneObject4);
				}
				if (parkedTrain2.m_RearLane != parkedTrain2.m_FrontLane && m_LaneObjects.TryGetBuffer(parkedTrain2.m_RearLane, ref buffer6))
				{
					NetUtils.RemoveLaneObject(buffer6, laneObject4);
				}
				if (m_SpawnLocationData.HasComponent(parkedTrain2.m_ParkingLocation))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(parkedTrain2.m_ParkingLocation, default(PathfindUpdated));
				}
			}
			NativeArray<CurrentRoute> nativeArray27 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentRoute>(ref m_CurrentRouteType);
			DynamicBuffer<RouteVehicle> val15 = default(DynamicBuffer<RouteVehicle>);
			for (int num16 = 0; num16 < nativeArray27.Length; num16++)
			{
				Entity vehicle6 = nativeArray[num16];
				CurrentRoute currentRoute2 = nativeArray27[num16];
				if (m_RouteVehicles.TryGetBuffer(currentRoute2.m_Route, ref val15))
				{
					CollectionUtils.RemoveValue<RouteVehicle>(val15, new RouteVehicle(vehicle6));
				}
			}
			BufferAccessor<BlockedLane> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BlockedLane>(ref m_BlockedLaneType);
			for (int num17 = 0; num17 < bufferAccessor.Length; num17++)
			{
				Entity laneObject5 = nativeArray[num17];
				DynamicBuffer<BlockedLane> val16 = bufferAccessor[num17];
				for (int num18 = 0; num18 < val16.Length; num18++)
				{
					BlockedLane blockedLane = val16[num18];
					if (m_LaneObjects.HasBuffer(blockedLane.m_Lane))
					{
						NetUtils.RemoveLaneObject(m_LaneObjects[blockedLane.m_Lane], laneObject5);
						if (!flag && m_CarLaneData.HasComponent(blockedLane.m_Lane))
						{
							AddLaneComponent<PathfindUpdated>(blockedLane.m_Lane, default(PathfindUpdated));
						}
					}
				}
			}
		}

		private void AddLaneComponent<T>(Entity lane, T component) where T : unmanaged, IComponentData
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<T>(lane, component);
			if (!m_SlaveLaneData.HasComponent(lane) || !m_OwnerData.HasComponent(lane))
			{
				return;
			}
			uint num = m_SlaveLaneData[lane].m_Group;
			Entity owner = m_OwnerData[lane].m_Owner;
			DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_MasterLaneData.HasComponent(subLane) && m_MasterLaneData[subLane].m_Group == num)
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<T>(subLane, component);
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
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MasterLane> __Game_Net_MasterLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public ComponentLookup<Controller> __Game_Vehicles_Controller_RW_ComponentLookup;

		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Target> __Game_Common_Target_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DeliveryTruck> __Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Moving> __Game_Objects_Moving_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Odometer> __Game_Vehicles_Odometer_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<BlockedLane> __Game_Objects_BlockedLane_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Objects.SpawnLocation> __Game_Objects_SpawnLocation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PublicTransportVehicleData> __Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CargoTransportVehicleData> __Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiData> __Game_Prefabs_TaxiData_RO_ComponentLookup;

		public ComponentLookup<CarKeeper> __Game_Citizens_CarKeeper_RW_ComponentLookup;

		public ComponentLookup<Game.Buildings.TransportDepot> __Game_Buildings_TransportDepot_RW_ComponentLookup;

		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RW_BufferLookup;

		public BufferLookup<GuestVehicle> __Game_Vehicles_GuestVehicle_RW_BufferLookup;

		public BufferLookup<LaneObject> __Game_Net_LaneObject_RW_BufferLookup;

		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RW_BufferLookup;

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
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Net_MasterLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MasterLane>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Vehicles_Controller_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(false);
			__Game_Vehicles_LayoutElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(false);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Common_Target_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PersonalCar>(true);
			__Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DeliveryTruck>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(true);
			__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTrailerLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AircraftCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrainCurrentLane>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkedTrain>(true);
			__Game_Objects_Moving_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Moving>(true);
			__Game_Vehicles_Odometer_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Odometer>(true);
			__Game_Routes_CurrentRoute_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentRoute>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_BlockedLane_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BlockedLane>(true);
			__Game_Objects_SpawnLocation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.SpawnLocation>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PublicTransportVehicleData>(true);
			__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CargoTransportVehicleData>(true);
			__Game_Prefabs_TaxiData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiData>(true);
			__Game_Citizens_CarKeeper_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarKeeper>(false);
			__Game_Buildings_TransportDepot_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TransportDepot>(false);
			__Game_Vehicles_OwnedVehicle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(false);
			__Game_Vehicles_GuestVehicle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<GuestVehicle>(false);
			__Game_Net_LaneObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(false);
			__Game_Routes_RouteVehicle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(false);
		}
	}

	private ModificationBarrier5 m_ModificationBarrier;

	private Game.Objects.SearchSystem m_SearchSystem;

	private EntityQuery m_CarQuery;

	private EntityQuery m_VehicleQuery;

	private EntityQuery m_LayoutQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_CarQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Vehicle>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LayoutElement>(),
			ComponentType.ReadOnly<Controller>()
		};
		array2[0] = val;
		m_LayoutQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[3] { m_CarQuery, m_VehicleQuery, m_LayoutQuery });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		if (!((EntityQuery)(ref m_CarQuery)).IsEmptyIgnoreFilter)
		{
			val = JobChunkExtensions.ScheduleParallel<InitializeCurrentLaneJob>(new InitializeCurrentLaneJob
			{
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneData = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_CarQuery, ((SystemBase)this).Dependency);
		}
		JobHandle val2 = default(JobHandle);
		if (!((EntityQuery)(ref m_LayoutQuery)).IsEmptyIgnoreFilter)
		{
			val2 = JobChunkExtensions.Schedule<UpdateLayoutReferencesJob>(new UpdateLayoutReferencesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Layouts = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, m_LayoutQuery, ((SystemBase)this).Dependency);
		}
		JobHandle val3 = default(JobHandle);
		if (!((EntityQuery)(ref m_VehicleQuery)).IsEmptyIgnoreFilter)
		{
			val3 = JobChunkExtensions.Schedule<UpdateVehicleReferencesJob>(new UpdateVehicleReferencesJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PersonalCarType = InternalCompilerInterface.GetComponentTypeHandle<PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeliveryTruckType = InternalCompilerInterface.GetComponentTypeHandle<DeliveryTruck>(ref __TypeHandle.__Game_Vehicles_DeliveryTruck_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarTrailerLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_WatercraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AircraftCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TrainCurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkedCarType = InternalCompilerInterface.GetComponentTypeHandle<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ParkedTrainType = InternalCompilerInterface.GetComponentTypeHandle<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_MovingType = InternalCompilerInterface.GetComponentTypeHandle<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OdometerType = InternalCompilerInterface.GetComponentTypeHandle<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentRouteType = InternalCompilerInterface.GetComponentTypeHandle<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BlockedLaneType = InternalCompilerInterface.GetBufferTypeHandle<BlockedLane>(ref __TypeHandle.__Game_Objects_BlockedLane_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnLocationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.SpawnLocation>(ref __TypeHandle.__Game_Objects_SpawnLocation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MasterLaneData = InternalCompilerInterface.GetComponentLookup<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportVehicleData = InternalCompilerInterface.GetComponentLookup<PublicTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_PublicTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportVehicleData = InternalCompilerInterface.GetComponentLookup<CargoTransportVehicleData>(ref __TypeHandle.__Game_Prefabs_CargoTransportVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TaxiData = InternalCompilerInterface.GetComponentLookup<TaxiData>(ref __TypeHandle.__Game_Prefabs_TaxiData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarKeepers = InternalCompilerInterface.GetComponentLookup<CarKeeper>(ref __TypeHandle.__Game_Citizens_CarKeeper_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportDepots = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TransportDepot>(ref __TypeHandle.__Game_Buildings_TransportDepot_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GuestVehicles = InternalCompilerInterface.GetBufferLookup<GuestVehicle>(ref __TypeHandle.__Game_Vehicles_GuestVehicle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SearchTree = m_SearchSystem.GetMovingSearchTree(readOnly: false, out var dependencies),
				m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
			}, m_VehicleQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val, dependencies));
			m_SearchSystem.AddMovingSearchTreeWriter(val3);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		}
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val, val2, val3);
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
	public ReferencesSystem()
	{
	}
}
