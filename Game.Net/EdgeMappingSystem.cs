using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Pathfind;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class EdgeMappingSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateMappingJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Lane> m_LaneType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeLane> m_EdgeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<NodeLane> m_NodeLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		public ComponentTypeHandle<EdgeMapping> m_EdgeMappingType;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<ConnectedNode> m_ConnectedNodes;

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
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Lane> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Lane>(ref m_LaneType);
			NativeArray<Curve> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<EdgeMapping> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeMapping>(ref m_EdgeMappingType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<EdgeLane>(ref m_EdgeLaneType);
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<NodeLane>(ref m_NodeLaneType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Lane lane = nativeArray[i];
				Curve laneCurve = nativeArray2[i];
				EdgeMapping edgeMapping = default(EdgeMapping);
				if (flag)
				{
					edgeMapping.m_Parent1 = nativeArray3[i].m_Owner;
					edgeMapping.m_CurveDelta1 = GetCurveDelta(laneCurve.m_Bezier, edgeMapping.m_Parent1);
				}
				else if (flag2)
				{
					Owner owner = nativeArray3[i];
					if (m_ConnectedEdges.HasBuffer(owner.m_Owner))
					{
						edgeMapping = GetNodeEdgeMapping(lane, laneCurve, owner.m_Owner);
					}
					else if (m_ConnectedNodes.HasBuffer(owner.m_Owner))
					{
						edgeMapping = GetEdgeNodeMapping(lane, owner.m_Owner);
					}
				}
				nativeArray4[i] = edgeMapping;
			}
		}

		private EdgeMapping GetNodeEdgeMapping(Lane lane, Curve laneCurve, Entity node)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			EdgeMapping result = default(EdgeMapping);
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
			bool2 val = bool2.op_Implicit(false);
			bool2 val2 = bool2.op_Implicit(false);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				PathNode other = new PathNode(value.m_Edge, 0);
				if (lane.m_StartNode.OwnerEquals(other))
				{
					result.m_Parent1 = value.m_Edge;
					val.x = value.m_End;
					val2.x = false;
				}
				if (lane.m_EndNode.OwnerEquals(other))
				{
					result.m_Parent2 = value.m_Edge;
					val.y = value.m_End;
					val2.y = false;
				}
				DynamicBuffer<ConnectedNode> val3 = m_ConnectedNodes[value.m_Edge];
				int num = 0;
				ConnectedNode connectedNode;
				while (num < val3.Length)
				{
					connectedNode = val3[num];
					PathNode other2 = new PathNode(connectedNode.m_Node, 0);
					if (lane.m_StartNode.OwnerEquals(other2))
					{
						goto IL_0107;
					}
					if (!lane.m_EndNode.OwnerEquals(other2))
					{
						EdgeIterator edgeIterator2 = new EdgeIterator(Entity.Null, connectedNode.m_Node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
						EdgeIteratorValue value2;
						while (edgeIterator2.GetNext(out value2))
						{
							if (value2.m_Edge == value.m_Edge)
							{
								continue;
							}
							other = new PathNode(value2.m_Edge, 0);
							if (lane.m_StartNode.OwnerEquals(other))
							{
								goto IL_01f7;
							}
							if (!lane.m_EndNode.OwnerEquals(other))
							{
								continue;
							}
							goto IL_0240;
						}
						num++;
						continue;
					}
					goto IL_0150;
				}
				continue;
				IL_0150:
				result.m_Parent1 = value.m_Edge;
				result.m_Parent2 = connectedNode.m_Node;
				((bool2)(ref val))._002Ector(value.m_End, true);
				((bool2)(ref val2))._002Ector(false, true);
				break;
				IL_0107:
				result.m_Parent1 = connectedNode.m_Node;
				result.m_Parent2 = value.m_Edge;
				((bool2)(ref val))._002Ector(false, value.m_End);
				((bool2)(ref val2))._002Ector(true, false);
				break;
				IL_0240:
				result.m_Parent1 = value.m_Edge;
				result.m_Parent2 = connectedNode.m_Node;
				((bool2)(ref val))._002Ector(value.m_End, true);
				((bool2)(ref val2))._002Ector(false, true);
				break;
				IL_01f7:
				result.m_Parent1 = connectedNode.m_Node;
				result.m_Parent2 = value.m_Edge;
				((bool2)(ref val))._002Ector(false, value.m_End);
				((bool2)(ref val2))._002Ector(true, false);
				break;
			}
			if (result.m_Parent1 != Entity.Null && result.m_Parent2 != Entity.Null)
			{
				if (((bool2)(ref val2)).Equals(new bool2(false, true)))
				{
					CommonUtils.Swap(ref result.m_Parent1, ref result.m_Parent2);
					val = ((bool2)(ref val)).yx;
					val2 = ((bool2)(ref val2)).yx;
				}
				Bezier4x3 laneCurve2 = default(Bezier4x3);
				Bezier4x3 laneCurve3 = default(Bezier4x3);
				MathUtils.Divide(laneCurve.m_Bezier, ref laneCurve2, ref laneCurve3, 0.5f);
				if (!val2.x)
				{
					result.m_CurveDelta1 = GetCurveDelta(laneCurve2, result.m_Parent1);
				}
				if (!val2.y)
				{
					result.m_CurveDelta2 = GetCurveDelta(laneCurve3, result.m_Parent2);
				}
			}
			else if (result.m_Parent1 != Entity.Null && !val2.x)
			{
				result.m_CurveDelta1 = GetCurveDelta(laneCurve.m_Bezier, result.m_Parent1);
			}
			else if (result.m_Parent2 != Entity.Null && !val2.y)
			{
				result.m_CurveDelta2 = GetCurveDelta(laneCurve.m_Bezier, result.m_Parent2);
			}
			if (val2.x)
			{
				if (val.x)
				{
					result.m_CurveDelta1 = new float2(1f, 0f);
				}
				else
				{
					result.m_CurveDelta1 = new float2(0f, 1f);
				}
			}
			else if (val.x)
			{
				result.m_CurveDelta1.y = math.cmax(result.m_CurveDelta1);
			}
			else
			{
				result.m_CurveDelta1.y = math.cmin(result.m_CurveDelta1);
			}
			if (val2.y)
			{
				if (val.y)
				{
					result.m_CurveDelta2 = new float2(1f, 0f);
				}
				else
				{
					result.m_CurveDelta2 = new float2(0f, 1f);
				}
			}
			else if (val.y)
			{
				result.m_CurveDelta2.x = math.cmax(result.m_CurveDelta2);
			}
			else
			{
				result.m_CurveDelta2.x = math.cmin(result.m_CurveDelta2);
			}
			if (result.m_Parent1 == Entity.Null)
			{
				result.m_Parent1 = result.m_Parent2;
				result.m_CurveDelta1 = result.m_CurveDelta2;
				result.m_Parent2 = Entity.Null;
				result.m_CurveDelta2 = default(float2);
			}
			return result;
		}

		private EdgeMapping GetEdgeNodeMapping(Lane lane, Entity edge)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			EdgeMapping result = default(EdgeMapping);
			DynamicBuffer<ConnectedNode> val = m_ConnectedNodes[edge];
			float2 val2 = default(float2);
			for (int i = 0; i < val.Length; i++)
			{
				ConnectedNode connectedNode = val[i];
				PathNode other = new PathNode(connectedNode.m_Node, 0);
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, connectedNode.m_Node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
				if (lane.m_StartNode.OwnerEquals(other))
				{
					result.m_Parent1 = connectedNode.m_Node;
					val2.x = connectedNode.m_CurvePosition;
					break;
				}
				if (lane.m_EndNode.OwnerEquals(other))
				{
					result.m_Parent2 = connectedNode.m_Node;
					val2.y = connectedNode.m_CurvePosition;
					break;
				}
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PathNode other2 = new PathNode(value.m_Edge, 0);
					if (lane.m_StartNode.OwnerEquals(other2))
					{
						goto IL_00f5;
					}
					if (!lane.m_EndNode.OwnerEquals(other2))
					{
						continue;
					}
					goto IL_0123;
				}
				continue;
				IL_0123:
				result.m_Parent2 = connectedNode.m_Node;
				val2.y = connectedNode.m_CurvePosition;
				break;
				IL_00f5:
				result.m_Parent1 = connectedNode.m_Node;
				val2.x = connectedNode.m_CurvePosition;
				break;
			}
			if (result.m_Parent1 != Entity.Null)
			{
				result.m_Parent2 = edge;
				result.m_CurveDelta1 = new float2(0f, 1f);
				result.m_CurveDelta2 = float2.op_Implicit(val2.x);
			}
			else if (result.m_Parent2 != Entity.Null)
			{
				result.m_Parent1 = result.m_Parent2;
				result.m_Parent2 = edge;
				result.m_CurveDelta1 = new float2(1f, 0f);
				result.m_CurveDelta2 = float2.op_Implicit(val2.y);
			}
			return result;
		}

		private float2 GetCurveDelta(Bezier4x3 laneCurve, Entity edge)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			float2 result = default(float2);
			Curve curve = default(Curve);
			if (m_CurveData.TryGetComponent(edge, ref curve))
			{
				MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref laneCurve.a)).xz, ref result.x);
				MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref laneCurve.d)).xz, ref result.y);
			}
			return result;
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
		public ComponentTypeHandle<NodeLane> __Game_Net_NodeLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		public ComponentTypeHandle<EdgeMapping> __Game_Net_EdgeMapping_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedNode> __Game_Net_ConnectedNode_RO_BufferLookup;

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
			__Game_Net_Lane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Lane>(true);
			__Game_Net_EdgeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeLane>(true);
			__Game_Net_NodeLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeLane>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_EdgeMapping_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeMapping>(false);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_ConnectedNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedNode>(true);
		}
	}

	private EntityQuery m_UpdatedLanesQuery;

	private EntityQuery m_AllLanesQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdatedLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<EdgeMapping>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_AllLanesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EdgeMapping>() });
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllLanesQuery : m_UpdatedLanesQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			UpdateMappingJob updateMappingJob = new UpdateMappingJob
			{
				m_LaneType = InternalCompilerInterface.GetComponentTypeHandle<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeLaneType = InternalCompilerInterface.GetComponentTypeHandle<EdgeLane>(ref __TypeHandle.__Game_Net_EdgeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeLaneType = InternalCompilerInterface.GetComponentTypeHandle<NodeLane>(ref __TypeHandle.__Game_Net_NodeLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeMappingType = InternalCompilerInterface.GetComponentTypeHandle<EdgeMapping>(ref __TypeHandle.__Game_Net_EdgeMapping_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedNodes = InternalCompilerInterface.GetBufferLookup<ConnectedNode>(ref __TypeHandle.__Game_Net_ConnectedNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<UpdateMappingJob>(updateMappingJob, val, ((SystemBase)this).Dependency);
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
	public EdgeMappingSystem()
	{
	}
}
