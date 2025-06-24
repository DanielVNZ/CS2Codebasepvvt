using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Mathematics;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class BatchManagerSystem : GameSystemBase, IPreDeserialize
{
	[BurstCompile]
	private struct MergeGroupsJob : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public BufferLookup<MeshBatch> m_MeshBatches;

		[NativeDisableParallelForRestriction]
		public BufferLookup<BatchGroup> m_BatchGroups;

		[ReadOnly]
		public NativeList<Entity> m_MergeMeshes;

		[ReadOnly]
		public NativeParallelMultiHashMap<Entity, int> m_MergeGroups;

		[NativeDisableParallelForRestriction]
		public ParallelGroupUpdater<CullingData, GroupData, BatchData, InstanceData> m_BatchGroupUpdater;

		[NativeDisableParallelForRestriction]
		public ParallelInstanceUpdater<CullingData, GroupData, BatchData, InstanceData> m_BatchInstanceUpdater;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_MergeMeshes[index];
			DynamicBuffer<BatchGroup> groups = m_BatchGroups[val];
			int num = default(int);
			NativeParallelMultiHashMapIterator<Entity> val2 = default(NativeParallelMultiHashMapIterator<Entity>);
			if (!m_MergeGroups.TryGetFirstValue(val, ref num, ref val2))
			{
				return;
			}
			do
			{
				GroupUpdater<CullingData, GroupData, BatchData, InstanceData> val3 = m_BatchGroupUpdater.BeginGroup(num);
				GroupInstanceUpdater<CullingData, GroupData, BatchData, InstanceData> val4 = m_BatchInstanceUpdater.BeginGroup(num);
				GroupData groupData = val3.GetGroupData();
				DynamicBuffer<BatchGroup> groups2 = m_BatchGroups[groupData.m_Mesh];
				int groupIndex = GetGroupIndex(groups, groupData);
				GroupUpdater<CullingData, GroupData, BatchData, InstanceData> val5 = m_BatchGroupUpdater.BeginGroup(groupIndex);
				GroupInstanceUpdater<CullingData, GroupData, BatchData, InstanceData> val6 = m_BatchInstanceUpdater.BeginGroup(groupIndex);
				int num2 = val5.MergeGroup(num, val6);
				SetGroupIndex(groups2, groupData, groupIndex, num2);
				for (int num3 = val4.GetInstanceCount() - 1; num3 >= 0; num3--)
				{
					InstanceData instanceData = val4.GetInstanceData(num3);
					CullingData cullingData = val4.GetCullingData(num3);
					val4.RemoveInstance(num3);
					int targetInstance = val6.AddInstance(instanceData, cullingData, num2);
					DynamicBuffer<MeshBatch> meshBatches = m_MeshBatches[instanceData.m_Entity];
					SetInstanceIndex(meshBatches, num, num3, groupIndex, targetInstance);
				}
				m_BatchInstanceUpdater.EndGroup(val4);
				m_BatchGroupUpdater.EndGroup(val3);
				m_BatchInstanceUpdater.EndGroup(val6);
				m_BatchGroupUpdater.EndGroup(val5);
			}
			while (m_MergeGroups.TryGetNextValue(ref num, ref val2));
		}

		private int GetGroupIndex(DynamicBuffer<BatchGroup> groups, GroupData groupData)
		{
			for (int i = 0; i < groups.Length; i++)
			{
				ref BatchGroup reference = ref groups.ElementAt(i);
				if (reference.m_Layer == groupData.m_Layer && reference.m_Type == groupData.m_MeshType && reference.m_Partition == groupData.m_Partition)
				{
					return reference.m_GroupIndex;
				}
			}
			return -1;
		}

		private void SetGroupIndex(DynamicBuffer<BatchGroup> groups, GroupData groupData, int groupIndex, int mergeIndex)
		{
			for (int i = 0; i < groups.Length; i++)
			{
				ref BatchGroup reference = ref groups.ElementAt(i);
				if (reference.m_Layer == groupData.m_Layer && reference.m_Type == groupData.m_MeshType && reference.m_Partition == groupData.m_Partition)
				{
					reference.m_GroupIndex = groupIndex;
					reference.m_MergeIndex = mergeIndex;
					break;
				}
			}
		}

		private void SetInstanceIndex(DynamicBuffer<MeshBatch> meshBatches, int sourceGroup, int sourceInstance, int targetGroup, int targetInstance)
		{
			for (int i = 0; i < meshBatches.Length; i++)
			{
				ref MeshBatch reference = ref meshBatches.ElementAt(i);
				if (reference.m_GroupIndex == sourceGroup && reference.m_InstanceIndex == sourceInstance)
				{
					reference.m_GroupIndex = targetGroup;
					reference.m_InstanceIndex = targetInstance;
					break;
				}
			}
		}
	}

	[BurstCompile]
	private struct MergeCleanupJob : IJob
	{
		public NativeList<Entity> m_MergeMeshes;

		public NativeParallelMultiHashMap<Entity, int> m_MergeGroups;

		public void Execute()
		{
			m_MergeMeshes.Clear();
			m_MergeGroups.Clear();
		}
	}

	[BurstCompile]
	private struct InitializeLodFadeJob : IJobParallelFor
	{
		public ParallelInstanceWriter<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			WriteableCullingAccessor<CullingData> cullingAccessor = m_NativeBatchInstances.GetCullingAccessor(index);
			for (int i = 0; i < cullingAccessor.Length; i++)
			{
				cullingAccessor.Get(i).lodFade = int4.op_Implicit(255);
			}
		}
	}

	[BurstCompile]
	private struct AllocateBuffersJob : IJob
	{
		[ReadOnly]
		public NativeList<PropertyData> m_ObjectProperties;

		[ReadOnly]
		public NativeList<PropertyData> m_NetProperties;

		[ReadOnly]
		public NativeList<PropertyData> m_LaneProperties;

		[ReadOnly]
		public NativeList<PropertyData> m_ZoneProperties;

		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		public void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			UpdatedPropertiesEnumerator updatedProperties = m_NativeBatchGroups.GetUpdatedProperties();
			int num = default(int);
			while (((UpdatedPropertiesEnumerator)(ref updatedProperties)).GetNextUpdatedGroup(ref num))
			{
				m_NativeBatchGroups.AllocatePropertyBuffers(num, 16777216u, m_NativeBatchInstances);
				GroupData groupData = m_NativeBatchGroups.GetGroupData(num);
				switch (groupData.m_MeshType)
				{
				case MeshType.Object:
					SetPropertyIndices(m_ObjectProperties, num, ref groupData);
					break;
				case MeshType.Net:
					SetPropertyIndices(m_NetProperties, num, ref groupData);
					break;
				case MeshType.Lane:
					SetPropertyIndices(m_LaneProperties, num, ref groupData);
					break;
				case MeshType.Zone:
					SetPropertyIndices(m_ZoneProperties, num, ref groupData);
					break;
				}
				m_NativeBatchGroups.SetGroupData(num, groupData);
			}
			m_NativeBatchGroups.ClearUpdatedProperties();
			UpdatedInstanceEnumerator updatedInstances = m_NativeBatchInstances.GetUpdatedInstances();
			int num2 = default(int);
			while (((UpdatedInstanceEnumerator)(ref updatedInstances)).GetNextUpdatedGroup(ref num2))
			{
				m_NativeBatchInstances.AllocateInstanceBuffers(num2, 16777216u, m_NativeBatchGroups);
			}
			m_NativeBatchInstances.ClearUpdatedInstances();
		}

		private void SetPropertyIndices(NativeList<PropertyData> properties, int groupIndex, ref GroupData groupData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			NativeGroupPropertyAccessor groupPropertyAccessor = m_NativeBatchGroups.GetGroupPropertyAccessor(groupIndex);
			for (int i = 0; i < properties.Length; i++)
			{
				groupData.SetPropertyIndex(i, -1);
			}
			int num = ((NativeGroupPropertyAccessor)(ref groupPropertyAccessor)).PropertyCount;
			if (num > 30)
			{
				Debug.Log((object)$"Too many group properties ({num})!");
				num = 30;
			}
			for (int j = 0; j < num; j++)
			{
				int propertyName = ((NativeGroupPropertyAccessor)(ref groupPropertyAccessor)).GetPropertyName(j);
				int dataIndex = ((NativeGroupPropertyAccessor)(ref groupPropertyAccessor)).GetDataIndex(j);
				for (int k = 0; k < properties.Length; k++)
				{
					PropertyData propertyData = properties[k];
					if (propertyName == propertyData.m_NameID && dataIndex == propertyData.m_DataIndex)
					{
						groupData.SetPropertyIndex(k, j);
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct GenerateSubBatchesJob : IJob
	{
		public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

		public void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			UpdatedSubBatchEnumerator updatedSubBatches = m_NativeSubBatches.GetUpdatedSubBatches();
			int num = default(int);
			while (((UpdatedSubBatchEnumerator)(ref updatedSubBatches)).GetNextUpdatedGroup(ref num))
			{
				m_NativeSubBatches.GenerateSubBatches(num);
			}
			m_NativeSubBatches.ClearUpdatedSubBatches();
		}
	}

	private struct ActiveGroupData
	{
		public int m_BatchOffset;

		public int m_InstanceOffset;
	}

	[BurstCompile]
	private struct AllocateCullingJob : IJob
	{
		[ReadOnly]
		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		[ReadOnly]
		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		[ReadOnly]
		public BatchRenderFlags m_RequiredFlagMask;

		[ReadOnly]
		public int m_MaxSplitBatchCount;

		public BatchCullingOutput m_CullingOutput;

		public NativeArray<ActiveGroupData> m_ActiveGroupData;

		public unsafe void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			ref BatchCullingOutputDrawCommands reference = ref CollectionUtils.ElementAt<BatchCullingOutputDrawCommands>(m_CullingOutput.drawCommands, 0);
			reference = default(BatchCullingOutputDrawCommands);
			int activeGroupCount = m_NativeBatchInstances.GetActiveGroupCount();
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < activeGroupCount; i++)
			{
				int groupIndex = m_NativeBatchInstances.GetGroupIndex(i);
				GroupData groupData = m_NativeBatchGroups.GetGroupData(groupIndex);
				if ((groupData.m_RenderFlags & m_RequiredFlagMask) == m_RequiredFlagMask)
				{
					m_ActiveGroupData[i] = new ActiveGroupData
					{
						m_BatchOffset = num,
						m_InstanceOffset = num2
					};
					int instanceCount = m_NativeBatchInstances.GetInstanceCount(groupIndex);
					num += m_NativeBatchGroups.GetBatchCount(groupIndex);
					num2 += instanceCount * (1 + groupData.m_LodCount);
					if (instanceCount > 16777216)
					{
						Debug.Log((object)$"Too many batch instances: {instanceCount} > 16777216");
					}
				}
			}
			int num3 = (reference.drawCommandCount = num * m_MaxSplitBatchCount);
			reference.drawCommands = (BatchDrawCommand*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<BatchDrawCommand>() * num3), UnsafeUtility.AlignOf<BatchDrawCommand>(), (Allocator)3);
			reference.visibleInstanceCount = num2;
			reference.visibleInstances = (int*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<int>() * num2), UnsafeUtility.AlignOf<int>(), (Allocator)3);
			reference.drawRangeCount = num;
			reference.drawRanges = (BatchDrawRange*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<BatchDrawRange>() * num), UnsafeUtility.AlignOf<BatchDrawRange>(), (Allocator)3);
		}
	}

	private struct CullingSplitData
	{
		public ulong m_PlaneMask;

		public int m_SplitMask;

		public float m_ShadowHeightThreshold;

		public float m_ShadowVolumeThreshold;
	}

	[BurstCompile]
	private struct CullingPlanesJob : IJob
	{
		[ReadOnly]
		public NativeArray<Plane> m_CullingPlanes;

		[ReadOnly]
		public NativeArray<CullingSplit> m_CullingSplits;

		[ReadOnly]
		public float3 m_ShadowCullingData;

		public NativeList<CullingSplitData> m_SplitData;

		public NativeList<FrustumPlanes.PlanePacket4> m_PlanePackets;

		private const float kGuardPixels = 5f;

		public void Execute()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Plane> cullingPlanes = default(NativeArray<Plane>);
			cullingPlanes._002Ector(m_CullingPlanes.Length, (Allocator)2, (NativeArrayOptions)1);
			int num = 0;
			for (int i = 0; i < m_CullingSplits.Length; i++)
			{
				CullingSplit val = m_CullingSplits[i];
				CullingSplitData cullingSplitData = new CullingSplitData
				{
					m_SplitMask = 1 << i,
					m_ShadowHeightThreshold = 0f,
					m_ShadowVolumeThreshold = 0f
				};
				for (int j = 0; j < val.cullingPlaneCount; j++)
				{
					Plane val2 = m_CullingPlanes[val.cullingPlaneOffset + j];
					int num2 = -1;
					for (int k = 0; k < num; k++)
					{
						Plane val3 = cullingPlanes[k];
						if (math.all(float3.op_Implicit(((Plane)(ref val2)).normal) == float3.op_Implicit(((Plane)(ref val3)).normal)) && ((Plane)(ref val2)).distance == ((Plane)(ref val3)).distance)
						{
							num2 = k;
							break;
						}
					}
					if (num2 == -1)
					{
						num2 = num++;
						cullingPlanes[num2] = val2;
					}
					cullingSplitData.m_PlaneMask |= (ulong)(1L << num2);
				}
				if (m_ShadowCullingData.x > 0f)
				{
					float num3 = CalculateCascadePixelToMeters(val.sphereRadius, m_ShadowCullingData.x);
					cullingSplitData.m_ShadowHeightThreshold = num3 * m_ShadowCullingData.y;
					cullingSplitData.m_ShadowVolumeThreshold = num3 * num3 * m_ShadowCullingData.z;
				}
				m_SplitData.Add(ref cullingSplitData);
			}
			FrustumPlanes.BuildSOAPlanePackets(cullingPlanes, num, m_PlanePackets);
			if (num > 64)
			{
				Debug.Log((object)$"Too many unique culling planes: {num} > 64");
			}
			if (m_CullingSplits.Length > 8)
			{
				Debug.Log((object)$"Too many culling splits: {m_CullingSplits.Length} > 8");
			}
			cullingPlanes.Dispose();
		}

		private float CalculateCascadePixelToMeters(float radius, float shadowResolution)
		{
			float num = radius * 2f;
			return (num + 10f * (num / shadowResolution)) / shadowResolution;
		}
	}

	[BurstCompile]
	private struct FinalizeCullingJob : IJob
	{
		public BatchCullingOutput m_CullingOutput;

		public unsafe void Execute()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			ref BatchCullingOutputDrawCommands reference = ref CollectionUtils.ElementAt<BatchCullingOutputDrawCommands>(m_CullingOutput.drawCommands, 0);
			BatchDrawRange val = default(BatchDrawRange);
			int num = 0;
			int drawRangeCount = 0;
			for (int i = 0; i < reference.drawRangeCount; i++)
			{
				BatchDrawRange val2 = ((BatchDrawRange*)reference.drawRanges)[i];
				if (val2.drawCommandsCount == 0)
				{
					continue;
				}
				int drawCommandsBegin = (int)val2.drawCommandsBegin;
				val2.drawCommandsBegin = (uint)num;
				for (int j = 0; j < val2.drawCommandsCount; j++)
				{
					System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawCommands + (nint)num++ * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawCommand>(), ((BatchDrawCommand*)reference.drawCommands)[drawCommandsBegin + j]);
				}
				if (UnsafeUtility.MemCmp((void*)(&val.filterSettings), (void*)(&val2.filterSettings), (long)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchFilterSettings>()) != 0)
				{
					if (val.drawCommandsCount != 0)
					{
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)drawRangeCount++ * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), val);
					}
					val = val2;
				}
				else
				{
					val.drawCommandsCount += val2.drawCommandsCount;
				}
			}
			if (val.drawCommandsCount != 0)
			{
				System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)drawRangeCount++ * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), val);
			}
			reference.drawCommandCount = num;
			reference.drawRangeCount = drawRangeCount;
		}
	}

	[BurstCompile]
	private struct BatchCullingJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

		[ReadOnly]
		public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

		[ReadOnly]
		public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

		[ReadOnly]
		public BatchRenderFlags m_RequiredFlagMask;

		[ReadOnly]
		public BatchRenderFlags m_RenderFlagMask;

		[ReadOnly]
		public int m_MaxSplitBatchCount;

		[ReadOnly]
		public bool m_IsShadowCulling;

		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<ActiveGroupData> m_ActiveGroupData;

		[ReadOnly]
		public NativeList<CullingSplitData> m_SplitData;

		[ReadOnly]
		public NativeList<FrustumPlanes.PlanePacket4> m_CullingPlanePackets;

		[NativeDisableParallelForRestriction]
		public BatchCullingOutput m_CullingOutput;

		public unsafe void Execute(int index)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_080a: Unknown result type (might be due to invalid IL or missing references)
			//IL_080f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0815: Unknown result type (might be due to invalid IL or missing references)
			//IL_081c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_0831: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_085f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_0719: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0782: Unknown result type (might be due to invalid IL or missing references)
			//IL_0787: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_079f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bc4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
			int groupIndex = m_NativeBatchInstances.GetGroupIndex(index);
			GroupData groupData = m_NativeBatchGroups.GetGroupData(groupIndex);
			if ((groupData.m_RenderFlags & m_RequiredFlagMask) != m_RequiredFlagMask)
			{
				return;
			}
			ActiveGroupData activeGroupData = m_ActiveGroupData[index];
			NativeBatchAccessor<BatchData> batchAccessor = m_NativeBatchGroups.GetBatchAccessor(groupIndex);
			NativeCullingAccessor<CullingData> cullingAccessor = m_NativeBatchInstances.GetCullingAccessor(groupIndex);
			NativeSubBatchAccessor<BatchData> subBatchAccessor = m_NativeSubBatches.GetSubBatchAccessor(groupIndex);
			ref BatchCullingOutputDrawCommands reference = ref CollectionUtils.ElementAt<BatchCullingOutputDrawCommands>(m_CullingOutput.drawCommands, 0);
			float3 center = default(float3);
			float3 extents = default(float3);
			if (m_IsShadowCulling)
			{
				m_NativeBatchInstances.GetShadowBounds(groupIndex, ref center, ref extents);
			}
			else
			{
				m_NativeBatchInstances.GetBounds(groupIndex, ref center, ref extents);
			}
			FrustumPlanes.PlanePacket4* unsafeReadOnlyPtr = NativeListUnsafeUtility.GetUnsafeReadOnlyPtr<FrustumPlanes.PlanePacket4>(m_CullingPlanePackets);
			int length = m_CullingPlanePackets.Length;
			CullingSplitData* unsafeReadOnlyPtr2 = NativeListUnsafeUtility.GetUnsafeReadOnlyPtr<CullingSplitData>(m_SplitData);
			int length2 = m_SplitData.Length;
			int batchCount = m_NativeBatchGroups.GetBatchCount(groupIndex);
			int instanceCount = m_NativeBatchInstances.GetInstanceCount(groupIndex);
			int* ptr = reference.visibleInstances + activeGroupData.m_InstanceOffset;
			int** ptr2 = stackalloc int*[16];
			bool flag = length2 == 4 && m_IsShadowCulling;
			if (length2 == 1)
			{
				CullingSplitData cullingSplitData = *unsafeReadOnlyPtr2;
				for (int i = 0; i <= groupData.m_LodCount; i++)
				{
					int num = i * instanceCount;
					ptr2[i] = ptr + num;
				}
				switch (FrustumPlanes.CalculateIntersectResult(unsafeReadOnlyPtr, length, center, extents))
				{
				case FrustumPlanes.IntersectResult.In:
				{
					for (int l = 0; l < instanceCount; l++)
					{
						int4 lodRange2 = cullingAccessor.Get(l).lodRange;
						((int4)(ref lodRange2)).xy = math.select(((int4)(ref lodRange2)).xy, ((int4)(ref lodRange2)).zw, m_IsShadowCulling);
						for (int m = lodRange2.x; m < lodRange2.y; m++)
						{
							int** num3 = ptr2 + m;
							int* ptr3 = *num3;
							*num3 = ptr3 + 1;
							*ptr3 = l;
						}
					}
					break;
				}
				case FrustumPlanes.IntersectResult.Partial:
				{
					for (int j = 0; j < instanceCount; j++)
					{
						CullingData cullingData = cullingAccessor.Get(j);
						float3 center2 = MathUtils.Center(cullingData.m_Bounds);
						float3 extents2 = MathUtils.Extents(cullingData.m_Bounds);
						if (FrustumPlanes.Intersect(unsafeReadOnlyPtr, length, center2, extents2))
						{
							int4 lodRange = cullingData.lodRange;
							((int4)(ref lodRange)).xy = math.select(((int4)(ref lodRange)).xy, ((int4)(ref lodRange)).zw, m_IsShadowCulling);
							for (int k = lodRange.x; k < lodRange.y; k++)
							{
								int** num2 = ptr2 + k;
								int* ptr3 = *num2;
								*num2 = ptr3 + 1;
								*ptr3 = j;
							}
						}
					}
					break;
				}
				}
				int num4 = -1;
				int visibleOffset = 0;
				int num5 = 0;
				BatchMeshID meshID = default(BatchMeshID);
				BatchMaterialID materialID = default(BatchMaterialID);
				int num8 = default(int);
				for (int n = 0; n < batchCount; n++)
				{
					BatchData batchData = batchAccessor.GetBatchData(n);
					int num6 = activeGroupData.m_BatchOffset + n;
					if (batchData.m_LodIndex != num4)
					{
						int num7 = groupData.m_LodCount - batchData.m_LodIndex;
						num4 = batchData.m_LodIndex;
						visibleOffset = num7 * instanceCount;
						num5 = (int)(ptr2[num7] - (ptr + visibleOffset));
						visibleOffset += activeGroupData.m_InstanceOffset;
					}
					if (num5 != 0 && (batchData.m_RenderFlags & m_RequiredFlagMask) == m_RequiredFlagMask)
					{
						batchAccessor.GetRenderData(n, ref meshID, ref materialID, ref num8);
						BatchID batchID = subBatchAccessor.GetBatchID(n);
						BatchRenderFlags num9 = batchData.m_RenderFlags & m_RenderFlagMask;
						BatchDrawCommand val = new BatchDrawCommand
						{
							visibleOffset = (uint)visibleOffset,
							visibleCount = (uint)num5,
							batchID = batchID,
							materialID = materialID,
							meshID = meshID,
							submeshIndex = (ushort)num8,
							splitVisibilityMask = (ushort)cullingSplitData.m_SplitMask
						};
						BatchFilterSettings filterSettings = new BatchFilterSettings
						{
							layer = batchData.m_Layer,
							renderingLayerMask = uint.MaxValue
						};
						((BatchFilterSettings)(ref filterSettings)).motionMode = (MotionVectorGenerationMode)2;
						((BatchFilterSettings)(ref filterSettings)).receiveShadows = (batchData.m_RenderFlags & BatchRenderFlags.ReceiveShadows) != 0;
						((BatchFilterSettings)(ref filterSettings)).shadowCastingMode = (ShadowCastingMode)batchData.m_ShadowCastingMode;
						if ((num9 & BatchRenderFlags.MotionVectors) != 0)
						{
							ref BatchDrawCommandFlags flags = ref val.flags;
							flags = (BatchDrawCommandFlags)((uint)flags | 2u);
							((BatchFilterSettings)(ref filterSettings)).motionMode = (MotionVectorGenerationMode)1;
						}
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawCommands + (nint)num6 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawCommand>(), val);
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)num6 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), new BatchDrawRange
						{
							drawCommandsBegin = (uint)num6,
							drawCommandsCount = 1u,
							filterSettings = filterSettings
						});
					}
					else
					{
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)num6 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), default(BatchDrawRange));
					}
				}
				return;
			}
			for (int num10 = 0; num10 <= groupData.m_LodCount; num10++)
			{
				int num11 = num10 * instanceCount;
				ptr2[num10] = ptr + num11;
			}
			FrustumPlanes.Intersect(unsafeReadOnlyPtr, length, center, extents, out var inMask, out var outMask);
			int num12 = 0;
			ulong num13 = 0uL;
			int num14 = length2;
			int num15 = 0;
			for (int num16 = 0; num16 < length2; num16++)
			{
				CullingSplitData cullingSplitData2 = unsafeReadOnlyPtr2[num16];
				if ((cullingSplitData2.m_PlaneMask & outMask) == 0L)
				{
					if ((cullingSplitData2.m_PlaneMask & inMask) == cullingSplitData2.m_PlaneMask)
					{
						num12 |= cullingSplitData2.m_SplitMask;
						continue;
					}
					num13 |= cullingSplitData2.m_PlaneMask & ~inMask;
					num14 = math.min(num14, num16);
					num15 = num16;
				}
			}
			if (num13 != 0L)
			{
				int num17 = length;
				int num18 = 0;
				for (int num19 = 0; num19 < length; num19++)
				{
					if ((num13 & (ulong)(15L << (num19 << 2))) != 0L)
					{
						num17 = math.min(num17, num19);
						num18 = num19;
					}
				}
				FrustumPlanes.PlanePacket4* cullingPlanePackets = unsafeReadOnlyPtr + num17;
				int length3 = num18 - num17 + 1;
				int num20 = num17 << 2;
				if (num18 < 8)
				{
					for (int num21 = 0; num21 < instanceCount; num21++)
					{
						CullingData cullingData2 = cullingAccessor.Get(num21);
						int num22 = num12;
						float3 center3 = MathUtils.Center(cullingData2.m_Bounds);
						float3 extents3 = MathUtils.Extents(cullingData2.m_Bounds);
						FrustumPlanes.Intersect(cullingPlanePackets, length3, center3, extents3, out uint outMask2);
						outMask2 <<= num20;
						for (int num23 = num14; num23 <= num15; num23++)
						{
							CullingSplitData cullingSplitData3 = unsafeReadOnlyPtr2[num23];
							num22 |= math.select(0, cullingSplitData3.m_SplitMask, ((uint)(int)cullingSplitData3.m_PlaneMask & outMask2) == 0);
						}
						if (num22 != 0)
						{
							int num24 = (num22 << 24) | num21;
							int4 lodRange3 = cullingData2.lodRange;
							((int4)(ref lodRange3)).xy = math.select(((int4)(ref lodRange3)).xy, ((int4)(ref lodRange3)).zw, m_IsShadowCulling);
							for (int num25 = lodRange3.x; num25 < lodRange3.y; num25++)
							{
								int** num26 = ptr2 + num25;
								int* ptr3 = *num26;
								*num26 = ptr3 + 1;
								*ptr3 = num24;
							}
						}
					}
				}
				else
				{
					for (int num27 = 0; num27 < instanceCount; num27++)
					{
						CullingData cullingData3 = cullingAccessor.Get(num27);
						int num28 = num12;
						float3 center4 = MathUtils.Center(cullingData3.m_Bounds);
						float3 extents4 = MathUtils.Extents(cullingData3.m_Bounds);
						FrustumPlanes.Intersect(cullingPlanePackets, length3, center4, extents4, out ulong outMask3);
						outMask3 <<= num20;
						for (int num29 = num14; num29 <= num15; num29++)
						{
							CullingSplitData cullingSplitData4 = unsafeReadOnlyPtr2[num29];
							num28 |= math.select(0, cullingSplitData4.m_SplitMask, (cullingSplitData4.m_PlaneMask & outMask3) == 0);
						}
						if (num28 != 0)
						{
							int num30 = (num28 << 24) | num27;
							int4 lodRange4 = cullingData3.lodRange;
							((int4)(ref lodRange4)).xy = math.select(((int4)(ref lodRange4)).xy, ((int4)(ref lodRange4)).zw, m_IsShadowCulling);
							for (int num31 = lodRange4.x; num31 < lodRange4.y; num31++)
							{
								int** num32 = ptr2 + num31;
								int* ptr3 = *num32;
								*num32 = ptr3 + 1;
								*ptr3 = num30;
							}
						}
					}
				}
			}
			else if (num12 != 0)
			{
				for (int num33 = 0; num33 < instanceCount; num33++)
				{
					int4 lodRange5 = cullingAccessor.Get(num33).lodRange;
					((int4)(ref lodRange5)).xy = math.select(((int4)(ref lodRange5)).xy, ((int4)(ref lodRange5)).zw, m_IsShadowCulling);
					for (int num34 = lodRange5.x; num34 < lodRange5.y; num34++)
					{
						int** num35 = ptr2 + num34;
						int* ptr3 = *num35;
						*num35 = ptr3 + 1;
						*ptr3 = num33;
					}
				}
			}
			int num36 = -1;
			int num37 = 0;
			int num38 = 0;
			int* ptr4 = stackalloc int[15];
			int* ptr5 = stackalloc int[15];
			int* ptr6 = stackalloc int[15];
			BatchMeshID meshID2 = default(BatchMeshID);
			BatchMaterialID materialID2 = default(BatchMaterialID);
			int num48 = default(int);
			for (int num39 = 0; num39 < batchCount; num39++)
			{
				BatchData batchData2 = batchAccessor.GetBatchData(num39);
				int num40 = activeGroupData.m_BatchOffset + num39;
				int num41 = num40 * m_MaxSplitBatchCount;
				if (batchData2.m_LodIndex != num36)
				{
					int num42 = groupData.m_LodCount - batchData2.m_LodIndex;
					num36 = batchData2.m_LodIndex;
					int num43 = num42 * instanceCount;
					int* ptr7 = ptr + num43;
					num37 = (int)(ptr2[num42] - ptr7);
					num43 += activeGroupData.m_InstanceOffset;
					if (num37 != 0)
					{
						if (num13 != 0L)
						{
							if (num37 >= 3)
							{
								NativeSortExtension.Sort<int>(ptr7, num37);
							}
							num38 = 0;
							int num44 = 0;
							while (num44 < num37)
							{
								int num45 = num44;
								int* ptr8 = ptr7 + num44++;
								int num46 = *ptr8 >>> 24;
								*ptr8 &= 16777215;
								if (num38 < m_MaxSplitBatchCount - 1)
								{
									for (; num44 < num37; num44++)
									{
										ptr8 = ptr7 + num44;
										if (*ptr8 >>> 24 != num46)
										{
											break;
										}
										*ptr8 &= 16777215;
									}
								}
								else
								{
									for (; num44 < num37; num44++)
									{
										ptr8 = ptr7 + num44;
										num46 |= *ptr8 >>> 24;
										*ptr8 &= 16777215;
									}
								}
								ptr4[num38] = num43 + num45;
								ptr5[num38] = num44 - num45;
								ptr6[num38] = num46;
								num38++;
							}
						}
						else
						{
							num38 = 1;
							*ptr4 = num43;
							*ptr5 = num37;
							*ptr6 = num12;
						}
					}
				}
				int num47 = (flag ? CalculateBatchMask(batchData2, unsafeReadOnlyPtr2, length2) : (-1));
				if (num37 != 0 && (batchData2.m_RenderFlags & m_RequiredFlagMask) == m_RequiredFlagMask)
				{
					batchAccessor.GetRenderData(num39, ref meshID2, ref materialID2, ref num48);
					BatchID batchID2 = subBatchAccessor.GetBatchID(num39);
					BatchRenderFlags num49 = batchData2.m_RenderFlags & m_RenderFlagMask;
					BatchDrawCommand val2 = new BatchDrawCommand
					{
						batchID = batchID2,
						materialID = materialID2,
						meshID = meshID2,
						submeshIndex = (ushort)num48
					};
					BatchFilterSettings filterSettings2 = new BatchFilterSettings
					{
						layer = batchData2.m_Layer,
						renderingLayerMask = uint.MaxValue
					};
					((BatchFilterSettings)(ref filterSettings2)).motionMode = (MotionVectorGenerationMode)2;
					((BatchFilterSettings)(ref filterSettings2)).receiveShadows = (batchData2.m_RenderFlags & BatchRenderFlags.ReceiveShadows) != 0;
					((BatchFilterSettings)(ref filterSettings2)).shadowCastingMode = (ShadowCastingMode)batchData2.m_ShadowCastingMode;
					if ((num49 & BatchRenderFlags.MotionVectors) != 0)
					{
						ref BatchDrawCommandFlags flags2 = ref val2.flags;
						flags2 = (BatchDrawCommandFlags)((uint)flags2 | 2u);
						((BatchFilterSettings)(ref filterSettings2)).motionMode = (MotionVectorGenerationMode)1;
					}
					int num50 = 0;
					for (int num51 = 0; num51 < num38; num51++)
					{
						val2.visibleOffset = (uint)ptr4[num51];
						val2.visibleCount = (uint)ptr5[num51];
						val2.splitVisibilityMask = (ushort)(ptr6[num51] & num47);
						if (val2.splitVisibilityMask != 0)
						{
							System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawCommands + (nint)(num41 + num50) * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawCommand>(), val2);
							num50++;
						}
					}
					if (num50 > 0)
					{
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)num40 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), new BatchDrawRange
						{
							drawCommandsBegin = (uint)num41,
							drawCommandsCount = (uint)num50,
							filterSettings = filterSettings2
						});
					}
					else
					{
						System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)num40 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), default(BatchDrawRange));
					}
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.Write((byte*)reference.drawRanges + (nint)num40 * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<BatchDrawRange>(), default(BatchDrawRange));
				}
			}
		}

		private unsafe int CalculateBatchMask(BatchData batchData, CullingSplitData* cullSplitPtr, int length)
		{
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				CullingSplitData cullingSplitData = cullSplitPtr[i];
				num |= ((!(batchData.m_ShadowArea < cullingSplitData.m_ShadowVolumeThreshold) && !(batchData.m_ShadowHeight < cullingSplitData.m_ShadowHeightThreshold)) ? (1 << i) : 0);
			}
			return num;
		}
	}

	private struct TypeHandle
	{
		public BufferLookup<MeshBatch> __Game_Rendering_MeshBatch_RW_BufferLookup;

		public BufferLookup<BatchGroup> __Game_Prefabs_BatchGroup_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Rendering_MeshBatch_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshBatch>(false);
			__Game_Prefabs_BatchGroup_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BatchGroup>(false);
		}
	}

	public const uint GPU_INSTANCE_MEMORY_DEFAULT = 67108864u;

	public const uint GPU_INSTANCE_MEMORY_INCREMENT = 16777216u;

	public const uint GPU_UPLOADER_CHUNK_SIZE = 2097152u;

	public const uint GPU_UPLOADER_OPERATION_SIZE = 65536u;

	public const int MAX_GROUP_BATCH_COUNT = 16;

	private RenderingSystem m_RenderingSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private BatchDataSystem m_BatchDataSystem;

	private TextureStreamingSystem m_TextureStreamingSystem;

	private PrefabSystem m_PrefabSystem;

	private NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchGroups;

	private NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> m_NativeBatchInstances;

	private NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> m_NativeSubBatches;

	private ManagedBatches<OptionalProperties> m_ManagedBatches;

	private NativeList<PropertyData> m_MaterialProperties;

	private NativeList<PropertyData> m_ObjectProperties;

	private NativeList<PropertyData> m_NetProperties;

	private NativeList<PropertyData> m_LaneProperties;

	private NativeList<PropertyData> m_ZoneProperties;

	private NativeList<Entity> m_MergeMeshes;

	private NativeParallelMultiHashMap<Entity, int> m_MergeGroups;

	private EntityQuery m_MeshSettingsQuery;

	private bool m_LastMotionVectorsEnabled;

	private bool m_LastLodFadeEnabled;

	private bool m_PropertiesChanged;

	private bool m_MotionVectorsChanged;

	private bool m_LodFadeChanged;

	private bool m_VirtualTexturingChanged;

	private JobHandle m_NativeBatchGroupsReadDependencies;

	private JobHandle m_NativeBatchGroupsWriteDependencies;

	private JobHandle m_NativeBatchInstancesReadDependencies;

	private JobHandle m_NativeBatchInstancesWriteDependencies;

	private JobHandle m_NativeSubBatchesReadDependencies;

	private JobHandle m_NativeSubBatchesWriteDependencies;

	private JobHandle m_MergeDependencies;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		m_TextureStreamingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TextureStreamingSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_NativeBatchGroups = new NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData>(67108864u, 65536u, (Allocator)4);
		m_NativeBatchInstances = new NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData>(m_NativeBatchGroups);
		m_NativeSubBatches = new NativeSubBatches<CullingData, GroupData, BatchData, InstanceData>(m_NativeBatchGroups);
		m_ManagedBatches = ManagedBatches<OptionalProperties>.Create<CullingData, GroupData, BatchData, InstanceData>(m_NativeBatchInstances, new OnPerformCulling(OnPerformCulling), 2097152u, new OptionalProperties(BatchFlags.MotionVectors, MeshType.Object));
		InitializeMaterialProperties<MaterialProperty>(out m_MaterialProperties);
		InitializeInstanceProperties<ObjectProperty>(out m_ObjectProperties, MeshType.Object);
		InitializeInstanceProperties<NetProperty>(out m_NetProperties, MeshType.Net);
		InitializeInstanceProperties<LaneProperty>(out m_LaneProperties, MeshType.Lane);
		InitializeInstanceProperties<ZoneProperty>(out m_ZoneProperties, MeshType.Zone);
		m_MergeMeshes = new NativeList<Entity>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_MergeGroups = new NativeParallelMultiHashMap<Entity, int>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_MeshSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MeshSettingsData>() });
		m_LastMotionVectorsEnabled = m_RenderingSystem.motionVectors;
		m_LastLodFadeEnabled = m_RenderingSystem.lodCrossFade;
	}

	private void InitializeMaterialProperties<T>(out NativeList<PropertyData> properties)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Array values = Enum.GetValues(typeof(T));
		properties = new NativeList<PropertyData>(values.Length, AllocatorHandle.op_Implicit((Allocator)4));
		foreach (T item in values)
		{
			object[] customAttributes = typeof(T).GetMember(item.ToString())[0].GetCustomAttributes(typeof(MaterialPropertyAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				MaterialPropertyAttribute materialPropertyAttribute = (MaterialPropertyAttribute)customAttributes[0];
				PropertyData propertyData = new PropertyData
				{
					m_NameID = Shader.PropertyToID(materialPropertyAttribute.ShaderPropertyName)
				};
				properties.Add(ref propertyData);
				m_ManagedBatches.RegisterMaterialPropertyType(materialPropertyAttribute.ShaderPropertyName, materialPropertyAttribute.DataType, false, default(MaterialPropertyDefaultValue), false, materialPropertyAttribute.IsBuiltin, default(OptionalProperties), default(OptionalProperties));
			}
		}
	}

	private void InitializeInstanceProperties<T>(out NativeList<PropertyData> properties, MeshType meshType)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Array values = Enum.GetValues(typeof(T));
		properties = new NativeList<PropertyData>(values.Length, AllocatorHandle.op_Implicit((Allocator)4));
		foreach (T item in values)
		{
			object[] customAttributes = typeof(T).GetMember(item.ToString())[0].GetCustomAttributes(typeof(InstancePropertyAttribute), inherit: false);
			if (customAttributes.Length != 0)
			{
				InstancePropertyAttribute instancePropertyAttribute = (InstancePropertyAttribute)customAttributes[0];
				PropertyData propertyData = new PropertyData
				{
					m_NameID = Shader.PropertyToID(instancePropertyAttribute.ShaderPropertyName),
					m_DataIndex = instancePropertyAttribute.DataIndex
				};
				properties.Add(ref propertyData);
				m_ManagedBatches.RegisterMaterialPropertyType(instancePropertyAttribute.ShaderPropertyName, instancePropertyAttribute.DataType, true, default(MaterialPropertyDefaultValue), false, instancePropertyAttribute.IsBuiltin, new OptionalProperties(instancePropertyAttribute.RequiredFlags, meshType), new OptionalProperties((BatchFlags)0, meshType));
			}
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_NativeBatchGroupsReadDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchGroupsWriteDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchInstancesReadDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchInstancesWriteDependencies)).Complete();
		((JobHandle)(ref m_NativeSubBatchesReadDependencies)).Complete();
		((JobHandle)(ref m_NativeSubBatchesWriteDependencies)).Complete();
		((JobHandle)(ref m_MergeDependencies)).Complete();
		m_ManagedBatches.EndUpload<CullingData, GroupData, BatchData, InstanceData>(m_NativeBatchInstances);
		m_ManagedBatches.Dispose();
		m_NativeSubBatches.Dispose();
		m_NativeBatchInstances.Dispose();
		m_NativeBatchGroups.Dispose();
		m_MaterialProperties.Dispose();
		m_ObjectProperties.Dispose();
		m_NetProperties.Dispose();
		m_LaneProperties.Dispose();
		m_ZoneProperties.Dispose();
		m_MergeMeshes.Dispose();
		m_MergeGroups.Dispose();
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_NativeBatchGroupsReadDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchGroupsWriteDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchInstancesReadDependencies)).Complete();
		((JobHandle)(ref m_NativeBatchInstancesWriteDependencies)).Complete();
		((JobHandle)(ref m_NativeSubBatchesReadDependencies)).Complete();
		((JobHandle)(ref m_NativeSubBatchesWriteDependencies)).Complete();
		m_ManagedBatches.EndUpload<CullingData, GroupData, BatchData, InstanceData>(m_NativeBatchInstances);
		int groupCount = m_NativeBatchGroups.GetGroupCount();
		for (int i = 0; i < groupCount; i++)
		{
			if (m_NativeBatchGroups.IsValidGroup(i))
			{
				m_NativeBatchInstances.RemoveInstances(i, m_NativeSubBatches);
			}
		}
	}

	public bool CheckPropertyUpdates()
	{
		bool motionVectors = m_RenderingSystem.motionVectors;
		if (motionVectors != m_LastMotionVectorsEnabled)
		{
			m_LastMotionVectorsEnabled = motionVectors;
			m_MotionVectorsChanged = true;
		}
		bool lodCrossFade = m_RenderingSystem.lodCrossFade;
		if (lodCrossFade != m_LastLodFadeEnabled)
		{
			m_LastLodFadeEnabled = lodCrossFade;
			m_LodFadeChanged = true;
		}
		if (!m_PropertiesChanged && !m_MotionVectorsChanged && !m_LodFadeChanged)
		{
			return m_VirtualTexturingChanged;
		}
		return true;
	}

	public void VirtualTexturingUpdated()
	{
		m_VirtualTexturingChanged = true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (m_PropertiesChanged || m_MotionVectorsChanged || m_LodFadeChanged || m_VirtualTexturingChanged)
		{
			try
			{
				RefreshProperties(m_PropertiesChanged, m_MotionVectorsChanged, m_LodFadeChanged, m_VirtualTexturingChanged);
			}
			finally
			{
				m_PropertiesChanged = false;
				m_MotionVectorsChanged = false;
				m_LodFadeChanged = false;
				m_VirtualTexturingChanged = false;
			}
		}
		((JobHandle)(ref m_MergeDependencies)).Complete();
		if (m_MergeMeshes.Length != 0)
		{
			MergeGroups();
		}
		JobHandle dependencies;
		JobHandle dependencies2;
		AllocateBuffersJob allocateBuffersJob = new AllocateBuffersJob
		{
			m_ObjectProperties = m_ObjectProperties,
			m_NetProperties = m_NetProperties,
			m_LaneProperties = m_LaneProperties,
			m_ZoneProperties = m_ZoneProperties,
			m_NativeBatchGroups = GetNativeBatchGroups(readOnly: false, out dependencies),
			m_NativeBatchInstances = GetNativeBatchInstances(readOnly: false, out dependencies2)
		};
		JobHandle dependencies3;
		GenerateSubBatchesJob obj = new GenerateSubBatchesJob
		{
			m_NativeSubBatches = GetNativeSubBatches(readOnly: false, out dependencies3)
		};
		JobHandle jobHandle = IJobExtensions.Schedule<AllocateBuffersJob>(allocateBuffersJob, JobHandle.CombineDependencies(dependencies, dependencies2));
		JobHandle jobHandle2 = IJobExtensions.Schedule<GenerateSubBatchesJob>(obj, dependencies3);
		AddNativeBatchGroupsWriter(jobHandle);
		AddNativeBatchInstancesWriter(jobHandle);
		AddNativeSubBatchesWriter(jobHandle2);
	}

	public NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> GetNativeBatchGroups(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_NativeBatchGroupsWriteDependencies : JobHandle.CombineDependencies(m_NativeBatchGroupsReadDependencies, m_NativeBatchGroupsWriteDependencies));
		return m_NativeBatchGroups;
	}

	public NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> GetNativeBatchInstances(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_NativeBatchInstancesWriteDependencies : JobHandle.CombineDependencies(m_NativeBatchInstancesReadDependencies, m_NativeBatchInstancesWriteDependencies));
		return m_NativeBatchInstances;
	}

	public NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> GetNativeSubBatches(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_NativeSubBatchesWriteDependencies : JobHandle.CombineDependencies(m_NativeSubBatchesReadDependencies, m_NativeSubBatchesWriteDependencies));
		return m_NativeSubBatches;
	}

	public ManagedBatches<OptionalProperties> GetManagedBatches()
	{
		return m_ManagedBatches;
	}

	public bool IsMotionVectorsEnabled()
	{
		return m_LastMotionVectorsEnabled;
	}

	public bool IsLodFadeEnabled()
	{
		return m_LastLodFadeEnabled;
	}

	public void AddNativeBatchGroupsReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_NativeBatchGroupsReadDependencies = JobHandle.CombineDependencies(m_NativeBatchGroupsReadDependencies, jobHandle);
	}

	public void AddNativeBatchGroupsWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_NativeBatchGroupsWriteDependencies = jobHandle;
	}

	public void AddNativeBatchInstancesReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_NativeBatchInstancesReadDependencies = JobHandle.CombineDependencies(m_NativeBatchInstancesReadDependencies, jobHandle);
	}

	public void AddNativeBatchInstancesWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_NativeBatchInstancesWriteDependencies = jobHandle;
	}

	public void AddNativeSubBatchesReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_NativeSubBatchesReadDependencies = JobHandle.CombineDependencies(m_NativeSubBatchesReadDependencies, jobHandle);
	}

	public void AddNativeSubBatchesWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_NativeSubBatchesWriteDependencies = jobHandle;
	}

	public void MergeGroups(Entity meshEntity, int mergeIndex)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_MergeDependencies)).Complete();
		if (!m_MergeGroups.ContainsKey(meshEntity))
		{
			m_MergeMeshes.Add(ref meshEntity);
		}
		m_MergeGroups.Add(meshEntity, mergeIndex);
	}

	public (int, int) GetVTTextureParamBlockID(int stackConfigIndex)
	{
		return stackConfigIndex switch
		{
			0 => (m_MaterialProperties[0].m_NameID, m_MaterialProperties[2].m_NameID), 
			1 => (m_MaterialProperties[1].m_NameID, m_MaterialProperties[3].m_NameID), 
			_ => throw new IndexOutOfRangeException("stackConfigIndex cannot be greated than 2"), 
		};
	}

	public PropertyData GetPropertyData(MaterialProperty property)
	{
		return m_MaterialProperties[(int)property];
	}

	public PropertyData GetPropertyData(ObjectProperty property)
	{
		return m_ObjectProperties[(int)property];
	}

	public PropertyData GetPropertyData(NetProperty property)
	{
		return m_NetProperties[(int)property];
	}

	public PropertyData GetPropertyData(LaneProperty property)
	{
		return m_LaneProperties[(int)property];
	}

	public PropertyData GetPropertyData(ZoneProperty property)
	{
		return m_ZoneProperties[(int)property];
	}

	private void RefreshProperties(bool propertiesChanged, bool motionVectorsChanged, bool lodFadeChanged, bool virtualTexturingChanged)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = GetNativeBatchGroups(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = GetNativeBatchInstances(readOnly: false, out dependencies2);
		JobHandle dependencies3;
		NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = GetNativeSubBatches(readOnly: false, out dependencies3);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		((JobHandle)(ref dependencies3)).Complete();
		int groupCount = nativeBatchGroups.GetGroupCount();
		bool flag = false;
		Dictionary<BatchPropertiesKey<OptionalProperties>, bool> dictionary = null;
		if (propertiesChanged)
		{
			dictionary = new Dictionary<BatchPropertiesKey<OptionalProperties>, bool>();
		}
		MeshSettingsData meshSettingsData = default(MeshSettingsData);
		if (!((EntityQuery)(ref m_MeshSettingsQuery)).IsEmptyIgnoreFilter)
		{
			meshSettingsData = ((EntityQuery)(ref m_MeshSettingsQuery)).GetSingleton<MeshSettingsData>();
		}
		BatchPropertiesKey<OptionalProperties> key = default(BatchPropertiesKey<OptionalProperties>);
		for (int i = 0; i < groupCount; i++)
		{
			if (!nativeBatchGroups.IsValidGroup(i))
			{
				continue;
			}
			int batchCount = nativeBatchGroups.GetBatchCount(i);
			GroupData groupData = nativeBatchGroups.GetGroupData(i);
			for (int j = 0; j < batchCount; j++)
			{
				int managedBatchIndex = nativeBatchGroups.GetManagedBatchIndex(i, j);
				if (managedBatchIndex < 0)
				{
					continue;
				}
				CustomBatch customBatch = (CustomBatch)(object)m_ManagedBatches.GetBatch(managedBatchIndex);
				BatchFlags batchFlags = customBatch.sourceFlags;
				if (!IsMotionVectorsEnabled())
				{
					batchFlags &= ~BatchFlags.MotionVectors;
				}
				if (!IsLodFadeEnabled())
				{
					batchFlags &= ~BatchFlags.LodFade;
				}
				OptionalProperties optionalProperties = new OptionalProperties(batchFlags, customBatch.sourceType);
				bool flag2 = ((customBatch.sourceFlags & BatchFlags.MotionVectors) != 0 && motionVectorsChanged) || ((customBatch.sourceFlags & BatchFlags.LodFade) != 0 && lodFadeChanged);
				if ((customBatch.sourceType & (MeshType.Net | MeshType.Zone)) == 0)
				{
					RenderPrefab renderPrefab = m_PrefabSystem.GetPrefab<RenderPrefab>(customBatch.sourceMeshEntity);
					if (virtualTexturingChanged)
					{
						DecalProperties decalProperties = renderPrefab.GetComponent<DecalProperties>();
						if ((Object)(object)decalProperties != (Object)null && groupData.m_Layer == MeshLayer.Outline)
						{
							decalProperties = null;
						}
						VTAtlassingInfo[] array = customBatch.sourceSurface.VTAtlassingInfos;
						if (array == null)
						{
							array = customBatch.sourceSurface.PreReservedAtlassingInfos;
						}
						if (array != null)
						{
							if ((Object)(object)decalProperties != (Object)null || renderPrefab.manualVTRequired || renderPrefab.isImpostor)
							{
								BatchData batchData = nativeBatchGroups.GetBatchData(i, j);
								Bounds2 bounds = MathUtils.Bounds(new float2(0f, 0f), new float2(1f, 1f));
								batchData.m_VTIndex0 = -1;
								batchData.m_VTIndex1 = -1;
								if ((Object)(object)decalProperties != (Object)null)
								{
									bounds = MathUtils.Bounds(decalProperties.m_TextureArea.min, decalProperties.m_TextureArea.max);
								}
								if (array.Length >= 1 && array[0].indexInStack >= 0)
								{
									batchData.m_VTIndex0 = m_ManagedBatchSystem.VTTextureRequester.RegisterTexture(0, array[0].stackGlobalIndex, array[0].indexInStack, bounds);
								}
								if (array.Length >= 2 && array[1].indexInStack >= 0)
								{
									batchData.m_VTIndex1 = m_ManagedBatchSystem.VTTextureRequester.RegisterTexture(1, array[1].stackGlobalIndex, array[1].indexInStack, bounds);
								}
								nativeBatchGroups.SetBatchData(i, j, batchData);
							}
							if (!renderPrefab.Has<DefaultMesh>())
							{
								for (int k = 0; k < 2; k++)
								{
									if (array.Length > k && array[k].indexInStack >= 0)
									{
										((ManagedBatch)customBatch).customProps.SetTextureParamBlock(GetVTTextureParamBlockID(k), m_TextureStreamingSystem.GetTextureParamBlock(array[k]));
										flag2 = true;
									}
								}
							}
						}
					}
					if (customBatch.generatedType == GeneratedType.ObjectBase)
					{
						BaseProperties component = renderPrefab.GetComponent<BaseProperties>();
						if ((Object)(object)component == (Object)null && (customBatch.sourceFlags & BatchFlags.Lod) != 0)
						{
							renderPrefab = m_PrefabSystem.GetPrefab<RenderPrefab>(groupData.m_Mesh);
							component = renderPrefab.GetComponent<BaseProperties>();
						}
						renderPrefab = ((!((Object)(object)component != (Object)null)) ? m_PrefabSystem.GetPrefab<RenderPrefab>(meshSettingsData.m_DefaultBaseMesh) : component.m_BaseType);
					}
					m_ManagedBatchSystem.SetupVT(renderPrefab, ((ManagedBatch)customBatch).material, customBatch.sourceSubMeshIndex);
				}
				if (propertiesChanged)
				{
					key._002Ector(((ManagedBatch)customBatch).material.shader, optionalProperties);
					if (!dictionary.TryGetValue(key, out var value))
					{
						value = m_ManagedBatches.RegenerateBatchProperties(((ManagedBatch)customBatch).material.shader, optionalProperties);
						dictionary.Add(key, value);
					}
					flag2 = flag2 || value;
				}
				if (flag2)
				{
					NativeBatchProperties batchProperties = m_ManagedBatches.GetBatchProperties(((ManagedBatch)customBatch).material.shader, optionalProperties);
					nativeBatchGroups.SetBatchProperties(i, j, batchProperties);
					nativeSubBatches.RecreateRenderers(i, j);
					WriteableBatchDefaultsAccessor batchDefaultsAccessor = nativeBatchGroups.GetBatchDefaultsAccessor(i, j);
					if ((AssetData)(object)customBatch.sourceSurface != (IAssetData)null)
					{
						m_ManagedBatches.SetDefaults(ManagedBatchSystem.GetTemplate(customBatch.sourceSurface), customBatch.sourceSurface.floats, customBatch.sourceSurface.ints, customBatch.sourceSurface.vectors, customBatch.sourceSurface.colors, ((ManagedBatch)customBatch).customProps, batchProperties, batchDefaultsAccessor);
					}
					else
					{
						m_ManagedBatches.SetDefaults(customBatch.sourceMaterial, ((ManagedBatch)customBatch).customProps, batchProperties, batchDefaultsAccessor);
					}
					flag |= nativeBatchInstances.GetInstanceCount(i) != 0;
				}
			}
		}
		if (flag)
		{
			m_BatchDataSystem.InstancePropertiesUpdated();
			if (IsLodFadeEnabled())
			{
				JobHandle jobHandle = IJobParallelForExtensions.Schedule<InitializeLodFadeJob>(new InitializeLodFadeJob
				{
					m_NativeBatchInstances = nativeBatchInstances.AsParallelInstanceWriter()
				}, nativeBatchInstances.GetActiveGroupCount(), 1, default(JobHandle));
				AddNativeBatchInstancesWriter(jobHandle);
			}
		}
	}

	private void MergeGroups()
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = GetNativeBatchGroups(readOnly: false, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = GetNativeBatchInstances(readOnly: false, out dependencies2);
		JobHandle dependencies3;
		NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = GetNativeSubBatches(readOnly: false, out dependencies3);
		GroupUpdater<CullingData, GroupData, BatchData, InstanceData> val = nativeBatchGroups.BeginGroupUpdate((Allocator)3);
		InstanceUpdater<CullingData, GroupData, BatchData, InstanceData> val2 = nativeBatchInstances.BeginInstanceUpdate((Allocator)3);
		MergeGroupsJob mergeGroupsJob = new MergeGroupsJob
		{
			m_MeshBatches = InternalCompilerInterface.GetBufferLookup<MeshBatch>(ref __TypeHandle.__Game_Rendering_MeshBatch_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BatchGroups = InternalCompilerInterface.GetBufferLookup<BatchGroup>(ref __TypeHandle.__Game_Prefabs_BatchGroup_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MergeMeshes = m_MergeMeshes,
			m_MergeGroups = m_MergeGroups,
			m_BatchGroupUpdater = val.AsParallel(int.MaxValue),
			m_BatchInstanceUpdater = val2.AsParallel(int.MaxValue)
		};
		MergeCleanupJob obj = new MergeCleanupJob
		{
			m_MergeMeshes = m_MergeMeshes,
			m_MergeGroups = m_MergeGroups
		};
		JobHandle val3 = IJobParallelForExtensions.Schedule<MergeGroupsJob>(mergeGroupsJob, m_MergeMeshes.Length, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, dependencies2));
		JobHandle mergeDependencies = IJobExtensions.Schedule<MergeCleanupJob>(obj, val3);
		JobHandle jobHandle = nativeBatchGroups.EndGroupUpdate(val, val3);
		JobHandle jobHandle2 = nativeBatchInstances.EndInstanceUpdate(val2, JobHandle.CombineDependencies(val3, dependencies3), nativeSubBatches);
		AddNativeBatchGroupsWriter(jobHandle);
		AddNativeBatchInstancesWriter(jobHandle2);
		AddNativeSubBatchesWriter(jobHandle2);
		((SystemBase)this).Dependency = val3;
		m_MergeDependencies = mergeDependencies;
	}

	private JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext, BatchCullingOutput cullingOutput, IntPtr userContext)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Invalid comparison between Unknown and I4
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Invalid comparison between Unknown and I4
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = GetNativeBatchGroups(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = GetNativeBatchInstances(readOnly: true, out dependencies2);
		JobHandle dependencies3;
		NativeSubBatches<CullingData, GroupData, BatchData, InstanceData> nativeSubBatches = GetNativeSubBatches(readOnly: true, out dependencies3);
		((JobHandle)(ref dependencies2)).Complete();
		int activeGroupCount = nativeBatchInstances.GetActiveGroupCount();
		NativeArray<CullingSplit> cullingSplits = cullingContext.cullingSplits;
		int maxSplitBatchCount = (cullingSplits.Length << 1) - 1;
		NativeArray<ActiveGroupData> activeGroupData = default(NativeArray<ActiveGroupData>);
		activeGroupData._002Ector(activeGroupCount, (Allocator)3, (NativeArrayOptions)0);
		cullingSplits = cullingContext.cullingSplits;
		NativeList<CullingSplitData> splitData = default(NativeList<CullingSplitData>);
		splitData._002Ector(cullingSplits.Length, AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Plane> cullingPlanes = cullingContext.cullingPlanes;
		NativeList<FrustumPlanes.PlanePacket4> val = default(NativeList<FrustumPlanes.PlanePacket4>);
		val._002Ector(FrustumPlanes.GetPacketCount(cullingPlanes.Length), AllocatorHandle.op_Implicit((Allocator)3));
		BatchRenderFlags batchRenderFlags = BatchRenderFlags.IsEnabled;
		BatchRenderFlags batchRenderFlags2 = BatchRenderFlags.All;
		if ((int)cullingContext.viewType == 2)
		{
			batchRenderFlags |= BatchRenderFlags.CastShadows;
		}
		if (!IsMotionVectorsEnabled())
		{
			batchRenderFlags2 &= ~BatchRenderFlags.MotionVectors;
		}
		AllocateCullingJob allocateCullingJob = new AllocateCullingJob
		{
			m_NativeBatchGroups = nativeBatchGroups,
			m_NativeBatchInstances = nativeBatchInstances,
			m_RequiredFlagMask = batchRenderFlags,
			m_MaxSplitBatchCount = maxSplitBatchCount,
			m_CullingOutput = cullingOutput,
			m_ActiveGroupData = activeGroupData
		};
		int num;
		if ((int)cullingContext.projectionType == 2 && (int)cullingContext.viewType == 2)
		{
			cullingSplits = cullingContext.cullingSplits;
			num = ((cullingSplits.Length == 4) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag = (byte)num != 0;
		CullingPlanesJob cullingPlanesJob = new CullingPlanesJob
		{
			m_CullingPlanes = cullingContext.cullingPlanes,
			m_CullingSplits = cullingContext.cullingSplits,
			m_ShadowCullingData = (flag ? m_RenderingSystem.GetShadowCullingData() : float3.zero),
			m_SplitData = splitData,
			m_PlanePackets = val
		};
		BatchCullingJob obj = new BatchCullingJob
		{
			m_NativeBatchGroups = nativeBatchGroups,
			m_NativeBatchInstances = nativeBatchInstances,
			m_NativeSubBatches = nativeSubBatches,
			m_RequiredFlagMask = batchRenderFlags,
			m_RenderFlagMask = batchRenderFlags2,
			m_MaxSplitBatchCount = maxSplitBatchCount,
			m_IsShadowCulling = ((int)cullingContext.viewType == 2),
			m_ActiveGroupData = activeGroupData,
			m_SplitData = splitData,
			m_CullingPlanePackets = val,
			m_CullingOutput = cullingOutput
		};
		FinalizeCullingJob finalizeCullingJob = new FinalizeCullingJob
		{
			m_CullingOutput = cullingOutput
		};
		JobHandle val2 = IJobExtensions.Schedule<AllocateCullingJob>(allocateCullingJob, dependencies);
		JobHandle val3 = IJobExtensions.Schedule<CullingPlanesJob>(cullingPlanesJob, default(JobHandle));
		JobHandle val4 = IJobParallelForExtensions.Schedule<BatchCullingJob>(obj, activeGroupCount, 1, JobHandle.CombineDependencies(val2, val3, dependencies3));
		JobHandle result = IJobExtensions.Schedule<FinalizeCullingJob>(finalizeCullingJob, val4);
		splitData.Dispose(val4);
		val.Dispose(val4);
		AddNativeBatchInstancesReader(val4);
		AddNativeBatchGroupsReader(val4);
		AddNativeSubBatchesReader(val4);
		return result;
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
	public BatchManagerSystem()
	{
	}
}
