using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Pathfind;

[CompilerGenerated]
public class LanesModifiedSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddPathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Density> m_DensityData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> m_PedestrianPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> m_CarPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> m_TrackPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> m_TransportPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_ConnectionPathfindData;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ParkingLane> m_ParkingLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentTypeHandle<GarageLane> m_GarageLaneType;

		[ReadOnly]
		public ComponentTypeHandle<LaneConnection> m_LaneConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[WriteOnly]
		public NativeArray<CreateActionData> m_Actions;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Lane> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Lane>(ref m_LaneType);
				NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Game.Net.CarLane> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.CarLane>(ref m_CarLaneType);
				NativeArray<Game.Net.TrackLane> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.TrackLane>(ref m_TrackLaneType);
				if (nativeArray5.Length != 0 && !((ArchetypeChunk)(ref val)).Has<SlaveLane>(ref m_SlaveLaneType))
				{
					NativeArray<Owner> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
					NativeArray<LaneConnection> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					((ArchetypeChunk)(ref val)).Has<EdgeLane>(ref m_EdgeLaneType);
					if (nativeArray6.Length != 0)
					{
						for (int j = 0; j < nativeArray5.Length; j++)
						{
							Lane lane = nativeArray2[j];
							Curve curve = nativeArray3[j];
							Game.Net.CarLane carLane = nativeArray5[j];
							Game.Net.TrackLane trackLaneData = nativeArray6[j];
							PrefabRef prefabRef = nativeArray4[j];
							NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
							CarLaneData carLaneData = m_CarLaneData[prefabRef.m_Prefab];
							PathfindCarData carPathfindData = m_CarPathfindData[netLaneData.m_PathfindPrefab];
							PathfindTransportData transportPathfindData = m_TransportPathfindData[netLaneData.m_PathfindPrefab];
							float num2 = 0.01f;
							if (nativeArray7.Length != 0)
							{
								Owner owner = nativeArray7[j];
								if (m_DensityData.HasComponent(owner.m_Owner))
								{
									num2 = math.max(num2, m_DensityData[owner.m_Owner].m_Density);
								}
							}
							if (nativeArray8.Length != 0)
							{
								CheckLaneConnections(ref lane, nativeArray8[j]);
							}
							CreateActionData createActionData = new CreateActionData
							{
								m_Owner = nativeArray[j],
								m_StartNode = lane.m_StartNode,
								m_MiddleNode = lane.m_MiddleNode,
								m_EndNode = lane.m_EndNode,
								m_Specification = PathUtils.GetCarDriveSpecification(curve, carLane, trackLaneData, carLaneData, carPathfindData, num2),
								m_Location = PathUtils.GetLocationSpecification(curve)
							};
							if (carLaneData.m_RoadTypes == RoadTypes.Car)
							{
								createActionData.m_SecondaryStartNode = createActionData.m_StartNode;
								createActionData.m_SecondaryEndNode = createActionData.m_EndNode;
								createActionData.m_SecondarySpecification = PathUtils.GetTaxiDriveSpecification(curve, carLane, carPathfindData, transportPathfindData, num2);
							}
							m_Actions[num++] = createActionData;
						}
						continue;
					}
					for (int k = 0; k < nativeArray5.Length; k++)
					{
						Lane lane2 = nativeArray2[k];
						Curve curve2 = nativeArray3[k];
						Game.Net.CarLane carLane2 = nativeArray5[k];
						PrefabRef prefabRef2 = nativeArray4[k];
						NetLaneData netLaneData2 = m_NetLaneData[prefabRef2.m_Prefab];
						CarLaneData carLaneData2 = m_CarLaneData[prefabRef2.m_Prefab];
						PathfindCarData carPathfindData2 = m_CarPathfindData[netLaneData2.m_PathfindPrefab];
						PathfindTransportData transportPathfindData2 = m_TransportPathfindData[netLaneData2.m_PathfindPrefab];
						float num3 = 0.01f;
						if (nativeArray7.Length != 0)
						{
							Owner owner2 = nativeArray7[k];
							if (m_DensityData.HasComponent(owner2.m_Owner))
							{
								num3 = math.max(num3, m_DensityData[owner2.m_Owner].m_Density);
							}
						}
						if (nativeArray8.Length != 0)
						{
							CheckLaneConnections(ref lane2, nativeArray8[k]);
						}
						CreateActionData createActionData2 = new CreateActionData
						{
							m_Owner = nativeArray[k],
							m_StartNode = lane2.m_StartNode,
							m_MiddleNode = lane2.m_MiddleNode,
							m_EndNode = lane2.m_EndNode,
							m_Specification = PathUtils.GetCarDriveSpecification(curve2, carLane2, carLaneData2, carPathfindData2, num3),
							m_Location = PathUtils.GetLocationSpecification(curve2)
						};
						if (carLaneData2.m_RoadTypes == RoadTypes.Car)
						{
							createActionData2.m_SecondaryStartNode = createActionData2.m_StartNode;
							createActionData2.m_SecondaryEndNode = createActionData2.m_EndNode;
							createActionData2.m_SecondarySpecification = PathUtils.GetTaxiDriveSpecification(curve2, carLane2, carPathfindData2, transportPathfindData2, num3);
						}
						m_Actions[num++] = createActionData2;
					}
					continue;
				}
				if (nativeArray6.Length != 0)
				{
					for (int l = 0; l < nativeArray6.Length; l++)
					{
						Lane lane3 = nativeArray2[l];
						Curve curveData = nativeArray3[l];
						Game.Net.TrackLane trackLaneData2 = nativeArray6[l];
						PrefabRef prefabRef3 = nativeArray4[l];
						NetLaneData netLaneData3 = m_NetLaneData[prefabRef3.m_Prefab];
						PathfindTrackData trackPathfindData = m_TrackPathfindData[netLaneData3.m_PathfindPrefab];
						CreateActionData createActionData3 = new CreateActionData
						{
							m_Owner = nativeArray[l],
							m_StartNode = lane3.m_StartNode,
							m_MiddleNode = lane3.m_MiddleNode,
							m_EndNode = lane3.m_EndNode,
							m_Specification = PathUtils.GetTrackDriveSpecification(curveData, trackLaneData2, trackPathfindData),
							m_Location = PathUtils.GetLocationSpecification(curveData)
						};
						m_Actions[num++] = createActionData3;
					}
					continue;
				}
				NativeArray<Game.Net.ParkingLane> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.ParkingLane>(ref m_ParkingLaneType);
				if (nativeArray9.Length != 0)
				{
					NativeArray<LaneConnection> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					for (int m = 0; m < nativeArray9.Length; m++)
					{
						Lane lane4 = nativeArray2[m];
						Curve curveData2 = nativeArray3[m];
						Game.Net.ParkingLane parkingLane = nativeArray9[m];
						PrefabRef prefabRef4 = nativeArray4[m];
						NetLaneData netLaneData4 = m_NetLaneData[prefabRef4.m_Prefab];
						ParkingLaneData parkingLaneData = m_ParkingLaneData[prefabRef4.m_Prefab];
						PathfindCarData carPathfindData3 = m_CarPathfindData[netLaneData4.m_PathfindPrefab];
						PathfindTransportData transportPathfindData3 = m_TransportPathfindData[netLaneData4.m_PathfindPrefab];
						if (nativeArray10.Length != 0)
						{
							CheckLaneConnections(ref lane4, nativeArray10[m]);
						}
						CreateActionData createActionData4 = new CreateActionData
						{
							m_Owner = nativeArray[m],
							m_StartNode = lane4.m_StartNode,
							m_MiddleNode = lane4.m_MiddleNode,
							m_EndNode = lane4.m_EndNode,
							m_Specification = PathUtils.GetParkingSpaceSpecification(parkingLane, parkingLaneData, carPathfindData3)
						};
						if ((parkingLane.m_Flags & ParkingLaneFlags.SecondaryStart) != 0)
						{
							createActionData4.m_SecondaryStartNode = parkingLane.m_SecondaryStartNode;
							createActionData4.m_SecondaryEndNode = createActionData4.m_EndNode;
							createActionData4.m_SecondarySpecification = PathUtils.GetParkingSpaceSpecification(parkingLane, parkingLaneData, carPathfindData3);
						}
						else
						{
							createActionData4.m_SecondaryStartNode = createActionData4.m_StartNode;
							createActionData4.m_SecondaryEndNode = createActionData4.m_EndNode;
							createActionData4.m_SecondarySpecification = PathUtils.GetTaxiAccessSpecification(parkingLane, carPathfindData3, transportPathfindData3);
						}
						createActionData4.m_Location = PathUtils.GetLocationSpecification(curveData2, parkingLane);
						m_Actions[num++] = createActionData4;
					}
					continue;
				}
				NativeArray<Game.Net.PedestrianLane> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.PedestrianLane>(ref m_PedestrianLaneType);
				if (nativeArray11.Length != 0)
				{
					NativeArray<LaneConnection> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					for (int n = 0; n < nativeArray11.Length; n++)
					{
						Lane lane5 = nativeArray2[n];
						Curve curveData3 = nativeArray3[n];
						Game.Net.PedestrianLane pedestrianLaneData = nativeArray11[n];
						PrefabRef prefabRef5 = nativeArray4[n];
						NetLaneData netLaneData5 = m_NetLaneData[prefabRef5.m_Prefab];
						PathfindPedestrianData pedestrianPathfindData = m_PedestrianPathfindData[netLaneData5.m_PathfindPrefab];
						if (nativeArray12.Length != 0)
						{
							CheckLaneConnections(ref lane5, nativeArray12[n]);
						}
						CreateActionData createActionData5 = new CreateActionData
						{
							m_Owner = nativeArray[n],
							m_StartNode = lane5.m_StartNode,
							m_MiddleNode = lane5.m_MiddleNode,
							m_EndNode = lane5.m_EndNode,
							m_Specification = PathUtils.GetSpecification(curveData3, pedestrianLaneData, pedestrianPathfindData),
							m_Location = PathUtils.GetLocationSpecification(curveData3)
						};
						m_Actions[num++] = createActionData5;
					}
					continue;
				}
				NativeArray<Game.Net.ConnectionLane> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.ConnectionLane>(ref m_ConnectionLaneType);
				if (nativeArray13.Length == 0)
				{
					continue;
				}
				NativeArray<GarageLane> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<GarageLane>(ref m_GarageLaneType);
				NativeArray<Game.Net.OutsideConnection> nativeArray15 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.OutsideConnection>(ref m_OutsideConnectionType);
				for (int num4 = 0; num4 < nativeArray13.Length; num4++)
				{
					Lane lane6 = nativeArray2[num4];
					Curve curveData4 = nativeArray3[num4];
					Game.Net.ConnectionLane connectionLaneData = nativeArray13[num4];
					PrefabRef prefabRef6 = nativeArray4[num4];
					NetLaneData netLaneData6 = m_NetLaneData[prefabRef6.m_Prefab];
					PathfindConnectionData connectionPathfindData = m_ConnectionPathfindData[netLaneData6.m_PathfindPrefab];
					GarageLane garageLane = default(GarageLane);
					if (nativeArray14.Length != 0)
					{
						garageLane = nativeArray14[num4];
					}
					else
					{
						garageLane.m_VehicleCapacity = ushort.MaxValue;
					}
					Game.Net.OutsideConnection outsideConnection = default(Game.Net.OutsideConnection);
					if (nativeArray15.Length != 0)
					{
						outsideConnection = nativeArray15[num4];
					}
					CreateActionData createActionData6 = new CreateActionData
					{
						m_Owner = nativeArray[num4],
						m_StartNode = lane6.m_StartNode,
						m_MiddleNode = lane6.m_MiddleNode,
						m_EndNode = lane6.m_EndNode,
						m_Specification = PathUtils.GetSpecification(curveData4, connectionLaneData, garageLane, outsideConnection, connectionPathfindData),
						m_Location = PathUtils.GetLocationSpecification(curveData4)
					};
					if ((connectionLaneData.m_Flags & (ConnectionLaneFlags.SecondaryStart | ConnectionLaneFlags.SecondaryEnd)) != 0)
					{
						createActionData6.m_SecondaryStartNode = createActionData6.m_StartNode;
						createActionData6.m_SecondaryEndNode = createActionData6.m_EndNode;
						createActionData6.m_SecondarySpecification = PathUtils.GetSecondarySpecification(curveData4, connectionLaneData, outsideConnection, connectionPathfindData);
					}
					m_Actions[num++] = createActionData6;
				}
			}
		}

		private void CheckLaneConnections(ref Lane lane, LaneConnection laneConnection)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneData.HasComponent(laneConnection.m_StartLane))
			{
				lane.m_StartNode = new PathNode(m_LaneData[laneConnection.m_StartLane].m_MiddleNode, laneConnection.m_StartPosition);
			}
			if (m_LaneData.HasComponent(laneConnection.m_EndLane))
			{
				lane.m_EndNode = new PathNode(m_LaneData[laneConnection.m_EndLane].m_MiddleNode, laneConnection.m_EndPosition);
			}
		}
	}

	[BurstCompile]
	private struct UpdatePathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<Density> m_DensityData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<CarLaneData> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> m_PedestrianPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> m_CarPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> m_TrackPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> m_TransportPathfindData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_ConnectionPathfindData;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> m_SlaveLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.CarLane> m_CarLaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> m_TrackLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ParkingLane> m_ParkingLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.PedestrianLane> m_PedestrianLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ConnectionLane> m_ConnectionLaneType;

		[ReadOnly]
		public ComponentTypeHandle<GarageLane> m_GarageLaneType;

		[ReadOnly]
		public ComponentTypeHandle<LaneConnection> m_LaneConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[WriteOnly]
		public NativeArray<UpdateActionData> m_Actions;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<Lane> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<Lane>(ref m_LaneType);
				NativeArray<Curve> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<Curve>(ref m_CurveType);
				NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
				NativeArray<Game.Net.CarLane> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.CarLane>(ref m_CarLaneType);
				NativeArray<Game.Net.TrackLane> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.TrackLane>(ref m_TrackLaneType);
				if (nativeArray5.Length != 0 && !((ArchetypeChunk)(ref val)).Has<SlaveLane>(ref m_SlaveLaneType))
				{
					NativeArray<Owner> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<Owner>(ref m_OwnerType);
					NativeArray<LaneConnection> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					((ArchetypeChunk)(ref val)).Has<EdgeLane>(ref m_EdgeLaneType);
					if (nativeArray6.Length != 0)
					{
						for (int j = 0; j < nativeArray5.Length; j++)
						{
							Lane lane = nativeArray2[j];
							Curve curve = nativeArray3[j];
							Game.Net.CarLane carLane = nativeArray5[j];
							Game.Net.TrackLane trackLaneData = nativeArray6[j];
							PrefabRef prefabRef = nativeArray4[j];
							NetLaneData netLaneData = m_NetLaneData[prefabRef.m_Prefab];
							CarLaneData carLaneData = m_CarLaneData[prefabRef.m_Prefab];
							PathfindCarData carPathfindData = m_CarPathfindData[netLaneData.m_PathfindPrefab];
							PathfindTransportData transportPathfindData = m_TransportPathfindData[netLaneData.m_PathfindPrefab];
							float num2 = 0.01f;
							if (nativeArray7.Length != 0)
							{
								Owner owner = nativeArray7[j];
								if (m_DensityData.HasComponent(owner.m_Owner))
								{
									num2 = math.max(num2, m_DensityData[owner.m_Owner].m_Density);
								}
							}
							if (nativeArray8.Length != 0)
							{
								CheckLaneConnections(ref lane, nativeArray8[j]);
							}
							UpdateActionData updateActionData = new UpdateActionData
							{
								m_Owner = nativeArray[j],
								m_StartNode = lane.m_StartNode,
								m_MiddleNode = lane.m_MiddleNode,
								m_EndNode = lane.m_EndNode,
								m_Specification = PathUtils.GetCarDriveSpecification(curve, carLane, trackLaneData, carLaneData, carPathfindData, num2),
								m_Location = PathUtils.GetLocationSpecification(curve)
							};
							if (carLaneData.m_RoadTypes == RoadTypes.Car)
							{
								updateActionData.m_SecondaryStartNode = updateActionData.m_StartNode;
								updateActionData.m_SecondaryEndNode = updateActionData.m_EndNode;
								updateActionData.m_SecondarySpecification = PathUtils.GetTaxiDriveSpecification(curve, carLane, carPathfindData, transportPathfindData, num2);
							}
							m_Actions[num++] = updateActionData;
						}
						continue;
					}
					for (int k = 0; k < nativeArray5.Length; k++)
					{
						Lane lane2 = nativeArray2[k];
						Curve curve2 = nativeArray3[k];
						Game.Net.CarLane carLane2 = nativeArray5[k];
						PrefabRef prefabRef2 = nativeArray4[k];
						NetLaneData netLaneData2 = m_NetLaneData[prefabRef2.m_Prefab];
						CarLaneData carLaneData2 = m_CarLaneData[prefabRef2.m_Prefab];
						PathfindCarData carPathfindData2 = m_CarPathfindData[netLaneData2.m_PathfindPrefab];
						PathfindTransportData transportPathfindData2 = m_TransportPathfindData[netLaneData2.m_PathfindPrefab];
						float num3 = 0.01f;
						if (nativeArray7.Length != 0)
						{
							Owner owner2 = nativeArray7[k];
							if (m_DensityData.HasComponent(owner2.m_Owner))
							{
								num3 = math.max(num3, m_DensityData[owner2.m_Owner].m_Density);
							}
						}
						if (nativeArray8.Length != 0)
						{
							CheckLaneConnections(ref lane2, nativeArray8[k]);
						}
						UpdateActionData updateActionData2 = new UpdateActionData
						{
							m_Owner = nativeArray[k],
							m_StartNode = lane2.m_StartNode,
							m_MiddleNode = lane2.m_MiddleNode,
							m_EndNode = lane2.m_EndNode,
							m_Specification = PathUtils.GetCarDriveSpecification(curve2, carLane2, carLaneData2, carPathfindData2, num3),
							m_Location = PathUtils.GetLocationSpecification(curve2)
						};
						if (carLaneData2.m_RoadTypes == RoadTypes.Car)
						{
							updateActionData2.m_SecondaryStartNode = updateActionData2.m_StartNode;
							updateActionData2.m_SecondaryEndNode = updateActionData2.m_EndNode;
							updateActionData2.m_SecondarySpecification = PathUtils.GetTaxiDriveSpecification(curve2, carLane2, carPathfindData2, transportPathfindData2, num3);
						}
						m_Actions[num++] = updateActionData2;
					}
					continue;
				}
				if (nativeArray6.Length != 0)
				{
					for (int l = 0; l < nativeArray6.Length; l++)
					{
						Lane lane3 = nativeArray2[l];
						Curve curveData = nativeArray3[l];
						Game.Net.TrackLane trackLaneData2 = nativeArray6[l];
						PrefabRef prefabRef3 = nativeArray4[l];
						NetLaneData netLaneData3 = m_NetLaneData[prefabRef3.m_Prefab];
						PathfindTrackData trackPathfindData = m_TrackPathfindData[netLaneData3.m_PathfindPrefab];
						UpdateActionData updateActionData3 = new UpdateActionData
						{
							m_Owner = nativeArray[l],
							m_StartNode = lane3.m_StartNode,
							m_MiddleNode = lane3.m_MiddleNode,
							m_EndNode = lane3.m_EndNode,
							m_Specification = PathUtils.GetTrackDriveSpecification(curveData, trackLaneData2, trackPathfindData),
							m_Location = PathUtils.GetLocationSpecification(curveData)
						};
						m_Actions[num++] = updateActionData3;
					}
					continue;
				}
				NativeArray<Game.Net.ParkingLane> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.ParkingLane>(ref m_ParkingLaneType);
				if (nativeArray9.Length != 0)
				{
					NativeArray<LaneConnection> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					for (int m = 0; m < nativeArray9.Length; m++)
					{
						Lane lane4 = nativeArray2[m];
						Curve curveData2 = nativeArray3[m];
						Game.Net.ParkingLane parkingLane = nativeArray9[m];
						PrefabRef prefabRef4 = nativeArray4[m];
						NetLaneData netLaneData4 = m_NetLaneData[prefabRef4.m_Prefab];
						ParkingLaneData parkingLaneData = m_ParkingLaneData[prefabRef4.m_Prefab];
						PathfindCarData carPathfindData3 = m_CarPathfindData[netLaneData4.m_PathfindPrefab];
						PathfindTransportData transportPathfindData3 = m_TransportPathfindData[netLaneData4.m_PathfindPrefab];
						if (nativeArray10.Length != 0)
						{
							CheckLaneConnections(ref lane4, nativeArray10[m]);
						}
						UpdateActionData updateActionData4 = new UpdateActionData
						{
							m_Owner = nativeArray[m],
							m_StartNode = lane4.m_StartNode,
							m_MiddleNode = lane4.m_MiddleNode,
							m_EndNode = lane4.m_EndNode,
							m_Specification = PathUtils.GetParkingSpaceSpecification(parkingLane, parkingLaneData, carPathfindData3)
						};
						if ((parkingLane.m_Flags & ParkingLaneFlags.SecondaryStart) != 0)
						{
							updateActionData4.m_SecondaryStartNode = parkingLane.m_SecondaryStartNode;
							updateActionData4.m_SecondaryEndNode = updateActionData4.m_EndNode;
							updateActionData4.m_SecondarySpecification = PathUtils.GetParkingSpaceSpecification(parkingLane, parkingLaneData, carPathfindData3);
						}
						else
						{
							updateActionData4.m_SecondaryStartNode = updateActionData4.m_StartNode;
							updateActionData4.m_SecondaryEndNode = updateActionData4.m_EndNode;
							updateActionData4.m_SecondarySpecification = PathUtils.GetTaxiAccessSpecification(parkingLane, carPathfindData3, transportPathfindData3);
						}
						updateActionData4.m_Location = PathUtils.GetLocationSpecification(curveData2, parkingLane);
						m_Actions[num++] = updateActionData4;
					}
					continue;
				}
				NativeArray<Game.Net.PedestrianLane> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.PedestrianLane>(ref m_PedestrianLaneType);
				if (nativeArray11.Length != 0)
				{
					NativeArray<LaneConnection> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<LaneConnection>(ref m_LaneConnectionType);
					for (int n = 0; n < nativeArray11.Length; n++)
					{
						Lane lane5 = nativeArray2[n];
						Curve curveData3 = nativeArray3[n];
						Game.Net.PedestrianLane pedestrianLaneData = nativeArray11[n];
						PrefabRef prefabRef5 = nativeArray4[n];
						NetLaneData netLaneData5 = m_NetLaneData[prefabRef5.m_Prefab];
						PathfindPedestrianData pedestrianPathfindData = m_PedestrianPathfindData[netLaneData5.m_PathfindPrefab];
						if (nativeArray12.Length != 0)
						{
							CheckLaneConnections(ref lane5, nativeArray12[n]);
						}
						UpdateActionData updateActionData5 = new UpdateActionData
						{
							m_Owner = nativeArray[n],
							m_StartNode = lane5.m_StartNode,
							m_MiddleNode = lane5.m_MiddleNode,
							m_EndNode = lane5.m_EndNode,
							m_Specification = PathUtils.GetSpecification(curveData3, pedestrianLaneData, pedestrianPathfindData),
							m_Location = PathUtils.GetLocationSpecification(curveData3)
						};
						m_Actions[num++] = updateActionData5;
					}
					continue;
				}
				NativeArray<Game.Net.ConnectionLane> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.ConnectionLane>(ref m_ConnectionLaneType);
				if (nativeArray13.Length == 0)
				{
					continue;
				}
				NativeArray<GarageLane> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<GarageLane>(ref m_GarageLaneType);
				NativeArray<Game.Net.OutsideConnection> nativeArray15 = ((ArchetypeChunk)(ref val)).GetNativeArray<Game.Net.OutsideConnection>(ref m_OutsideConnectionType);
				for (int num4 = 0; num4 < nativeArray13.Length; num4++)
				{
					Lane lane6 = nativeArray2[num4];
					Curve curveData4 = nativeArray3[num4];
					Game.Net.ConnectionLane connectionLaneData = nativeArray13[num4];
					PrefabRef prefabRef6 = nativeArray4[num4];
					NetLaneData netLaneData6 = m_NetLaneData[prefabRef6.m_Prefab];
					PathfindConnectionData connectionPathfindData = m_ConnectionPathfindData[netLaneData6.m_PathfindPrefab];
					GarageLane garageLane = default(GarageLane);
					if (nativeArray14.Length != 0)
					{
						garageLane = nativeArray14[num4];
					}
					else
					{
						garageLane.m_VehicleCapacity = ushort.MaxValue;
					}
					Game.Net.OutsideConnection outsideConnection = default(Game.Net.OutsideConnection);
					if (nativeArray15.Length != 0)
					{
						outsideConnection = nativeArray15[num4];
					}
					UpdateActionData updateActionData6 = new UpdateActionData
					{
						m_Owner = nativeArray[num4],
						m_StartNode = lane6.m_StartNode,
						m_MiddleNode = lane6.m_MiddleNode,
						m_EndNode = lane6.m_EndNode,
						m_Specification = PathUtils.GetSpecification(curveData4, connectionLaneData, garageLane, outsideConnection, connectionPathfindData),
						m_Location = PathUtils.GetLocationSpecification(curveData4)
					};
					if ((connectionLaneData.m_Flags & (ConnectionLaneFlags.SecondaryStart | ConnectionLaneFlags.SecondaryEnd)) != 0)
					{
						updateActionData6.m_SecondaryStartNode = updateActionData6.m_StartNode;
						updateActionData6.m_SecondaryEndNode = updateActionData6.m_EndNode;
						updateActionData6.m_SecondarySpecification = PathUtils.GetSecondarySpecification(curveData4, connectionLaneData, outsideConnection, connectionPathfindData);
					}
					m_Actions[num++] = updateActionData6;
				}
			}
		}

		private void CheckLaneConnections(ref Lane lane, LaneConnection laneConnection)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (m_LaneData.HasComponent(laneConnection.m_StartLane))
			{
				lane.m_StartNode = new PathNode(m_LaneData[laneConnection.m_StartLane].m_MiddleNode, laneConnection.m_StartPosition);
			}
			if (m_LaneData.HasComponent(laneConnection.m_EndLane))
			{
				lane.m_EndNode = new PathNode(m_LaneData[laneConnection.m_EndLane].m_MiddleNode, laneConnection.m_EndPosition);
			}
		}
	}

	[BurstCompile]
	private struct RemovePathEdgeJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[WriteOnly]
		public NativeArray<DeleteActionData> m_Actions;

		public void Execute()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					DeleteActionData deleteActionData = new DeleteActionData
					{
						m_Owner = nativeArray[j]
					};
					m_Actions[num++] = deleteActionData;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Density> __Game_Net_Density_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLaneData> __Game_Prefabs_CarLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> __Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> __Game_Prefabs_PathfindCarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> __Game_Prefabs_PathfindTrackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> __Game_Prefabs_PathfindTransportData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> __Game_Prefabs_PathfindConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Lane> __Game_Net_Lane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SlaveLane> __Game_Net_SlaveLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> __Game_Net_EdgeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.TrackLane> __Game_Net_TrackLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.ConnectionLane> __Game_Net_ConnectionLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<GarageLane> __Game_Net_GarageLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LaneConnection> __Game_Net_LaneConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

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
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_Density_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Density>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_CarLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLaneData>(true);
			__Game_Prefabs_ParkingLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLaneData>(true);
			__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindPedestrianData>(true);
			__Game_Prefabs_PathfindCarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindCarData>(true);
			__Game_Prefabs_PathfindTrackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTrackData>(true);
			__Game_Prefabs_PathfindTransportData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTransportData>(true);
			__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindConnectionData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_SlaveLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SlaveLane>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_CarLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.CarLane>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_TrackLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.TrackLane>(true);
			__Game_Net_ParkingLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ParkingLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.PedestrianLane>(true);
			__Game_Net_ConnectionLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.ConnectionLane>(true);
			__Game_Net_GarageLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<GarageLane>(true);
			__Game_Net_LaneConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LaneConnection>(true);
			__Game_Net_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.OutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private PathfindQueueSystem m_PathfindQueueSystem;

	private EntityQuery m_CreatedLanesQuery;

	private EntityQuery m_UpdatedLanesQuery;

	private EntityQuery m_DeletedLanesQuery;

	private EntityQuery m_AllLanesQuery;

	private bool m_Loaded;

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
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Expected O, but got Unknown
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Expected O, but got Unknown
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Expected O, but got Unknown
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SlaveLane>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.TrackLane>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[1] = val;
		m_CreatedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[3];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SlaveLane>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.TrackLane>() };
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[1] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PathfindUpdated>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SlaveLane>()
		};
		array2[2] = val;
		m_UpdatedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SlaveLane>()
		};
		array3[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Lane>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.TrackLane>() };
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array3[1] = val;
		m_DeletedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		EntityQueryDesc[] array4 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Lane>() };
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.CarLane>(),
			ComponentType.ReadOnly<Game.Net.ParkingLane>(),
			ComponentType.ReadOnly<Game.Net.PedestrianLane>(),
			ComponentType.ReadOnly<Game.Net.ConnectionLane>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<SlaveLane>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array4[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Lane>() };
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.TrackLane>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array4[1] = val;
		m_AllLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array4);
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		m_Loaded = true;
	}

	private bool GetLoaded()
	{
		if (m_Loaded)
		{
			m_Loaded = false;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val;
		int num;
		if (GetLoaded())
		{
			val = m_AllLanesQuery;
			num = 0;
		}
		else
		{
			val = m_CreatedLanesQuery;
			num = ((EntityQuery)(ref m_UpdatedLanesQuery)).CalculateEntityCount();
		}
		int num2 = ((EntityQuery)(ref val)).CalculateEntityCount();
		int num3 = ((EntityQuery)(ref m_DeletedLanesQuery)).CalculateEntityCount();
		if (num2 != 0 || num != 0 || num3 != 0)
		{
			JobHandle val2 = ((SystemBase)this).Dependency;
			if (num2 != 0)
			{
				CreateAction action = new CreateAction(num2, (Allocator)4);
				JobHandle val3 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref val)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
				JobHandle val4 = IJobExtensions.Schedule<AddPathEdgeJob>(new AddPathEdgeJob
				{
					m_Chunks = chunks,
					m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DensityData = InternalCompilerInterface.GetComponentLookup<Density>(ref __TypeHandle.__Game_Net_Density_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindPedestrianData>(ref __TypeHandle.__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindCarData>(ref __TypeHandle.__Game_Prefabs_PathfindCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TrackPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTrackData>(ref __TypeHandle.__Game_Prefabs_PathfindTrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTransportData>(ref __TypeHandle.__Game_Prefabs_PathfindTransportData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_GarageLaneType = InternalCompilerInterface.GetComponentTypeHandle<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LaneConnectionType = InternalCompilerInterface.GetComponentTypeHandle<LaneConnection>(ref __TypeHandle.__Game_Net_LaneConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action.m_CreateData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
				val2 = JobHandle.CombineDependencies(val2, val4);
				chunks.Dispose(val4);
				m_PathfindQueueSystem.Enqueue(action, val4);
			}
			if (num != 0)
			{
				UpdateAction action2 = new UpdateAction(num, (Allocator)4);
				JobHandle val5 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks2 = ((EntityQuery)(ref m_UpdatedLanesQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val5);
				JobHandle val6 = IJobExtensions.Schedule<UpdatePathEdgeJob>(new UpdatePathEdgeJob
				{
					m_Chunks = chunks2,
					m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_DensityData = InternalCompilerInterface.GetComponentLookup<Density>(ref __TypeHandle.__Game_Net_Density_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindPedestrianData>(ref __TypeHandle.__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_CarPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindCarData>(ref __TypeHandle.__Game_Prefabs_PathfindCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TrackPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTrackData>(ref __TypeHandle.__Game_Prefabs_PathfindTrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_TransportPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindTransportData>(ref __TypeHandle.__Game_Prefabs_PathfindTransportData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionPathfindData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SlaveLaneType = InternalCompilerInterface.GetComponentTypeHandle<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CarLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_TrackLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.TrackLane>(ref __TypeHandle.__Game_Net_TrackLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ParkingLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PedestrianLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ConnectionLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.ConnectionLane>(ref __TypeHandle.__Game_Net_ConnectionLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_GarageLaneType = InternalCompilerInterface.GetComponentTypeHandle<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_LaneConnectionType = InternalCompilerInterface.GetComponentTypeHandle<LaneConnection>(ref __TypeHandle.__Game_Net_LaneConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action2.m_UpdateData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val5));
				val2 = JobHandle.CombineDependencies(val2, val6);
				chunks2.Dispose(val6);
				m_PathfindQueueSystem.Enqueue(action2, val6);
			}
			if (num3 != 0)
			{
				DeleteAction action3 = new DeleteAction(num3, (Allocator)4);
				JobHandle val7 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks3 = ((EntityQuery)(ref m_DeletedLanesQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val7);
				JobHandle val8 = IJobExtensions.Schedule<RemovePathEdgeJob>(new RemovePathEdgeJob
				{
					m_Chunks = chunks3,
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_Actions = action3.m_DeleteData
				}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val7));
				val2 = JobHandle.CombineDependencies(val2, val8);
				chunks3.Dispose(val8);
				m_PathfindQueueSystem.Enqueue(action3, val8);
			}
			((SystemBase)this).Dependency = val2;
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
	public LanesModifiedSystem()
	{
	}
}
