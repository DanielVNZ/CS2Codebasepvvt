using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Net;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class NetCompositionSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeCompositionJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<NetCompositionData> m_NetCompositionType;

		public ComponentTypeHandle<PlaceableNetComposition> m_PlaceableNetCompositionType;

		public ComponentTypeHandle<RoadComposition> m_RoadCompositionType;

		public ComponentTypeHandle<TrackComposition> m_TrackCompositionType;

		public ComponentTypeHandle<WaterwayComposition> m_WaterwayCompositionType;

		public ComponentTypeHandle<PathwayComposition> m_PathwayCompositionType;

		public ComponentTypeHandle<TaxiwayComposition> m_TaxiwayCompositionType;

		public ComponentTypeHandle<TerrainComposition> m_TerrainCompositionType;

		public BufferTypeHandle<NetCompositionPiece> m_NetCompositionPieceType;

		public BufferTypeHandle<NetCompositionLane> m_NetCompositionLaneType;

		public BufferTypeHandle<NetCompositionObject> m_NetCompositionObjectType;

		public BufferTypeHandle<NetCompositionArea> m_NetCompositionAreaType;

		public BufferTypeHandle<NetCompositionCrosswalk> m_NetCompositionCrosswalkType;

		public BufferTypeHandle<NetCompositionCarriageway> m_NetCompositionCarriagewayType;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetPieceData> m_PlaceableNetPieceData;

		[ReadOnly]
		public ComponentLookup<NetPieceData> m_NetPieceData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<NetCrosswalkData> m_NetCrosswalkData;

		[ReadOnly]
		public ComponentLookup<NetVertexMatchData> m_NetVertexMatchData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadData;

		[ReadOnly]
		public ComponentLookup<TrackData> m_TrackData;

		[ReadOnly]
		public ComponentLookup<WaterwayData> m_WaterwayData;

		[ReadOnly]
		public ComponentLookup<PathwayData> m_PathwayData;

		[ReadOnly]
		public ComponentLookup<TaxiwayData> m_TaxiwayData;

		[ReadOnly]
		public ComponentLookup<StreetLightData> m_StreetLightData;

		[ReadOnly]
		public ComponentLookup<LaneDirectionData> m_LaneDirectionData;

		[ReadOnly]
		public ComponentLookup<TrafficSignData> m_TrafficSignData;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> m_UtilityObjectData;

		[ReadOnly]
		public ComponentLookup<MeshData> m_MeshData;

		[ReadOnly]
		public ComponentLookup<NetTerrainData> m_TerrainData;

		[ReadOnly]
		public ComponentLookup<BridgeData> m_BridgeData;

		[ReadOnly]
		public ComponentLookup<PrefabData> m_PrefabData;

		[ReadOnly]
		public BufferLookup<NetPieceLane> m_NetPieceLanes;

		[ReadOnly]
		public BufferLookup<NetPieceObject> m_NetPieceObjects;

		[ReadOnly]
		public BufferLookup<NetPieceArea> m_NetPieceAreas;

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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_084d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0872: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_091f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0913: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_0943: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0987: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<NetCompositionData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCompositionData>(ref m_NetCompositionType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<PlaceableNetComposition> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PlaceableNetComposition>(ref m_PlaceableNetCompositionType);
			NativeArray<RoadComposition> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RoadComposition>(ref m_RoadCompositionType);
			NativeArray<TrackComposition> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TrackComposition>(ref m_TrackCompositionType);
			NativeArray<WaterwayComposition> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<WaterwayComposition>(ref m_WaterwayCompositionType);
			NativeArray<PathwayComposition> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathwayComposition>(ref m_PathwayCompositionType);
			NativeArray<TaxiwayComposition> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TaxiwayComposition>(ref m_TaxiwayCompositionType);
			NativeArray<TerrainComposition> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TerrainComposition>(ref m_TerrainCompositionType);
			BufferAccessor<NetCompositionPiece> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionPiece>(ref m_NetCompositionPieceType);
			BufferAccessor<NetCompositionLane> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionLane>(ref m_NetCompositionLaneType);
			BufferAccessor<NetCompositionObject> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionObject>(ref m_NetCompositionObjectType);
			BufferAccessor<NetCompositionArea> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionArea>(ref m_NetCompositionAreaType);
			BufferAccessor<NetCompositionCrosswalk> bufferAccessor5 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionCrosswalk>(ref m_NetCompositionCrosswalkType);
			BufferAccessor<NetCompositionCarriageway> bufferAccessor6 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<NetCompositionCarriageway>(ref m_NetCompositionCarriagewayType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				NetCompositionData compositionData = nativeArray[i];
				DynamicBuffer<NetCompositionPiece> val = bufferAccessor[i];
				PrefabRef prefabRef = nativeArray2[i];
				NetGeometryData netGeometryData = m_NetGeometryData[prefabRef.m_Prefab];
				NetCompositionHelpers.CalculateCompositionData(ref compositionData, val.AsNativeArray(), m_NetPieceData, m_NetLaneData, m_NetVertexMatchData, m_NetPieceLanes);
				if (!m_PrefabData.IsComponentEnabled(prefabRef.m_Prefab))
				{
					if ((compositionData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0)
					{
						compositionData.m_Width = netGeometryData.m_ElevatedWidth;
						compositionData.m_HeightRange = netGeometryData.m_ElevatedHeightRange;
					}
					else
					{
						compositionData.m_Width = netGeometryData.m_DefaultWidth;
						compositionData.m_HeightRange = netGeometryData.m_DefaultHeightRange;
					}
					compositionData.m_SurfaceHeight = netGeometryData.m_DefaultSurfaceHeight;
				}
				NetCompositionHelpers.CalculateMinLod(ref compositionData, val.AsNativeArray(), m_MeshData);
				if ((netGeometryData.m_Flags & GeometryFlags.Marker) != 0)
				{
					compositionData.m_State |= CompositionState.Marker | CompositionState.NoSubCollisions;
				}
				if ((netGeometryData.m_Flags & GeometryFlags.BlockZone) != 0)
				{
					compositionData.m_State |= CompositionState.BlockZone;
				}
				nativeArray[i] = compositionData;
			}
			if (bufferAccessor2.Length != 0)
			{
				NativeList<NetCompositionLane> netLanes = default(NativeList<NetCompositionLane>);
				netLanes._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
				for (int j = 0; j < bufferAccessor2.Length; j++)
				{
					NetCompositionData compositionData2 = nativeArray[j];
					DynamicBuffer<NetCompositionPiece> pieces = bufferAccessor[j];
					DynamicBuffer<NetCompositionLane> val2 = bufferAccessor2[j];
					PrefabRef prefabRef2 = nativeArray2[j];
					DynamicBuffer<NetCompositionCarriageway> carriageways = default(DynamicBuffer<NetCompositionCarriageway>);
					if (bufferAccessor6.Length != 0)
					{
						carriageways = bufferAccessor6[j];
					}
					NetCompositionHelpers.AddCompositionLanes<DynamicBuffer<NetCompositionPiece>>(prefabRef2.m_Prefab, ref compositionData2, pieces, netLanes, carriageways, m_NetLaneData, m_NetPieceLanes);
					val2.CopyFrom(netLanes.AsArray());
					netLanes.Clear();
					nativeArray[j] = compositionData2;
				}
			}
			for (int k = 0; k < bufferAccessor3.Length; k++)
			{
				NetCompositionData compositionData3 = nativeArray[k];
				DynamicBuffer<NetCompositionPiece> pieces2 = bufferAccessor[k];
				DynamicBuffer<NetCompositionObject> objects = bufferAccessor3[k];
				AddCompositionObjects(nativeArray2[k].m_Prefab, compositionData3, pieces2, objects, m_StreetLightData, m_LaneDirectionData, m_TrafficSignData, m_UtilityObjectData, m_NetPieceObjects);
			}
			for (int l = 0; l < bufferAccessor4.Length; l++)
			{
				NetCompositionData compositionData4 = nativeArray[l];
				DynamicBuffer<NetCompositionPiece> pieces3 = bufferAccessor[l];
				DynamicBuffer<NetCompositionArea> netAreas = bufferAccessor4[l];
				PrefabRef prefabRef3 = nativeArray2[l];
				bool isBridge = (compositionData4.m_Flags.m_General & CompositionFlags.General.Elevated) != 0 && m_BridgeData.HasComponent(prefabRef3.m_Prefab);
				AddCompositionAreas(prefabRef3.m_Prefab, compositionData4, pieces3, netAreas, isBridge);
			}
			for (int m = 0; m < bufferAccessor5.Length; m++)
			{
				NetCompositionData compositionData5 = nativeArray[m];
				DynamicBuffer<NetCompositionPiece> pieces4 = bufferAccessor[m];
				DynamicBuffer<NetCompositionCrosswalk> crosswalks = bufferAccessor5[m];
				AddCompositionCrosswalks(nativeArray2[m].m_Prefab, compositionData5, pieces4, crosswalks);
			}
			for (int n = 0; n < nativeArray3.Length; n++)
			{
				PlaceableNetComposition placeableData = nativeArray3[n];
				NetCompositionHelpers.CalculatePlaceableData(ref placeableData, bufferAccessor[n].AsNativeArray(), m_PlaceableNetPieceData);
				nativeArray3[n] = placeableData;
			}
			for (int num = 0; num < nativeArray4.Length; num++)
			{
				PrefabRef prefabRef4 = nativeArray2[num];
				NetCompositionData netCompositionData = nativeArray[num];
				RoadComposition roadComposition = nativeArray4[num];
				RoadData roadData = m_RoadData[prefabRef4.m_Prefab];
				roadComposition.m_ZoneBlockPrefab = roadData.m_ZoneBlockPrefab;
				roadComposition.m_Flags = roadData.m_Flags;
				roadComposition.m_SpeedLimit = roadData.m_SpeedLimit;
				roadComposition.m_Priority = roadData.m_SpeedLimit;
				if ((netCompositionData.m_State & CompositionState.SeparatedCarriageways) != 0)
				{
					roadComposition.m_Flags |= RoadFlags.SeparatedCarriageways;
				}
				if ((netCompositionData.m_Flags.m_General & CompositionFlags.General.Gravel) != 0)
				{
					roadComposition.m_Priority -= 1.25f;
				}
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<NetCompositionLane> val3 = bufferAccessor2[num];
					int num2 = 0;
					int num3 = 0;
					for (int num4 = 0; num4 < val3.Length; num4++)
					{
						NetCompositionLane netCompositionLane = val3[num4];
						if ((netCompositionLane.m_Flags & (LaneFlags.Master | LaneFlags.Road)) == LaneFlags.Road)
						{
							if ((netCompositionLane.m_Flags & LaneFlags.Invert) != 0)
							{
								num3++;
							}
							else
							{
								num2++;
							}
						}
					}
					if ((roadData.m_Flags & RoadFlags.UseHighwayRules) != 0)
					{
						roadComposition.m_Priority += math.max(num2, num3);
					}
					else
					{
						roadComposition.m_Priority += (float)math.max(num2, num3) * 0.5f + (float)(num2 + num3) * 0.25f;
					}
				}
				if (bufferAccessor3.Length != 0)
				{
					DynamicBuffer<NetCompositionObject> val4 = bufferAccessor3[num];
					for (int num5 = 0; num5 < val4.Length; num5++)
					{
						NetCompositionObject netCompositionObject = val4[num5];
						if (m_StreetLightData.HasComponent(netCompositionObject.m_Prefab))
						{
							roadComposition.m_Flags |= RoadFlags.HasStreetLights;
							break;
						}
					}
				}
				nativeArray4[num] = roadComposition;
			}
			for (int num6 = 0; num6 < nativeArray5.Length; num6++)
			{
				PrefabRef prefabRef5 = nativeArray2[num6];
				TrackComposition trackComposition = nativeArray5[num6];
				TrackData trackData = m_TrackData[prefabRef5.m_Prefab];
				trackComposition.m_TrackType = trackData.m_TrackType;
				trackComposition.m_SpeedLimit = trackData.m_SpeedLimit;
				nativeArray5[num6] = trackComposition;
			}
			for (int num7 = 0; num7 < nativeArray6.Length; num7++)
			{
				PrefabRef prefabRef6 = nativeArray2[num7];
				WaterwayComposition waterwayComposition = nativeArray6[num7];
				waterwayComposition.m_SpeedLimit = m_WaterwayData[prefabRef6.m_Prefab].m_SpeedLimit;
				nativeArray6[num7] = waterwayComposition;
			}
			for (int num8 = 0; num8 < nativeArray7.Length; num8++)
			{
				PrefabRef prefabRef7 = nativeArray2[num8];
				PathwayComposition pathwayComposition = nativeArray7[num8];
				pathwayComposition.m_SpeedLimit = m_PathwayData[prefabRef7.m_Prefab].m_SpeedLimit;
				nativeArray7[num8] = pathwayComposition;
			}
			for (int num9 = 0; num9 < nativeArray8.Length; num9++)
			{
				PrefabRef prefabRef8 = nativeArray2[num9];
				TaxiwayComposition taxiwayComposition = nativeArray8[num9];
				NetCompositionData netCompositionData2 = nativeArray[num9];
				TaxiwayData taxiwayData = m_TaxiwayData[prefabRef8.m_Prefab];
				taxiwayComposition.m_SpeedLimit = taxiwayData.m_SpeedLimit;
				taxiwayComposition.m_Flags = taxiwayData.m_Flags;
				if ((taxiwayData.m_Flags & TaxiwayFlags.Airspace) != 0)
				{
					netCompositionData2.m_State &= ~CompositionState.NoSubCollisions;
					netCompositionData2.m_State |= CompositionState.Airspace;
				}
				nativeArray[num9] = netCompositionData2;
				nativeArray8[num9] = taxiwayComposition;
			}
			NetTerrainData netTerrainData = default(NetTerrainData);
			for (int num10 = 0; num10 < nativeArray9.Length; num10++)
			{
				TerrainComposition terrainComposition = nativeArray9[num10];
				NetCompositionData netCompositionData3 = nativeArray[num10];
				DynamicBuffer<NetCompositionPiece> val5 = bufferAccessor[num10];
				terrainComposition.m_ClipHeightOffset = new float2(float.MaxValue, float.MinValue);
				terrainComposition.m_MinHeightOffset = float3.op_Implicit(float.MaxValue);
				terrainComposition.m_MaxHeightOffset = float3.op_Implicit(float.MinValue);
				for (int num11 = 0; num11 < val5.Length; num11++)
				{
					NetCompositionPiece netCompositionPiece = val5[num11];
					if (m_TerrainData.TryGetComponent(netCompositionPiece.m_Piece, ref netTerrainData))
					{
						float2 val6 = netCompositionData3.m_Width * 0.5f + new float2(netCompositionPiece.m_Offset.x, 0f - netCompositionPiece.m_Offset.x) - netCompositionPiece.m_Size.x * 0.5f;
						if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0)
						{
							netTerrainData.m_WidthOffset = ((float2)(ref netTerrainData.m_WidthOffset)).yx;
							((float3)(ref netTerrainData.m_MinHeightOffset)).xz = ((float3)(ref netTerrainData.m_MinHeightOffset)).zx;
							((float3)(ref netTerrainData.m_MaxHeightOffset)).xz = ((float3)(ref netTerrainData.m_MaxHeightOffset)).zx;
						}
						if (netTerrainData.m_WidthOffset.x != 0f)
						{
							terrainComposition.m_WidthOffset.x = math.max(terrainComposition.m_WidthOffset.x, val6.x + netTerrainData.m_WidthOffset.x);
						}
						if (netTerrainData.m_WidthOffset.y != 0f)
						{
							terrainComposition.m_WidthOffset.y = math.max(terrainComposition.m_WidthOffset.y, val6.y + netTerrainData.m_WidthOffset.y);
						}
						terrainComposition.m_ClipHeightOffset.x = math.min(terrainComposition.m_ClipHeightOffset.x, netTerrainData.m_ClipHeightOffset.x);
						terrainComposition.m_ClipHeightOffset.y = math.max(terrainComposition.m_ClipHeightOffset.y, netTerrainData.m_ClipHeightOffset.y);
						terrainComposition.m_MinHeightOffset = math.min(terrainComposition.m_MinHeightOffset, netTerrainData.m_MinHeightOffset);
						terrainComposition.m_MaxHeightOffset = math.max(terrainComposition.m_MaxHeightOffset, netTerrainData.m_MaxHeightOffset);
					}
				}
				terrainComposition.m_ClipHeightOffset = math.select(terrainComposition.m_ClipHeightOffset, float2.op_Implicit(0f), terrainComposition.m_ClipHeightOffset == new float2(float.MaxValue, float.MinValue));
				terrainComposition.m_MinHeightOffset = math.select(terrainComposition.m_MinHeightOffset, float3.op_Implicit(0f), terrainComposition.m_MinHeightOffset == float.MaxValue);
				terrainComposition.m_MaxHeightOffset = math.select(terrainComposition.m_MaxHeightOffset, float3.op_Implicit(0f), terrainComposition.m_MaxHeightOffset == float.MinValue);
				nativeArray9[num10] = terrainComposition;
			}
		}

		private void AddCompositionAreas(Entity entity, NetCompositionData compositionData, DynamicBuffer<NetCompositionPiece> pieces, DynamicBuffer<NetCompositionArea> netAreas, bool isBridge)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				if (!m_NetPieceAreas.HasBuffer(netCompositionPiece.m_Piece))
				{
					continue;
				}
				DynamicBuffer<NetPieceArea> val = m_NetPieceAreas[netCompositionPiece.m_Piece];
				bool flag = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
				for (int j = 0; j < val.Length; j++)
				{
					NetPieceArea netPieceArea = val[math.select(j, val.Length - j - 1, flag)];
					if (!isBridge || (netPieceArea.m_Flags & NetAreaFlags.NoBridge) == 0)
					{
						if (flag)
						{
							netPieceArea.m_Position.x = 0f - netPieceArea.m_Position.x;
							netPieceArea.m_SnapPosition.x = 0f - netPieceArea.m_SnapPosition.x;
							netPieceArea.m_Flags |= NetAreaFlags.Invert;
						}
						netAreas.Add(new NetCompositionArea
						{
							m_Flags = netPieceArea.m_Flags,
							m_Position = netCompositionPiece.m_Offset + netPieceArea.m_Position,
							m_SnapPosition = netCompositionPiece.m_Offset + netPieceArea.m_SnapPosition,
							m_Width = netPieceArea.m_Width,
							m_SnapWidth = netPieceArea.m_SnapWidth
						});
					}
				}
			}
		}

		public static void AddCompositionObjects(Entity entity, NetCompositionData compositionData, DynamicBuffer<NetCompositionPiece> pieces, DynamicBuffer<NetCompositionObject> objects, ComponentLookup<StreetLightData> streetLightDatas, ComponentLookup<LaneDirectionData> laneDirectionDatas, ComponentLookup<TrafficSignData> trafficSignDatas, ComponentLookup<UtilityObjectData> utilityObjectDatas, BufferLookup<NetPieceObject> netPieceObjects)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			bool flag = (compositionData.m_Flags.m_General & CompositionFlags.General.Edge) != 0;
			bool flag2 = (compositionData.m_Flags.m_General & CompositionFlags.General.Invert) != 0;
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				if (!netPieceObjects.HasBuffer(netCompositionPiece.m_Piece))
				{
					continue;
				}
				DynamicBuffer<NetPieceObject> val = netPieceObjects[netCompositionPiece.m_Piece];
				bool flag3 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
				bool flag4 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.FlipLanes) != 0;
				bool flag5 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Median) != 0;
				bool flag6 = (netCompositionPiece.m_PieceFlags & NetPieceFlags.PreserveShape) != 0;
				bool flag7 = flag && flag4;
				CompositionFlags compositionFlags;
				NetSectionFlags sectionFlags;
				if (flag3)
				{
					compositionFlags = NetCompositionHelpers.InvertCompositionFlags(compositionData.m_Flags);
					sectionFlags = NetCompositionHelpers.InvertSectionFlags(netCompositionPiece.m_SectionFlags);
				}
				else
				{
					compositionFlags = compositionData.m_Flags;
					sectionFlags = netCompositionPiece.m_SectionFlags;
				}
				for (int j = 0; j < val.Length; j++)
				{
					NetPieceObject netPieceObject = val[math.select(j, val.Length - j - 1, flag3)];
					if (!NetCompositionHelpers.TestObjectFlags(netPieceObject, compositionFlags, sectionFlags))
					{
						continue;
					}
					NetCompositionObject netCompositionObject = default(NetCompositionObject);
					bool flag8 = false;
					if (flag3)
					{
						flag8 ^= (netPieceObject.m_Flags & SubObjectFlags.FlipInverted) != 0;
						netPieceObject.m_Position.x = 0f - netPieceObject.m_Position.x;
					}
					if (flag7)
					{
						flag8 ^= laneDirectionDatas.HasComponent(netPieceObject.m_Prefab) || trafficSignDatas.HasComponent(netPieceObject.m_Prefab);
					}
					if (flag8)
					{
						netPieceObject.m_Rotation = math.mul(quaternion.RotateY((float)Math.PI), netPieceObject.m_Rotation);
						netPieceObject.m_CurveOffsetRange = 1f - netPieceObject.m_CurveOffsetRange;
						if ((compositionData.m_Flags.m_General & CompositionFlags.General.Edge) != 0)
						{
							netPieceObject.m_Position.z = 0f - netPieceObject.m_Position.z;
						}
					}
					float3 val2 = netCompositionPiece.m_Offset + netPieceObject.m_Position;
					netCompositionObject.m_Prefab = netPieceObject.m_Prefab;
					netCompositionObject.m_Position = ((float3)(ref val2)).xz;
					netCompositionObject.m_Offset = math.rotate(netPieceObject.m_Rotation, netPieceObject.m_Offset);
					netCompositionObject.m_Offset.y += val2.y;
					netCompositionObject.m_Rotation = netPieceObject.m_Rotation;
					netCompositionObject.m_Flags = netPieceObject.m_Flags;
					netCompositionObject.m_SpacingIgnore = netPieceObject.m_CompositionNone.m_General;
					netCompositionObject.m_UseCurveRotation = netPieceObject.m_UseCurveRotation;
					netCompositionObject.m_Probability = netPieceObject.m_Probability;
					netCompositionObject.m_CurveOffsetRange = netPieceObject.m_CurveOffsetRange;
					netCompositionObject.m_Spacing = netPieceObject.m_Spacing.z;
					netCompositionObject.m_MinLength = netPieceObject.m_MinLength;
					if (netPieceObject.m_Spacing.z > 0.1f)
					{
						netCompositionObject.m_Flags |= SubObjectFlags.AllowCombine;
					}
					if (flag5)
					{
						netCompositionObject.m_Flags |= SubObjectFlags.OnMedian;
					}
					if (flag6)
					{
						netCompositionObject.m_Flags |= SubObjectFlags.PreserveShape;
					}
					int k;
					NetCompositionObject netCompositionObject2;
					bool flag11;
					if (netPieceObject.m_Spacing.x > 0.1f)
					{
						StreetLightData streetLightData = default(StreetLightData);
						bool flag9 = false;
						bool flag10 = utilityObjectDatas.HasComponent(netPieceObject.m_Prefab);
						if (streetLightDatas.HasComponent(netPieceObject.m_Prefab))
						{
							streetLightData = streetLightDatas[netPieceObject.m_Prefab];
							flag9 = true;
						}
						if (!flag9 || (compositionData.m_Flags.m_General & CompositionFlags.General.Intersection) == 0)
						{
							for (k = 0; k < objects.Length; k++)
							{
								netCompositionObject2 = objects[k];
								float num = math.abs(netCompositionObject.m_Position.x - netCompositionObject2.m_Position.x);
								flag11 = ((netCompositionObject.m_Flags ^ netCompositionObject2.m_Flags) & SubObjectFlags.SpacingOverride) != 0;
								if (num >= netPieceObject.m_Spacing.x && !flag11)
								{
									continue;
								}
								if (netCompositionObject2.m_Prefab != netCompositionObject.m_Prefab)
								{
									if (!flag9 || !streetLightDatas.HasComponent(netCompositionObject2.m_Prefab))
									{
										if (!flag9 && !flag10 && netCompositionObject.m_Spacing > 0.1f && netCompositionObject2.m_Spacing > 0.1f && ((netCompositionObject.m_Flags ^ netCompositionObject2.m_Flags) & SubObjectFlags.EvenSpacing) == 0)
										{
											netCompositionObject.m_AvoidSpacing = netCompositionObject2.m_Spacing;
										}
										continue;
									}
									StreetLightData streetLightData2 = streetLightDatas[netCompositionObject2.m_Prefab];
									if (streetLightData.m_Layer != streetLightData2.m_Layer)
									{
										continue;
									}
								}
								goto IL_0455;
							}
						}
					}
					goto IL_04cc;
					IL_04cc:
					objects.Add(netCompositionObject);
					continue;
					IL_0455:
					if (flag11)
					{
						if ((netCompositionObject.m_Flags & SubObjectFlags.SpacingOverride) == 0)
						{
							continue;
						}
						objects.RemoveAt(k);
					}
					else
					{
						float num2 = math.abs(netCompositionObject.m_Position.x) - math.abs(netCompositionObject2.m_Position.x);
						if (num2 >= 4f || (flag2 && num2 > -4f))
						{
							continue;
						}
						objects.RemoveAt(k);
					}
					goto IL_04cc;
				}
			}
		}

		private void AddCompositionCrosswalks(Entity entity, NetCompositionData compositionData, DynamicBuffer<NetCompositionPiece> pieces, DynamicBuffer<NetCompositionCrosswalk> crosswalks)
		{
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			NetCrosswalkData netCrosswalkData = default(NetCrosswalkData);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			for (int i = 0; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieces[i];
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.Surface) == 0)
				{
					continue;
				}
				if (m_NetCrosswalkData.HasComponent(netCompositionPiece.m_Piece))
				{
					NetCrosswalkData netCrosswalkData2 = m_NetCrosswalkData[netCompositionPiece.m_Piece];
					if (flag3)
					{
						if (!flag)
						{
							NetLaneData netLaneData = m_NetLaneData[netCrosswalkData.m_Lane];
							NetCompositionCrosswalk netCompositionCrosswalk = new NetCompositionCrosswalk
							{
								m_Lane = netCrosswalkData.m_Lane,
								m_Start = netCrosswalkData.m_Start,
								m_End = netCrosswalkData.m_End,
								m_Flags = netLaneData.m_Flags
							};
							if (flag4)
							{
								netCompositionCrosswalk.m_Flags |= LaneFlags.CrossRoad;
							}
							crosswalks.Add(netCompositionCrosswalk);
						}
						netCrosswalkData = default(NetCrosswalkData);
						flag = flag2;
						flag2 = false;
						flag3 = false;
						flag4 = false;
					}
					if (!flag4 && m_NetPieceLanes.HasBuffer(netCompositionPiece.m_Piece))
					{
						DynamicBuffer<NetPieceLane> val = m_NetPieceLanes[netCompositionPiece.m_Piece];
						for (int j = 0; j < val.Length; j++)
						{
							NetPieceLane netPieceLane = val[j];
							if (netPieceLane.m_Position.x >= netCrosswalkData2.m_Start.x && netPieceLane.m_Position.x <= netCrosswalkData2.m_End.x && (m_NetLaneData[netPieceLane.m_Lane].m_Flags & LaneFlags.Road) != 0)
							{
								flag4 = true;
								break;
							}
						}
					}
					if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0)
					{
						float x = netCrosswalkData2.m_Start.x;
						netCrosswalkData2.m_Start.x = 0f - netCrosswalkData2.m_End.x;
						netCrosswalkData2.m_End.x = 0f - x;
					}
					if (netCrosswalkData.m_Lane == Entity.Null)
					{
						netCrosswalkData.m_Lane = netCrosswalkData2.m_Lane;
						netCrosswalkData.m_Start = netCompositionPiece.m_Offset + netCrosswalkData2.m_Start;
						netCrosswalkData.m_End = netCompositionPiece.m_Offset + netCrosswalkData2.m_End;
					}
					else
					{
						netCrosswalkData.m_End = netCompositionPiece.m_Offset + netCrosswalkData2.m_End;
					}
					continue;
				}
				if (netCompositionPiece.m_Size.x > 0f)
				{
					flag2 = false;
					if (netCrosswalkData.m_Lane != Entity.Null)
					{
						flag3 = true;
					}
				}
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.BlockCrosswalk) != 0)
				{
					flag = true;
					flag2 = true;
				}
			}
			if (netCrosswalkData.m_Lane != Entity.Null && !flag)
			{
				NetLaneData netLaneData2 = m_NetLaneData[netCrosswalkData.m_Lane];
				NetCompositionCrosswalk netCompositionCrosswalk2 = new NetCompositionCrosswalk
				{
					m_Lane = netCrosswalkData.m_Lane,
					m_Start = netCrosswalkData.m_Start,
					m_End = netCrosswalkData.m_End,
					m_Flags = netLaneData2.m_Flags
				};
				if (flag4)
				{
					netCompositionCrosswalk2.m_Flags |= LaneFlags.CrossRoad;
				}
				crosswalks.Add(netCompositionCrosswalk2);
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
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<NetCompositionData> __Game_Prefabs_NetCompositionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PlaceableNetComposition> __Game_Prefabs_PlaceableNetComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<RoadComposition> __Game_Prefabs_RoadComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TrackComposition> __Game_Prefabs_TrackComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WaterwayComposition> __Game_Prefabs_WaterwayComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathwayComposition> __Game_Prefabs_PathwayComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TaxiwayComposition> __Game_Prefabs_TaxiwayComposition_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TerrainComposition> __Game_Prefabs_TerrainComposition_RW_ComponentTypeHandle;

		public BufferTypeHandle<NetCompositionPiece> __Game_Prefabs_NetCompositionPiece_RW_BufferTypeHandle;

		public BufferTypeHandle<NetCompositionLane> __Game_Prefabs_NetCompositionLane_RW_BufferTypeHandle;

		public BufferTypeHandle<NetCompositionObject> __Game_Prefabs_NetCompositionObject_RW_BufferTypeHandle;

		public BufferTypeHandle<NetCompositionArea> __Game_Prefabs_NetCompositionArea_RW_BufferTypeHandle;

		public BufferTypeHandle<NetCompositionCrosswalk> __Game_Prefabs_NetCompositionCrosswalk_RW_BufferTypeHandle;

		public BufferTypeHandle<NetCompositionCarriageway> __Game_Prefabs_NetCompositionCarriageway_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetPieceData> __Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetPieceData> __Game_Prefabs_NetPieceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCrosswalkData> __Game_Prefabs_NetCrosswalkData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetVertexMatchData> __Game_Prefabs_NetVertexMatchData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrackData> __Game_Prefabs_TrackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterwayData> __Game_Prefabs_WaterwayData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathwayData> __Game_Prefabs_PathwayData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TaxiwayData> __Game_Prefabs_TaxiwayData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StreetLightData> __Game_Prefabs_StreetLightData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneDirectionData> __Game_Prefabs_LaneDirectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrafficSignData> __Game_Prefabs_TrafficSignData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityObjectData> __Game_Prefabs_UtilityObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MeshData> __Game_Prefabs_MeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetTerrainData> __Game_Prefabs_NetTerrainData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BridgeData> __Game_Prefabs_BridgeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<NetPieceLane> __Game_Prefabs_NetPieceLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetPieceObject> __Game_Prefabs_NetPieceObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetPieceArea> __Game_Prefabs_NetPieceArea_RO_BufferLookup;

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
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Prefabs_NetCompositionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCompositionData>(false);
			__Game_Prefabs_PlaceableNetComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceableNetComposition>(false);
			__Game_Prefabs_RoadComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadComposition>(false);
			__Game_Prefabs_TrackComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackComposition>(false);
			__Game_Prefabs_WaterwayComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterwayComposition>(false);
			__Game_Prefabs_PathwayComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathwayComposition>(false);
			__Game_Prefabs_TaxiwayComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiwayComposition>(false);
			__Game_Prefabs_TerrainComposition_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TerrainComposition>(false);
			__Game_Prefabs_NetCompositionPiece_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionPiece>(false);
			__Game_Prefabs_NetCompositionLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionLane>(false);
			__Game_Prefabs_NetCompositionObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionObject>(false);
			__Game_Prefabs_NetCompositionArea_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionArea>(false);
			__Game_Prefabs_NetCompositionCrosswalk_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionCrosswalk>(false);
			__Game_Prefabs_NetCompositionCarriageway_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetCompositionCarriageway>(false);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetPieceData>(true);
			__Game_Prefabs_NetPieceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetPieceData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_NetCrosswalkData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCrosswalkData>(true);
			__Game_Prefabs_NetVertexMatchData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetVertexMatchData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Prefabs_TrackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrackData>(true);
			__Game_Prefabs_WaterwayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterwayData>(true);
			__Game_Prefabs_PathwayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathwayData>(true);
			__Game_Prefabs_TaxiwayData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TaxiwayData>(true);
			__Game_Prefabs_StreetLightData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StreetLightData>(true);
			__Game_Prefabs_LaneDirectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneDirectionData>(true);
			__Game_Prefabs_TrafficSignData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrafficSignData>(true);
			__Game_Prefabs_UtilityObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityObjectData>(true);
			__Game_Prefabs_MeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MeshData>(true);
			__Game_Prefabs_NetTerrainData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetTerrainData>(true);
			__Game_Prefabs_BridgeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BridgeData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Prefabs_NetPieceLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetPieceLane>(true);
			__Game_Prefabs_NetPieceObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetPieceObject>(true);
			__Game_Prefabs_NetPieceArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetPieceArea>(true);
		}
	}

	private EntityQuery m_CompositionQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CompositionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<NetCompositionData>(),
			ComponentType.ReadWrite<NetCompositionPiece>(),
			ComponentType.ReadOnly<NetCompositionMeshRef>(),
			ComponentType.ReadOnly<Created>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CompositionQuery);
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
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		InitializeCompositionJob initializeCompositionJob = new InitializeCompositionJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionType = InternalCompilerInterface.GetComponentTypeHandle<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetCompositionType = InternalCompilerInterface.GetComponentTypeHandle<PlaceableNetComposition>(ref __TypeHandle.__Game_Prefabs_PlaceableNetComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadCompositionType = InternalCompilerInterface.GetComponentTypeHandle<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TrackCompositionType = InternalCompilerInterface.GetComponentTypeHandle<TrackComposition>(ref __TypeHandle.__Game_Prefabs_TrackComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayCompositionType = InternalCompilerInterface.GetComponentTypeHandle<WaterwayComposition>(ref __TypeHandle.__Game_Prefabs_WaterwayComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathwayCompositionType = InternalCompilerInterface.GetComponentTypeHandle<PathwayComposition>(ref __TypeHandle.__Game_Prefabs_PathwayComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiwayCompositionType = InternalCompilerInterface.GetComponentTypeHandle<TaxiwayComposition>(ref __TypeHandle.__Game_Prefabs_TaxiwayComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainCompositionType = InternalCompilerInterface.GetComponentTypeHandle<TerrainComposition>(ref __TypeHandle.__Game_Prefabs_TerrainComposition_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionPieceType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionPiece>(ref __TypeHandle.__Game_Prefabs_NetCompositionPiece_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionLaneType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionLane>(ref __TypeHandle.__Game_Prefabs_NetCompositionLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionObjectType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionObject>(ref __TypeHandle.__Game_Prefabs_NetCompositionObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionAreaType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionArea>(ref __TypeHandle.__Game_Prefabs_NetCompositionArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionCrosswalkType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionCrosswalk>(ref __TypeHandle.__Game_Prefabs_NetCompositionCrosswalk_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionCarriagewayType = InternalCompilerInterface.GetBufferTypeHandle<NetCompositionCarriageway>(ref __TypeHandle.__Game_Prefabs_NetCompositionCarriageway_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetPieceData = InternalCompilerInterface.GetComponentLookup<PlaceableNetPieceData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceData = InternalCompilerInterface.GetComponentLookup<NetPieceData>(ref __TypeHandle.__Game_Prefabs_NetPieceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCrosswalkData = InternalCompilerInterface.GetComponentLookup<NetCrosswalkData>(ref __TypeHandle.__Game_Prefabs_NetCrosswalkData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetVertexMatchData = InternalCompilerInterface.GetComponentLookup<NetVertexMatchData>(ref __TypeHandle.__Game_Prefabs_NetVertexMatchData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrackData = InternalCompilerInterface.GetComponentLookup<TrackData>(ref __TypeHandle.__Game_Prefabs_TrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterwayData = InternalCompilerInterface.GetComponentLookup<WaterwayData>(ref __TypeHandle.__Game_Prefabs_WaterwayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathwayData = InternalCompilerInterface.GetComponentLookup<PathwayData>(ref __TypeHandle.__Game_Prefabs_PathwayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiwayData = InternalCompilerInterface.GetComponentLookup<TaxiwayData>(ref __TypeHandle.__Game_Prefabs_TaxiwayData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StreetLightData = InternalCompilerInterface.GetComponentLookup<StreetLightData>(ref __TypeHandle.__Game_Prefabs_StreetLightData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneDirectionData = InternalCompilerInterface.GetComponentLookup<LaneDirectionData>(ref __TypeHandle.__Game_Prefabs_LaneDirectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrafficSignData = InternalCompilerInterface.GetComponentLookup<TrafficSignData>(ref __TypeHandle.__Game_Prefabs_TrafficSignData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityObjectData = InternalCompilerInterface.GetComponentLookup<UtilityObjectData>(ref __TypeHandle.__Game_Prefabs_UtilityObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshData = InternalCompilerInterface.GetComponentLookup<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainData = InternalCompilerInterface.GetComponentLookup<NetTerrainData>(ref __TypeHandle.__Game_Prefabs_NetTerrainData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BridgeData = InternalCompilerInterface.GetComponentLookup<BridgeData>(ref __TypeHandle.__Game_Prefabs_BridgeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceLanes = InternalCompilerInterface.GetBufferLookup<NetPieceLane>(ref __TypeHandle.__Game_Prefabs_NetPieceLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceObjects = InternalCompilerInterface.GetBufferLookup<NetPieceObject>(ref __TypeHandle.__Game_Prefabs_NetPieceObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceAreas = InternalCompilerInterface.GetBufferLookup<NetPieceArea>(ref __TypeHandle.__Game_Prefabs_NetPieceArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeCompositionJob>(initializeCompositionJob, m_CompositionQuery, ((SystemBase)this).Dependency);
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
	public NetCompositionSystem()
	{
	}
}
