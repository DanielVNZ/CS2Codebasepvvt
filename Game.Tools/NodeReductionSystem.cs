using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class NodeReductionSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindCandidatesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<OwnerDefinition> m_OwnerDefinitionData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public bool m_EditorMode;

		public ParallelWriter<ReductionData> m_ReductionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity node = nativeArray[i];
				if (CanMove(node, out var data))
				{
					m_ReductionQueue.Enqueue(data);
				}
			}
		}

		private bool CanMove(Entity node, out ReductionData data)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0579: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0662: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0617: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			data = new ReductionData
			{
				m_Node = node
			};
			Temp temp = m_TempData[node];
			if (temp.m_Original == Entity.Null)
			{
				return false;
			}
			if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Replace | TempFlags.Upgrade)) != 0)
			{
				return false;
			}
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData, includeMiddleConnections: true);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				if (value.m_Middle)
				{
					return false;
				}
				if (!m_TempData.HasComponent(value.m_Edge))
				{
					return false;
				}
				if (m_FixedData.HasComponent(value.m_Edge))
				{
					return false;
				}
				if (val == Entity.Null)
				{
					val = value.m_Edge;
					continue;
				}
				if (val2 == Entity.Null)
				{
					val2 = value.m_Edge;
					continue;
				}
				return false;
			}
			if (val2 == Entity.Null)
			{
				return false;
			}
			Edge edge = m_EdgeData[val];
			Edge edge2 = m_EdgeData[val2];
			bool2 val3 = default(bool2);
			((bool2)(ref val3))._002Ector(edge.m_Start == node, edge2.m_Start == node);
			bool2 val4 = default(bool2);
			((bool2)(ref val4))._002Ector(edge.m_End == node, edge2.m_End == node);
			if (math.any(val3 == val4))
			{
				return false;
			}
			PrefabRef prefabRef = m_PrefabRefData[val];
			PrefabRef prefabRef2 = m_PrefabRefData[val2];
			if (prefabRef.m_Prefab != prefabRef2.m_Prefab)
			{
				return false;
			}
			NetGeometryData prefabGeometryData = default(NetGeometryData);
			if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
			{
				prefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				if (val3.x == val3.y && (prefabGeometryData.m_Flags & GeometryFlags.Asymmetric) != 0)
				{
					return false;
				}
				data.m_EdgeLengthRange = prefabGeometryData.m_EdgeLengthRange;
				data.m_SnapCellSize = (prefabGeometryData.m_Flags & GeometryFlags.SnapCellSize) != 0;
				data.m_ForbidMove = (prefabGeometryData.m_Flags & GeometryFlags.StraightEdges) != 0;
				data.m_NoEdgeConnection = (prefabGeometryData.m_Flags & GeometryFlags.NoEdgeConnection) != 0;
			}
			CompositionFlags compositionFlags = GetElevationFlags(val, edge.m_Start, edge.m_End, prefabGeometryData);
			CompositionFlags compositionFlags2 = GetElevationFlags(val2, edge2.m_Start, edge2.m_End, prefabGeometryData);
			if (val3.x)
			{
				compositionFlags = NetCompositionHelpers.InvertCompositionFlags(compositionFlags);
			}
			if (val4.y)
			{
				compositionFlags2 = NetCompositionHelpers.InvertCompositionFlags(compositionFlags2);
			}
			CompositionFlags compositionFlags3 = new CompositionFlags(CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel, CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered, CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered);
			if (((compositionFlags ^ compositionFlags2) & compositionFlags3) != default(CompositionFlags))
			{
				return false;
			}
			data.m_EdgeLengthRange.max = math.select(data.m_EdgeLengthRange.max, prefabGeometryData.m_ElevatedLength, (compositionFlags.m_General & CompositionFlags.General.Elevated) != 0);
			data.m_ForbidMove |= (compositionFlags.m_General & CompositionFlags.General.Elevated) != 0;
			data.m_CheckHeight |= (compositionFlags.m_General & compositionFlags3.m_General) != 0;
			data.m_CheckHeight |= (compositionFlags.m_Left & compositionFlags3.m_Left) != 0 && (compositionFlags.m_Right & compositionFlags3.m_Right) != 0;
			Upgraded upgraded = default(Upgraded);
			Upgraded upgraded2 = default(Upgraded);
			if (m_UpgradedData.HasComponent(val))
			{
				upgraded = m_UpgradedData[val];
				if (val3.x)
				{
					upgraded.m_Flags = NetCompositionHelpers.InvertCompositionFlags(upgraded.m_Flags);
				}
			}
			if (m_UpgradedData.HasComponent(val2))
			{
				upgraded2 = m_UpgradedData[val2];
				if (val4.y)
				{
					upgraded2.m_Flags = NetCompositionHelpers.InvertCompositionFlags(upgraded2.m_Flags);
				}
			}
			if (upgraded.m_Flags != upgraded2.m_Flags)
			{
				return false;
			}
			Owner owner = default(Owner);
			Owner owner2 = default(Owner);
			Owner owner3 = default(Owner);
			if (m_OwnerData.HasComponent(node))
			{
				owner = m_OwnerData[node];
			}
			if (m_OwnerData.HasComponent(val))
			{
				owner2 = m_OwnerData[val];
			}
			if (m_OwnerData.HasComponent(val2))
			{
				owner3 = m_OwnerData[val2];
			}
			if (m_EditorMode)
			{
				if (owner2.m_Owner != owner.m_Owner || owner3.m_Owner != owner.m_Owner)
				{
					return false;
				}
			}
			else if (owner.m_Owner != Entity.Null || owner2.m_Owner != Entity.Null || owner3.m_Owner != Entity.Null)
			{
				return false;
			}
			OwnerDefinition other = default(OwnerDefinition);
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			OwnerDefinition ownerDefinition2 = default(OwnerDefinition);
			if (m_OwnerDefinitionData.HasComponent(node))
			{
				other = m_OwnerDefinitionData[node];
			}
			if (m_OwnerDefinitionData.HasComponent(val))
			{
				ownerDefinition = m_OwnerDefinitionData[val];
			}
			if (m_OwnerDefinitionData.HasComponent(val2))
			{
				ownerDefinition2 = m_OwnerDefinitionData[val2];
			}
			if (m_EditorMode)
			{
				if (!ownerDefinition.Equals(other) || !ownerDefinition2.Equals(other))
				{
					return false;
				}
			}
			else if (other.m_Prefab != Entity.Null || ownerDefinition.m_Prefab != Entity.Null || ownerDefinition2.m_Prefab != Entity.Null)
			{
				return false;
			}
			Curve curve = m_CurveData[val];
			Curve curve2 = m_CurveData[val2];
			if (val3.x)
			{
				curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
			}
			if (val4.y)
			{
				curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
			}
			float3 val6;
			if (data.m_CheckHeight)
			{
				float3 val5 = MathUtils.EndTangent(curve.m_Bezier);
				val6 = default(float3);
				float3 val7 = math.normalizesafe(val5, val6);
				float3 val8 = MathUtils.StartTangent(curve2.m_Bezier);
				val6 = default(float3);
				float3 val9 = math.normalizesafe(val8, val6);
				return math.dot(val7, val9) >= 0.9995f;
			}
			val6 = MathUtils.EndTangent(curve.m_Bezier);
			float2 val10 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
			val6 = MathUtils.StartTangent(curve2.m_Bezier);
			float2 val11 = math.normalizesafe(((float3)(ref val6)).xz, default(float2));
			return math.dot(val10, val11) >= 0.9995f;
		}

		private CompositionFlags GetElevationFlags(Entity edge, Entity startNode, Entity endNode, NetGeometryData prefabGeometryData)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Elevation startElevation = default(Elevation);
			Elevation middleElevation = default(Elevation);
			Elevation endElevation = default(Elevation);
			if (m_ElevationData.HasComponent(startNode))
			{
				startElevation = m_ElevationData[startNode];
			}
			if (m_ElevationData.HasComponent(edge))
			{
				middleElevation = m_ElevationData[edge];
			}
			if (m_ElevationData.HasComponent(endNode))
			{
				endElevation = m_ElevationData[endNode];
			}
			return NetCompositionHelpers.GetElevationFlags(startElevation, middleElevation, endElevation, prefabGeometryData);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct ReductionData
	{
		public Entity m_Node;

		public Bounds1 m_EdgeLengthRange;

		public bool m_SnapCellSize;

		public bool m_ForbidMove;

		public bool m_CheckHeight;

		public bool m_NoEdgeConnection;
	}

	[BurstCompile]
	private struct NodeReductionJob : IJob
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_LocalConnectData;

		public ComponentLookup<Node> m_NodeData;

		public ComponentLookup<Edge> m_EdgeData;

		public ComponentLookup<Curve> m_CurveData;

		public ComponentLookup<Temp> m_TempData;

		public ComponentLookup<BuildOrder> m_BuildOrderData;

		public ComponentLookup<Road> m_RoadData;

		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public BufferLookup<ConnectedNode> m_ConnectedNodes;

		public NativeQueue<ReductionData> m_ReductionQueue;

		public void Execute()
		{
			int count = m_ReductionQueue.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					ReductionData data = m_ReductionQueue.Dequeue();
					TryReduceOrMove(data);
				}
			}
		}

		private void TryReduceOrMove(ReductionData data)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0613: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0644: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_0749: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0666: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0814: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0830: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Entity val2 = Entity.Null;
			DynamicBuffer<ConnectedEdge> val3 = m_ConnectedEdges[data.m_Node];
			for (int i = 0; i < val3.Length; i++)
			{
				Entity edge = val3[i].m_Edge;
				if ((m_TempData[edge].m_Flags & TempFlags.Delete) != 0)
				{
					continue;
				}
				if (val == Entity.Null)
				{
					val = edge;
					continue;
				}
				if (val2 == Entity.Null)
				{
					val2 = edge;
					continue;
				}
				return;
			}
			if (val2 == Entity.Null)
			{
				return;
			}
			Temp temp = m_TempData[data.m_Node];
			Temp temp2 = m_TempData[val];
			Temp temp3 = m_TempData[val2];
			Edge edge2 = m_EdgeData[val];
			Edge edge3 = m_EdgeData[val2];
			bool2 val4 = default(bool2);
			((bool2)(ref val4))._002Ector(edge2.m_Start == data.m_Node, edge3.m_Start == data.m_Node);
			bool2 val5 = default(bool2);
			((bool2)(ref val5))._002Ector(edge2.m_End == data.m_Node, edge3.m_End == data.m_Node);
			Curve curve = m_CurveData[val];
			Curve curve2 = m_CurveData[val2];
			if (val4.x)
			{
				curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
			}
			if (val5.y)
			{
				curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
			}
			float num = MathUtils.Length(((Bezier4x3)(ref curve.m_Bezier)).xz);
			float num2 = MathUtils.Length(((Bezier4x3)(ref curve2.m_Bezier)).xz);
			if (data.m_EdgeLengthRange.max == 0f || num + num2 <= data.m_EdgeLengthRange.max)
			{
				bool flag = ((temp2.m_Original != Entity.Null == (temp3.m_Original != Entity.Null)) ? (num < num2) : (temp2.m_Original != Entity.Null));
				if (!TryJoinCurve(curve.m_Bezier, curve2.m_Bezier, data.m_CheckHeight, out var curve3))
				{
					return;
				}
				if (temp2.m_Original != Entity.Null)
				{
					FixStartSlope(curve.m_Bezier, ref curve3);
				}
				if (temp3.m_Original != Entity.Null)
				{
					FixEndSlope(curve2.m_Bezier, ref curve3);
				}
				temp.m_Flags = TempFlags.Delete | TempFlags.Hidden;
				m_TempData[data.m_Node] = temp;
				if (flag)
				{
					if ((temp2.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						temp3.m_Flags |= TempFlags.Modify;
					}
					temp3.m_Flags |= temp2.m_Flags & (TempFlags.Essential | TempFlags.Upgrade | TempFlags.Parent);
					if (temp3.m_Original != Entity.Null)
					{
						temp3.m_Flags |= TempFlags.Combine;
					}
					temp2.m_Flags = (temp3.m_Flags & TempFlags.Essential) | (TempFlags.Delete | TempFlags.Hidden);
					if ((temp3.m_Flags & TempFlags.Essential) != 0 && (temp3.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0)
					{
						temp2.m_Flags |= TempFlags.RemoveCost;
					}
					curve2.m_Bezier = curve3;
					curve2.m_Length = MathUtils.Length(curve3);
					if (val5.y)
					{
						curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
						ReplaceEdgeConnection(ref edge3.m_End, val2, val4.x ? edge2.m_End : edge2.m_Start);
					}
					else
					{
						ReplaceEdgeConnection(ref edge3.m_Start, val2, val4.x ? edge2.m_End : edge2.m_Start);
					}
					ReplaceEdgeData(val, val2, val5.x, val4.y);
					MoveConnectedNodes(val, val2, curve2, data.m_NoEdgeConnection);
					m_CurveData[val2] = curve2;
					m_EdgeData[val2] = edge3;
				}
				else
				{
					if ((temp3.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace)) != 0)
					{
						temp2.m_Flags |= TempFlags.Modify;
					}
					temp2.m_Flags |= temp3.m_Flags & (TempFlags.Essential | TempFlags.Upgrade | TempFlags.Parent);
					if (temp2.m_Original != Entity.Null)
					{
						temp2.m_Flags |= TempFlags.Combine;
					}
					temp3.m_Flags = (temp2.m_Flags & TempFlags.Essential) | (TempFlags.Delete | TempFlags.Hidden);
					if ((temp2.m_Flags & TempFlags.Essential) != 0 && (temp2.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0)
					{
						temp3.m_Flags |= TempFlags.RemoveCost;
					}
					curve.m_Bezier = curve3;
					curve.m_Length = MathUtils.Length(curve3);
					if (val4.x)
					{
						curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
						ReplaceEdgeConnection(ref edge2.m_Start, val, val5.y ? edge3.m_Start : edge3.m_End);
					}
					else
					{
						ReplaceEdgeConnection(ref edge2.m_End, val, val5.y ? edge3.m_Start : edge3.m_End);
					}
					ReplaceEdgeData(val2, val, val5.y, val4.x);
					MoveConnectedNodes(val2, val, curve, data.m_NoEdgeConnection);
					m_CurveData[val] = curve;
					m_EdgeData[val] = edge2;
				}
				m_TempData[val] = temp2;
				m_TempData[val2] = temp3;
			}
			else
			{
				if (data.m_ForbidMove || (!(num < data.m_EdgeLengthRange.max * 0.5f) && !(num2 < data.m_EdgeLengthRange.max * 0.5f)))
				{
					return;
				}
				float num3 = math.abs(num - num2) * 0.5f;
				if (data.m_SnapCellSize)
				{
					num3 = MathUtils.Snap(num3 - 1f, 8f);
				}
				if (num3 < 1f)
				{
					return;
				}
				if (num >= num2)
				{
					Bounds1 val6 = default(Bounds1);
					((Bounds1)(ref val6))._002Ector(0f, 1f);
					MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val6, num3);
					Bezier4x3 bezier = default(Bezier4x3);
					Bezier4x3 curve4 = default(Bezier4x3);
					MathUtils.Divide(curve.m_Bezier, ref bezier, ref curve4, val6.min);
					if (!TryJoinCurve(curve4, curve2.m_Bezier, data.m_CheckHeight, out var curve5))
					{
						return;
					}
					if (temp3.m_Original != Entity.Null)
					{
						FixEndSlope(curve2.m_Bezier, ref curve5);
					}
					curve.m_Bezier = bezier;
					curve2.m_Bezier = curve5;
					if ((temp3.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0 && (temp2.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) != 0)
					{
						temp3.m_Flags |= (TempFlags)(((temp2.m_Flags & TempFlags.Parent) != 0) ? 2048 : 64);
					}
				}
				else
				{
					Bounds1 val7 = default(Bounds1);
					((Bounds1)(ref val7))._002Ector(0f, 1f);
					MathUtils.ClampLength(((Bezier4x3)(ref curve2.m_Bezier)).xz, ref val7, num3);
					Bezier4x3 curve6 = default(Bezier4x3);
					Bezier4x3 bezier2 = default(Bezier4x3);
					MathUtils.Divide(curve2.m_Bezier, ref curve6, ref bezier2, val7.max);
					if (!TryJoinCurve(curve.m_Bezier, curve6, data.m_CheckHeight, out var curve7))
					{
						return;
					}
					if (temp2.m_Original != Entity.Null)
					{
						FixStartSlope(curve.m_Bezier, ref curve7);
					}
					curve2.m_Bezier = bezier2;
					curve.m_Bezier = curve7;
					if ((temp2.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) == 0 && (temp3.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Parent)) != 0)
					{
						temp2.m_Flags |= (TempFlags)(((temp3.m_Flags & TempFlags.Parent) != 0) ? 2048 : 64);
					}
				}
				bool flag2 = (temp2.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0;
				bool flag3 = (temp3.m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Replace | TempFlags.Upgrade)) != 0;
				if ((temp2.m_Flags & TempFlags.Essential) != 0 && flag2 && !flag3)
				{
					temp3.m_Flags |= TempFlags.RemoveCost;
				}
				if ((temp3.m_Flags & TempFlags.Essential) != 0 && flag3 && !flag2)
				{
					temp2.m_Flags |= TempFlags.RemoveCost;
				}
				if (val4.x)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				}
				if (val5.y)
				{
					curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
				}
				curve.m_Length = MathUtils.Length(curve.m_Bezier);
				curve2.m_Length = MathUtils.Length(curve2.m_Bezier);
				MoveConnectedNodes(val, val2, curve, curve2, data.m_NoEdgeConnection);
				m_CurveData[val] = curve;
				m_CurveData[val2] = curve2;
				temp2.m_Flags |= temp3.m_Flags & TempFlags.Essential;
				temp3.m_Flags |= temp2.m_Flags & TempFlags.Essential;
				m_TempData[val] = temp2;
				m_TempData[val2] = temp3;
			}
		}

		private bool TryJoinCurve(Bezier4x3 curve1, Bezier4x3 curve2, bool checkHeight, out Bezier4x3 curve)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			curve = MathUtils.Join(curve1, curve2);
			float4 val = default(float4);
			float num = default(float);
			if (checkHeight)
			{
				val.x = MathUtils.Distance(curve, MathUtils.Position(curve1, 0.5f), ref num);
				val.y = MathUtils.Distance(curve, curve1.d, ref num);
				val.z = MathUtils.Distance(curve, curve2.a, ref num);
				val.w = MathUtils.Distance(curve, MathUtils.Position(curve2, 0.5f), ref num);
			}
			else
			{
				Bezier4x2 xz = ((Bezier4x3)(ref curve)).xz;
				float3 val2 = MathUtils.Position(curve1, 0.5f);
				val.x = MathUtils.Distance(xz, ((float3)(ref val2)).xz, ref num);
				val.y = MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref curve1.d)).xz, ref num);
				val.z = MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref curve2.a)).xz, ref num);
				Bezier4x2 xz2 = ((Bezier4x3)(ref curve)).xz;
				val2 = MathUtils.Position(curve2, 0.5f);
				val.w = MathUtils.Distance(xz2, ((float3)(ref val2)).xz, ref num);
				float num2 = FindHeightOffset(curve1, curve2, MathUtils.Position(curve, 1f / 3f));
				float num3 = FindHeightOffset(curve1, curve2, MathUtils.Position(curve, 2f / 3f));
				curve.b.y += num2 * 3f - num3 * 1.5f;
				curve.c.y += num3 * 3f - num2 * 1.5f;
			}
			return math.all(val < 0.1f);
		}

		private void FixStartSlope(Bezier4x3 originalCurve, ref Bezier4x3 newCurve)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			newCurve.b.y = originalCurve.b.y;
		}

		private void FixEndSlope(Bezier4x3 originalCurve, ref Bezier4x3 newCurve)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			newCurve.c.y = originalCurve.c.y;
		}

		private float FindHeightOffset(Bezier4x3 curve1, Bezier4x3 curve2, float3 position)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			float num2 = default(float);
			float num = MathUtils.Distance(((Bezier4x3)(ref curve1)).xz, ((float3)(ref position)).xz, ref num2);
			float num4 = default(float);
			float num3 = MathUtils.Distance(((Bezier4x3)(ref curve2)).xz, ((float3)(ref position)).xz, ref num4);
			if (num < num3)
			{
				return MathUtils.Position(curve1, num2).y - position.y;
			}
			return MathUtils.Position(curve2, num4).y - position.y;
		}

		private void ReplaceEdgeConnection(ref Entity node, Entity edge, Entity newNode)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (m_TempData.HasComponent(node))
			{
				CollectionUtils.RemoveValue<ConnectedEdge>(m_ConnectedEdges[node], new ConnectedEdge(edge));
			}
			node = newNode;
			if (m_TempData.HasComponent(node))
			{
				CollectionUtils.TryAddUniqueValue<ConnectedEdge>(m_ConnectedEdges[node], new ConnectedEdge(edge));
			}
		}

		private void ReplaceEdgeData(Entity source, Entity target, bool sourceStart, bool targetStart)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			if (m_RoadData.HasComponent(source) && m_RoadData.HasComponent(target))
			{
				Road road = m_RoadData[source];
				Road road2 = m_RoadData[target];
				if (((uint)road.m_Flags & (uint)(sourceStart ? 1 : 2)) != 0)
				{
					road2.m_Flags |= (Game.Net.RoadFlags)(targetStart ? 1 : 2);
				}
				else
				{
					road2.m_Flags &= (Game.Net.RoadFlags)(byte)(~(targetStart ? 1 : 2));
				}
				m_RoadData[target] = road2;
			}
			if (m_BuildOrderData.HasComponent(source) && m_BuildOrderData.HasComponent(target))
			{
				BuildOrder buildOrder = m_BuildOrderData[source];
				BuildOrder buildOrder2 = m_BuildOrderData[target];
				if (targetStart)
				{
					buildOrder2.m_Start = (sourceStart ? buildOrder.m_Start : buildOrder.m_End);
				}
				else
				{
					buildOrder2.m_End = (sourceStart ? buildOrder.m_Start : buildOrder.m_End);
				}
				m_BuildOrderData[target] = buildOrder2;
			}
		}

		private void MoveConnectedNodes(Entity source, Entity target, Curve curve, bool noEdgeConnection)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[source];
			DynamicBuffer<ConnectedNode> val2 = m_ConnectedNodes[target];
			for (int i = 0; i < val2.Length; i++)
			{
				ConnectedNode connectedNode = val2[i];
				Node node = m_NodeData[connectedNode.m_Node];
				GetDistance(curve, node, noEdgeConnection, out connectedNode.m_CurvePosition);
				val2[i] = connectedNode;
			}
			for (int j = 0; j < val.Length; j++)
			{
				ConnectedNode connectedNode2 = val[j];
				if (CollectionUtils.ContainsValue<ConnectedNode>(val2, connectedNode2))
				{
					RemoveConnectedEdge(connectedNode2.m_Node, source);
					continue;
				}
				Node node2 = m_NodeData[connectedNode2.m_Node];
				GetDistance(curve, node2, noEdgeConnection, out connectedNode2.m_CurvePosition);
				val2.Add(connectedNode2);
				SwitchConnectedEdge(connectedNode2.m_Node, source, target);
			}
			val.Clear();
		}

		private void MoveConnectedNodes(Entity edge1, Entity edge2, Curve curve1, Curve curve2, bool noEdgeConnection)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[edge1];
			DynamicBuffer<ConnectedNode> val2 = m_ConnectedNodes[edge2];
			int num = val.Length;
			int num2 = val2.Length;
			LocalConnectData localConnectData = default(LocalConnectData);
			for (int i = 0; i < num; i++)
			{
				ConnectedNode connectedNode = val[i];
				Node node = m_NodeData[connectedNode.m_Node];
				PrefabRef prefabRef = m_PrefabRefData[connectedNode.m_Node];
				if (m_LocalConnectData.TryGetComponent(prefabRef.m_Prefab, ref localConnectData) && (localConnectData.m_Flags & LocalConnectFlags.ChooseSides) != 0)
				{
					PrefabRef prefabRef2 = m_PrefabRefData[edge1];
					float num3 = m_PrefabGeometryData[prefabRef.m_Prefab].m_DefaultWidth * 0.5f + 0.1f;
					float num4 = m_PrefabGeometryData[prefabRef2.m_Prefab].m_DefaultWidth * 0.5f;
					float clampedDistance = GetClampedDistance(curve1, node, out var curvePosition);
					float clampedDistance2 = GetClampedDistance(curve2, node, out var curvePosition2);
					if (clampedDistance <= clampedDistance2)
					{
						connectedNode.m_CurvePosition = curvePosition;
						val[i] = connectedNode;
						clampedDistance2 -= math.sqrt(num4 * num4 + num3 * num3) - num4;
						if (clampedDistance2 <= clampedDistance && !CollectionUtils.ContainsValue<ConnectedNode>(val2, connectedNode))
						{
							connectedNode.m_CurvePosition = curvePosition2;
							val2.Add(connectedNode);
							AddConnectedEdge(connectedNode.m_Node, edge2);
						}
						continue;
					}
					clampedDistance -= math.sqrt(num4 * num4 + num3 * num3) - num4;
					if (clampedDistance <= clampedDistance2)
					{
						connectedNode.m_CurvePosition = curvePosition;
						val[i] = connectedNode;
						continue;
					}
					val.RemoveAt(i--);
					num--;
					if (CollectionUtils.ContainsValue<ConnectedNode>(val2, connectedNode))
					{
						RemoveConnectedEdge(connectedNode.m_Node, edge1);
						continue;
					}
					connectedNode.m_CurvePosition = curvePosition2;
					val2.Add(connectedNode);
					SwitchConnectedEdge(connectedNode.m_Node, edge1, edge2);
				}
				else
				{
					float curvePosition3;
					float distance = GetDistance(curve1, node, noEdgeConnection, out curvePosition3);
					float curvePosition4;
					float distance2 = GetDistance(curve2, node, noEdgeConnection, out curvePosition4);
					if (distance <= distance2 || CollectionUtils.ContainsValue<ConnectedNode>(val2, connectedNode))
					{
						connectedNode.m_CurvePosition = curvePosition3;
						val[i] = connectedNode;
						continue;
					}
					connectedNode.m_CurvePosition = curvePosition4;
					val2.Add(connectedNode);
					val.RemoveAt(i--);
					SwitchConnectedEdge(connectedNode.m_Node, edge1, edge2);
					num--;
				}
			}
			LocalConnectData localConnectData2 = default(LocalConnectData);
			for (int j = 0; j < num2; j++)
			{
				ConnectedNode connectedNode2 = val2[j];
				Node node2 = m_NodeData[connectedNode2.m_Node];
				PrefabRef prefabRef3 = m_PrefabRefData[connectedNode2.m_Node];
				if (m_LocalConnectData.TryGetComponent(prefabRef3.m_Prefab, ref localConnectData2) && (localConnectData2.m_Flags & LocalConnectFlags.ChooseSides) != 0)
				{
					PrefabRef prefabRef4 = m_PrefabRefData[edge1];
					float num5 = m_PrefabGeometryData[prefabRef3.m_Prefab].m_DefaultWidth * 0.5f + 0.1f;
					float num6 = m_PrefabGeometryData[prefabRef4.m_Prefab].m_DefaultWidth * 0.5f;
					float clampedDistance3 = GetClampedDistance(curve1, node2, out var curvePosition5);
					float clampedDistance4 = GetClampedDistance(curve2, node2, out var curvePosition6);
					if (clampedDistance4 <= clampedDistance3)
					{
						connectedNode2.m_CurvePosition = curvePosition6;
						val2[j] = connectedNode2;
						clampedDistance3 -= math.sqrt(num6 * num6 + num5 * num5) - num6;
						if (clampedDistance3 <= clampedDistance4 && !CollectionUtils.ContainsValue<ConnectedNode>(val, connectedNode2))
						{
							connectedNode2.m_CurvePosition = curvePosition5;
							val.Add(connectedNode2);
							AddConnectedEdge(connectedNode2.m_Node, edge1);
						}
						continue;
					}
					clampedDistance4 -= math.sqrt(num6 * num6 + num5 * num5) - num6;
					if (clampedDistance4 <= clampedDistance3)
					{
						connectedNode2.m_CurvePosition = curvePosition6;
						val2[j] = connectedNode2;
						continue;
					}
					val2.RemoveAt(j--);
					num2--;
					if (CollectionUtils.ContainsValue<ConnectedNode>(val, connectedNode2))
					{
						RemoveConnectedEdge(connectedNode2.m_Node, edge2);
						continue;
					}
					connectedNode2.m_CurvePosition = curvePosition5;
					val.Add(connectedNode2);
					SwitchConnectedEdge(connectedNode2.m_Node, edge2, edge1);
				}
				else
				{
					float curvePosition7;
					float distance3 = GetDistance(curve1, node2, noEdgeConnection, out curvePosition7);
					if (GetDistance(curve2, node2, noEdgeConnection, out var curvePosition8) <= distance3 || CollectionUtils.ContainsValue<ConnectedNode>(val, connectedNode2))
					{
						connectedNode2.m_CurvePosition = curvePosition8;
						val2[j] = connectedNode2;
						continue;
					}
					connectedNode2.m_CurvePosition = curvePosition7;
					val.Add(connectedNode2);
					val2.RemoveAt(j--);
					SwitchConnectedEdge(connectedNode2.m_Node, edge2, edge1);
					num2--;
				}
			}
		}

		private void RemoveConnectedEdge(Entity node, Entity source)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TempData.HasComponent(node))
			{
				return;
			}
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i].m_Edge == source)
				{
					val.RemoveAt(i);
					break;
				}
			}
		}

		private void AddConnectedEdge(Entity node, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (m_TempData.HasComponent(node))
			{
				m_ConnectedEdges[node].Add(new ConnectedEdge(target));
			}
		}

		private void SwitchConnectedEdge(Entity node, Entity source, Entity target)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (!m_TempData.HasComponent(node))
			{
				return;
			}
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				ConnectedEdge connectedEdge = val[i];
				if (connectedEdge.m_Edge == source)
				{
					connectedEdge.m_Edge = target;
					val[i] = connectedEdge;
				}
			}
		}

		private float GetClampedDistance(Curve curve, Node node, out float curvePosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref node.m_Position)).xz, ref curvePosition);
			float num2;
			if (curve.m_Length >= 0.2f)
			{
				float num = 0.1f / curve.m_Length;
				num2 = math.clamp(curvePosition, num, 1f - num);
			}
			else
			{
				num2 = 0.5f;
			}
			return math.distance(MathUtils.Position(curve.m_Bezier, num2), node.m_Position);
		}

		private float GetDistance(Curve curve, Node node, bool noEdgeConnection, out float curvePosition)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (noEdgeConnection)
			{
				float num = math.distance(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref node.m_Position)).xz);
				float num2 = math.distance(((float3)(ref curve.m_Bezier.d)).xz, ((float3)(ref node.m_Position)).xz);
				curvePosition = math.select(0f, 1f, num2 < num);
				return math.select(num, num2, num2 < num);
			}
			return MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref node.m_Position)).xz, ref curvePosition);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OwnerDefinition> __Game_Tools_OwnerDefinition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		public ComponentLookup<Node> __Game_Net_Node_RW_ComponentLookup;

		public ComponentLookup<Edge> __Game_Net_Edge_RW_ComponentLookup;

		public ComponentLookup<Curve> __Game_Net_Curve_RW_ComponentLookup;

		public ComponentLookup<Temp> __Game_Tools_Temp_RW_ComponentLookup;

		public ComponentLookup<BuildOrder> __Game_Net_BuildOrder_RW_ComponentLookup;

		public ComponentLookup<Road> __Game_Net_Road_RW_ComponentLookup;

		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RW_BufferLookup;

		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_OwnerDefinition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OwnerDefinition>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Net_Node_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(false);
			__Game_Net_Edge_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(false);
			__Game_Net_Curve_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(false);
			__Game_Tools_Temp_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(false);
			__Game_Net_BuildOrder_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildOrder>(false);
			__Game_Net_Road_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(false);
			__Game_Net_ConnectedEdge_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(false);
			__Game_Net_ConnectedNode_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(false);
		}
	}

	private ToolSystem m_ToolSystem;

	private EntityQuery m_TempNodeQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_TempNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Fixed>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_TempNodeQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<ReductionData> reductionQueue = default(NativeQueue<ReductionData>);
		reductionQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		FindCandidatesJob findCandidatesJob = new FindCandidatesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerDefinitionData = InternalCompilerInterface.GetComponentLookup<OwnerDefinition>(ref __TypeHandle.__Game_Tools_OwnerDefinition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_ReductionQueue = reductionQueue.AsParallelWriter()
		};
		NodeReductionJob obj = new NodeReductionJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ReductionQueue = reductionQueue
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<FindCandidatesJob>(findCandidatesJob, m_TempNodeQuery, ((SystemBase)this).Dependency);
		JobHandle val2 = IJobExtensions.Schedule<NodeReductionJob>(obj, val);
		reductionQueue.Dispose(val2);
		((SystemBase)this).Dependency = val2;
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
	public NodeReductionSystem()
	{
	}
}
