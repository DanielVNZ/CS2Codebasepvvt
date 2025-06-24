using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Mathematics;
using Game.Common;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Areas;

[CompilerGenerated]
public class UpdateCollectSystem : GameSystemBase
{
	private struct UpdateBufferData
	{
		public NativeList<Bounds2> m_Bounds;

		public EntityQuery m_Query;

		public JobHandle m_WriteDependencies;

		public JobHandle m_ReadDependencies;

		public bool m_IsUpdated;

		public void Create(EntityQuery query)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			m_Bounds = new NativeList<Bounds2>(AllocatorHandle.op_Implicit((Allocator)4));
			m_Query = query;
		}

		public void Dispose()
		{
			m_Bounds.Dispose();
		}

		public void Clear()
		{
			((JobHandle)(ref m_WriteDependencies)).Complete();
			((JobHandle)(ref m_ReadDependencies)).Complete();
			m_Bounds.Clear();
			m_IsUpdated = false;
		}
	}

	[BurstCompile]
	private struct CollectUpdatedAreaBoundsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentTypeHandle<Created> m_CreatedType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_SearchTree;

		[ReadOnly]
		public NativeParallelHashMap<Entity, int> m_TriangleCount;

		public ParallelWriter<Bounds2> m_ResultQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Created>(ref m_CreatedType))
			{
				BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
				BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					DynamicBuffer<Node> nodes = bufferAccessor[i];
					DynamicBuffer<Triangle> val = bufferAccessor2[i];
					for (int j = 0; j < val.Length; j++)
					{
						Bounds3 val2 = MathUtils.Bounds(AreaUtils.GetTriangle3(nodes, val[j]));
						m_ResultQueue.Enqueue(((Bounds3)(ref val2)).xz);
					}
				}
				return;
			}
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				int num = default(int);
				QuadTreeBoundsXZ quadTreeBoundsXZ = default(QuadTreeBoundsXZ);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity val3 = nativeArray[k];
					if (!m_TriangleCount.TryGetValue(val3, ref num))
					{
						continue;
					}
					for (int l = 0; l < num; l++)
					{
						if (m_SearchTree.TryGet(new AreaSearchItem(val3, l), ref quadTreeBoundsXZ))
						{
							m_ResultQueue.Enqueue(((Bounds3)(ref quadTreeBoundsXZ.m_Bounds)).xz);
						}
					}
				}
				return;
			}
			NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			BufferAccessor<Node> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			BufferAccessor<Triangle> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			int num2 = default(int);
			QuadTreeBoundsXZ quadTreeBoundsXZ2 = default(QuadTreeBoundsXZ);
			QuadTreeBoundsXZ quadTreeBoundsXZ3 = default(QuadTreeBoundsXZ);
			for (int m = 0; m < nativeArray2.Length; m++)
			{
				Entity val4 = nativeArray2[m];
				DynamicBuffer<Node> nodes2 = bufferAccessor3[m];
				DynamicBuffer<Triangle> val5 = bufferAccessor4[m];
				if (m_TriangleCount.TryGetValue(val4, ref num2))
				{
					int num3 = math.min(num2, val5.Length);
					for (int n = 0; n < num3; n++)
					{
						Bounds3 val6 = MathUtils.Bounds(AreaUtils.GetTriangle3(nodes2, val5[n]));
						Bounds2 xz = ((Bounds3)(ref val6)).xz;
						if (m_SearchTree.TryGet(new AreaSearchItem(val4, n), ref quadTreeBoundsXZ2))
						{
							Bounds2 xz2 = ((Bounds3)(ref quadTreeBoundsXZ2.m_Bounds)).xz;
							Bounds2 val7 = xz2 | xz;
							if (math.length(MathUtils.Size(val7)) < math.length(MathUtils.Size(xz2)) + math.length(MathUtils.Size(xz)))
							{
								m_ResultQueue.Enqueue(val7);
								continue;
							}
							m_ResultQueue.Enqueue(xz2);
							m_ResultQueue.Enqueue(xz);
						}
						else
						{
							m_ResultQueue.Enqueue(xz);
						}
					}
					for (int num4 = num3; num4 < num2; num4++)
					{
						if (m_SearchTree.TryGet(new AreaSearchItem(val4, num4), ref quadTreeBoundsXZ3))
						{
							m_ResultQueue.Enqueue(((Bounds3)(ref quadTreeBoundsXZ3.m_Bounds)).xz);
						}
					}
					for (int num5 = num3; num5 < val5.Length; num5++)
					{
						Bounds3 val8 = MathUtils.Bounds(AreaUtils.GetTriangle3(nodes2, val5[num5]));
						m_ResultQueue.Enqueue(((Bounds3)(ref val8)).xz);
					}
				}
				else
				{
					for (int num6 = 0; num6 < val5.Length; num6++)
					{
						Bounds3 val9 = MathUtils.Bounds(AreaUtils.GetTriangle3(nodes2, val5[num6]));
						m_ResultQueue.Enqueue(((Bounds3)(ref val9)).xz);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct DequeueBoundsJob : IJob
	{
		public NativeQueue<Bounds2> m_Queue;

		public NativeList<Bounds2> m_ResultList;

		public void Execute()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			int count = m_Queue.Count;
			m_ResultList.ResizeUninitialized(count);
			for (int i = 0; i < count; i++)
			{
				m_ResultList[i] = m_Queue.Dequeue();
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Node> __Game_Areas_Node_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Triangle> __Game_Areas_Triangle_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Created> __Game_Common_Created_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Areas_Node_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Node>(true);
			__Game_Areas_Triangle_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Triangle>(true);
			__Game_Common_Created_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Created>(true);
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
		}
	}

	private SearchSystem m_SearchSystem;

	private UpdateBufferData m_LotData;

	private UpdateBufferData m_DistrictData;

	private UpdateBufferData m_MapTileData;

	private UpdateBufferData m_SpaceData;

	private TypeHandle __TypeHandle;

	public bool lotsUpdated => m_LotData.m_IsUpdated;

	public bool districtsUpdated => m_DistrictData.m_IsUpdated;

	public bool mapTilesUpdated => m_MapTileData.m_IsUpdated;

	public bool spacesUpdated => m_SpaceData.m_IsUpdated;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_LotData.Create(GetQuery<Lot>());
		m_DistrictData.Create(GetQuery<District>());
		m_MapTileData.Create(GetQuery<MapTile>());
		m_SpaceData.Create(GetQuery<Space>());
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_LotData.Dispose();
		m_DistrictData.Dispose();
		m_MapTileData.Dispose();
		m_SpaceData.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		m_LotData.Clear();
		m_DistrictData.Clear();
		m_MapTileData.Clear();
		m_SpaceData.Clear();
		((COSystemBase)this).OnStopRunning();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependency = ((SystemBase)this).Dependency;
		if (((EntityQuery)(ref m_LotData.m_Query)).IsEmptyIgnoreFilter)
		{
			m_LotData.Clear();
		}
		else
		{
			JobHandle val = UpdateBounds(ref m_LotData, dependency);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val);
		}
		if (((EntityQuery)(ref m_DistrictData.m_Query)).IsEmptyIgnoreFilter)
		{
			m_DistrictData.Clear();
		}
		else
		{
			JobHandle val2 = UpdateBounds(ref m_DistrictData, dependency);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2);
		}
		if (((EntityQuery)(ref m_MapTileData.m_Query)).IsEmptyIgnoreFilter)
		{
			m_MapTileData.Clear();
		}
		else
		{
			JobHandle val3 = UpdateBounds(ref m_MapTileData, dependency);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3);
		}
		if (((EntityQuery)(ref m_SpaceData.m_Query)).IsEmptyIgnoreFilter)
		{
			m_SpaceData.Clear();
			return;
		}
		JobHandle val4 = UpdateBounds(ref m_SpaceData, dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, val4);
	}

	private JobHandle UpdateBounds(ref UpdateBufferData data, JobHandle inputDeps)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		data.m_IsUpdated = true;
		NativeQueue<Bounds2> queue = default(NativeQueue<Bounds2>);
		queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeParallelHashMap<Entity, int> triangleCount;
		NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> searchTree = m_SearchSystem.GetSearchTree(readOnly: true, out dependencies, out triangleCount);
		CollectUpdatedAreaBoundsJob collectUpdatedAreaBoundsJob = new CollectUpdatedAreaBoundsJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetBufferTypeHandle<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TriangleType = InternalCompilerInterface.GetBufferTypeHandle<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatedType = InternalCompilerInterface.GetComponentTypeHandle<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SearchTree = searchTree,
			m_TriangleCount = triangleCount,
			m_ResultQueue = queue.AsParallelWriter()
		};
		DequeueBoundsJob obj = new DequeueBoundsJob
		{
			m_Queue = queue,
			m_ResultList = data.m_Bounds
		};
		JobHandle val = JobChunkExtensions.ScheduleParallel<CollectUpdatedAreaBoundsJob>(collectUpdatedAreaBoundsJob, data.m_Query, JobHandle.CombineDependencies(inputDeps, dependencies));
		JobHandle val2 = IJobExtensions.Schedule<DequeueBoundsJob>(obj, JobHandle.CombineDependencies(val, data.m_ReadDependencies));
		queue.Dispose(val2);
		m_SearchSystem.AddSearchTreeReader(val);
		data.m_WriteDependencies = val2;
		data.m_ReadDependencies = default(JobHandle);
		return val2;
	}

	public NativeList<Bounds2> GetUpdatedLotBounds(out JobHandle dependencies)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_LotData.m_WriteDependencies;
		return m_LotData.m_Bounds;
	}

	public NativeList<Bounds2> GetUpdatedDistrictBounds(out JobHandle dependencies)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_DistrictData.m_WriteDependencies;
		return m_DistrictData.m_Bounds;
	}

	public NativeList<Bounds2> GetUpdatedMapTileBounds(out JobHandle dependencies)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_MapTileData.m_WriteDependencies;
		return m_MapTileData.m_Bounds;
	}

	public NativeList<Bounds2> GetUpdatedSpaceBounds(out JobHandle dependencies)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_SpaceData.m_WriteDependencies;
		return m_SpaceData.m_Bounds;
	}

	public void AddLotBoundsReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_LotData.m_ReadDependencies = JobHandle.CombineDependencies(m_LotData.m_ReadDependencies, handle);
	}

	public void AddDistrictBoundsReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_DistrictData.m_ReadDependencies = JobHandle.CombineDependencies(m_DistrictData.m_ReadDependencies, handle);
	}

	public void AddMapTileBoundsReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_MapTileData.m_ReadDependencies = JobHandle.CombineDependencies(m_MapTileData.m_ReadDependencies, handle);
	}

	public void AddSpaceBoundsReader(JobHandle handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_SpaceData.m_ReadDependencies = JobHandle.CombineDependencies(m_SpaceData.m_ReadDependencies, handle);
	}

	private EntityQuery GetQuery<T>()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Area>(),
			ComponentType.ReadOnly<Node>(),
			ComponentType.ReadOnly<Triangle>(),
			ComponentType.ReadOnly<T>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		return ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
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
	public UpdateCollectSystem()
	{
	}
}
