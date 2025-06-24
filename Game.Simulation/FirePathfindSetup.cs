using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
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

public struct FirePathfindSetup
{
	[BurstCompile]
	private struct SetupFireEnginesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.FireStation> m_FireStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.FireEngine> m_FireEngineType;

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
		public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public Entity m_City;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>() && !CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.FireStation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.FireStation>(ref m_FireStationType);
			if (nativeArray2.Length != 0)
			{
				FireRescueRequest fireRescueRequest = default(FireRescueRequest);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					Game.Buildings.FireStation fireStation = nativeArray2[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var owner, out var targetSeeker);
						m_FireRescueRequestData.TryGetComponent(owner, ref fireRescueRequest);
						RoadTypes roadTypes = RoadTypes.None;
						if (fireRescueRequest.m_Target == val)
						{
							if ((fireStation.m_Flags & FireStationFlags.HasFreeFireEngines) != 0)
							{
								roadTypes |= RoadTypes.Car;
							}
							if ((fireStation.m_Flags & FireStationFlags.HasFreeFireHelicopters) != 0)
							{
								roadTypes |= RoadTypes.Helicopter;
							}
						}
						else if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							if ((fireStation.m_Flags & FireStationFlags.HasAvailableFireEngines) != 0)
							{
								roadTypes |= RoadTypes.Car;
							}
							if ((fireStation.m_Flags & FireStationFlags.HasAvailableFireHelicopters) != 0)
							{
								roadTypes |= RoadTypes.Helicopter;
							}
						}
						if (fireRescueRequest.m_Type == FireRescueRequestType.Disaster && (fireStation.m_Flags & FireStationFlags.DisasterResponseAvailable) == 0)
						{
							roadTypes = RoadTypes.None;
						}
						roadTypes &= targetSeeker.m_SetupQueueTarget.m_RoadTypes | targetSeeker.m_SetupQueueTarget.m_FlyingTypes;
						if (roadTypes != RoadTypes.None)
						{
							float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
							RoadTypes roadTypes2 = targetSeeker.m_SetupQueueTarget.m_RoadTypes;
							RoadTypes flyingTypes = targetSeeker.m_SetupQueueTarget.m_FlyingTypes;
							targetSeeker.m_SetupQueueTarget.m_RoadTypes &= roadTypes;
							targetSeeker.m_SetupQueueTarget.m_FlyingTypes &= roadTypes;
							targetSeeker.FindTargets(val, cost);
							targetSeeker.m_SetupQueueTarget.m_RoadTypes = roadTypes2;
							targetSeeker.m_SetupQueueTarget.m_FlyingTypes = flyingTypes;
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.FireEngine> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.FireEngine>(ref m_FireEngineType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			FireRescueRequest fireRescueRequest2 = default(FireRescueRequest);
			Owner owner3 = default(Owner);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.FireEngine fireEngine = nativeArray3[k];
				if ((fireEngine.m_State & (FireEngineFlags.Empty | FireEngineFlags.EstimatedEmpty)) != 0)
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var owner2, out var targetSeeker2);
					m_FireRescueRequestData.TryGetComponent(owner2, ref fireRescueRequest2);
					if (fireRescueRequest2.m_Type == FireRescueRequestType.Disaster && (fireEngine.m_State & FireEngineFlags.DisasterResponse) == 0)
					{
						continue;
					}
					float num = 0f;
					if (CollectionUtils.TryGet<Owner>(nativeArray5, k, ref owner3))
					{
						if (!AreaUtils.CheckServiceDistrict(entity2, owner3.m_Owner, m_ServiceDistricts))
						{
							continue;
						}
						if (m_OutsideConnections.HasComponent(owner3.m_Owner))
						{
							num += 30f;
						}
					}
					if (fireRescueRequest2.m_Target != owner3.m_Owner && (fireEngine.m_State & FireEngineFlags.Disabled) != 0)
					{
						continue;
					}
					if ((fireEngine.m_State & FireEngineFlags.Returning) != 0 || nativeArray4.Length == 0)
					{
						targetSeeker2.FindTargets(val2, num);
						continue;
					}
					PathOwner pathOwner = nativeArray4[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num2 = math.min(fireEngine.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					bool flag = false;
					if (num2 >= 1)
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num += (float)(val4.Length - pathOwner.m_ElementIndex) * fireEngine.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
							pathElement = val4[val4.Length - 1];
							flag = true;
						}
					}
					for (int m = 1; m < num2; m++)
					{
						Entity request = val3[m].m_Request;
						if (m_PathInformationData.TryGetComponent(request, ref pathInformation))
						{
							num += pathInformation.m_Duration * targetSeeker2.m_PathfindParameters.m_Weights.time;
						}
						num += 10f * targetSeeker2.m_PathfindParameters.m_Weights.time;
						if (m_PathElements.TryGetBuffer(request, ref val5) && val5.Length != 0)
						{
							pathElement = val5[val5.Length - 1];
							flag = true;
						}
					}
					if (flag)
					{
						targetSeeker2.m_Buffer.Enqueue(new PathTarget(val2, pathElement.m_Target, pathElement.m_TargetDelta.y, num));
					}
					else
					{
						targetSeeker2.FindTargets(val2, val2, num, EdgeFlags.DefaultMask, allowAccessRestriction: true, num2 >= 1);
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
	private struct SetupEmergencySheltersJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.EmergencyShelter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.EmergencyShelter>(ref m_EmergencyShelterType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var targetSeeker);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity val = nativeArray[j];
					if ((nativeArray2[j].m_Flags & EmergencyShelterFlags.HasShelterSpace) != 0 && AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
					{
						targetSeeker.FindTargets(val, 0f);
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
	private struct SetupEvacuationTransportJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

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
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.EmergencyShelter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.EmergencyShelter>(ref m_EmergencyShelterType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					if ((nativeArray2[i].m_Flags & (EmergencyShelterFlags.HasAvailableVehicles | EmergencyShelterFlags.HasShelterSpace)) != (EmergencyShelterFlags.HasAvailableVehicles | EmergencyShelterFlags.HasShelterSpace))
					{
						continue;
					}
					Entity val = nativeArray[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
							targetSeeker.FindTargets(val, cost);
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.PublicTransport> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PublicTransport>(ref m_PublicTransportType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Game.Vehicles.PublicTransport publicTransport = nativeArray3[k];
				if ((publicTransport.m_State & (PublicTransportFlags.Evacuating | PublicTransportFlags.Disabled | PublicTransportFlags.Full)) != PublicTransportFlags.Evacuating)
				{
					continue;
				}
				Entity val2 = nativeArray[k];
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var targetSeeker2);
					if ((nativeArray5.Length != 0 && !AreaUtils.CheckServiceDistrict(entity2, nativeArray5[k].m_Owner, m_ServiceDistricts)) || (publicTransport.m_State & PublicTransportFlags.DummyTraffic) != 0)
					{
						continue;
					}
					if ((publicTransport.m_State & PublicTransportFlags.Returning) != 0 || nativeArray4.Length == 0)
					{
						targetSeeker2.FindTargets(val2, 0f);
						continue;
					}
					PathOwner pathOwner = nativeArray4[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num = math.min(publicTransport.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					float num2 = 0f;
					bool flag = false;
					if (num >= 1)
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num2 += (float)(val4.Length - pathOwner.m_ElementIndex) * publicTransport.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
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
	private struct EvacuationRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<EvacuationRequest> m_EvacuationRequestType;

		[ReadOnly]
		public ComponentLookup<EvacuationRequest> m_EvacuationRequestData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

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
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<EvacuationRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EvacuationRequest>(ref m_EvacuationRequestType);
			EvacuationRequest evacuationRequest = default(EvacuationRequest);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			Owner owner2 = default(Owner);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_EvacuationRequestData.TryGetComponent(owner, ref evacuationRequest))
				{
					continue;
				}
				Entity service = Entity.Null;
				if (m_PublicTransportData.TryGetComponent(evacuationRequest.m_Target, ref publicTransport))
				{
					if (targetSeeker.m_Owner.TryGetComponent(evacuationRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!targetSeeker.m_PrefabRef.HasComponent(evacuationRequest.m_Target))
					{
						continue;
					}
					service = evacuationRequest.m_Target;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) == 0)
					{
						EvacuationRequest evacuationRequest2 = nativeArray3[j];
						Entity district = Entity.Null;
						if (m_CurrentDistrictData.HasComponent(evacuationRequest2.m_Target))
						{
							district = m_CurrentDistrictData[evacuationRequest2.m_Target].m_District;
						}
						if (AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
						{
							targetSeeker.FindTargets(nativeArray[j], evacuationRequest2.m_Target, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
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
	private struct FireRescueRequestsJob : IJobChunk
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
		public ComponentTypeHandle<FireRescueRequest> m_FireRescueRequestType;

		[ReadOnly]
		public ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.FireEngine> m_FireEngineData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.FireStation> m_FireStationData;

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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<FireRescueRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<FireRescueRequest>(ref m_FireRescueRequestType);
			FireRescueRequest fireRescueRequest = default(FireRescueRequest);
			Game.Vehicles.FireEngine fireEngine = default(Game.Vehicles.FireEngine);
			Owner owner2 = default(Owner);
			Game.Buildings.FireStation fireStation = default(Game.Buildings.FireStation);
			Transform transform = default(Transform);
			Transform transform2 = default(Transform);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_FireRescueRequestData.TryGetComponent(owner, ref fireRescueRequest))
				{
					continue;
				}
				bool flag = false;
				Entity service = Entity.Null;
				if (m_FireEngineData.TryGetComponent(fireRescueRequest.m_Target, ref fireEngine))
				{
					if (targetSeeker.m_Owner.TryGetComponent(fireRescueRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
					flag = (fireEngine.m_State & FireEngineFlags.DisasterResponse) != 0;
				}
				else
				{
					if (!m_FireStationData.TryGetComponent(fireRescueRequest.m_Target, ref fireStation))
					{
						continue;
					}
					service = fireRescueRequest.m_Target;
					flag = (fireStation.m_Flags & FireStationFlags.DisasterResponseAvailable) != 0;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					FireRescueRequest fireRescueRequest2 = nativeArray3[j];
					if (fireRescueRequest2.m_Type == FireRescueRequestType.Disaster && !flag)
					{
						continue;
					}
					Entity district = Entity.Null;
					if (m_CurrentDistrictData.HasComponent(fireRescueRequest2.m_Target))
					{
						district = m_CurrentDistrictData[fireRescueRequest2.m_Target].m_District;
					}
					else if (targetSeeker.m_Transform.TryGetComponent(fireRescueRequest2.m_Target, ref transform))
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
					if (!AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
					{
						continue;
					}
					targetSeeker.FindTargets(nativeArray[j], fireRescueRequest2.m_Target, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
					if (!targetSeeker.m_Transform.TryGetComponent(fireRescueRequest2.m_Target, ref transform2))
					{
						continue;
					}
					if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) != 0 && (targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						Entity lane = Entity.Null;
						float curvePos = 0f;
						float distance = float.MaxValue;
						targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(transform2.m_Position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
						if (lane != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(nativeArray[j], lane, curvePos, 0f));
						}
					}
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

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_FireEngineQuery;

	private EntityQuery m_EmergencyShelterQuery;

	private EntityQuery m_EvacuationTransportQuery;

	private EntityQuery m_EvacuationRequestQuery;

	private EntityQuery m_FireRescueRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<FireRescueRequest> m_FireRescueRequestType;

	private ComponentTypeHandle<EvacuationRequest> m_EvacuationRequestType;

	private ComponentTypeHandle<Game.Buildings.FireStation> m_FireStationType;

	private ComponentTypeHandle<Game.Buildings.EmergencyShelter> m_EmergencyShelterType;

	private ComponentTypeHandle<Game.Vehicles.FireEngine> m_FireEngineType;

	private ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private ComponentLookup<PathInformation> m_PathInformationData;

	private ComponentLookup<FireRescueRequest> m_FireRescueRequestData;

	private ComponentLookup<EvacuationRequest> m_EvacuationRequestData;

	private ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

	private ComponentLookup<Composition> m_CompositionData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<District> m_DistrictData;

	private ComponentLookup<Game.Buildings.FireStation> m_FireStationData;

	private ComponentLookup<Game.Vehicles.FireEngine> m_FireEngineData;

	private ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

	private ComponentLookup<NetCompositionData> m_NetCompositionData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private ComponentLookup<Game.City.City> m_CityData;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private CitySystem m_CitySystem;

	public FirePathfindSetup(PathfindSetupSystem system)
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
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.FireStation>(),
			ComponentType.ReadOnly<Game.Vehicles.FireEngine>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_FireEngineQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_EmergencyShelterQuery = system.GetSetupQuery(ComponentType.ReadOnly<Game.Buildings.EmergencyShelter>(), ComponentType.ReadOnly<ServiceDispatch>(), ComponentType.Exclude<Temp>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Deleted>());
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.EmergencyShelter>(),
			ComponentType.ReadOnly<Game.Vehicles.PublicTransport>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_EvacuationTransportQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_EvacuationRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<EvacuationRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_FireRescueRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<FireRescueRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_FireRescueRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<FireRescueRequest>(true);
		m_EvacuationRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<EvacuationRequest>(true);
		m_FireStationType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.FireStation>(true);
		m_EmergencyShelterType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.EmergencyShelter>(true);
		m_FireEngineType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.FireEngine>(true);
		m_PublicTransportType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_PathInformationData = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_FireRescueRequestData = ((SystemBase)system).GetComponentLookup<FireRescueRequest>(true);
		m_EvacuationRequestData = ((SystemBase)system).GetComponentLookup<EvacuationRequest>(true);
		m_OutsideConnections = ((SystemBase)system).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		m_CompositionData = ((SystemBase)system).GetComponentLookup<Composition>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_DistrictData = ((SystemBase)system).GetComponentLookup<District>(true);
		m_FireStationData = ((SystemBase)system).GetComponentLookup<Game.Buildings.FireStation>(true);
		m_FireEngineData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.FireEngine>(true);
		m_PublicTransportData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
		m_NetCompositionData = ((SystemBase)system).GetComponentLookup<NetCompositionData>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_CityData = ((SystemBase)system).GetComponentLookup<Game.City.City>(true);
		m_AreaSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_CitySystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<CitySystem>();
	}

	public JobHandle SetupFireEngines(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_FireStationType.Update((SystemBase)(object)system);
		m_FireEngineType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_FireRescueRequestData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_OutsideConnections.Update((SystemBase)(object)system);
		m_CityData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupFireEnginesJob>(new SetupFireEnginesJob
		{
			m_EntityType = m_EntityType,
			m_FireStationType = m_FireStationType,
			m_FireEngineType = m_FireEngineType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_FireRescueRequestData = m_FireRescueRequestData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_OutsideConnections = m_OutsideConnections,
			m_CityData = m_CityData,
			m_City = m_CitySystem.City,
			m_SetupData = setupData
		}, m_FireEngineQuery, inputDeps);
	}

	public JobHandle SetupEmergencyShelters(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_EmergencyShelterType.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupEmergencySheltersJob>(new SetupEmergencySheltersJob
		{
			m_EntityType = m_EntityType,
			m_EmergencyShelterType = m_EmergencyShelterType,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_EmergencyShelterQuery, inputDeps);
	}

	public JobHandle SetupEvacuationTransport(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_EmergencyShelterType.Update((SystemBase)(object)system);
		m_PublicTransportType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupEvacuationTransportJob>(new SetupEvacuationTransportJob
		{
			m_EntityType = m_EntityType,
			m_EmergencyShelterType = m_EmergencyShelterType,
			m_PublicTransportType = m_PublicTransportType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_EvacuationTransportQuery, inputDeps);
	}

	public JobHandle SetupEvacuationRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_EvacuationRequestType.Update((SystemBase)(object)system);
		m_EvacuationRequestData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_PublicTransportData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<EvacuationRequestsJob>(new EvacuationRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_EvacuationRequestType = m_EvacuationRequestType,
			m_EvacuationRequestData = m_EvacuationRequestData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_PublicTransportData = m_PublicTransportData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_EvacuationRequestQuery, inputDeps);
	}

	public JobHandle SetupFireRescueRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_FireRescueRequestType.Update((SystemBase)(object)system);
		m_FireRescueRequestData.Update((SystemBase)(object)system);
		m_CompositionData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_DistrictData.Update((SystemBase)(object)system);
		m_FireEngineData.Update((SystemBase)(object)system);
		m_FireStationData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = JobChunkExtensions.ScheduleParallel<FireRescueRequestsJob>(new FireRescueRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_FireRescueRequestType = m_FireRescueRequestType,
			m_FireRescueRequestData = m_FireRescueRequestData,
			m_CompositionData = m_CompositionData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_DistrictData = m_DistrictData,
			m_FireEngineData = m_FireEngineData,
			m_FireStationData = m_FireStationData,
			m_NetCompositionData = m_NetCompositionData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_NetTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_SetupData = setupData
		}, m_FireRescueRequestQuery, JobHandle.CombineDependencies(inputDeps, dependencies, dependencies2));
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}
}
