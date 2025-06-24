using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
using Colossal.Rendering;
using Game.Common;
using Game.Rendering;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class MeshSystem : GameSystemBase
{
	[BurstCompile]
	private struct RemoveBatchGroupsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> m_PrefabDataType;

		public BufferLookup<MeshBatch> m_MeshBatches;

		public BufferLookup<FadeBatch> m_FadeBatches;

		public BufferLookup<BatchGroup> m_BatchGroups;

		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			if (!((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabData>(ref m_PrefabDataType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				if (nativeArray2[i].m_Index < 0)
				{
					DynamicBuffer<BatchGroup> val = m_BatchGroups[nativeArray[i]];
					for (int j = 0; j < val.Length; j++)
					{
						RemoveBatchGroup(val[j].m_GroupIndex, val[j].m_MergeIndex);
					}
					val.Clear();
				}
			}
		}

		private void RemoveBatchGroup(int groupIndex, int mergeIndex)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			int num = groupIndex;
			if (mergeIndex != -1)
			{
				num = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, mergeIndex);
				m_NativeBatchGroups.RemoveMergedGroup(groupIndex, mergeIndex);
			}
			else
			{
				int mergedGroupCount = m_NativeBatchGroups.GetMergedGroupCount(groupIndex);
				if (mergedGroupCount != 0)
				{
					int mergedGroupIndex = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, 0);
					GroupData groupData = m_NativeBatchGroups.GetGroupData(mergedGroupIndex);
					DynamicBuffer<BatchGroup> val = m_BatchGroups[groupData.m_Mesh];
					for (int i = 0; i < val.Length; i++)
					{
						BatchGroup batchGroup = val[i];
						if (batchGroup.m_GroupIndex == mergedGroupIndex)
						{
							batchGroup.m_MergeIndex = -1;
							val[i] = batchGroup;
							break;
						}
					}
					for (int j = 1; j < mergedGroupCount; j++)
					{
						int mergedGroupIndex2 = m_NativeBatchGroups.GetMergedGroupIndex(groupIndex, j);
						groupData = m_NativeBatchGroups.GetGroupData(mergedGroupIndex2);
						val = m_BatchGroups[groupData.m_Mesh];
						m_NativeBatchGroups.AddMergedGroup(mergedGroupIndex, mergedGroupIndex2);
						for (int k = 0; k < val.Length; k++)
						{
							BatchGroup batchGroup2 = val[k];
							if (batchGroup2.m_GroupIndex == mergedGroupIndex2)
							{
								batchGroup2.m_MergeIndex = mergedGroupIndex;
								val[j] = batchGroup2;
								break;
							}
						}
					}
				}
			}
			int instanceCount = m_NativeBatchInstances.GetInstanceCount(groupIndex);
			DynamicBuffer<MeshBatch> val2 = default(DynamicBuffer<MeshBatch>);
			DynamicBuffer<FadeBatch> val3 = default(DynamicBuffer<FadeBatch>);
			for (int l = 0; l < instanceCount; l++)
			{
				InstanceData instanceData = m_NativeBatchInstances.GetInstanceData(groupIndex, l);
				if (!m_MeshBatches.TryGetBuffer(instanceData.m_Entity, ref val2))
				{
					continue;
				}
				for (int m = 0; m < val2.Length; m++)
				{
					MeshBatch meshBatch = val2[m];
					if (meshBatch.m_GroupIndex == groupIndex && meshBatch.m_InstanceIndex == l)
					{
						if (m_FadeBatches.TryGetBuffer(instanceData.m_Entity, ref val3))
						{
							val2.RemoveAtSwapBack(m);
							val3.RemoveAtSwapBack(m);
						}
						else
						{
							meshBatch.m_GroupIndex = -1;
							meshBatch.m_InstanceIndex = -1;
							val2[m] = meshBatch;
						}
						break;
					}
				}
			}
			m_NativeBatchInstances.RemoveInstances(groupIndex, m_NativeSubBatches);
			m_NativeBatchGroups.DestroyGroup(num, m_NativeBatchInstances, m_NativeSubBatches);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct InitializeMeshJob : IJobParallelFor
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct BoneParentComparer : IComparer<ProceduralBone>
		{
			public int Compare(ProceduralBone x, ProceduralBone y)
			{
				return math.select(math.select(x.m_SourceIndex - y.m_SourceIndex, x.m_ParentIndex - y.m_ParentIndex, x.m_ParentIndex != y.m_ParentIndex), x.m_HierarchyDepth - y.m_HierarchyDepth, x.m_HierarchyDepth != y.m_HierarchyDepth);
			}
		}

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		public BufferTypeHandle<ProceduralBone> m_ProceduralBoneType;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			ArchetypeChunk val = m_Chunks[index];
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref m_DeletedType))
			{
				return;
			}
			BufferAccessor<ProceduralBone> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ProceduralBone>(ref m_ProceduralBoneType);
			if (bufferAccessor.Length == 0)
			{
				return;
			}
			NativeList<int> val2 = default(NativeList<int>);
			val2._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<ProceduralBone> val3 = bufferAccessor[i];
				if (val3.Length == 0)
				{
					continue;
				}
				val2.ResizeUninitialized(val3.Length);
				for (int j = 0; j < val3.Length; j++)
				{
					ProceduralBone proceduralBone = val3[j];
					int sourceIndex = proceduralBone.m_SourceIndex;
					proceduralBone.m_SourceIndex = -1;
					if (sourceIndex != 0)
					{
						for (int k = 0; k < val3.Length; k++)
						{
							if (val3[k].m_ConnectionID == sourceIndex)
							{
								proceduralBone.m_SourceIndex = k;
								break;
							}
						}
					}
					if (proceduralBone.m_ParentIndex >= 0 && proceduralBone.m_HierarchyDepth == 0)
					{
						int num = j;
						int num2 = 0;
						do
						{
							proceduralBone.m_HierarchyDepth = 1;
							val3[num] = proceduralBone;
							val2[num2++] = num;
							num = proceduralBone.m_ParentIndex;
							proceduralBone = val3[num];
						}
						while (proceduralBone.m_ParentIndex >= 0 && proceduralBone.m_HierarchyDepth == 0);
						while (num2 != 0)
						{
							int hierarchyDepth = proceduralBone.m_HierarchyDepth;
							num = val2[--num2];
							proceduralBone = val3[num];
							proceduralBone.m_HierarchyDepth += hierarchyDepth;
							val3[num] = proceduralBone;
						}
					}
				}
				NativeSortExtension.Sort<ProceduralBone, BoneParentComparer>(val3.AsNativeArray(), default(BoneParentComparer));
				for (int l = 0; l < val3.Length; l++)
				{
					val2[val3[l].m_BindIndex] = l;
				}
				for (int m = 0; m < val3.Length; m++)
				{
					ProceduralBone proceduralBone2 = val3[m];
					if (proceduralBone2.m_ParentIndex >= 0)
					{
						proceduralBone2.m_ParentIndex = val2[proceduralBone2.m_ParentIndex];
						ProceduralBone proceduralBone3 = val3[proceduralBone2.m_ParentIndex];
						proceduralBone2.m_ObjectPosition = proceduralBone3.m_ObjectPosition + math.mul(proceduralBone3.m_ObjectRotation, proceduralBone2.m_Position);
						proceduralBone2.m_ObjectRotation = math.mul(proceduralBone3.m_ObjectRotation, proceduralBone2.m_Rotation);
					}
					else
					{
						proceduralBone2.m_ObjectPosition = proceduralBone2.m_Position;
						proceduralBone2.m_ObjectRotation = proceduralBone2.m_Rotation;
					}
					if (proceduralBone2.m_SourceIndex >= 0)
					{
						proceduralBone2.m_SourceIndex = val2[proceduralBone2.m_SourceIndex];
					}
					val3[m] = proceduralBone2;
				}
			}
			val2.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<MeshData> __Game_Prefabs_MeshData_RW_ComponentTypeHandle;

		public BufferTypeHandle<LodMesh> __Game_Prefabs_LodMesh_RW_BufferTypeHandle;

		public BufferTypeHandle<ProceduralBone> __Game_Prefabs_ProceduralBone_RW_BufferTypeHandle;

		public BufferTypeHandle<ProceduralLight> __Game_Prefabs_ProceduralLight_RW_BufferTypeHandle;

		public BufferTypeHandle<LightAnimation> __Game_Prefabs_LightAnimation_RW_BufferTypeHandle;

		public BufferTypeHandle<MeshMaterial> __Game_Prefabs_MeshMaterial_RW_BufferTypeHandle;

		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

		public BufferLookup<FadeBatch> __Game_Rendering_FadeBatch_RW_BufferLookup;

		public BufferLookup<BatchGroup> __Game_Prefabs_BatchGroup_RW_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_MeshData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MeshData>(false);
			__Game_Prefabs_LodMesh_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LodMesh>(false);
			__Game_Prefabs_ProceduralBone_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ProceduralBone>(false);
			__Game_Prefabs_ProceduralLight_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ProceduralLight>(false);
			__Game_Prefabs_LightAnimation_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LightAnimation>(false);
			__Game_Prefabs_MeshMaterial_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MeshMaterial>(false);
			__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
			__Game_Rendering_FadeBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<FadeBatch>(false);
			__Game_Prefabs_BatchGroup_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BatchGroup>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private EntityQuery m_PrefabQuery;

	private Dictionary<ManagedBatchSystem.MaterialKey, int> m_MaterialIndex;

	private ManagedBatchSystem.MaterialKey m_CachedMaterialKey;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<MeshData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_MaterialIndex = new Dictionary<ManagedBatchSystem.MaterialKey, int>();
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	public int GetMaterialIndex(SurfaceAsset surface)
	{
		ManagedBatchSystem.MaterialKey materialKey;
		if (m_CachedMaterialKey != null)
		{
			materialKey = m_CachedMaterialKey;
			m_CachedMaterialKey = null;
		}
		else
		{
			materialKey = new ManagedBatchSystem.MaterialKey();
		}
		surface.LoadProperties(true);
		materialKey.Initialize(surface);
		if (m_MaterialIndex.TryGetValue(materialKey, out var value))
		{
			materialKey.Clear();
			m_CachedMaterialKey = materialKey;
		}
		else
		{
			value = m_MaterialIndex.Count;
			m_MaterialIndex.Add(materialKey, value);
		}
		((AssetData)surface).Unload(false);
		return value;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0907: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d09: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> chunks = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<MeshData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<MeshData>(ref __TypeHandle.__Game_Prefabs_MeshData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<LodMesh> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<LodMesh>(ref __TypeHandle.__Game_Prefabs_LodMesh_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<ProceduralBone> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<ProceduralBone>(ref __TypeHandle.__Game_Prefabs_ProceduralBone_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<ProceduralLight> bufferTypeHandle3 = InternalCompilerInterface.GetBufferTypeHandle<ProceduralLight>(ref __TypeHandle.__Game_Prefabs_ProceduralLight_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<LightAnimation> bufferTypeHandle4 = InternalCompilerInterface.GetBufferTypeHandle<LightAnimation>(ref __TypeHandle.__Game_Prefabs_LightAnimation_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<MeshMaterial> bufferTypeHandle5 = InternalCompilerInterface.GetBufferTypeHandle<MeshMaterial>(ref __TypeHandle.__Game_Prefabs_MeshMaterial_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		bool flag = false;
		((SystemBase)this).CompleteDependency();
		LodMesh lodMesh = default(LodMesh);
		for (int i = 0; i < chunks.Length; i++)
		{
			ArchetypeChunk val = chunks[i];
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
			if (((ArchetypeChunk)(ref val)).Has<Deleted>(ref componentTypeHandle))
			{
				NativeArray<Entity> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray(entityTypeHandle);
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					if (nativeArray[j].m_Index < 0)
					{
						m_ManagedBatchSystem.RemoveMesh(nativeArray2[j]);
						flag = true;
					}
				}
				continue;
			}
			NativeArray<MeshData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<MeshData>(ref componentTypeHandle3);
			BufferAccessor<LodMesh> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<LodMesh>(ref bufferTypeHandle);
			BufferAccessor<ProceduralBone> bufferAccessor2 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ProceduralBone>(ref bufferTypeHandle2);
			BufferAccessor<ProceduralLight> bufferAccessor3 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<ProceduralLight>(ref bufferTypeHandle3);
			BufferAccessor<LightAnimation> bufferAccessor4 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<LightAnimation>(ref bufferTypeHandle4);
			BufferAccessor<MeshMaterial> bufferAccessor5 = ((ArchetypeChunk)(ref val)).GetBufferAccessor<MeshMaterial>(ref bufferTypeHandle5);
			for (int k = 0; k < nativeArray.Length; k++)
			{
				RenderPrefab prefab = m_PrefabSystem.GetPrefab<RenderPrefab>(nativeArray[k]);
				MeshData meshData = nativeArray3[k];
				meshData.m_Bounds = prefab.bounds;
				meshData.m_SubMeshCount = prefab.meshCount;
				meshData.m_IndexCount = prefab.indexCount;
				meshData.m_SmoothingDistance = 0.001f;
				meshData.m_ShadowBias = 0.5f;
				if (prefab.meshCount != prefab.materialCount)
				{
					COSystemBase.baseLog.WarnFormat((Object)(object)prefab, "{0}: subMeshCount ({1}) != materialCount ({2})", (object)((Object)prefab).name, (object)prefab.meshCount, (object)prefab.materialCount);
				}
				if (bufferAccessor5.Length != 0)
				{
					int materialCount = prefab.materialCount;
					DynamicBuffer<MeshMaterial> val2 = bufferAccessor5[k];
					val2.ResizeUninitialized(materialCount);
					int num = 0;
					foreach (SurfaceAsset surfaceAsset in prefab.surfaceAssets)
					{
						val2[num++] = new MeshMaterial
						{
							m_MaterialIndex = GetMaterialIndex(surfaceAsset)
						};
					}
				}
				if (prefab.isImpostor)
				{
					meshData.m_State |= MeshFlags.Impostor;
				}
				if (bufferAccessor.Length != 0)
				{
					LodProperties component = prefab.GetComponent<LodProperties>();
					DynamicBuffer<LodMesh> val3 = bufferAccessor[k];
					meshData.m_LodBias = component.m_Bias;
					meshData.m_ShadowBias += component.m_Bias + component.m_ShadowBias;
					if (component.m_LodMeshes != null)
					{
						val3.ResizeUninitialized(component.m_LodMeshes.Length);
						for (int l = 0; l < component.m_LodMeshes.Length; l++)
						{
							RenderPrefab renderPrefab = component.m_LodMeshes[l];
							int num2 = l;
							for (int num3 = l - 1; num3 >= 0; num3--)
							{
								RenderPrefab renderPrefab2 = component.m_LodMeshes[num3];
								if (renderPrefab.indexCount <= renderPrefab2.indexCount)
								{
									break;
								}
								val3[num2] = val3[num3];
								num2 = num3;
							}
							lodMesh.m_LodMesh = m_PrefabSystem.GetEntity(renderPrefab);
							val3[num2] = lodMesh;
						}
					}
				}
				if (prefab.surfaceArea > 0f)
				{
					float3 val4 = meshData.m_Bounds.max - meshData.m_Bounds.min;
					float num4 = math.log2(math.sqrt(math.clamp(prefab.surfaceArea / (math.csum(val4 * ((float3)(ref val4)).yzx) * 2f), 1E-06f, 1f)));
					float num5 = math.log2(math.sqrt(math.clamp(math.cmax(math.min(val4, ((float3)(ref val4)).yzx)) * 3f / math.csum(val4), 1E-06f, 1f)));
					meshData.m_LodBias -= num4;
					meshData.m_ShadowBias -= 1.5f * num4 + num5;
				}
				if (bufferAccessor2.Length != 0)
				{
					ProceduralAnimationProperties component2 = prefab.GetComponent<ProceduralAnimationProperties>();
					if (component2.m_Bones != null)
					{
						DynamicBuffer<ProceduralBone> val5 = bufferAccessor2[k];
						val5.ResizeUninitialized(component2.m_Bones.Length);
						for (int m = 0; m < component2.m_Bones.Length; m++)
						{
							ProceduralAnimationProperties.BoneInfo boneInfo = component2.m_Bones[m];
							float speed;
							float acceleration;
							switch (boneInfo.m_Type)
							{
							case BoneType.LookAtDirection:
							case BoneType.WindTurbineRotation:
							case BoneType.WindSpeedRotation:
							case BoneType.PoweredRotation:
							case BoneType.TrafficBarrierDirection:
							case BoneType.RollingRotation:
							case BoneType.PropellerRotation:
							case BoneType.LookAtRotation:
							case BoneType.LookAtAim:
							case BoneType.PropellerAngle:
							case BoneType.PantographRotation:
							case BoneType.WorkingRotation:
							case BoneType.OperatingRotation:
							case BoneType.TimeRotation:
							case BoneType.LookAtRotationSide:
							case BoneType.RotationXFromMovementY:
							case BoneType.LookAtAimForward:
								speed = boneInfo.m_Speed * ((float)Math.PI * 2f);
								acceleration = boneInfo.m_Acceleration * ((float)Math.PI * 2f);
								break;
							default:
								speed = boneInfo.m_Speed;
								acceleration = boneInfo.m_Acceleration;
								break;
							}
							int num6 = boneInfo.m_ConnectionID;
							if (num6 < 0 || num6 > 900)
							{
								COSystemBase.baseLog.ErrorFormat((Object)(object)prefab, "{0}: boneInfo[{1}].ConnectionID ({2}) != 0->900", (object)((Object)prefab).name, (object)m, (object)num6);
								num6 = 0;
							}
							val5[m] = new ProceduralBone
							{
								m_Position = float3.op_Implicit(boneInfo.position),
								m_Rotation = quaternion.op_Implicit(boneInfo.rotation),
								m_Scale = float3.op_Implicit(boneInfo.scale),
								m_BindPose = float4x4.op_Implicit(boneInfo.bindPose),
								m_ParentIndex = boneInfo.parentId,
								m_BindIndex = m,
								m_Type = boneInfo.m_Type,
								m_ConnectionID = num6,
								m_SourceIndex = boneInfo.m_SourceID,
								m_Speed = speed,
								m_Acceleration = acceleration
							};
						}
					}
				}
				if (bufferAccessor3.Length != 0)
				{
					EmissiveProperties component3 = prefab.GetComponent<EmissiveProperties>();
					if (component3.hasAnyLights)
					{
						DynamicBuffer<ProceduralLight> val6 = bufferAccessor3[k];
						val6.ResizeUninitialized(component3.lightsCount);
						int num7 = 0;
						int num8 = 0;
						if (bufferAccessor4.Length != 0)
						{
							DynamicBuffer<LightAnimation> val7 = bufferAccessor4[k];
							int num9 = 0;
							if (component3.m_SignalGroupAnimations != null)
							{
								num9 += component3.m_SignalGroupAnimations.Count;
							}
							num7 = num9;
							if (component3.m_AnimationCurves != null)
							{
								num9 += component3.m_AnimationCurves.Count;
								num8 = component3.m_AnimationCurves.Count;
							}
							val7.ResizeUninitialized(num9);
							if (component3.m_SignalGroupAnimations != null)
							{
								for (int n = 0; n < component3.m_SignalGroupAnimations.Count; n++)
								{
									EmissiveProperties.SignalGroupAnimation signalGroupAnimation = component3.m_SignalGroupAnimations[n];
									val7[n] = new LightAnimation
									{
										m_DurationFrames = (uint)math.max(1, Mathf.RoundToInt(signalGroupAnimation.m_Duration * 60f)),
										m_SignalAnimation = new SignalAnimation(signalGroupAnimation.m_SignalGroupMasks)
									};
								}
							}
							if (component3.m_AnimationCurves != null)
							{
								for (int num10 = 0; num10 < component3.m_AnimationCurves.Count; num10++)
								{
									EmissiveProperties.AnimationProperties animationProperties = component3.m_AnimationCurves[num10];
									val7[num7 + num10] = new LightAnimation
									{
										m_DurationFrames = (uint)math.max(1, Mathf.RoundToInt(animationProperties.m_Duration * 60f)),
										m_AnimationCurve = new AnimationCurve1(animationProperties.m_Curve)
									};
								}
							}
						}
						int num11 = 0;
						if (component3.hasMultiLights)
						{
							num11 = component3.m_MultiLights.Count;
							for (int num12 = 0; num12 < component3.m_MultiLights.Count; num12++)
							{
								EmissiveProperties.MultiLightMapping multiLightMapping = component3.m_MultiLights[num12];
								Color linear = ((Color)(ref multiLightMapping.color)).linear;
								Color linear2 = ((Color)(ref multiLightMapping.colorOff)).linear;
								val6[num12] = new ProceduralLight
								{
									m_Color = new float4(linear.r, linear.g, linear.b, multiLightMapping.intensity * 100f),
									m_Color2 = new float4(linear2.r, linear2.g, linear2.b, multiLightMapping.intensity * 100f),
									m_Purpose = multiLightMapping.purpose,
									m_ResponseSpeed = 1f / math.max(0.001f, multiLightMapping.responseTime),
									m_AnimationIndex = math.select(-1, num7 + multiLightMapping.animationIndex, multiLightMapping.animationIndex >= 0 && multiLightMapping.animationIndex < num8)
								};
							}
						}
						if (component3.hasSingleLights)
						{
							for (int num13 = 0; num13 < component3.m_SingleLights.Count; num13++)
							{
								EmissiveProperties.SingleLightMapping singleLightMapping = component3.m_SingleLights[num13];
								Color linear3 = ((Color)(ref singleLightMapping.color)).linear;
								Color linear4 = ((Color)(ref singleLightMapping.colorOff)).linear;
								val6[num11 + num13] = new ProceduralLight
								{
									m_Color = new float4(linear3.r, linear3.g, linear3.b, singleLightMapping.intensity * 100f),
									m_Color2 = new float4(linear4.r, linear4.g, linear4.b, singleLightMapping.intensity * 100f),
									m_Purpose = singleLightMapping.purpose,
									m_ResponseSpeed = 1f / math.max(0.001f, singleLightMapping.responseTime),
									m_AnimationIndex = math.select(-1, num7 + singleLightMapping.animationIndex, singleLightMapping.animationIndex >= 0 && singleLightMapping.animationIndex < num8)
								};
							}
						}
					}
				}
				UndergroundMesh component4 = prefab.GetComponent<UndergroundMesh>();
				if ((Object)(object)component4 != (Object)null)
				{
					if (component4.m_IsTunnel)
					{
						meshData.m_DefaultLayers |= MeshLayer.Tunnel;
					}
					if (component4.m_IsPipeline)
					{
						meshData.m_DefaultLayers |= MeshLayer.Pipeline;
					}
					if (component4.m_IsSubPipeline)
					{
						meshData.m_DefaultLayers |= MeshLayer.SubPipeline;
					}
				}
				OverlayProperties component5 = prefab.GetComponent<OverlayProperties>();
				if ((Object)(object)component5 != (Object)null && component5.m_IsWaterway)
				{
					meshData.m_DefaultLayers |= MeshLayer.Waterway;
				}
				if ((Object)(object)prefab.GetComponent<DecalProperties>() != (Object)null)
				{
					meshData.m_State |= MeshFlags.Decal;
				}
				StackProperties component6 = prefab.GetComponent<StackProperties>();
				if ((Object)(object)component6 != (Object)null)
				{
					switch (component6.m_Direction)
					{
					case StackDirection.Right:
						meshData.m_State |= MeshFlags.StackX;
						break;
					case StackDirection.Up:
						meshData.m_State |= MeshFlags.StackY;
						break;
					case StackDirection.Forward:
						meshData.m_State |= MeshFlags.StackZ;
						break;
					}
				}
				if ((Object)(object)prefab.GetComponent<AnimationProperties>() != (Object)null)
				{
					meshData.m_State |= MeshFlags.Animated;
				}
				if ((Object)(object)prefab.GetComponent<ProceduralAnimationProperties>() != (Object)null)
				{
					meshData.m_State |= MeshFlags.Skeleton;
				}
				CurveProperties component7 = prefab.GetComponent<CurveProperties>();
				if ((Object)(object)component7 != (Object)null)
				{
					meshData.m_TilingCount = component7.m_TilingCount;
					if (component7.m_OverrideLength != 0f)
					{
						meshData.m_Bounds.min.z = component7.m_OverrideLength * -0.5f;
						meshData.m_Bounds.max.z = component7.m_OverrideLength * 0.5f;
					}
					if (component7.m_SmoothingDistance > meshData.m_SmoothingDistance)
					{
						meshData.m_SmoothingDistance = component7.m_SmoothingDistance;
					}
					if (component7.m_GeometryTiling)
					{
						meshData.m_State |= MeshFlags.Tiling;
					}
					if (component7.m_InvertCurve)
					{
						meshData.m_State |= MeshFlags.Invert;
					}
				}
				BaseProperties component8 = prefab.GetComponent<BaseProperties>();
				if ((Object)(object)component8 != (Object)null && (Object)(object)component8.m_BaseType != (Object)null)
				{
					meshData.m_State |= MeshFlags.Base;
					if (component8.m_UseMinBounds)
					{
						meshData.m_State |= MeshFlags.MinBounds;
					}
				}
				if (prefab.Has<DefaultMesh>())
				{
					float renderingSize = RenderingUtils.GetRenderingSize(MathUtils.Size(meshData.m_Bounds));
					meshData.m_State |= MeshFlags.Default;
					meshData.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(renderingSize, meshData.m_LodBias);
					meshData.m_ShadowLod = (byte)RenderingUtils.CalculateLodLimit(renderingSize, meshData.m_ShadowBias);
				}
				nativeArray3[k] = meshData;
			}
		}
		InitializeMeshJob initializeMeshJob = new InitializeMeshJob
		{
			m_Chunks = chunks,
			m_DeletedType = componentTypeHandle,
			m_ProceduralBoneType = bufferTypeHandle2
		};
		((SystemBase)this).Dependency = IJobParallelForExtensions.Schedule<InitializeMeshJob>(initializeMeshJob, chunks.Length, 1, ((SystemBase)this).Dependency);
		if (flag)
		{
			JobHandle dependencies;
			JobHandle dependencies2;
			JobHandle dependencies3;
			JobHandle val8 = JobChunkExtensions.Schedule<RemoveBatchGroupsJob>(new RemoveBatchGroupsJob
			{
				m_EntityType = entityTypeHandle,
				m_DeletedType = componentTypeHandle,
				m_PrefabDataType = componentTypeHandle2,
				m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FadeBatches = InternalCompilerInterface.GetBufferLookup<FadeBatch>(ref __TypeHandle.__Game_Rendering_FadeBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BatchGroups = InternalCompilerInterface.GetBufferLookup<BatchGroup>(ref __TypeHandle.__Game_Prefabs_BatchGroup_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: false, out dependencies),
				m_NativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: false, out dependencies2),
				m_NativeSubBatches = m_BatchManagerSystem.GetNativeSubBatches(readOnly: false, out dependencies3)
			}, m_PrefabQuery, JobUtils.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2, dependencies3));
			m_BatchManagerSystem.AddNativeBatchGroupsWriter(val8);
			m_BatchManagerSystem.AddNativeBatchInstancesWriter(val8);
			m_BatchManagerSystem.AddNativeSubBatchesWriter(val8);
			((SystemBase)this).Dependency = val8;
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
	public MeshSystem()
	{
	}
}
