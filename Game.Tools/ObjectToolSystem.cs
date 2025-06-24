using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.PSI;
using Game.Simulation;
using Game.Zones;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ObjectToolSystem : ObjectToolBaseSystem
{
	public enum Mode
	{
		Create,
		Upgrade,
		Move,
		Brush,
		Stamp,
		Line,
		Curve
	}

	public enum State
	{
		Default,
		Rotating,
		Adding,
		Removing
	}

	private struct Rotation
	{
		public quaternion m_Rotation;

		public quaternion m_ParentRotation;

		public bool m_IsAligned;

		public bool m_IsSnapped;
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		private struct LoweredParentIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public ControlPoint m_Result;

			public float3 m_Position;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Game.Net.Node> m_NodeData;

			public ComponentLookup<Orphan> m_OrphanData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((float3)(ref m_Position)).xz))
				{
					if (m_EdgeGeometryData.HasComponent(entity))
					{
						CheckEdge(entity);
					}
					else if (m_OrphanData.HasComponent(entity))
					{
						CheckNode(entity);
					}
				}
			}

			private void CheckNode(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				Game.Net.Node node = m_NodeData[entity];
				Orphan orphan = m_OrphanData[entity];
				NetCompositionData netCompositionData = m_PrefabCompositionData[orphan.m_Composition];
				if ((netCompositionData.m_State & CompositionState.Marker) == 0 && ((netCompositionData.m_Flags.m_Left | netCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
				{
					float3 position = node.m_Position;
					position.y += netCompositionData.m_SurfaceHeight.max;
					if (math.distance(((float3)(ref m_Position)).xz, ((float3)(ref position)).xz) <= netCompositionData.m_Width * 0.5f)
					{
						m_Result.m_OriginalEntity = entity;
						m_Result.m_Position = node.m_Position;
						m_Result.m_HitPosition = m_Position;
						m_Result.m_HitPosition.y = position.y;
						m_Result.m_HitDirection = default(float3);
					}
				}
			}

			private void CheckEdge(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0292: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0208: Unknown result type (might be due to invalid IL or missing references)
				//IL_0212: Unknown result type (might be due to invalid IL or missing references)
				//IL_0217: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_0229: Unknown result type (might be due to invalid IL or missing references)
				//IL_022e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0240: Unknown result type (might be due to invalid IL or missing references)
				//IL_0245: Unknown result type (might be due to invalid IL or missing references)
				//IL_024d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0256: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_026e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0426: Unknown result type (might be due to invalid IL or missing references)
				//IL_042f: Unknown result type (might be due to invalid IL or missing references)
				//IL_043e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0447: Unknown result type (might be due to invalid IL or missing references)
				//IL_0456: Unknown result type (might be due to invalid IL or missing references)
				//IL_045b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0463: Unknown result type (might be due to invalid IL or missing references)
				//IL_0468: Unknown result type (might be due to invalid IL or missing references)
				//IL_0470: Unknown result type (might be due to invalid IL or missing references)
				//IL_0479: Unknown result type (might be due to invalid IL or missing references)
				//IL_0488: Unknown result type (might be due to invalid IL or missing references)
				//IL_0491: Unknown result type (might be due to invalid IL or missing references)
				//IL_036d: Unknown result type (might be due to invalid IL or missing references)
				//IL_037a: Unknown result type (might be due to invalid IL or missing references)
				//IL_039e: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0406: Unknown result type (might be due to invalid IL or missing references)
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[entity];
				EdgeNodeGeometry geometry = m_StartNodeGeometryData[entity].m_Geometry;
				EdgeNodeGeometry geometry2 = m_EndNodeGeometryData[entity].m_Geometry;
				bool3 val = default(bool3);
				val.x = MathUtils.Intersect(((Bounds3)(ref edgeGeometry.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
				val.y = MathUtils.Intersect(((Bounds3)(ref geometry.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
				val.z = MathUtils.Intersect(((Bounds3)(ref geometry2.m_Bounds)).xz, ((float3)(ref m_Position)).xz);
				if (!math.any(val))
				{
					return;
				}
				Composition composition = m_CompositionData[entity];
				Edge edge = m_EdgeData[entity];
				Curve curve = m_CurveData[entity];
				if (val.x)
				{
					NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
					if ((prefabCompositionData.m_State & CompositionState.Marker) == 0 && ((prefabCompositionData.m_Flags.m_Left | prefabCompositionData.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
					{
						CheckSegment(entity, edgeGeometry.m_Start, curve.m_Bezier, prefabCompositionData);
						CheckSegment(entity, edgeGeometry.m_End, curve.m_Bezier, prefabCompositionData);
					}
				}
				if (val.y)
				{
					NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
					if ((prefabCompositionData2.m_State & CompositionState.Marker) == 0 && ((prefabCompositionData2.m_Flags.m_Left | prefabCompositionData2.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
					{
						if (geometry.m_MiddleRadius > 0f)
						{
							CheckSegment(edge.m_Start, geometry.m_Left, curve.m_Bezier, prefabCompositionData2);
							Segment right = geometry.m_Right;
							Segment right2 = geometry.m_Right;
							right.m_Right = MathUtils.Lerp(geometry.m_Right.m_Left, geometry.m_Right.m_Right, 0.5f);
							right2.m_Left = MathUtils.Lerp(geometry.m_Right.m_Left, geometry.m_Right.m_Right, 0.5f);
							right.m_Right.d = geometry.m_Middle.d;
							right2.m_Left.d = geometry.m_Middle.d;
							CheckSegment(edge.m_Start, right, curve.m_Bezier, prefabCompositionData2);
							CheckSegment(edge.m_Start, right2, curve.m_Bezier, prefabCompositionData2);
						}
						else
						{
							Segment left = geometry.m_Left;
							Segment right3 = geometry.m_Right;
							CheckSegment(edge.m_Start, left, curve.m_Bezier, prefabCompositionData2);
							CheckSegment(edge.m_Start, right3, curve.m_Bezier, prefabCompositionData2);
							left.m_Right = geometry.m_Middle;
							right3.m_Left = geometry.m_Middle;
							CheckSegment(edge.m_Start, left, curve.m_Bezier, prefabCompositionData2);
							CheckSegment(edge.m_Start, right3, curve.m_Bezier, prefabCompositionData2);
						}
					}
				}
				if (!val.z)
				{
					return;
				}
				NetCompositionData prefabCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
				if ((prefabCompositionData3.m_State & CompositionState.Marker) == 0 && ((prefabCompositionData3.m_Flags.m_Left | prefabCompositionData3.m_Flags.m_Right) & CompositionFlags.Side.Lowered) != 0)
				{
					if (geometry2.m_MiddleRadius > 0f)
					{
						CheckSegment(edge.m_End, geometry2.m_Left, curve.m_Bezier, prefabCompositionData3);
						Segment right4 = geometry2.m_Right;
						Segment right5 = geometry2.m_Right;
						right4.m_Right = MathUtils.Lerp(geometry2.m_Right.m_Left, geometry2.m_Right.m_Right, 0.5f);
						right4.m_Right.d = geometry2.m_Middle.d;
						right5.m_Left = right4.m_Right;
						CheckSegment(edge.m_End, right4, curve.m_Bezier, prefabCompositionData3);
						CheckSegment(edge.m_End, right5, curve.m_Bezier, prefabCompositionData3);
					}
					else
					{
						Segment left2 = geometry2.m_Left;
						Segment right6 = geometry2.m_Right;
						CheckSegment(edge.m_End, left2, curve.m_Bezier, prefabCompositionData3);
						CheckSegment(edge.m_End, right6, curve.m_Bezier, prefabCompositionData3);
						left2.m_Right = geometry2.m_Middle;
						right6.m_Left = geometry2.m_Middle;
						CheckSegment(edge.m_End, left2, curve.m_Bezier, prefabCompositionData3);
						CheckSegment(edge.m_End, right6, curve.m_Bezier, prefabCompositionData3);
					}
				}
			}

			private void CheckSegment(Entity entity, Segment segment, Bezier4x3 curve, NetCompositionData prefabCompositionData)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_018d: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				float3 val = segment.m_Left.a;
				float3 val2 = segment.m_Right.a;
				Triangle3 val5 = default(Triangle3);
				Triangle3 val6 = default(Triangle3);
				float2 val7 = default(float2);
				float num2 = default(float);
				float num3 = default(float);
				for (int i = 1; i <= 8; i++)
				{
					float num = (float)i / 8f;
					float3 val3 = MathUtils.Position(segment.m_Left, num);
					float3 val4 = MathUtils.Position(segment.m_Right, num);
					((Triangle3)(ref val5))._002Ector(val, val2, val3);
					((Triangle3)(ref val6))._002Ector(val4, val3, val2);
					if (MathUtils.Intersect(((Triangle3)(ref val5)).xz, ((float3)(ref m_Position)).xz, ref val7))
					{
						float3 hitPosition = m_Position;
						hitPosition.y = MathUtils.Position(((Triangle3)(ref val5)).y, val7) + prefabCompositionData.m_SurfaceHeight.max;
						MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref hitPosition)).xz, ref num2);
						m_Result.m_OriginalEntity = entity;
						m_Result.m_Position = MathUtils.Position(curve, num2);
						m_Result.m_HitPosition = hitPosition;
						m_Result.m_HitDirection = default(float3);
						m_Result.m_CurvePosition = num2;
					}
					else if (MathUtils.Intersect(((Triangle3)(ref val6)).xz, ((float3)(ref m_Position)).xz, ref val7))
					{
						float3 hitPosition2 = m_Position;
						hitPosition2.y = MathUtils.Position(((Triangle3)(ref val6)).y, val7) + prefabCompositionData.m_SurfaceHeight.max;
						MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref hitPosition2)).xz, ref num3);
						m_Result.m_OriginalEntity = entity;
						m_Result.m_Position = MathUtils.Position(curve, num3);
						m_Result.m_HitPosition = hitPosition2;
						m_Result.m_HitDirection = default(float3);
						m_Result.m_CurvePosition = num3;
					}
					val = val3;
					val2 = val4;
				}
			}
		}

		private struct OriginalObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_Parent;

			public Entity m_Result;

			public Bounds3 m_Bounds;

			public float m_BestDistance;

			public bool m_EditorMode;

			public TransportStopData m_TransportStopData1;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Attached> m_AttachedData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetObjectData> m_NetObjectData;

			public ComponentLookup<TransportStopData> m_TransportStopData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || !m_AttachedData.HasComponent(item) || (!m_EditorMode && m_OwnerData.HasComponent(item)) || m_AttachedData[item].m_Parent != m_Parent)
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[item];
				if (!m_NetObjectData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				TransportStopData transportStopData = default(TransportStopData);
				if (m_TransportStopData.HasComponent(prefabRef.m_Prefab))
				{
					transportStopData = m_TransportStopData[prefabRef.m_Prefab];
				}
				if (m_TransportStopData1.m_TransportType == transportStopData.m_TransportType)
				{
					float num = math.distance(MathUtils.Center(m_Bounds), MathUtils.Center(bounds.m_Bounds));
					if (num < m_BestDistance)
					{
						m_Result = item;
						m_BestDistance = num;
					}
				}
			}
		}

		private struct ParentObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public Bounds3 m_Bounds;

			public float m_BestOverlap;

			public bool m_IsBuilding;

			public ObjectGeometryData m_PrefabObjectGeometryData1;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<AssetStampData> m_AssetStampData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0402: Unknown result type (might be due to invalid IL or missing references)
				//IL_0408: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_041c: Unknown result type (might be due to invalid IL or missing references)
				//IL_025d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0262: Unknown result type (might be due to invalid IL or missing references)
				//IL_0264: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_027a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0284: Unknown result type (might be due to invalid IL or missing references)
				//IL_0289: Unknown result type (might be due to invalid IL or missing references)
				//IL_028e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0293: Unknown result type (might be due to invalid IL or missing references)
				//IL_0297: Unknown result type (might be due to invalid IL or missing references)
				//IL_029c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_042f: Unknown result type (might be due to invalid IL or missing references)
				//IL_043e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_035b: Unknown result type (might be due to invalid IL or missing references)
				//IL_035d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0364: Unknown result type (might be due to invalid IL or missing references)
				//IL_036b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0375: Unknown result type (might be due to invalid IL or missing references)
				//IL_037a: Unknown result type (might be due to invalid IL or missing references)
				//IL_037f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_0388: Unknown result type (might be due to invalid IL or missing references)
				//IL_038d: Unknown result type (might be due to invalid IL or missing references)
				//IL_038f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0391: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0466: Unknown result type (might be due to invalid IL or missing references)
				//IL_046b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0470: Unknown result type (might be due to invalid IL or missing references)
				//IL_0478: Unknown result type (might be due to invalid IL or missing references)
				//IL_047d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0484: Unknown result type (might be due to invalid IL or missing references)
				//IL_0489: Unknown result type (might be due to invalid IL or missing references)
				//IL_048e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0496: Unknown result type (might be due to invalid IL or missing references)
				//IL_049b: Unknown result type (might be due to invalid IL or missing references)
				//IL_049d: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04be: Unknown result type (might be due to invalid IL or missing references)
				//IL_045c: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03da: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0303: Unknown result type (might be due to invalid IL or missing references)
				//IL_0305: Unknown result type (might be due to invalid IL or missing references)
				//IL_030a: Unknown result type (might be due to invalid IL or missing references)
				//IL_031d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_032d: Unknown result type (might be due to invalid IL or missing references)
				//IL_033c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0344: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0206: Unknown result type (might be due to invalid IL or missing references)
				//IL_0208: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0220: Unknown result type (might be due to invalid IL or missing references)
				//IL_022b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0230: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0247: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0177: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_0196: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_06da: Unknown result type (might be due to invalid IL or missing references)
				//IL_06df: Unknown result type (might be due to invalid IL or missing references)
				//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0504: Unknown result type (might be due to invalid IL or missing references)
				//IL_0526: Unknown result type (might be due to invalid IL or missing references)
				//IL_052b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0532: Unknown result type (might be due to invalid IL or missing references)
				//IL_0537: Unknown result type (might be due to invalid IL or missing references)
				//IL_0544: Unknown result type (might be due to invalid IL or missing references)
				//IL_0549: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0801: Unknown result type (might be due to invalid IL or missing references)
				//IL_080b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0810: Unknown result type (might be due to invalid IL or missing references)
				//IL_0815: Unknown result type (might be due to invalid IL or missing references)
				//IL_081e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0823: Unknown result type (might be due to invalid IL or missing references)
				//IL_0828: Unknown result type (might be due to invalid IL or missing references)
				//IL_082a: Unknown result type (might be due to invalid IL or missing references)
				//IL_070b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0715: Unknown result type (might be due to invalid IL or missing references)
				//IL_072d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0732: Unknown result type (might be due to invalid IL or missing references)
				//IL_0737: Unknown result type (might be due to invalid IL or missing references)
				//IL_0745: Unknown result type (might be due to invalid IL or missing references)
				//IL_0755: Unknown result type (might be due to invalid IL or missing references)
				//IL_075a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0761: Unknown result type (might be due to invalid IL or missing references)
				//IL_0766: Unknown result type (might be due to invalid IL or missing references)
				//IL_076f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0774: Unknown result type (might be due to invalid IL or missing references)
				//IL_0779: Unknown result type (might be due to invalid IL or missing references)
				//IL_077b: Unknown result type (might be due to invalid IL or missing references)
				//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0608: Unknown result type (might be due to invalid IL or missing references)
				//IL_060d: Unknown result type (might be due to invalid IL or missing references)
				//IL_060f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0614: Unknown result type (might be due to invalid IL or missing references)
				//IL_061b: Unknown result type (might be due to invalid IL or missing references)
				//IL_061d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0627: Unknown result type (might be due to invalid IL or missing references)
				//IL_062c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0631: Unknown result type (might be due to invalid IL or missing references)
				//IL_063a: Unknown result type (might be due to invalid IL or missing references)
				//IL_063f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0644: Unknown result type (might be due to invalid IL or missing references)
				//IL_0646: Unknown result type (might be due to invalid IL or missing references)
				//IL_055e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0568: Unknown result type (might be due to invalid IL or missing references)
				//IL_0580: Unknown result type (might be due to invalid IL or missing references)
				//IL_0585: Unknown result type (might be due to invalid IL or missing references)
				//IL_058a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0598: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0837: Unknown result type (might be due to invalid IL or missing references)
				//IL_0839: Unknown result type (might be due to invalid IL or missing references)
				//IL_083e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0840: Unknown result type (might be due to invalid IL or missing references)
				//IL_0845: Unknown result type (might be due to invalid IL or missing references)
				//IL_084a: Unknown result type (might be due to invalid IL or missing references)
				//IL_084c: Unknown result type (might be due to invalid IL or missing references)
				//IL_084e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0853: Unknown result type (might be due to invalid IL or missing references)
				//IL_0855: Unknown result type (might be due to invalid IL or missing references)
				//IL_085a: Unknown result type (might be due to invalid IL or missing references)
				//IL_085f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0861: Unknown result type (might be due to invalid IL or missing references)
				//IL_0863: Unknown result type (might be due to invalid IL or missing references)
				//IL_0865: Unknown result type (might be due to invalid IL or missing references)
				//IL_086c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0871: Unknown result type (might be due to invalid IL or missing references)
				//IL_087c: Unknown result type (might be due to invalid IL or missing references)
				//IL_078b: Unknown result type (might be due to invalid IL or missing references)
				//IL_078d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0792: Unknown result type (might be due to invalid IL or missing references)
				//IL_0794: Unknown result type (might be due to invalid IL or missing references)
				//IL_0799: Unknown result type (might be due to invalid IL or missing references)
				//IL_079e: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0656: Unknown result type (might be due to invalid IL or missing references)
				//IL_0658: Unknown result type (might be due to invalid IL or missing references)
				//IL_065d: Unknown result type (might be due to invalid IL or missing references)
				//IL_065f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0664: Unknown result type (might be due to invalid IL or missing references)
				//IL_0669: Unknown result type (might be due to invalid IL or missing references)
				//IL_066b: Unknown result type (might be due to invalid IL or missing references)
				//IL_066d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0672: Unknown result type (might be due to invalid IL or missing references)
				//IL_0674: Unknown result type (might be due to invalid IL or missing references)
				//IL_0679: Unknown result type (might be due to invalid IL or missing references)
				//IL_067e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0680: Unknown result type (might be due to invalid IL or missing references)
				//IL_0682: Unknown result type (might be due to invalid IL or missing references)
				//IL_0684: Unknown result type (might be due to invalid IL or missing references)
				//IL_068b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0690: Unknown result type (might be due to invalid IL or missing references)
				//IL_069b: Unknown result type (might be due to invalid IL or missing references)
				//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[item];
				bool flag = m_BuildingData.HasComponent(prefabRef.m_Prefab);
				bool flag2 = m_AssetStampData.HasComponent(prefabRef.m_Prefab);
				if (m_IsBuilding && !flag2)
				{
					return;
				}
				float num = m_BestOverlap;
				if (flag || flag2)
				{
					Transform transform = m_TransformData[item];
					ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					float3 val = MathUtils.Center(bounds.m_Bounds);
					float3 val2;
					Quad3 val6;
					if ((m_PrefabObjectGeometryData1.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
					{
						float num2 = m_PrefabObjectGeometryData1.m_Size.x * 0.5f - 0.01f;
						val2 = m_ControlPoint.m_Position - val;
						Circle2 val3 = default(Circle2);
						((Circle2)(ref val3))._002Ector(num2, ((float3)(ref val2)).xz);
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
						{
							float num3 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
							val2 = transform.m_Position - val;
							Circle2 val4 = default(Circle2);
							((Circle2)(ref val4))._002Ector(num3, ((float3)(ref val2)).xz);
							if (MathUtils.Intersect(val3, val4))
							{
								float3 val5 = default(float3);
								((float3)(ref val5)).xz = ((float3)(ref val)).xz + MathUtils.Center(MathUtils.Bounds(val3) & MathUtils.Bounds(val4));
								val5.y = MathUtils.Center(((Bounds3)(ref bounds.m_Bounds)).y & ((Bounds3)(ref m_Bounds)).y);
								num = math.distance(val5, m_ControlPoint.m_Position);
							}
						}
						else
						{
							val6 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
							Bounds2 val7 = default(Bounds2);
							if (MathUtils.Intersect(((Quad3)(ref val6)).xz, val3, ref val7))
							{
								float3 val8 = default(float3);
								((float3)(ref val8)).xz = ((float3)(ref val)).xz + MathUtils.Center(val7);
								val8.y = MathUtils.Center(((Bounds3)(ref bounds.m_Bounds)).y & ((Bounds3)(ref m_Bounds)).y);
								num = math.distance(val8, m_ControlPoint.m_Position);
							}
						}
					}
					else
					{
						val6 = ObjectUtils.CalculateBaseCorners(m_ControlPoint.m_Position - val, m_ControlPoint.m_Rotation, MathUtils.Expand(m_PrefabObjectGeometryData1.m_Bounds, float3.op_Implicit(-0.01f)));
						Quad2 xz = ((Quad3)(ref val6)).xz;
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
						{
							float num4 = objectGeometryData.m_Size.x * 0.5f - 0.01f;
							val2 = transform.m_Position - val;
							Circle2 val9 = default(Circle2);
							((Circle2)(ref val9))._002Ector(num4, ((float3)(ref val2)).xz);
							Bounds2 val10 = default(Bounds2);
							if (MathUtils.Intersect(xz, val9, ref val10))
							{
								float3 val11 = default(float3);
								((float3)(ref val11)).xz = ((float3)(ref val)).xz + MathUtils.Center(val10);
								val11.y = MathUtils.Center(((Bounds3)(ref bounds.m_Bounds)).y & ((Bounds3)(ref m_Bounds)).y);
								num = math.distance(val11, m_ControlPoint.m_Position);
							}
						}
						else
						{
							val6 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val, transform.m_Rotation, MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.01f)));
							Quad2 xz2 = ((Quad3)(ref val6)).xz;
							Bounds2 val12 = default(Bounds2);
							if (MathUtils.Intersect(xz, xz2, ref val12))
							{
								float3 val13 = default(float3);
								((float3)(ref val13)).xz = ((float3)(ref val)).xz + MathUtils.Center(val12);
								val13.y = MathUtils.Center(((Bounds3)(ref bounds.m_Bounds)).y & ((Bounds3)(ref m_Bounds)).y);
								num = math.distance(val13, m_ControlPoint.m_Position);
							}
						}
					}
				}
				else
				{
					if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
					{
						return;
					}
					Transform transform2 = m_TransformData[item];
					ObjectGeometryData objectGeometryData2 = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
					if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Physical) == 0 && m_OwnerData.HasComponent(item))
					{
						return;
					}
					float3 val14 = MathUtils.Center(bounds.m_Bounds);
					quaternion val15 = math.inverse(m_ControlPoint.m_Rotation);
					quaternion val16 = math.inverse(transform2.m_Rotation);
					float3 val17 = math.mul(val15, m_ControlPoint.m_Position - val14);
					float3 val18 = math.mul(val16, transform2.m_Position - val14);
					if ((m_PrefabObjectGeometryData1.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
					{
						Cylinder3 val19 = new Cylinder3
						{
							circle = new Circle2(m_PrefabObjectGeometryData1.m_Size.x * 0.5f - 0.01f, ((float3)(ref val17)).xz),
							height = new Bounds1(0.01f, m_PrefabObjectGeometryData1.m_Size.y - 0.01f) + val17.y,
							rotation = m_ControlPoint.m_Rotation
						};
						if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
						{
							Cylinder3 cylinder = new Cylinder3
							{
								circle = new Circle2(objectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val18)).xz),
								height = new Bounds1(0.01f, objectGeometryData2.m_Size.y - 0.01f) + val18.y,
								rotation = transform2.m_Rotation
							};
							float3 pos = default(float3);
							if (Game.Objects.ValidationHelpers.Intersect(val19, cylinder, ref pos))
							{
								num = math.distance(pos, m_ControlPoint.m_Position);
							}
						}
						else
						{
							Box3 val20 = default(Box3);
							val20.bounds = objectGeometryData2.m_Bounds + val18;
							val20.bounds = MathUtils.Expand(val20.bounds, float3.op_Implicit(-0.01f));
							val20.rotation = transform2.m_Rotation;
							Bounds3 val21 = default(Bounds3);
							Bounds3 val22 = default(Bounds3);
							if (MathUtils.Intersect(val19, val20, ref val21, ref val22))
							{
								float3 val23 = math.mul(val19.rotation, MathUtils.Center(val21));
								float3 val24 = math.mul(val20.rotation, MathUtils.Center(val22));
								num = math.distance(val14 + math.lerp(val23, val24, 0.5f), m_ControlPoint.m_Position);
							}
						}
					}
					else
					{
						Box3 val25 = default(Box3);
						val25.bounds = m_PrefabObjectGeometryData1.m_Bounds + val17;
						val25.bounds = MathUtils.Expand(val25.bounds, float3.op_Implicit(-0.01f));
						val25.rotation = m_ControlPoint.m_Rotation;
						if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
						{
							Cylinder3 val26 = new Cylinder3
							{
								circle = new Circle2(objectGeometryData2.m_Size.x * 0.5f - 0.01f, ((float3)(ref val18)).xz),
								height = new Bounds1(0.01f, objectGeometryData2.m_Size.y - 0.01f) + val18.y,
								rotation = transform2.m_Rotation
							};
							Bounds3 val27 = default(Bounds3);
							Bounds3 val28 = default(Bounds3);
							if (MathUtils.Intersect(val26, val25, ref val27, ref val28))
							{
								float3 val29 = math.mul(val25.rotation, MathUtils.Center(val28));
								float3 val30 = math.mul(val26.rotation, MathUtils.Center(val27));
								num = math.distance(val14 + math.lerp(val29, val30, 0.5f), m_ControlPoint.m_Position);
							}
						}
						else
						{
							Box3 val31 = default(Box3);
							val31.bounds = objectGeometryData2.m_Bounds + val18;
							val31.bounds = MathUtils.Expand(val31.bounds, float3.op_Implicit(-0.01f));
							val31.rotation = transform2.m_Rotation;
							Bounds3 val32 = default(Bounds3);
							Bounds3 val33 = default(Bounds3);
							if (MathUtils.Intersect(val25, val31, ref val32, ref val33))
							{
								float3 val34 = math.mul(val25.rotation, MathUtils.Center(val32));
								float3 val35 = math.mul(val31.rotation, MathUtils.Center(val33));
								num = math.distance(val14 + math.lerp(val34, val35, 0.5f), m_ControlPoint.m_Position);
							}
						}
					}
				}
				if (num < m_BestOverlap)
				{
					m_BestSnapPosition = m_ControlPoint;
					m_BestSnapPosition.m_OriginalEntity = item;
					m_BestSnapPosition.m_ElementIndex = new int2(-1, -1);
					m_BestOverlap = num;
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public quaternion m_Rotation;

			public Bounds2 m_Bounds;

			public float3 m_LocalOffset;

			public float2 m_LocalTangent;

			public Entity m_IgnoreOwner;

			public float m_SnapFactor;

			public NetData m_NetData;

			public NetGeometryData m_NetGeometryData;

			public RoadData m_RoadData;

			public NativeList<SubSnapPoint> m_SubSnapPoints;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Game.Net.Node> m_NodeData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetData> m_PrefabNetData;

			public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity netEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_042e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0430: Unknown result type (might be due to invalid IL or missing references)
				//IL_0432: Unknown result type (might be due to invalid IL or missing references)
				//IL_0437: Unknown result type (might be due to invalid IL or missing references)
				//IL_0453: Unknown result type (might be due to invalid IL or missing references)
				//IL_045e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0465: Unknown result type (might be due to invalid IL or missing references)
				//IL_0470: Unknown result type (might be due to invalid IL or missing references)
				//IL_0477: Unknown result type (might be due to invalid IL or missing references)
				//IL_047c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0481: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0401: Unknown result type (might be due to invalid IL or missing references)
				//IL_040c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0588: Unknown result type (might be due to invalid IL or missing references)
				//IL_058a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0591: Unknown result type (might be due to invalid IL or missing references)
				//IL_0593: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_041a: Unknown result type (might be due to invalid IL or missing references)
				//IL_041f: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04da: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0503: Unknown result type (might be due to invalid IL or missing references)
				//IL_0507: Unknown result type (might be due to invalid IL or missing references)
				//IL_050d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0512: Unknown result type (might be due to invalid IL or missing references)
				//IL_0517: Unknown result type (might be due to invalid IL or missing references)
				//IL_051c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0538: Unknown result type (might be due to invalid IL or missing references)
				//IL_0543: Unknown result type (might be due to invalid IL or missing references)
				//IL_054a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0555: Unknown result type (might be due to invalid IL or missing references)
				//IL_055c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0561: Unknown result type (might be due to invalid IL or missing references)
				//IL_0566: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0205: Unknown result type (might be due to invalid IL or missing references)
				//IL_020a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0211: Unknown result type (might be due to invalid IL or missing references)
				//IL_0217: Unknown result type (might be due to invalid IL or missing references)
				//IL_0219: Unknown result type (might be due to invalid IL or missing references)
				//IL_021e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0228: Unknown result type (might be due to invalid IL or missing references)
				//IL_0239: Unknown result type (might be due to invalid IL or missing references)
				//IL_0281: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_032a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0394: Unknown result type (might be due to invalid IL or missing references)
				//IL_0396: Unknown result type (might be due to invalid IL or missing references)
				//IL_035a: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_035e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0369: Unknown result type (might be due to invalid IL or missing references)
				//IL_033d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0342: Unknown result type (might be due to invalid IL or missing references)
				//IL_0344: Unknown result type (might be due to invalid IL or missing references)
				//IL_034b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0350: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_037d: Unknown result type (might be due to invalid IL or missing references)
				//IL_037f: Unknown result type (might be due to invalid IL or missing references)
				Game.Net.Node node = default(Game.Net.Node);
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || !m_NodeData.TryGetComponent(netEntity, ref node))
				{
					return;
				}
				if (m_IgnoreOwner != Entity.Null)
				{
					Entity val = netEntity;
					Owner owner = default(Owner);
					while (m_OwnerData.TryGetComponent(val, ref owner))
					{
						if (owner.m_Owner == m_IgnoreOwner)
						{
							return;
						}
						val = owner.m_Owner;
					}
				}
				bool flag = true;
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				float3 val2 = default(float3);
				float2 val3 = default(float2);
				float3 val4 = math.mul(m_Rotation, m_LocalOffset);
				float3 val5 = math.mul(m_Rotation, new float3(m_LocalTangent.x, 0f, m_LocalTangent.y));
				float2 xz = ((float3)(ref val5)).xz;
				ControlPoint snapPosition = m_ControlPoint;
				snapPosition.m_OriginalEntity = Entity.Null;
				val5 = math.forward(m_Rotation);
				snapPosition.m_Direction = math.normalizesafe(((float3)(ref val5)).xz, default(float2));
				snapPosition.m_Rotation = m_Rotation;
				DynamicBuffer<ConnectedEdge> val6 = default(DynamicBuffer<ConnectedEdge>);
				if (m_ConnectedEdges.TryGetBuffer(netEntity, ref val6))
				{
					bool flag2 = (m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0 && (m_RoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0;
					NetGeometryData netGeometryData = default(NetGeometryData);
					for (int i = 0; i < val6.Length; i++)
					{
						Entity edge = val6[i].m_Edge;
						Edge edge2 = m_EdgeData[edge];
						Curve curve = m_CurveData[edge];
						float3 val7;
						float2 val8;
						if (edge2.m_Start == netEntity)
						{
							val7 = curve.m_Bezier.a;
							val5 = MathUtils.StartTangent(curve.m_Bezier);
							val8 = math.normalizesafe(((float3)(ref val5)).xz, default(float2));
						}
						else
						{
							if (!(edge2.m_End == netEntity))
							{
								continue;
							}
							val7 = curve.m_Bezier.d;
							val5 = MathUtils.EndTangent(curve.m_Bezier);
							val8 = math.normalizesafe(-((float3)(ref val5)).xz, default(float2));
						}
						flag = false;
						PrefabRef prefabRef = m_PrefabRefData[edge];
						NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
						if ((m_NetData.m_RequiredLayers & netData.m_RequiredLayers) == 0)
						{
							continue;
						}
						float defaultWidth = m_NetGeometryData.m_DefaultWidth;
						if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0 && m_PrefabNetGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
						{
							defaultWidth = netGeometryData.m_DefaultWidth;
						}
						int num3;
						float num4;
						float num5;
						if (flag2)
						{
							int cellWidth = ZoneUtils.GetCellWidth(m_NetGeometryData.m_DefaultWidth);
							int cellWidth2 = ZoneUtils.GetCellWidth(defaultWidth);
							num3 = 1 + math.abs(cellWidth2 - cellWidth);
							num4 = (float)(num3 - 1) * -4f;
							num5 = 8f;
						}
						else
						{
							float num6 = math.abs(defaultWidth - m_NetGeometryData.m_DefaultWidth);
							if (num6 > 1.6f)
							{
								num3 = 3;
								num4 = num6 * -0.5f;
								num5 = num6 * 0.5f;
							}
							else
							{
								num3 = 1;
								num4 = 0f;
								num5 = 0f;
							}
						}
						for (int j = 0; j < num3; j++)
						{
							float3 val9 = val7;
							if (math.abs(num4) >= 0.08f)
							{
								((float3)(ref val9)).xz = ((float3)(ref val9)).xz + MathUtils.Left(val8) * num4;
							}
							float num7 = math.distancesq(val9 - val4, m_ControlPoint.m_HitPosition);
							if (num7 < num)
							{
								num = num7;
								val2 = val9;
							}
							num4 += num5;
						}
						float num8 = math.dot(xz, val8);
						if (num8 < num2)
						{
							num2 = num8;
							val3 = val8;
						}
					}
				}
				if (flag)
				{
					PrefabRef prefabRef2 = m_PrefabRefData[netEntity];
					NetData netData2 = m_PrefabNetData[prefabRef2.m_Prefab];
					if ((m_NetData.m_RequiredLayers & netData2.m_RequiredLayers) != Layer.None && math.distancesq(node.m_Position - val4, m_ControlPoint.m_HitPosition) < num)
					{
						val2 = node.m_Position;
					}
				}
				if (num != float.MaxValue)
				{
					snapPosition.m_Position = val2 - val4;
					snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 1f, m_ControlPoint.m_HitPosition * m_SnapFactor, snapPosition.m_Position * m_SnapFactor, snapPosition.m_Direction);
					AddSnapPosition(ref m_BestSnapPosition, snapPosition);
					if (num2 != float.MaxValue && !((float2)(ref m_LocalTangent)).Equals(default(float2)))
					{
						snapPosition.m_Rotation = quaternion.RotateY(MathUtils.RotationAngleSignedRight(m_LocalTangent, -val3));
						val5 = math.forward(snapPosition.m_Rotation);
						snapPosition.m_Direction = math.normalizesafe(((float3)(ref val5)).xz, default(float2));
						snapPosition.m_Position = val2 - math.mul(snapPosition.m_Rotation, m_LocalOffset);
						snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 1f, m_ControlPoint.m_HitPosition * m_SnapFactor, snapPosition.m_Position * m_SnapFactor, snapPosition.m_Direction);
						AddSnapPosition(ref m_BestSnapPosition, snapPosition);
					}
					ref NativeList<SubSnapPoint> reference = ref m_SubSnapPoints;
					SubSnapPoint subSnapPoint = new SubSnapPoint
					{
						m_Position = val2,
						m_Tangent = val3
					};
					reference.Add(ref subSnapPoint);
				}
			}
		}

		private struct ZoneBlockIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public float m_BestDistance;

			public int2 m_LotSize;

			public Bounds2 m_Bounds;

			public float2 m_Direction;

			public Entity m_IgnoreOwner;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Block> m_BlockData;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity blockEntity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0222: Unknown result type (might be due to invalid IL or missing references)
				//IL_0233: Unknown result type (might be due to invalid IL or missing references)
				//IL_0239: Unknown result type (might be due to invalid IL or missing references)
				//IL_0246: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0261: Unknown result type (might be due to invalid IL or missing references)
				//IL_0266: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0280: Unknown result type (might be due to invalid IL or missing references)
				//IL_0285: Unknown result type (might be due to invalid IL or missing references)
				//IL_0296: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02df: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0300: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds))
				{
					return;
				}
				if (m_IgnoreOwner != Entity.Null)
				{
					Entity val = blockEntity;
					Owner owner = default(Owner);
					while (m_OwnerData.TryGetComponent(val, ref owner))
					{
						if (owner.m_Owner == m_IgnoreOwner)
						{
							return;
						}
						val = owner.m_Owner;
					}
				}
				Block block = m_BlockData[blockEntity];
				Quad2 val2 = ZoneUtils.CalculateCorners(block);
				Segment val3 = new Segment(val2.a, val2.b);
				Segment val4 = default(Segment);
				((Segment)(ref val4))._002Ector(((float3)(ref m_ControlPoint.m_HitPosition)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz);
				float2 val5 = m_Direction * (math.max(0f, (float)(m_LotSize.y - m_LotSize.x)) * 4f);
				ref float2 a = ref val4.a;
				a -= val5;
				ref float2 b = ref val4.b;
				b += val5;
				float2 val6 = default(float2);
				float num = MathUtils.Distance(val3, val4, ref val6);
				if (num == 0f)
				{
					num -= 0.5f - math.abs(val6.y - 0.5f);
				}
				if (!(num >= m_BestDistance))
				{
					m_BestDistance = num;
					float2 val7 = ((float3)(ref m_ControlPoint.m_HitPosition)).xz - ((float3)(ref block.m_Position)).xz;
					float2 val8 = MathUtils.Left(block.m_Direction);
					float num2 = (float)block.m_Size.y * 4f;
					float num3 = (float)m_LotSize.y * 4f;
					float num4 = math.dot(block.m_Direction, val7);
					float num5 = math.dot(val8, val7);
					float num6 = math.select(0f, 0.5f, ((block.m_Size.x ^ m_LotSize.x) & 1) != 0);
					num5 -= (math.round(num5 / 8f - num6) + num6) * 8f;
					m_BestSnapPosition = m_ControlPoint;
					m_BestSnapPosition.m_Position = m_ControlPoint.m_HitPosition;
					ref float3 position = ref m_BestSnapPosition.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + block.m_Direction * (num2 - num3 - num4);
					ref float3 position2 = ref m_BestSnapPosition.m_Position;
					((float3)(ref position2)).xz = ((float3)(ref position2)).xz - val8 * num5;
					m_BestSnapPosition.m_Direction = block.m_Direction;
					m_BestSnapPosition.m_Rotation = ToolUtils.CalculateRotation(m_BestSnapPosition.m_Direction);
					m_BestSnapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, m_ControlPoint.m_HitPosition * 0.5f, m_BestSnapPosition.m_Position * 0.5f, m_BestSnapPosition.m_Direction);
					m_BestSnapPosition.m_OriginalEntity = blockEntity;
				}
			}
		}

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_RemoveUpgrade;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public float m_Distance;

		[ReadOnly]
		public float m_DistanceScale;

		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_Selected;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<Game.Common.Terrain> m_TerrainData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Orphan> m_OrphanData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_RoadCompositionData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> m_MovingObjectData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_AssetStampData;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<NetObjectData> m_NetObjectData;

		[ReadOnly]
		public ComponentLookup<TransportStopData> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<StackData> m_StackData;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<NetData> m_NetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> m_PrefabCompositionAreas;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_ZoneSearchTree;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public NativeList<ControlPoint> m_ControlPoints;

		public NativeList<SubSnapPoint> m_SubSnapPoints;

		public NativeList<NetToolSystem.UpgradeState> m_UpgradeStates;

		public NativeReference<Rotation> m_Rotation;

		public NativeReference<NetToolSystem.AppliedUpgrade> m_AppliedUpgrade;

		public void Execute()
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f31: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_0968: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_096f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_0992: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0742: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0763: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_10c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1002: Unknown result type (might be due to invalid IL or missing references)
			//IL_1007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_10e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_1061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c93: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0802: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_1011: Unknown result type (might be due to invalid IL or missing references)
			//IL_1070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			//IL_104d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1054: Unknown result type (might be due to invalid IL or missing references)
			//IL_1059: Unknown result type (might be due to invalid IL or missing references)
			//IL_1020: Unknown result type (might be due to invalid IL or missing references)
			//IL_107f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_12da: Unknown result type (might be due to invalid IL or missing references)
			//IL_12ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_1304: Unknown result type (might be due to invalid IL or missing references)
			//IL_1317: Unknown result type (might be due to invalid IL or missing references)
			//IL_131c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1278: Unknown result type (might be due to invalid IL or missing references)
			//IL_1160: Unknown result type (might be due to invalid IL or missing references)
			//IL_1165: Unknown result type (might be due to invalid IL or missing references)
			//IL_108c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1352: Unknown result type (might be due to invalid IL or missing references)
			//IL_1329: Unknown result type (might be due to invalid IL or missing references)
			//IL_128b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1183: Unknown result type (might be due to invalid IL or missing references)
			//IL_1150: Unknown result type (might be due to invalid IL or missing references)
			//IL_1155: Unknown result type (might be due to invalid IL or missing references)
			//IL_112c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
			//IL_140f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1365: Unknown result type (might be due to invalid IL or missing references)
			//IL_136a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1337: Unknown result type (might be due to invalid IL or missing references)
			//IL_129e: Unknown result type (might be due to invalid IL or missing references)
			//IL_11bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_1204: Unknown result type (might be due to invalid IL or missing references)
			//IL_1209: Unknown result type (might be due to invalid IL or missing references)
			//IL_1211: Unknown result type (might be due to invalid IL or missing references)
			//IL_1216: Unknown result type (might be due to invalid IL or missing references)
			//IL_121e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1223: Unknown result type (might be due to invalid IL or missing references)
			//IL_122b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1230: Unknown result type (might be due to invalid IL or missing references)
			//IL_1238: Unknown result type (might be due to invalid IL or missing references)
			//IL_123d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1245: Unknown result type (might be due to invalid IL or missing references)
			//IL_124a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1196: Unknown result type (might be due to invalid IL or missing references)
			//IL_113f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e98: Unknown result type (might be due to invalid IL or missing references)
			//IL_1429: Unknown result type (might be due to invalid IL or missing references)
			//IL_1435: Unknown result type (might be due to invalid IL or missing references)
			//IL_1448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
			//IL_13bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_13db: Unknown result type (might be due to invalid IL or missing references)
			//IL_13f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_13fc: Unknown result type (might be due to invalid IL or missing references)
			m_SubSnapPoints.Clear();
			m_UpgradeStates.Clear();
			ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
			if ((m_Snap & (Snap.NetArea | Snap.NetNode)) != Snap.None && m_TerrainData.HasComponent(controlPoint.m_OriginalEntity) && !m_BuildingData.HasComponent(m_Prefab))
			{
				FindLoweredParent(ref controlPoint);
			}
			ControlPoint controlPoint2 = controlPoint;
			ControlPoint bestSnapPosition = controlPoint;
			bestSnapPosition.m_OriginalEntity = Entity.Null;
			if (m_OutsideConnectionData.HasComponent(m_Prefab))
			{
				HandleWorldSize(ref bestSnapPosition, controlPoint);
			}
			float waterSurfaceHeight = float.MinValue;
			if ((m_Snap & Snap.Shoreline) != Snap.None)
			{
				float radius = 1f;
				float3 offset = float3.op_Implicit(0f);
				BuildingData buildingData = default(BuildingData);
				BuildingExtensionData buildingExtensionData = default(BuildingExtensionData);
				if (m_BuildingData.TryGetComponent(m_Prefab, ref buildingData))
				{
					radius = math.length(float2.op_Implicit(buildingData.m_LotSize)) * 4f;
				}
				else if (m_BuildingExtensionData.TryGetComponent(m_Prefab, ref buildingExtensionData))
				{
					radius = math.length(float2.op_Implicit(buildingExtensionData.m_LotSize)) * 4f;
				}
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				if (m_PlaceableObjectData.TryGetComponent(m_Prefab, ref placeableObjectData))
				{
					offset = placeableObjectData.m_PlacementOffset;
				}
				SnapShoreline(controlPoint, ref bestSnapPosition, ref waterSurfaceHeight, radius, offset);
			}
			float3 val;
			if ((m_Snap & Snap.NetSide) != Snap.None)
			{
				BuildingData buildingData2 = m_BuildingData[m_Prefab];
				float num = (float)buildingData2.m_LotSize.y * 4f + 16f;
				float bestDistance = (float)math.cmin(buildingData2.m_LotSize) * 4f + 16f;
				ZoneBlockIterator zoneBlockIterator = new ZoneBlockIterator
				{
					m_ControlPoint = controlPoint,
					m_BestSnapPosition = bestSnapPosition,
					m_BestDistance = bestDistance,
					m_LotSize = buildingData2.m_LotSize,
					m_Bounds = new Bounds2(((float3)(ref controlPoint.m_Position)).xz - num, ((float3)(ref controlPoint.m_Position)).xz + num)
				};
				val = math.forward(m_Rotation.Value.m_Rotation);
				zoneBlockIterator.m_Direction = ((float3)(ref val)).xz;
				zoneBlockIterator.m_IgnoreOwner = ((m_Mode == Mode.Move) ? m_Selected : Entity.Null);
				zoneBlockIterator.m_OwnerData = m_OwnerData;
				zoneBlockIterator.m_BlockData = m_BlockData;
				ZoneBlockIterator zoneBlockIterator2 = zoneBlockIterator;
				m_ZoneSearchTree.Iterate<ZoneBlockIterator>(ref zoneBlockIterator2, 0);
				bestSnapPosition = zoneBlockIterator2.m_BestSnapPosition;
			}
			DynamicBuffer<Game.Prefabs.SubNet> val2 = default(DynamicBuffer<Game.Prefabs.SubNet>);
			if ((m_Snap & Snap.ExistingGeometry) != Snap.None && m_PrefabSubNets.TryGetBuffer(m_Prefab, ref val2))
			{
				float num2 = 2f;
				if (m_Mode == Mode.Stamp)
				{
					for (int i = 0; i < val2.Length; i++)
					{
						Game.Prefabs.SubNet subNet = val2[i];
						if (subNet.m_Snapping.x)
						{
							num2 = math.clamp(math.length(((float3)(ref subNet.m_Curve.a)).xz) * 0.02f, num2, 4f);
						}
						if (subNet.m_Snapping.y)
						{
							num2 = math.clamp(math.length(((float3)(ref subNet.m_Curve.d)).xz) * 0.02f, num2, 4f);
						}
					}
				}
				NetIterator netIterator = new NetIterator
				{
					m_ControlPoint = controlPoint,
					m_BestSnapPosition = bestSnapPosition,
					m_Rotation = m_Rotation.Value.m_Rotation,
					m_IgnoreOwner = ((m_Mode == Mode.Move) ? m_Selected : Entity.Null),
					m_SnapFactor = 1f / num2,
					m_SubSnapPoints = m_SubSnapPoints,
					m_OwnerData = m_OwnerData,
					m_NodeData = m_NodeData,
					m_EdgeData = m_EdgeData,
					m_CurveData = m_CurveData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabNetData = m_NetData,
					m_PrefabNetGeometryData = m_NetGeometryData,
					m_ConnectedEdges = m_ConnectedEdges
				};
				for (int j = 0; j < val2.Length; j++)
				{
					Game.Prefabs.SubNet subNet2 = val2[j];
					if (subNet2.m_Snapping.x)
					{
						val = ObjectUtils.LocalToWorld(controlPoint.m_HitPosition, controlPoint.m_Rotation, subNet2.m_Curve.a);
						float2 xz = ((float3)(ref val)).xz;
						netIterator.m_Bounds = new Bounds2(xz - 8f * num2, xz + 8f * num2);
						netIterator.m_LocalOffset = subNet2.m_Curve.a;
						val = MathUtils.StartTangent(subNet2.m_Curve);
						netIterator.m_LocalTangent = math.select(default(float2), math.normalizesafe(((float3)(ref val)).xz, default(float2)), subNet2.m_NodeIndex.y != subNet2.m_NodeIndex.x);
						m_NetData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_NetData);
						m_NetGeometryData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_NetGeometryData);
						m_RoadData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_RoadData);
						m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
					}
					if (subNet2.m_Snapping.y)
					{
						val = ObjectUtils.LocalToWorld(controlPoint.m_HitPosition, controlPoint.m_Rotation, subNet2.m_Curve.d);
						float2 xz2 = ((float3)(ref val)).xz;
						netIterator.m_Bounds = new Bounds2(xz2 - 8f * num2, xz2 + 8f * num2);
						netIterator.m_LocalOffset = subNet2.m_Curve.d;
						val = MathUtils.EndTangent(subNet2.m_Curve);
						netIterator.m_LocalTangent = math.normalizesafe(-((float3)(ref val)).xz, default(float2));
						m_NetData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_NetData);
						m_NetGeometryData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_NetGeometryData);
						m_RoadData.TryGetComponent(subNet2.m_Prefab, ref netIterator.m_RoadData);
						m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
					}
				}
				bestSnapPosition = netIterator.m_BestSnapPosition;
			}
			if ((m_Snap & Snap.OwnerSide) != Snap.None)
			{
				Entity val3 = Entity.Null;
				Owner owner = default(Owner);
				if (m_Mode == Mode.Upgrade)
				{
					val3 = m_Selected;
				}
				else if (m_Mode == Mode.Move && m_OwnerData.TryGetComponent(m_Selected, ref owner))
				{
					val3 = owner.m_Owner;
				}
				if (val3 != Entity.Null)
				{
					BuildingData buildingData3 = m_BuildingData[m_Prefab];
					PrefabRef prefabRef = m_PrefabRefData[val3];
					Transform transform = m_TransformData[val3];
					BuildingData buildingData4 = m_BuildingData[prefabRef.m_Prefab];
					int2 lotSize = buildingData4.m_LotSize + buildingData3.m_LotSize.y;
					Quad3 val4 = BuildingUtils.CalculateCorners(transform, lotSize);
					Quad2 xz3 = ((Quad3)(ref val4)).xz;
					int num3 = buildingData3.m_LotSize.x - 1;
					bool flag = false;
					ServiceUpgradeData serviceUpgradeData = default(ServiceUpgradeData);
					if (m_ServiceUpgradeData.TryGetComponent(m_Prefab, ref serviceUpgradeData))
					{
						num3 = math.select(num3, serviceUpgradeData.m_MaxPlacementOffset, serviceUpgradeData.m_MaxPlacementOffset >= 0);
						flag |= serviceUpgradeData.m_MaxPlacementDistance == 0f;
					}
					if (!flag)
					{
						float2 halfLotSize = float2.op_Implicit(buildingData3.m_LotSize) * 4f - 0.4f;
						val4 = BuildingUtils.CalculateCorners(transform, buildingData4.m_LotSize);
						Quad2 xz4 = ((Quad3)(ref val4)).xz;
						val4 = BuildingUtils.CalculateCorners(controlPoint.m_HitPosition, m_Rotation.Value.m_Rotation, halfLotSize);
						Quad2 xz5 = ((Quad3)(ref val4)).xz;
						flag = MathUtils.Intersect(xz4, xz5) && MathUtils.Intersect(xz3, ((float3)(ref controlPoint.m_HitPosition)).xz);
					}
					CheckSnapLine(buildingData3, transform, controlPoint, ref bestSnapPosition, new Line2(xz3.a, xz3.b), num3, 0f, flag);
					CheckSnapLine(buildingData3, transform, controlPoint, ref bestSnapPosition, new Line2(xz3.b, xz3.c), num3, (float)Math.PI / 2f, flag);
					CheckSnapLine(buildingData3, transform, controlPoint, ref bestSnapPosition, new Line2(xz3.c, xz3.d), num3, (float)Math.PI, flag);
					CheckSnapLine(buildingData3, transform, controlPoint, ref bestSnapPosition, new Line2(xz3.d, xz3.a), num3, 4.712389f, flag);
				}
			}
			if ((m_Snap & Snap.NetArea) != Snap.None)
			{
				PlaceableObjectData placeableObjectData2 = default(PlaceableObjectData);
				m_PlaceableObjectData.TryGetComponent(m_Prefab, ref placeableObjectData2);
				if (m_BuildingData.HasComponent(m_Prefab))
				{
					Curve curve = default(Curve);
					if (m_CurveData.TryGetComponent(controlPoint.m_OriginalEntity, ref curve))
					{
						ControlPoint snapPosition = controlPoint;
						snapPosition.m_OriginalEntity = controlPoint.m_OriginalEntity;
						snapPosition.m_Position = MathUtils.Position(curve.m_Bezier, controlPoint.m_CurvePosition);
						val = MathUtils.Tangent(curve.m_Bezier, controlPoint.m_CurvePosition);
						snapPosition.m_Direction = math.normalizesafe(((float3)(ref val)).xz, default(float2));
						snapPosition.m_Direction = MathUtils.Left(snapPosition.m_Direction);
						val = math.forward(m_Rotation.Value.m_Rotation);
						if (math.dot(((float3)(ref val)).xz, snapPosition.m_Direction) < 0f)
						{
							snapPosition.m_Direction = -snapPosition.m_Direction;
						}
						snapPosition.m_Rotation = ToolUtils.CalculateRotation(snapPosition.m_Direction);
						snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, 1f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
						AddSnapPosition(ref bestSnapPosition, snapPosition);
					}
				}
				else if (placeableObjectData2.m_SubReplacementType != SubReplacementType.None)
				{
					ControlPoint startPoint = ((m_ControlPoints.Length == 1) ? controlPoint : m_ControlPoints[0]);
					ControlPoint endPoint = controlPoint;
					if (m_EdgeData.HasComponent(startPoint.m_OriginalEntity) || m_NodeData.HasComponent(startPoint.m_OriginalEntity) || m_EdgeData.HasComponent(endPoint.m_OriginalEntity) || m_NodeData.HasComponent(endPoint.m_OriginalEntity))
					{
						PlaceableNetData placeableNetData = new PlaceableNetData
						{
							m_PlacementFlags = Game.Net.PlacementFlags.IsUpgrade,
							m_SetUpgradeFlags = GetCompositionFlags(placeableObjectData2.m_SubReplacementType),
							m_SnapDistance = m_DistanceScale * 0.5f
						};
						SubReplacement subReplacement = new SubReplacement
						{
							m_Type = placeableObjectData2.m_SubReplacementType,
							m_Prefab = m_Prefab
						};
						NativeList<NetToolSystem.PathEdge> path = default(NativeList<NetToolSystem.PathEdge>);
						path._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
						NetToolSystem.CreatePath(startPoint, endPoint, path, default(NetData), placeableNetData, ref m_EdgeData, ref m_NodeData, ref m_CurveData, ref m_PrefabRefData, ref m_NetData, ref m_ConnectedEdges);
						if (path.Length != 0)
						{
							m_ControlPoints.Clear();
							NetToolSystem.AddControlPoints(m_ControlPoints, m_UpgradeStates, m_AppliedUpgrade, startPoint, endPoint, path, m_Snap, m_RemoveUpgrade, m_LeftHandTraffic, m_EditorMode, default(NetGeometryData), default(RoadData), placeableNetData, subReplacement, ref m_OwnerData, ref m_EdgeData, ref m_NodeData, ref m_CurveData, ref m_CompositionData, ref m_UpgradedData, ref m_EdgeGeometryData, ref m_PrefabRefData, ref m_NetData, ref m_PrefabCompositionData, ref m_RoadCompositionData, ref m_ConnectedEdges, ref m_SubReplacements);
							return;
						}
						bestSnapPosition.m_Position = bestSnapPosition.m_HitPosition;
					}
				}
				else if (m_EdgeGeometryData.HasComponent(controlPoint.m_OriginalEntity))
				{
					EdgeGeometry edgeGeometry = m_EdgeGeometryData[controlPoint.m_OriginalEntity];
					Composition composition = m_CompositionData[controlPoint.m_OriginalEntity];
					NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
					DynamicBuffer<NetCompositionArea> areas = m_PrefabCompositionAreas[composition.m_Edge];
					float num4 = 0f;
					if (m_ObjectGeometryData.HasComponent(m_Prefab))
					{
						ObjectGeometryData objectGeometryData = m_ObjectGeometryData[m_Prefab];
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							num4 = objectGeometryData.m_LegSize.z * 0.5f + objectGeometryData.m_LegOffset.y;
							if (objectGeometryData.m_LegSize.y <= prefabCompositionData.m_HeightRange.max)
							{
								num4 = math.max(num4, objectGeometryData.m_Size.z * 0.5f);
							}
						}
						else
						{
							num4 = objectGeometryData.m_Size.z * 0.5f;
						}
					}
					SnapSegmentAreas(controlPoint, ref bestSnapPosition, num4, controlPoint.m_OriginalEntity, edgeGeometry.m_Start, prefabCompositionData, areas);
					SnapSegmentAreas(controlPoint, ref bestSnapPosition, num4, controlPoint.m_OriginalEntity, edgeGeometry.m_End, prefabCompositionData, areas);
				}
				else if (m_ConnectedEdges.HasBuffer(controlPoint.m_OriginalEntity))
				{
					DynamicBuffer<ConnectedEdge> val5 = m_ConnectedEdges[controlPoint.m_OriginalEntity];
					for (int k = 0; k < val5.Length; k++)
					{
						Entity edge = val5[k].m_Edge;
						Edge edge2 = m_EdgeData[edge];
						if ((edge2.m_Start != controlPoint.m_OriginalEntity && edge2.m_End != controlPoint.m_OriginalEntity) || !m_EdgeGeometryData.HasComponent(edge))
						{
							continue;
						}
						EdgeGeometry edgeGeometry2 = m_EdgeGeometryData[edge];
						Composition composition2 = m_CompositionData[edge];
						NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition2.m_Edge];
						DynamicBuffer<NetCompositionArea> areas2 = m_PrefabCompositionAreas[composition2.m_Edge];
						float num5 = 0f;
						if (m_ObjectGeometryData.HasComponent(m_Prefab))
						{
							ObjectGeometryData objectGeometryData2 = m_ObjectGeometryData[m_Prefab];
							if ((objectGeometryData2.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
							{
								num5 = objectGeometryData2.m_LegSize.z * 0.5f + objectGeometryData2.m_LegOffset.y;
								if (objectGeometryData2.m_LegSize.y <= prefabCompositionData2.m_HeightRange.max)
								{
									num5 = math.max(num5, objectGeometryData2.m_Size.z * 0.5f);
								}
							}
							else
							{
								num5 = objectGeometryData2.m_Size.z * 0.5f;
							}
						}
						SnapSegmentAreas(controlPoint, ref bestSnapPosition, num5, edge, edgeGeometry2.m_Start, prefabCompositionData2, areas2);
						SnapSegmentAreas(controlPoint, ref bestSnapPosition, num5, edge, edgeGeometry2.m_End, prefabCompositionData2, areas2);
					}
				}
			}
			if ((m_Snap & Snap.NetNode) != Snap.None)
			{
				if (m_NodeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					Game.Net.Node node = m_NodeData[controlPoint.m_OriginalEntity];
					SnapNode(controlPoint, ref bestSnapPosition, controlPoint.m_OriginalEntity, node);
				}
				else if (m_EdgeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					Edge edge3 = m_EdgeData[controlPoint.m_OriginalEntity];
					SnapNode(controlPoint, ref bestSnapPosition, edge3.m_Start, m_NodeData[edge3.m_Start]);
					SnapNode(controlPoint, ref bestSnapPosition, edge3.m_End, m_NodeData[edge3.m_End]);
				}
			}
			if ((m_Snap & Snap.ObjectSurface) != Snap.None && m_TransformData.HasComponent(controlPoint.m_OriginalEntity))
			{
				int parentMesh = controlPoint.m_ElementIndex.x;
				Entity val6 = controlPoint.m_OriginalEntity;
				while (m_OwnerData.HasComponent(val6))
				{
					if (m_LocalTransformCacheData.HasComponent(val6))
					{
						parentMesh = m_LocalTransformCacheData[val6].m_ParentMesh;
						parentMesh += math.select(1000, -1000, parentMesh < 0);
					}
					val6 = m_OwnerData[val6].m_Owner;
				}
				if (m_TransformData.HasComponent(val6) && m_SubObjects.HasBuffer(val6))
				{
					SnapSurface(controlPoint, ref bestSnapPosition, val6, parentMesh);
				}
			}
			if ((m_Snap & (Snap.StraightDirection | Snap.Distance)) != Snap.None && m_ControlPoints.Length >= 2)
			{
				HandleControlPoints(ref bestSnapPosition, controlPoint);
			}
			int num6;
			if (((float3)(ref controlPoint2.m_Position)).Equals(bestSnapPosition.m_Position))
			{
				Rotation value = m_Rotation.Value;
				num6 = ((!((quaternion)(ref value.m_Rotation)).Equals(bestSnapPosition.m_Rotation)) ? 1 : 0);
			}
			else
			{
				num6 = 1;
			}
			bool isSnapped = (byte)num6 != 0;
			CalculateHeight(ref bestSnapPosition, waterSurfaceHeight);
			if (m_EditorMode)
			{
				if ((m_Snap & Snap.AutoParent) == 0)
				{
					if ((m_Snap & (Snap.NetArea | Snap.NetNode)) == 0 || m_TransformData.HasComponent(bestSnapPosition.m_OriginalEntity) || m_BuildingData.HasComponent(m_Prefab))
					{
						bestSnapPosition.m_OriginalEntity = Entity.Null;
					}
				}
				else if (bestSnapPosition.m_OriginalEntity == Entity.Null)
				{
					ObjectGeometryData objectGeometryData3 = default(ObjectGeometryData);
					if (m_ObjectGeometryData.HasComponent(m_Prefab))
					{
						objectGeometryData3 = m_ObjectGeometryData[m_Prefab];
					}
					ParentObjectIterator parentObjectIterator = new ParentObjectIterator
					{
						m_ControlPoint = bestSnapPosition,
						m_BestSnapPosition = bestSnapPosition,
						m_Bounds = ObjectUtils.CalculateBounds(bestSnapPosition.m_Position, bestSnapPosition.m_Rotation, objectGeometryData3),
						m_BestOverlap = float.MaxValue,
						m_IsBuilding = m_BuildingData.HasComponent(m_Prefab),
						m_PrefabObjectGeometryData1 = objectGeometryData3,
						m_OwnerData = m_OwnerData,
						m_TransformData = m_TransformData,
						m_BuildingData = m_BuildingData,
						m_AssetStampData = m_AssetStampData,
						m_PrefabRefData = m_PrefabRefData,
						m_PrefabObjectGeometryData = m_ObjectGeometryData
					};
					m_ObjectSearchTree.Iterate<ParentObjectIterator>(ref parentObjectIterator, 0);
					bestSnapPosition = parentObjectIterator.m_BestSnapPosition;
				}
			}
			if (m_Mode == Mode.Create && m_NetObjectData.HasComponent(m_Prefab) && (m_NodeData.HasComponent(bestSnapPosition.m_OriginalEntity) || m_EdgeData.HasComponent(bestSnapPosition.m_OriginalEntity)))
			{
				FindOriginalObject(ref bestSnapPosition, controlPoint);
			}
			Rotation value2 = m_Rotation.Value;
			value2.m_IsSnapped = isSnapped;
			value2.m_IsAligned &= ((quaternion)(ref value2.m_Rotation)).Equals(bestSnapPosition.m_Rotation);
			AlignObject(ref bestSnapPosition, ref value2.m_ParentRotation, value2.m_IsAligned);
			value2.m_Rotation = bestSnapPosition.m_Rotation;
			m_Rotation.Value = value2;
			ObjectGeometryData objectGeometryData4 = default(ObjectGeometryData);
			PlaceableObjectData placeableObjectData3 = default(PlaceableObjectData);
			if ((bestSnapPosition.m_OriginalEntity == Entity.Null || bestSnapPosition.m_ElementIndex.x == -1 || bestSnapPosition.m_HitDirection.y > 0.99f) && m_ObjectGeometryData.TryGetComponent(m_Prefab, ref objectGeometryData4) && objectGeometryData4.m_Bounds.min.y <= -0.01f && ((m_PlaceableObjectData.TryGetComponent(m_Prefab, ref placeableObjectData3) && (placeableObjectData3.m_Flags & (Game.Objects.PlacementFlags.Wall | Game.Objects.PlacementFlags.Hanging)) != Game.Objects.PlacementFlags.None && (m_Snap & Snap.Upright) != Snap.None) || (m_EditorMode && m_MovingObjectData.HasComponent(m_Prefab))))
			{
				bestSnapPosition.m_Elevation -= objectGeometryData4.m_Bounds.min.y;
				bestSnapPosition.m_Position.y -= objectGeometryData4.m_Bounds.min.y;
			}
			StackData stackData = default(StackData);
			if (m_StackData.TryGetComponent(m_Prefab, ref stackData) && stackData.m_Direction == StackDirection.Up)
			{
				float num7 = stackData.m_FirstBounds.max + MathUtils.Size(stackData.m_MiddleBounds) * 2f - stackData.m_LastBounds.min;
				bestSnapPosition.m_Elevation += num7;
				bestSnapPosition.m_Position.y += num7;
			}
			m_ControlPoints[m_ControlPoints.Length - 1] = bestSnapPosition;
		}

		private CompositionFlags GetCompositionFlags(SubReplacementType subReplacementType)
		{
			if (subReplacementType == SubReplacementType.Tree)
			{
				return new CompositionFlags(CompositionFlags.General.SecondaryMiddleBeautification, (CompositionFlags.Side)0u, CompositionFlags.Side.SecondaryBeautification);
			}
			return default(CompositionFlags);
		}

		private void HandleControlPoints(ref ControlPoint bestSnapPosition, ControlPoint controlPoint)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint snapPosition = controlPoint;
			snapPosition.m_OriginalEntity = Entity.Null;
			snapPosition.m_Position = controlPoint.m_HitPosition;
			ControlPoint prev = m_ControlPoints[m_ControlPoints.Length - 2];
			if (((float2)(ref prev.m_Direction)).Equals(default(float2)) && m_ControlPoints.Length >= 3)
			{
				float2 xz = ((float3)(ref prev.m_Position)).xz;
				ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 3];
				prev.m_Direction = math.normalizesafe(xz - ((float3)(ref controlPoint2.m_Position)).xz, default(float2));
			}
			float3 snapDirection = controlPoint.m_HitPosition - prev.m_Position;
			snapDirection = MathUtils.Normalize(snapDirection, ((float3)(ref snapDirection)).xz);
			snapDirection.y = math.clamp(snapDirection.y, -1f, 1f);
			float num = float.MaxValue;
			bool flag = false;
			if ((m_Snap & Snap.StraightDirection) != Snap.None)
			{
				float bestDirectionDistance = float.MaxValue;
				if (prev.m_OriginalEntity != Entity.Null)
				{
					HandleStartDirection(prev.m_OriginalEntity, prev, controlPoint, ref bestDirectionDistance, ref snapPosition.m_Position, ref snapDirection);
				}
				if (!((float2)(ref prev.m_Direction)).Equals(default(float2)) && bestDirectionDistance == float.MaxValue)
				{
					ToolUtils.DirectionSnap(ref bestDirectionDistance, ref snapPosition.m_Position, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, new float3(prev.m_Direction.x, 0f, prev.m_Direction.y), m_DistanceScale);
				}
				num = math.min(num, 8f / m_DistanceScale);
				flag = bestDirectionDistance < m_DistanceScale;
			}
			if ((m_Snap & Snap.Distance) != Snap.None)
			{
				float num2 = math.distance(prev.m_Position, snapPosition.m_Position);
				snapPosition.m_Position = prev.m_Position + snapDirection * MathUtils.Snap(num2, m_Distance * m_DistanceScale);
				num = math.min(num, 8f / (m_Distance * m_DistanceScale));
				flag = true;
			}
			if (flag)
			{
				snapPosition.m_Direction = ((float3)(ref snapDirection)).xz;
				snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition * num, snapPosition.m_Position * num, snapPosition.m_Direction);
				ToolUtils.AddSnapPosition(ref bestSnapPosition, snapPosition);
			}
		}

		private void HandleStartDirection(Entity startEntity, ControlPoint prev, ControlPoint controlPoint, ref float bestDirectionDistance, ref float3 snapPosition, ref float3 snapDirection)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = default(Transform);
			if (m_TransformData.TryGetComponent(startEntity, ref transform))
			{
				float3 val = math.forward(transform.m_Rotation);
				val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
				val.y = math.clamp(val.y, -1f, 1f);
				ToolUtils.DirectionSnap(ref bestDirectionDistance, ref snapPosition, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, val, m_DistanceScale);
			}
		}

		private void FindLoweredParent(ref ControlPoint controlPoint)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			LoweredParentIterator loweredParentIterator = new LoweredParentIterator
			{
				m_Result = controlPoint,
				m_Position = controlPoint.m_HitPosition,
				m_EdgeData = m_EdgeData,
				m_NodeData = m_NodeData,
				m_OrphanData = m_OrphanData,
				m_CurveData = m_CurveData,
				m_CompositionData = m_CompositionData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_StartNodeGeometryData = m_StartNodeGeometryData,
				m_EndNodeGeometryData = m_EndNodeGeometryData,
				m_PrefabCompositionData = m_PrefabCompositionData
			};
			m_NetSearchTree.Iterate<LoweredParentIterator>(ref loweredParentIterator, 0);
			controlPoint = loweredParentIterator.m_Result;
		}

		private void FindOriginalObject(ref ControlPoint bestSnapPosition, ControlPoint controlPoint)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			OriginalObjectIterator originalObjectIterator = new OriginalObjectIterator
			{
				m_Parent = bestSnapPosition.m_OriginalEntity,
				m_BestDistance = float.MaxValue,
				m_EditorMode = m_EditorMode,
				m_OwnerData = m_OwnerData,
				m_AttachedData = m_AttachedData,
				m_PrefabRefData = m_PrefabRefData,
				m_NetObjectData = m_NetObjectData,
				m_TransportStopData = m_TransportStopData
			};
			ObjectGeometryData geometryData = default(ObjectGeometryData);
			if (m_ObjectGeometryData.TryGetComponent(m_Prefab, ref geometryData))
			{
				originalObjectIterator.m_Bounds = ObjectUtils.CalculateBounds(bestSnapPosition.m_Position, bestSnapPosition.m_Rotation, geometryData);
			}
			else
			{
				originalObjectIterator.m_Bounds = new Bounds3(bestSnapPosition.m_Position - 1f, bestSnapPosition.m_Position + 1f);
			}
			TransportStopData transportStopData = default(TransportStopData);
			if (m_TransportStopData.TryGetComponent(m_Prefab, ref transportStopData))
			{
				originalObjectIterator.m_TransportStopData1 = transportStopData;
			}
			m_ObjectSearchTree.Iterate<OriginalObjectIterator>(ref originalObjectIterator, 0);
			if (originalObjectIterator.m_Result != Entity.Null)
			{
				bestSnapPosition.m_OriginalEntity = originalObjectIterator.m_Result;
			}
		}

		private void HandleWorldSize(ref ControlPoint bestSnapPosition, ControlPoint controlPoint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 bounds = TerrainUtils.GetBounds(ref m_TerrainHeightData);
			bool2 val = bool2.op_Implicit(false);
			float2 val2 = float2.op_Implicit(0f);
			Bounds3 val3 = default(Bounds3);
			((Bounds3)(ref val3))._002Ector(controlPoint.m_HitPosition, controlPoint.m_HitPosition);
			ObjectGeometryData geometryData = default(ObjectGeometryData);
			if (m_ObjectGeometryData.TryGetComponent(m_Prefab, ref geometryData))
			{
				val3 = ObjectUtils.CalculateBounds(controlPoint.m_HitPosition, controlPoint.m_Rotation, geometryData);
			}
			if (val3.min.x < bounds.min.x)
			{
				val.x = true;
				val2.x = bounds.min.x;
			}
			else if (val3.max.x > bounds.max.x)
			{
				val.x = true;
				val2.x = bounds.max.x;
			}
			if (val3.min.z < bounds.min.z)
			{
				val.y = true;
				val2.y = bounds.min.z;
			}
			else if (val3.max.z > bounds.max.z)
			{
				val.y = true;
				val2.y = bounds.max.z;
			}
			if (math.any(val))
			{
				ControlPoint snapPosition = controlPoint;
				snapPosition.m_OriginalEntity = Entity.Null;
				snapPosition.m_Direction = new float2(0f, 1f);
				((float3)(ref snapPosition.m_Position)).xz = math.select(((float3)(ref controlPoint.m_HitPosition)).xz, val2, val);
				snapPosition.m_Position.y = controlPoint.m_HitPosition.y;
				snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(2f, 1f, 0f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
				float3 val4 = default(float3);
				((float3)(ref val4)).xz = math.sign(val2);
				snapPosition.m_Rotation = quaternion.LookRotationSafe(val4, math.up());
				AddSnapPosition(ref bestSnapPosition, snapPosition);
			}
		}

		public static void AlignRotation(ref quaternion rotation, quaternion parentRotation, bool zAxis)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (zAxis)
			{
				float3 val = math.rotate(rotation, new float3(0f, 0f, 1f));
				float3 val2 = math.rotate(parentRotation, new float3(0f, 1f, 0f));
				quaternion val3 = quaternion.LookRotationSafe(val, val2);
				quaternion val4 = rotation;
				float num = float.MaxValue;
				for (int i = 0; i < 8; i++)
				{
					quaternion val5 = math.mul(val3, quaternion.RotateZ((float)i * ((float)Math.PI / 4f)));
					float num2 = MathUtils.RotationAngle(rotation, val5);
					if (num2 < num)
					{
						val4 = val5;
						num = num2;
					}
				}
				rotation = math.normalizesafe(val4, quaternion.identity);
				return;
			}
			float3 val6 = math.rotate(rotation, new float3(0f, 1f, 0f));
			float3 val7 = math.rotate(parentRotation, new float3(1f, 0f, 0f));
			quaternion val8 = math.mul(quaternion.LookRotationSafe(val6, val7), quaternion.RotateX((float)Math.PI / 2f));
			quaternion val9 = rotation;
			float num3 = float.MaxValue;
			for (int j = 0; j < 8; j++)
			{
				quaternion val10 = math.mul(val8, quaternion.RotateY((float)j * ((float)Math.PI / 4f)));
				float num4 = MathUtils.RotationAngle(rotation, val10);
				if (num4 < num3)
				{
					val9 = val10;
					num3 = num4;
				}
			}
			rotation = math.normalizesafe(val9, quaternion.identity);
		}

		private void AlignObject(ref ControlPoint controlPoint, ref quaternion parentRotation, bool alignRotation)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
			if (m_PlaceableObjectData.HasComponent(m_Prefab))
			{
				placeableObjectData = m_PlaceableObjectData[m_Prefab];
			}
			if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hanging) != Game.Objects.PlacementFlags.None)
			{
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[m_Prefab];
				controlPoint.m_Position.y -= objectGeometryData.m_Bounds.max.y;
			}
			parentRotation = quaternion.identity;
			if (m_TransformData.HasComponent(controlPoint.m_OriginalEntity))
			{
				Entity val = controlPoint.m_OriginalEntity;
				PrefabRef prefabRef = m_PrefabRefData[val];
				parentRotation = m_TransformData[val].m_Rotation;
				while (m_OwnerData.HasComponent(val) && !m_BuildingData.HasComponent(prefabRef.m_Prefab))
				{
					val = m_OwnerData[val].m_Owner;
					prefabRef = m_PrefabRefData[val];
					if (m_TransformData.HasComponent(val))
					{
						parentRotation = m_TransformData[val].m_Rotation;
					}
				}
			}
			if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Wall) != Game.Objects.PlacementFlags.None)
			{
				float3 val2 = math.forward(controlPoint.m_Rotation);
				float3 val3 = controlPoint.m_HitDirection;
				val3.y = math.select(val3.y, 0f, (m_Snap & Snap.Upright) != 0);
				if (!MathUtils.TryNormalize(ref val3))
				{
					val3 = val2;
					val3.y = math.select(val3.y, 0f, (m_Snap & Snap.Upright) != 0);
					if (!MathUtils.TryNormalize(ref val3))
					{
						((float3)(ref val3))._002Ector(0f, 0f, 1f);
					}
				}
				float3 val4 = math.cross(val2, val3);
				if (MathUtils.TryNormalize(ref val4))
				{
					float num = math.acos(math.clamp(math.dot(val2, val3), -1f, 1f));
					controlPoint.m_Rotation = math.normalizesafe(math.mul(quaternion.AxisAngle(val4, num), controlPoint.m_Rotation), quaternion.identity);
					if (alignRotation)
					{
						AlignRotation(ref controlPoint.m_Rotation, parentRotation, zAxis: true);
					}
				}
				ref float3 position = ref controlPoint.m_Position;
				position += math.forward(controlPoint.m_Rotation) * placeableObjectData.m_PlacementOffset.z;
				return;
			}
			float3 val5 = math.rotate(controlPoint.m_Rotation, new float3(0f, 1f, 0f));
			float3 hitDirection = controlPoint.m_HitDirection;
			hitDirection = math.select(hitDirection, new float3(0f, 1f, 0f), (m_Snap & Snap.Upright) != 0);
			if (!MathUtils.TryNormalize(ref hitDirection))
			{
				hitDirection = val5;
			}
			float3 val6 = math.cross(val5, hitDirection);
			if (MathUtils.TryNormalize(ref val6))
			{
				float num2 = math.acos(math.clamp(math.dot(val5, hitDirection), -1f, 1f));
				controlPoint.m_Rotation = math.normalizesafe(math.mul(quaternion.AxisAngle(val6, num2), controlPoint.m_Rotation), quaternion.identity);
				if (alignRotation)
				{
					AlignRotation(ref controlPoint.m_Rotation, parentRotation, zAxis: false);
				}
			}
		}

		private void CalculateHeight(ref ControlPoint controlPoint, float waterSurfaceHeight)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PlaceableObjectData.HasComponent(m_Prefab))
			{
				return;
			}
			PlaceableObjectData placeableObjectData = m_PlaceableObjectData[m_Prefab];
			if (m_SubObjects.HasBuffer(controlPoint.m_OriginalEntity))
			{
				controlPoint.m_Position.y += placeableObjectData.m_PlacementOffset.y;
				return;
			}
			float num;
			if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.RoadSide) != Game.Objects.PlacementFlags.None && m_BuildingData.HasComponent(m_Prefab))
			{
				BuildingData buildingData = m_BuildingData[m_Prefab];
				float3 worldPosition = BuildingUtils.CalculateFrontPosition(new Transform(controlPoint.m_Position, controlPoint.m_Rotation), buildingData.m_LotSize.y);
				num = TerrainUtils.SampleHeight(ref m_TerrainHeightData, worldPosition);
			}
			else
			{
				num = TerrainUtils.SampleHeight(ref m_TerrainHeightData, controlPoint.m_Position);
			}
			if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
			{
				float num2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, controlPoint.m_Position);
				num2 += placeableObjectData.m_PlacementOffset.y;
				controlPoint.m_Elevation = math.max(0f, num2 - num);
				num = math.max(num, num2);
			}
			else if ((placeableObjectData.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating)) == 0)
			{
				num += placeableObjectData.m_PlacementOffset.y;
			}
			else
			{
				float num3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, controlPoint.m_Position, out var waterDepth);
				if (waterDepth >= 0.2f)
				{
					num3 += placeableObjectData.m_PlacementOffset.y;
					if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Floating) != Game.Objects.PlacementFlags.None)
					{
						controlPoint.m_Elevation = math.max(0f, num3 - num);
					}
					num = math.max(num, num3);
				}
			}
			if ((m_Snap & Snap.Shoreline) != Snap.None)
			{
				num = math.max(num, waterSurfaceHeight + placeableObjectData.m_PlacementOffset.y);
			}
			controlPoint.m_Position.y = num;
		}

		private void SnapSurface(ControlPoint controlPoint, ref ControlPoint bestPosition, Entity entity, int parentMesh)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[entity];
			ControlPoint snapPosition = controlPoint;
			snapPosition.m_OriginalEntity = entity;
			snapPosition.m_ElementIndex.x = parentMesh;
			snapPosition.m_Position = controlPoint.m_HitPosition;
			float3 val = math.forward(transform.m_Rotation);
			snapPosition.m_Direction = ((float3)(ref val)).xz;
			snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 1f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
			AddSnapPosition(ref bestPosition, snapPosition);
		}

		private void SnapNode(ControlPoint controlPoint, ref ControlPoint bestPosition, Entity entity, Game.Net.Node node)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[entity];
			for (int i = 0; i < val2.Length; i++)
			{
				Entity edge = val2[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if (edge2.m_Start == entity)
				{
					Composition composition = m_CompositionData[edge];
					NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_StartNode];
					val |= netCompositionData.m_SurfaceHeight;
				}
				else if (edge2.m_End == entity)
				{
					Composition composition2 = m_CompositionData[edge];
					NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition2.m_EndNode];
					val |= netCompositionData2.m_SurfaceHeight;
				}
			}
			ControlPoint snapPosition = controlPoint;
			snapPosition.m_OriginalEntity = entity;
			snapPosition.m_Position = node.m_Position;
			if (val.min < float.MaxValue)
			{
				snapPosition.m_Position.y += val.min;
			}
			float3 val3 = math.forward(node.m_Rotation);
			float3 val4 = default(float3);
			val4 = math.normalizesafe(val3, val4);
			snapPosition.m_Direction = ((float3)(ref val4)).xz;
			snapPosition.m_Rotation = node.m_Rotation;
			snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, 1f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
			AddSnapPosition(ref bestPosition, snapPosition);
		}

		private void SnapShoreline(ControlPoint controlPoint, ref ControlPoint bestPosition, ref float waterSurfaceHeight, float radius, float3 offset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			float3 val = WaterUtils.ToSurfaceSpace(ref m_WaterSurfaceData, controlPoint.m_HitPosition - radius);
			int2 val2 = (int2)math.floor(((float3)(ref val)).xz);
			val = WaterUtils.ToSurfaceSpace(ref m_WaterSurfaceData, controlPoint.m_HitPosition + radius);
			int2 val3 = (int2)math.ceil(((float3)(ref val)).xz);
			val2 = math.max(val2, default(int2));
			int2 val4 = val3;
			int3 resolution = m_WaterSurfaceData.resolution;
			val3 = math.min(val4, ((int3)(ref resolution)).xz - 1);
			float3 val5 = default(float3);
			float3 val6 = default(float3);
			float2 val7 = default(float2);
			for (int i = val2.y; i <= val3.y; i++)
			{
				for (int j = val2.x; j <= val3.x; j++)
				{
					float3 worldPosition = WaterUtils.GetWorldPosition(ref m_WaterSurfaceData, new int2(j, i));
					if (worldPosition.y > 0.2f)
					{
						float num = TerrainUtils.SampleHeight(ref m_TerrainHeightData, worldPosition) + worldPosition.y;
						float num2 = math.max(0f, radius * radius - math.distancesq(((float3)(ref worldPosition)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz));
						worldPosition.y = (worldPosition.y - 0.2f) * num2;
						((float3)(ref worldPosition)).xz = ((float3)(ref worldPosition)).xz * worldPosition.y;
						val6 += worldPosition;
						num *= num2;
						val7 += new float2(num, num2);
					}
					else if (worldPosition.y < 0.2f)
					{
						float num3 = math.max(0f, radius * radius - math.distancesq(((float3)(ref worldPosition)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz));
						worldPosition.y = (0.2f - worldPosition.y) * num3;
						((float3)(ref worldPosition)).xz = ((float3)(ref worldPosition)).xz * worldPosition.y;
						val5 += worldPosition;
					}
				}
			}
			if (val5.y != 0f && val6.y != 0f && val7.y != 0f)
			{
				val5 /= val5.y;
				val6 /= val6.y;
				float3 val8 = default(float3);
				((float3)(ref val8)).xz = ((float3)(ref val5)).xz - ((float3)(ref val6)).xz;
				if (MathUtils.TryNormalize(ref val8))
				{
					waterSurfaceHeight = val7.x / val7.y;
					bestPosition = controlPoint;
					((float3)(ref bestPosition.m_Position)).xz = math.lerp(((float3)(ref val6)).xz, ((float3)(ref val5)).xz, 0.5f);
					bestPosition.m_Position.y = waterSurfaceHeight + offset.y;
					ref float3 position = ref bestPosition.m_Position;
					position += val8 * offset.z;
					bestPosition.m_Direction = ((float3)(ref val8)).xz;
					bestPosition.m_Rotation = ToolUtils.CalculateRotation(bestPosition.m_Direction);
					bestPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, bestPosition.m_Position, bestPosition.m_Direction);
					bestPosition.m_OriginalEntity = Entity.Null;
				}
			}
		}

		private void SnapSegmentAreas(ControlPoint controlPoint, ref ControlPoint bestPosition, float radius, Entity entity, Segment segment1, NetCompositionData prefabCompositionData1, DynamicBuffer<NetCompositionArea> areas1)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			float num2 = default(float);
			for (int i = 0; i < areas1.Length; i++)
			{
				NetCompositionArea netCompositionArea = areas1[i];
				if ((netCompositionArea.m_Flags & NetAreaFlags.Buildable) == 0)
				{
					continue;
				}
				float num = netCompositionArea.m_Width * 0.51f;
				if (!(radius >= num))
				{
					Bezier4x3 val = MathUtils.Lerp(segment1.m_Left, segment1.m_Right, netCompositionArea.m_Position.x / prefabCompositionData1.m_Width + 0.5f);
					MathUtils.Distance(((Bezier4x3)(ref val)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz, ref num2);
					ControlPoint snapPosition = controlPoint;
					snapPosition.m_OriginalEntity = entity;
					snapPosition.m_Position = MathUtils.Position(val, num2);
					float3 val2 = MathUtils.Tangent(val, num2);
					snapPosition.m_Direction = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
					if ((netCompositionArea.m_Flags & NetAreaFlags.Invert) != 0)
					{
						snapPosition.m_Direction = MathUtils.Right(snapPosition.m_Direction);
					}
					else
					{
						snapPosition.m_Direction = MathUtils.Left(snapPosition.m_Direction);
					}
					float3 val3 = MathUtils.Position(MathUtils.Lerp(segment1.m_Left, segment1.m_Right, netCompositionArea.m_SnapPosition.x / prefabCompositionData1.m_Width + 0.5f), num2);
					float num3 = math.max(0f, math.min(netCompositionArea.m_Width * 0.5f, math.abs(netCompositionArea.m_SnapPosition.x - netCompositionArea.m_Position.x) + netCompositionArea.m_SnapWidth * 0.5f) - radius);
					float num4 = math.max(0f, netCompositionArea.m_SnapWidth * 0.5f - radius);
					ref float3 position = ref snapPosition.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + MathUtils.ClampLength(((float3)(ref val3)).xz - ((float3)(ref snapPosition.m_Position)).xz, num3);
					ref float3 position2 = ref snapPosition.m_Position;
					((float3)(ref position2)).xz = ((float3)(ref position2)).xz + MathUtils.ClampLength(((float3)(ref controlPoint.m_HitPosition)).xz - ((float3)(ref snapPosition.m_Position)).xz, num4);
					snapPosition.m_Position.y += netCompositionArea.m_Position.y;
					snapPosition.m_Rotation = ToolUtils.CalculateRotation(snapPosition.m_Direction);
					snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, 1f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
					AddSnapPosition(ref bestPosition, snapPosition);
				}
			}
		}

		private static Bounds3 SetHeightRange(Bounds3 bounds, Bounds1 heightRange)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			bounds.min.y += heightRange.min;
			bounds.max.y += heightRange.max;
			return bounds;
		}

		private static void CheckSnapLine(BuildingData buildingData, Transform ownerTransformData, ControlPoint controlPoint, ref ControlPoint bestPosition, Line2 line, int maxOffset, float angle, bool forceSnap)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			float num = default(float);
			MathUtils.Distance(line, ((float3)(ref controlPoint.m_Position)).xz, ref num);
			float num2 = math.select(0f, 4f, ((buildingData.m_LotSize.x - buildingData.m_LotSize.y) & 1) != 0);
			float num3 = (float)math.min(2 * maxOffset - buildingData.m_LotSize.y - buildingData.m_LotSize.x, buildingData.m_LotSize.y - buildingData.m_LotSize.x) * 4f;
			float num4 = math.distance(line.a, line.b);
			num *= num4;
			num = MathUtils.Snap(num + num2, 8f) - num2;
			num = math.clamp(num, 0f - num3, num4 + num3);
			ControlPoint snapPosition = controlPoint;
			snapPosition.m_OriginalEntity = Entity.Null;
			snapPosition.m_Position.y = ownerTransformData.m_Position.y;
			((float3)(ref snapPosition.m_Position)).xz = MathUtils.Position(line, num / num4);
			float3 val = math.mul(math.mul(ownerTransformData.m_Rotation, quaternion.RotateY(angle)), new float3(0f, 0f, 1f));
			snapPosition.m_Direction = ((float3)(ref val)).xz;
			snapPosition.m_Rotation = ToolUtils.CalculateRotation(snapPosition.m_Direction);
			float level = math.select(0f, 1f, forceSnap);
			snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, 0f, controlPoint.m_HitPosition * 0.5f, snapPosition.m_Position * 0.5f, snapPosition.m_Direction);
			AddSnapPosition(ref bestPosition, snapPosition);
		}

		private static void AddSnapPosition(ref ControlPoint bestSnapPosition, ControlPoint snapPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (ToolUtils.CompareSnapPriority(snapPosition.m_SnapPriority, bestSnapPosition.m_SnapPriority))
			{
				bestSnapPosition = snapPosition;
			}
		}
	}

	[BurstCompile]
	private struct FindAttachmentBuildingJob : IJob
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> m_BuildingDataType;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> m_SpawnableBuildingType;

		[ReadOnly]
		public BuildingData m_BuildingData;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		public NativeReference<AttachmentData> m_AttachmentPrefab;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(2000000);
			int2 lotSize = m_BuildingData.m_LotSize;
			bool2 val = default(bool2);
			((bool2)(ref val))._002Ector((m_BuildingData.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) != 0, (m_BuildingData.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0);
			AttachmentData value = default(AttachmentData);
			BuildingData buildingData = default(BuildingData);
			float num = 0f;
			bool2 val3 = default(bool2);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				NativeArray<BuildingData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<BuildingData>(ref m_BuildingDataType);
				NativeArray<SpawnableBuildingData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<SpawnableBuildingData>(ref m_SpawnableBuildingType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if (nativeArray3[j].m_Level != 1)
					{
						continue;
					}
					BuildingData buildingData2 = nativeArray2[j];
					int2 lotSize2 = buildingData2.m_LotSize;
					((bool2)(ref val3))._002Ector((buildingData2.m_Flags & Game.Prefabs.BuildingFlags.LeftAccess) != 0, (buildingData2.m_Flags & Game.Prefabs.BuildingFlags.RightAccess) != 0);
					if (math.all(lotSize2 <= lotSize))
					{
						int2 val4 = math.select(lotSize - lotSize2, int2.op_Implicit(0), lotSize2 == lotSize - 1);
						float num2 = (float)(lotSize2.x * lotSize2.y) * ((Random)(ref random)).NextFloat(1f, 1.05f);
						num2 += (float)(val4.x * lotSize2.y) * ((Random)(ref random)).NextFloat(0.95f, 1f);
						num2 += (float)(lotSize.x * val4.y) * ((Random)(ref random)).NextFloat(0.55f, 0.6f);
						num2 /= (float)(lotSize.x * lotSize.y);
						num2 *= math.csum(math.select(float2.op_Implicit(0.01f), float2.op_Implicit(0.5f), val == val3));
						if (num2 > num)
						{
							value.m_Entity = nativeArray[j];
							buildingData = buildingData2;
							num = num2;
						}
					}
				}
			}
			if (value.m_Entity != Entity.Null)
			{
				float num3 = (float)(m_BuildingData.m_LotSize.y - buildingData.m_LotSize.y) * 4f;
				value.m_Offset = new float3(0f, 0f, num3);
			}
			m_AttachmentPrefab.Value = value;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Brush> __Game_Tools_Brush_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Common.Terrain> __Game_Common_Terrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Orphan> __Game_Net_Orphan_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MovingObjectData> __Game_Prefabs_MovingObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnectionData> __Game_Prefabs_OutsideConnectionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetObjectData> __Game_Prefabs_NetObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceUpgradeData> __Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> __Game_Prefabs_NetCompositionArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Extension> __Game_Buildings_Extension_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Tools_Brush_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Brush>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Common_Terrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Common.Terrain>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Orphan_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Orphan>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_Prefabs_MovingObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MovingObjectData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnectionData>(true);
			__Game_Prefabs_NetObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetObjectData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportStopData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceUpgradeData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
			__Game_Prefabs_NetCompositionArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionArea>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_BuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<SpawnableBuildingData>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Extension_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extension>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
		}
	}

	public const string kToolID = "Object Tool";

	private const string kTree = "Tree";

	private Snap m_SelectedSnap;

	private float m_Distance;

	private AreaToolSystem m_AreaToolSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Zones.SearchSystem m_ZoneSearchSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private AudioManager m_AudioManager;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_ContainerQuery;

	private EntityQuery m_BrushQuery;

	private EntityQuery m_LotQuery;

	private EntityQuery m_BuildingQuery;

	private EntityQuery m_VisibleQuery;

	private IProxyAction m_EraseObject;

	private IProxyAction m_MoveObject;

	private IProxyAction m_PaintObject;

	private IProxyAction m_PlaceObject;

	private IProxyAction m_PlaceUpgrade;

	private IProxyAction m_PreciseRotation;

	private IProxyAction m_RotateObject;

	private IProxyAction m_PlaceNetEdge;

	private IProxyAction m_PlaceNetControlPoint;

	private IProxyAction m_UndoNetControlPoint;

	private IProxyAction m_DowngradeNetEdge;

	private IProxyAction m_UpgradeNetEdge;

	private IProxyAction m_DiscardUpgrade;

	private IProxyAction m_DiscardDowngrade;

	private IProxyAction m_ReplaceNetEdge;

	private bool m_ApplyBlocked;

	private NativeList<ControlPoint> m_ControlPoints;

	private NativeList<SubSnapPoint> m_SubSnapPoints;

	private NativeList<NetToolSystem.UpgradeState> m_UpgradeStates;

	private NativeReference<Rotation> m_Rotation;

	private NativeReference<NetToolSystem.AppliedUpgrade> m_AppliedUpgrade;

	private ControlPoint m_LastRaycastPoint;

	private ControlPoint m_StartPoint;

	private Entity m_UpgradingObject;

	private Entity m_MovingObject;

	private Entity m_MovingInitialized;

	private State m_State;

	private Mode m_LastActualMode;

	private bool m_RotationModified;

	private bool m_ForceCancel;

	private float3 m_RotationStartPosition;

	private quaternion m_StartRotation;

	private float m_StartCameraAngle;

	private EntityQuery m_SoundQuery;

	private RandomSeed m_RandomSeed;

	private ObjectPrefab m_Prefab;

	private ObjectPrefab m_SelectedPrefab;

	private TransformPrefab m_TransformPrefab;

	private CameraController m_CameraController;

	private TypeHandle __TypeHandle;

	public override string toolID => "Object Tool";

	public override int uiModeIndex => (int)actualMode;

	public Mode mode { get; set; }

	public Mode actualMode
	{
		get
		{
			Mode mode = this.mode;
			if (!allowBrush && mode == Mode.Brush)
			{
				mode = Mode.Create;
			}
			if (!allowLine && mode == Mode.Line)
			{
				mode = Mode.Create;
			}
			if (!allowCurve && mode == Mode.Curve)
			{
				mode = Mode.Create;
			}
			if (!allowStamp && mode == Mode.Stamp)
			{
				mode = Mode.Create;
			}
			if (!allowCreate && allowBrush && mode == Mode.Create)
			{
				mode = Mode.Brush;
			}
			if (!allowCreate && allowStamp && mode == Mode.Create)
			{
				mode = Mode.Stamp;
			}
			return mode;
		}
	}

	public bool isUpgradeMode
	{
		get
		{
			bool flag = m_UpgradeStates.Length >= 1;
			if (flag)
			{
				flag = actualMode switch
				{
					Mode.Create => true, 
					Mode.Line => true, 
					Mode.Curve => true, 
					_ => false, 
				};
			}
			return flag;
		}
	}

	public AgeMask ageMask { get; set; }

	public AgeMask actualAgeMask
	{
		get
		{
			if (!allowAge)
			{
				return AgeMask.Sapling;
			}
			if ((ageMask & (AgeMask.Sapling | AgeMask.Young | AgeMask.Mature | AgeMask.Elderly)) == 0)
			{
				return AgeMask.Sapling;
			}
			return ageMask;
		}
	}

	[CanBeNull]
	public ObjectPrefab prefab
	{
		get
		{
			return m_SelectedPrefab;
		}
		set
		{
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)value != (Object)(object)m_SelectedPrefab))
			{
				return;
			}
			m_SelectedPrefab = value;
			m_ForceUpdate = true;
			allowCreate = true;
			allowLine = false;
			allowCurve = false;
			allowBrush = false;
			allowStamp = false;
			allowAge = false;
			allowRotation = false;
			if ((Object)(object)value != (Object)null)
			{
				m_TransformPrefab = null;
				if (m_PrefabSystem.TryGetComponentData<ObjectGeometryData>((PrefabBase)m_SelectedPrefab, out ObjectGeometryData component))
				{
					allowLine = (component.m_Flags & Game.Objects.GeometryFlags.Brushable) != 0;
					allowCurve = (component.m_Flags & Game.Objects.GeometryFlags.Brushable) != 0;
					allowBrush = (component.m_Flags & Game.Objects.GeometryFlags.Brushable) != 0;
					allowStamp = (component.m_Flags & Game.Objects.GeometryFlags.Stampable) != 0;
					allowCreate = !allowStamp || m_ToolSystem.actionMode.IsEditor();
					float num = (((component.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None) ? component.m_Size.x : math.length(((float3)(ref component.m_Size)).xz));
					distanceScale = math.pow(2f, math.clamp(math.round(math.log2(num)), 0f, 5f));
				}
				if (m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_SelectedPrefab, out PlaceableObjectData component2))
				{
					allowRotation = component2.m_RotationSymmetry != RotationSymmetry.Any;
				}
				allowAge = m_ToolSystem.actionMode.IsGame() && m_PrefabSystem.HasComponent<TreeData>(m_SelectedPrefab);
			}
			m_ToolSystem.EventPrefabChanged?.Invoke(value);
		}
	}

	public TransformPrefab transform
	{
		get
		{
			return m_TransformPrefab;
		}
		set
		{
			if ((Object)(object)value != (Object)(object)m_TransformPrefab)
			{
				m_TransformPrefab = value;
				m_ForceUpdate = true;
				if ((Object)(object)value != (Object)null)
				{
					m_SelectedPrefab = null;
					allowCreate = true;
					allowLine = false;
					allowCurve = false;
					allowBrush = false;
					allowStamp = false;
					allowAge = false;
				}
				m_ToolSystem.EventPrefabChanged?.Invoke(value);
			}
		}
	}

	public override Snap selectedSnap
	{
		get
		{
			return m_SelectedSnap;
		}
		set
		{
			if (value != m_SelectedSnap)
			{
				m_SelectedSnap = value;
				m_ForceUpdate = true;
			}
		}
	}

	public float distance
	{
		get
		{
			return m_Distance;
		}
		set
		{
			if (value != m_Distance)
			{
				m_Distance = value;
				m_ForceUpdate = true;
			}
		}
	}

	public float distanceScale { get; private set; }

	public bool underground { get; set; }

	public bool allowCreate { get; private set; }

	public bool allowLine { get; private set; }

	public bool allowCurve { get; private set; }

	public bool allowBrush { get; private set; }

	public bool allowStamp { get; private set; }

	public bool allowAge { get; private set; }

	public bool allowRotation { get; private set; }

	public override bool brushing => actualMode == Mode.Brush;

	public State state => m_State;

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_EraseObject;
			yield return m_MoveObject;
			yield return m_PaintObject;
			yield return m_PlaceObject;
			yield return m_PlaceUpgrade;
			yield return m_PreciseRotation;
			yield return m_RotateObject;
			yield return m_PlaceNetEdge;
			yield return m_PlaceNetControlPoint;
			yield return m_UndoNetControlPoint;
			yield return m_DowngradeNetEdge;
			yield return m_UpgradeNetEdge;
			yield return m_DiscardUpgrade;
			yield return m_DiscardDowngrade;
			yield return m_ReplaceNetEdge;
		}
	}

	private float cameraAngle
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)m_CameraController != (Object)null))
			{
				return 0f;
			}
			return m_CameraController.angle.x;
		}
	}

	public override void GetUIModes(List<ToolMode> modes)
	{
		Mode mode = this.mode;
		if (mode != Mode.Create && (uint)(mode - 3) > 3u)
		{
			return;
		}
		if (allowCreate)
		{
			if ((Object)(object)prefab != (Object)null && prefab.Has<TreeObject>())
			{
				modes.Add(new ToolMode(Mode.Create.ToString() + "Tree", 0));
			}
			else
			{
				modes.Add(new ToolMode(Mode.Create.ToString(), 0));
			}
		}
		if (allowLine)
		{
			modes.Add(new ToolMode(Mode.Line.ToString(), 5));
		}
		if (allowCurve)
		{
			modes.Add(new ToolMode(Mode.Curve.ToString(), 6));
		}
		if (allowBrush)
		{
			modes.Add(new ToolMode(Mode.Brush.ToString(), 3));
		}
		if (allowStamp)
		{
			modes.Add(new ToolMode(Mode.Stamp.ToString(), 4));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AreaToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaToolSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Zones.SearchSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_DefinitionQuery = GetDefinitionQuery();
		m_ContainerQuery = GetContainerQuery();
		m_BrushQuery = GetBrushQuery();
		m_ControlPoints = new NativeList<ControlPoint>(1, AllocatorHandle.op_Implicit((Allocator)4));
		m_SubSnapPoints = new NativeList<SubSnapPoint>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_UpgradeStates = new NativeList<NetToolSystem.UpgradeState>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_Rotation = new NativeReference<Rotation>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_AppliedUpgrade = new NativeReference<NetToolSystem.AppliedUpgrade>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_Rotation.Value = new Rotation
		{
			m_Rotation = quaternion.identity,
			m_ParentRotation = quaternion.identity,
			m_IsAligned = true
		};
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_LotQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Areas.Lot>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<BuildingData>(),
			ComponentType.ReadOnly<SpawnableBuildingData>(),
			ComponentType.ReadOnly<BuildingSpawnGroupData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() });
		m_VisibleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_EraseObject = InputManager.instance.toolActionCollection.GetActionState("Erase Object", "ObjectToolSystem");
		m_MoveObject = InputManager.instance.toolActionCollection.GetActionState("Move Object", "ObjectToolSystem");
		m_PaintObject = InputManager.instance.toolActionCollection.GetActionState("Paint Object", "ObjectToolSystem");
		m_PlaceObject = InputManager.instance.toolActionCollection.GetActionState("Place Object", "ObjectToolSystem");
		m_PlaceUpgrade = InputManager.instance.toolActionCollection.GetActionState("Place Upgrade", "ObjectToolSystem");
		m_PreciseRotation = InputManager.instance.toolActionCollection.GetActionState("Precise Rotation", "ObjectToolSystem");
		m_RotateObject = InputManager.instance.toolActionCollection.GetActionState("Rotate Object", "ObjectToolSystem");
		m_PlaceNetEdge = InputManager.instance.toolActionCollection.GetActionState("Place Net Edge", "ObjectToolSystem");
		m_PlaceNetControlPoint = InputManager.instance.toolActionCollection.GetActionState("Place Net Control Point", "ObjectToolSystem");
		m_UndoNetControlPoint = InputManager.instance.toolActionCollection.GetActionState("Undo Net Control Point", "ObjectToolSystem");
		m_DowngradeNetEdge = InputManager.instance.toolActionCollection.GetActionState("Downgrade Net Edge", "ObjectToolSystem");
		m_UpgradeNetEdge = InputManager.instance.toolActionCollection.GetActionState("Upgrade Net Edge", "ObjectToolSystem");
		m_DiscardUpgrade = InputManager.instance.toolActionCollection.GetActionState("Discard Upgrade", "ObjectToolSystem");
		m_DiscardDowngrade = InputManager.instance.toolActionCollection.GetActionState("Discard Downgrade", "ObjectToolSystem");
		m_ReplaceNetEdge = InputManager.instance.toolActionCollection.GetActionState("Replace Net Edge", "ObjectToolSystem");
		base.brushSize = 200f;
		base.brushAngle = 0f;
		base.brushStrength = 0.5f;
		distance = 3f;
		distanceScale = 1f;
		selectedSnap &= ~(Snap.AutoParent | Snap.ContourLines);
		ageMask = AgeMask.Sapling;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		base.brushType = FindDefaultBrush(m_BrushQuery);
		base.brushSize = 200f;
		base.brushAngle = 0f;
		base.brushStrength = 0.5f;
		distance = 3f;
		distanceScale = 1f;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ControlPoints.Dispose();
		m_SubSnapPoints.Dispose();
		m_UpgradeStates.Dispose();
		m_Rotation.Dispose();
		m_AppliedUpgrade.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.OnStartRunning();
		m_ControlPoints.Clear();
		m_SubSnapPoints.Clear();
		m_UpgradeStates.Clear();
		m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
		m_LastRaycastPoint = default(ControlPoint);
		m_StartPoint = default(ControlPoint);
		m_State = State.Default;
		m_MovingInitialized = Entity.Null;
		m_ForceCancel = false;
		m_ApplyBlocked = false;
		Randomize();
		base.requireZones = false;
		base.requireUnderground = false;
		base.requireNetArrows = false;
		base.requireAreas = AreaTypeMask.Lots;
		base.requireNet = Layer.None;
		if (m_ToolSystem.actionMode.IsEditor())
		{
			base.requireAreas |= AreaTypeMask.Spaces;
		}
	}

	private protected override void ResetActions()
	{
		base.ResetActions();
		m_PreciseRotation.enabled = false;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			if (isUpgradeMode)
			{
				if (m_State == State.Default || m_UpgradeStates.Length == 1)
				{
					bool replacementExist = false;
					base.applyAction.enabled = base.actionsEnabled && GetAllowUpgrade(out replacementExist);
					base.applyActionOverride = (replacementExist ? m_ReplaceNetEdge : m_UpgradeNetEdge);
					base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowDowngrade(out var _);
					base.secondaryApplyActionOverride = m_DowngradeNetEdge;
					base.cancelAction.enabled = false;
					base.cancelActionOverride = null;
					m_PreciseRotation.enabled = false;
				}
				else if (m_State == State.Adding)
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = m_UpgradeNetEdge;
					base.secondaryApplyAction.enabled = false;
					base.secondaryApplyActionOverride = null;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardUpgrade;
					m_PreciseRotation.enabled = false;
				}
				else if (m_State == State.Removing)
				{
					base.applyAction.enabled = false;
					base.applyActionOverride = null;
					base.secondaryApplyAction.enabled = base.actionsEnabled;
					base.secondaryApplyActionOverride = m_DowngradeNetEdge;
					base.cancelAction.enabled = base.actionsEnabled;
					base.cancelActionOverride = m_DiscardDowngrade;
					m_PreciseRotation.enabled = false;
				}
				return;
			}
			switch (actualMode)
			{
			case Mode.Create:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
				base.applyActionOverride = m_PlaceObject;
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRotation();
				base.secondaryApplyActionOverride = m_RotateObject;
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
				m_PreciseRotation.enabled = base.actionsEnabled && GetAllowPreciseRotation();
				break;
			case Mode.Upgrade:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
				base.applyActionOverride = m_PlaceUpgrade;
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRotation();
				base.secondaryApplyActionOverride = m_RotateObject;
				base.cancelAction.enabled = base.actionsEnabled;
				base.cancelActionOverride = m_MouseCancel;
				m_PreciseRotation.enabled = base.actionsEnabled && GetAllowPreciseRotation();
				break;
			case Mode.Move:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
				base.applyActionOverride = m_MoveObject;
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRotation();
				base.secondaryApplyActionOverride = m_RotateObject;
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
				m_PreciseRotation.enabled = base.actionsEnabled && GetAllowPreciseRotation();
				break;
			case Mode.Brush:
			{
				IProxyAction proxyAction = base.applyAction;
				bool replacementExist2 = base.actionsEnabled;
				if (replacementExist2)
				{
					replacementExist2 = m_State switch
					{
						State.Default => GetAllowApply(), 
						State.Adding => true, 
						_ => false, 
					};
				}
				proxyAction.enabled = replacementExist2;
				base.applyActionOverride = m_PaintObject;
				proxyAction = base.secondaryApplyAction;
				replacementExist2 = base.actionsEnabled;
				if (replacementExist2)
				{
					replacementExist2 = m_State switch
					{
						State.Default => GetAllowApply(), 
						State.Removing => true, 
						_ => false, 
					};
				}
				proxyAction.enabled = replacementExist2;
				base.secondaryApplyActionOverride = m_EraseObject;
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
				m_PreciseRotation.enabled = false;
				break;
			}
			case Mode.Stamp:
				base.applyAction.enabled = base.actionsEnabled && (GetAllowApply() || m_State != State.Default);
				base.applyActionOverride = m_PlaceObject;
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRotation();
				base.secondaryApplyActionOverride = m_RotateObject;
				base.cancelAction.enabled = false;
				base.cancelActionOverride = null;
				m_PreciseRotation.enabled = base.actionsEnabled && GetAllowPreciseRotation();
				break;
			case Mode.Line:
			case Mode.Curve:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
				base.applyActionOverride = ((m_ControlPoints.Length < GetMaxControlPointCount(actualMode)) ? m_PlaceNetControlPoint : m_PlaceNetEdge);
				base.secondaryApplyAction.enabled = base.actionsEnabled && GetAllowRotation() && (InputManager.instance.isGamepadControlSchemeActive || m_ControlPoints.Length <= 1);
				base.secondaryApplyActionOverride = m_RotateObject;
				base.cancelAction.enabled = base.actionsEnabled && m_ControlPoints.Length >= 2;
				base.cancelActionOverride = m_UndoNetControlPoint;
				m_PreciseRotation.enabled = base.actionsEnabled && allowRotation && GetAllowPreciseRotation();
				break;
			}
		}
	}

	public override PrefabBase GetPrefab()
	{
		Mode mode = actualMode;
		if (mode == Mode.Create || (uint)(mode - 3) <= 3u)
		{
			if (!((Object)(object)prefab != (Object)null))
			{
				return transform;
			}
			return prefab;
		}
		return null;
	}

	public NativeList<ControlPoint> GetControlPoints(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_ControlPoints;
	}

	public NativeList<SubSnapPoint> GetSubSnapPoints(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_SubSnapPoints;
	}

	public NativeList<NetToolSystem.UpgradeState> GetNetUpgradeStates(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_UpgradeStates;
	}

	protected override bool GetAllowApply()
	{
		if (base.GetAllowApply())
		{
			return !((EntityQuery)(ref m_TempQuery)).IsEmptyIgnoreFilter;
		}
		return false;
	}

	protected bool GetAllowRotation()
	{
		if (allowRotation)
		{
			return !m_Rotation.Value.m_IsSnapped;
		}
		return false;
	}

	protected bool GetAllowUpgrade(out bool replacementExist)
	{
		return GetAllowUpgradeOrDowngrade(Condition, State.Adding, out replacementExist);
		bool Condition(NetToolSystem.UpgradeState upgradeState, SubReplacement replacement)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (upgradeState.m_SubReplacementPrefab == Entity.Null)
			{
				return true;
			}
			if (replacement.m_Side == upgradeState.m_SubReplacementSide && replacement.m_AgeMask == actualAgeMask)
			{
				return replacement.m_Prefab == upgradeState.m_SubReplacementPrefab;
			}
			return false;
		}
	}

	protected bool GetAllowDowngrade(out bool replacementExist)
	{
		return GetAllowUpgradeOrDowngrade(Condition, State.Removing, out replacementExist);
		static bool Condition(NetToolSystem.UpgradeState upgradeState, SubReplacement replacement)
		{
			return replacement.m_Side == upgradeState.m_SubReplacementSide;
		}
	}

	protected bool GetAllowUpgradeOrDowngrade(Func<NetToolSystem.UpgradeState, SubReplacement, bool> condition, State mode, out bool replacementExist)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		replacementExist = false;
		if (m_UpgradeStates.Length == 0 || m_ControlPoints.Length < 4)
		{
			return false;
		}
		ref NativeList<ControlPoint> reference = ref m_ControlPoints;
		Entity originalEntity = reference[reference.Length - 3].m_OriginalEntity;
		ref NativeList<ControlPoint> reference2 = ref m_ControlPoints;
		Entity originalEntity2 = reference2[reference2.Length - 2].m_OriginalEntity;
		DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
		if (!EntitiesExtensions.TryGetBuffer<ConnectedEdge>(((ComponentSystemBase)this).EntityManager, originalEntity, true, ref val))
		{
			return false;
		}
		Edge edge2 = default(Edge);
		DynamicBuffer<SubReplacement> val2 = default(DynamicBuffer<SubReplacement>);
		for (int i = 0; i < val.Length; i++)
		{
			Entity edge = val[i].m_Edge;
			if (!EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, edge, ref edge2) || ((edge2.m_Start != originalEntity || edge2.m_End != originalEntity2) && (edge2.m_End != originalEntity || edge2.m_Start != originalEntity2)))
			{
				continue;
			}
			if (!EntitiesExtensions.TryGetBuffer<SubReplacement>(((ComponentSystemBase)this).EntityManager, edge, true, ref val2))
			{
				return mode switch
				{
					State.Adding => true, 
					State.Removing => false, 
					_ => false, 
				};
			}
			for (int j = 0; j < val2.Length; j++)
			{
				SubReplacement arg = val2[j];
				if (arg.m_Side == m_UpgradeStates[0].m_SubReplacementSide)
				{
					replacementExist = true;
				}
				if (condition(m_UpgradeStates[0], arg))
				{
					return mode switch
					{
						State.Adding => false, 
						State.Removing => true, 
						_ => false, 
					};
				}
			}
			return mode switch
			{
				State.Adding => true, 
				State.Removing => false, 
				_ => false, 
			};
		}
		return false;
	}

	protected bool GetAllowPreciseRotation()
	{
		if (GetAllowRotation())
		{
			if (!InputManager.instance.isGamepadControlSchemeActive)
			{
				return !InputManager.instance.mouseOverUI;
			}
			return true;
		}
		return false;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (prefab is ObjectPrefab objectPrefab)
		{
			Mode mode = this.mode;
			if (!m_ToolSystem.actionMode.IsEditor() && prefab.Has<Game.Prefabs.ServiceUpgrade>())
			{
				Entity entity = m_PrefabSystem.GetEntity(prefab);
				if (!InternalCompilerInterface.HasComponentAfterCompletingDependency<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef, entity))
				{
					return false;
				}
				mode = Mode.Upgrade;
			}
			else if (mode == Mode.Upgrade || mode == Mode.Move)
			{
				mode = Mode.Create;
			}
			this.prefab = objectPrefab;
			this.mode = mode;
			return true;
		}
		if (prefab is TransformPrefab transformPrefab)
		{
			transform = transformPrefab;
			this.mode = Mode.Create;
			return true;
		}
		return false;
	}

	public void StartMoving(Entity movingObject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		m_MovingObject = movingObject;
		EntityManager entityManager;
		if (m_ToolSystem.actionMode.IsEditor())
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Owner owner = default(Owner);
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(m_MovingObject) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, m_MovingObject, ref owner))
			{
				m_MovingObject = owner.m_Owner;
			}
		}
		m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_RelocateBuildingSound);
		mode = Mode.Move;
		PrefabSystem prefabSystem = m_PrefabSystem;
		entityManager = ((ComponentSystemBase)this).EntityManager;
		prefab = prefabSystem.GetPrefab<ObjectPrefab>(((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(m_MovingObject));
	}

	private void Randomize()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		m_RandomSeed = RandomSeed.Next();
		if (!((Object)(object)m_SelectedPrefab != (Object)null) || !m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_SelectedPrefab, out PlaceableObjectData component) || component.m_RotationSymmetry == RotationSymmetry.None)
		{
			return;
		}
		Random random = m_RandomSeed.GetRandom(567890109);
		Rotation value = m_Rotation.Value;
		float num = (float)Math.PI * 2f;
		if (component.m_RotationSymmetry == RotationSymmetry.Any)
		{
			num = ((Random)(ref random)).NextFloat(num);
			value.m_IsAligned = false;
		}
		else
		{
			num *= (float)((Random)(ref random)).NextInt((int)component.m_RotationSymmetry) / (float)(int)component.m_RotationSymmetry;
		}
		if ((component.m_Flags & Game.Objects.PlacementFlags.Wall) != Game.Objects.PlacementFlags.None)
		{
			value.m_Rotation = math.normalizesafe(math.mul(value.m_Rotation, quaternion.RotateZ(num)), quaternion.identity);
			if (value.m_IsAligned)
			{
				SnapJob.AlignRotation(ref value.m_Rotation, value.m_ParentRotation, zAxis: true);
			}
		}
		else
		{
			value.m_Rotation = math.normalizesafe(math.mul(value.m_Rotation, quaternion.RotateY(num)), quaternion.identity);
			if (value.m_IsAligned)
			{
				SnapJob.AlignRotation(ref value.m_Rotation, value.m_ParentRotation, zAxis: false);
			}
		}
		m_Rotation.Value = value;
	}

	private ObjectPrefab GetObjectPrefab()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.actionMode.IsEditor() && (Object)(object)m_TransformPrefab != (Object)null && GetContainers(m_ContainerQuery, out var _, out var transformContainer))
		{
			return m_PrefabSystem.GetPrefab<ObjectPrefab>(transformContainer);
		}
		if (actualMode == Mode.Move)
		{
			Entity val = m_MovingObject;
			if (m_ToolSystem.actionMode.IsEditor())
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Owner owner = default(Owner);
				if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(val) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val, ref owner))
				{
					val = owner.m_Owner;
				}
			}
			PrefabRef refData = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val, ref refData))
			{
				return m_PrefabSystem.GetPrefab<ObjectPrefab>(refData);
			}
		}
		return m_SelectedPrefab;
	}

	public override void SetUnderground(bool underground)
	{
		this.underground = underground;
	}

	public override void ElevationUp()
	{
		underground = false;
	}

	public override void ElevationDown()
	{
		underground = true;
	}

	public override void ElevationScroll()
	{
		underground = !underground;
	}

	public override void InitializeRaycast()
	{
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		base.InitializeRaycast();
		m_Prefab = GetObjectPrefab();
		if ((Object)(object)m_Prefab != (Object)null)
		{
			float3 rayOffset = default(float3);
			Bounds3 val = default(Bounds3);
			if (m_PrefabSystem.TryGetComponentData<ObjectGeometryData>((PrefabBase)m_Prefab, out ObjectGeometryData component))
			{
				rayOffset.y -= component.m_Pivot.y;
				val = component.m_Bounds;
			}
			if (m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_Prefab, out PlaceableObjectData component2))
			{
				rayOffset.y -= component2.m_PlacementOffset.y;
				if ((component2.m_Flags & Game.Objects.PlacementFlags.Hanging) != Game.Objects.PlacementFlags.None)
				{
					rayOffset.y += val.max.y;
				}
			}
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.IgnoreSecondary;
			m_ToolRaycastSystem.rayOffset = rayOffset;
			GetAvailableSnapMask(out var onMask, out var offMask);
			Snap snap = ToolBaseSystem.GetActualSnap(selectedSnap, onMask, offMask);
			Mode mode = actualMode;
			if (component2.m_SubReplacementType != SubReplacementType.None && (snap & Snap.NetArea) != Snap.None && (mode == Mode.Line || mode == Mode.Curve) && m_UpgradeStates.Length == 0 && m_ControlPoints.Length >= 2 && m_State != State.Adding && m_State != State.Removing)
			{
				snap = (Snap)((uint)snap & 0xFFFFFFEFu);
			}
			if ((snap & (Snap.NetArea | Snap.NetNode)) != Snap.None)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Net;
				m_ToolRaycastSystem.netLayerMask |= Layer.Road | Layer.TrainTrack | Layer.TramTrack | Layer.SubwayTrack | Layer.PublicTransportRoad;
			}
			if ((snap & Snap.ObjectSurface) != Snap.None)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.StaticObjects;
				if (m_ToolSystem.actionMode.IsEditor())
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Placeholders;
				}
			}
			if ((snap & (Snap.NetArea | Snap.NetNode | Snap.ObjectSurface)) != Snap.None && !m_PrefabSystem.HasComponent<BuildingData>(m_Prefab))
			{
				if (underground)
				{
					m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.PartialSurface;
				}
				else
				{
					m_ToolRaycastSystem.typeMask |= TypeMask.Terrain;
					if ((component2.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Hovering)) != Game.Objects.PlacementFlags.None)
					{
						m_ToolRaycastSystem.typeMask |= TypeMask.Water;
					}
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Outside;
					m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
				}
			}
			else
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Terrain;
				if ((component2.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Hovering)) != Game.Objects.PlacementFlags.None)
				{
					m_ToolRaycastSystem.typeMask |= TypeMask.Water;
				}
				m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Outside;
				m_ToolRaycastSystem.netLayerMask |= Layer.None;
			}
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Water;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Outside;
			m_ToolRaycastSystem.netLayerMask = Layer.None;
			m_ToolRaycastSystem.rayOffset = default(float3);
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
		}
	}

	private void InitializeRotation(Entity entity, PlaceableObjectData placeableObjectData)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Rotation value = new Rotation
		{
			m_Rotation = quaternion.identity,
			m_ParentRotation = quaternion.identity,
			m_IsAligned = true
		};
		Transform transform = default(Transform);
		if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, entity, ref transform))
		{
			value.m_Rotation = transform.m_Rotation;
		}
		Owner owner = default(Owner);
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, entity, ref owner))
		{
			Entity owner2 = owner.m_Owner;
			Transform transform2 = default(Transform);
			if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, owner2, ref transform2))
			{
				value.m_ParentRotation = transform2.m_Rotation;
			}
			while (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, owner2, ref owner))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Building>(owner2))
				{
					break;
				}
				owner2 = owner.m_Owner;
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, owner2, ref transform2))
				{
					value.m_ParentRotation = transform2.m_Rotation;
				}
			}
		}
		quaternion rotation = value.m_Rotation;
		if ((placeableObjectData.m_Flags & Game.Objects.PlacementFlags.Wall) != Game.Objects.PlacementFlags.None)
		{
			SnapJob.AlignRotation(ref rotation, value.m_ParentRotation, zAxis: true);
		}
		else
		{
			SnapJob.AlignRotation(ref rotation, value.m_ParentRotation, zAxis: false);
		}
		if (MathUtils.RotationAngle(value.m_Rotation, rotation) > 0.01f)
		{
			value.m_IsAligned = false;
		}
		m_Rotation.Value = value;
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		m_UpgradingObject = Entity.Null;
		EntityManager entityManager;
		if (this.mode == Mode.Upgrade)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasBuffer<InstalledUpgrade>(GetUpgradable(m_ToolSystem.selected)))
			{
				this.mode = Mode.Create;
			}
		}
		Mode mode = actualMode;
		if (mode == Mode.Brush && (Object)(object)base.brushType == (Object)null)
		{
			base.brushType = FindDefaultBrush(m_BrushQuery);
		}
		if (mode != m_LastActualMode)
		{
			if (mode != Mode.Move)
			{
				m_MovingObject = Entity.Null;
			}
			if (m_LastActualMode == Mode.Brush)
			{
				m_ControlPoints.Clear();
			}
			bool flag = mode == Mode.Create || mode == Mode.Line || mode == Mode.Curve;
			if (m_UpgradeStates.Length != 0)
			{
				if (!flag)
				{
					m_ControlPoints.Clear();
					m_UpgradeStates.Clear();
					m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
				}
			}
			else
			{
				int maxControlPointCount = GetMaxControlPointCount(mode);
				if (maxControlPointCount < m_ControlPoints.Length)
				{
					m_ControlPoints.RemoveRange(maxControlPointCount, m_ControlPoints.Length - maxControlPointCount);
				}
			}
			m_LastActualMode = mode;
		}
		bool flag2 = m_ForceCancel;
		m_ForceCancel = false;
		if ((Object)(object)m_CameraController == (Object)null && CameraController.TryGet(out var cameraController))
		{
			m_CameraController = cameraController;
		}
		UpdateActions();
		if ((Object)(object)m_Prefab != (Object)null)
		{
			allowUnderground = false;
			base.requireUnderground = false;
			base.requireNet = Layer.None;
			base.requireNetArrows = false;
			base.requireStops = TransportType.None;
			UpdateInfoview(m_ToolSystem.actionMode.IsEditor() ? Entity.Null : m_PrefabSystem.GetEntity(m_Prefab));
			GetAvailableSnapMask(out m_SnapOnMask, out m_SnapOffMask);
			m_PrefabSystem.TryGetComponentData<ObjectGeometryData>((PrefabBase)m_Prefab, out ObjectGeometryData component);
			if (m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_Prefab, out PlaceableObjectData component2))
			{
				if ((component2.m_Flags & Game.Objects.PlacementFlags.HasUndergroundElements) != Game.Objects.PlacementFlags.None)
				{
					base.requireNet |= Layer.Road;
				}
				if ((component2.m_Flags & (Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating)) != Game.Objects.PlacementFlags.None)
				{
					base.requireNet |= Layer.Waterway;
				}
			}
			switch (mode)
			{
			case Mode.Upgrade:
				if (m_PrefabSystem.HasComponent<ServiceUpgradeData>(m_Prefab))
				{
					m_UpgradingObject = GetUpgradable(m_ToolSystem.selected);
				}
				break;
			case Mode.Move:
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).Exists(m_MovingObject))
				{
					m_MovingObject = Entity.Null;
				}
				if (m_MovingInitialized != m_MovingObject)
				{
					m_MovingInitialized = m_MovingObject;
					InitializeRotation(m_MovingObject, component2);
				}
				break;
			}
			if ((ToolBaseSystem.GetActualSnap(selectedSnap, m_SnapOnMask, m_SnapOffMask) & (Snap.NetArea | Snap.NetNode | Snap.ObjectSurface)) != Snap.None && !m_PrefabSystem.HasComponent<BuildingData>(m_Prefab) && component2.m_SubReplacementType != SubReplacementType.Tree)
			{
				allowUnderground = true;
			}
			if (m_PrefabSystem.TryGetComponentData<TransportStopData>((PrefabBase)m_Prefab, out TransportStopData component3))
			{
				base.requireNetArrows = component3.m_TransportType != TransportType.Post;
				base.requireStops = component3.m_TransportType;
			}
			base.requireUnderground = allowUnderground && underground;
			base.requireZones = !base.requireUnderground && ((component2.m_Flags & Game.Objects.PlacementFlags.RoadSide) != Game.Objects.PlacementFlags.None || ((component.m_Flags & Game.Objects.GeometryFlags.OccupyZone) != Game.Objects.GeometryFlags.None && base.requireStops == TransportType.None));
			if (m_State != State.Default && !base.applyAction.enabled && !base.secondaryApplyAction.enabled)
			{
				m_State = State.Default;
			}
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				if (isUpgradeMode)
				{
					switch (m_State)
					{
					case State.Default:
						if (m_ApplyBlocked)
						{
							if (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
							{
								m_ApplyBlocked = false;
							}
							return Update(inputDeps, fullUpdate: false);
						}
						if (base.applyAction.WasPressedThisFrame())
						{
							return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
						}
						if (base.secondaryApplyAction.WasPressedThisFrame())
						{
							return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
						}
						break;
					case State.Adding:
						if (base.cancelAction.WasPressedThisFrame())
						{
							m_ApplyBlocked = true;
							m_State = State.Default;
							return Update(inputDeps, fullUpdate: true);
						}
						if (base.applyAction.WasReleasedThisFrame())
						{
							return Apply(inputDeps);
						}
						break;
					case State.Removing:
						if (base.cancelAction.WasPressedThisFrame())
						{
							m_ApplyBlocked = true;
							m_State = State.Default;
							return Update(inputDeps, fullUpdate: true);
						}
						if (base.secondaryApplyAction.WasReleasedThisFrame())
						{
							return Cancel(inputDeps);
						}
						break;
					}
					return Update(inputDeps, fullUpdate: false);
				}
				if (base.cancelAction.WasPressedThisFrame())
				{
					if (mode == Mode.Upgrade && (m_SnapOnMask & ~m_SnapOffMask & Snap.OwnerSide) != Snap.None)
					{
						m_ToolSystem.activeTool = m_DefaultToolSystem;
					}
					return Cancel(inputDeps, base.cancelAction.WasReleasedThisFrame());
				}
				if (m_State == State.Adding || m_State == State.Removing)
				{
					if (base.applyAction.WasPressedThisFrame() || base.applyAction.WasReleasedThisFrame())
					{
						return Apply(inputDeps);
					}
					if (flag2 || base.secondaryApplyAction.WasPressedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame())
					{
						return Cancel(inputDeps);
					}
					return Update(inputDeps, fullUpdate: false);
				}
				if (m_State == State.Rotating && base.secondaryApplyAction.WasReleasedThisFrame())
				{
					if (m_RotationModified)
					{
						m_RotationModified = false;
					}
					else
					{
						Rotate((float)Math.PI / 4f, fromStart: false, align: true);
					}
					m_State = State.Default;
					return Update(inputDeps, fullUpdate: false);
				}
				if ((mode == Mode.Curve || mode == Mode.Line) && m_State == State.Default && base.secondaryApplyAction.WasPressedThisFrame())
				{
					if (m_ControlPoints.Length <= 1)
					{
						return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
					}
					Rotate((float)Math.PI / 4f, fromStart: false, align: true);
					for (int i = 0; i < m_ControlPoints.Length; i++)
					{
						ControlPoint controlPoint = m_ControlPoints[i];
						controlPoint.m_Rotation = m_Rotation.Value.m_Rotation;
						m_ControlPoints[i] = controlPoint;
					}
					return Update(inputDeps, fullUpdate: true);
				}
				if ((mode != Mode.Upgrade || (m_SnapOnMask & ~m_SnapOffMask & Snap.OwnerSide) == 0) && base.secondaryApplyAction.WasPressedThisFrame())
				{
					return Cancel(inputDeps, base.secondaryApplyAction.WasReleasedThisFrame());
				}
				if (base.applyAction.WasPressedThisFrame())
				{
					JobHandle result = Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
					if (base.applyMode == ApplyMode.Apply && mode == Mode.Move)
					{
						if (m_ToolSystem.activeTool == this)
						{
							m_ToolSystem.activeTool = m_DefaultToolSystem;
						}
						m_TerrainSystem.OnBuildingMoved(m_MovingObject);
					}
					return result;
				}
				if (m_PreciseRotation.IsInProgress())
				{
					if (m_State == State.Default)
					{
						float num = m_PreciseRotation.ReadValue<float>();
						float angle = (float)Math.PI / 2f * num * Time.deltaTime;
						Rotate(angle, fromStart: false, align: false);
						for (int j = 0; j < m_ControlPoints.Length; j++)
						{
							ControlPoint controlPoint2 = m_ControlPoints[j];
							controlPoint2.m_Rotation = m_Rotation.Value.m_Rotation;
							m_ControlPoints[j] = controlPoint2;
						}
					}
					return Update(inputDeps, fullUpdate: true);
				}
				if (m_State == State.Rotating && InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse)
				{
					float3 val = float3.op_Implicit(InputManager.instance.mousePosition);
					if (val.x != m_RotationStartPosition.x)
					{
						float angle2 = (val.x - m_RotationStartPosition.x) * ((float)Math.PI * 2f) * 0.002f;
						Rotate(angle2, fromStart: true, align: false);
						m_RotationModified = true;
					}
					return Update(inputDeps, fullUpdate: false);
				}
				return Update(inputDeps, fullUpdate: false);
			}
		}
		else
		{
			base.requireUnderground = false;
			base.requireZones = false;
			base.requireNetArrows = false;
			base.requireNet = Layer.None;
			UpdateInfoview(Entity.Null);
		}
		if (m_State == State.Adding && (base.applyAction.WasReleasedThisFrame() || base.cancelAction.WasPressedThisFrame()))
		{
			m_State = State.Default;
		}
		else if (m_State == State.Removing && (base.secondaryApplyAction.WasReleasedThisFrame() || base.cancelAction.WasPressedThisFrame()))
		{
			m_State = State.Default;
		}
		else if (m_State != State.Default && (base.applyAction.WasReleasedThisFrame() || base.secondaryApplyAction.WasReleasedThisFrame()))
		{
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	private static int GetMaxControlPointCount(Mode mode)
	{
		return mode switch
		{
			Mode.Brush => 0, 
			Mode.Line => 2, 
			Mode.Curve => 3, 
			_ => 1, 
		};
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		if ((Object)(object)m_Prefab != (Object)null)
		{
			bool flag = m_PrefabSystem.HasComponent<BuildingData>(m_Prefab);
			bool isAssetStamp = !flag && m_PrefabSystem.HasComponent<AssetStampData>(m_Prefab);
			m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_Prefab, out PlaceableObjectData component);
			GetAvailableSnapMask(component, m_ToolSystem.actionMode.IsEditor(), flag, isAssetStamp, actualMode, out onMask, out offMask);
		}
		else
		{
			base.GetAvailableSnapMask(out onMask, out offMask);
		}
	}

	private static void GetAvailableSnapMask(PlaceableObjectData prefabPlaceableData, bool editorMode, bool isBuilding, bool isAssetStamp, Mode mode, out Snap onMask, out Snap offMask)
	{
		onMask = Snap.Upright;
		offMask = Snap.None;
		if ((prefabPlaceableData.m_Flags & (Game.Objects.PlacementFlags.RoadSide | Game.Objects.PlacementFlags.OwnerSide)) == Game.Objects.PlacementFlags.OwnerSide)
		{
			onMask |= Snap.OwnerSide;
		}
		else if ((prefabPlaceableData.m_Flags & (Game.Objects.PlacementFlags.RoadSide | Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Hovering)) != Game.Objects.PlacementFlags.None)
		{
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.OwnerSide) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.OwnerSide;
				offMask |= Snap.OwnerSide;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadSide) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.NetSide;
				offMask |= Snap.NetSide;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadEdge) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.NetArea;
				offMask |= Snap.NetArea;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.Shoreline) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.Shoreline;
				offMask |= Snap.Shoreline;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.ObjectSurface;
				offMask |= Snap.ObjectSurface;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.SubNetSnap) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.ExistingGeometry;
				offMask |= Snap.ExistingGeometry;
			}
		}
		else if ((prefabPlaceableData.m_Flags & (Game.Objects.PlacementFlags.RoadNode | Game.Objects.PlacementFlags.RoadEdge)) != Game.Objects.PlacementFlags.None)
		{
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadNode) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.NetNode;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadEdge) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.NetArea;
			}
		}
		else
		{
			if (prefabPlaceableData.m_SubReplacementType != SubReplacementType.None && mode != Mode.Move)
			{
				onMask |= Snap.NetArea;
				offMask |= Snap.NetArea;
			}
			if (editorMode && !isBuilding)
			{
				onMask |= Snap.ObjectSurface;
				offMask |= Snap.ObjectSurface;
				offMask |= Snap.Upright;
			}
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.SubNetSnap) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.ExistingGeometry;
				offMask |= Snap.ExistingGeometry;
			}
		}
		if (editorMode && (!isAssetStamp || mode == Mode.Stamp))
		{
			onMask |= Snap.AutoParent;
			offMask |= Snap.AutoParent;
		}
		if (mode == Mode.Line || mode == Mode.Curve)
		{
			onMask |= Snap.Distance;
			offMask |= Snap.Distance;
		}
		if (mode == Mode.Curve || (editorMode && mode == Mode.Line))
		{
			onMask |= Snap.StraightDirection;
			offMask |= Snap.StraightDirection;
		}
		if (mode == Mode.Brush)
		{
			onMask &= Snap.Upright;
			offMask &= Snap.Upright;
			onMask |= Snap.PrefabType;
			offMask |= Snap.PrefabType;
		}
		if (isBuilding || isAssetStamp)
		{
			onMask |= Snap.ContourLines;
			offMask |= Snap.ContourLines;
		}
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		return inputDeps;
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Brush)
		{
			if (m_State == State.Default)
			{
				base.applyMode = ApplyMode.Clear;
				Randomize();
				m_StartPoint = m_LastRaycastPoint;
				m_State = State.Removing;
				m_ForceCancel = singleFrameOnly;
				GetRaycastResult(out m_LastRaycastPoint);
				return UpdateDefinitions(inputDeps);
			}
			if (m_State == State.Removing && GetAllowApply())
			{
				base.applyMode = ApplyMode.Apply;
				Randomize();
				m_StartPoint = default(ControlPoint);
				m_State = State.Default;
				GetRaycastResult(out m_LastRaycastPoint);
				return UpdateDefinitions(inputDeps);
			}
			base.applyMode = ApplyMode.Clear;
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_LastRaycastPoint);
			return UpdateDefinitions(inputDeps);
		}
		if (m_State != State.Removing && m_UpgradeStates.Length >= 1)
		{
			m_State = State.Removing;
			m_ForceCancel = singleFrameOnly;
			m_ForceUpdate = true;
			m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
			return Update(inputDeps, fullUpdate: true);
		}
		if (m_State == State.Removing)
		{
			m_State = State.Default;
			if (GetAllowApply())
			{
				SetAppliedUpgrade(removing: true);
				base.applyMode = ApplyMode.Apply;
				m_RandomSeed = RandomSeed.Next();
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				if (GetRaycastResult(out var controlPoint))
				{
					m_ControlPoints.Add(ref controlPoint);
					inputDeps = SnapControlPoint(inputDeps);
					inputDeps = FixNetControlPoints(inputDeps);
					inputDeps = UpdateDefinitions(inputDeps);
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolRemovePointSound);
				}
				else
				{
					inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
				}
			}
			else
			{
				base.applyMode = ApplyMode.Clear;
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				if (GetRaycastResult(out var controlPoint2))
				{
					m_ControlPoints.Add(ref controlPoint2);
					inputDeps = SnapControlPoint(inputDeps);
					inputDeps = UpdateDefinitions(inputDeps);
				}
				else
				{
					inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
				}
			}
			return inputDeps;
		}
		if ((actualMode != Mode.Upgrade || (m_SnapOnMask & ~m_SnapOffMask & Snap.OwnerSide) == 0) && m_ControlPoints.Length <= 1)
		{
			if (singleFrameOnly)
			{
				Rotate((float)Math.PI / 4f, fromStart: false, align: true);
			}
			else
			{
				m_State = State.Rotating;
				m_RotationStartPosition = float3.op_Implicit(InputManager.instance.mousePosition);
				m_StartRotation = m_Rotation.Value.m_Rotation;
				m_StartCameraAngle = cameraAngle;
			}
		}
		base.applyMode = ApplyMode.Clear;
		if (m_ControlPoints.Length > 0)
		{
			m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
		}
		m_UpgradeStates.Clear();
		m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
		if (GetRaycastResult(out var controlPoint3))
		{
			controlPoint3.m_Rotation = m_Rotation.Value.m_Rotation;
			if (m_ControlPoints.Length > 0)
			{
				m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint3;
			}
			else
			{
				m_ControlPoints.Add(ref controlPoint3);
			}
			inputDeps = SnapControlPoint(inputDeps);
			inputDeps = UpdateDefinitions(inputDeps);
		}
		return inputDeps;
	}

	private JobHandle FixNetControlPoints(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<NetToolSystem.FixControlPointsJob>(new NetToolSystem.FixControlPointsJob
		{
			m_Chunks = chunks,
			m_Mode = NetToolSystem.Mode.Replace,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControlPoints = m_ControlPoints
		}, JobHandle.CombineDependencies(inputDeps, val));
		chunks.Dispose(val2);
		return val2;
	}

	private void Rotate(float angle, bool fromStart, bool align)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabSystem.TryGetComponentData<PlaceableObjectData>((PrefabBase)m_Prefab, out PlaceableObjectData component);
		Rotation value = m_Rotation.Value;
		bool flag = (component.m_Flags & Game.Objects.PlacementFlags.Wall) != 0;
		value.m_Rotation = math.mul(fromStart ? m_StartRotation : value.m_Rotation, flag ? quaternion.RotateZ(angle) : quaternion.RotateY(angle));
		value.m_Rotation = math.normalizesafe(value.m_Rotation, quaternion.identity);
		if (align)
		{
			quaternion parentRotation = value.m_ParentRotation;
			if ((actualMode == Mode.Line || actualMode == Mode.Curve) && m_UpgradeStates.Length == 0 && m_ControlPoints.Length >= 2)
			{
				ControlPoint controlPoint = m_ControlPoints[1];
				float2 xz = ((float3)(ref controlPoint.m_Position)).xz;
				controlPoint = m_ControlPoints[0];
				float2 val = xz - ((float3)(ref controlPoint.m_Position)).xz;
				if (MathUtils.TryNormalize(ref val))
				{
					parentRotation = quaternion.LookRotation(new float3(val.x, 0f, val.y), math.up());
				}
			}
			SnapJob.AlignRotation(ref value.m_Rotation, parentRotation, flag);
		}
		value.m_IsAligned = align;
		m_Rotation.Value = value;
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Brush)
		{
			bool allowApply = GetAllowApply();
			if (m_State == State.Default)
			{
				base.applyMode = (allowApply ? ApplyMode.Apply : ApplyMode.Clear);
				Randomize();
				if (!singleFrameOnly)
				{
					m_StartPoint = m_LastRaycastPoint;
					m_State = State.Adding;
				}
				GetRaycastResult(out m_LastRaycastPoint);
				return UpdateDefinitions(inputDeps);
			}
			if (m_State == State.Adding && allowApply)
			{
				base.applyMode = ApplyMode.Apply;
				Randomize();
				m_StartPoint = default(ControlPoint);
				m_State = State.Default;
				GetRaycastResult(out m_LastRaycastPoint);
				return UpdateDefinitions(inputDeps);
			}
			base.applyMode = ApplyMode.Clear;
			m_StartPoint = default(ControlPoint);
			m_State = State.Default;
			GetRaycastResult(out m_LastRaycastPoint);
			return UpdateDefinitions(inputDeps);
		}
		if (m_State != State.Adding && m_UpgradeStates.Length >= 1 && !singleFrameOnly)
		{
			m_State = State.Adding;
			m_ForceUpdate = true;
			m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
			return Update(inputDeps, fullUpdate: true);
		}
		if (m_State == State.Adding)
		{
			m_State = State.Default;
			if (GetAllowApply())
			{
				SetAppliedUpgrade(removing: false);
				base.applyMode = ApplyMode.Apply;
				m_RandomSeed = RandomSeed.Next();
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetBuildSound);
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				if (GetRaycastResult(out var controlPoint))
				{
					m_ControlPoints.Add(ref controlPoint);
					inputDeps = SnapControlPoint(inputDeps);
					inputDeps = FixNetControlPoints(inputDeps);
					inputDeps = UpdateDefinitions(inputDeps);
				}
				else
				{
					inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
				}
			}
			else
			{
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				m_ForceUpdate = true;
				inputDeps = Update(inputDeps, fullUpdate: true);
			}
			return inputDeps;
		}
		if (m_ControlPoints.Length < GetMaxControlPointCount(actualMode))
		{
			base.applyMode = ApplyMode.Clear;
			if (GetRaycastResult(out var controlPoint2))
			{
				if (m_ControlPoints.Length <= 1)
				{
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetStartSound);
				}
				else
				{
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetNodeSound);
				}
				controlPoint2.m_Rotation = m_Rotation.Value.m_Rotation;
				m_ControlPoints.Add(ref controlPoint2);
				inputDeps = SnapControlPoint(inputDeps);
				inputDeps = UpdateDefinitions(inputDeps);
			}
			else
			{
				inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
			}
		}
		else if (GetAllowApply())
		{
			base.applyMode = ApplyMode.Apply;
			Randomize();
			if (m_Prefab is BuildingPrefab || m_Prefab is AssetStampPrefab)
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PlaceBuildingSound);
			}
			else if (m_Prefab is StaticObjectPrefab || m_ToolSystem.actionMode.IsEditor())
			{
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PlacePropSound);
			}
			m_ControlPoints.Clear();
			m_UpgradeStates.Clear();
			m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
			if (m_ToolSystem.actionMode.IsGame() && !((EntityQuery)(ref m_LotQuery)).IsEmptyIgnoreFilter)
			{
				NativeArray<Entity> val = ((EntityQuery)(ref m_LotQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
				try
				{
					for (int i = 0; i < val.Length; i++)
					{
						Entity val2 = val[i];
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						Area componentData = ((EntityManager)(ref entityManager)).GetComponentData<Area>(val2);
						entityManager = ((ComponentSystemBase)this).EntityManager;
						Temp componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Temp>(val2);
						if ((componentData.m_Flags & AreaFlags.Slave) == 0 && (componentData2.m_Flags & TempFlags.Create) != 0)
						{
							PrefabSystem prefabSystem = m_PrefabSystem;
							entityManager = ((ComponentSystemBase)this).EntityManager;
							LotPrefab lotPrefab = prefabSystem.GetPrefab<LotPrefab>(((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val2));
							if (!lotPrefab.m_AllowOverlap)
							{
								m_AreaToolSystem.recreate = val2;
								m_AreaToolSystem.prefab = lotPrefab;
								m_AreaToolSystem.mode = AreaToolSystem.Mode.Edit;
								m_ToolSystem.activeTool = m_AreaToolSystem;
								return inputDeps;
							}
						}
					}
				}
				finally
				{
					val.Dispose();
				}
			}
			if (GetRaycastResult(out var controlPoint3))
			{
				if (m_ToolSystem.actionMode.IsGame())
				{
					Telemetry.PlaceBuilding(m_UpgradingObject, m_Prefab, controlPoint3.m_Position);
				}
				controlPoint3.m_Rotation = m_Rotation.Value.m_Rotation;
				m_ControlPoints.Add(ref controlPoint3);
				inputDeps = SnapControlPoint(inputDeps);
				inputDeps = UpdateDefinitions(inputDeps);
			}
		}
		else
		{
			m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PlaceBuildingFailSound);
			inputDeps = Update(inputDeps, fullUpdate: false);
		}
		return inputDeps;
	}

	private JobHandle Update(JobHandle inputDeps, bool fullUpdate)
	{
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Brush)
		{
			if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
			{
				if (m_State != State.Default)
				{
					base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
					Randomize();
					m_StartPoint = m_LastRaycastPoint;
					m_LastRaycastPoint = controlPoint;
					return UpdateDefinitions(inputDeps);
				}
				if (m_LastRaycastPoint.Equals(controlPoint) && !forceUpdate)
				{
					if (HaveBrushSettingsChanged())
					{
						base.applyMode = ApplyMode.Clear;
						return UpdateDefinitions(inputDeps);
					}
					base.applyMode = ApplyMode.None;
					return inputDeps;
				}
				base.applyMode = ApplyMode.Clear;
				m_StartPoint = controlPoint;
				m_LastRaycastPoint = controlPoint;
				return UpdateDefinitions(inputDeps);
			}
			if (m_LastRaycastPoint.Equals(default(ControlPoint)) && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			if (m_State != State.Default)
			{
				base.applyMode = (GetAllowApply() ? ApplyMode.Apply : ApplyMode.Clear);
				Randomize();
				m_StartPoint = m_LastRaycastPoint;
				m_LastRaycastPoint = default(ControlPoint);
			}
			else
			{
				base.applyMode = ApplyMode.Clear;
				m_StartPoint = default(ControlPoint);
				m_LastRaycastPoint = default(ControlPoint);
			}
			return UpdateDefinitions(inputDeps);
		}
		if (GetRaycastResult(out ControlPoint controlPoint2, out bool forceUpdate2))
		{
			controlPoint2.m_Rotation = m_Rotation.Value.m_Rotation;
			forceUpdate2 = forceUpdate2 || fullUpdate;
			base.applyMode = ApplyMode.None;
			if (!m_LastRaycastPoint.Equals(controlPoint2) || forceUpdate2)
			{
				m_LastRaycastPoint = controlPoint2;
				ControlPoint controlPoint3 = default(ControlPoint);
				if (m_ControlPoints.Length != 0)
				{
					controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 1];
				}
				if (m_State == State.Adding || m_State == State.Removing)
				{
					if (m_ControlPoints.Length == 1)
					{
						m_ControlPoints.Add(ref controlPoint2);
					}
					else
					{
						m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint2;
					}
				}
				else
				{
					if (m_UpgradeStates.Length != 0)
					{
						m_ControlPoints.Clear();
						m_UpgradeStates.Clear();
					}
					if (m_ControlPoints.Length == 0)
					{
						m_ControlPoints.Add(ref controlPoint2);
					}
					else
					{
						m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint2;
					}
				}
				inputDeps = SnapControlPoint(inputDeps);
				JobHandle.ScheduleBatchedJobs();
				if (!forceUpdate2)
				{
					((JobHandle)(ref inputDeps)).Complete();
					ControlPoint other = m_ControlPoints[m_ControlPoints.Length - 1];
					forceUpdate2 = !controlPoint3.EqualsIgnoreHit(other);
				}
				if (forceUpdate2)
				{
					base.applyMode = ApplyMode.Clear;
					inputDeps = UpdateDefinitions(inputDeps);
				}
			}
		}
		else
		{
			base.applyMode = ApplyMode.Clear;
			m_LastRaycastPoint = default(ControlPoint);
			if (m_State == State.Default)
			{
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
			}
			inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		return inputDeps;
	}

	private bool HaveBrushSettingsChanged()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_VisibleQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			ComponentTypeHandle<Brush> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Brush>(ref __TypeHandle.__Game_Tools_Brush_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Brush> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<Brush>(ref componentTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if (!nativeArray[j].m_Size.Equals(base.brushSize))
					{
						return true;
					}
				}
			}
			return false;
		}
		finally
		{
			val.Dispose();
		}
	}

	private void SetAppliedUpgrade(bool removing)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
		if (m_UpgradeStates.Length < 1 || m_ControlPoints.Length < 4)
		{
			return;
		}
		Entity originalEntity = m_ControlPoints[m_ControlPoints.Length - 3].m_OriginalEntity;
		Entity originalEntity2 = m_ControlPoints[m_ControlPoints.Length - 2].m_OriginalEntity;
		NetToolSystem.UpgradeState upgradeState = m_UpgradeStates[m_UpgradeStates.Length - 1];
		NetToolSystem.AppliedUpgrade value = new NetToolSystem.AppliedUpgrade
		{
			m_SubReplacementPrefab = upgradeState.m_SubReplacementPrefab,
			m_Flags = (removing ? upgradeState.m_RemoveFlags : upgradeState.m_AddFlags),
			m_SubReplacementType = upgradeState.m_SubReplacementType,
			m_SubReplacementSide = upgradeState.m_SubReplacementSide
		};
		if (originalEntity == originalEntity2)
		{
			value.m_Entity = originalEntity;
			m_AppliedUpgrade.Value = value;
		}
		else
		{
			DynamicBuffer<ConnectedEdge> val = default(DynamicBuffer<ConnectedEdge>);
			if (!EntitiesExtensions.TryGetBuffer<ConnectedEdge>(((ComponentSystemBase)this).EntityManager, originalEntity, true, ref val))
			{
				return;
			}
			Edge edge2 = default(Edge);
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				if (EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, edge, ref edge2) && ((edge2.m_Start == originalEntity && edge2.m_End == originalEntity2) || (edge2.m_End == originalEntity && edge2.m_Start == originalEntity2)))
				{
					value.m_Entity = edge;
					m_AppliedUpgrade.Value = value;
				}
			}
		}
	}

	private Entity GetUpgradable(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Attached attached = default(Attached);
		if (EntitiesExtensions.TryGetComponent<Attached>(((ComponentSystemBase)this).EntityManager, entity, ref attached))
		{
			return attached.m_Parent;
		}
		return entity;
	}

	private JobHandle SnapControlPoint(JobHandle inputDeps)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		Entity selected = ((actualMode == Mode.Move) ? m_MovingObject : GetUpgradable(m_ToolSystem.selected));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle deps;
		JobHandle val = IJobExtensions.Schedule<SnapJob>(new SnapJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_RemoveUpgrade = (m_State == State.Removing),
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_Distance = math.max(1f, distance),
			m_DistanceScale = distanceScale,
			m_Snap = GetActualSnap(),
			m_Mode = actualMode,
			m_Prefab = m_PrefabSystem.GetEntity(m_Prefab),
			m_Selected = selected,
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainData = InternalCompilerInterface.GetComponentLookup<Game.Common.Terrain>(ref __TypeHandle.__Game_Common_Terrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanData = InternalCompilerInterface.GetComponentLookup<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadCompositionData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingObjectData = InternalCompilerInterface.GetComponentLookup<MovingObjectData>(ref __TypeHandle.__Game_Prefabs_MovingObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnectionData>(ref __TypeHandle.__Game_Prefabs_OutsideConnectionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetObjectData = InternalCompilerInterface.GetComponentLookup<NetObjectData>(ref __TypeHandle.__Game_Prefabs_NetObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopData = InternalCompilerInterface.GetComponentLookup<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<ServiceUpgradeData>(ref __TypeHandle.__Game_Prefabs_ServiceUpgradeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionAreas = InternalCompilerInterface.GetBufferLookup<NetCompositionArea>(ref __TypeHandle.__Game_Prefabs_NetCompositionArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_ZoneSearchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies3),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_ControlPoints = m_ControlPoints,
			m_SubSnapPoints = m_SubSnapPoints,
			m_UpgradeStates = m_UpgradeStates,
			m_Rotation = m_Rotation,
			m_AppliedUpgrade = m_AppliedUpgrade
		}, JobUtils.CombineDependencies(inputDeps, dependencies, dependencies2, dependencies3, deps));
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_ZoneSearchSystem.AddSearchTreeReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		return val;
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if ((Object)(object)m_Prefab != (Object)null)
		{
			Snap actualSnap = GetActualSnap();
			Entity entity = m_PrefabSystem.GetEntity(m_Prefab);
			if (actualMode != Mode.Brush && (actualSnap & Snap.NetArea) != Snap.None)
			{
				if (m_State == State.Adding || m_State == State.Removing)
				{
					return JobHandle.CombineDependencies(val, UpdateSubReplacementDefinitions(inputDeps));
				}
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				if (EntitiesExtensions.TryGetComponent<PlaceableObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref placeableObjectData) && placeableObjectData.m_SubReplacementType != SubReplacementType.None)
				{
					((JobHandle)(ref inputDeps)).Complete();
					if (m_UpgradeStates.Length != 0)
					{
						return JobHandle.CombineDependencies(val, UpdateSubReplacementDefinitions(default(JobHandle)));
					}
				}
			}
			Entity laneContainer = Entity.Null;
			Entity transformPrefab = Entity.Null;
			Entity brushPrefab = Entity.Null;
			float deltaTime = Time.deltaTime;
			float num = 0f;
			if (m_ToolSystem.actionMode.IsEditor())
			{
				GetContainers(m_ContainerQuery, out laneContainer, out var _);
			}
			if ((Object)(object)m_TransformPrefab != (Object)null)
			{
				transformPrefab = m_PrefabSystem.GetEntity(m_TransformPrefab);
			}
			if (actualMode == Mode.Brush && (Object)(object)base.brushType != (Object)null)
			{
				brushPrefab = m_PrefabSystem.GetEntity(base.brushType);
				EnsureCachedBrushData();
				ControlPoint controlPoint = m_StartPoint;
				ControlPoint controlPoint2 = m_LastRaycastPoint;
				controlPoint.m_OriginalEntity = Entity.Null;
				controlPoint2.m_OriginalEntity = Entity.Null;
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				m_AppliedUpgrade.Value = default(NetToolSystem.AppliedUpgrade);
				m_ControlPoints.Add(ref controlPoint);
				m_ControlPoints.Add(ref controlPoint2);
				if (m_State == State.Default)
				{
					deltaTime = 0.1f;
				}
			}
			if (actualMode == Mode.Line || actualMode == Mode.Curve)
			{
				num = math.max(1f, distance) * distanceScale;
			}
			NativeReference<AttachmentData> attachmentPrefab = default(NativeReference<AttachmentData>);
			PlaceholderBuildingData placeholderBuildingData = default(PlaceholderBuildingData);
			if (!m_ToolSystem.actionMode.IsEditor() && EntitiesExtensions.TryGetComponent<PlaceholderBuildingData>(((ComponentSystemBase)this).EntityManager, entity, ref placeholderBuildingData))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				ZoneData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(placeholderBuildingData.m_ZonePrefab);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				BuildingData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingData>(entity);
				((EntityQuery)(ref m_BuildingQuery)).ResetFilter();
				((EntityQuery)(ref m_BuildingQuery)).SetSharedComponentFilter<BuildingSpawnGroupData>(new BuildingSpawnGroupData(componentData.m_ZoneType));
				attachmentPrefab._002Ector(AllocatorHandle.op_Implicit((Allocator)3), (NativeArrayOptions)1);
				JobHandle val2 = default(JobHandle);
				NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_BuildingQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
				inputDeps = IJobExtensions.Schedule<FindAttachmentBuildingJob>(new FindAttachmentBuildingJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingDataType = InternalCompilerInterface.GetComponentTypeHandle<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_SpawnableBuildingType = InternalCompilerInterface.GetComponentTypeHandle<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_BuildingData = componentData2,
					m_RandomSeed = m_RandomSeed,
					m_Chunks = chunks,
					m_AttachmentPrefab = attachmentPrefab
				}, JobHandle.CombineDependencies(inputDeps, val2));
				chunks.Dispose(inputDeps);
			}
			val = JobHandle.CombineDependencies(val, CreateDefinitions(entity, transformPrefab, brushPrefab, m_UpgradingObject, m_MovingObject, laneContainer, m_CityConfigurationSystem.defaultTheme, m_ControlPoints, attachmentPrefab, m_ToolSystem.actionMode.IsEditor(), m_CityConfigurationSystem.leftHandTraffic, m_State == State.Removing, actualMode == Mode.Stamp, base.brushSize, math.radians(base.brushAngle), base.brushStrength, num, deltaTime, m_RandomSeed, actualSnap, actualAgeMask, inputDeps));
			if (attachmentPrefab.IsCreated)
			{
				attachmentPrefab.Dispose(val);
			}
		}
		return val;
	}

	private JobHandle UpdateSubReplacementDefinitions(JobHandle inputDeps)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps;
		JobHandle val = IJobExtensions.Schedule<NetToolSystem.CreateDefinitionsJob>(new NetToolSystem.CreateDefinitionsJob
		{
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_RemoveUpgrade = (m_State == State.Removing),
			m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_Mode = NetToolSystem.Mode.Replace,
			m_RandomSeed = m_RandomSeed,
			m_AgeMask = actualAgeMask,
			m_ControlPoints = m_ControlPoints,
			m_UpgradeStates = m_UpgradeStates,
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_FixedData = InternalCompilerInterface.GetComponentLookup<Fixed>(ref __TypeHandle.__Game_Net_Fixed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtensionData = InternalCompilerInterface.GetComponentLookup<Extension>(ref __TypeHandle.__Game_Buildings_Extension_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSpawnableObjectData = InternalCompilerInterface.GetComponentLookup<SpawnableObjectData>(ref __TypeHandle.__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CachedNodes = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubNets = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubNet>(ref __TypeHandle.__Game_Prefabs_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreas = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubArea>(ref __TypeHandle.__Game_Prefabs_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabSubAreaNodes = InternalCompilerInterface.GetBufferLookup<SubAreaNode>(ref __TypeHandle.__Game_Prefabs_SubAreaNode_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabPlaceholderElements = InternalCompilerInterface.GetBufferLookup<PlaceholderObjectElement>(ref __TypeHandle.__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPrefab = m_PrefabSystem.GetEntity(m_Prefab),
			m_WaterSurfaceData = m_WaterSystem.GetVelocitiesSurfaceData(out deps),
			m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(inputDeps, deps));
		m_WaterSystem.AddVelocitySurfaceReader(val);
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val);
		return val;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ObjectToolSystem()
	{
	}
}
