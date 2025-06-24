using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Net;

[CompilerGenerated]
public class GeometrySystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeNodeGeometryJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public bool m_Loaded;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Node> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<NodeGeometry> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity node = nativeArray[i];
				Node node2 = nativeArray2[i];
				NodeGeometry nodeGeometry = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				float2 val = float2.op_Implicit(0f);
				int num = 0;
				float2 val2 = float2.op_Implicit(0f);
				int num2 = 0;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef2 = m_PrefabRefDataFromEntity[value.m_Edge];
					NetGeometryData netGeometryData2 = m_PrefabGeometryData[prefabRef2.m_Prefab];
					if (!m_Loaded && (!flag || m_TempData.HasComponent(value.m_Edge)))
					{
						flag5 |= !m_UpdatedData.HasComponent(value.m_Edge);
					}
					if ((netGeometryData.m_MergeLayers & netGeometryData2.m_MergeLayers) == 0)
					{
						continue;
					}
					Composition composition = m_CompositionDataFromEntity[value.m_Edge];
					NetCompositionData netCompositionData = m_PrefabCompositionData[value.m_End ? composition.m_EndNode : composition.m_StartNode];
					flag2 |= (netCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0;
					flag3 |= (netCompositionData.m_Flags.m_General & CompositionFlags.General.LevelCrossing) != 0;
					flag4 |= (netCompositionData.m_Flags.m_General & CompositionFlags.General.FixedNodeSize) != 0;
					bool flag6 = false;
					if ((netGeometryData2.m_Flags & GeometryFlags.SmoothElevation) == 0)
					{
						NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_Edge];
						flag6 = (netCompositionData2.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) == 0;
						flag6 &= (netCompositionData2.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0 || (netCompositionData2.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0;
						flag6 &= !m_OwnerData.HasComponent(value.m_Edge);
						if (!flag6)
						{
							continue;
						}
					}
					Curve curve = m_CurveDataFromEntity[value.m_Edge];
					if (value.m_End)
					{
						curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					}
					float num3 = math.distance(((float3)(ref curve.m_Bezier.b)).xz, ((float3)(ref curve.m_Bezier.a)).xz);
					if (num3 >= 0.1f)
					{
						if (flag6)
						{
							val2 += new float2(curve.m_Bezier.b.y, 1f) / num3;
							num2++;
						}
						else
						{
							val += new float2(curve.m_Bezier.b.y, 1f) / num3;
							num++;
						}
					}
				}
				if (flag2 || flag3 || flag4)
				{
					nodeGeometry.m_Position = node2.m_Position.y;
					nodeGeometry.m_Flatness = 1f;
					nodeGeometry.m_Offset = 0f;
				}
				else if (num >= 2)
				{
					nodeGeometry.m_Position = math.lerp(node2.m_Position.y, val.x / val.y, 0.5f);
					nodeGeometry.m_Flatness = 0f;
					nodeGeometry.m_Offset = 0f;
				}
				else if (num2 >= 2)
				{
					nodeGeometry.m_Position = node2.m_Position.y;
					nodeGeometry.m_Flatness = 0f;
					nodeGeometry.m_Offset = node2.m_Position.y - math.lerp(node2.m_Position.y, val2.x / val2.y, 0.5f);
				}
				else
				{
					nodeGeometry.m_Position = node2.m_Position.y;
					nodeGeometry.m_Flatness = 0f;
					nodeGeometry.m_Offset = 0f;
				}
				nodeGeometry.m_Bounds.min.x = math.select(0f, 1f, flag5);
				nativeArray3[i] = nodeGeometry;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct CalculateEdgeGeometryJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public Bounds3 m_TerrainBounds;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionDataFromEntity;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> m_PlaceableNetData;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> m_PlaceableObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<NetLaneData> m_NetLaneData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[ReadOnly]
		public BufferLookup<SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> m_PrefabCompositionLanes;

		[ReadOnly]
		public BufferLookup<NetCompositionCrosswalk> m_PrefabCompositionCrosswalks;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0714: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_0718: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_072f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0739: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_073d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_074b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0750: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_075f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0761: Unknown result type (might be due to invalid IL or missing references)
			//IL_0767: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07da: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_0829: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0864: Unknown result type (might be due to invalid IL or missing references)
			//IL_0881: Unknown result type (might be due to invalid IL or missing references)
			//IL_0888: Unknown result type (might be due to invalid IL or missing references)
			//IL_089e: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_092a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0940: Unknown result type (might be due to invalid IL or missing references)
			//IL_094a: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			CalculateOffsets(val, out var startOffsets, out var endOffsets, out var startMiddleRadius, out var endMiddleRadius, out var startLeftTarget, out var startRightTarget, out var endRightTarget, out var endLeftTarget, out var leftStartCurve, out var rightStartCurve, out var leftEndCurve, out var rightEndCurve, out var startTangent, out var endTangent, out var edge, out var curve, out var nodeGeometryStart, out var nodeGeometryEnd, out var edgeCompositionData, out var startCompositionData, out var endCompositionData, out var edgePrefabGeometryData);
			Entity val2 = val;
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(val, ref owner) && m_EdgeData.HasComponent(owner.m_Owner))
			{
				val2 = owner.m_Owner;
			}
			PrefabRef prefabRef = m_PrefabRefDataFromEntity[val2];
			PlaceableNetData placeableNetData = default(PlaceableNetData);
			m_PlaceableNetData.TryGetComponent(prefabRef.m_Prefab, ref placeableNetData);
			if ((placeableNetData.m_PlacementFlags & PlacementFlags.LinkAuxOffsets) != PlacementFlags.None)
			{
				float startMiddleRadius2;
				float endMiddleRadius2;
				float3 startLeftTarget2;
				float3 startRightTarget2;
				float3 endRightTarget2;
				float3 endLeftTarget2;
				Bezier4x3 leftStartCurve2;
				Bezier4x3 rightStartCurve2;
				Bezier4x3 leftEndCurve2;
				Bezier4x3 rightEndCurve2;
				float3 startTangent2;
				float3 endTangent2;
				Edge edge2;
				NodeGeometry nodeGeometryStart2;
				NodeGeometry nodeGeometryEnd2;
				NetCompositionData edgeCompositionData2;
				NetCompositionData startCompositionData2;
				NetCompositionData endCompositionData2;
				NetGeometryData edgePrefabGeometryData2;
				if (val2 != val)
				{
					CalculateOffsets(val2, out var startOffsets2, out var endOffsets2, out startMiddleRadius2, out endMiddleRadius2, out startLeftTarget2, out startRightTarget2, out endRightTarget2, out endLeftTarget2, out leftStartCurve2, out rightStartCurve2, out leftEndCurve2, out rightEndCurve2, out startTangent2, out endTangent2, out edge2, out var curve2, out nodeGeometryStart2, out nodeGeometryEnd2, out edgeCompositionData2, out startCompositionData2, out endCompositionData2, out edgePrefabGeometryData2);
					if (math.dot(((float3)(ref curve.m_Bezier.d)).xz - ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve2.m_Bezier.d)).xz - ((float3)(ref curve2.m_Bezier.a)).xz) < 0f)
					{
						CommonUtils.Swap(ref startOffsets2, ref endOffsets2);
					}
					startOffsets = math.max(startOffsets, startOffsets2);
					endOffsets = math.max(endOffsets, endOffsets2);
				}
				DynamicBuffer<SubNet> val3 = default(DynamicBuffer<SubNet>);
				if (m_SubNets.TryGetBuffer(val2, ref val3))
				{
					for (int i = 0; i < val3.Length; i++)
					{
						Entity subNet = val3[i].m_SubNet;
						if (!(subNet == val) && m_EdgeData.HasComponent(subNet))
						{
							CalculateOffsets(subNet, out var startOffsets3, out var endOffsets3, out endMiddleRadius2, out startMiddleRadius2, out endTangent2, out startTangent2, out endLeftTarget2, out endRightTarget2, out rightEndCurve2, out leftEndCurve2, out rightStartCurve2, out leftStartCurve2, out startRightTarget2, out startLeftTarget2, out edge2, out var curve3, out nodeGeometryEnd2, out nodeGeometryStart2, out endCompositionData2, out startCompositionData2, out edgeCompositionData2, out edgePrefabGeometryData2);
							if (math.dot(((float3)(ref curve.m_Bezier.d)).xz - ((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curve3.m_Bezier.d)).xz - ((float3)(ref curve3.m_Bezier.a)).xz) < 0f)
							{
								CommonUtils.Swap(ref startOffsets3, ref endOffsets3);
							}
							startOffsets = math.max(startOffsets, startOffsets3);
							endOffsets = math.max(endOffsets, endOffsets3);
						}
					}
				}
			}
			EdgeGeometry edgeGeometry = default(EdgeGeometry);
			StartNodeGeometry startNodeGeometry = default(StartNodeGeometry);
			EndNodeGeometry endNodeGeometry = default(EndNodeGeometry);
			startNodeGeometry.m_Geometry.m_MiddleRadius = startMiddleRadius;
			endNodeGeometry.m_Geometry.m_MiddleRadius = endMiddleRadius;
			bool num = math.all(startOffsets == 0f) && (!((float3)(ref startLeftTarget)).Equals(default(float3)) || !((float3)(ref startRightTarget)).Equals(default(float3)));
			bool flag = math.all(endOffsets == 0f) && (!((float3)(ref endLeftTarget)).Equals(default(float3)) || !((float3)(ref endRightTarget)).Equals(default(float3)));
			if (num)
			{
				leftStartCurve.a = math.lerp(leftStartCurve.a, startLeftTarget, 0.5f);
				rightStartCurve.a = math.lerp(rightStartCurve.a, startRightTarget, 0.5f);
			}
			if (flag)
			{
				leftEndCurve.d = math.lerp(leftEndCurve.d, endLeftTarget, 0.5f);
				rightEndCurve.d = math.lerp(rightEndCurve.d, endRightTarget, 0.5f);
			}
			bool num2 = (startCompositionData.m_Flags.m_General & CompositionFlags.General.FixedNodeSize) != 0;
			flag = (endCompositionData.m_Flags.m_General & CompositionFlags.General.FixedNodeSize) != 0;
			if (num2)
			{
				startOffsets = CalculateFixedOffsets(val, edge.m_Start, isStart: true, edgePrefabGeometryData, startTangent, leftStartCurve, rightStartCurve, leftEndCurve, rightEndCurve, out startLeftTarget, out startRightTarget);
			}
			if (flag)
			{
				endOffsets = CalculateFixedOffsets(val, edge.m_End, isStart: false, edgePrefabGeometryData, -endTangent, MathUtils.Invert(rightEndCurve), MathUtils.Invert(leftEndCurve), MathUtils.Invert(rightStartCurve), MathUtils.Invert(leftStartCurve), out endRightTarget, out endLeftTarget);
			}
			startOffsets = math.min(startOffsets, float2.op_Implicit(1.98f));
			endOffsets = math.min(endOffsets, float2.op_Implicit(1.98f));
			float2 val4 = startOffsets + endOffsets;
			if (val4.x > 1.98f)
			{
				startOffsets.x *= 1.98f / val4.x;
				endOffsets.x *= 1.98f / val4.x;
			}
			if (val4.y > 1.98f)
			{
				startOffsets.y *= 1.98f / val4.y;
				endOffsets.y *= 1.98f / val4.y;
			}
			float y = startTangent.y;
			float slopeSteepness = 0f - endTangent.y;
			if ((edgePrefabGeometryData.m_Flags & GeometryFlags.SymmetricalEdges) != 0)
			{
				edgeGeometry.m_Start.m_Left = leftStartCurve;
				edgeGeometry.m_Start.m_Right = rightStartCurve;
				edgeGeometry.m_End.m_Left = leftEndCurve;
				edgeGeometry.m_End.m_Right = rightEndCurve;
				ConformLengths(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_End.m_Left, startOffsets.x, endOffsets.x);
				ConformLengths(ref edgeGeometry.m_Start.m_Right, ref edgeGeometry.m_End.m_Right, startOffsets.y, endOffsets.y);
				LimitHeightDelta(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_Start.m_Right, y, leftStartCurve, rightStartCurve, edgePrefabGeometryData, edgeCompositionData, startCompositionData, nodeGeometryStart.m_Flatness);
				leftEndCurve = MathUtils.Invert(leftEndCurve);
				rightEndCurve = MathUtils.Invert(rightEndCurve);
				edgeGeometry.m_End.m_Left = MathUtils.Invert(edgeGeometry.m_End.m_Left);
				edgeGeometry.m_End.m_Right = MathUtils.Invert(edgeGeometry.m_End.m_Right);
				LimitHeightDelta(ref edgeGeometry.m_End.m_Left, ref edgeGeometry.m_End.m_Right, slopeSteepness, leftEndCurve, rightEndCurve, edgePrefabGeometryData, edgeCompositionData, endCompositionData, nodeGeometryEnd.m_Flatness);
				edgeGeometry.m_End.m_Left = MathUtils.Invert(edgeGeometry.m_End.m_Left);
				edgeGeometry.m_End.m_Right = MathUtils.Invert(edgeGeometry.m_End.m_Right);
			}
			else
			{
				float cutOffset = CalculateCutOffset(leftStartCurve, leftEndCurve, startOffsets.x, endOffsets.x, edgeCompositionData.m_Width);
				float cutOffset2 = CalculateCutOffset(rightStartCurve, rightEndCurve, startOffsets.y, endOffsets.y, edgeCompositionData.m_Width);
				edgeGeometry.m_Start.m_Left = Cut(leftStartCurve, leftEndCurve, startOffsets.x, endOffsets.x, cutOffset);
				edgeGeometry.m_Start.m_Right = Cut(rightStartCurve, rightEndCurve, startOffsets.y, endOffsets.y, cutOffset2);
				LimitHeightDelta(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_Start.m_Right, y, leftStartCurve, rightStartCurve, edgePrefabGeometryData, edgeCompositionData, startCompositionData, nodeGeometryStart.m_Flatness);
				leftStartCurve = MathUtils.Invert(leftStartCurve);
				rightStartCurve = MathUtils.Invert(rightStartCurve);
				leftEndCurve = MathUtils.Invert(leftEndCurve);
				rightEndCurve = MathUtils.Invert(rightEndCurve);
				edgeGeometry.m_End.m_Left = Cut(leftEndCurve, leftStartCurve, endOffsets.x, startOffsets.x, cutOffset);
				edgeGeometry.m_End.m_Right = Cut(rightEndCurve, rightStartCurve, endOffsets.y, startOffsets.y, cutOffset2);
				LimitHeightDelta(ref edgeGeometry.m_End.m_Left, ref edgeGeometry.m_End.m_Right, slopeSteepness, leftEndCurve, rightEndCurve, edgePrefabGeometryData, edgeCompositionData, endCompositionData, nodeGeometryEnd.m_Flatness);
				edgeGeometry.m_End.m_Left = MathUtils.Invert(edgeGeometry.m_End.m_Left);
				edgeGeometry.m_End.m_Right = MathUtils.Invert(edgeGeometry.m_End.m_Right);
			}
			if (num2)
			{
				edgeGeometry.m_Start.m_Left.a = startLeftTarget;
				edgeGeometry.m_Start.m_Right.a = startRightTarget;
				((float3)(ref edgeGeometry.m_Start.m_Left.b)).xz = ((float3)(ref startLeftTarget)).xz + ((float3)(ref startTangent)).xz * math.distance(((float3)(ref edgeGeometry.m_Start.m_Left.a)).xz, ((float3)(ref edgeGeometry.m_Start.m_Left.b)).xz);
				((float3)(ref edgeGeometry.m_Start.m_Right.b)).xz = ((float3)(ref startRightTarget)).xz + ((float3)(ref startTangent)).xz * math.distance(((float3)(ref edgeGeometry.m_Start.m_Right.a)).xz, ((float3)(ref edgeGeometry.m_Start.m_Right.b)).xz);
			}
			if (flag)
			{
				edgeGeometry.m_End.m_Left.d = endLeftTarget;
				edgeGeometry.m_End.m_Right.d = endRightTarget;
				((float3)(ref edgeGeometry.m_End.m_Left.c)).xz = ((float3)(ref endLeftTarget)).xz - ((float3)(ref endTangent)).xz * math.distance(((float3)(ref edgeGeometry.m_End.m_Left.d)).xz, ((float3)(ref edgeGeometry.m_End.m_Left.c)).xz);
				((float3)(ref edgeGeometry.m_End.m_Right.c)).xz = ((float3)(ref endRightTarget)).xz - ((float3)(ref endTangent)).xz * math.distance(((float3)(ref edgeGeometry.m_End.m_Right.d)).xz, ((float3)(ref edgeGeometry.m_End.m_Right.c)).xz);
			}
			if (nodeGeometryStart.m_Bounds.min.x != 0f || nodeGeometryEnd.m_Bounds.min.x != 0f)
			{
				EdgeGeometry edgeGeometry2 = m_EdgeGeometryData[val];
				if (nodeGeometryStart.m_Bounds.min.x != 0f)
				{
					edgeGeometry.m_Start.m_Left.b.y += edgeGeometry2.m_Start.m_Left.a.y - edgeGeometry.m_Start.m_Left.a.y;
					edgeGeometry.m_Start.m_Right.b.y += edgeGeometry2.m_Start.m_Right.a.y - edgeGeometry.m_Start.m_Right.a.y;
					edgeGeometry.m_Start.m_Left.a.y = edgeGeometry2.m_Start.m_Left.a.y;
					edgeGeometry.m_Start.m_Right.a.y = edgeGeometry2.m_Start.m_Right.a.y;
				}
				if (nodeGeometryEnd.m_Bounds.min.x != 0f)
				{
					edgeGeometry.m_End.m_Left.c.y += edgeGeometry2.m_End.m_Left.d.y - edgeGeometry.m_End.m_Left.d.y;
					edgeGeometry.m_End.m_Right.c.y += edgeGeometry2.m_End.m_Right.d.y - edgeGeometry.m_End.m_Right.d.y;
					edgeGeometry.m_End.m_Left.d.y = edgeGeometry2.m_End.m_Left.d.y;
					edgeGeometry.m_End.m_Right.d.y = edgeGeometry2.m_End.m_Right.d.y;
				}
			}
			m_EdgeGeometryData[val] = edgeGeometry;
			m_StartNodeGeometryData[val] = startNodeGeometry;
			m_EndNodeGeometryData[val] = endNodeGeometry;
		}

		public void CalculateOffsets(Entity entity, out float2 startOffsets, out float2 endOffsets, out float startMiddleRadius, out float endMiddleRadius, out float3 startLeftTarget, out float3 startRightTarget, out float3 endRightTarget, out float3 endLeftTarget, out Bezier4x3 leftStartCurve, out Bezier4x3 rightStartCurve, out Bezier4x3 leftEndCurve, out Bezier4x3 rightEndCurve, out float3 startTangent, out float3 endTangent, out Edge edge, out Curve curve, out NodeGeometry nodeGeometryStart, out NodeGeometry nodeGeometryEnd, out NetCompositionData edgeCompositionData, out NetCompositionData startCompositionData, out NetCompositionData endCompositionData, out NetGeometryData edgePrefabGeometryData)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058d: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0614: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0704: Unknown result type (might be due to invalid IL or missing references)
			//IL_070b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_0737: Unknown result type (might be due to invalid IL or missing references)
			//IL_073e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_077e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_087c: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0909: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_092d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0934: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_093d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0944: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_0956: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_095d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0962: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0999: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_085e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			edge = m_EdgeData[entity];
			curve = m_CurveDataFromEntity[entity];
			Composition composition = m_CompositionDataFromEntity[entity];
			PrefabRef prefabRef = m_PrefabRefDataFromEntity[entity];
			PrefabRef prefabRef2 = m_PrefabRefDataFromEntity[edge.m_Start];
			PrefabRef prefabRef3 = m_PrefabRefDataFromEntity[edge.m_End];
			edgePrefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef2.m_Prefab];
			NetGeometryData netGeometryData2 = m_PrefabGeometryData[prefabRef3.m_Prefab];
			edgeCompositionData = m_PrefabCompositionData[composition.m_Edge];
			startCompositionData = m_PrefabCompositionData[composition.m_StartNode];
			endCompositionData = m_PrefabCompositionData[composition.m_EndNode];
			DynamicBuffer<NetCompositionLane> nodeCompositionLanes = m_PrefabCompositionLanes[composition.m_Edge];
			DynamicBuffer<NetCompositionCrosswalk> nodeCompositionCrosswalks = m_PrefabCompositionCrosswalks[composition.m_StartNode];
			DynamicBuffer<NetCompositionCrosswalk> nodeCompositionCrosswalks2 = m_PrefabCompositionCrosswalks[composition.m_EndNode];
			Owner owner = default(Owner);
			Composition composition2 = default(Composition);
			if (m_OwnerData.TryGetComponent(entity, ref owner) && m_CompositionDataFromEntity.TryGetComponent(owner.m_Owner, ref composition2))
			{
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition2.m_Edge];
				edgeCompositionData.m_NodeOffset = math.max(edgeCompositionData.m_NodeOffset, netCompositionData.m_NodeOffset);
			}
			float roundaboutSize = 0f;
			float roundaboutSize2 = 0f;
			startMiddleRadius = 0f;
			endMiddleRadius = 0f;
			bool flag = ((netGeometryData.m_MergeLayers ^ edgePrefabGeometryData.m_MergeLayers) & Layer.Waterway) == 0;
			bool flag2 = ((netGeometryData2.m_MergeLayers ^ edgePrefabGeometryData.m_MergeLayers) & Layer.Waterway) == 0;
			CutCurve(edge, ref curve, flag, flag2);
			float num = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref curve.m_Bezier)).xz, new float2(0f, 1f));
			Bezier4x3 val = default(Bezier4x3);
			Bezier4x3 val2 = default(Bezier4x3);
			MathUtils.Divide(curve.m_Bezier, ref val, ref val2, num);
			nodeGeometryStart = m_NodeGeometryData[edge.m_Start];
			nodeGeometryEnd = m_NodeGeometryData[edge.m_End];
			if ((startCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
			{
				roundaboutSize = startCompositionData.m_RoundaboutSize.x;
				startMiddleRadius = CalculateMiddleRadius(edge.m_Start, edgePrefabGeometryData);
			}
			else if ((startCompositionData.m_Flags.m_General & (CompositionFlags.General.LevelCrossing | CompositionFlags.General.FixedNodeSize)) == 0)
			{
				if (flag)
				{
					curve.m_Bezier.a.y = nodeGeometryStart.m_Position;
				}
				val.a.y = curve.m_Bezier.a.y;
				val.b.y += nodeGeometryStart.m_Offset;
				val.c.y -= nodeGeometryStart.m_Offset * 0.375f;
				val.d.y -= nodeGeometryStart.m_Offset * 0.1875f;
				val2.a.y -= nodeGeometryStart.m_Offset * 0.1875f;
				val2.b.y += nodeGeometryStart.m_Offset * 0.125f;
			}
			if ((endCompositionData.m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
			{
				roundaboutSize2 = endCompositionData.m_RoundaboutSize.y;
				endMiddleRadius = CalculateMiddleRadius(edge.m_End, edgePrefabGeometryData);
			}
			else if ((endCompositionData.m_Flags.m_General & (CompositionFlags.General.LevelCrossing | CompositionFlags.General.FixedNodeSize)) == 0)
			{
				if (flag2)
				{
					curve.m_Bezier.d.y = nodeGeometryEnd.m_Position;
				}
				val2.d.y = curve.m_Bezier.d.y;
				val2.c.y += nodeGeometryEnd.m_Offset;
				val2.b.y -= nodeGeometryEnd.m_Offset * 0.375f;
				val2.a.y -= nodeGeometryEnd.m_Offset * 0.1875f;
				val.d.y -= nodeGeometryEnd.m_Offset * 0.1875f;
				val.c.y += nodeGeometryEnd.m_Offset * 0.125f;
			}
			float num2 = math.distance(((float3)(ref val.c)).xz, ((float3)(ref val.d)).xz);
			float num3 = math.distance(((float3)(ref val2.b)).xz, ((float3)(ref val2.a)).xz);
			float num4 = math.lerp(val.c.y, val2.b.y, num2 / math.max(0.1f, num2 + num3)) - val.d.y;
			val.c.y -= num4 * 0.4f;
			val.d.y += num4 * 0.6f;
			val2.a.y += num4 * 0.6f;
			val2.b.y -= num4 * 0.4f;
			float2 val3 = edgeCompositionData.m_Width * new float2(0.5f, -0.5f) + edgeCompositionData.m_MiddleOffset;
			leftStartCurve = NetUtils.OffsetCurveLeftSmooth(val, float2.op_Implicit(val3.x));
			rightStartCurve = NetUtils.OffsetCurveLeftSmooth(val, float2.op_Implicit(val3.y));
			leftEndCurve = NetUtils.OffsetCurveLeftSmooth(val2, float2.op_Implicit(val3.x));
			rightEndCurve = NetUtils.OffsetCurveLeftSmooth(val2, float2.op_Implicit(val3.y));
			if ((edgeCompositionData.m_State & CompositionState.Airspace) != 0)
			{
				OffsetAirspaceCurves(edge, ref leftStartCurve, ref rightStartCurve, ref leftEndCurve, ref rightEndCurve);
			}
			startTangent = MathUtils.StartTangent(val);
			endTangent = MathUtils.EndTangent(val2);
			startTangent = MathUtils.Normalize(startTangent, ((float3)(ref startTangent)).xz);
			endTangent = MathUtils.Normalize(endTangent, ((float3)(ref endTangent)).xz);
			startTangent.y = math.clamp(startTangent.y, -1f, 1f);
			endTangent.y = math.clamp(endTangent.y, -1f, 1f);
			float y = startTangent.y;
			float slopeSteepness = 0f - endTangent.y;
			float2 val4 = startCompositionData.m_Width * new float2(0.5f, -0.5f) + startCompositionData.m_MiddleOffset;
			float2 val5 = endCompositionData.m_Width * new float2(0.5f, -0.5f) + endCompositionData.m_MiddleOffset;
			bool2 useEdgeWidth = default(bool2);
			((bool2)(ref useEdgeWidth))._002Ector(val3.x - val4.x > 0.001f, val4.y - val3.y > 0.001f);
			bool2 useEdgeWidth2 = default(bool2);
			((bool2)(ref useEdgeWidth2))._002Ector(val3.x - val5.x > 0.001f, val5.y - val3.y > 0.001f);
			startOffsets = CalculateCornerOffset(entity, edge.m_Start, y, useEdgeWidth, curve, leftStartCurve, rightStartCurve, leftEndCurve, rightEndCurve, prefabRef, edgePrefabGeometryData, edgeCompositionData, startCompositionData, nodeCompositionLanes, nodeCompositionCrosswalks, startMiddleRadius, roundaboutSize, nodeGeometryStart.m_Offset, isEnd: false, out startLeftTarget, out startRightTarget);
			float2 val6 = CalculateCornerOffset(entity, edge.m_End, slopeSteepness, useEdgeWidth2, curve, MathUtils.Invert(rightEndCurve), MathUtils.Invert(leftEndCurve), MathUtils.Invert(rightStartCurve), MathUtils.Invert(leftStartCurve), prefabRef, edgePrefabGeometryData, edgeCompositionData, endCompositionData, nodeCompositionLanes, nodeCompositionCrosswalks2, endMiddleRadius, roundaboutSize2, nodeGeometryEnd.m_Offset, isEnd: true, out endRightTarget, out endLeftTarget);
			endOffsets = ((float2)(ref val6)).yx;
			if (math.any(math.abs(val4 - val3) > 0.001f) || math.any(math.abs(val5 - val3) > 0.001f))
			{
				Bezier4x3 val7 = NetUtils.OffsetCurveLeftSmooth(val, float2.op_Implicit(val4.x));
				Bezier4x3 val8 = NetUtils.OffsetCurveLeftSmooth(val, float2.op_Implicit(val4.y));
				Bezier4x3 val9 = NetUtils.OffsetCurveLeftSmooth(val2, float2.op_Implicit(val5.x));
				Bezier4x3 val10 = NetUtils.OffsetCurveLeftSmooth(val2, float2.op_Implicit(val5.y));
				startOffsets = math.max(startOffsets, CalculateCornerOffset(entity, edge.m_Start, y, useEdgeWidth, curve, val7, val8, val9, val10, prefabRef, edgePrefabGeometryData, edgeCompositionData, startCompositionData, nodeCompositionLanes, nodeCompositionCrosswalks, 0f, 0f, nodeGeometryStart.m_Offset, isEnd: false, out var leftTarget, out var rightTarget));
				float2 val11 = endOffsets;
				val6 = CalculateCornerOffset(entity, edge.m_End, slopeSteepness, useEdgeWidth2, curve, MathUtils.Invert(val10), MathUtils.Invert(val9), MathUtils.Invert(val8), MathUtils.Invert(val7), prefabRef, edgePrefabGeometryData, edgeCompositionData, endCompositionData, nodeCompositionLanes, nodeCompositionCrosswalks2, 0f, 0f, nodeGeometryEnd.m_Offset, isEnd: true, out rightTarget, out leftTarget);
				endOffsets = math.max(val11, ((float2)(ref val6)).yx);
			}
		}

		private float2 CalculateFixedOffsets(Entity edge, Entity node, bool isStart, NetGeometryData prefabGeometryData, float3 startTangent, Bezier4x3 leftStartCurve, Bezier4x3 rightStartCurve, Bezier4x3 leftEndCurve, Bezier4x3 rightEndCurve, out float3 leftSnapPos, out float3 rightSnapPos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			float num = GetFixedNodeOffset(node, prefabGeometryData);
			Owner owner = default(Owner);
			Edge edge2 = default(Edge);
			if (m_OwnerData.TryGetComponent(edge, ref owner) && m_EdgeData.TryGetComponent(owner.m_Owner, ref edge2))
			{
				num = math.max(num, GetFixedNodeOffset(isStart ? edge2.m_Start : edge2.m_End, prefabGeometryData));
			}
			startTangent *= num;
			leftSnapPos = leftStartCurve.a;
			rightSnapPos = rightStartCurve.a;
			((float3)(ref leftSnapPos)).xz = ((float3)(ref leftSnapPos)).xz + ((float3)(ref startTangent)).xz;
			((float3)(ref rightSnapPos)).xz = ((float3)(ref rightSnapPos)).xz + ((float3)(ref startTangent)).xz;
			Segment val = default(Segment);
			((Segment)(ref val))._002Ector(((float3)(ref leftSnapPos)).xz + MathUtils.Left(((float3)(ref startTangent)).xz), ((float3)(ref rightSnapPos)).xz + MathUtils.Right(((float3)(ref startTangent)).xz));
			float2 result = float2.op_Implicit(1f);
			float2 val2 = default(float2);
			if (MathUtils.Intersect(((Bezier4x3)(ref leftStartCurve)).xz, val, ref val2, 4))
			{
				result.x = val2.x;
			}
			else if (MathUtils.Intersect(((Bezier4x3)(ref leftEndCurve)).xz, val, ref val2, 4))
			{
				result.x = 1f + val2.x;
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref rightStartCurve)).xz, val, ref val2, 4))
			{
				result.y = val2.x;
			}
			else if (MathUtils.Intersect(((Bezier4x3)(ref rightEndCurve)).xz, val, ref val2, 4))
			{
				result.y = 1f + val2.x;
			}
			return result;
		}

		private void OffsetAirspaceCurves(Edge edge, ref Bezier4x3 leftStartCurve, ref Bezier4x3 rightStartCurve, ref Bezier4x3 leftEndCurve, ref Bezier4x3 rightEndCurve)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x1 y = ((Bezier4x3)(ref leftStartCurve)).y;
			float4 abcd = ((Bezier4x1)(ref y)).abcd;
			y = ((Bezier4x3)(ref rightStartCurve)).y;
			float4 val = (abcd + ((Bezier4x1)(ref y)).abcd) * 0.5f;
			y = ((Bezier4x3)(ref leftEndCurve)).y;
			float4 abcd2 = ((Bezier4x1)(ref y)).abcd;
			y = ((Bezier4x3)(ref rightEndCurve)).y;
			float4 val2 = (abcd2 + ((Bezier4x1)(ref y)).abcd) * 0.5f;
			float2 val3 = float2.op_Implicit(0f);
			float2 val4 = default(float2);
			((float2)(ref val4))._002Ector(val.x, val2.w);
			Elevation elevation = default(Elevation);
			if (m_ElevationData.TryGetComponent(edge.m_Start, ref elevation))
			{
				val3.x = math.csum(elevation.m_Elevation) * 0.5f;
			}
			Elevation elevation2 = default(Elevation);
			if (m_ElevationData.TryGetComponent(edge.m_End, ref elevation2))
			{
				val3.y = math.csum(elevation2.m_Elevation) * 0.5f;
			}
			float4 val5 = math.lerp(float4.op_Implicit(val3.x), float4.op_Implicit(val3.y), math.saturate((val - val4.x) / (val4.y - val4.x)));
			float4 val6 = math.lerp(float4.op_Implicit(val3.x), float4.op_Implicit(val3.y), math.saturate((val2 - val4.x) / (val4.y - val4.x)));
			float3 val7 = math.normalizesafe(rightStartCurve.a - leftStartCurve.a, default(float3)) * val5.x;
			float3 val8 = math.normalizesafe(rightStartCurve.b - leftStartCurve.b, default(float3)) * val5.y;
			float3 val9 = math.normalizesafe(rightStartCurve.c - leftStartCurve.c, default(float3)) * val5.z;
			float3 val10 = math.normalizesafe(rightStartCurve.d - leftStartCurve.d, default(float3)) * val5.w;
			ref float3 a = ref leftStartCurve.a;
			a -= val7;
			ref float3 b = ref leftStartCurve.b;
			b -= val8;
			ref float3 c = ref leftStartCurve.c;
			c -= val9;
			ref float3 d = ref leftStartCurve.d;
			d -= val10;
			ref float3 a2 = ref rightStartCurve.a;
			a2 += val7;
			ref float3 b2 = ref rightStartCurve.b;
			b2 += val8;
			ref float3 c2 = ref rightStartCurve.c;
			c2 += val9;
			ref float3 d2 = ref rightStartCurve.d;
			d2 += val10;
			float3 val11 = math.normalizesafe(rightEndCurve.a - leftEndCurve.a, default(float3)) * val6.x;
			float3 val12 = math.normalizesafe(rightEndCurve.b - leftEndCurve.b, default(float3)) * val6.y;
			float3 val13 = math.normalizesafe(rightEndCurve.c - leftEndCurve.c, default(float3)) * val6.z;
			float3 val14 = math.normalizesafe(rightEndCurve.d - leftEndCurve.d, default(float3)) * val6.w;
			ref float3 a3 = ref leftEndCurve.a;
			a3 -= val11;
			ref float3 b3 = ref leftEndCurve.b;
			b3 -= val12;
			ref float3 c3 = ref leftEndCurve.c;
			c3 -= val13;
			ref float3 d3 = ref leftEndCurve.d;
			d3 -= val14;
			ref float3 a4 = ref rightEndCurve.a;
			a4 += val11;
			ref float3 b4 = ref rightEndCurve.b;
			b4 += val12;
			ref float3 c4 = ref rightEndCurve.c;
			c4 += val13;
			ref float3 d4 = ref rightEndCurve.d;
			d4 += val14;
		}

		private void CutCurve(Edge edge, ref Curve curve, bool useStartNodeHeight, bool useEndNodeHeight)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			Node node = m_NodeDataFromEntity[edge.m_Start];
			Node node2 = m_NodeDataFromEntity[edge.m_End];
			float num = default(float);
			MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref node.m_Position)).xz, ref num);
			float num2 = default(float);
			MathUtils.Distance(((Bezier4x3)(ref curve.m_Bezier)).xz, ((float3)(ref node2.m_Position)).xz, ref num2);
			if (num < 0.001f)
			{
				num = 0f;
			}
			if (num2 > 0.999f)
			{
				num2 = 1f;
			}
			if (num != 0f || num2 != 1f)
			{
				if (num2 < num + 0.02f)
				{
					num = (num2 = (num + num2) * 0.5f);
					num = math.max(0f, num - 0.01f);
					num2 = math.min(1f, num2 + 0.01f);
				}
				if (!useStartNodeHeight)
				{
					node.m_Position.y = curve.m_Bezier.a.y;
				}
				if (!useEndNodeHeight)
				{
					node2.m_Position.y = curve.m_Bezier.d.y;
				}
				curve.m_Bezier = MathUtils.Cut(curve.m_Bezier, new float2(num, num2));
				curve.m_Bezier.a.y = node.m_Position.y;
				curve.m_Bezier.d.y = node2.m_Position.y;
			}
		}

		private float CalculateMiddleRadius(Entity node, NetGeometryData netGeometryData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(node, ref val))
			{
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				ObjectGeometryData objectGeometryData = default(ObjectGeometryData);
				for (int i = 0; i < val.Length; i++)
				{
					PrefabRef prefabRef = m_PrefabRefDataFromEntity[val[i].m_SubObject];
					if (m_PlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData) && m_ObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref objectGeometryData) && (placeableObjectData.m_Flags & Game.Objects.PlacementFlags.RoadNode) != Game.Objects.PlacementFlags.None)
					{
						float num2 = math.cmax(((float3)(ref objectGeometryData.m_Size)).xz) * 0.5f;
						if ((objectGeometryData.m_Flags & Game.Objects.GeometryFlags.Standing) != Game.Objects.GeometryFlags.None)
						{
							num2 = ((!(objectGeometryData.m_LegSize.y >= netGeometryData.m_DefaultHeightRange.max)) ? math.max(num2, math.cmax(((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f + objectGeometryData.m_LegOffset)) : math.cmax(((float3)(ref objectGeometryData.m_LegSize)).xz * 0.5f + objectGeometryData.m_LegOffset));
						}
						num = math.max(num, num2 + 1f);
					}
				}
			}
			return num;
		}

		private float GetFixedNodeOffset(Entity node, NetGeometryData netGeometryData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(node, ref val))
			{
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				for (int i = 0; i < val.Length; i++)
				{
					PrefabRef prefabRef = m_PrefabRefDataFromEntity[val[i].m_SubObject];
					if (m_PlaceableObjectData.TryGetComponent(prefabRef.m_Prefab, ref placeableObjectData) && (placeableObjectData.m_Flags & Game.Objects.PlacementFlags.RoadNode) != Game.Objects.PlacementFlags.None)
					{
						num = math.max(num, placeableObjectData.m_PlacementOffset.z);
					}
				}
			}
			return num;
		}

		private void LimitHeightDelta(ref Bezier4x3 left, ref Bezier4x3 right, float slopeSteepness, Bezier4x3 originalLeft, Bezier4x3 originalRight, NetGeometryData prefabGeometryData, NetCompositionData edgeCompositionData, NetCompositionData nodeCompositionData, float nodeFlatness)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if ((nodeCompositionData.m_Flags.m_General & (CompositionFlags.General.LevelCrossing | CompositionFlags.General.FixedNodeSize)) != 0)
			{
				left.a.y = originalLeft.a.y;
				left.b.y = originalLeft.a.y;
				right.a.y = originalRight.a.y;
				right.b.y = originalRight.a.y;
				return;
			}
			float num = math.max(edgeCompositionData.m_NodeOffset, nodeCompositionData.m_NodeOffset) + math.abs(slopeSteepness) * edgeCompositionData.m_Width * 0.25f;
			float num2 = prefabGeometryData.m_MaxSlopeSteepness * num * 1.5f;
			float num3 = math.max(math.min(0f, originalLeft.a.y + num2 - left.a.y), originalLeft.a.y - num2 - left.a.y);
			float num4 = math.max(math.min(0f, originalRight.a.y + num2 - right.a.y), originalRight.a.y - num2 - right.a.y);
			left.a.y += num3 * nodeFlatness;
			left.b.y += num3 * nodeFlatness;
			right.a.y += num4 * nodeFlatness;
			right.b.y += num4 * nodeFlatness;
		}

		private float CalculateCutOffset(Bezier4x3 start, Bezier4x3 end, float startOffset, float endOffset, float width)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			float num = ((startOffset >= 1f) ? MathUtils.Length(((Bezier4x3)(ref end)).xz, new Bounds1(startOffset - 1f, 1f - endOffset)) : ((!(endOffset >= 1f)) ? (MathUtils.Length(((Bezier4x3)(ref start)).xz, new Bounds1(startOffset, 1f)) + MathUtils.Length(((Bezier4x3)(ref end)).xz, new Bounds1(0f, 1f - endOffset))) : MathUtils.Length(((Bezier4x3)(ref start)).xz, new Bounds1(startOffset, 2f - endOffset))));
			return 1f - (2f - startOffset - endOffset) * 0.5f / (num / math.max(0.01f, width) + 1f);
		}

		private Bezier4x3 Cut(Bezier4x3 start, Bezier4x3 end, float startOffset, float endOffset, float cutOffset)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (startOffset >= 1f)
			{
				return MathUtils.Cut(end, new float2(startOffset - 1f, startOffset - cutOffset));
			}
			if (startOffset > cutOffset)
			{
				float3 startPos = MathUtils.Position(start, startOffset);
				float3 endPos = MathUtils.Position(end, startOffset - cutOffset);
				float3 val = MathUtils.Tangent(start, startOffset);
				float3 val2 = MathUtils.Tangent(end, startOffset - cutOffset);
				val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
				val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
				val.y = math.clamp(val.y, -1f, 1f);
				val2.y = math.clamp(val2.y, -1f, 1f);
				return NetUtils.FitCurve(startPos, val, val2, endPos);
			}
			return MathUtils.Cut(start, new float2(startOffset, math.min(1f, 1f + cutOffset - endOffset)));
		}

		private void ConformLengths(ref Bezier4x3 start, ref Bezier4x3 end, float startOffset, float endOffset)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			if (startOffset >= 1f)
			{
				Bezier4x2 xz = ((Bezier4x3)(ref end)).xz;
				Bounds1 val = default(Bounds1);
				((Bounds1)(ref val))._002Ector(startOffset - 1f, 1f - endOffset);
				float num = MathUtils.Length(xz, val);
				MathUtils.ClampLength(xz, ref val, num * 0.5f);
				start = MathUtils.Cut(end, val);
				end = MathUtils.Cut(end, new Bounds1(val.max, 1f - endOffset));
				return;
			}
			if (endOffset >= 1f)
			{
				Bezier4x2 xz2 = ((Bezier4x3)(ref start)).xz;
				Bounds1 val2 = default(Bounds1);
				((Bounds1)(ref val2))._002Ector(startOffset, 2f - endOffset);
				float num2 = MathUtils.Length(xz2, val2);
				MathUtils.ClampLengthInverse(xz2, ref val2, num2 * 0.5f);
				end = MathUtils.Cut(start, val2);
				start = MathUtils.Cut(start, new Bounds1(startOffset, val2.min));
				return;
			}
			Bezier4x2 xz3 = ((Bezier4x3)(ref start)).xz;
			Bezier4x2 xz4 = ((Bezier4x3)(ref end)).xz;
			Bounds1 val3 = default(Bounds1);
			((Bounds1)(ref val3))._002Ector(startOffset, 1f);
			Bounds1 val4 = default(Bounds1);
			((Bounds1)(ref val4))._002Ector(0f, 1f - endOffset);
			float num3 = MathUtils.Length(xz3, val3);
			float num4 = MathUtils.Length(xz4, val4);
			float3 val5;
			float3 val6;
			if (num3 > num4)
			{
				MathUtils.ClampLength(xz3, ref val3, math.lerp(num3, num4, 0.5f));
				val5 = MathUtils.Position(start, val3.max);
				val6 = MathUtils.Tangent(start, val3.max);
			}
			else
			{
				MathUtils.ClampLengthInverse(xz4, ref val4, math.lerp(num3, num4, 0.5f));
				val5 = MathUtils.Position(end, val4.min);
				val6 = MathUtils.Tangent(end, val4.min);
			}
			float3 startPos = MathUtils.Position(start, startOffset);
			float3 val7 = MathUtils.Tangent(start, startOffset);
			float3 endPos = MathUtils.Position(end, 1f - endOffset);
			float3 val8 = MathUtils.Tangent(end, 1f - endOffset);
			val7 = MathUtils.Normalize(val7, ((float3)(ref val7)).xz);
			val6 = MathUtils.Normalize(val6, ((float3)(ref val6)).xz);
			val8 = MathUtils.Normalize(val8, ((float3)(ref val8)).xz);
			start = NetUtils.FitCurve(startPos, val7, val6, val5);
			end = NetUtils.FitCurve(val5, val6, val8, endPos);
		}

		private float2 CalculateCornerOffset(Entity edge, Entity node, float slopeSteepness, bool2 useEdgeWidth, Curve curveData, Bezier4x3 leftStartCurve, Bezier4x3 rightStartCurve, Bezier4x3 leftEndCurve, Bezier4x3 rightEndCurve, PrefabRef prefabRef, NetGeometryData prefabGeometryData, NetCompositionData edgeCompositionData, NetCompositionData nodeCompositionData, DynamicBuffer<NetCompositionLane> nodeCompositionLanes, DynamicBuffer<NetCompositionCrosswalk> nodeCompositionCrosswalks, float middleRadius, float roundaboutSize, float bOffset, bool isEnd, out float3 leftTarget, out float3 rightTarget)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fdc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fcb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0efe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ebb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ec6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f86: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
			//IL_089a: Unknown result type (might be due to invalid IL or missing references)
			//IL_089f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0974: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_097e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_098e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0993: Unknown result type (might be due to invalid IL or missing references)
			//IL_0995: Unknown result type (might be due to invalid IL or missing references)
			//IL_0997: Unknown result type (might be due to invalid IL or missing references)
			//IL_099e: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_061d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			//IL_062c: Unknown result type (might be due to invalid IL or missing references)
			//IL_062e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0675: Unknown result type (might be due to invalid IL or missing references)
			//IL_067a: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0684: Unknown result type (might be due to invalid IL or missing references)
			//IL_0689: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_090d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0923: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_0709: Unknown result type (might be due to invalid IL or missing references)
			//IL_0710: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_0930: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_0746: Unknown result type (might be due to invalid IL or missing references)
			//IL_074f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0754: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_0769: Unknown result type (might be due to invalid IL or missing references)
			//IL_0730: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0744: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0786: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c44: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07be: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			leftTarget = default(float3);
			rightTarget = default(float3);
			if (isEnd)
			{
				curveData.m_Bezier = MathUtils.Invert(curveData.m_Bezier);
				edgeCompositionData.m_MiddleOffset = 0f - edgeCompositionData.m_MiddleOffset;
			}
			float3 val2 = MathUtils.StartTangent(leftStartCurve);
			float2 xz = ((float3)(ref val2)).xz;
			val2 = MathUtils.StartTangent(rightStartCurve);
			float2 xz2 = ((float3)(ref val2)).xz;
			float2 val3 = ((float3)(ref leftStartCurve.a)).xz - ((float3)(ref rightStartCurve.a)).xz;
			MathUtils.TryNormalize(ref xz);
			MathUtils.TryNormalize(ref xz2);
			MathUtils.TryNormalize(ref val3);
			float2 val4 = default(float2);
			float2 val5 = default(float2);
			float2 val6 = float2.op_Implicit(float.MinValue);
			float2 val7 = float2.op_Implicit(0f);
			float num = prefabGeometryData.m_MinNodeOffset;
			bool flag = middleRadius > 0f;
			float num2 = math.max(edgeCompositionData.m_NodeOffset, nodeCompositionData.m_NodeOffset);
			float num3 = num2 + math.abs(slopeSteepness) * nodeCompositionData.m_Width * 0.25f;
			bool flag2 = true;
			EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_EdgeData, m_TempData, m_HiddenData);
			EdgeIteratorValue value;
			Owner owner = default(Owner);
			Composition composition2 = default(Composition);
			Bezier4x3 val8 = default(Bezier4x3);
			Bezier4x3 val9 = default(Bezier4x3);
			float2 val12 = default(float2);
			float2 val19 = default(float2);
			Bezier4x2 nodeCurve = default(Bezier4x2);
			while (edgeIterator.GetNext(out value))
			{
				if (!(value.m_Edge != edge))
				{
					continue;
				}
				PrefabRef prefabRef2 = m_PrefabRefDataFromEntity[value.m_Edge];
				NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef2.m_Prefab];
				if ((netGeometryData.m_IntersectLayers & prefabGeometryData.m_IntersectLayers) == 0)
				{
					continue;
				}
				Edge edge2 = m_EdgeData[value.m_Edge];
				Curve curve = m_CurveDataFromEntity[value.m_Edge];
				Composition composition = m_CompositionDataFromEntity[value.m_Edge];
				NetCompositionData edgeCompositionData2 = m_PrefabCompositionData[composition.m_Edge];
				DynamicBuffer<NetCompositionLane> prefabCompositionLanes = m_PrefabCompositionLanes[composition.m_Edge];
				NetCompositionData nodeCompositionData2;
				DynamicBuffer<NetCompositionCrosswalk> prefabCompositionCrosswalks;
				if (value.m_End)
				{
					nodeCompositionData2 = m_PrefabCompositionData[composition.m_EndNode];
					prefabCompositionCrosswalks = m_PrefabCompositionCrosswalks[composition.m_EndNode];
					edgeCompositionData2.m_MiddleOffset = 0f - edgeCompositionData2.m_MiddleOffset;
				}
				else
				{
					nodeCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
					prefabCompositionCrosswalks = m_PrefabCompositionCrosswalks[composition.m_StartNode];
				}
				if (m_OwnerData.TryGetComponent(value.m_Edge, ref owner) && m_CompositionDataFromEntity.TryGetComponent(owner.m_Owner, ref composition2))
				{
					NetCompositionData netCompositionData = m_PrefabCompositionData[composition2.m_Edge];
					edgeCompositionData2.m_WidthOffset = math.max(edgeCompositionData2.m_WidthOffset, netCompositionData.m_WidthOffset);
					nodeCompositionData2.m_WidthOffset = math.max(nodeCompositionData2.m_WidthOffset, netCompositionData.m_WidthOffset);
				}
				CutCurve(edge2, ref curve, useStartNodeHeight: true, useEndNodeHeight: true);
				float num4 = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref curve.m_Bezier)).xz, new float2(0f, 1f));
				MathUtils.Divide(curve.m_Bezier, ref val8, ref val9, num4);
				if (value.m_End)
				{
					curve.m_Bezier = MathUtils.Invert(curve.m_Bezier);
					Bezier4x3 val10 = MathUtils.Invert(val8);
					val8 = MathUtils.Invert(val9);
					val9 = val10;
				}
				curve.m_Bezier.a.y = curveData.m_Bezier.a.y;
				val8.a.y = curveData.m_Bezier.a.y;
				val8.b.y += bOffset;
				bool flag3 = false;
				if ((netGeometryData.m_MergeLayers & prefabGeometryData.m_MergeLayers) != Layer.None)
				{
					num = math.max(num, netGeometryData.m_MinNodeOffset);
					if (middleRadius > 0f)
					{
						roundaboutSize = ((!value.m_End) ? math.max(roundaboutSize, nodeCompositionData2.m_RoundaboutSize.x) : math.max(roundaboutSize, nodeCompositionData2.m_RoundaboutSize.y));
					}
					float3 val11 = MathUtils.StartTangent(val8);
					val11 = MathUtils.Normalize(val11, ((float3)(ref val11)).xz);
					val11.y = math.clamp(val11.y, -1f, 1f);
					float num5 = 0f - val11.y;
					val12.x = math.dot(((float3)(ref curve.m_Bezier.a)).xz - ((float3)(ref leftStartCurve.a)).xz, xz);
					val12.y = math.dot(((float3)(ref curve.m_Bezier.a)).xz - ((float3)(ref rightStartCurve.a)).xz, xz2);
					val6 = math.max(val6, val12 * 0.5f);
					float num6 = math.abs(slopeSteepness - num5) * nodeCompositionData.m_Width;
					bool dontCrossTracks = false;
					if (((edgeCompositionData.m_State | edgeCompositionData2.m_State) & (CompositionState.HasForwardTrackLanes | CompositionState.HasBackwardTrackLanes)) != 0 && ((edgeCompositionData.m_State | edgeCompositionData2.m_State) & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes)) == 0 && (nodeCompositionData.m_Flags.m_General & CompositionFlags.General.Intersection) == 0)
					{
						dontCrossTracks = GetTopOwner(edge) == GetTopOwner(value.m_Edge);
					}
					float4 val13 = (1f - nodeCompositionData.m_SyncVertexOffsetsLeft) * (nodeCompositionData.m_Width * 0.5f + nodeCompositionData.m_MiddleOffset);
					float4 val14 = nodeCompositionData.m_SyncVertexOffsetsRight * (nodeCompositionData.m_Width * 0.5f - nodeCompositionData.m_MiddleOffset);
					float4 val15 = (1f - ((float4)(ref nodeCompositionData2.m_SyncVertexOffsetsLeft)).wzyx) * (nodeCompositionData2.m_Width * 0.5f + nodeCompositionData2.m_MiddleOffset);
					float4 val16 = ((float4)(ref nodeCompositionData2.m_SyncVertexOffsetsRight)).wzyx * (nodeCompositionData2.m_Width * 0.5f - nodeCompositionData2.m_MiddleOffset);
					float2 val17 = edgeCompositionData.m_Width * 0.5f + new float2(edgeCompositionData.m_MiddleOffset, 0f - edgeCompositionData.m_MiddleOffset);
					float2 val18 = edgeCompositionData2.m_Width * 0.5f + new float2(0f - edgeCompositionData2.m_MiddleOffset, edgeCompositionData2.m_MiddleOffset);
					float num7 = math.dot(((float3)(ref curveData.m_Bezier.a)).xz - ((float3)(ref curve.m_Bezier.a)).xz, val3);
					((float2)(ref val19))._002Ector(math.cmax(math.abs(val13 - val16 + num7)), math.cmax(math.abs(val14 - val15 - num7)));
					val19 = math.max(val19, math.abs(val17 - val18 + new float2(num7, 0f - num7)));
					float num8 = math.max(0.1f, math.min(edgeCompositionData.m_Width, edgeCompositionData2.m_Width));
					val19 *= num8 / (num8 + val19 * 0.75f);
					val19 = math.max(val19, float2.op_Implicit(CompareLanes(nodeCompositionLanes, prefabCompositionLanes, num7, isEnd, value.m_End, dontCrossTracks)));
					if ((nodeCompositionData.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0)
					{
						val19 = math.max(val19, float2.op_Implicit(CheckCrosswalks(nodeCompositionCrosswalks)));
					}
					if ((nodeCompositionData2.m_Flags.m_General & CompositionFlags.General.Crosswalk) != 0)
					{
						val19 = math.max(val19, float2.op_Implicit(CheckCrosswalks(prefabCompositionCrosswalks)));
					}
					val19 *= math.max(0f, math.dot(((float3)(ref val11)).xz, -xz));
					if (num6 > 0.2f)
					{
						val7 = math.max(val7, float2.op_Implicit(num6));
					}
					if (prefabRef2.m_Prefab != prefabRef.m_Prefab)
					{
						val7 = math.max(val7, float2.op_Implicit(num2));
					}
					if (math.any(val19 > 0.1f))
					{
						val7 = math.max(val7, val19);
						flag3 = true;
					}
					if (math.distancesq(((float3)(ref curve.m_Bezier.a)).xz, ((float3)(ref curveData.m_Bezier.a)).xz) > 0.01f || math.dot(((float3)(ref val11)).xz, -xz) < 0.9995f)
					{
						flag3 = true;
						flag = true;
					}
					else if (RequireTransition(nodeCompositionData, nodeCompositionData2, edgeCompositionData, edgeCompositionData2))
					{
						flag3 = true;
					}
					if (flag3)
					{
						nodeCompositionData2.m_Width += num3 * 2f;
						nodeCompositionData2.m_Width += nodeCompositionData2.m_WidthOffset;
					}
					if (!flag2)
					{
						flag = true;
					}
				}
				else
				{
					flag3 = true;
					flag = true;
					nodeCompositionData2.m_Width += num2 * 2f;
					nodeCompositionData2.m_Width += nodeCompositionData2.m_WidthOffset;
				}
				float2 val20 = nodeCompositionData2.m_Width * new float2(0.5f, -0.5f) - nodeCompositionData2.m_MiddleOffset;
				if (math.any(useEdgeWidth))
				{
					if (flag3)
					{
						edgeCompositionData2.m_Width += (((netGeometryData.m_MergeLayers & prefabGeometryData.m_MergeLayers) != Layer.None) ? num3 : num2) * 2f;
						edgeCompositionData2.m_Width += edgeCompositionData2.m_WidthOffset;
					}
					float2 val21 = edgeCompositionData2.m_Width * new float2(0.5f, -0.5f) - edgeCompositionData2.m_MiddleOffset;
					if (useEdgeWidth.y)
					{
						val20.x = math.max(val20.x, val21.x);
					}
					if (useEdgeWidth.x)
					{
						val20.y = math.min(val20.y, val21.y);
					}
				}
				Bezier4x3 val22 = NetUtils.OffsetCurveLeftSmooth(val8, float2.op_Implicit(val20.x));
				Bezier4x3 val23 = NetUtils.OffsetCurveLeftSmooth(val9, float2.op_Implicit(val20.x));
				Bezier4x3 val24 = NetUtils.OffsetCurveLeftSmooth(val8, float2.op_Implicit(val20.y));
				Bezier4x3 val25 = NetUtils.OffsetCurveLeftSmooth(val9, float2.op_Implicit(val20.y));
				val2 = MathUtils.StartTangent(val22);
				float2 val26 = ((float3)(ref val2)).xz;
				val2 = MathUtils.StartTangent(val24);
				float2 val27 = ((float3)(ref val2)).xz;
				MathUtils.TryNormalize(ref val26);
				MathUtils.TryNormalize(ref val27);
				if (flag3)
				{
					float2 val28 = float2.op_Implicit(math.max(nodeCompositionData.m_Width, nodeCompositionData2.m_Width * 0.5f));
					val28 = math.max(float2.op_Implicit(num3), val28 * math.saturate(new float2(math.dot(xz2, val26), math.dot(xz, val27)) + 1f));
					nodeCurve.a = ((float3)(ref val22.a)).xz;
					nodeCurve.b = ((float3)(ref val22.a)).xz - val26 * (val28.x * 1.3333334f);
					nodeCurve.c = ((float3)(ref val24.a)).xz - val27 * (val28.y * 1.3333334f);
					nodeCurve.d = ((float3)(ref val24.a)).xz;
					float2 val29 = Intersect(((Bezier4x3)(ref leftStartCurve)).xz, ((Bezier4x3)(ref leftEndCurve)).xz, nodeCurve, ((Bezier4x3)(ref val24)).xz, ((Bezier4x3)(ref val25)).xz);
					float2 val30 = Intersect(((Bezier4x3)(ref rightStartCurve)).xz, ((Bezier4x3)(ref rightEndCurve)).xz, nodeCurve, ((Bezier4x3)(ref val22)).xz, ((Bezier4x3)(ref val23)).xz);
					if (val29.x > 0f)
					{
						if (val29.x > val.x)
						{
							val.x = val29.x;
						}
						val27 = Tangent(((Bezier4x3)(ref val24)).xz, ((Bezier4x3)(ref val25)).xz, val29.y);
						MathUtils.TryNormalize(ref val27);
					}
					if (val30.x > 0f)
					{
						if (val30.x > val.y)
						{
							val.y = val30.x;
						}
						val26 = Tangent(((Bezier4x3)(ref val22)).xz, ((Bezier4x3)(ref val23)).xz, val30.y);
						MathUtils.TryNormalize(ref val26);
					}
				}
				else
				{
					leftTarget = val24.a;
					rightTarget = val22.a;
				}
				if (flag2)
				{
					val4 = val27;
				}
				else if (math.dot(val27, val3) > 0f)
				{
					if (math.dot(val4, val3) <= 0f || math.dot(val27, xz) >= math.dot(val4, xz))
					{
						val4 = val27;
					}
				}
				else if (math.dot(val4, val3) <= 0f && math.dot(val27, xz) <= math.dot(val4, xz))
				{
					val4 = val27;
				}
				if (flag2)
				{
					val5 = val26;
				}
				else if (math.dot(val26, val3) < 0f)
				{
					if (math.dot(val5, val3) >= 0f || math.dot(val26, xz2) >= math.dot(val5, xz2))
					{
						val5 = val26;
					}
				}
				else if (math.dot(val5, val3) >= 0f && math.dot(val26, xz2) <= math.dot(val5, xz2))
				{
					val5 = val26;
				}
				flag2 = false;
			}
			if (math.any((val6 > 0.1f) | (val7 > 0.1f)) || flag)
			{
				if (flag)
				{
					val7 = math.max(val7, float2.op_Implicit(num));
				}
				val6 += val7;
				if (middleRadius > 0f)
				{
					float num9 = middleRadius + roundaboutSize;
					float3 position = m_NodeDataFromEntity[node].m_Position;
					float2 val31 = default(float2);
					val31.x = math.dot(((float3)(ref position)).xz - ((float3)(ref leftStartCurve.a)).xz, xz);
					val31.y = math.dot(((float3)(ref position)).xz - ((float3)(ref rightStartCurve.a)).xz, xz2);
					val6 = math.max(val6, num9 + num2 + val31);
				}
				if (val6.x > 0f)
				{
					Bounds1 val32 = default(Bounds1);
					((Bounds1)(ref val32))._002Ector(0f, 1f);
					if (MathUtils.ClampLength(leftStartCurve, ref val32, val6.x))
					{
						val.x = math.max(val.x, val32.max);
					}
					else
					{
						((Bounds1)(ref val32))._002Ector(0f, 1f);
						val6.x = math.max(0f, val6.x - MathUtils.Length(leftStartCurve));
						MathUtils.ClampLength(leftEndCurve, ref val32, val6.x);
						val.x = math.max(val.x, 1f + val32.max);
					}
				}
				if (val6.y > 0f)
				{
					Bounds1 val33 = default(Bounds1);
					((Bounds1)(ref val33))._002Ector(0f, 1f);
					if (MathUtils.ClampLength(rightStartCurve, ref val33, val6.y))
					{
						val.y = math.max(val.y, val33.max);
					}
					else
					{
						((Bounds1)(ref val33))._002Ector(0f, 1f);
						val6.y = math.max(0f, val6.y - MathUtils.Length(rightStartCurve));
						MathUtils.ClampLength(rightEndCurve, ref val33, val6.y);
						val.y = math.max(val.y, 1f + val33.max);
					}
				}
				if ((prefabGeometryData.m_Flags & GeometryFlags.StraightEnds) != 0)
				{
					val = float2.op_Implicit(math.cmax(val));
				}
				else if (val.y > val.x)
				{
					CheckOppositeSide(((Bezier4x3)(ref leftStartCurve)).xz, ((Bezier4x3)(ref leftEndCurve)).xz, ((Bezier4x3)(ref rightStartCurve)).xz, ((Bezier4x3)(ref rightEndCurve)).xz, val4, val5, ref val.x, val.y);
				}
				else if (val.y < val.x)
				{
					CheckOppositeSide(((Bezier4x3)(ref rightStartCurve)).xz, ((Bezier4x3)(ref rightEndCurve)).xz, ((Bezier4x3)(ref leftStartCurve)).xz, ((Bezier4x3)(ref leftEndCurve)).xz, val5, val4, ref val.y, val.x);
				}
				leftTarget = default(float3);
				rightTarget = default(float3);
			}
			if (m_OutsideConnectionData.HasComponent(node))
			{
				float2 val34 = default(float2);
				val34.x = IntersectBounds(leftStartCurve, leftEndCurve);
				val34.y = IntersectBounds(rightStartCurve, rightEndCurve);
				if ((prefabGeometryData.m_Flags & GeometryFlags.StraightEnds) != 0)
				{
					val34 = float2.op_Implicit(math.cmin(val34));
				}
				val = math.max(val, val34);
				leftTarget = default(float3);
				rightTarget = default(float3);
			}
			return val;
		}

		private Entity GetTopOwner(Entity entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			Entity result = Entity.Null;
			Owner owner = default(Owner);
			Temp temp = default(Temp);
			while (m_OwnerData.TryGetComponent(entity, ref owner))
			{
				entity = owner.m_Owner;
				result = entity;
				if (m_TempData.TryGetComponent(entity, ref temp) && temp.m_Original != Entity.Null)
				{
					entity = temp.m_Original;
					result = entity;
				}
			}
			return result;
		}

		private float IntersectBounds(Bezier4x3 startCurve, Bezier4x3 endCurve)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			float num2 = default(float);
			if (MathUtils.Intersect(((Bezier4x3)(ref startCurve)).x, m_TerrainBounds.min.x, ref num2, 4))
			{
				num = math.max(num, num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref startCurve)).x, m_TerrainBounds.max.x, ref num2, 4))
			{
				num = math.max(num, num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref startCurve)).z, m_TerrainBounds.min.z, ref num2, 4))
			{
				num = math.max(num, num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref startCurve)).z, m_TerrainBounds.max.z, ref num2, 4))
			{
				num = math.max(num, num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref endCurve)).x, m_TerrainBounds.min.x, ref num2, 4))
			{
				num = math.max(num, 1f + num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref endCurve)).x, m_TerrainBounds.max.x, ref num2, 4))
			{
				num = math.max(num, 1f + num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref endCurve)).z, m_TerrainBounds.min.z, ref num2, 4))
			{
				num = math.max(num, 1f + num2);
			}
			if (MathUtils.Intersect(((Bezier4x3)(ref endCurve)).z, m_TerrainBounds.max.z, ref num2, 4))
			{
				num = math.max(num, 1f + num2);
			}
			return num;
		}

		private void CheckOppositeSide(Bezier4x2 startCurve, Bezier4x2 endCurve, Bezier4x2 oppositeStartCurve, Bezier4x2 oppositeEndCurve, float2 intersectTangent, float2 oppositeIntersectTangent, ref float t, float oppositeT)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			float2 val = Position(startCurve, endCurve, t);
			float2 val2 = Tangent(startCurve, endCurve, t);
			float2 val3 = Position(oppositeStartCurve, oppositeEndCurve, t);
			float2 val4 = val - val3;
			float2 val5 = Position(oppositeStartCurve, oppositeEndCurve, oppositeT);
			float2 val6 = Tangent(oppositeStartCurve, oppositeEndCurve, oppositeT);
			float2 val7 = val5 - val;
			MathUtils.TryNormalize(ref intersectTangent);
			MathUtils.TryNormalize(ref oppositeIntersectTangent);
			MathUtils.TryNormalize(ref val2);
			MathUtils.TryNormalize(ref val4);
			MathUtils.TryNormalize(ref val6);
			MathUtils.TryNormalize(ref val7);
			float num = math.dot(intersectTangent, val2);
			float num2 = math.dot(intersectTangent, val4);
			math.dot(oppositeIntersectTangent, val2);
			math.dot(oppositeIntersectTangent, val6);
			Segment val8 = default(Segment);
			((Segment)(ref val8))._002Ector(val5, val5 - oppositeIntersectTangent * (math.distance(val, val3) * 3f));
			float num3 = t;
			float2 val9 = default(float2);
			if (MathUtils.Intersect(startCurve, val8, ref val9, 4))
			{
				num3 = math.max(num3, val9.x);
			}
			if (MathUtils.Intersect(endCurve, val8, ref val9, 4))
			{
				num3 = math.max(num3, val9.x + 1f);
			}
			if (num > 0f && num2 > 0f)
			{
				num2 = 1f;
			}
			float num4 = 0f;
			float num5 = math.acos(math.saturate(math.dot(val6, val7)));
			float num6 = math.acos(math.saturate(math.dot(val2, val7)));
			float num7 = (float)Math.PI / 2f;
			if (num6 > 0.0001f)
			{
				num7 = math.min(num7, math.sin(num5) / math.tan(num6) * num5);
			}
			if (num7 > num6 && num5 > num6)
			{
				num4 = math.lerp(t, oppositeT, (num7 - num6) / ((float)Math.PI / 2f - num6));
			}
			t = math.lerp(num3, t, math.saturate(num2));
			t = math.lerp(t, oppositeT, math.saturate(math.dot(intersectTangent, oppositeIntersectTangent)));
			t = math.min(oppositeT, math.max(num4, t));
		}

		private float2 Position(Bezier4x2 startCurve, Bezier4x2 endCurve, float t)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (t < 1f)
			{
				return MathUtils.Position(startCurve, math.max(0f, t));
			}
			return MathUtils.Position(endCurve, math.min(1f, t - 1f));
		}

		private float2 Tangent(Bezier4x2 startCurve, Bezier4x2 endCurve, float t)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (t < 1f)
			{
				return MathUtils.Tangent(startCurve, math.max(0f, t));
			}
			return MathUtils.Tangent(endCurve, math.min(1f, t - 1f));
		}

		private float2 Intersect(Bezier4x2 startCurve1, Bezier4x2 endCurve1, Bezier4x2 nodeCurve2, Bezier4x2 startCurve2, Bezier4x2 endCurve2)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			float2 t = default(float2);
			Intersect(startCurve1, nodeCurve2, new float2(0f, -1f), ref t);
			Intersect(startCurve1, startCurve2, new float2(0f, 0f), ref t);
			Intersect(startCurve1, endCurve2, new float2(0f, 1f), ref t);
			Intersect(endCurve1, nodeCurve2, new float2(1f, -1f), ref t);
			Intersect(endCurve1, startCurve2, new float2(1f, 0f), ref t);
			Intersect(endCurve1, endCurve2, new float2(1f, 1f), ref t);
			return t;
		}

		private void Intersect(Bezier4x2 curve1, Bezier4x2 curve2, float2 offset, ref float2 t)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			if (MathUtils.Intersect(curve1, curve2, ref val, 4))
			{
				val += offset;
				if (val.x > t.x)
				{
					t = val;
				}
			}
		}

		private float CheckCrosswalks(DynamicBuffer<NetCompositionCrosswalk> prefabCompositionCrosswalks)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = 0; i < prefabCompositionCrosswalks.Length; i++)
			{
				NetCompositionCrosswalk netCompositionCrosswalk = prefabCompositionCrosswalks[i];
				num = math.max(num, math.max(netCompositionCrosswalk.m_Start.z, netCompositionCrosswalk.m_End.z));
			}
			return num;
		}

		private float CompareLanes(DynamicBuffer<NetCompositionLane> prefabCompositionLanes1, DynamicBuffer<NetCompositionLane> prefabCompositionLanes2, float offset, bool isEnd1, bool isEnd2, bool dontCrossTracks)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			GetLaneLimits(prefabCompositionLanes1, isEnd1, !isEnd1, out var minRoadLimits, out var maxRoadLimits, out var trackLimits, out var masterLanes);
			GetLaneLimits(prefabCompositionLanes2, isEnd2, isEnd2, out var minRoadLimits2, out var maxRoadLimits2, out var trackLimits2, out var masterLanes2);
			float4 val = math.abs(minRoadLimits - minRoadLimits2 + offset);
			float4 val2 = math.abs(minRoadLimits - maxRoadLimits2 + offset);
			float4 val3 = math.abs(maxRoadLimits - minRoadLimits2 + offset);
			float4 val4 = math.abs(maxRoadLimits - maxRoadLimits2 + offset);
			val = math.max(math.max(val, val2), math.max(val3, val4));
			float4 val5 = math.abs(trackLimits - trackLimits2 + offset);
			float4 val6 = math.abs(trackLimits - ((float4)(ref trackLimits2)).yxwz + offset);
			val = math.select(val, float4.op_Implicit(0f), (math.abs(minRoadLimits) > 100000f) | (math.abs(minRoadLimits2) > 100000f));
			val5 = math.select(val5, float4.op_Implicit(0f), (math.abs(trackLimits) > 100000f) | (math.abs(trackLimits2) > 100000f));
			val6 = math.select(val6, float4.op_Implicit(0f), (math.abs(trackLimits) > 100000f) | (math.abs(((float4)(ref trackLimits2)).yxwz) > 100000f));
			val6 = math.select(val6, val5, dontCrossTracks);
			float2 val7 = default(float2);
			((float2)(ref val7))._002Ector(math.cmax(val), math.cmax(math.max(val5, val6)));
			float num = math.cmax(math.max(masterLanes, masterLanes2)) * 0.5f;
			num = math.select(num, 0f, math.all(masterLanes == 0f == (masterLanes2 == 0f)));
			val7.x = math.max(val7.x, num);
			return math.cmax(math.sqrt(val7) * new float2(3f, 4f));
		}

		private void GetLaneLimits(DynamicBuffer<NetCompositionLane> prefabCompositionLanes, bool isEnd, bool invert, out float4 minRoadLimits, out float4 maxRoadLimits, out float4 trackLimits, out float2 masterLanes)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			((float4)(ref val))._002Ector(1000000f, -1000000f, 1000000f, -1000000f);
			minRoadLimits = new float4(1000000f, 1000000f, 1000000f, 1000000f);
			maxRoadLimits = new float4(-1000000f, -1000000f, -1000000f, -1000000f);
			trackLimits = val;
			masterLanes = float2.op_Implicit(0f);
			float4 val2 = val;
			int num = -1;
			NetLaneData netLaneData = default(NetLaneData);
			for (int i = 0; i < prefabCompositionLanes.Length; i++)
			{
				NetCompositionLane netCompositionLane = prefabCompositionLanes[i];
				LaneFlags laneFlags = (((netCompositionLane.m_Flags & LaneFlags.Invert) != 0 != isEnd) ? LaneFlags.DisconnectedEnd : LaneFlags.DisconnectedStart);
				if ((netCompositionLane.m_Flags & laneFlags) != 0)
				{
					continue;
				}
				netCompositionLane.m_Position.x = math.select(netCompositionLane.m_Position.x, 0f - netCompositionLane.m_Position.x, invert);
				if ((netCompositionLane.m_Flags & LaneFlags.Road) != 0)
				{
					if ((netCompositionLane.m_Flags & LaneFlags.Master) != 0)
					{
						if (m_NetLaneData.TryGetComponent(netCompositionLane.m_Lane, ref netLaneData))
						{
							if ((netCompositionLane.m_Flags & LaneFlags.Twoway) != 0)
							{
								masterLanes = math.max(masterLanes, float2.op_Implicit(netLaneData.m_Width));
							}
							else if ((netCompositionLane.m_Flags & LaneFlags.Invert) != 0 == invert)
							{
								masterLanes.x = math.max(masterLanes.x, netLaneData.m_Width);
							}
							else
							{
								masterLanes.y = math.max(masterLanes.y, netLaneData.m_Width);
							}
						}
						continue;
					}
					if (netCompositionLane.m_Carriageway != num)
					{
						minRoadLimits = math.select(minRoadLimits, val2, (val2 < minRoadLimits) & (val2 != val));
						maxRoadLimits = math.select(maxRoadLimits, val2, (val2 > maxRoadLimits) & (val2 != val));
						val2 = val;
						num = netCompositionLane.m_Carriageway;
					}
					if ((netCompositionLane.m_Flags & LaneFlags.Twoway) != 0)
					{
						((float4)(ref val2)).xz = math.min(((float4)(ref val2)).xz, float2.op_Implicit(netCompositionLane.m_Position.x));
						((float4)(ref val2)).yw = math.max(((float4)(ref val2)).yw, float2.op_Implicit(netCompositionLane.m_Position.x));
					}
					else if ((netCompositionLane.m_Flags & LaneFlags.Invert) != 0 == invert)
					{
						val2.x = math.min(val2.x, netCompositionLane.m_Position.x);
						val2.y = math.max(val2.y, netCompositionLane.m_Position.x);
					}
					else
					{
						val2.z = math.min(val2.z, netCompositionLane.m_Position.x);
						val2.w = math.max(val2.w, netCompositionLane.m_Position.x);
					}
				}
				else if ((netCompositionLane.m_Flags & LaneFlags.Track) != 0)
				{
					if ((netCompositionLane.m_Flags & LaneFlags.Twoway) != 0)
					{
						((float4)(ref trackLimits)).xz = math.min(((float4)(ref trackLimits)).xz, float2.op_Implicit(netCompositionLane.m_Position.x));
						((float4)(ref trackLimits)).yw = math.max(((float4)(ref trackLimits)).yw, float2.op_Implicit(netCompositionLane.m_Position.x));
					}
					else if ((netCompositionLane.m_Flags & LaneFlags.Invert) != 0 == invert)
					{
						trackLimits.x = math.min(trackLimits.x, netCompositionLane.m_Position.x);
						trackLimits.y = math.max(trackLimits.y, netCompositionLane.m_Position.x);
					}
					else
					{
						trackLimits.z = math.min(trackLimits.z, netCompositionLane.m_Position.x);
						trackLimits.w = math.max(trackLimits.w, netCompositionLane.m_Position.x);
					}
				}
			}
			minRoadLimits = math.select(minRoadLimits, val2, (val2 < minRoadLimits) & (val2 != val));
			maxRoadLimits = math.select(maxRoadLimits, val2, (val2 > maxRoadLimits) & (val2 != val));
		}

		private bool RequireTransition(NetCompositionData nodeCompositionData, NetCompositionData nodeCompositionData2, NetCompositionData edgeCompositionData, NetCompositionData edgeCompositionData2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (math.abs(nodeCompositionData.m_HeightRange.min - nodeCompositionData2.m_HeightRange.min) > 0.1f || math.abs(nodeCompositionData.m_HeightRange.max - nodeCompositionData2.m_HeightRange.max) > 0.1f)
			{
				return true;
			}
			CompositionFlags compositionFlags = new CompositionFlags(CompositionFlags.General.StyleBreak, CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition, CompositionFlags.Side.LowTransition | CompositionFlags.Side.HighTransition);
			if (((nodeCompositionData.m_Flags | nodeCompositionData2.m_Flags) & compositionFlags) != default(CompositionFlags))
			{
				return true;
			}
			CompositionFlags compositionFlags2 = new CompositionFlags(CompositionFlags.General.Pavement | CompositionFlags.General.Gravel | CompositionFlags.General.Tiles, (CompositionFlags.Side)0u, (CompositionFlags.Side)0u);
			if ((edgeCompositionData.m_Flags & compositionFlags2) != (edgeCompositionData2.m_Flags & compositionFlags2))
			{
				return true;
			}
			return false;
		}
	}

	private struct EdgeData
	{
		public float3 m_Left;

		public float3 m_Right;

		public Layer m_Layers;

		public Entity m_Entity;

		public float2 m_Changes;

		public float m_MaxSlope;

		public bool m_IsEnd;

		public bool m_IsTemp;
	}

	[BurstCompile]
	private struct AllocateBuffersJob : IJob
	{
		[ReadOnly]
		public NativeList<Entity> m_Entities;

		public NativeList<IntersectionData> m_IntersectionData;

		public NativeParallelHashMap<int2, float4> m_EdgeHeightMap;

		public void Execute()
		{
			m_IntersectionData.ResizeUninitialized(m_Entities.Length);
			m_EdgeHeightMap.Capacity = m_Entities.Length * 2;
		}
	}

	[BurstCompile]
	private struct FlattenNodeGeometryJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		public ParallelWriter<int2, float4> m_EdgeHeightMap;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0837: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0869: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0903: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_0936: Unknown result type (might be due to invalid IL or missing references)
			//IL_093b: Unknown result type (might be due to invalid IL or missing references)
			//IL_094c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0951: Unknown result type (might be due to invalid IL or missing references)
			//IL_088c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0891: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08da: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_0661: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_069e: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0706: Unknown result type (might be due to invalid IL or missing references)
			//IL_072a: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<NodeGeometry> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
			NativeList<EdgeData> val = default(NativeList<EdgeData>);
			val._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Temp>(ref m_TempType);
			EdgeGeometry edgeGeometry2 = default(EdgeGeometry);
			int2 val2 = default(int2);
			float4 val3 = default(float4);
			float2 val6 = default(float2);
			float2 val7 = default(float2);
			float2 val11 = default(float2);
			int2 val12 = default(int2);
			float4 val13 = default(float4);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity node = nativeArray[i];
				NodeGeometry nodeGeometry = nativeArray2[i];
				if (nodeGeometry.m_Bounds.min.x != 0f)
				{
					continue;
				}
				bool flag2 = true;
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					PrefabRef prefabRef = m_PrefabRefDataFromEntity[value.m_Edge];
					NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					if (netGeometryData.m_MergeLayers != Layer.None)
					{
						EdgeGeometry edgeGeometry = m_EdgeGeometryData[value.m_Edge];
						bool flag3 = m_TempData.HasComponent(value.m_Edge);
						flag2 = flag2 && flag3;
						EdgeData edgeData = new EdgeData
						{
							m_Left = (value.m_End ? edgeGeometry.m_End.m_Left.d : edgeGeometry.m_Start.m_Right.a),
							m_Right = (value.m_End ? edgeGeometry.m_End.m_Right.d : edgeGeometry.m_Start.m_Left.a),
							m_Layers = netGeometryData.m_MergeLayers,
							m_Entity = value.m_Edge,
							m_MaxSlope = netGeometryData.m_MaxSlopeSteepness,
							m_IsEnd = value.m_End,
							m_IsTemp = flag3
						};
						val.Add(ref edgeData);
					}
				}
				if (flag && !flag2)
				{
					for (int j = 0; j < val.Length; j++)
					{
						EdgeData edgeData2 = val[j];
						if (!edgeData2.m_IsTemp)
						{
							continue;
						}
						Temp temp = m_TempData[edgeData2.m_Entity];
						if (m_EdgeGeometryData.TryGetComponent(temp.m_Original, ref edgeGeometry2))
						{
							EdgeGeometry edgeGeometry3 = m_EdgeGeometryData[edgeData2.m_Entity];
							((int2)(ref val2))._002Ector(edgeData2.m_Entity.Index, math.select(0, 1, edgeData2.m_IsEnd));
							if (edgeData2.m_IsEnd)
							{
								((float4)(ref val3))._002Ector(((float3)(ref edgeGeometry2.m_End.m_Left.d)).yy, ((float3)(ref edgeGeometry2.m_End.m_Right.d)).yy);
								val3.x += edgeGeometry3.m_End.m_Left.c.y - edgeGeometry3.m_End.m_Left.d.y;
								val3.z += edgeGeometry3.m_End.m_Right.c.y - edgeGeometry3.m_End.m_Right.d.y;
							}
							else
							{
								((float4)(ref val3))._002Ector(((float3)(ref edgeGeometry2.m_Start.m_Right.a)).yy, ((float3)(ref edgeGeometry2.m_Start.m_Left.a)).yy);
								val3.x += edgeGeometry3.m_Start.m_Right.b.y - edgeGeometry3.m_Start.m_Right.a.y;
								val3.z += edgeGeometry3.m_Start.m_Left.b.y - edgeGeometry3.m_Start.m_Left.a.y;
							}
							m_EdgeHeightMap.TryAdd(val2, val3);
						}
					}
				}
				else
				{
					bool flag4 = false;
					for (int k = 0; k < 100; k++)
					{
						bool flag5 = false;
						for (int l = 1; l < val.Length; l++)
						{
							ref EdgeData reference = ref val.ElementAt(l);
							for (int m = 0; m < l; m++)
							{
								ref EdgeData reference2 = ref val.ElementAt(m);
								if ((reference.m_Layers & reference2.m_Layers) == 0)
								{
									continue;
								}
								float3 val4 = reference2.m_Right - reference.m_Left;
								float3 val5 = reference2.m_Left - reference.m_Right;
								((float2)(ref val6))._002Ector(math.lengthsq(((float3)(ref val4)).xz), math.lengthsq(((float3)(ref val5)).xz));
								((float2)(ref val7))._002Ector(val4.y, val5.y);
								float num = reference.m_MaxSlope + reference2.m_MaxSlope;
								if (math.any(val7 * val7 > val6 * (num * num * 1.0001f)))
								{
									val6 = math.sqrt(val6);
									float2 val8 = math.abs(val7);
									float2 val9 = math.max(float2.op_Implicit(0f), val8 - val6 * num);
									bool2 val10 = val7 >= 0f;
									float num2;
									if (val10.x != val10.y)
									{
										val9 = math.select(-val9, val9, val10);
										num2 = math.csum(val9) * 0.5f;
									}
									else
									{
										num2 = math.max(val9.x, val9.y);
										num2 = math.select(0f - num2, num2, val10.x);
									}
									if (num2 >= 0f)
									{
										val11.x = math.max(reference.m_Left.y, reference.m_Right.y);
										val11.y = math.min(reference2.m_Left.y, reference2.m_Right.y);
										val11 = nodeGeometry.m_Position - val11;
										val11.x = math.max(0f, val11.x);
										val11.y = math.min(0f, val11.y);
									}
									else
									{
										val11.x = math.min(reference.m_Left.y, reference.m_Right.y);
										val11.y = math.max(reference2.m_Left.y, reference2.m_Right.y);
										val11 = nodeGeometry.m_Position - val11;
										val11.x = math.min(0f, val11.x);
										val11.y = math.max(0f, val11.y);
									}
									val11 = math.select(val11, float2.op_Implicit(0f), flag != new bool2(reference.m_IsTemp, reference2.m_IsTemp));
									val9 = val11 * math.min(1f, math.abs(num2) / math.max(0.001f, math.csum(math.abs(val11))));
									reference.m_Changes.x = math.min(reference.m_Changes.x, val9.x);
									reference.m_Changes.y = math.max(reference.m_Changes.y, val9.x);
									reference2.m_Changes.x = math.min(reference2.m_Changes.x, val9.y);
									reference2.m_Changes.y = math.max(reference2.m_Changes.y, val9.y);
									flag5 = true;
								}
							}
						}
						if (!flag5)
						{
							break;
						}
						for (int n = 0; n < val.Length; n++)
						{
							ref EdgeData reference3 = ref val.ElementAt(n);
							float num3 = math.csum(reference3.m_Changes);
							reference3.m_Left.y += num3;
							reference3.m_Right.y += num3;
							reference3.m_Changes = float2.op_Implicit(0f);
						}
						flag4 = true;
					}
					if (flag4)
					{
						for (int num4 = 0; num4 < val.Length; num4++)
						{
							EdgeData edgeData3 = val[num4];
							if (flag == edgeData3.m_IsTemp)
							{
								EdgeGeometry edgeGeometry4 = m_EdgeGeometryData[edgeData3.m_Entity];
								((int2)(ref val12))._002Ector(edgeData3.m_Entity.Index, math.select(0, 1, edgeData3.m_IsEnd));
								((float4)(ref val13))._002Ector(((float3)(ref edgeData3.m_Left)).yy, ((float3)(ref edgeData3.m_Right)).yy);
								if (edgeData3.m_IsEnd)
								{
									val13.x += edgeGeometry4.m_End.m_Left.c.y - edgeGeometry4.m_End.m_Left.d.y;
									val13.z += edgeGeometry4.m_End.m_Right.c.y - edgeGeometry4.m_End.m_Right.d.y;
								}
								else
								{
									val13.x += edgeGeometry4.m_Start.m_Right.b.y - edgeGeometry4.m_Start.m_Right.a.y;
									val13.z += edgeGeometry4.m_Start.m_Left.b.y - edgeGeometry4.m_Start.m_Left.a.y;
								}
								m_EdgeHeightMap.TryAdd(val12, val13);
							}
						}
					}
				}
				val.Clear();
			}
			val.Dispose();
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct FinishEdgeGeometryJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionDataFromEntity;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public NativeParallelHashMap<int2, float4> m_EdgeHeightMap;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			Composition composition = m_CompositionDataFromEntity[val];
			PrefabRef prefabRef = m_PrefabRefDataFromEntity[val];
			NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
			EdgeGeometry edgeGeometry = m_EdgeGeometryData[val];
			float4 val2 = default(float4);
			if (m_EdgeHeightMap.TryGetValue(new int2(val.Index, 0), ref val2))
			{
				edgeGeometry.m_Start.m_Right.b.y = val2.x;
				edgeGeometry.m_Start.m_Right.a.y = val2.y;
				edgeGeometry.m_Start.m_Left.b.y = val2.z;
				edgeGeometry.m_Start.m_Left.a.y = val2.w;
			}
			float4 val3 = default(float4);
			if (m_EdgeHeightMap.TryGetValue(new int2(val.Index, 1), ref val3))
			{
				edgeGeometry.m_End.m_Left.c.y = val3.x;
				edgeGeometry.m_End.m_Left.d.y = val3.y;
				edgeGeometry.m_End.m_Right.c.y = val3.z;
				edgeGeometry.m_End.m_Right.d.y = val3.w;
			}
			if ((netGeometryData.m_Flags & GeometryFlags.SmoothSlopes) == 0)
			{
				if (!(((netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) == 0) & ((netCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0 || (netCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0) & !m_OwnerData.HasComponent(val)))
				{
					StraightenMiddleHeights(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_End.m_Left);
					StraightenMiddleHeights(ref edgeGeometry.m_Start.m_Right, ref edgeGeometry.m_End.m_Right);
				}
				else
				{
					LimitMiddleHeights(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_End.m_Left, netGeometryData.m_MaxSlopeSteepness, netCompositionData.m_Width);
					LimitMiddleHeights(ref edgeGeometry.m_Start.m_Right, ref edgeGeometry.m_End.m_Right, netGeometryData.m_MaxSlopeSteepness, netCompositionData.m_Width);
				}
			}
			else
			{
				LimitMiddleHeights(ref edgeGeometry.m_Start.m_Left, ref edgeGeometry.m_End.m_Left, netGeometryData.m_MaxSlopeSteepness, netCompositionData.m_Width);
				LimitMiddleHeights(ref edgeGeometry.m_Start.m_Right, ref edgeGeometry.m_End.m_Right, netGeometryData.m_MaxSlopeSteepness, netCompositionData.m_Width);
			}
			edgeGeometry.m_Start.m_Length.x = MathUtils.Length(edgeGeometry.m_Start.m_Left);
			edgeGeometry.m_Start.m_Length.y = MathUtils.Length(edgeGeometry.m_Start.m_Right);
			edgeGeometry.m_End.m_Length.x = MathUtils.Length(edgeGeometry.m_End.m_Left);
			edgeGeometry.m_End.m_Length.y = MathUtils.Length(edgeGeometry.m_End.m_Right);
			edgeGeometry.m_Bounds = MathUtils.TightBounds(edgeGeometry.m_Start.m_Left) | MathUtils.TightBounds(edgeGeometry.m_Start.m_Right) | MathUtils.TightBounds(edgeGeometry.m_End.m_Left) | MathUtils.TightBounds(edgeGeometry.m_End.m_Right);
			edgeGeometry.m_Bounds.min.y += netCompositionData.m_HeightRange.min;
			edgeGeometry.m_Bounds.max.y += netCompositionData.m_HeightRange.max;
			if ((netCompositionData.m_State & (CompositionState.LowerToTerrain | CompositionState.RaiseToTerrain)) != 0)
			{
				Bounds1 val4 = SampleTerrain(edgeGeometry.m_Start.m_Left) | SampleTerrain(edgeGeometry.m_Start.m_Right) | SampleTerrain(edgeGeometry.m_End.m_Left) | SampleTerrain(edgeGeometry.m_End.m_Right);
				if ((netCompositionData.m_State & CompositionState.LowerToTerrain) != 0)
				{
					edgeGeometry.m_Bounds.min.y = math.min(edgeGeometry.m_Bounds.min.y, val4.min);
				}
				if ((netCompositionData.m_State & CompositionState.RaiseToTerrain) != 0)
				{
					edgeGeometry.m_Bounds.max.y = math.max(edgeGeometry.m_Bounds.max.y, val4.max);
				}
			}
			m_EdgeGeometryData[val] = edgeGeometry;
		}

		private Bounds1 SampleTerrain(Bezier4x3 curve)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			for (int i = 0; i <= 8; i++)
			{
				val |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, MathUtils.Position(curve, (float)i * 0.125f));
			}
			return val;
		}

		private void StraightenMiddleHeights(ref Bezier4x3 start, ref Bezier4x3 end)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			float4 val = default(float4);
			val.x = math.distance(((float3)(ref start.b)).xz, ((float3)(ref start.c)).xz);
			val.y = val.x + math.distance(((float3)(ref start.c)).xz, ((float3)(ref start.d)).xz);
			val.z = val.y + math.distance(((float3)(ref end.a)).xz, ((float3)(ref end.b)).xz);
			val.w = val.z + math.distance(((float3)(ref end.b)).xz, ((float3)(ref end.c)).xz);
			val = math.select(val / val.w, float4.op_Implicit(0f), val.w == 0f);
			float3 val2 = math.lerp(float3.op_Implicit(start.b.y), float3.op_Implicit(end.c.y), ((float4)(ref val)).xyz);
			start.c.y = val2.x;
			start.d.y = val2.y;
			end.a.y = val2.y;
			end.b.y = val2.z;
		}

		private void LimitMiddleHeights(ref Bezier4x3 start, ref Bezier4x3 end, float maxSlope, float width)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			float num = MathUtils.Length(((Bezier4x3)(ref start)).xz);
			float num2 = MathUtils.Length(((Bezier4x3)(ref end)).xz);
			float num3 = num * maxSlope;
			float num4 = num2 * maxSlope;
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(math.max(start.a.y - num3, end.d.y - num4), math.min(start.a.y + num3, end.d.y + num4));
			if (val.max < val.min)
			{
				((Bounds1)(ref val))._002Ector(float2.op_Implicit((val.min + val.max) * 0.5f));
			}
			else
			{
				float num5 = (val.min + val.max) * 0.5f;
				float num6 = 1f / (0.5f * (num + num2) / math.max(0.01f, width) + 1f);
				val.min = math.lerp(val.min, num5, num6);
				val.max = math.lerp(val.max, num5, num6);
			}
			float num7 = MathUtils.Clamp(start.d.y, val) - start.d.y;
			start.c.y += num7;
			start.d.y += num7;
			end.a.y += num7;
			end.b.y += num7;
		}
	}

	[BurstCompile]
	private struct CalculateNodeGeometryJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionDataFromEntity;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_GeometryDataFromEntity;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public int m_IterationIndex;

		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_061c: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0884: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_082d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_0839: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0857: Unknown result type (might be due to invalid IL or missing references)
			//IL_085e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_086c: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08be: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0905: Unknown result type (might be due to invalid IL or missing references)
			//IL_090a: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_092b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_095e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_096a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_097a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[val];
			EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[val];
			if (m_IterationIndex == 1 && startNodeGeometry.m_Geometry.m_Left.m_Length.x >= 0f && startNodeGeometry.m_Geometry.m_Right.m_Length.x >= 0f && endNodeGeometry.m_Geometry.m_Left.m_Length.x >= 0f && endNodeGeometry.m_Geometry.m_Right.m_Length.x >= 0f)
			{
				return;
			}
			Edge edge = m_EdgeDataFromEntity[val];
			Composition composition = m_CompositionDataFromEntity[val];
			EdgeGeometry edgeGeometry = m_GeometryDataFromEntity[val];
			PrefabRef prefabRef = m_PrefabRefDataFromEntity[val];
			NetGeometryData prefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			NetCompositionData edgeCompositionData = m_PrefabCompositionData[composition.m_Edge];
			NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_StartNode];
			NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_EndNode];
			NodeGeometry nodeGeometry = m_NodeGeometryData[edge.m_Start];
			NodeGeometry nodeGeometry2 = m_NodeGeometryData[edge.m_End];
			float3 val2 = FindMiddleNodePos(val, edge.m_Start);
			val2.y = nodeGeometry.m_Position;
			float3 val3 = FindMiddleNodePos(val, edge.m_End);
			val3.y = nodeGeometry2.m_Position;
			float2 offset = StartOffset(edgeGeometry.m_Start, ((float3)(ref val2)).xz);
			float2 offset2 = EndOffset(edgeGeometry.m_End, ((float3)(ref val3)).xz);
			FindTargetSegments(val, edge.m_Start, offset, ((float3)(ref val2)).xz, prefabGeometryData, netCompositionData, out var leftSegment, out var rightSegment, out var distances, out var syncVertexOffsetsLeft, out var syncVertexOffsetsRight, out var roundaboutSize, out var sideConnect, out var middleConnect);
			FindTargetSegments(val, edge.m_End, offset2, ((float3)(ref val3)).xz, prefabGeometryData, netCompositionData2, out var leftSegment2, out var rightSegment2, out var distances2, out var syncVertexOffsetsLeft2, out var syncVertexOffsetsRight2, out var roundaboutSize2, out var sideConnect2, out var middleConnect2);
			Segment segment = Invert(edgeGeometry.m_Start);
			Segment segment2 = edgeGeometry.m_End;
			AdjustSegmentWidth(ref segment, edgeCompositionData, netCompositionData, invertEdge: true);
			AdjustSegmentWidth(ref segment2, edgeCompositionData, netCompositionData2, invertEdge: false);
			Segment segment3 = segment;
			Segment segment4 = segment2;
			float num = netCompositionData.m_MiddleOffset / math.max(0.01f, netCompositionData.m_Width) + 0.5f;
			float num2 = netCompositionData2.m_MiddleOffset / math.max(0.01f, netCompositionData2.m_Width) + 0.5f;
			segment.m_Right = MathUtils.Lerp(segment.m_Left, segment.m_Right, num);
			segment3.m_Left = MathUtils.Lerp(segment3.m_Left, segment3.m_Right, num);
			segment2.m_Right = MathUtils.Lerp(segment2.m_Left, segment2.m_Right, num2);
			segment4.m_Left = MathUtils.Lerp(segment4.m_Left, segment4.m_Right, num2);
			float leftRadius = 0f;
			float rightRadius = 0f;
			float leftRadius2 = 0f;
			float rightRadius2 = 0f;
			startNodeGeometry.m_Geometry.m_Middle = default(Bezier4x3);
			endNodeGeometry.m_Geometry.m_Middle = default(Bezier4x3);
			float2 divPos;
			float2 divPos2;
			if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float3 position = m_NodeDataFromEntity[edge.m_Start].m_Position;
				position.y = nodeGeometry.m_Position;
				leftRadius = startNodeGeometry.m_Geometry.m_MiddleRadius + roundaboutSize;
				rightRadius = leftRadius;
				CalculateSegments(segment, segment3, leftSegment, rightSegment, position, ref leftRadius, ref rightRadius, out startNodeGeometry.m_Geometry.m_Left, out startNodeGeometry.m_Geometry.m_Right, out divPos, out divPos2);
			}
			else
			{
				if (sideConnect.x)
				{
					startNodeGeometry.m_Geometry.m_Left = CalculateSideConnection(segment, leftSegment, rightSegment, edgeCompositionData, netCompositionData, right: false, out divPos);
					startNodeGeometry.m_Geometry.m_Middle.d.x = 1f;
				}
				else if (middleConnect.x)
				{
					if (m_IterationIndex == 0)
					{
						startNodeGeometry.m_Geometry.m_Left.m_Length = float2.op_Implicit(-1f);
						divPos = float2.op_Implicit(-1f);
					}
					else
					{
						startNodeGeometry.m_Geometry.m_Left = CalculateMiddleConnection(segment, leftSegment, out divPos);
						startNodeGeometry.m_Geometry.m_Middle.d.x = 1f;
					}
				}
				else
				{
					startNodeGeometry.m_Geometry.m_Left = CalculateMiddleSegment(segment, leftSegment, out divPos);
				}
				if (sideConnect.y)
				{
					startNodeGeometry.m_Geometry.m_Right = CalculateSideConnection(segment3, leftSegment, rightSegment, edgeCompositionData, netCompositionData, right: true, out divPos2);
					startNodeGeometry.m_Geometry.m_Middle.d.y = 1f;
				}
				else if (middleConnect.y)
				{
					if (m_IterationIndex == 0)
					{
						startNodeGeometry.m_Geometry.m_Right.m_Length = float2.op_Implicit(-1f);
						divPos2 = float2.op_Implicit(-1f);
					}
					else
					{
						startNodeGeometry.m_Geometry.m_Right = CalculateMiddleConnection(segment3, rightSegment, out divPos2);
						startNodeGeometry.m_Geometry.m_Middle.d.y = 1f;
					}
				}
				else
				{
					startNodeGeometry.m_Geometry.m_Right = CalculateMiddleSegment(segment3, rightSegment, out divPos2);
				}
			}
			float2 divPos3;
			float2 divPos4;
			if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float3 position2 = m_NodeDataFromEntity[edge.m_End].m_Position;
				position2.y = nodeGeometry2.m_Position;
				leftRadius2 = endNodeGeometry.m_Geometry.m_MiddleRadius + roundaboutSize2;
				rightRadius2 = leftRadius2;
				CalculateSegments(segment2, segment4, leftSegment2, rightSegment2, position2, ref leftRadius2, ref rightRadius2, out endNodeGeometry.m_Geometry.m_Left, out endNodeGeometry.m_Geometry.m_Right, out divPos3, out divPos4);
			}
			else
			{
				if (sideConnect2.x)
				{
					endNodeGeometry.m_Geometry.m_Left = CalculateSideConnection(segment2, leftSegment2, rightSegment2, edgeCompositionData, netCompositionData2, right: false, out divPos3);
					endNodeGeometry.m_Geometry.m_Middle.d.x = 1f;
				}
				else if (middleConnect2.x)
				{
					if (m_IterationIndex == 0)
					{
						endNodeGeometry.m_Geometry.m_Left.m_Length = float2.op_Implicit(-1f);
						divPos3 = float2.op_Implicit(-1f);
					}
					else
					{
						endNodeGeometry.m_Geometry.m_Left = CalculateMiddleConnection(segment2, leftSegment2, out divPos3);
						endNodeGeometry.m_Geometry.m_Middle.d.x = 1f;
					}
				}
				else
				{
					endNodeGeometry.m_Geometry.m_Left = CalculateMiddleSegment(segment2, leftSegment2, out divPos3);
				}
				if (sideConnect2.y)
				{
					endNodeGeometry.m_Geometry.m_Right = CalculateSideConnection(segment4, leftSegment2, rightSegment2, edgeCompositionData, netCompositionData2, right: true, out divPos4);
					endNodeGeometry.m_Geometry.m_Middle.d.y = 1f;
				}
				else if (middleConnect2.y)
				{
					if (m_IterationIndex == 0)
					{
						endNodeGeometry.m_Geometry.m_Right.m_Length = float2.op_Implicit(-1f);
						divPos4 = float2.op_Implicit(-1f);
					}
					else
					{
						endNodeGeometry.m_Geometry.m_Right = CalculateMiddleConnection(segment4, rightSegment2, out divPos4);
						endNodeGeometry.m_Geometry.m_Middle.d.y = 1f;
					}
				}
				else
				{
					endNodeGeometry.m_Geometry.m_Right = CalculateMiddleSegment(segment4, rightSegment2, out divPos4);
				}
			}
			float2 val4 = netCompositionData.m_Width * 0.5f + new float2(netCompositionData.m_MiddleOffset, 0f - netCompositionData.m_MiddleOffset);
			float2 val5 = netCompositionData2.m_Width * 0.5f + new float2(netCompositionData2.m_MiddleOffset, 0f - netCompositionData2.m_MiddleOffset);
			if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float3 position3 = m_NodeDataFromEntity[edge.m_Start].m_Position;
				position3.y = nodeGeometry.m_Position;
				startNodeGeometry.m_Geometry.m_SyncVertexTargetsLeft = CalculateVertexSyncTarget(ref startNodeGeometry.m_Geometry.m_Right, position3, leftRadius, isRight: false, val4.x, distances.x, netCompositionData.m_SyncVertexOffsetsLeft, syncVertexOffsetsLeft, divPos.y);
				startNodeGeometry.m_Geometry.m_SyncVertexTargetsRight = CalculateVertexSyncTarget(ref startNodeGeometry.m_Geometry.m_Right, position3, rightRadius, isRight: true, val4.y, distances.y, netCompositionData.m_SyncVertexOffsetsRight, syncVertexOffsetsRight, divPos2.x);
			}
			else if (!math.any(middleConnect) || m_IterationIndex != 0)
			{
				startNodeGeometry.m_Geometry.m_SyncVertexTargetsLeft = CalculateVertexSyncTarget(ref startNodeGeometry.m_Geometry.m_Left, val4.x, distances.x, netCompositionData.m_SyncVertexOffsetsLeft, syncVertexOffsetsLeft, divPos.y, isRight: false);
				startNodeGeometry.m_Geometry.m_SyncVertexTargetsRight = CalculateVertexSyncTarget(ref startNodeGeometry.m_Geometry.m_Right, val4.y, distances.y, netCompositionData.m_SyncVertexOffsetsRight, syncVertexOffsetsRight, divPos2.x, isRight: true);
			}
			if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float3 position4 = m_NodeDataFromEntity[edge.m_End].m_Position;
				position4.y = nodeGeometry2.m_Position;
				endNodeGeometry.m_Geometry.m_SyncVertexTargetsLeft = CalculateVertexSyncTarget(ref endNodeGeometry.m_Geometry.m_Right, position4, leftRadius2, isRight: false, val5.x, distances2.x, netCompositionData2.m_SyncVertexOffsetsLeft, syncVertexOffsetsLeft2, divPos3.y);
				endNodeGeometry.m_Geometry.m_SyncVertexTargetsRight = CalculateVertexSyncTarget(ref endNodeGeometry.m_Geometry.m_Right, position4, rightRadius2, isRight: true, val5.y, distances2.y, netCompositionData2.m_SyncVertexOffsetsRight, syncVertexOffsetsRight2, divPos4.x);
			}
			else if (!math.any(middleConnect2) || m_IterationIndex != 0)
			{
				endNodeGeometry.m_Geometry.m_SyncVertexTargetsLeft = CalculateVertexSyncTarget(ref endNodeGeometry.m_Geometry.m_Left, val5.x, distances2.x, netCompositionData2.m_SyncVertexOffsetsLeft, syncVertexOffsetsLeft2, divPos3.y, isRight: false);
				endNodeGeometry.m_Geometry.m_SyncVertexTargetsRight = CalculateVertexSyncTarget(ref endNodeGeometry.m_Geometry.m_Right, val5.y, distances2.y, netCompositionData2.m_SyncVertexOffsetsRight, syncVertexOffsetsRight2, divPos4.x, isRight: true);
			}
			m_StartNodeGeometryData[val] = startNodeGeometry;
			m_EndNodeGeometryData[val] = endNodeGeometry;
		}

		private Segment Invert(Segment segment)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Segment result = default(Segment);
			result.m_Left = MathUtils.Invert(segment.m_Right);
			result.m_Right = MathUtils.Invert(segment.m_Left);
			result.m_Length = ((float2)(ref segment.m_Length)).yx;
			return result;
		}

		private float4 CalculateVertexSyncTarget(ref Segment roundaboutSegment, float3 middlePos, float radius, bool isRight, float startDistance, float endDistance, float4 a, float4 b, float t)
		{
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			float num2;
			if (isRight)
			{
				val = 1f - new float2(a.w, b.w);
				float3 val2 = middlePos - roundaboutSegment.m_Right.d;
				float num = (val.y * endDistance - val.x * startDistance) * 0.5f;
				val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz) * num;
				ref float3 c = ref roundaboutSegment.m_Right.c;
				c += val2 * 0.5f;
				ref float3 d = ref roundaboutSegment.m_Right.d;
				d += val2;
				a = math.saturate(1f - (1f - a - val.x) / (1f - val.x));
				b = math.saturate(1f - (1f - b - val.y) / (1f - val.y));
				num2 = math.distance(((float3)(ref middlePos)).xz, ((float3)(ref roundaboutSegment.m_Right.d)).xz);
			}
			else
			{
				((float2)(ref val))._002Ector(a.x, b.x);
				float3 val3 = middlePos - roundaboutSegment.m_Left.d;
				float num3 = (val.y * endDistance - val.x * startDistance) * 0.5f;
				val3 = MathUtils.Normalize(val3, ((float3)(ref val3)).xz) * num3;
				ref float3 c2 = ref roundaboutSegment.m_Left.c;
				c2 += val3 * 0.5f;
				ref float3 d2 = ref roundaboutSegment.m_Left.d;
				d2 += val3;
				a = math.saturate((a - val.x) / (1f - val.x));
				b = math.saturate((b - val.y) / (1f - val.y));
				num2 = math.distance(((float3)(ref middlePos)).xz, ((float3)(ref roundaboutSegment.m_Left.d)).xz);
			}
			float num4 = val.x * startDistance / num2;
			startDistance -= val.x * startDistance;
			endDistance -= val.y * endDistance;
			float4 val4 = default(float4);
			((float4)(ref val4))._002Ector(((float4)(ref a)).xw, ((float4)(ref b)).xw);
			float4 val5 = default(float4);
			((float4)(ref val5))._002Ector(((float4)(ref a)).yz, ((float4)(ref b)).yz);
			float4 val6 = val4;
			float4 val7 = val5;
			bool4 val8 = val4 == val5;
			val4 = math.select(val6, val7, ((bool4)(ref val8)).zwxy);
			((float4)(ref a)).xw = ((float4)(ref val4)).xy;
			((float4)(ref b)).xw = ((float4)(ref val4)).zw;
			float num5 = math.lerp(startDistance, endDistance, t);
			float4 val9 = math.select(a, math.lerp(a * startDistance, b * endDistance, t) / num5, num5 >= float.Epsilon);
			if (isRight)
			{
				val9 = math.saturate((1f - val9) * num5 / radius);
				return math.saturate(1f - num4 - val9 * (1f - num4));
			}
			val9 = math.saturate(val9 * num5 / radius);
			return math.saturate(num4 + val9 * (1f - num4));
		}

		private float4 CalculateVertexSyncTarget(ref Segment startSegment, float startDistance, float endDistance, float4 a, float4 b, float t, bool isRight)
		{
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			float num = (startDistance + endDistance) * 0.5f;
			float2 val = default(float2);
			if (isRight)
			{
				val = 1f - new float2(a.w, b.w);
				float3 val2 = startSegment.m_Left.d - startSegment.m_Right.d;
				float num2 = (val.y * endDistance - val.x * startDistance) * 0.5f;
				num -= num2;
				val2 *= num2 / math.max(0.01f, startDistance);
				ref float3 c = ref startSegment.m_Right.c;
				c += val2 * 0.5f;
				ref float3 d = ref startSegment.m_Right.d;
				d += val2;
				a = math.saturate(1f - (1f - a - val.x) / (1f - val.x));
				b = math.saturate(1f - (1f - b - val.y) / (1f - val.y));
			}
			else
			{
				((float2)(ref val))._002Ector(a.x, b.x);
				float3 val3 = startSegment.m_Right.d - startSegment.m_Left.d;
				float num3 = (val.y * endDistance - val.x * startDistance) * 0.5f;
				num -= num3;
				val3 *= num3 / math.max(0.01f, startDistance);
				ref float3 c2 = ref startSegment.m_Left.c;
				c2 += val3 * 0.5f;
				ref float3 d2 = ref startSegment.m_Left.d;
				d2 += val3;
				a = math.saturate((a - val.x) / (1f - val.x));
				b = math.saturate((b - val.y) / (1f - val.y));
			}
			float num4 = val.x * startDistance / math.max(0.01f, num);
			startDistance -= val.x * startDistance;
			endDistance -= val.y * endDistance;
			float4 val4 = default(float4);
			((float4)(ref val4))._002Ector(((float4)(ref a)).xw, ((float4)(ref b)).xw);
			float4 val5 = default(float4);
			((float4)(ref val5))._002Ector(((float4)(ref a)).yz, ((float4)(ref b)).yz);
			float4 val6 = val4;
			float4 val7 = val5;
			bool4 val8 = val4 == val5;
			val4 = math.select(val6, val7, ((bool4)(ref val8)).zwxy);
			((float4)(ref a)).xw = ((float4)(ref val4)).xy;
			((float4)(ref b)).xw = ((float4)(ref val4)).zw;
			float num5 = math.lerp(startDistance, endDistance, t);
			float4 val9 = math.select(a, math.lerp(a * startDistance, b * endDistance, t) / num5, num5 >= float.Epsilon);
			if (isRight)
			{
				return math.saturate(1f - num4 - (1f - val9) * (1f - num4));
			}
			return math.saturate(num4 + val9 * (1f - num4));
		}

		private float3 FindMiddleNodePos(Entity edge, Entity node)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			((float3)(ref val))._002Ector(1E+09f, 1E+09f, 1E+09f);
			float3 val2 = default(float3);
			((float3)(ref val2))._002Ector(-1E+09f, -1E+09f, -1E+09f);
			EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				EdgeGeometry edgeGeometry = m_GeometryDataFromEntity[value.m_Edge];
				Composition composition = m_CompositionDataFromEntity[value.m_Edge];
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
				float3 val3;
				if (value.m_End)
				{
					float num = 0.5f + netCompositionData.m_MiddleOffset / math.max(0.01f, netCompositionData.m_Width);
					val3 = math.lerp(edgeGeometry.m_End.m_Left.d, edgeGeometry.m_End.m_Right.d, num);
				}
				else
				{
					float num2 = 0.5f + netCompositionData.m_MiddleOffset / math.max(0.01f, netCompositionData.m_Width);
					val3 = math.lerp(edgeGeometry.m_Start.m_Left.a, edgeGeometry.m_Start.m_Right.a, num2);
				}
				val = math.min(val, val3);
				val2 = math.max(val2, val3);
			}
			return math.lerp(val, val2, 0.5f);
		}

		private float2 StartOffset(Segment segment, float2 nodePos)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.lerp(((float3)(ref segment.m_Left.a)).xz, ((float3)(ref segment.m_Right.a)).xz, 0.5f) - nodePos;
			float3 val2 = MathUtils.StartTangent(segment.m_Left);
			float2 xz = ((float3)(ref val2)).xz;
			val2 = MathUtils.StartTangent(segment.m_Right);
			return math.normalizesafe(val + math.normalizesafe(xz + ((float3)(ref val2)).xz, default(float2)), new float2(0f, 1f));
		}

		private float2 EndOffset(Segment segment, float2 nodePos)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.lerp(((float3)(ref segment.m_Left.d)).xz, ((float3)(ref segment.m_Right.d)).xz, 0.5f) - nodePos;
			float3 val2 = MathUtils.EndTangent(segment.m_Left);
			float2 xz = ((float3)(ref val2)).xz;
			val2 = MathUtils.EndTangent(segment.m_Right);
			return math.normalizesafe(val - math.normalizesafe(xz + ((float3)(ref val2)).xz, default(float2)), new float2(0f, 1f));
		}

		private void AdjustSegmentWidth(ref Segment segment, NetCompositionData edgeCompositionData, NetCompositionData nodeCompositionData, bool invertEdge)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			Segment segment2 = segment;
			float num = math.select(edgeCompositionData.m_MiddleOffset, 0f - edgeCompositionData.m_MiddleOffset, invertEdge);
			float2 val = edgeCompositionData.m_Width * 0.5f + num - nodeCompositionData.m_MiddleOffset + nodeCompositionData.m_Width * new float2(-0.5f, 0.5f);
			val /= math.max(0.01f, edgeCompositionData.m_Width);
			val.y -= 1f;
			if (math.abs(val.x) > 0.001f)
			{
				segment.m_Left = MathUtils.Lerp(segment2.m_Left, segment2.m_Right, val.x);
			}
			if (math.abs(val.y) > 0.001f)
			{
				segment.m_Right = MathUtils.Lerp(segment2.m_Right, segment2.m_Left, 0f - val.y);
			}
		}

		private void CheckBestCurve(Bezier4x3 curve, float2 position, bool isRight, bool isEnd, ref Bezier4x3 bestCurve, ref float bestDistance, ref float bestT, ref bool2 bestStartEnd)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			float num2 = default(float);
			float num = MathUtils.Distance(((Bezier4x3)(ref curve)).xz, position, ref num2);
			if (num < bestDistance)
			{
				bestDistance = num;
				bestStartEnd = new bool2(!isEnd, isEnd) & new bool2(num2 < 0.001f, num2 > 0.999f);
				if (isRight)
				{
					bestCurve = curve;
					bestT = num2;
				}
				else
				{
					bestCurve = MathUtils.Invert(curve);
					bestT = 1f - num2;
				}
			}
		}

		private void CheckNodeCurves(Entity node, float2 middlePos, NetGeometryData edgePrefabGeometryData, bool isEnd, ref Bezier4x3 bestCurve, ref float bestDistance, ref float bestT, ref bool2 bestStartEnd)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 bestCurve2 = bestCurve;
			float bestDistance2 = bestDistance;
			float bestT2 = bestT;
			bool2 bestStartEnd2 = bestStartEnd;
			EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData, includeMiddleConnections: true);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				if (value.m_Middle)
				{
					return;
				}
				PrefabRef prefabRef = m_PrefabRefDataFromEntity[value.m_Edge];
				NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
				if ((edgePrefabGeometryData.m_MergeLayers & netGeometryData.m_MergeLayers) == 0)
				{
					continue;
				}
				Composition composition = m_CompositionDataFromEntity[value.m_Edge];
				EdgeNodeGeometry geometry;
				NetCompositionData netCompositionData;
				if (value.m_End)
				{
					geometry = m_EndNodeGeometryData[value.m_Edge].m_Geometry;
					netCompositionData = m_PrefabCompositionData[composition.m_EndNode];
				}
				else
				{
					geometry = m_StartNodeGeometryData[value.m_Edge].m_Geometry;
					netCompositionData = m_PrefabCompositionData[composition.m_StartNode];
				}
				if (!(math.any(geometry.m_Left.m_Length > 0.05f) | math.any(geometry.m_Right.m_Length > 0.05f)))
				{
					continue;
				}
				if (geometry.m_MiddleRadius > 0f)
				{
					if (netCompositionData.m_SideConnectionOffset.x != 0f)
					{
						geometry.m_Left.m_Left = NetUtils.OffsetCurveLeftSmooth(geometry.m_Left.m_Left, float2.op_Implicit(netCompositionData.m_SideConnectionOffset.x));
						geometry.m_Right.m_Left = NetUtils.OffsetCurveLeftSmooth(geometry.m_Right.m_Left, float2.op_Implicit(netCompositionData.m_SideConnectionOffset.x));
					}
					if (netCompositionData.m_SideConnectionOffset.y != 0f)
					{
						geometry.m_Left.m_Right = NetUtils.OffsetCurveLeftSmooth(geometry.m_Left.m_Right, float2.op_Implicit(0f - netCompositionData.m_SideConnectionOffset.y));
						geometry.m_Right.m_Right = NetUtils.OffsetCurveLeftSmooth(geometry.m_Right.m_Right, float2.op_Implicit(0f - netCompositionData.m_SideConnectionOffset.y));
					}
					OffsetCurveHeight(ref geometry.m_Left.m_Left, ((float4)(ref netCompositionData.m_EdgeHeights)).xz, new float2(0f, 0.5f));
					OffsetCurveHeight(ref geometry.m_Left.m_Right, ((float4)(ref netCompositionData.m_EdgeHeights)).yw, new float2(0f, 0.5f));
					OffsetCurveHeight(ref geometry.m_Right.m_Left, ((float4)(ref netCompositionData.m_EdgeHeights)).xz, new float2(0.5f, 1f));
					OffsetCurveHeight(ref geometry.m_Right.m_Right, ((float4)(ref netCompositionData.m_EdgeHeights)).yw, new float2(0.5f, 1f));
					CheckBestCurve(geometry.m_Left.m_Left, middlePos, isRight: false, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
					CheckBestCurve(geometry.m_Left.m_Right, middlePos, isRight: true, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
					CheckBestCurve(geometry.m_Right.m_Left, middlePos, isRight: false, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
					CheckBestCurve(geometry.m_Right.m_Right, middlePos, isRight: true, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
				}
				else
				{
					if (netCompositionData.m_SideConnectionOffset.x != 0f)
					{
						geometry.m_Left.m_Left = NetUtils.OffsetCurveLeftSmooth(geometry.m_Left.m_Left, float2.op_Implicit(netCompositionData.m_SideConnectionOffset.x));
					}
					if (netCompositionData.m_SideConnectionOffset.y != 0f)
					{
						geometry.m_Right.m_Right = NetUtils.OffsetCurveLeftSmooth(geometry.m_Right.m_Right, float2.op_Implicit(0f - netCompositionData.m_SideConnectionOffset.y));
					}
					OffsetCurveHeight(ref geometry.m_Left.m_Left, ((float4)(ref netCompositionData.m_EdgeHeights)).xz, new float2(0f, 1f));
					OffsetCurveHeight(ref geometry.m_Right.m_Right, ((float4)(ref netCompositionData.m_EdgeHeights)).yw, new float2(0f, 1f));
					CheckBestCurve(geometry.m_Left.m_Left, middlePos, isRight: false, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
					CheckBestCurve(geometry.m_Right.m_Right, middlePos, isRight: true, isEnd, ref bestCurve2, ref bestDistance2, ref bestT2, ref bestStartEnd2);
				}
			}
			bestCurve = bestCurve2;
			bestDistance = bestDistance2;
			bestT = bestT2;
			bestStartEnd = bestStartEnd2;
		}

		private void OffsetCurveHeight(ref Bezier4x3 curve, float2 heights, float2 heightDelta)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			float4 val = math.lerp(float4.op_Implicit(heights.x), float4.op_Implicit(heights.y), new float4(heightDelta.x, math.lerp(heightDelta.x, heightDelta.y, 1f / 3f), math.lerp(heightDelta.x, heightDelta.y, 2f / 3f), heightDelta.y));
			curve.a.y += val.x;
			curve.b.y += val.y;
			curve.c.y += val.z;
			curve.d.y += val.w;
		}

		private void FindTargetSegments(Entity edge, Entity node, float2 offset, float2 middlePos, NetGeometryData prefabGeometryData, NetCompositionData compositionData, out Segment leftSegment, out Segment rightSegment, out float2 distances, out float4 syncVertexOffsetsLeft, out float4 syncVertexOffsetsRight, out float roundaboutSize, out bool2 sideConnect, out bool2 middleConnect)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0984: Unknown result type (might be due to invalid IL or missing references)
			//IL_0989: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09af: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_073a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_083c: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_085d: Unknown result type (might be due to invalid IL or missing references)
			//IL_085e: Unknown result type (might be due to invalid IL or missing references)
			//IL_084b: Unknown result type (might be due to invalid IL or missing references)
			//IL_084c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0615: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_062d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_0937: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0902: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0805: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0821: Unknown result type (might be due to invalid IL or missing references)
			//IL_0828: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(offset.y, 0f - offset.x);
			float num = -2f;
			float num2 = 2f;
			float num3 = float.MaxValue;
			leftSegment = default(Segment);
			rightSegment = default(Segment);
			EdgeIteratorValue edgeIteratorValue = default(EdgeIteratorValue);
			EdgeIteratorValue edgeIteratorValue2 = default(EdgeIteratorValue);
			distances = float2.op_Implicit(0f);
			roundaboutSize = 0f;
			sideConnect = bool2.op_Implicit(false);
			bool flag = m_OutsideConnectionData.HasComponent(node);
			EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData, includeMiddleConnections: true);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				if (value.m_Middle)
				{
					if ((prefabGeometryData.m_MergeLayers & (Layer.Pathway | Layer.MarkerPathway)) == 0)
					{
						continue;
					}
					PrefabRef prefabRef = m_PrefabRefDataFromEntity[value.m_Edge];
					NetGeometryData edgePrefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					EdgeGeometry edgeGeometry = m_GeometryDataFromEntity[value.m_Edge];
					Composition composition = m_CompositionDataFromEntity[value.m_Edge];
					NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
					if (netCompositionData.m_SideConnectionOffset.x != 0f)
					{
						edgeGeometry.m_Start.m_Left = NetUtils.OffsetCurveLeftSmooth(edgeGeometry.m_Start.m_Left, float2.op_Implicit(netCompositionData.m_SideConnectionOffset.x));
						edgeGeometry.m_End.m_Left = NetUtils.OffsetCurveLeftSmooth(edgeGeometry.m_End.m_Left, float2.op_Implicit(netCompositionData.m_SideConnectionOffset.x));
					}
					if (netCompositionData.m_SideConnectionOffset.y != 0f)
					{
						edgeGeometry.m_Start.m_Right = NetUtils.OffsetCurveLeftSmooth(edgeGeometry.m_Start.m_Right, float2.op_Implicit(0f - netCompositionData.m_SideConnectionOffset.y));
						edgeGeometry.m_End.m_Right = NetUtils.OffsetCurveLeftSmooth(edgeGeometry.m_End.m_Right, float2.op_Implicit(0f - netCompositionData.m_SideConnectionOffset.y));
					}
					OffsetCurveHeight(ref edgeGeometry.m_Start.m_Left, ((float4)(ref netCompositionData.m_EdgeHeights)).xz, new float2(0f, 0.5f));
					OffsetCurveHeight(ref edgeGeometry.m_Start.m_Right, ((float4)(ref netCompositionData.m_EdgeHeights)).yw, new float2(0f, 0.5f));
					OffsetCurveHeight(ref edgeGeometry.m_End.m_Left, ((float4)(ref netCompositionData.m_EdgeHeights)).xz, new float2(0.5f, 1f));
					OffsetCurveHeight(ref edgeGeometry.m_End.m_Right, ((float4)(ref netCompositionData.m_EdgeHeights)).yw, new float2(0.5f, 1f));
					Bezier4x3 bestCurve = default(Bezier4x3);
					float bestDistance = float.MaxValue;
					float bestT = 0f;
					bool2 bestStartEnd = bool2.op_Implicit(false);
					CheckBestCurve(edgeGeometry.m_Start.m_Left, middlePos, isRight: false, isEnd: false, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
					CheckBestCurve(edgeGeometry.m_Start.m_Right, middlePos, isRight: true, isEnd: false, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
					CheckBestCurve(edgeGeometry.m_End.m_Left, middlePos, isRight: false, isEnd: true, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
					CheckBestCurve(edgeGeometry.m_End.m_Right, middlePos, isRight: true, isEnd: true, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
					if (m_IterationIndex == 1 && math.any(bestStartEnd))
					{
						Edge edge2 = m_EdgeDataFromEntity[value.m_Edge];
						if (bestStartEnd.x)
						{
							CheckNodeCurves(edge2.m_Start, middlePos, edgePrefabGeometryData, isEnd: false, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
						}
						if (bestStartEnd.y)
						{
							CheckNodeCurves(edge2.m_End, middlePos, edgePrefabGeometryData, isEnd: true, ref bestCurve, ref bestDistance, ref bestT, ref bestStartEnd);
						}
					}
					if (bestDistance < num3)
					{
						num3 = bestDistance;
						float3 val2 = MathUtils.Position(bestCurve, bestT);
						float3 val3 = default(float3);
						float3 val4 = MathUtils.Tangent(bestCurve, bestT);
						((float3)(ref val3)).xz = math.normalizesafe(MathUtils.Left(((float3)(ref val4)).xz), default(float2));
						float3 val5 = val2;
						((float3)(ref val5)).xz = ((float3)(ref val5)).xz + MathUtils.Left(((float3)(ref val3)).xz) * (compositionData.m_Width * 0.5f);
						leftSegment.m_Left = NetUtils.StraightCurve(val5, val5 + val3);
						leftSegment.m_Right = NetUtils.StraightCurve(val2, val2 + val3);
						edgeIteratorValue = value;
						distances.x = compositionData.m_Width * 0.5f;
						float3 val6 = val2;
						((float3)(ref val6)).xz = ((float3)(ref val6)).xz + MathUtils.Right(((float3)(ref val3)).xz) * (compositionData.m_Width * 0.5f);
						rightSegment.m_Left = NetUtils.StraightCurve(val2, val2 + val3);
						rightSegment.m_Right = NetUtils.StraightCurve(val6, val6 + val3);
						edgeIteratorValue2 = value;
						distances.y = compositionData.m_Width * 0.5f;
					}
				}
				else
				{
					if (edgeIteratorValue.m_Middle)
					{
						continue;
					}
					Composition composition2 = m_CompositionDataFromEntity[value.m_Edge];
					EdgeGeometry edgeGeometry2 = m_GeometryDataFromEntity[value.m_Edge];
					NetCompositionData edgeCompositionData = m_PrefabCompositionData[composition2.m_Edge];
					Segment segment;
					NetCompositionData nodeCompositionData;
					if (value.m_End)
					{
						segment = Invert(edgeGeometry2.m_End);
						nodeCompositionData = m_PrefabCompositionData[composition2.m_EndNode];
					}
					else
					{
						segment = edgeGeometry2.m_Start;
						nodeCompositionData = m_PrefabCompositionData[composition2.m_StartNode];
					}
					nodeCompositionData.m_MiddleOffset = 0f - nodeCompositionData.m_MiddleOffset;
					AdjustSegmentWidth(ref segment, edgeCompositionData, nodeCompositionData, value.m_End);
					float num4 = 0.5f + nodeCompositionData.m_MiddleOffset / math.max(0.01f, nodeCompositionData.m_Width);
					float2 val7 = StartOffset(segment, middlePos);
					if (value.m_Edge == edge)
					{
						if (num < -1f)
						{
							if (prefabGeometryData.m_MergeLayers == Layer.None || flag)
							{
								leftSegment.m_Left = MathUtils.StartReflect(segment.m_Right);
								leftSegment.m_Right = MathUtils.StartReflect(segment.m_Left);
								num4 = 1f - num4;
							}
							else
							{
								leftSegment = segment;
							}
							leftSegment.m_Right = MathUtils.Lerp(leftSegment.m_Left, leftSegment.m_Right, num4);
							edgeIteratorValue = value;
							distances.x = nodeCompositionData.m_Width * 0.5f + nodeCompositionData.m_MiddleOffset;
						}
						if (num2 > 1f)
						{
							if (prefabGeometryData.m_MergeLayers == Layer.None || flag)
							{
								rightSegment.m_Left = MathUtils.StartReflect(segment.m_Right);
								rightSegment.m_Right = MathUtils.StartReflect(segment.m_Left);
								num4 = 1f - num4;
							}
							else
							{
								rightSegment = segment;
							}
							rightSegment.m_Left = MathUtils.Lerp(rightSegment.m_Left, rightSegment.m_Right, num4);
							edgeIteratorValue2 = value;
							distances.y = nodeCompositionData.m_Width * 0.5f - nodeCompositionData.m_MiddleOffset;
						}
					}
					else
					{
						PrefabRef prefabRef2 = m_PrefabRefDataFromEntity[value.m_Edge];
						NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef2.m_Prefab];
						if ((netGeometryData.m_MergeLayers & prefabGeometryData.m_MergeLayers) == 0)
						{
							if ((prefabGeometryData.m_MergeLayers & (Layer.Pathway | Layer.MarkerPathway)) == 0 || (netGeometryData.m_MergeLayers & Layer.Road) == 0)
							{
								continue;
							}
							sideConnect = bool2.op_Implicit(true);
							if (nodeCompositionData.m_SideConnectionOffset.y != 0f)
							{
								segment.m_Left = NetUtils.OffsetCurveLeftSmooth(segment.m_Left, float2.op_Implicit(nodeCompositionData.m_SideConnectionOffset.y));
							}
							if (nodeCompositionData.m_SideConnectionOffset.x != 0f)
							{
								segment.m_Right = NetUtils.OffsetCurveLeftSmooth(segment.m_Right, float2.op_Implicit(0f - nodeCompositionData.m_SideConnectionOffset.x));
							}
							ref Bezier4x3 left = ref segment.m_Left;
							((Bezier4x3)(ref left)).y = ((Bezier4x3)(ref left)).y + nodeCompositionData.m_EdgeHeights.y;
							ref Bezier4x3 right = ref segment.m_Right;
							((Bezier4x3)(ref right)).y = ((Bezier4x3)(ref right)).y + nodeCompositionData.m_EdgeHeights.x;
						}
						float num5;
						if (math.dot(offset, val7) < 0f)
						{
							num5 = math.dot(val, val7) * 0.5f;
						}
						else
						{
							float num6 = math.dot(val, val7);
							num5 = math.select(-1f, 1f, num6 >= 0f) - num6 * 0.5f;
						}
						if (num5 > num)
						{
							num = num5;
							leftSegment = segment;
							leftSegment.m_Right = MathUtils.Lerp(leftSegment.m_Left, leftSegment.m_Right, num4);
							edgeIteratorValue = value;
							distances.x = nodeCompositionData.m_Width * 0.5f + nodeCompositionData.m_MiddleOffset;
						}
						if (num5 < num2)
						{
							num2 = num5;
							rightSegment = segment;
							rightSegment.m_Left = MathUtils.Lerp(rightSegment.m_Left, rightSegment.m_Right, num4);
							edgeIteratorValue2 = value;
							distances.y = nodeCompositionData.m_Width * 0.5f - nodeCompositionData.m_MiddleOffset;
						}
					}
					if (value.m_End)
					{
						roundaboutSize = math.max(roundaboutSize, nodeCompositionData.m_RoundaboutSize.y);
					}
					else
					{
						roundaboutSize = math.max(roundaboutSize, nodeCompositionData.m_RoundaboutSize.x);
					}
				}
			}
			middleConnect = new bool2(edgeIteratorValue.m_Middle, edgeIteratorValue2.m_Middle);
			if (edgeIteratorValue.m_Edge != Entity.Null && !sideConnect.x && !edgeIteratorValue.m_Middle)
			{
				Composition composition3 = m_CompositionDataFromEntity[edgeIteratorValue.m_Edge];
				Entity val8 = (edgeIteratorValue.m_End ? composition3.m_EndNode : composition3.m_StartNode);
				NetCompositionData netCompositionData2 = m_PrefabCompositionData[val8];
				syncVertexOffsetsLeft = 1f - ((float4)(ref netCompositionData2.m_SyncVertexOffsetsRight)).wzyx;
			}
			else
			{
				syncVertexOffsetsLeft = new float4(0f, 1f / 3f, 2f / 3f, 1f);
			}
			if (edgeIteratorValue2.m_Edge != Entity.Null && !sideConnect.y && !edgeIteratorValue2.m_Middle)
			{
				Composition composition4 = m_CompositionDataFromEntity[edgeIteratorValue2.m_Edge];
				Entity val9 = (edgeIteratorValue2.m_End ? composition4.m_EndNode : composition4.m_StartNode);
				NetCompositionData netCompositionData3 = m_PrefabCompositionData[val9];
				syncVertexOffsetsRight = 1f - ((float4)(ref netCompositionData3.m_SyncVertexOffsetsLeft)).wzyx;
			}
			else
			{
				syncVertexOffsetsRight = new float4(0f, 1f / 3f, 2f / 3f, 1f);
			}
		}

		private void CalculateSegments(Segment startLeftSegment, Segment startRightSegment, Segment endLeftSegment, Segment endRightSegment, float3 nodePosition, ref float leftRadius, ref float rightRadius, out Segment startSegment, out Segment endSegment, out float2 leftDivPos, out float2 rightDivPos)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			CalculateMiddleCurves(startLeftSegment.m_Left, endLeftSegment.m_Left, nodePosition, ref leftRadius, right: false, out startSegment.m_Left, out endSegment.m_Left, out var divPos);
			CalculateMiddleCurves(startRightSegment.m_Right, endRightSegment.m_Right, nodePosition, ref rightRadius, right: true, out startSegment.m_Right, out endSegment.m_Right, out var divPos2);
			if (math.distancesq(endSegment.m_Left.d, endSegment.m_Right.d) < 0.1f)
			{
				endSegment.m_Left.d = math.lerp(endSegment.m_Left.d, endSegment.m_Right.d, 0.5f);
				endSegment.m_Right.d = endSegment.m_Left.d;
			}
			startSegment.m_Length.x = MathUtils.Length(startSegment.m_Left);
			startSegment.m_Length.y = MathUtils.Length(startSegment.m_Right);
			endSegment.m_Length.x = MathUtils.Length(endSegment.m_Left);
			endSegment.m_Length.y = MathUtils.Length(endSegment.m_Right);
			leftDivPos = float2.op_Implicit(divPos);
			rightDivPos = float2.op_Implicit(divPos2);
		}

		private void CalculateMiddleCurves(Bezier4x3 startCurve, Bezier4x3 endCurve, float3 nodePosition, ref float radius, bool right, out Bezier4x3 startMiddleCurve, out Bezier4x3 endMiddleCurve, out float divPos)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			float2 val = math.normalizesafe(((float3)(ref startCurve.d)).xz - ((float3)(ref nodePosition)).xz, default(float2));
			float2 val2 = math.normalizesafe(((float3)(ref endCurve.a)).xz - ((float3)(ref nodePosition)).xz, default(float2));
			float3 endTangent = default(float3);
			float3 val3 = default(float3);
			float num2;
			float2 val4;
			float2 val5;
			if (right)
			{
				float num = MathUtils.RotationAngleLeft(val, val2) * 0.5f;
				num2 = math.max(math.min(num * 0.5f, (float)Math.PI / 8f), num - (float)Math.PI / 2f);
				val4 = MathUtils.RotateLeft(val, num);
				val5 = MathUtils.RotateLeft(val, num2);
				((float3)(ref endTangent)).xz = MathUtils.Left(val4);
				((float3)(ref val3)).xz = MathUtils.Left(val5);
			}
			else
			{
				float num3 = MathUtils.RotationAngleRight(val, val2) * 0.5f;
				num2 = math.max(math.min(num3 * 0.5f, (float)Math.PI / 8f), num3 - (float)Math.PI / 2f);
				val4 = MathUtils.RotateRight(val, num3);
				val5 = MathUtils.RotateRight(val, num2);
				((float3)(ref endTangent)).xz = MathUtils.Right(val4);
				((float3)(ref val3)).xz = MathUtils.Right(val5);
			}
			float3 val6 = MathUtils.EndTangent(startCurve);
			val6 = MathUtils.Normalize(val6, ((float3)(ref val6)).xz);
			float divPos2;
			float middleDistance;
			Bezier4x3 val7 = CalculateMiddleCurve(startCurve, endCurve, nodePosition, out middleDistance, out divPos2);
			Bezier4x3 val8 = default(Bezier4x3);
			Bezier4x3 val9 = default(Bezier4x3);
			MathUtils.Divide(val7, ref val8, ref val9, 0.5f);
			val8.c.y = val9.d.y;
			val8.d.y = val9.d.y;
			val9.a.y = val9.d.y;
			val9.b.y = val9.d.y;
			val9.c.y = val9.d.y;
			middleDistance = math.select(middleDistance, 0f, math.dot(((float3)(ref val7.d)).xz - ((float3)(ref nodePosition)).xz, val4) <= 0f);
			float num4 = math.smoothstep(radius, radius * 1.2f, middleDistance);
			float num5 = math.max(radius, middleDistance);
			float3 d = startCurve.d;
			float3 endPos = nodePosition;
			float3 val10 = nodePosition;
			((float3)(ref endPos)).xz = ((float3)(ref endPos)).xz + val4 * num5;
			((float3)(ref val10)).xz = ((float3)(ref val10)).xz + val5 * num5;
			startMiddleCurve = NetUtils.FitCurve(d, val6, val3, val10);
			endMiddleCurve = NetUtils.FitCurve(val10, val3, endTangent, endPos);
			divPos = 0.5f;
			float num6 = math.max(0f, (num2 - (float)Math.PI / 8f) * 0.84882635f);
			ref float3 b = ref startMiddleCurve.b;
			b += (startMiddleCurve.a - startMiddleCurve.b) * (num6 * 0.5f);
			ref float3 c = ref startMiddleCurve.c;
			c += (startMiddleCurve.c - startMiddleCurve.d) * num6;
			startMiddleCurve = MathUtils.Lerp(startMiddleCurve, val8, num4);
			endMiddleCurve = MathUtils.Lerp(endMiddleCurve, val9, num4);
			divPos = math.lerp(divPos, divPos2, num4);
		}

		private Segment CalculateSideConnection(Segment startSegment, Segment endLeftSegment, Segment endRightSegment, NetCompositionData edgeCompositionData, NetCompositionData nodeCompositionData, bool right, out float2 divPos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			float3 val = -MathUtils.StartTangent(endLeftSegment.m_Left);
			float3 val2 = MathUtils.StartTangent(endRightSegment.m_Right);
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val.y = math.clamp(val.y, -1f, 1f);
			val2.y = math.clamp(val2.y, -1f, 1f);
			Bezier4x3 val3 = NetUtils.FitCurve(endLeftSegment.m_Left.a, val, val2, endRightSegment.m_Right.a);
			float max = default(float);
			MathUtils.Distance(val3, startSegment.m_Left.d, ref max);
			float min = default(float);
			MathUtils.Distance(val3, startSegment.m_Right.d, ref min);
			float num = math.max(edgeCompositionData.m_NodeOffset, nodeCompositionData.m_NodeOffset);
			if (right)
			{
				Bounds1 val4 = default(Bounds1);
				((Bounds1)(ref val4))._002Ector(min, 1f);
				MathUtils.ClampLength(val3, ref val4, num * 0.5f);
				min = val4.min;
				val = MathUtils.Tangent(val3, max);
				((float3)(ref val)).xz = math.normalizesafe(MathUtils.Left(((float3)(ref val)).xz), default(float2));
				val.y = 0f;
				val2 = MathUtils.Tangent(val3, min);
				((float3)(ref val2)).xz = math.normalizesafe(MathUtils.Left(((float3)(ref val2)).xz) + ((float3)(ref val2)).xz, default(float2));
				val2.y = math.clamp(val2.y * 0.5f, -1f, 1f);
			}
			else
			{
				Bounds1 val5 = default(Bounds1);
				((Bounds1)(ref val5))._002Ector(0f, max);
				MathUtils.ClampLengthInverse(val3, ref val5, num * 0.5f);
				max = val5.max;
				val = MathUtils.Tangent(val3, min);
				((float3)(ref val)).xz = math.normalizesafe(MathUtils.Left(((float3)(ref val)).xz) - ((float3)(ref val)).xz, default(float2));
				val.y = math.clamp(val.y * 0.5f, -1f, 1f);
				val2 = MathUtils.Tangent(val3, min);
				((float3)(ref val2)).xz = math.normalizesafe(MathUtils.Left(((float3)(ref val2)).xz), default(float2));
				val2.y = 0f;
			}
			float3 endPos = MathUtils.Position(val3, max);
			float3 endPos2 = MathUtils.Position(val3, min);
			float3 val6 = MathUtils.EndTangent(startSegment.m_Left);
			float3 val7 = MathUtils.EndTangent(startSegment.m_Right);
			val6 = MathUtils.Normalize(val6, ((float3)(ref val6)).xz);
			val7 = MathUtils.Normalize(val7, ((float3)(ref val7)).xz);
			val6.y = math.clamp(val6.y, -1f, 1f);
			val7.y = math.clamp(val7.y, -1f, 1f);
			Segment result = default(Segment);
			result.m_Left = NetUtils.FitCurve(startSegment.m_Left.d, val6, val, endPos);
			result.m_Right = NetUtils.FitCurve(startSegment.m_Right.d, val7, val2, endPos2);
			result.m_Length.x = MathUtils.Length(result.m_Left);
			result.m_Length.y = MathUtils.Length(result.m_Right);
			divPos = float2.op_Implicit(0f);
			return result;
		}

		private Segment CalculateMiddleConnection(Segment startSegment, Segment endSegment, out float2 divPos)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			Segment result = default(Segment);
			result.m_Left = CalculateMiddleConnection(startSegment.m_Left, endSegment.m_Left, out divPos.x);
			result.m_Right = CalculateMiddleConnection(startSegment.m_Right, endSegment.m_Right, out divPos.y);
			result.m_Length.x = MathUtils.Length(result.m_Left);
			result.m_Length.y = MathUtils.Length(result.m_Right);
			return result;
		}

		private Bezier4x3 CalculateMiddleConnection(Bezier4x3 startCurve, Bezier4x3 endCurve, out float divPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.EndTangent(startCurve);
			float3 val2 = MathUtils.StartTangent(endCurve);
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val.y = math.clamp(val.y, -1f, 1f);
			val2.y = math.clamp(val2.y, -1f, 1f);
			divPos = 0f;
			return NetUtils.FitCurve(startCurve.d, val, val2, endCurve.a);
		}

		private Segment CalculateMiddleSegment(Segment startSegment, Segment endSegment, out float2 divPos)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			Segment result = default(Segment);
			result.m_Left = CalculateMiddleCurve(startSegment.m_Left, endSegment.m_Left, out divPos.x);
			result.m_Right = CalculateMiddleCurve(startSegment.m_Right, endSegment.m_Right, out divPos.y);
			result.m_Length.x = MathUtils.Length(result.m_Left);
			result.m_Length.y = MathUtils.Length(result.m_Right);
			return result;
		}

		private Bezier4x3 CalculateMiddleCurve(Bezier4x3 startCurve, Bezier4x3 endCurve, out float divPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.EndTangent(startCurve);
			float3 val2 = MathUtils.StartTangent(endCurve);
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val.y = math.clamp(val.y, -1f, 1f);
			val2.y = math.clamp(val2.y, -1f, 1f);
			Bezier4x3 val3 = NetUtils.FitCurve(startCurve.d, val, val2, endCurve.a);
			divPos = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref val3)).xz, new float2(0f, 1f));
			float3 endPos = MathUtils.Position(val3, divPos);
			val2 = MathUtils.Tangent(val3, divPos);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val2.y = math.clamp(val2.y, -1f, 1f);
			return NetUtils.FitCurve(startCurve.d, val, val2, endPos);
		}

		private Bezier4x3 CalculateMiddleCurve(Bezier4x3 startCurve, Bezier4x3 endCurve, float3 middlePosition, out float middleDistance, out float divPos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.EndTangent(startCurve);
			float3 val2 = MathUtils.StartTangent(endCurve);
			val = MathUtils.Normalize(val, ((float3)(ref val)).xz);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val.y = math.clamp(val.y, -1f, 1f);
			val2.y = math.clamp(val2.y, -1f, 1f);
			Bezier4x3 val3 = NetUtils.FitCurve(startCurve.d, val, val2, endCurve.a);
			float num = default(float);
			middleDistance = MathUtils.Distance(((Bezier4x3)(ref val3)).xz, ((float3)(ref middlePosition)).xz, ref num);
			divPos = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref val3)).xz, new float2(0f, 1f));
			float3 endPos = MathUtils.Position(val3, divPos);
			val2 = MathUtils.Tangent(val3, divPos);
			val2 = MathUtils.Normalize(val2, ((float3)(ref val2)).xz);
			val2.y = math.clamp(val2.y, -1f, 1f);
			return NetUtils.FitCurve(startCurve.d, val, val2, endPos);
		}
	}

	private struct IntersectionData
	{
		public Bezier4x3 m_StartMiddle;

		public Bezier4x3 m_EndMiddle;

		public Bounds3 m_StartBounds;

		public Bounds3 m_EndBounds;
	}

	[BurstCompile]
	private struct CalculateIntersectionGeometryJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeDataFromEntity;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> m_NodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartGeometryDataFromEntity;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndGeometryDataFromEntity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_Edges;

		[NativeDisableParallelForRestriction]
		public NativeList<IntersectionData> m_BufferedData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0582: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0755: Unknown result type (might be due to invalid IL or missing references)
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_076c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0771: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0690: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_071b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_0826: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			Edge edge = m_EdgeDataFromEntity[val];
			Composition composition = m_CompositionData[val];
			StartNodeGeometry startNodeGeometry = m_StartGeometryDataFromEntity[val];
			EndNodeGeometry endNodeGeometry = m_EndGeometryDataFromEntity[val];
			PrefabRef prefabRef = m_PrefabRefData[val];
			NetGeometryData prefabGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
			NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_StartNode];
			NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_EndNode];
			IntersectionData intersectionData = default(IntersectionData);
			NodeGeometry nodeGeometry = m_NodeGeometryData[edge.m_Start];
			NodeGeometry nodeGeometry2 = m_NodeGeometryData[edge.m_End];
			bool flag = (netCompositionData.m_Flags.m_General & (CompositionFlags.General.Roundabout | CompositionFlags.General.LevelCrossing | CompositionFlags.General.FixedNodeSize)) != 0;
			bool flag2 = (netCompositionData2.m_Flags.m_General & (CompositionFlags.General.Roundabout | CompositionFlags.General.LevelCrossing | CompositionFlags.General.FixedNodeSize)) != 0;
			float3 val2;
			if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				val2 = m_NodeDataFromEntity[edge.m_Start].m_Position;
				val2.y = nodeGeometry.m_Position;
			}
			else
			{
				val2 = FindIntersectionPos(val, edge.m_Start, prefabGeometryData);
				if (flag)
				{
					val2.y = nodeGeometry.m_Position;
				}
				Flatten(ref startNodeGeometry.m_Geometry, val2, flag);
			}
			float3 val3;
			if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				val3 = m_NodeDataFromEntity[edge.m_End].m_Position;
				val3.y = nodeGeometry2.m_Position;
			}
			else
			{
				val3 = FindIntersectionPos(val, edge.m_End, prefabGeometryData);
				if (flag2)
				{
					val3.y = nodeGeometry2.m_Position;
				}
				Flatten(ref endNodeGeometry.m_Geometry, val3, flag2);
			}
			if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float num = netCompositionData.m_MiddleOffset / math.max(0.01f, netCompositionData.m_Width) + 0.5f;
				intersectionData.m_StartMiddle = MathUtils.Lerp(startNodeGeometry.m_Geometry.m_Left.m_Left, startNodeGeometry.m_Geometry.m_Left.m_Right, num);
				MoveEndTo(ref intersectionData.m_StartMiddle, val2);
			}
			else
			{
				bool num2 = math.all(((float3)(ref startNodeGeometry.m_Geometry.m_Middle.d)).xy != 0f);
				intersectionData.m_StartMiddle = MathUtils.Lerp(startNodeGeometry.m_Geometry.m_Left.m_Right, startNodeGeometry.m_Geometry.m_Right.m_Left, 0.5f);
				if (!num2)
				{
					intersectionData.m_StartMiddle.d = val2;
				}
			}
			if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
			{
				float num3 = netCompositionData2.m_MiddleOffset / math.max(0.01f, netCompositionData2.m_Width) + 0.5f;
				intersectionData.m_EndMiddle = MathUtils.Lerp(endNodeGeometry.m_Geometry.m_Left.m_Left, endNodeGeometry.m_Geometry.m_Left.m_Right, num3);
				MoveEndTo(ref intersectionData.m_EndMiddle, val3);
			}
			else
			{
				bool num4 = math.all(((float3)(ref endNodeGeometry.m_Geometry.m_Middle.d)).xy != 0f);
				intersectionData.m_EndMiddle = MathUtils.Lerp(endNodeGeometry.m_Geometry.m_Left.m_Right, endNodeGeometry.m_Geometry.m_Right.m_Left, 0.5f);
				if (!num4)
				{
					intersectionData.m_EndMiddle.d = val3;
				}
			}
			if (prefabGeometryData.m_MergeLayers == Layer.None)
			{
				float num5 = netCompositionData.m_Width * 0.5f;
				float num6 = netCompositionData2.m_Width * 0.5f;
				float3 val4 = math.lerp(startNodeGeometry.m_Geometry.m_Left.m_Left.a, startNodeGeometry.m_Geometry.m_Right.m_Right.a, 0.5f);
				float3 val5 = math.lerp(endNodeGeometry.m_Geometry.m_Left.m_Left.a, endNodeGeometry.m_Geometry.m_Right.m_Right.a, 0.5f);
				intersectionData.m_StartBounds = MathUtils.Bounds(val4, val4);
				intersectionData.m_EndBounds = MathUtils.Bounds(val5, val5);
				ref float3 min = ref intersectionData.m_StartBounds.min;
				((float3)(ref min)).xz = ((float3)(ref min)).xz - num5;
				ref float3 max = ref intersectionData.m_StartBounds.max;
				((float3)(ref max)).xz = ((float3)(ref max)).xz + num5;
				ref float3 min2 = ref intersectionData.m_EndBounds.min;
				((float3)(ref min2)).xz = ((float3)(ref min2)).xz - num6;
				ref float3 max2 = ref intersectionData.m_EndBounds.max;
				((float3)(ref max2)).xz = ((float3)(ref max2)).xz + num6;
			}
			else
			{
				intersectionData.m_StartBounds = MathUtils.TightBounds(startNodeGeometry.m_Geometry.m_Left.m_Left) | MathUtils.TightBounds(startNodeGeometry.m_Geometry.m_Left.m_Right) | MathUtils.TightBounds(startNodeGeometry.m_Geometry.m_Right.m_Left) | MathUtils.TightBounds(startNodeGeometry.m_Geometry.m_Right.m_Right) | MathUtils.TightBounds(intersectionData.m_StartMiddle);
				intersectionData.m_EndBounds = MathUtils.TightBounds(endNodeGeometry.m_Geometry.m_Left.m_Left) | MathUtils.TightBounds(endNodeGeometry.m_Geometry.m_Left.m_Right) | MathUtils.TightBounds(endNodeGeometry.m_Geometry.m_Right.m_Left) | MathUtils.TightBounds(endNodeGeometry.m_Geometry.m_Right.m_Right) | MathUtils.TightBounds(intersectionData.m_EndMiddle);
			}
			intersectionData.m_StartBounds.min.y += netCompositionData.m_HeightRange.min;
			intersectionData.m_StartBounds.max.y += netCompositionData.m_HeightRange.max;
			intersectionData.m_EndBounds.min.y += netCompositionData2.m_HeightRange.min;
			intersectionData.m_EndBounds.max.y += netCompositionData2.m_HeightRange.max;
			if ((netCompositionData.m_State & (CompositionState.LowerToTerrain | CompositionState.RaiseToTerrain)) != 0)
			{
				Bounds1 val6 = SampleTerrain(startNodeGeometry.m_Geometry.m_Left.m_Left) | SampleTerrain(startNodeGeometry.m_Geometry.m_Right.m_Right);
				if (startNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
				{
					val6 |= SampleTerrain(startNodeGeometry.m_Geometry.m_Left.m_Right) | SampleTerrain(startNodeGeometry.m_Geometry.m_Right.m_Left);
				}
				if ((netCompositionData.m_State & CompositionState.LowerToTerrain) != 0)
				{
					intersectionData.m_StartBounds.min.y = math.min(intersectionData.m_StartBounds.min.y, val6.min);
				}
				if ((netCompositionData.m_State & CompositionState.RaiseToTerrain) != 0)
				{
					intersectionData.m_StartBounds.max.y = math.max(intersectionData.m_StartBounds.max.y, val6.max);
				}
			}
			if ((netCompositionData2.m_State & (CompositionState.LowerToTerrain | CompositionState.RaiseToTerrain)) != 0)
			{
				Bounds1 val7 = SampleTerrain(endNodeGeometry.m_Geometry.m_Left.m_Left) | SampleTerrain(endNodeGeometry.m_Geometry.m_Right.m_Right);
				if (endNodeGeometry.m_Geometry.m_MiddleRadius > 0f)
				{
					val7 |= SampleTerrain(endNodeGeometry.m_Geometry.m_Left.m_Right) | SampleTerrain(endNodeGeometry.m_Geometry.m_Right.m_Left);
				}
				if ((netCompositionData2.m_State & CompositionState.LowerToTerrain) != 0)
				{
					intersectionData.m_EndBounds.min.y = math.min(intersectionData.m_EndBounds.min.y, val7.min);
				}
				if ((netCompositionData2.m_State & CompositionState.RaiseToTerrain) != 0)
				{
					intersectionData.m_EndBounds.max.y = math.max(intersectionData.m_EndBounds.max.y, val7.max);
				}
			}
			m_BufferedData[index] = intersectionData;
		}

		private Bounds1 SampleTerrain(Bezier4x3 curve)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			Bounds1 val = default(Bounds1);
			((Bounds1)(ref val))._002Ector(float.MaxValue, float.MinValue);
			for (int i = 0; i <= 8; i++)
			{
				val |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, MathUtils.Position(curve, (float)i * 0.125f));
			}
			return val;
		}

		private void MoveEndTo(ref Bezier4x3 curve, float3 pos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			float num = math.distance(curve.d, pos);
			ref float3 b = ref curve.b;
			b += math.normalizesafe(curve.b - curve.a, default(float3)) * (num * (1f / 3f));
			curve.c = pos + (curve.c - curve.d) + math.normalizesafe(curve.c - curve.d, default(float3)) * (num * (1f / 3f));
			curve.d = pos;
		}

		private float3 FindIntersectionPos(Entity edge, Entity node, NetGeometryData prefabGeometryData)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			float3 val = default(float3);
			float num = 0f;
			EdgeIterator edgeIterator = new EdgeIterator(edge, node, m_Edges, m_EdgeDataFromEntity, m_TempData, m_HiddenData);
			EdgeIteratorValue value;
			while (edgeIterator.GetNext(out value))
			{
				EdgeNodeGeometry edgeNodeGeometry = ((!value.m_End) ? m_StartGeometryDataFromEntity[value.m_Edge].m_Geometry : m_EndGeometryDataFromEntity[value.m_Edge].m_Geometry);
				if (value.m_Edge != edge)
				{
					PrefabRef prefabRef = m_PrefabRefData[value.m_Edge];
					NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					if ((prefabGeometryData.m_MergeLayers & netGeometryData.m_MergeLayers) == 0)
					{
						continue;
					}
				}
				val += math.lerp(edgeNodeGeometry.m_Left.m_Right.d, edgeNodeGeometry.m_Right.m_Left.d, 0.5f);
				num += 1f;
			}
			if (num != 0f)
			{
				val /= num;
			}
			return val;
		}

		private void Flatten(ref EdgeNodeGeometry geometry, float3 middlePos, bool edges)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (edges)
			{
				Flatten(ref geometry.m_Left.m_Left, middlePos.y);
				Flatten(ref geometry.m_Left.m_Right, middlePos.y);
				Flatten(ref geometry.m_Right.m_Left, middlePos.y);
				Flatten(ref geometry.m_Right.m_Right, middlePos.y);
				return;
			}
			float num = math.distance(((float3)(ref geometry.m_Left.m_Left.d)).xz, ((float3)(ref middlePos)).xz);
			float num2 = math.distance(((float3)(ref geometry.m_Left.m_Right.d)).xz, ((float3)(ref middlePos)).xz);
			float num3 = math.distance(((float3)(ref geometry.m_Right.m_Left.d)).xz, ((float3)(ref middlePos)).xz);
			float num4 = math.distance(((float3)(ref geometry.m_Right.m_Right.d)).xz, ((float3)(ref middlePos)).xz);
			float middleHeight = math.lerp(middlePos.y, geometry.m_Left.m_Left.d.y, math.saturate(num2 / num));
			float middleHeight2 = math.lerp(middlePos.y, geometry.m_Right.m_Right.d.y, math.saturate(num3 / num4));
			Flatten(ref geometry.m_Left.m_Right, middleHeight);
			Flatten(ref geometry.m_Right.m_Left, middleHeight2);
		}

		private void Flatten(ref Bezier4x3 curve, float middleHeight)
		{
			curve.c.y += middleHeight - curve.d.y;
			curve.d.y = middleHeight;
		}
	}

	[BurstCompile]
	private struct CopyNodeGeometryJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public NativeList<IntersectionData> m_BufferedData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			IntersectionData intersectionData = m_BufferedData[index];
			StartNodeGeometry startNodeGeometry = m_StartNodeGeometryData[val];
			startNodeGeometry.m_Geometry.m_Middle = intersectionData.m_StartMiddle;
			startNodeGeometry.m_Geometry.m_Bounds = intersectionData.m_StartBounds;
			m_StartNodeGeometryData[val] = startNodeGeometry;
			EndNodeGeometry endNodeGeometry = m_EndNodeGeometryData[val];
			endNodeGeometry.m_Geometry.m_Middle = intersectionData.m_EndMiddle;
			endNodeGeometry.m_Geometry.m_Bounds = intersectionData.m_EndBounds;
			m_EndNodeGeometryData[val] = endNodeGeometry;
		}
	}

	[BurstCompile]
	private struct UpdateNodeGeometryJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabGeometryData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Node> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<Orphan> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
			NativeArray<NodeGeometry> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
			NativeArray<PrefabRef> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			Bounds1 val2 = default(Bounds1);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				Entity node = nativeArray[i];
				Node node2 = nativeArray2[i];
				Bounds3 val = default(Bounds3);
				if (nativeArray3.Length != 0)
				{
					Orphan orphan = nativeArray3[i];
					NetCompositionData netCompositionData = m_PrefabCompositionData[orphan.m_Composition];
					float num = netCompositionData.m_Width * 0.5f;
					((Bounds3)(ref val)).xz = new Bounds2(((float3)(ref node2.m_Position)).xz - num, ((float3)(ref node2.m_Position)).xz + num);
					((Bounds3)(ref val)).y = node2.m_Position.y + netCompositionData.m_HeightRange;
					if ((netCompositionData.m_State & (CompositionState.LowerToTerrain | CompositionState.RaiseToTerrain)) != 0)
					{
						((Bounds1)(ref val2))._002Ector(float.MaxValue, float.MinValue);
						val2 |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, new float3(((float3)(ref node2.m_Position)).xy, val.min.z));
						val2 |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, new float3(val.min.x, ((float3)(ref node2.m_Position)).yz));
						val2 |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, new float3(val.max.x, ((float3)(ref node2.m_Position)).yz));
						val2 |= TerrainUtils.SampleHeight(ref m_TerrainHeightData, new float3(((float3)(ref node2.m_Position)).xy, val.max.z));
						if ((netCompositionData.m_State & CompositionState.LowerToTerrain) != 0)
						{
							val.min.y = math.min(val.min.y, val2.min);
						}
						if ((netCompositionData.m_State & CompositionState.RaiseToTerrain) != 0)
						{
							val.max.y = math.max(val.max.y, val2.max);
						}
					}
				}
				else
				{
					PrefabRef prefabRef = nativeArray5[i];
					NetGeometryData netGeometryData = m_PrefabGeometryData[prefabRef.m_Prefab];
					float num = netGeometryData.m_DefaultWidth * 0.5f;
					((Bounds3)(ref val)).xz = new Bounds2(((float3)(ref node2.m_Position)).xz - num, ((float3)(ref node2.m_Position)).xz + num);
					((Bounds3)(ref val)).y = node2.m_Position.y + netGeometryData.m_DefaultHeightRange;
				}
				EdgeIterator edgeIterator = new EdgeIterator(Entity.Null, node, m_ConnectedEdges, m_EdgeData, m_TempData, m_HiddenData);
				EdgeIteratorValue value;
				while (edgeIterator.GetNext(out value))
				{
					EdgeNodeGeometry edgeNodeGeometry = ((!value.m_End) ? m_StartNodeGeometryData[value.m_Edge].m_Geometry : m_EndNodeGeometryData[value.m_Edge].m_Geometry);
					val |= edgeNodeGeometry.m_Bounds;
				}
				NodeGeometry nodeGeometry = nativeArray4[i];
				nodeGeometry.m_Bounds = val;
				nativeArray4[i] = nodeGeometry;
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
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<NodeGeometry> __Game_Net_NodeGeometry_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Elevation> __Game_Net_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OutsideConnection> __Game_Net_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableNetData> __Game_Prefabs_PlaceableNetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceableObjectData> __Game_Prefabs_PlaceableObjectData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneData> __Game_Prefabs_NetLaneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionLane> __Game_Prefabs_NetCompositionLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<NetCompositionCrosswalk> __Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup;

		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RW_ComponentLookup;

		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RW_ComponentLookup;

		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_NodeGeometry_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeGeometry>(false);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Elevation>(true);
			__Game_Net_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OutsideConnection>(true);
			__Game_Net_NodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeGeometry>(true);
			__Game_Prefabs_PlaceableNetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableNetData>(true);
			__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceableObjectData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_NetLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneData>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubNet>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Prefabs_NetCompositionLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionLane>(true);
			__Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<NetCompositionCrosswalk>(true);
			__Game_Net_EdgeGeometry_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(false);
			__Game_Net_StartNodeGeometry_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(false);
			__Game_Net_EndNodeGeometry_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(false);
			__Game_Net_NodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeGeometry>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Orphan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(true);
		}
	}

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_UpdatedEdgesQuery;

	private EntityQuery m_UpdatedNodesQuery;

	private EntityQuery m_AllEdgesQuery;

	private EntityQuery m_AllNodesQuery;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_UpdatedEdgesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<EdgeGeometry>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_UpdatedNodesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<NodeGeometry>(),
			ComponentType.ReadOnly<Updated>()
		});
		m_AllEdgesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<EdgeGeometry>() });
		m_AllNodesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<NodeGeometry>() });
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8b: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		EntityQuery val = (loaded ? m_AllEdgesQuery : m_UpdatedEdgesQuery);
		EntityQuery val2 = (loaded ? m_AllNodesQuery : m_UpdatedNodesQuery);
		if (!((EntityQuery)(ref val)).IsEmptyIgnoreFilter || !((EntityQuery)(ref val2)).IsEmptyIgnoreFilter)
		{
			JobHandle val4 = default(JobHandle);
			NativeList<Entity> val3 = ((EntityQuery)(ref val)).ToEntityListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val4);
			NativeList<IntersectionData> val5 = default(NativeList<IntersectionData>);
			val5._002Ector(0, AllocatorHandle.op_Implicit((Allocator)3));
			NativeParallelHashMap<int2, float4> edgeHeightMap = default(NativeParallelHashMap<int2, float4>);
			edgeHeightMap._002Ector(0, AllocatorHandle.op_Implicit((Allocator)3));
			TerrainHeightData data = m_TerrainSystem.GetHeightData();
			Bounds3 bounds = TerrainUtils.GetBounds(ref data);
			AllocateBuffersJob allocateBuffersJob = new AllocateBuffersJob
			{
				m_Entities = val3,
				m_IntersectionData = val5,
				m_EdgeHeightMap = edgeHeightMap
			};
			InitializeNodeGeometryJob initializeNodeGeometryJob = new InitializeNodeGeometryJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionDataFromEntity = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveDataFromEntity = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Loaded = loaded
			};
			CalculateEdgeGeometryJob calculateEdgeGeometryJob = new CalculateEdgeGeometryJob
			{
				m_Entities = val3.AsDeferredJobArray(),
				m_TerrainBounds = bounds,
				m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CurveDataFromEntity = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionDataFromEntity = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceableNetData = InternalCompilerInterface.GetComponentLookup<PlaceableNetData>(ref __TypeHandle.__Game_Prefabs_PlaceableNetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PlaceableObjectData = InternalCompilerInterface.GetComponentLookup<PlaceableObjectData>(ref __TypeHandle.__Game_Prefabs_PlaceableObjectData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetLaneData = InternalCompilerInterface.GetComponentLookup<NetLaneData>(ref __TypeHandle.__Game_Prefabs_NetLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubNets = InternalCompilerInterface.GetBufferLookup<SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionLanes = InternalCompilerInterface.GetBufferLookup<NetCompositionLane>(ref __TypeHandle.__Game_Prefabs_NetCompositionLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionCrosswalks = InternalCompilerInterface.GetBufferLookup<NetCompositionCrosswalk>(ref __TypeHandle.__Game_Prefabs_NetCompositionCrosswalk_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			FlattenNodeGeometryJob flattenNodeGeometryJob = new FlattenNodeGeometryJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeHeightMap = edgeHeightMap.AsParallelWriter()
			};
			FinishEdgeGeometryJob obj = new FinishEdgeGeometryJob
			{
				m_Entities = val3.AsDeferredJobArray(),
				m_TerrainHeightData = data,
				m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionDataFromEntity = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeHeightMap = edgeHeightMap
			};
			CalculateNodeGeometryJob calculateNodeGeometryJob = new CalculateNodeGeometryJob
			{
				m_Entities = val3.AsDeferredJobArray(),
				m_PrefabRefDataFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionDataFromEntity = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<OutsideConnection>(ref __TypeHandle.__Game_Net_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_GeometryDataFromEntity = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			CalculateIntersectionGeometryJob calculateIntersectionGeometryJob = new CalculateIntersectionGeometryJob
			{
				m_Entities = val3.AsDeferredJobArray(),
				m_TerrainHeightData = data,
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeDataFromEntity = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryData = InternalCompilerInterface.GetComponentLookup<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartGeometryDataFromEntity = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndGeometryDataFromEntity = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Edges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BufferedData = val5
			};
			CopyNodeGeometryJob copyNodeGeometryJob = new CopyNodeGeometryJob
			{
				m_Entities = val3.AsDeferredJobArray(),
				m_BufferedData = val5,
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			UpdateNodeGeometryJob updateNodeGeometryJob = new UpdateNodeGeometryJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TerrainHeightData = data,
				m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val6 = IJobExtensions.Schedule<AllocateBuffersJob>(allocateBuffersJob, val4);
			JobHandle val7 = JobChunkExtensions.ScheduleParallel<InitializeNodeGeometryJob>(initializeNodeGeometryJob, val2, ((SystemBase)this).Dependency);
			JobHandle val8 = IJobParallelForDeferExtensions.Schedule<CalculateEdgeGeometryJob, Entity>(calculateEdgeGeometryJob, val3, 1, JobHandle.CombineDependencies(val7, val4));
			JobHandle val9 = JobChunkExtensions.ScheduleParallel<FlattenNodeGeometryJob>(flattenNodeGeometryJob, val2, JobHandle.CombineDependencies(val8, val6));
			JobHandle val10 = IJobParallelForDeferExtensions.Schedule<FinishEdgeGeometryJob, Entity>(obj, val3, 1, val9);
			JobHandle val11 = val10;
			calculateNodeGeometryJob.m_IterationIndex = 0;
			while (calculateNodeGeometryJob.m_IterationIndex < 2)
			{
				val11 = IJobParallelForDeferExtensions.Schedule<CalculateNodeGeometryJob, Entity>(calculateNodeGeometryJob, val3, 1, val11);
				calculateNodeGeometryJob.m_IterationIndex++;
			}
			JobHandle val12 = IJobParallelForDeferExtensions.Schedule<CalculateIntersectionGeometryJob, Entity>(calculateIntersectionGeometryJob, val3, 1, val11);
			JobHandle val13 = IJobParallelForDeferExtensions.Schedule<CopyNodeGeometryJob, Entity>(copyNodeGeometryJob, val3, 16, val12);
			JobHandle val14 = JobChunkExtensions.ScheduleParallel<UpdateNodeGeometryJob>(updateNodeGeometryJob, val2, val13);
			val3.Dispose(val13);
			val5.Dispose(val13);
			edgeHeightMap.Dispose(val10);
			m_TerrainSystem.AddCPUHeightReader(val14);
			((SystemBase)this).Dependency = val14;
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
	public GeometrySystem()
	{
	}
}
