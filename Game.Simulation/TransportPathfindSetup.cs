using Colossal.Collections;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct TransportPathfindSetup
{
	[BurstCompile]
	private struct SetupTransportVehiclesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Controller> m_ControllerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.Color> m_RouteColorType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<VehicleModel> m_VehicleModelData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.Color> m_RouteColorData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.TransportDepot> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TransportDepot>(ref m_TransportDepotType);
			Entity entity;
			if (nativeArray2.Length != 0)
			{
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				TransportVehicleRequest transportVehicleRequest = default(TransportVehicleRequest);
				PrefabRef prefabRef = default(PrefabRef);
				TransportDepotData transportDepotData = default(TransportDepotData);
				for (int i = 0; i < m_SetupData.Length; i++)
				{
					m_SetupData.GetItem(i, out entity, out var owner, out var targetSeeker);
					m_TransportVehicleRequestData.TryGetComponent(owner, ref transportVehicleRequest);
					TransportLineData transportLineData = default(TransportLineData);
					if (targetSeeker.m_PrefabRef.TryGetComponent(transportVehicleRequest.m_Route, ref prefabRef))
					{
						m_TransportLineData.TryGetComponent(prefabRef.m_Prefab, ref transportLineData);
					}
					for (int j = 0; j < nativeArray2.Length; j++)
					{
						if ((nativeArray2[j].m_Flags & TransportDepotFlags.HasAvailableVehicles) != 0)
						{
							prefabRef = nativeArray3[j];
							if (m_PrefabTransportDepotData.TryGetComponent(prefabRef.m_Prefab, ref transportDepotData) && transportDepotData.m_TransportType == transportLineData.m_TransportType)
							{
								float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
								Entity entity2 = nativeArray[j];
								targetSeeker.FindTargets(entity2, cost);
							}
						}
					}
				}
			}
			else
			{
				if (!((ArchetypeChunk)(ref chunk)).Has<Owner>(ref m_OwnerType))
				{
					return;
				}
				NativeArray<Game.Vehicles.CargoTransport> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.CargoTransport>(ref m_CargoTransportType);
				NativeArray<Game.Vehicles.PublicTransport> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PublicTransport>(ref m_PublicTransportType);
				NativeArray<Controller> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Controller>(ref m_ControllerType);
				NativeArray<Game.Routes.Color> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.Color>(ref m_RouteColorType);
				NativeArray<PrefabRef> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
				TransportVehicleRequest transportVehicleRequest2 = default(TransportVehicleRequest);
				PrefabRef prefabRef2 = default(PrefabRef);
				VehicleModel vehicleModel = default(VehicleModel);
				Game.Routes.Color color = default(Game.Routes.Color);
				Game.Routes.Color color2 = default(Game.Routes.Color);
				for (int k = 0; k < m_SetupData.Length; k++)
				{
					m_SetupData.GetItem(k, out entity, out var owner2, out var targetSeeker2);
					m_TransportVehicleRequestData.TryGetComponent(owner2, ref transportVehicleRequest2);
					TransportLineData transportLineData2 = default(TransportLineData);
					if (targetSeeker2.m_PrefabRef.TryGetComponent(transportVehicleRequest2.m_Route, ref prefabRef2))
					{
						m_TransportLineData.TryGetComponent(prefabRef2.m_Prefab, ref transportLineData2);
					}
					bool flag = m_VehicleModelData.TryGetComponent(transportVehicleRequest2.m_Route, ref vehicleModel);
					bool flag2 = m_RouteColorData.TryGetComponent(transportVehicleRequest2.m_Route, ref color);
					if (nativeArray4.Length != 0 != transportLineData2.m_CargoTransport || nativeArray5.Length != 0 != transportLineData2.m_PassengerTransport)
					{
						continue;
					}
					for (int l = 0; l < nativeArray.Length; l++)
					{
						Entity val = nativeArray[l];
						float num = 0f;
						if (nativeArray6.Length != 0)
						{
							Controller controller = nativeArray6[l];
							if (controller.m_Controller != Entity.Null && controller.m_Controller != val)
							{
								continue;
							}
						}
						if (nativeArray4.Length != 0)
						{
							Game.Vehicles.CargoTransport cargoTransport = nativeArray4[l];
							if (cargoTransport.m_RequestCount > 0 || (cargoTransport.m_State & (CargoTransportFlags.EnRoute | CargoTransportFlags.RequiresMaintenance | CargoTransportFlags.DummyTraffic | CargoTransportFlags.Disabled)) != 0)
							{
								continue;
							}
						}
						if (nativeArray5.Length != 0)
						{
							Game.Vehicles.PublicTransport publicTransport = nativeArray5[l];
							if (publicTransport.m_RequestCount > 0 || (publicTransport.m_State & (PublicTransportFlags.EnRoute | PublicTransportFlags.Evacuating | PublicTransportFlags.PrisonerTransport | PublicTransportFlags.RequiresMaintenance | PublicTransportFlags.DummyTraffic | PublicTransportFlags.Disabled)) != 0)
							{
								continue;
							}
						}
						if (flag)
						{
							prefabRef2 = nativeArray8[l];
							DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
							if (bufferAccessor.Length != 0)
							{
								layout = bufferAccessor[l];
							}
							if (!RouteUtils.CheckVehicleModel(vehicleModel, prefabRef2, layout, ref targetSeeker2.m_PrefabRef))
							{
								continue;
							}
						}
						if (CollectionUtils.TryGet<Game.Routes.Color>(nativeArray7, l, ref color2))
						{
							if (flag2 && color.m_Color.r == color2.m_Color.r && color.m_Color.g == color2.m_Color.g && color.m_Color.b == color2.m_Color.b && color.m_Color.a == color2.m_Color.a)
							{
								num -= targetSeeker2.m_PathfindParameters.m_Weights.time * 10f;
							}
						}
						else
						{
							num -= targetSeeker2.m_PathfindParameters.m_Weights.time * math.select(10f, 5f, flag2);
						}
						targetSeeker2.FindTargets(val, num);
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
	private struct SetupTaxisJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.Taxi> m_TaxiType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> m_TaxiRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

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
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.TransportDepot> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TransportDepot>(ref m_TransportDepotType);
			Entity entity;
			if (nativeArray2.Length != 0)
			{
				NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				TaxiRequest taxiRequest = default(TaxiRequest);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Game.Buildings.TransportDepot transportDepot = nativeArray2[i];
					if ((transportDepot.m_Flags & TransportDepotFlags.HasAvailableVehicles) == 0)
					{
						continue;
					}
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out entity, out var owner, out var targetSeeker);
						m_TaxiRequestData.TryGetComponent(owner, ref taxiRequest);
						switch (taxiRequest.m_Type)
						{
						case TaxiRequestType.Stand:
							if (flag)
							{
								continue;
							}
							break;
						case TaxiRequestType.Customer:
							if ((transportDepot.m_Flags & TransportDepotFlags.HasDispatchCenter) == 0)
							{
								continue;
							}
							break;
						case TaxiRequestType.Outside:
							if (!flag)
							{
								continue;
							}
							break;
						default:
							continue;
						}
						PrefabRef prefabRef = nativeArray3[i];
						if (m_PrefabTransportDepotData[prefabRef.m_Prefab].m_TransportType == TransportType.Taxi)
						{
							Entity val = nativeArray[i];
							if (AreaUtils.CheckServiceDistrict(taxiRequest.m_District1, taxiRequest.m_District2, val, m_ServiceDistricts))
							{
								float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
								targetSeeker.FindTargets(val, cost);
							}
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.Taxi> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.Taxi>(ref m_TaxiType);
			if (nativeArray4.Length == 0)
			{
				return;
			}
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PathOwner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			Game.Buildings.TransportDepot transportDepot2 = default(Game.Buildings.TransportDepot);
			TaxiRequest taxiRequest2 = default(TaxiRequest);
			DynamicBuffer<ServiceDispatch> val3 = default(DynamicBuffer<ServiceDispatch>);
			TaxiRequest taxiRequest3 = default(TaxiRequest);
			PathOwner pathOwner = default(PathOwner);
			for (int k = 0; k < nativeArray4.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.Taxi taxi = nativeArray4[k];
				Owner owner2 = nativeArray5[k];
				if ((taxi.m_State & (TaxiFlags.RequiresMaintenance | TaxiFlags.Dispatched | TaxiFlags.Disabled)) != 0 || !m_TransportDepotData.TryGetComponent(owner2.m_Owner, ref transportDepot2))
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out entity, out var owner3, out var targetSeeker2);
					m_TaxiRequestData.TryGetComponent(owner3, ref taxiRequest2);
					switch (taxiRequest2.m_Type)
					{
					case TaxiRequestType.Stand:
						if (((taxi.m_State & (TaxiFlags.Returning | TaxiFlags.Transporting)) == 0 && nativeArray6.Length != 0) || (taxi.m_State & TaxiFlags.FromOutside) != 0)
						{
							continue;
						}
						break;
					case TaxiRequestType.Customer:
						if ((transportDepot2.m_Flags & TransportDepotFlags.HasDispatchCenter) == 0)
						{
							continue;
						}
						break;
					case TaxiRequestType.Outside:
						if ((taxi.m_State & TaxiFlags.FromOutside) == 0)
						{
							continue;
						}
						break;
					default:
						continue;
					}
					if (!AreaUtils.CheckServiceDistrict(taxiRequest2.m_District1, taxiRequest2.m_District2, owner2.m_Owner, m_ServiceDistricts))
					{
						continue;
					}
					if (CollectionUtils.TryGet<ServiceDispatch>(bufferAccessor2, k, ref val3) && val3.Length != 0 && (taxi.m_State & TaxiFlags.Requested) != 0)
					{
						Entity request = val3[0].m_Request;
						if (m_TaxiRequestData.TryGetComponent(request, ref taxiRequest3) && ((int)taxiRequest2.m_Type < (int)taxiRequest3.m_Type || (taxiRequest2.m_Type == taxiRequest3.m_Type && taxiRequest2.m_Priority <= taxiRequest3.m_Priority)))
						{
							continue;
						}
					}
					if (CollectionUtils.TryGet<PathOwner>(nativeArray6, k, ref pathOwner))
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						int num = val4.Length - taxi.m_ExtraPathElementCount;
						if (num > val4.Length || (taxi.m_State & TaxiFlags.Transporting) == 0)
						{
							targetSeeker2.FindTargets(val2, 0f);
							continue;
						}
						if (num <= pathOwner.m_ElementIndex)
						{
							targetSeeker2.FindTargets(val2, val2, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: true);
							continue;
						}
						float cost2 = (float)(num - pathOwner.m_ElementIndex) * taxi.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
						PathElement pathElement = val4[num - 1];
						targetSeeker2.m_Buffer.Enqueue(new PathTarget(val2, pathElement.m_Target, pathElement.m_TargetDelta.y, cost2));
					}
					else
					{
						targetSeeker2.FindTargets(val2, 0f);
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
	private struct SetupRouteWaypointsJob : IJobParallelFor
	{
		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			if (!m_Waypoints.HasBuffer(entity))
			{
				return;
			}
			DynamicBuffer<RouteWaypoint> val = m_Waypoints[entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity waypoint = val[i].m_Waypoint;
				if (targetSeeker.m_RouteLane.HasComponent(waypoint))
				{
					RouteLane routeLane = targetSeeker.m_RouteLane[waypoint];
					if (!(routeLane.m_StartLane == Entity.Null))
					{
						targetSeeker.m_Buffer.Enqueue(new PathTarget(waypoint, routeLane.m_StartLane, routeLane.m_StartCurvePos, 0f));
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct TransportVehicleRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<TransportVehicleRequest> m_TransportVehicleRequestType;

		[ReadOnly]
		public ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

		[ReadOnly]
		public ComponentLookup<VehicleModel> m_VehicleModelData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineData;

		[ReadOnly]
		public ComponentLookup<TransportDepotData> m_TransportDepotData;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_Waypoints;

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
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<TransportVehicleRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TransportVehicleRequest>(ref m_TransportVehicleRequestType);
			TransportVehicleRequest transportVehicleRequest = default(TransportVehicleRequest);
			PrefabRef prefabRef = default(PrefabRef);
			TransportDepotData transportDepotData = default(TransportDepotData);
			PrefabRef prefabRef2 = default(PrefabRef);
			TransportLineData transportLineData = default(TransportLineData);
			VehicleModel vehicleModel = default(VehicleModel);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_TransportVehicleRequestData.TryGetComponent(owner, ref transportVehicleRequest) || !targetSeeker.m_PrefabRef.TryGetComponent(transportVehicleRequest.m_Route, ref prefabRef))
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
				if (m_TransportDepotData.TryGetComponent(prefabRef.m_Prefab, ref transportDepotData))
				{
					flag = true;
				}
				else
				{
					flag2 = m_PublicTransportData.HasComponent(transportVehicleRequest.m_Route);
					flag3 = m_CargoTransportData.HasComponent(transportVehicleRequest.m_Route);
					targetSeeker.m_VehicleLayout.TryGetBuffer(transportVehicleRequest.m_Route, ref layout);
				}
				if (!flag && !flag2 && !flag3)
				{
					continue;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					TransportVehicleRequest transportVehicleRequest2 = nativeArray3[j];
					if (!targetSeeker.m_PrefabRef.TryGetComponent(transportVehicleRequest2.m_Route, ref prefabRef2) || !m_TransportLineData.TryGetComponent(prefabRef2.m_Prefab, ref transportLineData))
					{
						continue;
					}
					if (flag)
					{
						if (transportLineData.m_TransportType != transportDepotData.m_TransportType)
						{
							continue;
						}
					}
					else if (flag3 != transportLineData.m_CargoTransport || flag2 != transportLineData.m_PassengerTransport || (m_VehicleModelData.TryGetComponent(transportVehicleRequest2.m_Route, ref vehicleModel) && !RouteUtils.CheckVehicleModel(vehicleModel, prefabRef, layout, ref targetSeeker.m_PrefabRef)))
					{
						continue;
					}
					Entity target = nativeArray[j];
					DynamicBuffer<RouteWaypoint> val = m_Waypoints[transportVehicleRequest2.m_Route];
					for (int k = 0; k < val.Length; k++)
					{
						Entity waypoint = val[k].m_Waypoint;
						if (targetSeeker.m_RouteLane.HasComponent(waypoint))
						{
							RouteLane routeLane = targetSeeker.m_RouteLane[waypoint];
							if (!(routeLane.m_StartLane == Entity.Null))
							{
								targetSeeker.m_Buffer.Enqueue(new PathTarget(target, routeLane.m_StartLane, routeLane.m_StartCurvePos, 0f));
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
	private struct TaxiRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<TaxiRequest> m_TaxiRequestType;

		[ReadOnly]
		public ComponentLookup<TaxiRequest> m_TaxiRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

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
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<TaxiRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxiRequest>(ref m_TaxiRequestType);
			TaxiRequest taxiRequest = default(TaxiRequest);
			Game.Buildings.TransportDepot transportDepot = default(Game.Buildings.TransportDepot);
			Owner owner2 = default(Owner);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_TaxiRequestData.TryGetComponent(owner, ref taxiRequest))
				{
					continue;
				}
				bool flag = false;
				Entity service = Entity.Null;
				if (taxiRequest.m_Type != TaxiRequestType.Outside)
				{
					if (m_TransportDepotData.TryGetComponent(taxiRequest.m_Seeker, ref transportDepot))
					{
						flag = (transportDepot.m_Flags & TransportDepotFlags.HasDispatchCenter) != 0;
						service = taxiRequest.m_Seeker;
					}
					else
					{
						if (!targetSeeker.m_PrefabRef.HasComponent(taxiRequest.m_Seeker))
						{
							continue;
						}
						if (targetSeeker.m_Owner.TryGetComponent(taxiRequest.m_Seeker, ref owner2) && m_TransportDepotData.TryGetComponent(owner2.m_Owner, ref transportDepot))
						{
							flag = (transportDepot.m_Flags & TransportDepotFlags.HasDispatchCenter) != 0;
							service = owner2.m_Owner;
						}
					}
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					TaxiRequest taxiRequest2 = nativeArray3[j];
					switch (taxiRequest2.m_Type)
					{
					case TaxiRequestType.Stand:
						if (taxiRequest.m_Type == TaxiRequestType.Outside)
						{
							continue;
						}
						targetSeeker.m_SetupQueueTarget.m_Methods = PathMethod.Road | PathMethod.MediumRoad;
						break;
					case TaxiRequestType.Customer:
						if (!flag)
						{
							continue;
						}
						targetSeeker.m_SetupQueueTarget.m_Methods = PathMethod.Boarding;
						break;
					case TaxiRequestType.Outside:
						if (taxiRequest.m_Type != TaxiRequestType.Outside)
						{
							continue;
						}
						targetSeeker.m_SetupQueueTarget.m_Methods = PathMethod.Boarding;
						break;
					default:
						continue;
					}
					if (AreaUtils.CheckServiceDistrict(taxiRequest2.m_District1, taxiRequest2.m_District2, service, m_ServiceDistricts))
					{
						targetSeeker.FindTargets(nativeArray[j], taxiRequest2.m_Seeker, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_TransportVehicleQuery;

	private EntityQuery m_TaxiQuery;

	private EntityQuery m_TransportVehicleRequestQuery;

	private EntityQuery m_TaxiRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<TransportVehicleRequest> m_TransportVehicleRequestType;

	private ComponentTypeHandle<TaxiRequest> m_TaxiRequestType;

	private ComponentTypeHandle<Game.Buildings.TransportDepot> m_TransportDepotType;

	private ComponentTypeHandle<Game.Vehicles.CargoTransport> m_CargoTransportType;

	private ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

	private ComponentTypeHandle<Game.Vehicles.Taxi> m_TaxiType;

	private ComponentTypeHandle<Controller> m_ControllerType;

	private ComponentTypeHandle<Game.Routes.Color> m_RouteColorType;

	private ComponentTypeHandle<PrefabRef> m_PrefabRefType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private BufferTypeHandle<LayoutElement> m_LayoutElementType;

	private ComponentLookup<TransportVehicleRequest> m_TransportVehicleRequestData;

	private ComponentLookup<TaxiRequest> m_TaxiRequestData;

	private ComponentLookup<VehicleModel> m_VehicleModelData;

	private ComponentLookup<Game.Routes.Color> m_RouteColorData;

	private ComponentLookup<Game.Buildings.TransportDepot> m_TransportDepotData;

	private ComponentLookup<Game.Vehicles.CargoTransport> m_CargoTransportData;

	private ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

	private ComponentLookup<TransportLineData> m_TransportLineData;

	private ComponentLookup<TransportDepotData> m_PrefabTransportDepotData;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private BufferLookup<RouteWaypoint> m_Waypoints;

	public TransportPathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.TransportDepot>(),
			ComponentType.ReadOnly<Game.Vehicles.CargoTransport>(),
			ComponentType.ReadOnly<Game.Vehicles.PublicTransport>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_TransportVehicleQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.TransportDepot>(),
			ComponentType.ReadOnly<Game.Vehicles.Taxi>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_TaxiQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_TransportVehicleRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<TransportVehicleRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_TaxiRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<TaxiRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_OutsideConnectionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_TransportVehicleRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<TransportVehicleRequest>(true);
		m_TaxiRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<TaxiRequest>(true);
		m_TransportDepotType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.TransportDepot>(true);
		m_CargoTransportType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.CargoTransport>(true);
		m_PublicTransportType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(true);
		m_TaxiType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.Taxi>(true);
		m_ControllerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Controller>(true);
		m_RouteColorType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Routes.Color>(true);
		m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_LayoutElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<LayoutElement>(true);
		m_TransportVehicleRequestData = ((SystemBase)system).GetComponentLookup<TransportVehicleRequest>(true);
		m_TaxiRequestData = ((SystemBase)system).GetComponentLookup<TaxiRequest>(true);
		m_VehicleModelData = ((SystemBase)system).GetComponentLookup<VehicleModel>(true);
		m_RouteColorData = ((SystemBase)system).GetComponentLookup<Game.Routes.Color>(true);
		m_TransportDepotData = ((SystemBase)system).GetComponentLookup<Game.Buildings.TransportDepot>(true);
		m_CargoTransportData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.CargoTransport>(true);
		m_PublicTransportData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
		m_TransportLineData = ((SystemBase)system).GetComponentLookup<TransportLineData>(true);
		m_PrefabTransportDepotData = ((SystemBase)system).GetComponentLookup<TransportDepotData>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_Waypoints = ((SystemBase)system).GetBufferLookup<RouteWaypoint>(true);
	}

	public JobHandle SetupTransportVehicle(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_TransportDepotType.Update((SystemBase)(object)system);
		m_CargoTransportType.Update((SystemBase)(object)system);
		m_PublicTransportType.Update((SystemBase)(object)system);
		m_ControllerType.Update((SystemBase)(object)system);
		m_RouteColorType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_LayoutElementType.Update((SystemBase)(object)system);
		m_TransportVehicleRequestData.Update((SystemBase)(object)system);
		m_VehicleModelData.Update((SystemBase)(object)system);
		m_RouteColorData.Update((SystemBase)(object)system);
		m_PrefabTransportDepotData.Update((SystemBase)(object)system);
		m_TransportLineData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupTransportVehiclesJob>(new SetupTransportVehiclesJob
		{
			m_EntityType = m_EntityType,
			m_TransportDepotType = m_TransportDepotType,
			m_CargoTransportType = m_CargoTransportType,
			m_PublicTransportType = m_PublicTransportType,
			m_ControllerType = m_ControllerType,
			m_RouteColorType = m_RouteColorType,
			m_OwnerType = m_OwnerType,
			m_PrefabRefType = m_PrefabRefType,
			m_LayoutElementType = m_LayoutElementType,
			m_TransportVehicleRequestData = m_TransportVehicleRequestData,
			m_VehicleModelData = m_VehicleModelData,
			m_RouteColorData = m_RouteColorData,
			m_PrefabTransportDepotData = m_PrefabTransportDepotData,
			m_TransportLineData = m_TransportLineData,
			m_SetupData = setupData
		}, m_TransportVehicleQuery, inputDeps);
	}

	public JobHandle SetupTaxi(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_TransportDepotType.Update((SystemBase)(object)system);
		m_TaxiType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OutsideConnectionType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_TaxiRequestData.Update((SystemBase)(object)system);
		m_TransportDepotData.Update((SystemBase)(object)system);
		m_PrefabTransportDepotData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupTaxisJob>(new SetupTaxisJob
		{
			m_EntityType = m_EntityType,
			m_TransportDepotType = m_TransportDepotType,
			m_TaxiType = m_TaxiType,
			m_OwnerType = m_OwnerType,
			m_PathOwnerType = m_PathOwnerType,
			m_OutsideConnectionType = m_OutsideConnectionType,
			m_PrefabRefType = m_PrefabRefType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_TaxiRequestData = m_TaxiRequestData,
			m_TransportDepotData = m_TransportDepotData,
			m_PrefabTransportDepotData = m_PrefabTransportDepotData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_TaxiQuery, inputDeps);
	}

	public JobHandle SetupRouteWaypoints(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		m_Waypoints.Update((SystemBase)(object)system);
		return IJobParallelForExtensions.Schedule<SetupRouteWaypointsJob>(new SetupRouteWaypointsJob
		{
			m_Waypoints = m_Waypoints,
			m_SetupData = setupData
		}, setupData.Length, 1, inputDeps);
	}

	public JobHandle SetupTransportVehicleRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_TransportVehicleRequestType.Update((SystemBase)(object)system);
		m_TransportVehicleRequestData.Update((SystemBase)(object)system);
		m_VehicleModelData.Update((SystemBase)(object)system);
		m_PublicTransportData.Update((SystemBase)(object)system);
		m_CargoTransportData.Update((SystemBase)(object)system);
		m_TransportLineData.Update((SystemBase)(object)system);
		m_PrefabTransportDepotData.Update((SystemBase)(object)system);
		m_Waypoints.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<TransportVehicleRequestsJob>(new TransportVehicleRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_TransportVehicleRequestType = m_TransportVehicleRequestType,
			m_TransportVehicleRequestData = m_TransportVehicleRequestData,
			m_VehicleModelData = m_VehicleModelData,
			m_PublicTransportData = m_PublicTransportData,
			m_CargoTransportData = m_CargoTransportData,
			m_TransportLineData = m_TransportLineData,
			m_TransportDepotData = m_PrefabTransportDepotData,
			m_Waypoints = m_Waypoints,
			m_SetupData = setupData
		}, m_TransportVehicleRequestQuery, inputDeps);
	}

	public JobHandle SetupTaxiRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_TaxiRequestType.Update((SystemBase)(object)system);
		m_TaxiRequestData.Update((SystemBase)(object)system);
		m_TransportDepotData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<TaxiRequestsJob>(new TaxiRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_TaxiRequestType = m_TaxiRequestType,
			m_TaxiRequestData = m_TaxiRequestData,
			m_TransportDepotData = m_TransportDepotData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_TaxiRequestQuery, inputDeps);
	}
}
