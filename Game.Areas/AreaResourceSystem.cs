using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Areas;

[CompilerGenerated]
public class AreaResourceSystem : GameSystemBase, IPostDeserialize
{
	[BurstCompile]
	private struct FindUpdatedAreasWithBrushesJob : IJobParallelForDefer
	{
		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public BufferLookup<WoodResource> m_WoodResourceData;

			public BufferLookup<MapFeatureElement> m_MapFeatureElements;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public ParallelWriter<Entity> m_UpdateBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && (m_WoodResourceData.HasBuffer(item.m_Area) || m_MapFeatureElements.HasBuffer(item.m_Area)))
				{
					Triangle2 triangle = AreaUtils.GetTriangle2(m_Nodes[item.m_Area], m_Triangles[item.m_Area][item.m_Triangle]);
					if (MathUtils.Intersect(m_Bounds, triangle))
					{
						m_UpdateBuffer.Enqueue(item.m_Area);
					}
				}
			}
		}

		[ReadOnly]
		public NativeArray<Brush> m_Brushes;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		[ReadOnly]
		public BufferLookup<WoodResource> m_WoodResourceData;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> m_MapFeatureElements;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		public ParallelWriter<Entity> m_UpdateBuffer;

		public void Execute(int index)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			AreaIterator areaIterator = new AreaIterator
			{
				m_Bounds = ToolUtils.GetBounds(m_Brushes[index]),
				m_WoodResourceData = m_WoodResourceData,
				m_MapFeatureElements = m_MapFeatureElements,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles,
				m_UpdateBuffer = m_UpdateBuffer
			};
			m_AreaTree.Iterate<AreaIterator>(ref areaIterator, 0);
			m_WoodResourceData = areaIterator.m_WoodResourceData;
			m_MapFeatureElements = areaIterator.m_MapFeatureElements;
			m_Nodes = areaIterator.m_Nodes;
			m_Triangles = areaIterator.m_Triangles;
		}
	}

	[BurstCompile]
	private struct FindUpdatedAreasWithBoundsJob : IJobParallelForDefer
	{
		private struct AreaIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public BufferLookup<WoodResource> m_WoodResourceData;

			public BufferLookup<MapFeatureElement> m_MapFeatureElements;

			public BufferLookup<Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public ParallelWriter<Entity> m_UpdateBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem item)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && (m_WoodResourceData.HasBuffer(item.m_Area) || m_MapFeatureElements.HasBuffer(item.m_Area)))
				{
					Triangle2 triangle = AreaUtils.GetTriangle2(m_Nodes[item.m_Area], m_Triangles[item.m_Area][item.m_Triangle]);
					if (MathUtils.Intersect(m_Bounds, triangle))
					{
						m_UpdateBuffer.Enqueue(item.m_Area);
					}
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		[ReadOnly]
		public BufferLookup<WoodResource> m_WoodResourceData;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> m_MapFeatureElements;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		public ParallelWriter<Entity> m_UpdateBuffer;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			AreaIterator areaIterator = new AreaIterator
			{
				m_Bounds = m_Bounds[index],
				m_WoodResourceData = m_WoodResourceData,
				m_MapFeatureElements = m_MapFeatureElements,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles,
				m_UpdateBuffer = m_UpdateBuffer
			};
			m_AreaTree.Iterate<AreaIterator>(ref areaIterator, 0);
			m_WoodResourceData = areaIterator.m_WoodResourceData;
			m_MapFeatureElements = areaIterator.m_MapFeatureElements;
			m_Nodes = areaIterator.m_Nodes;
			m_Triangles = areaIterator.m_Triangles;
		}
	}

	[BurstCompile]
	private struct CollectUpdatedAreasJob : IJob
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct EntityComparer : IComparer<Entity>
		{
			public int Compare(Entity x, Entity y)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return x.Index - y.Index;
			}
		}

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_UpdatedAreaChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_MapTileChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public NativeQueue<Entity> m_UpdateBuffer;

		public NativeList<Entity> m_UpdateList;

		public NativeArray<float2> m_LastCityModifiers;

		public Entity m_City;

		public void Execute()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			int count = m_UpdateBuffer.Count;
			int num = 0;
			ArchetypeChunk val;
			for (int i = 0; i < m_UpdatedAreaChunks.Length; i++)
			{
				int num2 = num;
				val = m_UpdatedAreaChunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			bool flag = UpdateResourceModifiers();
			if (flag)
			{
				for (int j = 0; j < m_MapTileChunks.Length; j++)
				{
					int num3 = num;
					val = m_MapTileChunks[j];
					num = num3 + ((ArchetypeChunk)(ref val)).Count;
				}
			}
			m_UpdateList.ResizeUninitialized(count + num);
			for (int k = 0; k < count; k++)
			{
				m_UpdateList[k] = m_UpdateBuffer.Dequeue();
			}
			for (int l = 0; l < m_UpdatedAreaChunks.Length; l++)
			{
				val = m_UpdatedAreaChunks[l];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				for (int m = 0; m < nativeArray.Length; m++)
				{
					m_UpdateList[count++] = nativeArray[m];
				}
			}
			if (flag)
			{
				for (int n = 0; n < m_MapTileChunks.Length; n++)
				{
					val = m_MapTileChunks[n];
					NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
					for (int num4 = 0; num4 < nativeArray2.Length; num4++)
					{
						m_UpdateList[count++] = nativeArray2[num4];
					}
				}
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdateList, default(EntityComparer));
			Entity val2 = Entity.Null;
			int num5 = 0;
			int num6 = 0;
			while (num5 < m_UpdateList.Length)
			{
				Entity val3 = m_UpdateList[num5++];
				if (val3 != val2)
				{
					m_UpdateList[num6++] = val3;
					val2 = val3;
				}
			}
			if (num6 < m_UpdateList.Length)
			{
				m_UpdateList.RemoveRange(num6, m_UpdateList.Length - num6);
			}
		}

		private bool UpdateResourceModifiers()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			float2 val;
			float2 val2;
			if (m_City != Entity.Null)
			{
				DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
				val = CityUtils.GetModifier(modifiers, CityModifierType.OreResourceAmount);
				val2 = CityUtils.GetModifier(modifiers, CityModifierType.OilResourceAmount);
			}
			else
			{
				val2 = default(float2);
				val = val2;
			}
			float2 val3 = m_LastCityModifiers[0];
			int result;
			if (((float2)(ref val3)).Equals(val))
			{
				val3 = m_LastCityModifiers[1];
				result = ((!((float2)(ref val3)).Equals(val2)) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			m_LastCityModifiers[0] = val;
			m_LastCityModifiers[1] = val2;
			return (byte)result != 0;
		}
	}

	[BurstCompile]
	public struct UpdateAreaResourcesJob : IJobParallelForDefer
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct WoodResourceComparer : IComparer<WoodResource>
		{
			public int Compare(WoodResource x, WoodResource y)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return x.m_Tree.Index - y.m_Tree.Index;
			}
		}

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public bool m_FullUpdate;

		[ReadOnly]
		public NativeArray<Entity> m_UpdateList;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectTree;

		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_NaturalResourceData;

		[ReadOnly]
		public CellMapData<GroundWater> m_GroundWaterResourceData;

		[ReadOnly]
		public ComponentLookup<Geometry> m_GeometryData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Plant> m_PlantData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> m_ExtractorAreaData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Extractor> m_ExtractorData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<WoodResource> m_WoodResources;

		[NativeDisableParallelForRestriction]
		public BufferLookup<MapFeatureElement> m_MapFeatureElements;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public Bounds1 m_BuildableLandMaxSlope;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_UpdateList[index];
			DynamicBuffer<Node> nodes = m_Nodes[val];
			DynamicBuffer<Triangle> triangles = m_Triangles[val];
			DynamicBuffer<CityModifier> cityModifiers = default(DynamicBuffer<CityModifier>);
			if (m_City != Entity.Null)
			{
				cityModifiers = m_CityModifiers[m_City];
			}
			if (m_ExtractorData.HasComponent(val))
			{
				PrefabRef prefabRef = m_PrefabRefData[val];
				ExtractorAreaData extractorAreaData = m_ExtractorAreaData[prefabRef.m_Prefab];
				Extractor extractor = m_ExtractorData[val];
				extractor.m_ResourceAmount = 0f;
				extractor.m_MaxConcentration = 0f;
				switch (extractorAreaData.m_MapFeature)
				{
				case MapFeature.Forest:
					if (m_WoodResources.HasBuffer(val))
					{
						DynamicBuffer<WoodResource> woodResources = m_WoodResources[val];
						CalculateWoodResources(nodes, triangles, ref extractor, woodResources);
					}
					break;
				case MapFeature.FertileLand:
				case MapFeature.Oil:
				case MapFeature.Ore:
				case MapFeature.Fish:
					CalculateNaturalResources(nodes, triangles, cityModifiers, ref extractor, extractorAreaData.m_MapFeature);
					break;
				}
				m_ExtractorData[val] = extractor;
			}
			if (m_MapFeatureElements.HasBuffer(val))
			{
				Geometry geometry = m_GeometryData[val];
				DynamicBuffer<MapFeatureElement> val2 = m_MapFeatureElements[val];
				CollectionUtils.ResizeInitialized<MapFeatureElement>(val2, 9, default(MapFeatureElement));
				WoodIterator woodIterator = new WoodIterator
				{
					m_TreeData = m_TreeData,
					m_PlantData = m_PlantData,
					m_TransformData = m_TransformData,
					m_DamagedData = m_DamagedData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabTreeData = m_PrefabTreeData
				};
				for (int i = 0; i < triangles.Length; i++)
				{
					woodIterator.m_Triangle = AreaUtils.GetTriangle2(nodes, triangles[i]);
					woodIterator.m_Bounds = MathUtils.Bounds(woodIterator.m_Triangle);
					m_ObjectTree.Iterate<WoodIterator>(ref woodIterator, 0);
				}
				float4 resources = float4.zero;
				float4 renewal = float4.zero;
				float groundWater = 0f;
				float buildableArea = 0f;
				CalculateNaturalResources(nodes, triangles, cityModifiers, ref resources, ref renewal, ref groundWater, ref buildableArea);
				val2[0] = new MapFeatureElement(geometry.m_SurfaceArea, 0f);
				val2[3] = new MapFeatureElement(woodIterator.m_WoodAmount, woodIterator.m_GrowthRate);
				val2[2] = new MapFeatureElement(resources.x, renewal.x);
				val2[5] = new MapFeatureElement(resources.y, renewal.y);
				val2[4] = new MapFeatureElement(resources.z, renewal.z);
				val2[8] = new MapFeatureElement(resources.w, renewal.w);
				val2[7] = new MapFeatureElement(groundWater, 0f);
				val2[1] = new MapFeatureElement(buildableArea, 0f);
			}
		}

		private void CalculateWoodResources(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, ref Extractor extractor, DynamicBuffer<WoodResource> woodResources)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			if (m_FullUpdate)
			{
				woodResources.Clear();
				TreeIterator treeIterator = new TreeIterator
				{
					m_TransformData = m_TransformData,
					m_PrefabRefData = m_PrefabRefData,
					m_PrefabTreeData = m_PrefabTreeData,
					m_Buffer = woodResources
				};
				for (int i = 0; i < triangles.Length; i++)
				{
					treeIterator.m_Triangle = AreaUtils.GetTriangle2(nodes, triangles[i]);
					treeIterator.m_Bounds = MathUtils.Bounds(treeIterator.m_Triangle);
					m_ObjectTree.Iterate<TreeIterator>(ref treeIterator, 0);
				}
				NativeSortExtension.Sort<WoodResource, WoodResourceComparer>(woodResources.AsNativeArray(), default(WoodResourceComparer));
				WoodResource woodResource = default(WoodResource);
				int num = 0;
				int num2 = 0;
				while (num < woodResources.Length)
				{
					WoodResource woodResource2 = woodResources[num++];
					if (woodResource2.m_Tree != woodResource.m_Tree)
					{
						woodResources[num2++] = woodResource2;
						woodResource = woodResource2;
					}
				}
				if (num2 < woodResources.Length)
				{
					woodResources.RemoveRange(num2, woodResources.Length - num2);
				}
			}
			Damaged damaged = default(Damaged);
			TreeData treeData = default(TreeData);
			for (int j = 0; j < woodResources.Length; j++)
			{
				Entity tree = woodResources[j].m_Tree;
				Tree tree2 = m_TreeData[tree];
				Plant plant = m_PlantData[tree];
				PrefabRef prefabRef = m_PrefabRefData[tree];
				m_DamagedData.TryGetComponent(tree, ref damaged);
				if (m_PrefabTreeData.TryGetComponent(prefabRef.m_Prefab, ref treeData))
				{
					float num3 = ObjectUtils.CalculateWoodAmount(tree2, plant, damaged, treeData);
					if (num3 > 0f)
					{
						extractor.m_ResourceAmount += num3;
						extractor.m_MaxConcentration = math.max(extractor.m_MaxConcentration, num3 * (1f / treeData.m_WoodAmount));
					}
				}
			}
			extractor.m_MaxConcentration = math.min(extractor.m_MaxConcentration, 1f);
		}

		private void CalculateNaturalResources(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, DynamicBuffer<CityModifier> cityModifiers, ref Extractor extractor, MapFeature mapFeature)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			float2 val = 1f / m_NaturalResourceData.m_CellSize;
			float4 xyxy = ((float2)(ref val)).xyxy;
			val = float2.op_Implicit(m_NaturalResourceData.m_TextureSize) * 0.5f;
			float4 xyxy2 = ((float2)(ref val)).xyxy;
			float num = 1f / (m_NaturalResourceData.m_CellSize.x * m_NaturalResourceData.m_CellSize.y);
			Bounds2 val4 = default(Bounds2);
			float num5 = default(float);
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle2 triangle = AreaUtils.GetTriangle2(nodes, triangles[i]);
				Bounds2 val2 = MathUtils.Bounds(triangle);
				int4 val3 = (int4)math.floor(new float4(val2.min, val2.max) * xyxy + xyxy2);
				val3 = math.clamp(val3, int4.op_Implicit(0), ((int2)(ref m_NaturalResourceData.m_TextureSize)).xyxy - 1);
				float num2 = 0f;
				float num3 = 0f;
				for (int j = val3.y; j <= val3.w; j++)
				{
					val4.min.y = ((float)j - xyxy2.y) * m_NaturalResourceData.m_CellSize.y;
					val4.max.y = val4.min.y + m_NaturalResourceData.m_CellSize.y;
					for (int k = val3.x; k <= val3.z; k++)
					{
						NaturalResourceCell naturalResourceCell = m_NaturalResourceData.m_Buffer[k + m_NaturalResourceData.m_TextureSize.x * j];
						float num4;
						switch (mapFeature)
						{
						case MapFeature.FertileLand:
							num4 = (int)naturalResourceCell.m_Fertility.m_Base;
							num4 -= (float)(int)naturalResourceCell.m_Fertility.m_Used;
							break;
						case MapFeature.Ore:
							num4 = (int)naturalResourceCell.m_Ore.m_Base;
							if (cityModifiers.IsCreated)
							{
								CityUtils.ApplyModifier(ref num4, cityModifiers, CityModifierType.OreResourceAmount);
							}
							num4 -= (float)(int)naturalResourceCell.m_Ore.m_Used;
							break;
						case MapFeature.Oil:
							num4 = (int)naturalResourceCell.m_Oil.m_Base;
							if (cityModifiers.IsCreated)
							{
								CityUtils.ApplyModifier(ref num4, cityModifiers, CityModifierType.OilResourceAmount);
							}
							num4 -= (float)(int)naturalResourceCell.m_Oil.m_Used;
							break;
						case MapFeature.Fish:
							num4 = (int)naturalResourceCell.m_Fish.m_Base;
							num4 -= (float)(int)naturalResourceCell.m_Fish.m_Used;
							break;
						default:
							num4 = 0f;
							break;
						}
						num4 = math.clamp(num4, 0f, 65535f);
						if (num4 != 0f)
						{
							val4.min.x = ((float)k - xyxy2.x) * m_NaturalResourceData.m_CellSize.x;
							val4.max.x = val4.min.x + m_NaturalResourceData.m_CellSize.x;
							if (MathUtils.Intersect(val4, triangle, ref num5))
							{
								num2 += num5 * math.min(num4 * 0.0001f, 1f);
								num3 += num5;
								extractor.m_ResourceAmount += num4 * num5 * num;
							}
						}
					}
				}
				num2 = ((num3 > 0.01f) ? (num2 / num3) : 0f);
				extractor.m_MaxConcentration = math.max(extractor.m_MaxConcentration, num2);
			}
		}

		private void CalculateNaturalResources(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, DynamicBuffer<CityModifier> cityModifiers, ref float4 resources, ref float4 renewal, ref float groundWater, ref float buildableArea)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			((float4)(ref val))._002Ector(800f, 0f, 0f, 800f);
			float2 val2 = 1f / m_NaturalResourceData.m_CellSize;
			float4 xyxy = ((float2)(ref val2)).xyxy;
			val2 = float2.op_Implicit(m_NaturalResourceData.m_TextureSize) * 0.5f;
			float4 xyxy2 = ((float2)(ref val2)).xyxy;
			float num = 1f / (m_NaturalResourceData.m_CellSize.x * m_NaturalResourceData.m_CellSize.y);
			Bounds2 val5 = default(Bounds2);
			float num3 = default(float);
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle2 triangle = AreaUtils.GetTriangle2(nodes, triangles[i]);
				Bounds2 val3 = MathUtils.Bounds(triangle);
				int4 val4 = (int4)math.floor(new float4(val3.min, val3.max) * xyxy + xyxy2);
				val4 = math.clamp(val4, int4.op_Implicit(0), ((int2)(ref m_NaturalResourceData.m_TextureSize)).xyxy - 1);
				for (int j = val4.y; j <= val4.w; j++)
				{
					val5.min.y = ((float)j - xyxy2.y) * m_NaturalResourceData.m_CellSize.y;
					val5.max.y = val5.min.y + m_NaturalResourceData.m_CellSize.y;
					for (int k = val4.x; k <= val4.z; k++)
					{
						NaturalResourceCell naturalResourceCell = m_NaturalResourceData.m_Buffer[k + m_NaturalResourceData.m_TextureSize.x * j];
						GroundWater groundWater2 = m_GroundWaterResourceData.m_Buffer[k + m_GroundWaterResourceData.m_TextureSize.x * j];
						float4 baseResources = naturalResourceCell.GetBaseResources();
						float4 usedResources = naturalResourceCell.GetUsedResources();
						if (cityModifiers.IsCreated)
						{
							CityUtils.ApplyModifier(ref baseResources.y, cityModifiers, CityModifierType.OreResourceAmount);
							CityUtils.ApplyModifier(ref baseResources.z, cityModifiers, CityModifierType.OilResourceAmount);
						}
						float4 val6 = math.clamp(baseResources, float4.op_Implicit(0f), val);
						baseResources -= usedResources;
						baseResources = math.clamp(baseResources, float4.op_Implicit(0f), float4.op_Implicit(65535f));
						val5.min.x = ((float)k - xyxy2.x) * m_NaturalResourceData.m_CellSize.x;
						val5.max.x = val5.min.x + m_NaturalResourceData.m_CellSize.x;
						float3 worldPos = new float3(0.5f * (val5.min.x + val5.max.x), 0f, 0.5f * (val5.min.y + val5.max.y));
						groundWater += groundWater2.m_Amount;
						float num2 = CalculateBuildable(worldPos, m_NaturalResourceData.m_CellSize, m_WaterSurfaceData, m_TerrainHeightData, m_BuildableLandMaxSlope);
						if (MathUtils.Intersect(val5, triangle, ref num3))
						{
							float num4 = num3 * num;
							resources += baseResources * num4;
							renewal += val6 * num4;
							buildableArea += num2 * num3;
						}
					}
				}
			}
		}
	}

	private struct TreeIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds2 m_Bounds;

		public Triangle2 m_Triangle;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<TreeData> m_PrefabTreeData;

		public DynamicBuffer<WoodResource> m_Buffer;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & (BoundsMask.IsTree | BoundsMask.NotOverridden)) != (BoundsMask.IsTree | BoundsMask.NotOverridden))
			{
				return false;
			}
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
			{
				return false;
			}
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Triangle);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & (BoundsMask.IsTree | BoundsMask.NotOverridden)) != (BoundsMask.IsTree | BoundsMask.NotOverridden) || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Triangle))
			{
				return;
			}
			Transform transform = m_TransformData[entity];
			if (MathUtils.Intersect(m_Triangle, ((float3)(ref transform.m_Position)).xz))
			{
				PrefabRef prefabRef = m_PrefabRefData[entity];
				if (m_PrefabTreeData.HasComponent(prefabRef.m_Prefab) && m_PrefabTreeData[prefabRef.m_Prefab].m_WoodAmount >= 1f)
				{
					m_Buffer.Add(new WoodResource(entity));
				}
			}
		}
	}

	private struct WoodIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Bounds2 m_Bounds;

		public Triangle2 m_Triangle;

		public ComponentLookup<Tree> m_TreeData;

		public ComponentLookup<Plant> m_PlantData;

		public ComponentLookup<Transform> m_TransformData;

		public ComponentLookup<Damaged> m_DamagedData;

		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<TreeData> m_PrefabTreeData;

		public float m_WoodAmount;

		public float m_GrowthRate;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & (BoundsMask.IsTree | BoundsMask.NotOverridden)) != (BoundsMask.IsTree | BoundsMask.NotOverridden))
			{
				return false;
			}
			if (!MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds))
			{
				return false;
			}
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Triangle);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if ((bounds.m_Mask & (BoundsMask.IsTree | BoundsMask.NotOverridden)) != (BoundsMask.IsTree | BoundsMask.NotOverridden) || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) || !MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Triangle))
			{
				return;
			}
			Transform transform = m_TransformData[entity];
			if (MathUtils.Intersect(m_Triangle, ((float3)(ref transform.m_Position)).xz))
			{
				Tree tree = m_TreeData[entity];
				Plant plant = m_PlantData[entity];
				PrefabRef prefabRef = m_PrefabRefData[entity];
				Damaged damaged = default(Damaged);
				m_DamagedData.TryGetComponent(entity, ref damaged);
				TreeData treeData = default(TreeData);
				if (m_PrefabTreeData.TryGetComponent(prefabRef.m_Prefab, ref treeData) && treeData.m_WoodAmount >= 1f)
				{
					m_WoodAmount += ObjectUtils.CalculateWoodAmount(tree, plant, damaged, treeData);
					m_GrowthRate += ObjectUtils.CalculateGrowthRate(tree, plant, treeData);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<WoodResource> __Game_Areas_WoodResource_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		public ComponentLookup<Extractor> __Game_Areas_Extractor_RW_ComponentLookup;

		public BufferLookup<WoodResource> __Game_Areas_WoodResource_RW_BufferLookup;

		public BufferLookup<MapFeatureElement> __Game_Areas_MapFeatureElement_RW_BufferLookup;

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
			__Game_Areas_WoodResource_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WoodResource>(true);
			__Game_Areas_MapFeatureElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MapFeatureElement>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Areas_Extractor_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(false);
			__Game_Areas_WoodResource_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<WoodResource>(false);
			__Game_Areas_MapFeatureElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MapFeatureElement>(false);
		}
	}

	private Game.Objects.UpdateCollectSystem m_ObjectUpdateCollectSystem;

	private SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private CitySystem m_CitySystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_UpdatedAreaQuery;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_BrushQuery;

	private NativeArray<float2> m_LastCityModifiers;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_596039173_0;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ObjectUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.UpdateCollectSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Extractor>(),
			ComponentType.ReadOnly<MapFeatureElement>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_UpdatedAreaQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<MapFeatureElement>(),
			ComponentType.Exclude<Native>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Updated>()
		});
		m_BrushQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Brush>(),
			ComponentType.ReadOnly<Applied>()
		});
		m_LastCityModifiers = new NativeArray<float2>(2, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireForUpdate<AreasConfigurationData>();
	}

	public unsafe void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ContextFormat format = ((Context)(ref context)).format;
		if (!((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.FishResource))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MapFeatureElement>() });
			try
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Updated>(val);
			}
			finally
			{
				((IDisposable)(*(EntityQuery*)(&val))/*cast due to .constrained prefix*/).Dispose();
			}
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (m_CitySystem.City != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<CityModifier> buffer = ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true);
			m_LastCityModifiers[0] = CityUtils.GetModifier(buffer, CityModifierType.OreResourceAmount);
			m_LastCityModifiers[1] = CityUtils.GetModifier(buffer, CityModifierType.OilResourceAmount);
		}
		else
		{
			ref NativeArray<float2> reference = ref m_LastCityModifiers;
			float2 val = (m_LastCityModifiers[1] = default(float2));
			reference[0] = val;
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_LastCityModifiers.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((EntityQuery)(ref m_BrushQuery)).IsEmptyIgnoreFilter;
		if (!((EntityQuery)(ref m_UpdatedAreaQuery)).IsEmptyIgnoreFilter || flag || m_ObjectUpdateCollectSystem.isUpdated)
		{
			NativeQueue<Entity> updateBuffer = default(NativeQueue<Entity>);
			updateBuffer._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<Entity> val = default(NativeList<Entity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			ParallelWriter<Entity> updateBuffer2 = updateBuffer.AsParallelWriter();
			if (flag)
			{
				JobHandle val3 = default(JobHandle);
				NativeList<Brush> val2 = ((EntityQuery)(ref m_BrushQuery)).ToComponentDataListAsync<Brush>(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
				JobHandle dependencies;
				JobHandle val4 = IJobParallelForDeferExtensions.Schedule<FindUpdatedAreasWithBrushesJob, Brush>(new FindUpdatedAreasWithBrushesJob
				{
					m_Brushes = val2.AsDeferredJobArray(),
					m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
					m_WoodResourceData = InternalCompilerInterface.GetBufferLookup<WoodResource>(ref __TypeHandle.__Game_Areas_WoodResource_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UpdateBuffer = updateBuffer2
				}, val2, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3, dependencies));
				val2.Dispose(val4);
				m_AreaSearchSystem.AddSearchTreeReader(val4);
				((SystemBase)this).Dependency = val4;
			}
			if (m_ObjectUpdateCollectSystem.isUpdated)
			{
				JobHandle dependencies2;
				NativeList<Bounds2> updatedBounds = m_ObjectUpdateCollectSystem.GetUpdatedBounds(out dependencies2);
				JobHandle dependencies3;
				JobHandle val5 = IJobParallelForDeferExtensions.Schedule<FindUpdatedAreasWithBoundsJob, Bounds2>(new FindUpdatedAreasWithBoundsJob
				{
					m_Bounds = updatedBounds.AsDeferredJobArray(),
					m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies3),
					m_WoodResourceData = InternalCompilerInterface.GetBufferLookup<WoodResource>(ref __TypeHandle.__Game_Areas_WoodResource_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UpdateBuffer = updateBuffer2
				}, updatedBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2, dependencies3));
				m_ObjectUpdateCollectSystem.AddBoundsReader(val5);
				m_AreaSearchSystem.AddSearchTreeReader(val5);
				((SystemBase)this).Dependency = val5;
			}
			JobHandle val6 = default(JobHandle);
			NativeList<ArchetypeChunk> updatedAreaChunks = ((EntityQuery)(ref m_UpdatedAreaQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val6);
			JobHandle val7 = default(JobHandle);
			NativeList<ArchetypeChunk> mapTileChunks = ((EntityQuery)(ref m_MapTileQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val7);
			CollectUpdatedAreasJob collectUpdatedAreasJob = new CollectUpdatedAreasJob
			{
				m_UpdatedAreaChunks = updatedAreaChunks,
				m_MapTileChunks = mapTileChunks,
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdateBuffer = updateBuffer,
				m_UpdateList = val,
				m_LastCityModifiers = m_LastCityModifiers,
				m_City = m_CitySystem.City
			};
			JobHandle dependencies4;
			JobHandle dependencies5;
			JobHandle dependencies6;
			JobHandle deps;
			UpdateAreaResourcesJob obj = new UpdateAreaResourcesJob
			{
				m_City = m_CitySystem.City,
				m_FullUpdate = true,
				m_UpdateList = val.AsDeferredJobArray(),
				m_ObjectTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies4),
				m_NaturalResourceData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies5),
				m_GroundWaterResourceData = m_GroundWaterSystem.GetData(readOnly: true, out dependencies6),
				m_GeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorAreaData = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ExtractorData = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WoodResources = InternalCompilerInterface.GetBufferLookup<WoodResource>(ref __TypeHandle.__Game_Areas_WoodResource_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_MapFeatureElements = InternalCompilerInterface.GetBufferLookup<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_BuildableLandMaxSlope = ((EntityQuery)(ref __query_596039173_0)).GetSingleton<AreasConfigurationData>().m_BuildableLandMaxSlope
			};
			JobHandle val8 = IJobExtensions.Schedule<CollectUpdatedAreasJob>(collectUpdatedAreasJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val6, val7));
			JobHandle val9 = IJobParallelForDeferExtensions.Schedule<UpdateAreaResourcesJob, Entity>(obj, val, 1, JobUtils.CombineDependencies(val8, dependencies4, dependencies5, deps, dependencies6));
			updateBuffer.Dispose(val8);
			val.Dispose(val9);
			updatedAreaChunks.Dispose(val8);
			mapTileChunks.Dispose(val8);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val9);
			m_NaturalResourceSystem.AddReader(val9);
			m_WaterSystem.AddSurfaceReader(val9);
			m_TerrainSystem.AddCPUHeightReader(val9);
			m_GroundWaterSystem.AddReader(val9);
			((SystemBase)this).Dependency = val9;
		}
	}

	public static float CalculateBuildable(float3 worldPos, float2 cellSize, WaterSurfaceData m_WaterSurfaceData, TerrainHeightData terrainHeightData, Bounds1 buildableLandMaxSlope)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		float num = WaterUtils.SampleDepth(ref m_WaterSurfaceData, worldPos);
		float result = 0f;
		if (num < 0.1f)
		{
			float num2 = TerrainUtils.SampleHeight(ref terrainHeightData, worldPos + new float3(-0.5f * cellSize.x, 0f, 0f));
			float num3 = TerrainUtils.SampleHeight(ref terrainHeightData, worldPos + new float3(0.5f * cellSize.x, 0f, 0f));
			float3 val = new float3(cellSize.x, num3 - num2, 0f);
			float num4 = TerrainUtils.SampleHeight(ref terrainHeightData, worldPos + new float3(0f, 0f, 0f - cellSize.y));
			float num5 = TerrainUtils.SampleHeight(ref terrainHeightData, worldPos + new float3(0f, 0f, cellSize.y));
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(0f, num5 - num4, cellSize.y);
			float3 val3 = math.cross(val, val2);
			float3 val4 = math.up();
			float num6 = math.length(math.cross(val3, val4)) / math.dot(val3, val4);
			result = math.saturate(math.unlerp(buildableLandMaxSlope.max, buildableLandMaxSlope.min, math.abs(num6)));
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<AreasConfigurationData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_596039173_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public AreaResourceSystem()
	{
	}
}
