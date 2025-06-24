using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Zones;

public static class CellBlockJobs
{
	[BurstCompile]
	public struct BlockCellsJob : IJobParallelForDefer
	{
		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Entity m_BlockEntity;

			public Block m_BlockData;

			public ValidArea m_ValidAreaData;

			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public Quad2 m_IgnoreQuad;

			public Circle2 m_IgnoreCircle;

			public bool2 m_HasIgnore;

			public DynamicBuffer<Cell> m_Cells;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public ComponentLookup<RoadComposition> m_PrefabRoadCompositionData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0294: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_041f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0430: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_024c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0257: Unknown result type (might be due to invalid IL or missing references)
				//IL_0262: Unknown result type (might be due to invalid IL or missing references)
				//IL_0273: Unknown result type (might be due to invalid IL or missing references)
				//IL_027e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0289: Unknown result type (might be due to invalid IL or missing references)
				//IL_0239: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0446: Unknown result type (might be due to invalid IL or missing references)
				//IL_0461: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_017a: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_019a: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0474: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0409: Unknown result type (might be due to invalid IL or missing references)
				//IL_0414: Unknown result type (might be due to invalid IL or missing references)
				//IL_0316: Unknown result type (might be due to invalid IL or missing references)
				//IL_0326: Unknown result type (might be due to invalid IL or missing references)
				//IL_0331: Unknown result type (might be due to invalid IL or missing references)
				//IL_0346: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_0360: Unknown result type (might be due to invalid IL or missing references)
				//IL_0365: Unknown result type (might be due to invalid IL or missing references)
				//IL_036f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0374: Unknown result type (might be due to invalid IL or missing references)
				//IL_0379: Unknown result type (might be due to invalid IL or missing references)
				//IL_038a: Unknown result type (might be due to invalid IL or missing references)
				//IL_038f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0397: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03af: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_055c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0567: Unknown result type (might be due to invalid IL or missing references)
				//IL_0572: Unknown result type (might be due to invalid IL or missing references)
				//IL_0583: Unknown result type (might be due to invalid IL or missing references)
				//IL_0593: Unknown result type (might be due to invalid IL or missing references)
				//IL_059e: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0504: Unknown result type (might be due to invalid IL or missing references)
				//IL_0515: Unknown result type (might be due to invalid IL or missing references)
				//IL_051a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0522: Unknown result type (might be due to invalid IL or missing references)
				//IL_052d: Unknown result type (might be due to invalid IL or missing references)
				//IL_053a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0545: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || !m_EdgeGeometryData.HasComponent(edgeEntity))
				{
					return;
				}
				m_HasIgnore = bool2.op_Implicit(false);
				if (m_OwnerData.HasComponent(edgeEntity))
				{
					Owner owner = m_OwnerData[edgeEntity];
					if (m_TransformData.HasComponent(owner.m_Owner))
					{
						PrefabRef prefabRef = m_PrefabRefData[owner.m_Owner];
						if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
						{
							Transform transform = m_TransformData[owner.m_Owner];
							ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
							if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
							{
								float3 val = math.max(objectGeometryData.m_Size - 0.16f, float3.op_Implicit(0f));
								m_IgnoreCircle = new Circle2(val.x * 0.5f, ((float3)(ref transform.m_Position)).xz);
								m_HasIgnore.y = true;
							}
							else
							{
								Bounds3 val2 = MathUtils.Expand(objectGeometryData.m_Bounds, float3.op_Implicit(-0.08f));
								float3 val3 = MathUtils.Center(val2);
								bool3 val4 = val2.min > val2.max;
								val2.min = math.select(val2.min, val3, val4);
								val2.max = math.select(val2.max, val3, val4);
								Quad3 val5 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, val2);
								m_IgnoreQuad = ((Quad3)(ref val5)).xz;
								m_HasIgnore.x = true;
							}
						}
					}
				}
				Composition composition = m_CompositionData[edgeEntity];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[edgeEntity];
				StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[edgeEntity];
				EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[edgeEntity];
				if (MathUtils.Intersect(m_Bounds, ((Bounds3)(ref edgeGeometry.m_Bounds)).xz))
				{
					NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
					RoadComposition prefabRoadData = default(RoadComposition);
					if (m_PrefabRoadCompositionData.HasComponent(composition.m_Edge))
					{
						prefabRoadData = m_PrefabRoadCompositionData[composition.m_Edge];
					}
					CheckSegment(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, prefabCompositionData, prefabRoadData, new bool2(true, true));
					CheckSegment(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, prefabCompositionData, prefabRoadData, new bool2(true, true));
				}
				if (MathUtils.Intersect(m_Bounds, ((Bounds3)(ref startNodeGeometry.m_Geometry.m_Bounds)).xz))
				{
					NetCompositionData prefabCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
					RoadComposition prefabRoadData2 = default(RoadComposition);
					if (m_PrefabRoadCompositionData.HasComponent(composition.m_StartNode))
					{
						prefabRoadData2 = m_PrefabRoadCompositionData[composition.m_StartNode];
					}
					if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
					{
						CheckSegment(startNodeGeometry.m_Geometry.m_Left.m_Left, startNodeGeometry.m_Geometry.m_Left.m_Right, prefabCompositionData2, prefabRoadData2, new bool2(true, true));
						Bezier4x3 val6 = MathUtils.Lerp(startNodeGeometry.m_Geometry.m_Right.m_Left, startNodeGeometry.m_Geometry.m_Right.m_Right, 0.5f);
						val6.d = startNodeGeometry.m_Geometry.m_Middle.d;
						CheckSegment(startNodeGeometry.m_Geometry.m_Right.m_Left, val6, prefabCompositionData2, prefabRoadData2, new bool2(true, false));
						CheckSegment(val6, startNodeGeometry.m_Geometry.m_Right.m_Right, prefabCompositionData2, prefabRoadData2, new bool2(false, true));
					}
					else
					{
						CheckSegment(startNodeGeometry.m_Geometry.m_Left.m_Left, startNodeGeometry.m_Geometry.m_Middle, prefabCompositionData2, prefabRoadData2, new bool2(true, false));
						CheckSegment(startNodeGeometry.m_Geometry.m_Middle, startNodeGeometry.m_Geometry.m_Right.m_Right, prefabCompositionData2, prefabRoadData2, new bool2(false, true));
					}
				}
				if (MathUtils.Intersect(m_Bounds, ((Bounds3)(ref endNodeGeometry.m_Geometry.m_Bounds)).xz))
				{
					NetCompositionData prefabCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
					RoadComposition prefabRoadData3 = default(RoadComposition);
					if (m_PrefabRoadCompositionData.HasComponent(composition.m_EndNode))
					{
						prefabRoadData3 = m_PrefabRoadCompositionData[composition.m_EndNode];
					}
					if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
					{
						CheckSegment(endNodeGeometry.m_Geometry.m_Left.m_Left, endNodeGeometry.m_Geometry.m_Left.m_Right, prefabCompositionData3, prefabRoadData3, new bool2(true, true));
						Bezier4x3 val7 = MathUtils.Lerp(endNodeGeometry.m_Geometry.m_Right.m_Left, endNodeGeometry.m_Geometry.m_Right.m_Right, 0.5f);
						val7.d = endNodeGeometry.m_Geometry.m_Middle.d;
						CheckSegment(endNodeGeometry.m_Geometry.m_Right.m_Left, val7, prefabCompositionData3, prefabRoadData3, new bool2(true, false));
						CheckSegment(val7, endNodeGeometry.m_Geometry.m_Right.m_Right, prefabCompositionData3, prefabRoadData3, new bool2(false, true));
					}
					else
					{
						CheckSegment(endNodeGeometry.m_Geometry.m_Left.m_Left, endNodeGeometry.m_Geometry.m_Middle, prefabCompositionData3, prefabRoadData3, new bool2(true, false));
						CheckSegment(endNodeGeometry.m_Geometry.m_Middle, endNodeGeometry.m_Geometry.m_Right.m_Right, prefabCompositionData3, prefabRoadData3, new bool2(false, true));
					}
				}
			}

			private void CheckSegment(Bezier4x3 left, Bezier4x3 right, NetCompositionData prefabCompositionData, RoadComposition prefabRoadData, bool2 isEdge)
			{
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0269: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0276: Unknown result type (might be due to invalid IL or missing references)
				//IL_0277: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0281: Unknown result type (might be due to invalid IL or missing references)
				//IL_0283: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_0246: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_024e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0253: Unknown result type (might be due to invalid IL or missing references)
				//IL_025a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_021c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0222: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_0229: Unknown result type (might be due to invalid IL or missing references)
				if ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0 || (prefabCompositionData.m_State & CompositionState.BlockZone) == 0)
				{
					return;
				}
				bool flag = (prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Elevated) != 0;
				flag |= (prefabCompositionData.m_State & CompositionState.ExclusiveGround) == 0;
				Bounds3 val = MathUtils.Bounds(left) | MathUtils.Bounds(right);
				if (!MathUtils.Intersect(((Bounds3)(ref val)).xz, m_Bounds))
				{
					return;
				}
				isEdge &= ((prefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0) & ((prefabCompositionData.m_Flags.m_General & CompositionFlags.General.Elevated) == 0);
				isEdge &= new bool2((prefabCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0, (prefabCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0);
				Quad3 val2 = default(Quad3);
				val2.a = left.a;
				val2.b = right.a;
				Bounds3 val3 = SetHeightRange(MathUtils.Bounds(val2.a, val2.b), prefabCompositionData.m_HeightRange);
				for (int i = 1; i <= 8; i++)
				{
					float num = (float)i / 8f;
					val2.d = MathUtils.Position(left, num);
					val2.c = MathUtils.Position(right, num);
					Bounds3 val4 = SetHeightRange(MathUtils.Bounds(val2.d, val2.c), prefabCompositionData.m_HeightRange);
					Bounds3 bounds = val3 | val4;
					if (MathUtils.Intersect(((Bounds3)(ref bounds)).xz, m_Bounds) && MathUtils.Intersect(m_Quad, ((Quad3)(ref val2)).xz))
					{
						CellFlags cellFlags = CellFlags.Blocked;
						if (isEdge.x)
						{
							Block source = new Block
							{
								m_Direction = math.normalizesafe(MathUtils.Right(((float3)(ref val2.d)).xz - ((float3)(ref val2.a)).xz), default(float2))
							};
							cellFlags |= ZoneUtils.GetRoadDirection(m_BlockData, source);
						}
						if (isEdge.y)
						{
							Block source2 = new Block
							{
								m_Direction = math.normalizesafe(MathUtils.Left(((float3)(ref val2.c)).xz - ((float3)(ref val2.b)).xz), default(float2))
							};
							cellFlags |= ZoneUtils.GetRoadDirection(m_BlockData, source2);
						}
						CheckOverlapX(m_Bounds, bounds, m_Quad, val2, m_ValidAreaData.m_Area, cellFlags, flag);
					}
					val2.a = val2.d;
					val2.b = val2.c;
					val3 = val4;
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

			private void CheckOverlapX(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Quad3 quad2, int4 xxzz1, CellFlags flags, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz2.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val4, bounds2, val2, quad2, val, flags, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapZ(val5, bounds2, val3, quad2, xxzz2, flags, isElevated);
					}
				}
				else
				{
					CheckOverlapZ(bounds1, bounds2, quad1, quad2, xxzz1, flags, isElevated);
				}
			}

			private void CheckOverlapZ(Bounds2 bounds1, Bounds3 bounds2, Quad2 quad1, Quad3 quad2, int4 xxzz1, CellFlags flags, bool isElevated)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz2.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val4, bounds2, val2, quad2, val, flags, isElevated);
					}
					if (MathUtils.Intersect(val5, ((Bounds3)(ref bounds2)).xz))
					{
						CheckOverlapX(val5, bounds2, val3, quad2, xxzz2, flags, isElevated);
					}
					return;
				}
				if (xxzz1.y - xxzz1.x >= 2)
				{
					CheckOverlapX(bounds1, bounds2, quad1, quad2, xxzz1, flags, isElevated);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				Cell cell = m_Cells[num2];
				if ((cell.m_State & flags) == flags)
				{
					return;
				}
				quad1 = MathUtils.Expand(quad1, -0.0625f);
				if (MathUtils.Intersect(quad1, ((Quad3)(ref quad2)).xz) && (!math.any(m_HasIgnore) || ((!m_HasIgnore.x || !MathUtils.Intersect(quad1, m_IgnoreQuad)) && (!m_HasIgnore.y || !MathUtils.Intersect(quad1, m_IgnoreCircle)))))
				{
					if (isElevated)
					{
						cell.m_Height = (short)math.clamp(Mathf.FloorToInt(bounds2.min.y), -32768, math.min((int)cell.m_Height, 32767));
					}
					else
					{
						cell.m_State |= flags;
					}
					m_Cells[num2] = cell;
				}
			}
		}

		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Entity m_BlockEntity;

			public Block m_BlockData;

			public ValidArea m_ValidAreaData;

			public Bounds2 m_Bounds;

			public Quad2 m_Quad;

			public DynamicBuffer<Cell> m_Cells;

			public ComponentLookup<Native> m_NativeData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

			public BufferLookup<Game.Areas.Node> m_AreaNodes;

			public BufferLookup<Triangle> m_AreaTriangles;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[areaItem.m_Area];
				AreaGeometryData areaGeometryData = m_PrefabAreaGeometryData[prefabRef.m_Prefab];
				if ((areaGeometryData.m_Flags & (Game.Areas.GeometryFlags.PhysicalGeometry | Game.Areas.GeometryFlags.ProtectedArea)) != 0 && ((areaGeometryData.m_Flags & Game.Areas.GeometryFlags.ProtectedArea) == 0 || m_NativeData.HasComponent(areaItem.m_Area)))
				{
					DynamicBuffer<Game.Areas.Node> nodes = m_AreaNodes[areaItem.m_Area];
					DynamicBuffer<Triangle> val = m_AreaTriangles[areaItem.m_Area];
					if (val.Length > areaItem.m_Triangle)
					{
						Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val[areaItem.m_Triangle]);
						CheckOverlapX(m_Bounds, ((Bounds3)(ref bounds.m_Bounds)).xz, m_Quad, ((Triangle3)(ref triangle)).xz, m_ValidAreaData.m_Area);
					}
				}
			}

			private void CheckOverlapX(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Triangle2 triangle2, int4 xxzz1)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.y - xxzz1.x >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.y = xxzz1.x + xxzz1.y >> 1;
					xxzz2.x = val.y;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.y - xxzz1.x) / (float)(xxzz1.y - xxzz1.x);
					val2.b = math.lerp(quad1.a, quad1.b, num);
					val2.c = math.lerp(quad1.d, quad1.c, num);
					val3.a = val2.b;
					val3.d = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapZ(val4, bounds2, val2, triangle2, val);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapZ(val5, bounds2, val3, triangle2, xxzz2);
					}
				}
				else
				{
					CheckOverlapZ(bounds1, bounds2, quad1, triangle2, xxzz1);
				}
			}

			private void CheckOverlapZ(Bounds2 bounds1, Bounds2 bounds2, Quad2 quad1, Triangle2 triangle2, int4 xxzz1)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				if (xxzz1.w - xxzz1.z >= 2)
				{
					int4 val = xxzz1;
					int4 xxzz2 = xxzz1;
					val.w = xxzz1.z + xxzz1.w >> 1;
					xxzz2.z = val.w;
					Quad2 val2 = quad1;
					Quad2 val3 = quad1;
					float num = (float)(val.w - xxzz1.z) / (float)(xxzz1.w - xxzz1.z);
					val2.d = math.lerp(quad1.a, quad1.d, num);
					val2.c = math.lerp(quad1.b, quad1.c, num);
					val3.a = val2.d;
					val3.b = val2.c;
					Bounds2 val4 = MathUtils.Bounds(val2);
					Bounds2 val5 = MathUtils.Bounds(val3);
					if (MathUtils.Intersect(val4, bounds2))
					{
						CheckOverlapX(val4, bounds2, val2, triangle2, val);
					}
					if (MathUtils.Intersect(val5, bounds2))
					{
						CheckOverlapX(val5, bounds2, val3, triangle2, xxzz2);
					}
					return;
				}
				if (xxzz1.y - xxzz1.x >= 2)
				{
					CheckOverlapX(bounds1, bounds2, quad1, triangle2, xxzz1);
					return;
				}
				int num2 = xxzz1.z * m_BlockData.m_Size.x + xxzz1.x;
				Cell cell = m_Cells[num2];
				if ((cell.m_State & CellFlags.Blocked) == 0)
				{
					quad1 = MathUtils.Expand(quad1, -0.02f);
					if (MathUtils.Intersect(quad1, triangle2))
					{
						cell.m_State |= CellFlags.Blocked;
						m_Cells[num2] = cell;
					}
				}
			}
		}

		[ReadOnly]
		public NativeArray<CellCheckHelpers.SortedEntity> m_Blocks;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_PrefabRoadCompositionData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<Native> m_NativeData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_AreaTriangles;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Cell> m_Cells;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<ValidArea> m_ValidAreaData;

		public void Execute(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			Entity entity = m_Blocks[index].m_Entity;
			Block block = m_BlockData[entity];
			DynamicBuffer<Cell> cells = m_Cells[entity];
			ValidArea validAreaData = new ValidArea
			{
				m_Area = new int4(0, block.m_Size.x, 0, block.m_Size.y)
			};
			Bounds2 bounds = ZoneUtils.CalculateBounds(block);
			Quad2 quad = ZoneUtils.CalculateCorners(block);
			ClearBlockStatus(block, cells);
			NetIterator netIterator = new NetIterator
			{
				m_BlockEntity = entity,
				m_BlockData = block,
				m_Bounds = bounds,
				m_Quad = quad,
				m_ValidAreaData = validAreaData,
				m_Cells = cells,
				m_OwnerData = m_OwnerData,
				m_TransformData = m_TransformData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_StartNodeGeometryData = m_StartNodeGeometryData,
				m_EndNodeGeometryData = m_EndNodeGeometryData,
				m_CompositionData = m_CompositionData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabCompositionData = m_PrefabCompositionData,
				m_PrefabRoadCompositionData = m_PrefabRoadCompositionData,
				m_PrefabObjectGeometryData = m_PrefabObjectGeometryData
			};
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			AreaIterator areaIterator = new AreaIterator
			{
				m_BlockEntity = entity,
				m_BlockData = block,
				m_Bounds = bounds,
				m_Quad = quad,
				m_ValidAreaData = validAreaData,
				m_Cells = cells,
				m_NativeData = m_NativeData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabAreaGeometryData = m_PrefabAreaGeometryData,
				m_AreaNodes = m_AreaNodes,
				m_AreaTriangles = m_AreaTriangles
			};
			m_AreaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
			CleanBlockedCells(block, ref validAreaData, cells);
			m_ValidAreaData[entity] = validAreaData;
		}

		private static void ClearBlockStatus(Block blockData, DynamicBuffer<Cell> cells)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < blockData.m_Size.x; i++)
			{
				Cell cell = cells[i];
				if ((cell.m_State & (CellFlags.Blocked | CellFlags.Shared | CellFlags.Roadside | CellFlags.Occupied | CellFlags.Updating | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) != (CellFlags.Roadside | CellFlags.Updating))
				{
					cell.m_State = (cell.m_State & ~(CellFlags.Blocked | CellFlags.Shared | CellFlags.Roadside | CellFlags.Occupied | CellFlags.Updating | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) | (CellFlags.Roadside | CellFlags.Updating);
					cell.m_Height = short.MaxValue;
					cells[i] = cell;
				}
			}
			for (int j = 1; j < blockData.m_Size.y; j++)
			{
				for (int k = 0; k < blockData.m_Size.x; k++)
				{
					int num = j * blockData.m_Size.x + k;
					Cell cell2 = cells[num];
					if ((cell2.m_State & (CellFlags.Blocked | CellFlags.Shared | CellFlags.Roadside | CellFlags.Occupied | CellFlags.Updating | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) != CellFlags.Updating)
					{
						cell2.m_State = (cell2.m_State & ~(CellFlags.Blocked | CellFlags.Shared | CellFlags.Roadside | CellFlags.Occupied | CellFlags.Updating | CellFlags.RoadLeft | CellFlags.RoadRight | CellFlags.RoadBack)) | CellFlags.Updating;
						cell2.m_Height = short.MaxValue;
						cells[num] = cell2;
					}
				}
			}
		}

		private static void CleanBlockedCells(Block blockData, ref ValidArea validAreaData, DynamicBuffer<Cell> cells)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			ValidArea validArea = default(ValidArea);
			((int4)(ref validArea.m_Area)).xz = blockData.m_Size;
			for (int i = validAreaData.m_Area.x; i < validAreaData.m_Area.y; i++)
			{
				Cell cell = cells[i];
				Cell cell2 = cells[blockData.m_Size.x + i];
				if (((cell.m_State & CellFlags.Blocked) == 0) & ((cell2.m_State & CellFlags.Blocked) != 0))
				{
					cell.m_State |= CellFlags.Blocked;
					cells[i] = cell;
				}
				int num = 0;
				for (int j = validAreaData.m_Area.z + 1; j < validAreaData.m_Area.w; j++)
				{
					int num2 = j * blockData.m_Size.x + i;
					Cell cell3 = cells[num2];
					if (((cell3.m_State & CellFlags.Blocked) == 0) & ((cell.m_State & CellFlags.Blocked) != 0))
					{
						cell3.m_State |= CellFlags.Blocked;
						cells[num2] = cell3;
					}
					if ((cell3.m_State & CellFlags.Blocked) == 0)
					{
						num = j + 1;
					}
					cell = cell3;
				}
				if (num > validAreaData.m_Area.z)
				{
					((int4)(ref validArea.m_Area)).xz = math.min(((int4)(ref validArea.m_Area)).xz, new int2(i, validAreaData.m_Area.z));
					((int4)(ref validArea.m_Area)).yw = math.max(((int4)(ref validArea.m_Area)).yw, new int2(i + 1, num));
				}
			}
			validAreaData = validArea;
			for (int k = validAreaData.m_Area.z; k < validAreaData.m_Area.w; k++)
			{
				for (int l = validAreaData.m_Area.x; l < validAreaData.m_Area.y; l++)
				{
					int num3 = k * blockData.m_Size.x + l;
					Cell cell4 = cells[num3];
					if ((cell4.m_State & (CellFlags.Blocked | CellFlags.RoadLeft)) == 0 && l > 0 && (cells[num3 - 1].m_State & (CellFlags.Blocked | CellFlags.RoadLeft)) == (CellFlags.Blocked | CellFlags.RoadLeft))
					{
						cell4.m_State |= CellFlags.RoadLeft;
						cells[num3] = cell4;
					}
					if ((cell4.m_State & (CellFlags.Blocked | CellFlags.RoadRight)) == 0 && l < blockData.m_Size.x - 1 && (cells[num3 + 1].m_State & (CellFlags.Blocked | CellFlags.RoadRight)) == (CellFlags.Blocked | CellFlags.RoadRight))
					{
						cell4.m_State |= CellFlags.RoadRight;
						cells[num3] = cell4;
					}
				}
			}
		}
	}
}
