using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Notifications;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
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
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TransportLineSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	private struct SortedVehicle : IComparable<SortedVehicle>
	{
		public Entity m_Vehicle;

		public float m_Distance;

		public int CompareTo(SortedVehicle other)
		{
			return math.select(0, math.select(1, -1, m_Distance < other.m_Distance), m_Distance != other.m_Distance);
		}
	}

	private struct VehicleAction
	{
		public VehicleActionType m_Type;

		public Entity m_Vehicle;
	}

	private enum VehicleActionType
	{
		AbandonRoute,
		CancelAbandon
	}

	[BurstCompile]
	private struct TransportLineTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Route> m_RouteType;

		[ReadOnly]
		public ComponentTypeHandle<VehicleModel> m_VehicleModelType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> m_RouteWaypointType;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> m_RouteSegmentType;

		[ReadOnly]
		public BufferTypeHandle<RouteModifier> m_RouteModifierType;

		public ComponentTypeHandle<TransportLine> m_TransportLineType;

		public BufferTypeHandle<RouteVehicle> m_RouteVehicleType;

		public BufferTypeHandle<DispatchedRequest> m_DispatchedRequestType;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> m_ServiceRequestData;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> m_VehicleTimingData;

		[ReadOnly]
		public ComponentLookup<Connected> m_ConnectedData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> m_CurrentRouteData;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_DispatchedData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Odometer> m_OdometerData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineDataData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<RouteInfo> m_RouteInfoData;

		[ReadOnly]
		public EntityArchetype m_VehicleRequestArchetype;

		[ReadOnly]
		public bool m_IsNight;

		[NativeDisableParallelForRestriction]
		public NativeArray<float> m_MaxTransportSpeed;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<VehicleAction> m_ActionQueue;

		public IconCommandBuffer m_IconCommandBuffer;

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
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Route> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Route>(ref m_RouteType);
			NativeArray<VehicleModel> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<VehicleModel>(ref m_VehicleModelType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<TransportLine> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportLine>(ref m_TransportLineType);
			BufferAccessor<RouteWaypoint> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteWaypoint>(ref m_RouteWaypointType);
			BufferAccessor<RouteSegment> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteSegment>(ref m_RouteSegmentType);
			BufferAccessor<RouteModifier> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteModifier>(ref m_RouteModifierType);
			BufferAccessor<RouteVehicle> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteVehicle>(ref m_RouteVehicleType);
			BufferAccessor<DispatchedRequest> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<DispatchedRequest>(ref m_DispatchedRequestType);
			NativeList<SortedVehicle> sortBuffer = default(NativeList<SortedVehicle>);
			bool3 val2 = default(bool3);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Route route = nativeArray2[i];
				VehicleModel vehicleModel = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				TransportLine transportLine = nativeArray5[i];
				DynamicBuffer<RouteWaypoint> waypoints = bufferAccessor[i];
				DynamicBuffer<RouteSegment> segments = bufferAccessor2[i];
				DynamicBuffer<RouteModifier> modifiers = bufferAccessor3[i];
				DynamicBuffer<RouteVehicle> vehicles = bufferAccessor4[i];
				DynamicBuffer<DispatchedRequest> requests = bufferAccessor5[i];
				TransportLineData prefabLineData = m_TransportLineDataData[prefabRef.m_Prefab];
				float value = prefabLineData.m_DefaultVehicleInterval;
				RouteUtils.ApplyModifier(ref value, modifiers, RouteModifierType.VehicleInterval);
				ushort num = 0;
				if (RouteUtils.CheckOption(route, RouteOption.PaidTicket))
				{
					float value2 = 0f;
					RouteUtils.ApplyModifier(ref value2, modifiers, RouteModifierType.TicketPrice);
					num = (ushort)math.clamp(Mathf.RoundToInt(value2), 0, 65535);
				}
				if (RouteUtils.CheckOption(route, RouteOption.Inactive))
				{
					val2 = bool3.op_Implicit(false);
				}
				else if (RouteUtils.CheckOption(route, RouteOption.Day))
				{
					((bool3)(ref val2))._002Ector(!m_IsNight, true, false);
				}
				else if (RouteUtils.CheckOption(route, RouteOption.Night))
				{
					((bool3)(ref val2))._002Ector(m_IsNight, false, true);
				}
				else
				{
					val2 = bool3.op_Implicit(true);
				}
				RefreshLineSegments(unfilteredChunkIndex, prefabLineData, waypoints, segments, val2, out var lineDuration, out var stableDuration);
				int num2 = CalculateVehicleCount(value, stableDuration);
				float num3 = math.min(value * 10f, CalculateVehicleInterval(lineDuration, num2));
				bool flag = false;
				if (math.abs(num3 - transportLine.m_VehicleInterval) >= 1f)
				{
					transportLine.m_VehicleInterval = num3;
					flag = true;
				}
				if (num != transportLine.m_TicketPrice)
				{
					transportLine.m_TicketPrice = num;
					flag = true;
				}
				if (flag)
				{
					UpdateStopPathfind(unfilteredChunkIndex, waypoints);
				}
				CheckVehicles(val, vehicleModel, vehicles, out var totalCount, out var continuingCount);
				num2 = math.select(0, num2, val2.x);
				if (continuingCount < num2 && continuingCount < totalCount)
				{
					int count = num2 - continuingCount;
					CancelAbandon(vehicleModel, vehicles, count, ref sortBuffer);
				}
				else if (continuingCount > num2)
				{
					int count2 = continuingCount - num2;
					AbandonVehicles(vehicleModel, vehicles, count2, ref sortBuffer);
				}
				CheckRequests(ref transportLine, requests);
				bool flag2 = false;
				if (totalCount < num2)
				{
					transportLine.m_Flags |= TransportLineFlags.RequireVehicles;
					totalCount += requests.Length;
					if (totalCount < num2)
					{
						flag2 = !RequestNewVehicleIfNeeded(unfilteredChunkIndex, val, transportLine, totalCount, num2);
					}
				}
				else
				{
					transportLine.m_Flags &= ~TransportLineFlags.RequireVehicles;
				}
				if (flag2)
				{
					if ((transportLine.m_Flags & TransportLineFlags.NotEnoughVehicles) == 0 && waypoints.Length != 0)
					{
						transportLine.m_Flags |= TransportLineFlags.NotEnoughVehicles;
						m_IconCommandBuffer.Add(val, prefabLineData.m_VehicleNotification, IconPriority.Problem);
					}
				}
				else if ((transportLine.m_Flags & TransportLineFlags.NotEnoughVehicles) != 0)
				{
					transportLine.m_Flags &= ~TransportLineFlags.NotEnoughVehicles;
					m_IconCommandBuffer.Remove(val, prefabLineData.m_VehicleNotification);
				}
				nativeArray5[i] = transportLine;
			}
			if (sortBuffer.IsCreated)
			{
				sortBuffer.Dispose();
			}
		}

		private void UpdateStopPathfind(int jobIndex, DynamicBuffer<RouteWaypoint> waypoints)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < waypoints.Length; i++)
			{
				Entity waypoint = waypoints[i].m_Waypoint;
				if (m_VehicleTimingData.HasComponent(waypoint))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, waypoint, default(PathfindUpdated));
				}
			}
		}

		private void CheckVehicles(Entity route, VehicleModel vehicleModel, DynamicBuffer<RouteVehicle> vehicles, out int totalCount, out int continuingCount)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			totalCount = 0;
			continuingCount = 0;
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			while (totalCount < vehicles.Length)
			{
				Entity vehicle = vehicles[totalCount].m_Vehicle;
				CurrentRoute currentRoute = default(CurrentRoute);
				if (m_CurrentRouteData.HasComponent(vehicle))
				{
					currentRoute = m_CurrentRouteData[vehicle];
				}
				if (currentRoute.m_Route == route)
				{
					totalCount++;
					Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
					if (m_CargoTransportData.HasComponent(vehicle))
					{
						cargoTransport = m_CargoTransportData[vehicle];
					}
					Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
					if (m_PublicTransportData.HasComponent(vehicle))
					{
						publicTransport = m_PublicTransportData[vehicle];
					}
					if ((cargoTransport.m_State & CargoTransportFlags.AbandonRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.AbandonRoute) == 0)
					{
						PrefabRef prefabRef = m_PrefabRefData[vehicle];
						m_LayoutElements.TryGetBuffer(vehicle, ref layout);
						if (RouteUtils.CheckVehicleModel(vehicleModel, prefabRef, layout, ref m_PrefabRefData))
						{
							continuingCount++;
							continue;
						}
						m_ActionQueue.Enqueue(new VehicleAction
						{
							m_Type = VehicleActionType.AbandonRoute,
							m_Vehicle = vehicle
						});
					}
				}
				else
				{
					vehicles.RemoveAt(totalCount);
				}
			}
		}

		private void AbandonVehicles(VehicleModel vehicleModel, DynamicBuffer<RouteVehicle> vehicles, int count, ref NativeList<SortedVehicle> sortBuffer)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			if (!sortBuffer.IsCreated)
			{
				sortBuffer = new NativeList<SortedVehicle>(vehicles.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < vehicles.Length; i++)
			{
				Entity vehicle = vehicles[i].m_Vehicle;
				Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
				if (m_CargoTransportData.HasComponent(vehicle))
				{
					cargoTransport = m_CargoTransportData[vehicle];
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (m_PublicTransportData.HasComponent(vehicle))
				{
					publicTransport = m_PublicTransportData[vehicle];
				}
				if ((cargoTransport.m_State & CargoTransportFlags.AbandonRoute) == 0 && (publicTransport.m_State & PublicTransportFlags.AbandonRoute) == 0)
				{
					PrefabRef prefabRef = m_PrefabRefData[vehicle];
					m_LayoutElements.TryGetBuffer(vehicle, ref layout);
					if (RouteUtils.CheckVehicleModel(vehicleModel, prefabRef, layout, ref m_PrefabRefData))
					{
						SortedVehicle sortedVehicle = new SortedVehicle
						{
							m_Vehicle = vehicle,
							m_Distance = m_OdometerData[vehicle].m_Distance
						};
						sortBuffer.Add(ref sortedVehicle);
					}
				}
			}
			NativeSortExtension.Sort<SortedVehicle>(sortBuffer);
			count = math.min(count, sortBuffer.Length);
			for (int j = 0; j < count; j++)
			{
				Entity vehicle2 = sortBuffer[sortBuffer.Length - j - 1].m_Vehicle;
				m_ActionQueue.Enqueue(new VehicleAction
				{
					m_Type = VehicleActionType.AbandonRoute,
					m_Vehicle = vehicle2
				});
			}
			sortBuffer.Clear();
		}

		private void CancelAbandon(VehicleModel vehicleModel, DynamicBuffer<RouteVehicle> vehicles, int count, ref NativeList<SortedVehicle> sortBuffer)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			if (!sortBuffer.IsCreated)
			{
				sortBuffer = new NativeList<SortedVehicle>(vehicles.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < vehicles.Length; i++)
			{
				Entity vehicle = vehicles[i].m_Vehicle;
				Game.Vehicles.CargoTransport cargoTransport = default(Game.Vehicles.CargoTransport);
				if (m_CargoTransportData.HasComponent(vehicle))
				{
					cargoTransport = m_CargoTransportData[vehicle];
				}
				Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
				if (m_PublicTransportData.HasComponent(vehicle))
				{
					publicTransport = m_PublicTransportData[vehicle];
				}
				if ((cargoTransport.m_State & (CargoTransportFlags.AbandonRoute | CargoTransportFlags.Disabled)) == CargoTransportFlags.AbandonRoute || (publicTransport.m_State & (PublicTransportFlags.AbandonRoute | PublicTransportFlags.Disabled)) == PublicTransportFlags.AbandonRoute)
				{
					PrefabRef prefabRef = m_PrefabRefData[vehicle];
					m_LayoutElements.TryGetBuffer(vehicle, ref layout);
					if (RouteUtils.CheckVehicleModel(vehicleModel, prefabRef, layout, ref m_PrefabRefData))
					{
						SortedVehicle sortedVehicle = new SortedVehicle
						{
							m_Vehicle = vehicle,
							m_Distance = m_OdometerData[vehicle].m_Distance
						};
						sortBuffer.Add(ref sortedVehicle);
					}
				}
			}
			NativeSortExtension.Sort<SortedVehicle>(sortBuffer);
			count = math.min(count, sortBuffer.Length);
			for (int j = 0; j < count; j++)
			{
				Entity vehicle2 = sortBuffer[j].m_Vehicle;
				m_ActionQueue.Enqueue(new VehicleAction
				{
					m_Type = VehicleActionType.CancelAbandon,
					m_Vehicle = vehicle2
				});
			}
			sortBuffer.Clear();
		}

		private unsafe void RefreshLineSegments(int jobIndex, TransportLineData prefabLineData, DynamicBuffer<RouteWaypoint> waypoints, DynamicBuffer<RouteSegment> segments, bool3 isActive, out float lineDuration, out float stableDuration)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			lineDuration = 0f;
			stableDuration = 0f;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < waypoints.Length; i++)
			{
				if (m_VehicleTimingData.HasComponent(waypoints[i].m_Waypoint))
				{
					num6 = i;
					break;
				}
			}
			RouteInfoFlags routeInfoFlags = (RouteInfoFlags)0;
			if (!isActive.y)
			{
				routeInfoFlags |= RouteInfoFlags.InactiveDay;
			}
			if (!isActive.z)
			{
				routeInfoFlags |= RouteInfoFlags.InactiveNight;
			}
			int num7 = num6;
			for (int j = 0; j < waypoints.Length; j++)
			{
				int2 val = int2.op_Implicit(num6 + j);
				val.y++;
				val = math.select(val, val - waypoints.Length, val >= waypoints.Length);
				num5++;
				Entity waypoint = waypoints[val.y].m_Waypoint;
				Entity segment = segments[val.x].m_Segment;
				if (m_PathInformationData.HasComponent(segment))
				{
					PathInformation pathInformation = m_PathInformationData[segment];
					num += pathInformation.m_Duration;
					num2 += pathInformation.m_Distance;
					num3 += pathInformation.m_Duration;
					stableDuration += pathInformation.m_Duration;
				}
				if (!m_VehicleTimingData.HasComponent(waypoint))
				{
					continue;
				}
				VehicleTiming vehicleTiming = m_VehicleTimingData[waypoint];
				float stopDuration = prefabLineData.m_StopDuration;
				if (m_ConnectedData.HasComponent(waypoint))
				{
					Connected connected = m_ConnectedData[waypoint];
					if (m_TransportStopData.HasComponent(connected.m_Connected))
					{
						stopDuration = RouteUtils.GetStopDuration(prefabLineData, m_TransportStopData[connected.m_Connected]);
					}
				}
				num = math.max(num, vehicleTiming.m_AverageTravelTime) + stopDuration;
				lineDuration += num;
				stableDuration += prefabLineData.m_StopDuration;
				for (int k = 0; k < num5; k++)
				{
					int num8 = num7 + k;
					num8 = math.select(num8, num8 - waypoints.Length, num8 >= waypoints.Length);
					Entity segment2 = segments[num8].m_Segment;
					if (m_PathInformationData.HasComponent(segment2))
					{
						PathInformation pathInformation2 = m_PathInformationData[segment2];
						RouteInfo routeInfo = m_RouteInfoData[segment2];
						RouteInfo routeInfo2 = routeInfo;
						routeInfo2.m_Duration = pathInformation2.m_Duration * math.max(1f, num / math.max(1f, num3));
						routeInfo2.m_Distance = pathInformation2.m_Distance;
						routeInfo2.m_Flags = routeInfoFlags;
						if (routeInfo2.m_Distance != routeInfo.m_Distance || math.abs(routeInfo2.m_Duration - routeInfo.m_Duration) >= 1f || routeInfo2.m_Flags != routeInfo.m_Flags)
						{
							m_RouteInfoData[segment2] = routeInfo2;
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, segment2, default(PathfindUpdated));
						}
					}
				}
				num4 = math.max(num4, math.max(1f, num2) / math.max(1f, num));
				num = 0f;
				num2 = 0f;
				num3 = 0f;
				num5 = 0;
				num7 = val.y;
			}
			if (prefabLineData.m_PassengerTransport)
			{
				float* unsafePtr = (float*)NativeArrayUnsafeUtility.GetUnsafePtr<float>(m_MaxTransportSpeed);
				if (num4 > *unsafePtr)
				{
					float num9;
					do
					{
						num9 = num4;
						num4 = Interlocked.Exchange(ref *unsafePtr, num9);
					}
					while (num4 > num9);
				}
			}
			if (!prefabLineData.m_CargoTransport)
			{
				return;
			}
			float* unsafePtr2 = (float*)NativeArrayUnsafeUtility.GetUnsafePtr<float>(m_MaxTransportSpeed);
			unsafePtr2++;
			if (num4 > *unsafePtr2)
			{
				float num10;
				do
				{
					num10 = num4;
					num4 = Interlocked.Exchange(ref *unsafePtr2, num10);
				}
				while (num4 > num10);
			}
		}

		private void CheckRequests(ref TransportLine transportLine, DynamicBuffer<DispatchedRequest> requests)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < requests.Length; i++)
			{
				Entity vehicleRequest = requests[i].m_VehicleRequest;
				if (!m_ServiceRequestData.HasComponent(vehicleRequest))
				{
					requests.RemoveAtSwapBack(i--);
				}
			}
			if (m_DispatchedData.HasComponent(transportLine.m_VehicleRequest))
			{
				requests.Add(new DispatchedRequest
				{
					m_VehicleRequest = transportLine.m_VehicleRequest
				});
				transportLine.m_VehicleRequest = Entity.Null;
			}
		}

		private bool RequestNewVehicleIfNeeded(int jobIndex, Entity entity, TransportLine transportLine, int vehicleCount, int targetCount)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			ServiceRequest serviceRequest = default(ServiceRequest);
			if (m_ServiceRequestData.TryGetComponent(transportLine.m_VehicleRequest, ref serviceRequest))
			{
				return serviceRequest.m_FailCount < 2;
			}
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_VehicleRequestArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TransportVehicleRequest>(jobIndex, val, new TransportVehicleRequest(entity, 1f - (float)vehicleCount / (float)targetCount));
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val, new RequestGroup(8u));
			return true;
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct VehicleActionJob : IJob
	{
		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		public NativeQueue<VehicleAction> m_ActionQueue;

		public void Execute()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			int count = m_ActionQueue.Count;
			for (int i = 0; i < count; i++)
			{
				VehicleAction vehicleAction = m_ActionQueue.Dequeue();
				switch (vehicleAction.m_Type)
				{
				case VehicleActionType.AbandonRoute:
					if (m_CargoTransportData.HasComponent(vehicleAction.m_Vehicle))
					{
						Game.Vehicles.CargoTransport cargoTransport2 = m_CargoTransportData[vehicleAction.m_Vehicle];
						cargoTransport2.m_State |= CargoTransportFlags.AbandonRoute;
						m_CargoTransportData[vehicleAction.m_Vehicle] = cargoTransport2;
					}
					if (m_PublicTransportData.HasComponent(vehicleAction.m_Vehicle))
					{
						Game.Vehicles.PublicTransport publicTransport2 = m_PublicTransportData[vehicleAction.m_Vehicle];
						publicTransport2.m_State |= PublicTransportFlags.AbandonRoute;
						m_PublicTransportData[vehicleAction.m_Vehicle] = publicTransport2;
					}
					break;
				case VehicleActionType.CancelAbandon:
					if (m_CargoTransportData.HasComponent(vehicleAction.m_Vehicle))
					{
						Game.Vehicles.CargoTransport cargoTransport = m_CargoTransportData[vehicleAction.m_Vehicle];
						cargoTransport.m_State &= ~CargoTransportFlags.AbandonRoute;
						m_CargoTransportData[vehicleAction.m_Vehicle] = cargoTransport;
					}
					if (m_PublicTransportData.HasComponent(vehicleAction.m_Vehicle))
					{
						Game.Vehicles.PublicTransport publicTransport = m_PublicTransportData[vehicleAction.m_Vehicle];
						publicTransport.m_State &= ~PublicTransportFlags.AbandonRoute;
						m_PublicTransportData[vehicleAction.m_Vehicle] = publicTransport;
					}
					break;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Route> __Game_Routes_Route_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<VehicleModel> __Game_Routes_VehicleModel_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteSegment> __Game_Routes_RouteSegment_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<RouteModifier> __Game_Routes_RouteModifier_RO_BufferTypeHandle;

		public ComponentTypeHandle<TransportLine> __Game_Routes_TransportLine_RW_ComponentTypeHandle;

		public BufferTypeHandle<RouteVehicle> __Game_Routes_RouteVehicle_RW_BufferTypeHandle;

		public BufferTypeHandle<DispatchedRequest> __Game_Routes_DispatchedRequest_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<ServiceRequest> __Game_Simulation_ServiceRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> __Game_Routes_VehicleTiming_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Connected> __Game_Routes_Connected_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentRoute> __Game_Routes_CurrentRoute_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Odometer> __Game_Vehicles_Odometer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		public ComponentLookup<RouteInfo> __Game_Routes_RouteInfo_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.CargoTransport> __Game_Vehicles_CargoTransport_RW_ComponentLookup;

		public ComponentLookup<Game.Vehicles.PublicTransport> __Game_Vehicles_PublicTransport_RW_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Routes_Route_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Route>(true);
			__Game_Routes_VehicleModel_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<VehicleModel>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Routes_RouteWaypoint_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteSegment>(true);
			__Game_Routes_RouteModifier_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteModifier>(true);
			__Game_Routes_TransportLine_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportLine>(false);
			__Game_Routes_RouteVehicle_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteVehicle>(false);
			__Game_Routes_DispatchedRequest_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<DispatchedRequest>(false);
			__Game_Simulation_ServiceRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceRequest>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Routes_VehicleTiming_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleTiming>(true);
			__Game_Routes_Connected_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Connected>(true);
			__Game_Routes_TransportStop_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TransportStop>(true);
			__Game_Routes_CurrentRoute_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentRoute>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Vehicles_CargoTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.CargoTransport>(true);
			__Game_Vehicles_PublicTransport_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
			__Game_Vehicles_Odometer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Odometer>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Game_Routes_RouteInfo_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RouteInfo>(false);
			__Game_Vehicles_CargoTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.CargoTransport>(false);
			__Game_Vehicles_PublicTransport_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PublicTransport>(false);
		}
	}

	public const uint UPDATE_INTERVAL = 256u;

	private EntityQuery m_LineQuery;

	private EntityArchetype m_VehicleRequestArchetype;

	private NativeArray<float> m_MaxTransportSpeed;

	private JobHandle m_MaxTransportSpeedDeps;

	private TimeSystem m_TimeSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private IconCommandSystem m_IconCommandSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 256;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_LineQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Route>(),
			ComponentType.ReadWrite<TransportLine>(),
			ComponentType.ReadOnly<RouteWaypoint>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_VehicleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<TransportVehicleRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		m_MaxTransportSpeed = new NativeArray<float>(2, (Allocator)4, (NativeArrayOptions)1);
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_MaxTransportSpeed.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		m_MaxTransportSpeed[0] = 0f;
		m_MaxTransportSpeed[1] = 0f;
		if (!((EntityQuery)(ref m_LineQuery)).IsEmptyIgnoreFilter)
		{
			NativeQueue<VehicleAction> actionQueue = default(NativeQueue<VehicleAction>);
			actionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			float normalizedTime = m_TimeSystem.normalizedTime;
			bool isNight = normalizedTime < 0.25f || normalizedTime >= 11f / 12f;
			TransportLineTickJob transportLineTickJob = new TransportLineTickJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteType = InternalCompilerInterface.GetComponentTypeHandle<Route>(ref __TypeHandle.__Game_Routes_Route_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleModelType = InternalCompilerInterface.GetComponentTypeHandle<VehicleModel>(ref __TypeHandle.__Game_Routes_VehicleModel_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypointType = InternalCompilerInterface.GetBufferTypeHandle<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteSegmentType = InternalCompilerInterface.GetBufferTypeHandle<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteModifierType = InternalCompilerInterface.GetBufferTypeHandle<RouteModifier>(ref __TypeHandle.__Game_Routes_RouteModifier_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransportLineType = InternalCompilerInterface.GetComponentTypeHandle<TransportLine>(ref __TypeHandle.__Game_Routes_TransportLine_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_RouteVehicleType = InternalCompilerInterface.GetBufferTypeHandle<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DispatchedRequestType = InternalCompilerInterface.GetBufferTypeHandle<DispatchedRequest>(ref __TypeHandle.__Game_Routes_DispatchedRequest_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceRequestData = InternalCompilerInterface.GetComponentLookup<ServiceRequest>(ref __TypeHandle.__Game_Simulation_ServiceRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformationData = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleTimingData = InternalCompilerInterface.GetComponentLookup<VehicleTiming>(ref __TypeHandle.__Game_Routes_VehicleTiming_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedData = InternalCompilerInterface.GetComponentLookup<Connected>(ref __TypeHandle.__Game_Routes_Connected_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportStopData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurrentRouteData = InternalCompilerInterface.GetComponentLookup<CurrentRoute>(ref __TypeHandle.__Game_Routes_CurrentRoute_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CargoTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OdometerData = InternalCompilerInterface.GetComponentLookup<Odometer>(ref __TypeHandle.__Game_Vehicles_Odometer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransportLineDataData = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteInfoData = InternalCompilerInterface.GetComponentLookup<RouteInfo>(ref __TypeHandle.__Game_Routes_RouteInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleRequestArchetype = m_VehicleRequestArchetype,
				m_IsNight = isNight,
				m_MaxTransportSpeed = m_MaxTransportSpeed
			};
			EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
			transportLineTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
			transportLineTickJob.m_ActionQueue = actionQueue.AsParallelWriter();
			transportLineTickJob.m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer();
			TransportLineTickJob transportLineTickJob2 = transportLineTickJob;
			VehicleActionJob obj = new VehicleActionJob
			{
				m_CargoTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.CargoTransport>(ref __TypeHandle.__Game_Vehicles_CargoTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PublicTransportData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PublicTransport>(ref __TypeHandle.__Game_Vehicles_PublicTransport_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ActionQueue = actionQueue
			};
			JobHandle val2 = JobChunkExtensions.ScheduleParallel<TransportLineTickJob>(transportLineTickJob2, m_LineQuery, ((SystemBase)this).Dependency);
			JobHandle val3 = IJobExtensions.Schedule<VehicleActionJob>(obj, val2);
			actionQueue.Dispose(val3);
			m_MaxTransportSpeedDeps = val2;
			m_EndFrameBarrier.AddJobHandleForProducer(val2);
			m_IconCommandSystem.AddCommandBufferWriter(val2);
			((SystemBase)this).Dependency = val3;
		}
	}

	public void GetMaxTransportSpeed(out float maxPassengerTransportSpeed, out float maxCargoTransportSpeed)
	{
		((JobHandle)(ref m_MaxTransportSpeedDeps)).Complete();
		maxPassengerTransportSpeed = m_MaxTransportSpeed[0];
		maxCargoTransportSpeed = m_MaxTransportSpeed[1];
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		float num = m_MaxTransportSpeed[0];
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		float num2 = m_MaxTransportSpeed[1];
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		float num = default(float);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		float num2 = default(float);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
		m_MaxTransportSpeed[0] = num;
		m_MaxTransportSpeed[1] = num2;
	}

	public void SetDefaults(Context context)
	{
		m_MaxTransportSpeed[0] = 277.77777f;
		m_MaxTransportSpeed[1] = 277.77777f;
	}

	public static int CalculateVehicleCount(float vehicleInterval, float lineDuration)
	{
		return math.max(1, (int)math.round(lineDuration / math.max(1f, vehicleInterval)));
	}

	public static float CalculateVehicleInterval(float lineDuration, int vehicleCount)
	{
		return lineDuration / (float)math.max(1, vehicleCount);
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
	public TransportLineSystem()
	{
	}
}
