using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Events;
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

public struct PolicePathfindSetup
{
	[BurstCompile]
	private struct SetupPolicePatrolsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PoliceStation> m_PoliceStationType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PoliceCar> m_PoliceCarType;

		[ReadOnly]
		public ComponentTypeHandle<Helicopter> m_HelicopterType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public BufferTypeHandle<Passenger> m_PassengerType;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> m_PoliceEmergencyRequestData;

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
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>() && !CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.PoliceStation> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.PoliceStation>(ref m_PoliceStationType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					Game.Buildings.PoliceStation policeStation = nativeArray2[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						PolicePurpose value = (PolicePurpose)targetSeeker.m_SetupQueueTarget.m_Value;
						if ((policeStation.m_PurposeMask & value) == 0)
						{
							continue;
						}
						RoadTypes roadTypes = RoadTypes.None;
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							if ((policeStation.m_Flags & PoliceStationFlags.HasAvailablePatrolCars) != 0)
							{
								roadTypes |= RoadTypes.Car;
							}
							if ((policeStation.m_Flags & PoliceStationFlags.HasAvailablePoliceHelicopters) != 0)
							{
								roadTypes |= RoadTypes.Helicopter;
							}
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
			NativeArray<Game.Vehicles.PoliceCar> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PoliceCar>(ref m_PoliceCarType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			BufferAccessor<Passenger> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Passenger>(ref m_PassengerType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Helicopter>(ref m_HelicopterType);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.PoliceCar policeCar = nativeArray3[k];
				if ((policeCar.m_State & (PoliceCarFlags.ShiftEnded | PoliceCarFlags.Disabled)) != 0)
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var targetSeeker2);
					PolicePurpose value2 = (PolicePurpose)targetSeeker2.m_SetupQueueTarget.m_Value;
					if ((policeCar.m_PurposeMask & value2) == 0 || ((targetSeeker2.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) == 0 && flag))
					{
						continue;
					}
					float num = 0f;
					if ((value2 & PolicePurpose.Patrol) != 0)
					{
						if ((policeCar.m_State & (PoliceCarFlags.Empty | PoliceCarFlags.EstimatedShiftEnd)) != PoliceCarFlags.Empty)
						{
							continue;
						}
					}
					else if (bufferAccessor3.Length != 0)
					{
						num += (float)bufferAccessor3[k].Length * 10f;
					}
					if (nativeArray5.Length != 0)
					{
						if (!AreaUtils.CheckServiceDistrict(entity2, nativeArray5[k].m_Owner, m_ServiceDistricts))
						{
							continue;
						}
						if (m_OutsideConnections.HasComponent(nativeArray5[k].m_Owner))
						{
							num += 30f;
						}
					}
					if ((policeCar.m_State & PoliceCarFlags.Returning) != 0 || nativeArray4.Length == 0)
					{
						targetSeeker2.FindTargets(val2, num);
						continue;
					}
					PathOwner pathOwner = nativeArray4[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num2 = math.min(policeCar.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					bool flag2 = false;
					if (num2 >= 1 && ((value2 & (PolicePurpose.Emergency | PolicePurpose.Intelligence)) == 0 || m_PoliceEmergencyRequestData.HasComponent(val3[0].m_Request)))
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num += (float)(val4.Length - pathOwner.m_ElementIndex) * policeCar.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
							pathElement = val4[val4.Length - 1];
							flag2 = true;
						}
					}
					for (int m = 1; m < num2; m++)
					{
						Entity request = val3[m].m_Request;
						bool flag3 = m_PoliceEmergencyRequestData.HasComponent(request);
						if ((value2 & (PolicePurpose.Emergency | PolicePurpose.Intelligence)) == 0 || flag3)
						{
							if (m_PathInformationData.TryGetComponent(request, ref pathInformation))
							{
								num += pathInformation.m_Duration * targetSeeker2.m_PathfindParameters.m_Weights.time;
							}
							if (flag3)
							{
								num += 30f * targetSeeker2.m_PathfindParameters.m_Weights.time;
							}
							if (m_PathElements.TryGetBuffer(request, ref val5) && val5.Length != 0)
							{
								pathElement = val5[val5.Length - 1];
								flag2 = true;
							}
						}
					}
					if (flag2)
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
	private struct SetupCrimeProducersJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentTypeHandle<CrimeProducer> m_CrimeProducerType;

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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CrimeProducer> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CrimeProducer>(ref m_CrimeProducerType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			BufferAccessor<Employee> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var targetSeeker);
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity entity2 = nativeArray[j];
					CrimeProducer crimeProducer = nativeArray2[j];
					if ((bufferAccessor2.Length <= 0 || bufferAccessor2[j].Length > 0) && (bufferAccessor.Length <= 0 || bufferAccessor[j].Length > 0))
					{
						targetSeeker.FindTargets(entity2, (0f - ((Random)(ref random)).NextFloat(0.5f, 0.7f)) * crimeProducer.m_Crime);
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
	private struct SetupPrisonerTransportJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Prison> m_PrisonType;

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
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.Prison> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.Prison>(ref m_PrisonType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Game.Buildings.Prison prison = nativeArray2[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						if ((prison.m_Flags & (PrisonFlags.HasAvailablePrisonVans | PrisonFlags.HasPrisonerSpace)) == (PrisonFlags.HasAvailablePrisonVans | PrisonFlags.HasPrisonerSpace))
						{
							Entity val = nativeArray[i];
							if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
							{
								float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
								targetSeeker.FindTargets(val, cost);
							}
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
				if ((publicTransport.m_State & (PublicTransportFlags.PrisonerTransport | PublicTransportFlags.Disabled | PublicTransportFlags.Full)) != PublicTransportFlags.PrisonerTransport)
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
	private struct PrisonerTransportRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<PrisonerTransportRequest> m_PrisonerTransportRequestType;

		[ReadOnly]
		public ComponentLookup<PrisonerTransportRequest> m_PrisonerTransportRequestData;

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
			NativeArray<PrisonerTransportRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrisonerTransportRequest>(ref m_PrisonerTransportRequestType);
			PrisonerTransportRequest prisonerTransportRequest = default(PrisonerTransportRequest);
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			Owner owner2 = default(Owner);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_PrisonerTransportRequestData.TryGetComponent(owner, ref prisonerTransportRequest))
				{
					continue;
				}
				Entity service = Entity.Null;
				if (m_PublicTransportData.TryGetComponent(prisonerTransportRequest.m_Target, ref publicTransport))
				{
					if (targetSeeker.m_Owner.TryGetComponent(prisonerTransportRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!targetSeeker.m_PrefabRef.HasComponent(prisonerTransportRequest.m_Target))
					{
						continue;
					}
					service = prisonerTransportRequest.m_Target;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) == 0)
					{
						PrisonerTransportRequest prisonerTransportRequest2 = nativeArray3[j];
						Entity district = Entity.Null;
						if (m_CurrentDistrictData.HasComponent(prisonerTransportRequest2.m_Target))
						{
							district = m_CurrentDistrictData[prisonerTransportRequest2.m_Target].m_District;
						}
						if (AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
						{
							targetSeeker.FindTargets(nativeArray[j], prisonerTransportRequest2.m_Target, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
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
	private struct PoliceRequestsJob : IJobChunk
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
		public ComponentTypeHandle<PolicePatrolRequest> m_PolicePatrolRequestType;

		[ReadOnly]
		public ComponentTypeHandle<PoliceEmergencyRequest> m_PoliceEmergencyRequestType;

		[ReadOnly]
		public ComponentLookup<PolicePatrolRequest> m_PolicePatrolRequestData;

		[ReadOnly]
		public ComponentLookup<PoliceEmergencyRequest> m_PoliceEmergencyRequestData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PoliceCar> m_PoliceCarData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PoliceStation> m_PoliceStationData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

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
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<PolicePatrolRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PolicePatrolRequest>(ref m_PolicePatrolRequestType);
			NativeArray<PoliceEmergencyRequest> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PoliceEmergencyRequest>(ref m_PoliceEmergencyRequestType);
			PolicePatrolRequest policePatrolRequest = default(PolicePatrolRequest);
			PoliceEmergencyRequest policeEmergencyRequest = default(PoliceEmergencyRequest);
			Game.Vehicles.PoliceCar policeCar = default(Game.Vehicles.PoliceCar);
			Owner owner2 = default(Owner);
			Game.Buildings.PoliceStation policeStation = default(Game.Buildings.PoliceStation);
			Transform transform = default(Transform);
			Transform transform2 = default(Transform);
			AccidentSite accidentSite = default(AccidentSite);
			DynamicBuffer<TargetElement> val2 = default(DynamicBuffer<TargetElement>);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				Entity val;
				PolicePurpose policePurpose;
				if (m_PolicePatrolRequestData.TryGetComponent(owner, ref policePatrolRequest))
				{
					val = policePatrolRequest.m_Target;
					policePurpose = PolicePurpose.Patrol | PolicePurpose.Emergency | PolicePurpose.Intelligence;
				}
				else
				{
					if (!m_PoliceEmergencyRequestData.TryGetComponent(owner, ref policeEmergencyRequest))
					{
						continue;
					}
					val = policeEmergencyRequest.m_Site;
					policePurpose = policeEmergencyRequest.m_Purpose;
				}
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				Entity service = Entity.Null;
				if (m_PoliceCarData.TryGetComponent(val, ref policeCar))
				{
					policePurpose &= policeCar.m_PurposeMask;
					if (targetSeeker.m_Owner.TryGetComponent(val, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!m_PoliceStationData.TryGetComponent(val, ref policeStation))
					{
						continue;
					}
					policePurpose &= policeStation.m_PurposeMask;
					service = val;
				}
				if ((policePurpose & PolicePurpose.Patrol) != 0)
				{
					for (int j = 0; j < nativeArray3.Length; j++)
					{
						if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
						{
							continue;
						}
						PolicePatrolRequest policePatrolRequest2 = nativeArray3[j];
						Entity district = Entity.Null;
						if (m_CurrentDistrictData.HasComponent(policePatrolRequest2.m_Target))
						{
							district = m_CurrentDistrictData[policePatrolRequest2.m_Target].m_District;
						}
						if (!AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
						{
							continue;
						}
						float cost = ((Random)(ref random)).NextFloat(30f);
						targetSeeker.FindTargets(nativeArray[j], policePatrolRequest2.m_Target, cost, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
						if (targetSeeker.m_Transform.TryGetComponent(policePatrolRequest2.m_Target, ref transform) && (targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) != 0 && (targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
						{
							Entity lane = Entity.Null;
							float curvePos = 0f;
							float distance = float.MaxValue;
							targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(transform.m_Position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
							if (lane != Entity.Null)
							{
								targetSeeker.m_Buffer.Enqueue(new PathTarget(nativeArray[j], lane, curvePos, 0f));
							}
						}
					}
				}
				if ((policePurpose & (PolicePurpose.Emergency | PolicePurpose.Intelligence)) == 0)
				{
					continue;
				}
				targetSeeker.m_SetupQueueTarget.m_Methods &= PathMethod.Road;
				targetSeeker.m_SetupQueueTarget.m_RoadTypes &= RoadTypes.Car;
				for (int k = 0; k < nativeArray4.Length; k++)
				{
					if ((nativeArray2[k].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					PoliceEmergencyRequest policeEmergencyRequest2 = nativeArray4[k];
					if ((policePurpose & policeEmergencyRequest2.m_Purpose) == 0)
					{
						continue;
					}
					Entity district2 = Entity.Null;
					if (m_CurrentDistrictData.HasComponent(policeEmergencyRequest2.m_Target))
					{
						district2 = m_CurrentDistrictData[policeEmergencyRequest2.m_Target].m_District;
					}
					else if (targetSeeker.m_Transform.TryGetComponent(policeEmergencyRequest2.m_Target, ref transform2))
					{
						DistrictIterator districtIterator = new DistrictIterator
						{
							m_Position = ((float3)(ref transform2.m_Position)).xz,
							m_DistrictData = m_DistrictData,
							m_Nodes = targetSeeker.m_AreaNode,
							m_Triangles = targetSeeker.m_AreaTriangle
						};
						m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
						district2 = districtIterator.m_Result;
					}
					if (!AreaUtils.CheckServiceDistrict(district2, service, m_ServiceDistricts))
					{
						continue;
					}
					if (m_AccidentSiteData.TryGetComponent(policeEmergencyRequest2.m_Site, ref accidentSite))
					{
						if (!m_TargetElements.TryGetBuffer(accidentSite.m_Event, ref val2))
						{
							continue;
						}
						bool allowAccessRestriction = true;
						CheckTarget(nativeArray[k], policeEmergencyRequest2.m_Site, accidentSite, ref targetSeeker, ref allowAccessRestriction);
						for (int l = 0; l < val2.Length; l++)
						{
							Entity entity2 = val2[l].m_Entity;
							if (entity2 != policeEmergencyRequest2.m_Site)
							{
								CheckTarget(nativeArray[k], entity2, accidentSite, ref targetSeeker, ref allowAccessRestriction);
							}
						}
					}
					else
					{
						targetSeeker.FindTargets(nativeArray[k], policeEmergencyRequest2.m_Target, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
					}
				}
			}
		}

		private void CheckTarget(Entity target, Entity entity, AccidentSite accidentSite, ref PathfindTargetSeeker<PathfindSetupBuffer> targetSeeker, ref bool allowAccessRestriction)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			if ((accidentSite.m_Flags & AccidentSiteFlags.TrafficAccident) == 0 || m_CreatureData.HasComponent(entity) || m_VehicleData.HasComponent(entity))
			{
				int num = targetSeeker.FindTargets(target, entity, 0f, EdgeFlags.DefaultMask, allowAccessRestriction, navigationEnd: false);
				allowAccessRestriction &= num == 0;
				Entity val = entity;
				if (targetSeeker.m_CurrentTransport.HasComponent(val))
				{
					val = targetSeeker.m_CurrentTransport[val].m_CurrentTransport;
				}
				else if (targetSeeker.m_CurrentBuilding.HasComponent(val))
				{
					val = targetSeeker.m_CurrentBuilding[val].m_CurrentBuilding;
				}
				if (targetSeeker.m_Transform.HasComponent(val))
				{
					float3 position = targetSeeker.m_Transform[val].m_Position;
					float num2 = 30f;
					CommonPathfindSetup.TargetIterator targetIterator = new CommonPathfindSetup.TargetIterator
					{
						m_Entity = target,
						m_Bounds = new Bounds3(position - num2, position + num2),
						m_Position = position,
						m_MaxDistance = num2,
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

	private EntityQuery m_PolicePatrolQuery;

	private EntityQuery m_CrimeProducerQuery;

	private EntityQuery m_PrisonerTransportQuery;

	private EntityQuery m_PrisonerTransportRequestQuery;

	private EntityQuery m_PoliceRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<PrisonerTransportRequest> m_PrisonerTransportRequestType;

	private ComponentTypeHandle<PolicePatrolRequest> m_PolicePatrolRequestType;

	private ComponentTypeHandle<PoliceEmergencyRequest> m_PoliceEmergencyRequestType;

	private ComponentTypeHandle<Game.Buildings.PoliceStation> m_PoliceStationType;

	private ComponentTypeHandle<CrimeProducer> m_CrimeProducerType;

	private ComponentTypeHandle<Game.Buildings.Prison> m_PrisonType;

	private ComponentTypeHandle<Game.Vehicles.PoliceCar> m_PoliceCarType;

	private ComponentTypeHandle<Helicopter> m_HelicopterType;

	private ComponentTypeHandle<Game.Vehicles.PublicTransport> m_PublicTransportType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private BufferTypeHandle<Passenger> m_PassengerType;

	private BufferTypeHandle<Renter> m_RenterType;

	private BufferTypeHandle<Employee> m_EmployeeType;

	private ComponentLookup<PathInformation> m_PathInformationData;

	private ComponentLookup<PolicePatrolRequest> m_PolicePatrolRequestData;

	private ComponentLookup<PoliceEmergencyRequest> m_PoliceEmergencyRequestData;

	private ComponentLookup<PrisonerTransportRequest> m_PrisonerTransportRequestData;

	private ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnections;

	private ComponentLookup<Composition> m_CompositionData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<District> m_DistrictData;

	private ComponentLookup<Game.Buildings.PoliceStation> m_PoliceStationData;

	private ComponentLookup<Creature> m_CreatureData;

	private ComponentLookup<Vehicle> m_VehicleData;

	private ComponentLookup<Game.Vehicles.PoliceCar> m_PoliceCarData;

	private ComponentLookup<Game.Vehicles.PublicTransport> m_PublicTransportData;

	private ComponentLookup<AccidentSite> m_AccidentSiteData;

	private ComponentLookup<NetCompositionData> m_NetCompositionData;

	private ComponentLookup<Game.City.City> m_CityData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private BufferLookup<TargetElement> m_TargetElements;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private CitySystem m_CitySystem;

	public PolicePathfindSetup(PathfindSetupSystem system)
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
		//IL_018c: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.PoliceStation>(),
			ComponentType.ReadOnly<Game.Vehicles.PoliceCar>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_PolicePatrolQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_CrimeProducerQuery = system.GetSetupQuery(ComponentType.ReadOnly<CrimeProducer>(), ComponentType.Exclude<PropertyOnMarket>(), ComponentType.Exclude<Deleted>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Temp>());
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.Prison>(),
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
		m_PrisonerTransportQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_PrisonerTransportRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<PrisonerTransportRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PolicePatrolRequest>(),
			ComponentType.ReadOnly<PoliceEmergencyRequest>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.Exclude<Dispatched>(),
			ComponentType.Exclude<PathInformation>()
		};
		array3[0] = val;
		m_PoliceRequestQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array3);
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_PrisonerTransportRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrisonerTransportRequest>(true);
		m_PolicePatrolRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<PolicePatrolRequest>(true);
		m_PoliceEmergencyRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<PoliceEmergencyRequest>(true);
		m_PoliceStationType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.PoliceStation>(true);
		m_CrimeProducerType = ((ComponentSystemBase)system).GetComponentTypeHandle<CrimeProducer>(true);
		m_PrisonType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.Prison>(true);
		m_PoliceCarType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.PoliceCar>(true);
		m_HelicopterType = ((ComponentSystemBase)system).GetComponentTypeHandle<Helicopter>(true);
		m_PublicTransportType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.PublicTransport>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_PassengerType = ((ComponentSystemBase)system).GetBufferTypeHandle<Passenger>(true);
		m_RenterType = ((ComponentSystemBase)system).GetBufferTypeHandle<Renter>(true);
		m_EmployeeType = ((ComponentSystemBase)system).GetBufferTypeHandle<Employee>(true);
		m_PathInformationData = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_PolicePatrolRequestData = ((SystemBase)system).GetComponentLookup<PolicePatrolRequest>(true);
		m_PoliceEmergencyRequestData = ((SystemBase)system).GetComponentLookup<PoliceEmergencyRequest>(true);
		m_PrisonerTransportRequestData = ((SystemBase)system).GetComponentLookup<PrisonerTransportRequest>(true);
		m_OutsideConnections = ((SystemBase)system).GetComponentLookup<Game.Objects.OutsideConnection>(true);
		m_CompositionData = ((SystemBase)system).GetComponentLookup<Composition>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_DistrictData = ((SystemBase)system).GetComponentLookup<District>(true);
		m_PoliceStationData = ((SystemBase)system).GetComponentLookup<Game.Buildings.PoliceStation>(true);
		m_CreatureData = ((SystemBase)system).GetComponentLookup<Creature>(true);
		m_VehicleData = ((SystemBase)system).GetComponentLookup<Vehicle>(true);
		m_PoliceCarData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.PoliceCar>(true);
		m_PublicTransportData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.PublicTransport>(true);
		m_AccidentSiteData = ((SystemBase)system).GetComponentLookup<AccidentSite>(true);
		m_NetCompositionData = ((SystemBase)system).GetComponentLookup<NetCompositionData>(true);
		m_CityData = ((SystemBase)system).GetComponentLookup<Game.City.City>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_TargetElements = ((SystemBase)system).GetBufferLookup<TargetElement>(true);
		m_AreaSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_CitySystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<CitySystem>();
	}

	public JobHandle SetupPolicePatrols(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PoliceStationType.Update((SystemBase)(object)system);
		m_PoliceCarType.Update((SystemBase)(object)system);
		m_HelicopterType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PassengerType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_PoliceEmergencyRequestData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_OutsideConnections.Update((SystemBase)(object)system);
		m_CityData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupPolicePatrolsJob>(new SetupPolicePatrolsJob
		{
			m_EntityType = m_EntityType,
			m_PoliceStationType = m_PoliceStationType,
			m_PoliceCarType = m_PoliceCarType,
			m_HelicopterType = m_HelicopterType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PassengerType = m_PassengerType,
			m_PathInformationData = m_PathInformationData,
			m_PoliceEmergencyRequestData = m_PoliceEmergencyRequestData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_OutsideConnections = m_OutsideConnections,
			m_CityData = m_CityData,
			m_City = m_CitySystem.City,
			m_SetupData = setupData
		}, m_PolicePatrolQuery, inputDeps);
	}

	public JobHandle SetupCrimeProducer(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_RenterType.Update((SystemBase)(object)system);
		m_EmployeeType.Update((SystemBase)(object)system);
		m_CrimeProducerType.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupCrimeProducersJob>(new SetupCrimeProducersJob
		{
			m_EntityType = m_EntityType,
			m_CrimeProducerType = m_CrimeProducerType,
			m_RenterType = m_RenterType,
			m_EmployeeType = m_EmployeeType,
			m_SetupData = setupData
		}, m_CrimeProducerQuery, inputDeps);
	}

	public JobHandle SetupPrisonerTransport(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_PrisonType.Update((SystemBase)(object)system);
		m_PublicTransportType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupPrisonerTransportJob>(new SetupPrisonerTransportJob
		{
			m_EntityType = m_EntityType,
			m_PrisonType = m_PrisonType,
			m_PublicTransportType = m_PublicTransportType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_PrisonerTransportQuery, inputDeps);
	}

	public JobHandle SetupPrisonerTransportRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
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
		m_PrisonerTransportRequestType.Update((SystemBase)(object)system);
		m_PrisonerTransportRequestData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_PublicTransportData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<PrisonerTransportRequestsJob>(new PrisonerTransportRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_PrisonerTransportRequestType = m_PrisonerTransportRequestType,
			m_PrisonerTransportRequestData = m_PrisonerTransportRequestData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_PublicTransportData = m_PublicTransportData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_PrisonerTransportRequestQuery, inputDeps);
	}

	public JobHandle SetupPoliceRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_PolicePatrolRequestType.Update((SystemBase)(object)system);
		m_PoliceEmergencyRequestType.Update((SystemBase)(object)system);
		m_PolicePatrolRequestData.Update((SystemBase)(object)system);
		m_PoliceEmergencyRequestData.Update((SystemBase)(object)system);
		m_CompositionData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_DistrictData.Update((SystemBase)(object)system);
		m_CreatureData.Update((SystemBase)(object)system);
		m_VehicleData.Update((SystemBase)(object)system);
		m_PoliceCarData.Update((SystemBase)(object)system);
		m_PoliceStationData.Update((SystemBase)(object)system);
		m_AccidentSiteData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_TargetElements.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = JobChunkExtensions.ScheduleParallel<PoliceRequestsJob>(new PoliceRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_PolicePatrolRequestType = m_PolicePatrolRequestType,
			m_PoliceEmergencyRequestType = m_PoliceEmergencyRequestType,
			m_PolicePatrolRequestData = m_PolicePatrolRequestData,
			m_PoliceEmergencyRequestData = m_PoliceEmergencyRequestData,
			m_CompositionData = m_CompositionData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_DistrictData = m_DistrictData,
			m_CreatureData = m_CreatureData,
			m_VehicleData = m_VehicleData,
			m_PoliceCarData = m_PoliceCarData,
			m_PoliceStationData = m_PoliceStationData,
			m_AccidentSiteData = m_AccidentSiteData,
			m_NetCompositionData = m_NetCompositionData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_TargetElements = m_TargetElements,
			m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_NetTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_SetupData = setupData
		}, m_PoliceRequestQuery, JobHandle.CombineDependencies(inputDeps, dependencies, dependencies2));
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}
}
