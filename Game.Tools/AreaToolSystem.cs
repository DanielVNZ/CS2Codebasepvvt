using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.Common;
using Game.Input;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class AreaToolSystem : ToolBaseSystem
{
	public enum Mode
	{
		Edit,
		Generate
	}

	public enum State
	{
		Default,
		Create,
		Modify,
		Remove
	}

	public enum Tooltip
	{
		None,
		CreateArea,
		ModifyNode,
		ModifyEdge,
		CreateAreaOrModifyNode,
		CreateAreaOrModifyEdge,
		AddNode,
		InsertNode,
		MoveNode,
		MergeNodes,
		CompleteArea,
		DeleteArea,
		RemoveNode,
		GenerateAreas
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		private struct ParentObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Segment m_Line;

			public float m_BoundsOffset;

			public float m_MaxDistance;

			public Entity m_Parent;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				return MathUtils.Intersect(MathUtils.Expand(bounds.m_Bounds, float3.op_Implicit(m_BoundsOffset)), m_Line, ref val);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_0199: Unknown result type (might be due to invalid IL or missing references)
				//IL_019a: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				float2 val = default(float2);
				if (!MathUtils.Intersect(MathUtils.Expand(bounds.m_Bounds, float3.op_Implicit(m_BoundsOffset)), m_Line, ref val) || !m_TransformData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				Transform transform = m_TransformData[entity];
				if (!m_ObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				if (m_BuildingData.HasComponent(prefabRef.m_Prefab))
				{
					float2 val2 = float2.op_Implicit(m_BuildingData[prefabRef.m_Prefab].m_LotSize);
					((float3)(ref objectGeometryData.m_Bounds.min)).xz = val2 * -4f - m_MaxDistance;
					((float3)(ref objectGeometryData.m_Bounds.max)).xz = val2 * 4f + m_MaxDistance;
				}
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					float num = math.max(math.cmax(((float3)(ref objectGeometryData.m_Bounds.max)).xz), 0f - math.cmin(((float3)(ref objectGeometryData.m_Bounds.max)).xz));
					float num2 = default(float);
					if (MathUtils.Distance(((Segment)(ref m_Line)).xz, ((float3)(ref transform.m_Position)).xz, ref num2) < num + m_MaxDistance)
					{
						m_Parent = entity;
					}
				}
				else
				{
					Quad3 val3 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
					if (MathUtils.Intersect(((Quad3)(ref val3)).xz, ((Segment)(ref m_Line)).xz, ref val))
					{
						m_Parent = entity;
					}
				}
			}
		}

		private struct AreaIterator2 : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public bool m_EditorMode;

			public Game.Areas.AreaType m_AreaType;

			public Bounds3 m_Bounds;

			public float m_MaxDistance1;

			public float m_MaxDistance2;

			public ControlPoint m_ControlPoint1;

			public ControlPoint m_ControlPoint2;

			public NativeParallelHashSet<Entity> m_IgnoreAreas;

			public NativeList<ControlPoint> m_ControlPoints;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

			public ComponentLookup<Game.Areas.Lot> m_LotData;

			public ComponentLookup<Owner> m_OwnerData;

			public BufferLookup<Game.Areas.Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0198: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				Owner owner = default(Owner);
				DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || (m_IgnoreAreas.IsCreated && m_IgnoreAreas.Contains(areaItem.m_Area)) || (m_OwnerData.TryGetComponent(areaItem.m_Area, ref owner) && (m_Nodes.HasBuffer(owner.m_Owner) || (m_EditorMode && m_InstalledUpgrades.TryGetBuffer(owner.m_Owner, ref val) && val.Length != 0))))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[areaItem.m_Area];
				AreaGeometryData areaGeometryData = m_PrefabAreaData[prefabRef.m_Prefab];
				if (areaGeometryData.m_Type != m_AreaType)
				{
					return;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_Nodes[areaItem.m_Area];
				Triangle triangle = m_Triangles[areaItem.m_Area][areaItem.m_Triangle];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				int3 val2 = math.abs(triangle.m_Indices - ((int3)(ref triangle.m_Indices)).yzx);
				bool3 val3 = (val2 == 1) | (val2 == nodes.Length - 1);
				if (math.any(val3))
				{
					bool lockFirstEdge = !m_EditorMode && m_LotData.HasComponent(areaItem.m_Area);
					if (val3.x)
					{
						CheckLine(((Triangle3)(ref triangle2)).ab, areaGeometryData.m_SnapDistance, areaItem.m_Area, ((int3)(ref triangle.m_Indices)).xy, lockFirstEdge);
					}
					if (val3.y)
					{
						CheckLine(((Triangle3)(ref triangle2)).bc, areaGeometryData.m_SnapDistance, areaItem.m_Area, ((int3)(ref triangle.m_Indices)).yz, lockFirstEdge);
					}
					if (val3.z)
					{
						CheckLine(((Triangle3)(ref triangle2)).ca, areaGeometryData.m_SnapDistance, areaItem.m_Area, ((int3)(ref triangle.m_Indices)).zx, lockFirstEdge);
					}
				}
			}

			public void CheckLine(Segment line, float snapDistance, Entity area, int2 nodeIndex, bool lockFirstEdge)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0004: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				if (lockFirstEdge && math.cmin(nodeIndex) == 0 && math.cmax(nodeIndex) == 1)
				{
					return;
				}
				float num2 = default(float);
				float num = MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref m_ControlPoint1.m_Position)).xz, ref num2);
				float num4 = default(float);
				float num3 = MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref m_ControlPoint2.m_HitPosition)).xz, ref num4);
				if (!(num < m_MaxDistance1) || !(num3 < m_MaxDistance2))
				{
					return;
				}
				float num5 = math.distance(((float3)(ref line.a)).xz, ((float3)(ref m_ControlPoint2.m_HitPosition)).xz);
				float num6 = math.distance(((float3)(ref line.b)).xz, ((float3)(ref m_ControlPoint2.m_HitPosition)).xz);
				ControlPoint controlPoint = m_ControlPoint1;
				controlPoint.m_OriginalEntity = area;
				if (num5 <= snapDistance && num5 <= num6 && (!lockFirstEdge || nodeIndex.x >= 2))
				{
					controlPoint.m_ElementIndex = new int2(nodeIndex.x, -1);
				}
				else if (num6 <= snapDistance && (!lockFirstEdge || nodeIndex.y >= 2))
				{
					controlPoint.m_ElementIndex = new int2(nodeIndex.y, -1);
				}
				else
				{
					controlPoint.m_ElementIndex = new int2(-1, math.select(math.cmax(nodeIndex), math.cmin(nodeIndex), math.abs(nodeIndex.y - nodeIndex.x) == 1));
				}
				for (int i = 0; i < m_ControlPoints.Length; i++)
				{
					if (m_ControlPoints[i].m_OriginalEntity == area)
					{
						return;
					}
				}
				m_ControlPoints.Add(ref controlPoint);
			}
		}

		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public bool m_EditorMode;

			public bool m_IgnoreStartPositions;

			public Snap m_Snap;

			public Game.Areas.AreaType m_AreaType;

			public Bounds3 m_Bounds;

			public float m_MaxDistance;

			public NativeParallelHashSet<Entity> m_IgnoreAreas;

			public Entity m_PreferArea;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public NativeList<ControlPoint> m_MoveStartPositions;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

			public ComponentLookup<Game.Areas.Lot> m_LotData;

			public ComponentLookup<Owner> m_OwnerData;

			public BufferLookup<Game.Areas.Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_020a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0211: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_021d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_023b: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0301: Unknown result type (might be due to invalid IL or missing references)
				//IL_0308: Unknown result type (might be due to invalid IL or missing references)
				//IL_0310: Unknown result type (might be due to invalid IL or missing references)
				//IL_0247: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0255: Unknown result type (might be due to invalid IL or missing references)
				//IL_0257: Unknown result type (might be due to invalid IL or missing references)
				//IL_025c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0261: Unknown result type (might be due to invalid IL or missing references)
				//IL_0263: Unknown result type (might be due to invalid IL or missing references)
				//IL_0267: Unknown result type (might be due to invalid IL or missing references)
				//IL_0271: Unknown result type (might be due to invalid IL or missing references)
				//IL_0273: Unknown result type (might be due to invalid IL or missing references)
				//IL_0278: Unknown result type (might be due to invalid IL or missing references)
				//IL_027d: Unknown result type (might be due to invalid IL or missing references)
				//IL_027f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0283: Unknown result type (might be due to invalid IL or missing references)
				//IL_028d: Unknown result type (might be due to invalid IL or missing references)
				//IL_028f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0294: Unknown result type (might be due to invalid IL or missing references)
				//IL_0299: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0343: Unknown result type (might be due to invalid IL or missing references)
				//IL_0328: Unknown result type (might be due to invalid IL or missing references)
				//IL_032f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0337: Unknown result type (might be due to invalid IL or missing references)
				//IL_034f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_035e: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || (m_IgnoreAreas.IsCreated && m_IgnoreAreas.Contains(areaItem.m_Area)))
				{
					return;
				}
				Entity area = areaItem.m_Area;
				if (areaItem.m_Area != m_PreferArea)
				{
					if ((m_Snap & Snap.ExistingGeometry) == 0)
					{
						bool flag = false;
						if (m_IgnoreStartPositions)
						{
							for (int i = 0; i < m_MoveStartPositions.Length; i++)
							{
								flag |= m_MoveStartPositions[i].m_OriginalEntity == areaItem.m_Area;
							}
						}
						if (!flag)
						{
							return;
						}
					}
					Owner owner = default(Owner);
					if (m_OwnerData.TryGetComponent(areaItem.m_Area, ref owner))
					{
						if (m_Nodes.HasBuffer(owner.m_Owner))
						{
							return;
						}
						DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
						if (m_EditorMode && m_InstalledUpgrades.TryGetBuffer(owner.m_Owner, ref val) && val.Length != 0)
						{
							area = Entity.Null;
						}
					}
				}
				PrefabRef prefabRef = m_PrefabRefData[areaItem.m_Area];
				AreaGeometryData areaGeometryData = m_PrefabAreaData[prefabRef.m_Prefab];
				if (areaGeometryData.m_Type != m_AreaType)
				{
					return;
				}
				DynamicBuffer<Game.Areas.Node> nodes = m_Nodes[areaItem.m_Area];
				Triangle triangle = m_Triangles[areaItem.m_Area][areaItem.m_Triangle];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				int3 val2 = math.abs(triangle.m_Indices - ((int3)(ref triangle.m_Indices)).yzx);
				bool3 val3 = val2 == nodes.Length - 1;
				bool3 val4 = (val2 == 1) | val3;
				if (!math.any(val4))
				{
					return;
				}
				if (m_IgnoreStartPositions)
				{
					bool3 val5 = ((int3)(ref triangle.m_Indices)).yzx < triangle.m_Indices != val3;
					int3 val6 = math.select(triangle.m_Indices, ((int3)(ref triangle.m_Indices)).yzx, val5);
					int3 val7 = math.select(((int3)(ref triangle.m_Indices)).yzx, triangle.m_Indices, val5);
					for (int j = 0; j < m_MoveStartPositions.Length; j++)
					{
						ControlPoint controlPoint = m_MoveStartPositions[j];
						if (!(controlPoint.m_OriginalEntity != areaItem.m_Area))
						{
							val4 &= controlPoint.m_ElementIndex.x != val6;
							val4 &= controlPoint.m_ElementIndex.x != val7;
							val4 &= controlPoint.m_ElementIndex.y != val6;
						}
					}
				}
				bool lockFirstEdge = !m_EditorMode && m_LotData.HasComponent(areaItem.m_Area);
				float snapDistance = math.select(areaGeometryData.m_SnapDistance, areaGeometryData.m_SnapDistance * 0.5f, (m_Snap & Snap.ExistingGeometry) == 0);
				if (val4.x)
				{
					CheckLine(((Triangle3)(ref triangle2)).ab, snapDistance, area, ((int3)(ref triangle.m_Indices)).xy, lockFirstEdge);
				}
				if (val4.y)
				{
					CheckLine(((Triangle3)(ref triangle2)).bc, snapDistance, area, ((int3)(ref triangle.m_Indices)).yz, lockFirstEdge);
				}
				if (val4.z)
				{
					CheckLine(((Triangle3)(ref triangle2)).ca, snapDistance, area, ((int3)(ref triangle.m_Indices)).zx, lockFirstEdge);
				}
			}

			public void CheckLine(Segment line, float snapDistance, Entity area, int2 nodeIndex, bool lockFirstEdge)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0004: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0249: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0250: Unknown result type (might be due to invalid IL or missing references)
				//IL_0258: Unknown result type (might be due to invalid IL or missing references)
				//IL_025f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0266: Unknown result type (might be due to invalid IL or missing references)
				//IL_026d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0282: Unknown result type (might be due to invalid IL or missing references)
				//IL_0287: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02da: Unknown result type (might be due to invalid IL or missing references)
				//IL_02df: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0152: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0190: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_0196: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0204: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_0222: Unknown result type (might be due to invalid IL or missing references)
				//IL_0229: Unknown result type (might be due to invalid IL or missing references)
				//IL_022a: Unknown result type (might be due to invalid IL or missing references)
				//IL_022f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0230: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				float num = default(float);
				if ((!lockFirstEdge || math.cmin(nodeIndex) != 0 || math.cmax(nodeIndex) != 1) && MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num) < m_MaxDistance)
				{
					float heightWeight = math.select(0f, 1f, m_AreaType == Game.Areas.AreaType.Space);
					float level = math.select(2f, 3f, area == m_PreferArea);
					float num2 = math.distance(((float3)(ref line.a)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz);
					float num3 = math.distance(((float3)(ref line.b)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz);
					ControlPoint controlPoint = m_ControlPoint;
					controlPoint.m_OriginalEntity = area;
					controlPoint.m_Direction = ((float3)(ref line.b)).xz - ((float3)(ref line.a)).xz;
					MathUtils.TryNormalize(ref controlPoint.m_Direction);
					if (num2 <= snapDistance && num2 <= num3 && (!lockFirstEdge || nodeIndex.x >= 2))
					{
						controlPoint.m_Position = line.a;
						controlPoint.m_ElementIndex = new int2(nodeIndex.x, -1);
						controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(line.a, line.b), (SnapLineFlags)0, heightWeight));
					}
					else if (num3 <= snapDistance && (!lockFirstEdge || nodeIndex.y >= 2))
					{
						controlPoint.m_Position = line.b;
						controlPoint.m_ElementIndex = new int2(nodeIndex.y, -1);
						controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(line.a, line.b), (SnapLineFlags)0, heightWeight));
					}
					else
					{
						controlPoint.m_Position = MathUtils.Position(line, num);
						controlPoint.m_ElementIndex = new int2(-1, math.select(math.cmax(nodeIndex), math.cmin(nodeIndex), math.abs(nodeIndex.y - nodeIndex.x) == 1));
						controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(line.a, line.b), (SnapLineFlags)0, heightWeight));
					}
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Snap m_Snap;

			public Bounds3 m_Bounds;

			public float m_MaxDistance;

			public Game.Areas.AreaType m_AreaType;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<StartNodeGeometry> m_StartGeometryData;

			public ComponentLookup<EndNodeGeometry> m_EndGeometryData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0253: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_025d: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_026f: Unknown result type (might be due to invalid IL or missing references)
				//IL_027a: Unknown result type (might be due to invalid IL or missing references)
				//IL_018b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0207: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
				{
					return;
				}
				Composition composition = default(Composition);
				if (m_CompositionData.HasComponent(entity))
				{
					composition = m_CompositionData[entity];
				}
				if ((m_Snap & Snap.NetSide) != Snap.None)
				{
					if (m_EdgeGeometryData.HasComponent(entity) && CheckComposition(composition.m_Edge))
					{
						EdgeGeometry edgeGeometry = m_EdgeGeometryData[entity];
						SnapEdgeCurve(edgeGeometry.m_Start.m_Left);
						SnapEdgeCurve(edgeGeometry.m_Start.m_Right);
						SnapEdgeCurve(edgeGeometry.m_End.m_Left);
						SnapEdgeCurve(edgeGeometry.m_End.m_Right);
					}
					if (m_StartGeometryData.HasComponent(entity) && CheckComposition(composition.m_StartNode))
					{
						StartNodeGeometry startNodeGeometry = m_StartGeometryData[entity];
						if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
						{
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Left.m_Left);
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Left.m_Right);
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Right.m_Left);
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Right.m_Right);
						}
						else
						{
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Left.m_Left);
							SnapNodeCurve(startNodeGeometry.m_Geometry.m_Right.m_Right);
						}
					}
					if (m_EndGeometryData.HasComponent(entity) && CheckComposition(composition.m_EndNode))
					{
						EndNodeGeometry endNodeGeometry = m_EndGeometryData[entity];
						if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
						{
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Left.m_Left);
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Left.m_Right);
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Right.m_Left);
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Right.m_Right);
						}
						else
						{
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Left.m_Left);
							SnapNodeCurve(endNodeGeometry.m_Geometry.m_Right.m_Right);
						}
					}
				}
				if ((m_Snap & Snap.NetMiddle) != Snap.None && m_CurveData.HasComponent(entity) && CheckComposition(composition.m_Edge))
				{
					SnapEdgeCurve(m_CurveData[entity].m_Bezier);
				}
			}

			private bool CheckComposition(Entity composition)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				NetCompositionData netCompositionData = default(NetCompositionData);
				if (m_PrefabCompositionData.TryGetComponent(composition, ref netCompositionData) && (netCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
				{
					return false;
				}
				return true;
			}

			private void SnapEdgeCurve(Bezier4x3 curve)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(m_Bounds, MathUtils.Bounds(curve)))
				{
					float heightWeight = math.select(0f, 1f, m_AreaType == Game.Areas.AreaType.Space);
					float num = default(float);
					if (MathUtils.Distance(((Bezier4x3)(ref curve)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num) < m_MaxDistance)
					{
						ControlPoint controlPoint = m_ControlPoint;
						controlPoint.m_OriginalEntity = Entity.Null;
						controlPoint.m_Position = MathUtils.Position(curve, num);
						float3 val = MathUtils.Tangent(curve, num);
						controlPoint.m_Direction = ((float3)(ref val)).xz;
						MathUtils.TryNormalize(ref controlPoint.m_Direction);
						controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, curve, (SnapLineFlags)0, heightWeight));
					}
				}
			}

			private void SnapNodeCurve(Bezier4x3 curve)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_013f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				float3 val = MathUtils.StartTangent(curve);
				val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
				val.y = math.clamp(val.y, -1f, 1f);
				Segment val2 = default(Segment);
				((Segment)(ref val2))._002Ector(curve.a, curve.a + val * math.dot(curve.d - curve.a, val));
				if (MathUtils.Intersect(m_Bounds, MathUtils.Bounds(val2)))
				{
					float heightWeight = math.select(0f, 1f, m_AreaType == Game.Areas.AreaType.Space);
					float num = default(float);
					if (MathUtils.Distance(((Segment)(ref val2)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num) < m_MaxDistance)
					{
						ControlPoint controlPoint = m_ControlPoint;
						controlPoint.m_OriginalEntity = Entity.Null;
						controlPoint.m_Direction = ((float3)(ref val)).xz;
						controlPoint.m_Position = MathUtils.Position(val2, num);
						controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val2.a, val2.b), (SnapLineFlags)0, heightWeight));
					}
				}
			}
		}

		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public float m_MaxDistance;

			public Game.Areas.AreaType m_AreaType;

			public Snap m_Snap;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

			public ComponentLookup<AssetStampData> m_AssetStampData;

			public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_045b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0206: Unknown result type (might be due to invalid IL or missing references)
				//IL_0207: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0213: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_04da: Unknown result type (might be due to invalid IL or missing references)
				//IL_04df: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0503: Unknown result type (might be due to invalid IL or missing references)
				//IL_0510: Unknown result type (might be due to invalid IL or missing references)
				//IL_0515: Unknown result type (might be due to invalid IL or missing references)
				//IL_046e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0478: Unknown result type (might be due to invalid IL or missing references)
				//IL_047d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0482: Unknown result type (might be due to invalid IL or missing references)
				//IL_0490: Unknown result type (might be due to invalid IL or missing references)
				//IL_0497: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_024d: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0254: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_026a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_022d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				//IL_0281: Unknown result type (might be due to invalid IL or missing references)
				//IL_0286: Unknown result type (might be due to invalid IL or missing references)
				//IL_028d: Unknown result type (might be due to invalid IL or missing references)
				//IL_028f: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02de: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0305: Unknown result type (might be due to invalid IL or missing references)
				//IL_030c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0316: Unknown result type (might be due to invalid IL or missing references)
				//IL_031b: Unknown result type (might be due to invalid IL or missing references)
				//IL_032d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0334: Unknown result type (might be due to invalid IL or missing references)
				//IL_033e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0343: Unknown result type (might be due to invalid IL or missing references)
				//IL_0355: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0361: Unknown result type (might be due to invalid IL or missing references)
				//IL_036b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0370: Unknown result type (might be due to invalid IL or missing references)
				//IL_0382: Unknown result type (might be due to invalid IL or missing references)
				//IL_0389: Unknown result type (might be due to invalid IL or missing references)
				//IL_038e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0398: Unknown result type (might be due to invalid IL or missing references)
				//IL_039d: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0411: Unknown result type (might be due to invalid IL or missing references)
				//IL_0418: Unknown result type (might be due to invalid IL or missing references)
				//IL_041a: Unknown result type (might be due to invalid IL or missing references)
				//IL_041f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0421: Unknown result type (might be due to invalid IL or missing references)
				//IL_0426: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_TransformData.HasComponent(entity))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[entity];
				Transform transform = m_TransformData[entity];
				if (!m_ObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				if ((m_Snap & Snap.LotGrid) != Snap.None && (m_BuildingData.HasComponent(prefabRef.m_Prefab) || m_BuildingExtensionData.HasComponent(prefabRef.m_Prefab) || m_AssetStampData.HasComponent(prefabRef.m_Prefab)))
				{
					float3 val = math.forward(transform.m_Rotation);
					float2 val2 = math.normalizesafe(((float3)(ref val)).xz, new float2(0f, 1f));
					float2 val3 = MathUtils.Right(val2);
					float2 val4 = ((float3)(ref m_ControlPoint.m_HitPosition)).xz - ((float3)(ref transform.m_Position)).xz;
					float heightWeight = math.select(0f, 1f, m_AreaType == Game.Areas.AreaType.Space);
					int2 val5 = default(int2);
					val5.x = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.x);
					val5.y = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.z);
					float2 val6 = float2.op_Implicit(val5) * 8f;
					float2 val7 = math.select(float2.op_Implicit(0f), float2.op_Implicit(4f), (val5 & 1) != 0);
					float2 val8 = default(float2);
					((float2)(ref val8))._002Ector(math.dot(val4, val3), math.dot(val4, val2));
					float2 val9 = MathUtils.Snap(val8, float2.op_Implicit(8f), val7);
					bool2 val10 = math.abs(val8 - val9) < m_MaxDistance;
					if (!math.any(val10))
					{
						return;
					}
					val9 = math.select(val8, val9, val10);
					float2 val11 = ((float3)(ref transform.m_Position)).xz + val3 * val9.x + val2 * val9.y;
					if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
					{
						if (math.distance(val11, ((float3)(ref transform.m_Position)).xz) > val6.x * 0.5f + 4f)
						{
							return;
						}
					}
					else if (math.any(math.abs(val9) > val6 * 0.5f + 4f))
					{
						return;
					}
					ControlPoint controlPoint = m_ControlPoint;
					controlPoint.m_OriginalEntity = Entity.Null;
					controlPoint.m_Direction = val3;
					((float3)(ref controlPoint.m_Position)).xz = val11;
					controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
					Line3 val12 = default(Line3);
					((Line3)(ref val12))._002Ector(controlPoint.m_Position, controlPoint.m_Position);
					Line3 val13 = default(Line3);
					((Line3)(ref val13))._002Ector(controlPoint.m_Position, controlPoint.m_Position);
					ref float3 a = ref val12.a;
					((float3)(ref a)).xz = ((float3)(ref a)).xz - controlPoint.m_Direction * 8f;
					ref float3 b = ref val12.b;
					((float3)(ref b)).xz = ((float3)(ref b)).xz + controlPoint.m_Direction * 8f;
					ref float3 a2 = ref val13.a;
					((float3)(ref a2)).xz = ((float3)(ref a2)).xz - MathUtils.Right(controlPoint.m_Direction) * 8f;
					ref float3 b2 = ref val13.b;
					((float3)(ref b2)).xz = ((float3)(ref b2)).xz + MathUtils.Right(controlPoint.m_Direction) * 8f;
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
					if (val10.y)
					{
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val12.a, val12.b), SnapLineFlags.Hidden, heightWeight));
					}
					controlPoint.m_Direction = MathUtils.Right(controlPoint.m_Direction);
					if (val10.x)
					{
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val13.a, val13.b), SnapLineFlags.Hidden, heightWeight));
					}
				}
				else if ((m_Snap & Snap.ObjectSide) != Snap.None && (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) == 0)
				{
					if (m_BuildingData.HasComponent(prefabRef.m_Prefab))
					{
						float2 val14 = float2.op_Implicit(m_BuildingData[prefabRef.m_Prefab].m_LotSize);
						((float3)(ref objectGeometryData.m_Bounds.min)).xz = val14 * -4f;
						((float3)(ref objectGeometryData.m_Bounds.max)).xz = val14 * 4f;
					}
					Quad3 val15 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
					CheckLine(Line3.op_Implicit(((Quad3)(ref val15)).ab));
					CheckLine(Line3.op_Implicit(((Quad3)(ref val15)).bc));
					CheckLine(Line3.op_Implicit(((Quad3)(ref val15)).cd));
					CheckLine(Line3.op_Implicit(((Quad3)(ref val15)).da));
				}
			}

			private void CheckLine(Line3 line)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				float heightWeight = math.select(0f, 1f, m_AreaType == Game.Areas.AreaType.Space);
				float num = default(float);
				if (MathUtils.Distance(((Line3)(ref line)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num) < m_MaxDistance)
				{
					ControlPoint controlPoint = m_ControlPoint;
					controlPoint.m_OriginalEntity = Entity.Null;
					controlPoint.m_Direction = math.normalizesafe(MathUtils.Tangent(((Line3)(ref line)).xz), default(float2));
					controlPoint.m_Position = MathUtils.Position(line, num);
					controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, heightWeight, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
					ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(line.a, line.b), (SnapLineFlags)0, heightWeight));
				}
			}
		}

		[ReadOnly]
		public bool m_AllowCreateArea;

		[ReadOnly]
		public bool m_ControlPointsMoved;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<Entity> m_ApplyTempAreas;

		[ReadOnly]
		public NativeList<ControlPoint> m_MoveStartPositions;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_AssetStampData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> m_LotData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_CachedNodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		public NativeList<ControlPoint> m_ControlPoints;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			AreaGeometryData areaGeometryData = m_PrefabAreaData[m_Prefab];
			int num = math.select(0, m_ControlPoints.Length - 1, m_State == State.Create);
			ControlPoint controlPoint = m_ControlPoints[num];
			controlPoint.m_Position = controlPoint.m_HitPosition;
			ControlPoint bestSnapPosition = controlPoint;
			switch (m_State)
			{
			case State.Default:
				if (FindControlPoint(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance, controlPoint.m_OriginalEntity, ignoreStartPositions: false, 0))
				{
					FixControlPointPosition(ref bestSnapPosition);
				}
				else if (!m_AllowCreateArea)
				{
					bestSnapPosition = default(ControlPoint);
				}
				else if (m_EditorMode)
				{
					FindParent(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance);
				}
				else
				{
					bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
				}
				break;
			case State.Create:
				FindControlPoint(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance, Entity.Null, ignoreStartPositions: false, m_ControlPoints.Length - 3);
				if (m_ControlPoints.Length >= 4)
				{
					ControlPoint controlPoint3 = m_ControlPoints[0];
					if (math.distance(controlPoint3.m_Position, bestSnapPosition.m_Position) < areaGeometryData.m_SnapDistance * 0.5f)
					{
						bestSnapPosition.m_Position = controlPoint3.m_Position;
					}
				}
				if (m_EditorMode)
				{
					FindParent(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance);
					if (m_ControlPoints.Length >= 2 && m_Nodes.HasBuffer(m_ControlPoints[0].m_OriginalEntity))
					{
						ControlPoint controlPoint4 = m_ControlPoints[0];
						controlPoint4.m_ElementIndex = new int2(FindParentMesh(m_ControlPoints[0]), -1);
						m_ControlPoints[0] = controlPoint4;
					}
				}
				else
				{
					bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
				}
				break;
			case State.Modify:
				if (m_ControlPointsMoved)
				{
					FindControlPoint(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance, Entity.Null, ignoreStartPositions: true, 0);
					float num2 = areaGeometryData.m_SnapDistance * 0.5f;
					for (int i = 0; i < m_MoveStartPositions.Length; i++)
					{
						ControlPoint controlPoint2 = m_MoveStartPositions[i];
						if (m_Nodes.HasBuffer(controlPoint2.m_OriginalEntity) && controlPoint2.m_ElementIndex.x >= 0)
						{
							DynamicBuffer<Game.Areas.Node> val = m_Nodes[controlPoint2.m_OriginalEntity];
							int num3 = math.select(controlPoint2.m_ElementIndex.x - 1, val.Length - 1, controlPoint2.m_ElementIndex.x == 0);
							int num4 = math.select(controlPoint2.m_ElementIndex.x + 1, 0, controlPoint2.m_ElementIndex.x == val.Length - 1);
							float3 position = val[num3].m_Position;
							float3 position2 = val[num4].m_Position;
							float num5 = math.distance(bestSnapPosition.m_Position, position);
							float num6 = math.distance(bestSnapPosition.m_Position, position2);
							if (num5 < num2)
							{
								bestSnapPosition.m_Position = position;
								num2 = num5;
							}
							if (num6 < num2)
							{
								bestSnapPosition.m_Position = position2;
								num2 = num6;
							}
						}
					}
					if (m_EditorMode)
					{
						bestSnapPosition.m_ElementIndex = new int2(FindParentMesh(controlPoint), -1);
					}
					else
					{
						bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
					}
				}
				else
				{
					FindControlPoint(ref bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance, controlPoint.m_OriginalEntity, ignoreStartPositions: false, 0);
					FixControlPointPosition(ref bestSnapPosition);
				}
				break;
			case State.Remove:
				bestSnapPosition = m_MoveStartPositions[0];
				break;
			}
			if (m_State == State.Default)
			{
				m_ControlPoints.Clear();
				m_ControlPoints.Add(ref bestSnapPosition);
				if (m_Nodes.HasBuffer(bestSnapPosition.m_OriginalEntity) && math.any(bestSnapPosition.m_ElementIndex >= 0))
				{
					AddControlPoints(bestSnapPosition, controlPoint, areaGeometryData.m_Type, areaGeometryData.m_SnapDistance * 0.5f);
				}
			}
			else
			{
				m_ControlPoints[num] = bestSnapPosition;
			}
		}

		private void FindParent(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, Game.Areas.AreaType type, float snapDistance)
		{
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			if ((m_Snap & Snap.AutoParent) != Snap.None)
			{
				ParentObjectIterator parentObjectIterator = new ParentObjectIterator
				{
					m_BoundsOffset = snapDistance * 0.125f + 0.4f,
					m_MaxDistance = snapDistance * 0.125f,
					m_TransformData = m_TransformData,
					m_PrefabRefData = m_PrefabRefData,
					m_BuildingData = m_PrefabBuildingData,
					m_ObjectGeometryData = m_ObjectGeometryData
				};
				Entity val = controlPoint.m_OriginalEntity;
				if (m_EditorMode)
				{
					Owner owner = default(Owner);
					while (m_OwnerData.TryGetComponent(val, ref owner) && !m_BuildingData.HasComponent(val))
					{
						val = owner.m_Owner;
					}
					DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
					if (m_InstalledUpgrades.TryGetBuffer(val, ref val2) && val2.Length != 0)
					{
						val = val2[0].m_Upgrade;
					}
				}
				int num = math.max(1, m_ControlPoints.Length - 1);
				Owner owner2 = default(Owner);
				DynamicBuffer<InstalledUpgrade> val4 = default(DynamicBuffer<InstalledUpgrade>);
				for (int i = 0; i < num; i++)
				{
					if (i == m_ControlPoints.Length - 1)
					{
						parentObjectIterator.m_Line.a = bestSnapPosition.m_Position;
					}
					else
					{
						parentObjectIterator.m_Line.a = m_ControlPoints[i].m_Position;
					}
					if (i + 1 >= m_ControlPoints.Length - 1)
					{
						parentObjectIterator.m_Line.b = bestSnapPosition.m_Position;
					}
					else
					{
						parentObjectIterator.m_Line.b = m_ControlPoints[i + 1].m_Position;
					}
					m_ObjectSearchTree.Iterate<ParentObjectIterator>(ref parentObjectIterator, 0);
					if (!(parentObjectIterator.m_Parent != Entity.Null))
					{
						continue;
					}
					Entity val3 = parentObjectIterator.m_Parent;
					if (m_EditorMode)
					{
						while (m_OwnerData.TryGetComponent(val3, ref owner2) && !m_BuildingData.HasComponent(val3))
						{
							val3 = owner2.m_Owner;
						}
						if (m_InstalledUpgrades.TryGetBuffer(val3, ref val4) && val4.Length != 0)
						{
							val3 = val4[0].m_Upgrade;
						}
					}
					if (val3 != val)
					{
						bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
					}
					else
					{
						bestSnapPosition.m_ElementIndex = new int2(FindParentMesh(controlPoint), -1);
					}
					bestSnapPosition.m_OriginalEntity = parentObjectIterator.m_Parent;
					return;
				}
			}
			bestSnapPosition.m_OriginalEntity = Entity.Null;
			bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
		}

		private int FindParentMesh(ControlPoint controlPoint)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			if (m_TransformData.HasComponent(controlPoint.m_OriginalEntity))
			{
				return controlPoint.m_ElementIndex.x;
			}
			DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
			if (!m_Nodes.TryGetBuffer(controlPoint.m_OriginalEntity, ref val) || val.Length < 2)
			{
				return -1;
			}
			int result = 0;
			float num = float.MaxValue;
			Game.Areas.Node node = val[val.Length - 1];
			LocalNodeCache localNodeCache = default(LocalNodeCache);
			DynamicBuffer<LocalNodeCache> val2 = default(DynamicBuffer<LocalNodeCache>);
			if (m_CachedNodes.TryGetBuffer(controlPoint.m_OriginalEntity, ref val2))
			{
				localNodeCache = val2[val.Length - 1];
			}
			float num3 = default(float);
			for (int i = 0; i < val.Length; i++)
			{
				Game.Areas.Node node2 = val[i];
				LocalNodeCache localNodeCache2 = default(LocalNodeCache);
				if (val2.IsCreated)
				{
					localNodeCache2 = val2[i];
				}
				float num2 = MathUtils.DistanceSquared(new Segment(node.m_Position, node2.m_Position), controlPoint.m_HitPosition, ref num3);
				if (num2 < num)
				{
					num = num2;
					result = ((!val2.IsCreated) ? math.select(0, -1, ((num3 >= 0.5f) ? node2.m_Elevation : node.m_Elevation) == float.MinValue) : ((num3 >= 0.5f) ? localNodeCache2.m_ParentMesh : localNodeCache.m_ParentMesh));
				}
				node = node2;
				localNodeCache = localNodeCache2;
			}
			return result;
		}

		private void FixControlPointPosition(ref ControlPoint bestSnapPosition)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (!m_Nodes.HasBuffer(bestSnapPosition.m_OriginalEntity) || bestSnapPosition.m_ElementIndex.x < 0)
			{
				return;
			}
			Entity val = bestSnapPosition.m_OriginalEntity;
			if (m_ApplyTempAreas.IsCreated)
			{
				for (int i = 0; i < m_ApplyTempAreas.Length; i++)
				{
					Entity val2 = m_ApplyTempAreas[i];
					if (m_TempData[val2].m_Original == val)
					{
						val = val2;
						break;
					}
				}
			}
			bestSnapPosition.m_Position = m_Nodes[val][bestSnapPosition.m_ElementIndex.x].m_Position;
		}

		private void AddControlPoints(ControlPoint bestSnapPosition, ControlPoint controlPoint, Game.Areas.AreaType type, float snapDistance)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			AreaIterator2 areaIterator = new AreaIterator2
			{
				m_EditorMode = m_EditorMode,
				m_AreaType = type,
				m_Bounds = new Bounds3(controlPoint.m_HitPosition - snapDistance, controlPoint.m_HitPosition + snapDistance),
				m_MaxDistance1 = snapDistance * 0.1f,
				m_MaxDistance2 = snapDistance,
				m_ControlPoint1 = bestSnapPosition,
				m_ControlPoint2 = controlPoint,
				m_ControlPoints = m_ControlPoints,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabAreaData = m_PrefabAreaData,
				m_LotData = m_LotData,
				m_OwnerData = m_OwnerData,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles,
				m_InstalledUpgrades = m_InstalledUpgrades
			};
			if (m_ApplyTempAreas.IsCreated && m_ApplyTempAreas.Length != 0)
			{
				areaIterator.m_IgnoreAreas = new NativeParallelHashSet<Entity>(m_ApplyTempAreas.Length, AllocatorHandle.op_Implicit((Allocator)2));
				Owner owner = default(Owner);
				int2 val3 = default(int2);
				Segment line = default(Segment);
				for (int i = 0; i < m_ApplyTempAreas.Length; i++)
				{
					Entity val = m_ApplyTempAreas[i];
					Temp temp = m_TempData[val];
					areaIterator.m_IgnoreAreas.Add(temp.m_Original);
					if ((!m_OwnerData.TryGetComponent(val, ref owner) || !m_Nodes.HasBuffer(owner.m_Owner)) && (temp.m_Flags & TempFlags.Delete) == 0)
					{
						Entity area = (((temp.m_Flags & TempFlags.Create) != 0) ? val : temp.m_Original);
						DynamicBuffer<Game.Areas.Node> val2 = m_Nodes[val];
						for (int j = 0; j < val2.Length; j++)
						{
							((int2)(ref val3))._002Ector(j, math.select(j + 1, 0, j == val2.Length - 1));
							((Segment)(ref line))._002Ector(val2[val3.x].m_Position, val2[val3.y].m_Position);
							areaIterator.CheckLine(line, snapDistance, area, val3, m_LotData.HasComponent(val));
						}
					}
				}
			}
			m_AreaSearchTree.Iterate<AreaIterator2>(ref areaIterator, 0);
			if (areaIterator.m_IgnoreAreas.IsCreated)
			{
				areaIterator.m_IgnoreAreas.Dispose();
			}
		}

		private bool FindControlPoint(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, Game.Areas.AreaType type, float snapDistance, Entity preferredArea, bool ignoreStartPositions, int selfSnap)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_070b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_0720: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0926: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0955: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0824: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0876: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_0895: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			bestSnapPosition.m_OriginalEntity = Entity.Null;
			NativeList<SnapLine> snapLines = default(NativeList<SnapLine>);
			snapLines._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			if ((m_Snap & Snap.StraightDirection) != Snap.None)
			{
				if (m_State == State.Create)
				{
					ControlPoint controlPoint2 = controlPoint;
					controlPoint2.m_OriginalEntity = Entity.Null;
					controlPoint2.m_Position = controlPoint.m_HitPosition;
					float3 resultDir = default(float3);
					float bestDirectionDistance = float.MaxValue;
					if (m_ControlPoints.Length >= 2)
					{
						ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 2];
						if (!((float2)(ref controlPoint3.m_Direction)).Equals(default(float2)))
						{
							ToolUtils.DirectionSnap(ref bestDirectionDistance, ref controlPoint2.m_Position, ref resultDir, controlPoint.m_HitPosition, controlPoint3.m_Position, new float3(controlPoint3.m_Direction.x, 0f, controlPoint3.m_Direction.y), snapDistance);
						}
					}
					if (m_ControlPoints.Length >= 3)
					{
						ControlPoint controlPoint4 = m_ControlPoints[m_ControlPoints.Length - 3];
						ControlPoint controlPoint5 = m_ControlPoints[m_ControlPoints.Length - 2];
						float2 val = math.normalizesafe(((float3)(ref controlPoint4.m_Position)).xz - ((float3)(ref controlPoint5.m_Position)).xz, default(float2));
						if (!((float2)(ref val)).Equals(default(float2)))
						{
							ToolUtils.DirectionSnap(ref bestDirectionDistance, ref controlPoint2.m_Position, ref resultDir, controlPoint.m_HitPosition, controlPoint5.m_Position, new float3(val.x, 0f, val.y), snapDistance);
						}
					}
					if (!((float3)(ref resultDir)).Equals(default(float3)))
					{
						controlPoint2.m_Direction = ((float3)(ref resultDir)).xz;
						controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
						ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
						float3 position = controlPoint2.m_Position;
						float3 endPos = position;
						((float3)(ref endPos)).xz = ((float3)(ref endPos)).xz + controlPoint2.m_Direction;
						ToolUtils.AddSnapLine(ref bestSnapPosition, snapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(position, endPos), SnapLineFlags.Hidden, 0f));
					}
				}
				else if (m_State == State.Modify)
				{
					for (int i = 0; i < m_MoveStartPositions.Length; i++)
					{
						ControlPoint controlPoint6 = m_MoveStartPositions[i];
						if (!m_Nodes.HasBuffer(controlPoint6.m_OriginalEntity) || !math.any(controlPoint6.m_ElementIndex >= 0))
						{
							continue;
						}
						DynamicBuffer<Game.Areas.Node> val2 = m_Nodes[controlPoint6.m_OriginalEntity];
						if (val2.Length < 3)
						{
							continue;
						}
						int4 val3 = math.select(controlPoint6.m_ElementIndex.x + new int4(-2, -1, 1, 2), controlPoint6.m_ElementIndex.y + new int4(-1, 0, 1, 2), controlPoint6.m_ElementIndex.y >= 0);
						int4 val4 = val3;
						int4 val5 = val3;
						int2 val6 = new int2(val2.Length, -val2.Length);
						val3 = math.select(val4, val5 + ((int2)(ref val6)).xxyy, new bool4(((int4)(ref val3)).xy < 0, ((int4)(ref val3)).zw >= val2.Length));
						float3 position2 = val2[val3.x].m_Position;
						float3 position3 = val2[val3.y].m_Position;
						float3 position4 = val2[val3.z].m_Position;
						float3 position5 = val2[val3.w].m_Position;
						float2 val7 = math.normalizesafe(((float3)(ref position2)).xz - ((float3)(ref position3)).xz, default(float2));
						float2 val8 = math.normalizesafe(((float3)(ref position5)).xz - ((float3)(ref position4)).xz, default(float2));
						if (!((float2)(ref val7)).Equals(default(float2)))
						{
							ControlPoint controlPoint7 = controlPoint;
							controlPoint7.m_OriginalEntity = Entity.Null;
							controlPoint7.m_Position = controlPoint.m_HitPosition;
							float3 resultDir2 = default(float3);
							float bestDirectionDistance2 = float.MaxValue;
							ToolUtils.DirectionSnap(ref bestDirectionDistance2, ref controlPoint7.m_Position, ref resultDir2, controlPoint.m_HitPosition, position3, new float3(val7.x, 0f, val7.y), snapDistance);
							if (!((float3)(ref resultDir2)).Equals(default(float3)))
							{
								controlPoint7.m_Direction = ((float3)(ref resultDir2)).xz;
								controlPoint7.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, controlPoint7.m_Position, controlPoint7.m_Direction);
								ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint7);
								float3 position6 = controlPoint7.m_Position;
								float3 endPos2 = position6;
								((float3)(ref endPos2)).xz = ((float3)(ref endPos2)).xz + controlPoint7.m_Direction;
								ToolUtils.AddSnapLine(ref bestSnapPosition, snapLines, new SnapLine(controlPoint7, NetUtils.StraightCurve(position6, endPos2), SnapLineFlags.Hidden, 0f));
							}
						}
						if (!((float2)(ref val8)).Equals(default(float2)))
						{
							ControlPoint controlPoint8 = controlPoint;
							controlPoint8.m_OriginalEntity = Entity.Null;
							controlPoint8.m_Position = controlPoint.m_HitPosition;
							float3 resultDir3 = default(float3);
							float bestDirectionDistance3 = float.MaxValue;
							ToolUtils.DirectionSnap(ref bestDirectionDistance3, ref controlPoint8.m_Position, ref resultDir3, controlPoint.m_HitPosition, position4, new float3(val8.x, 0f, val8.y), snapDistance);
							if (!((float3)(ref resultDir3)).Equals(default(float3)))
							{
								controlPoint8.m_Direction = ((float3)(ref resultDir3)).xz;
								controlPoint8.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, controlPoint8.m_Position, controlPoint8.m_Direction);
								ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint8);
								float3 position7 = controlPoint8.m_Position;
								float3 endPos3 = position7;
								((float3)(ref endPos3)).xz = ((float3)(ref endPos3)).xz + controlPoint8.m_Direction;
								ToolUtils.AddSnapLine(ref bestSnapPosition, snapLines, new SnapLine(controlPoint8, NetUtils.StraightCurve(position7, endPos3), SnapLineFlags.Hidden, 0f));
							}
						}
					}
				}
			}
			if ((m_Snap & Snap.ExistingGeometry) != Snap.None || preferredArea != Entity.Null || ignoreStartPositions || selfSnap >= 1)
			{
				float num = math.select(snapDistance, snapDistance * 0.5f, (m_Snap & Snap.ExistingGeometry) == 0);
				AreaIterator areaIterator = new AreaIterator
				{
					m_EditorMode = m_EditorMode,
					m_IgnoreStartPositions = ignoreStartPositions,
					m_Snap = m_Snap,
					m_AreaType = type,
					m_Bounds = new Bounds3(controlPoint.m_HitPosition - num, controlPoint.m_HitPosition + num),
					m_MaxDistance = num,
					m_PreferArea = preferredArea,
					m_ControlPoint = controlPoint,
					m_BestSnapPosition = bestSnapPosition,
					m_SnapLines = snapLines,
					m_MoveStartPositions = m_MoveStartPositions,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabAreaData = m_PrefabAreaData,
					m_LotData = m_LotData,
					m_OwnerData = m_OwnerData,
					m_Nodes = m_Nodes,
					m_Triangles = m_Triangles,
					m_InstalledUpgrades = m_InstalledUpgrades
				};
				if (m_ApplyTempAreas.IsCreated && m_ApplyTempAreas.Length != 0)
				{
					areaIterator.m_IgnoreAreas = new NativeParallelHashSet<Entity>(m_ApplyTempAreas.Length, AllocatorHandle.op_Implicit((Allocator)2));
					Owner owner = default(Owner);
					int2 val12 = default(int2);
					Segment line = default(Segment);
					for (int j = 0; j < m_ApplyTempAreas.Length; j++)
					{
						Entity val9 = m_ApplyTempAreas[j];
						Temp temp = m_TempData[val9];
						areaIterator.m_IgnoreAreas.Add(temp.m_Original);
						if ((m_OwnerData.TryGetComponent(val9, ref owner) && m_Nodes.HasBuffer(owner.m_Owner)) || (temp.m_Flags & TempFlags.Delete) != 0)
						{
							continue;
						}
						Entity val10 = (((temp.m_Flags & TempFlags.Create) != 0) ? val9 : temp.m_Original);
						if ((m_Snap & Snap.ExistingGeometry) != Snap.None || val10 == preferredArea)
						{
							DynamicBuffer<Game.Areas.Node> val11 = m_Nodes[val9];
							for (int k = 0; k < val11.Length; k++)
							{
								((int2)(ref val12))._002Ector(k, math.select(k + 1, 0, k == val11.Length - 1));
								((Segment)(ref line))._002Ector(val11[val12.x].m_Position, val11[val12.y].m_Position);
								areaIterator.CheckLine(line, num, val10, val12, !m_EditorMode && m_LotData.HasComponent(val9));
							}
						}
					}
				}
				if ((m_Snap & Snap.ExistingGeometry) != Snap.None || preferredArea != Entity.Null || ignoreStartPositions)
				{
					m_AreaSearchTree.Iterate<AreaIterator>(ref areaIterator, 0);
				}
				Segment line2 = default(Segment);
				for (int l = 0; l < selfSnap; l++)
				{
					((Segment)(ref line2))._002Ector(m_ControlPoints[l].m_Position, m_ControlPoints[l + 1].m_Position);
					areaIterator.CheckLine(line2, num, Entity.Null, new int2(l, l + 1), lockFirstEdge: false);
				}
				bestSnapPosition = areaIterator.m_BestSnapPosition;
				if (areaIterator.m_IgnoreAreas.IsCreated)
				{
					areaIterator.m_IgnoreAreas.Dispose();
				}
			}
			if ((m_Snap & (Snap.NetSide | Snap.NetMiddle)) != Snap.None && (m_State != State.Default || m_AllowCreateArea))
			{
				NetIterator netIterator = new NetIterator
				{
					m_Snap = m_Snap,
					m_Bounds = new Bounds3(controlPoint.m_HitPosition - snapDistance, controlPoint.m_HitPosition + snapDistance),
					m_MaxDistance = snapDistance,
					m_AreaType = type,
					m_ControlPoint = controlPoint,
					m_BestSnapPosition = bestSnapPosition,
					m_SnapLines = snapLines,
					m_CurveData = m_CurveData,
					m_EdgeGeometryData = m_EdgeGeometryData,
					m_StartGeometryData = m_StartGeometryData,
					m_EndGeometryData = m_EndGeometryData,
					m_CompositionData = m_CompositionData,
					m_PrefabCompositionData = m_PrefabCompositionData
				};
				m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
				bestSnapPosition = netIterator.m_BestSnapPosition;
			}
			if ((m_Snap & (Snap.ObjectSide | Snap.LotGrid)) != Snap.None && (m_State != State.Default || m_AllowCreateArea))
			{
				ObjectIterator objectIterator = new ObjectIterator
				{
					m_Bounds = new Bounds3(controlPoint.m_HitPosition - snapDistance, controlPoint.m_HitPosition + snapDistance),
					m_MaxDistance = snapDistance,
					m_AreaType = type,
					m_Snap = m_Snap,
					m_ControlPoint = controlPoint,
					m_BestSnapPosition = bestSnapPosition,
					m_SnapLines = snapLines,
					m_TransformData = m_TransformData,
					m_PrefabRefData = m_PrefabRefData,
					m_BuildingData = m_PrefabBuildingData,
					m_BuildingExtensionData = m_BuildingExtensionData,
					m_AssetStampData = m_AssetStampData,
					m_ObjectGeometryData = m_ObjectGeometryData
				};
				m_ObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
				bestSnapPosition = objectIterator.m_BestSnapPosition;
			}
			snapLines.Dispose();
			return m_Nodes.HasBuffer(bestSnapPosition.m_OriginalEntity);
		}
	}

	[BurstCompile]
	private struct RemoveMapTilesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<LocalNodeCache> m_CacheType;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			if (m_ControlPoints.Length == 1 && m_ControlPoints[0].Equals(default(ControlPoint)))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Game.Areas.Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Areas.Node>(ref m_NodeType);
			BufferAccessor<LocalNodeCache> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LocalNodeCache>(ref m_CacheType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity original = nativeArray[i];
				DynamicBuffer<Game.Areas.Node> val = bufferAccessor[i];
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(unfilteredChunkIndex);
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = original
				};
				creationDefinition.m_Flags |= CreationFlags.Delete;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(unfilteredChunkIndex, val2, creationDefinition);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(unfilteredChunkIndex, val2, default(Updated));
				((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(unfilteredChunkIndex, val2).CopyFrom(val.AsNativeArray());
				if (bufferAccessor2.Length != 0)
				{
					DynamicBuffer<LocalNodeCache> val3 = bufferAccessor2[i];
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(unfilteredChunkIndex, val2).CopyFrom(val3.AsNativeArray());
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public bool m_AllowCreateArea;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public State m_State;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_Recreate;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<Entity> m_ApplyTempAreas;

		[NativeDisableContainerSafetyRestriction]
		[ReadOnly]
		public NativeArray<Entity> m_ApplyTempBuildings;

		[ReadOnly]
		public NativeList<ControlPoint> m_MoveStartPositions;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Clear> m_ClearData;

		[ReadOnly]
		public ComponentLookup<Space> m_SpaceData;

		[ReadOnly]
		public ComponentLookup<Area> m_AreaData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> m_NetElevationData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_CachedNodes;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		public NativeValue<Tooltip> m_Tooltip;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			if (m_ControlPoints.Length != 1 || !m_ControlPoints[0].Equals(default(ControlPoint)))
			{
				switch (m_Mode)
				{
				case Mode.Edit:
					Edit();
					break;
				case Mode.Generate:
					Generate();
					break;
				}
			}
		}

		private void Generate()
		{
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			int2 val = default(int2);
			val.y = 0;
			Bounds2 val4 = default(Bounds2);
			while (val.y < 23)
			{
				val.x = 0;
				while (val.x < 23)
				{
					Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Prefab = m_Prefab
					};
					float2 val3 = new float2(23f, 23f) * 311.65216f;
					val4.min = float2.op_Implicit(val) * 623.3043f - val3;
					val4.max = float2.op_Implicit(val + 1) * 623.3043f - val3;
					DynamicBuffer<Game.Areas.Node> val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val2);
					val5.ResizeUninitialized(5);
					val5[0] = new Game.Areas.Node(new float3(val4.min.x, 0f, val4.min.y), float.MinValue);
					val5[1] = new Game.Areas.Node(new float3(val4.min.x, 0f, val4.max.y), float.MinValue);
					val5[2] = new Game.Areas.Node(new float3(val4.max.x, 0f, val4.max.y), float.MinValue);
					val5[3] = new Game.Areas.Node(new float3(val4.max.x, 0f, val4.min.y), float.MinValue);
					val5[4] = val5[0];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
					val.x++;
				}
				val.y++;
			}
			m_Tooltip.value = Tooltip.GenerateAreas;
		}

		private void GetControlPoints(int index, out ControlPoint firstPoint, out ControlPoint lastPoint)
		{
			switch (m_State)
			{
			case State.Default:
				firstPoint = m_ControlPoints[index];
				lastPoint = m_ControlPoints[index];
				break;
			case State.Create:
				firstPoint = default(ControlPoint);
				lastPoint = m_ControlPoints[m_ControlPoints.Length - 1];
				break;
			case State.Modify:
				firstPoint = m_MoveStartPositions[index];
				lastPoint = m_ControlPoints[0];
				break;
			case State.Remove:
				firstPoint = m_MoveStartPositions[index];
				lastPoint = m_ControlPoints[0];
				break;
			default:
				firstPoint = default(ControlPoint);
				lastPoint = default(ControlPoint);
				break;
			}
		}

		private void Edit()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			AreaGeometryData areaData = m_PrefabAreaData[m_Prefab];
			int num = m_State switch
			{
				State.Default => m_ControlPoints.Length, 
				State.Create => 1, 
				State.Modify => m_MoveStartPositions.Length, 
				State.Remove => m_MoveStartPositions.Length, 
				_ => 0, 
			};
			m_Tooltip.value = Tooltip.None;
			bool flag = false;
			NativeParallelHashSet<Entity> createdEntities = default(NativeParallelHashSet<Entity>);
			createdEntities._002Ector(num * 2, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < num; i++)
			{
				GetControlPoints(i, out var firstPoint, out var _);
				if (m_Nodes.HasBuffer(firstPoint.m_OriginalEntity) && math.any(firstPoint.m_ElementIndex >= 0))
				{
					createdEntities.Add(firstPoint.m_OriginalEntity);
				}
			}
			NativeList<ClearAreaData> clearAreas = default(NativeList<ClearAreaData>);
			Owner owner = default(Owner);
			Transform transform = default(Transform);
			float2 val6 = default(float2);
			float2 val7 = default(float2);
			Transform transform2 = default(Transform);
			Area area = default(Area);
			DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int j = 0; j < num; j++)
			{
				GetControlPoints(j, out var firstPoint2, out var lastPoint2);
				if (j == 0 && m_State == State.Modify)
				{
					flag = !firstPoint2.Equals(lastPoint2);
				}
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = m_Prefab
				};
				if (m_Nodes.HasBuffer(firstPoint2.m_OriginalEntity) && math.any(firstPoint2.m_ElementIndex >= 0))
				{
					creationDefinition.m_Original = firstPoint2.m_OriginalEntity;
				}
				else if (m_Recreate != Entity.Null)
				{
					creationDefinition.m_Original = m_Recreate;
				}
				float minNodeDistance = AreaUtils.GetMinNodeDistance(areaData);
				int2 val2 = default(int2);
				DynamicBuffer<Game.Areas.Node> nodes = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val);
				DynamicBuffer<LocalNodeCache> val3 = default(DynamicBuffer<LocalNodeCache>);
				bool isComplete = false;
				if (m_Nodes.HasBuffer(firstPoint2.m_OriginalEntity) && math.any(firstPoint2.m_ElementIndex >= 0))
				{
					creationDefinition.m_Flags |= CreationFlags.Relocate;
					isComplete = true;
					Entity sourceArea = GetSourceArea(firstPoint2.m_OriginalEntity);
					DynamicBuffer<Game.Areas.Node> val4 = m_Nodes[sourceArea];
					DynamicBuffer<LocalNodeCache> val5 = default(DynamicBuffer<LocalNodeCache>);
					if (m_CachedNodes.HasBuffer(sourceArea))
					{
						val5 = m_CachedNodes[sourceArea];
					}
					float num2 = float.MinValue;
					int num3 = -1;
					if (lastPoint2.m_ElementIndex.x >= 0)
					{
						num3 = lastPoint2.m_ElementIndex.x;
						if (m_OwnerData.TryGetComponent(firstPoint2.m_OriginalEntity, ref owner))
						{
							Entity owner2 = owner.m_Owner;
							while (m_OwnerData.HasComponent(owner2) && !m_BuildingData.HasComponent(owner2))
							{
								if (m_LocalTransformCacheData.HasComponent(owner2))
								{
									num3 = m_LocalTransformCacheData[owner2].m_ParentMesh;
								}
								owner2 = m_OwnerData[owner2].m_Owner;
							}
							if (m_TransformData.TryGetComponent(owner2, ref transform))
							{
								num2 = lastPoint2.m_Position.y - transform.m_Position.y;
							}
						}
						if (num3 != -1)
						{
							if (num2 == float.MinValue)
							{
								num2 = 0f;
							}
						}
						else
						{
							num2 = float.MinValue;
						}
					}
					if (firstPoint2.m_ElementIndex.y >= 0)
					{
						int y = firstPoint2.m_ElementIndex.y;
						int num4 = math.select(firstPoint2.m_ElementIndex.y + 1, 0, firstPoint2.m_ElementIndex.y == val4.Length - 1);
						((float2)(ref val6))._002Ector(math.distance(lastPoint2.m_Position, val4[y].m_Position), math.distance(lastPoint2.m_Position, val4[num4].m_Position));
						bool flag2 = flag && math.any(val6 < minNodeDistance);
						int num5 = math.select(1, 0, flag2 || !flag);
						int num6 = val4.Length + num5;
						nodes.ResizeUninitialized(num6);
						int num7 = 0;
						if (val5.IsCreated)
						{
							val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val);
							val3.ResizeUninitialized(num6);
							for (int k = 0; k <= firstPoint2.m_ElementIndex.y; k++)
							{
								nodes[num7] = val4[k];
								val3[num7] = val5[k];
								num7++;
							}
							val2.x = num7;
							for (int l = 0; l < num5; l++)
							{
								nodes[num7] = new Game.Areas.Node(lastPoint2.m_Position, num2);
								val3[num7] = new LocalNodeCache
								{
									m_Position = lastPoint2.m_Position,
									m_ParentMesh = num3
								};
								num7++;
							}
							val2.y = num7;
							for (int m = firstPoint2.m_ElementIndex.y + 1; m < val4.Length; m++)
							{
								nodes[num7] = val4[m];
								val3[num7] = val5[m];
								num7++;
							}
						}
						else
						{
							for (int n = 0; n <= firstPoint2.m_ElementIndex.y; n++)
							{
								nodes[num7++] = val4[n];
							}
							for (int num8 = 0; num8 < num5; num8++)
							{
								nodes[num7++] = new Game.Areas.Node(lastPoint2.m_Position, num2);
							}
							for (int num9 = firstPoint2.m_ElementIndex.y + 1; num9 < val4.Length; num9++)
							{
								nodes[num7++] = val4[num9];
							}
						}
						switch (m_State)
						{
						case State.Default:
							if (m_AllowCreateArea)
							{
								m_Tooltip.value = Tooltip.CreateAreaOrModifyEdge;
							}
							else
							{
								m_Tooltip.value = Tooltip.ModifyEdge;
							}
							break;
						case State.Modify:
							if (!flag2 && flag)
							{
								m_Tooltip.value = Tooltip.InsertNode;
							}
							break;
						}
					}
					else
					{
						bool flag3 = false;
						if (!m_OwnerData.HasComponent(creationDefinition.m_Original) || val4.Length >= 4)
						{
							if (m_State == State.Remove)
							{
								flag3 = true;
							}
							else
							{
								int num10 = math.select(firstPoint2.m_ElementIndex.x - 1, val4.Length - 1, firstPoint2.m_ElementIndex.x == 0);
								int num11 = math.select(firstPoint2.m_ElementIndex.x + 1, 0, firstPoint2.m_ElementIndex.x == val4.Length - 1);
								((float2)(ref val7))._002Ector(math.distance(lastPoint2.m_Position, val4[num10].m_Position), math.distance(lastPoint2.m_Position, val4[num11].m_Position));
								flag3 = flag && math.any(val7 < minNodeDistance);
							}
						}
						int num12 = math.select(0, 1, flag || flag3);
						int num13 = math.select(1, 0, flag3 || !flag);
						int num14 = val4.Length + num13 - num12;
						nodes.ResizeUninitialized(num14);
						int num15 = 0;
						if (val5.IsCreated)
						{
							val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val);
							val3.ResizeUninitialized(num14);
							for (int num16 = 0; num16 <= firstPoint2.m_ElementIndex.x - num12; num16++)
							{
								nodes[num15] = val4[num16];
								val3[num15] = val5[num16];
								num15++;
							}
							val2.x = num15;
							for (int num17 = 0; num17 < num13; num17++)
							{
								nodes[num15] = new Game.Areas.Node(lastPoint2.m_Position, num2);
								val3[num15] = new LocalNodeCache
								{
									m_Position = lastPoint2.m_Position,
									m_ParentMesh = num3
								};
								num15++;
							}
							val2.y = num15;
							for (int num18 = firstPoint2.m_ElementIndex.x + 1; num18 < val4.Length; num18++)
							{
								nodes[num15] = val4[num18];
								val3[num15] = val5[num18];
								num15++;
							}
						}
						else
						{
							for (int num19 = 0; num19 <= firstPoint2.m_ElementIndex.x - num12; num19++)
							{
								nodes[num15++] = val4[num19];
							}
							for (int num20 = 0; num20 < num13; num20++)
							{
								nodes[num15++] = new Game.Areas.Node(lastPoint2.m_Position, num2);
							}
							for (int num21 = firstPoint2.m_ElementIndex.x + 1; num21 < val4.Length; num21++)
							{
								nodes[num15++] = val4[num21];
							}
						}
						if (num14 < 3)
						{
							creationDefinition.m_Flags |= CreationFlags.Delete;
						}
						switch (m_State)
						{
						case State.Default:
							if (m_AllowCreateArea)
							{
								m_Tooltip.value = Tooltip.CreateAreaOrModifyNode;
							}
							else
							{
								m_Tooltip.value = Tooltip.ModifyNode;
							}
							break;
						case State.Modify:
							if (num14 < 3)
							{
								m_Tooltip.value = Tooltip.DeleteArea;
							}
							else if (flag3)
							{
								m_Tooltip.value = Tooltip.MergeNodes;
							}
							else if (flag)
							{
								m_Tooltip.value = Tooltip.MoveNode;
							}
							break;
						case State.Remove:
							if (num14 < 3)
							{
								m_Tooltip.value = Tooltip.DeleteArea;
							}
							else if (flag3)
							{
								m_Tooltip.value = Tooltip.RemoveNode;
							}
							break;
						}
					}
				}
				else
				{
					if (m_Recreate != Entity.Null)
					{
						creationDefinition.m_Flags |= CreationFlags.Recreate;
					}
					bool flag4 = false;
					if (m_ControlPoints.Length >= 2)
					{
						flag4 = math.distance(m_ControlPoints[m_ControlPoints.Length - 2].m_Position, m_ControlPoints[m_ControlPoints.Length - 1].m_Position) < minNodeDistance;
					}
					int num22 = math.select(m_ControlPoints.Length, m_ControlPoints.Length - 1, flag4);
					nodes.ResizeUninitialized(num22);
					if (m_EditorMode)
					{
						val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val);
						val3.ResizeUninitialized(num22);
						((int2)(ref val2))._002Ector(0, num22);
						float num23 = float.MinValue;
						int num24 = lastPoint2.m_ElementIndex.x;
						if (m_TransformData.HasComponent(lastPoint2.m_OriginalEntity))
						{
							Entity val8 = lastPoint2.m_OriginalEntity;
							while (m_OwnerData.HasComponent(val8) && !m_BuildingData.HasComponent(val8))
							{
								if (m_LocalTransformCacheData.HasComponent(val8))
								{
									num24 = m_LocalTransformCacheData[val8].m_ParentMesh;
								}
								val8 = m_OwnerData[val8].m_Owner;
							}
							if (m_TransformData.TryGetComponent(val8, ref transform2))
							{
								num23 = transform2.m_Position.y;
							}
						}
						for (int num25 = 0; num25 < num22; num25++)
						{
							int num26 = -1;
							float num27 = float.MinValue;
							if (m_ControlPoints[num25].m_ElementIndex.x >= 0)
							{
								num26 = math.select(m_ControlPoints[num25].m_ElementIndex.x, num24, num24 != -1);
								num27 = math.select(num27, m_ControlPoints[num25].m_Position.y - num23, num23 != float.MinValue);
							}
							if (num26 != -1)
							{
								if (num27 == float.MinValue)
								{
									num27 = 0f;
								}
							}
							else
							{
								num27 = float.MinValue;
							}
							nodes[num25] = new Game.Areas.Node(m_ControlPoints[num25].m_Position, num27);
							val3[num25] = new LocalNodeCache
							{
								m_Position = m_ControlPoints[num25].m_Position,
								m_ParentMesh = num26
							};
						}
					}
					else
					{
						for (int num28 = 0; num28 < num22; num28++)
						{
							nodes[num28] = new Game.Areas.Node(m_ControlPoints[num28].m_Position, float.MinValue);
						}
					}
					switch (m_State)
					{
					case State.Default:
						if (m_ControlPoints.Length == 1 && m_AllowCreateArea)
						{
							m_Tooltip.value = Tooltip.CreateArea;
						}
						break;
					case State.Create:
						if (flag4)
						{
							break;
						}
						if (m_ControlPoints.Length >= 4)
						{
							ControlPoint controlPoint = m_ControlPoints[0];
							if (((float3)(ref controlPoint.m_Position)).Equals(m_ControlPoints[m_ControlPoints.Length - 1].m_Position))
							{
								m_Tooltip.value = Tooltip.CompleteArea;
								break;
							}
						}
						m_Tooltip.value = Tooltip.AddNode;
						break;
					}
				}
				bool flag5 = false;
				Transform inverseParentTransform = default(Transform);
				if (m_TransformData.HasComponent(lastPoint2.m_OriginalEntity))
				{
					if ((areaData.m_Flags & Game.Areas.GeometryFlags.ClearArea) != 0)
					{
						ClearAreaHelpers.FillClearAreas(m_PrefabRefData[lastPoint2.m_OriginalEntity].m_Prefab, m_TransformData[lastPoint2.m_OriginalEntity], nodes, isComplete, m_PrefabObjectGeometryData, ref clearAreas);
					}
					OwnerDefinition ownerDefinition = GetOwnerDefinition(lastPoint2.m_OriginalEntity, creationDefinition.m_Original, createdEntities, upgrade: true, (areaData.m_Flags & Game.Areas.GeometryFlags.ClearArea) != 0, clearAreas);
					if (ownerDefinition.m_Prefab != Entity.Null)
					{
						inverseParentTransform.m_Position = -ownerDefinition.m_Position;
						inverseParentTransform.m_Rotation = math.inverse(ownerDefinition.m_Rotation);
						flag5 = true;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
					}
				}
				else if (m_OwnerData.HasComponent(creationDefinition.m_Original))
				{
					Entity owner3 = m_OwnerData[creationDefinition.m_Original].m_Owner;
					if (m_TransformData.HasComponent(owner3))
					{
						if ((areaData.m_Flags & Game.Areas.GeometryFlags.ClearArea) != 0)
						{
							ClearAreaHelpers.FillClearAreas(m_PrefabRefData[owner3].m_Prefab, m_TransformData[owner3], nodes, isComplete, m_PrefabObjectGeometryData, ref clearAreas);
						}
						OwnerDefinition ownerDefinition2 = GetOwnerDefinition(owner3, creationDefinition.m_Original, createdEntities, upgrade: true, (areaData.m_Flags & Game.Areas.GeometryFlags.ClearArea) != 0, clearAreas);
						if (ownerDefinition2.m_Prefab != Entity.Null)
						{
							inverseParentTransform.m_Position = -ownerDefinition2.m_Position;
							inverseParentTransform.m_Rotation = math.inverse(ownerDefinition2.m_Rotation);
							flag5 = true;
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition2);
						}
						else
						{
							Transform transform3 = m_TransformData[owner3];
							inverseParentTransform.m_Position = -transform3.m_Position;
							inverseParentTransform.m_Rotation = math.inverse(transform3.m_Rotation);
							flag5 = true;
							creationDefinition.m_Owner = owner3;
						}
					}
					else
					{
						creationDefinition.m_Owner = owner3;
					}
				}
				if (flag5)
				{
					for (int num29 = val2.x; num29 < val2.y; num29++)
					{
						LocalNodeCache localNodeCache = val3[num29];
						localNodeCache.m_Position = ObjectUtils.WorldToLocal(inverseParentTransform, localNodeCache.m_Position);
					}
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
				if (m_AreaData.TryGetComponent(creationDefinition.m_Original, ref area) && m_SubObjects.TryGetBuffer(creationDefinition.m_Original, ref subObjects) && (area.m_Flags & AreaFlags.Complete) != 0)
				{
					CheckSubObjects(subObjects, nodes, createdEntities, minNodeDistance, (area.m_Flags & AreaFlags.CounterClockwise) != 0);
				}
				if (clearAreas.IsCreated)
				{
					clearAreas.Clear();
				}
			}
			if (clearAreas.IsCreated)
			{
				clearAreas.Dispose();
			}
			createdEntities.Dispose();
		}

		private Entity GetSourceArea(Entity originalArea)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (m_ApplyTempAreas.IsCreated)
			{
				for (int i = 0; i < m_ApplyTempAreas.Length; i++)
				{
					Entity val = m_ApplyTempAreas[i];
					if (originalArea == m_TempData[val].m_Original)
					{
						return val;
					}
				}
			}
			return originalArea;
		}

		private void CheckSubObjects(DynamicBuffer<Game.Objects.SubObject> subObjects, DynamicBuffer<Game.Areas.Node> nodes, NativeParallelHashSet<Entity> createdEntities, float minNodeDistance, bool isCounterClockwise)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
			Segment val2 = default(Segment);
			float num7 = default(float);
			for (int i = 0; i < subObjects.Length; i++)
			{
				Game.Objects.SubObject subObject = subObjects[i];
				if (!m_BuildingData.HasComponent(subObject.m_SubObject))
				{
					continue;
				}
				if (m_ApplyTempBuildings.IsCreated)
				{
					bool flag = false;
					for (int j = 0; j < m_ApplyTempBuildings.Length; j++)
					{
						if (m_ApplyTempBuildings[j] == subObject.m_SubObject)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				Transform transform = m_TransformData[subObject.m_SubObject];
				PrefabRef prefabRef = m_PrefabRefData[subObject.m_SubObject];
				if (!m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData))
				{
					continue;
				}
				float num;
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					num = objectGeometryData.m_Size.x * 0.5f;
				}
				else
				{
					num = math.length(MathUtils.Size(((Bounds3)(ref objectGeometryData.m_Bounds)).xz)) * 0.5f;
					ref float3 position = ref transform.m_Position;
					float2 xz = ((float3)(ref position)).xz;
					float3 val = math.rotate(transform.m_Rotation, MathUtils.Center(objectGeometryData.m_Bounds));
					((float3)(ref position)).xz = xz - ((float3)(ref val)).xz;
				}
				float num2 = 0f;
				int num3 = -1;
				bool flag2 = nodes.Length <= 2;
				Game.Areas.Node node;
				if (!flag2)
				{
					float num4 = float.MaxValue;
					float num5 = num + minNodeDistance;
					num5 *= num5;
					node = nodes[nodes.Length - 1];
					val2.a = ((float3)(ref node.m_Position)).xz;
					for (int k = 0; k < nodes.Length; k++)
					{
						node = nodes[k];
						val2.b = ((float3)(ref node.m_Position)).xz;
						float num6 = MathUtils.DistanceSquared(val2, ((float3)(ref transform.m_Position)).xz, ref num7);
						if (num6 < num5)
						{
							flag2 = true;
							break;
						}
						if (num6 < num4)
						{
							num4 = num6;
							num2 = num7;
							num3 = k;
						}
						val2.a = val2.b;
					}
				}
				if (!flag2 && num3 >= 0)
				{
					int2 val3 = math.select(new int2(num3 - 1, num3), new int2(num3 - 2, num3 + 1), new bool2(num2 == 0f, num2 == 1f));
					val3 = math.select(val3, val3 + new int2(nodes.Length, -nodes.Length), new bool2(val3.x < 0, val3.y >= nodes.Length));
					val3 = math.select(val3, ((int2)(ref val3)).yx, isCounterClockwise);
					node = nodes[val3.x];
					float2 xz2 = ((float3)(ref node.m_Position)).xz;
					node = nodes[val3.y];
					float2 xz3 = ((float3)(ref node.m_Position)).xz;
					flag2 = math.dot(((float3)(ref transform.m_Position)).xz - xz2, MathUtils.Right(xz3 - xz2)) <= 0f;
				}
				if (flag2)
				{
					Entity val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Original = subObject.m_SubObject
					};
					creationDefinition.m_Flags |= CreationFlags.Delete;
					ObjectDefinition objectDefinition = new ObjectDefinition
					{
						m_ParentMesh = -1,
						m_Position = transform.m_Position,
						m_Rotation = transform.m_Rotation,
						m_LocalPosition = transform.m_Position,
						m_LocalRotation = transform.m_Rotation
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val4, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val4, objectDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val4, default(Updated));
					UpdateSubNets(transform, prefabRef.m_Prefab, subObject.m_SubObject, default(NativeList<ClearAreaData>), removeAll: true);
					UpdateSubAreas(transform, prefabRef.m_Prefab, subObject.m_SubObject, createdEntities, default(NativeList<ClearAreaData>), removeAll: true);
				}
			}
		}

		private OwnerDefinition GetOwnerDefinition(Entity parent, Entity area, NativeParallelHashSet<Entity> createdEntities, bool upgrade, bool fullUpdate, NativeList<ClearAreaData> clearAreas)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			OwnerDefinition result = default(OwnerDefinition);
			if (!m_EditorMode)
			{
				return result;
			}
			Entity val = parent;
			while (m_OwnerData.HasComponent(val) && !m_BuildingData.HasComponent(val))
			{
				val = m_OwnerData[val].m_Owner;
			}
			OwnerDefinition ownerDefinition = default(OwnerDefinition);
			DynamicBuffer<InstalledUpgrade> installedUpgrades = default(DynamicBuffer<InstalledUpgrade>);
			if (m_InstalledUpgrades.TryGetBuffer(val, ref installedUpgrades) && installedUpgrades.Length != 0)
			{
				if (fullUpdate && m_TransformData.HasComponent(val))
				{
					Transform transform = m_TransformData[val];
					ClearAreaHelpers.FillClearAreas(installedUpgrades, area, m_TransformData, m_ClearData, m_PrefabRefData, m_PrefabObjectGeometryData, m_SubAreas, m_Nodes, m_Triangles, ref clearAreas);
					ClearAreaHelpers.InitClearAreas(clearAreas, transform);
					if (createdEntities.Add(val))
					{
						Entity owner = Entity.Null;
						if (m_OwnerData.HasComponent(val))
						{
							owner = m_OwnerData[val].m_Owner;
						}
						UpdateOwnerObject(owner, val, createdEntities, transform, default(OwnerDefinition), upgrade: false, clearAreas);
					}
					ownerDefinition.m_Prefab = m_PrefabRefData[val].m_Prefab;
					ownerDefinition.m_Position = transform.m_Position;
					ownerDefinition.m_Rotation = transform.m_Rotation;
				}
				val = installedUpgrades[0].m_Upgrade;
			}
			if (m_TransformData.HasComponent(val))
			{
				Transform transform2 = m_TransformData[val];
				if (createdEntities.Add(val))
				{
					Entity owner2 = Entity.Null;
					if (ownerDefinition.m_Prefab == Entity.Null && m_OwnerData.HasComponent(val))
					{
						owner2 = m_OwnerData[val].m_Owner;
					}
					UpdateOwnerObject(owner2, val, createdEntities, transform2, ownerDefinition, upgrade, default(NativeList<ClearAreaData>));
				}
				result.m_Prefab = m_PrefabRefData[val].m_Prefab;
				result.m_Position = transform2.m_Position;
				result.m_Rotation = transform2.m_Rotation;
			}
			return result;
		}

		private void UpdateOwnerObject(Entity owner, Entity original, NativeParallelHashSet<Entity> createdEntities, Transform transform, OwnerDefinition ownerDefinition, bool upgrade, NativeList<ClearAreaData> clearAreas)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			Entity prefab = m_PrefabRefData[original].m_Prefab;
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Owner = owner,
				m_Original = original
			};
			if (upgrade)
			{
				creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
			}
			ObjectDefinition objectDefinition = new ObjectDefinition
			{
				m_ParentMesh = -1,
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation
			};
			if (m_TransformData.HasComponent(owner))
			{
				Transform transform2 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(m_TransformData[owner]), transform);
				objectDefinition.m_LocalPosition = transform2.m_Position;
				objectDefinition.m_LocalRotation = transform2.m_Rotation;
			}
			else
			{
				objectDefinition.m_LocalPosition = transform.m_Position;
				objectDefinition.m_LocalRotation = transform.m_Rotation;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
			}
			UpdateSubNets(transform, prefab, original, clearAreas, removeAll: false);
			UpdateSubAreas(transform, prefab, original, createdEntities, clearAreas, removeAll: false);
		}

		private void UpdateSubNets(Transform transform, Entity prefab, Entity original, NativeList<ClearAreaData> clearAreas, bool removeAll)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubNets.HasBuffer(original))
			{
				return;
			}
			DynamicBuffer<Game.Net.SubNet> val = m_SubNets[original];
			Game.Net.Elevation elevation = default(Game.Net.Elevation);
			Game.Net.Elevation elevation2 = default(Game.Net.Elevation);
			for (int i = 0; i < val.Length; i++)
			{
				Entity subNet = val[i].m_SubNet;
				if (m_NodeData.HasComponent(subNet))
				{
					if (!HasEdgeStartOrEnd(subNet, original))
					{
						Game.Net.Node node = m_NodeData[subNet];
						Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
						CreationDefinition creationDefinition = new CreationDefinition
						{
							m_Original = subNet
						};
						if (m_EditorContainerData.HasComponent(subNet))
						{
							creationDefinition.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
						}
						bool onGround = !m_NetElevationData.TryGetComponent(subNet, ref elevation) || math.cmin(math.abs(elevation.m_Elevation)) < 2f;
						if (removeAll)
						{
							creationDefinition.m_Flags |= CreationFlags.Delete;
						}
						else if (ClearAreaHelpers.ShouldClear(clearAreas, node.m_Position, onGround))
						{
							creationDefinition.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
						}
						OwnerDefinition ownerDefinition = new OwnerDefinition
						{
							m_Prefab = prefab,
							m_Position = transform.m_Position,
							m_Rotation = transform.m_Rotation
						};
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
						NetCourse netCourse = new NetCourse
						{
							m_Curve = new Bezier4x3(node.m_Position, node.m_Position, node.m_Position, node.m_Position),
							m_Length = 0f,
							m_FixedIndex = -1,
							m_StartPosition = 
							{
								m_Entity = subNet,
								m_Position = node.m_Position,
								m_Rotation = node.m_Rotation,
								m_CourseDelta = 0f
							},
							m_EndPosition = 
							{
								m_Entity = subNet,
								m_Position = node.m_Position,
								m_Rotation = node.m_Rotation,
								m_CourseDelta = 1f
							}
						};
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, netCourse);
					}
				}
				else if (m_EdgeData.HasComponent(subNet))
				{
					Edge edge = m_EdgeData[subNet];
					Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition2 = new CreationDefinition
					{
						m_Original = subNet
					};
					if (m_EditorContainerData.HasComponent(subNet))
					{
						creationDefinition2.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
					}
					Curve curve = m_CurveData[subNet];
					bool onGround2 = !m_NetElevationData.TryGetComponent(subNet, ref elevation2) || math.cmin(math.abs(elevation2.m_Elevation)) < 2f;
					if (removeAll)
					{
						creationDefinition2.m_Flags |= CreationFlags.Delete;
					}
					else if (ClearAreaHelpers.ShouldClear(clearAreas, curve.m_Bezier, onGround2))
					{
						creationDefinition2.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
					}
					OwnerDefinition ownerDefinition2 = new OwnerDefinition
					{
						m_Prefab = prefab,
						m_Position = transform.m_Position,
						m_Rotation = transform.m_Rotation
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val3, ownerDefinition2);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition2);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
					NetCourse netCourse2 = default(NetCourse);
					netCourse2.m_Curve = curve.m_Bezier;
					netCourse2.m_Length = MathUtils.Length(netCourse2.m_Curve);
					netCourse2.m_FixedIndex = -1;
					netCourse2.m_StartPosition.m_Entity = edge.m_Start;
					netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
					netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse2.m_Curve));
					netCourse2.m_StartPosition.m_CourseDelta = 0f;
					netCourse2.m_EndPosition.m_Entity = edge.m_End;
					netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
					netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse2.m_Curve));
					netCourse2.m_EndPosition.m_CourseDelta = 1f;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse2);
				}
			}
		}

		private bool HasEdgeStartOrEnd(Entity node, Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge = val[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				if ((edge2.m_Start == node || edge2.m_End == node) && m_OwnerData.HasComponent(edge) && m_OwnerData[edge].m_Owner == owner)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateSubAreas(Transform transform, Entity prefab, Entity original, NativeParallelHashSet<Entity> createdEntities, NativeList<ClearAreaData> clearAreas, bool removeAll)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubAreas.HasBuffer(original))
			{
				return;
			}
			DynamicBuffer<Game.Areas.SubArea> val = m_SubAreas[original];
			for (int i = 0; i < val.Length; i++)
			{
				Entity area = val[i].m_Area;
				if (!createdEntities.Add(area))
				{
					continue;
				}
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Original = area
				};
				OwnerDefinition ownerDefinition = new OwnerDefinition
				{
					m_Prefab = prefab,
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
				DynamicBuffer<Game.Areas.Node> nodes = m_Nodes[area];
				if (removeAll)
				{
					creationDefinition.m_Flags |= CreationFlags.Delete;
				}
				else if (m_SpaceData.HasComponent(area))
				{
					DynamicBuffer<Triangle> triangles = m_Triangles[area];
					if (ClearAreaHelpers.ShouldClear(clearAreas, nodes, triangles, transform))
					{
						creationDefinition.m_Flags |= CreationFlags.Delete | CreationFlags.Hidden;
					}
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val2).CopyFrom(nodes.AsNativeArray());
				if (m_CachedNodes.HasBuffer(area))
				{
					DynamicBuffer<LocalNodeCache> val3 = m_CachedNodes[area];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val2).CopyFrom(val3.AsNativeArray());
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Areas.Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Clear> __Game_Areas_Clear_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Space> __Game_Areas_Space_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Area> __Game_Areas_Area_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Areas.Node>(true);
			__Game_Tools_LocalNodeCache_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LocalNodeCache>(true);
			__Game_Areas_Clear_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Clear>(true);
			__Game_Areas_Space_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Space>(true);
			__Game_Areas_Area_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Elevation>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	public const string kToolID = "Area Tool";

	private ObjectToolSystem m_ObjectToolSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private AudioManager m_AudioManager;

	private IProxyAction m_AddAreaNode;

	private IProxyAction m_InsertAreaNode;

	private IProxyAction m_MergeAreaNode;

	private IProxyAction m_MoveAreaNode;

	private IProxyAction m_DeleteAreaNode;

	private IProxyAction m_UndoAreaNode;

	private IProxyAction m_CompleteArea;

	private IProxyAction m_CreateArea;

	private IProxyAction m_DeleteArea;

	private IProxyAction m_DiscardInsertAreaNode;

	private IProxyAction m_DiscardMoveAreaNode;

	private IProxyAction m_DiscardMergeAreaNode;

	private IProxyAction m_CreateAreaOrMoveAreaNode;

	private IProxyAction m_CreateAreaOrInsertAreaNode;

	private bool m_ApplyBlocked;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_TempAreaQuery;

	private EntityQuery m_TempBuildingQuery;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_SoundQuery;

	private ControlPoint m_LastRaycastPoint;

	private NativeList<ControlPoint> m_ControlPoints;

	private NativeList<ControlPoint> m_MoveStartPositions;

	private NativeValue<Tooltip> m_Tooltip;

	private Mode m_LastMode;

	private State m_State;

	private AreaPrefab m_Prefab;

	private bool m_ControlPointsMoved;

	private bool m_AllowCreateArea;

	private bool m_ForceCancel;

	private TypeHandle __TypeHandle;

	public override string toolID => "Area Tool";

	public override int uiModeIndex => (int)actualMode;

	public Mode mode { get; set; }

	public Mode actualMode
	{
		get
		{
			if (!allowGenerate)
			{
				return Mode.Edit;
			}
			return mode;
		}
	}

	public Entity recreate { get; set; }

	public bool underground { get; set; }

	public bool allowGenerate { get; private set; }

	public State state => m_State;

	public Tooltip tooltip => m_Tooltip.value;

	public AreaPrefab prefab
	{
		get
		{
			return m_Prefab;
		}
		set
		{
			if ((Object)(object)value != (Object)(object)m_Prefab)
			{
				m_Prefab = value;
				allowGenerate = m_ToolSystem.actionMode.IsEditor() && value is MapTilePrefab;
				m_ToolSystem.EventPrefabChanged?.Invoke(value);
			}
		}
	}

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_AddAreaNode;
			yield return m_InsertAreaNode;
			yield return m_MergeAreaNode;
			yield return m_MoveAreaNode;
			yield return m_DeleteAreaNode;
			yield return m_UndoAreaNode;
			yield return m_CompleteArea;
			yield return m_CreateArea;
			yield return m_DeleteArea;
			yield return m_DiscardInsertAreaNode;
			yield return m_DiscardMoveAreaNode;
			yield return m_DiscardMergeAreaNode;
			yield return m_CreateAreaOrMoveAreaNode;
			yield return m_CreateAreaOrInsertAreaNode;
		}
	}

	public override void GetUIModes(List<ToolMode> modes)
	{
		modes.Add(new ToolMode(Mode.Edit.ToString(), 0));
		if (allowGenerate)
		{
			modes.Add(new ToolMode(Mode.Generate.ToString(), 1));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ObjectToolSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_DefinitionQuery = GetDefinitionQuery();
		m_TempAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_TempBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Temp>()
		});
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_AddAreaNode = InputManager.instance.toolActionCollection.GetActionState("Add Area Node", "AreaToolSystem");
		m_InsertAreaNode = InputManager.instance.toolActionCollection.GetActionState("Insert Area Node", "AreaToolSystem");
		m_MergeAreaNode = InputManager.instance.toolActionCollection.GetActionState("Merge Area Node", "AreaToolSystem");
		m_MoveAreaNode = InputManager.instance.toolActionCollection.GetActionState("Move Area Node", "AreaToolSystem");
		m_DeleteAreaNode = InputManager.instance.toolActionCollection.GetActionState("Delete Area Node", "AreaToolSystem");
		m_UndoAreaNode = InputManager.instance.toolActionCollection.GetActionState("Undo Area Node", "AreaToolSystem");
		m_CompleteArea = InputManager.instance.toolActionCollection.GetActionState("Complete Area", "AreaToolSystem");
		m_CreateArea = InputManager.instance.toolActionCollection.GetActionState("Create Area", "AreaToolSystem");
		m_DeleteArea = InputManager.instance.toolActionCollection.GetActionState("Delete Area", "AreaToolSystem");
		m_DiscardInsertAreaNode = InputManager.instance.toolActionCollection.GetActionState("Discard Insert Area Node", "AreaToolSystem");
		m_DiscardMoveAreaNode = InputManager.instance.toolActionCollection.GetActionState("Discard Move Area Node", "AreaToolSystem");
		m_DiscardMergeAreaNode = InputManager.instance.toolActionCollection.GetActionState("Discard Merge Area Node", "AreaToolSystem");
		m_CreateAreaOrMoveAreaNode = InputManager.instance.toolActionCollection.GetActionState("Create Area Or Move Area Node", "AreaToolSystem");
		m_CreateAreaOrInsertAreaNode = InputManager.instance.toolActionCollection.GetActionState("Create Area Or Insert Area Node", "AreaToolSystem");
		selectedSnap &= ~Snap.AutoParent;
		m_ControlPoints = new NativeList<ControlPoint>(20, AllocatorHandle.op_Implicit((Allocator)4));
		m_MoveStartPositions = new NativeList<ControlPoint>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_Tooltip = new NativeValue<Tooltip>((Allocator)4);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ControlPoints.Dispose();
		m_MoveStartPositions.Dispose();
		m_Tooltip.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		base.OnStartRunning();
		m_ControlPoints.Clear();
		m_MoveStartPositions.Clear();
		m_LastRaycastPoint = default(ControlPoint);
		m_LastMode = actualMode;
		m_State = State.Default;
		m_Tooltip.value = Tooltip.None;
		m_AllowCreateArea = false;
		m_ForceCancel = false;
		m_ApplyBlocked = false;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		recreate = Entity.Null;
		base.OnStopRunning();
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			UpdateApplyAction();
			UpdateSecondaryApplyAction();
			UpdateCancelAction();
		}
	}

	private void UpdateApplyAction()
	{
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		ControlPoint controlPoint;
		Game.Areas.Node node;
		switch (state)
		{
		case State.Default:
			if (m_ControlPoints.Length >= 1)
			{
				controlPoint = m_ControlPoints[0];
				if (!controlPoint.Equals(default(ControlPoint)))
				{
					Area area2 = default(Area);
					DynamicBuffer<Game.Areas.Node> val2 = default(DynamicBuffer<Game.Areas.Node>);
					EntityManager entityManager;
					for (int k = 0; k < m_ControlPoints.Length; k++)
					{
						if (!EntitiesExtensions.TryGetComponent<Area>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[k].m_OriginalEntity, ref area2) || (area2.m_Flags & AreaFlags.Complete) == 0 || !EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[k].m_OriginalEntity, true, ref val2))
						{
							continue;
						}
						for (int l = 0; l < val2.Length; l++)
						{
							node = val2[l];
							if (((float3)(ref node.m_Position)).Equals(m_ControlPoints[k].m_Position))
							{
								base.applyAction.enabled = base.actionsEnabled;
								entityManager = ((ComponentSystemBase)this).EntityManager;
								base.applyActionOverride = (((EntityManager)(ref entityManager)).HasComponent<Game.Areas.Lot>(m_ControlPoints[k].m_OriginalEntity) ? m_MoveAreaNode : m_CreateAreaOrMoveAreaNode);
								return;
							}
						}
						base.applyAction.enabled = base.actionsEnabled;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						base.applyActionOverride = (((EntityManager)(ref entityManager)).HasComponent<Game.Areas.Lot>(m_ControlPoints[k].m_OriginalEntity) ? m_InsertAreaNode : m_CreateAreaOrInsertAreaNode);
						return;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Areas.Node>(m_ControlPoints[0].m_OriginalEntity) || !math.any(m_ControlPoints[0].m_ElementIndex >= 0))
					{
						base.applyAction.enabled = base.actionsEnabled;
						base.applyActionOverride = m_CreateArea;
					}
					else
					{
						base.applyAction.enabled = false;
						base.applyActionOverride = null;
					}
					break;
				}
			}
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		case State.Create:
		{
			ref NativeList<ControlPoint> reference = ref m_ControlPoints;
			controlPoint = reference[reference.Length - 1];
			if (controlPoint.Equals(default(ControlPoint)))
			{
				base.applyAction.enabled = base.actionsEnabled;
				base.applyActionOverride = null;
				break;
			}
			ref NativeList<ControlPoint> reference2 = ref m_ControlPoints;
			controlPoint = reference2[reference2.Length - 1];
			ref float3 position = ref controlPoint.m_Position;
			ref NativeList<ControlPoint> reference3 = ref m_ControlPoints;
			if (!((float3)(ref position)).Equals(reference3[reference3.Length - 2].m_Position))
			{
				ref NativeList<ControlPoint> reference4 = ref m_ControlPoints;
				controlPoint = reference4[reference4.Length - 1];
				if (!((float3)(ref controlPoint.m_Position)).Equals(m_ControlPoints[0].m_Position))
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = (GetAllowApply() ? m_AddAreaNode : null);
					break;
				}
			}
			if (m_ControlPoints.Length >= 3)
			{
				ref NativeList<ControlPoint> reference5 = ref m_ControlPoints;
				controlPoint = reference5[reference5.Length - 1];
				if (((float3)(ref controlPoint.m_Position)).Equals(m_ControlPoints[0].m_Position))
				{
					base.applyAction.enabled = base.actionsEnabled;
					base.applyActionOverride = (GetAllowApply() ? m_CompleteArea : null);
					break;
				}
			}
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		}
		case State.Modify:
		{
			Area area = default(Area);
			DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
			for (int i = 0; i < m_MoveStartPositions.Length; i++)
			{
				if (!EntitiesExtensions.TryGetComponent<Area>(((ComponentSystemBase)this).EntityManager, m_MoveStartPositions[i].m_OriginalEntity, ref area) || (area.m_Flags & AreaFlags.Complete) == 0 || !EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, m_MoveStartPositions[i].m_OriginalEntity, true, ref val))
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					node = val[j];
					if (!((float3)(ref node.m_Position)).Equals(m_MoveStartPositions[i].m_Position))
					{
						node = val[j];
						if (((float3)(ref node.m_Position)).Equals(m_ControlPoints[0].m_Position))
						{
							base.applyAction.enabled = base.actionsEnabled;
							base.applyActionOverride = m_MergeAreaNode;
							return;
						}
					}
					node = val[j];
					if (((float3)(ref node.m_Position)).Equals(m_MoveStartPositions[i].m_Position))
					{
						base.applyAction.enabled = base.actionsEnabled;
						base.applyActionOverride = m_MoveAreaNode;
						return;
					}
				}
				base.applyAction.enabled = base.actionsEnabled;
				base.applyActionOverride = m_InsertAreaNode;
				return;
			}
			base.applyAction.enabled = false;
			base.applyActionOverride = null;
			break;
		}
		case State.Remove:
			base.applyAction.enabled = base.actionsEnabled;
			base.applyActionOverride = null;
			break;
		default:
			base.applyAction.enabled = false;
			base.applyActionOverride = null;
			break;
		}
	}

	private void UpdateSecondaryApplyAction()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		switch (state)
		{
		case State.Default:
		{
			Area area = default(Area);
			DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
			if (m_ControlPoints.Length == 1 && EntitiesExtensions.TryGetComponent<Area>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, ref area) && (area.m_Flags & AreaFlags.Complete) != 0 && EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, m_ControlPoints[0].m_OriginalEntity, true, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Game.Areas.Node node = val[i];
					if (!((float3)(ref node.m_Position)).Equals(m_ControlPoints[0].m_Position))
					{
						continue;
					}
					if (val.Length > 3)
					{
						base.secondaryApplyAction.enabled = base.actionsEnabled;
						base.secondaryApplyActionOverride = m_DeleteAreaNode;
						return;
					}
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Owner>(m_ControlPoints[0].m_OriginalEntity))
					{
						base.secondaryApplyAction.enabled = base.actionsEnabled;
						base.secondaryApplyActionOverride = m_DeleteArea;
					}
					else
					{
						base.secondaryApplyAction.enabled = false;
						base.secondaryApplyActionOverride = null;
					}
					return;
				}
			}
			base.secondaryApplyAction.enabled = false;
			base.secondaryApplyActionOverride = null;
			break;
		}
		case State.Create:
			if (m_ControlPoints.Length > 1)
			{
				base.secondaryApplyAction.enabled = base.actionsEnabled;
				base.secondaryApplyActionOverride = m_UndoAreaNode;
			}
			else
			{
				base.secondaryApplyAction.enabled = base.actionsEnabled;
				base.secondaryApplyActionOverride = null;
			}
			break;
		case State.Remove:
			base.secondaryApplyAction.enabled = base.actionsEnabled;
			base.secondaryApplyActionOverride = m_DeleteAreaNode;
			break;
		default:
			base.secondaryApplyAction.enabled = false;
			base.secondaryApplyActionOverride = null;
			break;
		}
	}

	private void UpdateCancelAction()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (state == State.Modify)
		{
			Area area = default(Area);
			DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
			for (int i = 0; i < m_MoveStartPositions.Length; i++)
			{
				if (!EntitiesExtensions.TryGetComponent<Area>(((ComponentSystemBase)this).EntityManager, m_MoveStartPositions[i].m_OriginalEntity, ref area) || (area.m_Flags & AreaFlags.Complete) == 0 || !EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, m_MoveStartPositions[i].m_OriginalEntity, true, ref val))
				{
					continue;
				}
				for (int j = 0; j < val.Length; j++)
				{
					Game.Areas.Node node = val[j];
					if (!((float3)(ref node.m_Position)).Equals(m_MoveStartPositions[i].m_Position))
					{
						node = val[j];
						if (((float3)(ref node.m_Position)).Equals(m_ControlPoints[0].m_Position))
						{
							base.cancelAction.enabled = base.actionsEnabled;
							base.cancelActionOverride = m_DiscardMergeAreaNode;
							return;
						}
					}
					node = val[j];
					if (((float3)(ref node.m_Position)).Equals(m_MoveStartPositions[i].m_Position))
					{
						base.cancelAction.enabled = base.actionsEnabled;
						base.cancelActionOverride = m_DiscardMoveAreaNode;
						return;
					}
				}
				base.cancelAction.enabled = base.actionsEnabled;
				base.cancelActionOverride = m_DiscardInsertAreaNode;
				return;
			}
			base.cancelAction.enabled = false;
			base.cancelActionOverride = null;
		}
		else
		{
			base.cancelAction.enabled = false;
			base.cancelActionOverride = null;
		}
	}

	public NativeList<ControlPoint> GetControlPoints(out NativeList<ControlPoint> moveStartPositions, out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		moveStartPositions = m_MoveStartPositions;
		dependencies = ((SystemBase)this).Dependency;
		return m_ControlPoints;
	}

	public override PrefabBase GetPrefab()
	{
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is AreaPrefab areaPrefab)
		{
			this.prefab = areaPrefab;
			return true;
		}
		return false;
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
		base.InitializeRaycast();
		if ((Object)(object)prefab != (Object)null)
		{
			AreaGeometryData componentData = m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab);
			GetAvailableSnapMask(out var onMask, out var offMask);
			Snap actualSnap = ToolBaseSystem.GetActualSnap(selectedSnap, onMask, offMask);
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Areas;
			m_ToolRaycastSystem.areaTypeMask = AreaUtils.GetTypeMask(componentData.m_Type);
			if ((componentData.m_Flags & Game.Areas.GeometryFlags.OnWaterSurface) != 0)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Water;
			}
			if ((actualSnap & Snap.ObjectSurface) != Snap.None)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.StaticObjects;
				if (m_ToolSystem.actionMode.IsEditor())
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Placeholders;
				}
				if (underground)
				{
					m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
					m_ToolRaycastSystem.typeMask &= ~(TypeMask.Terrain | TypeMask.Water);
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.PartialSurface;
				}
			}
			if ((actualSnap & Snap.ExistingGeometry) == 0 && m_State != State.Default)
			{
				m_ToolRaycastSystem.typeMask &= ~TypeMask.Areas;
			}
		}
		else
		{
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Areas;
			m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.None;
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.UpgradeIsMain;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		if (m_FocusChanged)
		{
			return inputDeps;
		}
		if (actualMode != m_LastMode)
		{
			m_ControlPoints.Clear();
			m_MoveStartPositions.Clear();
			m_LastRaycastPoint = default(ControlPoint);
			m_LastMode = actualMode;
			m_State = State.Default;
			m_Tooltip.value = Tooltip.None;
			m_AllowCreateArea = false;
		}
		bool flag = m_ForceCancel;
		m_ForceCancel = false;
		DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
		if (EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, recreate, true, ref val))
		{
			m_State = State.Create;
			if (m_ControlPoints.Length < 3 && val.Length >= 2)
			{
				ref NativeList<ControlPoint> reference = ref m_ControlPoints;
				ControlPoint controlPoint = new ControlPoint
				{
					m_OriginalEntity = recreate,
					m_ElementIndex = new int2(0, -1),
					m_Position = val[0].m_Position,
					m_HitPosition = val[0].m_Position
				};
				reference.Add(ref controlPoint);
				ref NativeList<ControlPoint> reference2 = ref m_ControlPoints;
				controlPoint = new ControlPoint
				{
					m_OriginalEntity = recreate,
					m_ElementIndex = new int2(1, -1),
					m_Position = val[1].m_Position,
					m_HitPosition = val[1].m_Position
				};
				reference2.Add(ref controlPoint);
				ref NativeList<ControlPoint> reference3 = ref m_ControlPoints;
				controlPoint = new ControlPoint
				{
					m_ElementIndex = new int2(-1, -1),
					m_Position = math.lerp(val[0].m_Position, val[1].m_Position, 0.5f),
					m_HitPosition = math.lerp(val[0].m_Position, val[1].m_Position, 0.5f)
				};
				reference3.Add(ref controlPoint);
			}
		}
		UpdateActions();
		if ((Object)(object)prefab != (Object)null)
		{
			AreaGeometryData componentData = m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab);
			base.requireAreas = AreaUtils.GetTypeMask(componentData.m_Type);
			base.requireZones = componentData.m_Type == Game.Areas.AreaType.Lot;
			base.requireNet = Layer.None;
			if ((componentData.m_Flags & Game.Areas.GeometryFlags.PhysicalGeometry) != 0 && (componentData.m_Flags & Game.Areas.GeometryFlags.OnWaterSurface) != 0)
			{
				base.requireNet |= Layer.Waterway;
			}
			m_AllowCreateArea = (m_ToolSystem.actionMode.IsEditor() || componentData.m_Type != Game.Areas.AreaType.Lot) && (componentData.m_Type != Game.Areas.AreaType.Surface || (componentData.m_Flags & Game.Areas.GeometryFlags.ClipTerrain) != 0 || m_PrefabSystem.HasComponent<RenderedAreaData>(prefab));
			Entity val2 = Entity.Null;
			Owner owner = default(Owner);
			PrefabRef prefabRef = default(PrefabRef);
			if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, recreate, ref owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref prefabRef))
			{
				val2 = prefabRef.m_Prefab;
			}
			else if (!m_ToolSystem.actionMode.IsEditor())
			{
				val2 = m_PrefabSystem.GetEntity(prefab);
			}
			UpdateInfoview(val2);
			GetAvailableSnapMask(componentData, m_ToolSystem.actionMode.IsEditor(), out m_SnapOnMask, out m_SnapOffMask);
			allowUnderground = (ToolBaseSystem.GetActualSnap(selectedSnap, m_SnapOnMask, m_SnapOffMask) & Snap.ObjectSurface) != 0;
			base.requireUnderground = allowUnderground && underground;
			if (m_State != State.Default && base.actionsEnabled && !base.applyAction.enabled)
			{
				m_State = State.Default;
				return Clear(inputDeps);
			}
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				switch (m_State)
				{
				case State.Default:
				case State.Create:
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
				case State.Modify:
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
				case State.Remove:
					if (flag || base.cancelAction.WasPressedThisFrame())
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
		}
		else
		{
			base.requireAreas = AreaTypeMask.None;
			base.requireZones = false;
			base.requireNet = Layer.None;
			base.requireUnderground = false;
			m_AllowCreateArea = false;
			allowUnderground = false;
			UpdateInfoview(Entity.Null);
		}
		if (m_State == State.Modify && base.applyAction.WasReleasedThisFrame())
		{
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				return Cancel(inputDeps);
			}
			m_ControlPoints.Clear();
			m_State = State.Default;
		}
		else if (m_State == State.Remove && base.secondaryApplyAction.WasReleasedThisFrame())
		{
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				return Apply(inputDeps);
			}
			m_ControlPoints.Clear();
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		if ((Object)(object)prefab != (Object)null)
		{
			GetAvailableSnapMask(m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab), m_ToolSystem.actionMode.IsEditor(), out onMask, out offMask);
		}
		else
		{
			base.GetAvailableSnapMask(out onMask, out offMask);
		}
	}

	private static void GetAvailableSnapMask(AreaGeometryData prefabAreaData, bool editorMode, out Snap onMask, out Snap offMask)
	{
		onMask = Snap.ExistingGeometry | Snap.StraightDirection;
		offMask = onMask;
		switch (prefabAreaData.m_Type)
		{
		case Game.Areas.AreaType.Lot:
			onMask |= Snap.NetSide | Snap.ObjectSide;
			offMask |= Snap.NetSide | Snap.ObjectSide;
			if (editorMode)
			{
				onMask |= Snap.LotGrid | Snap.AutoParent;
				offMask |= Snap.LotGrid | Snap.AutoParent;
			}
			break;
		case Game.Areas.AreaType.District:
			onMask |= Snap.NetMiddle;
			offMask |= Snap.NetMiddle;
			break;
		case Game.Areas.AreaType.Space:
			onMask |= Snap.NetSide | Snap.ObjectSide | Snap.ObjectSurface;
			offMask |= Snap.NetSide | Snap.ObjectSide | Snap.ObjectSurface;
			if (editorMode)
			{
				onMask |= Snap.LotGrid | Snap.AutoParent;
				offMask |= Snap.LotGrid | Snap.AutoParent;
			}
			break;
		case Game.Areas.AreaType.Surface:
			onMask |= Snap.NetSide | Snap.ObjectSide;
			offMask |= Snap.NetSide | Snap.ObjectSide;
			if (editorMode)
			{
				onMask |= Snap.LotGrid | Snap.AutoParent;
				offMask |= Snap.LotGrid | Snap.AutoParent;
			}
			break;
		case Game.Areas.AreaType.MapTile:
			break;
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
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
			if (actualMode == Mode.Generate)
			{
				return Update(inputDeps, fullUpdate: false);
			}
			if (GetAllowApply() && m_ControlPoints.Length > 0)
			{
				base.applyMode = ApplyMode.Clear;
				ControlPoint controlPoint2 = m_ControlPoints[0];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Area>(controlPoint2.m_OriginalEntity) && controlPoint2.m_ElementIndex.x >= 0)
				{
					DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
					if (EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, controlPoint2.m_OriginalEntity, true, ref val) && val.Length <= 3)
					{
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDeleteAreaSound);
					}
					else
					{
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolRemovePointSound);
					}
					m_State = State.Remove;
					m_ControlPointsMoved = false;
					m_ForceCancel = singleFrameOnly;
					m_MoveStartPositions.Clear();
					m_MoveStartPositions.AddRange(m_ControlPoints.AsArray());
					m_ControlPoints.Clear();
					if (GetRaycastResult(out var controlPoint3))
					{
						m_LastRaycastPoint = controlPoint3;
						m_ControlPoints.Add(ref controlPoint3);
						inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
						inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
					}
					else
					{
						m_ControlPoints.Add(ref controlPoint2);
					}
					return inputDeps;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Create:
		{
			m_ControlPoints.RemoveAtSwapBack(m_ControlPoints.Length - 1);
			base.applyMode = ApplyMode.Clear;
			if (m_ControlPoints.Length <= 1)
			{
				m_State = State.Default;
			}
			if (recreate != Entity.Null && m_ControlPoints.Length <= 2)
			{
				m_ToolSystem.activeTool = m_ObjectToolSystem;
				return inputDeps;
			}
			m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolRemovePointSound);
			if (GetRaycastResult(out var controlPoint5))
			{
				m_LastRaycastPoint = controlPoint5;
				m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint5;
				inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
				inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			else if (m_ControlPoints.Length >= 2)
			{
				m_ControlPoints[m_ControlPoints.Length - 1] = m_ControlPoints[m_ControlPoints.Length - 2];
				inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			return inputDeps;
		}
		case State.Modify:
		{
			m_ControlPoints.Clear();
			base.applyMode = ApplyMode.Clear;
			m_State = State.Default;
			if (GetRaycastResult(out var controlPoint4))
			{
				m_LastRaycastPoint = controlPoint4;
				m_ControlPoints.Add(ref controlPoint4);
				inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
				inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			return inputDeps;
		}
		case State.Remove:
		{
			NativeArray<Entity> applyTempAreas = default(NativeArray<Entity>);
			NativeArray<Entity> applyTempBuildings = default(NativeArray<Entity>);
			if (GetAllowApply() && !((EntityQuery)(ref m_TempAreaQuery)).IsEmptyIgnoreFilter)
			{
				base.applyMode = ApplyMode.Apply;
				applyTempAreas = ((EntityQuery)(ref m_TempAreaQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
				applyTempBuildings = ((EntityQuery)(ref m_TempBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			}
			else
			{
				base.applyMode = ApplyMode.Clear;
			}
			m_State = State.Default;
			m_ControlPoints.Clear();
			if (GetRaycastResult(out var controlPoint))
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps, applyTempAreas);
				inputDeps = UpdateDefinitions(inputDeps, applyTempAreas, applyTempBuildings);
			}
			if (applyTempAreas.IsCreated)
			{
				applyTempAreas.Dispose(inputDeps);
			}
			if (applyTempBuildings.IsCreated)
			{
				applyTempBuildings.Dispose(inputDeps);
			}
			return inputDeps;
		}
		default:
			return Update(inputDeps, fullUpdate: false);
		}
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		switch (m_State)
		{
		case State.Default:
			if (actualMode == Mode.Generate)
			{
				if (GetAllowApply() && !((EntityQuery)(ref m_TempAreaQuery)).IsEmptyIgnoreFilter)
				{
					NativeArray<Entity> applyTempAreas = ((EntityQuery)(ref m_TempAreaQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
					base.applyMode = ApplyMode.Apply;
					m_ControlPoints.Clear();
					if (GetRaycastResult(out var controlPoint2))
					{
						m_LastRaycastPoint = controlPoint2;
						m_ControlPoints.Add(ref controlPoint2);
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
						inputDeps = SnapControlPoints(inputDeps, applyTempAreas);
						inputDeps = UpdateDefinitions(inputDeps, applyTempAreas, default(NativeArray<Entity>));
					}
					if (applyTempAreas.IsCreated)
					{
						applyTempAreas.Dispose(inputDeps);
					}
					return inputDeps;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			if (m_ControlPoints.Length > 0)
			{
				base.applyMode = ApplyMode.Clear;
				ControlPoint controlPoint3 = m_ControlPoints[0];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Area>(controlPoint3.m_OriginalEntity) && math.any(controlPoint3.m_ElementIndex >= 0) && !singleFrameOnly)
				{
					m_State = State.Modify;
					m_ControlPointsMoved = false;
					m_MoveStartPositions.Clear();
					m_MoveStartPositions.AddRange(m_ControlPoints.AsArray());
					m_ControlPoints.Clear();
					if (GetRaycastResult(out var controlPoint4))
					{
						m_LastRaycastPoint = controlPoint4;
						m_ControlPoints.Add(ref controlPoint4);
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolSelectPointSound);
						inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
						JobHandle.ScheduleBatchedJobs();
						((JobHandle)(ref inputDeps)).Complete();
						ControlPoint other = m_ControlPoints[0];
						if (!m_MoveStartPositions[0].Equals(other))
						{
							float minNodeDistance = AreaUtils.GetMinNodeDistance(m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab));
							if (math.distance(m_MoveStartPositions[0].m_Position, other.m_Position) < minNodeDistance * 0.5f)
							{
								m_ControlPoints[0] = m_MoveStartPositions[0];
							}
							else
							{
								m_ControlPointsMoved = true;
							}
						}
						inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
					}
					else
					{
						m_ControlPoints.Add(ref controlPoint3);
					}
					return inputDeps;
				}
				if (GetAllowApply() && !controlPoint3.Equals(default(ControlPoint)) && m_AllowCreateArea)
				{
					m_State = State.Create;
					m_MoveStartPositions.Clear();
					m_ControlPoints.Clear();
					m_ControlPoints.Add(ref controlPoint3);
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
					if (GetRaycastResult(out var controlPoint5))
					{
						m_LastRaycastPoint = controlPoint5;
						m_ControlPoints.Add(ref controlPoint5);
						inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
						inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
					}
					else
					{
						m_ControlPoints.Add(ref controlPoint3);
					}
					return inputDeps;
				}
				return Update(inputDeps, fullUpdate: false);
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Create:
			if (!((EntityQuery)(ref m_TempAreaQuery)).IsEmptyIgnoreFilter)
			{
				if (!GetAllowApply())
				{
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PlaceBuildingFailSound);
				}
				else
				{
					AreaGeometryData componentData = m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab);
					float num = math.distance(m_ControlPoints[m_ControlPoints.Length - 2].m_Position, m_ControlPoints[m_ControlPoints.Length - 1].m_Position);
					float minNodeDistance2 = AreaUtils.GetMinNodeDistance(componentData);
					if (num >= minNodeDistance2)
					{
						bool flag = true;
						NativeArray<Area> val = ((EntityQuery)(ref m_TempAreaQuery)).ToComponentDataArray<Area>(AllocatorHandle.op_Implicit((Allocator)3));
						for (int i = 0; i < val.Length; i++)
						{
							flag &= (val[i].m_Flags & AreaFlags.Complete) != 0;
						}
						val.Dispose();
						NativeArray<Entity> applyTempAreas2 = default(NativeArray<Entity>);
						if (flag)
						{
							m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolFinishAreaSound);
							base.applyMode = ApplyMode.Apply;
							m_State = State.Default;
							m_ControlPoints.Clear();
							if (recreate != Entity.Null)
							{
								if (m_ObjectToolSystem.mode == ObjectToolSystem.Mode.Move)
								{
									m_ToolSystem.activeTool = m_DefaultToolSystem;
								}
								else
								{
									m_ToolSystem.activeTool = m_ObjectToolSystem;
								}
								return inputDeps;
							}
							applyTempAreas2 = ((EntityQuery)(ref m_TempAreaQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
						}
						else
						{
							m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
							base.applyMode = ApplyMode.Clear;
						}
						if (GetRaycastResult(out var controlPoint6))
						{
							m_LastRaycastPoint = controlPoint6;
							m_ControlPoints.Add(ref controlPoint6);
							inputDeps = SnapControlPoints(inputDeps, applyTempAreas2);
							inputDeps = UpdateDefinitions(inputDeps, applyTempAreas2, default(NativeArray<Entity>));
						}
						if (applyTempAreas2.IsCreated)
						{
							applyTempAreas2.Dispose(inputDeps);
						}
						return inputDeps;
					}
				}
			}
			return Update(inputDeps, fullUpdate: false);
		case State.Modify:
		{
			if (!m_ControlPointsMoved && GetAllowApply() && m_ControlPoints.Length > 0)
			{
				if (m_AllowCreateArea)
				{
					ControlPoint controlPoint7 = m_ControlPoints[0];
					base.applyMode = ApplyMode.Clear;
					m_State = State.Create;
					m_MoveStartPositions.Clear();
					m_ControlPoints.Clear();
					m_ControlPoints.Add(ref controlPoint7);
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
					if (GetRaycastResult(out var controlPoint8))
					{
						m_LastRaycastPoint = controlPoint8;
						m_ControlPoints.Add(ref controlPoint8);
						inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
						inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
					}
					else
					{
						m_ControlPoints.Add(ref controlPoint7);
					}
					return inputDeps;
				}
				base.applyMode = ApplyMode.Clear;
				m_State = State.Default;
				m_ControlPoints.Clear();
				if (GetRaycastResult(out var controlPoint9))
				{
					m_LastRaycastPoint = controlPoint9;
					m_ControlPoints.Add(ref controlPoint9);
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
					inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
					inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
				}
				return inputDeps;
			}
			NativeArray<Entity> applyTempAreas3 = default(NativeArray<Entity>);
			NativeArray<Entity> applyTempBuildings = default(NativeArray<Entity>);
			if (GetAllowApply() && !((EntityQuery)(ref m_TempAreaQuery)).IsEmptyIgnoreFilter)
			{
				base.applyMode = ApplyMode.Apply;
				applyTempAreas3 = ((EntityQuery)(ref m_TempAreaQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
				applyTempBuildings = ((EntityQuery)(ref m_TempBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			}
			else
			{
				base.applyMode = ApplyMode.Clear;
			}
			m_State = State.Default;
			m_ControlPoints.Clear();
			if (GetRaycastResult(out var controlPoint10))
			{
				m_LastRaycastPoint = controlPoint10;
				m_ControlPoints.Add(ref controlPoint10);
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolDropPointSound);
				inputDeps = SnapControlPoints(inputDeps, applyTempAreas3);
				inputDeps = UpdateDefinitions(inputDeps, applyTempAreas3, applyTempBuildings);
			}
			if (applyTempAreas3.IsCreated)
			{
				applyTempAreas3.Dispose(inputDeps);
			}
			if (applyTempBuildings.IsCreated)
			{
				applyTempBuildings.Dispose(inputDeps);
			}
			return inputDeps;
		}
		case State.Remove:
		{
			m_ControlPoints.Clear();
			base.applyMode = ApplyMode.Clear;
			m_State = State.Default;
			if (GetRaycastResult(out var controlPoint))
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PolygonToolRemovePointSound);
				inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
				inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			return inputDeps;
		}
		default:
			return Update(inputDeps, fullUpdate: false);
		}
	}

	private JobHandle Update(JobHandle inputDeps, bool fullUpdate)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
		{
			forceUpdate = forceUpdate || fullUpdate;
			if (m_ControlPoints.Length == 0)
			{
				m_LastRaycastPoint = controlPoint;
				m_ControlPoints.Add(ref controlPoint);
				inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
				base.applyMode = ApplyMode.Clear;
				return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			if (m_LastRaycastPoint.Equals(controlPoint) && !forceUpdate)
			{
				base.applyMode = ApplyMode.None;
				return inputDeps;
			}
			m_LastRaycastPoint = controlPoint;
			int num = math.select(0, m_ControlPoints.Length - 1, m_State == State.Create);
			ControlPoint controlPoint2 = m_ControlPoints[num];
			m_ControlPoints[num] = controlPoint;
			inputDeps = SnapControlPoints(inputDeps, default(NativeArray<Entity>));
			JobHandle.ScheduleBatchedJobs();
			((JobHandle)(ref inputDeps)).Complete();
			ControlPoint other = m_ControlPoints[num];
			if (controlPoint2.EqualsIgnoreHit(other))
			{
				base.applyMode = ApplyMode.None;
			}
			else
			{
				float minNodeDistance = AreaUtils.GetMinNodeDistance(m_PrefabSystem.GetComponentData<AreaGeometryData>((PrefabBase)prefab));
				if (m_State == State.Modify && !m_ControlPointsMoved && math.distance(controlPoint2.m_Position, other.m_Position) < minNodeDistance * 0.5f)
				{
					m_ControlPoints[num] = controlPoint2;
					base.applyMode = ApplyMode.None;
				}
				else
				{
					m_ControlPointsMoved = true;
					base.applyMode = ApplyMode.Clear;
					inputDeps = UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
				}
			}
			return inputDeps;
		}
		if (m_LastRaycastPoint.Equals(controlPoint))
		{
			if (forceUpdate || fullUpdate)
			{
				base.applyMode = ApplyMode.Clear;
				return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
			}
			base.applyMode = ApplyMode.None;
			return inputDeps;
		}
		m_LastRaycastPoint = controlPoint;
		if (m_State == State.Default && m_ControlPoints.Length >= 1)
		{
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints.Clear();
			ref NativeList<ControlPoint> reference = ref m_ControlPoints;
			ControlPoint controlPoint3 = default(ControlPoint);
			reference.Add(ref controlPoint3);
			return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
		}
		if (m_State == State.Modify && m_ControlPoints.Length >= 1)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[0] = m_MoveStartPositions[0];
			return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
		}
		if (m_State == State.Remove && m_ControlPoints.Length >= 1)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[0] = m_MoveStartPositions[0];
			return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
		}
		if (m_ControlPoints.Length >= 2)
		{
			m_ControlPointsMoved = true;
			base.applyMode = ApplyMode.Clear;
			m_ControlPoints[m_ControlPoints.Length - 1] = m_ControlPoints[m_ControlPoints.Length - 2];
			return UpdateDefinitions(inputDeps, default(NativeArray<Entity>), default(NativeArray<Entity>));
		}
		return inputDeps;
	}

	private JobHandle SnapControlPoints(JobHandle inputDeps, NativeArray<Entity> applyTempAreas)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		SnapJob obj = new SnapJob
		{
			m_AllowCreateArea = m_AllowCreateArea,
			m_ControlPointsMoved = m_ControlPointsMoved,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_Snap = GetActualSnap(),
			m_State = m_State,
			m_Prefab = m_PrefabSystem.GetEntity(prefab),
			m_ApplyTempAreas = applyTempAreas,
			m_MoveStartPositions = m_MoveStartPositions,
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabAreaData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LotData = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Nodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CachedNodes = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies3),
			m_ControlPoints = m_ControlPoints
		};
		inputDeps = JobHandle.CombineDependencies(inputDeps, JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3));
		JobHandle val = IJobExtensions.Schedule<SnapJob>(obj, inputDeps);
		m_AreaSearchSystem.AddSearchTreeReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		return val;
	}

	private JobHandle UpdateDefinitions(JobHandle inputDeps, NativeArray<Entity> applyTempAreas, NativeArray<Entity> applyTempBuildings)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if ((Object)(object)prefab != (Object)null)
		{
			if (mode == Mode.Generate)
			{
				RemoveMapTilesJob removeMapTilesJob = new RemoveMapTilesJob
				{
					m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CacheType = InternalCompilerInterface.GetBufferTypeHandle<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_ControlPoints = m_ControlPoints
				};
				EntityCommandBuffer val2 = m_ToolOutputBarrier.CreateCommandBuffer();
				removeMapTilesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
				JobHandle val3 = JobChunkExtensions.ScheduleParallel<RemoveMapTilesJob>(removeMapTilesJob, m_MapTileQuery, inputDeps);
				((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val3);
				val = JobHandle.CombineDependencies(val, val3);
			}
			JobHandle val4 = IJobExtensions.Schedule<CreateDefinitionsJob>(new CreateDefinitionsJob
			{
				m_AllowCreateArea = m_AllowCreateArea,
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_Mode = actualMode,
				m_State = m_State,
				m_Prefab = m_PrefabSystem.GetEntity(prefab),
				m_Recreate = recreate,
				m_ApplyTempAreas = applyTempAreas,
				m_ApplyTempBuildings = applyTempBuildings,
				m_MoveStartPositions = m_MoveStartPositions,
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ClearData = InternalCompilerInterface.GetComponentLookup<Clear>(ref __TypeHandle.__Game_Areas_Clear_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpaceData = InternalCompilerInterface.GetComponentLookup<Space>(ref __TypeHandle.__Game_Areas_Space_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetElevationData = InternalCompilerInterface.GetComponentLookup<Game.Net.Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EditorContainerData = InternalCompilerInterface.GetComponentLookup<EditorContainer>(ref __TypeHandle.__Game_Tools_EditorContainer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAreaData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CachedNodes = InternalCompilerInterface.GetBufferLookup<LocalNodeCache>(ref __TypeHandle.__Game_Tools_LocalNodeCache_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ControlPoints = m_ControlPoints,
				m_Tooltip = m_Tooltip,
				m_CommandBuffer = m_ToolOutputBarrier.CreateCommandBuffer()
			}, inputDeps);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val4);
			val = JobHandle.CombineDependencies(val, val4);
		}
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
	public AreaToolSystem()
	{
	}
}
