using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Zones;

[CompilerGenerated]
public class BlockSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateBlocksJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.BuildOrder> m_BuildOrderType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public BufferTypeHandle<SubBlock> m_SubBlockType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Block> m_BlockData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<Game.Net.BuildOrder> m_BuildOrderData;

		[ReadOnly]
		public ComponentLookup<Road> m_RoadData;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> m_StartNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> m_EndNodeGeometryData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_RoadCompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<ZoneBlockData> m_ZoneBlockData;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> m_ConnectedEdges;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				BufferAccessor<SubBlock> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubBlock>(ref m_SubBlockType);
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					DynamicBuffer<SubBlock> val = bufferAccessor[i];
					for (int j = 0; j < val.Length; j++)
					{
						Entity subBlock = val[j].m_SubBlock;
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(unfilteredChunkIndex, subBlock, default(Deleted));
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Edge> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<Curve> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Composition> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<Game.Net.BuildOrder> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.BuildOrder>(ref m_BuildOrderType);
			NativeArray<Road> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
			BufferAccessor<SubBlock> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<SubBlock>(ref m_SubBlockType);
			NativeParallelHashMap<Block, Entity> oldBlockBuffer = default(NativeParallelHashMap<Block, Entity>);
			oldBlockBuffer._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
			Owner ownerOwner = default(Owner);
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity owner = nativeArray[k];
				Edge edge = nativeArray3[k];
				Curve curve = nativeArray4[k];
				Composition composition = nativeArray5[k];
				Game.Net.BuildOrder buildOrder = nativeArray6[k];
				Road road = nativeArray7[k];
				DynamicBuffer<SubBlock> blocks = bufferAccessor2[k];
				CollectionUtils.TryGet<Owner>(nativeArray2, k, ref ownerOwner);
				FillOldBlockBuffer(blocks, oldBlockBuffer);
				CreateBlocks(unfilteredChunkIndex, owner, oldBlockBuffer, ownerOwner, composition, edge, curve, buildOrder, road);
				RemoveUnusedOldBlocks(unfilteredChunkIndex, blocks, oldBlockBuffer);
				oldBlockBuffer.Clear();
			}
			oldBlockBuffer.Dispose();
		}

		private void FillOldBlockBuffer(DynamicBuffer<SubBlock> blocks, NativeParallelHashMap<Block, Entity> oldBlockBuffer)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < blocks.Length; i++)
			{
				Entity subBlock = blocks[i].m_SubBlock;
				Block block = m_BlockData[subBlock];
				oldBlockBuffer.TryAdd(block, subBlock);
			}
		}

		private void RemoveUnusedOldBlocks(int jobIndex, DynamicBuffer<SubBlock> blocks, NativeParallelHashMap<Block, Entity> oldBlockBuffer)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Entity val = default(Entity);
			for (int i = 0; i < blocks.Length; i++)
			{
				Entity subBlock = blocks[i].m_SubBlock;
				Block block = m_BlockData[subBlock];
				if (oldBlockBuffer.TryGetValue(block, ref val))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(jobIndex, subBlock, default(Deleted));
					oldBlockBuffer.Remove(block);
				}
			}
		}

		private void CreateBlocks(int jobIndex, Entity owner, NativeParallelHashMap<Block, Entity> oldBlockBuffer, Owner ownerOwner, Composition composition, Edge edge, Curve curve, Game.Net.BuildOrder buildOrder, Road road)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			RoadComposition roadComposition = default(RoadComposition);
			if (!m_RoadCompositionData.TryGetComponent(composition.m_Edge, ref roadComposition) || (roadComposition.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) == 0)
			{
				return;
			}
			Owner owner2 = default(Owner);
			while (ownerOwner.m_Owner != Entity.Null)
			{
				if (m_BuildingData.HasComponent(ownerOwner.m_Owner))
				{
					return;
				}
				m_OwnerData.TryGetComponent(ownerOwner.m_Owner, ref owner2);
				ownerOwner = owner2;
			}
			NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
			if ((netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0)
			{
				return;
			}
			bool flag = (netCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0;
			bool flag2 = (netCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) == 0;
			if (!flag && !flag2)
			{
				return;
			}
			uint buildOrder2 = math.max(buildOrder.m_Start, buildOrder.m_End);
			bool flag3 = (road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0;
			bool flag4 = (road.m_Flags & Game.Net.RoadFlags.EndHalfAligned) != 0;
			if (flag)
			{
				int cellWidth = ZoneUtils.GetCellWidth(netCompositionData.m_Width - netCompositionData.m_MiddleOffset * 2f);
				CreateBlocks(jobIndex, owner, edge.m_Start, edge.m_End, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, curve.m_Bezier, cellWidth, buildOrder.m_Start, buildOrder.m_End, flag3, flag4, invert: false);
			}
			if (flag2)
			{
				int cellWidth2 = ZoneUtils.GetCellWidth(netCompositionData.m_Width + netCompositionData.m_MiddleOffset * 2f);
				CreateBlocks(jobIndex, owner, edge.m_End, edge.m_Start, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, MathUtils.Invert(curve.m_Bezier), cellWidth2, buildOrder.m_End, buildOrder.m_Start, flag4, flag3, invert: true);
			}
			if ((m_PrefabCompositionData[composition.m_StartNode].m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
			{
				if (flag)
				{
					CreateBlocks(jobIndex, owner, edge.m_Start, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, buildOrder2, start: true, right: false);
				}
				if (flag2)
				{
					CreateBlocks(jobIndex, owner, edge.m_Start, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, buildOrder2, start: true, right: true);
				}
			}
			if ((m_PrefabCompositionData[composition.m_EndNode].m_Flags.m_General & CompositionFlags.General.Roundabout) != 0)
			{
				if (flag2)
				{
					CreateBlocks(jobIndex, owner, edge.m_End, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, buildOrder2, start: false, right: false);
				}
				if (flag)
				{
					CreateBlocks(jobIndex, owner, edge.m_End, oldBlockBuffer, roadComposition.m_ZoneBlockPrefab, buildOrder2, start: false, right: true);
				}
			}
		}

		private void CreateBlocks(int jobIndex, Entity owner, Entity node, NativeParallelHashMap<Block, Entity> oldBlockBuffer, Entity blockPrefab, uint buildOrder, bool start, bool right)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0551: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_056c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0716: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0731: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_074c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_0764: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0773: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0776: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			//IL_078b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_0630: Unknown result type (might be due to invalid IL or missing references)
			//IL_0631: Unknown result type (might be due to invalid IL or missing references)
			//IL_0636: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_0648: Unknown result type (might be due to invalid IL or missing references)
			//IL_064a: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_081a: Unknown result type (might be due to invalid IL or missing references)
			//IL_081e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0823: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_0676: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Unknown result type (might be due to invalid IL or missing references)
			//IL_0846: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0859: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0887: Unknown result type (might be due to invalid IL or missing references)
			//IL_0889: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_0897: Unknown result type (might be due to invalid IL or missing references)
			//IL_0873: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0883: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_090b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0910: Unknown result type (might be due to invalid IL or missing references)
			//IL_091e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0920: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0939: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0953: Unknown result type (might be due to invalid IL or missing references)
			//IL_095e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0963: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0981: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_09df: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bcc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d22: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0da0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ddb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e25: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e2a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f47: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f7f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0faa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ef9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
			EdgeNodeGeometry edgeNodeGeometry = ((!start) ? m_EndNodeGeometryData[owner].m_Geometry : m_StartNodeGeometryData[owner].m_Geometry);
			Bezier4x3 curve;
			Bezier4x3 curve2;
			Bezier4x3 curve3;
			Bezier4x3 curve4;
			if (right)
			{
				curve = edgeNodeGeometry.m_Left.m_Right;
				curve2 = edgeNodeGeometry.m_Right.m_Right;
				curve3 = edgeNodeGeometry.m_Right.m_Left;
				curve4 = edgeNodeGeometry.m_Left.m_Left;
			}
			else
			{
				curve = edgeNodeGeometry.m_Left.m_Right;
				curve2 = edgeNodeGeometry.m_Right.m_Right;
				curve3 = edgeNodeGeometry.m_Right.m_Left;
				curve4 = edgeNodeGeometry.m_Left.m_Left;
			}
			float num = float.MaxValue;
			Entity val = owner;
			DynamicBuffer<ConnectedEdge> val2 = m_ConnectedEdges[node];
			for (int i = 0; i < val2.Length; i++)
			{
				Entity edge = val2[i].m_Edge;
				Edge edge2 = m_EdgeData[edge];
				EdgeNodeGeometry geometry;
				if (edge2.m_Start == node)
				{
					geometry = m_StartNodeGeometryData[edge].m_Geometry;
				}
				else
				{
					if (!(edge2.m_End == node))
					{
						continue;
					}
					geometry = m_EndNodeGeometryData[edge].m_Geometry;
				}
				float num2 = ((!right) ? math.distancesq(geometry.m_Right.m_Right.d, edgeNodeGeometry.m_Right.m_Left.d) : math.distancesq(geometry.m_Right.m_Left.d, edgeNodeGeometry.m_Right.m_Right.d));
				if (num2 < num)
				{
					if (right)
					{
						curve4 = geometry.m_Left.m_Left;
						curve3 = geometry.m_Right.m_Left;
					}
					else
					{
						curve = geometry.m_Left.m_Right;
						curve2 = geometry.m_Right.m_Right;
					}
					num = num2;
					val = edge;
				}
			}
			Game.Net.BuildOrder buildOrder2 = default(Game.Net.BuildOrder);
			if (m_BuildOrderData.TryGetComponent(val, ref buildOrder2))
			{
				buildOrder = math.max(buildOrder, math.max(buildOrder2.m_Start, buildOrder2.m_End));
			}
			bool flag = false;
			Composition composition = default(Composition);
			if (m_CompositionData.TryGetComponent(val, ref composition))
			{
				RoadComposition roadComposition = default(RoadComposition);
				flag = !m_RoadCompositionData.TryGetComponent(composition.m_Edge, ref roadComposition) || (roadComposition.m_Flags & Game.Prefabs.RoadFlags.EnableZoning) == 0;
				NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
				flag |= (netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0;
				flag = ((!right) ? (flag | ((netCompositionData.m_Flags.m_Right & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) != 0)) : (flag | ((netCompositionData.m_Flags.m_Left & (CompositionFlags.Side.Raised | CompositionFlags.Side.Lowered)) != 0)));
			}
			CutStart(ref curve, ref curve2, !right && flag);
			CutStart(ref curve4, ref curve3, right && flag);
			curve3 = MathUtils.Invert(curve3);
			curve4 = MathUtils.Invert(curve4);
			float4 val3 = default(float4);
			((float4)(ref val3))._002Ector(MathUtils.Length(((Bezier4x3)(ref curve)).xz), MathUtils.Length(((Bezier4x3)(ref curve2)).xz), MathUtils.Length(((Bezier4x3)(ref curve3)).xz), MathUtils.Length(((Bezier4x3)(ref curve4)).xz));
			float num3 = math.csum(val3);
			if (num3 < 8f)
			{
				return;
			}
			float3 val4 = MathUtils.StartTangent(curve2);
			float2 xz = ((float3)(ref val4)).xz;
			val4 = MathUtils.EndTangent(curve3);
			float2 xz2 = ((float3)(ref val4)).xz;
			if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
			{
				return;
			}
			int num4 = (int)math.floor(num3 / 8f);
			int baseWidth = 0;
			int middleWidth = 0;
			int splitCount = 0;
			if (num4 <= 1)
			{
				return;
			}
			if (num4 <= 3)
			{
				middleWidth = num4;
				splitCount = 1;
			}
			else if (num4 <= 5)
			{
				baseWidth = 2;
				splitCount = 2;
			}
			else if (num4 <= 7)
			{
				baseWidth = 2;
				middleWidth = num4 - 4;
				splitCount = 3;
			}
			else
			{
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 3, 3);
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 3, 0);
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 2, 2);
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 2, 0);
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 3, 2);
				TryOption(ref baseWidth, ref middleWidth, ref splitCount, num4, 2, 3);
			}
			int num5 = math.select(splitCount >> 1, 0, flag || right);
			int num6 = math.select(splitCount >> 1, splitCount, flag || !right);
			if (num5 >= num6)
			{
				return;
			}
			num4 = middleWidth + baseWidth * (splitCount & -2);
			float num7 = (num3 - (float)num4 * 8f) * 0.5f;
			if (num7 > 0f)
			{
				Bounds1 val5 = default(Bounds1);
				((Bounds1)(ref val5))._002Ector(0f, 1f);
				Bezier4x3 val6 = default(Bezier4x3);
				if (MathUtils.ClampLength(((Bezier4x3)(ref curve)).xz, ref val5, num7))
				{
					MathUtils.Divide(curve, ref val6, ref curve, val5.max);
					val3.x = math.max(0f, val3.x - num7);
				}
				else
				{
					float num8 = math.max(0f, num7 - val3.x);
					if (MathUtils.ClampLength(((Bezier4x3)(ref curve2)).xz, ref val5, num8))
					{
						MathUtils.Divide(curve2, ref val6, ref curve2, val5.max);
						val3.y = math.max(0f, val3.y - num8);
					}
					else
					{
						val4 = (curve2.d = curve3.a);
						val4 = (curve2.c = val4);
						val4 = (curve2.a = (curve2.b = val4));
						val3.y = 0f;
					}
					val4 = (curve.d = curve2.a);
					val4 = (curve.c = val4);
					val4 = (curve.a = (curve.b = val4));
					val3.x = 0f;
				}
				((Bounds1)(ref val5))._002Ector(0f, 1f);
				if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve4)).xz, ref val5, num7))
				{
					MathUtils.Divide(curve4, ref curve4, ref val6, val5.min);
					val3.w = math.max(0f, val3.w - num7);
				}
				else
				{
					float num9 = math.max(0f, num7 - val3.w);
					if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve3)).xz, ref val5, num9))
					{
						MathUtils.Divide(curve3, ref curve3, ref val6, val5.min);
						val3.z = math.max(0f, val3.z - num9);
					}
					else
					{
						val4 = (curve3.d = curve2.d);
						val4 = (curve3.c = val4);
						val4 = (curve3.a = (curve3.b = val4));
						val3.z = 0f;
					}
					val4 = (curve4.d = curve3.d);
					val4 = (curve4.c = val4);
					val4 = (curve4.a = (curve4.b = val4));
					val3.w = 0f;
				}
				num3 = math.csum(val3);
			}
			BuildOrder buildOrder3 = new BuildOrder
			{
				m_Order = buildOrder
			};
			CurvePosition curvePosition = new CurvePosition
			{
				m_CurvePosition = float2.op_Implicit(math.select(1f, 0f, start))
			};
			int2 val7 = default(int2);
			float num12 = default(float);
			float2 val18 = default(float2);
			Entity val19 = default(Entity);
			for (int j = num5; j < num6; j++)
			{
				int num10 = math.select(baseWidth, middleWidth, j == splitCount >> 1 && middleWidth > 0);
				((int2)(ref val7))._002Ector(j, j + 1);
				val7 = val7 * baseWidth + math.select(int2.op_Implicit(0), int2.op_Implicit(middleWidth - baseWidth), (val7 > splitCount >> 1) & (middleWidth > 0));
				float2 val8 = new float2((float)val7.x / (float)num4, (float)val7.y / (float)num4) * num3;
				CutCurves(curve, curve2, curve3, curve4, val3, val8, out var curve1B, out var curve2B, out var curve3B, out var curve4B);
				if (math.distancesq(curve1B.a, curve1B.d) < 0.01f)
				{
					if (math.distancesq(curve2B.a, curve2B.d) < 0.01f)
					{
						if (math.distancesq(curve3B.a, curve3B.d) < 0.01f)
						{
							val4 = MathUtils.StartTangent(curve4B);
							xz = ((float3)(ref val4)).xz;
						}
						else
						{
							val4 = MathUtils.StartTangent(curve3B);
							xz = ((float3)(ref val4)).xz;
						}
					}
					else
					{
						val4 = MathUtils.StartTangent(curve2B);
						xz = ((float3)(ref val4)).xz;
					}
				}
				else
				{
					val4 = MathUtils.StartTangent(curve1B);
					xz = ((float3)(ref val4)).xz;
				}
				if (math.distancesq(curve4B.a, curve4B.d) < 0.01f)
				{
					if (math.distancesq(curve3B.a, curve3B.d) < 0.01f)
					{
						if (math.distancesq(curve2B.a, curve2B.d) < 0.01f)
						{
							val4 = MathUtils.EndTangent(curve1B);
							xz2 = ((float3)(ref val4)).xz;
						}
						else
						{
							val4 = MathUtils.EndTangent(curve2B);
							xz2 = ((float3)(ref val4)).xz;
						}
					}
					else
					{
						val4 = MathUtils.EndTangent(curve3B);
						xz2 = ((float3)(ref val4)).xz;
					}
				}
				else
				{
					val4 = MathUtils.EndTangent(curve4B);
					xz2 = ((float3)(ref val4)).xz;
				}
				if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
				{
					continue;
				}
				float2 val9 = MathUtils.Right(xz);
				float2 val10 = MathUtils.Right(xz2);
				float3 val11 = curve1B.a;
				float3 val12 = curve4B.d;
				float2 val13 = ((float3)(ref val12)).xz - ((float3)(ref val11)).xz;
				if (!MathUtils.TryNormalize(ref val13))
				{
					continue;
				}
				float2 val14 = MathUtils.Right(val13);
				float num11 = math.max(math.max(MathUtils.MaxDot(((Bezier4x3)(ref curve1B)).xz, val14, ref num12), MathUtils.MaxDot(((Bezier4x3)(ref curve2B)).xz, val14, ref num12)), math.max(MathUtils.MaxDot(((Bezier4x3)(ref curve3B)).xz, val14, ref num12), MathUtils.MaxDot(((Bezier4x3)(ref curve4B)).xz, val14, ref num12)));
				num11 -= math.dot(((float3)(ref val11)).xz, val14);
				float num13 = math.distance(((float3)(ref val11)).xz, ((float3)(ref val12)).xz);
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz + val9 * math.clamp(num11 / math.dot(val9, val14), 0f, num13);
				((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val10 * math.clamp(num11 / math.dot(val10, val14), 0f, num13);
				float3 val15 = math.lerp(val11, val12, 0.5f);
				float2 val16 = val13 * ((float)num10 * 4f);
				val11 = val15;
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz - val16;
				val12 = val15;
				((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val16;
				float2 val17 = val8;
				if (MathUtils.Intersect(((Bezier4x3)(ref curve1B)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val18, 4))
				{
					val17.x = math.max(val17.x, val18.x * val3.x);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve2B)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val18, 4))
				{
					val17.x = math.max(val17.x, val3.x + val18.x * val3.y);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve3B)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val18, 4))
				{
					val17.x = math.max(val17.x, val3.x + val3.y + val18.x * val3.z);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve4B)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val18, 4))
				{
					val17.x = math.max(val17.x, val3.x + val3.y + val3.z + val18.x * val3.w);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve1B)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val18, 4))
				{
					val17.y = math.min(val17.y, val18.x * val3.x);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve2B)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val18, 4))
				{
					val17.y = math.min(val17.y, val3.x + val18.x * val3.y);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve3B)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val18, 4))
				{
					val17.y = math.min(val17.y, val3.x + val3.y + val18.x * val3.z);
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref curve4B)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val18, 4))
				{
					val17.y = math.min(val17.y, val3.x + val3.y + val3.z + val18.x * val3.w);
				}
				val17.x = math.min(val17.x, num3);
				val17.y = math.max(val17.y, 0f);
				CutCurves(curve, curve2, curve3, curve4, val3, val17, out curve1B, out curve2B, out curve3B, out curve4B);
				num11 = math.max(math.max(MathUtils.MaxDot(((Bezier4x3)(ref curve1B)).xz, val14, ref num12), MathUtils.MaxDot(((Bezier4x3)(ref curve2B)).xz, val14, ref num12)), math.max(MathUtils.MaxDot(((Bezier4x3)(ref curve3B)).xz, val14, ref num12), MathUtils.MaxDot(((Bezier4x3)(ref curve4B)).xz, val14, ref num12)));
				num11 -= math.dot(((float3)(ref val11)).xz, val14);
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz + val14 * (num11 + 24f);
				int num14 = (num10 + 10 - 1) / 10;
				for (int k = 0; k < num14; k++)
				{
					int num15 = k * num10 / num14;
					int num16 = (k + 1) * num10 / num14;
					Block block = new Block
					{
						m_Position = val11
					};
					ref float3 position = ref block.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + val13 * ((float)(num15 + num16) * 4f);
					block.m_Direction = -val14;
					block.m_Size.x = (byte)(num16 - num15);
					block.m_Size.y = 6;
					if (oldBlockBuffer.TryGetValue(block, ref val19))
					{
						oldBlockBuffer.Remove(block);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val19, new PrefabRef(blockPrefab));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurvePosition>(jobIndex, val19, curvePosition);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<BuildOrder>(jobIndex, val19, buildOrder3);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val19, default(Updated));
						continue;
					}
					ZoneBlockData zoneBlockData = m_ZoneBlockData[blockPrefab];
					Entity val20 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, zoneBlockData.m_Archetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val20, new PrefabRef(blockPrefab));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Block>(jobIndex, val20, block);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurvePosition>(jobIndex, val20, curvePosition);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<BuildOrder>(jobIndex, val20, buildOrder3);
					DynamicBuffer<Cell> val21 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Cell>(jobIndex, val20);
					int num17 = block.m_Size.x * block.m_Size.y;
					for (int l = 0; l < num17; l++)
					{
						val21.Add(default(Cell));
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val20, new Owner
					{
						m_Owner = owner
					});
				}
			}
		}

		private void TryOption(ref int baseWidth, ref int middleWidth, ref int splitCount, int totalWidth, int newBaseWidth, int newMiddleWidth)
		{
			int num = math.min(1, newMiddleWidth);
			num += (math.max(0, totalWidth - newMiddleWidth) / math.max(1, newBaseWidth)) & -2;
			int num2 = middleWidth + baseWidth * (splitCount & -2);
			if (newMiddleWidth + newBaseWidth * (num & -2) > num2)
			{
				baseWidth = newBaseWidth;
				middleWidth = newMiddleWidth;
				splitCount = num;
			}
		}

		private void CutStart(ref Bezier4x3 curve1, ref Bezier4x3 curve2, bool cutFirst)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			float num = 8f;
			if (cutFirst)
			{
				curve1.a = (curve1.b = (curve1.c = (curve1.d = curve2.a)));
				return;
			}
			Bezier4x3 val2 = default(Bezier4x3);
			if (FindCutPos(curve1, curve1, out var t))
			{
				if (t != 0f)
				{
					Bounds1 val = default(Bounds1);
					((Bounds1)(ref val))._002Ector(0f, t);
					if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve1)).xz, ref val, num))
					{
						MathUtils.Divide(curve1, ref val2, ref curve1, val.min);
					}
				}
				return;
			}
			FindCutPos(curve1, curve2, out t);
			if (t != 0f)
			{
				Bounds1 val3 = default(Bounds1);
				((Bounds1)(ref val3))._002Ector(0f, t);
				if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve2)).xz, ref val3, num))
				{
					MathUtils.Divide(curve2, ref val2, ref curve2, val3.min);
					num = 0f;
				}
				else
				{
					num -= MathUtils.Length(((Bezier4x3)(ref curve2)).xz, val3);
				}
			}
			if (num > 0f)
			{
				Bounds1 val4 = default(Bounds1);
				((Bounds1)(ref val4))._002Ector(0f, 1f);
				if (MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve1)).xz, ref val4, num))
				{
					MathUtils.Divide(curve1, ref val2, ref curve1, val4.min);
				}
			}
			else
			{
				curve1.a = (curve1.b = (curve1.c = (curve1.d = curve2.a)));
			}
		}

		private bool FindCutPos(Bezier4x3 startCurve, Bezier4x3 curve, out float t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			Bezier4x3 val = default(Bezier4x3);
			((Bezier4x3)(ref val))._002Ector(curve.a - startCurve.a, curve.b - startCurve.a, curve.c - startCurve.a, curve.d - startCurve.a);
			float2 val2 = default(float2);
			((float2)(ref val2))._002Ector(0f, 1f);
			float3 val3 = MathUtils.StartTangent(startCurve);
			float2 xz = ((float3)(ref val3)).xz;
			t = 0f;
			if (!MathUtils.TryNormalize(ref xz))
			{
				return true;
			}
			for (int i = 0; i < 8; i++)
			{
				float num = math.csum(val2) * 0.5f;
				val3 = MathUtils.Position(val, num);
				float2 xz2 = ((float3)(ref val3)).xz;
				val3 = MathUtils.Tangent(val, num);
				float2 xz3 = ((float3)(ref val3)).xz;
				if (!MathUtils.TryNormalize(ref xz3))
				{
					return true;
				}
				if (math.dot(xz, xz2 - xz3 * 8f) - math.abs(math.dot(xz, MathUtils.Right(xz3) * 16f)) < 0f)
				{
					val2.x = num;
				}
				else
				{
					val2.y = num;
				}
			}
			if (val2.x != 0f)
			{
				t = val2.y;
				return val2.y != 1f;
			}
			return true;
		}

		private void CutCurves(Bezier4x3 curve1, Bezier4x3 curve2, Bezier4x3 curve3, Bezier4x3 curve4, float4 curveLengths, float2 cutRange, out Bezier4x3 curve1B, out Bezier4x3 curve2B, out Bezier4x3 curve3B, out Bezier4x3 curve4B)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(0f, 1f);
			float2 val2 = val;
			float2 val3 = val;
			float2 val4 = val;
			float2 val5 = val;
			curve1B = curve1;
			curve2B = curve2;
			curve3B = curve3;
			curve4B = curve4;
			if (cutRange.x - curveLengths.x - curveLengths.y < curveLengths.z)
			{
				if (cutRange.x - curveLengths.x < curveLengths.y)
				{
					if (cutRange.x < curveLengths.x)
					{
						val2.x = cutRange.x / curveLengths.x;
					}
					else
					{
						val2.x = 2f;
						val3.x = math.saturate((cutRange.x - curveLengths.x) / curveLengths.y);
					}
				}
				else
				{
					val2.x = 2f;
					val3.x = 2f;
					val4.x = math.saturate((cutRange.x - curveLengths.x - curveLengths.y) / curveLengths.z);
				}
			}
			else
			{
				val2.x = 2f;
				val3.x = 2f;
				val4.x = 2f;
				val5.x = math.saturate((cutRange.x - curveLengths.x - curveLengths.y - curveLengths.z) / curveLengths.w);
			}
			if (cutRange.y > curveLengths.x)
			{
				if (cutRange.y - curveLengths.x > curveLengths.y)
				{
					if (cutRange.y - curveLengths.x - curveLengths.y > curveLengths.z)
					{
						val5.y = math.saturate((cutRange.y - curveLengths.x - curveLengths.y - curveLengths.z) / curveLengths.w);
					}
					else
					{
						val5.y = -1f;
						val4.y = math.saturate((cutRange.y - curveLengths.x - curveLengths.y) / curveLengths.z);
					}
				}
				else
				{
					val5.y = -1f;
					val4.y = -1f;
					val3.y = math.saturate((cutRange.y - curveLengths.x) / curveLengths.y);
				}
			}
			else
			{
				val5.y = -1f;
				val4.y = -1f;
				val3.y = -1f;
				val2.y = math.saturate(cutRange.y / curveLengths.x);
			}
			if (math.any(val2 != val) && val2.x <= val2.y)
			{
				curve1B = MathUtils.Cut(curve1, val2);
			}
			if (math.any(val3 != val) && val3.x <= val3.y)
			{
				curve2B = MathUtils.Cut(curve2, val3);
			}
			if (math.any(val4 != val) && val4.x <= val4.y)
			{
				curve3B = MathUtils.Cut(curve3, val4);
			}
			if (math.any(val5 != val) && val5.x <= val5.y)
			{
				curve4B = MathUtils.Cut(curve4, val5);
			}
			if (val4.x == 2f)
			{
				curve3B.a = (curve3B.b = (curve3B.c = (curve3B.d = curve4B.a)));
			}
			if (val3.x == 2f)
			{
				curve2B.a = (curve2B.b = (curve2B.c = (curve2B.d = curve3B.a)));
			}
			if (val2.x == 2f)
			{
				curve1B.a = (curve1B.b = (curve1B.c = (curve1B.d = curve2B.a)));
			}
			if (val3.y == -1f)
			{
				curve2B.a = (curve2B.b = (curve2B.c = (curve2B.d = curve1B.d)));
			}
			if (val4.y == -1f)
			{
				curve3B.a = (curve3B.b = (curve3B.c = (curve3B.d = curve2B.d)));
			}
			if (val5.y == -1f)
			{
				curve4B.a = (curve4B.b = (curve4B.c = (curve4B.d = curve3B.d)));
			}
		}

		private bool FindContinuousEdge(Entity node, Entity edge, float2 position, float2 tangent, int cellWidth, bool halfAligned, bool invert)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ConnectedEdge> val = m_ConnectedEdges[node];
			for (int i = 0; i < val.Length; i++)
			{
				Entity edge2 = val[i].m_Edge;
				if (edge2 == edge)
				{
					continue;
				}
				Edge edge3 = m_EdgeData[edge2];
				Curve curve = m_CurveData[edge2];
				float2 xz;
				float3 val2;
				float2 val3;
				bool flag;
				if (edge3.m_Start == node)
				{
					xz = ((float3)(ref curve.m_Bezier.a)).xz;
					val2 = MathUtils.StartTangent(curve.m_Bezier);
					val3 = ((float3)(ref val2)).xz;
					flag = invert;
				}
				else
				{
					if (!(edge3.m_End == node))
					{
						continue;
					}
					xz = ((float3)(ref curve.m_Bezier.d)).xz;
					val2 = MathUtils.EndTangent(curve.m_Bezier);
					val3 = -((float3)(ref val2)).xz;
					flag = !invert;
				}
				if (!MathUtils.TryNormalize(ref val3) || math.dot(tangent, val3) > -0.99f || math.distance(position, xz) > 0.01f)
				{
					continue;
				}
				Entity edge4 = m_CompositionData[edge2].m_Edge;
				if (!m_RoadCompositionData.HasComponent(edge4) || (m_RoadCompositionData[edge4].m_Flags & Game.Prefabs.RoadFlags.EnableZoning) == 0)
				{
					continue;
				}
				NetCompositionData netCompositionData = m_PrefabCompositionData[edge4];
				if ((netCompositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) != 0)
				{
					continue;
				}
				int cellWidth2 = ZoneUtils.GetCellWidth(netCompositionData.m_Width + netCompositionData.m_MiddleOffset * math.select(-2f, 2f, flag));
				if (cellWidth != cellWidth2)
				{
					continue;
				}
				Road road = m_RoadData[edge2];
				if (edge3.m_Start == node)
				{
					if ((road.m_Flags & Game.Net.RoadFlags.StartHalfAligned) != 0 != halfAligned)
					{
						continue;
					}
				}
				else if ((road.m_Flags & Game.Net.RoadFlags.EndHalfAligned) != 0 != halfAligned)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		private void CreateBlocks(int jobIndex, Entity owner, Entity startNode, Entity endNode, NativeParallelHashMap<Block, Entity> oldBlockBuffer, Entity blockPrefab, Bezier4x3 curve, int cellWidth, uint startOrder, uint endOrder, bool startHalf, bool endHalf, bool invert)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0655: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_065f: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0715: Unknown result type (might be due to invalid IL or missing references)
			//IL_071c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0721: Unknown result type (might be due to invalid IL or missing references)
			//IL_0728: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0732: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0784: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0792: Unknown result type (might be due to invalid IL or missing references)
			//IL_0799: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_074a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0758: Unknown result type (might be due to invalid IL or missing references)
			//IL_07de: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0800: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_0808: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0858: Unknown result type (might be due to invalid IL or missing references)
			//IL_085a: Unknown result type (might be due to invalid IL or missing references)
			//IL_085c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0825: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_082e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0834: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0840: Unknown result type (might be due to invalid IL or missing references)
			//IL_0845: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_0848: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_084f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0851: Unknown result type (might be due to invalid IL or missing references)
			//IL_0865: Unknown result type (might be due to invalid IL or missing references)
			//IL_086a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0893: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0901: Unknown result type (might be due to invalid IL or missing references)
			//IL_0906: Unknown result type (might be due to invalid IL or missing references)
			//IL_0914: Unknown result type (might be due to invalid IL or missing references)
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_0925: Unknown result type (might be due to invalid IL or missing references)
			//IL_0927: Unknown result type (might be due to invalid IL or missing references)
			//IL_092c: Unknown result type (might be due to invalid IL or missing references)
			//IL_095a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0961: Unknown result type (might be due to invalid IL or missing references)
			//IL_0966: Unknown result type (might be due to invalid IL or missing references)
			//IL_096d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0978: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0985: Unknown result type (might be due to invalid IL or missing references)
			//IL_098a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0993: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			float3 val = MathUtils.StartTangent(curve);
			float2 xz = ((float3)(ref val)).xz;
			val = MathUtils.EndTangent(curve);
			float2 xz2 = ((float3)(ref val)).xz;
			if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
			{
				return;
			}
			bool flag = FindContinuousEdge(startNode, owner, ((float3)(ref curve.a)).xz, xz, cellWidth, startHalf, !invert);
			bool flag2 = FindContinuousEdge(endNode, owner, ((float3)(ref curve.d)).xz, -xz2, cellWidth, endHalf, invert);
			float num = NetUtils.FindMiddleTangentPos(((Bezier4x3)(ref curve)).xz, new float2(0f, 1f));
			Bezier4x3 val2 = default(Bezier4x3);
			Bezier4x3 val3 = default(Bezier4x3);
			MathUtils.Divide(curve, ref val2, ref val3, num);
			float num2 = (float)cellWidth * 4f;
			val2 = NetUtils.OffsetCurveLeftSmooth(val2, float2.op_Implicit(0f - num2));
			val3 = NetUtils.OffsetCurveLeftSmooth(val3, float2.op_Implicit(0f - num2));
			float num3 = MathUtils.Length(((Bezier4x3)(ref val2)).xz) + MathUtils.Length(((Bezier4x3)(ref val3)).xz);
			if (num3 < 8f)
			{
				return;
			}
			val = MathUtils.StartTangent(val2);
			xz = ((float3)(ref val)).xz;
			val = MathUtils.EndTangent(val3);
			xz2 = ((float3)(ref val)).xz;
			if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
			{
				return;
			}
			float num4 = math.degrees(math.acos(math.clamp(math.dot(xz, xz2), -1f, 1f)) * 8f / num3);
			float num5 = 2f / math.sqrt(math.clamp(num4 / 15f, 0.0001f, 1f)) * 8f;
			int num6 = math.max(1, (int)(num3 / num5));
			BuildOrder buildOrder = new BuildOrder
			{
				m_Order = startOrder
			};
			float2 val4 = default(float2);
			Bezier4x3 val7 = default(Bezier4x3);
			Bezier4x3 val8 = default(Bezier4x3);
			float num10 = default(float);
			float2 val19 = default(float2);
			Entity val20 = default(Entity);
			for (int i = 0; i < num6; i++)
			{
				((float2)(ref val4))._002Ector((float)i / (float)num6, (float)(i + 1) / (float)num6);
				float2 val5 = math.min(float2.op_Implicit(1f), val4 / num);
				float2 val6 = math.max(float2.op_Implicit(0f), (val4 - num) / (1f - num));
				if (val5.x < 1f)
				{
					val = MathUtils.Tangent(val2, val5.x);
					xz = ((float3)(ref val)).xz;
					val7 = MathUtils.Cut(val2, val5);
				}
				else
				{
					val = MathUtils.Tangent(val3, val6.x);
					xz = ((float3)(ref val)).xz;
					val = (val7.d = MathUtils.Position(val3, val6.x));
					val = (val7.c = val);
					val = (val7.a = (val7.b = val));
				}
				if (val5.y < 1f)
				{
					val = MathUtils.Tangent(val2, val5.y);
					xz2 = ((float3)(ref val)).xz;
					val = (val8.d = MathUtils.Position(val2, val5.y));
					val = (val8.c = val);
					val = (val8.a = (val8.b = val));
				}
				else
				{
					val = MathUtils.Tangent(val3, val6.y);
					xz2 = ((float3)(ref val)).xz;
					val8 = MathUtils.Cut(val3, val6);
				}
				if (!MathUtils.TryNormalize(ref xz) || !MathUtils.TryNormalize(ref xz2))
				{
					continue;
				}
				float2 val9 = MathUtils.Right(xz);
				float2 val10 = MathUtils.Right(xz2);
				float3 val11 = val7.a;
				float3 val12 = val8.d;
				float2 val13 = ((float3)(ref val12)).xz - ((float3)(ref val11)).xz;
				if (!MathUtils.TryNormalize(ref val13))
				{
					continue;
				}
				if (i == 0)
				{
					if (flag)
					{
						((float3)(ref val11)).xz = ((float3)(ref val11)).xz - val13 * math.select(0f, 4f, ((cellWidth & 1) != 0) ^ startHalf);
					}
					else
					{
						float num7 = num2 - math.select(0f, 8f, cellWidth > 1);
						num7 += math.select(0f, math.select(4f, -4f, (cellWidth & 1) != 0), startHalf);
						((float3)(ref val11)).xz = ((float3)(ref val11)).xz - val13 * num7;
					}
				}
				if (i == num6 - 1)
				{
					if (flag2)
					{
						((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val13 * math.select(0f, 4f, ((cellWidth & 1) != 0) ^ endHalf);
					}
					else
					{
						float num8 = num2 - math.select(0f, 8f, cellWidth > 1);
						num8 += math.select(0f, math.select(4f, -4f, (cellWidth & 1) != 0), endHalf);
						((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val13 * num8;
					}
				}
				float2 val14 = MathUtils.Right(val13);
				float num9 = math.max(MathUtils.MaxDot(((Bezier4x3)(ref val7)).xz, val14, ref num10), MathUtils.MaxDot(((Bezier4x3)(ref val8)).xz, val14, ref num10));
				num9 -= math.dot(((float3)(ref val11)).xz, val14);
				float num11 = math.distance(((float3)(ref val11)).xz, ((float3)(ref val12)).xz);
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz + val9 * math.clamp(num9 / math.dot(val9, val14), 0f, num11);
				((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val10 * math.clamp(num9 / math.dot(val10, val14), 0f, num11);
				float3 val15 = val12 - val11;
				int num12 = (int)math.floor((math.length(((float3)(ref val15)).xz) + 0.1f) / 8f);
				if (num12 < 2)
				{
					continue;
				}
				float3 val16 = math.lerp(val11, val12, 0.5f);
				float2 val17 = val13 * ((float)num12 * 4f);
				val11 = val16;
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz - val17;
				val12 = val16;
				((float3)(ref val12)).xz = ((float3)(ref val12)).xz + val17;
				float2 val18 = val4;
				if (MathUtils.Intersect(((Bezier4x3)(ref val7)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val19, 4))
				{
					val18.x = math.max(val18.x, math.lerp(val4.x, val4.y, val19.x * num));
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref val8)).xz, new Segment(((float3)(ref val11)).xz, ((float3)(ref val11)).xz - val14 * 48f), ref val19, 4))
				{
					val18.x = math.max(val18.x, math.lerp(val4.x, val4.y, val19.x * (1f - num) + num));
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref val7)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val19, 4))
				{
					val18.y = math.min(val18.y, math.lerp(val4.x, val4.y, val19.x * num));
				}
				if (MathUtils.Intersect(((Bezier4x3)(ref val8)).xz, new Segment(((float3)(ref val12)).xz, ((float3)(ref val12)).xz - val14 * 48f), ref val19, 4))
				{
					val18.y = math.min(val18.y, math.lerp(val4.x, val4.y, val19.x * (1f - num) + num));
				}
				val5 = math.min(float2.op_Implicit(1f), val18 / num);
				val6 = math.max(float2.op_Implicit(0f), (val18 - num) / (1f - num));
				if (val5.x < 1f)
				{
					val7 = MathUtils.Cut(val2, val5);
				}
				else
				{
					val = (val7.d = MathUtils.Position(val3, val6.x));
					val = (val7.c = val);
					val = (val7.a = (val7.b = val));
				}
				if (val5.y < 1f)
				{
					val = (val8.d = MathUtils.Position(val2, val5.y));
					val = (val8.c = val);
					val = (val8.a = (val8.b = val));
				}
				else
				{
					val8 = MathUtils.Cut(val3, val6);
				}
				num9 = math.max(MathUtils.MaxDot(((Bezier4x3)(ref val7)).xz, val14, ref num10), MathUtils.MaxDot(((Bezier4x3)(ref val8)).xz, val14, ref num10));
				num9 -= math.dot(((float3)(ref val11)).xz, val14);
				((float3)(ref val11)).xz = ((float3)(ref val11)).xz + val14 * (num9 + 24f);
				int num13 = (num12 + 10 - 1) / 10;
				for (int j = 0; j < num13; j++)
				{
					int num14 = j * num12 / num13;
					int num15 = (j + 1) * num12 / num13;
					Block block = new Block
					{
						m_Position = val11
					};
					ref float3 position = ref block.m_Position;
					((float3)(ref position)).xz = ((float3)(ref position)).xz + val13 * ((float)(num14 + num15) * 4f);
					block.m_Direction = -val14;
					block.m_Size.x = (byte)(num15 - num14);
					block.m_Size.y = 6;
					CurvePosition curvePosition = default(CurvePosition);
					curvePosition.m_CurvePosition = math.lerp(float2.op_Implicit(val4.x), float2.op_Implicit(val4.y), new float2((float)num15, (float)num14) / (float)num12);
					curvePosition.m_CurvePosition = math.select(curvePosition.m_CurvePosition, 1f - curvePosition.m_CurvePosition, invert);
					if (endOrder > startOrder)
					{
						buildOrder.m_Order++;
					}
					else if (endOrder < startOrder)
					{
						buildOrder.m_Order--;
					}
					if (oldBlockBuffer.TryGetValue(block, ref val20))
					{
						oldBlockBuffer.Remove(block);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val20, new PrefabRef(blockPrefab));
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurvePosition>(jobIndex, val20, curvePosition);
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<BuildOrder>(jobIndex, val20, buildOrder);
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(jobIndex, val20, default(Updated));
						continue;
					}
					ZoneBlockData zoneBlockData = m_ZoneBlockData[blockPrefab];
					Entity val21 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, zoneBlockData.m_Archetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val21, new PrefabRef(blockPrefab));
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Block>(jobIndex, val21, block);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CurvePosition>(jobIndex, val21, curvePosition);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<BuildOrder>(jobIndex, val21, buildOrder);
					DynamicBuffer<Cell> val22 = ((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<Cell>(jobIndex, val21);
					int num16 = block.m_Size.x * block.m_Size.y;
					for (int k = 0; k < num16; k++)
					{
						val22.Add(default(Cell));
					}
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Owner>(jobIndex, val21, new Owner
					{
						m_Owner = owner
					});
				}
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
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.BuildOrder> __Game_Net_BuildOrder_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<SubBlock> __Game_Zones_SubBlock_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.BuildOrder> __Game_Net_BuildOrder_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Road> __Game_Net_Road_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneBlockData> __Game_Prefabs_ZoneBlockData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_BuildOrder_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.BuildOrder>(true);
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Zones_SubBlock_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<SubBlock>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Net_BuildOrder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.BuildOrder>(true);
			__Game_Net_Road_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Road>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_ZoneBlockData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneBlockData>(true);
			__Game_Net_ConnectedEdge_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ConnectedEdge>(true);
		}
	}

	private EntityQuery m_UpdatedEdgesQuery;

	private ModificationBarrier4 m_ModificationBarrier;

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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<SubBlock>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedEdgesQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_UpdatedEdgesQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		UpdateBlocksJob updateBlocksJob = new UpdateBlocksJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrderType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubBlockType = InternalCompilerInterface.GetBufferTypeHandle<SubBlock>(ref __TypeHandle.__Game_Zones_SubBlock_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<Game.Net.BuildOrder>(ref __TypeHandle.__Game_Net_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadData = InternalCompilerInterface.GetComponentLookup<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RoadCompositionData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockData = InternalCompilerInterface.GetComponentLookup<ZoneBlockData>(ref __TypeHandle.__Game_Prefabs_ZoneBlockData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdges = InternalCompilerInterface.GetBufferLookup<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_ModificationBarrier.CreateCommandBuffer();
		updateBlocksJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<UpdateBlocksJob>(updateBlocksJob, m_UpdatedEdgesQuery, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
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
	public BlockSystem()
	{
	}
}
