using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Rendering;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class NetInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct FixPlaceholdersJob : IJobChunk
	{
		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		public BufferTypeHandle<PlaceholderObjectElement> m_PlaceholderObjectElementType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<PlaceholderObjectElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PlaceholderObjectElement>(ref m_PlaceholderObjectElementType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<PlaceholderObjectElement> val = bufferAccessor[i];
				for (int j = 0; j < val.Length; j++)
				{
					if (m_DeletedData.HasComponent(val[j].m_Object))
					{
						val.RemoveAtSwapBack(j--);
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
	private struct InitializeNetDefaultsJob : IJobParallelFor
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public BufferTypeHandle<NetGeometrySection> m_NetGeometrySectionType;

		public ComponentTypeHandle<NetData> m_NetType;

		public ComponentTypeHandle<NetGeometryData> m_NetGeometryType;

		public ComponentTypeHandle<PlaceableNetData> m_PlaceableNetType;

		public ComponentTypeHandle<RoadData> m_RoadType;

		public BufferTypeHandle<DefaultNetLane> m_DefaultNetLaneType;

		[ReadOnly]
		public ComponentLookup<NetPieceData> m_NetPieceData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<NetVertexMatchData> m_NetVertexMatchData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetPieceData> m_PlaceableNetPieceData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectData;

		[ReadOnly]
		public BufferLookup<NetSubSection> m_NetSubSectionData;

		[ReadOnly]
		public BufferLookup<NetSectionPiece> m_NetSectionPieceData;

		[ReadOnly]
		public BufferLookup<NetPieceLane> m_NetPieceLanes;

		[ReadOnly]
		public BufferLookup<NetPieceObject> m_NetPieceObjects;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			NativeArray<NetGeometryData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<NetGeometryData>(ref m_NetGeometryType);
			if (nativeArray.Length == 0)
			{
				return;
			}
			NativeArray<NetData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetData>(ref m_NetType);
			NativeArray<PlaceableNetData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<PlaceableNetData>(ref m_PlaceableNetType);
			NativeArray<RoadData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<RoadData>(ref m_RoadType);
			BufferAccessor<DefaultNetLane> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<DefaultNetLane>(ref m_DefaultNetLaneType);
			BufferAccessor<NetGeometrySection> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetGeometrySection>(ref m_NetGeometrySectionType);
			NativeList<NetCompositionPiece> val2 = default(NativeList<NetCompositionPiece>);
			val2._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<NetCompositionLane> netLanes = default(NativeList<NetCompositionLane>);
			netLanes._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			CompositionFlags flags = default(CompositionFlags);
			CompositionFlags flags2 = new CompositionFlags(CompositionFlags.General.Elevated, (CompositionFlags.Side)0u, (CompositionFlags.Side)0u);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<NetGeometrySection> geometrySections = bufferAccessor2[i];
				NetCompositionHelpers.GetCompositionPieces(val2, geometrySections.AsNativeArray(), flags, m_NetSubSectionData, m_NetSectionPieceData);
				NetCompositionData compositionData = default(NetCompositionData);
				NetCompositionHelpers.CalculateCompositionData(ref compositionData, val2.AsArray(), m_NetPieceData, m_NetLaneData, m_NetVertexMatchData, m_NetPieceLanes);
				NetCompositionHelpers.AddCompositionLanes<NativeList<NetCompositionPiece>>(Entity.Null, ref compositionData, val2, netLanes, default(DynamicBuffer<NetCompositionCarriageway>), m_NetLaneData, m_NetPieceLanes);
				if (bufferAccessor.Length != 0)
				{
					DynamicBuffer<DefaultNetLane> val3 = bufferAccessor[i];
					val3.ResizeUninitialized(netLanes.Length);
					for (int j = 0; j < netLanes.Length; j++)
					{
						val3[j] = new DefaultNetLane(netLanes[j]);
					}
				}
				NetData netData = nativeArray2[i];
				netData.m_NodePriority += compositionData.m_Width;
				NetGeometryData netGeometryData = nativeArray[i];
				netGeometryData.m_DefaultWidth = compositionData.m_Width;
				netGeometryData.m_DefaultHeightRange = compositionData.m_HeightRange;
				netGeometryData.m_DefaultSurfaceHeight = compositionData.m_SurfaceHeight;
				UpdateFlagMasks(ref netData, geometrySections);
				if ((netData.m_RequiredLayers & (Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad)) != Layer.None)
				{
					netData.m_GeneralFlagMask |= CompositionFlags.General.TrafficLights | CompositionFlags.General.RemoveTrafficLights;
					netData.m_SideFlagMask |= CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk;
				}
				if ((netData.m_RequiredLayers & (Layer.Road | Layer.PublicTransportRoad)) != Layer.None)
				{
					netData.m_GeneralFlagMask |= CompositionFlags.General.AllWayStop;
					netData.m_SideFlagMask |= CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.ForbidStraight;
				}
				bool num = (compositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasForwardTrackLanes)) != 0;
				bool flag = (compositionData.m_State & (CompositionState.HasBackwardRoadLanes | CompositionState.HasBackwardTrackLanes)) != 0;
				if (num != flag)
				{
					netGeometryData.m_Flags |= GeometryFlags.FlipTrafficHandedness;
				}
				if ((compositionData.m_State & CompositionState.Asymmetric) != 0)
				{
					netGeometryData.m_Flags |= GeometryFlags.Asymmetric;
				}
				if ((compositionData.m_State & CompositionState.ExclusiveGround) != 0)
				{
					netGeometryData.m_Flags |= GeometryFlags.ExclusiveGround;
				}
				if (nativeArray3.Length != 0 && (netGeometryData.m_Flags & GeometryFlags.RequireElevated) == 0)
				{
					PlaceableNetComposition placeableData = default(PlaceableNetComposition);
					NetCompositionHelpers.CalculatePlaceableData(ref placeableData, val2.AsArray(), m_PlaceableNetPieceData);
					AddObjectCosts(ref placeableData, val2);
					PlaceableNetData placeableNetData = nativeArray3[i];
					placeableNetData.m_DefaultConstructionCost = placeableData.m_ConstructionCost;
					placeableNetData.m_DefaultUpkeepCost = placeableData.m_UpkeepCost;
					nativeArray3[i] = placeableNetData;
				}
				if (nativeArray4.Length != 0)
				{
					RoadData roadData = nativeArray4[i];
					if ((compositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes)) == CompositionState.HasForwardRoadLanes)
					{
						roadData.m_Flags |= RoadFlags.DefaultIsForward;
					}
					else if ((compositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes)) == CompositionState.HasBackwardRoadLanes)
					{
						roadData.m_Flags |= RoadFlags.DefaultIsBackward;
					}
					if ((roadData.m_Flags & RoadFlags.UseHighwayRules) != 0)
					{
						netGeometryData.m_MinNodeOffset += netGeometryData.m_DefaultWidth * 0.5f;
					}
					nativeArray4[i] = roadData;
				}
				val2.Clear();
				NetCompositionHelpers.GetCompositionPieces(val2, geometrySections.AsNativeArray(), flags2, m_NetSubSectionData, m_NetSectionPieceData);
				NetCompositionData compositionData2 = default(NetCompositionData);
				NetCompositionHelpers.CalculateCompositionData(ref compositionData2, val2.AsArray(), m_NetPieceData, m_NetLaneData, m_NetVertexMatchData, m_NetPieceLanes);
				netGeometryData.m_ElevatedWidth = compositionData2.m_Width;
				netGeometryData.m_ElevatedHeightRange = compositionData2.m_HeightRange;
				if (nativeArray3.Length != 0 && (netGeometryData.m_Flags & GeometryFlags.RequireElevated) != 0)
				{
					PlaceableNetComposition placeableData2 = default(PlaceableNetComposition);
					NetCompositionHelpers.CalculatePlaceableData(ref placeableData2, val2.AsArray(), m_PlaceableNetPieceData);
					AddObjectCosts(ref placeableData2, val2);
					PlaceableNetData placeableNetData2 = nativeArray3[i];
					placeableNetData2.m_DefaultConstructionCost = placeableData2.m_ConstructionCost;
					placeableNetData2.m_DefaultUpkeepCost = placeableData2.m_UpkeepCost;
					nativeArray3[i] = placeableNetData2;
				}
				nativeArray2[i] = netData;
				nativeArray[i] = netGeometryData;
				val2.Clear();
				netLanes.Clear();
			}
			val2.Dispose();
			netLanes.Dispose();
		}

		private void UpdateFlagMasks(ref NetData netData, DynamicBuffer<NetGeometrySection> geometrySections)
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < geometrySections.Length; i++)
			{
				NetGeometrySection netGeometrySection = geometrySections[i];
				netData.m_GeneralFlagMask |= netGeometrySection.m_CompositionAll.m_General;
				netData.m_SideFlagMask |= netGeometrySection.m_CompositionAll.m_Left | netGeometrySection.m_CompositionAll.m_Right;
				netData.m_GeneralFlagMask |= netGeometrySection.m_CompositionAny.m_General;
				netData.m_SideFlagMask |= netGeometrySection.m_CompositionAny.m_Left | netGeometrySection.m_CompositionAny.m_Right;
				netData.m_GeneralFlagMask |= netGeometrySection.m_CompositionNone.m_General;
				netData.m_SideFlagMask |= netGeometrySection.m_CompositionNone.m_Left | netGeometrySection.m_CompositionNone.m_Right;
				UpdateFlagMasks(ref netData, netGeometrySection.m_Section);
			}
		}

		private void UpdateFlagMasks(ref NetData netData, Entity section)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<NetSubSection> val = default(DynamicBuffer<NetSubSection>);
			if (m_NetSubSectionData.TryGetBuffer(section, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					NetSubSection netSubSection = val[i];
					netData.m_GeneralFlagMask |= netSubSection.m_CompositionAll.m_General;
					netData.m_SideFlagMask |= netSubSection.m_CompositionAll.m_Left | netSubSection.m_CompositionAll.m_Right;
					netData.m_GeneralFlagMask |= netSubSection.m_CompositionAny.m_General;
					netData.m_SideFlagMask |= netSubSection.m_CompositionAny.m_Left | netSubSection.m_CompositionAny.m_Right;
					netData.m_GeneralFlagMask |= netSubSection.m_CompositionNone.m_General;
					netData.m_SideFlagMask |= netSubSection.m_CompositionNone.m_Left | netSubSection.m_CompositionNone.m_Right;
					UpdateFlagMasks(ref netData, netSubSection.m_SubSection);
				}
			}
			DynamicBuffer<NetSectionPiece> val2 = default(DynamicBuffer<NetSectionPiece>);
			if (!m_NetSectionPieceData.TryGetBuffer(section, ref val2))
			{
				return;
			}
			DynamicBuffer<NetPieceObject> val3 = default(DynamicBuffer<NetPieceObject>);
			for (int j = 0; j < val2.Length; j++)
			{
				NetSectionPiece netSectionPiece = val2[j];
				netData.m_GeneralFlagMask |= netSectionPiece.m_CompositionAll.m_General;
				netData.m_SideFlagMask |= netSectionPiece.m_CompositionAll.m_Left | netSectionPiece.m_CompositionAll.m_Right;
				netData.m_GeneralFlagMask |= netSectionPiece.m_CompositionAny.m_General;
				netData.m_SideFlagMask |= netSectionPiece.m_CompositionAny.m_Left | netSectionPiece.m_CompositionAny.m_Right;
				netData.m_GeneralFlagMask |= netSectionPiece.m_CompositionNone.m_General;
				netData.m_SideFlagMask |= netSectionPiece.m_CompositionNone.m_Left | netSectionPiece.m_CompositionNone.m_Right;
				if (m_NetPieceObjects.TryGetBuffer(netSectionPiece.m_Piece, ref val3))
				{
					for (int k = 0; k < val3.Length; k++)
					{
						NetPieceObject netPieceObject = val3[k];
						netData.m_GeneralFlagMask |= netPieceObject.m_CompositionAll.m_General;
						netData.m_SideFlagMask |= netPieceObject.m_CompositionAll.m_Left | netPieceObject.m_CompositionAll.m_Right;
						netData.m_GeneralFlagMask |= netPieceObject.m_CompositionAny.m_General;
						netData.m_SideFlagMask |= netPieceObject.m_CompositionAny.m_Left | netPieceObject.m_CompositionAny.m_Right;
						netData.m_GeneralFlagMask |= netPieceObject.m_CompositionNone.m_General;
						netData.m_SideFlagMask |= netPieceObject.m_CompositionNone.m_Left | netPieceObject.m_CompositionNone.m_Right;
					}
				}
			}
		}

		private void AddObjectCosts(ref PlaceableNetComposition placeableCompositionData, NativeList<NetCompositionPiece> pieceBuffer)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < pieceBuffer.Length; i++)
			{
				NetCompositionPiece netCompositionPiece = pieceBuffer[i];
				if (!m_NetPieceObjects.HasBuffer(netCompositionPiece.m_Piece))
				{
					continue;
				}
				DynamicBuffer<NetPieceObject> val = m_NetPieceObjects[netCompositionPiece.m_Piece];
				for (int j = 0; j < val.Length; j++)
				{
					NetPieceObject netPieceObject = val[j];
					if (m_PlaceableObjectData.HasComponent(netPieceObject.m_Prefab))
					{
						uint num = m_PlaceableObjectData[netPieceObject.m_Prefab].m_ConstructionCost;
						if (netPieceObject.m_Spacing.z > 0.1f)
						{
							num = (uint)Mathf.RoundToInt((float)num * (8f / netPieceObject.m_Spacing.z));
						}
						placeableCompositionData.m_ConstructionCost += num;
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct CollectPathfindDataJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<NetLaneData> m_NetLaneDataType;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLaneData> m_ConnectionLaneDataType;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> m_PathfindCarData;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> m_PathfindPedestrianData;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> m_PathfindTrackData;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> m_PathfindTransportData;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> m_PathfindConnectionData;

		public NativeValue<PathfindHeuristicData> m_PathfindHeuristicData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<NetLaneData> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetLaneData>(ref m_NetLaneDataType);
			PathfindHeuristicData value = m_PathfindHeuristicData.value;
			if (((ArchetypeChunk)(ref chunk)).Has<ConnectionLaneData>(ref m_ConnectionLaneDataType))
			{
				PathfindConnectionData pathfindConnectionData = default(PathfindConnectionData);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					NetLaneData netLaneData = nativeArray[i];
					if (m_PathfindConnectionData.TryGetComponent(netLaneData.m_PathfindPrefab, ref pathfindConnectionData))
					{
						value.m_FlyingCosts.m_Value = math.min(value.m_FlyingCosts.m_Value, pathfindConnectionData.m_AirwayCost.m_Value);
						value.m_OffRoadCosts.m_Value = math.min(value.m_OffRoadCosts.m_Value, pathfindConnectionData.m_AreaCost.m_Value);
					}
				}
			}
			else
			{
				PathfindCarData pathfindCarData = default(PathfindCarData);
				PathfindTransportData pathfindTransportData = default(PathfindTransportData);
				PathfindTrackData pathfindTrackData = default(PathfindTrackData);
				PathfindPedestrianData pathfindPedestrianData = default(PathfindPedestrianData);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					NetLaneData netLaneData2 = nativeArray[j];
					if ((netLaneData2.m_Flags & LaneFlags.Road) != 0)
					{
						if (m_PathfindCarData.TryGetComponent(netLaneData2.m_PathfindPrefab, ref pathfindCarData))
						{
							value.m_CarCosts.m_Value = math.min(value.m_CarCosts.m_Value, pathfindCarData.m_DrivingCost.m_Value);
						}
						if (m_PathfindTransportData.TryGetComponent(netLaneData2.m_PathfindPrefab, ref pathfindTransportData))
						{
							value.m_TaxiCosts.m_Value = math.min(value.m_TaxiCosts.m_Value, pathfindTransportData.m_TravelCost.m_Value);
						}
					}
					if ((netLaneData2.m_Flags & LaneFlags.Track) != 0 && m_PathfindTrackData.TryGetComponent(netLaneData2.m_PathfindPrefab, ref pathfindTrackData))
					{
						value.m_TrackCosts.m_Value = math.min(value.m_TrackCosts.m_Value, pathfindTrackData.m_DrivingCost.m_Value);
					}
					if ((netLaneData2.m_Flags & LaneFlags.Pedestrian) != 0 && m_PathfindPedestrianData.TryGetComponent(netLaneData2.m_PathfindPrefab, ref pathfindPedestrianData))
					{
						value.m_PedestrianCosts.m_Value = math.min(value.m_PedestrianCosts.m_Value, pathfindPedestrianData.m_WalkingCost.m_Value);
					}
				}
			}
			m_PathfindHeuristicData.value = value;
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
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<NetData> __Game_Prefabs_NetData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetPieceData> __Game_Prefabs_NetPieceData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetGeometryData> __Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MarkerNetData> __Game_Prefabs_MarkerNetData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<LocalConnectData> __Game_Prefabs_LocalConnectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetLaneData> __Game_Prefabs_NetLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetLaneGeometryData> __Game_Prefabs_NetLaneGeometryData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarLaneData> __Game_Prefabs_CarLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TrackLaneData> __Game_Prefabs_TrackLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ParkingLaneData> __Game_Prefabs_ParkingLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PedestrianLaneData> __Game_Prefabs_PedestrianLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<SecondaryLaneData> __Game_Prefabs_SecondaryLaneData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetCrosswalkData> __Game_Prefabs_NetCrosswalkData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<RoadData> __Game_Prefabs_RoadData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TrackData> __Game_Prefabs_TrackData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WaterwayData> __Game_Prefabs_WaterwayData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathwayData> __Game_Prefabs_PathwayData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<TaxiwayData> __Game_Prefabs_TaxiwayData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PowerLineData> __Game_Prefabs_PowerLineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PipelineData> __Game_Prefabs_PipelineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<FenceData> __Game_Prefabs_FenceData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EditorContainerData> __Game_Prefabs_EditorContainerData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ElectricityConnectionData> __Game_Prefabs_ElectricityConnectionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<WaterPipeConnectionData> __Game_Prefabs_WaterPipeConnectionData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<ResourceConnectionData> __Game_Prefabs_ResourceConnectionData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BridgeData> __Game_Prefabs_BridgeData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NetTerrainData> __Game_Prefabs_NetTerrainData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<UIObjectData> __Game_Prefabs_UIObjectData_RO_ComponentTypeHandle;

		public BufferTypeHandle<NetSubSection> __Game_Prefabs_NetSubSection_RW_BufferTypeHandle;

		public BufferTypeHandle<NetSectionPiece> __Game_Prefabs_NetSectionPiece_RW_BufferTypeHandle;

		public BufferTypeHandle<NetPieceLane> __Game_Prefabs_NetPieceLane_RW_BufferTypeHandle;

		public BufferTypeHandle<NetPieceArea> __Game_Prefabs_NetPieceArea_RW_BufferTypeHandle;

		public BufferTypeHandle<NetPieceObject> __Game_Prefabs_NetPieceObject_RW_BufferTypeHandle;

		public BufferTypeHandle<NetGeometrySection> __Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle;

		public BufferTypeHandle<NetGeometryEdgeState> __Game_Prefabs_NetGeometryEdgeState_RW_BufferTypeHandle;

		public BufferTypeHandle<NetGeometryNodeState> __Game_Prefabs_NetGeometryNodeState_RW_BufferTypeHandle;

		public BufferTypeHandle<SubObject> __Game_Prefabs_SubObject_RW_BufferTypeHandle;

		public BufferTypeHandle<SubMesh> __Game_Prefabs_SubMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<FixedNetElement> __Game_Prefabs_FixedNetElement_RW_BufferTypeHandle;

		public BufferTypeHandle<AuxiliaryNetLane> __Game_Prefabs_AuxiliaryNetLane_RW_BufferTypeHandle;

		public BufferTypeHandle<AuxiliaryNet> __Game_Prefabs_AuxiliaryNet_RW_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<NetGeometrySection> __Game_Prefabs_NetGeometrySection_RO_BufferTypeHandle;

		public BufferTypeHandle<DefaultNetLane> __Game_Prefabs_DefaultNetLane_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<NetPieceData> __Game_Prefabs_NetPieceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetVertexMatchData> __Game_Prefabs_NetVertexMatchData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetPieceData> __Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<NetSubSection> __Game_Prefabs_NetSubSection_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetSectionPiece> __Game_Prefabs_NetSectionPiece_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetPieceLane> __Game_Prefabs_NetPieceLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetPieceObject> __Game_Prefabs_NetPieceObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ConnectionLaneData> __Game_Prefabs_ConnectionLaneData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PathfindCarData> __Game_Prefabs_PathfindCarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTrackData> __Game_Prefabs_PathfindTrackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindPedestrianData> __Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindTransportData> __Game_Prefabs_PathfindTransportData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathfindConnectionData> __Game_Prefabs_PathfindConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		public BufferTypeHandle<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle;

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
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_NetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetData>(false);
			__Game_Prefabs_NetPieceData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetPieceData>(false);
			__Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetGeometryData>(false);
			__Game_Prefabs_PlaceableNetData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PlaceableNetData>(false);
			__Game_Prefabs_MarkerNetData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MarkerNetData>(true);
			__Game_Prefabs_LocalConnectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LocalConnectData>(false);
			__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneData>(false);
			__Game_Prefabs_NetLaneGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneGeometryData>(false);
			__Game_Prefabs_CarLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarLaneData>(false);
			__Game_Prefabs_TrackLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackLaneData>(false);
			__Game_Prefabs_UtilityLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UtilityLaneData>(false);
			__Game_Prefabs_ParkingLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ParkingLaneData>(false);
			__Game_Prefabs_PedestrianLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PedestrianLaneData>(false);
			__Game_Prefabs_SecondaryLaneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SecondaryLaneData>(false);
			__Game_Prefabs_NetCrosswalkData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCrosswalkData>(false);
			__Game_Prefabs_RoadData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RoadData>(false);
			__Game_Prefabs_TrackData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrackData>(false);
			__Game_Prefabs_WaterwayData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterwayData>(false);
			__Game_Prefabs_PathwayData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathwayData>(false);
			__Game_Prefabs_TaxiwayData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TaxiwayData>(false);
			__Game_Prefabs_PowerLineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PowerLineData>(true);
			__Game_Prefabs_PipelineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PipelineData>(true);
			__Game_Prefabs_FenceData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<FenceData>(true);
			__Game_Prefabs_EditorContainerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EditorContainerData>(true);
			__Game_Prefabs_ElectricityConnectionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ElectricityConnectionData>(false);
			__Game_Prefabs_WaterPipeConnectionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WaterPipeConnectionData>(false);
			__Game_Prefabs_ResourceConnectionData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ResourceConnectionData>(false);
			__Game_Prefabs_BridgeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BridgeData>(true);
			__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableObjectData>(false);
			__Game_Prefabs_NetTerrainData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetTerrainData>(false);
			__Game_Prefabs_UIObjectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<UIObjectData>(true);
			__Game_Prefabs_NetSubSection_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetSubSection>(false);
			__Game_Prefabs_NetSectionPiece_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetSectionPiece>(false);
			__Game_Prefabs_NetPieceLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetPieceLane>(false);
			__Game_Prefabs_NetPieceArea_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetPieceArea>(false);
			__Game_Prefabs_NetPieceObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetPieceObject>(false);
			__Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetGeometrySection>(false);
			__Game_Prefabs_NetGeometryEdgeState_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetGeometryEdgeState>(false);
			__Game_Prefabs_NetGeometryNodeState_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetGeometryNodeState>(false);
			__Game_Prefabs_SubObject_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubObject>(false);
			__Game_Prefabs_SubMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubMesh>(false);
			__Game_Prefabs_FixedNetElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<FixedNetElement>(false);
			__Game_Prefabs_AuxiliaryNetLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AuxiliaryNetLane>(false);
			__Game_Prefabs_AuxiliaryNet_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<AuxiliaryNet>(false);
			__Game_Prefabs_NetGeometrySection_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<NetGeometrySection>(true);
			__Game_Prefabs_DefaultNetLane_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<DefaultNetLane>(false);
			__Game_Prefabs_NetPieceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetPieceData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Prefabs_NetVertexMatchData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetVertexMatchData>(true);
			__Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetPieceData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_NetSubSection_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetSubSection>(true);
			__Game_Prefabs_NetSectionPiece_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetSectionPiece>(true);
			__Game_Prefabs_NetPieceLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetPieceLane>(true);
			__Game_Prefabs_NetPieceObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetPieceObject>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetLaneData>(true);
			__Game_Prefabs_ConnectionLaneData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ConnectionLaneData>(true);
			__Game_Prefabs_PathfindCarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindCarData>(true);
			__Game_Prefabs_PathfindTrackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTrackData>(true);
			__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindPedestrianData>(true);
			__Game_Prefabs_PathfindTransportData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindTransportData>(true);
			__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathfindConnectionData>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PlaceholderObjectElement>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_LaneQuery;

	private EntityQuery m_PlaceholderQuery;

	private NativeValue<PathfindHeuristicData> m_PathfindHeuristicData;

	private JobHandle m_PathfindHeuristicDeps;

	private Layer m_InGameLayersOnce;

	private Layer m_InGameLayersTwice;

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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<NetData>(),
			ComponentType.ReadWrite<NetSectionData>(),
			ComponentType.ReadWrite<NetPieceData>(),
			ComponentType.ReadWrite<NetLaneData>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<NetLaneData>() };
		array[1] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_LaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NetLaneData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PlaceholderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<NetLaneData>(),
			ComponentType.ReadOnly<PlaceholderObjectElement>(),
			ComponentType.Exclude<Deleted>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
		m_PathfindHeuristicData = new NativeValue<PathfindHeuristicData>((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_PathfindHeuristicData.Dispose();
		base.OnDestroy();
	}

	public PathfindHeuristicData GetHeuristicData()
	{
		((JobHandle)(ref m_PathfindHeuristicDeps)).Complete();
		return m_PathfindHeuristicData.value;
	}

	public bool CanReplace(NetData netData, bool inGame)
	{
		if (!inGame)
		{
			return true;
		}
		return (netData.m_RequiredLayers & m_InGameLayersOnce & ~m_InGameLayersTwice) == 0;
	}

	private void AddSections(PrefabBase prefab, NetSectionInfo[] source, DynamicBuffer<NetGeometrySection> target, NetSectionFlags flags)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		int2 val = default(int2);
		((int2)(ref val))._002Ector(int.MaxValue, int.MinValue);
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i].m_Median)
			{
				int num = i << 1;
				val.x = math.min(val.x, num);
				val.y = math.max(val.y, num);
			}
		}
		if (((int2)(ref val)).Equals(new int2(int.MaxValue, int.MinValue)))
		{
			val = int2.op_Implicit(source.Length - 1);
			flags |= NetSectionFlags.AlignCenter;
		}
		for (int j = 0; j < source.Length; j++)
		{
			NetSectionInfo netSectionInfo = source[j];
			NetGeometrySection netGeometrySection = new NetGeometrySection
			{
				m_Section = m_PrefabSystem.GetEntity(netSectionInfo.m_Section),
				m_Offset = netSectionInfo.m_Offset,
				m_Flags = flags
			};
			NetCompositionHelpers.GetRequirementFlags(netSectionInfo.m_RequireAll, out netGeometrySection.m_CompositionAll, out var sectionFlags);
			NetCompositionHelpers.GetRequirementFlags(netSectionInfo.m_RequireAny, out netGeometrySection.m_CompositionAny, out var sectionFlags2);
			NetCompositionHelpers.GetRequirementFlags(netSectionInfo.m_RequireNone, out netGeometrySection.m_CompositionNone, out var sectionFlags3);
			NetSectionFlags netSectionFlags = sectionFlags | sectionFlags2 | sectionFlags3;
			if (netSectionFlags != 0)
			{
				COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, "Net section ({0}: {1}) cannot require section flags: {2}", (object)((Object)prefab).name, (object)((Object)netSectionInfo.m_Section).name, (object)netSectionFlags);
			}
			if (netSectionInfo.m_Invert)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.Invert;
			}
			if (netSectionInfo.m_Flip)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.FlipLanes | NetSectionFlags.FlipMesh;
			}
			if (netSectionInfo.m_HalfLength)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.HalfLength;
			}
			NetPieceLayerMask netPieceLayerMask = NetPieceLayerMask.Surface | NetPieceLayerMask.Bottom | NetPieceLayerMask.Top | NetPieceLayerMask.Side;
			if ((netSectionInfo.m_HiddenLayers & netPieceLayerMask) == netPieceLayerMask)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.Hidden;
			}
			if ((netSectionInfo.m_HiddenLayers & NetPieceLayerMask.Surface) != 0)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.HiddenSurface;
			}
			if ((netSectionInfo.m_HiddenLayers & NetPieceLayerMask.Bottom) != 0)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.HiddenBottom;
			}
			if ((netSectionInfo.m_HiddenLayers & NetPieceLayerMask.Top) != 0)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.HiddenTop;
			}
			if ((netSectionInfo.m_HiddenLayers & NetPieceLayerMask.Side) != 0)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.HiddenSide;
			}
			int num2 = j << 1;
			if (num2 >= val.x && num2 <= val.y)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.Median;
			}
			else if (num2 > val.y)
			{
				netGeometrySection.m_Flags |= NetSectionFlags.Right;
			}
			else
			{
				netGeometrySection.m_Flags |= NetSectionFlags.Left;
			}
			target.Add(netGeometrySection);
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_4961: Unknown result type (might be due to invalid IL or missing references)
		//IL_4962: Unknown result type (might be due to invalid IL or missing references)
		//IL_497e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4983: Unknown result type (might be due to invalid IL or missing references)
		//IL_499f: Unknown result type (might be due to invalid IL or missing references)
		//IL_49a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_49c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_49c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_49e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_49e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ac8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ba1: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_4be8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c04: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c25: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c83: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c90: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ccc: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ce8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cff: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d11: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_098f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_117c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1506: Unknown result type (might be due to invalid IL or missing references)
		//IL_150b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1193: Unknown result type (might be due to invalid IL or missing references)
		//IL_1198: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19be: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1522: Unknown result type (might be due to invalid IL or missing references)
		//IL_1528: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1533: Unknown result type (might be due to invalid IL or missing references)
		//IL_1538: Unknown result type (might be due to invalid IL or missing references)
		//IL_153e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1543: Unknown result type (might be due to invalid IL or missing references)
		//IL_1549: Unknown result type (might be due to invalid IL or missing references)
		//IL_154e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1124: Unknown result type (might be due to invalid IL or missing references)
		//IL_1129: Unknown result type (might be due to invalid IL or missing references)
		//IL_1132: Unknown result type (might be due to invalid IL or missing references)
		//IL_1137: Unknown result type (might be due to invalid IL or missing references)
		//IL_28e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_19db: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_157d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1582: Unknown result type (might be due to invalid IL or missing references)
		//IL_158b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1590: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2578: Unknown result type (might be due to invalid IL or missing references)
		//IL_257d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2583: Unknown result type (might be due to invalid IL or missing references)
		//IL_2588: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1600: Unknown result type (might be due to invalid IL or missing references)
		//IL_1605: Unknown result type (might be due to invalid IL or missing references)
		//IL_1205: Unknown result type (might be due to invalid IL or missing references)
		//IL_120a: Unknown result type (might be due to invalid IL or missing references)
		//IL_33b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_33b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_25c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_25c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_27dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_27e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1742: Unknown result type (might be due to invalid IL or missing references)
		//IL_1747: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1908: Unknown result type (might be due to invalid IL or missing references)
		//IL_190d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1916: Unknown result type (might be due to invalid IL or missing references)
		//IL_191b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1924: Unknown result type (might be due to invalid IL or missing references)
		//IL_1929: Unknown result type (might be due to invalid IL or missing references)
		//IL_1728: Unknown result type (might be due to invalid IL or missing references)
		//IL_162f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1634: Unknown result type (might be due to invalid IL or missing references)
		//IL_163d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_37c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_37c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_37cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_37d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1957: Unknown result type (might be due to invalid IL or missing references)
		//IL_195c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_196a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1973: Unknown result type (might be due to invalid IL or missing references)
		//IL_1978: Unknown result type (might be due to invalid IL or missing references)
		//IL_1981: Unknown result type (might be due to invalid IL or missing references)
		//IL_1986: Unknown result type (might be due to invalid IL or missing references)
		//IL_16de: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1689: Unknown result type (might be due to invalid IL or missing references)
		//IL_100c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1011: Unknown result type (might be due to invalid IL or missing references)
		//IL_1017: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_2613: Unknown result type (might be due to invalid IL or missing references)
		//IL_2618: Unknown result type (might be due to invalid IL or missing references)
		//IL_261b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2620: Unknown result type (might be due to invalid IL or missing references)
		//IL_2624: Unknown result type (might be due to invalid IL or missing references)
		//IL_262f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2634: Unknown result type (might be due to invalid IL or missing references)
		//IL_2639: Unknown result type (might be due to invalid IL or missing references)
		//IL_263f: Unknown result type (might be due to invalid IL or missing references)
		//IL_265f: Unknown result type (might be due to invalid IL or missing references)
		//IL_267f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2684: Unknown result type (might be due to invalid IL or missing references)
		//IL_2686: Unknown result type (might be due to invalid IL or missing references)
		//IL_268b: Unknown result type (might be due to invalid IL or missing references)
		//IL_27fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2801: Unknown result type (might be due to invalid IL or missing references)
		//IL_179a: Unknown result type (might be due to invalid IL or missing references)
		//IL_179f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_180d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1812: Unknown result type (might be due to invalid IL or missing references)
		//IL_181b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1820: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_382a: Unknown result type (might be due to invalid IL or missing references)
		//IL_36b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3222: Unknown result type (might be due to invalid IL or missing references)
		//IL_202b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2030: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ebf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ecd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ede: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2842: Unknown result type (might be due to invalid IL or missing references)
		//IL_2847: Unknown result type (might be due to invalid IL or missing references)
		//IL_284e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2853: Unknown result type (might be due to invalid IL or missing references)
		//IL_2857: Unknown result type (might be due to invalid IL or missing references)
		//IL_285e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2863: Unknown result type (might be due to invalid IL or missing references)
		//IL_286f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4065: Unknown result type (might be due to invalid IL or missing references)
		//IL_406a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_39bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_329d: Unknown result type (might be due to invalid IL or missing references)
		//IL_32a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_23fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2401: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f70: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_28b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_407c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4081: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_43cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_43d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_4646: Unknown result type (might be due to invalid IL or missing references)
		//IL_464b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_2454: Unknown result type (might be due to invalid IL or missing references)
		//IL_2459: Unknown result type (might be due to invalid IL or missing references)
		//IL_2462: Unknown result type (might be due to invalid IL or missing references)
		//IL_2467: Unknown result type (might be due to invalid IL or missing references)
		//IL_2470: Unknown result type (might be due to invalid IL or missing references)
		//IL_2475: Unknown result type (might be due to invalid IL or missing references)
		//IL_2725: Unknown result type (might be due to invalid IL or missing references)
		//IL_46eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_46f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_41d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_41de: Unknown result type (might be due to invalid IL or missing references)
		//IL_276e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2773: Unknown result type (might be due to invalid IL or missing references)
		//IL_2777: Unknown result type (might be due to invalid IL or missing references)
		//IL_4505: Unknown result type (might be due to invalid IL or missing references)
		//IL_450a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_213b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2140: Unknown result type (might be due to invalid IL or missing references)
		//IL_2149: Unknown result type (might be due to invalid IL or missing references)
		//IL_214e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ee2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f36: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2212: Unknown result type (might be due to invalid IL or missing references)
		//IL_2217: Unknown result type (might be due to invalid IL or missing references)
		//IL_221a: Unknown result type (might be due to invalid IL or missing references)
		//IL_221f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2223: Unknown result type (might be due to invalid IL or missing references)
		//IL_2226: Unknown result type (might be due to invalid IL or missing references)
		//IL_222b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2239: Unknown result type (might be due to invalid IL or missing references)
		//IL_223b: Unknown result type (might be due to invalid IL or missing references)
		//IL_238f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2394: Unknown result type (might be due to invalid IL or missing references)
		//IL_2397: Unknown result type (might be due to invalid IL or missing references)
		//IL_239c: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_22a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_22ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_22b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_22b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_22b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_22bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_22d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_22da: Unknown result type (might be due to invalid IL or missing references)
		//IL_2329: Unknown result type (might be due to invalid IL or missing references)
		//IL_232b: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		bool flag = false;
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetPieceData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<NetPieceData>(ref __TypeHandle.__Game_Prefabs_NetPieceData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetGeometryData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PlaceableNetData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<MarkerNetData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<MarkerNetData>(ref __TypeHandle.__Game_Prefabs_MarkerNetData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<LocalConnectData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetLaneData> componentTypeHandle9 = InternalCompilerInterface.GetComponentTypeHandle<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetLaneGeometryData> componentTypeHandle10 = InternalCompilerInterface.GetComponentTypeHandle<NetLaneGeometryData>(ref __TypeHandle.__Game_Prefabs_NetLaneGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<CarLaneData> componentTypeHandle11 = InternalCompilerInterface.GetComponentTypeHandle<CarLaneData>(ref __TypeHandle.__Game_Prefabs_CarLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<TrackLaneData> componentTypeHandle12 = InternalCompilerInterface.GetComponentTypeHandle<TrackLaneData>(ref __TypeHandle.__Game_Prefabs_TrackLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<UtilityLaneData> componentTypeHandle13 = InternalCompilerInterface.GetComponentTypeHandle<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ParkingLaneData> componentTypeHandle14 = InternalCompilerInterface.GetComponentTypeHandle<ParkingLaneData>(ref __TypeHandle.__Game_Prefabs_ParkingLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PedestrianLaneData> componentTypeHandle15 = InternalCompilerInterface.GetComponentTypeHandle<PedestrianLaneData>(ref __TypeHandle.__Game_Prefabs_PedestrianLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SecondaryLaneData> componentTypeHandle16 = InternalCompilerInterface.GetComponentTypeHandle<SecondaryLaneData>(ref __TypeHandle.__Game_Prefabs_SecondaryLaneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetCrosswalkData> componentTypeHandle17 = InternalCompilerInterface.GetComponentTypeHandle<NetCrosswalkData>(ref __TypeHandle.__Game_Prefabs_NetCrosswalkData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<RoadData> componentTypeHandle18 = InternalCompilerInterface.GetComponentTypeHandle<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<TrackData> componentTypeHandle19 = InternalCompilerInterface.GetComponentTypeHandle<TrackData>(ref __TypeHandle.__Game_Prefabs_TrackData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<WaterwayData> componentTypeHandle20 = InternalCompilerInterface.GetComponentTypeHandle<WaterwayData>(ref __TypeHandle.__Game_Prefabs_WaterwayData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PathwayData> componentTypeHandle21 = InternalCompilerInterface.GetComponentTypeHandle<PathwayData>(ref __TypeHandle.__Game_Prefabs_PathwayData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<TaxiwayData> componentTypeHandle22 = InternalCompilerInterface.GetComponentTypeHandle<TaxiwayData>(ref __TypeHandle.__Game_Prefabs_TaxiwayData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PowerLineData> componentTypeHandle23 = InternalCompilerInterface.GetComponentTypeHandle<PowerLineData>(ref __TypeHandle.__Game_Prefabs_PowerLineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PipelineData> componentTypeHandle24 = InternalCompilerInterface.GetComponentTypeHandle<PipelineData>(ref __TypeHandle.__Game_Prefabs_PipelineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<FenceData> componentTypeHandle25 = InternalCompilerInterface.GetComponentTypeHandle<FenceData>(ref __TypeHandle.__Game_Prefabs_FenceData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<EditorContainerData> componentTypeHandle26 = InternalCompilerInterface.GetComponentTypeHandle<EditorContainerData>(ref __TypeHandle.__Game_Prefabs_EditorContainerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ElectricityConnectionData> componentTypeHandle27 = InternalCompilerInterface.GetComponentTypeHandle<ElectricityConnectionData>(ref __TypeHandle.__Game_Prefabs_ElectricityConnectionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<WaterPipeConnectionData> componentTypeHandle28 = InternalCompilerInterface.GetComponentTypeHandle<WaterPipeConnectionData>(ref __TypeHandle.__Game_Prefabs_WaterPipeConnectionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<ResourceConnectionData> componentTypeHandle29 = InternalCompilerInterface.GetComponentTypeHandle<ResourceConnectionData>(ref __TypeHandle.__Game_Prefabs_ResourceConnectionData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<BridgeData> componentTypeHandle30 = InternalCompilerInterface.GetComponentTypeHandle<BridgeData>(ref __TypeHandle.__Game_Prefabs_BridgeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<SpawnableObjectData> componentTypeHandle31 = InternalCompilerInterface.GetComponentTypeHandle<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NetTerrainData> componentTypeHandle32 = InternalCompilerInterface.GetComponentTypeHandle<NetTerrainData>(ref __TypeHandle.__Game_Prefabs_NetTerrainData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<UIObjectData> componentTypeHandle33 = InternalCompilerInterface.GetComponentTypeHandle<UIObjectData>(ref __TypeHandle.__Game_Prefabs_UIObjectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetSubSection> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<NetSubSection>(ref __TypeHandle.__Game_Prefabs_NetSubSection_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetSectionPiece> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<NetSectionPiece>(ref __TypeHandle.__Game_Prefabs_NetSectionPiece_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetPieceLane> bufferTypeHandle3 = InternalCompilerInterface.GetBufferTypeHandle<NetPieceLane>(ref __TypeHandle.__Game_Prefabs_NetPieceLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetPieceArea> bufferTypeHandle4 = InternalCompilerInterface.GetBufferTypeHandle<NetPieceArea>(ref __TypeHandle.__Game_Prefabs_NetPieceArea_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetPieceObject> bufferTypeHandle5 = InternalCompilerInterface.GetBufferTypeHandle<NetPieceObject>(ref __TypeHandle.__Game_Prefabs_NetPieceObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetGeometrySection> bufferTypeHandle6 = InternalCompilerInterface.GetBufferTypeHandle<NetGeometrySection>(ref __TypeHandle.__Game_Prefabs_NetGeometrySection_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetGeometryEdgeState> bufferTypeHandle7 = InternalCompilerInterface.GetBufferTypeHandle<NetGeometryEdgeState>(ref __TypeHandle.__Game_Prefabs_NetGeometryEdgeState_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<NetGeometryNodeState> bufferTypeHandle8 = InternalCompilerInterface.GetBufferTypeHandle<NetGeometryNodeState>(ref __TypeHandle.__Game_Prefabs_NetGeometryNodeState_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubObject> bufferTypeHandle9 = InternalCompilerInterface.GetBufferTypeHandle<SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<SubMesh> bufferTypeHandle10 = InternalCompilerInterface.GetBufferTypeHandle<SubMesh>(ref __TypeHandle.__Game_Prefabs_SubMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<FixedNetElement> bufferTypeHandle11 = InternalCompilerInterface.GetBufferTypeHandle<FixedNetElement>(ref __TypeHandle.__Game_Prefabs_FixedNetElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<AuxiliaryNetLane> bufferTypeHandle12 = InternalCompilerInterface.GetBufferTypeHandle<AuxiliaryNetLane>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNetLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<AuxiliaryNet> bufferTypeHandle13 = InternalCompilerInterface.GetBufferTypeHandle<AuxiliaryNet>(ref __TypeHandle.__Game_Prefabs_AuxiliaryNet_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			PathwayData pathwayData = default(PathwayData);
			PlaceableNetData placeableNetData2 = default(PlaceableNetData);
			float2 val11 = default(float2);
			FixedNetElement fixedNetElement = default(FixedNetElement);
			for (int i = 0; i < chunks.Length; i++)
			{
				ArchetypeChunk val = chunks[i];
				if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref componentTypeHandle))
				{
					flag = ((ArchetypeChunk)(ref val)).Has<SpawnableObjectData>(ref componentTypeHandle31);
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
				bool flag2 = ((ArchetypeChunk)(ref val)).Has<MarkerNetData>(ref componentTypeHandle7);
				bool flag3 = ((ArchetypeChunk)(ref val)).Has<BridgeData>(ref componentTypeHandle30);
				bool flag4 = ((ArchetypeChunk)(ref val)).Has<UIObjectData>(ref componentTypeHandle33);
				NativeArray<NetData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetData>(ref componentTypeHandle3);
				NativeArray<NetGeometryData> nativeArray4 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetGeometryData>(ref componentTypeHandle5);
				NativeArray<PlaceableNetData> nativeArray5 = ((ArchetypeChunk)(ref val)).GetNativeArray<PlaceableNetData>(ref componentTypeHandle6);
				NativeArray<PathwayData> nativeArray6 = ((ArchetypeChunk)(ref val)).GetNativeArray<PathwayData>(ref componentTypeHandle21);
				if (nativeArray4.Length != 0)
				{
					BufferAccessor<NetGeometrySection> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetGeometrySection>(ref bufferTypeHandle6);
					for (int j = 0; j < nativeArray4.Length; j++)
					{
						_ = nativeArray[j];
						NetGeometryPrefab prefab = m_PrefabSystem.GetPrefab<NetGeometryPrefab>(nativeArray2[j]);
						NetGeometryData netGeometryData = nativeArray4[j];
						DynamicBuffer<NetGeometrySection> target = bufferAccessor[j];
						netGeometryData.m_EdgeLengthRange.max = 200f;
						netGeometryData.m_ElevatedLength = 80f;
						netGeometryData.m_MaxSlopeSteepness = math.select(prefab.m_MaxSlopeSteepness, 0f, prefab.m_MaxSlopeSteepness < 0.001f);
						netGeometryData.m_ElevationLimit = 4f;
						if ((Object)(object)prefab.m_AggregateType != (Object)null)
						{
							netGeometryData.m_AggregateType = m_PrefabSystem.GetEntity(prefab.m_AggregateType);
						}
						if ((Object)(object)prefab.m_StyleType != (Object)null)
						{
							netGeometryData.m_StyleType = m_PrefabSystem.GetEntity(prefab.m_StyleType);
						}
						if (flag2)
						{
							netGeometryData.m_Flags |= GeometryFlags.Marker;
						}
						AddSections(prefab, prefab.m_Sections, target, (NetSectionFlags)0);
						UndergroundNetSections component = prefab.GetComponent<UndergroundNetSections>();
						if ((Object)(object)component != (Object)null)
						{
							AddSections(prefab, component.m_Sections, target, NetSectionFlags.Underground);
						}
						OverheadNetSections component2 = prefab.GetComponent<OverheadNetSections>();
						if ((Object)(object)component2 != (Object)null)
						{
							AddSections(prefab, component2.m_Sections, target, NetSectionFlags.Overhead);
						}
						switch (prefab.m_InvertMode)
						{
						case CompositionInvertMode.InvertLefthandTraffic:
							netGeometryData.m_Flags |= GeometryFlags.InvertCompositionHandedness;
							break;
						case CompositionInvertMode.FlipLefthandTraffic:
							netGeometryData.m_Flags |= GeometryFlags.FlipCompositionHandedness;
							break;
						case CompositionInvertMode.InvertRighthandTraffic:
							netGeometryData.m_Flags |= GeometryFlags.IsLefthanded | GeometryFlags.InvertCompositionHandedness;
							break;
						case CompositionInvertMode.FlipRighthandTraffic:
							netGeometryData.m_Flags |= GeometryFlags.IsLefthanded | GeometryFlags.FlipCompositionHandedness;
							break;
						}
						nativeArray4[j] = netGeometryData;
					}
					BufferAccessor<NetGeometryEdgeState> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetGeometryEdgeState>(ref bufferTypeHandle7);
					BufferAccessor<NetGeometryNodeState> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetGeometryNodeState>(ref bufferTypeHandle8);
					for (int k = 0; k < nativeArray4.Length; k++)
					{
						NetGeometryPrefab prefab2 = m_PrefabSystem.GetPrefab<NetGeometryPrefab>(nativeArray2[k]);
						DynamicBuffer<NetGeometryEdgeState> val2 = bufferAccessor2[k];
						DynamicBuffer<NetGeometryNodeState> val3 = bufferAccessor3[k];
						if (prefab2.m_EdgeStates != null)
						{
							for (int l = 0; l < prefab2.m_EdgeStates.Length; l++)
							{
								NetEdgeStateInfo obj = prefab2.m_EdgeStates[l];
								NetGeometryEdgeState netGeometryEdgeState = default(NetGeometryEdgeState);
								NetCompositionHelpers.GetRequirementFlags(obj.m_RequireAll, out netGeometryEdgeState.m_CompositionAll, out var sectionFlags);
								NetCompositionHelpers.GetRequirementFlags(obj.m_RequireAny, out netGeometryEdgeState.m_CompositionAny, out var sectionFlags2);
								NetCompositionHelpers.GetRequirementFlags(obj.m_RequireNone, out netGeometryEdgeState.m_CompositionNone, out var sectionFlags3);
								NetCompositionHelpers.GetRequirementFlags(obj.m_SetState, out netGeometryEdgeState.m_State, out var sectionFlags4);
								NetSectionFlags netSectionFlags = sectionFlags | sectionFlags2 | sectionFlags3 | sectionFlags4;
								if (netSectionFlags != 0)
								{
									COSystemBase.baseLog.ErrorFormat((Object)(object)prefab2, "Net edge state ({0}) cannot require/set section flags: {1}", (object)((Object)prefab2).name, (object)netSectionFlags);
								}
								val2.Add(netGeometryEdgeState);
							}
						}
						if (prefab2.m_NodeStates == null)
						{
							continue;
						}
						for (int m = 0; m < prefab2.m_NodeStates.Length; m++)
						{
							NetNodeStateInfo netNodeStateInfo = prefab2.m_NodeStates[m];
							NetGeometryNodeState netGeometryNodeState = default(NetGeometryNodeState);
							NetCompositionHelpers.GetRequirementFlags(netNodeStateInfo.m_RequireAll, out netGeometryNodeState.m_CompositionAll, out var sectionFlags5);
							NetCompositionHelpers.GetRequirementFlags(netNodeStateInfo.m_RequireAny, out netGeometryNodeState.m_CompositionAny, out var sectionFlags6);
							NetCompositionHelpers.GetRequirementFlags(netNodeStateInfo.m_RequireNone, out netGeometryNodeState.m_CompositionNone, out var sectionFlags7);
							NetCompositionHelpers.GetRequirementFlags(netNodeStateInfo.m_SetState, out netGeometryNodeState.m_State, out var sectionFlags8);
							NetSectionFlags netSectionFlags2 = sectionFlags5 | sectionFlags6 | sectionFlags7 | sectionFlags8;
							if (netSectionFlags2 != 0)
							{
								COSystemBase.baseLog.ErrorFormat((Object)(object)prefab2, "Net node state ({0}) cannot require/set section flags: {1}", (object)((Object)prefab2).name, (object)netSectionFlags2);
							}
							netGeometryNodeState.m_MatchType = netNodeStateInfo.m_MatchType;
							val3.Add(netGeometryNodeState);
						}
					}
				}
				for (int n = 0; n < nativeArray5.Length; n++)
				{
					NetPrefab prefab3 = m_PrefabSystem.GetPrefab<NetPrefab>(nativeArray2[n]);
					PlaceableNetData placeableNetData = nativeArray5[n];
					placeableNetData.m_SnapDistance = 8f;
					placeableNetData.m_MinWaterElevation = 5f;
					PlaceableNet component3 = prefab3.GetComponent<PlaceableNet>();
					if ((Object)(object)component3 != (Object)null)
					{
						placeableNetData.m_ElevationRange = component3.m_ElevationRange;
						placeableNetData.m_XPReward = component3.m_XPReward;
						if ((Object)(object)component3.m_UndergroundPrefab != (Object)null)
						{
							placeableNetData.m_UndergroundPrefab = m_PrefabSystem.GetEntity(component3.m_UndergroundPrefab);
						}
						if (component3.m_AllowParallelMode)
						{
							placeableNetData.m_PlacementFlags |= PlacementFlags.AllowParallel;
						}
					}
					NetUpgrade component4 = prefab3.GetComponent<NetUpgrade>();
					if ((Object)(object)component4 != (Object)null)
					{
						NetCompositionHelpers.GetRequirementFlags(component4.m_SetState, out placeableNetData.m_SetUpgradeFlags, out var sectionFlags9);
						NetCompositionHelpers.GetRequirementFlags(component4.m_UnsetState, out placeableNetData.m_UnsetUpgradeFlags, out var sectionFlags10);
						placeableNetData.m_PlacementFlags |= PlacementFlags.IsUpgrade;
						if (!component4.m_Standalone)
						{
							placeableNetData.m_PlacementFlags |= PlacementFlags.UpgradeOnly;
						}
						if (component4.m_Underground)
						{
							placeableNetData.m_PlacementFlags |= PlacementFlags.UndergroundUpgrade;
						}
						if (((placeableNetData.m_SetUpgradeFlags | placeableNetData.m_UnsetUpgradeFlags) & CompositionFlags.nodeMask) != default(CompositionFlags))
						{
							placeableNetData.m_PlacementFlags |= PlacementFlags.NodeUpgrade;
						}
						NetSectionFlags netSectionFlags3 = sectionFlags9 | sectionFlags10;
						if (netSectionFlags3 != 0)
						{
							COSystemBase.baseLog.ErrorFormat((Object)(object)prefab3, "PlaceableNet ({0}) cannot upgrade section flags: {1}", (object)((Object)prefab3).name, (object)netSectionFlags3);
						}
					}
					nativeArray5[n] = placeableNetData;
				}
				BufferAccessor<SubObject> bufferAccessor4 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubObject>(ref bufferTypeHandle9);
				EntityManager entityManager;
				for (int num = 0; num < bufferAccessor4.Length; num++)
				{
					NetSubObjects component5 = m_PrefabSystem.GetPrefab<NetPrefab>(nativeArray2[num]).GetComponent<NetSubObjects>();
					bool flag5 = false;
					NetGeometryData netGeometryData2 = default(NetGeometryData);
					if (nativeArray4.Length != 0)
					{
						netGeometryData2 = nativeArray4[num];
					}
					DynamicBuffer<SubObject> val4 = bufferAccessor4[num];
					for (int num2 = 0; num2 < component5.m_SubObjects.Length; num2++)
					{
						NetSubObjectInfo netSubObjectInfo = component5.m_SubObjects[num2];
						ObjectPrefab prefab4 = netSubObjectInfo.m_Object;
						SubObject subObject = new SubObject
						{
							m_Prefab = m_PrefabSystem.GetEntity(prefab4),
							m_Position = netSubObjectInfo.m_Position,
							m_Rotation = netSubObjectInfo.m_Rotation,
							m_Probability = 100
						};
						switch (netSubObjectInfo.m_Placement)
						{
						case NetObjectPlacement.EdgeEndsOrNode:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.AllowCombine;
							break;
						case NetObjectPlacement.EdgeMiddle:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.MiddlePlacement;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<PillarData>(subObject.m_Prefab))
							{
								netGeometryData2.m_Flags |= GeometryFlags.MiddlePillars;
							}
							if (netSubObjectInfo.m_Spacing != 0f)
							{
								subObject.m_Flags |= SubObjectFlags.EvenSpacing;
								subObject.m_Position.z = netSubObjectInfo.m_Spacing;
							}
							break;
						case NetObjectPlacement.EdgeEnds:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement;
							break;
						case NetObjectPlacement.CourseStart:
							subObject.m_Flags |= SubObjectFlags.CoursePlacement | SubObjectFlags.StartPlacement;
							if (!flag5)
							{
								subObject.m_Flags |= SubObjectFlags.MakeOwner;
								netGeometryData2.m_Flags |= GeometryFlags.SubOwner;
								flag5 = true;
							}
							break;
						case NetObjectPlacement.CourseEnd:
							subObject.m_Flags |= SubObjectFlags.CoursePlacement | SubObjectFlags.EndPlacement;
							if (!flag5)
							{
								subObject.m_Flags |= SubObjectFlags.MakeOwner;
								netGeometryData2.m_Flags |= GeometryFlags.SubOwner;
								flag5 = true;
							}
							break;
						case NetObjectPlacement.NodeBeforeFixedSegment:
							subObject.m_Flags |= SubObjectFlags.StartPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.NodeBetweenFixedSegment:
							subObject.m_Flags |= SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.NodeAfterFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EndPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeMiddleFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.MiddlePlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<PillarData>(subObject.m_Prefab))
							{
								netGeometryData2.m_Flags |= GeometryFlags.MiddlePillars;
							}
							if (netSubObjectInfo.m_Spacing != 0f)
							{
								subObject.m_Flags |= SubObjectFlags.EvenSpacing;
								subObject.m_Position.z = netSubObjectInfo.m_Spacing;
							}
							break;
						case NetObjectPlacement.EdgeEndsFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeStartFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.StartPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeEndFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.EndPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeEndsOrNodeFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.AllowCombine | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeStartOrNodeFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.AllowCombine | SubObjectFlags.StartPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.EdgeEndOrNodeFixedSegment:
							subObject.m_Flags |= SubObjectFlags.EdgePlacement | SubObjectFlags.AllowCombine | SubObjectFlags.EndPlacement | SubObjectFlags.FixedPlacement;
							subObject.m_ParentIndex = netSubObjectInfo.m_FixedIndex;
							break;
						case NetObjectPlacement.WaterwayCrossingNode:
							subObject.m_Flags |= SubObjectFlags.WaterwayCrossing;
							netGeometryData2.m_IntersectLayers |= Layer.Waterway;
							break;
						case NetObjectPlacement.NotWaterwayCrossingNode:
							subObject.m_Flags |= SubObjectFlags.NotWaterwayCrossing;
							netGeometryData2.m_IntersectLayers |= Layer.Waterway;
							break;
						case NetObjectPlacement.NotWaterwayCrossingEdgeMiddle:
							subObject.m_Flags |= SubObjectFlags.NotWaterwayCrossing | SubObjectFlags.EdgePlacement | SubObjectFlags.MiddlePlacement;
							netGeometryData2.m_IntersectLayers |= Layer.Waterway;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<PillarData>(subObject.m_Prefab))
							{
								netGeometryData2.m_Flags |= GeometryFlags.MiddlePillars;
							}
							if (netSubObjectInfo.m_Spacing != 0f)
							{
								subObject.m_Flags |= SubObjectFlags.EvenSpacing;
								subObject.m_Position.z = netSubObjectInfo.m_Spacing;
							}
							break;
						case NetObjectPlacement.NotWaterwayCrossingEdgeEndsOrNode:
							subObject.m_Flags |= SubObjectFlags.NotWaterwayCrossing | SubObjectFlags.EdgePlacement | SubObjectFlags.AllowCombine;
							netGeometryData2.m_IntersectLayers |= Layer.Waterway;
							break;
						}
						if (netSubObjectInfo.m_AnchorTop)
						{
							subObject.m_Flags |= SubObjectFlags.AnchorTop;
						}
						if (netSubObjectInfo.m_AnchorCenter)
						{
							subObject.m_Flags |= SubObjectFlags.AnchorCenter;
						}
						if (netSubObjectInfo.m_RequireElevated)
						{
							subObject.m_Flags |= SubObjectFlags.RequireElevated;
						}
						if (netSubObjectInfo.m_RequireOutsideConnection)
						{
							subObject.m_Flags |= SubObjectFlags.RequireOutsideConnection;
						}
						if (netSubObjectInfo.m_RequireDeadEnd)
						{
							subObject.m_Flags |= SubObjectFlags.RequireDeadEnd;
						}
						if (netSubObjectInfo.m_RequireOrphan)
						{
							subObject.m_Flags |= SubObjectFlags.RequireOrphan;
						}
						if (CollectionUtils.TryGet<PathwayData>(nativeArray6, num, ref pathwayData))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<LeisureProviderData>(subObject.m_Prefab))
							{
								pathwayData.m_LeisureProvider = true;
								nativeArray6[num] = pathwayData;
							}
						}
						val4.Add(subObject);
					}
					if (nativeArray4.Length != 0)
					{
						nativeArray4[num] = netGeometryData2;
					}
				}
				BufferAccessor<AuxiliaryNet> bufferAccessor5 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<AuxiliaryNet>(ref bufferTypeHandle13);
				for (int num3 = 0; num3 < bufferAccessor5.Length; num3++)
				{
					AuxiliaryNets component6 = m_PrefabSystem.GetPrefab<NetPrefab>(nativeArray2[num3]).GetComponent<AuxiliaryNets>();
					DynamicBuffer<AuxiliaryNet> val5 = bufferAccessor5[num3];
					val5.ResizeUninitialized(component6.m_AuxiliaryNets.Length);
					if (CollectionUtils.TryGet<PlaceableNetData>(nativeArray5, num3, ref placeableNetData2))
					{
						if (component6.m_LinkEndOffsets)
						{
							placeableNetData2.m_PlacementFlags |= PlacementFlags.LinkAuxOffsets;
						}
						nativeArray5[num3] = placeableNetData2;
					}
					for (int num4 = 0; num4 < component6.m_AuxiliaryNets.Length; num4++)
					{
						AuxiliaryNetInfo auxiliaryNetInfo = component6.m_AuxiliaryNets[num4];
						val5[num4] = new AuxiliaryNet
						{
							m_Prefab = m_PrefabSystem.GetEntity(auxiliaryNetInfo.m_Prefab),
							m_Position = auxiliaryNetInfo.m_Position,
							m_InvertMode = auxiliaryNetInfo.m_InvertWhen
						};
					}
				}
				BufferAccessor<NetSectionPiece> bufferAccessor6 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetSectionPiece>(ref bufferTypeHandle2);
				if (bufferAccessor6.Length != 0)
				{
					BufferAccessor<NetSubSection> bufferAccessor7 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetSubSection>(ref bufferTypeHandle);
					for (int num5 = 0; num5 < bufferAccessor6.Length; num5++)
					{
						NetSectionPrefab prefab5 = m_PrefabSystem.GetPrefab<NetSectionPrefab>(nativeArray2[num5]);
						DynamicBuffer<NetSubSection> val6 = bufferAccessor7[num5];
						DynamicBuffer<NetSectionPiece> val7 = bufferAccessor6[num5];
						if (prefab5.m_SubSections != null)
						{
							for (int num6 = 0; num6 < prefab5.m_SubSections.Length; num6++)
							{
								NetSubSectionInfo netSubSectionInfo = prefab5.m_SubSections[num6];
								NetSubSection netSubSection = new NetSubSection
								{
									m_SubSection = m_PrefabSystem.GetEntity(netSubSectionInfo.m_Section)
								};
								NetCompositionHelpers.GetRequirementFlags(netSubSectionInfo.m_RequireAll, out netSubSection.m_CompositionAll, out netSubSection.m_SectionAll);
								NetCompositionHelpers.GetRequirementFlags(netSubSectionInfo.m_RequireAny, out netSubSection.m_CompositionAny, out netSubSection.m_SectionAny);
								NetCompositionHelpers.GetRequirementFlags(netSubSectionInfo.m_RequireNone, out netSubSection.m_CompositionNone, out netSubSection.m_SectionNone);
								val6.Add(netSubSection);
							}
						}
						if (prefab5.m_Pieces == null)
						{
							continue;
						}
						for (int num7 = 0; num7 < prefab5.m_Pieces.Length; num7++)
						{
							NetPieceInfo netPieceInfo = prefab5.m_Pieces[num7];
							NetSectionPiece netSectionPiece = new NetSectionPiece
							{
								m_Piece = m_PrefabSystem.GetEntity(netPieceInfo.m_Piece)
							};
							NetCompositionHelpers.GetRequirementFlags(netPieceInfo.m_RequireAll, out netSectionPiece.m_CompositionAll, out netSectionPiece.m_SectionAll);
							NetCompositionHelpers.GetRequirementFlags(netPieceInfo.m_RequireAny, out netSectionPiece.m_CompositionAny, out netSectionPiece.m_SectionAny);
							NetCompositionHelpers.GetRequirementFlags(netPieceInfo.m_RequireNone, out netSectionPiece.m_CompositionNone, out netSectionPiece.m_SectionNone);
							switch (netPieceInfo.m_Piece.m_Layer)
							{
							case NetPieceLayer.Surface:
								netSectionPiece.m_Flags |= NetPieceFlags.Surface;
								break;
							case NetPieceLayer.Bottom:
								netSectionPiece.m_Flags |= NetPieceFlags.Bottom;
								break;
							case NetPieceLayer.Top:
								netSectionPiece.m_Flags |= NetPieceFlags.Top;
								break;
							case NetPieceLayer.Side:
								netSectionPiece.m_Flags |= NetPieceFlags.Side;
								break;
							}
							if (netPieceInfo.m_Piece.meshCount != 0)
							{
								netSectionPiece.m_Flags |= NetPieceFlags.HasMesh;
							}
							NetDividerPiece component7 = netPieceInfo.m_Piece.GetComponent<NetDividerPiece>();
							if ((Object)(object)component7 != (Object)null)
							{
								if (component7.m_PreserveShape)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.PreserveShape | NetPieceFlags.DisableTiling;
								}
								if (component7.m_BlockTraffic)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.BlockTraffic;
								}
								if (component7.m_BlockCrosswalk)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.BlockCrosswalk;
								}
							}
							NetPieceTiling component8 = netPieceInfo.m_Piece.GetComponent<NetPieceTiling>();
							if ((Object)(object)component8 != (Object)null && component8.m_DisableTextureTiling)
							{
								netSectionPiece.m_Flags |= NetPieceFlags.DisableTiling;
							}
							MovePieceVertices component9 = netPieceInfo.m_Piece.GetComponent<MovePieceVertices>();
							if ((Object)(object)component9 != (Object)null)
							{
								if (component9.m_LowerBottomToTerrain)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.LowerBottomToTerrain;
								}
								if (component9.m_RaiseTopToTerrain)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.RaiseTopToTerrain;
								}
								if (component9.m_SmoothTopNormal)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.SmoothTopNormal;
								}
							}
							AsymmetricPieceMesh component10 = netPieceInfo.m_Piece.GetComponent<AsymmetricPieceMesh>();
							if ((Object)(object)component10 != (Object)null)
							{
								if (component10.m_Sideways)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.AsymmetricMeshX;
								}
								if (component10.m_Lengthwise)
								{
									netSectionPiece.m_Flags |= NetPieceFlags.AsymmetricMeshZ;
								}
							}
							netSectionPiece.m_Offset = netPieceInfo.m_Offset;
							val7.Add(netSectionPiece);
						}
					}
				}
				NativeArray<NetPieceData> nativeArray7 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetPieceData>(ref componentTypeHandle4);
				if (nativeArray7.Length != 0)
				{
					BufferAccessor<NetPieceLane> bufferAccessor8 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetPieceLane>(ref bufferTypeHandle3);
					BufferAccessor<NetPieceArea> bufferAccessor9 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetPieceArea>(ref bufferTypeHandle4);
					BufferAccessor<NetPieceObject> bufferAccessor10 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<NetPieceObject>(ref bufferTypeHandle5);
					NativeArray<NetCrosswalkData> nativeArray8 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetCrosswalkData>(ref componentTypeHandle17);
					NativeArray<NetTerrainData> nativeArray9 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetTerrainData>(ref componentTypeHandle32);
					for (int num8 = 0; num8 < nativeArray7.Length; num8++)
					{
						NetPiecePrefab prefab6 = m_PrefabSystem.GetPrefab<NetPiecePrefab>(nativeArray2[num8]);
						NetPieceData netPieceData = nativeArray7[num8];
						netPieceData.m_HeightRange = prefab6.m_HeightRange;
						netPieceData.m_SurfaceHeights = prefab6.m_SurfaceHeights;
						netPieceData.m_Width = prefab6.m_Width;
						netPieceData.m_Length = prefab6.m_Length;
						netPieceData.m_WidthOffset = prefab6.m_WidthOffset;
						netPieceData.m_NodeOffset = prefab6.m_NodeOffset;
						netPieceData.m_SideConnectionOffset = prefab6.m_SideConnectionOffset;
						if (bufferAccessor8.Length != 0)
						{
							NetPieceLanes component11 = prefab6.GetComponent<NetPieceLanes>();
							if (component11.m_Lanes != null)
							{
								DynamicBuffer<NetPieceLane> val8 = bufferAccessor8[num8];
								for (int num9 = 0; num9 < component11.m_Lanes.Length; num9++)
								{
									NetLaneInfo netLaneInfo = component11.m_Lanes[num9];
									NetPieceLane netPieceLane = new NetPieceLane
									{
										m_Lane = m_PrefabSystem.GetEntity(netLaneInfo.m_Lane),
										m_Position = netLaneInfo.m_Position
									};
									if (netLaneInfo.m_FindAnchor)
									{
										netPieceLane.m_ExtraFlags |= LaneFlags.FindAnchor;
									}
									val8.Add(netPieceLane);
								}
								if (val8.Length > 1)
								{
									NativeSortExtension.Sort<NetPieceLane>(val8.AsNativeArray());
								}
							}
						}
						if (bufferAccessor9.Length != 0)
						{
							DynamicBuffer<NetPieceArea> val9 = bufferAccessor9[num8];
							BuildableNetPiece component12 = prefab6.GetComponent<BuildableNetPiece>();
							if ((Object)(object)component12 != (Object)null)
							{
								val9.Add(new NetPieceArea
								{
									m_Flags = (component12.m_AllowOnBridge ? NetAreaFlags.Buildable : (NetAreaFlags.Buildable | NetAreaFlags.NoBridge)),
									m_Position = component12.m_Position,
									m_Width = component12.m_Width,
									m_SnapPosition = component12.m_SnapPosition,
									m_SnapWidth = component12.m_SnapWidth
								});
							}
							if (val9.Length > 1)
							{
								NativeSortExtension.Sort<NetPieceArea>(val9.AsNativeArray());
							}
						}
						if (bufferAccessor10.Length != 0)
						{
							DynamicBuffer<NetPieceObject> val10 = bufferAccessor10[num8];
							NetPieceObjects component13 = prefab6.GetComponent<NetPieceObjects>();
							if ((Object)(object)component13 != (Object)null)
							{
								val10.ResizeUninitialized(component13.m_PieceObjects.Length);
								for (int num10 = 0; num10 < component13.m_PieceObjects.Length; num10++)
								{
									NetPieceObjectInfo netPieceObjectInfo = component13.m_PieceObjects[num10];
									NetPieceObject netPieceObject = new NetPieceObject
									{
										m_Prefab = m_PrefabSystem.GetEntity(netPieceObjectInfo.m_Object),
										m_Position = netPieceObjectInfo.m_Position,
										m_Offset = netPieceObjectInfo.m_Offset,
										m_Spacing = netPieceObjectInfo.m_Spacing,
										m_UseCurveRotation = netPieceObjectInfo.m_UseCurveRotation,
										m_MinLength = netPieceObjectInfo.m_MinLength,
										m_Probability = math.select(netPieceObjectInfo.m_Probability, 100, netPieceObjectInfo.m_Probability == 0),
										m_CurveOffsetRange = netPieceObjectInfo.m_CurveOffsetRange,
										m_Rotation = netPieceObjectInfo.m_Rotation
									};
									NetCompositionHelpers.GetRequirementFlags(netPieceObjectInfo.m_RequireAll, out netPieceObject.m_CompositionAll, out netPieceObject.m_SectionAll);
									NetCompositionHelpers.GetRequirementFlags(netPieceObjectInfo.m_RequireAny, out netPieceObject.m_CompositionAny, out netPieceObject.m_SectionAny);
									NetCompositionHelpers.GetRequirementFlags(netPieceObjectInfo.m_RequireNone, out netPieceObject.m_CompositionNone, out netPieceObject.m_SectionNone);
									if (netPieceObjectInfo.m_FlipWhenInverted)
									{
										netPieceObject.m_Flags |= SubObjectFlags.FlipInverted;
									}
									if (netPieceObjectInfo.m_EvenSpacing)
									{
										netPieceObject.m_Flags |= SubObjectFlags.EvenSpacing;
									}
									if (netPieceObjectInfo.m_SpacingOverride)
									{
										netPieceObject.m_Flags |= SubObjectFlags.SpacingOverride;
									}
									val10[num10] = netPieceObject;
								}
							}
						}
						if (nativeArray8.Length != 0)
						{
							NetPieceCrosswalk component14 = prefab6.GetComponent<NetPieceCrosswalk>();
							nativeArray8[num8] = new NetCrosswalkData
							{
								m_Lane = m_PrefabSystem.GetEntity(component14.m_Lane),
								m_Start = component14.m_Start,
								m_End = component14.m_End
							};
						}
						if (nativeArray9.Length != 0)
						{
							NetTerrainPiece component15 = prefab6.GetComponent<NetTerrainPiece>();
							nativeArray9[num8] = new NetTerrainData
							{
								m_WidthOffset = component15.m_WidthOffset,
								m_ClipHeightOffset = component15.m_ClipHeightOffset,
								m_MinHeightOffset = component15.m_MinHeightOffset,
								m_MaxHeightOffset = component15.m_MaxHeightOffset
							};
						}
						nativeArray7[num8] = netPieceData;
					}
				}
				NativeArray<NetLaneData> nativeArray10 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetLaneData>(ref componentTypeHandle9);
				if (nativeArray10.Length != 0)
				{
					NativeArray<ParkingLaneData> nativeArray11 = ((ArchetypeChunk)(ref val)).GetNativeArray<ParkingLaneData>(ref componentTypeHandle14);
					NativeArray<CarLaneData> nativeArray12 = ((ArchetypeChunk)(ref val)).GetNativeArray<CarLaneData>(ref componentTypeHandle11);
					NativeArray<TrackLaneData> nativeArray13 = ((ArchetypeChunk)(ref val)).GetNativeArray<TrackLaneData>(ref componentTypeHandle12);
					NativeArray<UtilityLaneData> nativeArray14 = ((ArchetypeChunk)(ref val)).GetNativeArray<UtilityLaneData>(ref componentTypeHandle13);
					NativeArray<SecondaryLaneData> nativeArray15 = ((ArchetypeChunk)(ref val)).GetNativeArray<SecondaryLaneData>(ref componentTypeHandle16);
					BufferAccessor<AuxiliaryNetLane> bufferAccessor11 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<AuxiliaryNetLane>(ref bufferTypeHandle12);
					bool flag6 = ((ArchetypeChunk)(ref val)).Has<PedestrianLaneData>(ref componentTypeHandle15);
					for (int num11 = 0; num11 < nativeArray10.Length; num11++)
					{
						NetLanePrefab prefab7 = m_PrefabSystem.GetPrefab<NetLanePrefab>(nativeArray2[num11]);
						NetLaneData netLaneData = nativeArray10[num11];
						if ((Object)(object)prefab7.m_PathfindPrefab != (Object)null)
						{
							netLaneData.m_PathfindPrefab = m_PrefabSystem.GetEntity(prefab7.m_PathfindPrefab);
						}
						if (nativeArray12.Length != 0)
						{
							CarLane component16 = prefab7.GetComponent<CarLane>();
							netLaneData.m_Flags |= LaneFlags.Road;
							netLaneData.m_Width = component16.m_Width;
							if (component16.m_StartingLane)
							{
								netLaneData.m_Flags |= LaneFlags.DisconnectedStart;
							}
							if (component16.m_EndingLane)
							{
								netLaneData.m_Flags |= LaneFlags.DisconnectedEnd;
							}
							if (component16.m_Twoway)
							{
								netLaneData.m_Flags |= LaneFlags.Twoway;
							}
							if (component16.m_BusLane)
							{
								netLaneData.m_Flags |= LaneFlags.PublicOnly;
							}
							if (component16.m_RoadType == RoadTypes.Watercraft)
							{
								netLaneData.m_Flags |= LaneFlags.OnWater;
							}
							CarLaneData carLaneData = nativeArray12[num11];
							if ((Object)(object)component16.m_NotTrackLane != (Object)null)
							{
								carLaneData.m_NotTrackLanePrefab = m_PrefabSystem.GetEntity(component16.m_NotTrackLane);
							}
							if ((Object)(object)component16.m_NotBusLane != (Object)null)
							{
								carLaneData.m_NotBusLanePrefab = m_PrefabSystem.GetEntity(component16.m_NotBusLane);
							}
							carLaneData.m_RoadTypes = component16.m_RoadType;
							carLaneData.m_MaxSize = component16.m_MaxSize;
							nativeArray12[num11] = carLaneData;
						}
						if (nativeArray13.Length != 0)
						{
							TrackLane component17 = prefab7.GetComponent<TrackLane>();
							netLaneData.m_Flags |= LaneFlags.Track;
							netLaneData.m_Width = component17.m_Width;
							if (component17.m_Twoway)
							{
								netLaneData.m_Flags |= LaneFlags.Twoway;
							}
							TrackLaneData trackLaneData = nativeArray13[num11];
							if ((Object)(object)component17.m_FallbackLane != (Object)null)
							{
								trackLaneData.m_FallbackPrefab = m_PrefabSystem.GetEntity(component17.m_FallbackLane);
							}
							if ((Object)(object)component17.m_EndObject != (Object)null)
							{
								trackLaneData.m_EndObjectPrefab = m_PrefabSystem.GetEntity(component17.m_EndObject);
							}
							trackLaneData.m_TrackTypes = component17.m_TrackType;
							trackLaneData.m_MaxCurviness = math.radians(component17.m_MaxCurviness);
							nativeArray13[num11] = trackLaneData;
						}
						if (nativeArray14.Length != 0)
						{
							UtilityLane component18 = prefab7.GetComponent<UtilityLane>();
							netLaneData.m_Flags |= LaneFlags.Utility;
							netLaneData.m_Width = component18.m_Width;
							if (component18.m_Underground)
							{
								netLaneData.m_Flags |= LaneFlags.Underground;
							}
							UtilityLaneData utilityLaneData = nativeArray14[num11];
							if ((Object)(object)component18.m_LocalConnectionLane != (Object)null)
							{
								utilityLaneData.m_LocalConnectionPrefab = m_PrefabSystem.GetEntity(component18.m_LocalConnectionLane);
							}
							if ((Object)(object)component18.m_LocalConnectionLane2 != (Object)null)
							{
								utilityLaneData.m_LocalConnectionPrefab2 = m_PrefabSystem.GetEntity(component18.m_LocalConnectionLane2);
							}
							if ((Object)(object)component18.m_NodeObject != (Object)null)
							{
								utilityLaneData.m_NodeObjectPrefab = m_PrefabSystem.GetEntity(component18.m_NodeObject);
							}
							utilityLaneData.m_VisualCapacity = component18.m_VisualCapacity;
							utilityLaneData.m_Hanging = component18.m_Hanging;
							utilityLaneData.m_UtilityTypes = component18.m_UtilityType;
							nativeArray14[num11] = utilityLaneData;
						}
						if (nativeArray11.Length != 0)
						{
							ParkingLane component19 = prefab7.GetComponent<ParkingLane>();
							netLaneData.m_Flags |= LaneFlags.Parking;
							ParkingLaneData parkingLaneData = nativeArray11[num11];
							parkingLaneData.m_RoadTypes = component19.m_RoadType;
							parkingLaneData.m_SlotSize = math.select(component19.m_SlotSize, float2.op_Implicit(0f), component19.m_SlotSize < 0.001f);
							parkingLaneData.m_SlotAngle = math.radians(math.clamp(component19.m_SlotAngle, 0f, 90f));
							parkingLaneData.m_MaxCarLength = math.select(0f, parkingLaneData.m_SlotSize.y + 0.4f, parkingLaneData.m_SlotSize.y != 0f);
							((float2)(ref val11))._002Ector(math.cos(parkingLaneData.m_SlotAngle), math.sin(parkingLaneData.m_SlotAngle));
							if (val11.y < 0.001f)
							{
								parkingLaneData.m_SlotInterval = parkingLaneData.m_SlotSize.y;
							}
							else if (val11.x < 0.001f)
							{
								parkingLaneData.m_SlotInterval = parkingLaneData.m_SlotSize.x;
								netLaneData.m_Flags |= LaneFlags.Twoway;
							}
							else
							{
								float2 val12 = parkingLaneData.m_SlotSize / ((float2)(ref val11)).yx;
								val12 = math.select(val12, float2.op_Implicit(0f), val12 < 0.001f);
								if (val12.x < val12.y)
								{
									parkingLaneData.m_SlotInterval = val12.x;
								}
								else
								{
									parkingLaneData.m_SlotInterval = val12.y;
									parkingLaneData.m_MaxCarLength = math.max(0f, parkingLaneData.m_SlotSize.y - 1f);
								}
							}
							netLaneData.m_Width = math.dot(parkingLaneData.m_SlotSize, val11);
							netLaneData.m_Width = math.select(netLaneData.m_Width, parkingLaneData.m_SlotSize.y, parkingLaneData.m_SlotSize.y != 0f && parkingLaneData.m_SlotSize.y < netLaneData.m_Width);
							if (parkingLaneData.m_SlotSize.x == 0f)
							{
								netLaneData.m_Flags |= LaneFlags.Virtual;
							}
							if (component19.m_SpecialVehicles)
							{
								netLaneData.m_Flags |= LaneFlags.PublicOnly;
							}
							nativeArray11[num11] = parkingLaneData;
						}
						if (flag6)
						{
							PedestrianLane component20 = prefab7.GetComponent<PedestrianLane>();
							netLaneData.m_Flags |= LaneFlags.Pedestrian | LaneFlags.Twoway;
							netLaneData.m_Width = component20.m_Width;
							if (component20.m_OnWater)
							{
								netLaneData.m_Flags |= LaneFlags.OnWater;
							}
						}
						if (nativeArray15.Length != 0)
						{
							Entity val13 = nativeArray[num11];
							SecondaryLane component21 = prefab7.GetComponent<SecondaryLane>();
							netLaneData.m_Flags |= LaneFlags.Secondary;
							bool flag7 = component21.m_LeftLanes != null && component21.m_LeftLanes.Length != 0;
							bool flag8 = component21.m_RightLanes != null && component21.m_RightLanes.Length != 0;
							bool flag9 = component21.m_CrossingLanes != null && component21.m_CrossingLanes.Length != 0;
							SecondaryLaneData secondaryLaneData = nativeArray15[num11];
							if (component21.m_SkipSafePedestrianOverlap)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.SkipSafePedestrianOverlap;
							}
							if (component21.m_SkipSafeCarOverlap)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.SkipSafeCarOverlap;
							}
							if (component21.m_SkipUnsafeCarOverlap)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.SkipUnsafeCarOverlap;
							}
							if (component21.m_SkipTrackOverlap)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.SkipTrackOverlap;
							}
							if (component21.m_SkipMergeOverlap)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.SkipMergeOverlap;
							}
							if (component21.m_FitToParkingSpaces)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.FitToParkingSpaces;
							}
							if (component21.m_EvenSpacing)
							{
								secondaryLaneData.m_Flags |= SecondaryLaneDataFlags.EvenSpacing;
							}
							secondaryLaneData.m_PositionOffset = component21.m_PositionOffset;
							secondaryLaneData.m_LengthOffset = component21.m_LengthOffset;
							secondaryLaneData.m_CutMargin = component21.m_CutMargin;
							secondaryLaneData.m_CutOffset = component21.m_CutOffset;
							secondaryLaneData.m_CutOverlap = component21.m_CutOverlap;
							secondaryLaneData.m_Spacing = component21.m_Spacing;
							SecondaryNetLaneFlags secondaryNetLaneFlags = (SecondaryNetLaneFlags)0;
							if (component21.m_CanFlipSides)
							{
								secondaryNetLaneFlags |= SecondaryNetLaneFlags.CanFlipSides;
							}
							if (component21.m_DuplicateSides)
							{
								secondaryNetLaneFlags |= SecondaryNetLaneFlags.DuplicateSides;
							}
							if (component21.m_RequireParallel)
							{
								secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireParallel;
							}
							if (component21.m_RequireOpposite)
							{
								secondaryNetLaneFlags |= SecondaryNetLaneFlags.RequireOpposite;
							}
							if (flag7)
							{
								SecondaryNetLaneFlags secondaryNetLaneFlags2 = secondaryNetLaneFlags | SecondaryNetLaneFlags.Left;
								if (!flag8)
								{
									secondaryNetLaneFlags2 |= SecondaryNetLaneFlags.OneSided;
								}
								for (int num12 = 0; num12 < component21.m_LeftLanes.Length; num12++)
								{
									SecondaryLaneInfo secondaryLaneInfo = component21.m_LeftLanes[num12];
									SecondaryNetLaneFlags flags = secondaryNetLaneFlags2 | secondaryLaneInfo.GetFlags();
									Entity entity = m_PrefabSystem.GetEntity(secondaryLaneInfo.m_Lane);
									entityManager = ((ComponentSystemBase)this).EntityManager;
									((EntityManager)(ref entityManager)).GetBuffer<SecondaryNetLane>(entity, false).Add(new SecondaryNetLane
									{
										m_Lane = val13,
										m_Flags = flags
									});
								}
							}
							if (flag8)
							{
								SecondaryNetLaneFlags secondaryNetLaneFlags3 = secondaryNetLaneFlags | SecondaryNetLaneFlags.Right;
								if (!flag7)
								{
									secondaryNetLaneFlags3 |= SecondaryNetLaneFlags.OneSided;
								}
								for (int num13 = 0; num13 < component21.m_RightLanes.Length; num13++)
								{
									SecondaryLaneInfo secondaryLaneInfo2 = component21.m_RightLanes[num13];
									SecondaryNetLaneFlags secondaryNetLaneFlags4 = secondaryNetLaneFlags3 | secondaryLaneInfo2.GetFlags();
									Entity entity2 = m_PrefabSystem.GetEntity(secondaryLaneInfo2.m_Lane);
									entityManager = ((ComponentSystemBase)this).EntityManager;
									DynamicBuffer<SecondaryNetLane> buffer = ((EntityManager)(ref entityManager)).GetBuffer<SecondaryNetLane>(entity2, false);
									int num14 = 0;
									while (true)
									{
										if (num14 < buffer.Length)
										{
											SecondaryNetLane secondaryNetLane = buffer[num14];
											if (secondaryNetLane.m_Lane == val13 && ((secondaryNetLane.m_Flags ^ secondaryNetLaneFlags4) & ~(SecondaryNetLaneFlags.Left | SecondaryNetLaneFlags.Right)) == 0)
											{
												secondaryNetLane.m_Flags |= secondaryNetLaneFlags4;
												buffer[num14] = secondaryNetLane;
												break;
											}
											num14++;
											continue;
										}
										buffer.Add(new SecondaryNetLane
										{
											m_Lane = val13,
											m_Flags = secondaryNetLaneFlags4
										});
										break;
									}
								}
							}
							if (flag9)
							{
								SecondaryNetLaneFlags secondaryNetLaneFlags5 = SecondaryNetLaneFlags.Crossing;
								for (int num15 = 0; num15 < component21.m_CrossingLanes.Length; num15++)
								{
									SecondaryLaneInfo2 secondaryLaneInfo3 = component21.m_CrossingLanes[num15];
									SecondaryNetLaneFlags flags2 = secondaryNetLaneFlags5 | secondaryLaneInfo3.GetFlags();
									Entity entity3 = m_PrefabSystem.GetEntity(secondaryLaneInfo3.m_Lane);
									entityManager = ((ComponentSystemBase)this).EntityManager;
									((EntityManager)(ref entityManager)).GetBuffer<SecondaryNetLane>(entity3, false).Add(new SecondaryNetLane
									{
										m_Lane = val13,
										m_Flags = flags2
									});
								}
							}
							nativeArray15[num11] = secondaryLaneData;
						}
						if (bufferAccessor11.Length != 0)
						{
							DynamicBuffer<AuxiliaryNetLane> val14 = bufferAccessor11[num11];
							AuxiliaryLanes component22 = prefab7.GetComponent<AuxiliaryLanes>();
							if ((Object)(object)component22 != (Object)null)
							{
								val14.ResizeUninitialized(component22.m_AuxiliaryLanes.Length);
								for (int num16 = 0; num16 < component22.m_AuxiliaryLanes.Length; num16++)
								{
									AuxiliaryLaneInfo auxiliaryLaneInfo = component22.m_AuxiliaryLanes[num16];
									AuxiliaryNetLane auxiliaryNetLane = new AuxiliaryNetLane
									{
										m_Prefab = m_PrefabSystem.GetEntity(auxiliaryLaneInfo.m_Lane),
										m_Position = auxiliaryLaneInfo.m_Position,
										m_Spacing = auxiliaryLaneInfo.m_Spacing
									};
									if (auxiliaryLaneInfo.m_EvenSpacing)
									{
										auxiliaryNetLane.m_Flags |= LaneFlags.EvenSpacing;
									}
									if (auxiliaryLaneInfo.m_FindAnchor)
									{
										auxiliaryNetLane.m_Flags |= LaneFlags.FindAnchor;
									}
									NetCompositionHelpers.GetRequirementFlags(auxiliaryLaneInfo.m_RequireAll, out auxiliaryNetLane.m_CompositionAll, out var sectionFlags11);
									NetCompositionHelpers.GetRequirementFlags(auxiliaryLaneInfo.m_RequireAny, out auxiliaryNetLane.m_CompositionAny, out var sectionFlags12);
									NetCompositionHelpers.GetRequirementFlags(auxiliaryLaneInfo.m_RequireNone, out auxiliaryNetLane.m_CompositionNone, out var sectionFlags13);
									NetSectionFlags netSectionFlags4 = sectionFlags11 | sectionFlags12 | sectionFlags13;
									if (netSectionFlags4 != 0)
									{
										COSystemBase.baseLog.ErrorFormat((Object)(object)prefab7, "Auxiliary net lane ({0}: {1}) cannot require section flags: {2}", (object)((Object)prefab7).name, (object)((Object)auxiliaryLaneInfo.m_Lane).name, (object)netSectionFlags4);
									}
									val14[num16] = auxiliaryNetLane;
									netLaneData.m_Flags |= LaneFlags.HasAuxiliary;
								}
							}
						}
						nativeArray10[num11] = netLaneData;
					}
					NativeArray<NetLaneGeometryData> nativeArray16 = ((ArchetypeChunk)(ref val)).GetNativeArray<NetLaneGeometryData>(ref componentTypeHandle10);
					BufferAccessor<SubMesh> bufferAccessor12 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<SubMesh>(ref bufferTypeHandle10);
					for (int num17 = 0; num17 < nativeArray16.Length; num17++)
					{
						NetLaneGeometryPrefab prefab8 = m_PrefabSystem.GetPrefab<NetLaneGeometryPrefab>(nativeArray2[num17]);
						NetLaneData netLaneData2 = nativeArray10[num17];
						NetLaneGeometryData netLaneGeometryData = nativeArray16[num17];
						DynamicBuffer<SubMesh> val15 = bufferAccessor12[num17];
						netLaneGeometryData.m_MinLod = 255;
						netLaneGeometryData.m_GameLayers = (MeshLayer)0;
						netLaneGeometryData.m_EditorLayers = (MeshLayer)0;
						if (prefab8.m_Meshes != null)
						{
							for (int num18 = 0; num18 < prefab8.m_Meshes.Length; num18++)
							{
								NetLaneMeshInfo obj2 = prefab8.m_Meshes[num18];
								RenderPrefab mesh = obj2.m_Mesh;
								Entity entity4 = m_PrefabSystem.GetEntity(mesh);
								entityManager = ((ComponentSystemBase)this).EntityManager;
								MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(entity4);
								float3 val16 = MathUtils.Size(mesh.bounds);
								componentData.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(((float3)(ref val16)).xy), componentData.m_LodBias);
								componentData.m_ShadowLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetShadowRenderingSize(((float3)(ref val16)).xy), componentData.m_ShadowBias);
								netLaneGeometryData.m_Size = math.max(netLaneGeometryData.m_Size, val16);
								netLaneGeometryData.m_MinLod = math.min(netLaneGeometryData.m_MinLod, (int)componentData.m_MinLod);
								SubMeshFlags subMeshFlags = (SubMeshFlags)0u;
								if (obj2.m_RequireSafe)
								{
									subMeshFlags |= SubMeshFlags.RequireSafe;
								}
								if (obj2.m_RequireLevelCrossing)
								{
									subMeshFlags |= SubMeshFlags.RequireLevelCrossing;
								}
								if (obj2.m_RequireEditor)
								{
									subMeshFlags |= SubMeshFlags.RequireEditor;
								}
								if (obj2.m_RequireTrackCrossing)
								{
									subMeshFlags |= SubMeshFlags.RequireTrack;
								}
								if (obj2.m_RequireClear)
								{
									subMeshFlags |= SubMeshFlags.RequireClear;
								}
								if (obj2.m_RequireLeftHandTraffic)
								{
									subMeshFlags |= SubMeshFlags.RequireLeftHandTraffic;
								}
								if (obj2.m_RequireRightHandTraffic)
								{
									subMeshFlags |= SubMeshFlags.RequireRightHandTraffic;
								}
								val15.Add(new SubMesh(entity4, subMeshFlags, (ushort)num18));
								MeshLayer meshLayer = ((componentData.m_DefaultLayers == (MeshLayer)0) ? MeshLayer.Default : componentData.m_DefaultLayers);
								if (!obj2.m_RequireEditor)
								{
									netLaneGeometryData.m_GameLayers |= meshLayer;
								}
								netLaneGeometryData.m_EditorLayers |= meshLayer;
								entityManager = ((ComponentSystemBase)this).EntityManager;
								((EntityManager)(ref entityManager)).SetComponentData<MeshData>(entity4, componentData);
								if (mesh.Has<ColorProperties>())
								{
									netLaneData2.m_Flags |= LaneFlags.PseudoRandom;
								}
							}
						}
						nativeArray10[num17] = netLaneData2;
						nativeArray16[num17] = netLaneGeometryData;
					}
					NativeArray<SpawnableObjectData> nativeArray17 = ((ArchetypeChunk)(ref val)).GetNativeArray<SpawnableObjectData>(ref componentTypeHandle31);
					if (nativeArray17.Length != 0)
					{
						for (int num19 = 0; num19 < nativeArray17.Length; num19++)
						{
							Entity obj3 = nativeArray[num19];
							SpawnableObjectData spawnableObjectData = nativeArray17[num19];
							SpawnableLane component23 = m_PrefabSystem.GetPrefab<NetLanePrefab>(nativeArray2[num19]).GetComponent<SpawnableLane>();
							for (int num20 = 0; num20 < component23.m_Placeholders.Length; num20++)
							{
								NetLanePrefab prefab9 = component23.m_Placeholders[num20];
								Entity entity5 = m_PrefabSystem.GetEntity(prefab9);
								entityManager = ((ComponentSystemBase)this).EntityManager;
								((EntityManager)(ref entityManager)).GetBuffer<PlaceholderObjectElement>(entity5, false).Add(new PlaceholderObjectElement(obj3));
							}
							if ((Object)(object)component23.m_RandomizationGroup != (Object)null)
							{
								spawnableObjectData.m_RandomizationGroup = m_PrefabSystem.GetEntity(component23.m_RandomizationGroup);
							}
							spawnableObjectData.m_Probability = component23.m_Probability;
							nativeArray17[num19] = spawnableObjectData;
						}
					}
				}
				NativeArray<RoadData> nativeArray18 = ((ArchetypeChunk)(ref val)).GetNativeArray<RoadData>(ref componentTypeHandle18);
				if (nativeArray18.Length != 0)
				{
					for (int num21 = 0; num21 < nativeArray18.Length; num21++)
					{
						RoadPrefab prefab10 = m_PrefabSystem.GetPrefab<RoadPrefab>(nativeArray2[num21]);
						NetData netData = nativeArray3[num21];
						NetGeometryData netGeometryData3 = nativeArray4[num21];
						RoadData roadData = nativeArray18[num21];
						switch (prefab10.m_RoadType)
						{
						case RoadType.Normal:
							netData.m_RequiredLayers |= Layer.Road;
							break;
						case RoadType.PublicTransport:
							netData.m_RequiredLayers |= Layer.PublicTransportRoad;
							break;
						}
						netData.m_ConnectLayers |= Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.Fence | Layer.PublicTransportRoad;
						netData.m_ConnectLayers |= netGeometryData3.m_IntersectLayers & Layer.Waterway;
						netData.m_LocalConnectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netData.m_NodePriority += 2000f;
						netGeometryData3.m_MergeLayers |= Layer.Road | Layer.TramTrack | Layer.PublicTransportRoad;
						netGeometryData3.m_IntersectLayers |= Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.TramTrack | Layer.PublicTransportRoad;
						netGeometryData3.m_Flags |= GeometryFlags.SupportRoundabout | GeometryFlags.BlockZone | GeometryFlags.Directional | GeometryFlags.FlattenTerrain | GeometryFlags.ClipTerrain;
						roadData.m_SpeedLimit = prefab10.m_SpeedLimit / 3.6f;
						if ((Object)(object)prefab10.m_ZoneBlock != (Object)null)
						{
							netGeometryData3.m_Flags |= GeometryFlags.SnapCellSize;
							roadData.m_ZoneBlockPrefab = m_PrefabSystem.GetEntity(prefab10.m_ZoneBlock);
							roadData.m_Flags |= RoadFlags.EnableZoning;
						}
						if (prefab10.m_TrafficLights)
						{
							roadData.m_Flags |= RoadFlags.PreferTrafficLights;
						}
						if (prefab10.m_HighwayRules)
						{
							roadData.m_Flags |= RoadFlags.UseHighwayRules;
							netGeometryData3.m_MinNodeOffset = math.max(netGeometryData3.m_MinNodeOffset, 2f);
							netGeometryData3.m_Flags |= GeometryFlags.SmoothElevation;
						}
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData3 = nativeArray5[num21];
							placeableNetData3.m_PlacementFlags |= PlacementFlags.OnGround;
							nativeArray5[num21] = placeableNetData3;
						}
						nativeArray3[num21] = netData;
						nativeArray4[num21] = netGeometryData3;
						nativeArray18[num21] = roadData;
					}
				}
				NativeArray<TrackData> nativeArray19 = ((ArchetypeChunk)(ref val)).GetNativeArray<TrackData>(ref componentTypeHandle19);
				if (nativeArray19.Length != 0)
				{
					for (int num22 = 0; num22 < nativeArray19.Length; num22++)
					{
						TrackPrefab prefab11 = m_PrefabSystem.GetPrefab<TrackPrefab>(nativeArray2[num22]);
						NetData netData2 = nativeArray3[num22];
						NetGeometryData netGeometryData4 = nativeArray4[num22];
						TrackData trackData = nativeArray19[num22];
						Layer layer;
						Layer layer2;
						float num23;
						float num24;
						switch (prefab11.m_TrackType)
						{
						case TrackTypes.Train:
							layer = Layer.TrainTrack;
							layer2 = Layer.TrainTrack | Layer.Pathway;
							num23 = 200f;
							num24 = 10f;
							netGeometryData4.m_Flags |= GeometryFlags.SmoothElevation;
							break;
						case TrackTypes.Tram:
							layer = Layer.TramTrack;
							layer2 = Layer.TramTrack;
							num23 = 0f;
							num24 = 8f;
							netGeometryData4.m_Flags |= GeometryFlags.SupportRoundabout;
							break;
						case TrackTypes.Subway:
							layer = Layer.SubwayTrack;
							layer2 = Layer.SubwayTrack;
							num23 = 200f;
							num24 = 9f;
							netGeometryData4.m_Flags |= GeometryFlags.SmoothElevation;
							break;
						default:
							layer = Layer.None;
							layer2 = Layer.None;
							num23 = 0f;
							num24 = 0f;
							break;
						}
						netData2.m_RequiredLayers |= layer;
						netData2.m_ConnectLayers |= layer2;
						netData2.m_ConnectLayers |= netGeometryData4.m_IntersectLayers & Layer.Waterway;
						netData2.m_LocalConnectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netGeometryData4.m_MergeLayers |= layer;
						netGeometryData4.m_IntersectLayers |= layer2;
						netGeometryData4.m_EdgeLengthRange.max = math.max(netGeometryData4.m_EdgeLengthRange.max, num23 * 1.5f);
						netGeometryData4.m_MinNodeOffset = math.max(netGeometryData4.m_MinNodeOffset, num24);
						netGeometryData4.m_Flags |= GeometryFlags.BlockZone | GeometryFlags.Directional | GeometryFlags.FlattenTerrain | GeometryFlags.ClipTerrain;
						trackData.m_TrackType = prefab11.m_TrackType;
						trackData.m_SpeedLimit = prefab11.m_SpeedLimit / 3.6f;
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData4 = nativeArray5[num22];
							placeableNetData4.m_PlacementFlags |= PlacementFlags.OnGround;
							nativeArray5[num22] = placeableNetData4;
						}
						nativeArray3[num22] = netData2;
						nativeArray4[num22] = netGeometryData4;
						nativeArray19[num22] = trackData;
					}
				}
				NativeArray<WaterwayData> nativeArray20 = ((ArchetypeChunk)(ref val)).GetNativeArray<WaterwayData>(ref componentTypeHandle20);
				if (nativeArray20.Length != 0)
				{
					for (int num25 = 0; num25 < nativeArray20.Length; num25++)
					{
						WaterwayPrefab prefab12 = m_PrefabSystem.GetPrefab<WaterwayPrefab>(nativeArray2[num25]);
						NetData netData3 = nativeArray3[num25];
						NetGeometryData netGeometryData5 = nativeArray4[num25];
						WaterwayData waterwayData = nativeArray20[num25];
						netData3.m_RequiredLayers |= Layer.Waterway;
						netData3.m_ConnectLayers |= Layer.Waterway;
						netData3.m_LocalConnectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netGeometryData5.m_MergeLayers |= Layer.Waterway;
						netGeometryData5.m_IntersectLayers |= Layer.Waterway;
						netGeometryData5.m_EdgeLengthRange.max = 1000f;
						netGeometryData5.m_ElevatedLength = 1000f;
						netGeometryData5.m_Flags |= GeometryFlags.BlockZone | GeometryFlags.Directional | GeometryFlags.FlattenTerrain | GeometryFlags.OnWater;
						waterwayData.m_SpeedLimit = prefab12.m_SpeedLimit / 3.6f;
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData5 = nativeArray5[num25];
							placeableNetData5.m_PlacementFlags |= PlacementFlags.Floating;
							placeableNetData5.m_SnapDistance = 16f;
							nativeArray5[num25] = placeableNetData5;
						}
						nativeArray3[num25] = netData3;
						nativeArray4[num25] = netGeometryData5;
						nativeArray20[num25] = waterwayData;
					}
				}
				if (nativeArray6.Length != 0)
				{
					NativeArray<LocalConnectData> nativeArray21 = ((ArchetypeChunk)(ref val)).GetNativeArray<LocalConnectData>(ref componentTypeHandle8);
					for (int num26 = 0; num26 < nativeArray6.Length; num26++)
					{
						PathwayPrefab prefab13 = m_PrefabSystem.GetPrefab<PathwayPrefab>(nativeArray2[num26]);
						NetData netData4 = nativeArray3[num26];
						NetGeometryData netGeometryData6 = nativeArray4[num26];
						LocalConnectData localConnectData = nativeArray21[num26];
						PathwayData pathwayData2 = nativeArray6[num26];
						Layer layer3 = (flag2 ? Layer.MarkerPathway : Layer.Pathway);
						netData4.m_RequiredLayers |= layer3;
						netData4.m_ConnectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netData4.m_LocalConnectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netGeometryData6.m_MergeLayers |= layer3;
						netGeometryData6.m_IntersectLayers |= Layer.Pathway | Layer.MarkerPathway;
						netGeometryData6.m_ElevationLimit = 2f;
						netGeometryData6.m_Flags |= GeometryFlags.Directional;
						if (flag2)
						{
							netGeometryData6.m_ElevatedLength = netGeometryData6.m_EdgeLengthRange.max;
							netGeometryData6.m_Flags |= GeometryFlags.LoweredIsTunnel | GeometryFlags.RaisedIsElevated;
						}
						else
						{
							netGeometryData6.m_ElevatedLength = 40f;
							netGeometryData6.m_Flags |= GeometryFlags.BlockZone | GeometryFlags.FlattenTerrain | GeometryFlags.ClipTerrain;
						}
						localConnectData.m_Flags |= LocalConnectFlags.KeepOpen | LocalConnectFlags.RequireDeadend | LocalConnectFlags.ChooseBest | LocalConnectFlags.ChooseSides;
						localConnectData.m_Layers |= Layer.Road | Layer.TrainTrack | Layer.Pathway | Layer.Waterway | Layer.TramTrack | Layer.SubwayTrack | Layer.MarkerPathway | Layer.PublicTransportRoad;
						localConnectData.m_HeightRange = new Bounds1(-8f, 8f);
						localConnectData.m_SearchDistance = 4f;
						pathwayData2.m_SpeedLimit = prefab13.m_SpeedLimit / 3.6f;
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData6 = nativeArray5[num26];
							placeableNetData6.m_PlacementFlags |= PlacementFlags.OnGround;
							placeableNetData6.m_SnapDistance = (flag2 ? 2f : 4f);
							placeableNetData6.m_MinWaterElevation = 2.5f;
							nativeArray5[num26] = placeableNetData6;
						}
						nativeArray3[num26] = netData4;
						nativeArray4[num26] = netGeometryData6;
						nativeArray21[num26] = localConnectData;
						nativeArray6[num26] = pathwayData2;
					}
				}
				NativeArray<TaxiwayData> nativeArray22 = ((ArchetypeChunk)(ref val)).GetNativeArray<TaxiwayData>(ref componentTypeHandle22);
				if (nativeArray22.Length != 0)
				{
					for (int num27 = 0; num27 < nativeArray22.Length; num27++)
					{
						TaxiwayPrefab prefab14 = m_PrefabSystem.GetPrefab<TaxiwayPrefab>(nativeArray2[num27]);
						NetData netData5 = nativeArray3[num27];
						NetGeometryData netGeometryData7 = nativeArray4[num27];
						TaxiwayData taxiwayData = nativeArray22[num27];
						Layer layer4 = (flag2 ? Layer.MarkerTaxiway : Layer.Taxiway);
						netData5.m_RequiredLayers |= layer4;
						netData5.m_ConnectLayers |= Layer.Pathway | Layer.Taxiway | Layer.MarkerPathway | Layer.MarkerTaxiway;
						netGeometryData7.m_MergeLayers |= layer4;
						netGeometryData7.m_IntersectLayers |= Layer.Pathway | Layer.Taxiway | Layer.MarkerPathway | Layer.MarkerTaxiway;
						netGeometryData7.m_EdgeLengthRange.max = 1000f;
						netGeometryData7.m_ElevatedLength = 1000f;
						netGeometryData7.m_Flags |= GeometryFlags.Directional;
						if (!flag2)
						{
							netGeometryData7.m_Flags |= GeometryFlags.BlockZone | GeometryFlags.FlattenTerrain | GeometryFlags.ClipTerrain;
						}
						taxiwayData.m_SpeedLimit = prefab14.m_SpeedLimit / 3.6f;
						if (prefab14.m_Airspace)
						{
							if (prefab14.m_Runway)
							{
								taxiwayData.m_Flags |= TaxiwayFlags.Runway;
							}
							else if (!prefab14.m_Taxiway)
							{
								netGeometryData7.m_Flags |= GeometryFlags.RaisedIsElevated | GeometryFlags.BlockZone | GeometryFlags.FlattenTerrain;
							}
							taxiwayData.m_Flags |= TaxiwayFlags.Airspace;
						}
						else if (prefab14.m_Runway)
						{
							taxiwayData.m_Flags |= TaxiwayFlags.Runway;
						}
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData7 = nativeArray5[num27];
							placeableNetData7.m_PlacementFlags |= PlacementFlags.OnGround;
							placeableNetData7.m_SnapDistance = (flag2 ? 4f : 8f);
							nativeArray5[num27] = placeableNetData7;
						}
						nativeArray3[num27] = netData5;
						nativeArray4[num27] = netGeometryData7;
						nativeArray22[num27] = taxiwayData;
					}
				}
				bool flag10 = ((ArchetypeChunk)(ref val)).Has<PowerLineData>(ref componentTypeHandle23);
				if (flag10)
				{
					for (int num28 = 0; num28 < nativeArray.Length; num28++)
					{
						PowerLinePrefab prefab15 = m_PrefabSystem.GetPrefab<PowerLinePrefab>(nativeArray2[num28]);
						NetGeometryData netGeometryData8 = nativeArray4[num28];
						bool flag11 = false;
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData8 = nativeArray5[num28];
							placeableNetData8.m_PlacementFlags |= PlacementFlags.OnGround;
							flag11 = placeableNetData8.m_ElevationRange.max < 0f;
							nativeArray5[num28] = placeableNetData8;
						}
						netGeometryData8.m_EdgeLengthRange.max = prefab15.m_MaxPylonDistance;
						netGeometryData8.m_ElevatedLength = prefab15.m_MaxPylonDistance;
						netGeometryData8.m_Hanging = prefab15.m_Hanging;
						netGeometryData8.m_Flags |= GeometryFlags.StrictNodes | GeometryFlags.LoweredIsTunnel | GeometryFlags.RaisedIsElevated;
						if (!flag2)
						{
							netGeometryData8.m_Flags |= GeometryFlags.FlattenTerrain;
						}
						if (flag11)
						{
							netGeometryData8.m_IntersectLayers |= Layer.PowerlineLow | Layer.PowerlineHigh;
							netGeometryData8.m_MergeLayers |= Layer.PowerlineLow | Layer.PowerlineHigh;
						}
						else
						{
							netGeometryData8.m_Flags |= GeometryFlags.StraightEdges | GeometryFlags.NoEdgeConnection | GeometryFlags.SnapToNetAreas | GeometryFlags.BlockZone | GeometryFlags.StandingNodes;
						}
						nativeArray4[num28] = netGeometryData8;
					}
				}
				NativeArray<WaterPipeConnectionData> nativeArray23 = ((ArchetypeChunk)(ref val)).GetNativeArray<WaterPipeConnectionData>(ref componentTypeHandle28);
				NativeArray<ResourceConnectionData> nativeArray24 = ((ArchetypeChunk)(ref val)).GetNativeArray<ResourceConnectionData>(ref componentTypeHandle29);
				bool flag12 = ((ArchetypeChunk)(ref val)).Has<PipelineData>(ref componentTypeHandle24);
				if (flag12)
				{
					for (int num29 = 0; num29 < nativeArray.Length; num29++)
					{
						m_PrefabSystem.GetPrefab<PipelinePrefab>(nativeArray2[num29]);
						NetGeometryData netGeometryData9 = nativeArray4[num29];
						netGeometryData9.m_ElevatedLength = netGeometryData9.m_EdgeLengthRange.max;
						netGeometryData9.m_Flags |= GeometryFlags.StrictNodes | GeometryFlags.LoweredIsTunnel | GeometryFlags.RaisedIsElevated;
						if (nativeArray23.Length != 0)
						{
							netGeometryData9.m_IntersectLayers |= Layer.WaterPipe | Layer.SewagePipe | Layer.StormwaterPipe;
							netGeometryData9.m_MergeLayers |= Layer.WaterPipe | Layer.SewagePipe | Layer.StormwaterPipe;
						}
						if (nativeArray24.Length != 0)
						{
							netGeometryData9.m_IntersectLayers |= Layer.ResourceLine;
							netGeometryData9.m_MergeLayers |= Layer.ResourceLine;
						}
						if (!flag2)
						{
							netGeometryData9.m_Flags |= GeometryFlags.FlattenTerrain;
						}
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData9 = nativeArray5[num29];
							placeableNetData9.m_PlacementFlags |= PlacementFlags.OnGround;
							nativeArray5[num29] = placeableNetData9;
						}
						nativeArray4[num29] = netGeometryData9;
					}
				}
				if (((ArchetypeChunk)(ref val)).Has<FenceData>(ref componentTypeHandle25))
				{
					for (int num30 = 0; num30 < nativeArray.Length; num30++)
					{
						m_PrefabSystem.GetPrefab<FencePrefab>(nativeArray2[num30]);
						NetData netData6 = nativeArray3[num30];
						NetGeometryData netGeometryData10 = nativeArray4[num30];
						netData6.m_RequiredLayers |= Layer.Fence;
						netData6.m_ConnectLayers |= Layer.Fence;
						netGeometryData10.m_ElevatedLength = netGeometryData10.m_EdgeLengthRange.max;
						netGeometryData10.m_Flags |= GeometryFlags.StrictNodes | GeometryFlags.BlockZone | GeometryFlags.FlattenTerrain;
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData10 = nativeArray5[num30];
							placeableNetData10.m_PlacementFlags |= PlacementFlags.OnGround;
							placeableNetData10.m_SnapDistance = 4f;
							nativeArray5[num30] = placeableNetData10;
						}
						nativeArray3[num30] = netData6;
						nativeArray4[num30] = netGeometryData10;
					}
				}
				if (((ArchetypeChunk)(ref val)).Has<EditorContainerData>(ref componentTypeHandle26))
				{
					for (int num31 = 0; num31 < nativeArray3.Length; num31++)
					{
						NetData netData7 = nativeArray3[num31];
						netData7.m_RequiredLayers |= Layer.LaneEditor;
						netData7.m_ConnectLayers |= Layer.LaneEditor;
						nativeArray3[num31] = netData7;
					}
				}
				if (flag3)
				{
					BufferAccessor<FixedNetElement> bufferAccessor13 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<FixedNetElement>(ref bufferTypeHandle11);
					for (int num32 = 0; num32 < nativeArray4.Length; num32++)
					{
						NetGeometryPrefab prefab16 = m_PrefabSystem.GetPrefab<NetGeometryPrefab>(nativeArray2[num32]);
						Bridge component24 = prefab16.GetComponent<Bridge>();
						NetData netData8 = nativeArray3[num32];
						NetGeometryData netGeometryData11 = nativeArray4[num32];
						netData8.m_NodePriority += 1000f;
						if (component24.m_SegmentLength > 0.1f)
						{
							if (!component24.m_AllowMinimalLength)
							{
								netGeometryData11.m_EdgeLengthRange.min = component24.m_SegmentLength * 0.6f;
							}
							netGeometryData11.m_EdgeLengthRange.max = component24.m_SegmentLength * 1.4f;
						}
						netGeometryData11.m_ElevatedLength = netGeometryData11.m_EdgeLengthRange.max;
						netGeometryData11.m_Hanging = component24.m_Hanging;
						netGeometryData11.m_Flags |= GeometryFlags.StraightEdges | GeometryFlags.StraightEnds | GeometryFlags.RequireElevated | GeometryFlags.SymmetricalEdges | GeometryFlags.SmoothSlopes;
						if (component24.m_CanCurve)
						{
							netGeometryData11.m_Flags &= ~GeometryFlags.StraightEdges;
						}
						switch (component24.m_BuildStyle)
						{
						case BridgeBuildStyle.Raised:
							netGeometryData11.m_Flags |= GeometryFlags.ElevatedIsRaised;
							break;
						case BridgeBuildStyle.Quay:
							netGeometryData11.m_Flags |= GeometryFlags.ElevatedIsRaised;
							netGeometryData11.m_Flags &= ~GeometryFlags.RequireElevated;
							break;
						}
						if (nativeArray5.Length != 0)
						{
							PlaceableNetData placeableNetData11 = nativeArray5[num32];
							switch (component24.m_WaterFlow)
							{
							case BridgeWaterFlow.Left:
								placeableNetData11.m_PlacementFlags |= PlacementFlags.FlowLeft;
								break;
							case BridgeWaterFlow.Right:
								placeableNetData11.m_PlacementFlags |= PlacementFlags.FlowRight;
								break;
							}
							if (component24.m_BuildStyle == BridgeBuildStyle.Quay)
							{
								placeableNetData11.m_PlacementFlags |= PlacementFlags.ShoreLine;
							}
							placeableNetData11.m_MinWaterElevation = component24.m_ElevationOnWater;
							nativeArray5[num32] = placeableNetData11;
						}
						if (bufferAccessor13.Length != 0)
						{
							DynamicBuffer<FixedNetElement> val17 = bufferAccessor13[num32];
							val17.ResizeUninitialized(component24.m_FixedSegments.Length);
							int num33 = 0;
							bool flag13 = false;
							for (int num34 = 0; num34 < val17.Length; num34++)
							{
								FixedNetSegmentInfo fixedNetSegmentInfo = component24.m_FixedSegments[num34];
								flag13 |= fixedNetSegmentInfo.m_Length <= 0.1f;
							}
							for (int num35 = 0; num35 < val17.Length; num35++)
							{
								FixedNetSegmentInfo fixedNetSegmentInfo2 = component24.m_FixedSegments[num35];
								fixedNetElement.m_Flags = (FixedNetFlags)0u;
								if (fixedNetSegmentInfo2.m_Length > 0.1f)
								{
									if (flag13)
									{
										fixedNetElement.m_LengthRange.min = fixedNetSegmentInfo2.m_Length;
										fixedNetElement.m_LengthRange.max = fixedNetSegmentInfo2.m_Length;
									}
									else
									{
										fixedNetElement.m_LengthRange.min = fixedNetSegmentInfo2.m_Length * 0.6f;
										fixedNetElement.m_LengthRange.max = fixedNetSegmentInfo2.m_Length * 1.4f;
									}
								}
								else
								{
									fixedNetElement.m_LengthRange = netGeometryData11.m_EdgeLengthRange;
								}
								if (fixedNetSegmentInfo2.m_CanCurve)
								{
									netGeometryData11.m_Flags &= ~GeometryFlags.StraightEdges;
									num33++;
								}
								else
								{
									fixedNetElement.m_Flags |= FixedNetFlags.Straight;
								}
								fixedNetElement.m_CountRange = fixedNetSegmentInfo2.m_CountRange;
								NetCompositionHelpers.GetRequirementFlags(fixedNetSegmentInfo2.m_SetState, out fixedNetElement.m_SetState, out var sectionFlags14);
								NetCompositionHelpers.GetRequirementFlags(fixedNetSegmentInfo2.m_UnsetState, out fixedNetElement.m_UnsetState, out var sectionFlags15);
								if ((sectionFlags14 | sectionFlags15) != 0)
								{
									COSystemBase.baseLog.ErrorFormat((Object)(object)prefab16, "Net segment state ({0}) cannot (un)set section flags: {1}", (object)((Object)prefab16).name, (object)(sectionFlags14 | sectionFlags15));
								}
								val17[num35] = fixedNetElement;
							}
							if (num33 >= 2)
							{
								netGeometryData11.m_Flags |= GeometryFlags.NoCurveSplit;
							}
						}
						nativeArray3[num32] = netData8;
						nativeArray4[num32] = netGeometryData11;
					}
				}
				NativeArray<ElectricityConnectionData> nativeArray25 = ((ArchetypeChunk)(ref val)).GetNativeArray<ElectricityConnectionData>(ref componentTypeHandle27);
				if (nativeArray25.Length != 0)
				{
					NativeArray<LocalConnectData> nativeArray26 = ((ArchetypeChunk)(ref val)).GetNativeArray<LocalConnectData>(ref componentTypeHandle8);
					for (int num36 = 0; num36 < nativeArray25.Length; num36++)
					{
						NetPrefab prefab17 = m_PrefabSystem.GetPrefab<NetPrefab>(nativeArray2[num36]);
						ElectricityConnection component25 = prefab17.GetComponent<ElectricityConnection>();
						NetData netData9 = nativeArray3[num36];
						ElectricityConnectionData electricityConnectionData = nativeArray25[num36];
						Layer layer5;
						Layer layer6;
						float snapDistance;
						switch (component25.m_Voltage)
						{
						case ElectricityConnection.Voltage.Low:
							layer5 = Layer.PowerlineLow;
							layer6 = Layer.Road | Layer.PowerlineLow;
							snapDistance = 4f;
							break;
						case ElectricityConnection.Voltage.High:
							layer5 = Layer.PowerlineHigh;
							layer6 = Layer.PowerlineHigh;
							snapDistance = 8f;
							break;
						default:
							layer5 = Layer.None;
							layer6 = Layer.None;
							snapDistance = 8f;
							break;
						}
						if (flag10)
						{
							netData9.m_RequiredLayers |= layer5;
							netData9.m_ConnectLayers |= layer5;
							LocalConnectData localConnectData2 = nativeArray26[num36];
							localConnectData2.m_Flags |= LocalConnectFlags.ExplicitNodes | LocalConnectFlags.ChooseBest;
							localConnectData2.m_Layers |= layer6;
							localConnectData2.m_HeightRange = new Bounds1(-1000f, 1000f);
							localConnectData2.m_SearchDistance = 0f;
							if (flag2)
							{
								localConnectData2.m_Flags |= LocalConnectFlags.KeepOpen;
								localConnectData2.m_SearchDistance = 4f;
							}
							nativeArray26[num36] = localConnectData2;
							if (nativeArray5.Length != 0)
							{
								PlaceableNetData placeableNetData12 = nativeArray5[num36];
								placeableNetData12.m_SnapDistance = snapDistance;
								nativeArray5[num36] = placeableNetData12;
							}
						}
						netData9.m_LocalConnectLayers |= layer5;
						electricityConnectionData.m_Direction = component25.m_Direction;
						electricityConnectionData.m_Capacity = component25.m_Capacity;
						electricityConnectionData.m_Voltage = component25.m_Voltage;
						NetCompositionHelpers.GetRequirementFlags(component25.m_RequireAll, out electricityConnectionData.m_CompositionAll, out var sectionFlags16);
						NetCompositionHelpers.GetRequirementFlags(component25.m_RequireAny, out electricityConnectionData.m_CompositionAny, out var sectionFlags17);
						NetCompositionHelpers.GetRequirementFlags(component25.m_RequireNone, out electricityConnectionData.m_CompositionNone, out var sectionFlags18);
						NetSectionFlags netSectionFlags5 = sectionFlags16 | sectionFlags17 | sectionFlags18;
						if (netSectionFlags5 != 0)
						{
							COSystemBase.baseLog.ErrorFormat((Object)(object)prefab17, "Electricity connection ({0}) cannot require section flags: {1}", (object)((Object)prefab17).name, (object)netSectionFlags5);
						}
						nativeArray3[num36] = netData9;
						nativeArray25[num36] = electricityConnectionData;
					}
				}
				if (nativeArray23.Length != 0)
				{
					NativeArray<LocalConnectData> nativeArray27 = ((ArchetypeChunk)(ref val)).GetNativeArray<LocalConnectData>(ref componentTypeHandle8);
					for (int num37 = 0; num37 < nativeArray23.Length; num37++)
					{
						WaterPipeConnection component26 = m_PrefabSystem.GetPrefab<NetPrefab>(nativeArray2[num37]).GetComponent<WaterPipeConnection>();
						NetData netData10 = nativeArray3[num37];
						WaterPipeConnectionData waterPipeConnectionData = nativeArray23[num37];
						Layer layer7 = Layer.None;
						if (component26.m_FreshCapacity != 0)
						{
							layer7 |= Layer.WaterPipe;
						}
						if (component26.m_SewageCapacity != 0)
						{
							layer7 |= Layer.SewagePipe;
						}
						if (component26.m_StormCapacity != 0)
						{
							layer7 |= Layer.StormwaterPipe;
						}
						if (flag12)
						{
							netData10.m_RequiredLayers |= layer7;
							netData10.m_ConnectLayers |= layer7;
							LocalConnectData localConnectData3 = nativeArray27[num37];
							localConnectData3.m_Flags |= LocalConnectFlags.ExplicitNodes | LocalConnectFlags.ChooseBest;
							localConnectData3.m_Layers |= Layer.Road | layer7;
							localConnectData3.m_HeightRange = new Bounds1(-1000f, 1000f);
							localConnectData3.m_SearchDistance = 0f;
							if (flag2)
							{
								localConnectData3.m_Flags |= LocalConnectFlags.KeepOpen;
								localConnectData3.m_SearchDistance = 4f;
							}
							nativeArray27[num37] = localConnectData3;
							if (nativeArray5.Length != 0)
							{
								PlaceableNetData placeableNetData13 = nativeArray5[num37];
								placeableNetData13.m_SnapDistance = 4f;
								nativeArray5[num37] = placeableNetData13;
							}
						}
						netData10.m_LocalConnectLayers |= layer7;
						waterPipeConnectionData.m_FreshCapacity = component26.m_FreshCapacity;
						waterPipeConnectionData.m_SewageCapacity = component26.m_SewageCapacity;
						waterPipeConnectionData.m_StormCapacity = component26.m_StormCapacity;
						nativeArray3[num37] = netData10;
						nativeArray23[num37] = waterPipeConnectionData;
					}
				}
				if (nativeArray24.Length != 0)
				{
					NativeArray<LocalConnectData> nativeArray28 = ((ArchetypeChunk)(ref val)).GetNativeArray<LocalConnectData>(ref componentTypeHandle8);
					for (int num38 = 0; num38 < nativeArray.Length; num38++)
					{
						NetData netData11 = nativeArray3[num38];
						if (flag12)
						{
							netData11.m_RequiredLayers |= Layer.ResourceLine;
							netData11.m_ConnectLayers |= Layer.ResourceLine;
							LocalConnectData localConnectData4 = nativeArray28[num38];
							localConnectData4.m_Flags |= LocalConnectFlags.ExplicitNodes | LocalConnectFlags.ChooseBest;
							localConnectData4.m_Layers |= Layer.Pathway | Layer.ResourceLine;
							localConnectData4.m_HeightRange = new Bounds1(-1000f, 1000f);
							localConnectData4.m_SearchDistance = 0f;
							if (flag2)
							{
								localConnectData4.m_Flags |= LocalConnectFlags.KeepOpen;
								localConnectData4.m_SearchDistance = 4f;
							}
							nativeArray28[num38] = localConnectData4;
							if (nativeArray5.Length != 0)
							{
								PlaceableNetData placeableNetData14 = nativeArray5[num38];
								placeableNetData14.m_SnapDistance = 4f;
								nativeArray5[num38] = placeableNetData14;
							}
						}
						netData11.m_LocalConnectLayers |= Layer.ResourceLine;
						nativeArray3[num38] = netData11;
					}
				}
				if (flag4)
				{
					for (int num39 = 0; num39 < nativeArray3.Length; num39++)
					{
						NetData netData12 = nativeArray3[num39];
						m_InGameLayersTwice |= m_InGameLayersOnce & netData12.m_RequiredLayers;
						m_InGameLayersOnce |= netData12.m_RequiredLayers;
					}
				}
			}
		}
		catch
		{
			chunks.Dispose();
			throw;
		}
		m_PathfindHeuristicData.value = new PathfindHeuristicData
		{
			m_CarCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f),
			m_TrackCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f),
			m_PedestrianCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f),
			m_FlyingCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f),
			m_TaxiCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f),
			m_OffRoadCosts = new PathfindCosts(1000000f, 1000000f, 1000000f, 1000000f)
		};
		InitializeNetDefaultsJob initializeNetDefaultsJob = new InitializeNetDefaultsJob
		{
			m_Chunks = chunks,
			m_NetGeometrySectionType = InternalCompilerInterface.GetBufferTypeHandle<NetGeometrySection>(ref __TypeHandle.__Game_Prefabs_NetGeometrySection_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetType = InternalCompilerInterface.GetComponentTypeHandle<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetType = InternalCompilerInterface.GetComponentTypeHandle<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DefaultNetLaneType = InternalCompilerInterface.GetBufferTypeHandle<DefaultNetLane>(ref __TypeHandle.__Game_Prefabs_DefaultNetLane_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceData = InternalCompilerInterface.GetComponentLookup<NetPieceData>(ref __TypeHandle.__Game_Prefabs_NetPieceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetVertexMatchData = InternalCompilerInterface.GetComponentLookup<NetVertexMatchData>(ref __TypeHandle.__Game_Prefabs_NetVertexMatchData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableNetPieceData = InternalCompilerInterface.GetComponentLookup<PlaceableNetPieceData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetPieceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSubSectionData = InternalCompilerInterface.GetBufferLookup<NetSubSection>(ref __TypeHandle.__Game_Prefabs_NetSubSection_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSectionPieceData = InternalCompilerInterface.GetBufferLookup<NetSectionPiece>(ref __TypeHandle.__Game_Prefabs_NetSectionPiece_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceLanes = InternalCompilerInterface.GetBufferLookup<NetPieceLane>(ref __TypeHandle.__Game_Prefabs_NetPieceLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPieceObjects = InternalCompilerInterface.GetBufferLookup<NetPieceObject>(ref __TypeHandle.__Game_Prefabs_NetPieceObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		CollectPathfindDataJob obj5 = new CollectPathfindDataJob
		{
			m_NetLaneDataType = InternalCompilerInterface.GetComponentTypeHandle<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectionLaneDataType = InternalCompilerInterface.GetComponentTypeHandle<ConnectionLaneData>(ref __TypeHandle.__Game_Prefabs_ConnectionLaneData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindCarData = InternalCompilerInterface.GetComponentLookup<PathfindCarData>(ref __TypeHandle.__Game_Prefabs_PathfindCarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindTrackData = InternalCompilerInterface.GetComponentLookup<PathfindTrackData>(ref __TypeHandle.__Game_Prefabs_PathfindTrackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindPedestrianData = InternalCompilerInterface.GetComponentLookup<PathfindPedestrianData>(ref __TypeHandle.__Game_Prefabs_PathfindPedestrianData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindTransportData = InternalCompilerInterface.GetComponentLookup<PathfindTransportData>(ref __TypeHandle.__Game_Prefabs_PathfindTransportData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindConnectionData = InternalCompilerInterface.GetComponentLookup<PathfindConnectionData>(ref __TypeHandle.__Game_Prefabs_PathfindConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PathfindHeuristicData = m_PathfindHeuristicData
		};
		JobHandle val18 = IJobParallelForExtensions.Schedule<InitializeNetDefaultsJob>(initializeNetDefaultsJob, chunks.Length, 1, ((SystemBase)this).Dependency);
		JobHandle val19 = JobHandle.CombineDependencies(val18, m_PathfindHeuristicDeps = JobChunkExtensions.Schedule<CollectPathfindDataJob>(obj5, m_LaneQuery, ((SystemBase)this).Dependency));
		if (flag)
		{
			JobHandle val20 = JobChunkExtensions.ScheduleParallel<FixPlaceholdersJob>(new FixPlaceholdersJob
			{
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceholderObjectElementType = InternalCompilerInterface.GetBufferTypeHandle<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			}, m_PlaceholderQuery, ((SystemBase)this).Dependency);
			val19 = JobHandle.CombineDependencies(val19, val20);
		}
		((SystemBase)this).Dependency = val19;
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
	public NetInitializeSystem()
	{
	}
}
