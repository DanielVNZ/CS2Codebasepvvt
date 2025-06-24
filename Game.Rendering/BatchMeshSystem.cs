using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Rendering;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BatchMeshSystem : GameSystemBase
{
	private struct LoadingData : IComparable<LoadingData>
	{
		public int m_Priority;

		public int m_BatchIndex;

		public LoadingData(int priority, int batchIndex)
		{
			m_Priority = priority;
			m_BatchIndex = batchIndex;
		}

		public int CompareTo(LoadingData other)
		{
			return other.m_Priority - m_Priority;
		}
	}

	private struct MeshInfo
	{
		public RenderPrefab m_Prefab;

		public ShapeAllocation[] m_ShapeAllocations;

		public ulong m_SizeInMemory;

		public int m_GeneratedIndex;

		public int m_BatchCount;
	}

	private struct CacheInfo
	{
		public GeometryAsset m_GeometryAsset;

		public EntityCommandBuffer m_CommandBuffer;

		public JobHandle m_Dependency;
	}

	private struct ShapeAllocation
	{
		public NativeHeapBlock m_Allocation;

		public int m_Stride;

		public float3 m_PositionExtent;

		public float3 m_NormalExtent;
	}

	[BurstCompile]
	private struct LoadingPriorityJob : IJob
	{
		[ReadOnly]
		public int m_PriorityLimit;

		public NativeList<int> m_BatchPriority;

		public NativeList<MeshLoadingState> m_LoadingState;

		public NativeList<LoadingData> m_LoadingData;

		public NativeList<LoadingData> m_UnloadingData;

		public void Execute()
		{
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_LoadingData.Length; i++)
			{
				ref LoadingData reference = ref m_LoadingData.ElementAt(i);
				ref int reference2 = ref m_BatchPriority.ElementAt(reference.m_BatchIndex);
				ref MeshLoadingState reference3 = ref m_LoadingState.ElementAt(reference.m_BatchIndex);
				if (reference3 == MeshLoadingState.Pending)
				{
					if (reference2 < m_PriorityLimit)
					{
						reference3 = MeshLoadingState.None;
						m_LoadingData.RemoveAtSwapBack(i--);
					}
					else
					{
						reference.m_Priority = reference2;
					}
				}
				else
				{
					reference.m_Priority = 1000256 + reference2;
				}
			}
			for (int j = 0; j < m_UnloadingData.Length; j++)
			{
				ref LoadingData reference4 = ref m_UnloadingData.ElementAt(j);
				ref int reference5 = ref m_BatchPriority.ElementAt(reference4.m_BatchIndex);
				ref MeshLoadingState reference6 = ref m_LoadingState.ElementAt(reference4.m_BatchIndex);
				if (reference6 == MeshLoadingState.Obsolete)
				{
					if (reference5 >= m_PriorityLimit)
					{
						reference6 = MeshLoadingState.Complete;
						m_UnloadingData.RemoveAtSwapBack(j--);
					}
					else
					{
						reference4.m_Priority = -reference5;
					}
				}
				else
				{
					reference4.m_Priority = 1000256 - reference5;
				}
			}
			for (int k = 0; k < m_BatchPriority.Length; k++)
			{
				ref int reference7 = ref m_BatchPriority.ElementAt(k);
				ref MeshLoadingState reference8 = ref m_LoadingState.ElementAt(k);
				if (reference7 >= m_PriorityLimit)
				{
					if (reference8 == MeshLoadingState.None)
					{
						reference8 = MeshLoadingState.Pending;
						ref NativeList<LoadingData> reference9 = ref m_LoadingData;
						LoadingData loadingData = new LoadingData(reference7, k);
						reference9.Add(ref loadingData);
					}
					reference7 -= 256;
				}
				else
				{
					if (reference8 == MeshLoadingState.Complete)
					{
						reference8 = MeshLoadingState.Obsolete;
						ref NativeList<LoadingData> reference10 = ref m_UnloadingData;
						LoadingData loadingData = new LoadingData(-reference7, k);
						reference10.Add(ref loadingData);
					}
					reference7 = math.max(-1000000, reference7 - 1);
				}
			}
			if (m_LoadingData.Length >= 2)
			{
				NativeSortExtension.Sort<LoadingData>(m_LoadingData);
			}
			if (m_UnloadingData.Length >= 2)
			{
				NativeSortExtension.Sort<LoadingData>(m_UnloadingData);
			}
		}
	}

	public const string kDisableMeshLoadingKey = "bh.devtools.disableMeshLoadingKey";

	public const string kForceMeshUnloadingKey = "bh.devtools.forceMeshUnloadingKey";

	public const uint MAX_LOADING_COUNT = 30u;

	public const int MIN_BATCH_PRIORITY = -1000000;

	public const int SHAPEBUFFER_ELEMENT_SIZE = 8;

	public const uint SHAPEBUFFER_MEMORY_DEFAULT = 33554432u;

	public const uint SHAPEBUFFER_MEMORY_INCREMENT = 8388608u;

	public const ulong DEFAULT_MEMORY_BUDGET = 1610612736uL;

	public const bool DEFAULT_MEMORY_BUDGET_IS_STRICT = false;

	private GeometryAssetLoadingSystem m_GeometryLoadingSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private PrefabSystem m_PrefabSystem;

	private Mesh m_DefaultObjectMesh;

	private Mesh m_DefaultBaseMesh;

	private Mesh m_DefaultLaneMesh;

	private Mesh m_ZoneBlockMesh;

	private Mesh m_ZoneLodMesh;

	private Mesh m_DefaultEdgeMesh;

	private Mesh m_DefaultNodeMesh;

	private Mesh m_DefaultRoundaboutMesh;

	private List<Mesh> m_GeneratedMeshes;

	private List<int> m_FreeMeshIndices;

	private HashSet<GeometryAsset> m_UnloadGeometryAssets;

	private HashSet<GeometryAsset> m_LoadingGeometries;

	private Dictionary<Entity, CacheInfo> m_CachingMeshes;

	private Dictionary<Entity, MeshInfo> m_MeshInfos;

	private NativeList<int> m_BatchPriority;

	private NativeList<MeshLoadingState> m_LoadingState;

	private NativeList<LoadingData> m_LoadingData;

	private NativeList<LoadingData> m_UnloadingData;

	private NativeList<Entity> m_GenerateMeshEntities;

	private NativeHeapAllocator m_ShapeAllocator;

	private JobHandle m_PriorityDeps;

	private JobHandle m_StateDeps;

	private JobHandle m_GenerateMeshDeps;

	private GraphicsBuffer m_ShapeBuffer;

	private MeshDataArray m_GenerateMeshDataArray;

	private int m_ShapeCount;

	private int m_PriorityLimit;

	private bool m_AddMeshes;

	public ulong memoryBudget { get; set; }

	public bool strictMemoryBudget { get; set; }

	public bool enableMeshLoading { get; set; }

	public bool forceMeshUnloading { get; set; }

	public ulong totalSizeInMemory { get; private set; }

	public int loadedMeshCount => m_MeshInfos.Count;

	public int loadingRemaining { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GeometryLoadingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GeometryAssetLoadingSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_GeneratedMeshes = new List<Mesh>();
		m_FreeMeshIndices = new List<int>();
		m_UnloadGeometryAssets = new HashSet<GeometryAsset>();
		m_LoadingGeometries = new HashSet<GeometryAsset>();
		m_CachingMeshes = new Dictionary<Entity, CacheInfo>();
		m_MeshInfos = new Dictionary<Entity, MeshInfo>();
		m_BatchPriority = new NativeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_LoadingState = new NativeList<MeshLoadingState>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_LoadingData = new NativeList<LoadingData>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_UnloadingData = new NativeList<LoadingData>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_ShapeAllocator = new NativeHeapAllocator(4194304u, 1u, (Allocator)4);
		((NativeHeapAllocator)(ref m_ShapeAllocator)).Allocate(1u, 1u);
		ResizeShapeBuffer();
		memoryBudget = 1610612736uL;
		strictMemoryBudget = false;
		enableMeshLoading = true;
		forceMeshUnloading = false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if ((Object)(object)m_DefaultObjectMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultObjectMesh);
		}
		if ((Object)(object)m_DefaultBaseMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultBaseMesh);
		}
		if ((Object)(object)m_DefaultLaneMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultLaneMesh);
		}
		if ((Object)(object)m_ZoneBlockMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_ZoneBlockMesh);
		}
		if ((Object)(object)m_ZoneLodMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_ZoneLodMesh);
		}
		if ((Object)(object)m_DefaultEdgeMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultEdgeMesh);
		}
		if ((Object)(object)m_DefaultNodeMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultNodeMesh);
		}
		if ((Object)(object)m_DefaultRoundaboutMesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_DefaultRoundaboutMesh);
		}
		for (int i = 0; i < m_GeneratedMeshes.Count; i++)
		{
			Object.Destroy((Object)(object)m_GeneratedMeshes[i]);
		}
		m_GeneratedMeshes.Clear();
		foreach (GeometryAsset item in m_LoadingGeometries)
		{
			item.UnloadPartial(false);
		}
		m_LoadingGeometries.Clear();
		UnloadMeshAndGeometryAssets();
		((JobHandle)(ref m_PriorityDeps)).Complete();
		((JobHandle)(ref m_StateDeps)).Complete();
		foreach (KeyValuePair<Entity, CacheInfo> item2 in m_CachingMeshes)
		{
			CacheInfo value = item2.Value;
			((JobHandle)(ref value.m_Dependency)).Complete();
			value = item2.Value;
			((EntityCommandBuffer)(ref value.m_CommandBuffer)).Dispose();
			if ((AssetData)(object)item2.Value.m_GeometryAsset != (IAssetData)null)
			{
				item2.Value.m_GeometryAsset.UnloadPartial(false);
			}
		}
		foreach (KeyValuePair<Entity, MeshInfo> item3 in m_MeshInfos)
		{
			if ((Object)(object)item3.Value.m_Prefab != (Object)null)
			{
				for (int j = 0; j < item3.Value.m_BatchCount; j++)
				{
					item3.Value.m_Prefab.ReleaseMeshes();
				}
			}
		}
		if (m_ShapeBuffer != null)
		{
			m_ShapeBuffer.Release();
			m_ShapeBuffer = null;
		}
		if (m_GenerateMeshEntities.IsCreated)
		{
			((JobHandle)(ref m_GenerateMeshDeps)).Complete();
			m_GenerateMeshEntities.Dispose();
			((MeshDataArray)(ref m_GenerateMeshDataArray)).Dispose();
		}
		m_BatchPriority.Dispose();
		m_LoadingState.Dispose();
		m_LoadingData.Dispose();
		m_UnloadingData.Dispose();
		((NativeHeapAllocator)(ref m_ShapeAllocator)).Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		ulong loadingMemorySize = 0uL;
		ulong neededMemorySize = 0uL;
		LoadMeshes(ref loadingMemorySize, ref neededMemorySize);
		UnloadMeshes(loadingMemorySize, neededMemorySize);
		GenerateMeshes();
	}

	public void ReplaceMesh(Entity oldMesh, Entity newMesh)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (m_GenerateMeshEntities.IsCreated)
		{
			for (int i = 0; i < m_GenerateMeshEntities.Length; i++)
			{
				if (m_GenerateMeshEntities[i] == oldMesh)
				{
					m_GenerateMeshEntities[i] = newMesh;
					break;
				}
			}
		}
		CompleteCaching(oldMesh);
		TryCopyBuffer<MeshVertex>(oldMesh, newMesh);
		TryCopyBuffer<MeshIndex>(oldMesh, newMesh);
		TryCopyBuffer<MeshNode>(oldMesh, newMesh);
		TryCopyBuffer<MeshNormal>(oldMesh, newMesh);
		if (m_MeshInfos.TryGetValue(oldMesh, out var value))
		{
			value.m_Prefab = m_PrefabSystem.GetPrefab<RenderPrefab>(newMesh);
			m_MeshInfos.Add(newMesh, value);
			m_MeshInfos.Remove(oldMesh);
		}
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		int batchCount = managedBatches.BatchCount;
		for (int j = 0; j < batchCount; j++)
		{
			((CustomBatch)(object)managedBatches.GetBatch(j))?.ReplaceMesh(oldMesh, newMesh);
		}
	}

	private void TryCopyBuffer<T>(Entity source, Entity target) where T : unmanaged, IBufferElementData
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasBuffer<T>(source))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<T> val = ((EntityManager)(ref entityManager)).AddBuffer<T>(target);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			val.CopyFrom(((EntityManager)(ref entityManager)).GetBuffer<T>(source, false));
		}
	}

	public Mesh GetDefaultMesh(MeshType type, BatchFlags flags, GeneratedType generatedType)
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		switch (type)
		{
		case MeshType.Object:
			if (generatedType == GeneratedType.ObjectBase)
			{
				if ((Object)(object)m_DefaultBaseMesh == (Object)null)
				{
					m_DefaultBaseMesh = ObjectMeshHelpers.CreateDefaultBaseMesh();
				}
				return m_DefaultBaseMesh;
			}
			if ((Object)(object)m_DefaultObjectMesh == (Object)null)
			{
				m_DefaultObjectMesh = ObjectMeshHelpers.CreateDefaultMesh();
			}
			return m_DefaultObjectMesh;
		case MeshType.Net:
			if ((flags & BatchFlags.Roundabout) != 0)
			{
				if ((Object)(object)m_DefaultRoundaboutMesh == (Object)null)
				{
					m_DefaultRoundaboutMesh = NetMeshHelpers.CreateDefaultRoundaboutMesh();
				}
				return m_DefaultRoundaboutMesh;
			}
			if ((flags & BatchFlags.Node) != 0)
			{
				if ((Object)(object)m_DefaultNodeMesh == (Object)null)
				{
					m_DefaultNodeMesh = NetMeshHelpers.CreateDefaultNodeMesh();
				}
				return m_DefaultNodeMesh;
			}
			if ((Object)(object)m_DefaultEdgeMesh == (Object)null)
			{
				m_DefaultEdgeMesh = NetMeshHelpers.CreateDefaultEdgeMesh();
			}
			return m_DefaultEdgeMesh;
		case MeshType.Lane:
			if ((Object)(object)m_DefaultLaneMesh == (Object)null)
			{
				m_DefaultLaneMesh = NetMeshHelpers.CreateDefaultLaneMesh();
			}
			return m_DefaultLaneMesh;
		case MeshType.Zone:
			if ((flags & BatchFlags.Lod) != 0)
			{
				if ((Object)(object)m_ZoneLodMesh == (Object)null)
				{
					m_ZoneLodMesh = ZoneMeshHelpers.CreateMesh(new int2(5, 3), int2.op_Implicit(2));
				}
				return m_ZoneLodMesh;
			}
			if ((Object)(object)m_ZoneBlockMesh == (Object)null)
			{
				m_ZoneBlockMesh = ZoneMeshHelpers.CreateMesh(new int2(10, 6), int2.op_Implicit(1));
			}
			return m_ZoneBlockMesh;
		default:
			return null;
		}
	}

	public NativeList<int> GetBatchPriority(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_PriorityDeps;
		return m_BatchPriority;
	}

	public NativeList<MeshLoadingState> GetLoadingState(out JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		dependencies = m_StateDeps;
		return m_LoadingState;
	}

	public void AddBatchPriorityWriter(JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_PriorityDeps = dependencies;
	}

	public void AddLoadingStateReader(JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_StateDeps = dependencies;
	}

	public void UpdateMeshes()
	{
		AddMeshes();
		UpdateMeshesForAddedInstances();
		UnloadMeshAndGeometryAssets();
	}

	public void CompleteMeshes()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!m_GenerateMeshEntities.IsCreated)
		{
			return;
		}
		Mesh[] array = (Mesh[])(object)new Mesh[m_GenerateMeshEntities.Length];
		MeshData meshData = default(MeshData);
		NetCompositionMeshData netCompositionMeshData = default(NetCompositionMeshData);
		for (int i = 0; i < m_GenerateMeshEntities.Length; i++)
		{
			Entity val = m_GenerateMeshEntities[i];
			if (m_MeshInfos.TryGetValue(val, out var value))
			{
				Mesh val2 = m_GeneratedMeshes[value.m_GeneratedIndex];
				Bounds bounds = default(Bounds);
				if (EntitiesExtensions.TryGetComponent<MeshData>(((ComponentSystemBase)this).EntityManager, val, ref meshData))
				{
					((Bounds)(ref bounds)).SetMinMax(float3.op_Implicit(meshData.m_Bounds.min), float3.op_Implicit(meshData.m_Bounds.max));
				}
				else if (EntitiesExtensions.TryGetComponent<NetCompositionMeshData>(((ComponentSystemBase)this).EntityManager, val, ref netCompositionMeshData))
				{
					((Bounds)(ref bounds)).SetMinMax(float3.op_Implicit(new float3(netCompositionMeshData.m_Width * -0.5f, netCompositionMeshData.m_HeightRange.min, netCompositionMeshData.m_Width * -0.5f) - 500f), float3.op_Implicit(new float3(netCompositionMeshData.m_Width * 0.5f, netCompositionMeshData.m_HeightRange.max, netCompositionMeshData.m_Width * 0.5f) + 500f));
				}
				val2.bounds = bounds;
				array[i] = val2;
			}
			else
			{
				array[i] = new Mesh();
			}
		}
		((JobHandle)(ref m_GenerateMeshDeps)).Complete();
		Mesh.ApplyAndDisposeWritableMeshData(m_GenerateMeshDataArray, array, (MeshUpdateFlags)9);
		for (int j = 0; j < m_GenerateMeshEntities.Length; j++)
		{
			if (m_MeshInfos.ContainsKey(m_GenerateMeshEntities[j]))
			{
				array[j].UploadMeshData(true);
			}
			else
			{
				Object.Destroy((Object)(object)array[j]);
			}
		}
		m_GenerateMeshEntities.Dispose();
	}

	public void UpdateBatchPriorities()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (enableMeshLoading)
		{
			m_StateDeps = (m_PriorityDeps = IJobExtensions.Schedule<LoadingPriorityJob>(new LoadingPriorityJob
			{
				m_PriorityLimit = m_PriorityLimit,
				m_BatchPriority = m_BatchPriority,
				m_LoadingState = m_LoadingState,
				m_LoadingData = m_LoadingData,
				m_UnloadingData = m_UnloadingData
			}, JobHandle.CombineDependencies(m_PriorityDeps, m_StateDeps)));
		}
	}

	public void CompleteCaching()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		List<Entity> list = null;
		foreach (KeyValuePair<Entity, CacheInfo> item in m_CachingMeshes)
		{
			CacheInfo value = item.Value;
			if (((JobHandle)(ref value.m_Dependency)).IsCompleted)
			{
				CompleteCaching(item.Key, item.Value);
				if (list == null)
				{
					list = new List<Entity>();
				}
				list.Add(item.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (Entity item2 in list)
		{
			m_CachingMeshes.Remove(item2);
		}
	}

	private void CompleteCaching(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (m_CachingMeshes.TryGetValue(entity, out var value))
		{
			CompleteCaching(entity, value);
			m_CachingMeshes.Remove(entity);
		}
	}

	private void CompleteCaching(Entity entity, CacheInfo cacheInfo)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref cacheInfo.m_Dependency)).Complete();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(entity))
		{
			((EntityCommandBuffer)(ref cacheInfo.m_CommandBuffer)).Playback(((ComponentSystemBase)this).EntityManager);
		}
		((EntityCommandBuffer)(ref cacheInfo.m_CommandBuffer)).Dispose();
		if ((AssetData)(object)cacheInfo.m_GeometryAsset != (IAssetData)null)
		{
			m_UnloadGeometryAssets.Add(cacheInfo.m_GeometryAsset);
		}
	}

	private void AddCaching(Entity entity, CacheInfo cacheInfo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		CompleteCaching(entity);
		m_CachingMeshes.Add(entity, cacheInfo);
	}

	public void AddBatch(CustomBatch batch, int batchIndex)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_PriorityDeps)).Complete();
		while (m_BatchPriority.Length <= batchIndex)
		{
			ref NativeList<int> reference = ref m_BatchPriority;
			int num = -1000000;
			reference.Add(ref num);
		}
		m_BatchPriority[batchIndex] = -1000000;
		((JobHandle)(ref m_StateDeps)).Complete();
		while (m_LoadingState.Length <= batchIndex)
		{
			ref NativeList<MeshLoadingState> reference2 = ref m_LoadingState;
			MeshLoadingState meshLoadingState = MeshLoadingState.None;
			reference2.Add(ref meshLoadingState);
		}
		m_LoadingState[batchIndex] = MeshLoadingState.None;
		EntityManager entityManager;
		if ((batch.sourceType & (MeshType.Net | MeshType.Zone)) == 0)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if ((((EntityManager)(ref entityManager)).GetComponentData<MeshData>(batch.sourceMeshEntity).m_State & MeshFlags.Default) != 0)
			{
				m_LoadingState[batchIndex] = MeshLoadingState.Default;
			}
		}
		else if ((batch.sourceType & MeshType.Net) != 0)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if ((((EntityManager)(ref entityManager)).GetComponentData<NetCompositionMeshData>(batch.sourceMeshEntity).m_State & MeshFlags.Default) != 0)
			{
				m_LoadingState[batchIndex] = MeshLoadingState.Default;
			}
		}
	}

	public void RemoveBatch(CustomBatch batch, int batchIndex)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_StateDeps)).Complete();
		MeshLoadingState meshLoadingState = m_LoadingState[batchIndex];
		if ((meshLoadingState == MeshLoadingState.Copying || meshLoadingState == MeshLoadingState.Complete || meshLoadingState == MeshLoadingState.Obsolete) && m_MeshInfos.TryGetValue(batch.sharedMeshEntity, out var value))
		{
			if (value.m_BatchCount > 1)
			{
				value.m_BatchCount--;
				m_MeshInfos[batch.sharedMeshEntity] = value;
			}
			else
			{
				if (value.m_GeneratedIndex >= 0)
				{
					Object.Destroy((Object)(object)m_GeneratedMeshes[value.m_GeneratedIndex]);
					m_GeneratedMeshes[value.m_GeneratedIndex] = null;
					if (value.m_GeneratedIndex == m_GeneratedMeshes.Count - 1)
					{
						m_GeneratedMeshes.RemoveAt(value.m_GeneratedIndex);
					}
					else
					{
						m_FreeMeshIndices.Add(value.m_GeneratedIndex);
					}
				}
				RemoveShapeData(value.m_ShapeAllocations);
				UncacheMeshData(batch.sharedMeshEntity, batch.sourceType);
				totalSizeInMemory -= value.m_SizeInMemory;
				m_MeshInfos.Remove(batch.sharedMeshEntity);
			}
			if (batch.generatedType == GeneratedType.None && (Object)(object)value.m_Prefab != (Object)null)
			{
				value.m_Prefab.ReleaseMeshes();
			}
		}
		if (meshLoadingState == MeshLoadingState.Pending || meshLoadingState == MeshLoadingState.Loading || meshLoadingState == MeshLoadingState.Copying)
		{
			for (int i = 0; i < m_LoadingData.Length; i++)
			{
				if (m_LoadingData.ElementAt(i).m_BatchIndex == batchIndex)
				{
					m_LoadingData.RemoveAt(i);
					break;
				}
			}
		}
		if (meshLoadingState == MeshLoadingState.Obsolete)
		{
			for (int j = 0; j < m_UnloadingData.Length; j++)
			{
				if (m_UnloadingData.ElementAt(j).m_BatchIndex == batchIndex)
				{
					m_UnloadingData.RemoveAt(j);
					break;
				}
			}
		}
		if (batchIndex == m_LoadingState.Length - 1)
		{
			m_LoadingState.RemoveAt(batchIndex);
		}
		else
		{
			m_LoadingState[batchIndex] = MeshLoadingState.None;
		}
		((JobHandle)(ref m_PriorityDeps)).Complete();
		if (batchIndex == m_BatchPriority.Length - 1)
		{
			m_BatchPriority.RemoveAt(batchIndex);
		}
		else
		{
			m_BatchPriority[batchIndex] = -1000000;
		}
	}

	private void LoadMeshes(ref ulong loadingMemorySize, ref ulong neededMemorySize)
	{
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Expected O, but got Unknown
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		m_PriorityLimit = 0;
		((JobHandle)(ref m_StateDeps)).Complete();
		loadingRemaining = m_LoadingData.Length;
		if (loadingRemaining == 0)
		{
			return;
		}
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		int num = 0;
		foreach (GeometryAsset item in m_LoadingGeometries)
		{
			if (!item.loading.m_AsyncLoadingDone)
			{
				num++;
			}
		}
		DynamicBuffer<NetCompositionPiece> val = default(DynamicBuffer<NetCompositionPiece>);
		for (int i = 0; i < m_LoadingData.Length; i++)
		{
			ref LoadingData reference = ref m_LoadingData.ElementAt(i);
			ref MeshLoadingState reference2 = ref m_LoadingState.ElementAt(reference.m_BatchIndex);
			if (reference2 != MeshLoadingState.Pending && reference2 != MeshLoadingState.Loading)
			{
				continue;
			}
			CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(reference.m_BatchIndex);
			if (m_CachingMeshes.ContainsKey(customBatch.sharedMeshEntity))
			{
				continue;
			}
			if (m_MeshInfos.TryGetValue(customBatch.sharedMeshEntity, out var value))
			{
				value.m_BatchCount++;
				m_MeshInfos[customBatch.sharedMeshEntity] = value;
				reference2 = MeshLoadingState.Copying;
				m_AddMeshes = true;
				if ((customBatch.sourceFlags & BatchFlags.BlendWeights) != 0)
				{
					SetShapeParameters(customBatch, value.m_ShapeAllocations);
				}
				continue;
			}
			value.m_Prefab = null;
			value.m_ShapeAllocations = null;
			value.m_SizeInMemory = 0uL;
			value.m_GeneratedIndex = -1;
			value.m_BatchCount = 1;
			bool flag = false;
			bool flag2 = false;
			string name = null;
			EntityManager entityManager;
			RenderPrefab prefab2;
			if (EntitiesExtensions.TryGetBuffer<NetCompositionPiece>(((ComponentSystemBase)this).EntityManager, customBatch.sharedMeshEntity, true, ref val))
			{
				flag2 = true;
				if (reference2 == MeshLoadingState.Pending)
				{
					if (m_PriorityLimit > reference.m_Priority)
					{
						loadingRemaining--;
						continue;
					}
					ulong num2 = 0uL;
					for (int j = 0; j < val.Length; j++)
					{
						NetCompositionPiece netCompositionPiece = val[j];
						MeshInfo value2 = default(MeshInfo);
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<MeshVertex>(netCompositionPiece.m_Piece))
						{
							if (m_MeshInfos.TryGetValue(netCompositionPiece.m_Piece, out value2))
							{
								num2 += value2.m_SizeInMemory;
							}
							continue;
						}
						if (m_MeshInfos.TryGetValue(netCompositionPiece.m_Piece, out value2))
						{
							num2 += value2.m_SizeInMemory;
							continue;
						}
						RenderPrefab prefab = m_PrefabSystem.GetPrefab<RenderPrefab>(netCompositionPiece.m_Piece);
						ulong num3 = EstimateSizeInMemory(prefab);
						num2 += num3;
						if (!m_CachingMeshes.ContainsKey(netCompositionPiece.m_Piece))
						{
							num2 += num3;
						}
					}
					if (strictMemoryBudget && totalSizeInMemory + loadingMemorySize + num2 > memoryBudget)
					{
						neededMemorySize += num2;
						m_PriorityLimit = reference.m_Priority;
						loadingRemaining--;
						continue;
					}
					reference2 = MeshLoadingState.Loading;
				}
				for (int k = 0; k < val.Length; k++)
				{
					NetCompositionPiece netCompositionPiece2 = val[k];
					MeshInfo value3 = default(MeshInfo);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<MeshVertex>(netCompositionPiece2.m_Piece))
					{
						if (m_MeshInfos.TryGetValue(netCompositionPiece2.m_Piece, out value3))
						{
							value.m_SizeInMemory += value3.m_SizeInMemory;
						}
						continue;
					}
					flag = true;
					if (m_MeshInfos.TryGetValue(netCompositionPiece2.m_Piece, out value3))
					{
						value.m_SizeInMemory += value3.m_SizeInMemory;
						continue;
					}
					RenderPrefab renderPrefab = (value3.m_Prefab = m_PrefabSystem.GetPrefab<RenderPrefab>(netCompositionPiece2.m_Piece));
					value3.m_SizeInMemory = EstimateSizeInMemory(renderPrefab);
					value3.m_GeneratedIndex = -1;
					value3.m_BatchCount = 0;
					value.m_SizeInMemory += value3.m_SizeInMemory;
					if (m_CachingMeshes.ContainsKey(netCompositionPiece2.m_Piece))
					{
						continue;
					}
					uint num4 = 23u;
					GeometryAsset geometryAsset = renderPrefab.geometryAsset;
					if ((AssetData)(object)geometryAsset != (IAssetData)null)
					{
						if (!m_LoadingGeometries.Contains(geometryAsset))
						{
							if ((long)num < 30L)
							{
								m_LoadingGeometries.Add(geometryAsset);
								geometryAsset.RequestDataAsync(m_GeometryLoadingSystem, num4);
								num++;
							}
							loadingMemorySize += value3.m_SizeInMemory;
							continue;
						}
						if (!geometryAsset.loading.m_AsyncLoadingDone)
						{
							loadingMemorySize += value3.m_SizeInMemory;
							continue;
						}
						try
						{
							CacheMeshData(renderPrefab, geometryAsset, netCompositionPiece2.m_Piece, customBatch.sourceType);
						}
						catch (Exception ex)
						{
							Debug.LogError((object)("Error when accessing mesh data (" + ((Object)renderPrefab).name + "):\n" + ex.Message), (Object)(object)renderPrefab);
						}
						value.m_SizeInMemory -= value3.m_SizeInMemory;
						value3.m_SizeInMemory = GetSizeInMemory(geometryAsset);
						value.m_SizeInMemory += value3.m_SizeInMemory;
						m_LoadingGeometries.Remove(geometryAsset);
					}
					if (!m_CachingMeshes.ContainsKey(netCompositionPiece2.m_Piece) && (AssetData)(object)geometryAsset != (IAssetData)null)
					{
						m_UnloadGeometryAssets.Add(geometryAsset);
					}
					totalSizeInMemory += value3.m_SizeInMemory;
					m_MeshInfos.Add(netCompositionPiece2.m_Piece, value3);
				}
				if (flag)
				{
					loadingMemorySize += value.m_SizeInMemory;
					continue;
				}
				for (int l = 0; l < val.Length; l++)
				{
					NetCompositionPiece netCompositionPiece3 = val[l];
					if (m_MeshInfos.TryGetValue(netCompositionPiece3.m_Piece, out var value4))
					{
						value4.m_BatchCount++;
						m_MeshInfos[netCompositionPiece3.m_Piece] = value4;
					}
				}
				if (flag2)
				{
					name = $"Net composition {customBatch.sharedMeshEntity.Index}";
				}
			}
			else if (m_PrefabSystem.TryGetPrefab<RenderPrefab>(customBatch.sharedMeshEntity, out prefab2) && (Object)(object)prefab2 != (Object)null)
			{
				value.m_Prefab = prefab2;
				value.m_SizeInMemory = EstimateSizeInMemory(prefab2);
				GeometryAsset geometryAsset2 = prefab2.geometryAsset;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				MeshData componentData = ((EntityManager)(ref entityManager)).GetComponentData<MeshData>(customBatch.sharedMeshEntity);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				flag = !((EntityManager)(ref entityManager)).HasComponent<MeshVertex>(customBatch.sharedMeshEntity);
				flag2 = (componentData.m_State & MeshFlags.Base) != 0;
				if (flag)
				{
					if ((AssetData)(object)geometryAsset2 != (IAssetData)null)
					{
						if (!m_LoadingGeometries.Contains(geometryAsset2))
						{
							if (m_PriorityLimit > reference.m_Priority)
							{
								loadingRemaining--;
							}
							else if (strictMemoryBudget && totalSizeInMemory + loadingMemorySize + value.m_SizeInMemory > memoryBudget)
							{
								neededMemorySize += value.m_SizeInMemory;
								m_PriorityLimit = reference.m_Priority;
								loadingRemaining--;
							}
							else if ((long)num < 30L)
							{
								loadingMemorySize += value.m_SizeInMemory;
								m_LoadingGeometries.Add(geometryAsset2);
								geometryAsset2.RequestDataAsync(m_GeometryLoadingSystem, 16383u);
								reference2 = MeshLoadingState.Loading;
								num++;
							}
							continue;
						}
						if (!geometryAsset2.loading.m_AsyncLoadingDone)
						{
							loadingMemorySize += value.m_SizeInMemory;
							continue;
						}
						try
						{
							CacheMeshData(prefab2, geometryAsset2, customBatch.sharedMeshEntity, customBatch.sourceType);
						}
						catch (Exception ex2)
						{
							Debug.LogError((object)("Error when accessing mesh data (" + ((Object)value.m_Prefab).name + "):\n" + ex2.Message), (Object)(object)value.m_Prefab);
						}
						value.m_SizeInMemory = GetSizeInMemory(geometryAsset2);
						m_LoadingGeometries.Remove(geometryAsset2);
					}
				}
				else if ((AssetData)(object)geometryAsset2 != (IAssetData)null)
				{
					if (geometryAsset2.data.attrData.IsCreated)
					{
						value.m_SizeInMemory = GetSizeInMemory(geometryAsset2);
					}
					else
					{
						Debug.Log((object)("Geometry asset not loaded: " + ((AssetData)geometryAsset2).name));
					}
				}
				if (flag2 && flag)
				{
					loadingMemorySize += value.m_SizeInMemory;
					continue;
				}
				if (!m_CachingMeshes.ContainsKey(customBatch.sharedMeshEntity) && (AssetData)(object)geometryAsset2 != (IAssetData)null)
				{
					m_UnloadGeometryAssets.Add(geometryAsset2);
				}
				if (flag2 && (AssetData)(object)geometryAsset2 != (IAssetData)null)
				{
					name = string.Concat("Generated base (" + ((AssetData)geometryAsset2).name, ")");
				}
			}
			if (flag2)
			{
				Mesh val2 = new Mesh();
				((Object)val2).name = name;
				if (m_FreeMeshIndices.Count != 0)
				{
					value.m_GeneratedIndex = m_FreeMeshIndices[m_FreeMeshIndices.Count - 1];
					m_FreeMeshIndices.RemoveAt(m_FreeMeshIndices.Count - 1);
					m_GeneratedMeshes[value.m_GeneratedIndex] = val2;
				}
				else
				{
					value.m_GeneratedIndex = m_GeneratedMeshes.Count;
					m_GeneratedMeshes.Add(val2);
				}
				if (!m_GenerateMeshEntities.IsCreated)
				{
					m_GenerateMeshEntities = new NativeList<Entity>(30, AllocatorHandle.op_Implicit((Allocator)3));
				}
				ref NativeList<Entity> reference3 = ref m_GenerateMeshEntities;
				Entity sharedMeshEntity = customBatch.sharedMeshEntity;
				reference3.Add(ref sharedMeshEntity);
			}
			if ((customBatch.sourceFlags & BatchFlags.BlendWeights) != 0)
			{
				value.m_ShapeAllocations = AddShapeData(value.m_Prefab);
				SetShapeParameters(customBatch, value.m_ShapeAllocations);
			}
			totalSizeInMemory += value.m_SizeInMemory;
			m_MeshInfos.Add(customBatch.sharedMeshEntity, value);
			reference2 = MeshLoadingState.Copying;
			m_AddMeshes = true;
		}
	}

	private ulong EstimateSizeInMemory(RenderPrefab meshPrefab)
	{
		int indexCount = meshPrefab.indexCount;
		int vertexCount = meshPrefab.vertexCount;
		int num = ((vertexCount > 65536) ? 4 : 2);
		int num2 = 32;
		return (ulong)((long)indexCount * (long)num + (long)vertexCount * (long)num2);
	}

	private ulong GetSizeInMemory(GeometryAsset geometryAsset)
	{
		return (ulong)geometryAsset.data.attrData.Length + (ulong)geometryAsset.data.indexData.Length;
	}

	private void AddMeshes()
	{
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!m_AddMeshes)
		{
			return;
		}
		m_AddMeshes = false;
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		for (int i = 0; i < m_LoadingData.Length; i++)
		{
			ref LoadingData reference = ref m_LoadingData.ElementAt(i);
			ref MeshLoadingState reference2 = ref m_LoadingState.ElementAt(reference.m_BatchIndex);
			if (reference2 != MeshLoadingState.Copying)
			{
				continue;
			}
			CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(reference.m_BatchIndex);
			try
			{
				if (m_MeshInfos.TryGetValue(customBatch.sharedMeshEntity, out var value))
				{
					if (customBatch.generatedType != GeneratedType.None)
					{
						if (value.m_GeneratedIndex >= 0)
						{
							managedBatches.SetMesh<CullingData, GroupData, BatchData, InstanceData>(reference.m_BatchIndex, m_GeneratedMeshes[value.m_GeneratedIndex], customBatch.sourceSubMeshIndex, nativeBatchGroups);
						}
					}
					else if ((Object)(object)value.m_Prefab != (Object)null)
					{
						int subMeshIndex;
						Mesh val = value.m_Prefab.ObtainMesh(customBatch.sourceSubMeshIndex, out subMeshIndex);
						managedBatches.SetMesh<CullingData, GroupData, BatchData, InstanceData>(reference.m_BatchIndex, val, subMeshIndex, nativeBatchGroups);
					}
				}
				if ((Object)(object)customBatch.defaultMaterial != (Object)(object)customBatch.loadedMaterial)
				{
					managedBatches.SetMaterial<CullingData, GroupData, BatchData, InstanceData>(reference.m_BatchIndex, customBatch.loadedMaterial, nativeBatchGroups);
				}
			}
			catch (Exception ex)
			{
				if (m_MeshInfos.TryGetValue(customBatch.sharedMeshEntity, out var value2) && (Object)(object)value2.m_Prefab != (Object)null)
				{
					COSystemBase.baseLog.ErrorFormat((Object)(object)value2.m_Prefab, ex, "Error when setting mesh for {0}", (object)((Object)value2.m_Prefab).name);
				}
				else
				{
					COSystemBase.baseLog.ErrorFormat(ex, "Error when setting mesh for {0}", (object)customBatch.sourceMeshEntity);
				}
			}
			reference2 = MeshLoadingState.Complete;
			m_LoadingData.RemoveAtSwapBack(i--);
		}
	}

	private void UpdateMeshesForAddedInstances()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_StateDeps)).Complete();
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies2);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		AddedInstanceEnumerator addedInstances = nativeBatchInstances.GetAddedInstances();
		int num = default(int);
		while (((AddedInstanceEnumerator)(ref addedInstances)).GetNextUpdatedGroup(ref num))
		{
			NativeBatchAccessor<BatchData> batchAccessor = nativeBatchGroups.GetBatchAccessor(num);
			for (int i = 0; i < batchAccessor.Length; i++)
			{
				int managedBatchIndex = batchAccessor.GetManagedBatchIndex(i);
				if (managedBatchIndex < 0)
				{
					continue;
				}
				ref MeshLoadingState reference = ref m_LoadingState.ElementAt(managedBatchIndex);
				if (reference != MeshLoadingState.None)
				{
					continue;
				}
				CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(managedBatchIndex);
				if (!m_MeshInfos.TryGetValue(customBatch.sharedMeshEntity, out var value))
				{
					continue;
				}
				value.m_BatchCount++;
				m_MeshInfos[customBatch.sharedMeshEntity] = value;
				if ((customBatch.sourceFlags & BatchFlags.BlendWeights) != 0)
				{
					SetShapeParameters(customBatch, value.m_ShapeAllocations);
				}
				try
				{
					if (customBatch.generatedType != GeneratedType.None)
					{
						if (value.m_GeneratedIndex >= 0)
						{
							managedBatches.SetMesh<CullingData, GroupData, BatchData, InstanceData>(managedBatchIndex, m_GeneratedMeshes[value.m_GeneratedIndex], customBatch.sourceSubMeshIndex, nativeBatchGroups);
						}
					}
					else if ((Object)(object)value.m_Prefab != (Object)null)
					{
						int subMeshIndex;
						Mesh val = value.m_Prefab.ObtainMesh(customBatch.sourceSubMeshIndex, out subMeshIndex);
						managedBatches.SetMesh<CullingData, GroupData, BatchData, InstanceData>(managedBatchIndex, val, subMeshIndex, nativeBatchGroups);
					}
					if ((Object)(object)customBatch.defaultMaterial != (Object)(object)customBatch.loadedMaterial)
					{
						managedBatches.SetMaterial<CullingData, GroupData, BatchData, InstanceData>(managedBatchIndex, customBatch.loadedMaterial, nativeBatchGroups);
					}
				}
				catch (Exception ex)
				{
					if ((Object)(object)value.m_Prefab != (Object)null)
					{
						COSystemBase.baseLog.ErrorFormat((Object)(object)value.m_Prefab, ex, "Error when setting mesh for {0}", (object)((Object)value.m_Prefab).name);
					}
					else
					{
						COSystemBase.baseLog.ErrorFormat(ex, "Error when setting mesh for {0}", (object)customBatch.sourceMeshEntity);
					}
				}
				reference = MeshLoadingState.Complete;
			}
		}
		nativeBatchInstances.ClearAddedInstances();
	}

	private void UnloadMeshes(ulong loadingMemorySize, ulong neededMemorySize)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if (!forceMeshUnloading && totalSizeInMemory + loadingMemorySize + neededMemorySize <= memoryBudget)
		{
			return;
		}
		((JobHandle)(ref m_StateDeps)).Complete();
		if (m_UnloadingData.Length == 0)
		{
			return;
		}
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		int num = 0;
		bool flag = false;
		((JobHandle)(ref dependencies)).Complete();
		DynamicBuffer<NetCompositionPiece> val = default(DynamicBuffer<NetCompositionPiece>);
		for (int i = 0; i < m_UnloadingData.Length; i++)
		{
			ref LoadingData reference = ref m_UnloadingData.ElementAt(i);
			ref MeshLoadingState reference2 = ref m_LoadingState.ElementAt(reference.m_BatchIndex);
			if (reference2 != MeshLoadingState.Obsolete)
			{
				continue;
			}
			if ((!forceMeshUnloading && totalSizeInMemory + loadingMemorySize + neededMemorySize <= memoryBudget) || (!forceMeshUnloading && totalSizeInMemory + loadingMemorySize <= memoryBudget && -reference.m_Priority >= m_PriorityLimit))
			{
				break;
			}
			CustomBatch customBatch = (CustomBatch)(object)managedBatches.GetBatch(reference.m_BatchIndex);
			if (m_MeshInfos.TryGetValue(customBatch.sharedMeshEntity, out var value))
			{
				if (value.m_BatchCount > 1)
				{
					value.m_BatchCount--;
					m_MeshInfos[customBatch.sharedMeshEntity] = value;
				}
				else
				{
					if ((long)num >= 30L)
					{
						continue;
					}
					num++;
					if (EntitiesExtensions.TryGetBuffer<NetCompositionPiece>(((ComponentSystemBase)this).EntityManager, customBatch.sharedMeshEntity, true, ref val))
					{
						for (int j = 0; j < val.Length; j++)
						{
							NetCompositionPiece netCompositionPiece = val[j];
							if (m_MeshInfos.TryGetValue(netCompositionPiece.m_Piece, out var value2))
							{
								if (value2.m_BatchCount > 1)
								{
									value2.m_BatchCount--;
									m_MeshInfos[netCompositionPiece.m_Piece] = value2;
								}
								else
								{
									UncacheMeshData(netCompositionPiece.m_Piece, customBatch.sourceType);
									totalSizeInMemory -= value2.m_SizeInMemory;
									m_MeshInfos.Remove(netCompositionPiece.m_Piece);
								}
							}
						}
					}
					if (value.m_GeneratedIndex >= 0)
					{
						Object.Destroy((Object)(object)m_GeneratedMeshes[value.m_GeneratedIndex]);
						m_GeneratedMeshes[value.m_GeneratedIndex] = null;
						m_FreeMeshIndices.Add(value.m_GeneratedIndex);
					}
					RemoveShapeData(value.m_ShapeAllocations);
					UncacheMeshData(customBatch.sharedMeshEntity, customBatch.sourceType);
					totalSizeInMemory -= value.m_SizeInMemory;
					m_MeshInfos.Remove(customBatch.sharedMeshEntity);
				}
				managedBatches.SetMesh<CullingData, GroupData, BatchData, InstanceData>(reference.m_BatchIndex, GetDefaultMesh(customBatch.sourceType, customBatch.sourceFlags, customBatch.generatedType), 0, nativeBatchGroups);
				if (customBatch.generatedType == GeneratedType.None && (Object)(object)value.m_Prefab != (Object)null)
				{
					value.m_Prefab.ReleaseMeshes();
				}
			}
			reference2 = MeshLoadingState.Unloading;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		for (int k = 0; k < m_UnloadingData.Length; k++)
		{
			ref LoadingData reference3 = ref m_UnloadingData.ElementAt(k);
			ref MeshLoadingState reference4 = ref m_LoadingState.ElementAt(reference3.m_BatchIndex);
			if (reference4 == MeshLoadingState.Unloading)
			{
				CustomBatch customBatch2 = (CustomBatch)(object)managedBatches.GetBatch(reference3.m_BatchIndex);
				if ((Object)(object)customBatch2.defaultMaterial != (Object)(object)customBatch2.loadedMaterial)
				{
					managedBatches.SetMaterial<CullingData, GroupData, BatchData, InstanceData>(reference3.m_BatchIndex, customBatch2.defaultMaterial, nativeBatchGroups);
				}
				reference4 = MeshLoadingState.None;
				m_UnloadingData.RemoveAtSwapBack(k--);
			}
		}
	}

	private void UnloadMeshAndGeometryAssets()
	{
		if (m_UnloadGeometryAssets.Count == 0)
		{
			return;
		}
		foreach (GeometryAsset item in m_UnloadGeometryAssets)
		{
			item.UnloadPartial(false);
		}
		m_UnloadGeometryAssets.Clear();
	}

	private void CacheMeshData(RenderPrefab meshPrefab, GeometryAsset asset, Entity entity, MeshType type)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		switch (type)
		{
		case MeshType.Object:
		case MeshType.Lane:
		{
			int boneCount = 0;
			DynamicBuffer<ProceduralBone> val = default(DynamicBuffer<ProceduralBone>);
			if (EntitiesExtensions.TryGetBuffer<ProceduralBone>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
			{
				boneCount = val.Length;
			}
			bool cacheNormals = false;
			MeshData meshData = default(MeshData);
			if (EntitiesExtensions.TryGetComponent<MeshData>(((ComponentSystemBase)this).EntityManager, entity, ref meshData))
			{
				cacheNormals = (meshData.m_State & MeshFlags.Base) != 0;
			}
			CacheInfo cacheInfo2 = default(CacheInfo);
			cacheInfo2.m_GeometryAsset = asset;
			cacheInfo2.m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0);
			cacheInfo2.m_Dependency = ObjectMeshHelpers.CacheMeshData(meshPrefab, asset, entity, boneCount, cacheNormals, cacheInfo2.m_CommandBuffer);
			CompleteCaching(entity);
			AddCaching(entity, cacheInfo2);
			break;
		}
		case MeshType.Net:
		{
			CacheInfo cacheInfo = default(CacheInfo);
			cacheInfo.m_GeometryAsset = asset;
			cacheInfo.m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0);
			cacheInfo.m_Dependency = NetMeshHelpers.CacheMeshData(asset, entity, ((ComponentSystemBase)this).EntityManager, cacheInfo.m_CommandBuffer);
			AddCaching(entity, cacheInfo);
			break;
		}
		case MeshType.Object | MeshType.Net:
			break;
		}
	}

	private void CacheMeshData(Mesh mesh, Entity entity, MeshType type)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		switch (type)
		{
		case MeshType.Object:
		case MeshType.Lane:
		{
			bool cacheNormals = false;
			MeshData meshData = default(MeshData);
			if (EntitiesExtensions.TryGetComponent<MeshData>(((ComponentSystemBase)this).EntityManager, entity, ref meshData))
			{
				cacheNormals = (meshData.m_State & MeshFlags.Base) != 0;
			}
			CacheInfo cacheInfo2 = new CacheInfo
			{
				m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0)
			};
			ObjectMeshHelpers.CacheMeshData(mesh, entity, cacheNormals, cacheInfo2.m_CommandBuffer);
			AddCaching(entity, cacheInfo2);
			break;
		}
		case MeshType.Net:
		{
			CacheInfo cacheInfo = new CacheInfo
			{
				m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0)
			};
			NetMeshHelpers.CacheMeshData(mesh, entity, ((ComponentSystemBase)this).EntityManager, cacheInfo.m_CommandBuffer);
			AddCaching(entity, cacheInfo);
			break;
		}
		case MeshType.Object | MeshType.Net:
			break;
		}
	}

	private void UncacheMeshData(Entity mesh, MeshType type)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		switch (type)
		{
		case MeshType.Object:
		case MeshType.Lane:
		{
			CacheInfo cacheInfo2 = new CacheInfo
			{
				m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0)
			};
			ObjectMeshHelpers.UncacheMeshData(mesh, cacheInfo2.m_CommandBuffer);
			AddCaching(mesh, cacheInfo2);
			break;
		}
		case MeshType.Net:
		{
			CacheInfo cacheInfo = new CacheInfo
			{
				m_CommandBuffer = new EntityCommandBuffer((Allocator)4, (PlaybackPolicy)0)
			};
			NetMeshHelpers.UncacheMeshData(mesh, cacheInfo.m_CommandBuffer);
			AddCaching(mesh, cacheInfo);
			break;
		}
		case MeshType.Object | MeshType.Net:
			break;
		}
	}

	private void GenerateMeshes()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (m_GenerateMeshEntities.IsCreated)
		{
			m_GenerateMeshDataArray = Mesh.AllocateWritableMeshData(m_GenerateMeshEntities.Length);
			m_GenerateMeshDeps = BatchMeshHelpers.GenerateMeshes(this, m_GenerateMeshEntities, m_GenerateMeshDataArray, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = m_GenerateMeshDeps;
		}
	}

	private ShapeAllocation[] AddShapeData(RenderPrefab meshPrefab)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		GeometryAsset geometryAsset = meshPrefab.geometryAsset;
		if ((AssetData)(object)geometryAsset == (IAssetData)null)
		{
			return null;
		}
		NativeArray<byte> shapeDataBuffer = geometryAsset.shapeDataBuffer;
		if (!shapeDataBuffer.IsCreated)
		{
			return null;
		}
		NativeArray<ulong> val = shapeDataBuffer.Reinterpret<ulong>(1);
		ShapeAllocation[] array = new ShapeAllocation[meshPrefab.meshCount];
		for (int i = 0; i < array.Length; i++)
		{
			int shapeDataSize = geometryAsset.GetShapeDataSize(i);
			if (shapeDataSize != 0)
			{
				array[i].m_Stride = geometryAsset.GetVertexCount(i);
				array[i].m_PositionExtent = float3.op_Implicit(geometryAsset.GetShapePositionExtent(i));
				array[i].m_NormalExtent = float3.op_Implicit(geometryAsset.GetShapeNormalExtent(i));
				uint num = (uint)shapeDataSize / 8u;
				array[i].m_Allocation = ((NativeHeapAllocator)(ref m_ShapeAllocator)).Allocate(num, 1u);
				if (((NativeHeapBlock)(ref array[i].m_Allocation)).Empty)
				{
					uint num2 = 1048576u;
					num2 = (num2 + num - 1) / num2 * num2;
					((NativeHeapAllocator)(ref m_ShapeAllocator)).Resize(((NativeHeapAllocator)(ref m_ShapeAllocator)).Size + num2);
					array[i].m_Allocation = ((NativeHeapAllocator)(ref m_ShapeAllocator)).Allocate(num, 1u);
				}
				m_ShapeCount++;
			}
		}
		ResizeShapeBuffer();
		for (int j = 0; j < array.Length; j++)
		{
			int shapeStartOffset = geometryAsset.GetShapeStartOffset(j);
			int shapeDataSize2 = geometryAsset.GetShapeDataSize(j);
			if (shapeDataSize2 != 0)
			{
				m_ShapeBuffer.SetData<ulong>(val, shapeStartOffset / 8, (int)((NativeHeapBlock)(ref array[j].m_Allocation)).Begin, shapeDataSize2 / 8);
			}
		}
		return array;
	}

	private void RemoveShapeData(ShapeAllocation[] allocations)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (allocations == null)
		{
			return;
		}
		for (int i = 0; i < allocations.Length; i++)
		{
			ShapeAllocation shapeAllocation = allocations[i];
			if (!((NativeHeapBlock)(ref shapeAllocation.m_Allocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_ShapeAllocator)).Release(shapeAllocation.m_Allocation);
				m_ShapeCount--;
			}
		}
	}

	public void SetShapeParameters(MaterialPropertyBlock customProps, Entity sharedMeshEntity, int subMeshIndex)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ShapeAllocation allocation = default(ShapeAllocation);
		if (m_MeshInfos.TryGetValue(sharedMeshEntity, out var value) && value.m_ShapeAllocations != null && value.m_ShapeAllocations.Length > subMeshIndex)
		{
			allocation = value.m_ShapeAllocations[subMeshIndex];
		}
		SetShapeParameters(customProps, allocation);
	}

	public void GetShapeStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		allocatedSize = ((NativeHeapAllocator)(ref m_ShapeAllocator)).UsedSpace * 8;
		bufferSize = ((NativeHeapAllocator)(ref m_ShapeAllocator)).Size * 8;
		count = (uint)m_ShapeCount;
	}

	private void SetShapeParameters(CustomBatch batch, ShapeAllocation[] allocations)
	{
		ShapeAllocation allocation = default(ShapeAllocation);
		if (allocations != null && allocations.Length > batch.sourceSubMeshIndex)
		{
			allocation = allocations[batch.sourceSubMeshIndex];
		}
		SetShapeParameters(((ManagedBatch)batch).customProps, allocation);
		BatchPropertyUpdated(batch);
	}

	private void SetShapeParameters(MaterialPropertyBlock customProps, ShapeAllocation allocation)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		PropertyData propertyData = m_BatchManagerSystem.GetPropertyData(MaterialProperty.ShapeParameters1);
		PropertyData propertyData2 = m_BatchManagerSystem.GetPropertyData(MaterialProperty.ShapeParameters2);
		Vector4 zero = Vector4.zero;
		Vector4 zero2 = Vector4.zero;
		if (!((NativeHeapBlock)(ref allocation.m_Allocation)).Empty)
		{
			zero.x = allocation.m_PositionExtent.x;
			zero.y = allocation.m_PositionExtent.y;
			zero.z = allocation.m_PositionExtent.z;
			zero.w = math.asfloat(((NativeHeapBlock)(ref allocation.m_Allocation)).Begin);
			zero2.x = allocation.m_NormalExtent.x;
			zero2.y = allocation.m_NormalExtent.y;
			zero2.z = allocation.m_NormalExtent.z;
			zero2.w = math.asfloat(allocation.m_Stride);
		}
		customProps.SetVector(propertyData.m_NameID, zero);
		customProps.SetVector(propertyData2.m_NameID, zero2);
	}

	private void BatchPropertyUpdated(CustomBatch batch)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies);
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		BatchFlags batchFlags = batch.sourceFlags;
		if (!m_BatchManagerSystem.IsMotionVectorsEnabled())
		{
			batchFlags &= ~BatchFlags.MotionVectors;
		}
		if (!m_BatchManagerSystem.IsLodFadeEnabled())
		{
			batchFlags &= ~BatchFlags.LodFade;
		}
		OptionalProperties optionalProperties = new OptionalProperties(batchFlags, batch.sourceType);
		((JobHandle)(ref dependencies)).Complete();
		NativeBatchProperties batchProperties = managedBatches.GetBatchProperties(((ManagedBatch)batch).material.shader, optionalProperties);
		WriteableBatchDefaultsAccessor batchDefaultsAccessor = nativeBatchGroups.GetBatchDefaultsAccessor(((ManagedBatch)batch).groupIndex, ((ManagedBatch)batch).batchIndex);
		if ((AssetData)(object)batch.sourceSurface != (IAssetData)null)
		{
			managedBatches.SetDefaults(ManagedBatchSystem.GetTemplate(batch.sourceSurface), batch.sourceSurface.floats, batch.sourceSurface.ints, batch.sourceSurface.vectors, batch.sourceSurface.colors, ((ManagedBatch)batch).customProps, batchProperties, batchDefaultsAccessor);
		}
		else
		{
			managedBatches.SetDefaults(batch.sourceMaterial, ((ManagedBatch)batch).customProps, batchProperties, batchDefaultsAccessor);
		}
	}

	private void ResizeShapeBuffer()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		int num = ((m_ShapeBuffer != null) ? m_ShapeBuffer.count : 0);
		int size = (int)((NativeHeapAllocator)(ref m_ShapeAllocator)).Size;
		if (num != size)
		{
			GraphicsBuffer val = new GraphicsBuffer((Target)16, size, 8);
			val.name = "Shape buffer";
			Shader.SetGlobalBuffer("shapeBuffer", val);
			if (m_ShapeBuffer != null)
			{
				ulong[] array = new ulong[num];
				m_ShapeBuffer.GetData((Array)array);
				val.SetData((Array)array, 0, 0, num);
				m_ShapeBuffer.Release();
			}
			else
			{
				val.SetData((Array)new ulong[1], 0, 0, 1);
			}
			m_ShapeBuffer = val;
		}
	}

	[Preserve]
	public BatchMeshSystem()
	{
	}
}
