using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
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
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct RoadPathfindSetup
{
	[BurstCompile]
	private struct SetupMaintenanceProvidersJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.MaintenanceDepot> m_MaintenanceDepotType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<MaintenanceDepotData> m_PrefabMaintenanceDepotData;

		[ReadOnly]
		public ComponentLookup<MaintenanceVehicleData> m_PrefabMaintenanceVehicleData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Buildings.MaintenanceDepot> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.MaintenanceDepot>(ref m_MaintenanceDepotType);
			if (nativeArray3.Length != 0)
			{
				for (int i = 0; i < nativeArray3.Length; i++)
				{
					if ((nativeArray3[i].m_Flags & MaintenanceDepotFlags.HasAvailableVehicles) == 0)
					{
						continue;
					}
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						Entity val = nativeArray[i];
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							PrefabRef prefabRef = nativeArray2[i];
							if ((m_PrefabMaintenanceDepotData[prefabRef.m_Prefab].m_MaintenanceType & targetSeeker.m_SetupQueueTarget.m_MaintenanceType) == targetSeeker.m_SetupQueueTarget.m_MaintenanceType)
							{
								float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
								targetSeeker.FindTargets(val, cost);
							}
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.MaintenanceVehicle> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.MaintenanceVehicle>(ref m_MaintenanceVehicleType);
			if (nativeArray4.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray4.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.MaintenanceVehicle maintenanceVehicle = nativeArray4[k];
				PrefabRef prefabRef2 = nativeArray2[k];
				MaintenanceVehicleData maintenanceVehicleData = m_PrefabMaintenanceVehicleData[prefabRef2.m_Prefab];
				if ((maintenanceVehicle.m_State & (MaintenanceVehicleFlags.EstimatedFull | MaintenanceVehicleFlags.Disabled)) != 0)
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var targetSeeker2);
					if ((maintenanceVehicleData.m_MaintenanceType & targetSeeker2.m_SetupQueueTarget.m_MaintenanceType) != targetSeeker2.m_SetupQueueTarget.m_MaintenanceType || (nativeArray6.Length != 0 && !AreaUtils.CheckServiceDistrict(entity2, nativeArray6[k].m_Owner, m_ServiceDistricts)))
					{
						continue;
					}
					if ((maintenanceVehicle.m_State & MaintenanceVehicleFlags.Returning) != 0 || nativeArray5.Length == 0)
					{
						targetSeeker2.FindTargets(val2, 0f);
						continue;
					}
					PathOwner pathOwner = nativeArray5[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num = math.min(maintenanceVehicle.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					float num2 = 0f;
					bool flag = false;
					if (num >= 1)
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num2 += (float)(val4.Length - pathOwner.m_ElementIndex) * maintenanceVehicle.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
							pathElement = val4[val4.Length - 1];
							flag = true;
						}
					}
					for (int m = 1; m < num; m++)
					{
						Entity request = val3[m].m_Request;
						if (m_PathInformationData.TryGetComponent(request, ref pathInformation))
						{
							num2 += pathInformation.m_Duration * targetSeeker2.m_PathfindParameters.m_Weights.time;
						}
						if (m_PathElements.TryGetBuffer(request, ref val5) && val5.Length != 0)
						{
							pathElement = val5[val5.Length - 1];
							flag = true;
						}
					}
					if (flag)
					{
						targetSeeker2.m_Buffer.Enqueue(new PathTarget(val2, pathElement.m_Target, pathElement.m_TargetDelta.y, num2));
					}
					else
					{
						targetSeeker2.FindTargets(val2, val2, num2, EdgeFlags.DefaultMask, allowAccessRestriction: true, num >= 1);
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
	private struct SetupRandomTrafficJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<RandomTrafficRequest> m_RandomTrafficRequestData;

		[ReadOnly]
		public ComponentLookup<TrafficSpawnerData> m_PrefabTrafficSpawnerData;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var owner, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				RandomTrafficRequest randomTrafficRequest = default(RandomTrafficRequest);
				if (m_RandomTrafficRequestData.HasComponent(owner))
				{
					randomTrafficRequest = m_RandomTrafficRequestData[owner];
				}
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					if (!(val == entity))
					{
						PrefabRef prefabRef = nativeArray2[j];
						TrafficSpawnerData trafficSpawnerData = default(TrafficSpawnerData);
						if (m_PrefabTrafficSpawnerData.HasComponent(prefabRef.m_Prefab))
						{
							trafficSpawnerData = m_PrefabTrafficSpawnerData[prefabRef.m_Prefab];
						}
						if ((randomTrafficRequest.m_RoadType & trafficSpawnerData.m_RoadType) != RoadTypes.None || (randomTrafficRequest.m_TrackType & trafficSpawnerData.m_TrackType) != TrackTypes.None)
						{
							float cost = ((Random)(ref random)).NextFloat(10000f);
							targetSeeker.FindTargets(val, cost);
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
	private struct SetupOutsideConnectionsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				float value = targetSeeker.m_SetupQueueTarget.m_Value2;
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity val = nativeArray[j];
					float cost = 0f;
					if (value > 0f)
					{
						cost = ((Random)(ref random)).NextFloat(value);
					}
					targetSeeker.FindTargets(val, cost);
					if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) == 0 || !targetSeeker.m_Transform.HasComponent(val))
					{
						continue;
					}
					float3 position = targetSeeker.m_Transform[val].m_Position;
					if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						Entity lane = Entity.Null;
						float curvePos = 0f;
						float distance = float.MaxValue;
						targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
						if (lane != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(val, lane, curvePos, cost));
						}
					}
					if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Airplane) != RoadTypes.None)
					{
						Entity lane2 = Entity.Null;
						float curvePos2 = 0f;
						float distance2 = float.MaxValue;
						targetSeeker.m_AirwayData.airplaneMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane2, ref curvePos2, ref distance2);
						if (lane2 != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(val, lane2, curvePos2, cost));
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
	private struct MaintenanceRequestsJob : IJobChunk
	{
		private struct DistrictIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public float2 m_Position;

			public ComponentLookup<District> m_DistrictData;

			public BufferLookup<Game.Areas.Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public Entity m_Result;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position) && m_DistrictData.HasComponent(areaItem.m_Area))
				{
					DynamicBuffer<Game.Areas.Node> nodes = m_Nodes[areaItem.m_Area];
					DynamicBuffer<Triangle> val = m_Triangles[areaItem.m_Area];
					float2 val2 = default(float2);
					if (val.Length > areaItem.m_Triangle && MathUtils.Intersect(AreaUtils.GetTriangle2(nodes, val[areaItem.m_Triangle]), m_Position, ref val2))
					{
						m_Result = areaItem.m_Area;
					}
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<MaintenanceRequest> m_MaintenanceRequestType;

		[ReadOnly]
		public ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Surface> m_SurfaceData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Park> m_ParkData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<NetCondition> m_NetConditionData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetTree;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<MaintenanceRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MaintenanceRequest>(ref m_MaintenanceRequestType);
			MaintenanceRequest maintenanceRequest = default(MaintenanceRequest);
			Owner owner2 = default(Owner);
			CurrentDistrict currentDistrict = default(CurrentDistrict);
			BorderDistrict borderDistrict = default(BorderDistrict);
			Transform transform = default(Transform);
			Transform transform2 = default(Transform);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_MaintenanceRequestData.TryGetComponent(owner, ref maintenanceRequest))
				{
					continue;
				}
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				Entity service = Entity.Null;
				if (m_VehicleData.HasComponent(maintenanceRequest.m_Target))
				{
					if (targetSeeker.m_Owner.TryGetComponent(maintenanceRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!targetSeeker.m_PrefabRef.HasComponent(maintenanceRequest.m_Target))
					{
						continue;
					}
					service = maintenanceRequest.m_Target;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					MaintenanceRequest maintenanceRequest2 = nativeArray3[j];
					MaintenanceType maintenanceType = BuildingUtils.GetMaintenanceType(maintenanceRequest2.m_Target, ref m_ParkData, ref m_NetConditionData, ref m_EdgeData, ref m_SurfaceData, ref m_VehicleData);
					if ((maintenanceType & targetSeeker.m_SetupQueueTarget.m_MaintenanceType) != maintenanceType)
					{
						continue;
					}
					float cost = 0f;
					if ((maintenanceType & MaintenanceType.Vehicle) == 0)
					{
						cost = ((Random)(ref random)).NextFloat(30f);
					}
					Entity district = Entity.Null;
					if (m_CurrentDistrictData.TryGetComponent(maintenanceRequest2.m_Target, ref currentDistrict))
					{
						district = currentDistrict.m_District;
					}
					else if (m_BorderDistrictData.TryGetComponent(maintenanceRequest2.m_Target, ref borderDistrict))
					{
						district = borderDistrict.m_Right;
					}
					else if (targetSeeker.m_Transform.TryGetComponent(maintenanceRequest2.m_Target, ref transform))
					{
						DistrictIterator districtIterator = new DistrictIterator
						{
							m_Position = ((float3)(ref transform.m_Position)).xz,
							m_DistrictData = m_DistrictData,
							m_Nodes = targetSeeker.m_AreaNode,
							m_Triangles = targetSeeker.m_AreaTriangle
						};
						m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
						district = districtIterator.m_Result;
					}
					if (AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
					{
						targetSeeker.FindTargets(nativeArray[j], maintenanceRequest2.m_Target, cost, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
						if (targetSeeker.m_Transform.TryGetComponent(maintenanceRequest2.m_Target, ref transform2))
						{
							float num = 30f;
							CommonPathfindSetup.TargetIterator targetIterator = new CommonPathfindSetup.TargetIterator
							{
								m_Entity = nativeArray[j],
								m_Bounds = new Bounds3(transform2.m_Position - num, transform2.m_Position + num),
								m_Position = transform2.m_Position,
								m_MaxDistance = num,
								m_TargetSeeker = targetSeeker,
								m_Flags = EdgeFlags.DefaultMask,
								m_CompositionData = m_CompositionData,
								m_NetCompositionData = m_NetCompositionData
							};
							m_NetTree.Iterate<CommonPathfindSetup.TargetIterator>(ref targetIterator, 0);
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

	private EntityQuery m_MaintenanceProviderQuery;

	private EntityQuery m_RandomTrafficQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private EntityQuery m_MaintenanceRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<MaintenanceRequest> m_MaintenanceRequestType;

	private ComponentTypeHandle<Game.Buildings.MaintenanceDepot> m_MaintenanceDepotType;

	private ComponentTypeHandle<Game.Vehicles.MaintenanceVehicle> m_MaintenanceVehicleType;

	private ComponentTypeHandle<PrefabRef> m_PrefabRefType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private ComponentLookup<PathInformation> m_PathInformationData;

	private ComponentLookup<RandomTrafficRequest> m_RandomTrafficRequestData;

	private ComponentLookup<MaintenanceRequest> m_MaintenanceRequestData;

	private ComponentLookup<Game.Objects.Surface> m_SurfaceData;

	private ComponentLookup<Game.Buildings.Park> m_ParkData;

	private ComponentLookup<Game.Net.Edge> m_EdgeData;

	private ComponentLookup<NetCondition> m_NetConditionData;

	private ComponentLookup<Composition> m_CompositionData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<BorderDistrict> m_BorderDistrictData;

	private ComponentLookup<District> m_DistrictData;

	private ComponentLookup<Vehicle> m_VehicleData;

	private ComponentLookup<MaintenanceDepotData> m_PrefabMaintenanceDepotData;

	private ComponentLookup<MaintenanceVehicleData> m_PrefabMaintenanceVehicleData;

	private ComponentLookup<TrafficSpawnerData> m_PrefabTrafficSpawnerData;

	private ComponentLookup<NetCompositionData> m_NetCompositionData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	public RoadPathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.MaintenanceDepot>(),
			ComponentType.ReadOnly<Game.Vehicles.MaintenanceVehicle>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_MaintenanceProviderQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_RandomTrafficQuery = system.GetSetupQuery(ComponentType.ReadOnly<Game.Buildings.TrafficSpawner>(), ComponentType.ReadOnly<PrefabRef>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Objects.OutsideConnection>() };
		val.None = (ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.WaterPipeOutsideConnection>()
		};
		array2[0] = val;
		m_OutsideConnectionQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_MaintenanceRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<MaintenanceRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_OutsideConnectionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_MaintenanceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<MaintenanceRequest>(true);
		m_MaintenanceDepotType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.MaintenanceDepot>(true);
		m_MaintenanceVehicleType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.MaintenanceVehicle>(true);
		m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_PathInformationData = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_RandomTrafficRequestData = ((SystemBase)system).GetComponentLookup<RandomTrafficRequest>(true);
		m_MaintenanceRequestData = ((SystemBase)system).GetComponentLookup<MaintenanceRequest>(true);
		m_SurfaceData = ((SystemBase)system).GetComponentLookup<Game.Objects.Surface>(true);
		m_ParkData = ((SystemBase)system).GetComponentLookup<Game.Buildings.Park>(true);
		m_EdgeData = ((SystemBase)system).GetComponentLookup<Game.Net.Edge>(true);
		m_NetConditionData = ((SystemBase)system).GetComponentLookup<NetCondition>(true);
		m_CompositionData = ((SystemBase)system).GetComponentLookup<Composition>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_BorderDistrictData = ((SystemBase)system).GetComponentLookup<BorderDistrict>(true);
		m_DistrictData = ((SystemBase)system).GetComponentLookup<District>(true);
		m_VehicleData = ((SystemBase)system).GetComponentLookup<Vehicle>(true);
		m_PrefabMaintenanceDepotData = ((SystemBase)system).GetComponentLookup<MaintenanceDepotData>(true);
		m_PrefabMaintenanceVehicleData = ((SystemBase)system).GetComponentLookup<MaintenanceVehicleData>(true);
		m_PrefabTrafficSpawnerData = ((SystemBase)system).GetComponentLookup<TrafficSpawnerData>(true);
		m_NetCompositionData = ((SystemBase)system).GetComponentLookup<NetCompositionData>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_AreaSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
	}

	public JobHandle SetupMaintenanceProviders(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_MaintenanceDepotType.Update((SystemBase)(object)system);
		m_MaintenanceVehicleType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_PrefabMaintenanceDepotData.Update((SystemBase)(object)system);
		m_PrefabMaintenanceVehicleData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupMaintenanceProvidersJob>(new SetupMaintenanceProvidersJob
		{
			m_EntityType = m_EntityType,
			m_PrefabRefType = m_PrefabRefType,
			m_MaintenanceDepotType = m_MaintenanceDepotType,
			m_MaintenanceVehicleType = m_MaintenanceVehicleType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_PrefabMaintenanceDepotData = m_PrefabMaintenanceDepotData,
			m_PrefabMaintenanceVehicleData = m_PrefabMaintenanceVehicleData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_MaintenanceProviderQuery, inputDeps);
	}

	public JobHandle SetupRandomTraffic(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_RandomTrafficRequestData.Update((SystemBase)(object)system);
		m_PrefabTrafficSpawnerData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupRandomTrafficJob>(new SetupRandomTrafficJob
		{
			m_EntityType = m_EntityType,
			m_PrefabRefType = m_PrefabRefType,
			m_RandomTrafficRequestData = m_RandomTrafficRequestData,
			m_PrefabTrafficSpawnerData = m_PrefabTrafficSpawnerData,
			m_SetupData = setupData
		}, m_RandomTrafficQuery, inputDeps);
	}

	public JobHandle SetupOutsideConnections(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_OutsideConnectionType.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupOutsideConnectionsJob>(new SetupOutsideConnectionsJob
		{
			m_EntityType = m_EntityType,
			m_OutsideConnectionType = m_OutsideConnectionType,
			m_SetupData = setupData
		}, m_OutsideConnectionQuery, inputDeps);
	}

	public JobHandle SetupMaintenanceRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_MaintenanceRequestType.Update((SystemBase)(object)system);
		m_MaintenanceRequestData.Update((SystemBase)(object)system);
		m_SurfaceData.Update((SystemBase)(object)system);
		m_ParkData.Update((SystemBase)(object)system);
		m_EdgeData.Update((SystemBase)(object)system);
		m_NetConditionData.Update((SystemBase)(object)system);
		m_CompositionData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_BorderDistrictData.Update((SystemBase)(object)system);
		m_DistrictData.Update((SystemBase)(object)system);
		m_VehicleData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = JobChunkExtensions.ScheduleParallel<MaintenanceRequestsJob>(new MaintenanceRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_MaintenanceRequestType = m_MaintenanceRequestType,
			m_MaintenanceRequestData = m_MaintenanceRequestData,
			m_SurfaceData = m_SurfaceData,
			m_ParkData = m_ParkData,
			m_EdgeData = m_EdgeData,
			m_NetConditionData = m_NetConditionData,
			m_CompositionData = m_CompositionData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_BorderDistrictData = m_BorderDistrictData,
			m_DistrictData = m_DistrictData,
			m_VehicleData = m_VehicleData,
			m_NetCompositionData = m_NetCompositionData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_NetTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_SetupData = setupData
		}, m_MaintenanceRequestQuery, JobHandle.CombineDependencies(inputDeps, dependencies, dependencies2));
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}
}
