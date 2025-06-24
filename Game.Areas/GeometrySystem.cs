using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
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
public class GeometrySystem : GameSystemBase
{
	[BurstCompile]
	private struct TriangulateAreasJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Space> m_SpaceData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<TerrainAreaData> m_PrefabTerrainAreaData;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Area> m_AreaData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Geometry> m_GeometryData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Node> m_Nodes;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public bool m_Loaded;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public NativeList<Entity> m_Buildings;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			Area area = m_AreaData[val];
			DynamicBuffer<Node> nodes = m_Nodes[val];
			DynamicBuffer<Triangle> triangles = m_Triangles[val];
			if ((area.m_Flags & AreaFlags.Slave) != 0 && m_UpdatedData.HasComponent(val))
			{
				GenerateSlaveArea(val, ref area, nodes);
			}
			bool isComplete = (area.m_Flags & AreaFlags.Complete) != 0;
			bool flag = Area(nodes) > 0f;
			NativeArray<float3> val2 = default(NativeArray<float3>);
			val2._002Ector(nodes.Length, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < nodes.Length; i++)
			{
				val2[i] = AreaUtils.GetExpandedNode(nodes, i, -0.1f, isComplete, flag);
			}
			NativeArray<Bounds2> edgeBounds = default(NativeArray<Bounds2>);
			int totalDepth = 0;
			if (nodes.Length > 20)
			{
				BuildEdgeBounds(nodes, val2, out edgeBounds, out totalDepth);
			}
			GeometrySystem.Triangulate<DynamicBuffer<Triangle>>(val2, triangles, edgeBounds, totalDepth, flag);
			GeometrySystem.EqualizeTriangles<DynamicBuffer<Triangle>>(val2, triangles);
			val2.Dispose();
			if (flag)
			{
				area.m_Flags |= AreaFlags.CounterClockwise;
			}
			else
			{
				area.m_Flags &= ~AreaFlags.CounterClockwise;
			}
			if (triangles.Length == 0)
			{
				area.m_Flags |= AreaFlags.NoTriangles;
			}
			else
			{
				area.m_Flags &= ~AreaFlags.NoTriangles;
			}
			m_AreaData[val] = area;
			if (m_GeometryData.HasComponent(val))
			{
				bool flag2 = !m_SpaceData.HasComponent(val);
				bool useWaterHeight = false;
				bool useTriangleHeight = !flag2 || HasBuildingOwner(val);
				float heightOffset = 0f;
				float nodeDistance = 0f;
				float lodBias = 0f;
				PrefabRef prefabRef = m_PrefabRefData[val];
				TerrainAreaData terrainAreaData = default(TerrainAreaData);
				if (m_PrefabTerrainAreaData.TryGetComponent(prefabRef.m_Prefab, ref terrainAreaData))
				{
					heightOffset = terrainAreaData.m_HeightOffset;
				}
				AreaGeometryData areaGeometryData = default(AreaGeometryData);
				if (m_PrefabAreaGeometryData.TryGetComponent(prefabRef.m_Prefab, ref areaGeometryData))
				{
					useWaterHeight = (areaGeometryData.m_Flags & GeometryFlags.OnWaterSurface) != 0;
					nodeDistance = AreaUtils.GetMinNodeDistance(areaGeometryData.m_Type);
					lodBias = areaGeometryData.m_LodBias;
				}
				UpdateHeightRange(nodes, triangles, flag2, useWaterHeight, useTriangleHeight, heightOffset);
				m_GeometryData[val] = CalculateGeometry(nodes, triangles, edgeBounds, totalDepth, nodeDistance, lodBias, flag2, useWaterHeight);
			}
			if (edgeBounds.IsCreated)
			{
				edgeBounds.Dispose();
			}
		}

		private void GenerateSlaveArea(Entity entity, ref Area area, DynamicBuffer<Node> nodes)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			nodes.Clear();
			Owner owner = default(Owner);
			Area area2 = default(Area);
			DynamicBuffer<Node> val = default(DynamicBuffer<Node>);
			if (!m_OwnerData.TryGetComponent(entity, ref owner) || !m_AreaData.TryGetComponent(owner.m_Owner, ref area2) || !m_Nodes.TryGetBuffer(owner.m_Owner, ref val) || val.Length < 3)
			{
				return;
			}
			if ((area2.m_Flags & AreaFlags.Complete) != 0)
			{
				area.m_Flags |= AreaFlags.Complete;
			}
			else
			{
				area.m_Flags &= ~AreaFlags.Complete;
			}
			nodes.CopyFrom(val);
			bool isCounterClockwise = Area(nodes) > 0f;
			NativeList<Node> extraNodes = default(NativeList<Node>);
			extraNodes._002Ector(128, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<int2> extraRanges = default(NativeList<int2>);
			extraRanges._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			DynamicBuffer<Game.Objects.SubObject> val2 = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(owner.m_Owner, ref val2))
			{
				for (int i = 0; i < val2.Length; i++)
				{
					Game.Objects.SubObject subObject = val2[i];
					if (m_BuildingData.HasComponent(subObject.m_SubObject) && !m_DeletedData.HasComponent(subObject.m_SubObject))
					{
						AddObjectHole(subObject.m_SubObject, extraNodes, extraRanges, isCounterClockwise);
					}
				}
			}
			for (int j = 0; j < m_Buildings.Length; j++)
			{
				Entity val3 = m_Buildings[j];
				if (!(m_OwnerData[val3].m_Owner != owner.m_Owner))
				{
					AddObjectHole(val3, extraNodes, extraRanges, isCounterClockwise);
				}
			}
			Segment val6 = default(Segment);
			Segment val8 = default(Segment);
			for (int num = extraRanges.Length - 1; num >= 0; num--)
			{
				int2 val4 = extraRanges[num];
				int3 val5 = int3.op_Implicit(-1);
				float num2 = float.MaxValue;
				for (int k = val4.x; k < val4.y; k++)
				{
					Node node = extraNodes[k];
					for (int l = 0; l < nodes.Length; l++)
					{
						Node node2 = nodes[l];
						((Segment)(ref val6))._002Ector(((float3)(ref node.m_Position)).xz, ((float3)(ref node2.m_Position)).xz);
						float num3 = math.distancesq(val6.a, val6.b);
						if (num3 < num2 && CanAddEdge(val6, nodes, extraNodes, extraRanges, num, new int4(-1, l, num, k)))
						{
							num2 = num3;
							((int3)(ref val5))._002Ector(k, -1, l);
						}
					}
					for (int m = 0; m < num; m++)
					{
						int2 val7 = extraRanges[m];
						for (int n = val7.x; n < val7.y; n++)
						{
							Node node3 = extraNodes[n];
							((Segment)(ref val8))._002Ector(((float3)(ref node.m_Position)).xz, ((float3)(ref node3.m_Position)).xz);
							float num4 = math.distancesq(val8.a, val8.b);
							if (num4 < num2 && CanAddEdge(val8, nodes, extraNodes, extraRanges, num, new int4(m, n, num, k)))
							{
								num2 = num4;
								((int3)(ref val5))._002Ector(k, m, n);
							}
						}
					}
				}
				if (val5.x != -1)
				{
					int num5 = 2 + val4.y - val4.x;
					if (val5.y == -1)
					{
						Node node4 = nodes[val5.z];
						int num6 = math.select(val5.z, nodes.Length, val5.z == 0);
						nodes.ResizeUninitialized(nodes.Length + num5);
						for (int num7 = nodes.Length - num5 - 1; num7 >= num6; num7--)
						{
							nodes[num7 + num5] = nodes[num7];
						}
						nodes[num6++] = node4;
						for (int num8 = val5.x; num8 < val4.y; num8++)
						{
							nodes[num6++] = extraNodes[num8];
						}
						for (int num9 = val4.x; num9 <= val5.x; num9++)
						{
							nodes[num6++] = extraNodes[num9];
						}
					}
					else
					{
						int2 val9 = extraRanges[val5.y];
						Node node5 = extraNodes[val5.z];
						int num10 = math.select(val5.z, val9.y, val5.z == val9.x);
						val5.x += num5;
						val4 += num5;
						val9.y += num5;
						extraRanges[val5.y] = val9;
						extraNodes.ResizeUninitialized(val4.y);
						for (int num11 = val4.y - num5 - 1; num11 >= num10; num11--)
						{
							extraNodes[num11 + num5] = extraNodes[num11];
						}
						for (int num12 = val5.y + 1; num12 < num; num12++)
						{
							int num13 = num12;
							extraRanges[num13] += num5;
						}
						extraNodes[num10++] = node5;
						for (int num14 = val5.x; num14 < val4.y; num14++)
						{
							extraNodes[num10++] = extraNodes[num14];
						}
						for (int num15 = val4.x; num15 <= val5.x; num15++)
						{
							extraNodes[num10++] = extraNodes[num15];
						}
					}
				}
			}
			extraNodes.Dispose();
			extraRanges.Dispose();
		}

		private void AddObjectHole(Entity objectEntity, NativeList<Node> extraNodes, NativeList<int2> extraRanges, bool isCounterClockwise)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = m_TransformData[objectEntity];
			PrefabRef prefabRef = m_PrefabRefData[objectEntity];
			ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
			Quad3 val = ObjectUtils.CalculateBaseCorners(transform.m_Position, transform.m_Rotation, objectGeometryData.m_Bounds);
			int2 val2 = int2.op_Implicit(extraNodes.Length);
			if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Circular) != Game.Objects.GeometryFlags.None)
			{
				float num = objectGeometryData.m_Size.x * 0.5f;
				float num2 = (float)Math.PI / 4f;
				num2 = math.select(num2, 0f - num2, isCounterClockwise);
				for (int i = 0; i < 8; i++)
				{
					float num3 = (float)i * num2;
					Node node = new Node(transform.m_Position, float.MinValue);
					node.m_Position.x += math.cos(num3) * num;
					node.m_Position.z += math.sin(num3) * num;
					extraNodes.Add(ref node);
				}
			}
			else if (isCounterClockwise)
			{
				Node node2 = new Node(val.a, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.b, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.c, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.d, float.MinValue);
				extraNodes.Add(ref node2);
			}
			else
			{
				Node node2 = new Node(val.b, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.a, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.d, float.MinValue);
				extraNodes.Add(ref node2);
				node2 = new Node(val.c, float.MinValue);
				extraNodes.Add(ref node2);
			}
			val2.y = extraNodes.Length;
			extraRanges.Add(ref val2);
		}

		private bool CanAddEdge(Segment newEdge, DynamicBuffer<Node> nodes, NativeList<Node> extraNodes, NativeList<int2> extraRanges, int extraRangeLimit, int4 ignoreIndex)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			Node node = nodes[nodes.Length - 1];
			Segment val = default(Segment);
			val.a = ((float3)(ref node.m_Position)).xz;
			float2 val2 = default(float2);
			for (int i = 0; i < nodes.Length; i++)
			{
				node = nodes[i];
				val.b = ((float3)(ref node.m_Position)).xz;
				if (MathUtils.Intersect(val, newEdge, ref val2) && ((ignoreIndex.x != -1) | ((i != ignoreIndex.y) & (math.select(i, nodes.Length, i == 0) - 1 != ignoreIndex.y))))
				{
					return false;
				}
				val.a = val.b;
			}
			for (int j = 0; j <= extraRangeLimit; j++)
			{
				int2 val3 = extraRanges[j];
				node = extraNodes[val3.y - 1];
				val.a = ((float3)(ref node.m_Position)).xz;
				for (int k = val3.x; k < val3.y; k++)
				{
					node = extraNodes[k];
					val.b = ((float3)(ref node.m_Position)).xz;
					if (MathUtils.Intersect(val, newEdge, ref val2) && math.all((((int4)(ref ignoreIndex)).xz != j) | ((k != ((int4)(ref ignoreIndex)).yw) & (math.select(k, val3.y, k == val3.x) - 1 != ((int4)(ref ignoreIndex)).yw))))
					{
						return false;
					}
					val.a = val.b;
				}
			}
			return true;
		}

		private bool HasBuildingOwner(Entity entity)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			while (m_OwnerData.HasComponent(entity))
			{
				entity = m_OwnerData[entity].m_Owner;
				if (m_BuildingData.HasComponent(entity))
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateHeightRange(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, bool useTerrainHeight, bool useWaterHeight, bool useTriangleHeight, float heightOffset)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle triangle = triangles[i];
				Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
				((Bounds1)(ref val))._002Ector(math.min(0f, heightOffset), math.max(0f, heightOffset));
				if (useTerrainHeight)
				{
					triangle.m_HeightRange = GetHeightRange(ref m_TerrainHeightData, triangle2);
					if (triangle.m_HeightRange.min > triangle.m_HeightRange.max)
					{
						triangle.m_HeightRange = val;
					}
					else if (useTriangleHeight)
					{
						ref Bounds1 heightRange = ref triangle.m_HeightRange;
						heightRange |= val;
					}
					if (useWaterHeight)
					{
						ref Bounds1 heightRange2 = ref triangle.m_HeightRange;
						heightRange2 |= WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, triangle2.a) - triangle2.a.y;
						ref Bounds1 heightRange3 = ref triangle.m_HeightRange;
						heightRange3 |= WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, triangle2.b) - triangle2.b.y;
						ref Bounds1 heightRange4 = ref triangle.m_HeightRange;
						heightRange4 |= WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, triangle2.c) - triangle2.c.y;
					}
				}
				else
				{
					triangle.m_HeightRange = val;
				}
				triangles[i] = triangle;
			}
		}

		private static Bounds1 GetHeightRange(ref TerrainHeightData data, Triangle3 triangle3)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			triangle3.a = TerrainUtils.ToHeightmapSpace(ref data, triangle3.a);
			triangle3.b = TerrainUtils.ToHeightmapSpace(ref data, triangle3.b);
			triangle3.c = TerrainUtils.ToHeightmapSpace(ref data, triangle3.c);
			if (triangle3.b.z < triangle3.a.z)
			{
				CommonUtils.Swap(ref triangle3.b, ref triangle3.a);
			}
			if (triangle3.c.z < triangle3.a.z)
			{
				CommonUtils.Swap(ref triangle3.c, ref triangle3.a);
			}
			if (triangle3.c.z < triangle3.b.z)
			{
				CommonUtils.Swap(ref triangle3.c, ref triangle3.b);
			}
			int num = math.max(0, (int)math.floor(triangle3.a.z));
			int num2 = math.min(data.resolution.z - 1, (int)math.ceil(triangle3.c.z));
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			float zFactorAC;
			float zFactorAB;
			float zFactorBC;
			if (num2 >= num)
			{
				zFactorAC = 1f / (triangle3.c.z - triangle3.a.z);
				zFactorAB = 1f / (triangle3.b.z - triangle3.a.z);
				zFactorBC = 1f / (triangle3.c.z - triangle3.b.z);
				(float2 left, float2 right) tuple = GetLeftRight(num);
				float2 val2 = tuple.left;
				float2 val3 = tuple.right;
				float2 val4 = val2;
				float2 val5 = val3;
				float2 val6 = val4;
				float2 val7 = val5;
				float2 val10 = default(float2);
				for (int i = num; i <= num2; i++)
				{
					float2 val8 = val2;
					float2 val9 = val3;
					((float2)(ref val10))._002Ector(math.min(val2.x, val6.x), math.max(val3.x, val7.x));
					if (i < num2)
					{
						(val8, val9) = GetLeftRight(i + 1);
						((float2)(ref val10))._002Ector(math.min(val10.x, val8.x), math.max(val10.x, val9.x));
					}
					int num3 = math.max(0, (int)math.floor(val10.x));
					int num4 = math.min(data.resolution.x - 1, (int)math.ceil(val10.y));
					float num5 = 1f / (val3.x - val2.x);
					int num6 = i * data.resolution.x;
					for (int j = num3; j <= num4; j++)
					{
						float num7 = math.saturate(((float)j - val2.x) * num5);
						float num8 = math.lerp(val2.y, val3.y, num7);
						float num9 = (int)data.heights[num6 + j];
						val |= num9 - num8;
					}
					float2 val11 = val2;
					val5 = val3;
					val6 = val11;
					val7 = val5;
					float2 val12 = val8;
					val5 = val9;
					val2 = val12;
					val3 = val5;
				}
			}
			val.min /= data.scale.y;
			val.max /= data.scale.y;
			return val;
			(float2 left, float2 right) GetLeftRight(float z)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				float num10 = math.saturate((z - triangle3.a.z) * zFactorAC);
				float2 a = math.lerp(((float3)(ref triangle3.a)).xy, ((float3)(ref triangle3.c)).xy, num10);
				float2 b;
				if (z <= triangle3.b.z)
				{
					float num11 = math.saturate((z - triangle3.a.z) * zFactorAB);
					b = math.lerp(((float3)(ref triangle3.a)).xy, ((float3)(ref triangle3.b)).xy, num11);
				}
				else
				{
					float num12 = math.saturate((z - triangle3.b.z) * zFactorBC);
					b = math.lerp(((float3)(ref triangle3.b)).xy, ((float3)(ref triangle3.c)).xy, num12);
				}
				if (b.x < a.x)
				{
					CommonUtils.Swap(ref a, ref b);
				}
				return (left: a, right: b);
			}
		}

		private Geometry CalculateGeometry(DynamicBuffer<Node> nodes, DynamicBuffer<Triangle> triangles, NativeArray<Bounds2> edgeBounds, int totalDepth, float nodeDistance, float lodBias, bool useTerrainHeight, bool useWaterHeight)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			Geometry result = new Geometry
			{
				m_Bounds = 
				{
					min = float3.op_Implicit(float.MaxValue),
					max = float3.op_Implicit(float.MinValue)
				}
			};
			if (triangles.Length != 0)
			{
				float num = -1f;
				for (int i = 0; i < triangles.Length; i++)
				{
					ref Triangle reference = ref triangles.ElementAt(i);
					Triangle3 triangle = AreaUtils.GetTriangle3(nodes, reference);
					ref Bounds3 bounds = ref result.m_Bounds;
					bounds |= MathUtils.Bounds(triangle);
					result.m_SurfaceArea += MathUtils.Area(((Triangle3)(ref triangle)).xz);
					int3 val = math.abs(((int3)(ref reference.m_Indices)).zxy - ((int3)(ref reference.m_Indices)).yzx);
					bool3 val2 = (val == 1) | (val == nodes.Length - 1);
					bool3 val3 = !val2;
					float2 bestMinDistance = float2.op_Implicit(-1f);
					float3 bestPosition = default(float3);
					if (val3.x)
					{
						float3 position = math.lerp(triangle.b, triangle.c, 0.5f);
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position, triangle, nodes, edgeBounds, totalDepth);
					}
					if (val3.y)
					{
						float3 position2 = math.lerp(triangle.c, triangle.a, 0.5f);
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position2, triangle, nodes, edgeBounds, totalDepth);
					}
					if (val3.z)
					{
						float3 position3 = math.lerp(triangle.a, triangle.b, 0.5f);
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position3, triangle, nodes, edgeBounds, totalDepth);
					}
					if (math.all(((bool3)(ref val3)).xy) & val2.z)
					{
						float3 position4 = triangle.c * 0.5f + (triangle.a + triangle.b) * 0.25f;
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position4, triangle, nodes, edgeBounds, totalDepth);
					}
					else if (math.all(((bool3)(ref val3)).yz) & val2.x)
					{
						float3 position5 = triangle.a * 0.5f + (triangle.b + triangle.c) * 0.25f;
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position5, triangle, nodes, edgeBounds, totalDepth);
					}
					else if (math.all(((bool3)(ref val3)).zx) & val2.y)
					{
						float3 position6 = triangle.b * 0.5f + (triangle.c + triangle.a) * 0.25f;
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position6, triangle, nodes, edgeBounds, totalDepth);
					}
					else
					{
						float3 position7 = (triangle.a + triangle.b + triangle.c) * (1f / 3f);
						CheckCenterPositionCandidate(ref bestMinDistance, ref bestPosition, position7, triangle, nodes, edgeBounds, totalDepth);
					}
					float2 val4 = math.sqrt(bestMinDistance) * 4f;
					reference.m_MinLod = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(val4.x, nodeDistance, val4.y)), lodBias);
					if (bestMinDistance.x > num)
					{
						num = bestMinDistance.x;
						result.m_CenterPosition = bestPosition;
					}
				}
			}
			else if (nodes.Length != 0)
			{
				for (int j = 0; j < nodes.Length; j++)
				{
					float3 position8 = nodes[j].m_Position;
					ref Bounds3 bounds2 = ref result.m_Bounds;
					bounds2 |= position8;
					ref float3 centerPosition = ref result.m_CenterPosition;
					centerPosition += position8;
				}
				ref float3 centerPosition2 = ref result.m_CenterPosition;
				centerPosition2 /= (float)nodes.Length;
			}
			if (useTerrainHeight)
			{
				if (useWaterHeight)
				{
					result.m_CenterPosition.y = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, result.m_CenterPosition);
				}
				else
				{
					result.m_CenterPosition.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, result.m_CenterPosition);
				}
			}
			return result;
		}

		private void CheckCenterPositionCandidate(ref float2 bestMinDistance, ref float3 bestPosition, float3 position, Triangle3 triangle, DynamicBuffer<Node> nodes, NativeArray<Bounds2> edgeBounds, int totalDepth)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			float2 val = float2.op_Implicit(float.MaxValue);
			Node node;
			float num8 = default(float);
			if (edgeBounds.IsCreated)
			{
				float num = math.sqrt(math.max(math.distancesq(((float3)(ref position)).xz, ((float3)(ref triangle.a)).xz), math.max(math.distancesq(((float3)(ref position)).xz, ((float3)(ref triangle.b)).xz), math.distancesq(((float3)(ref position)).xz, ((float3)(ref triangle.c)).xz)))) + 0.1f;
				Bounds2 val2 = default(Bounds2);
				((Bounds2)(ref val2))._002Ector(((float3)(ref position)).xz - num, ((float3)(ref position)).xz + num);
				int num2 = 0;
				int num3 = 1;
				int num4 = 0;
				int length = nodes.Length;
				Segment val3 = default(Segment);
				while (num3 > 0)
				{
					if (MathUtils.Intersect(edgeBounds[num2 + num4], val2))
					{
						if (num3 != totalDepth)
						{
							num4 <<= 1;
							num2 += 1 << num3++;
							continue;
						}
						int num5 = num4 * length >> num3;
						int num6 = (num4 + 1) * length >> num3;
						node = nodes[num5++];
						val3.a = ((float3)(ref node.m_Position)).xz;
						for (int i = num5; i <= num6; i++)
						{
							node = nodes[math.select(i, 0, i == length)];
							val3.b = ((float3)(ref node.m_Position)).xz;
							float num7 = MathUtils.DistanceSquared(val3, ((float3)(ref position)).xz, ref num8);
							val.y = math.select(val.y, num7, num7 < val.y);
							val = math.select(val, new float2(num7, val.x), num7 < val.x);
							val3.a = val3.b;
						}
					}
					while ((num4 & 1) != 0)
					{
						num4 >>= 1;
						num2 -= 1 << --num3;
					}
					num4++;
				}
			}
			else
			{
				Segment val4 = default(Segment);
				node = nodes[nodes.Length - 1];
				val4.a = ((float3)(ref node.m_Position)).xz;
				for (int j = 0; j < nodes.Length; j++)
				{
					node = nodes[j];
					val4.b = ((float3)(ref node.m_Position)).xz;
					float num9 = MathUtils.DistanceSquared(val4, ((float3)(ref position)).xz, ref num8);
					val.y = math.select(val.y, num9, num9 < val.y);
					val = math.select(val, new float2(num9, val.x), num9 < val.x);
					val4.a = val4.b;
				}
			}
			if (val.x > bestMinDistance.x)
			{
				bestMinDistance = val;
				bestPosition = position;
			}
		}
	}

	private struct Index
	{
		public int m_NodeIndex;

		public int m_PrevIndex;

		public int m_NextIndex;

		public int m_SkipIndex;
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Space> __Game_Areas_Space_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TerrainAreaData> __Game_Prefabs_TerrainAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		public ComponentLookup<Area> __Game_Areas_Area_RW_ComponentLookup;

		public ComponentLookup<Geometry> __Game_Areas_Geometry_RW_ComponentLookup;

		public BufferLookup<Node> __Game_Areas_Node_RW_BufferLookup;

		public BufferLookup<Triangle> __Game_Areas_Triangle_RW_BufferLookup;

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
			__Game_Areas_Space_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Space>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TerrainAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TerrainAreaData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Areas_Area_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(false);
			__Game_Areas_Geometry_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(false);
			__Game_Areas_Node_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(false);
			__Game_Areas_Triangle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(false);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private EntityQuery m_UpdatedAreasQuery;

	private EntityQuery m_AllAreasQuery;

	private EntityQuery m_CreatedBuildingsQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_UpdatedAreasQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadWrite<Triangle>()
		});
		m_AllAreasQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadWrite<Triangle>()
		});
		m_CreatedBuildingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.Exclude<Temp>()
		});
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

	public void TerrainHeightsReadyAfterLoading()
	{
		m_Loaded = true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery val = (GetLoaded() ? m_AllAreasQuery : m_UpdatedAreasQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter)
		{
			JobHandle val3 = default(JobHandle);
			NativeList<Entity> val2 = ((EntityQuery)(ref val)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
			JobHandle val4 = default(JobHandle);
			NativeList<Entity> buildings = ((EntityQuery)(ref m_CreatedBuildingsQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			JobHandle deps;
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<TriangulateAreasJob, Entity>(new TriangulateAreasJob
			{
				m_Entities = val2.AsDeferredJobArray(),
				m_Buildings = buildings,
				m_SpaceData = InternalCompilerInterface.GetComponentLookup<Space>(ref __TypeHandle.__Game_Areas_Space_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTerrainAreaData = InternalCompilerInterface.GetComponentLookup<TerrainAreaData>(ref __TypeHandle.__Game_Prefabs_TerrainAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(waitForPending: true),
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GeometryData = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			}, val2, 1, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val4, val3, deps));
			val2.Dispose(val5);
			buildings.Dispose(val5);
			m_TerrainSystem.AddCPUHeightReader(val5);
			m_WaterSystem.AddSurfaceReader(val5);
			((SystemBase)this).Dependency = val5;
		}
	}

	public static void BuildEdgeBounds(DynamicBuffer<Node> nodes, NativeArray<float3> expandedNodes, out NativeArray<Bounds2> edgeBounds, out int totalDepth)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		int num2 = 0;
		int num3;
		for (num3 = nodes.Length; num3 >= 4; num3 >>= 1)
		{
			num += 1 << num2++;
		}
		edgeBounds = new NativeArray<Bounds2>(num, (Allocator)2, (NativeArrayOptions)1);
		num2 = (totalDepth = num2 - 1);
		int num4 = 1 << num2;
		int num5 = num - num4;
		num3 = nodes.Length;
		for (int i = 0; i < num4; i++)
		{
			int num6 = i * num3 >> num2;
			int num7 = (i + 1) * num3 >> num2;
			Node node = nodes[num6];
			float2 xz = ((float3)(ref node.m_Position)).xz;
			float3 val = expandedNodes[num6];
			Bounds2 val2 = MathUtils.Bounds(xz, ((float3)(ref val)).xz);
			num6++;
			for (int j = num6; j <= num7; j++)
			{
				int num8 = math.select(j, 0, j == num3);
				Bounds2 val3 = val2;
				node = nodes[num8];
				val2 = val3 | ((float3)(ref node.m_Position)).xz;
				Bounds2 val4 = val2;
				val = expandedNodes[num8];
				val2 = val4 | ((float3)(ref val)).xz;
			}
			edgeBounds[num5 + i] = val2;
		}
		while (--num2 > 0)
		{
			int num9 = num5;
			num4 = 1 << num2;
			num5 -= num4;
			for (int k = 0; k < num4; k++)
			{
				edgeBounds[num5 + k] = edgeBounds[num9 + (k << 1)] | edgeBounds[num9 + (k << 1) + 1];
			}
		}
	}

	public static void Triangulate<T>(NativeArray<float3> nodes, T triangles, NativeArray<Bounds2> edgeBounds, int totalDepth, bool isCounterClockwise) where T : INativeList<Triangle>
	{
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		if (nodes.Length < 3)
		{
			((INativeList<Triangle>)triangles/*cast due to .constrained prefix*/).Clear();
			return;
		}
		int length = nodes.Length - 2;
		((IIndexable<Triangle>)triangles).Length = length;
		int num = 0;
		NativeArray<Index> indexBuffer = default(NativeArray<Index>);
		indexBuffer._002Ector(nodes.Length, (Allocator)2, (NativeArrayOptions)1);
		if (isCounterClockwise)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				int prevIndex = math.select(i - 1, nodes.Length - 1, i == 0);
				int num2 = math.select(i + 1, 0, i + 1 == nodes.Length);
				indexBuffer[i] = new Index
				{
					m_NodeIndex = i,
					m_PrevIndex = prevIndex,
					m_NextIndex = num2,
					m_SkipIndex = num2
				};
			}
		}
		else
		{
			for (int j = 0; j < nodes.Length; j++)
			{
				int prevIndex2 = math.select(j - 1, nodes.Length - 1, j == 0);
				int num3 = math.select(j + 1, 0, j + 1 == nodes.Length);
				indexBuffer[j] = new Index
				{
					m_NodeIndex = nodes.Length - 1 - j,
					m_PrevIndex = prevIndex2,
					m_NextIndex = num3,
					m_SkipIndex = num3
				};
			}
		}
		int num4 = nodes.Length;
		int num5 = 2 * num4;
		int num6 = num4 - 2;
		int num7 = num4 - 1;
		while (num4 > 2)
		{
			if (0 >= num5--)
			{
				((INativeList<Triangle>)triangles/*cast due to .constrained prefix*/).Clear();
				break;
			}
			Index index = indexBuffer[num7];
			Index index2 = indexBuffer[index.m_NextIndex];
			Index index3 = indexBuffer[index2.m_NextIndex];
			if (Snip(nodes, index, index2, index3, num4, totalDepth, edgeBounds, indexBuffer))
			{
				if (num == ((IIndexable<Triangle>)triangles).Length)
				{
					((INativeList<Triangle>)triangles/*cast due to .constrained prefix*/).Clear();
					break;
				}
				int num8 = num++;
				Triangle triangle = new Triangle(index.m_NodeIndex, index2.m_NodeIndex, index3.m_NodeIndex);
				((INativeList<Triangle>)triangles)[num8] = triangle;
				index.m_SkipIndex = math.select(index.m_SkipIndex, index2.m_SkipIndex, index.m_SkipIndex == index.m_NextIndex);
				index.m_NextIndex = index2.m_NextIndex;
				index3.m_PrevIndex = num7;
				indexBuffer[num7] = index;
				indexBuffer[index2.m_NextIndex] = index3;
				if (num6 != index.m_PrevIndex)
				{
					index2 = indexBuffer[num6];
					index3 = indexBuffer[index.m_PrevIndex];
					index2.m_SkipIndex = index.m_PrevIndex;
					index3.m_SkipIndex = num7;
					indexBuffer[num6] = index2;
					indexBuffer[index.m_PrevIndex] = index3;
				}
				num6 = num7;
				num5 = 2 * --num4;
			}
			num7 = index.m_SkipIndex;
		}
		if (num != ((IIndexable<Triangle>)triangles).Length)
		{
			((INativeList<Triangle>)triangles/*cast due to .constrained prefix*/).Clear();
		}
		indexBuffer.Dispose();
	}

	public static float Area(DynamicBuffer<Node> nodes)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		int length = nodes.Length;
		float num = 0f;
		float3 val = float3.op_Implicit(0f);
		if (nodes.Length != 0)
		{
			val = nodes[0].m_Position;
		}
		int num2 = length - 1;
		int num3 = 0;
		while (num3 < length)
		{
			Node node = nodes[num2];
			float2 val2 = ((float3)(ref node.m_Position)).xz - ((float3)(ref val)).xz;
			node = nodes[num3];
			float2 val3 = val2 * (((float3)(ref node.m_Position)).zx - ((float3)(ref val)).zx);
			num += val3.x - val3.y;
			num2 = num3++;
		}
		return num * 0.5f;
	}

	public static float Area(NativeArray<SubAreaNode> nodes)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		int length = nodes.Length;
		float num = 0f;
		float3 val = float3.op_Implicit(0f);
		if (nodes.Length != 0)
		{
			val = nodes[0].m_Position;
		}
		int num2 = length - 1;
		int num3 = 0;
		while (num3 < length)
		{
			SubAreaNode subAreaNode = nodes[num2];
			float2 val2 = ((float3)(ref subAreaNode.m_Position)).xz - ((float3)(ref val)).xz;
			subAreaNode = nodes[num3];
			float2 val3 = val2 * (((float3)(ref subAreaNode.m_Position)).zx - ((float3)(ref val)).zx);
			num += val3.x - val3.y;
			num2 = num3++;
		}
		return num * 0.5f;
	}

	private static bool Snip(NativeArray<float3> nodes, Index index0, Index index1, Index index2, int nodeCount, int totalDepth, NativeArray<Bounds2> edgeBounds, NativeArray<Index> indexBuffer)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		float3 val = nodes[index0.m_NodeIndex];
		float2 xz = ((float3)(ref val)).xz;
		val = nodes[index1.m_NodeIndex];
		float2 xz2 = ((float3)(ref val)).xz;
		val = nodes[index2.m_NodeIndex];
		Triangle2 val2 = default(Triangle2);
		((Triangle2)(ref val2))._002Ector(xz, xz2, ((float3)(ref val)).xz);
		float2 val3 = val2.b - val2.a;
		float2 val4 = val2.c - val2.a;
		float2 val5 = val3 * ((float2)(ref val4)).yx;
		if (val5.x - val5.y < float.Epsilon)
		{
			return false;
		}
		if (edgeBounds.IsCreated)
		{
			Bounds2 val6 = MathUtils.Bounds(val2);
			int num = 0;
			int num2 = 1;
			int num3 = 0;
			while (num2 > 0)
			{
				if (MathUtils.Intersect(edgeBounds[num + num3], val6))
				{
					if (num2 != totalDepth)
					{
						num3 <<= 1;
						num += 1 << num2++;
						continue;
					}
					int num4 = num3 * nodes.Length >> num2;
					int num5 = (num3 + 1) * nodes.Length >> num2;
					for (int i = num4; i < num5; i++)
					{
						if (i != index0.m_NodeIndex && i != index1.m_NodeIndex && i != index2.m_NodeIndex)
						{
							Triangle2 val7 = val2;
							val = nodes[i];
							if (MathUtils.Intersect(val7, ((float3)(ref val)).xz))
							{
								return false;
							}
						}
					}
				}
				while ((num3 & 1) != 0)
				{
					num3 >>= 1;
					num -= 1 << --num2;
				}
				num3++;
			}
		}
		else
		{
			Index index3 = indexBuffer[index2.m_NextIndex];
			for (int j = 3; j < nodeCount; j++)
			{
				Triangle2 val8 = val2;
				val = nodes[index3.m_NodeIndex];
				if (MathUtils.Intersect(val8, ((float3)(ref val)).xz))
				{
					return false;
				}
				index3 = indexBuffer[index3.m_NextIndex];
			}
		}
		return true;
	}

	public static void EqualizeTriangles<T>(NativeArray<float3> nodes, T triangles) where T : INativeList<Triangle>
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		if (((IIndexable<Triangle>)triangles).Length < 2)
		{
			return;
		}
		NativeParallelHashMap<int2, int2> edgeMap = default(NativeParallelHashMap<int2, int2>);
		edgeMap._002Ector(((IIndexable<Triangle>)triangles).Length * 3, AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < ((IIndexable<Triangle>)triangles).Length; i++)
		{
			Triangle triangle = ((INativeList<Triangle>)triangles)[i];
			bool3 val = ((int3)(ref triangle.m_Indices)).yzx - ((int3)(ref triangle.m_Indices)).zxy > 1;
			if (val.x)
			{
				edgeMap.TryAdd(((int3)(ref triangle.m_Indices)).yz, new int2(i, 0));
			}
			if (val.y)
			{
				edgeMap.TryAdd(((int3)(ref triangle.m_Indices)).zx, new int2(i, 1));
			}
			if (val.z)
			{
				edgeMap.TryAdd(((int3)(ref triangle.m_Indices)).xy, new int2(i, 2));
			}
		}
		int2 index = default(int2);
		int2 index2 = default(int2);
		int2 index3 = default(int2);
		for (int j = 0; j < 1000; j++)
		{
			bool flag = false;
			int num = 0;
			while (num < ((IIndexable<Triangle>)triangles).Length)
			{
				Triangle triangle2 = ((INativeList<Triangle>)triangles)[num];
				bool3 val2 = ((int3)(ref triangle2.m_Indices)).zxy - ((int3)(ref triangle2.m_Indices)).yzx > 1;
				if (val2.x && edgeMap.TryGetValue(((int3)(ref triangle2.m_Indices)).zy, ref index) && TurnEdgeIfNeeded(nodes, triangles, edgeMap, new int2(num, 0), index))
				{
					flag = true;
				}
				else if (val2.y && edgeMap.TryGetValue(((int3)(ref triangle2.m_Indices)).xz, ref index2) && TurnEdgeIfNeeded(nodes, triangles, edgeMap, new int2(num, 1), index2))
				{
					flag = true;
				}
				else if (val2.z && edgeMap.TryGetValue(((int3)(ref triangle2.m_Indices)).yx, ref index3) && TurnEdgeIfNeeded(nodes, triangles, edgeMap, new int2(num, 2), index3))
				{
					flag = true;
				}
				else
				{
					num++;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		edgeMap.Dispose();
	}

	private static bool TurnEdgeIfNeeded<T>(NativeArray<float3> nodes, T triangles, NativeParallelHashMap<int2, int2> edgeMap, int2 index1, int2 index2) where T : INativeList<Triangle>
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		int x = index1.x;
		Triangle triangle = ((INativeList<Triangle>)triangles)[x];
		int x2 = index2.x;
		Triangle triangle2 = ((INativeList<Triangle>)triangles)[x2];
		int2 val = default(int2);
		((int2)(ref val))._002Ector(index1.y, index2.y);
		int2 val2 = default(int2);
		((int2)(ref val2))._002Ector(((int3)(ref triangle.m_Indices))[val.x], ((int3)(ref triangle2.m_Indices))[val.y]);
		int2 val3 = math.select(val + 1, int2.op_Implicit(0), val == 2);
		((int2)(ref val3))._002Ector(((int3)(ref triangle.m_Indices))[val3.x], ((int3)(ref triangle2.m_Indices))[val3.y]);
		Triangle triangle3 = new Triangle(val2.x, val3.x, val2.y);
		Triangle triangle4 = new Triangle(val2.y, val3.y, val2.x);
		float num = math.min(GetEqualizationValue(nodes, triangle), GetEqualizationValue(nodes, triangle2));
		if (math.min(GetEqualizationValue(nodes, triangle3), GetEqualizationValue(nodes, triangle4)) > num)
		{
			bool3 val4 = ((int3)(ref triangle.m_Indices)).yzx - ((int3)(ref triangle.m_Indices)).zxy > 1;
			bool3 val5 = ((int3)(ref triangle2.m_Indices)).yzx - ((int3)(ref triangle2.m_Indices)).zxy > 1;
			bool3 val6 = ((int3)(ref triangle3.m_Indices)).yzx - ((int3)(ref triangle3.m_Indices)).zxy > 1;
			bool3 val7 = ((int3)(ref triangle4.m_Indices)).yzx - ((int3)(ref triangle4.m_Indices)).zxy > 1;
			if (val4.x)
			{
				edgeMap.Remove(((int3)(ref triangle.m_Indices)).yz);
			}
			if (val4.y)
			{
				edgeMap.Remove(((int3)(ref triangle.m_Indices)).zx);
			}
			if (val4.z)
			{
				edgeMap.Remove(((int3)(ref triangle.m_Indices)).xy);
			}
			if (val5.x)
			{
				edgeMap.Remove(((int3)(ref triangle2.m_Indices)).yz);
			}
			if (val5.y)
			{
				edgeMap.Remove(((int3)(ref triangle2.m_Indices)).zx);
			}
			if (val5.z)
			{
				edgeMap.Remove(((int3)(ref triangle2.m_Indices)).xy);
			}
			if (val6.x)
			{
				edgeMap.TryAdd(((int3)(ref triangle3.m_Indices)).yz, new int2(index1.x, 0));
			}
			if (val6.y)
			{
				edgeMap.TryAdd(((int3)(ref triangle3.m_Indices)).zx, new int2(index1.x, 1));
			}
			if (val6.z)
			{
				edgeMap.TryAdd(((int3)(ref triangle3.m_Indices)).xy, new int2(index1.x, 2));
			}
			if (val7.x)
			{
				edgeMap.TryAdd(((int3)(ref triangle4.m_Indices)).yz, new int2(index2.x, 0));
			}
			if (val7.y)
			{
				edgeMap.TryAdd(((int3)(ref triangle4.m_Indices)).zx, new int2(index2.x, 1));
			}
			if (val7.z)
			{
				edgeMap.TryAdd(((int3)(ref triangle4.m_Indices)).xy, new int2(index2.x, 2));
			}
			int x3 = index1.x;
			Triangle triangle5 = triangle3;
			((INativeList<Triangle>)triangles)[x3] = triangle5;
			int x4 = index2.x;
			Triangle triangle6 = triangle4;
			((INativeList<Triangle>)triangles)[x4] = triangle6;
			return true;
		}
		return false;
	}

	private static float GetEqualizationValue(NativeArray<float3> nodes, Triangle triangle)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		float3 val = nodes[triangle.m_Indices.x];
		float2 xz = ((float3)(ref val)).xz;
		val = nodes[triangle.m_Indices.y];
		float2 xz2 = ((float3)(ref val)).xz;
		val = nodes[triangle.m_Indices.z];
		Triangle2 val2 = default(Triangle2);
		((Triangle2)(ref val2))._002Ector(xz, xz2, ((float3)(ref val)).xz);
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(val2.a.x, val2.b.x, val2.c.x);
		float3 val4 = default(float3);
		((float3)(ref val4))._002Ector(val2.a.y, val2.b.y, val2.c.y);
		float3 val5 = val3 - ((float3)(ref val3)).yzx;
		float3 val6 = val4 - ((float3)(ref val4)).yzx;
		float num = math.dot(val3, ((float3)(ref val4)).yzx - ((float3)(ref val4)).zxy) * 0.5f;
		float num2 = math.csum(math.sqrt(val5 * val5 + val6 * val6));
		return num / (num2 * num2);
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
	public GeometrySystem()
	{
	}
}
