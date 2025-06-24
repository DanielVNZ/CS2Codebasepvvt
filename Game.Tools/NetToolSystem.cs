using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Audio;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Effects;
using Game.Input;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Settings;
using Game.Simulation;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class NetToolSystem : ToolBaseSystem
{
	public enum Mode
	{
		Straight,
		SimpleCurve,
		ComplexCurve,
		Continuous,
		Grid,
		Replace,
		Point
	}

	private class NetToolPreferences
	{
		public Mode m_Mode;

		public Snap m_Snap;

		public float m_Elevation;

		public float m_ElevationStep;

		public int m_ParallelCount;

		public float m_ParallelOffset;

		public bool m_Underground;

		public void Save(NetToolSystem netTool)
		{
			m_Mode = netTool.mode;
			m_Snap = netTool.selectedSnap;
			m_Elevation = netTool.elevation;
			m_ElevationStep = netTool.elevationStep;
			m_ParallelCount = netTool.parallelCount;
			m_ParallelOffset = netTool.parallelOffset;
			m_Underground = netTool.underground;
		}

		public void Load(NetToolSystem netTool)
		{
			netTool.mode = m_Mode;
			netTool.selectedSnap = m_Snap;
			netTool.elevation = m_Elevation;
			netTool.elevationStep = m_ElevationStep;
			netTool.parallelCount = m_ParallelCount;
			netTool.parallelOffset = m_ParallelOffset;
			netTool.underground = m_Underground;
		}
	}

	private enum State
	{
		Default,
		Applying,
		Cancelling
	}

	public struct UpgradeState
	{
		public bool m_IsUpgrading;

		public bool m_SkipFlags;

		public SubReplacementSide m_SubReplacementSide;

		public SubReplacementType m_SubReplacementType;

		public CompositionFlags m_OldFlags;

		public CompositionFlags m_AddFlags;

		public CompositionFlags m_RemoveFlags;

		public Entity m_SubReplacementPrefab;
	}

	public struct PathEdge
	{
		public Entity m_Entity;

		public bool m_Invert;

		public bool m_Upgrade;
	}

	public struct PathItem : ILessThan<PathItem>
	{
		public Entity m_Node;

		public Entity m_Edge;

		public float m_Cost;

		public bool LessThan(PathItem other)
		{
			return m_Cost < other.m_Cost;
		}
	}

	[BurstCompile]
	private struct UpdateStartEntityJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<NetCourse> m_NetCourseType;

		public NativeReference<Entity> m_StartEntity;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<NetCourse> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCourse>(ref m_NetCourseType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				NetCourse netCourse = nativeArray[i];
				if ((netCourse.m_StartPosition.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsParallel)) == CoursePosFlags.IsFirst)
				{
					m_StartEntity.Value = netCourse.m_StartPosition.m_Entity;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct AppliedUpgrade
	{
		public Entity m_Entity;

		public Entity m_SubReplacementPrefab;

		public CompositionFlags m_Flags;

		public SubReplacementType m_SubReplacementType;

		public SubReplacementSide m_SubReplacementSide;
	}

	[BurstCompile]
	private struct SnapJob : IJob
	{
		private struct ParentObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public ControlPoint m_BestSnapPosition;

			public Segment m_Line;

			public Bounds3 m_Bounds;

			public float m_Radius;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

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
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0239: Unknown result type (might be due to invalid IL or missing references)
				//IL_023e: Unknown result type (might be due to invalid IL or missing references)
				//IL_023f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0245: Unknown result type (might be due to invalid IL or missing references)
				//IL_024a: Unknown result type (might be due to invalid IL or missing references)
				//IL_024c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0251: Unknown result type (might be due to invalid IL or missing references)
				//IL_0255: Unknown result type (might be due to invalid IL or missing references)
				//IL_025a: Unknown result type (might be due to invalid IL or missing references)
				//IL_025c: Unknown result type (might be due to invalid IL or missing references)
				//IL_026b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0270: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0289: Unknown result type (might be due to invalid IL or missing references)
				//IL_0298: Unknown result type (might be due to invalid IL or missing references)
				//IL_029d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0282: Unknown result type (might be due to invalid IL or missing references)
				//IL_0283: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02af: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_017a: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0301: Unknown result type (might be due to invalid IL or missing references)
				//IL_030c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0311: Unknown result type (might be due to invalid IL or missing references)
				//IL_0313: Unknown result type (might be due to invalid IL or missing references)
				//IL_031f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0324: Unknown result type (might be due to invalid IL or missing references)
				//IL_0326: Unknown result type (might be due to invalid IL or missing references)
				//IL_0332: Unknown result type (might be due to invalid IL or missing references)
				//IL_0337: Unknown result type (might be due to invalid IL or missing references)
				//IL_0339: Unknown result type (might be due to invalid IL or missing references)
				//IL_0345: Unknown result type (might be due to invalid IL or missing references)
				//IL_034a: Unknown result type (might be due to invalid IL or missing references)
				//IL_034c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_0358: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0200: Unknown result type (might be due to invalid IL or missing references)
				//IL_0202: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0213: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0367: Unknown result type (might be due to invalid IL or missing references)
				//IL_0368: Unknown result type (might be due to invalid IL or missing references)
				//IL_0231: Unknown result type (might be due to invalid IL or missing references)
				//IL_0232: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, ((Bounds3)(ref m_Bounds)).xz) || m_OwnerData.HasComponent(item))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[item];
				if (!m_BuildingData.HasComponent(prefabRef.m_Prefab) && !m_BuildingExtensionData.HasComponent(prefabRef.m_Prefab) && !m_AssetStampData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				Transform transform = m_TransformData[item];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				float3 val = MathUtils.Center(bounds.m_Bounds);
				Segment val2 = m_Line - val;
				int2 val3 = default(int2);
				val3.x = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.x);
				val3.y = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.z);
				float2 val4 = float2.op_Implicit(val3) * 8f;
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					float num = val4.x * 0.5f;
					float3 val5 = transform.m_Position - val;
					Circle2 val6 = default(Circle2);
					((Circle2)(ref val6))._002Ector(num, ((float3)(ref val5)).xz);
					if (MathUtils.Intersect(val6, new Circle2(m_Radius, ((float3)(ref val2.a)).xz)))
					{
						m_BestSnapPosition.m_OriginalEntity = item;
						return;
					}
					if (MathUtils.Intersect(val6, new Circle2(m_Radius, ((float3)(ref val2.b)).xz)))
					{
						m_BestSnapPosition.m_OriginalEntity = item;
						return;
					}
					float num2 = MathUtils.Length(((Segment)(ref val2)).xz);
					if (num2 > m_Radius)
					{
						float2 val7 = MathUtils.Right((((float3)(ref val2.b)).xz - ((float3)(ref val2.a)).xz) * (m_Radius / num2));
						if (MathUtils.Intersect(new Quad2(((float3)(ref val2.a)).xz + val7, ((float3)(ref val2.b)).xz + val7, ((float3)(ref val2.b)).xz - val7, ((float3)(ref val2.a)).xz - val7), val6))
						{
							m_BestSnapPosition.m_OriginalEntity = item;
						}
					}
					return;
				}
				Quad3 val8 = ObjectUtils.CalculateBaseCorners(transform.m_Position - val, transform.m_Rotation, val4);
				Quad2 xz = ((Quad3)(ref val8)).xz;
				if (MathUtils.Intersect(xz, new Circle2(m_Radius, ((float3)(ref val2.a)).xz)))
				{
					m_BestSnapPosition.m_OriginalEntity = item;
					return;
				}
				if (MathUtils.Intersect(xz, new Circle2(m_Radius, ((float3)(ref val2.b)).xz)))
				{
					m_BestSnapPosition.m_OriginalEntity = item;
					return;
				}
				float num3 = MathUtils.Length(((Segment)(ref val2)).xz);
				if (num3 > m_Radius)
				{
					float2 val9 = MathUtils.Right((((float3)(ref val2.b)).xz - ((float3)(ref val2.a)).xz) * (m_Radius / num3));
					Quad2 val10 = default(Quad2);
					((Quad2)(ref val10))._002Ector(((float3)(ref val2.a)).xz + val9, ((float3)(ref val2.b)).xz + val9, ((float3)(ref val2.b)).xz - val9, ((float3)(ref val2.a)).xz - val9);
					if (MathUtils.Intersect(xz, val10))
					{
						m_BestSnapPosition.m_OriginalEntity = item;
					}
				}
			}
		}

		private struct LotIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public float m_Radius;

			public float m_EdgeOffset;

			public float m_MaxDistance;

			public int m_CellWidth;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Game.Net.Node> m_NodeData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

			public ComponentLookup<AssetStampData> m_AssetStampData;

			public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0206: Unknown result type (might be due to invalid IL or missing references)
				//IL_0211: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0222: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_0228: Unknown result type (might be due to invalid IL or missing references)
				//IL_022d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0236: Unknown result type (might be due to invalid IL or missing references)
				//IL_023b: Unknown result type (might be due to invalid IL or missing references)
				//IL_023d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0244: Unknown result type (might be due to invalid IL or missing references)
				//IL_0249: Unknown result type (might be due to invalid IL or missing references)
				//IL_024e: Unknown result type (might be due to invalid IL or missing references)
				//IL_024f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0256: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0260: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_029c: Unknown result type (might be due to invalid IL or missing references)
				//IL_029e: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_026c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0275: Unknown result type (might be due to invalid IL or missing references)
				//IL_027f: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0317: Unknown result type (might be due to invalid IL or missing references)
				//IL_0319: Unknown result type (might be due to invalid IL or missing references)
				//IL_0325: Unknown result type (might be due to invalid IL or missing references)
				//IL_0332: Unknown result type (might be due to invalid IL or missing references)
				//IL_0337: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_030b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0310: Unknown result type (might be due to invalid IL or missing references)
				//IL_0385: Unknown result type (might be due to invalid IL or missing references)
				//IL_038c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0393: Unknown result type (might be due to invalid IL or missing references)
				//IL_0398: Unknown result type (might be due to invalid IL or missing references)
				//IL_039d: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_03db: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0403: Unknown result type (might be due to invalid IL or missing references)
				//IL_040d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0412: Unknown result type (might be due to invalid IL or missing references)
				//IL_0424: Unknown result type (might be due to invalid IL or missing references)
				//IL_042b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0430: Unknown result type (might be due to invalid IL or missing references)
				//IL_043a: Unknown result type (might be due to invalid IL or missing references)
				//IL_043f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0451: Unknown result type (might be due to invalid IL or missing references)
				//IL_0458: Unknown result type (might be due to invalid IL or missing references)
				//IL_045d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0467: Unknown result type (might be due to invalid IL or missing references)
				//IL_046c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0483: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0493: Unknown result type (might be due to invalid IL or missing references)
				//IL_049a: Unknown result type (might be due to invalid IL or missing references)
				//IL_049c: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || m_OwnerData.HasComponent(item))
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefData[item];
				if (!m_BuildingData.HasComponent(prefabRef.m_Prefab) && !m_BuildingExtensionData.HasComponent(prefabRef.m_Prefab) && !m_AssetStampData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				Transform transform = m_TransformData[item];
				ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
				float3 val = math.forward(transform.m_Rotation);
				float2 val2 = math.normalizesafe(((float3)(ref val)).xz, new float2(0f, 1f));
				float2 val3 = MathUtils.Right(val2);
				float2 val4 = ((float3)(ref m_ControlPoint.m_HitPosition)).xz - ((float3)(ref transform.m_Position)).xz;
				int2 val5 = default(int2);
				val5.x = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.x);
				val5.y = ZoneUtils.GetCellWidth(objectGeometryData.m_Size.z);
				float2 val6 = float2.op_Implicit(val5) * 8f;
				float2 val7 = math.select(float2.op_Implicit(0f), float2.op_Implicit(4f), ((m_CellWidth + val5) & 1) != 0);
				float2 val8 = default(float2);
				((float2)(ref val8))._002Ector(math.dot(val4, val3), math.dot(val4, val2));
				float2 val9 = MathUtils.Snap(val8, float2.op_Implicit(8f), val7);
				if (m_EdgeOffset != 0f && (objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) == 0)
				{
					val9 = math.select(val9, val9 + math.select(float2.op_Implicit(m_EdgeOffset), float2.op_Implicit(0f - m_EdgeOffset), val9 > 0f), math.abs(math.abs(val9) - val6 * 0.5f) < 4f);
				}
				bool2 val10 = math.abs(val8 - val9) < m_MaxDistance;
				if (!math.any(val10))
				{
					return;
				}
				val9 = math.select(val8, val9, val10);
				float2 val11 = ((float3)(ref transform.m_Position)).xz + val3 * val9.x + val2 * val9.y;
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
				{
					if (math.distance(val11, ((float3)(ref transform.m_Position)).xz) > val6.x * 0.5f + m_Radius + 4f)
					{
						return;
					}
				}
				else if (math.any(math.abs(val9) > val6 * 0.5f + m_Radius + 4f))
				{
					return;
				}
				ControlPoint controlPoint = m_ControlPoint;
				if (!m_EdgeData.HasComponent(m_ControlPoint.m_OriginalEntity) && !m_NodeData.HasComponent(m_ControlPoint.m_OriginalEntity))
				{
					controlPoint.m_OriginalEntity = Entity.Null;
				}
				controlPoint.m_Direction = val3;
				((float3)(ref controlPoint.m_Position)).xz = val11;
				if (m_ControlPoint.m_OriginalEntity != item || m_ControlPoint.m_ElementIndex.x != -1)
				{
					controlPoint.m_Position.y = m_ControlPoint.m_HitPosition.y;
				}
				controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
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
					ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val12.a, val12.b), SnapLineFlags.Hidden, 0f));
				}
				controlPoint.m_Direction = MathUtils.Right(controlPoint.m_Direction);
				if (val10.x)
				{
					ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val13.a, val13.b), SnapLineFlags.Hidden, 0f));
				}
			}
		}

		private struct ZoneIterator : INativeQuadTreeIterator<Entity, Bounds2>, IUnsafeQuadTreeIterator<Entity, Bounds2>
		{
			public Bounds2 m_Bounds;

			public float2 m_HitPosition;

			public float3 m_BestPosition;

			public float2 m_BestDirection;

			public float m_BestDistance;

			public ComponentLookup<Block> m_ZoneBlockData;

			public BufferLookup<Cell> m_ZoneCells;

			public bool Intersect(Bounds2 bounds)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds, m_Bounds);
			}

			public void Iterate(Bounds2 bounds, Entity entity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds, m_Bounds))
				{
					return;
				}
				Block block = m_ZoneBlockData[entity];
				DynamicBuffer<Cell> val = m_ZoneCells[entity];
				int2 val2 = math.clamp(ZoneUtils.GetCellIndex(block, m_HitPosition), int2.op_Implicit(0), block.m_Size - 1);
				float3 cellPosition = ZoneUtils.GetCellPosition(block, val2);
				float num = math.distance(((float3)(ref cellPosition)).xz, m_HitPosition);
				if (num >= m_BestDistance)
				{
					return;
				}
				if ((val[val2.x + val2.y * block.m_Size.x].m_State & CellFlags.Visible) != CellFlags.None)
				{
					m_BestPosition = cellPosition;
					m_BestDirection = block.m_Direction;
					m_BestDistance = num;
					return;
				}
				val2.y = 0;
				while (val2.y < block.m_Size.y)
				{
					val2.x = 0;
					while (val2.x < block.m_Size.x)
					{
						if ((val[val2.x + val2.y * block.m_Size.x].m_State & CellFlags.Visible) != CellFlags.None)
						{
							cellPosition = ZoneUtils.GetCellPosition(block, val2);
							num = math.distance(((float3)(ref cellPosition)).xz, m_HitPosition);
							if (num < m_BestDistance)
							{
								m_BestPosition = cellPosition;
								m_BestDirection = block.m_Direction;
								m_BestDistance = num;
							}
						}
						val2.x++;
					}
					val2.y++;
				}
			}
		}

		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds3 m_Bounds;

			public Snap m_Snap;

			public float m_MaxDistance;

			public float m_NetSnapOffset;

			public float m_ObjectSnapOffset;

			public bool m_SnapCellLength;

			public NetData m_NetData;

			public NetGeometryData m_NetGeometryData;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Game.Net.Node> m_NodeData;

			public ComponentLookup<Transform> m_TransformData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<BuildingData> m_BuildingData;

			public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

			public ComponentLookup<NetData> m_PrefabNetData;

			public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

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
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds))
				{
					return;
				}
				if ((m_Snap & (Snap.ExistingGeometry | Snap.NearbyGeometry)) != Snap.None && m_OwnerData.HasComponent(entity))
				{
					Owner owner = m_OwnerData[entity];
					if (m_NodeData.HasComponent(owner.m_Owner))
					{
						SnapToNode(owner.m_Owner);
					}
				}
				if ((m_Snap & Snap.ObjectSide) != Snap.None)
				{
					SnapObjectSide(entity);
				}
			}

			private void SnapToNode(Entity entity)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				if ((entity == m_ControlPoint.m_OriginalEntity && (m_Snap & Snap.ExistingGeometry) != Snap.None) || (m_ConnectedEdges.HasBuffer(entity) && m_ConnectedEdges[entity].Length > 0))
				{
					return;
				}
				Game.Net.Node node = m_NodeData[entity];
				PrefabRef prefabRef = m_PrefabRefData[entity];
				if (!m_PrefabNetData.HasComponent(prefabRef.m_Prefab) || !NetUtils.CanConnect(m_PrefabNetData[prefabRef.m_Prefab], m_NetData))
				{
					return;
				}
				ControlPoint snapPosition = m_ControlPoint;
				snapPosition.m_OriginalEntity = entity;
				snapPosition.m_Position = node.m_Position;
				float3 val = math.mul(node.m_Rotation, new float3(0f, 0f, 1f));
				snapPosition.m_Direction = ((float3)(ref val)).xz;
				MathUtils.TryNormalize(ref snapPosition.m_Direction);
				float level = 1f;
				float num = math.distance(((float3)(ref node.m_Position)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz);
				float num2 = m_NetGeometryData.m_DefaultWidth * 0.5f;
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabGeometryData.TryGetComponent(prefabRef.m_Prefab, ref netGeometryData))
				{
					num2 += netGeometryData.m_DefaultWidth * 0.5f;
				}
				if (!(num >= num2 + m_NetSnapOffset))
				{
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0 && num <= num2 && num <= num2)
					{
						level = 2f;
					}
					snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, 1f, m_ControlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, snapPosition);
				}
			}

			private void SnapObjectSide(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				if (!m_TransformData.HasComponent(entity))
				{
					return;
				}
				Transform transform = m_TransformData[entity];
				PrefabRef prefabRef = m_PrefabRefData[entity];
				if (!m_ObjectGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					return;
				}
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) == 0)
				{
					bool flag = false;
					if (m_BuildingData.HasComponent(prefabRef.m_Prefab))
					{
						float2 val = float2.op_Implicit(m_BuildingData[prefabRef.m_Prefab].m_LotSize);
						((float3)(ref objectGeometryData.m_Bounds.min)).xz = val * -4f;
						((float3)(ref objectGeometryData.m_Bounds.max)).xz = val * 4f;
						flag = m_SnapCellLength;
						ref float3 min = ref objectGeometryData.m_Bounds.min;
						((float3)(ref min)).xz = ((float3)(ref min)).xz - m_ObjectSnapOffset;
						ref float3 max = ref objectGeometryData.m_Bounds.max;
						((float3)(ref max)).xz = ((float3)(ref max)).xz + m_ObjectSnapOffset;
						Quad3 val2 = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
						CheckLine(((Quad3)(ref val2)).ab, flag);
						CheckLine(((Quad3)(ref val2)).bc, flag);
						CheckLine(((Quad3)(ref val2)).cd, flag);
						CheckLine(((Quad3)(ref val2)).da, flag);
					}
				}
			}

			private void CheckLine(Segment line, bool snapCellLength)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_013a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				float num = default(float);
				if (MathUtils.Distance(((Segment)(ref line)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num) < m_MaxDistance)
				{
					if (snapCellLength)
					{
						num = MathUtils.Snap(num, 8f / MathUtils.Length(((Segment)(ref line)).xz));
					}
					ControlPoint controlPoint = m_ControlPoint;
					controlPoint.m_Direction = math.normalizesafe(MathUtils.Tangent(((Segment)(ref line)).xz), default(float2));
					controlPoint.m_Position = MathUtils.Position(line, num);
					controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(1f, 1f, 1f, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
					if (m_CurveData.HasComponent(m_ControlPoint.m_OriginalEntity))
					{
						Curve curve = m_CurveData[m_ControlPoint.m_OriginalEntity];
						MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref controlPoint.m_Position)).xz, ref controlPoint.m_CurvePosition);
					}
					else if (!m_NodeData.HasComponent(m_ControlPoint.m_OriginalEntity))
					{
						controlPoint.m_OriginalEntity = Entity.Null;
					}
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
					ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(line.a, line.b), SnapLineFlags.Secondary, 1f));
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public bool m_EditorMode;

			public Bounds3 m_TotalBounds;

			public Bounds3 m_Bounds;

			public Snap m_Snap;

			public Entity m_ServiceUpgradeOwner;

			public float m_SnapOffset;

			public float m_SnapDistance;

			public float m_Elevation;

			public float m_GuideLength;

			public float m_LegSnapWidth;

			public Bounds1 m_HeightRange;

			public NetData m_NetData;

			public RoadData m_PrefabRoadData;

			public NetGeometryData m_NetGeometryData;

			public LocalConnectData m_LocalConnectData;

			public ControlPoint m_ControlPoint;

			public ControlPoint m_BestSnapPosition;

			public NativeList<SnapLine> m_SnapLines;

			public TerrainHeightData m_TerrainHeightData;

			public WaterSurfaceData m_WaterSurfaceData;

			public ComponentLookup<Owner> m_OwnerData;

			public ComponentLookup<Game.Net.Node> m_NodeData;

			public ComponentLookup<Edge> m_EdgeData;

			public ComponentLookup<Curve> m_CurveData;

			public ComponentLookup<Composition> m_CompositionData;

			public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

			public ComponentLookup<Road> m_RoadData;

			public ComponentLookup<PrefabRef> m_PrefabRefData;

			public ComponentLookup<NetData> m_PrefabNetData;

			public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

			public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

			public ComponentLookup<RoadComposition> m_RoadCompositionData;

			public BufferLookup<ConnectedEdge> m_ConnectedEdges;

			public BufferLookup<Game.Net.SubNet> m_SubNets;

			public BufferLookup<NetCompositionArea> m_PrefabCompositionAreas;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(bounds.m_Bounds, m_TotalBounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(bounds.m_Bounds, m_TotalBounds) && (!(entity == m_ControlPoint.m_OriginalEntity) || (m_Snap & Snap.ExistingGeometry) == 0) && (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || (m_Snap & (Snap.ExistingGeometry | Snap.NearbyGeometry)) == 0 || !HandleGeometry(entity)) && (m_Snap & Snap.GuideLines) != Snap.None)
				{
					HandleGuideLines(entity);
				}
			}

			public void HandleGuideLines(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0304: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_030e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0312: Unknown result type (might be due to invalid IL or missing references)
				//IL_0317: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0320: Unknown result type (might be due to invalid IL or missing references)
				//IL_0325: Unknown result type (might be due to invalid IL or missing references)
				//IL_032a: Unknown result type (might be due to invalid IL or missing references)
				//IL_032e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0333: Unknown result type (might be due to invalid IL or missing references)
				//IL_035a: Unknown result type (might be due to invalid IL or missing references)
				//IL_035f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0364: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_0374: Unknown result type (might be due to invalid IL or missing references)
				//IL_0379: Unknown result type (might be due to invalid IL or missing references)
				//IL_037b: Unknown result type (might be due to invalid IL or missing references)
				//IL_037d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0401: Unknown result type (might be due to invalid IL or missing references)
				//IL_0406: Unknown result type (might be due to invalid IL or missing references)
				//IL_0408: Unknown result type (might be due to invalid IL or missing references)
				//IL_040a: Unknown result type (might be due to invalid IL or missing references)
				//IL_038b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0396: Unknown result type (might be due to invalid IL or missing references)
				//IL_039d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0418: Unknown result type (might be due to invalid IL or missing references)
				//IL_0423: Unknown result type (might be due to invalid IL or missing references)
				//IL_042a: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_047c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0481: Unknown result type (might be due to invalid IL or missing references)
				//IL_0486: Unknown result type (might be due to invalid IL or missing references)
				//IL_048b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0490: Unknown result type (might be due to invalid IL or missing references)
				//IL_0492: Unknown result type (might be due to invalid IL or missing references)
				//IL_0499: Unknown result type (might be due to invalid IL or missing references)
				//IL_049e: Unknown result type (might be due to invalid IL or missing references)
				//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0438: Unknown result type (might be due to invalid IL or missing references)
				//IL_043f: Unknown result type (might be due to invalid IL or missing references)
				//IL_087c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0881: Unknown result type (might be due to invalid IL or missing references)
				//IL_0886: Unknown result type (might be due to invalid IL or missing references)
				//IL_088b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0890: Unknown result type (might be due to invalid IL or missing references)
				//IL_0892: Unknown result type (might be due to invalid IL or missing references)
				//IL_0899: Unknown result type (might be due to invalid IL or missing references)
				//IL_089e: Unknown result type (might be due to invalid IL or missing references)
				//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_050a: Unknown result type (might be due to invalid IL or missing references)
				//IL_050f: Unknown result type (might be due to invalid IL or missing references)
				//IL_090a: Unknown result type (might be due to invalid IL or missing references)
				//IL_090f: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0607: Unknown result type (might be due to invalid IL or missing references)
				//IL_060c: Unknown result type (might be due to invalid IL or missing references)
				//IL_060e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0619: Unknown result type (might be due to invalid IL or missing references)
				//IL_061e: Unknown result type (might be due to invalid IL or missing references)
				//IL_062a: Unknown result type (might be due to invalid IL or missing references)
				//IL_063a: Unknown result type (might be due to invalid IL or missing references)
				//IL_053f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0543: Unknown result type (might be due to invalid IL or missing references)
				//IL_0548: Unknown result type (might be due to invalid IL or missing references)
				//IL_054f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0551: Unknown result type (might be due to invalid IL or missing references)
				//IL_056d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0574: Unknown result type (might be due to invalid IL or missing references)
				//IL_057b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0580: Unknown result type (might be due to invalid IL or missing references)
				//IL_0585: Unknown result type (might be due to invalid IL or missing references)
				//IL_059e: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
				//IL_093f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0943: Unknown result type (might be due to invalid IL or missing references)
				//IL_0948: Unknown result type (might be due to invalid IL or missing references)
				//IL_094f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0951: Unknown result type (might be due to invalid IL or missing references)
				//IL_096d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0974: Unknown result type (might be due to invalid IL or missing references)
				//IL_097b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0980: Unknown result type (might be due to invalid IL or missing references)
				//IL_0985: Unknown result type (might be due to invalid IL or missing references)
				//IL_099e: Unknown result type (might be due to invalid IL or missing references)
				//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_072c: Unknown result type (might be due to invalid IL or missing references)
				//IL_072e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0733: Unknown result type (might be due to invalid IL or missing references)
				//IL_0738: Unknown result type (might be due to invalid IL or missing references)
				//IL_073c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0741: Unknown result type (might be due to invalid IL or missing references)
				//IL_074d: Unknown result type (might be due to invalid IL or missing references)
				//IL_074f: Unknown result type (might be due to invalid IL or missing references)
				//IL_075e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0763: Unknown result type (might be due to invalid IL or missing references)
				//IL_0765: Unknown result type (might be due to invalid IL or missing references)
				//IL_0770: Unknown result type (might be due to invalid IL or missing references)
				//IL_0775: Unknown result type (might be due to invalid IL or missing references)
				//IL_0781: Unknown result type (might be due to invalid IL or missing references)
				//IL_0791: Unknown result type (might be due to invalid IL or missing references)
				//IL_065b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0660: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b2e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b70: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b75: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_068e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0692: Unknown result type (might be due to invalid IL or missing references)
				//IL_0697: Unknown result type (might be due to invalid IL or missing references)
				//IL_069e: Unknown result type (might be due to invalid IL or missing references)
				//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0700: Unknown result type (might be due to invalid IL or missing references)
				//IL_0702: Unknown result type (might be due to invalid IL or missing references)
				//IL_0707: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
				//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
				//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0818: Unknown result type (might be due to invalid IL or missing references)
				//IL_081f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0826: Unknown result type (might be due to invalid IL or missing references)
				//IL_082b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0830: Unknown result type (might be due to invalid IL or missing references)
				//IL_0849: Unknown result type (might be due to invalid IL or missing references)
				//IL_0850: Unknown result type (might be due to invalid IL or missing references)
				//IL_0852: Unknown result type (might be due to invalid IL or missing references)
				//IL_0857: Unknown result type (might be due to invalid IL or missing references)
				//IL_0859: Unknown result type (might be due to invalid IL or missing references)
				//IL_085e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c1f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c49: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
				//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
				if (!m_CurveData.HasComponent(entity))
				{
					return;
				}
				bool flag = false;
				bool flag2 = (m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0 && (m_PrefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0 && (m_Snap & Snap.CellLength) != 0;
				float defaultWidth = m_NetGeometryData.m_DefaultWidth;
				float num = defaultWidth;
				float num2 = m_NetGeometryData.m_DefaultWidth * 0.5f;
				bool flag3 = false;
				bool flag4 = false;
				PrefabRef prefabRef = m_PrefabRefData[entity];
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				}
				if (!NetUtils.CanConnect(m_NetData, netData) || (!m_EditorMode && (netGeometryData.m_Flags & Game.Net.GeometryFlags.Marker) != 0))
				{
					return;
				}
				if (m_CompositionData.HasComponent(entity))
				{
					Composition composition = m_CompositionData[entity];
					num2 += m_PrefabCompositionData[composition.m_Edge].m_Width * 0.5f;
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0)
					{
						num = netGeometryData.m_DefaultWidth;
						if (m_RoadCompositionData.HasComponent(composition.m_Edge))
						{
							flag = (m_RoadCompositionData[composition.m_Edge].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0 && (m_Snap & Snap.CellLength) != 0;
							if (flag && m_RoadData.HasComponent(entity))
							{
								Road road = m_RoadData[entity];
								flag3 = (road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0;
								flag4 = (road.m_Flags & Game.Net.RoadFlags.EndHalfAligned) != 0;
							}
						}
					}
				}
				int cellWidth = ZoneUtils.GetCellWidth(defaultWidth);
				int cellWidth2 = ZoneUtils.GetCellWidth(num);
				int num3;
				float num4;
				float num5;
				if (flag2)
				{
					num3 = 1 + math.abs(cellWidth2 - cellWidth);
					num4 = (float)(num3 - 1) * -4f;
					num5 = 8f;
				}
				else
				{
					float num6 = math.abs(num - defaultWidth);
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
				float num7;
				float num8;
				float num9;
				float num10;
				if (flag)
				{
					num7 = math.select(0f, 4f, (((cellWidth ^ cellWidth2) & 1) != 0) ^ flag3);
					num8 = math.select(0f, 4f, (((cellWidth ^ cellWidth2) & 1) != 0) ^ flag4);
					num9 = math.select(num7, 0f - num7, cellWidth > cellWidth2);
					num10 = math.select(num8, 0f - num8, cellWidth > cellWidth2);
					num9 += 8f * (float)((math.max(2, cellWidth2) - math.max(2, cellWidth)) / 2);
					num10 += 8f * (float)((math.max(2, cellWidth2) - math.max(2, cellWidth)) / 2);
				}
				else
				{
					num7 = 0f;
					num8 = 0f;
					num9 = 0f;
					num10 = 0f;
				}
				Curve curve = m_CurveData[entity];
				Edge edge = m_EdgeData[entity];
				float3 val = MathUtils.StartTangent(curve.m_Bezier);
				float2 val2 = -((float3)(ref val)).xz;
				val = MathUtils.EndTangent(curve.m_Bezier);
				float2 xz = ((float3)(ref val)).xz;
				bool flag5 = MathUtils.TryNormalize(ref val2);
				bool flag6 = MathUtils.TryNormalize(ref xz);
				bool flag7 = flag5;
				if (flag5)
				{
					DynamicBuffer<ConnectedEdge> val3 = m_ConnectedEdges[edge.m_Start];
					for (int i = 0; i < val3.Length; i++)
					{
						Entity edge2 = val3[i].m_Edge;
						if (!(edge2 == entity))
						{
							Edge edge3 = m_EdgeData[edge2];
							if (edge3.m_Start == edge.m_Start || edge3.m_End == edge.m_Start)
							{
								flag7 = false;
								break;
							}
						}
					}
				}
				bool flag8 = flag6;
				if (flag6)
				{
					DynamicBuffer<ConnectedEdge> val4 = m_ConnectedEdges[edge.m_End];
					for (int j = 0; j < val4.Length; j++)
					{
						Entity edge4 = val4[j].m_Edge;
						if (!(edge4 == entity))
						{
							Edge edge5 = m_EdgeData[edge4];
							if (edge5.m_Start == edge.m_End || edge5.m_End == edge.m_End)
							{
								flag8 = false;
								break;
							}
						}
					}
				}
				if (!(flag5 || flag6))
				{
					return;
				}
				Segment val5 = default(Segment);
				float num11 = default(float);
				Segment val7 = default(Segment);
				float num12 = default(float);
				Segment val9 = default(Segment);
				float num13 = default(float);
				Segment val10 = default(Segment);
				float num14 = default(float);
				Segment val12 = default(Segment);
				float num15 = default(float);
				Segment val14 = default(Segment);
				float num16 = default(float);
				for (int k = 0; k < num3; k++)
				{
					if (flag5)
					{
						float3 a = curve.m_Bezier.a;
						((float3)(ref a)).xz = ((float3)(ref a)).xz + MathUtils.Left(val2) * num4;
						((Segment)(ref val5))._002Ector(a, a);
						ref float3 b = ref val5.b;
						((float3)(ref b)).xz = ((float3)(ref b)).xz + val2 * m_GuideLength;
						if (MathUtils.Distance(((Segment)(ref val5)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num11) < m_SnapDistance)
						{
							ControlPoint controlPoint = m_ControlPoint;
							controlPoint.m_OriginalEntity = Entity.Null;
							if ((m_Snap & Snap.CellLength) != Snap.None)
							{
								num11 = MathUtils.Snap(m_GuideLength * num11, m_SnapDistance, num7) / m_GuideLength;
							}
							controlPoint.m_Position = MathUtils.Position(val5, num11);
							controlPoint.m_Direction = val2;
							controlPoint.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint.m_Position, controlPoint.m_Direction);
							ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint);
							ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint, NetUtils.StraightCurve(val5.a, val5.b), SnapLineFlags.GuideLine, 0.1f));
						}
						if (k == 0 && flag7)
						{
							float3 val6 = a;
							((float3)(ref val6)).xz = ((float3)(ref val6)).xz + val2 * num9;
							((Segment)(ref val7))._002Ector(val6, val6);
							ref float3 b2 = ref val7.b;
							((float3)(ref b2)).xz = ((float3)(ref b2)).xz + MathUtils.Right(val2) * m_GuideLength;
							if (MathUtils.Distance(((Segment)(ref val7)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num12) < m_SnapDistance)
							{
								ControlPoint controlPoint2 = m_ControlPoint;
								controlPoint2.m_OriginalEntity = Entity.Null;
								if ((m_Snap & Snap.CellLength) != Snap.None)
								{
									num12 = MathUtils.Snap(m_GuideLength * num12, m_SnapDistance) / m_GuideLength;
								}
								controlPoint2.m_Position = MathUtils.Position(val7, num12);
								controlPoint2.m_Direction = MathUtils.Right(val2);
								controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
								ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint2);
								ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val7.a, val7.b), SnapLineFlags.GuideLine, 0.1f));
							}
						}
						if (k == num3 - 1 && flag7)
						{
							float3 val8 = a;
							((float3)(ref val8)).xz = ((float3)(ref val8)).xz + val2 * num9;
							((Segment)(ref val9))._002Ector(val8, val8);
							ref float3 b3 = ref val9.b;
							((float3)(ref b3)).xz = ((float3)(ref b3)).xz + MathUtils.Left(val2) * m_GuideLength;
							if (MathUtils.Distance(((Segment)(ref val9)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num13) < m_SnapDistance)
							{
								ControlPoint controlPoint3 = m_ControlPoint;
								controlPoint3.m_OriginalEntity = Entity.Null;
								if ((m_Snap & Snap.CellLength) != Snap.None)
								{
									num13 = MathUtils.Snap(m_GuideLength * num13, m_SnapDistance) / m_GuideLength;
								}
								controlPoint3.m_Position = MathUtils.Position(val9, num13);
								controlPoint3.m_Direction = MathUtils.Left(val2);
								controlPoint3.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint3.m_Position, controlPoint3.m_Direction);
								ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint3);
								ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint3, NetUtils.StraightCurve(val9.a, val9.b), SnapLineFlags.GuideLine, 0.1f));
							}
						}
					}
					if (flag6)
					{
						float3 d = curve.m_Bezier.d;
						((float3)(ref d)).xz = ((float3)(ref d)).xz + MathUtils.Left(xz) * num4;
						((Segment)(ref val10))._002Ector(d, d);
						ref float3 b4 = ref val10.b;
						((float3)(ref b4)).xz = ((float3)(ref b4)).xz + xz * m_GuideLength;
						if (MathUtils.Distance(((Segment)(ref val10)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num14) < m_SnapDistance)
						{
							ControlPoint controlPoint4 = m_ControlPoint;
							controlPoint4.m_OriginalEntity = Entity.Null;
							if ((m_Snap & Snap.CellLength) != Snap.None)
							{
								num14 = MathUtils.Snap(m_GuideLength * num14, m_SnapDistance, num8) / m_GuideLength;
							}
							controlPoint4.m_Position = MathUtils.Position(val10, num14);
							controlPoint4.m_Direction = xz;
							controlPoint4.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint4.m_Position, controlPoint4.m_Direction);
							ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint4);
							ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint4, NetUtils.StraightCurve(val10.a, val10.b), SnapLineFlags.GuideLine, 0.1f));
						}
						if (k == 0 && flag8)
						{
							float3 val11 = d;
							((float3)(ref val11)).xz = ((float3)(ref val11)).xz + xz * num10;
							((Segment)(ref val12))._002Ector(val11, val11);
							ref float3 b5 = ref val12.b;
							((float3)(ref b5)).xz = ((float3)(ref b5)).xz + MathUtils.Right(xz) * m_GuideLength;
							if (MathUtils.Distance(((Segment)(ref val12)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num15) < m_SnapDistance)
							{
								ControlPoint controlPoint5 = m_ControlPoint;
								controlPoint5.m_OriginalEntity = Entity.Null;
								if ((m_Snap & Snap.CellLength) != Snap.None)
								{
									num15 = MathUtils.Snap(m_GuideLength * num15, m_SnapDistance) / m_GuideLength;
								}
								controlPoint5.m_Position = MathUtils.Position(val12, num15);
								controlPoint5.m_Direction = MathUtils.Right(xz);
								controlPoint5.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint5.m_Position, controlPoint5.m_Direction);
								ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint5);
								ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint5, NetUtils.StraightCurve(val12.a, val12.b), SnapLineFlags.GuideLine, 0.1f));
							}
						}
						if (k == num3 - 1 && flag8)
						{
							float3 val13 = d;
							((float3)(ref val13)).xz = ((float3)(ref val13)).xz + xz * num10;
							((Segment)(ref val14))._002Ector(val13, val13);
							ref float3 b6 = ref val14.b;
							((float3)(ref b6)).xz = ((float3)(ref b6)).xz + MathUtils.Left(xz) * m_GuideLength;
							if (MathUtils.Distance(((Segment)(ref val14)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref num16) < m_SnapDistance)
							{
								ControlPoint controlPoint6 = m_ControlPoint;
								controlPoint6.m_OriginalEntity = Entity.Null;
								if ((m_Snap & Snap.CellLength) != Snap.None)
								{
									num16 = MathUtils.Snap(m_GuideLength * num16, m_SnapDistance) / m_GuideLength;
								}
								controlPoint6.m_Position = MathUtils.Position(val14, num16);
								controlPoint6.m_Direction = MathUtils.Left(xz);
								controlPoint6.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0.1f, m_ControlPoint.m_HitPosition, controlPoint6.m_Position, controlPoint6.m_Direction);
								ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint6);
								ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint6, NetUtils.StraightCurve(val14.a, val14.b), SnapLineFlags.GuideLine, 0.1f));
							}
						}
					}
					num4 += num5;
				}
			}

			public bool HandleGeometry(Entity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0194: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0216: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0144: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				PrefabRef prefabRef = m_PrefabRefData[entity];
				ControlPoint controlPoint = m_ControlPoint;
				controlPoint.m_OriginalEntity = entity;
				float num = m_NetGeometryData.m_DefaultWidth * 0.5f + m_SnapOffset;
				if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					if ((m_NetGeometryData.m_Flags & ~netGeometryData.m_Flags & Game.Net.GeometryFlags.StandingNodes) != 0)
					{
						num = m_LegSnapWidth * 0.5f + m_SnapOffset;
					}
				}
				if (m_ConnectedEdges.HasBuffer(entity))
				{
					Game.Net.Node node = m_NodeData[entity];
					DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[entity];
					for (int i = 0; i < val.Length; i++)
					{
						Edge edge = m_EdgeData[val[i].m_Edge];
						if (edge.m_Start == entity || edge.m_End == entity)
						{
							return false;
						}
					}
					if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
					{
						num += m_PrefabGeometryData[prefabRef.m_Prefab].m_DefaultWidth * 0.5f;
					}
					if (math.distance(((float3)(ref node.m_Position)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz) >= num)
					{
						return false;
					}
					float y = node.m_Position.y;
					return HandleGeometry(controlPoint, y, prefabRef, ignoreHeightDistance: false);
				}
				if (m_CurveData.HasComponent(entity))
				{
					Curve curve = m_CurveData[entity];
					if (m_CompositionData.HasComponent(entity))
					{
						Composition composition = m_CompositionData[entity];
						num += m_PrefabCompositionData[composition.m_Edge].m_Width * 0.5f;
					}
					if (MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref m_ControlPoint.m_HitPosition)).xz, ref controlPoint.m_CurvePosition) >= num)
					{
						return false;
					}
					float y2 = MathUtils.Position(curve.m_Bezier, controlPoint.m_CurvePosition).y;
					return HandleGeometry(controlPoint, y2, prefabRef, ignoreHeightDistance: false);
				}
				return false;
			}

			public bool HandleGeometry(ControlPoint controlPoint, float snapHeight, PrefabRef prefabRef, bool ignoreHeightDistance)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0311: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_021e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0223: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02db: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_029d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0278: Unknown result type (might be due to invalid IL or missing references)
				//IL_0177: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				if (!m_PrefabNetData.HasComponent(prefabRef.m_Prefab))
				{
					return false;
				}
				NetData netData = m_PrefabNetData[prefabRef.m_Prefab];
				bool snapAdded = false;
				bool flag = true;
				bool allowEdgeSnap = true;
				float num = ((!(m_Elevation < 0f)) ? (WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, controlPoint.m_HitPosition) + m_Elevation) : (TerrainUtils.SampleHeight(ref m_TerrainHeightData, controlPoint.m_HitPosition) + m_Elevation));
				if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					Bounds1 val = m_NetGeometryData.m_DefaultHeightRange + num;
					Bounds1 val2 = netGeometryData.m_DefaultHeightRange + snapHeight;
					if (!MathUtils.Intersect(val, val2))
					{
						flag = false;
						allowEdgeSnap = (netGeometryData.m_Flags & Game.Net.GeometryFlags.NoEdgeConnection) == 0;
					}
				}
				if (flag && !NetUtils.CanConnect(netData, m_NetData))
				{
					return snapAdded;
				}
				if ((m_NetData.m_ConnectLayers & ~netData.m_RequiredLayers & Layer.LaneEditor) != Layer.None)
				{
					return snapAdded;
				}
				float num2 = snapHeight - num;
				if (!ignoreHeightDistance && !MathUtils.Intersect(m_HeightRange, num2))
				{
					return snapAdded;
				}
				if (m_NodeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					if (m_ConnectedEdges.HasBuffer(controlPoint.m_OriginalEntity))
					{
						DynamicBuffer<ConnectedEdge> val3 = m_ConnectedEdges[controlPoint.m_OriginalEntity];
						if (val3.Length != 0)
						{
							for (int i = 0; i < val3.Length; i++)
							{
								Entity edge = val3[i].m_Edge;
								Edge edge2 = m_EdgeData[edge];
								if (!(edge2.m_Start != controlPoint.m_OriginalEntity) || !(edge2.m_End != controlPoint.m_OriginalEntity))
								{
									HandleCurve(controlPoint, edge, allowEdgeSnap, ref snapAdded);
								}
							}
							return snapAdded;
						}
					}
					ControlPoint snapPosition = controlPoint;
					Game.Net.Node node = m_NodeData[controlPoint.m_OriginalEntity];
					snapPosition.m_Position = node.m_Position;
					float3 val4 = math.mul(node.m_Rotation, new float3(0f, 0f, 1f));
					snapPosition.m_Direction = ((float3)(ref val4)).xz;
					MathUtils.TryNormalize(ref snapPosition.m_Direction);
					float level = 1f;
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
					{
						float num3 = m_NetGeometryData.m_DefaultWidth * 0.5f;
						if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
						{
							num3 += m_PrefabGeometryData[prefabRef.m_Prefab].m_DefaultWidth * 0.5f;
						}
						if (math.distance(((float3)(ref node.m_Position)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz) <= num3)
						{
							level = 2f;
						}
					}
					snapPosition.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, 1f, controlPoint.m_HitPosition, snapPosition.m_Position, snapPosition.m_Direction);
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, snapPosition);
					snapAdded = true;
				}
				else if (m_CurveData.HasComponent(controlPoint.m_OriginalEntity))
				{
					HandleCurve(controlPoint, controlPoint.m_OriginalEntity, allowEdgeSnap, ref snapAdded);
				}
				return snapAdded;
			}

			private bool SnapSegmentAreas(ControlPoint controlPoint, NetCompositionData prefabCompositionData, DynamicBuffer<NetCompositionArea> areas, Segment segment, ref bool snapAdded)
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_018d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0192: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0213: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0221: Unknown result type (might be due to invalid IL or missing references)
				//IL_0226: Unknown result type (might be due to invalid IL or missing references)
				//IL_022b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0244: Unknown result type (might be due to invalid IL or missing references)
				//IL_024b: Unknown result type (might be due to invalid IL or missing references)
				bool result = false;
				float num3 = default(float);
				for (int i = 0; i < areas.Length; i++)
				{
					NetCompositionArea netCompositionArea = areas[i];
					if ((netCompositionArea.m_Flags & NetAreaFlags.Buildable) == 0)
					{
						continue;
					}
					float num = netCompositionArea.m_Width * 0.51f;
					if (!(m_LegSnapWidth * 0.5f >= num))
					{
						result = true;
						Bezier4x3 val = MathUtils.Lerp(segment.m_Left, segment.m_Right, netCompositionArea.m_Position.x / prefabCompositionData.m_Width + 0.5f);
						float num2 = MathUtils.Distance(((Bezier4x3)(ref val)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz, ref num3);
						ControlPoint controlPoint2 = controlPoint;
						controlPoint2.m_Position = MathUtils.Position(val, num3);
						float3 val2 = MathUtils.Tangent(val, num3);
						controlPoint2.m_Direction = math.normalizesafe(((float3)(ref val2)).xz, default(float2));
						if ((netCompositionArea.m_Flags & NetAreaFlags.Invert) != 0)
						{
							controlPoint2.m_Direction = -controlPoint2.m_Direction;
						}
						float3 val3 = MathUtils.Position(MathUtils.Lerp(segment.m_Left, segment.m_Right, netCompositionArea.m_SnapPosition.x / prefabCompositionData.m_Width + 0.5f), num3);
						float num4 = math.max(0f, math.min(netCompositionArea.m_Width * 0.5f, math.abs(netCompositionArea.m_SnapPosition.x - netCompositionArea.m_Position.x) + netCompositionArea.m_SnapWidth * 0.5f) - m_LegSnapWidth * 0.5f);
						ref float3 position = ref controlPoint2.m_Position;
						((float3)(ref position)).xz = ((float3)(ref position)).xz + MathUtils.ClampLength(((float3)(ref val3)).xz - ((float3)(ref controlPoint2.m_Position)).xz, num4);
						controlPoint2.m_Position.y += netCompositionArea.m_Position.y;
						float level = 1f;
						if (num2 <= prefabCompositionData.m_Width * 0.5f - math.abs(netCompositionArea.m_Position.x) + m_LegSnapWidth * 0.5f)
						{
							level = 2f;
						}
						controlPoint2.m_Rotation = ToolUtils.CalculateRotation(controlPoint2.m_Direction);
						controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, 1f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
						ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint2);
						ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, val, GetSnapLineFlags(m_NetGeometryData.m_Flags), 1f));
						snapAdded = true;
					}
				}
				return result;
			}

			private void HandleCurve(ControlPoint controlPoint, Entity curveEntity, bool allowEdgeSnap, ref bool snapAdded)
			{
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0198: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0274: Unknown result type (might be due to invalid IL or missing references)
				//IL_0292: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_030e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0313: Unknown result type (might be due to invalid IL or missing references)
				//IL_0317: Unknown result type (might be due to invalid IL or missing references)
				//IL_031c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0323: Unknown result type (might be due to invalid IL or missing references)
				//IL_0329: Unknown result type (might be due to invalid IL or missing references)
				//IL_032b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0330: Unknown result type (might be due to invalid IL or missing references)
				//IL_033e: Unknown result type (might be due to invalid IL or missing references)
				//IL_034f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0354: Unknown result type (might be due to invalid IL or missing references)
				//IL_0359: Unknown result type (might be due to invalid IL or missing references)
				//IL_0360: Unknown result type (might be due to invalid IL or missing references)
				//IL_0366: Unknown result type (might be due to invalid IL or missing references)
				//IL_0368: Unknown result type (might be due to invalid IL or missing references)
				//IL_036d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0371: Unknown result type (might be due to invalid IL or missing references)
				//IL_0376: Unknown result type (might be due to invalid IL or missing references)
				//IL_037b: Unknown result type (might be due to invalid IL or missing references)
				//IL_037f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_038b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0391: Unknown result type (might be due to invalid IL or missing references)
				//IL_0393: Unknown result type (might be due to invalid IL or missing references)
				//IL_0398: Unknown result type (might be due to invalid IL or missing references)
				//IL_039a: Unknown result type (might be due to invalid IL or missing references)
				//IL_039c: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_048f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0496: Unknown result type (might be due to invalid IL or missing references)
				//IL_049b: Unknown result type (might be due to invalid IL or missing references)
				//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0400: Unknown result type (might be due to invalid IL or missing references)
				//IL_0404: Unknown result type (might be due to invalid IL or missing references)
				//IL_0409: Unknown result type (might be due to invalid IL or missing references)
				//IL_041b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0420: Unknown result type (might be due to invalid IL or missing references)
				//IL_0422: Unknown result type (might be due to invalid IL or missing references)
				//IL_0429: Unknown result type (might be due to invalid IL or missing references)
				//IL_0430: Unknown result type (might be due to invalid IL or missing references)
				//IL_0435: Unknown result type (might be due to invalid IL or missing references)
				//IL_0447: Unknown result type (might be due to invalid IL or missing references)
				//IL_044c: Unknown result type (might be due to invalid IL or missing references)
				//IL_044e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0455: Unknown result type (might be due to invalid IL or missing references)
				//IL_045c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0461: Unknown result type (might be due to invalid IL or missing references)
				//IL_0473: Unknown result type (might be due to invalid IL or missing references)
				//IL_0478: Unknown result type (might be due to invalid IL or missing references)
				//IL_047c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0481: Unknown result type (might be due to invalid IL or missing references)
				//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0515: Unknown result type (might be due to invalid IL or missing references)
				//IL_051c: Unknown result type (might be due to invalid IL or missing references)
				//IL_054c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0570: Unknown result type (might be due to invalid IL or missing references)
				//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_05da: Unknown result type (might be due to invalid IL or missing references)
				//IL_062b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0637: Unknown result type (might be due to invalid IL or missing references)
				//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0609: Unknown result type (might be due to invalid IL or missing references)
				//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0714: Unknown result type (might be due to invalid IL or missing references)
				//IL_0719: Unknown result type (might be due to invalid IL or missing references)
				//IL_075d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0764: Unknown result type (might be due to invalid IL or missing references)
				//IL_076b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0770: Unknown result type (might be due to invalid IL or missing references)
				//IL_0775: Unknown result type (might be due to invalid IL or missing references)
				//IL_078e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0795: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0687: Unknown result type (might be due to invalid IL or missing references)
				//IL_0693: Unknown result type (might be due to invalid IL or missing references)
				bool flag = false;
				bool flag2 = (m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0 && (m_PrefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0 && (m_Snap & Snap.CellLength) != 0;
				float defaultWidth = m_NetGeometryData.m_DefaultWidth;
				float num = defaultWidth;
				float num2 = m_NetGeometryData.m_DefaultWidth * 0.5f;
				bool2 val = bool2.op_Implicit(false);
				PrefabRef prefabRef = m_PrefabRefData[curveEntity];
				NetGeometryData netGeometryData = default(NetGeometryData);
				if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				}
				if (m_CompositionData.HasComponent(curveEntity))
				{
					Composition composition = m_CompositionData[curveEntity];
					NetCompositionData prefabCompositionData = m_PrefabCompositionData[composition.m_Edge];
					num2 += prefabCompositionData.m_Width * 0.5f;
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0)
					{
						num = netGeometryData.m_DefaultWidth;
						if (m_RoadCompositionData.HasComponent(composition.m_Edge))
						{
							flag = (m_RoadCompositionData[composition.m_Edge].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0 && (m_Snap & Snap.CellLength) != 0;
							if (flag && m_RoadData.HasComponent(curveEntity))
							{
								Road road = m_RoadData[curveEntity];
								val.x = (road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0;
								val.y = (road.m_Flags & Game.Net.RoadFlags.EndHalfAligned) != 0;
							}
						}
					}
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.SnapToNetAreas) != 0)
					{
						DynamicBuffer<NetCompositionArea> areas = m_PrefabCompositionAreas[composition.m_Edge];
						EdgeGeometry edgeGeometry = m_EdgeGeometryData[curveEntity];
						if (SnapSegmentAreas(controlPoint, prefabCompositionData, areas, edgeGeometry.m_Start, ref snapAdded) | SnapSegmentAreas(controlPoint, prefabCompositionData, areas, edgeGeometry.m_End, ref snapAdded))
						{
							return;
						}
					}
				}
				int num3;
				float num4;
				float num5;
				if (flag2)
				{
					int cellWidth = ZoneUtils.GetCellWidth(defaultWidth);
					int cellWidth2 = ZoneUtils.GetCellWidth(num);
					num3 = 1 + math.abs(cellWidth2 - cellWidth);
					num4 = (float)(num3 - 1) * -4f;
					num5 = 8f;
				}
				else
				{
					float num6 = math.abs(num - defaultWidth);
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
				float num7;
				if (flag)
				{
					int cellWidth3 = ZoneUtils.GetCellWidth(defaultWidth);
					int cellWidth4 = ZoneUtils.GetCellWidth(num);
					num7 = math.select(0f, 4f, (((cellWidth3 ^ cellWidth4) & 1) != 0) ^ val.x);
				}
				else
				{
					num7 = 0f;
				}
				Curve curve = m_CurveData[curveEntity];
				Owner owner = default(Owner);
				Owner owner2 = default(Owner);
				if (!m_EditorMode && m_OwnerData.TryGetComponent(curveEntity, ref owner) && owner.m_Owner != m_ServiceUpgradeOwner && (!m_EdgeData.HasComponent(owner.m_Owner) || (m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2) && owner2.m_Owner != m_ServiceUpgradeOwner)))
				{
					allowEdgeSnap = false;
				}
				float3 val2 = MathUtils.StartTangent(curve.m_Bezier);
				float2 val3 = math.normalizesafe(MathUtils.Left(((float3)(ref val2)).xz), default(float2));
				float2 val4 = math.normalizesafe(MathUtils.Left(((float3)(ref curve.m_Bezier.c)).xz - ((float3)(ref curve.m_Bezier.b)).xz), default(float2));
				val2 = MathUtils.EndTangent(curve.m_Bezier);
				float2 val5 = math.normalizesafe(MathUtils.Left(((float3)(ref val2)).xz), default(float2));
				bool flag3 = math.dot(val3, val4) > 0.9998477f && math.dot(val4, val5) > 0.9998477f;
				float t = default(float);
				for (int i = 0; i < num3; i++)
				{
					Bezier4x3 curve2;
					if (math.abs(num4) < 0.08f)
					{
						curve2 = curve.m_Bezier;
					}
					else if (flag3)
					{
						curve2 = curve.m_Bezier;
						ref float3 a = ref curve2.a;
						((float3)(ref a)).xz = ((float3)(ref a)).xz + val3 * num4;
						ref float3 b = ref curve2.b;
						((float3)(ref b)).xz = ((float3)(ref b)).xz + math.lerp(val3, val5, 1f / 3f) * num4;
						ref float3 c = ref curve2.c;
						((float3)(ref c)).xz = ((float3)(ref c)).xz + math.lerp(val3, val5, 2f / 3f) * num4;
						ref float3 d = ref curve2.d;
						((float3)(ref d)).xz = ((float3)(ref d)).xz + val5 * num4;
					}
					else
					{
						curve2 = NetUtils.OffsetCurveLeftSmooth(curve.m_Bezier, float2.op_Implicit(num4));
					}
					float num8 = (((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0) ? MathUtils.Distance(((Bezier4x3)(ref curve2)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz, ref t) : NetUtils.ExtendedDistance(((Bezier4x3)(ref curve2)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz, out t));
					ControlPoint controlPoint2 = controlPoint;
					if ((m_Snap & Snap.CellLength) != Snap.None)
					{
						float num9 = MathUtils.Length(((Bezier4x3)(ref curve2)).xz);
						num9 += math.select(0f, 4f, val.x != val.y);
						num9 = math.fmod(num9 + 0.1f, 8f) * 0.5f;
						float num10 = NetUtils.ExtendedLength(((Bezier4x3)(ref curve2)).xz, t);
						num10 = MathUtils.Snap(num10, m_SnapDistance, num7 + num9);
						t = NetUtils.ExtendedClampLength(((Bezier4x3)(ref curve2)).xz, num10);
						if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
						{
							t = math.saturate(t);
						}
						controlPoint2.m_CurvePosition = t;
					}
					else
					{
						t = math.saturate(t);
						if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
						{
							float num11 = NetUtils.ExtendedLength(((Bezier4x3)(ref curve2)).xz, t);
							num11 = MathUtils.Snap(num11, 4f);
							controlPoint2.m_CurvePosition = NetUtils.ExtendedClampLength(((Bezier4x3)(ref curve2)).xz, num11);
						}
						else
						{
							if (t >= 0.5f)
							{
								if (math.distance(((float3)(ref curve2.d)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz) < m_SnapOffset)
								{
									t = 1f;
								}
							}
							else if (math.distance(((float3)(ref curve2.a)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz) < m_SnapOffset)
							{
								t = 0f;
							}
							controlPoint2.m_CurvePosition = t;
						}
					}
					if (!allowEdgeSnap && t > 0f && t < 1f)
					{
						if (t >= 0.5f)
						{
							if (math.distance(((float3)(ref curve2.d)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz) >= num2 + m_SnapOffset)
							{
								continue;
							}
							t = 1f;
							controlPoint2.m_CurvePosition = 1f;
						}
						else
						{
							if (math.distance(((float3)(ref curve2.a)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz) >= num2 + m_SnapOffset)
							{
								continue;
							}
							t = 0f;
							controlPoint2.m_CurvePosition = 0f;
						}
					}
					NetUtils.ExtendedPositionAndTangent(curve2, t, out controlPoint2.m_Position, out var tangent);
					controlPoint2.m_Direction = ((float3)(ref tangent)).xz;
					MathUtils.TryNormalize(ref controlPoint2.m_Direction);
					float level = 1f;
					if ((m_NetGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0 && num8 <= num2)
					{
						level = 2f;
					}
					controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(level, 1f, 1f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
					ToolUtils.AddSnapPosition(ref m_BestSnapPosition, controlPoint2);
					ToolUtils.AddSnapLine(ref m_BestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, curve2, GetSnapLineFlags(m_NetGeometryData.m_Flags), 1f));
					snapAdded = true;
					num4 += num5;
				}
			}
		}

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public Snap m_Snap;

		[ReadOnly]
		public float m_Elevation;

		[ReadOnly]
		public Entity m_Prefab;

		[ReadOnly]
		public Entity m_LanePrefab;

		[ReadOnly]
		public Entity m_ServiceUpgradeOwner;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_RemoveUpgrade;

		[ReadOnly]
		public bool m_LeftHandTraffic;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Block> m_ZoneBlockData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RoadData> m_PrefabRoadData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_RoadCompositionData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> m_BuildingExtensionData;

		[ReadOnly]
		public ComponentLookup<AssetStampData> m_AssetStampData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> m_LocalConnectData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_PrefabLaneData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Cell> m_ZoneCells;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> m_PrefabCompositionAreas;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_SubObjects;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, Bounds2> m_ZoneSearchTree;

		public NativeList<ControlPoint> m_ControlPoints;

		public NativeList<SnapLine> m_SnapLines;

		public NativeList<UpgradeState> m_UpgradeStates;

		public NativeReference<Entity> m_StartEntity;

		public NativeReference<Entity> m_LastSnappedEntity;

		public NativeReference<int> m_LastControlPointsAngle;

		public NativeReference<AppliedUpgrade> m_AppliedUpgrade;

		public SourceUpdateData m_SourceUpdateData;

		public void Execute()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			RoadData prefabRoadData = default(RoadData);
			NetGeometryData netGeometryData = default(NetGeometryData);
			LocalConnectData localConnectData = default(LocalConnectData);
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			NetData prefabNetData = m_PrefabNetData[m_Prefab];
			if (m_PrefabRoadData.HasComponent(m_Prefab))
			{
				prefabRoadData = m_PrefabRoadData[m_Prefab];
			}
			if (m_PrefabGeometryData.HasComponent(m_Prefab))
			{
				netGeometryData = m_PrefabGeometryData[m_Prefab];
			}
			if (m_LocalConnectData.HasComponent(m_Prefab))
			{
				localConnectData = m_LocalConnectData[m_Prefab];
			}
			if (m_PlaceableData.HasComponent(m_Prefab))
			{
				placeableNetData = m_PlaceableData[m_Prefab];
			}
			placeableNetData.m_SnapDistance = math.max(placeableNetData.m_SnapDistance, 1f);
			if (m_LanePrefab != Entity.Null)
			{
				netGeometryData.m_Flags |= Game.Net.GeometryFlags.StrictNodes;
			}
			m_SnapLines.Clear();
			m_UpgradeStates.Clear();
			if (m_Mode == Mode.Replace || m_ControlPoints.Length <= 1)
			{
				m_StartEntity.Value = default(Entity);
			}
			if (m_Mode == Mode.Replace)
			{
				ControlPoint startPoint = m_ControlPoints[0];
				ControlPoint endPoint = m_ControlPoints[m_ControlPoints.Length - 1];
				m_ControlPoints.Clear();
				SubReplacement subReplacement = default(SubReplacement);
				if ((placeableNetData.m_SetUpgradeFlags.m_General & CompositionFlags.General.SecondaryMiddleBeautification) != 0 || (placeableNetData.m_SetUpgradeFlags.m_Left & CompositionFlags.Side.SecondaryBeautification) != 0 || (placeableNetData.m_SetUpgradeFlags.m_Right & CompositionFlags.Side.SecondaryBeautification) != 0)
				{
					subReplacement.m_Type = SubReplacementType.Tree;
				}
				NativeList<PathEdge> path = default(NativeList<PathEdge>);
				path._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
				CreatePath(startPoint, endPoint, path, prefabNetData, placeableNetData, ref m_EdgeData, ref m_NodeData, ref m_CurveData, ref m_PrefabRefData, ref m_PrefabNetData, ref m_ConnectedEdges);
				AddControlPoints(m_ControlPoints, m_UpgradeStates, m_AppliedUpgrade, startPoint, endPoint, path, m_Snap, m_RemoveUpgrade, m_LeftHandTraffic, m_EditorMode, netGeometryData, prefabRoadData, placeableNetData, subReplacement, ref m_OwnerData, ref m_EdgeData, ref m_NodeData, ref m_CurveData, ref m_CompositionData, ref m_UpgradedData, ref m_EdgeGeometryData, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabCompositionData, ref m_RoadCompositionData, ref m_ConnectedEdges, ref m_SubReplacements);
				return;
			}
			ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 1];
			ControlPoint bestSnapPosition = controlPoint;
			bestSnapPosition.m_Position = bestSnapPosition.m_HitPosition;
			bestSnapPosition.m_OriginalEntity = Entity.Null;
			HandleWorldSize(ref bestSnapPosition, controlPoint, netGeometryData);
			if ((m_Snap & Snap.ObjectSurface) != Snap.None && m_TransformData.HasComponent(controlPoint.m_OriginalEntity) && m_SubNets.HasBuffer(controlPoint.m_OriginalEntity))
			{
				bestSnapPosition.m_OriginalEntity = controlPoint.m_OriginalEntity;
			}
			float waterSurfaceHeight = float.MinValue;
			if ((m_Snap & Snap.Shoreline) != Snap.None)
			{
				SnapShoreline(ref bestSnapPosition, controlPoint, netGeometryData, ref waterSurfaceHeight);
			}
			if ((m_Snap & (Snap.CellLength | Snap.StraightDirection)) != Snap.None && m_ControlPoints.Length >= 2)
			{
				HandleControlPoints(ref bestSnapPosition, controlPoint, netGeometryData, placeableNetData);
			}
			if ((m_Snap & (Snap.ExistingGeometry | Snap.NearbyGeometry | Snap.GuideLines)) != Snap.None)
			{
				HandleExistingGeometry(ref bestSnapPosition, controlPoint, prefabRoadData, netGeometryData, prefabNetData, localConnectData, placeableNetData);
			}
			if ((m_Snap & (Snap.ExistingGeometry | Snap.ObjectSide | Snap.NearbyGeometry)) != Snap.None)
			{
				HandleExistingObjects(ref bestSnapPosition, controlPoint, prefabRoadData, netGeometryData, prefabNetData, placeableNetData);
			}
			if ((m_Snap & Snap.LotGrid) != Snap.None)
			{
				HandleLotGrid(ref bestSnapPosition, controlPoint, prefabRoadData, netGeometryData, prefabNetData, placeableNetData);
			}
			if ((m_Snap & Snap.ZoneGrid) != Snap.None)
			{
				HandleZoneGrid(ref bestSnapPosition, controlPoint, prefabRoadData, netGeometryData, prefabNetData);
			}
			ControlPoint snapTargetControlPoint = bestSnapPosition;
			if (m_Mode == Mode.Grid)
			{
				AdjustMiddlePoint(ref bestSnapPosition, netGeometryData);
				AdjustControlPointHeight(ref bestSnapPosition, controlPoint, netGeometryData, placeableNetData, waterSurfaceHeight);
			}
			else
			{
				AdjustControlPointHeight(ref bestSnapPosition, controlPoint, netGeometryData, placeableNetData, waterSurfaceHeight);
				if (m_Mode == Mode.Continuous)
				{
					AdjustMiddlePoint(ref bestSnapPosition, netGeometryData);
				}
			}
			if (m_EditorMode)
			{
				if ((m_Snap & Snap.AutoParent) == 0)
				{
					bestSnapPosition.m_OriginalEntity = Entity.Null;
				}
				else if (bestSnapPosition.m_OriginalEntity == Entity.Null)
				{
					FindParent(ref bestSnapPosition, netGeometryData);
				}
			}
			if ((m_Snap & Snap.ObjectSurface) != Snap.None && m_TransformData.HasComponent(controlPoint.m_OriginalEntity) && m_SubNets.HasBuffer(controlPoint.m_OriginalEntity) && bestSnapPosition.m_OriginalEntity == controlPoint.m_OriginalEntity)
			{
				bestSnapPosition.m_ElementIndex = controlPoint.m_ElementIndex;
			}
			else
			{
				bestSnapPosition.m_ElementIndex = int2.op_Implicit(-1);
			}
			if (CanPlaySnapSound(ref snapTargetControlPoint, ref controlPoint))
			{
				m_SourceUpdateData.AddSnap();
			}
			m_ControlPoints[m_ControlPoints.Length - 1] = bestSnapPosition;
			m_LastSnappedEntity.Value = snapTargetControlPoint.m_OriginalEntity;
		}

		private bool CanPlaySnapSound(ref ControlPoint snapTargetControlPoint, ref ControlPoint controlPoint)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			Layer layer = Layer.None;
			if (m_PrefabNetData.HasComponent(m_Prefab))
			{
				layer = m_PrefabNetData[m_Prefab].m_RequiredLayers;
				int num = 0;
				int value = m_LastControlPointsAngle.Value;
				if ((m_Snap & Snap.StraightDirection) != Snap.None && m_ControlPoints.Length >= 2 && snapTargetControlPoint.m_OriginalEntity == Entity.Null)
				{
					ControlPoint controlPoint2;
					if (m_Mode == Mode.Continuous && m_ControlPoints.Length == 3)
					{
						controlPoint2 = m_ControlPoints[0];
						controlPoint2.m_Direction = m_ControlPoints[1].m_Direction;
					}
					else
					{
						controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 2];
					}
					Segment val = default(Segment);
					((Segment)(ref val))._002Ector(controlPoint2.m_Position, snapTargetControlPoint.m_Position);
					float num2 = MathUtils.Length(((Segment)(ref val)).xz);
					if (num2 > 1f)
					{
						float2 direction = controlPoint2.m_Direction;
						float2 val2 = (((float3)(ref val.b)).xz - ((float3)(ref val.a)).xz) / num2;
						float num3 = math.dot(val2, direction);
						num = (int)math.round(math.degrees(math.atan2(val2.x * direction.y - direction.x * val2.y, num3)) + 180f);
						if (num % 180 == 0 && m_StartEntity.Value == Entity.Null)
						{
							num = 0;
						}
					}
				}
				m_LastControlPointsAngle.Value = num;
				if (snapTargetControlPoint.m_OriginalEntity != Entity.Null)
				{
					if (m_LastSnappedEntity.Value == Entity.Null && snapTargetControlPoint.m_OriginalEntity != controlPoint.m_OriginalEntity)
					{
						if (m_RoadData.HasComponent(snapTargetControlPoint.m_OriginalEntity))
						{
							return true;
						}
						Layer layer2 = Layer.None;
						PrefabRef prefabRef = default(PrefabRef);
						LocalConnectData localConnectData = default(LocalConnectData);
						if (!m_PrefabRefData.TryGetComponent(snapTargetControlPoint.m_OriginalEntity, ref prefabRef) || !m_LocalConnectData.TryGetComponent((Entity)prefabRef, ref localConnectData))
						{
							return false;
						}
						layer2 = localConnectData.m_Layers;
						layer2 = (Layer)((uint)layer2 & 0xFFFFFFFEu);
						if ((layer2 & layer) != Layer.None)
						{
							return true;
						}
						Layer layer3 = Layer.WaterPipe | Layer.SewagePipe;
						if ((layer2 & layer3) != Layer.None && (layer & layer3) != Layer.None)
						{
							return true;
						}
					}
				}
				else if (value % 360 != 0 && value != num && num % 360 != 0 && (num % 90 == 0 || (num % 45 == 0 && m_Mode == Mode.Continuous)))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private void HandleWorldSize(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, NetGeometryData prefabGeometryData)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 bounds = TerrainUtils.GetBounds(ref m_TerrainHeightData);
			float num = prefabGeometryData.m_DefaultWidth * 0.5f;
			bool2 val = bool2.op_Implicit(false);
			float2 val2 = float2.op_Implicit(0f);
			if (controlPoint.m_HitPosition.x < bounds.min.x + num)
			{
				val.x = true;
				val2.x = bounds.min.x - num;
			}
			else if (controlPoint.m_HitPosition.x > bounds.max.x - num)
			{
				val.x = true;
				val2.x = bounds.max.x + num;
			}
			if (controlPoint.m_HitPosition.z < bounds.min.z + num)
			{
				val.y = true;
				val2.y = bounds.min.z - num;
			}
			else if (controlPoint.m_HitPosition.z > bounds.max.z - num)
			{
				val.y = true;
				val2.y = bounds.max.z + num;
			}
			if (math.any(val))
			{
				ControlPoint controlPoint2 = controlPoint;
				controlPoint2.m_OriginalEntity = Entity.Null;
				controlPoint2.m_Direction = new float2(0f, 1f);
				((float3)(ref controlPoint2.m_Position)).xz = math.select(((float3)(ref controlPoint.m_HitPosition)).xz, val2, val);
				controlPoint2.m_Position.y = controlPoint.m_HitPosition.y;
				controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(2f, 1f, 0f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
				ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
				if (val.x)
				{
					Line3 val3 = default(Line3);
					((Line3)(ref val3))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
					val3.a.z = bounds.min.z;
					val3.b.z = bounds.max.z;
					ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val3.a, val3.b), SnapLineFlags.Hidden, 0f));
				}
				if (val.y)
				{
					controlPoint2.m_Direction = new float2(1f, 0f);
					Line3 val4 = default(Line3);
					((Line3)(ref val4))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
					val4.a.x = bounds.min.x;
					val4.b.x = bounds.max.x;
					ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val4.a, val4.b), SnapLineFlags.Hidden, 0f));
				}
			}
		}

		private void FindParent(ref ControlPoint bestSnapPosition, NetGeometryData prefabGeometryData)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
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
			Segment val = default(Segment);
			if (m_ControlPoints.Length >= 2)
			{
				((Segment)(ref val))._002Ector(m_ControlPoints[m_ControlPoints.Length - 2].m_Position, bestSnapPosition.m_Position);
			}
			else
			{
				((Segment)(ref val))._002Ector(bestSnapPosition.m_Position, bestSnapPosition.m_Position);
			}
			float num = math.max(0.01f, prefabGeometryData.m_DefaultWidth * 0.5f - 0.5f);
			ParentObjectIterator parentObjectIterator = new ParentObjectIterator
			{
				m_BestSnapPosition = bestSnapPosition,
				m_Line = val,
				m_Bounds = MathUtils.Expand(MathUtils.Bounds(val), float3.op_Implicit(num + 0.4f)),
				m_Radius = num,
				m_OwnerData = m_OwnerData,
				m_TransformData = m_TransformData,
				m_BuildingData = m_BuildingData,
				m_BuildingExtensionData = m_BuildingExtensionData,
				m_AssetStampData = m_AssetStampData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabObjectGeometryData = m_ObjectGeometryData
			};
			m_ObjectSearchTree.Iterate<ParentObjectIterator>(ref parentObjectIterator, 0);
			bestSnapPosition = parentObjectIterator.m_BestSnapPosition;
		}

		private void HandleLotGrid(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, RoadData prefabRoadData, NetGeometryData prefabGeometryData, NetData prefabNetData, PlaceableNetData placeableNetData)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			int cellWidth = ZoneUtils.GetCellWidth(prefabGeometryData.m_DefaultWidth);
			float num = (float)(cellWidth + 1) * (placeableNetData.m_SnapDistance * 0.5f) * 1.4142135f;
			float edgeOffset = 0f;
			float maxDistance = placeableNetData.m_SnapDistance * 0.5f + 0.1f;
			if (m_PrefabLaneData.HasComponent(m_LanePrefab))
			{
				edgeOffset = m_PrefabLaneData[m_LanePrefab].m_Width * 0.5f;
			}
			LotIterator lotIterator = new LotIterator
			{
				m_Bounds = new Bounds2(((float3)(ref controlPoint.m_HitPosition)).xz - num, ((float3)(ref controlPoint.m_HitPosition)).xz + num),
				m_Radius = prefabGeometryData.m_DefaultWidth * 0.5f,
				m_EdgeOffset = edgeOffset,
				m_MaxDistance = maxDistance,
				m_CellWidth = cellWidth,
				m_ControlPoint = controlPoint,
				m_BestSnapPosition = bestSnapPosition,
				m_SnapLines = m_SnapLines,
				m_OwnerData = m_OwnerData,
				m_EdgeData = m_EdgeData,
				m_NodeData = m_NodeData,
				m_TransformData = m_TransformData,
				m_PrefabRefData = m_PrefabRefData,
				m_BuildingData = m_BuildingData,
				m_BuildingExtensionData = m_BuildingExtensionData,
				m_AssetStampData = m_AssetStampData,
				m_PrefabObjectGeometryData = m_ObjectGeometryData
			};
			m_ObjectSearchTree.Iterate<LotIterator>(ref lotIterator, 0);
			bestSnapPosition = lotIterator.m_BestSnapPosition;
		}

		private void SnapShoreline(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, NetGeometryData prefabGeometryData, ref float waterSurfaceHeight)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			float num = prefabGeometryData.m_DefaultWidth * 2f + 10f;
			float3 val = WaterUtils.ToSurfaceSpace(ref m_WaterSurfaceData, controlPoint.m_HitPosition - num);
			int2 val2 = (int2)math.floor(((float3)(ref val)).xz);
			val = WaterUtils.ToSurfaceSpace(ref m_WaterSurfaceData, controlPoint.m_HitPosition + num);
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
						float num2 = TerrainUtils.SampleHeight(ref m_TerrainHeightData, worldPosition) + worldPosition.y;
						float num3 = math.max(0f, num * num - math.distancesq(((float3)(ref worldPosition)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz));
						worldPosition.y = (worldPosition.y - 0.2f) * num3;
						((float3)(ref worldPosition)).xz = ((float3)(ref worldPosition)).xz * worldPosition.y;
						val6 += worldPosition;
						num2 *= num3;
						val7 += new float2(num2, num3);
					}
					else if (worldPosition.y < 0.2f)
					{
						float num4 = math.max(0f, num * num - math.distancesq(((float3)(ref worldPosition)).xz, ((float3)(ref controlPoint.m_HitPosition)).xz));
						worldPosition.y = (0.2f - worldPosition.y) * num4;
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
					float num5 = 8f / num;
					waterSurfaceHeight = val7.x / val7.y;
					ControlPoint controlPoint2 = controlPoint;
					((float3)(ref controlPoint2.m_Position)).xz = math.lerp(((float3)(ref val6)).xz, ((float3)(ref val5)).xz, 0.5f);
					controlPoint2.m_Position.y = waterSurfaceHeight;
					ref float3 position = ref controlPoint2.m_Position;
					position += val8;
					controlPoint2.m_Direction = ((float3)(ref val8)).xz;
					controlPoint2.m_Rotation = ToolUtils.CalculateRotation(controlPoint2.m_Direction);
					controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition * num5, controlPoint2.m_Position * num5, controlPoint2.m_Direction);
					controlPoint2.m_OriginalEntity = Entity.Null;
					float3 startPos = controlPoint2.m_Position + val8 * num;
					float3 endPos = controlPoint2.m_Position - val8 * num;
					ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
					ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(startPos, endPos), SnapLineFlags.Hidden, 0f));
				}
			}
		}

		private void AdjustControlPointHeight(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, float waterSurfaceHeight)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			float y = bestSnapPosition.m_Position.y;
			if ((m_Snap & Snap.ObjectSurface) == 0 || !m_TransformData.HasComponent(controlPoint.m_OriginalEntity))
			{
				if (m_Elevation < 0f)
				{
					bestSnapPosition.m_Position.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, bestSnapPosition.m_Position) + m_Elevation;
				}
				else
				{
					bestSnapPosition.m_Position.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, bestSnapPosition.m_Position, out var waterDepth);
					float num = m_Elevation;
					if (waterDepth >= 0.2f && (placeableNetData.m_PlacementFlags & (Game.Net.PlacementFlags.OnGround | Game.Net.PlacementFlags.Floating)) == Game.Net.PlacementFlags.OnGround)
					{
						num = math.max(m_Elevation, placeableNetData.m_MinWaterElevation);
						bestSnapPosition.m_Elevation = math.max(bestSnapPosition.m_Elevation, placeableNetData.m_MinWaterElevation);
					}
					else if ((m_Snap & Snap.Shoreline) != Snap.None)
					{
						float num2 = math.max(m_Elevation, placeableNetData.m_MinWaterElevation);
						if (waterSurfaceHeight + num2 > bestSnapPosition.m_Position.y)
						{
							num = num2;
							bestSnapPosition.m_Elevation = math.max(bestSnapPosition.m_Elevation, placeableNetData.m_MinWaterElevation);
							bestSnapPosition.m_Position.y = waterSurfaceHeight;
						}
					}
					bestSnapPosition.m_Position.y += num;
				}
			}
			else
			{
				bestSnapPosition.m_Position.y = controlPoint.m_HitPosition.y;
			}
			Bounds1 val = prefabGeometryData.m_DefaultHeightRange + bestSnapPosition.m_Position.y;
			if (m_PrefabRefData.HasComponent(controlPoint.m_OriginalEntity))
			{
				PrefabRef prefabRef = m_PrefabRefData[controlPoint.m_OriginalEntity];
				if (m_PrefabGeometryData.HasComponent(prefabRef.m_Prefab))
				{
					Bounds1 val2 = m_PrefabGeometryData[prefabRef.m_Prefab].m_DefaultHeightRange + controlPoint.m_Position.y;
					if (val2.max > val.min)
					{
						val.max = math.max(val.max, val2.max);
						if (bestSnapPosition.m_OriginalEntity == Entity.Null)
						{
							bestSnapPosition.m_OriginalEntity = controlPoint.m_OriginalEntity;
						}
					}
				}
			}
			if (!m_PrefabRefData.HasComponent(bestSnapPosition.m_OriginalEntity))
			{
				return;
			}
			PrefabRef prefabRef2 = m_PrefabRefData[bestSnapPosition.m_OriginalEntity];
			if (!m_PrefabGeometryData.HasComponent(prefabRef2.m_Prefab))
			{
				return;
			}
			NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef2.m_Prefab];
			Bounds1 val3 = netGeometryData.m_DefaultHeightRange + y;
			if (MathUtils.Intersect(val, val3))
			{
				if (((prefabGeometryData.m_MergeLayers ^ netGeometryData.m_MergeLayers) & Layer.Waterway) == 0)
				{
					bestSnapPosition.m_Elevation += y - bestSnapPosition.m_Position.y;
					bestSnapPosition.m_Position.y = y;
					bestSnapPosition.m_Elevation = MathUtils.Clamp(bestSnapPosition.m_Elevation, placeableNetData.m_ElevationRange);
				}
			}
			else
			{
				bestSnapPosition.m_OriginalEntity = Entity.Null;
			}
		}

		private void AdjustMiddlePoint(ref ControlPoint bestSnapPosition, NetGeometryData netGeometryData)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			float2 val = (((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) == 0) ? (netGeometryData.m_DefaultWidth * new float2(16f, 8f)) : ((float)ZoneUtils.GetCellWidth(netGeometryData.m_DefaultWidth) * 8f + new float2(192f, 96f)));
			float2 val2 = val * 11f;
			if (m_ControlPoints.Length == 2)
			{
				ControlPoint controlPoint = m_ControlPoints[m_ControlPoints.Length - 2];
				float2 val3 = ((float3)(ref bestSnapPosition.m_Position)).xz - ((float3)(ref controlPoint.m_Position)).xz;
				if (MathUtils.TryNormalize(ref val3))
				{
					bestSnapPosition.m_Direction = val3;
				}
				if (m_Mode == Mode.Grid && math.distance(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref bestSnapPosition.m_Position)).xz) > val2.x)
				{
					((float3)(ref bestSnapPosition.m_Position)).xz = ((float3)(ref controlPoint.m_Position)).xz + val3 * val2.x;
					bestSnapPosition.m_OriginalEntity = Entity.Null;
				}
			}
			else
			{
				if (m_ControlPoints.Length != 3)
				{
					return;
				}
				ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 3];
				ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 2];
				if (m_Mode == Mode.Grid)
				{
					float2 val4 = ((float3)(ref bestSnapPosition.m_Position)).xz - ((float3)(ref controlPoint2.m_Position)).xz;
					float2 val5 = default(float2);
					((float2)(ref val5))._002Ector(math.dot(val4, controlPoint3.m_Direction), math.dot(val4, MathUtils.Right(controlPoint3.m_Direction)));
					bool2 val6 = math.abs(val5) > val2;
					val5 = math.select(val5, math.select(val2, -val2, val5 < 0f), val6);
					controlPoint3.m_Position = controlPoint2.m_Position;
					ref float3 position = ref controlPoint3.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + controlPoint3.m_Direction * val5.x;
					if (math.any(val6))
					{
						((float3)(ref bestSnapPosition.m_Position)).xz = ((float3)(ref controlPoint3.m_Position)).xz + MathUtils.Right(controlPoint3.m_Direction) * val5.y;
						bestSnapPosition.m_OriginalEntity = Entity.Null;
					}
				}
				else
				{
					controlPoint3.m_Elevation = (controlPoint2.m_Elevation + bestSnapPosition.m_Elevation) * 0.5f;
					float2 val7 = ((float3)(ref bestSnapPosition.m_Position)).xz - ((float3)(ref controlPoint2.m_Position)).xz;
					float2 direction = controlPoint3.m_Direction;
					float2 val8 = val7;
					if (MathUtils.TryNormalize(ref val8))
					{
						float num = math.dot(direction, val8);
						if (num >= 0.70710677f)
						{
							float2 val9 = math.lerp(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref bestSnapPosition.m_Position)).xz, 0.5f);
							Line2 val10 = new Line2(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz + direction);
							Line2 val11 = default(Line2);
							((Line2)(ref val11))._002Ector(val9, val9 + MathUtils.Right(val8));
							float2 val12 = default(float2);
							if (MathUtils.Intersect(val10, val11, ref val12))
							{
								controlPoint3.m_Position = controlPoint2.m_Position;
								ref float3 position2 = ref controlPoint3.m_Position;
								((float3)(ref position2)).xz = ((float3)(ref position2)).xz + direction * val12.x;
								float2 direction2 = ((float3)(ref bestSnapPosition.m_Position)).xz - ((float3)(ref controlPoint3.m_Position)).xz;
								if (MathUtils.TryNormalize(ref direction2))
								{
									bestSnapPosition.m_Direction = direction2;
								}
							}
						}
						else if (num >= 0f)
						{
							float2 val13 = math.lerp(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref bestSnapPosition.m_Position)).xz, 0.5f);
							Line2 val14 = default(Line2);
							((Line2)(ref val14))._002Ector(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz + MathUtils.Right(direction));
							Line2 val15 = default(Line2);
							((Line2)(ref val15))._002Ector(val13, val13 + MathUtils.Right(val8));
							float2 val16 = default(float2);
							if (MathUtils.Intersect(val14, val15, ref val16))
							{
								controlPoint3.m_Position = controlPoint2.m_Position;
								ref float3 position3 = ref controlPoint3.m_Position;
								((float3)(ref position3)).xz = ((float3)(ref position3)).xz + direction * math.abs(val16.x);
								float2 val17 = ((float3)(ref bestSnapPosition.m_Position)).xz - MathUtils.Position(val14, val16.x);
								if (MathUtils.TryNormalize(ref val17))
								{
									bestSnapPosition.m_Direction = math.select(MathUtils.Right(val17), MathUtils.Left(val17), math.dot(MathUtils.Right(direction), val8) < 0f);
								}
							}
						}
						else
						{
							controlPoint3.m_Position = controlPoint2.m_Position;
							ref float3 position4 = ref controlPoint3.m_Position;
							((float3)(ref position4)).xz = ((float3)(ref position4)).xz + direction * math.abs(math.dot(val7, MathUtils.Right(direction)) * 0.5f);
							bestSnapPosition.m_Direction = -controlPoint3.m_Direction;
						}
					}
					else
					{
						controlPoint3.m_Position = controlPoint2.m_Position;
					}
				}
				if (controlPoint3.m_Elevation < 0f)
				{
					controlPoint3.m_Position.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, controlPoint3.m_Position) + controlPoint3.m_Elevation;
				}
				else
				{
					controlPoint3.m_Position.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, controlPoint3.m_Position) + controlPoint3.m_Elevation;
				}
				m_ControlPoints[m_ControlPoints.Length - 2] = controlPoint3;
			}
		}

		private void HandleControlPoints(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0634: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_055a: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint2 = controlPoint;
			controlPoint2.m_OriginalEntity = Entity.Null;
			controlPoint2.m_Position = controlPoint.m_HitPosition;
			float num = placeableNetData.m_SnapDistance;
			ControlPoint controlPoint3;
			if (m_Mode == Mode.Grid && m_ControlPoints.Length == 3)
			{
				if ((m_Snap & Snap.CellLength) != Snap.None)
				{
					controlPoint3 = m_ControlPoints[0];
					float2 xz = ((float3)(ref controlPoint3.m_Position)).xz;
					float2 direction = m_ControlPoints[1].m_Direction;
					float2 val = MathUtils.Right(direction);
					float2 val2 = ((float3)(ref controlPoint.m_HitPosition)).xz - xz;
					((float2)(ref val2))._002Ector(math.dot(val2, direction), math.dot(val2, val));
					val2 = MathUtils.Snap(val2, float2.op_Implicit(num));
					xz += val2.x * direction + val2.y * val;
					controlPoint2.m_Direction = direction;
					((float3)(ref controlPoint2.m_Position)).xz = xz;
					controlPoint2.m_Position.y = controlPoint.m_HitPosition.y;
					controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
					Line3 val3 = default(Line3);
					((Line3)(ref val3))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
					Line3 val4 = default(Line3);
					((Line3)(ref val4))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
					ref float3 a = ref val3.a;
					((float3)(ref a)).xz = ((float3)(ref a)).xz - controlPoint2.m_Direction * 8f;
					ref float3 b = ref val3.b;
					((float3)(ref b)).xz = ((float3)(ref b)).xz + controlPoint2.m_Direction * 8f;
					ref float3 a2 = ref val4.a;
					((float3)(ref a2)).xz = ((float3)(ref a2)).xz - MathUtils.Right(controlPoint2.m_Direction) * 8f;
					ref float3 b2 = ref val4.b;
					((float3)(ref b2)).xz = ((float3)(ref b2)).xz + MathUtils.Right(controlPoint2.m_Direction) * 8f;
					ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
					ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val3.a, val3.b), SnapLineFlags.Hidden, 0f));
					controlPoint2.m_Direction = MathUtils.Right(controlPoint2.m_Direction);
					ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val4.a, val4.b), SnapLineFlags.Hidden, 0f));
				}
				return;
			}
			ControlPoint prev;
			if (m_Mode == Mode.Continuous && m_ControlPoints.Length == 3)
			{
				prev = m_ControlPoints[0];
				prev.m_OriginalEntity = Entity.Null;
				prev.m_Direction = m_ControlPoints[1].m_Direction;
			}
			else
			{
				prev = m_ControlPoints[m_ControlPoints.Length - 2];
				if (((float2)(ref prev.m_Direction)).Equals(default(float2)) && m_ControlPoints.Length >= 3)
				{
					float2 xz2 = ((float3)(ref prev.m_Position)).xz;
					controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 3];
					prev.m_Direction = math.normalizesafe(xz2 - ((float3)(ref controlPoint3.m_Position)).xz, default(float2));
				}
			}
			float3 snapDirection = controlPoint.m_HitPosition - prev.m_Position;
			snapDirection = MathUtils.Normalize(snapDirection, ((float3)(ref snapDirection)).xz);
			snapDirection.y = math.clamp(snapDirection.y, -1f, 1f);
			bool flag = false;
			bool flag2 = false;
			if ((m_Snap & Snap.StraightDirection) != Snap.None)
			{
				float bestDirectionDistance = float.MaxValue;
				if (prev.m_OriginalEntity != Entity.Null)
				{
					HandleStartDirection(prev.m_OriginalEntity, prev, controlPoint, placeableNetData, ref bestDirectionDistance, ref controlPoint2.m_Position, ref snapDirection);
				}
				if (m_StartEntity.Value != Entity.Null && m_StartEntity.Value != prev.m_OriginalEntity && m_ControlPoints.Length == 2)
				{
					HandleStartDirection(m_StartEntity.Value, prev, controlPoint, placeableNetData, ref bestDirectionDistance, ref controlPoint2.m_Position, ref snapDirection);
				}
				if (!((float2)(ref prev.m_Direction)).Equals(default(float2)) && bestDirectionDistance == float.MaxValue)
				{
					ToolUtils.DirectionSnap(ref bestDirectionDistance, ref controlPoint2.m_Position, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, new float3(prev.m_Direction.x, 0f, prev.m_Direction.y), placeableNetData.m_SnapDistance);
					if (bestDirectionDistance >= placeableNetData.m_SnapDistance && m_Mode == Mode.Continuous && m_ControlPoints.Length == 3)
					{
						float2 val5 = MathUtils.RotateLeft(prev.m_Direction, (float)Math.PI / 4f);
						ToolUtils.DirectionSnap(ref bestDirectionDistance, ref controlPoint2.m_Position, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, new float3(val5.x, 0f, val5.y), placeableNetData.m_SnapDistance);
						val5 = MathUtils.RotateRight(prev.m_Direction, (float)Math.PI / 4f);
						ToolUtils.DirectionSnap(ref bestDirectionDistance, ref controlPoint2.m_Position, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, new float3(val5.x, 0f, val5.y), placeableNetData.m_SnapDistance);
						num *= 1.4142135f;
					}
				}
				flag = bestDirectionDistance < placeableNetData.m_SnapDistance;
				flag2 = bestDirectionDistance < placeableNetData.m_SnapDistance;
			}
			if ((m_Snap & Snap.CellLength) != Snap.None && (m_Mode != Mode.Continuous || (m_ControlPoints.Length == 3 && flag2)))
			{
				float num2 = math.distance(prev.m_Position, controlPoint2.m_Position);
				controlPoint2.m_Position = prev.m_Position + snapDirection * MathUtils.Snap(num2, num);
				flag = true;
			}
			controlPoint2.m_Direction = ((float3)(ref snapDirection)).xz;
			controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 0f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
			if (flag)
			{
				ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
			}
			if (flag2)
			{
				float3 position = controlPoint2.m_Position;
				float3 endPos = position;
				((float3)(ref endPos)).xz = ((float3)(ref endPos)).xz + controlPoint2.m_Direction;
				ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(position, endPos), GetSnapLineFlags(prefabGeometryData.m_Flags) | SnapLineFlags.Hidden, 0f));
			}
		}

		private void HandleStartDirection(Entity startEntity, ControlPoint prev, ControlPoint controlPoint, PlaceableNetData placeableNetData, ref float bestDirectionDistance, ref float3 snapPosition, ref float3 snapDirection)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			if (m_ConnectedEdges.HasBuffer(startEntity))
			{
				DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[startEntity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity edge = val[i].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (!(edge2.m_Start != startEntity) || !(edge2.m_End != startEntity))
					{
						Curve curve = m_CurveData[edge];
						float3 val2 = ((edge2.m_Start == startEntity) ? MathUtils.StartTangent(curve.m_Bezier) : MathUtils.EndTangent(curve.m_Bezier));
						val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
						val2.y = math.clamp(val2.y, -1f, 1f);
						ToolUtils.DirectionSnap(ref bestDirectionDistance, ref snapPosition, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, val2, placeableNetData.m_SnapDistance);
					}
				}
			}
			else if (m_CurveData.HasComponent(startEntity))
			{
				float3 val3 = MathUtils.Tangent(m_CurveData[startEntity].m_Bezier, prev.m_CurvePosition);
				val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz);
				val3.y = math.clamp(val3.y, -1f, 1f);
				ToolUtils.DirectionSnap(ref bestDirectionDistance, ref snapPosition, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, val3, placeableNetData.m_SnapDistance);
			}
			else if (m_TransformData.HasComponent(startEntity))
			{
				float3 val4 = math.forward(m_TransformData[startEntity].m_Rotation);
				val4 = MathUtils.Normalize(val4, ((float3)(ref val4)).xz);
				val4.y = math.clamp(val4.y, -1f, 1f);
				ToolUtils.DirectionSnap(ref bestDirectionDistance, ref snapPosition, ref snapDirection, controlPoint.m_HitPosition, prev.m_Position, val4, placeableNetData.m_SnapDistance);
			}
		}

		private void HandleZoneGrid(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, RoadData prefabRoadData, NetGeometryData prefabGeometryData, NetData prefabNetData)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			int cellWidth = ZoneUtils.GetCellWidth(prefabGeometryData.m_DefaultWidth);
			float num = (float)(cellWidth + 1) * 4f * 1.4142135f;
			float num2 = math.select(0f, 4f, (cellWidth & 1) == 0);
			ZoneIterator zoneIterator = new ZoneIterator
			{
				m_Bounds = new Bounds2(((float3)(ref controlPoint.m_HitPosition)).xz - num, ((float3)(ref controlPoint.m_HitPosition)).xz + num),
				m_HitPosition = ((float3)(ref controlPoint.m_HitPosition)).xz,
				m_BestDistance = num,
				m_ZoneBlockData = m_ZoneBlockData,
				m_ZoneCells = m_ZoneCells
			};
			m_ZoneSearchTree.Iterate<ZoneIterator>(ref zoneIterator, 0);
			if (zoneIterator.m_BestDistance < num)
			{
				float2 val = ((float3)(ref controlPoint.m_HitPosition)).xz - ((float3)(ref zoneIterator.m_BestPosition)).xz;
				float2 val2 = MathUtils.Right(zoneIterator.m_BestDirection);
				float num3 = MathUtils.Snap(math.dot(val, zoneIterator.m_BestDirection), 8f, num2);
				float num4 = MathUtils.Snap(math.dot(val, val2), 8f, num2);
				ControlPoint controlPoint2 = controlPoint;
				if (!m_EdgeData.HasComponent(controlPoint.m_OriginalEntity) && !m_NodeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					controlPoint2.m_OriginalEntity = Entity.Null;
				}
				controlPoint2.m_Direction = zoneIterator.m_BestDirection;
				((float3)(ref controlPoint2.m_Position)).xz = ((float3)(ref zoneIterator.m_BestPosition)).xz + zoneIterator.m_BestDirection * num3 + val2 * num4;
				controlPoint2.m_SnapPriority = ToolUtils.CalculateSnapPriority(0f, 1f, 1f, controlPoint.m_HitPosition, controlPoint2.m_Position, controlPoint2.m_Direction);
				Line3 val3 = default(Line3);
				((Line3)(ref val3))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
				Line3 val4 = default(Line3);
				((Line3)(ref val4))._002Ector(controlPoint2.m_Position, controlPoint2.m_Position);
				ref float3 a = ref val3.a;
				((float3)(ref a)).xz = ((float3)(ref a)).xz - controlPoint2.m_Direction * 8f;
				ref float3 b = ref val3.b;
				((float3)(ref b)).xz = ((float3)(ref b)).xz + controlPoint2.m_Direction * 8f;
				ref float3 a2 = ref val4.a;
				((float3)(ref a2)).xz = ((float3)(ref a2)).xz - MathUtils.Right(controlPoint2.m_Direction) * 8f;
				ref float3 b2 = ref val4.b;
				((float3)(ref b2)).xz = ((float3)(ref b2)).xz + MathUtils.Right(controlPoint2.m_Direction) * 8f;
				ToolUtils.AddSnapPosition(ref bestSnapPosition, controlPoint2);
				ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val3.a, val3.b), SnapLineFlags.Hidden, 1f));
				controlPoint2.m_Direction = MathUtils.Right(controlPoint2.m_Direction);
				ToolUtils.AddSnapLine(ref bestSnapPosition, m_SnapLines, new SnapLine(controlPoint2, NetUtils.StraightCurve(val4.a, val4.b), SnapLineFlags.Hidden, 1f));
			}
		}

		private void HandleExistingObjects(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, RoadData prefabRoadData, NetGeometryData prefabGeometryData, NetData prefabNetData, PlaceableNetData placeableNetData)
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			float num = (((m_Snap & Snap.NearbyGeometry) != Snap.None) ? placeableNetData.m_SnapDistance : 0f);
			float num2 = (((prefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) == 0 || (m_Snap & Snap.CellLength) == 0) ? (prefabGeometryData.m_DefaultWidth * 0.5f) : ((float)ZoneUtils.GetCellWidth(prefabGeometryData.m_DefaultWidth) * 4f));
			float num3 = 0f;
			if ((m_Snap & (Snap.ExistingGeometry | Snap.NearbyGeometry)) != Snap.None)
			{
				num3 = math.max(num3, prefabGeometryData.m_DefaultWidth + num);
			}
			if ((m_Snap & Snap.ObjectSide) != Snap.None)
			{
				num3 = math.max(num3, num2 + placeableNetData.m_SnapDistance);
			}
			ObjectIterator objectIterator = new ObjectIterator
			{
				m_Bounds = new Bounds3(controlPoint.m_HitPosition - num3, controlPoint.m_HitPosition + num3),
				m_Snap = m_Snap,
				m_MaxDistance = placeableNetData.m_SnapDistance,
				m_NetSnapOffset = num,
				m_ObjectSnapOffset = num2,
				m_SnapCellLength = ((prefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0 && (m_Snap & Snap.CellLength) != 0),
				m_NetData = prefabNetData,
				m_NetGeometryData = prefabGeometryData,
				m_ControlPoint = controlPoint,
				m_BestSnapPosition = bestSnapPosition,
				m_SnapLines = m_SnapLines,
				m_OwnerData = m_OwnerData,
				m_CurveData = m_CurveData,
				m_NodeData = m_NodeData,
				m_TransformData = m_TransformData,
				m_PrefabRefData = m_PrefabRefData,
				m_BuildingData = m_BuildingData,
				m_ObjectGeometryData = m_ObjectGeometryData,
				m_PrefabNetData = m_PrefabNetData,
				m_PrefabGeometryData = m_PrefabGeometryData,
				m_ConnectedEdges = m_ConnectedEdges
			};
			m_ObjectSearchTree.Iterate<ObjectIterator>(ref objectIterator, 0);
			bestSnapPosition = objectIterator.m_BestSnapPosition;
		}

		private static SnapLineFlags GetSnapLineFlags(Game.Net.GeometryFlags geometryFlags)
		{
			SnapLineFlags snapLineFlags = (SnapLineFlags)0;
			if ((geometryFlags & Game.Net.GeometryFlags.StrictNodes) == 0)
			{
				snapLineFlags |= SnapLineFlags.ExtendedCurve;
			}
			return snapLineFlags;
		}

		private void HandleExistingGeometry(ref ControlPoint bestSnapPosition, ControlPoint controlPoint, RoadData prefabRoadData, NetGeometryData prefabGeometryData, NetData prefabNetData, LocalConnectData localConnectData, PlaceableNetData placeableNetData)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			float num = (((m_Snap & Snap.NearbyGeometry) != Snap.None) ? placeableNetData.m_SnapDistance : 0f);
			float num2 = prefabGeometryData.m_DefaultWidth + num;
			float num3 = placeableNetData.m_SnapDistance * 64f;
			Bounds1 val = new Bounds1(-50f, 50f) | localConnectData.m_HeightRange;
			Bounds3 val2 = default(Bounds3);
			((Bounds3)(ref val2)).xz = new Bounds2(((float3)(ref controlPoint.m_HitPosition)).xz - num2, ((float3)(ref controlPoint.m_HitPosition)).xz + num2);
			((Bounds3)(ref val2)).y = controlPoint.m_HitPosition.y + val;
			Bounds3 totalBounds = val2;
			if ((m_Snap & Snap.GuideLines) != Snap.None)
			{
				ref float3 min = ref totalBounds.min;
				min -= num3;
				ref float3 max = ref totalBounds.max;
				max += num3;
			}
			float num4 = -1f;
			if ((prefabGeometryData.m_Flags & (Game.Net.GeometryFlags.SnapToNetAreas | Game.Net.GeometryFlags.StandingNodes)) != 0 && m_SubObjects.HasBuffer(m_Prefab))
			{
				DynamicBuffer<Game.Prefabs.SubObject> val3 = m_SubObjects[m_Prefab];
				for (int i = 0; i < val3.Length; i++)
				{
					Game.Prefabs.SubObject subObject = val3[i];
					if (m_ObjectGeometryData.HasComponent(subObject.m_Prefab))
					{
						ObjectGeometryData objectGeometryData = m_ObjectGeometryData[subObject.m_Prefab];
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							num4 = math.max(num4, objectGeometryData.m_LegSize.x + objectGeometryData.m_LegOffset.x * 2f);
						}
					}
				}
			}
			num4 = math.select(num4, prefabGeometryData.m_DefaultWidth, num4 <= 0f);
			NetIterator netIterator = new NetIterator
			{
				m_TotalBounds = totalBounds,
				m_Bounds = val2,
				m_Snap = m_Snap,
				m_ServiceUpgradeOwner = m_ServiceUpgradeOwner,
				m_SnapOffset = num,
				m_SnapDistance = placeableNetData.m_SnapDistance,
				m_Elevation = m_Elevation,
				m_GuideLength = num3,
				m_LegSnapWidth = num4,
				m_HeightRange = val,
				m_NetData = prefabNetData,
				m_PrefabRoadData = prefabRoadData,
				m_NetGeometryData = prefabGeometryData,
				m_LocalConnectData = localConnectData,
				m_ControlPoint = controlPoint,
				m_BestSnapPosition = bestSnapPosition,
				m_SnapLines = m_SnapLines,
				m_TerrainHeightData = m_TerrainHeightData,
				m_WaterSurfaceData = m_WaterSurfaceData,
				m_OwnerData = m_OwnerData,
				m_EditorMode = m_EditorMode,
				m_NodeData = m_NodeData,
				m_EdgeData = m_EdgeData,
				m_CurveData = m_CurveData,
				m_CompositionData = m_CompositionData,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_RoadData = m_RoadData,
				m_PrefabRefData = m_PrefabRefData,
				m_PrefabNetData = m_PrefabNetData,
				m_PrefabGeometryData = m_PrefabGeometryData,
				m_PrefabCompositionData = m_PrefabCompositionData,
				m_RoadCompositionData = m_RoadCompositionData,
				m_ConnectedEdges = m_ConnectedEdges,
				m_SubNets = m_SubNets,
				m_PrefabCompositionAreas = m_PrefabCompositionAreas
			};
			if ((m_Snap & Snap.ExistingGeometry) != Snap.None && m_PrefabRefData.HasComponent(controlPoint.m_OriginalEntity))
			{
				PrefabRef prefabRef = m_PrefabRefData[controlPoint.m_OriginalEntity];
				if (!netIterator.HandleGeometry(controlPoint, controlPoint.m_HitPosition.y, prefabRef, ignoreHeightDistance: true) && (m_Snap & Snap.GuideLines) != Snap.None)
				{
					netIterator.HandleGuideLines(controlPoint.m_OriginalEntity);
				}
			}
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			bestSnapPosition = netIterator.m_BestSnapPosition;
		}
	}

	[BurstCompile]
	public struct FixControlPointsJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		public NativeList<ControlPoint> m_ControlPoints;

		public void Execute()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			Edge edge = default(Edge);
			Edge edge2 = default(Edge);
			Temp temp2 = default(Temp);
			Temp temp3 = default(Temp);
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				ArchetypeChunk val2 = m_Chunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(m_EntityType);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<Temp>(ref m_TempType);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Temp temp = nativeArray2[j];
					Entity val3 = nativeArray[j];
					if ((temp.m_Flags & TempFlags.Delete) != 0)
					{
						if (temp.m_Original != Entity.Null)
						{
							FixControlPoints(temp.m_Original, Entity.Null);
						}
					}
					else if ((temp.m_Flags & (TempFlags.Replace | TempFlags.Combine)) != 0)
					{
						if (temp.m_Original != Entity.Null)
						{
							FixControlPoints(temp.m_Original, val3);
						}
					}
					else if ((temp.m_Flags & TempFlags.Modify) != 0 && m_EdgeData.TryGetComponent(val3, ref edge) && m_EdgeData.TryGetComponent(temp.m_Original, ref edge2) && ((m_TempData.TryGetComponent(edge.m_Start, ref temp2) && temp2.m_Original == edge2.m_End) || (m_TempData.TryGetComponent(edge.m_End, ref temp3) && temp3.m_Original == edge2.m_Start)))
					{
						InverseCurvePositions(temp.m_Original);
					}
					if ((temp.m_Flags & TempFlags.IsLast) != 0)
					{
						val = (((temp.m_Flags & (TempFlags.Create | TempFlags.Replace)) == 0) ? temp.m_Original : val3);
					}
				}
			}
			if (val != Entity.Null && m_Mode != Mode.Replace)
			{
				for (int k = 0; k < m_ControlPoints.Length; k++)
				{
					ControlPoint controlPoint = m_ControlPoints[k];
					controlPoint.m_OriginalEntity = val;
					m_ControlPoints[k] = controlPoint;
				}
			}
		}

		private void FixControlPoints(Entity entity, Entity replace)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (!(entity != Entity.Null))
			{
				return;
			}
			for (int i = 0; i < m_ControlPoints.Length; i++)
			{
				ControlPoint controlPoint = m_ControlPoints[i];
				if (controlPoint.m_OriginalEntity == entity)
				{
					controlPoint.m_OriginalEntity = replace;
					m_ControlPoints[i] = controlPoint;
				}
			}
		}

		private void InverseCurvePositions(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (!(entity != Entity.Null))
			{
				return;
			}
			for (int i = 0; i < m_ControlPoints.Length; i++)
			{
				ControlPoint controlPoint = m_ControlPoints[i];
				if (controlPoint.m_OriginalEntity == entity)
				{
					controlPoint.m_CurvePosition = 1f - controlPoint.m_CurvePosition;
					m_ControlPoints[i] = controlPoint;
				}
			}
		}
	}

	[BurstCompile]
	public struct CreateDefinitionsJob : IJob
	{
		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_RemoveUpgrade;

		[ReadOnly]
		public bool m_LefthandTraffic;

		[ReadOnly]
		public Mode m_Mode;

		[ReadOnly]
		public int2 m_ParallelCount;

		[ReadOnly]
		public float m_ParallelOffset;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public AgeMask m_AgeMask;

		[ReadOnly]
		public NativeList<ControlPoint> m_ControlPoints;

		[ReadOnly]
		public NativeList<UpgradeState> m_UpgradeStates;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Fixed> m_FixedData;

		[ReadOnly]
		public ComponentLookup<EditorContainer> m_EditorContainerData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Extension> m_ExtensionData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_NetGeometryData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableData;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> m_PrefabSpawnableObjectData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		[ReadOnly]
		public BufferLookup<SubReplacement> m_SubReplacements;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> m_CachedNodes;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> m_AreaNodes;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> m_PrefabSubObjects;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> m_PrefabSubNets;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> m_PrefabSubAreas;

		[ReadOnly]
		public BufferLookup<SubAreaNode> m_PrefabSubAreaNodes;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> m_PrefabPlaceholderElements;

		[ReadOnly]
		public Entity m_NetPrefab;

		[ReadOnly]
		public Entity m_LanePrefab;

		[ReadOnly]
		public Entity m_ServiceUpgradeOwner;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions = default(NativeParallelHashMap<Entity, OwnerDefinition>);
			if (m_Mode == Mode.Replace)
			{
				CreateReplacement(ref ownerDefinitions);
			}
			else
			{
				int length = m_ControlPoints.Length;
				if (length == 1)
				{
					CreateSinglePoint(ref ownerDefinitions);
				}
				else
				{
					switch (m_Mode)
					{
					case Mode.Straight:
						CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
						break;
					case Mode.SimpleCurve:
						if (length == 2)
						{
							CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
						}
						else
						{
							CreateSimpleCurve(ref ownerDefinitions, 1);
						}
						break;
					case Mode.ComplexCurve:
						switch (length)
						{
						case 2:
							CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
							break;
						case 3:
							CreateSimpleCurve(ref ownerDefinitions, 1);
							break;
						default:
							CreateComplexCurve(ref ownerDefinitions);
							break;
						}
						break;
					case Mode.Grid:
						if (length == 2)
						{
							CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
						}
						else
						{
							CreateGrid(ref ownerDefinitions);
						}
						break;
					case Mode.Continuous:
						if (length == 2)
						{
							CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
						}
						else
						{
							CreateContinuousCurve(ref ownerDefinitions);
						}
						break;
					}
				}
			}
			if (ownerDefinitions.IsCreated)
			{
				ownerDefinitions.Dispose();
			}
		}

		private bool GetLocalCurve(NetCourse course, OwnerDefinition ownerDefinition, out LocalCurveCache localCurveCache)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			Transform inverseParentTransform = ObjectUtils.InverseTransform(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation));
			localCurveCache = default(LocalCurveCache);
			localCurveCache.m_Curve.a = ObjectUtils.WorldToLocal(inverseParentTransform, course.m_Curve.a);
			localCurveCache.m_Curve.b = ObjectUtils.WorldToLocal(inverseParentTransform, course.m_Curve.b);
			localCurveCache.m_Curve.c = ObjectUtils.WorldToLocal(inverseParentTransform, course.m_Curve.c);
			localCurveCache.m_Curve.d = ObjectUtils.WorldToLocal(inverseParentTransform, course.m_Curve.d);
			return true;
		}

		private bool GetOwnerDefinition(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, Entity original, bool checkControlPoints, CoursePos startPos, CoursePos endPos, out OwnerDefinition ownerDefinition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			Entity val = Entity.Null;
			ownerDefinition = default(OwnerDefinition);
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(original, ref owner))
			{
				val = owner.m_Owner;
			}
			else if (m_EditorMode)
			{
				if (checkControlPoints)
				{
					DynamicBuffer<InstalledUpgrade> val3 = default(DynamicBuffer<InstalledUpgrade>);
					for (int i = 0; i < m_ControlPoints.Length; i++)
					{
						Entity val2 = m_ControlPoints[i].m_OriginalEntity;
						if (m_NodeData.HasComponent(val2))
						{
							val2 = Entity.Null;
						}
						while (m_OwnerData.HasComponent(val2) && !m_BuildingData.HasComponent(val2))
						{
							val2 = m_OwnerData[val2].m_Owner;
							if (m_TempData.HasComponent(val2))
							{
								Temp temp = m_TempData[val2];
								if (temp.m_Original != Entity.Null)
								{
									val2 = temp.m_Original;
								}
							}
						}
						if (m_InstalledUpgrades.TryGetBuffer(val2, ref val3) && val3.Length != 0)
						{
							val2 = val3[0].m_Upgrade;
						}
						if (m_TransformData.HasComponent(val2) && m_SubNets.HasBuffer(val2))
						{
							val = val2;
							break;
						}
					}
				}
			}
			else
			{
				val = m_ServiceUpgradeOwner;
			}
			OwnerDefinition ownerDefinition2 = default(OwnerDefinition);
			Transform transform = default(Transform);
			Curve curve = default(Curve);
			if (ownerDefinitions.IsCreated && ownerDefinitions.TryGetValue(val, ref ownerDefinition2))
			{
				ownerDefinition = ownerDefinition2;
			}
			else if (m_TransformData.TryGetComponent(val, ref transform))
			{
				Entity owner2 = Entity.Null;
				if (m_OwnerData.TryGetComponent(val, ref owner))
				{
					owner2 = owner.m_Owner;
				}
				UpdateOwnerObject(owner2, val, Entity.Null, transform);
				ownerDefinition.m_Prefab = m_PrefabRefData[val].m_Prefab;
				ownerDefinition.m_Position = transform.m_Position;
				ownerDefinition.m_Rotation = transform.m_Rotation;
				Attachment attachment = default(Attachment);
				Transform transform2 = default(Transform);
				if (m_AttachmentData.TryGetComponent(val, ref attachment) && m_TransformData.TryGetComponent(attachment.m_Attached, ref transform2))
				{
					UpdateOwnerObject(Entity.Null, attachment.m_Attached, val, transform2);
				}
				if (!ownerDefinitions.IsCreated)
				{
					ownerDefinitions = new NativeParallelHashMap<Entity, OwnerDefinition>(8, AllocatorHandle.op_Implicit((Allocator)2));
				}
				ownerDefinitions.Add(val, ownerDefinition);
			}
			else if (m_CurveData.TryGetComponent(val, ref curve))
			{
				ownerDefinition.m_Prefab = m_PrefabRefData[val].m_Prefab;
				ownerDefinition.m_Position = curve.m_Bezier.a;
				ownerDefinition.m_Rotation = quaternion.op_Implicit(new float4(curve.m_Bezier.d, 0f));
			}
			if ((startPos.m_Flags & endPos.m_Flags & (CoursePosFlags.IsFirst | CoursePosFlags.IsLast)) != (CoursePosFlags.IsFirst | CoursePosFlags.IsLast) && m_PrefabSubObjects.HasBuffer(m_NetPrefab))
			{
				DynamicBuffer<Game.Prefabs.SubObject> val4 = m_PrefabSubObjects[m_NetPrefab];
				NativeParallelHashMap<Entity, int> selectedSpawnables = default(NativeParallelHashMap<Entity, int>);
				for (int j = 0; j < val4.Length; j++)
				{
					Game.Prefabs.SubObject subObject = val4[j];
					if ((subObject.m_Flags & SubObjectFlags.MakeOwner) != 0)
					{
						Transform courseObjectTransform = GetCourseObjectTransform(subObject, startPos, endPos);
						CreateCourseObject(subObject.m_Prefab, courseObjectTransform, ownerDefinition, ref selectedSpawnables);
						ownerDefinition.m_Prefab = subObject.m_Prefab;
						ownerDefinition.m_Position = courseObjectTransform.m_Position;
						ownerDefinition.m_Rotation = courseObjectTransform.m_Rotation;
						break;
					}
				}
				for (int k = 0; k < val4.Length; k++)
				{
					Game.Prefabs.SubObject subObject2 = val4[k];
					if ((subObject2.m_Flags & (SubObjectFlags.CoursePlacement | SubObjectFlags.MakeOwner)) == SubObjectFlags.CoursePlacement)
					{
						Transform courseObjectTransform2 = GetCourseObjectTransform(subObject2, startPos, endPos);
						CreateCourseObject(subObject2.m_Prefab, courseObjectTransform2, ownerDefinition, ref selectedSpawnables);
					}
				}
				if (selectedSpawnables.IsCreated)
				{
					selectedSpawnables.Dispose();
				}
			}
			return ownerDefinition.m_Prefab != Entity.Null;
		}

		private void UpdateOwnerObject(Entity owner, Entity original, Entity attachedParent, Transform transform)
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
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			Entity prefab = m_PrefabRefData[original].m_Prefab;
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Owner = owner,
				m_Original = original
			};
			creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
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
			PrefabRef prefabRef = default(PrefabRef);
			if (m_PrefabRefData.TryGetComponent(attachedParent, ref prefabRef))
			{
				creationDefinition.m_Attached = prefabRef.m_Prefab;
				creationDefinition.m_Flags |= CreationFlags.Attach;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			UpdateSubNets(transform, prefab, original);
			UpdateSubAreas(transform, prefab, original);
		}

		private Transform GetCourseObjectTransform(Game.Prefabs.SubObject subObject, CoursePos startPos, CoursePos endPos)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			CoursePos coursePos = (((subObject.m_Flags & SubObjectFlags.StartPlacement) != 0) ? startPos : endPos);
			Transform result = default(Transform);
			result.m_Position = ObjectUtils.LocalToWorld(coursePos.m_Position, coursePos.m_Rotation, subObject.m_Position);
			result.m_Rotation = math.mul(coursePos.m_Rotation, subObject.m_Rotation);
			return result;
		}

		private void CreateCourseObject(Entity prefab, Transform transform, OwnerDefinition ownerDefinition, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = prefab
			};
			ObjectDefinition objectDefinition = new ObjectDefinition
			{
				m_ParentMesh = -1,
				m_Position = transform.m_Position,
				m_Rotation = transform.m_Rotation
			};
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				Transform transform2 = ObjectUtils.WorldToLocal(ObjectUtils.InverseTransform(new Transform(ownerDefinition.m_Position, ownerDefinition.m_Rotation)), transform);
				objectDefinition.m_LocalPosition = transform2.m_Position;
				objectDefinition.m_LocalRotation = transform2.m_Rotation;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
			}
			else
			{
				objectDefinition.m_LocalPosition = transform.m_Position;
				objectDefinition.m_LocalRotation = transform.m_Rotation;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<ObjectDefinition>(val, objectDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			CreateSubNets(transform, prefab);
			CreateSubAreas(transform, prefab, ref selectedSpawnables);
		}

		private void CreateSubAreas(Transform transform, Entity prefab, ref NativeParallelHashMap<Entity, int> selectedSpawnables)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabSubAreas.HasBuffer(prefab))
			{
				return;
			}
			DynamicBuffer<Game.Prefabs.SubArea> val = m_PrefabSubAreas[prefab];
			DynamicBuffer<SubAreaNode> nodes = m_PrefabSubAreaNodes[prefab];
			Random random = m_RandomSeed.GetRandom(10000);
			for (int i = 0; i < val.Length; i++)
			{
				Game.Prefabs.SubArea subArea = val[i];
				int seed;
				if (!m_EditorMode && m_PrefabPlaceholderElements.HasBuffer(subArea.m_Prefab))
				{
					DynamicBuffer<PlaceholderObjectElement> placeholderElements = m_PrefabPlaceholderElements[subArea.m_Prefab];
					if (!selectedSpawnables.IsCreated)
					{
						selectedSpawnables = new NativeParallelHashMap<Entity, int>(10, AllocatorHandle.op_Implicit((Allocator)2));
					}
					if (!AreaUtils.SelectAreaPrefab(placeholderElements, m_PrefabSpawnableObjectData, selectedSpawnables, ref random, out subArea.m_Prefab, out seed))
					{
						continue;
					}
				}
				else
				{
					seed = ((Random)(ref random)).NextInt();
				}
				AreaGeometryData areaGeometryData = m_PrefabAreaGeometryData[subArea.m_Prefab];
				Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = subArea.m_Prefab,
					m_RandomSeed = seed
				};
				if (areaGeometryData.m_Type != Game.Areas.AreaType.Lot)
				{
					creationDefinition.m_Flags |= CreationFlags.Hidden;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
				OwnerDefinition ownerDefinition = new OwnerDefinition
				{
					m_Prefab = prefab,
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
				DynamicBuffer<Game.Areas.Node> val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val2);
				val3.ResizeUninitialized(subArea.m_NodeRange.y - subArea.m_NodeRange.x + 1);
				DynamicBuffer<LocalNodeCache> val4 = default(DynamicBuffer<LocalNodeCache>);
				if (m_EditorMode)
				{
					val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val2);
					val4.ResizeUninitialized(val3.Length);
				}
				int num = ObjectToolBaseSystem.GetFirstNodeIndex(nodes, subArea.m_NodeRange);
				int num2 = 0;
				for (int j = subArea.m_NodeRange.x; j <= subArea.m_NodeRange.y; j++)
				{
					float3 position = nodes[num].m_Position;
					float3 position2 = ObjectUtils.LocalToWorld(transform, position);
					int parentMesh = nodes[num].m_ParentMesh;
					float elevation = math.select(float.MinValue, position.y, parentMesh >= 0);
					val3[num2] = new Game.Areas.Node(position2, elevation);
					if (m_EditorMode)
					{
						val4[num2] = new LocalNodeCache
						{
							m_Position = position,
							m_ParentMesh = parentMesh
						};
					}
					num2++;
					if (++num == subArea.m_NodeRange.y)
					{
						num = subArea.m_NodeRange.x;
					}
				}
			}
		}

		private void CreateSubNets(Transform transform, Entity prefab)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			if (!m_PrefabSubNets.HasBuffer(prefab))
			{
				return;
			}
			DynamicBuffer<Game.Prefabs.SubNet> subNets = m_PrefabSubNets[prefab];
			NativeList<float4> val = default(NativeList<float4>);
			val._002Ector(subNets.Length * 2, AllocatorHandle.op_Implicit((Allocator)2));
			float4 val2;
			for (int i = 0; i < subNets.Length; i++)
			{
				Game.Prefabs.SubNet subNet = subNets[i];
				if (subNet.m_NodeIndex.x >= 0)
				{
					while (val.Length <= subNet.m_NodeIndex.x)
					{
						val2 = default(float4);
						val.Add(ref val2);
					}
					ref NativeList<float4> reference = ref val;
					int x = subNet.m_NodeIndex.x;
					reference[x] += new float4(subNet.m_Curve.a, 1f);
				}
				if (subNet.m_NodeIndex.y >= 0)
				{
					while (val.Length <= subNet.m_NodeIndex.y)
					{
						val2 = default(float4);
						val.Add(ref val2);
					}
					ref NativeList<float4> reference = ref val;
					int x = subNet.m_NodeIndex.y;
					reference[x] += new float4(subNet.m_Curve.d, 1f);
				}
			}
			for (int j = 0; j < val.Length; j++)
			{
				ref NativeList<float4> reference = ref val;
				int x = j;
				reference[x] /= math.max(1f, val[j].w);
			}
			for (int k = 0; k < subNets.Length; k++)
			{
				Game.Prefabs.SubNet subNet2 = NetUtils.GetSubNet(subNets, k, m_LefthandTraffic, ref m_NetGeometryData);
				Entity val3 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = subNet2.m_Prefab
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val3, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val3, default(Updated));
				OwnerDefinition ownerDefinition = new OwnerDefinition
				{
					m_Prefab = prefab,
					m_Position = transform.m_Position,
					m_Rotation = transform.m_Rotation
				};
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val3, ownerDefinition);
				NetCourse netCourse = default(NetCourse);
				netCourse.m_Curve = TransformCurve(subNet2.m_Curve, transform.m_Position, transform.m_Rotation);
				netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
				netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve), transform.m_Rotation);
				netCourse.m_StartPosition.m_CourseDelta = 0f;
				netCourse.m_StartPosition.m_Elevation = float2.op_Implicit(subNet2.m_Curve.a.y);
				netCourse.m_StartPosition.m_ParentMesh = subNet2.m_ParentMesh.x;
				if (subNet2.m_NodeIndex.x >= 0)
				{
					ref CoursePos startPosition = ref netCourse.m_StartPosition;
					Transform transform2 = transform;
					val2 = val[subNet2.m_NodeIndex.x];
					startPosition.m_Position = ObjectUtils.LocalToWorld(transform2, ((float4)(ref val2)).xyz);
				}
				netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
				netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve), transform.m_Rotation);
				netCourse.m_EndPosition.m_CourseDelta = 1f;
				netCourse.m_EndPosition.m_Elevation = float2.op_Implicit(subNet2.m_Curve.d.y);
				netCourse.m_EndPosition.m_ParentMesh = subNet2.m_ParentMesh.y;
				if (subNet2.m_NodeIndex.y >= 0)
				{
					ref CoursePos endPosition = ref netCourse.m_EndPosition;
					Transform transform3 = transform;
					val2 = val[subNet2.m_NodeIndex.y];
					endPosition.m_Position = ObjectUtils.LocalToWorld(transform3, ((float4)(ref val2)).xyz);
				}
				netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
				netCourse.m_FixedIndex = -1;
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
				if (((float3)(ref netCourse.m_StartPosition.m_Position)).Equals(netCourse.m_EndPosition.m_Position))
				{
					netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
					netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val3, netCourse);
				if (subNet2.m_Upgrades != default(CompositionFlags))
				{
					Upgraded upgraded = new Upgraded
					{
						m_Flags = subNet2.m_Upgrades
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Upgraded>(val3, upgraded);
				}
				if (m_EditorMode)
				{
					LocalCurveCache localCurveCache = new LocalCurveCache
					{
						m_Curve = subNet2.m_Curve
					};
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val3, localCurveCache);
				}
			}
			val.Dispose();
		}

		private Bezier4x3 TransformCurve(Bezier4x3 curve, float3 position, quaternion rotation)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			curve.a = ObjectUtils.LocalToWorld(position, rotation, curve.a);
			curve.b = ObjectUtils.LocalToWorld(position, rotation, curve.b);
			curve.c = ObjectUtils.LocalToWorld(position, rotation, curve.c);
			curve.d = ObjectUtils.LocalToWorld(position, rotation, curve.d);
			return curve;
		}

		private void UpdateSubNets(Transform transform, Entity prefab, Entity original)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<Entity> val = default(NativeParallelHashSet<Entity>);
			if (m_Mode == Mode.Replace && m_UpgradeStates.Length != 0)
			{
				val._002Ector(m_UpgradeStates.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < m_UpgradeStates.Length; i++)
				{
					ControlPoint controlPoint = m_ControlPoints[i * 2 + 1];
					ControlPoint controlPoint2 = m_ControlPoints[i * 2 + 2];
					DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[controlPoint.m_OriginalEntity];
					for (int j = 0; j < val2.Length; j++)
					{
						Entity edge = val2[j].m_Edge;
						Edge edge2 = m_EdgeData[edge];
						if (edge2.m_Start == controlPoint.m_OriginalEntity && edge2.m_End == controlPoint2.m_OriginalEntity)
						{
							val.Add(edge);
						}
						else if (edge2.m_End == controlPoint.m_OriginalEntity && edge2.m_Start == controlPoint2.m_OriginalEntity)
						{
							val.Add(edge);
						}
					}
				}
			}
			if (m_SubNets.HasBuffer(original))
			{
				DynamicBuffer<Game.Net.SubNet> val3 = m_SubNets[original];
				for (int k = 0; k < val3.Length; k++)
				{
					Entity subNet = val3[k].m_SubNet;
					if (m_NodeData.HasComponent(subNet))
					{
						if (!HasEdgeStartOrEnd(subNet, original))
						{
							Game.Net.Node node = m_NodeData[subNet];
							Entity val4 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
							CreationDefinition creationDefinition = new CreationDefinition
							{
								m_Original = subNet
							};
							if (m_EditorContainerData.HasComponent(subNet))
							{
								creationDefinition.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
							}
							OwnerDefinition ownerDefinition = new OwnerDefinition
							{
								m_Prefab = prefab,
								m_Position = transform.m_Position,
								m_Rotation = transform.m_Rotation
							};
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val4, ownerDefinition);
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val4, creationDefinition);
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val4, default(Updated));
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
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val4, netCourse);
						}
					}
					else if (m_EdgeData.HasComponent(subNet) && (!val.IsCreated || !val.Contains(subNet)))
					{
						Edge edge3 = m_EdgeData[subNet];
						Entity val5 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
						CreationDefinition creationDefinition2 = new CreationDefinition
						{
							m_Original = subNet
						};
						if (m_EditorContainerData.HasComponent(subNet))
						{
							creationDefinition2.m_SubPrefab = m_EditorContainerData[subNet].m_Prefab;
						}
						OwnerDefinition ownerDefinition2 = new OwnerDefinition
						{
							m_Prefab = prefab,
							m_Position = transform.m_Position,
							m_Rotation = transform.m_Rotation
						};
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val5, ownerDefinition2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val5, creationDefinition2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val5, default(Updated));
						NetCourse netCourse2 = default(NetCourse);
						netCourse2.m_Curve = m_CurveData[subNet].m_Bezier;
						netCourse2.m_Length = MathUtils.Length(netCourse2.m_Curve);
						netCourse2.m_FixedIndex = -1;
						netCourse2.m_StartPosition.m_Entity = edge3.m_Start;
						netCourse2.m_StartPosition.m_Position = netCourse2.m_Curve.a;
						netCourse2.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse2.m_Curve));
						netCourse2.m_StartPosition.m_CourseDelta = 0f;
						netCourse2.m_EndPosition.m_Entity = edge3.m_End;
						netCourse2.m_EndPosition.m_Position = netCourse2.m_Curve.d;
						netCourse2.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse2.m_Curve));
						netCourse2.m_EndPosition.m_CourseDelta = 1f;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val5, netCourse2);
					}
				}
			}
			if (val.IsCreated)
			{
				val.Dispose();
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

		private void UpdateSubAreas(Transform transform, Entity prefab, Entity original)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			if (!m_SubAreas.HasBuffer(original))
			{
				return;
			}
			DynamicBuffer<Game.Areas.SubArea> val = m_SubAreas[original];
			for (int i = 0; i < val.Length; i++)
			{
				Entity area = val[i].m_Area;
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
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
				DynamicBuffer<Game.Areas.Node> val3 = m_AreaNodes[area];
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<Game.Areas.Node>(val2).CopyFrom(val3.AsNativeArray());
				if (m_CachedNodes.HasBuffer(area))
				{
					DynamicBuffer<LocalNodeCache> val4 = m_CachedNodes[area];
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<LocalNodeCache>(val2).CopyFrom(val4.AsNativeArray());
				}
			}
		}

		private void CreateReplacement(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_UpgradeStates.Length; i++)
			{
				ControlPoint controlPoint = m_ControlPoints[i * 2 + 1];
				ControlPoint endPoint = m_ControlPoints[i * 2 + 2];
				UpgradeState upgradeState = m_UpgradeStates[i];
				if (controlPoint.m_OriginalEntity == Entity.Null || endPoint.m_OriginalEntity == Entity.Null)
				{
					continue;
				}
				if (controlPoint.m_OriginalEntity == endPoint.m_OriginalEntity)
				{
					if (upgradeState.m_IsUpgrading || m_RemoveUpgrade)
					{
						CreateUpgrade(ref ownerDefinitions, upgradeState, controlPoint, i == 0, i == m_UpgradeStates.Length - 1);
					}
					else
					{
						CreateReplacement(ref ownerDefinitions, controlPoint, i == 0, i == m_UpgradeStates.Length - 1);
					}
					continue;
				}
				DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[controlPoint.m_OriginalEntity];
				for (int j = 0; j < val.Length; j++)
				{
					Entity edge = val[j].m_Edge;
					Edge edge2 = m_EdgeData[edge];
					if (edge2.m_Start == controlPoint.m_OriginalEntity && edge2.m_End == endPoint.m_OriginalEntity)
					{
						if (upgradeState.m_IsUpgrading || m_RemoveUpgrade)
						{
							CreateUpgrade(ref ownerDefinitions, edge, upgradeState, invert: false, i == 0, i == m_UpgradeStates.Length - 1);
						}
						else
						{
							CreateReplacement(ref ownerDefinitions, controlPoint, endPoint, edge, invert: false, i == 0, i == m_UpgradeStates.Length - 1);
						}
					}
					else if (edge2.m_End == controlPoint.m_OriginalEntity && edge2.m_Start == endPoint.m_OriginalEntity)
					{
						if (upgradeState.m_IsUpgrading || m_RemoveUpgrade)
						{
							CreateUpgrade(ref ownerDefinitions, edge, upgradeState, invert: true, i == 0, i == m_UpgradeStates.Length - 1);
						}
						else
						{
							CreateReplacement(ref ownerDefinitions, controlPoint, endPoint, edge, invert: true, i == 0, i == m_UpgradeStates.Length - 1);
						}
					}
				}
			}
		}

		private void CreateReplacement(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, ControlPoint point, bool isStart, bool isEnd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = point.m_OriginalEntity,
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab
			};
			creationDefinition.m_Flags |= CreationFlags.SubElevation;
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = new Bezier4x3(point.m_Position, point.m_Position, point.m_Position, point.m_Position);
			netCourse.m_StartPosition = GetCoursePos(netCourse.m_Curve, point, 0f);
			netCourse.m_EndPosition = GetCoursePos(netCourse.m_Curve, point, 1f);
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			if (isStart)
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			if (isEnd)
			{
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			}
			if (GetOwnerDefinition(ref ownerDefinitions, point.m_OriginalEntity, checkControlPoints: false, netCourse.m_StartPosition, netCourse.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val, localCurveCache);
				}
			}
			else
			{
				netCourse.m_StartPosition.m_ParentMesh = -1;
				netCourse.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
		}

		private void CreateReplacement(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, ControlPoint startPoint, ControlPoint endPoint, Entity edge, bool invert, bool isStart, bool isEnd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = edge,
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab
			};
			creationDefinition.m_Flags |= CreationFlags.Align | CreationFlags.SubElevation;
			Curve curve = m_CurveData[edge];
			if (invert)
			{
				curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
				creationDefinition.m_Flags |= CreationFlags.Invert;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			NetCourse netCourse = default(NetCourse);
			if (((float3)(ref startPoint.m_Position)).Equals(curve.m_Bezier.a) && ((float3)(ref endPoint.m_Position)).Equals(curve.m_Bezier.d))
			{
				netCourse.m_Curve = curve.m_Bezier;
				netCourse.m_Length = curve.m_Length;
				netCourse.m_FixedIndex = -1;
			}
			else
			{
				float3 val2 = MathUtils.StartTangent(curve.m_Bezier);
				float3 val3 = MathUtils.EndTangent(curve.m_Bezier);
				val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
				val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz);
				netCourse.m_Curve = NetUtils.FitCurve(startPoint.m_Position, val2, val3, endPoint.m_Position);
				netCourse.m_Curve.b.y = curve.m_Bezier.b.y;
				netCourse.m_Curve.c.y = curve.m_Bezier.c.y;
				netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
				netCourse.m_FixedIndex = -1;
			}
			Fixed obj = default(Fixed);
			if (m_FixedData.TryGetComponent(edge, ref obj))
			{
				netCourse.m_FixedIndex = obj.m_Index;
			}
			netCourse.m_StartPosition.m_Entity = startPoint.m_OriginalEntity;
			netCourse.m_StartPosition.m_Position = startPoint.m_Position;
			netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve));
			netCourse.m_StartPosition.m_CourseDelta = 0f;
			netCourse.m_EndPosition.m_Entity = endPoint.m_OriginalEntity;
			netCourse.m_EndPosition.m_Position = endPoint.m_Position;
			netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve));
			netCourse.m_EndPosition.m_CourseDelta = 1f;
			if (isStart)
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			if (isEnd)
			{
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			}
			if (GetOwnerDefinition(ref ownerDefinitions, edge, checkControlPoints: false, netCourse.m_StartPosition, netCourse.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val, localCurveCache);
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
		}

		private void CreateUpgrade(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, UpgradeState upgradeState, ControlPoint point, bool isStart, bool isEnd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = point.m_OriginalEntity,
				m_Prefab = m_PrefabRefData[point.m_OriginalEntity].m_Prefab
			};
			creationDefinition.m_Flags |= CreationFlags.Align | CreationFlags.SubElevation;
			if (!upgradeState.m_SkipFlags)
			{
				Upgraded upgraded = default(Upgraded);
				m_UpgradedData.TryGetComponent(point.m_OriginalEntity, ref upgraded);
				upgraded.m_Flags = (upgraded.m_Flags & ~upgradeState.m_RemoveFlags) | (upgradeState.m_AddFlags & ~upgradeState.m_OldFlags);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Upgraded>(val, upgraded);
				if (((upgradeState.m_OldFlags & ~upgradeState.m_RemoveFlags) | upgradeState.m_AddFlags) != upgradeState.m_OldFlags)
				{
					creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = new Bezier4x3(point.m_Position, point.m_Position, point.m_Position, point.m_Position);
			netCourse.m_StartPosition = GetCoursePos(netCourse.m_Curve, point, 0f);
			netCourse.m_EndPosition = GetCoursePos(netCourse.m_Curve, point, 1f);
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			if (isStart)
			{
				netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			if (isEnd)
			{
				netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			}
			if (GetOwnerDefinition(ref ownerDefinitions, point.m_OriginalEntity, checkControlPoints: false, netCourse.m_StartPosition, netCourse.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val, localCurveCache);
				}
			}
			else
			{
				netCourse.m_StartPosition.m_ParentMesh = -1;
				netCourse.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
		}

		private void CreateUpgrade(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, Entity edge, UpgradeState upgradeState, bool invert, bool isStart, bool isEnd)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Original = edge,
				m_Prefab = m_PrefabRefData[edge].m_Prefab
			};
			creationDefinition.m_Flags |= CreationFlags.Align | CreationFlags.SubElevation;
			if (!upgradeState.m_SkipFlags)
			{
				Upgraded upgraded = default(Upgraded);
				m_UpgradedData.TryGetComponent(edge, ref upgraded);
				upgraded.m_Flags = (upgraded.m_Flags & ~upgradeState.m_RemoveFlags) | (upgradeState.m_AddFlags & ~upgradeState.m_OldFlags);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Upgraded>(val, upgraded);
				DynamicBuffer<SubReplacement> val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<SubReplacement>(val);
				DynamicBuffer<SubReplacement> val3 = default(DynamicBuffer<SubReplacement>);
				if (m_SubReplacements.TryGetBuffer(edge, ref val3))
				{
					for (int i = 0; i < val3.Length; i++)
					{
						SubReplacement subReplacement = val3[i];
						if (subReplacement.m_Side != upgradeState.m_SubReplacementSide || subReplacement.m_Type != upgradeState.m_SubReplacementType)
						{
							val2.Add(subReplacement);
						}
					}
				}
				if (upgradeState.m_SubReplacementType != SubReplacementType.None && upgradeState.m_SubReplacementPrefab != Entity.Null)
				{
					val2.Add(new SubReplacement
					{
						m_Prefab = upgradeState.m_SubReplacementPrefab,
						m_Type = upgradeState.m_SubReplacementType,
						m_Side = upgradeState.m_SubReplacementSide,
						m_AgeMask = m_AgeMask
					});
				}
				if (((upgradeState.m_OldFlags & ~upgradeState.m_RemoveFlags) | upgradeState.m_AddFlags) != upgradeState.m_OldFlags || upgradeState.m_SubReplacementType != SubReplacementType.None)
				{
					creationDefinition.m_Flags |= CreationFlags.Upgrade | CreationFlags.Parent;
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			Edge edge2 = m_EdgeData[edge];
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = m_CurveData[edge].m_Bezier;
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			Fixed obj = default(Fixed);
			if (m_FixedData.TryGetComponent(edge, ref obj))
			{
				netCourse.m_FixedIndex = obj.m_Index;
			}
			netCourse.m_StartPosition.m_Entity = edge2.m_Start;
			netCourse.m_StartPosition.m_Position = netCourse.m_Curve.a;
			netCourse.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.StartTangent(netCourse.m_Curve));
			netCourse.m_StartPosition.m_CourseDelta = 0f;
			netCourse.m_EndPosition.m_Entity = edge2.m_End;
			netCourse.m_EndPosition.m_Position = netCourse.m_Curve.d;
			netCourse.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(MathUtils.EndTangent(netCourse.m_Curve));
			netCourse.m_EndPosition.m_CourseDelta = 1f;
			if (invert)
			{
				if (isStart)
				{
					netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
				}
				if (isEnd)
				{
					netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				}
			}
			else
			{
				if (isStart)
				{
					netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
				}
				if (isEnd)
				{
					netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
				}
			}
			if (GetOwnerDefinition(ref ownerDefinitions, edge, checkControlPoints: false, netCourse.m_StartPosition, netCourse.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val, localCurveCache);
				}
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
		}

		private void CreateSinglePoint(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[0];
			Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			Random random = m_RandomSeed.GetRandom(0);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.SubElevation;
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val, default(Updated));
			NetCourse netCourse = default(NetCourse);
			netCourse.m_Curve = new Bezier4x3(controlPoint.m_Position, controlPoint.m_Position, controlPoint.m_Position, controlPoint.m_Position);
			netCourse.m_StartPosition = GetCoursePos(netCourse.m_Curve, controlPoint, 0f);
			netCourse.m_EndPosition = GetCoursePos(netCourse.m_Curve, controlPoint, 1f);
			netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst | CoursePosFlags.IsLast | CoursePosFlags.IsRight | CoursePosFlags.IsLeft;
			netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst | CoursePosFlags.IsLast | CoursePosFlags.IsRight | CoursePosFlags.IsLeft;
			netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
			netCourse.m_FixedIndex = -1;
			if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, netCourse.m_StartPosition, netCourse.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val, localCurveCache);
				}
			}
			else
			{
				netCourse.m_StartPosition.m_ParentMesh = -1;
				netCourse.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val, netCourse);
		}

		private void CreateStraightLine(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, int2 index)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[index.x];
			ControlPoint controlPoint2 = m_ControlPoints[index.y];
			FixElevation(ref controlPoint);
			if (m_NetGeometryData.HasComponent(m_NetPrefab) && m_NetGeometryData[m_NetPrefab].m_MaxSlopeSteepness == 0f)
			{
				SetHeight(controlPoint, ref controlPoint2);
			}
			Random random = m_RandomSeed.GetRandom(0);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.SubElevation;
			NetCourse course = default(NetCourse);
			course.m_Curve = NetUtils.StraightCurve(controlPoint.m_Position, controlPoint2.m_Position);
			course.m_StartPosition = GetCoursePos(course.m_Curve, controlPoint, 0f);
			course.m_EndPosition = GetCoursePos(course.m_Curve, controlPoint2, 1f);
			course.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			course.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			if (((float3)(ref course.m_StartPosition.m_Position)).Equals(course.m_EndPosition.m_Position) && ((Entity)(ref course.m_StartPosition.m_Entity)).Equals(course.m_EndPosition.m_Entity))
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			bool2 val = m_ParallelCount > 0;
			if (!val.x)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
			if (!val.y)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			course.m_Length = MathUtils.Length(course.m_Curve);
			course.m_FixedIndex = -1;
			if (m_PlaceableData.HasComponent(m_NetPrefab))
			{
				PlaceableNetData placeableNetData = m_PlaceableData[m_NetPrefab];
				if (CalculatedInverseWeight(course, placeableNetData.m_PlacementFlags) < 0f)
				{
					InvertCourse(ref course);
				}
			}
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
			if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, course.m_StartPosition, course.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(course, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val2, localCurveCache);
				}
			}
			else
			{
				course.m_StartPosition.m_ParentMesh = -1;
				course.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, course);
			if (math.any(val))
			{
				NativeParallelHashMap<float4, float3> nodeMap = default(NativeParallelHashMap<float4, float3>);
				nodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				CreateParallelCourses(creationDefinition, ownerDefinition, course, nodeMap);
				nodeMap.Dispose();
			}
		}

		private void InvertCourse(ref NetCourse course)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			course.m_Curve = MathUtils.Invert(course.m_Curve);
			CommonUtils.Swap(ref course.m_StartPosition.m_Position, ref course.m_EndPosition.m_Position);
			CommonUtils.Swap(ref course.m_StartPosition.m_Rotation, ref course.m_EndPosition.m_Rotation);
			CommonUtils.Swap(ref course.m_StartPosition.m_Elevation, ref course.m_EndPosition.m_Elevation);
			CommonUtils.Swap(ref course.m_StartPosition.m_Flags, ref course.m_EndPosition.m_Flags);
			CommonUtils.Swap(ref course.m_StartPosition.m_ParentMesh, ref course.m_EndPosition.m_ParentMesh);
			quaternion val = quaternion.RotateY((float)Math.PI);
			course.m_StartPosition.m_Rotation = math.mul(val, course.m_StartPosition.m_Rotation);
			course.m_EndPosition.m_Rotation = math.mul(val, course.m_EndPosition.m_Rotation);
			if ((course.m_StartPosition.m_Flags & (CoursePosFlags.IsRight | CoursePosFlags.IsLeft)) == CoursePosFlags.IsLeft)
			{
				course.m_StartPosition.m_Flags &= ~CoursePosFlags.IsLeft;
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			else if ((course.m_StartPosition.m_Flags & (CoursePosFlags.IsRight | CoursePosFlags.IsLeft)) == CoursePosFlags.IsRight)
			{
				course.m_StartPosition.m_Flags &= ~CoursePosFlags.IsRight;
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
			if ((course.m_EndPosition.m_Flags & (CoursePosFlags.IsRight | CoursePosFlags.IsLeft)) == CoursePosFlags.IsLeft)
			{
				course.m_EndPosition.m_Flags &= ~CoursePosFlags.IsLeft;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			else if ((course.m_EndPosition.m_Flags & (CoursePosFlags.IsRight | CoursePosFlags.IsLeft)) == CoursePosFlags.IsRight)
			{
				course.m_EndPosition.m_Flags &= ~CoursePosFlags.IsRight;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
		}

		private float CalculatedInverseWeight(NetCourse course, Game.Net.PlacementFlags placementFlags)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if ((placementFlags & (Game.Net.PlacementFlags.FlowLeft | Game.Net.PlacementFlags.FlowRight)) != Game.Net.PlacementFlags.None)
			{
				int num2 = math.max(1, Mathf.RoundToInt(course.m_Length * m_WaterSurfaceData.scale.x));
				for (int i = 0; i < num2; i++)
				{
					float num3 = ((float)i + 0.5f) / (float)num2;
					float3 worldPosition = MathUtils.Position(course.m_Curve, num3);
					float3 val = MathUtils.Tangent(course.m_Curve, num3);
					float2 val2 = WaterUtils.SampleVelocity(ref m_WaterSurfaceData, worldPosition);
					float2 val3 = math.normalizesafe(MathUtils.Right(((float3)(ref val)).xz), default(float2));
					float num4 = math.dot(val2, val3);
					num += math.select(num4, 0f - num4, (placementFlags & Game.Net.PlacementFlags.FlowLeft) != 0);
				}
			}
			return num;
		}

		private void FixElevation(ref ControlPoint controlPoint)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			if (m_PlaceableData.HasComponent(m_NetPrefab))
			{
				PlaceableNetData placeableNetData = m_PlaceableData[m_NetPrefab];
				if (controlPoint.m_Elevation < placeableNetData.m_ElevationRange.min)
				{
					controlPoint.m_Position.y += placeableNetData.m_ElevationRange.min - controlPoint.m_Elevation;
					controlPoint.m_Elevation = placeableNetData.m_ElevationRange.min;
					controlPoint.m_OriginalEntity = Entity.Null;
				}
				else if (controlPoint.m_Elevation > placeableNetData.m_ElevationRange.max)
				{
					controlPoint.m_Position.y += placeableNetData.m_ElevationRange.max - controlPoint.m_Elevation;
					controlPoint.m_Elevation = placeableNetData.m_ElevationRange.max;
					controlPoint.m_OriginalEntity = Entity.Null;
				}
			}
		}

		private void SetHeight(ControlPoint startPoint, ref ControlPoint controlPoint)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			float y = startPoint.m_Position.y;
			controlPoint.m_Position.y = y;
		}

		private void CreateParallelCourses(CreationDefinition definitionData, OwnerDefinition ownerDefinition, NetCourse courseData, NativeParallelHashMap<float4, float3> nodeMap)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			if (((float3)(ref courseData.m_StartPosition.m_Position)).Equals(courseData.m_EndPosition.m_Position))
			{
				return;
			}
			float num = m_ParallelOffset;
			float elevationLimit = 1f;
			if (m_NetGeometryData.HasComponent(m_NetPrefab))
			{
				NetGeometryData netGeometryData = m_NetGeometryData[m_NetPrefab];
				num += netGeometryData.m_DefaultWidth;
				elevationLimit = netGeometryData.m_ElevationLimit;
			}
			NetCourse netCourse = courseData;
			NetCourse netCourse2 = courseData;
			float4 val = default(float4);
			for (int i = 1; i <= m_ParallelCount.x; i++)
			{
				NetCourse netCourse3 = netCourse;
				netCourse3.m_Curve = NetUtils.OffsetCurveLeftSmooth(netCourse.m_Curve, float2.op_Implicit(num));
				((float4)(ref val))._002Ector(netCourse.m_Curve.a, (float)(-i));
				if (!nodeMap.TryAdd(val, netCourse3.m_Curve.a))
				{
					netCourse3.m_Curve.a = nodeMap[val];
				}
				((float4)(ref val))._002Ector(netCourse.m_Curve.d, (float)(-i));
				if (!nodeMap.TryAdd(val, netCourse3.m_Curve.d))
				{
					netCourse3.m_Curve.d = nodeMap[val];
				}
				Random random = m_RandomSeed.GetRandom(-i);
				CreateParallelCourse(definitionData, ownerDefinition, netCourse, netCourse3, num, elevationLimit, (i & 1) != 0, i == m_ParallelCount.x, isRight: false, 0, ref random);
				netCourse = netCourse3;
			}
			float4 val2 = default(float4);
			for (int j = 1; j <= m_ParallelCount.y; j++)
			{
				NetCourse netCourse4 = netCourse2;
				netCourse4.m_Curve = NetUtils.OffsetCurveLeftSmooth(netCourse2.m_Curve, float2.op_Implicit(0f - num));
				((float4)(ref val2))._002Ector(netCourse2.m_Curve.a, (float)j);
				if (!nodeMap.TryAdd(val2, netCourse4.m_Curve.a))
				{
					netCourse4.m_Curve.a = nodeMap[val2];
				}
				((float4)(ref val2))._002Ector(netCourse2.m_Curve.d, (float)j);
				if (!nodeMap.TryAdd(val2, netCourse4.m_Curve.d))
				{
					netCourse4.m_Curve.d = nodeMap[val2];
				}
				Random random2 = m_RandomSeed.GetRandom(j);
				CreateParallelCourse(definitionData, ownerDefinition, netCourse2, netCourse4, 0f - num, elevationLimit, (j & 1) != 0, isLeft: false, j == m_ParallelCount.y, 0, ref random2);
				netCourse2 = netCourse4;
			}
		}

		private void CreateParallelCourse(CreationDefinition definitionData, OwnerDefinition ownerDefinition, NetCourse courseData, NetCourse courseData2, float parallelOffset, float elevationLimit, bool invert, bool isLeft, bool isRight, int level, ref Random random)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			float num = math.abs(parallelOffset);
			if (++level >= 10 || math.distance(((float3)(ref courseData2.m_Curve.a)).xz, ((float3)(ref courseData2.m_Curve.d)).xz) < num * 2f)
			{
				CreateParallelCourse(definitionData, ownerDefinition, courseData2, elevationLimit, invert, isLeft, isRight, ref random);
				return;
			}
			float3 val = MathUtils.Position(courseData2.m_Curve, 0.5f);
			float num3 = default(float);
			float num2 = MathUtils.Distance(((Bezier4x3)(ref courseData.m_Curve)).xz, ((float3)(ref val)).xz, ref num3);
			float3 val2 = MathUtils.Position(courseData.m_Curve, num3);
			float3 val3 = MathUtils.Tangent(courseData.m_Curve, num3);
			val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz);
			float2 val4 = ((float3)(ref val3)).zx * new float2(0f - parallelOffset, parallelOffset);
			if (math.abs(num2 - num) > num * 0.02f || math.dot(val4, ((float3)(ref val)).xz - ((float3)(ref val2)).xz) < 0f)
			{
				((float3)(ref val2)).xz = ((float3)(ref val2)).xz + val4;
				float3 val5 = MathUtils.StartTangent(courseData2.m_Curve);
				float3 val6 = MathUtils.EndTangent(courseData2.m_Curve);
				val5 = MathUtils.Normalize(val5, ((float3)(ref val5)).xz);
				val6 = MathUtils.Normalize(val6, ((float3)(ref val6)).xz);
				NetCourse courseData3 = courseData2;
				NetCourse courseData4 = courseData2;
				courseData3.m_Curve = NetUtils.FitCurve(courseData2.m_Curve.a, val5, val3, val2);
				courseData3.m_EndPosition.m_Flags &= ~(CoursePosFlags.IsFirst | CoursePosFlags.IsLast);
				courseData3.m_EndPosition.m_Rotation = NetUtils.GetNodeRotation(val3);
				courseData4.m_Curve = NetUtils.FitCurve(val2, val3, val6, courseData2.m_Curve.d);
				courseData4.m_StartPosition.m_Flags &= ~(CoursePosFlags.IsFirst | CoursePosFlags.IsLast);
				courseData4.m_StartPosition.m_Rotation = NetUtils.GetNodeRotation(val3);
				float num4 = MathUtils.Length(courseData3.m_Curve);
				float num5 = MathUtils.Length(courseData4.m_Curve);
				num3 = math.saturate(num4 / (num4 + num5));
				courseData3.m_EndPosition.m_Elevation = math.lerp(courseData2.m_StartPosition.m_Elevation, courseData2.m_EndPosition.m_Elevation, num3);
				courseData3.m_EndPosition.m_ParentMesh = math.select(courseData2.m_StartPosition.m_ParentMesh, -1, courseData2.m_StartPosition.m_ParentMesh != courseData2.m_EndPosition.m_ParentMesh);
				courseData4.m_StartPosition.m_Elevation = math.lerp(courseData2.m_StartPosition.m_Elevation, courseData2.m_EndPosition.m_Elevation, num3);
				courseData4.m_StartPosition.m_ParentMesh = math.select(courseData2.m_StartPosition.m_ParentMesh, -1, courseData2.m_StartPosition.m_ParentMesh != courseData2.m_EndPosition.m_ParentMesh);
				CreateParallelCourse(definitionData, ownerDefinition, courseData, courseData3, parallelOffset, elevationLimit, invert, isLeft, isRight, level, ref random);
				CreateParallelCourse(definitionData, ownerDefinition, courseData, courseData4, parallelOffset, elevationLimit, invert, isLeft, isRight, level, ref random);
			}
			else
			{
				CreateParallelCourse(definitionData, ownerDefinition, courseData2, elevationLimit, invert, isLeft, isRight, ref random);
			}
		}

		private void CreateParallelCourse(CreationDefinition definitionData, OwnerDefinition ownerDefinition, NetCourse courseData, float elevationLimit, bool invert, bool isLeft, bool isRight, ref Random random)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			LinearizeElevation(ref courseData.m_Curve);
			courseData.m_StartPosition.m_Position = courseData.m_Curve.a;
			courseData.m_StartPosition.m_Entity = Entity.Null;
			courseData.m_StartPosition.m_SplitPosition = 0f;
			courseData.m_StartPosition.m_Flags &= ~(CoursePosFlags.IsRight | CoursePosFlags.IsLeft);
			courseData.m_StartPosition.m_Flags |= CoursePosFlags.IsParallel;
			courseData.m_EndPosition.m_Position = courseData.m_Curve.d;
			courseData.m_EndPosition.m_Entity = Entity.Null;
			courseData.m_EndPosition.m_SplitPosition = 0f;
			courseData.m_EndPosition.m_Flags &= ~(CoursePosFlags.IsRight | CoursePosFlags.IsLeft);
			courseData.m_EndPosition.m_Flags |= CoursePosFlags.IsParallel;
			courseData.m_Length = MathUtils.Length(courseData.m_Curve);
			courseData.m_FixedIndex = -1;
			if (courseData.m_StartPosition.m_Elevation.x > 0f - elevationLimit && courseData.m_StartPosition.m_Elevation.x < elevationLimit)
			{
				courseData.m_StartPosition.m_Flags |= CoursePosFlags.FreeHeight;
			}
			if (courseData.m_EndPosition.m_Elevation.x > 0f - elevationLimit && courseData.m_EndPosition.m_Elevation.x < elevationLimit)
			{
				courseData.m_EndPosition.m_Flags |= CoursePosFlags.FreeHeight;
			}
			if (invert)
			{
				courseData.m_Curve = MathUtils.Invert(courseData.m_Curve);
				CommonUtils.Swap(ref courseData.m_StartPosition.m_Position, ref courseData.m_EndPosition.m_Position);
				CommonUtils.Swap(ref courseData.m_StartPosition.m_Rotation, ref courseData.m_EndPosition.m_Rotation);
				CommonUtils.Swap(ref courseData.m_StartPosition.m_Elevation, ref courseData.m_EndPosition.m_Elevation);
				CommonUtils.Swap(ref courseData.m_StartPosition.m_Flags, ref courseData.m_EndPosition.m_Flags);
				CommonUtils.Swap(ref courseData.m_StartPosition.m_ParentMesh, ref courseData.m_EndPosition.m_ParentMesh);
				quaternion val = quaternion.RotateY((float)Math.PI);
				courseData.m_StartPosition.m_Rotation = math.mul(val, courseData.m_StartPosition.m_Rotation);
				courseData.m_EndPosition.m_Rotation = math.mul(val, courseData.m_EndPosition.m_Rotation);
			}
			if (isLeft || isRight)
			{
				if (invert == isLeft)
				{
					courseData.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
					courseData.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
				}
				else
				{
					courseData.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
					courseData.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
				}
			}
			definitionData.m_RandomSeed ^= ((Random)(ref random)).NextInt();
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, definitionData);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, courseData);
			if (ownerDefinition.m_Prefab != Entity.Null)
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
			}
		}

		private void CreateSimpleCurve(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions, int middleIndex)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[0];
			ControlPoint controlPoint2 = m_ControlPoints[middleIndex];
			ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 1];
			FixElevation(ref controlPoint);
			FixElevation(ref controlPoint2);
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(m_NetPrefab))
			{
				netGeometryData = m_NetGeometryData[m_NetPrefab];
				if (netGeometryData.m_MaxSlopeSteepness == 0f)
				{
					SetHeight(controlPoint, ref controlPoint2);
					SetHeight(controlPoint, ref controlPoint3);
				}
			}
			else
			{
				netGeometryData.m_DefaultWidth = 0.02f;
			}
			float num2 = default(float);
			float num = MathUtils.Distance(new Segment(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz), ((float3)(ref controlPoint3.m_Position)).xz, ref num2);
			float num4 = default(float);
			float num3 = MathUtils.Distance(new Segment(((float3)(ref controlPoint3.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz), ((float3)(ref controlPoint.m_Position)).xz, ref num4);
			if (num <= netGeometryData.m_DefaultWidth * 0.75f && num <= num3)
			{
				num2 *= 0.5f + num / netGeometryData.m_DefaultWidth * (2f / 3f);
				controlPoint2.m_Position = math.lerp(controlPoint.m_Position, controlPoint2.m_Position, num2);
			}
			else if (num3 <= netGeometryData.m_DefaultWidth * 0.75f)
			{
				num4 *= 0.5f + num3 / netGeometryData.m_DefaultWidth * (2f / 3f);
				controlPoint2.m_Position = math.lerp(controlPoint3.m_Position, controlPoint2.m_Position, num4);
			}
			Random random = m_RandomSeed.GetRandom(0);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.SubElevation;
			NetCourse course = new NetCourse
			{
				m_Curve = NetUtils.FitCurve(new Segment(controlPoint.m_Position, controlPoint2.m_Position), new Segment(controlPoint3.m_Position, controlPoint2.m_Position))
			};
			LinearizeElevation(ref course.m_Curve);
			course.m_StartPosition = GetCoursePos(course.m_Curve, controlPoint, 0f);
			course.m_EndPosition = GetCoursePos(course.m_Curve, controlPoint3, 1f);
			course.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			course.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			if (((float3)(ref course.m_StartPosition.m_Position)).Equals(course.m_EndPosition.m_Position) && ((Entity)(ref course.m_StartPosition.m_Entity)).Equals(course.m_EndPosition.m_Entity))
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			bool2 val = m_ParallelCount > 0;
			if (!val.x)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
			if (!val.y)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			course.m_Length = MathUtils.Length(course.m_Curve);
			course.m_FixedIndex = -1;
			if (m_PlaceableData.HasComponent(m_NetPrefab))
			{
				PlaceableNetData placeableNetData = m_PlaceableData[m_NetPrefab];
				if (CalculatedInverseWeight(course, placeableNetData.m_PlacementFlags) < 0f)
				{
					InvertCourse(ref course);
				}
			}
			Entity val2 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val2, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val2, default(Updated));
			if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, course.m_StartPosition, course.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val2, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(course, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val2, localCurveCache);
				}
			}
			else
			{
				course.m_StartPosition.m_ParentMesh = -1;
				course.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val2, course);
			if (math.any(val))
			{
				NativeParallelHashMap<float4, float3> nodeMap = default(NativeParallelHashMap<float4, float3>);
				nodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				CreateParallelCourses(creationDefinition, ownerDefinition, course, nodeMap);
				nodeMap.Dispose();
			}
		}

		private float GetCutPosition(NetGeometryData netGeometryData, float length, float t)
		{
			if ((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
			{
				return math.saturate(MathUtils.Snap(length * t + 0.16f, 8f) / length);
			}
			return t;
		}

		private void CreateGrid(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0950: Unknown result type (might be due to invalid IL or missing references)
			//IL_0957: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05da: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_074d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0756: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_089c: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0916: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[0];
			ControlPoint controlPoint2 = m_ControlPoints[1];
			ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 1];
			FixElevation(ref controlPoint);
			FixElevation(ref controlPoint2);
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(m_NetPrefab))
			{
				netGeometryData = m_NetGeometryData[m_NetPrefab];
				if (netGeometryData.m_MaxSlopeSteepness == 0f)
				{
					SetHeight(controlPoint, ref controlPoint2);
					SetHeight(controlPoint, ref controlPoint3);
				}
			}
			bool flag = math.dot(((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint2.m_Position)).xz, MathUtils.Right(controlPoint2.m_Direction)) > 0f;
			flag ^= math.dot(((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint.m_Position)).xz, controlPoint2.m_Direction) < 0f;
			float3 val = default(float3);
			((float3)(ref val))._002Ector(controlPoint2.m_Direction.x, 0f, controlPoint2.m_Direction.y);
			controlPoint2.m_Position = controlPoint.m_Position + val * math.dot(controlPoint3.m_Position - controlPoint.m_Position, val);
			float2 val2 = default(float2);
			((float2)(ref val2))._002Ector(math.distance(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz), math.distance(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint3.m_Position)).xz));
			float2 val3 = (((netGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) == 0) ? (netGeometryData.m_DefaultWidth * new float2(16f, 8f)) : ((float)ZoneUtils.GetCellWidth(netGeometryData.m_DefaultWidth) * 8f + new float2(192f, 96f)));
			float2 val4 = math.max(float2.op_Implicit(1f), math.ceil((val2 - 0.16f) / val3));
			val3 = val2 / val4;
			val4 -= math.select(float2.op_Implicit(0f), float2.op_Implicit(1f), val3 < netGeometryData.m_DefaultWidth + 3f);
			int2 val5 = default(int2);
			((int2)(ref val5))._002Ector(Mathf.RoundToInt(val4.x), Mathf.RoundToInt(val4.y));
			if (val5.y == 0)
			{
				CreateStraightLine(ref ownerDefinitions, new int2(0, 1));
				return;
			}
			if (val5.x == 0)
			{
				CreateStraightLine(ref ownerDefinitions, new int2(1, m_ControlPoints.Length - 1));
				return;
			}
			Random random = m_RandomSeed.GetRandom(0);
			CoursePos coursePos = GetCoursePos(new Bezier4x3(controlPoint.m_Position, controlPoint.m_Position, controlPoint.m_Position, controlPoint.m_Position), controlPoint, 0f);
			CoursePos coursePos2 = GetCoursePos(new Bezier4x3(controlPoint3.m_Position, controlPoint3.m_Position, controlPoint3.m_Position, controlPoint3.m_Position), controlPoint3, 1f);
			coursePos.m_Flags |= CoursePosFlags.IsFirst;
			coursePos2.m_Flags |= CoursePosFlags.IsLast;
			OwnerDefinition ownerDefinition2;
			bool ownerDefinition = GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, coursePos, coursePos2, out ownerDefinition2);
			float length = math.distance(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz);
			float length2 = math.distance(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint3.m_Position)).xz);
			Segment val6 = default(Segment);
			((Segment)(ref val6))._002Ector(controlPoint.m_Position, controlPoint.m_Position + controlPoint3.m_Position - controlPoint2.m_Position);
			Segment val7 = default(Segment);
			((Segment)(ref val7))._002Ector(controlPoint2.m_Position, controlPoint3.m_Position);
			Segment val8 = default(Segment);
			((Segment)(ref val8))._002Ector(controlPoint.m_Elevation, controlPoint2.m_Elevation);
			Segment val9 = default(Segment);
			((Segment)(ref val9))._002Ector(controlPoint2.m_Elevation, controlPoint3.m_Elevation);
			int2 val10 = default(int2);
			val10.y = 0;
			Segment val11 = default(Segment);
			Segment val12 = default(Segment);
			Segment val13 = default(Segment);
			Segment val14 = default(Segment);
			while (val10.y <= val5.y)
			{
				float cutPosition = GetCutPosition(netGeometryData, length2, (float)val10.y / (float)val5.y);
				float cutPosition2 = GetCutPosition(netGeometryData, length2, (float)(val10.y + 1) / (float)val5.y);
				val11.a = MathUtils.Position(val6, cutPosition);
				val11.b = MathUtils.Position(val7, cutPosition);
				val12.a = MathUtils.Position(val6, cutPosition2);
				val12.b = MathUtils.Position(val7, cutPosition2);
				val13.a = MathUtils.Position(val8, cutPosition);
				val13.b = MathUtils.Position(val9, cutPosition);
				val14.a = MathUtils.Position(val8, cutPosition2);
				val14.b = MathUtils.Position(val9, cutPosition2);
				val10.x = 0;
				while (val10.x < val5.x)
				{
					Entity val15 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
					CreationDefinition creationDefinition = new CreationDefinition
					{
						m_Prefab = m_NetPrefab,
						m_SubPrefab = m_LanePrefab,
						m_RandomSeed = ((Random)(ref random)).NextInt()
					};
					creationDefinition.m_Flags |= CreationFlags.SubElevation;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val15, creationDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val15, default(Updated));
					bool num = math.all(val10 == 0);
					bool flag2 = math.all(new int2(val10.x + 1, val10.y) == val5);
					bool flag3 = (val10.y & 1) == 1 || val10.y == val5.y;
					ControlPoint controlPoint4;
					if (num)
					{
						controlPoint4 = controlPoint;
					}
					else
					{
						float cutPosition3 = GetCutPosition(netGeometryData, length, (float)val10.x / (float)val5.x);
						controlPoint4 = new ControlPoint
						{
							m_Rotation = controlPoint.m_Rotation,
							m_Position = MathUtils.Position(val11, cutPosition3),
							m_Elevation = MathUtils.Position(val13, cutPosition3)
						};
					}
					ControlPoint controlPoint5;
					if (flag2)
					{
						controlPoint5 = controlPoint3;
					}
					else
					{
						float cutPosition4 = GetCutPosition(netGeometryData, length, (float)(val10.x + 1) / (float)val5.x);
						controlPoint5 = new ControlPoint
						{
							m_Rotation = controlPoint.m_Rotation,
							m_Position = MathUtils.Position(val11, cutPosition4),
							m_Elevation = MathUtils.Position(val13, cutPosition4)
						};
					}
					NetCourse netCourse = default(NetCourse);
					netCourse.m_Curve = NetUtils.StraightCurve(controlPoint4.m_Position, controlPoint5.m_Position);
					netCourse.m_StartPosition = GetCoursePos(netCourse.m_Curve, controlPoint4, 0f);
					netCourse.m_EndPosition = GetCoursePos(netCourse.m_Curve, controlPoint5, 1f);
					if (!ownerDefinition)
					{
						netCourse.m_StartPosition.m_ParentMesh = -1;
						netCourse.m_EndPosition.m_ParentMesh = -1;
					}
					netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsGrid;
					netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsGrid;
					if (val10.y != 0)
					{
						netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsParallel;
						netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsParallel;
					}
					if (!num)
					{
						netCourse.m_StartPosition.m_Flags |= CoursePosFlags.FreeHeight;
					}
					if (!flag2)
					{
						netCourse.m_EndPosition.m_Flags |= CoursePosFlags.FreeHeight;
					}
					if (val10.y == 0 || val10.y == val5.y)
					{
						if (val10.x == 0)
						{
							netCourse.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
						}
						if (val10.x + 1 == val5.x)
						{
							netCourse.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
						}
						netCourse.m_StartPosition.m_Flags |= (CoursePosFlags)(flag ? 32 : 16);
						netCourse.m_EndPosition.m_Flags |= (CoursePosFlags)(flag ? 32 : 16);
					}
					if (flag3)
					{
						netCourse.m_Curve = MathUtils.Invert(netCourse.m_Curve);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_Entity, ref netCourse.m_EndPosition.m_Entity);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_SplitPosition, ref netCourse.m_EndPosition.m_SplitPosition);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_Position, ref netCourse.m_EndPosition.m_Position);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_Rotation, ref netCourse.m_EndPosition.m_Rotation);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_Elevation, ref netCourse.m_EndPosition.m_Elevation);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_Flags, ref netCourse.m_EndPosition.m_Flags);
						CommonUtils.Swap(ref netCourse.m_StartPosition.m_ParentMesh, ref netCourse.m_EndPosition.m_ParentMesh);
						quaternion val16 = quaternion.RotateY((float)Math.PI);
						netCourse.m_StartPosition.m_Rotation = math.mul(val16, netCourse.m_StartPosition.m_Rotation);
						netCourse.m_EndPosition.m_Rotation = math.mul(val16, netCourse.m_EndPosition.m_Rotation);
					}
					netCourse.m_Length = MathUtils.Length(netCourse.m_Curve);
					netCourse.m_FixedIndex = -1;
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val15, netCourse);
					if (ownerDefinition2.m_Prefab != Entity.Null)
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val15, ownerDefinition2);
						if (m_EditorMode && GetLocalCurve(netCourse, ownerDefinition2, out var localCurveCache))
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val15, localCurveCache);
						}
					}
					val10.x++;
				}
				if (val10.y != val5.y)
				{
					val10.x = 0;
					while (val10.x <= val5.x)
					{
						Entity val17 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
						CreationDefinition creationDefinition2 = new CreationDefinition
						{
							m_Prefab = m_NetPrefab,
							m_SubPrefab = m_LanePrefab,
							m_RandomSeed = ((Random)(ref random)).NextInt()
						};
						creationDefinition2.m_Flags |= CreationFlags.SubElevation;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val17, creationDefinition2);
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val17, default(Updated));
						bool num2 = math.all(val10 == 0);
						bool flag4 = math.all(new int2(val10.x, val10.y + 1) == val5);
						bool flag5 = ((val5.x - val10.x) & 1) == 1 || val10.x == 0;
						float cutPosition5 = GetCutPosition(netGeometryData, length, (float)val10.x / (float)val5.x);
						ControlPoint controlPoint6 = ((!num2) ? new ControlPoint
						{
							m_Rotation = controlPoint.m_Rotation,
							m_Position = MathUtils.Position(val11, cutPosition5),
							m_Elevation = MathUtils.Position(val13, cutPosition5)
						} : controlPoint);
						ControlPoint controlPoint7 = ((!flag4) ? new ControlPoint
						{
							m_Rotation = controlPoint.m_Rotation,
							m_Position = MathUtils.Position(val12, cutPosition5),
							m_Elevation = MathUtils.Position(val14, cutPosition5)
						} : controlPoint3);
						NetCourse netCourse2 = default(NetCourse);
						netCourse2.m_Curve = NetUtils.StraightCurve(controlPoint6.m_Position, controlPoint7.m_Position);
						netCourse2.m_StartPosition = GetCoursePos(netCourse2.m_Curve, controlPoint6, 0f);
						netCourse2.m_EndPosition = GetCoursePos(netCourse2.m_Curve, controlPoint7, 1f);
						if (!ownerDefinition)
						{
							netCourse2.m_StartPosition.m_ParentMesh = -1;
							netCourse2.m_EndPosition.m_ParentMesh = -1;
						}
						netCourse2.m_StartPosition.m_Flags |= CoursePosFlags.IsGrid;
						netCourse2.m_EndPosition.m_Flags |= CoursePosFlags.IsGrid;
						if (val10.x != val5.x)
						{
							netCourse2.m_StartPosition.m_Flags |= CoursePosFlags.IsParallel;
							netCourse2.m_EndPosition.m_Flags |= CoursePosFlags.IsParallel;
						}
						if (!num2)
						{
							netCourse2.m_StartPosition.m_Flags |= CoursePosFlags.FreeHeight;
						}
						if (!flag4)
						{
							netCourse2.m_EndPosition.m_Flags |= CoursePosFlags.FreeHeight;
						}
						if (val10.x == 0 || val10.x == val5.x)
						{
							if (val10.y == 0)
							{
								netCourse2.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
							}
							if (val10.y + 1 == val5.y)
							{
								netCourse2.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
							}
							netCourse2.m_StartPosition.m_Flags |= (CoursePosFlags)(flag ? 32 : 16);
							netCourse2.m_EndPosition.m_Flags |= (CoursePosFlags)(flag ? 32 : 16);
						}
						if (flag5)
						{
							netCourse2.m_Curve = MathUtils.Invert(netCourse2.m_Curve);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_Entity, ref netCourse2.m_EndPosition.m_Entity);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_SplitPosition, ref netCourse2.m_EndPosition.m_SplitPosition);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_Position, ref netCourse2.m_EndPosition.m_Position);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_Rotation, ref netCourse2.m_EndPosition.m_Rotation);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_Elevation, ref netCourse2.m_EndPosition.m_Elevation);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_Flags, ref netCourse2.m_EndPosition.m_Flags);
							CommonUtils.Swap(ref netCourse2.m_StartPosition.m_ParentMesh, ref netCourse2.m_EndPosition.m_ParentMesh);
							quaternion val18 = quaternion.RotateY((float)Math.PI);
							netCourse2.m_StartPosition.m_Rotation = math.mul(val18, netCourse2.m_StartPosition.m_Rotation);
							netCourse2.m_EndPosition.m_Rotation = math.mul(val18, netCourse2.m_EndPosition.m_Rotation);
						}
						netCourse2.m_Length = MathUtils.Length(netCourse2.m_Curve);
						netCourse2.m_FixedIndex = -1;
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val17, netCourse2);
						if (ownerDefinition2.m_Prefab != Entity.Null)
						{
							((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val17, ownerDefinition2);
							if (m_EditorMode && GetLocalCurve(netCourse2, ownerDefinition2, out var localCurveCache2))
							{
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val17, localCurveCache2);
							}
						}
						val10.x++;
					}
				}
				val10.y++;
			}
		}

		private void CreateComplexCurve(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_068c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_064f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0736: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[0];
			ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 3];
			ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 2];
			ControlPoint controlPoint4 = m_ControlPoints[m_ControlPoints.Length - 1];
			FixElevation(ref controlPoint);
			FixElevation(ref controlPoint2);
			FixElevation(ref controlPoint3);
			NetGeometryData netGeometryData = default(NetGeometryData);
			if (m_NetGeometryData.HasComponent(m_NetPrefab))
			{
				netGeometryData = m_NetGeometryData[m_NetPrefab];
				if (netGeometryData.m_MaxSlopeSteepness == 0f)
				{
					SetHeight(controlPoint, ref controlPoint2);
					SetHeight(controlPoint, ref controlPoint3);
					SetHeight(controlPoint, ref controlPoint4);
				}
			}
			else
			{
				netGeometryData.m_DefaultWidth = 0.02f;
			}
			float num2 = default(float);
			float num = MathUtils.Distance(new Segment(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz), ((float3)(ref controlPoint3.m_Position)).xz, ref num2);
			float num4 = default(float);
			float num3 = MathUtils.Distance(new Segment(((float3)(ref controlPoint3.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz), ((float3)(ref controlPoint.m_Position)).xz, ref num4);
			if (num <= netGeometryData.m_DefaultWidth * 0.75f && num <= num3)
			{
				num2 *= 0.5f + num / netGeometryData.m_DefaultWidth * (2f / 3f);
				controlPoint2.m_Position = math.lerp(controlPoint.m_Position, controlPoint2.m_Position, num2);
			}
			else if (num3 <= netGeometryData.m_DefaultWidth * 0.75f)
			{
				num4 *= 0.5f + num3 / netGeometryData.m_DefaultWidth * (2f / 3f);
				controlPoint2.m_Position = math.lerp(controlPoint3.m_Position, controlPoint2.m_Position, num4);
			}
			float2 val = ((float3)(ref controlPoint2.m_Position)).xz - ((float3)(ref controlPoint.m_Position)).xz;
			float2 val2 = ((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint4.m_Position)).xz;
			float2 val3 = val;
			float2 val4 = val2;
			if (!MathUtils.TryNormalize(ref val3))
			{
				CreateSimpleCurve(ref ownerDefinitions, 2);
				return;
			}
			if (!MathUtils.TryNormalize(ref val4))
			{
				CreateSimpleCurve(ref ownerDefinitions, 1);
				return;
			}
			float2 val5 = math.lerp(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint3.m_Position)).xz, 0.5f);
			num = MathUtils.Distance(new Segment(((float3)(ref controlPoint2.m_Position)).xz, ((float3)(ref controlPoint3.m_Position)).xz), ((float3)(ref controlPoint4.m_Position)).xz, ref num2);
			if (num <= netGeometryData.m_DefaultWidth * 0.75f)
			{
				num2 *= 0.5f + num / netGeometryData.m_DefaultWidth * (2f / 3f);
				controlPoint3.m_Position = math.lerp(controlPoint2.m_Position, controlPoint3.m_Position, num2);
				val2 = ((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint4.m_Position)).xz;
				val4 = val2;
				if (!MathUtils.TryNormalize(ref val4))
				{
					CreateSimpleCurve(ref ownerDefinitions, 1);
					return;
				}
			}
			Bezier4x3 val6 = default(Bezier4x3);
			((Bezier4x3)(ref val6))._002Ector(controlPoint.m_Position, controlPoint2.m_Position, controlPoint3.m_Position, controlPoint4.m_Position);
			float3 val7 = MathUtils.Position(val6, 0.5f);
			float2 xz = ((float3)(ref val7)).xz;
			float2 val8 = val5 - xz;
			float num5 = math.dot(val3, val4);
			float2 val12;
			if (math.abs(num5) < 0.999f)
			{
				float2 val9 = ((float2)(ref val4)).yx * val3;
				float2 val10 = ((float2)(ref val8)).yx * val3;
				float2 val11 = ((float2)(ref val4)).yx * val8;
				float num6 = (val9.x - val9.y) * 0.375f;
				val12 = new float2(val11.x - val11.y, val10.x - val10.y) / num6;
				val12 *= math.abs(num5);
			}
			else
			{
				float2 val13 = default(float2);
				((float2)(ref val13))._002Ector(math.length(val), math.length(val2));
				val12 = ((!(num5 > 0f)) ? (val13 / 3f) : (new float2(math.dot(val8, val3), math.dot(val8, val4)) * (val13 / (math.csum(val13) * 0.375f))));
			}
			ref float3 b = ref val6.b;
			((float3)(ref b)).xz = ((float3)(ref b)).xz + val3 * val12.x;
			ref float3 c = ref val6.c;
			((float3)(ref c)).xz = ((float3)(ref c)).xz + val4 * val12.y;
			Random random = m_RandomSeed.GetRandom(0);
			CreationDefinition creationDefinition = new CreationDefinition
			{
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition.m_Flags |= CreationFlags.SubElevation;
			NetCourse course = new NetCourse
			{
				m_Curve = val6
			};
			LinearizeElevation(ref course.m_Curve);
			course.m_StartPosition = GetCoursePos(course.m_Curve, controlPoint, 0f);
			course.m_EndPosition = GetCoursePos(course.m_Curve, controlPoint4, 1f);
			course.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			course.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			if (((float3)(ref course.m_StartPosition.m_Position)).Equals(course.m_EndPosition.m_Position) && ((Entity)(ref course.m_StartPosition.m_Entity)).Equals(course.m_EndPosition.m_Entity))
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			bool2 val14 = m_ParallelCount > 0;
			if (!val14.x)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
			if (!val14.y)
			{
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
				course.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			course.m_Length = MathUtils.Length(course.m_Curve);
			course.m_FixedIndex = -1;
			if (m_PlaceableData.HasComponent(m_NetPrefab))
			{
				PlaceableNetData placeableNetData = m_PlaceableData[m_NetPrefab];
				if (CalculatedInverseWeight(course, placeableNetData.m_PlacementFlags) < 0f)
				{
					InvertCourse(ref course);
				}
			}
			Entity val15 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val15, creationDefinition);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val15, default(Updated));
			if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, course.m_StartPosition, course.m_EndPosition, out var ownerDefinition))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val15, ownerDefinition);
				if (m_EditorMode && GetLocalCurve(course, ownerDefinition, out var localCurveCache))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val15, localCurveCache);
				}
			}
			else
			{
				course.m_StartPosition.m_ParentMesh = -1;
				course.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val15, course);
			if (math.any(val14))
			{
				NativeParallelHashMap<float4, float3> nodeMap = default(NativeParallelHashMap<float4, float3>);
				nodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				CreateParallelCourses(creationDefinition, ownerDefinition, course, nodeMap);
				nodeMap.Dispose();
			}
		}

		private void CreateContinuousCurve(ref NativeParallelHashMap<Entity, OwnerDefinition> ownerDefinitions)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_070a: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0722: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Unknown result type (might be due to invalid IL or missing references)
			//IL_0729: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0779: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0524: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_090f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0922: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0945: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_069b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			ControlPoint controlPoint = m_ControlPoints[0];
			ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 2];
			ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 1];
			FixElevation(ref controlPoint);
			FixElevation(ref controlPoint2);
			bool flag = false;
			if (m_NetGeometryData.HasComponent(m_NetPrefab))
			{
				NetGeometryData netGeometryData = m_NetGeometryData[m_NetPrefab];
				flag = (netGeometryData.m_Flags & Game.Net.GeometryFlags.NoCurveSplit) != 0;
				if (netGeometryData.m_MaxSlopeSteepness == 0f)
				{
					SetHeight(controlPoint, ref controlPoint2);
					SetHeight(controlPoint, ref controlPoint3);
				}
			}
			Random random = m_RandomSeed.GetRandom(0);
			float3 startTangent = default(float3);
			((float3)(ref startTangent))._002Ector(controlPoint2.m_Direction.x, 0f, controlPoint2.m_Direction.y);
			float3 val = default(float3);
			((float3)(ref val))._002Ector(controlPoint3.m_Direction.x, 0f, controlPoint3.m_Direction.y);
			float num = math.dot(math.normalizesafe(((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint.m_Position)).xz, default(float2)), controlPoint2.m_Direction);
			if (math.abs(num) < 0.01f && !flag)
			{
				int2 val2 = ((Random)(ref random)).NextInt2();
				CreationDefinition creationDefinition = new CreationDefinition
				{
					m_Prefab = m_NetPrefab,
					m_SubPrefab = m_LanePrefab
				};
				creationDefinition.m_Flags |= CreationFlags.SubElevation;
				NetCourse course = default(NetCourse);
				NetCourse course2 = default(NetCourse);
				float num2 = math.distance(((float3)(ref controlPoint.m_Position)).xz, ((float3)(ref controlPoint2.m_Position)).xz);
				controlPoint2.m_Direction = MathUtils.Right(controlPoint2.m_Direction);
				if (math.dot(((float3)(ref controlPoint3.m_Position)).xz - ((float3)(ref controlPoint.m_Position)).xz, controlPoint2.m_Direction) < 0f)
				{
					controlPoint2.m_Direction = -controlPoint2.m_Direction;
				}
				float3 val3 = default(float3);
				((float3)(ref val3))._002Ector(controlPoint2.m_Direction.x, 0f, controlPoint2.m_Direction.y);
				controlPoint2.m_OriginalEntity = Entity.Null;
				ref float3 position = ref controlPoint2.m_Position;
				position += val3 * num2;
				course.m_Curve = NetUtils.FitCurve(controlPoint.m_Position, startTangent, val3, controlPoint2.m_Position);
				course2.m_Curve = NetUtils.FitCurve(controlPoint2.m_Position, val3, val, controlPoint3.m_Position);
				Bezier4x3 val4 = default(Bezier4x3);
				Bezier4x3 val5 = default(Bezier4x3);
				MathUtils.Divide(NetUtils.FitCurve(controlPoint.m_Position, startTangent, val, controlPoint3.m_Position), ref val4, ref val5, 0.5f);
				float num3 = math.abs(num) * 100f;
				course.m_Curve = MathUtils.Lerp(course.m_Curve, val4, num3);
				course2.m_Curve = MathUtils.Lerp(course2.m_Curve, val5, num3);
				LinearizeElevation(ref course.m_Curve, ref course2.m_Curve);
				controlPoint2.m_Position = course.m_Curve.d;
				controlPoint2.m_Elevation = math.lerp(controlPoint.m_Elevation, controlPoint3.m_Elevation, 0.5f);
				course.m_StartPosition = GetCoursePos(course.m_Curve, controlPoint, 0f);
				course.m_EndPosition = GetCoursePos(course.m_Curve, controlPoint2, 1f);
				course2.m_StartPosition = GetCoursePos(course2.m_Curve, controlPoint2, 0f);
				course2.m_EndPosition = GetCoursePos(course2.m_Curve, controlPoint3, 1f);
				course.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
				course.m_EndPosition.m_Flags |= CoursePosFlags.FreeHeight;
				course2.m_StartPosition.m_Flags |= CoursePosFlags.FreeHeight;
				course2.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
				bool2 val6 = m_ParallelCount > 0;
				if (!val6.x)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
					course.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
					course2.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
					course2.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
				}
				if (!val6.y)
				{
					course.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
					course.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
					course2.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
					course2.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
				}
				course.m_Length = MathUtils.Length(course.m_Curve);
				course2.m_Length = MathUtils.Length(course2.m_Curve);
				course.m_FixedIndex = -1;
				course2.m_FixedIndex = -1;
				if (m_PlaceableData.HasComponent(m_NetPrefab))
				{
					PlaceableNetData placeableNetData = m_PlaceableData[m_NetPrefab];
					if (CalculatedInverseWeight(course, placeableNetData.m_PlacementFlags) + CalculatedInverseWeight(course2, placeableNetData.m_PlacementFlags) < 0f)
					{
						InvertCourse(ref course);
						InvertCourse(ref course2);
					}
				}
				Entity val7 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				Entity val8 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
				creationDefinition.m_RandomSeed = val2.x;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val7, creationDefinition);
				creationDefinition.m_RandomSeed = val2.y;
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val8, creationDefinition);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val7, default(Updated));
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val8, default(Updated));
				if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, course.m_StartPosition, course2.m_EndPosition, out var ownerDefinition))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val7, ownerDefinition);
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val8, ownerDefinition);
					if (m_EditorMode && GetLocalCurve(course, ownerDefinition, out var localCurveCache))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val7, localCurveCache);
					}
					if (m_EditorMode && GetLocalCurve(course2, ownerDefinition, out var localCurveCache2))
					{
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val8, localCurveCache2);
					}
				}
				else
				{
					course.m_StartPosition.m_ParentMesh = -1;
					course.m_EndPosition.m_ParentMesh = -1;
					course2.m_StartPosition.m_ParentMesh = -1;
					course2.m_EndPosition.m_ParentMesh = -1;
				}
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val7, course);
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val8, course2);
				if (math.any(val6))
				{
					NativeParallelHashMap<float4, float3> nodeMap = default(NativeParallelHashMap<float4, float3>);
					nodeMap._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
					CreateParallelCourses(creationDefinition, ownerDefinition, course, nodeMap);
					CreateParallelCourses(creationDefinition, ownerDefinition, course2, nodeMap);
					nodeMap.Dispose();
				}
				return;
			}
			CreationDefinition creationDefinition2 = new CreationDefinition
			{
				m_Prefab = m_NetPrefab,
				m_SubPrefab = m_LanePrefab,
				m_RandomSeed = ((Random)(ref random)).NextInt()
			};
			creationDefinition2.m_Flags |= CreationFlags.SubElevation;
			NetCourse course3 = default(NetCourse);
			if (num < 0f)
			{
				float3 endPos = controlPoint3.m_Position + val * num;
				course3.m_Curve = NetUtils.FitCurve(controlPoint.m_Position, startTangent, val, endPos);
				course3.m_Curve.d = controlPoint3.m_Position;
			}
			else
			{
				course3.m_Curve = NetUtils.FitCurve(controlPoint.m_Position, startTangent, val, controlPoint3.m_Position);
			}
			LinearizeElevation(ref course3.m_Curve);
			course3.m_StartPosition = GetCoursePos(course3.m_Curve, controlPoint, 0f);
			course3.m_EndPosition = GetCoursePos(course3.m_Curve, controlPoint3, 1f);
			course3.m_StartPosition.m_Flags |= CoursePosFlags.IsFirst;
			course3.m_EndPosition.m_Flags |= CoursePosFlags.IsLast;
			if (((float3)(ref course3.m_StartPosition.m_Position)).Equals(course3.m_EndPosition.m_Position) && ((Entity)(ref course3.m_StartPosition.m_Entity)).Equals(course3.m_EndPosition.m_Entity))
			{
				course3.m_StartPosition.m_Flags |= CoursePosFlags.IsLast;
				course3.m_EndPosition.m_Flags |= CoursePosFlags.IsFirst;
			}
			bool2 val9 = m_ParallelCount > 0;
			if (!val9.x)
			{
				course3.m_StartPosition.m_Flags |= CoursePosFlags.IsLeft;
				course3.m_EndPosition.m_Flags |= CoursePosFlags.IsLeft;
			}
			if (!val9.y)
			{
				course3.m_StartPosition.m_Flags |= CoursePosFlags.IsRight;
				course3.m_EndPosition.m_Flags |= CoursePosFlags.IsRight;
			}
			course3.m_Length = MathUtils.Length(course3.m_Curve);
			course3.m_FixedIndex = -1;
			if (m_PlaceableData.HasComponent(m_NetPrefab))
			{
				PlaceableNetData placeableNetData2 = m_PlaceableData[m_NetPrefab];
				if (CalculatedInverseWeight(course3, placeableNetData2.m_PlacementFlags) < 0f)
				{
					InvertCourse(ref course3);
				}
			}
			Entity val10 = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity();
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<CreationDefinition>(val10, creationDefinition2);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(val10, default(Updated));
			if (GetOwnerDefinition(ref ownerDefinitions, Entity.Null, checkControlPoints: true, course3.m_StartPosition, course3.m_EndPosition, out var ownerDefinition2))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OwnerDefinition>(val10, ownerDefinition2);
				if (m_EditorMode && GetLocalCurve(course3, ownerDefinition2, out var localCurveCache3))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<LocalCurveCache>(val10, localCurveCache3);
				}
			}
			else
			{
				course3.m_StartPosition.m_ParentMesh = -1;
				course3.m_EndPosition.m_ParentMesh = -1;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<NetCourse>(val10, course3);
			if (math.any(val9))
			{
				NativeParallelHashMap<float4, float3> nodeMap2 = default(NativeParallelHashMap<float4, float3>);
				nodeMap2._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
				CreateParallelCourses(creationDefinition2, ownerDefinition2, course3, nodeMap2);
				nodeMap2.Dispose();
			}
		}

		private void LinearizeElevation(ref Bezier4x3 curve)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.lerp(float2.op_Implicit(curve.a.y), float2.op_Implicit(curve.d.y), new float2(1f / 3f, 2f / 3f));
			curve.b.y = val.x;
			curve.c.y = val.y;
		}

		private void LinearizeElevation(ref Bezier4x3 curve1, ref Bezier4x3 curve2)
		{
			curve1.d.y = (curve2.a.y = math.lerp(curve1.a.y, curve2.d.y, 0.5f));
			LinearizeElevation(ref curve1);
			LinearizeElevation(ref curve2);
		}

		private CoursePos GetCoursePos(Bezier4x3 curve, ControlPoint controlPoint, float courseDelta)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			CoursePos result = default(CoursePos);
			if (controlPoint.m_OriginalEntity != Entity.Null)
			{
				if (m_EdgeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					if (controlPoint.m_CurvePosition <= 0f)
					{
						result.m_Entity = m_EdgeData[controlPoint.m_OriginalEntity].m_Start;
						result.m_SplitPosition = 0f;
					}
					else if (controlPoint.m_CurvePosition >= 1f)
					{
						result.m_Entity = m_EdgeData[controlPoint.m_OriginalEntity].m_End;
						result.m_SplitPosition = 1f;
					}
					else
					{
						result.m_Entity = controlPoint.m_OriginalEntity;
						result.m_SplitPosition = controlPoint.m_CurvePosition;
					}
				}
				else if (m_NodeData.HasComponent(controlPoint.m_OriginalEntity))
				{
					result.m_Entity = controlPoint.m_OriginalEntity;
					result.m_SplitPosition = controlPoint.m_CurvePosition;
				}
			}
			result.m_Position = controlPoint.m_Position;
			result.m_Elevation = float2.op_Implicit(controlPoint.m_Elevation);
			result.m_Rotation = NetUtils.GetNodeRotation(MathUtils.Tangent(curve, courseDelta));
			result.m_CourseDelta = courseDelta;
			result.m_ParentMesh = controlPoint.m_ElementIndex.x;
			Entity val = controlPoint.m_OriginalEntity;
			Edge edge = default(Edge);
			LocalTransformCache localTransformCache = default(LocalTransformCache);
			LocalTransformCache localTransformCache2 = default(LocalTransformCache);
			while (m_OwnerData.HasComponent(val) && !m_BuildingData.HasComponent(val) && !m_ExtensionData.HasComponent(val))
			{
				if (m_LocalTransformCacheData.HasComponent(val))
				{
					result.m_ParentMesh = m_LocalTransformCacheData[val].m_ParentMesh;
				}
				else if (m_EdgeData.TryGetComponent(val, ref edge) && m_LocalTransformCacheData.TryGetComponent(edge.m_Start, ref localTransformCache) && m_LocalTransformCacheData.TryGetComponent(edge.m_End, ref localTransformCache2))
				{
					result.m_ParentMesh = math.select(localTransformCache.m_ParentMesh, -1, localTransformCache.m_ParentMesh != localTransformCache2.m_ParentMesh);
				}
				val = m_OwnerData[val].m_Owner;
			}
			return result;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NetCourse> __Game_Tools_NetCourse_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadData> __Game_Prefabs_RoadData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingExtensionData> __Game_Prefabs_BuildingExtensionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AssetStampData> __Game_Prefabs_AssetStampData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalConnectData> __Game_Prefabs_LocalConnectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubReplacement> __Game_Net_SubReplacement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionArea> __Game_Prefabs_NetCompositionArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubObject> __Game_Prefabs_SubObject_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Fixed> __Game_Net_Fixed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EditorContainer> __Game_Tools_EditorContainer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Extension> __Game_Buildings_Extension_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableObjectData> __Game_Prefabs_SpawnableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<LocalNodeCache> __Game_Tools_LocalNodeCache_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubNet> __Game_Prefabs_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Prefabs.SubArea> __Game_Prefabs_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<SubAreaNode> __Game_Prefabs_SubAreaNode_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<PlaceholderObjectElement> __Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Tools_NetCourse_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCourse>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.Node>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_RoadData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingExtensionData>(true);
			__Game_Prefabs_AssetStampData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AssetStampData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_LocalConnectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalConnectData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_SubReplacement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubReplacement>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Prefabs_NetCompositionArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionArea>(true);
			__Game_Prefabs_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubObject>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Net_Fixed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Fixed>(true);
			__Game_Tools_EditorContainer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EditorContainer>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Extension_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extension>(true);
			__Game_Prefabs_SpawnableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableObjectData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Tools_LocalNodeCache_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LocalNodeCache>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Prefabs_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubNet>(true);
			__Game_Prefabs_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Prefabs.SubArea>(true);
			__Game_Prefabs_SubAreaNode_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubAreaNode>(true);
			__Game_Prefabs_PlaceholderObjectElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<PlaceholderObjectElement>(true);
		}
	}

	public const string kToolID = "Net Tool";

	private bool m_LoadingPreferences;

	private Mode m_Mode;

	private float m_Elevation;

	private float m_LastMouseElevation;

	private float m_ElevationStep;

	private int m_ParallelCount;

	private float m_ParallelOffset;

	private bool m_Underground;

	private Snap m_SelectedSnap;

	private ToolOutputBarrier m_ToolOutputBarrier;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Zones.SearchSystem m_ZoneSearchSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private AudioManager m_AudioManager;

	private NetInitializeSystem m_NetInitializeSystem;

	private EntityQuery m_DefinitionQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_NodeQuery;

	private EntityQuery m_SoundQuery;

	private EntityQuery m_ContainerQuery;

	private IProxyAction m_DowngradeNetEdge;

	private IProxyAction m_PlaceNetControlPoint;

	private IProxyAction m_PlaceNetEdge;

	private IProxyAction m_PlaceNetNode;

	private IProxyAction m_ReplaceNetEdge;

	private IProxyAction m_UndoNetControlPoint;

	private IProxyAction m_UpgradeNetEdge;

	private IProxyAction m_DiscardUpgrade;

	private IProxyAction m_DiscardDowngrade;

	private IProxyAction m_DiscardReplace;

	private bool m_ApplyBlocked;

	private NativeList<ControlPoint> m_ControlPoints;

	private NativeList<SnapLine> m_SnapLines;

	private NativeList<UpgradeState> m_UpgradeStates;

	private NativeReference<Entity> m_StartEntity;

	private NativeReference<Entity> m_LastSnappedEntity;

	private NativeReference<int> m_LastControlPointsAngle;

	private NativeReference<AppliedUpgrade> m_AppliedUpgrade;

	private ControlPoint m_LastRaycastPoint;

	private ControlPoint m_ApplyStartPoint;

	private State m_State;

	private Bounds1 m_LastElevationRange;

	private Mode m_LastActualMode;

	private float m_ApplyTimer;

	private NetPrefab m_Prefab;

	private NetPrefab m_SelectedPrefab;

	private NetLanePrefab m_LanePrefab;

	private bool m_AllowUndergroundReplace;

	private bool m_ForceCancel;

	private RandomSeed m_RandomSeed;

	private NetToolPreferences m_DefaultToolPreferences;

	private Dictionary<Entity, NetToolPreferences> m_ToolPreferences;

	private TypeHandle __TypeHandle;

	public override string toolID => "Net Tool";

	public override int uiModeIndex => (int)actualMode;

	public Mode mode
	{
		get
		{
			return m_Mode;
		}
		set
		{
			if (value != m_Mode)
			{
				m_Mode = value;
				m_ForceUpdate = true;
				SaveToolPreferences();
			}
		}
	}

	public Mode actualMode
	{
		get
		{
			if (upgradeOnly)
			{
				return Mode.Replace;
			}
			switch (mode)
			{
			case Mode.Grid:
				if (!allowGrid)
				{
					return Mode.Straight;
				}
				return mode;
			case Mode.Replace:
				if (!allowReplace)
				{
					return Mode.Straight;
				}
				return mode;
			case Mode.Point:
				if (!m_ToolSystem.actionMode.IsEditor())
				{
					return Mode.Straight;
				}
				return mode;
			default:
				return mode;
			}
		}
	}

	public float elevation
	{
		get
		{
			return m_Elevation;
		}
		set
		{
			if (value != m_Elevation)
			{
				m_Elevation = value;
				m_ForceUpdate = true;
				SaveToolPreferences();
			}
		}
	}

	public float elevationStep
	{
		get
		{
			return m_ElevationStep;
		}
		set
		{
			if (value != m_ElevationStep)
			{
				m_ElevationStep = value;
				SaveToolPreferences();
			}
		}
	}

	public int parallelCount
	{
		get
		{
			return m_ParallelCount;
		}
		set
		{
			if (value != m_ParallelCount)
			{
				m_ParallelCount = value;
				m_ForceUpdate = true;
				SaveToolPreferences();
			}
		}
	}

	public int actualParallelCount
	{
		get
		{
			if (!allowParallel || (allowGrid && mode == Mode.Grid))
			{
				return 0;
			}
			return parallelCount;
		}
	}

	public float parallelOffset
	{
		get
		{
			return m_ParallelOffset;
		}
		set
		{
			if (value != m_ParallelOffset)
			{
				m_ParallelOffset = value;
				m_ForceUpdate = true;
				SaveToolPreferences();
			}
		}
	}

	public bool underground
	{
		get
		{
			return m_Underground;
		}
		set
		{
			if (value != m_Underground)
			{
				m_Underground = value;
				m_ForceUpdate = true;
				SaveToolPreferences();
			}
		}
	}

	public override bool allowUnderground
	{
		get
		{
			if (actualMode == Mode.Replace)
			{
				return m_AllowUndergroundReplace;
			}
			return false;
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
				SaveToolPreferences();
			}
		}
	}

	public NetPrefab prefab
	{
		get
		{
			return m_SelectedPrefab;
		}
		set
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)value != (Object)(object)m_SelectedPrefab)
			{
				m_SelectedPrefab = value;
				m_ForceUpdate = true;
				if ((Object)(object)value != (Object)null)
				{
					m_LanePrefab = null;
				}
				if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_SelectedPrefab, out PlaceableNetData component))
				{
					upgradeOnly = (component.m_PlacementFlags & Game.Net.PlacementFlags.UpgradeOnly) != 0;
					allowParallel = (component.m_PlacementFlags & Game.Net.PlacementFlags.AllowParallel) != 0;
					allowGrid = allowParallel && (!m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)m_SelectedPrefab, out NetGeometryData component2) || component2.m_EdgeLengthRange.min == 0f);
					allowReplace = m_PrefabSystem.TryGetComponentData<NetData>((PrefabBase)m_SelectedPrefab, out NetData component3) && m_NetInitializeSystem.CanReplace(component3, m_ToolSystem.actionMode.IsGame());
					serviceUpgrade = m_PrefabSystem.HasComponent<ServiceUpgradeData>(m_SelectedPrefab);
					m_AllowUndergroundReplace = component.m_ElevationRange.min < 0f || (component.m_PlacementFlags & Game.Net.PlacementFlags.UndergroundUpgrade) != 0;
				}
				else
				{
					upgradeOnly = false;
					allowParallel = false;
					allowGrid = false;
					allowReplace = false;
					serviceUpgrade = false;
					m_AllowUndergroundReplace = false;
				}
				LoadToolPreferences();
				m_ToolSystem.EventPrefabChanged?.Invoke(value);
			}
		}
	}

	public NetLanePrefab lane
	{
		get
		{
			return m_LanePrefab;
		}
		set
		{
			if ((Object)(object)value != (Object)(object)m_LanePrefab)
			{
				m_LanePrefab = value;
				m_ForceUpdate = true;
				if ((Object)(object)value != (Object)null)
				{
					m_SelectedPrefab = null;
					upgradeOnly = false;
					allowParallel = true;
					allowGrid = false;
					allowReplace = false;
					serviceUpgrade = false;
				}
				LoadToolPreferences();
				m_ToolSystem.EventPrefabChanged?.Invoke(value);
			}
		}
	}

	public bool upgradeOnly { get; private set; }

	public bool allowParallel { get; private set; }

	public bool allowGrid { get; private set; }

	public bool allowReplace { get; private set; }

	public bool serviceUpgrade { get; private set; }

	private protected override IEnumerable<IProxyAction> toolActions
	{
		get
		{
			yield return m_DowngradeNetEdge;
			yield return m_PlaceNetControlPoint;
			yield return m_PlaceNetEdge;
			yield return m_PlaceNetNode;
			yield return m_ReplaceNetEdge;
			yield return m_UndoNetControlPoint;
			yield return m_UpgradeNetEdge;
			yield return m_DiscardUpgrade;
			yield return m_DiscardDowngrade;
			yield return m_DiscardReplace;
		}
	}

	public override void GetUIModes(List<ToolMode> modes)
	{
		if (upgradeOnly)
		{
			modes.Add(new ToolMode(Mode.Replace.ToString(), 5));
			return;
		}
		modes.Add(new ToolMode(Mode.Straight.ToString(), 0));
		modes.Add(new ToolMode(Mode.SimpleCurve.ToString(), 1));
		modes.Add(new ToolMode(Mode.ComplexCurve.ToString(), 2));
		modes.Add(new ToolMode(Mode.Continuous.ToString(), 3));
		if (allowGrid)
		{
			modes.Add(new ToolMode(Mode.Grid.ToString(), 4));
		}
		if (allowReplace)
		{
			modes.Add(new ToolMode(Mode.Replace.ToString(), 5));
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			modes.Add(new ToolMode(Mode.Point.ToString(), 6));
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Zones.SearchSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_AudioManager = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AudioManager>();
		m_NetInitializeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NetInitializeSystem>();
		m_ControlPoints = new NativeList<ControlPoint>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_SnapLines = new NativeList<SnapLine>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_UpgradeStates = new NativeList<UpgradeState>(4, AllocatorHandle.op_Implicit((Allocator)4));
		m_StartEntity = new NativeReference<Entity>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_LastSnappedEntity = new NativeReference<Entity>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_LastControlPointsAngle = new NativeReference<int>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_AppliedUpgrade = new NativeReference<AppliedUpgrade>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_DefinitionQuery = GetDefinitionQuery();
		m_ContainerQuery = GetContainerQuery();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Lane>()
		});
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Edge>()
		});
		m_NodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Game.Net.Node>()
		});
		m_SoundQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ToolUXSoundSettingsData>() });
		m_DowngradeNetEdge = InputManager.instance.toolActionCollection.GetActionState("Downgrade Net Edge", "NetToolSystem");
		m_PlaceNetControlPoint = InputManager.instance.toolActionCollection.GetActionState("Place Net Control Point", "NetToolSystem");
		m_PlaceNetEdge = InputManager.instance.toolActionCollection.GetActionState("Place Net Edge", "NetToolSystem");
		m_PlaceNetNode = InputManager.instance.toolActionCollection.GetActionState("Place Net Node", "NetToolSystem");
		m_ReplaceNetEdge = InputManager.instance.toolActionCollection.GetActionState("Replace Net Edge", "NetToolSystem");
		m_UndoNetControlPoint = InputManager.instance.toolActionCollection.GetActionState("Undo Net Control Point", "NetToolSystem");
		m_UpgradeNetEdge = InputManager.instance.toolActionCollection.GetActionState("Upgrade Net Edge", "NetToolSystem");
		m_DiscardUpgrade = InputManager.instance.toolActionCollection.GetActionState("Discard Upgrade", "NetToolSystem");
		m_DiscardDowngrade = InputManager.instance.toolActionCollection.GetActionState("Discard Downgrade", "NetToolSystem");
		m_DiscardReplace = InputManager.instance.toolActionCollection.GetActionState("Discard Replace", "NetToolSystem");
		elevationStep = 10f;
		parallelOffset = 8f;
		selectedSnap &= ~(Snap.AutoParent | Snap.ContourLines);
		m_DefaultToolPreferences = new NetToolPreferences();
		m_DefaultToolPreferences.Save(this);
		m_ToolPreferences = new Dictionary<Entity, NetToolPreferences>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ControlPoints.Dispose();
		m_SnapLines.Dispose();
		m_UpgradeStates.Dispose();
		m_StartEntity.Dispose();
		m_LastSnappedEntity.Dispose();
		m_LastControlPointsAngle.Dispose();
		m_AppliedUpgrade.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.OnStartRunning();
		m_ControlPoints.Clear();
		m_SnapLines.Clear();
		m_UpgradeStates.Clear();
		m_StartEntity.Value = default(Entity);
		m_LastSnappedEntity.Value = default(Entity);
		m_LastControlPointsAngle.Value = 0;
		m_AppliedUpgrade.Value = default(AppliedUpgrade);
		m_LastRaycastPoint = default(ControlPoint);
		m_ApplyStartPoint = default(ControlPoint);
		m_State = State.Default;
		m_ApplyTimer = 0f;
		m_RandomSeed = RandomSeed.Next();
		m_ForceCancel = false;
		m_ApplyBlocked = false;
		base.requireZones = false;
		base.requireUnderground = false;
		base.requirePipelines = false;
		base.requireNetArrows = false;
		base.requireAreas = AreaTypeMask.None;
		base.requireNet = Layer.None;
	}

	private protected override void UpdateActions()
	{
		using (ProxyAction.DeferStateUpdating())
		{
			switch (actualMode)
			{
			case Mode.Straight:
			case Mode.SimpleCurve:
			case Mode.ComplexCurve:
			case Mode.Continuous:
			case Mode.Grid:
			{
				int maxControlPointCount = GetMaxControlPointCount(actualMode);
				base.applyAction.enabled = base.actionsEnabled && ((m_ControlPoints.Length > 1 && m_ControlPoints.Length < maxControlPointCount) || GetAllowApply());
				base.applyActionOverride = ((m_ControlPoints.Length < maxControlPointCount) ? m_PlaceNetControlPoint : m_PlaceNetEdge);
				base.secondaryApplyAction.enabled = false;
				base.secondaryApplyActionOverride = null;
				base.cancelAction.enabled = base.actionsEnabled && m_ControlPoints.Length >= 2;
				base.cancelActionOverride = m_UndoNetControlPoint;
				break;
			}
			case Mode.Replace:
			{
				bool flag = m_ControlPoints.Length > 4;
				if (prefab.Has<NetUpgrade>())
				{
					if (!flag)
					{
						base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
						base.applyActionOverride = m_UpgradeNetEdge;
						base.secondaryApplyAction.enabled = base.actionsEnabled;
						base.secondaryApplyActionOverride = m_DowngradeNetEdge;
						base.cancelAction.enabled = false;
						base.cancelActionOverride = null;
					}
					else if (m_State == State.Applying)
					{
						base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
						base.applyActionOverride = m_UpgradeNetEdge;
						base.secondaryApplyAction.enabled = false;
						base.secondaryApplyActionOverride = null;
						base.cancelAction.enabled = base.actionsEnabled;
						base.cancelActionOverride = m_DiscardUpgrade;
					}
					else if (m_State == State.Cancelling)
					{
						base.applyAction.enabled = false;
						base.applyActionOverride = null;
						base.secondaryApplyAction.enabled = base.actionsEnabled;
						base.secondaryApplyActionOverride = m_DowngradeNetEdge;
						base.cancelAction.enabled = base.actionsEnabled;
						base.cancelActionOverride = m_DiscardDowngrade;
					}
				}
				else
				{
					base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
					base.applyActionOverride = m_ReplaceNetEdge;
					base.secondaryApplyAction.enabled = false;
					base.secondaryApplyActionOverride = null;
					if (flag)
					{
						base.cancelAction.enabled = base.actionsEnabled;
						base.cancelActionOverride = m_DiscardReplace;
					}
					else
					{
						base.cancelAction.enabled = false;
						base.cancelActionOverride = null;
					}
				}
				break;
			}
			case Mode.Point:
				base.applyAction.enabled = base.actionsEnabled && GetAllowApply();
				base.applyActionOverride = m_PlaceNetNode;
				base.secondaryApplyAction.enabled = false;
				base.secondaryApplyActionOverride = null;
				base.cancelAction.enabled = base.actionsEnabled && m_ControlPoints.Length >= 2;
				base.cancelActionOverride = m_UndoNetControlPoint;
				break;
			}
		}
	}

	public override PrefabBase GetPrefab()
	{
		if (!((Object)(object)prefab != (Object)null))
		{
			return lane;
		}
		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if (prefab is NetPrefab netPrefab)
		{
			this.prefab = netPrefab;
			return true;
		}
		if (prefab is NetLanePrefab netLanePrefab)
		{
			lane = netLanePrefab;
			return true;
		}
		return false;
	}

	private void LoadToolPreferences()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		PrefabBase prefabBase = GetPrefab();
		if (!((Object)(object)prefabBase == (Object)null))
		{
			m_LoadingPreferences = true;
			Entity entity = m_PrefabSystem.GetEntity(prefabBase);
			UIObjectData uIObjectData = default(UIObjectData);
			EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref uIObjectData);
			if (m_ToolPreferences.TryGetValue(uIObjectData.m_Group, out var value))
			{
				value.Load(this);
			}
			else
			{
				m_DefaultToolPreferences.Load(this);
			}
			m_LoadingPreferences = false;
		}
	}

	private void SaveToolPreferences()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (m_LoadingPreferences)
		{
			return;
		}
		PrefabBase prefabBase = GetPrefab();
		if (!((Object)(object)prefabBase == (Object)null))
		{
			Entity entity = m_PrefabSystem.GetEntity(prefabBase);
			UIObjectData uIObjectData = default(UIObjectData);
			EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, entity, ref uIObjectData);
			if (!m_ToolPreferences.ContainsKey(uIObjectData.m_Group))
			{
				m_ToolPreferences[uIObjectData.m_Group] = new NetToolPreferences();
			}
			m_ToolPreferences[uIObjectData.m_Group].Save(this);
		}
	}

	public void ResetToolPreferences()
	{
		m_ToolPreferences.Clear();
		m_DefaultToolPreferences.Load(this);
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		ResetToolPreferences();
	}

	public NativeList<ControlPoint> GetControlPoints(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_ControlPoints;
	}

	public NativeList<SnapLine> GetSnapLines(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = ((SystemBase)this).Dependency;
		return m_SnapLines;
	}

	private NetPrefab GetNetPrefab()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.actionMode.IsEditor() && (Object)(object)m_LanePrefab != (Object)null && GetContainers(m_ContainerQuery, out var laneContainer, out var _))
		{
			return m_PrefabSystem.GetPrefab<NetPrefab>(laneContainer);
		}
		return m_SelectedPrefab;
	}

	public override void SetUnderground(bool underground)
	{
		if (actualMode == Mode.Replace)
		{
			this.underground = underground;
		}
	}

	public override void ElevationUp()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		NetPrefab netPrefab = GetNetPrefab();
		if (!((Object)(object)netPrefab != (Object)null))
		{
			return;
		}
		if (actualMode == Mode.Replace)
		{
			underground = false;
			return;
		}
		m_Prefab = netPrefab;
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out PlaceableNetData component) && component.m_UndergroundPrefab != Entity.Null && elevation < 0f)
		{
			m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
		}
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component))
		{
			CheckElevationRange(component);
			elevation = math.floor(elevation / elevationStep + 1.00001f) * elevationStep;
			if (elevation > component.m_ElevationRange.max + elevationStep * 0.5f && (Object)(object)m_Prefab != (Object)(object)netPrefab)
			{
				m_Prefab = netPrefab;
				m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component);
				CheckElevationRange(component);
			}
		}
	}

	public override void ElevationDown()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		NetPrefab netPrefab = GetNetPrefab();
		if (!((Object)(object)netPrefab != (Object)null))
		{
			return;
		}
		if (actualMode == Mode.Replace)
		{
			underground = true;
			return;
		}
		m_Prefab = netPrefab;
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out PlaceableNetData component) && component.m_UndergroundPrefab != Entity.Null && elevation < 0f)
		{
			m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
		}
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component))
		{
			CheckElevationRange(component);
			elevation = math.ceil(elevation / elevationStep - 1.00001f) * elevationStep;
			if (elevation < component.m_ElevationRange.min - elevationStep * 0.5f && (Object)(object)m_Prefab == (Object)(object)netPrefab && component.m_UndergroundPrefab != Entity.Null)
			{
				m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
				m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component);
				CheckElevationRange(component);
			}
		}
	}

	public override void ElevationScroll()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		NetPrefab netPrefab = GetNetPrefab();
		if (!((Object)(object)netPrefab != (Object)null))
		{
			return;
		}
		if (actualMode == Mode.Replace)
		{
			underground = !underground;
			return;
		}
		m_Prefab = netPrefab;
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out PlaceableNetData component) && component.m_UndergroundPrefab != Entity.Null && elevation < 0f)
		{
			m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
		}
		if (!m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component))
		{
			return;
		}
		elevation = math.floor(elevation / elevationStep + 1.00001f) * elevationStep;
		if (!(elevation > component.m_ElevationRange.max + elevationStep * 0.5f))
		{
			return;
		}
		if ((Object)(object)m_Prefab != (Object)(object)netPrefab)
		{
			m_Prefab = netPrefab;
			m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component);
			CheckElevationRange(component);
			return;
		}
		if (component.m_UndergroundPrefab != Entity.Null)
		{
			m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
			m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component);
		}
		elevation = math.ceil(component.m_ElevationRange.min / elevationStep) * elevationStep;
		CheckElevationRange(component);
	}

	public override void InitializeRaycast()
	{
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		base.InitializeRaycast();
		NetPrefab netPrefab = GetNetPrefab();
		m_Prefab = null;
		if (actualMode == Mode.Replace)
		{
			if ((Object)(object)netPrefab != (Object)null)
			{
				if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)netPrefab, out PlaceableNetData component))
				{
					if ((component.m_PlacementFlags & Game.Net.PlacementFlags.UndergroundUpgrade) == 0)
					{
						if (component.m_ElevationRange.min >= 0f && component.m_UndergroundPrefab == Entity.Null)
						{
							underground = false;
						}
						else if (component.m_ElevationRange.max < 0f && component.m_UndergroundPrefab == Entity.Null)
						{
							underground = true;
						}
					}
				}
				else
				{
					underground = false;
				}
				m_Prefab = ((underground && component.m_UndergroundPrefab != Entity.Null) ? m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab) : netPrefab);
				NetData componentData = m_PrefabSystem.GetComponentData<NetData>((PrefabBase)m_Prefab);
				m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)m_Prefab, out NetGeometryData component2);
				if (underground)
				{
					m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
				}
				else
				{
					m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
				}
				if ((component2.m_Flags & Game.Net.GeometryFlags.Marker) != 0)
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Markers;
				}
				m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements | RaycastFlags.IgnoreSecondary;
				m_ToolRaycastSystem.typeMask = TypeMask.Net;
				m_ToolRaycastSystem.netLayerMask = componentData.m_RequiredLayers;
			}
			else
			{
				m_ToolRaycastSystem.collisionMask = (CollisionMask)0;
				m_ToolRaycastSystem.typeMask = TypeMask.Net;
				m_ToolRaycastSystem.netLayerMask = Layer.None;
			}
		}
		else if ((Object)(object)netPrefab != (Object)null)
		{
			if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse && m_State == State.Applying && SharedSettings.instance.input.elevationDraggingEnabled)
			{
				Camera main = Camera.main;
				if ((Object)(object)main != (Object)null && InputManager.instance.mouseOnScreen)
				{
					Line3 val = Line3.op_Implicit(ToolRaycastSystem.CalculateRaycastLine(main));
					float3 hitPosition = m_ApplyStartPoint.m_HitPosition;
					hitPosition.y += m_ApplyStartPoint.m_Elevation;
					Triangle3 plane = default(Triangle3);
					((Triangle3)(ref plane))._002Ector(hitPosition, hitPosition + math.up(), hitPosition + float3.op_Implicit(((Component)main).transform.right));
					if (TryIntersectLineWithPlane(val, plane, 0.05f, out var d) && d >= 0f && (double)d <= 1.0)
					{
						float3 val2 = MathUtils.Position(val, d);
						float num = val2.y - hitPosition.y;
						float num2 = math.distance(val.a, val2);
						float num3 = 2f * math.tan(math.radians(math.min(89f, main.fieldOfView * 0.5f))) * num2;
						float num4 = math.abs(num) / math.max(1f, num3);
						float num5 = 0.5f / (1f + num4 * 20f);
						if (m_ApplyTimer >= num5)
						{
							GetSurfaceHeights(netPrefab, out var overground, out var num6);
							bool flag = m_ApplyStartPoint.m_Elevation < 0f;
							float num7 = math.select(overground, num6, flag);
							elevation = m_ApplyStartPoint.m_Elevation + num - num7;
							elevation = math.round(elevation / elevationStep) * elevationStep;
							bool flag2 = elevation < 0f;
							if (overground != num6 && flag2 != flag)
							{
								num7 = math.select(overground, num6, flag2);
								elevation = m_ApplyStartPoint.m_Elevation + num - num7;
								elevation = math.round(elevation / elevationStep) * elevationStep;
								bool flag3 = elevation < 0f;
								if (flag3 != flag2)
								{
									elevation = math.select(0f, 0f - elevationStep, flag3);
								}
							}
							if (elevation > m_LastMouseElevation)
							{
								m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetElevationUpSound);
							}
							else if (elevation < m_LastMouseElevation)
							{
								m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetElevationDownSound);
							}
							m_LastMouseElevation = elevation;
						}
					}
				}
				m_ApplyTimer += Time.deltaTime;
			}
			m_Prefab = netPrefab;
			if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)netPrefab, out PlaceableNetData component3) && component3.m_UndergroundPrefab != Entity.Null && elevation < 0f)
			{
				m_Prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component3.m_UndergroundPrefab);
			}
			if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out component3))
			{
				CheckElevationRange(component3);
				elevation = MathUtils.Clamp(elevation, component3.m_ElevationRange);
			}
			else
			{
				m_LastElevationRange = default(Bounds1);
				elevation = 0f;
			}
			NetData componentData2 = m_PrefabSystem.GetComponentData<NetData>((PrefabBase)m_Prefab);
			m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)m_Prefab, out NetGeometryData component4);
			if (elevation < 0f)
			{
				m_ToolRaycastSystem.collisionMask = CollisionMask.Underground;
				m_ToolRaycastSystem.typeMask = TypeMask.Terrain;
			}
			else
			{
				m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
				m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Water;
			}
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.ElevateOffset | RaycastFlags.SubElements | RaycastFlags.Outside | RaycastFlags.IgnoreSecondary;
			m_ToolRaycastSystem.netLayerMask = componentData2.m_ConnectLayers;
			m_ToolRaycastSystem.rayOffset = new float3(0f, 0f - component4.m_DefaultSurfaceHeight.max - elevation, 0f);
			GetAvailableSnapMask(out var onMask, out var offMask);
			Snap actualSnap = ToolBaseSystem.GetActualSnap(selectedSnap, onMask, offMask);
			if ((actualSnap & (Snap.ExistingGeometry | Snap.NearbyGeometry)) != Snap.None)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.Net;
				if ((component4.m_Flags & Game.Net.GeometryFlags.Marker) != 0)
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Markers;
				}
			}
			if ((actualSnap & Snap.ObjectSurface) != Snap.None)
			{
				m_ToolRaycastSystem.typeMask |= TypeMask.StaticObjects;
				if (m_ToolSystem.actionMode.IsEditor())
				{
					m_ToolRaycastSystem.raycastFlags |= RaycastFlags.Placeholders;
				}
			}
		}
		else
		{
			m_ToolRaycastSystem.collisionMask = (CollisionMask)0;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.ElevateOffset | RaycastFlags.SubElements | RaycastFlags.Outside;
			m_ToolRaycastSystem.typeMask = TypeMask.Terrain | TypeMask.Net | TypeMask.Water;
			m_ToolRaycastSystem.netLayerMask = Layer.None;
			m_ToolRaycastSystem.rayOffset = default(float3);
		}
		if (m_ToolSystem.actionMode.IsEditor())
		{
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements | RaycastFlags.UpgradeIsMain;
		}
	}

	private static bool TryIntersectLineWithPlane(Line3 line, Triangle3 plane, float minDot, out float d)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float3 val = math.normalize(MathUtils.NormalCW(plane));
		if (math.abs(math.dot(val, math.normalize(((Line3)(ref line)).ab))) > minDot)
		{
			float3 val2 = line.a - plane.a;
			d = (0f - math.dot(val, val2)) / math.dot(val, ((Line3)(ref line)).ab);
			return true;
		}
		d = 0f;
		return false;
	}

	private void GetSurfaceHeights(NetPrefab prefab, out float overground, out float underground)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		overground = 0f;
		underground = 0f;
		if (m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)prefab, out NetGeometryData component))
		{
			overground = component.m_DefaultSurfaceHeight.max;
			underground = component.m_DefaultSurfaceHeight.max;
		}
		if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)prefab, out PlaceableNetData component2) && component2.m_UndergroundPrefab != Entity.Null)
		{
			NetPrefab netPrefab = m_PrefabSystem.GetPrefab<NetPrefab>(component2.m_UndergroundPrefab);
			if (m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)netPrefab, out NetGeometryData component3))
			{
				underground = component3.m_DefaultSurfaceHeight.max;
			}
		}
	}

	private void CheckElevationRange(PlaceableNetData placeableNetData)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!((Bounds1)(ref placeableNetData.m_ElevationRange)).Equals(m_LastElevationRange))
		{
			float num = MathUtils.Clamp(0f, placeableNetData.m_ElevationRange);
			if (!MathUtils.Intersect(m_LastElevationRange, num) || !MathUtils.Intersect(placeableNetData.m_ElevationRange, elevation))
			{
				elevation = num;
			}
			m_LastElevationRange = placeableNetData.m_ElevationRange;
		}
	}

	[Preserve]
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		UpdateActions();
		if (m_FocusChanged)
		{
			return inputDeps;
		}
		Mode mode = actualMode;
		if (mode != m_LastActualMode)
		{
			if (m_LastActualMode == Mode.Replace || mode == Mode.Replace)
			{
				m_ControlPoints.Clear();
				m_State = State.Default;
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
		bool flag = m_ForceCancel;
		m_ForceCancel = false;
		if (mode != Mode.Replace)
		{
			inputDeps = UpdateStartEntity(inputDeps);
		}
		if ((Object)(object)m_Prefab != (Object)null)
		{
			NetData componentData = m_PrefabSystem.GetComponentData<NetData>((PrefabBase)m_Prefab);
			m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)m_Prefab, out NetGeometryData component);
			bool laneContainer = m_PrefabSystem.HasComponent<EditorContainerData>(m_Prefab);
			base.requireZones = false;
			base.requireUnderground = underground;
			base.requirePipelines = false;
			base.requireNetArrows = (component.m_Flags & Game.Net.GeometryFlags.Directional) != 0;
			base.requireAreas = AreaTypeMask.None;
			base.requireNet = componentData.m_ConnectLayers | componentData.m_RequiredLayers | component.m_MergeLayers | component.m_IntersectLayers;
			if (actualMode != Mode.Replace)
			{
				base.requireUnderground = elevation < 0f && (elevation <= component.m_ElevationLimit * -3f || (component.m_Flags & Game.Net.GeometryFlags.LoweredIsTunnel) != 0);
				base.requirePipelines = elevation < 0f;
			}
			if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out PlaceableNetData component2))
			{
				if ((component2.m_PlacementFlags & Game.Net.PlacementFlags.OnGround) != Game.Net.PlacementFlags.None && !base.requireUnderground)
				{
					base.requireZones = true;
					base.requireAreas |= AreaTypeMask.Lots;
					if (m_ToolSystem.actionMode.IsEditor())
					{
						base.requireAreas |= AreaTypeMask.Spaces;
					}
				}
				if (mode != Mode.Replace && (component2.m_ElevationRange.max > 0f || (component.m_Flags & Game.Net.GeometryFlags.RequireElevated) != 0) && !base.requireUnderground)
				{
					base.requireNet |= Layer.Waterway;
				}
			}
			UpdateInfoview(m_ToolSystem.actionMode.IsEditor() ? Entity.Null : m_PrefabSystem.GetEntity(m_Prefab));
			GetAvailableSnapMask(component, component2, mode, m_ToolSystem.actionMode.IsEditor(), laneContainer, base.requireUnderground, out m_SnapOnMask, out m_SnapOffMask);
			if (m_State != State.Default && !base.applyAction.enabled && !base.secondaryApplyAction.enabled)
			{
				m_State = State.Default;
			}
			if ((m_ToolRaycastSystem.raycastFlags & (RaycastFlags.DebugDisable | RaycastFlags.UIDisable)) == 0)
			{
				if (actualMode == Mode.Replace)
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
						return Update(inputDeps, fullUpdate: false);
					case State.Applying:
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
						return Update(inputDeps, fullUpdate: false);
					case State.Cancelling:
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
						return Update(inputDeps, fullUpdate: false);
					default:
						return Update(inputDeps, fullUpdate: false);
					}
				}
				if (m_State != State.Cancelling && base.cancelAction.WasPressedThisFrame())
				{
					return Cancel(inputDeps, base.cancelAction.WasReleasedThisFrame());
				}
				if (m_State == State.Cancelling && (flag || base.cancelAction.WasReleasedThisFrame()))
				{
					return Cancel(inputDeps);
				}
				if (m_State != State.Applying && base.applyAction.WasPressedThisFrame())
				{
					return Apply(inputDeps, base.applyAction.WasReleasedThisFrame());
				}
				if (m_State == State.Applying && base.applyAction.WasReleasedThisFrame())
				{
					return Apply(inputDeps);
				}
				return Update(inputDeps, fullUpdate: false);
			}
		}
		else
		{
			base.requireZones = false;
			base.requireUnderground = false;
			base.requirePipelines = false;
			base.requireNetArrows = false;
			base.requireAreas = AreaTypeMask.None;
			base.requireNet = Layer.None;
			UpdateInfoview(Entity.Null);
		}
		if (m_State == State.Applying && (base.applyAction.WasReleasedThisFrame() || base.cancelAction.WasPressedThisFrame()))
		{
			m_State = State.Default;
		}
		else if (m_State == State.Cancelling && (base.secondaryApplyAction.WasReleasedThisFrame() || base.cancelAction.WasPressedThisFrame()))
		{
			m_State = State.Default;
		}
		return Clear(inputDeps);
	}

	private static int GetMaxControlPointCount(Mode mode)
	{
		switch (mode)
		{
		case Mode.Straight:
			return 2;
		case Mode.SimpleCurve:
		case Mode.Continuous:
		case Mode.Grid:
			return 3;
		case Mode.ComplexCurve:
			return 4;
		default:
			return 1;
		}
	}

	public override void GetAvailableSnapMask(out Snap onMask, out Snap offMask)
	{
		if ((Object)(object)m_Prefab != (Object)null)
		{
			m_PrefabSystem.TryGetComponentData<NetGeometryData>((PrefabBase)m_Prefab, out NetGeometryData component);
			m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)m_Prefab, out PlaceableNetData component2);
			bool laneContainer = m_PrefabSystem.HasComponent<EditorContainerData>(m_Prefab);
			bool flag = underground;
			if (actualMode != Mode.Replace)
			{
				flag = elevation < 0f && (elevation <= component.m_ElevationLimit * -3f || (component.m_Flags & Game.Net.GeometryFlags.LoweredIsTunnel) != 0);
			}
			GetAvailableSnapMask(component, component2, actualMode, m_ToolSystem.actionMode.IsEditor(), laneContainer, flag, out onMask, out offMask);
		}
		else
		{
			base.GetAvailableSnapMask(out onMask, out offMask);
		}
	}

	private static void GetAvailableSnapMask(NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, Mode mode, bool editorMode, bool laneContainer, bool underground, out Snap onMask, out Snap offMask)
	{
		if (mode == Mode.Replace)
		{
			onMask = Snap.ExistingGeometry;
			offMask = onMask;
			if ((placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.UpgradeOnly) == 0)
			{
				onMask |= Snap.ContourLines;
				offMask |= Snap.ContourLines;
			}
			if (laneContainer)
			{
				onMask &= ~Snap.ExistingGeometry;
				offMask &= ~Snap.ExistingGeometry;
				onMask |= Snap.NearbyGeometry;
				return;
			}
			if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
			{
				offMask &= ~Snap.ExistingGeometry;
			}
			if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != 0)
			{
				onMask |= Snap.CellLength;
				offMask |= Snap.CellLength;
			}
			return;
		}
		onMask = Snap.ExistingGeometry | Snap.CellLength | Snap.StraightDirection | Snap.ObjectSide | Snap.GuideLines | Snap.ZoneGrid | Snap.ContourLines;
		offMask = onMask;
		if (underground)
		{
			onMask &= ~(Snap.ObjectSide | Snap.ZoneGrid);
		}
		else if ((placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.ShoreLine) != Game.Net.PlacementFlags.None)
		{
			onMask |= Snap.Shoreline;
			offMask |= Snap.Shoreline;
		}
		if (laneContainer)
		{
			onMask &= ~(Snap.CellLength | Snap.ObjectSide);
			offMask &= ~(Snap.CellLength | Snap.ObjectSide);
		}
		else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.Marker) != 0)
		{
			onMask &= ~Snap.ObjectSide;
			offMask &= ~Snap.ObjectSide;
		}
		if (laneContainer)
		{
			onMask &= ~Snap.ExistingGeometry;
			offMask &= ~Snap.ExistingGeometry;
			onMask |= Snap.NearbyGeometry;
			offMask |= Snap.NearbyGeometry;
		}
		else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != 0)
		{
			offMask &= ~Snap.ExistingGeometry;
			onMask |= Snap.NearbyGeometry;
			offMask |= Snap.NearbyGeometry;
		}
		if (editorMode)
		{
			onMask |= Snap.ObjectSurface | Snap.LotGrid | Snap.AutoParent;
			offMask |= Snap.ObjectSurface | Snap.LotGrid | Snap.AutoParent;
		}
	}

	private JobHandle Cancel(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Replace)
		{
			if (m_State != State.Cancelling && m_ControlPoints.Length >= 1)
			{
				m_State = State.Cancelling;
				m_ForceCancel = singleFrameOnly;
				m_AppliedUpgrade.Value = default(AppliedUpgrade);
				return Update(inputDeps, fullUpdate: true);
			}
			m_State = State.Default;
			if (GetAllowApply() && !((EntityQuery)(ref m_EdgeQuery)).IsEmptyIgnoreFilter)
			{
				SetAppliedUpgrade(removing: true);
				base.applyMode = ApplyMode.Apply;
				m_RandomSeed = RandomSeed.Next();
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				if (GetRaycastResult(out var controlPoint))
				{
					controlPoint.m_Elevation = elevation;
					m_ControlPoints.Add(ref controlPoint);
					inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
					inputDeps = FixControlPoints(inputDeps);
					inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
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
					controlPoint2.m_Elevation = elevation;
					m_ControlPoints.Add(ref controlPoint2);
					inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
					inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
				}
				else
				{
					inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
				}
			}
			return inputDeps;
		}
		m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetCancelSound);
		m_State = State.Default;
		base.applyMode = ApplyMode.Clear;
		m_UpgradeStates.Clear();
		if (m_ControlPoints.Length > 0)
		{
			m_ControlPoints.RemoveAt(m_ControlPoints.Length - 1);
		}
		if (GetRaycastResult(out var controlPoint3))
		{
			controlPoint3.m_HitPosition.y += elevation;
			controlPoint3.m_Elevation = elevation;
			if (m_ControlPoints.Length > 0)
			{
				m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint3;
			}
			else
			{
				m_ControlPoints.Add(ref controlPoint3);
			}
			inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
			inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
		}
		else
		{
			inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		return inputDeps;
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
		m_AppliedUpgrade.Value = default(AppliedUpgrade);
		if (m_UpgradeStates.Length < 1 || m_ControlPoints.Length < 4)
		{
			return;
		}
		Entity originalEntity = m_ControlPoints[m_ControlPoints.Length - 3].m_OriginalEntity;
		Entity originalEntity2 = m_ControlPoints[m_ControlPoints.Length - 2].m_OriginalEntity;
		UpgradeState upgradeState = m_UpgradeStates[m_UpgradeStates.Length - 1];
		AppliedUpgrade value = new AppliedUpgrade
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

	protected override bool GetRaycastResult(out ControlPoint controlPoint)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out Entity entity, out RaycastHit hit))
		{
			controlPoint = FilterRaycastResult(entity, hit);
			return controlPoint.m_OriginalEntity != Entity.Null;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	protected override bool GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (GetRaycastResult(out var entity, out var hit, out forceUpdate))
		{
			controlPoint = FilterRaycastResult(entity, hit);
			return controlPoint.m_OriginalEntity != Entity.Null;
		}
		controlPoint = default(ControlPoint);
		return false;
	}

	private ControlPoint FilterRaycastResult(Entity entity, RaycastHit hit)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Replace)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.Node>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Edge>(hit.m_HitEntity) && m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)prefab, out PlaceableNetData component) && (component.m_PlacementFlags & Game.Net.PlacementFlags.NodeUpgrade) == 0)
				{
					entity = hit.m_HitEntity;
				}
			}
			bool flag = false;
			Entity val = entity;
			Owner owner = default(Owner);
			while (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val, ref owner))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Edge>(owner.m_Owner))
				{
					flag = true;
				}
				else if (!m_ToolSystem.actionMode.IsEditor() || flag)
				{
					return default(ControlPoint);
				}
				val = owner.m_Owner;
			}
		}
		return new ControlPoint(entity, hit);
	}

	private JobHandle Clear(JobHandle inputDeps)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.applyMode = ApplyMode.Clear;
		inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		return inputDeps;
	}

	private JobHandle Apply(JobHandle inputDeps, bool singleFrameOnly = false)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		Mode mode = actualMode;
		if (mode == Mode.Replace)
		{
			if (m_State != State.Applying && m_ControlPoints.Length >= 1 && !singleFrameOnly)
			{
				m_State = State.Applying;
				m_AppliedUpgrade.Value = default(AppliedUpgrade);
				return Update(inputDeps, fullUpdate: true);
			}
			m_State = State.Default;
			if (GetAllowApply() && !((EntityQuery)(ref m_EdgeQuery)).IsEmptyIgnoreFilter)
			{
				SetAppliedUpgrade(removing: false);
				base.applyMode = ApplyMode.Apply;
				m_RandomSeed = RandomSeed.Next();
				m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetBuildSound);
				m_ControlPoints.Clear();
				m_UpgradeStates.Clear();
				if (GetRaycastResult(out var controlPoint))
				{
					controlPoint.m_Elevation = elevation;
					m_ControlPoints.Add(ref controlPoint);
					inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
					inputDeps = FixControlPoints(inputDeps);
					inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
				}
				else
				{
					inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
				}
			}
			else
			{
				inputDeps = Update(inputDeps, fullUpdate: true);
			}
			return inputDeps;
		}
		if (m_State != State.Applying && m_ControlPoints.Length >= 1 && !singleFrameOnly)
		{
			m_State = State.Applying;
			m_ApplyStartPoint = m_LastRaycastPoint;
			m_ApplyStartPoint.m_HitPosition.y -= m_ApplyStartPoint.m_Elevation;
			m_ApplyTimer = 0f;
			return Update(inputDeps, fullUpdate: true);
		}
		m_State = State.Default;
		if (m_ControlPoints.Length < mode switch
		{
			Mode.Straight => 2, 
			Mode.SimpleCurve => 3, 
			Mode.ComplexCurve => 4, 
			Mode.Continuous => 3, 
			Mode.Grid => 3, 
			_ => 1, 
		})
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
				controlPoint2.m_HitPosition.y += elevation;
				controlPoint2.m_Elevation = elevation;
				m_ControlPoints.Add(ref controlPoint2);
				inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
				inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
			}
			else
			{
				inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
			}
		}
		else
		{
			EntityQuery val = ((mode == Mode.Point) ? m_NodeQuery : m_EdgeQuery);
			if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
			{
				if (!GetAllowApply())
				{
					m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_PlaceBuildingFailSound);
				}
				else
				{
					base.applyMode = ApplyMode.Apply;
					m_RandomSeed = RandomSeed.Next();
					int num = 0;
					switch (mode)
					{
					case Mode.Continuous:
					{
						ControlPoint controlPoint4 = m_ControlPoints[m_ControlPoints.Length - 2];
						ControlPoint controlPoint5 = m_ControlPoints[m_ControlPoints.Length - 1];
						m_ControlPoints.Clear();
						float num2 = math.distance(((float3)(ref controlPoint4.m_Position)).xz, ((float3)(ref controlPoint5.m_Position)).xz);
						controlPoint4.m_OriginalEntity = Entity.Null;
						controlPoint4.m_Direction = controlPoint5.m_Direction;
						controlPoint4.m_Position = controlPoint5.m_Position;
						ref float3 position = ref controlPoint4.m_Position;
						((float3)(ref position)).xz = ((float3)(ref position)).xz + controlPoint4.m_Direction * num2;
						m_ControlPoints.Add(ref controlPoint5);
						num++;
						m_ControlPoints.Add(ref controlPoint4);
						num++;
						break;
					}
					case Mode.Point:
						m_ControlPoints.Clear();
						break;
					default:
					{
						ControlPoint controlPoint3 = m_ControlPoints[m_ControlPoints.Length - 1];
						m_ControlPoints.Clear();
						m_ControlPoints.Add(ref controlPoint3);
						num++;
						break;
					}
					}
					if (GetRaycastResult(out var controlPoint6))
					{
						controlPoint6.m_HitPosition.y += elevation;
						controlPoint6.m_Elevation = elevation;
						m_ControlPoints.Add(ref controlPoint6);
						inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
						num++;
					}
					if (num >= 1)
					{
						inputDeps = FixControlPoints(inputDeps);
						m_AudioManager.PlayUISound(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetBuildSound);
						inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
					}
					else
					{
						inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
					}
				}
			}
			else
			{
				inputDeps = Update(inputDeps, fullUpdate: true);
			}
		}
		return inputDeps;
	}

	protected override bool GetAllowApply()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		Mode mode = actualMode;
		if (mode != Mode.Replace)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_EdgeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
			NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref m_NodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
			NativeHashSet<Entity> val3 = default(NativeHashSet<Entity>);
			val3._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Edge> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Owner> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Temp> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val4 = val[i];
				NativeArray<Edge> nativeArray = ((ArchetypeChunk)(ref val4)).GetNativeArray<Edge>(ref componentTypeHandle);
				NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref val4)).GetNativeArray<Temp>(ref componentTypeHandle3);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & TempFlags.Create) != 0)
					{
						Edge edge = nativeArray[j];
						val3.Add(edge.m_Start);
						val3.Add(edge.m_End);
					}
				}
			}
			if (mode != Mode.Point && m_ControlPoints.Length > 1 && val3.IsEmpty)
			{
				return false;
			}
			Owner owner = default(Owner);
			for (int k = 0; k < val2.Length; k++)
			{
				ArchetypeChunk val5 = val2[k];
				NativeArray<Entity> nativeArray3 = ((ArchetypeChunk)(ref val5)).GetNativeArray(entityTypeHandle);
				NativeArray<Owner> nativeArray4 = ((ArchetypeChunk)(ref val5)).GetNativeArray<Owner>(ref componentTypeHandle2);
				NativeArray<Temp> nativeArray5 = ((ArchetypeChunk)(ref val5)).GetNativeArray<Temp>(ref componentTypeHandle3);
				for (int l = 0; l < nativeArray3.Length; l++)
				{
					Entity val6 = nativeArray3[l];
					Temp temp = nativeArray5[l];
					if ((temp.m_Flags & (TempFlags.Create | TempFlags.Replace)) != 0 && (temp.m_Flags & TempFlags.Essential) != 0 && !val3.Contains(val6) && ((mode != Mode.Point && m_ControlPoints.Length > 1) || (CollectionUtils.TryGet<Owner>(nativeArray4, l, ref owner) && owner.m_Owner == Entity.Null && (mode == Mode.Point || m_ControlPoints.Length > 1))))
					{
						return false;
					}
				}
			}
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
			return base.GetAllowApply();
		}
		if (m_ControlPoints.Length >= 1)
		{
			return base.GetAllowApply();
		}
		return false;
	}

	private JobHandle Update(JobHandle inputDeps, bool fullUpdate)
	{
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if (actualMode == Mode.Replace)
		{
			if (GetRaycastResult(out ControlPoint controlPoint, out bool forceUpdate))
			{
				controlPoint.m_Elevation = elevation;
				fullUpdate = fullUpdate || forceUpdate;
				if (m_ControlPoints.Length == 0)
				{
					base.applyMode = ApplyMode.Clear;
					m_ControlPoints.Add(ref controlPoint);
					inputDeps = SnapControlPoints(inputDeps, m_State == State.Cancelling);
					inputDeps = UpdateCourse(inputDeps, m_State == State.Cancelling);
				}
				else
				{
					base.applyMode = ApplyMode.None;
					if (fullUpdate || !m_LastRaycastPoint.Equals(controlPoint))
					{
						m_LastRaycastPoint = controlPoint;
						ControlPoint controlPoint2 = m_ControlPoints[m_ControlPoints.Length - 1];
						if (m_State == State.Applying || m_State == State.Cancelling)
						{
							if (m_ControlPoints.Length == 1)
							{
								m_ControlPoints.Add(ref controlPoint);
							}
							else
							{
								m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint;
							}
						}
						else
						{
							m_ControlPoints.Clear();
							m_UpgradeStates.Clear();
							m_ControlPoints.Add(ref controlPoint);
						}
						inputDeps = SnapControlPoints(inputDeps, m_State == State.Cancelling);
						JobHandle.ScheduleBatchedJobs();
						if (!fullUpdate)
						{
							((JobHandle)(ref inputDeps)).Complete();
							ControlPoint other = m_ControlPoints[m_ControlPoints.Length - 1];
							fullUpdate = !controlPoint2.EqualsIgnoreHit(other);
						}
						if (fullUpdate)
						{
							base.applyMode = ApplyMode.Clear;
							inputDeps = UpdateCourse(inputDeps, m_State == State.Cancelling);
						}
					}
				}
			}
			else
			{
				if (m_State == State.Default)
				{
					m_ControlPoints.Clear();
					m_UpgradeStates.Clear();
					m_AppliedUpgrade.Value = default(AppliedUpgrade);
				}
				base.applyMode = ApplyMode.Clear;
				inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
			}
			return inputDeps;
		}
		if (GetRaycastResult(out ControlPoint controlPoint3, out bool forceUpdate2))
		{
			if (m_State == State.Applying)
			{
				controlPoint3 = m_ApplyStartPoint;
			}
			controlPoint3.m_HitPosition.y += elevation;
			controlPoint3.m_Elevation = elevation;
			fullUpdate = fullUpdate || forceUpdate2;
			if (m_ControlPoints.Length == 0)
			{
				base.applyMode = ApplyMode.Clear;
				m_ControlPoints.Add(ref controlPoint3);
				inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
				inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
			}
			else
			{
				base.applyMode = ApplyMode.None;
				if (fullUpdate || !m_LastRaycastPoint.Equals(controlPoint3))
				{
					if (m_ControlPoints.Length >= 2 && math.distance(m_LastRaycastPoint.m_Position, controlPoint3.m_Position) > 0.01f)
					{
						m_AudioManager.PlayUISoundIfNotPlaying(((EntityQuery)(ref m_SoundQuery)).GetSingleton<ToolUXSoundSettingsData>().m_NetExpandSound);
					}
					m_LastRaycastPoint = controlPoint3;
					ControlPoint controlPoint4 = m_ControlPoints[m_ControlPoints.Length - 1];
					m_ControlPoints[m_ControlPoints.Length - 1] = controlPoint3;
					inputDeps = SnapControlPoints(inputDeps, removeUpgrade: false);
					JobHandle.ScheduleBatchedJobs();
					if (!fullUpdate)
					{
						((JobHandle)(ref inputDeps)).Complete();
						ControlPoint other2 = m_ControlPoints[m_ControlPoints.Length - 1];
						fullUpdate = !controlPoint4.EqualsIgnoreHit(other2);
					}
					if (fullUpdate)
					{
						base.applyMode = ApplyMode.Clear;
						inputDeps = UpdateCourse(inputDeps, removeUpgrade: false);
					}
				}
			}
		}
		else
		{
			base.applyMode = ApplyMode.Clear;
			inputDeps = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		}
		return inputDeps;
	}

	private JobHandle UpdateStartEntity(JobHandle inputDeps)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_DefinitionQuery)).IsEmptyIgnoreFilter)
		{
			return JobChunkExtensions.Schedule<UpdateStartEntityJob>(new UpdateStartEntityJob
			{
				m_NetCourseType = InternalCompilerInterface.GetComponentTypeHandle<NetCourse>(ref __TypeHandle.__Game_Tools_NetCourse_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StartEntity = m_StartEntity
			}, m_DefinitionQuery, inputDeps);
		}
		return inputDeps;
	}

	private JobHandle SnapControlPoints(JobHandle inputDeps, bool removeUpgrade)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Entity lanePrefab = Entity.Null;
		Entity serviceUpgradeOwner = Entity.Null;
		if ((Object)(object)m_LanePrefab != (Object)null)
		{
			lanePrefab = m_PrefabSystem.GetEntity(m_LanePrefab);
		}
		if (serviceUpgrade)
		{
			serviceUpgradeOwner = GetUpgradable(m_ToolSystem.selected);
		}
		JobHandle deps;
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle deps2;
		SnapJob obj = new SnapJob
		{
			m_Mode = actualMode,
			m_Snap = GetActualSnap(),
			m_Elevation = elevation,
			m_Prefab = m_PrefabSystem.GetEntity(m_Prefab),
			m_LanePrefab = lanePrefab,
			m_ServiceUpgradeOwner = serviceUpgradeOwner,
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_RemoveUpgrade = removeUpgrade,
			m_LeftHandTraffic = m_CityConfigurationSystem.leftHandTraffic,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Game.Net.Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRoadData = InternalCompilerInterface.GetComponentLookup<RoadData>(ref __TypeHandle.__Game_Prefabs_RoadData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadCompositionData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlaceableData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingExtensionData = InternalCompilerInterface.GetComponentLookup<BuildingExtensionData>(ref __TypeHandle.__Game_Prefabs_BuildingExtensionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AssetStampData = InternalCompilerInterface.GetComponentLookup<AssetStampData>(ref __TypeHandle.__Game_Prefabs_AssetStampData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalConnectData = InternalCompilerInterface.GetComponentLookup<LocalConnectData>(ref __TypeHandle.__Game_Prefabs_LocalConnectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubReplacements = InternalCompilerInterface.GetBufferLookup<SubReplacement>(ref __TypeHandle.__Game_Net_SubReplacement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneCells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionAreas = InternalCompilerInterface.GetBufferLookup<NetCompositionArea>(ref __TypeHandle.__Game_Prefabs_NetCompositionArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Prefabs.SubObject>(ref __TypeHandle.__Game_Prefabs_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
			m_ZoneSearchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies3),
			m_ControlPoints = m_ControlPoints,
			m_SnapLines = m_SnapLines,
			m_UpgradeStates = m_UpgradeStates,
			m_StartEntity = m_StartEntity,
			m_AppliedUpgrade = m_AppliedUpgrade,
			m_LastSnappedEntity = m_LastSnappedEntity,
			m_LastControlPointsAngle = m_LastControlPointsAngle,
			m_SourceUpdateData = m_AudioManager.GetSourceUpdateData(out deps2)
		};
		inputDeps = JobHandle.CombineDependencies(inputDeps, dependencies, dependencies2);
		inputDeps = JobHandle.CombineDependencies(inputDeps, dependencies3, deps);
		inputDeps = JobHandle.CombineDependencies(inputDeps, deps2);
		JobHandle val = IJobExtensions.Schedule<SnapJob>(obj, inputDeps);
		m_TerrainSystem.AddCPUHeightReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val);
		return val;
	}

	private JobHandle FixControlPoints(JobHandle inputDeps)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<FixControlPointsJob>(new FixControlPointsJob
		{
			m_Chunks = chunks,
			m_Mode = mode,
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControlPoints = m_ControlPoints
		}, JobHandle.CombineDependencies(inputDeps, val));
		chunks.Dispose(val2);
		return val2;
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

	private JobHandle UpdateCourse(JobHandle inputDeps, bool removeUpgrade)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = DestroyDefinitions(m_DefinitionQuery, m_ToolOutputBarrier, inputDeps);
		if ((Object)(object)m_Prefab != (Object)null)
		{
			JobHandle deps;
			CreateDefinitionsJob createDefinitionsJob = new CreateDefinitionsJob
			{
				m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
				m_RemoveUpgrade = removeUpgrade,
				m_LefthandTraffic = m_CityConfigurationSystem.leftHandTraffic,
				m_Mode = actualMode,
				m_ParallelCount = math.select(new int2(actualParallelCount, 0), new int2(0, actualParallelCount), m_CityConfigurationSystem.leftHandTraffic),
				m_ParallelOffset = parallelOffset,
				m_RandomSeed = m_RandomSeed,
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
			};
			if ((Object)(object)m_LanePrefab != (Object)null)
			{
				createDefinitionsJob.m_LanePrefab = m_PrefabSystem.GetEntity(m_LanePrefab);
			}
			if (serviceUpgrade)
			{
				createDefinitionsJob.m_ServiceUpgradeOwner = GetUpgradable(m_ToolSystem.selected);
			}
			JobHandle val2 = IJobExtensions.Schedule<CreateDefinitionsJob>(createDefinitionsJob, JobHandle.CombineDependencies(inputDeps, deps));
			m_WaterSystem.AddVelocitySurfaceReader(val2);
			((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		return val;
	}

	public static void CreatePath(ControlPoint startPoint, ControlPoint endPoint, NativeList<PathEdge> path, NetData prefabNetData, PlaceableNetData placeableNetData, ref ComponentLookup<Edge> edgeData, ref ComponentLookup<Game.Net.Node> nodeData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<NetData> prefabNetDatas, ref BufferLookup<ConnectedEdge> connectedEdgeData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		if (math.distance(startPoint.m_Position, endPoint.m_Position) < placeableNetData.m_SnapDistance * 0.5f)
		{
			endPoint = startPoint;
		}
		CompositionFlags.General general = placeableNetData.m_SetUpgradeFlags.m_General | placeableNetData.m_UnsetUpgradeFlags.m_General;
		CompositionFlags.Side side = placeableNetData.m_SetUpgradeFlags.m_Left | placeableNetData.m_SetUpgradeFlags.m_Right | placeableNetData.m_UnsetUpgradeFlags.m_Left | placeableNetData.m_UnsetUpgradeFlags.m_Right;
		if (startPoint.m_OriginalEntity == endPoint.m_OriginalEntity)
		{
			if (edgeData.HasComponent(endPoint.m_OriginalEntity))
			{
				NetData netData = prefabNetDatas[prefabRefData[endPoint.m_OriginalEntity].m_Prefab];
				bool num = (prefabNetData.m_RequiredLayers & netData.m_RequiredLayers) != 0;
				bool flag = !num && (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.IsUpgrade) != Game.Net.PlacementFlags.None && (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.NodeUpgrade) == 0 && ((netData.m_GeneralFlagMask & general) != 0 || (netData.m_SideFlagMask & side) != 0);
				if (num || flag)
				{
					PathEdge pathEdge = new PathEdge
					{
						m_Entity = endPoint.m_OriginalEntity,
						m_Invert = (endPoint.m_CurvePosition < startPoint.m_CurvePosition),
						m_Upgrade = flag
					};
					path.Add(ref pathEdge);
				}
			}
			else
			{
				if (!nodeData.HasComponent(endPoint.m_OriginalEntity))
				{
					return;
				}
				NetData netData2 = prefabNetDatas[prefabRefData[endPoint.m_OriginalEntity].m_Prefab];
				bool flag2 = (prefabNetData.m_RequiredLayers & netData2.m_RequiredLayers) != 0;
				if (flag2)
				{
					DynamicBuffer<ConnectedEdge> val = connectedEdgeData[endPoint.m_OriginalEntity];
					for (int i = 0; i < val.Length; i++)
					{
						Entity edge = val[i].m_Edge;
						Edge edge2 = edgeData[edge];
						if (edge2.m_Start == endPoint.m_OriginalEntity || edge2.m_End == endPoint.m_OriginalEntity)
						{
							flag2 = false;
							break;
						}
					}
				}
				bool flag3 = !flag2 && (placeableNetData.m_PlacementFlags & (Game.Net.PlacementFlags.IsUpgrade | Game.Net.PlacementFlags.NodeUpgrade)) == (Game.Net.PlacementFlags.IsUpgrade | Game.Net.PlacementFlags.NodeUpgrade) && ((netData2.m_GeneralFlagMask & general) != 0 || (netData2.m_SideFlagMask & side) != 0);
				if (flag2 || flag3)
				{
					PathEdge pathEdge = new PathEdge
					{
						m_Entity = endPoint.m_OriginalEntity,
						m_Upgrade = flag3
					};
					path.Add(ref pathEdge);
				}
			}
			return;
		}
		NativeMinHeap<PathItem> val2 = default(NativeMinHeap<PathItem>);
		val2._002Ector(100, (Allocator)2);
		NativeParallelHashMap<Entity, Entity> val3 = default(NativeParallelHashMap<Entity, Entity>);
		val3._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
		Edge edge3 = default(Edge);
		if (edgeData.TryGetComponent(endPoint.m_OriginalEntity, ref edge3))
		{
			NetData netData3 = prefabNetDatas[prefabRefData[endPoint.m_OriginalEntity].m_Prefab];
			bool num2 = (prefabNetData.m_RequiredLayers & netData3.m_RequiredLayers) != 0;
			bool flag4 = !num2 && (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.IsUpgrade) != Game.Net.PlacementFlags.None && ((netData3.m_GeneralFlagMask & general) != 0 || (netData3.m_SideFlagMask & side) != 0);
			if (num2 || flag4)
			{
				val2.Insert(new PathItem
				{
					m_Node = edge3.m_Start,
					m_Edge = endPoint.m_OriginalEntity,
					m_Cost = 0f
				});
				val2.Insert(new PathItem
				{
					m_Node = edge3.m_End,
					m_Edge = endPoint.m_OriginalEntity,
					m_Cost = 0f
				});
			}
		}
		else if (nodeData.HasComponent(endPoint.m_OriginalEntity))
		{
			val2.Insert(new PathItem
			{
				m_Node = endPoint.m_OriginalEntity,
				m_Edge = Entity.Null,
				m_Cost = 0f
			});
		}
		Entity val4 = Entity.Null;
		while (val2.Length != 0)
		{
			PathItem pathItem = val2.Extract();
			if (pathItem.m_Edge == startPoint.m_OriginalEntity)
			{
				val3[pathItem.m_Node] = pathItem.m_Edge;
				val4 = pathItem.m_Node;
				break;
			}
			if (!val3.TryAdd(pathItem.m_Node, pathItem.m_Edge))
			{
				continue;
			}
			if (pathItem.m_Node == startPoint.m_OriginalEntity)
			{
				val4 = pathItem.m_Node;
				break;
			}
			DynamicBuffer<ConnectedEdge> val5 = connectedEdgeData[pathItem.m_Node];
			PrefabRef prefabRef = default(PrefabRef);
			if (pathItem.m_Edge != Entity.Null)
			{
				prefabRef = prefabRefData[pathItem.m_Edge];
			}
			for (int j = 0; j < val5.Length; j++)
			{
				Entity edge4 = val5[j].m_Edge;
				if (edge4 == pathItem.m_Edge)
				{
					continue;
				}
				edge3 = edgeData[edge4];
				Entity val6;
				if (edge3.m_Start == pathItem.m_Node)
				{
					val6 = edge3.m_End;
				}
				else
				{
					if (!(edge3.m_End == pathItem.m_Node))
					{
						continue;
					}
					val6 = edge3.m_Start;
				}
				if (!val3.ContainsKey(val6) || !(edge4 != startPoint.m_OriginalEntity))
				{
					PrefabRef prefabRef2 = prefabRefData[edge4];
					NetData netData4 = prefabNetDatas[prefabRef2.m_Prefab];
					bool num3 = (prefabNetData.m_RequiredLayers & netData4.m_RequiredLayers) != 0;
					bool flag5 = !num3 && (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.IsUpgrade) != Game.Net.PlacementFlags.None && ((netData4.m_GeneralFlagMask & general) != 0 || (netData4.m_SideFlagMask & side) != 0);
					if (num3 || flag5)
					{
						Curve curve = curveData[edge4];
						float num4 = pathItem.m_Cost + curve.m_Length;
						num4 += math.select(0f, 9.9f, prefabRef2.m_Prefab != prefabRef.m_Prefab);
						num4 += math.select(0f, 10f, val5.Length > 2);
						val2.Insert(new PathItem
						{
							m_Node = val6,
							m_Edge = edge4,
							m_Cost = num4
						});
					}
				}
			}
		}
		Entity val7 = default(Entity);
		while (val3.TryGetValue(val4, ref val7) && !(val7 == Entity.Null))
		{
			edge3 = edgeData[val7];
			NetData netData5 = prefabNetDatas[prefabRefData[val7].m_Prefab];
			bool flag6 = edge3.m_End == val4;
			bool flag7 = (prefabNetData.m_RequiredLayers & netData5.m_RequiredLayers) != 0;
			Entity val8 = (flag6 ? edge3.m_Start : edge3.m_End);
			if (flag7 || (placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.NodeUpgrade) == 0)
			{
				PathEdge pathEdge = new PathEdge
				{
					m_Entity = val7,
					m_Invert = flag6,
					m_Upgrade = !flag7
				};
				path.Add(ref pathEdge);
			}
			else
			{
				if (val4 == startPoint.m_OriginalEntity)
				{
					PathEdge pathEdge = new PathEdge
					{
						m_Entity = val4,
						m_Upgrade = true
					};
					path.Add(ref pathEdge);
				}
				if (val7 != endPoint.m_OriginalEntity)
				{
					PathEdge pathEdge = new PathEdge
					{
						m_Entity = val8,
						m_Upgrade = true
					};
					path.Add(ref pathEdge);
				}
			}
			if (!(val7 == endPoint.m_OriginalEntity))
			{
				val4 = val8;
				continue;
			}
			break;
		}
	}

	private static bool IsNearEnd(Entity edge, Curve curve, float3 position, bool invert, ref ComponentLookup<EdgeGeometry> edgeGeometryData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		EdgeGeometry edgeGeometry = default(EdgeGeometry);
		if (edgeGeometryData.TryGetComponent(edge, ref edgeGeometry))
		{
			Bezier4x3 val = MathUtils.Lerp(edgeGeometry.m_Start.m_Left, edgeGeometry.m_Start.m_Right, 0.5f);
			Bezier4x3 val2 = MathUtils.Lerp(edgeGeometry.m_End.m_Left, edgeGeometry.m_End.m_Right, 0.5f);
			float num2 = default(float);
			float num = MathUtils.Distance(((Bezier4x3)(ref val)).xz, ((float3)(ref position)).xz, ref num2);
			float num4 = default(float);
			float num3 = MathUtils.Distance(((Bezier4x3)(ref val2)).xz, ((float3)(ref position)).xz, ref num4);
			float middleLength = edgeGeometry.m_Start.middleLength;
			float middleLength2 = edgeGeometry.m_End.middleLength;
			return math.select(num2 * middleLength, middleLength + num4 * middleLength2, num3 < num) > (middleLength + middleLength2) * 0.5f != invert;
		}
		float num5 = default(float);
		MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref position)).xz, ref num5);
		return num5 > 0.5f;
	}

	public static void AddControlPoints(NativeList<ControlPoint> controlPoints, NativeList<UpgradeState> upgradeStates, NativeReference<AppliedUpgrade> appliedUpgrade, ControlPoint startPoint, ControlPoint endPoint, NativeList<PathEdge> path, Snap snap, bool removeUpgrade, bool leftHandTraffic, bool editorMode, NetGeometryData prefabGeometryData, RoadData prefabRoadData, PlaceableNetData placeableNetData, SubReplacement subReplacement, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Edge> edgeData, ref ComponentLookup<Game.Net.Node> nodeData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Composition> compositionData, ref ComponentLookup<Upgraded> upgradedData, ref ComponentLookup<EdgeGeometry> edgeGeometryData, ref ComponentLookup<PrefabRef> prefabRefData, ref ComponentLookup<NetData> prefabNetData, ref ComponentLookup<NetCompositionData> prefabCompositionData, ref ComponentLookup<RoadComposition> prefabRoadCompositionData, ref BufferLookup<ConnectedEdge> connectedEdgeData, ref BufferLookup<SubReplacement> subReplacementData)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		controlPoints.Add(ref startPoint);
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		CompositionFlags.General general = placeableNetData.m_SetUpgradeFlags.m_General | placeableNetData.m_UnsetUpgradeFlags.m_General;
		CompositionFlags.Side side = placeableNetData.m_SetUpgradeFlags.m_Left | placeableNetData.m_SetUpgradeFlags.m_Right | placeableNetData.m_UnsetUpgradeFlags.m_Left | placeableNetData.m_UnsetUpgradeFlags.m_Right;
		if (path.Length != 0)
		{
			PathEdge pathEdge = path[path.Length - 1];
			if (edgeData.HasComponent(pathEdge.m_Entity))
			{
				NetData netData = prefabNetData[prefabRefData[pathEdge.m_Entity].m_Prefab];
				bool flag2 = (netData.m_GeneralFlagMask & general) != 0;
				bool flag3 = (netData.m_SideFlagMask & side) != 0;
				if (pathEdge.m_Upgrade && !flag3)
				{
					flag = true;
				}
				else
				{
					Composition composition = compositionData[pathEdge.m_Entity];
					Curve curve = curveData[pathEdge.m_Entity];
					NetCompositionData netCompositionData = prefabCompositionData[composition.m_Edge];
					num2 = netCompositionData.m_Width * 0.5f;
					float num3 = default(float);
					MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref endPoint.m_HitPosition)).xz, ref num3);
					float3 val = MathUtils.Position(curve.m_Bezier, num3);
					float3 val2 = MathUtils.Tangent(curve.m_Bezier, num3);
					val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
					num = math.dot(((float3)(ref endPoint.m_HitPosition)).xz - ((float3)(ref val)).xz, MathUtils.Right(((float3)(ref val2)).xz));
					num = math.select(num, 0f - num, pathEdge.m_Invert);
					flag = flag2 && math.abs(num) <= netCompositionData.m_Width * (1f / 6f);
				}
			}
		}
		Owner owner = default(Owner);
		Edge edge = default(Edge);
		Upgraded upgraded = default(Upgraded);
		Composition composition2 = default(Composition);
		NetCompositionData netCompositionData2 = default(NetCompositionData);
		NetCompositionData netCompositionData3 = default(NetCompositionData);
		Edge edge2 = default(Edge);
		Edge edge3 = default(Edge);
		DynamicBuffer<SubReplacement> val5 = default(DynamicBuffer<SubReplacement>);
		RoadComposition roadComposition = default(RoadComposition);
		Game.Net.Node node = default(Game.Net.Node);
		Upgraded upgraded2 = default(Upgraded);
		DynamicBuffer<ConnectedEdge> val8 = default(DynamicBuffer<ConnectedEdge>);
		Composition composition4 = default(Composition);
		NetCompositionData netCompositionData5 = default(NetCompositionData);
		Composition composition5 = default(Composition);
		NetCompositionData netCompositionData6 = default(NetCompositionData);
		for (int i = 0; i < path.Length; i++)
		{
			PathEdge pathEdge2 = path[i];
			bool flag4 = false;
			Entity val3 = pathEdge2.m_Entity;
			while (ownerData.TryGetComponent(val3, ref owner))
			{
				if (edgeData.HasComponent(owner.m_Owner))
				{
					flag4 = true;
				}
				else if (!editorMode || flag4)
				{
					pathEdge2.m_Entity = Entity.Null;
				}
				val3 = owner.m_Owner;
			}
			if (edgeData.TryGetComponent(pathEdge2.m_Entity, ref edge))
			{
				Curve curve2 = curveData[pathEdge2.m_Entity];
				if (pathEdge2.m_Invert)
				{
					CommonUtils.Swap(ref edge.m_Start, ref edge.m_End);
					curve2.m_Bezier = MathUtils.Invert(curve2.m_Bezier);
				}
				float num4 = 0f;
				if (pathEdge2.m_Upgrade)
				{
					UpgradeState upgradeState = new UpgradeState
					{
						m_IsUpgrading = true
					};
					if (upgradedData.TryGetComponent(pathEdge2.m_Entity, ref upgraded))
					{
						upgradeState.m_OldFlags = upgraded.m_Flags;
					}
					if (compositionData.TryGetComponent(pathEdge2.m_Entity, ref composition2))
					{
						if (prefabCompositionData.TryGetComponent(composition2.m_StartNode, ref netCompositionData2))
						{
							if ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0)
							{
								if ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Invert) != 0)
								{
									upgradeState.m_OldFlags.m_Left |= CompositionFlags.Side.AddCrosswalk;
								}
								else
								{
									upgradeState.m_OldFlags.m_Right |= CompositionFlags.Side.AddCrosswalk;
								}
							}
							else if ((netCompositionData2.m_Flags.m_General & CompositionFlags.General.Invert) != 0)
							{
								upgradeState.m_OldFlags.m_Left |= CompositionFlags.Side.RemoveCrosswalk;
							}
							else
							{
								upgradeState.m_OldFlags.m_Right |= CompositionFlags.Side.RemoveCrosswalk;
							}
						}
						if (prefabCompositionData.TryGetComponent(composition2.m_EndNode, ref netCompositionData3))
						{
							if ((netCompositionData3.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0)
							{
								if ((netCompositionData3.m_Flags.m_General & CompositionFlags.General.Invert) != 0)
								{
									upgradeState.m_OldFlags.m_Left |= CompositionFlags.Side.AddCrosswalk;
								}
								else
								{
									upgradeState.m_OldFlags.m_Right |= CompositionFlags.Side.AddCrosswalk;
								}
							}
							else if ((netCompositionData3.m_Flags.m_General & CompositionFlags.General.Invert) != 0)
							{
								upgradeState.m_OldFlags.m_Left |= CompositionFlags.Side.RemoveCrosswalk;
							}
							else
							{
								upgradeState.m_OldFlags.m_Right |= CompositionFlags.Side.RemoveCrosswalk;
							}
						}
					}
					CompositionFlags compositionFlags;
					CompositionFlags compositionFlags2;
					if (num < 0f != pathEdge2.m_Invert)
					{
						compositionFlags = NetCompositionHelpers.InvertCompositionFlags(placeableNetData.m_SetUpgradeFlags);
						compositionFlags2 = NetCompositionHelpers.InvertCompositionFlags(placeableNetData.m_UnsetUpgradeFlags);
						upgradeState.m_SubReplacementSide = SubReplacementSide.Left;
					}
					else
					{
						compositionFlags = placeableNetData.m_SetUpgradeFlags;
						compositionFlags2 = placeableNetData.m_UnsetUpgradeFlags;
						upgradeState.m_SubReplacementSide = SubReplacementSide.Right;
					}
					CompositionFlags.Side side2 = CompositionFlags.Side.ForbidLeftTurn | CompositionFlags.Side.ForbidRightTurn | CompositionFlags.Side.AddCrosswalk | CompositionFlags.Side.RemoveCrosswalk | CompositionFlags.Side.ForbidStraight;
					CompositionFlags.Side side3 = (compositionFlags.m_Left | compositionFlags.m_Right) & side2;
					CompositionFlags.Side side4 = (compositionFlags2.m_Left | compositionFlags2.m_Right) & side2;
					if ((side3 | side4) != 0)
					{
						bool2 val4 = bool2.op_Implicit(false);
						if ((i > 0) & (i < path.Length - 1))
						{
							val4 = bool2.op_Implicit(true);
						}
						else
						{
							if (i == 0)
							{
								bool flag5 = IsNearEnd(pathEdge2.m_Entity, curve2, startPoint.m_HitPosition, pathEdge2.m_Invert, ref edgeGeometryData);
								val4 |= new bool2(!flag5, flag5);
								if (i + 1 < path.Length && edgeData.TryGetComponent(path[i + 1].m_Entity, ref edge2))
								{
									val4 |= new bool2((edge.m_Start == edge2.m_Start) | (edge.m_Start == edge2.m_End), (edge.m_End == edge2.m_Start) | (edge.m_End == edge2.m_End));
								}
							}
							if (i == path.Length - 1)
							{
								bool flag6 = IsNearEnd(pathEdge2.m_Entity, curve2, endPoint.m_HitPosition, pathEdge2.m_Invert, ref edgeGeometryData);
								val4 |= new bool2(!flag6, flag6);
								if (i - 1 >= 0 && edgeData.TryGetComponent(path[i - 1].m_Entity, ref edge3))
								{
									val4 |= new bool2((edge.m_Start == edge3.m_Start) | (edge.m_Start == edge3.m_End), (edge.m_End == edge3.m_Start) | (edge.m_End == edge3.m_End));
								}
							}
						}
						if (pathEdge2.m_Invert != leftHandTraffic)
						{
							val4 = ((bool2)(ref val4)).yx;
						}
						if (val4.x)
						{
							compositionFlags.m_Left |= side3;
							compositionFlags2.m_Left |= side4;
						}
						else
						{
							compositionFlags.m_Left &= ~side3;
							compositionFlags2.m_Left &= ~side4;
						}
						if (val4.y)
						{
							compositionFlags.m_Right |= side3;
							compositionFlags2.m_Right |= side4;
						}
						else
						{
							compositionFlags.m_Right &= ~side3;
							compositionFlags2.m_Right &= ~side4;
						}
					}
					NetData netData2 = prefabNetData[prefabRefData[pathEdge2.m_Entity].m_Prefab];
					bool flag7 = (netData2.m_GeneralFlagMask & general) != 0;
					bool flag8 = (netData2.m_SideFlagMask & side) != 0;
					if (flag || !flag8)
					{
						CompositionFlags.Side side5 = ~(CompositionFlags.Side.PrimaryBeautification | CompositionFlags.Side.SecondaryBeautification | CompositionFlags.Side.WideSidewalk);
						compositionFlags.m_Left &= side5;
						compositionFlags.m_Right &= side5;
						compositionFlags2.m_Left &= side5;
						compositionFlags2.m_Right &= side5;
					}
					if (!flag || !flag7)
					{
						CompositionFlags.General general2 = ~(CompositionFlags.General.WideMedian | CompositionFlags.General.PrimaryMiddleBeautification | CompositionFlags.General.SecondaryMiddleBeautification);
						compositionFlags.m_General &= general2;
						compositionFlags2.m_General &= general2;
					}
					if (flag && flag7)
					{
						upgradeState.m_SubReplacementSide = SubReplacementSide.Middle;
						upgradeState.m_SubReplacementType = subReplacement.m_Type;
					}
					else if (!flag && flag8)
					{
						upgradeState.m_SubReplacementType = subReplacement.m_Type;
					}
					if (upgradeState.m_SubReplacementType != SubReplacementType.None)
					{
						if (!removeUpgrade)
						{
							upgradeState.m_SubReplacementPrefab = subReplacement.m_Prefab;
						}
						bool flag9 = false;
						bool flag10 = subReplacement.m_Prefab != Entity.Null;
						if (subReplacementData.TryGetBuffer(pathEdge2.m_Entity, ref val5))
						{
							for (int j = 0; j < val5.Length; j++)
							{
								SubReplacement subReplacement2 = val5[j];
								if (subReplacement2.m_Side == upgradeState.m_SubReplacementSide && subReplacement2.m_Type == upgradeState.m_SubReplacementType)
								{
									flag9 = true;
									flag10 = subReplacement2.m_Prefab != subReplacement.m_Prefab;
									break;
								}
							}
						}
						if (!(removeUpgrade ? flag9 : flag10))
						{
							upgradeState.m_SubReplacementType = SubReplacementType.None;
						}
					}
					if (removeUpgrade)
					{
						compositionFlags2.m_General = (CompositionFlags.General)0u;
						compositionFlags2.m_Left &= CompositionFlags.Side.RemoveCrosswalk;
						compositionFlags2.m_Right &= CompositionFlags.Side.RemoveCrosswalk;
						upgradeState.m_AddFlags = compositionFlags2;
						upgradeState.m_RemoveFlags = compositionFlags;
					}
					else
					{
						upgradeState.m_AddFlags = compositionFlags;
						upgradeState.m_RemoveFlags = compositionFlags2;
					}
					upgradeStates.Add(ref upgradeState);
				}
				else
				{
					UpgradeState upgradeState2 = default(UpgradeState);
					upgradeStates.Add(ref upgradeState2);
					if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) == 0)
					{
						num4 = num;
						if ((snap & Snap.ExistingGeometry) != Snap.None)
						{
							Composition composition3 = compositionData[pathEdge2.m_Entity];
							NetCompositionData netCompositionData4 = prefabCompositionData[composition3.m_Edge];
							prefabRoadCompositionData.TryGetComponent(composition3.m_Edge, ref roadComposition);
							float num5 = math.abs(netCompositionData4.m_Width - prefabGeometryData.m_DefaultWidth);
							if ((snap & Snap.CellLength) != Snap.None && (roadComposition.m_Flags & prefabRoadData.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) != 0)
							{
								int cellWidth = ZoneUtils.GetCellWidth(netCompositionData4.m_Width);
								int cellWidth2 = ZoneUtils.GetCellWidth(prefabGeometryData.m_DefaultWidth);
								float num6 = math.select(0f, 4f, ((cellWidth ^ cellWidth2) & 1) != 0);
								num5 = (float)math.abs(cellWidth - cellWidth2) * 8f;
								num4 *= (num5 * 0.5f + 3.92f) / num2;
								num4 = MathUtils.Snap(num4, 8f, num6);
								num4 = math.clamp(num4, num5 * -0.5f, num5 * 0.5f);
							}
							else if (num5 > 1.6f)
							{
								num4 *= num5 * 0.74f / num2;
								num4 = MathUtils.Snap(num4, num5 * 0.5f);
								num4 = math.clamp(num4, num5 * -0.5f, num5 * 0.5f);
							}
							else
							{
								num4 = 0f;
							}
						}
					}
				}
				ControlPoint controlPoint = endPoint;
				controlPoint.m_OriginalEntity = edge.m_Start;
				controlPoint.m_Position = curve2.m_Bezier.a;
				ControlPoint controlPoint2 = endPoint;
				controlPoint2.m_OriginalEntity = edge.m_End;
				controlPoint2.m_Position = curve2.m_Bezier.d;
				if (math.abs(num4) >= 0.01f)
				{
					float3 val6 = MathUtils.StartTangent(curve2.m_Bezier);
					float3 val7 = MathUtils.EndTangent(curve2.m_Bezier);
					val6 = MathUtils.Normalize(val6, ((float3)(ref val6)).xz);
					val7 = MathUtils.Normalize(val7, ((float3)(ref val7)).xz);
					ref float3 position = ref controlPoint.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + MathUtils.Right(((float3)(ref val6)).xz) * num4;
					ref float3 position2 = ref controlPoint2.m_Position;
					((float3)(ref position2)).xz = ((float3)(ref position2)).xz + MathUtils.Right(((float3)(ref val7)).xz) * num4;
				}
				controlPoints.Add(ref controlPoint);
				controlPoints.Add(ref controlPoint2);
			}
			else
			{
				if (!nodeData.TryGetComponent(pathEdge2.m_Entity, ref node))
				{
					continue;
				}
				if (pathEdge2.m_Upgrade)
				{
					UpgradeState upgradeState3 = new UpgradeState
					{
						m_IsUpgrading = true
					};
					if (upgradedData.TryGetComponent(pathEdge2.m_Entity, ref upgraded2))
					{
						upgradeState3.m_OldFlags = upgraded2.m_Flags;
					}
					if (connectedEdgeData.TryGetBuffer(pathEdge2.m_Entity, ref val8))
					{
						CompositionFlags compositionFlags3 = default(CompositionFlags);
						for (int k = 0; k < val8.Length; k++)
						{
							Entity edge4 = val8[k].m_Edge;
							edge = edgeData[edge4];
							if (edge.m_Start == pathEdge2.m_Entity)
							{
								if (compositionData.TryGetComponent(edge4, ref composition4) && prefabCompositionData.TryGetComponent(composition4.m_StartNode, ref netCompositionData5))
								{
									compositionFlags3 |= netCompositionData5.m_Flags;
								}
							}
							else if (edge.m_End == pathEdge2.m_Entity && compositionData.TryGetComponent(edge4, ref composition5) && prefabCompositionData.TryGetComponent(composition5.m_EndNode, ref netCompositionData6))
							{
								compositionFlags3 |= netCompositionData6.m_Flags;
							}
						}
						if ((compositionFlags3.m_General & CompositionFlags.General.TrafficLights) != 0)
						{
							upgradeState3.m_OldFlags.m_General |= CompositionFlags.General.TrafficLights;
						}
						else
						{
							upgradeState3.m_OldFlags.m_General |= CompositionFlags.General.RemoveTrafficLights;
						}
					}
					CompositionFlags setUpgradeFlags = placeableNetData.m_SetUpgradeFlags;
					CompositionFlags unsetUpgradeFlags = placeableNetData.m_UnsetUpgradeFlags;
					if (removeUpgrade)
					{
						unsetUpgradeFlags.m_General &= CompositionFlags.General.RemoveTrafficLights;
						unsetUpgradeFlags.m_Left = (CompositionFlags.Side)0u;
						unsetUpgradeFlags.m_Right = (CompositionFlags.Side)0u;
						upgradeState3.m_AddFlags = unsetUpgradeFlags;
						upgradeState3.m_RemoveFlags = setUpgradeFlags;
					}
					else
					{
						upgradeState3.m_AddFlags = setUpgradeFlags;
						upgradeState3.m_RemoveFlags = unsetUpgradeFlags;
					}
					upgradeStates.Add(ref upgradeState3);
				}
				else
				{
					UpgradeState upgradeState2 = default(UpgradeState);
					upgradeStates.Add(ref upgradeState2);
				}
				ControlPoint controlPoint3 = endPoint;
				controlPoint3.m_OriginalEntity = pathEdge2.m_Entity;
				controlPoint3.m_Position = node.m_Position;
				controlPoints.Add(ref controlPoint3);
				controlPoints.Add(ref controlPoint3);
			}
		}
		controlPoints.Add(ref endPoint);
		AppliedUpgrade value = appliedUpgrade.Value;
		if (value.m_Entity != Entity.Null)
		{
			if (upgradeStates.Length != 1 || path[path.Length - 1].m_Entity != value.m_Entity || upgradeStates[0].m_AddFlags != value.m_Flags || upgradeStates[0].m_SubReplacementSide != value.m_SubReplacementSide || (subReplacement.m_Type != value.m_SubReplacementType && value.m_SubReplacementType != SubReplacementType.None) || (subReplacement.m_Prefab != value.m_SubReplacementPrefab && value.m_SubReplacementPrefab != Entity.Null))
			{
				appliedUpgrade.Value = default(AppliedUpgrade);
				return;
			}
			UpgradeState upgradeState4 = upgradeStates[0];
			upgradeState4.m_SkipFlags = true;
			upgradeStates[0] = upgradeState4;
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
		base.OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public NetToolSystem()
	{
	}
}
