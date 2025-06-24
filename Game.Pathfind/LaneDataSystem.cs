using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class LaneDataSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateLaneDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> m_MasterLaneType;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LaneObject> m_LaneObjectType;

		public ComponentTypeHandle<Game.Net.CarLane> m_CarLaneType;

		public ComponentTypeHandle<Game.Net.PedestrianLane> m_PedestrianLaneType;

		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLaneType;

		public ComponentTypeHandle<Game.Net.ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Gate> m_GateData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Moving> m_MovingData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidenteData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_PrefabCarLaneData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public BufferLookup<LaneObject> m_LaneObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

		[ReadOnly]
		public Entity m_City;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
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
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Lane> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_LaneType);
			NativeArray<Game.Net.CarLane> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.CarLane>(ref m_CarLaneType);
			NativeArray<Game.Net.PedestrianLane> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.PedestrianLane>(ref m_PedestrianLaneType);
			NativeArray<Game.Net.TrackLane> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.TrackLane>(ref m_TrackLaneType);
			NativeArray<Game.Net.ConnectionLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.ConnectionLane>(ref m_ConnectionLaneType);
			NativeArray<Owner> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			bool isEdgeLane = ((ArchetypeChunk)(ref chunk)).Has<EdgeLane>(ref m_EdgeLaneType);
			bool allowExit;
			if (nativeArray2.Length != 0)
			{
				NativeArray<MasterLane> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<MasterLane>(ref m_MasterLaneType);
				NativeArray<SlaveLane> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<SlaveLane>(ref m_SlaveLaneType);
				NativeArray<PrefabRef> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				Game.City.City city = default(Game.City.City);
				if (m_City != Entity.Null)
				{
					city = m_CityData[m_City];
				}
				if (nativeArray7.Length != 0)
				{
					Bounds1 val2 = default(Bounds1);
					for (int i = 0; i < nativeArray2.Length; i++)
					{
						Game.Net.CarLane carLane = nativeArray2[i];
						carLane.m_AccessRestriction = Entity.Null;
						carLane.m_Flags &= ~(Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.AllowEnter);
						carLane.m_SpeedLimit = carLane.m_DefaultSpeedLimit;
						carLane.m_BlockageStart = byte.MaxValue;
						carLane.m_BlockageEnd = 0;
						carLane.m_CautionStart = byte.MaxValue;
						carLane.m_CautionEnd = 0;
						if (nativeArray6.Length != 0)
						{
							MasterLane masterLane = nativeArray7[i];
							Owner owner = nativeArray6[i];
							PrefabRef prefabRef = nativeArray9[i];
							DynamicBuffer<Game.Net.SubLane> val = m_SubLanes[owner.m_Owner];
							((Bounds1)(ref val2))._002Ector(1f, 0f);
							int minIndex = masterLane.m_MinIndex;
							int num = math.min((int)masterLane.m_MaxIndex, val.Length - 1);
							bool flag = true;
							bool isSideConnection = (carLane.m_Flags & Game.Net.CarLaneFlags.SideConnection) != 0;
							for (int j = minIndex; j <= num; j++)
							{
								Entity subLane = val[j].m_SubLane;
								if (!m_LaneObjects.HasBuffer(subLane))
								{
									continue;
								}
								DynamicBuffer<LaneObject> laneObjects = m_LaneObjects[subLane];
								if (flag)
								{
									val2 = CheckBlockage(laneObjects, out allowExit, out var isSecured);
									flag = false;
									if (isSecured)
									{
										carLane.m_Flags |= Game.Net.CarLaneFlags.IsSecured;
									}
								}
								else
								{
									val2 &= CheckBlockage(laneObjects, out allowExit, out var isSecured2);
									if (isSecured2)
									{
										carLane.m_Flags |= Game.Net.CarLaneFlags.IsSecured;
									}
								}
							}
							CarLaneData carLaneData = m_PrefabCarLaneData[prefabRef.m_Prefab];
							Game.Prefabs.BuildingFlags flag2 = (((carLaneData.m_RoadTypes & (RoadTypes.Car | RoadTypes.Helicopter | RoadTypes.Airplane)) != RoadTypes.None) ? Game.Prefabs.BuildingFlags.RestrictedCar : ((Game.Prefabs.BuildingFlags)0u));
							carLane.m_AccessRestriction = GetAccessRestriction(owner, flag2, isEdgeLane, isSideConnection, nativeArray[i], out var allowEnter, out allowExit);
							if (allowEnter)
							{
								carLane.m_Flags |= Game.Net.CarLaneFlags.AllowEnter;
							}
							AddOptionData(ref carLane, city);
							AddOptionData(ref carLane, owner, carLaneData);
							AddBlockageData(ref carLane, val2, addCaution: false);
						}
						nativeArray2[i] = carLane;
					}
				}
				else
				{
					BufferAccessor<LaneObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LaneObject>(ref m_LaneObjectType);
					for (int k = 0; k < nativeArray2.Length; k++)
					{
						Game.Net.CarLane carLane2 = nativeArray2[k];
						carLane2.m_AccessRestriction = Entity.Null;
						carLane2.m_Flags &= ~(Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.AllowEnter);
						carLane2.m_SpeedLimit = carLane2.m_DefaultSpeedLimit;
						carLane2.m_BlockageStart = byte.MaxValue;
						carLane2.m_BlockageEnd = 0;
						carLane2.m_CautionStart = byte.MaxValue;
						carLane2.m_CautionEnd = 0;
						AddOptionData(ref carLane2, city);
						if (nativeArray6.Length != 0)
						{
							Owner owner2 = nativeArray6[k];
							PrefabRef prefabRef2 = nativeArray9[k];
							CarLaneData carLaneData2 = m_PrefabCarLaneData[prefabRef2.m_Prefab];
							Game.Prefabs.BuildingFlags flag3 = (((carLaneData2.m_RoadTypes & (RoadTypes.Car | RoadTypes.Helicopter | RoadTypes.Airplane)) != RoadTypes.None) ? Game.Prefabs.BuildingFlags.RestrictedCar : ((Game.Prefabs.BuildingFlags)0u));
							bool isSideConnection2 = (carLane2.m_Flags & Game.Net.CarLaneFlags.SideConnection) != 0;
							carLane2.m_AccessRestriction = GetAccessRestriction(owner2, flag3, isEdgeLane, isSideConnection2, nativeArray[k], out var allowEnter2, out allowExit);
							if (allowEnter2)
							{
								carLane2.m_Flags |= Game.Net.CarLaneFlags.AllowEnter;
							}
							AddOptionData(ref carLane2, owner2, carLaneData2);
						}
						if (bufferAccessor.Length != 0)
						{
							DynamicBuffer<LaneObject> laneObjects2 = bufferAccessor[k];
							bool isEmergency;
							bool isSecured3;
							Bounds1 val3 = CheckBlockage(laneObjects2, out isEmergency, out isSecured3);
							bool flag4 = isEmergency;
							if (val3.min <= val3.max && !flag4)
							{
								flag4 = nativeArray8.Length == 0 || (nativeArray8[k].m_Flags & (SlaveLaneFlags.StartingLane | SlaveLaneFlags.EndingLane)) != (SlaveLaneFlags.StartingLane | SlaveLaneFlags.EndingLane);
							}
							AddBlockageData(ref carLane2, val3, flag4);
							if (isSecured3)
							{
								carLane2.m_Flags |= Game.Net.CarLaneFlags.IsSecured;
							}
						}
						nativeArray2[k] = carLane2;
					}
				}
			}
			if (nativeArray3.Length != 0)
			{
				for (int l = 0; l < nativeArray3.Length; l++)
				{
					Game.Net.PedestrianLane pedestrianLane = nativeArray3[l];
					pedestrianLane.m_AccessRestriction = Entity.Null;
					pedestrianLane.m_Flags &= ~(PedestrianLaneFlags.AllowEnter | PedestrianLaneFlags.ForbidTransitTraffic | PedestrianLaneFlags.AllowExit);
					if (nativeArray6.Length != 0)
					{
						Owner owner3 = nativeArray6[l];
						if (m_BorderDistrictData.HasComponent(owner3.m_Owner))
						{
							BorderDistrict borderDistrict = m_BorderDistrictData[owner3.m_Owner];
							PedestrianLaneFlags pedestrianLaneFlags = (PedestrianLaneFlags)0;
							PedestrianLaneFlags pedestrianLaneFlags2 = (PedestrianLaneFlags)0;
							if (m_DistrictData.HasComponent(borderDistrict.m_Left))
							{
								if (AreaUtils.CheckOption(m_DistrictData[borderDistrict.m_Left], DistrictOption.ForbidTransitTraffic))
								{
									pedestrianLaneFlags |= PedestrianLaneFlags.ForbidTransitTraffic;
								}
								else
								{
									pedestrianLaneFlags2 |= PedestrianLaneFlags.ForbidTransitTraffic;
								}
							}
							if (m_DistrictData.HasComponent(borderDistrict.m_Right))
							{
								if (AreaUtils.CheckOption(m_DistrictData[borderDistrict.m_Right], DistrictOption.ForbidTransitTraffic))
								{
									pedestrianLaneFlags |= PedestrianLaneFlags.ForbidTransitTraffic;
								}
								else
								{
									pedestrianLaneFlags2 |= PedestrianLaneFlags.ForbidTransitTraffic;
								}
							}
							pedestrianLane.m_Flags |= pedestrianLaneFlags & ~pedestrianLaneFlags2;
						}
						Game.Prefabs.BuildingFlags flag5 = (((pedestrianLane.m_Flags & PedestrianLaneFlags.OnWater) == 0) ? Game.Prefabs.BuildingFlags.RestrictedPedestrian : ((Game.Prefabs.BuildingFlags)0u));
						bool isSideConnection3 = (pedestrianLane.m_Flags & PedestrianLaneFlags.SideConnection) != 0;
						pedestrianLane.m_AccessRestriction = GetAccessRestriction(owner3, flag5, isEdgeLane, isSideConnection3, nativeArray[l], out var allowEnter3, out var allowExit2);
						if (allowEnter3)
						{
							pedestrianLane.m_Flags |= PedestrianLaneFlags.AllowEnter;
						}
						if (allowExit2)
						{
							pedestrianLane.m_Flags |= PedestrianLaneFlags.AllowExit;
						}
					}
					nativeArray3[l] = pedestrianLane;
				}
			}
			bool allowExit3;
			if (nativeArray4.Length != 0)
			{
				for (int m = 0; m < nativeArray4.Length; m++)
				{
					Game.Net.TrackLane trackLane = nativeArray4[m];
					trackLane.m_AccessRestriction = Entity.Null;
					if (nativeArray6.Length != 0)
					{
						Owner owner4 = nativeArray6[m];
						Game.Prefabs.BuildingFlags flag6 = (((trackLane.m_Flags & TrackLaneFlags.Station) != 0) ? Game.Prefabs.BuildingFlags.RestrictedTrack : ((Game.Prefabs.BuildingFlags)0u));
						trackLane.m_AccessRestriction = GetAccessRestriction(owner4, flag6, isEdgeLane, isSideConnection: false, nativeArray[m], out allowExit, out allowExit3);
					}
					nativeArray4[m] = trackLane;
				}
			}
			if (nativeArray5.Length == 0)
			{
				return;
			}
			for (int n = 0; n < nativeArray5.Length; n++)
			{
				Game.Net.ConnectionLane connectionLane = nativeArray5[n];
				connectionLane.m_AccessRestriction = Entity.Null;
				connectionLane.m_Flags &= ~(ConnectionLaneFlags.AllowEnter | ConnectionLaneFlags.AllowExit);
				if (nativeArray6.Length != 0)
				{
					Owner owner5 = nativeArray6[n];
					if ((connectionLane.m_Flags & ConnectionLaneFlags.Pedestrian) != 0)
					{
						connectionLane.m_AccessRestriction = GetAccessRestriction(owner5, Game.Prefabs.BuildingFlags.RestrictedPedestrian, isEdgeLane, isSideConnection: false, nativeArray[n], out var allowEnter4, out var allowExit4);
						if (allowEnter4)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowEnter;
						}
						if (allowExit4)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowExit;
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Road) != 0)
					{
						Game.Prefabs.BuildingFlags flag7 = (((connectionLane.m_RoadTypes & (RoadTypes.Car | RoadTypes.Helicopter | RoadTypes.Airplane)) != RoadTypes.None) ? Game.Prefabs.BuildingFlags.RestrictedCar : ((Game.Prefabs.BuildingFlags)0u));
						connectionLane.m_AccessRestriction = GetAccessRestriction(owner5, flag7, isEdgeLane, isSideConnection: false, nativeArray[n], out var allowEnter5, out allowExit3);
						if (allowEnter5)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowEnter;
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Parking) != 0)
					{
						connectionLane.m_AccessRestriction = GetAccessRestriction(owner5, Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar, isEdgeLane, isSideConnection: false, nativeArray[n], out var allowEnter6, out var allowExit5);
						if (allowEnter6)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowEnter;
						}
						if (allowExit5)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowExit;
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.AllowCargo) != 0)
					{
						Game.Prefabs.BuildingFlags flag8 = Game.Prefabs.BuildingFlags.RestrictedCar;
						connectionLane.m_AccessRestriction = GetAccessRestriction(owner5, flag8, isEdgeLane, isSideConnection: false, nativeArray[n], out var allowEnter7, out allowExit3);
						if (allowEnter7)
						{
							connectionLane.m_Flags |= ConnectionLaneFlags.AllowEnter;
						}
					}
					else if ((connectionLane.m_Flags & ConnectionLaneFlags.Track) != 0)
					{
						connectionLane.m_AccessRestriction = GetAccessRestriction(owner5, Game.Prefabs.BuildingFlags.RestrictedTrack, isEdgeLane, isSideConnection: false, nativeArray[n], out allowExit3, out allowExit);
					}
				}
				nativeArray5[n] = connectionLane;
			}
		}

		private bool IsSecured(InvolvedInAccident involvedInAccident)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Entity val = FindAccidentSite(involvedInAccident.m_Event);
			if (val != Entity.Null)
			{
				return (m_AccidentSiteData[val].m_Flags & AccidentSiteFlags.Secured) != 0;
			}
			return true;
		}

		private Entity FindAccidentSite(Entity _event)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (m_TargetElements.HasBuffer(_event))
			{
				DynamicBuffer<TargetElement> val = m_TargetElements[_event];
				for (int i = 0; i < val.Length; i++)
				{
					Entity entity = val[i].m_Entity;
					if (m_AccidentSiteData.HasComponent(entity))
					{
						return entity;
					}
				}
			}
			return Entity.Null;
		}

		private Entity GetAccessRestriction(Owner owner, Game.Prefabs.BuildingFlags flag, bool isEdgeLane, bool isSideConnection, Lane lane, out bool allowEnter, out bool allowExit)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			allowEnter = false;
			allowExit = false;
			isSideConnection |= !isEdgeLane && m_ConnectedNodes.HasBuffer(owner.m_Owner);
			if (isSideConnection)
			{
				DynamicBuffer<ConnectedEdge> val4 = default(DynamicBuffer<ConnectedEdge>);
				DynamicBuffer<ConnectedNode> val = default(DynamicBuffer<ConnectedNode>);
				if (m_ConnectedNodes.TryGetBuffer(owner.m_Owner, ref val))
				{
					DynamicBuffer<Game.Net.SubLane> val3 = default(DynamicBuffer<Game.Net.SubLane>);
					for (int i = 0; i < val.Length; i++)
					{
						ConnectedNode connectedNode = val[i];
						DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[connectedNode.m_Node];
						ConnectedEdge connectedEdge;
						for (int j = 0; j < val2.Length; j++)
						{
							connectedEdge = val2[j];
							if (connectedEdge.m_Edge == owner.m_Owner || !m_SubLanes.TryGetBuffer(connectedEdge.m_Edge, ref val3))
							{
								continue;
							}
							int num = 0;
							while (num < val3.Length)
							{
								Game.Net.SubLane subLane = val3[num];
								Lane lane2 = m_LaneData[subLane.m_SubLane];
								if (!lane2.m_StartNode.Equals(lane.m_StartNode) && !lane2.m_EndNode.Equals(lane.m_StartNode) && !lane2.m_StartNode.Equals(lane.m_EndNode) && !lane2.m_EndNode.Equals(lane.m_EndNode))
								{
									num++;
									continue;
								}
								goto IL_0128;
							}
						}
						continue;
						IL_0128:
						owner.m_Owner = connectedEdge.m_Edge;
						break;
					}
				}
				else if (m_ConnectedEdges.TryGetBuffer(owner.m_Owner, ref val4))
				{
					DynamicBuffer<Game.Net.SubLane> val6 = default(DynamicBuffer<Game.Net.SubLane>);
					for (int k = 0; k < val4.Length; k++)
					{
						ConnectedEdge connectedEdge2 = val4[k];
						Game.Net.Edge edge = m_EdgeData[connectedEdge2.m_Edge];
						if (edge.m_Start != owner.m_Owner && edge.m_End != owner.m_Owner)
						{
							continue;
						}
						val = m_ConnectedNodes[connectedEdge2.m_Edge];
						ConnectedEdge connectedEdge3;
						for (int l = 0; l < val.Length; l++)
						{
							ConnectedNode connectedNode2 = val[l];
							DynamicBuffer<ConnectedEdge> val5 = m_ConnectedEdges[connectedNode2.m_Node];
							for (int m = 0; m < val5.Length; m++)
							{
								connectedEdge3 = val5[m];
								if (connectedEdge3.m_Edge == connectedEdge2.m_Edge || !m_SubLanes.TryGetBuffer(connectedEdge3.m_Edge, ref val6))
								{
									continue;
								}
								int num2 = 0;
								while (num2 < val6.Length)
								{
									Game.Net.SubLane subLane2 = val6[num2];
									Lane lane3 = m_LaneData[subLane2.m_SubLane];
									if (!lane3.m_StartNode.Equals(lane.m_StartNode) && !lane3.m_EndNode.Equals(lane.m_StartNode) && !lane3.m_StartNode.Equals(lane.m_EndNode) && !lane3.m_EndNode.Equals(lane.m_EndNode))
									{
										num2++;
										continue;
									}
									goto IL_02df;
								}
							}
						}
						continue;
						IL_02df:
						owner.m_Owner = connectedEdge3.m_Edge;
						break;
					}
				}
			}
			PrefabRef prefabRef = default(PrefabRef);
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_PrefabRefData.TryGetComponent(owner.m_Owner, ref prefabRef) && m_PrefabGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData) && (netGeometryData.m_Flags & Game.Net.GeometryFlags.SubOwner) != 0)
			{
				return Entity.Null;
			}
			Entity topLevelOwner = GetTopLevelOwner(owner.m_Owner);
			if (m_BuildingData.HasComponent(topLevelOwner))
			{
				PrefabRef prefabRef2 = m_PrefabRefData[topLevelOwner];
				BuildingData buildingData = m_PrefabBuildingData[prefabRef2.m_Prefab];
				if (m_RoadData.HasComponent(owner.m_Owner))
				{
					buildingData.m_Flags &= ~(Game.Prefabs.BuildingFlags.RestrictedPedestrian | Game.Prefabs.BuildingFlags.RestrictedCar);
				}
				if (m_GateData.HasComponent(owner.m_Owner))
				{
					return Entity.Null;
				}
				bool flag2 = (buildingData.m_Flags & flag) != 0;
				bool flag3 = (flag & Game.Prefabs.BuildingFlags.RestrictedCar) != 0;
				bool flag4 = (flag & Game.Prefabs.BuildingFlags.RestrictedPedestrian) != 0;
				if (flag2 || flag3 || flag4)
				{
					if (!isEdgeLane && !isSideConnection && m_ConnectedEdges.HasBuffer(owner.m_Owner))
					{
						DynamicBuffer<ConnectedEdge> val7 = m_ConnectedEdges[owner.m_Owner];
						bool2 val8 = bool2.op_Implicit(false);
						DynamicBuffer<Game.Net.SubLane> val9 = default(DynamicBuffer<Game.Net.SubLane>);
						bool2 val10 = default(bool2);
						DynamicBuffer<Game.Net.SubLane> val11 = default(DynamicBuffer<Game.Net.SubLane>);
						for (int n = 0; n < val7.Length; n++)
						{
							ConnectedEdge connectedEdge4 = val7[n];
							Game.Net.Edge edge2 = m_EdgeData[connectedEdge4.m_Edge];
							if (edge2.m_Start != owner.m_Owner && edge2.m_End != owner.m_Owner)
							{
								continue;
							}
							if (m_GateData.HasComponent(connectedEdge4.m_Edge) && m_SubLanes.TryGetBuffer(connectedEdge4.m_Edge, ref val9))
							{
								for (int num3 = 0; num3 < val9.Length; num3++)
								{
									Game.Net.SubLane subLane3 = val9[num3];
									Lane lane4 = m_LaneData[subLane3.m_SubLane];
									val10.x = lane4.m_StartNode.Equals(lane.m_StartNode) || lane4.m_EndNode.Equals(lane.m_StartNode);
									val10.y = lane4.m_StartNode.Equals(lane.m_EndNode) || lane4.m_EndNode.Equals(lane.m_EndNode);
									if (math.any(val10))
									{
										return Entity.Null;
									}
								}
							}
							if (topLevelOwner == GetTopLevelOwner(connectedEdge4.m_Edge))
							{
								continue;
							}
							if (!flag3)
							{
								return Entity.Null;
							}
							if (!m_SubLanes.TryGetBuffer(connectedEdge4.m_Edge, ref val11))
							{
								continue;
							}
							for (int num4 = 0; num4 < val11.Length; num4++)
							{
								Game.Net.SubLane subLane4 = val11[num4];
								Lane lane5 = m_LaneData[subLane4.m_SubLane];
								val8.x |= lane5.m_StartNode.Equals(lane.m_StartNode) || lane5.m_EndNode.Equals(lane.m_StartNode);
								val8.y |= lane5.m_StartNode.Equals(lane.m_EndNode) || lane5.m_EndNode.Equals(lane.m_EndNode);
								if (math.all(val8))
								{
									return Entity.Null;
								}
							}
						}
					}
					allowEnter = !flag2;
					if (flag3 && flag4)
					{
						allowExit = (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RestrictedParking) == 0;
					}
					else if (flag4)
					{
						allowExit = allowEnter && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.RestrictedCar) != 0;
					}
					else
					{
						allowExit = false;
					}
					return topLevelOwner;
				}
			}
			return Entity.Null;
		}

		private Entity GetTopLevelOwner(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Entity val = entity;
			Owner owner = default(Owner);
			while (m_OwnerData.TryGetComponent(val, ref owner))
			{
				val = owner.m_Owner;
			}
			Attachment attachment = default(Attachment);
			if (m_AttachmentData.TryGetComponent(val, ref attachment) && attachment.m_Attached != Entity.Null)
			{
				val = attachment.m_Attached;
			}
			return val;
		}

		private void AddOptionData(ref Game.Net.CarLane carLane, Game.City.City city)
		{
			if ((carLane.m_Flags & Game.Net.CarLaneFlags.Highway) != 0 && CityUtils.CheckOption(city, CityOption.UnlimitedHighwaySpeed))
			{
				carLane.m_SpeedLimit = 111.111115f;
			}
		}

		private void AddOptionData(ref Game.Net.CarLane carLane, Owner owner, CarLaneData carLaneData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			if (!m_BorderDistrictData.HasComponent(owner.m_Owner))
			{
				return;
			}
			BorderDistrict borderDistrict = m_BorderDistrictData[owner.m_Owner];
			Game.Net.CarLaneFlags carLaneFlags = ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
			Game.Net.CarLaneFlags carLaneFlags2 = ~(Game.Net.CarLaneFlags.Unsafe | Game.Net.CarLaneFlags.UTurnLeft | Game.Net.CarLaneFlags.Invert | Game.Net.CarLaneFlags.SideConnection | Game.Net.CarLaneFlags.TurnLeft | Game.Net.CarLaneFlags.TurnRight | Game.Net.CarLaneFlags.LevelCrossing | Game.Net.CarLaneFlags.Twoway | Game.Net.CarLaneFlags.IsSecured | Game.Net.CarLaneFlags.Runway | Game.Net.CarLaneFlags.Yield | Game.Net.CarLaneFlags.Stop | Game.Net.CarLaneFlags.ForbidCombustionEngines | Game.Net.CarLaneFlags.ForbidTransitTraffic | Game.Net.CarLaneFlags.ForbidHeavyTraffic | Game.Net.CarLaneFlags.PublicOnly | Game.Net.CarLaneFlags.Highway | Game.Net.CarLaneFlags.UTurnRight | Game.Net.CarLaneFlags.GentleTurnLeft | Game.Net.CarLaneFlags.GentleTurnRight | Game.Net.CarLaneFlags.Forward | Game.Net.CarLaneFlags.Approach | Game.Net.CarLaneFlags.Roundabout | Game.Net.CarLaneFlags.RightLimit | Game.Net.CarLaneFlags.LeftLimit | Game.Net.CarLaneFlags.ForbidPassing | Game.Net.CarLaneFlags.RightOfWay | Game.Net.CarLaneFlags.TrafficLights | Game.Net.CarLaneFlags.ParkingLeft | Game.Net.CarLaneFlags.ParkingRight | Game.Net.CarLaneFlags.Forbidden | Game.Net.CarLaneFlags.AllowEnter);
			float2 val = float2.op_Implicit(carLane.m_SpeedLimit);
			if (m_DistrictData.HasComponent(borderDistrict.m_Left))
			{
				District district = m_DistrictData[borderDistrict.m_Left];
				DynamicBuffer<DistrictModifier> modifiers = m_DistrictModifiers[borderDistrict.m_Left];
				if ((carLaneData.m_RoadTypes & RoadTypes.Airplane) == 0)
				{
					if (AreaUtils.CheckOption(district, DistrictOption.ForbidCombustionEngines))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidCombustionEngines;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidCombustionEngines;
					}
					if (AreaUtils.CheckOption(district, DistrictOption.ForbidTransitTraffic))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidTransitTraffic;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidTransitTraffic;
					}
				}
				if ((carLaneData.m_RoadTypes & RoadTypes.Car) != RoadTypes.None && (carLane.m_Flags & Game.Net.CarLaneFlags.Highway) == 0)
				{
					if (AreaUtils.CheckOption(district, DistrictOption.ForbidHeavyTraffic))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidHeavyTraffic;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidHeavyTraffic;
					}
					AreaUtils.ApplyModifier(ref val.x, modifiers, DistrictModifierType.StreetSpeedLimit);
				}
			}
			if (m_DistrictData.HasComponent(borderDistrict.m_Right))
			{
				District district2 = m_DistrictData[borderDistrict.m_Right];
				DynamicBuffer<DistrictModifier> modifiers2 = m_DistrictModifiers[borderDistrict.m_Right];
				if ((carLaneData.m_RoadTypes & RoadTypes.Airplane) == 0)
				{
					if (AreaUtils.CheckOption(district2, DistrictOption.ForbidCombustionEngines))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidCombustionEngines;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidCombustionEngines;
					}
					if (AreaUtils.CheckOption(district2, DistrictOption.ForbidTransitTraffic))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidTransitTraffic;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidTransitTraffic;
					}
				}
				if ((carLaneData.m_RoadTypes & RoadTypes.Car) != RoadTypes.None && (carLane.m_Flags & Game.Net.CarLaneFlags.Highway) == 0)
				{
					if (AreaUtils.CheckOption(district2, DistrictOption.ForbidHeavyTraffic))
					{
						carLaneFlags |= Game.Net.CarLaneFlags.ForbidHeavyTraffic;
					}
					else
					{
						carLaneFlags2 |= Game.Net.CarLaneFlags.ForbidHeavyTraffic;
					}
					AreaUtils.ApplyModifier(ref val.y, modifiers2, DistrictModifierType.StreetSpeedLimit);
				}
			}
			carLane.m_Flags |= carLaneFlags & ~carLaneFlags2;
			if (math.cmax(val) >= carLane.m_SpeedLimit)
			{
				carLane.m_SpeedLimit = math.max(carLane.m_SpeedLimit, math.cmin(val));
			}
			else
			{
				carLane.m_SpeedLimit = math.min(carLane.m_SpeedLimit, math.cmax(val));
			}
		}

		private Bounds1 CheckBlockage(DynamicBuffer<LaneObject> laneObjects, out bool isEmergency, out bool isSecured)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(1f, 0f);
			isEmergency = false;
			isSecured = false;
			InvolvedInAccident involvedInAccident = default(InvolvedInAccident);
			Car car = default(Car);
			for (int i = 0; i < laneObjects.Length; i++)
			{
				LaneObject laneObject = laneObjects[i];
				if (!m_MovingData.HasComponent(laneObject.m_LaneObject))
				{
					val |= MathUtils.Bounds(laneObject.m_CurvePosition.x, laneObject.m_CurvePosition.y);
					if (m_InvolvedInAccidenteData.TryGetComponent(laneObject.m_LaneObject, ref involvedInAccident))
					{
						isSecured |= IsSecured(involvedInAccident);
						isEmergency = true;
					}
					else if (m_CarData.TryGetComponent(laneObject.m_LaneObject, ref car))
					{
						isEmergency |= (car.m_Flags & CarFlags.Emergency) != 0;
					}
				}
			}
			return val;
		}

		private void AddBlockageData(ref Game.Net.CarLane carLane, Bounds1 bounds, bool addCaution)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (bounds.min <= bounds.max)
			{
				carLane.m_BlockageStart = (byte)math.max(0, Mathf.FloorToInt(bounds.min * 255f));
				carLane.m_BlockageEnd = (byte)math.min(255, Mathf.CeilToInt(bounds.max * 255f));
				if (addCaution)
				{
					carLane.m_CautionStart = carLane.m_BlockageStart;
					carLane.m_CautionEnd = carLane.m_BlockageEnd;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct UpdateLaneData2Job : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				Game.Net.CarLane carLane = m_CarLaneData[val];
				DynamicBuffer<Game.Net.SubLane> val2 = m_SubLanes[owner.m_Owner];
				bool flag = false;
				for (int j = 0; j < val2.Length; j++)
				{
					Entity subLane = val2[j].m_SubLane;
					if (!(subLane != val) || !m_CarLaneData.HasComponent(subLane))
					{
						continue;
					}
					Game.Net.CarLane carLane2 = m_CarLaneData[subLane];
					if (carLane2.m_CarriagewayGroup == carLane.m_CarriagewayGroup && carLane2.m_CautionEnd >= carLane2.m_CautionStart)
					{
						if (((carLane.m_Flags ^ carLane2.m_Flags) & Game.Net.CarLaneFlags.Invert) != 0)
						{
							carLane.m_CautionStart = (byte)math.min((int)carLane.m_CautionStart, 255 - carLane2.m_CautionEnd);
							carLane.m_CautionEnd = (byte)math.max((int)carLane.m_CautionEnd, 255 - carLane2.m_CautionStart);
						}
						else
						{
							carLane.m_CautionStart = (byte)math.min((int)carLane.m_CautionStart, (int)carLane2.m_CautionStart);
							carLane.m_CautionEnd = (byte)math.max((int)carLane.m_CautionEnd, (int)carLane2.m_CautionEnd);
						}
						carLane.m_Flags |= carLane2.m_Flags & Game.Net.CarLaneFlags.IsSecured;
						flag = true;
					}
				}
				if (flag)
				{
					m_CarLaneData[val] = carLane;
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
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MasterLane> __Game_Net_MasterLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> __Game_Net_SlaveLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LaneObject> __Game_Net_LaneObject_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Net.CarLane> __Game_Net_CarLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Net.TrackLane> __Game_Net_TrackLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Gate> __Game_Net_Gate_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.City.City> __Game_City_City_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<District> __Game_Areas_District_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Moving> __Game_Objects_Moving_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AccidentSite> __Game_Events_AccidentSite_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LaneObject> __Game_Net_LaneObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<TargetElement> __Game_Events_TargetElement_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RW_ComponentLookup;

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
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_MasterLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MasterLane>(true);
			__Game_Net_SlaveLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SlaveLane>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_LaneObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LaneObject>(true);
			__Game_Net_CarLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.CarLane>(false);
			__Game_Net_PedestrianLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.PedestrianLane>(false);
			__Game_Net_TrackLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.TrackLane>(false);
			__Game_Net_ConnectionLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ConnectionLane>(false);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_Gate_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Gate>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Edge>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_City_City_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.City>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_Areas_District_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<District>(true);
			__Game_Objects_Moving_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Events_InvolvedInAccident_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(true);
			__Game_Events_AccidentSite_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AccidentSite>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_LaneObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LaneObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
			__Game_Events_TargetElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_CarLane_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(false);
		}
	}

	private CitySystem m_CitySystem;

	private EntityQuery m_LaneQuery;

	private EntityQuery m_LaneQuery2;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.TrackLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<GarageLane>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PathfindUpdated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.TrackLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<GarageLane>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.CarLane>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PathfindUpdated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.CarLane>() };
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[1] = val;
		m_LaneQuery2 = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
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
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		UpdateLaneDataJob updateLaneDataJob = new UpdateLaneDataJob
		{
			m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_MasterLaneType = InternalCompilerInterface.GetComponentTypeHandle<MasterLane>(ref __TypeHandle.__Game_Net_MasterLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjectType = InternalCompilerInterface.GetBufferTypeHandle<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GateData = InternalCompilerInterface.GetComponentLookup<Gate>(ref __TypeHandle.__Game_Net_Gate_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityData = InternalCompilerInterface.GetComponentLookup<Game.City.City>(ref __TypeHandle.__Game_City_City_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictData = InternalCompilerInterface.GetComponentLookup<District>(ref __TypeHandle.__Game_Areas_District_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidenteData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AccidentSiteData = InternalCompilerInterface.GetComponentLookup<AccidentSite>(ref __TypeHandle.__Game_Events_AccidentSite_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneObjects = InternalCompilerInterface.GetBufferLookup<LaneObject>(ref __TypeHandle.__Game_Net_LaneObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateLaneDataJob>(updateLaneDataJob, m_LaneQuery, ((SystemBase)this).Dependency);
		if (!((EntityQuery)(ref m_LaneQuery)).IsEmptyIgnoreFilter)
		{
			UpdateLaneData2Job updateLaneData2Job = new UpdateLaneData2Job
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateLaneData2Job>(updateLaneData2Job, m_LaneQuery2, ((SystemBase)this).Dependency);
		}
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
	public LaneDataSystem()
	{
	}
}
