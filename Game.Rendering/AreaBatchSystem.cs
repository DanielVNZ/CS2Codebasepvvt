using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
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
public class AreaBatchSystem : GameSystemBase, IPreDeserialize
{
	private class ManagedBatchData
	{
		public GraphicsBuffer m_VisibleIndices;

		public Material m_Material;

		public int m_RendererPriority;
	}

	private struct NativeBatchData
	{
		public UnsafeList<AreaMetaData> m_AreaMetaData;

		public UnsafeList<int> m_VisibleIndices;

		public Bounds3 m_Bounds;

		public Entity m_Prefab;

		public bool m_VisibleUpdated;

		public bool m_BoundsUpdated;

		public bool m_VisibleIndicesUpdated;

		public bool m_IsEnabled;
	}

	[BurstCompile]
	private struct TreeCullingJob1 : IJob
	{
		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float4 m_PrevLodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_PrevCameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float3 m_PrevCameraDirection;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[ReadOnly]
		public BoundsMask m_PrevVisibleMask;

		public NativeArray<int> m_NodeBuffer;

		public NativeArray<int> m_SubDataBuffer;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			TreeCullingIterator treeCullingIterator = new TreeCullingIterator
			{
				m_LodParameters = m_LodParameters,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = m_CameraPosition,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = m_CameraDirection,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_VisibleMask = m_VisibleMask,
				m_PrevVisibleMask = m_PrevVisibleMask,
				m_ActionQueue = m_ActionQueue
			};
			m_AreaSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, 3, m_NodeBuffer, m_SubDataBuffer);
		}
	}

	[BurstCompile]
	private struct TreeCullingJob2 : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaSearchTree;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float4 m_PrevLodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_PrevCameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float3 m_PrevCameraDirection;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[ReadOnly]
		public BoundsMask m_PrevVisibleMask;

		[ReadOnly]
		public NativeArray<int> m_NodeBuffer;

		[ReadOnly]
		public NativeArray<int> m_SubDataBuffer;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute(int index)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			TreeCullingIterator treeCullingIterator = new TreeCullingIterator
			{
				m_LodParameters = m_LodParameters,
				m_PrevLodParameters = m_PrevLodParameters,
				m_CameraPosition = m_CameraPosition,
				m_PrevCameraPosition = m_PrevCameraPosition,
				m_CameraDirection = m_CameraDirection,
				m_PrevCameraDirection = m_PrevCameraDirection,
				m_VisibleMask = m_VisibleMask,
				m_PrevVisibleMask = m_PrevVisibleMask,
				m_ActionQueue = m_ActionQueue
			};
			m_AreaSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, m_SubDataBuffer[index], m_NodeBuffer[index]);
		}
	}

	private struct TreeCullingIterator : INativeQuadTreeIteratorWithSubData<AreaSearchItem, QuadTreeBoundsXZ, int>, IUnsafeQuadTreeIteratorWithSubData<AreaSearchItem, QuadTreeBoundsXZ, int>
	{
		public float4 m_LodParameters;

		public float3 m_CameraPosition;

		public float3 m_CameraDirection;

		public float3 m_PrevCameraPosition;

		public float4 m_PrevLodParameters;

		public float3 m_PrevCameraDirection;

		public BoundsMask m_VisibleMask;

		public BoundsMask m_PrevVisibleMask;

		public Writer<CullingAction> m_ActionQueue;

		public bool Intersect(QuadTreeBoundsXZ bounds, ref int subData)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			switch (subData)
			{
			case 1:
			{
				BoundsMask boundsMask4 = m_VisibleMask & bounds.m_Mask;
				float num13 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num14 = RenderingUtils.CalculateLod(num13 * num13, m_LodParameters);
				if (boundsMask4 == (BoundsMask)0 || num14 < bounds.m_MinLod)
				{
					return false;
				}
				float num15 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num16 = RenderingUtils.CalculateLod(num15 * num15, m_PrevLodParameters);
				if (((uint)boundsMask4 & (uint)(ushort)(~(int)m_PrevVisibleMask)) == 0)
				{
					if (num16 < bounds.m_MaxLod)
					{
						return num14 > num16;
					}
					return false;
				}
				return true;
			}
			case 2:
			{
				BoundsMask boundsMask3 = m_PrevVisibleMask & bounds.m_Mask;
				float num9 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num10 = RenderingUtils.CalculateLod(num9 * num9, m_PrevLodParameters);
				if (boundsMask3 == (BoundsMask)0 || num10 < bounds.m_MinLod)
				{
					return false;
				}
				float num11 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num12 = RenderingUtils.CalculateLod(num11 * num11, m_LodParameters);
				if (((uint)boundsMask3 & (uint)(ushort)(~(int)m_VisibleMask)) == 0)
				{
					if (num12 < bounds.m_MaxLod)
					{
						return num10 > num12;
					}
					return false;
				}
				return true;
			}
			default:
			{
				BoundsMask boundsMask = m_VisibleMask & bounds.m_Mask;
				BoundsMask boundsMask2 = m_PrevVisibleMask & bounds.m_Mask;
				float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				float num2 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num3 = RenderingUtils.CalculateLod(num * num, m_LodParameters);
				int num4 = RenderingUtils.CalculateLod(num2 * num2, m_PrevLodParameters);
				subData = 0;
				if (boundsMask != 0 && num3 >= bounds.m_MinLod)
				{
					float num5 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
					int num6 = RenderingUtils.CalculateLod(num5 * num5, m_PrevLodParameters);
					subData |= math.select(0, 1, ((uint)boundsMask & (uint)(ushort)(~(int)m_PrevVisibleMask)) != 0 || (num6 < bounds.m_MaxLod && num3 > num6));
				}
				if (boundsMask2 != 0 && num4 >= bounds.m_MinLod)
				{
					float num7 = RenderingUtils.CalculateMaxDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					int num8 = RenderingUtils.CalculateLod(num7 * num7, m_LodParameters);
					subData |= math.select(0, 2, ((uint)boundsMask2 & (uint)(ushort)(~(int)m_VisibleMask)) != 0 || (num8 < bounds.m_MaxLod && num4 > num8));
				}
				return subData != 0;
			}
			}
		}

		public void Iterate(QuadTreeBoundsXZ bounds, int subData, AreaSearchItem item)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			switch (subData)
			{
			case 1:
			{
				float num5 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num6 = RenderingUtils.CalculateLod(num5 * num5, m_LodParameters);
				if ((m_VisibleMask & bounds.m_Mask) != 0 && num6 >= bounds.m_MinLod)
				{
					float num7 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
					int num8 = RenderingUtils.CalculateLod(num7 * num7, m_PrevLodParameters);
					if ((m_PrevVisibleMask & bounds.m_Mask) == 0 || num8 < bounds.m_MaxLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Item = item,
							m_PassedCulling = true
						});
					}
				}
				break;
			}
			case 2:
			{
				float num9 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num10 = RenderingUtils.CalculateLod(num9 * num9, m_PrevLodParameters);
				if ((m_PrevVisibleMask & bounds.m_Mask) != 0 && num10 >= bounds.m_MinLod)
				{
					float num11 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					int num12 = RenderingUtils.CalculateLod(num11 * num11, m_LodParameters);
					if ((m_VisibleMask & bounds.m_Mask) == 0 || num12 < bounds.m_MaxLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Item = item
						});
					}
				}
				break;
			}
			default:
			{
				float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				float num2 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num3 = RenderingUtils.CalculateLod(num * num, m_LodParameters);
				int num4 = RenderingUtils.CalculateLod(num2 * num2, m_PrevLodParameters);
				bool flag = (m_VisibleMask & bounds.m_Mask) != 0 && num3 >= bounds.m_MinLod;
				bool flag2 = (m_PrevVisibleMask & bounds.m_Mask) != 0 && num4 >= bounds.m_MaxLod;
				if (flag != flag2)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Item = item,
						m_PassedCulling = flag
					});
				}
				break;
			}
			}
		}
	}

	[BurstCompile]
	private struct QueryCullingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Batch> m_BatchType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public BufferTypeHandle<Triangle> m_TriangleType;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> m_PrefabAreaGeometryData;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Batch> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Batch>(ref m_BatchType);
			if (((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType))
			{
				for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
				{
					Entity area = nativeArray[i];
					if (nativeArray2[i].m_AllocatedSize != 0)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Item = new AreaSearchItem(area, -1)
						});
					}
				}
				return;
			}
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Node> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Node>(ref m_NodeType);
			BufferAccessor<Triangle> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Triangle>(ref m_TriangleType);
			BoundsMask boundsMask = BoundsMask.Debug | BoundsMask.NormalLayers | BoundsMask.NotOverridden | BoundsMask.NotWalkThrough;
			for (int j = 0; j < ((ArchetypeChunk)(ref chunk)).Count; j++)
			{
				Entity area2 = nativeArray[j];
				if (nativeArray2[j].m_AllocatedSize != 0)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Item = new AreaSearchItem(area2, -1)
					});
				}
				if ((m_VisibleMask & boundsMask) == 0)
				{
					continue;
				}
				PrefabRef prefabRef = nativeArray3[j];
				DynamicBuffer<Node> nodes = bufferAccessor[j];
				DynamicBuffer<Triangle> val = bufferAccessor2[j];
				AreaGeometryData areaData = m_PrefabAreaGeometryData[prefabRef.m_Prefab];
				for (int k = 0; k < val.Length; k++)
				{
					Triangle triangle = val[k];
					Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, triangle);
					float num = RenderingUtils.CalculateMinDistance(AreaUtils.GetBounds(triangle, triangle2, areaData), m_CameraPosition, m_CameraDirection, m_LodParameters);
					if (RenderingUtils.CalculateLod(num * num, m_LodParameters) >= triangle.m_MinLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Item = new AreaSearchItem(area2, k),
							m_PassedCulling = true
						});
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct AreaMetaData
	{
		public Entity m_Entity;

		public Bounds3 m_Bounds;

		public int m_StartIndex;

		public int m_VisibleCount;
	}

	private struct TriangleMetaData
	{
		public int m_Index;

		public bool m_IsVisible;
	}

	private struct TriangleSortData : IComparable<TriangleSortData>
	{
		public int m_Index;

		public int m_MinLod;

		public int CompareTo(TriangleSortData other)
		{
			return m_MinLod - other.m_MinLod;
		}
	}

	private struct CullingAction
	{
		public AreaSearchItem m_Item;

		public bool m_PassedCulling;

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Item.m_Area)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct AllocationAction
	{
		public Entity m_Entity;

		public int m_TriangleCount;
	}

	[BurstCompile]
	private struct CullingActionJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RenderedAreaData> m_PrefabRenderedAreaData;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public Reader<CullingAction> m_CullingActions;

		public ParallelWriter<AllocationAction> m_AllocationActions;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Batch> m_BatchData;

		[NativeDisableParallelForRestriction]
		public NativeList<TriangleMetaData> m_TriangleMetaData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<CullingAction> enumerator = m_CullingActions.GetEnumerator(index);
			while (enumerator.MoveNext())
			{
				CullingAction current = enumerator.Current;
				if (current.m_PassedCulling)
				{
					PassedCulling(current);
				}
				else
				{
					FailedCulling(current);
				}
			}
			enumerator.Dispose();
		}

		private void PassedCulling(CullingAction cullingAction)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			ref Batch valueRW = ref m_BatchData.GetRefRW(cullingAction.m_Item.m_Area).ValueRW;
			if (valueRW.m_VisibleCount == 0)
			{
				valueRW.m_VisibleCount = -1;
				PrefabRef prefabRef = m_PrefabRefData[cullingAction.m_Item.m_Area];
				RenderedAreaData renderedAreaData = default(RenderedAreaData);
				if (m_PrefabRenderedAreaData.TryGetComponent(prefabRef.m_Prefab, ref renderedAreaData))
				{
					valueRW.m_BatchIndex = renderedAreaData.m_BatchIndex;
					m_AllocationActions.Enqueue(new AllocationAction
					{
						m_Entity = cullingAction.m_Item.m_Area,
						m_TriangleCount = m_Triangles[cullingAction.m_Item.m_Area].Length
					});
				}
				else
				{
					valueRW.m_BatchIndex = -1;
				}
			}
		}

		private void FailedCulling(CullingAction cullingAction)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			ref Batch valueRW = ref m_BatchData.GetRefRW(cullingAction.m_Item.m_Area).ValueRW;
			if (valueRW.m_AllocatedSize == 0)
			{
				return;
			}
			if (cullingAction.m_Item.m_Triangle == -1)
			{
				if (valueRW.m_VisibleCount > 0)
				{
					for (int i = 0; i < valueRW.m_AllocatedSize; i++)
					{
						m_TriangleMetaData.ElementAt((int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin + i).m_IsVisible = false;
					}
					valueRW.m_VisibleCount = 0;
					m_AllocationActions.Enqueue(new AllocationAction
					{
						m_Entity = cullingAction.m_Item.m_Area
					});
				}
				return;
			}
			ref TriangleMetaData reference = ref m_TriangleMetaData.ElementAt((int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin + cullingAction.m_Item.m_Triangle);
			if (reference.m_IsVisible)
			{
				reference.m_IsVisible = false;
				if (--valueRW.m_VisibleCount == 0)
				{
					m_AllocationActions.Enqueue(new AllocationAction
					{
						m_Entity = cullingAction.m_Item.m_Area
					});
				}
			}
		}
	}

	[BurstCompile]
	private struct BatchAllocationJob : IJob
	{
		public ComponentLookup<Batch> m_BatchData;

		public NativeList<NativeBatchData> m_NativeBatchData;

		public NativeList<TriangleMetaData> m_TriangleMetaData;

		public NativeList<AreaTriangleData> m_AreaTriangleData;

		public NativeList<AreaColorData> m_AreaColorData;

		public NativeList<NativeHeapBlock> m_UpdatedTriangles;

		public NativeQueue<AllocationAction> m_AllocationActions;

		public NativeHeapAllocator m_AreaBufferAllocator;

		public NativeReference<int> m_AllocationCount;

		public void Execute()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			AllocationAction allocationAction = default(AllocationAction);
			while (m_AllocationActions.TryDequeue(ref allocationAction))
			{
				RefRW<Batch> refRW = m_BatchData.GetRefRW(allocationAction.m_Entity);
				ref Batch valueRW = ref refRW.ValueRW;
				ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(valueRW.m_BatchIndex);
				reference.m_BoundsUpdated = true;
				if (allocationAction.m_TriangleCount != 0)
				{
					if (valueRW.m_AllocatedSize == 0)
					{
						Allocate(ref valueRW, allocationAction.m_TriangleCount);
						valueRW.m_MetaIndex = reference.m_AreaMetaData.Length;
						ref UnsafeList<AreaMetaData> reference2 = ref reference.m_AreaMetaData;
						AreaMetaData areaMetaData = new AreaMetaData
						{
							m_Entity = allocationAction.m_Entity,
							m_StartIndex = (int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin
						};
						reference2.Add(ref areaMetaData);
						ref NativeReference<int> reference3 = ref m_AllocationCount;
						int value = reference3.Value;
						reference3.Value = value + 1;
					}
					else
					{
						ref AreaMetaData reference4 = ref reference.m_AreaMetaData.ElementAt(valueRW.m_MetaIndex);
						reference4.m_VisibleCount = 0;
						if (allocationAction.m_TriangleCount != valueRW.m_AllocatedSize)
						{
							((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Release(valueRW.m_BatchAllocation);
							Allocate(ref valueRW, allocationAction.m_TriangleCount);
							reference4.m_StartIndex = (int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin;
						}
					}
					m_UpdatedTriangles.Add(ref valueRW.m_BatchAllocation);
				}
				else if (valueRW.m_VisibleCount == 0)
				{
					ref NativeReference<int> reference5 = ref m_AllocationCount;
					int value = reference5.Value;
					reference5.Value = value - 1;
					((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Release(valueRW.m_BatchAllocation);
					valueRW.m_BatchAllocation = default(NativeHeapBlock);
					valueRW.m_AllocatedSize = 0;
					reference.m_AreaMetaData.RemoveAtSwapBack(valueRW.m_MetaIndex);
					reference.m_VisibleUpdated = true;
					if (valueRW.m_MetaIndex < reference.m_AreaMetaData.Length)
					{
						refRW = m_BatchData.GetRefRW(reference.m_AreaMetaData[valueRW.m_MetaIndex].m_Entity);
						refRW.ValueRW.m_MetaIndex = valueRW.m_MetaIndex;
					}
				}
			}
		}

		private void Allocate(ref Batch batch, int allocationSize)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			batch.m_BatchAllocation = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Allocate((uint)allocationSize, 1u);
			batch.m_AllocatedSize = allocationSize;
			if (((NativeHeapBlock)(ref batch.m_BatchAllocation)).Empty)
			{
				((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Resize(((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size + 1048576 / GetTriangleSize());
				m_TriangleMetaData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
				m_AreaTriangleData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
				m_AreaColorData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
				batch.m_BatchAllocation = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Allocate((uint)allocationSize, 1u);
			}
		}
	}

	[BurstCompile]
	private struct TriangleUpdateJob : IJobParallelFor
	{
		private struct Border : IEquatable<Border>
		{
			public float3 m_StartPos;

			public float3 m_EndPos;

			public bool Equals(Border other)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				return ((float3)(ref m_StartPos)).Equals(other.m_StartPos) & ((float3)(ref m_EndPos)).Equals(other.m_EndPos);
			}

			public override int GetHashCode()
			{
				return ((object)System.Runtime.CompilerServices.Unsafe.As<float3, float3>(ref m_StartPos)/*cast due to .constrained prefix*/).GetHashCode();
			}
		}

		[ReadOnly]
		public ComponentLookup<Area> m_AreaData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<RenderedAreaData> m_PrefabRenderedAreaData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[ReadOnly]
		public BufferLookup<Expand> m_Expands;

		[ReadOnly]
		public Reader<CullingAction> m_CullingActions;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Batch> m_BatchData;

		[NativeDisableParallelForRestriction]
		public NativeList<TriangleMetaData> m_TriangleMetaData;

		[NativeDisableParallelForRestriction]
		public NativeList<AreaTriangleData> m_AreaTriangleData;

		[NativeDisableParallelForRestriction]
		public NativeList<NativeBatchData> m_NativeBatchData;

		[NativeDisableContainerSafetyRestriction]
		private NativeParallelHashMap<Border, int2> m_BorderMap;

		[NativeDisableContainerSafetyRestriction]
		private NativeList<int2> m_AdjacentNodes;

		[NativeDisableContainerSafetyRestriction]
		private NativeList<Node> m_NodeList;

		[NativeDisableContainerSafetyRestriction]
		private NativeList<TriangleSortData> m_TriangleSortData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<CullingAction> enumerator = m_CullingActions.GetEnumerator(index);
			while (enumerator.MoveNext())
			{
				CullingAction current = enumerator.Current;
				if (current.m_PassedCulling)
				{
					PassedCulling(current);
				}
				else
				{
					FailedCulling(current);
				}
			}
			enumerator.Dispose();
		}

		private void PassedCulling(CullingAction cullingAction)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			ref Batch valueRW = ref m_BatchData.GetRefRW(cullingAction.m_Item.m_Area).ValueRW;
			if (valueRW.m_AllocatedSize == 0)
			{
				return;
			}
			if (valueRW.m_VisibleCount == -1)
			{
				GenerateTriangleData(cullingAction.m_Item.m_Area, ref valueRW);
				valueRW.m_VisibleCount = 0;
			}
			ref TriangleMetaData reference = ref m_TriangleMetaData.ElementAt((int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin + cullingAction.m_Item.m_Triangle);
			if (reference.m_IsVisible)
			{
				return;
			}
			reference.m_IsVisible = true;
			valueRW.m_VisibleCount++;
			ref NativeBatchData reference2 = ref m_NativeBatchData.ElementAt(valueRW.m_BatchIndex);
			ref AreaMetaData reference3 = ref reference2.m_AreaMetaData.ElementAt(valueRW.m_MetaIndex);
			if (reference.m_Index >= reference3.m_VisibleCount)
			{
				reference3.m_VisibleCount = reference.m_Index + 1;
				if (!reference2.m_VisibleUpdated)
				{
					reference2.m_VisibleUpdated = true;
				}
			}
		}

		private void FailedCulling(CullingAction cullingAction)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			ref Batch valueRW = ref m_BatchData.GetRefRW(cullingAction.m_Item.m_Area).ValueRW;
			if (valueRW.m_AllocatedSize == 0 || cullingAction.m_Item.m_Triangle == -1)
			{
				return;
			}
			ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(valueRW.m_BatchIndex);
			ref AreaMetaData reference2 = ref reference.m_AreaMetaData.ElementAt(valueRW.m_MetaIndex);
			if (m_TriangleMetaData[(int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin + cullingAction.m_Item.m_Triangle].m_Index == reference2.m_VisibleCount - 1)
			{
				reference2.m_VisibleCount = 0;
				for (int i = 0; i < valueRW.m_AllocatedSize; i++)
				{
					TriangleMetaData triangleMetaData = m_TriangleMetaData[(int)((NativeHeapBlock)(ref valueRW.m_BatchAllocation)).Begin + i];
					reference2.m_VisibleCount = math.select(reference2.m_VisibleCount, triangleMetaData.m_Index + 1, triangleMetaData.m_IsVisible && triangleMetaData.m_Index >= reference2.m_VisibleCount);
				}
				if (!reference.m_VisibleUpdated)
				{
					reference.m_VisibleUpdated = true;
				}
			}
		}

		private void GenerateTriangleData(Entity entity, ref Batch batch)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			Area area = m_AreaData[entity];
			DynamicBuffer<Node> nodes = m_Nodes[entity];
			DynamicBuffer<Triangle> triangles = m_Triangles[entity];
			PrefabRef prefabRef = m_PrefabRefData[entity];
			RenderedAreaData renderedAreaData = m_PrefabRenderedAreaData[prefabRef.m_Prefab];
			float4 offsetDir = default(float4);
			((float4)(ref offsetDir))._002Ector(0f, 0f, 0f, 1f);
			bool flag = false;
			Owner owner = default(Owner);
			if (m_OwnerData.TryGetComponent(entity, ref owner))
			{
				Transform transform = default(Transform);
				Owner owner2 = default(Owner);
				while (true)
				{
					if (m_TransformData.TryGetComponent(owner.m_Owner, ref transform))
					{
						((float4)(ref offsetDir)).xy = ((float3)(ref transform.m_Position)).xz;
						float3 val = math.forward(transform.m_Rotation);
						((float4)(ref offsetDir)).zw = ((float3)(ref val)).xz;
						flag = true;
						break;
					}
					if (!m_OwnerData.TryGetComponent(owner.m_Owner, ref owner2))
					{
						break;
					}
					owner = owner2;
				}
			}
			if (!flag)
			{
				Node node;
				if (nodes.Length >= 1)
				{
					node = nodes[0];
					((float4)(ref offsetDir)).xy = ((float3)(ref node.m_Position)).xz;
				}
				if (nodes.Length >= 2)
				{
					node = nodes[1];
					float2 val2 = ((float3)(ref node.m_Position)).xz - ((float4)(ref offsetDir)).xy;
					if (MathUtils.TryNormalize(ref val2))
					{
						((float4)(ref offsetDir)).zw = MathUtils.Left(val2);
					}
				}
			}
			ref AreaMetaData reference = ref m_NativeBatchData.ElementAt(batch.m_BatchIndex).m_AreaMetaData.ElementAt(batch.m_MetaIndex);
			bool isCounterClockwise = (area.m_Flags & AreaFlags.CounterClockwise) != 0;
			if (!m_BorderMap.IsCreated)
			{
				m_BorderMap = new NativeParallelHashMap<Border, int2>(nodes.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			if (!m_AdjacentNodes.IsCreated)
			{
				m_AdjacentNodes = new NativeList<int2>(nodes.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			if (!m_TriangleSortData.IsCreated)
			{
				m_TriangleSortData = new NativeList<TriangleSortData>(triangles.Length, AllocatorHandle.op_Implicit((Allocator)2));
			}
			SortTriangles(triangles, ref batch);
			DynamicBuffer<Expand> expands = default(DynamicBuffer<Expand>);
			if (m_Expands.TryGetBuffer(entity, ref expands))
			{
				if (!m_NodeList.IsCreated)
				{
					m_NodeList = new NativeList<Node>(nodes.Length, AllocatorHandle.op_Implicit((Allocator)2));
				}
				FillExpandedNodes(nodes, expands);
				AddBorders<NativeList<Node>>(m_NodeList, isCounterClockwise);
				AddNodes<NativeList<Node>>(m_NodeList, triangles, isCounterClockwise);
				reference.m_Bounds = AddTriangles<NativeList<Node>>(m_NodeList, triangles, renderedAreaData, (int)((NativeHeapBlock)(ref batch.m_BatchAllocation)).Begin, offsetDir, isCounterClockwise);
			}
			else
			{
				AddBorders<DynamicBuffer<Node>>(nodes, isCounterClockwise);
				AddNodes<DynamicBuffer<Node>>(nodes, triangles, isCounterClockwise);
				reference.m_Bounds = AddTriangles<DynamicBuffer<Node>>(nodes, triangles, renderedAreaData, (int)((NativeHeapBlock)(ref batch.m_BatchAllocation)).Begin, offsetDir, isCounterClockwise);
			}
		}

		private void SortTriangles(DynamicBuffer<Triangle> triangles, ref Batch batch)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			m_TriangleSortData.ResizeUninitialized(triangles.Length);
			for (int i = 0; i < m_TriangleSortData.Length; i++)
			{
				m_TriangleSortData[i] = new TriangleSortData
				{
					m_Index = i,
					m_MinLod = triangles[i].m_MinLod
				};
			}
			NativeSortExtension.Sort<TriangleSortData>(m_TriangleSortData);
			for (int j = 0; j < m_TriangleSortData.Length; j++)
			{
				TriangleSortData triangleSortData = m_TriangleSortData[j];
				m_TriangleMetaData[(int)((NativeHeapBlock)(ref batch.m_BatchAllocation)).Begin + triangleSortData.m_Index] = new TriangleMetaData
				{
					m_Index = j
				};
			}
		}

		private void FillExpandedNodes(DynamicBuffer<Node> nodes, DynamicBuffer<Expand> expands)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			m_NodeList.ResizeUninitialized(nodes.Length);
			for (int i = 0; i < nodes.Length; i++)
			{
				Node node = nodes[i];
				Expand expand = expands[i];
				ref float3 position = ref node.m_Position;
				((float3)(ref position)).xz = ((float3)(ref position)).xz + expand.m_Offset;
				m_NodeList[i] = node;
			}
		}

		private void AddBorders<TNodeList>(TNodeList nodes, bool isCounterClockwise) where TNodeList : INativeList<Node>
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			m_BorderMap.Clear();
			float3 val = ((INativeList<Node>)nodes)[0].m_Position;
			for (int i = 1; i < ((IIndexable<Node>)nodes).Length; i++)
			{
				float3 position = ((INativeList<Node>)nodes)[i].m_Position;
				if (isCounterClockwise)
				{
					m_BorderMap.Add(new Border
					{
						m_StartPos = position,
						m_EndPos = val
					}, new int2(i, i - 1));
				}
				else
				{
					m_BorderMap.Add(new Border
					{
						m_StartPos = val,
						m_EndPos = position
					}, new int2(i - 1, i));
				}
				val = position;
			}
			float3 position2 = ((INativeList<Node>)nodes)[0].m_Position;
			if (isCounterClockwise)
			{
				m_BorderMap.Add(new Border
				{
					m_StartPos = position2,
					m_EndPos = val
				}, new int2(0, ((IIndexable<Node>)nodes).Length - 1));
			}
			else
			{
				m_BorderMap.Add(new Border
				{
					m_StartPos = val,
					m_EndPos = position2
				}, new int2(((IIndexable<Node>)nodes).Length - 1, 0));
			}
		}

		private void AddNodes<TNodeList>(TNodeList nodes, DynamicBuffer<Triangle> triangles, bool isCounterClockwise) where TNodeList : INativeList<Node>
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			m_AdjacentNodes.ResizeUninitialized(((IIndexable<Node>)nodes).Length);
			for (int i = 0; i < m_AdjacentNodes.Length; i++)
			{
				m_AdjacentNodes[i] = int2.op_Implicit(i);
			}
			for (int j = 0; j < triangles.Length; j++)
			{
				Triangle triangle = triangles[j];
				int2 adjacentA = m_AdjacentNodes[triangle.m_Indices.x];
				int2 adjacentB = m_AdjacentNodes[triangle.m_Indices.y];
				int2 adjacentB2 = m_AdjacentNodes[triangle.m_Indices.z];
				CheckBorder(ref adjacentA, ref adjacentB, nodes, triangle.m_Indices.x, triangle.m_Indices.y, isCounterClockwise);
				CheckBorder(ref adjacentB, ref adjacentB2, nodes, triangle.m_Indices.y, triangle.m_Indices.z, isCounterClockwise);
				CheckBorder(ref adjacentB2, ref adjacentA, nodes, triangle.m_Indices.z, triangle.m_Indices.x, isCounterClockwise);
				m_AdjacentNodes[triangle.m_Indices.x] = adjacentA;
				m_AdjacentNodes[triangle.m_Indices.y] = adjacentB;
				m_AdjacentNodes[triangle.m_Indices.z] = adjacentB2;
			}
			for (int k = 0; k < m_AdjacentNodes.Length; k++)
			{
				int2 val = m_AdjacentNodes[k];
				bool2 val2 = val != k;
				if (!math.any(val2))
				{
					continue;
				}
				if (val2.x)
				{
					for (int l = 0; l < ((IIndexable<Node>)nodes).Length; l++)
					{
						int x = m_AdjacentNodes[val.x].x;
						if (x == val.x)
						{
							break;
						}
						if (x == k || x == -1)
						{
							val.x = -1;
							break;
						}
						val.x = x;
					}
				}
				if (val2.y)
				{
					for (int m = 0; m < ((IIndexable<Node>)nodes).Length; m++)
					{
						int y = m_AdjacentNodes[val.y].y;
						if (y == val.y)
						{
							break;
						}
						if (y == k || y == -1)
						{
							val.y = -1;
							break;
						}
						val.y = y;
					}
				}
				m_AdjacentNodes[k] = val;
			}
			for (int n = 0; n < m_AdjacentNodes.Length; n++)
			{
				int2 val3 = m_AdjacentNodes[n];
				m_AdjacentNodes[n] = math.select(math.select(val3 + new int2(-1, 1), new int2(((IIndexable<Node>)nodes).Length - 1, 0), val3 == new int2(0, ((IIndexable<Node>)nodes).Length - 1)), int2.op_Implicit(n), val3 == -1);
			}
		}

		private void CheckBorder<TNodeList>(ref int2 adjacentA, ref int2 adjacentB, TNodeList nodes, int nodeA, int nodeB, bool isCounterClockwise) where TNodeList : INativeList<Node>
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Border border = new Border
			{
				m_StartPos = ((INativeList<Node>)nodes)[nodeA].m_Position,
				m_EndPos = ((INativeList<Node>)nodes)[nodeB].m_Position
			};
			int2 val = default(int2);
			if (m_BorderMap.TryGetValue(border, ref val))
			{
				if (isCounterClockwise)
				{
					adjacentB.x = val.y;
					adjacentA.y = val.x;
				}
				else
				{
					adjacentA.x = val.x;
					adjacentB.y = val.y;
				}
			}
		}

		private Bounds3 AddTriangles<TNodeList>(TNodeList nodes, DynamicBuffer<Triangle> triangles, RenderedAreaData renderedAreaData, int triangleOffset, float4 offsetDir, bool isCounterClockwise) where TNodeList : INativeList<Node>
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			Bounds3 val = default(Bounds3);
			((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
			float3 val5 = default(float3);
			for (int i = 0; i < triangles.Length; i++)
			{
				Triangle triangle = triangles[i];
				int2 val2 = m_AdjacentNodes[triangle.m_Indices.x];
				int2 val3 = m_AdjacentNodes[triangle.m_Indices.y];
				int2 val4 = m_AdjacentNodes[triangle.m_Indices.z];
				int x = m_AdjacentNodes[val2.x].x;
				int x2 = m_AdjacentNodes[val3.x].x;
				int x3 = m_AdjacentNodes[val4.x].x;
				int y = m_AdjacentNodes[val2.y].y;
				int y2 = m_AdjacentNodes[val3.y].y;
				int y3 = m_AdjacentNodes[val4.y].y;
				AreaTriangleData areaTriangleData = new AreaTriangleData
				{
					m_APos = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.x, val2.x, val2.y, renderedAreaData.m_ExpandAmount, isCounterClockwise),
					m_BPos = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.y, val3.x, val3.y, renderedAreaData.m_ExpandAmount, isCounterClockwise),
					m_CPos = AreaUtils.GetExpandedNode(nodes, triangle.m_Indices.z, val4.x, val4.y, renderedAreaData.m_ExpandAmount, isCounterClockwise)
				};
				float3 expandedNode = AreaUtils.GetExpandedNode(nodes, val2.x, x, triangle.m_Indices.x, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_APrevXZ = ((float3)(ref expandedNode)).xz;
				expandedNode = AreaUtils.GetExpandedNode(nodes, val3.x, x2, triangle.m_Indices.y, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_BPrevXZ = ((float3)(ref expandedNode)).xz;
				expandedNode = AreaUtils.GetExpandedNode(nodes, val4.x, x3, triangle.m_Indices.z, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_CPrevXZ = ((float3)(ref expandedNode)).xz;
				expandedNode = AreaUtils.GetExpandedNode(nodes, val2.y, triangle.m_Indices.x, y, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_ANextXZ = ((float3)(ref expandedNode)).xz;
				expandedNode = AreaUtils.GetExpandedNode(nodes, val3.y, triangle.m_Indices.y, y2, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_BNextXZ = ((float3)(ref expandedNode)).xz;
				expandedNode = AreaUtils.GetExpandedNode(nodes, val4.y, triangle.m_Indices.z, y3, renderedAreaData.m_ExpandAmount, isCounterClockwise);
				areaTriangleData.m_CNextXZ = ((float3)(ref expandedNode)).xz;
				((float3)(ref val5))._002Ector(areaTriangleData.m_APos.y, areaTriangleData.m_BPos.y, areaTriangleData.m_CPos.y);
				areaTriangleData.m_YMinMax.x = triangle.m_HeightRange.min - renderedAreaData.m_HeightOffset + math.cmin(val5);
				areaTriangleData.m_YMinMax.y = triangle.m_HeightRange.max + renderedAreaData.m_HeightOffset + math.cmax(val5);
				areaTriangleData.m_OffsetDir = offsetDir;
				areaTriangleData.m_LodDistanceFactor = RenderingUtils.CalculateDistanceFactor(triangle.m_MinLod);
				Bounds3 val6 = MathUtils.Bounds(new Triangle3(areaTriangleData.m_APos, areaTriangleData.m_BPos, areaTriangleData.m_CPos));
				val6.min.y = areaTriangleData.m_YMinMax.x;
				val6.max.y = areaTriangleData.m_YMinMax.y;
				val |= val6;
				ref TriangleMetaData reference = ref m_TriangleMetaData.ElementAt(triangleOffset + i);
				m_AreaTriangleData[triangleOffset + reference.m_Index] = areaTriangleData;
			}
			return val;
		}
	}

	[BurstCompile]
	private struct VisibleUpdateJob : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public NativeList<NativeBatchData> m_NativeBatchData;

		public void Execute(int index)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(index);
			if (reference.m_BoundsUpdated)
			{
				reference.m_Bounds = new Bounds3(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
				reference.m_BoundsUpdated = false;
				for (int i = 0; i < reference.m_AreaMetaData.Length; i++)
				{
					ref AreaMetaData reference2 = ref reference.m_AreaMetaData.ElementAt(i);
					ref Bounds3 reference3 = ref reference.m_Bounds;
					reference3 |= reference2.m_Bounds;
				}
			}
			if (!reference.m_VisibleUpdated)
			{
				return;
			}
			reference.m_VisibleIndices.Clear();
			reference.m_VisibleIndicesUpdated = true;
			reference.m_VisibleUpdated = false;
			if (!reference.m_IsEnabled)
			{
				return;
			}
			for (int j = 0; j < reference.m_AreaMetaData.Length; j++)
			{
				ref AreaMetaData reference4 = ref reference.m_AreaMetaData.ElementAt(j);
				ref Bounds3 reference5 = ref reference.m_Bounds;
				reference5 |= float3.op_Implicit(reference4.m_StartIndex);
				for (int k = 0; k < reference4.m_VisibleCount; k++)
				{
					ref UnsafeList<int> reference6 = ref reference.m_VisibleIndices;
					int num = reference4.m_StartIndex + k;
					reference6.Add(ref num);
				}
			}
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

		[ReadOnly]
		public ComponentTypeHandle<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<AreaGeometryData>(true);
		}
	}

	public const uint AREABUFFER_MEMORY_DEFAULT = 4194304u;

	public const uint AREABUFFER_MEMORY_INCREMENT = 1048576u;

	private PrefabSystem m_PrefabSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private RenderingSystem m_RenderingSystem;

	private BatchDataSystem m_BatchDataSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private ComputeBuffer m_AreaTriangleBuffer;

	private ComputeBuffer m_AreaColorBuffer;

	private List<ManagedBatchData> m_ManagedBatchData;

	private NativeHeapAllocator m_AreaBufferAllocator;

	private NativeReference<int> m_AllocationCount;

	private NativeList<NativeBatchData> m_NativeBatchData;

	private NativeList<AreaTriangleData> m_AreaTriangleData;

	private NativeList<TriangleMetaData> m_TriangleMetaData;

	private NativeList<AreaColorData> m_AreaColorData;

	private NativeList<NativeHeapBlock> m_UpdatedTriangles;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_PrefabQuery;

	private JobHandle m_DataDependencies;

	private float3 m_PrevCameraPosition;

	private float3 m_PrevCameraDirection;

	private float4 m_PrevLodParameters;

	private int m_AreaParameters;

	private int m_DecalLayerMask;

	private bool m_Loaded;

	private bool m_ColorsUpdated;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Batch>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<RenderedAreaData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array2[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_ManagedBatchData = new List<ManagedBatchData>();
		m_AreaBufferAllocator = new NativeHeapAllocator(4194304 / GetTriangleSize(), 1u, (Allocator)4);
		m_AllocationCount = new NativeReference<int>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_NativeBatchData = new NativeList<NativeBatchData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_AreaTriangleData = new NativeList<AreaTriangleData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_TriangleMetaData = new NativeList<TriangleMetaData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_AreaColorData = new NativeList<AreaColorData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_UpdatedTriangles = new NativeList<NativeHeapBlock>(100, AllocatorHandle.op_Implicit((Allocator)4));
		m_AreaTriangleData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
		m_TriangleMetaData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
		m_AreaColorData.ResizeUninitialized((int)((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size);
		m_AreaTriangleBuffer = new ComputeBuffer(m_AreaTriangleData.Capacity, System.Runtime.CompilerServices.Unsafe.SizeOf<AreaTriangleData>());
		m_AreaColorBuffer = new ComputeBuffer(m_AreaColorData.Capacity, System.Runtime.CompilerServices.Unsafe.SizeOf<AreaColorData>());
		m_AreaParameters = Shader.PropertyToID("colossal_AreaParameters");
		m_DecalLayerMask = Shader.PropertyToID("colossal_DecalLayerMask");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_AreaTriangleBuffer.Release();
		m_AreaColorBuffer.Release();
		for (int i = 0; i < m_ManagedBatchData.Count; i++)
		{
			ManagedBatchData managedBatchData = m_ManagedBatchData[i];
			if ((Object)(object)managedBatchData.m_Material != (Object)null)
			{
				Object.Destroy((Object)(object)managedBatchData.m_Material);
			}
			if (managedBatchData.m_VisibleIndices != null)
			{
				managedBatchData.m_VisibleIndices.Release();
			}
		}
		((JobHandle)(ref m_DataDependencies)).Complete();
		for (int j = 0; j < m_NativeBatchData.Length; j++)
		{
			ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(j);
			if (reference.m_AreaMetaData.IsCreated)
			{
				reference.m_AreaMetaData.Dispose();
			}
			if (reference.m_VisibleIndices.IsCreated)
			{
				reference.m_VisibleIndices.Dispose();
			}
		}
		((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Dispose();
		m_AllocationCount.Dispose();
		m_NativeBatchData.Dispose();
		m_AreaTriangleData.Dispose();
		m_TriangleMetaData.Dispose();
		m_AreaColorData.Dispose();
		m_UpdatedTriangles.Dispose();
		base.OnDestroy();
	}

	public void PreDeserialize(Context context)
	{
		((JobHandle)(ref m_DataDependencies)).Complete();
		m_AllocationCount.Value = 0;
		((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Clear();
		m_UpdatedTriangles.Clear();
		for (int i = 0; i < m_NativeBatchData.Length; i++)
		{
			ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(i);
			if (reference.m_AreaMetaData.IsCreated)
			{
				reference.m_AreaMetaData.Clear();
			}
			if (reference.m_VisibleIndices.IsCreated)
			{
				reference.m_VisibleIndices.Clear();
			}
		}
		m_Loaded = true;
	}

	public int GetBatchCount()
	{
		return m_ManagedBatchData.Count;
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

	public unsafe bool GetAreaBatch(int index, out ComputeBuffer buffer, out ComputeBuffer colors, out GraphicsBuffer indices, out Material material, out Bounds bounds, out int count, out int rendererPriority)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_DataDependencies)).Complete();
		ManagedBatchData managedBatchData = m_ManagedBatchData[index];
		ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(index);
		if (m_AreaTriangleBuffer.count != m_AreaTriangleData.Capacity)
		{
			m_AreaTriangleBuffer.Release();
			m_AreaTriangleBuffer = new ComputeBuffer(m_AreaTriangleData.Capacity, System.Runtime.CompilerServices.Unsafe.SizeOf<AreaTriangleData>());
			m_UpdatedTriangles.Clear();
			uint onePastHighestUsedAddress = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).OnePastHighestUsedAddress;
			if (onePastHighestUsedAddress != 0)
			{
				ref NativeList<NativeHeapBlock> reference2 = ref m_UpdatedTriangles;
				NativeHeapBlock val = new NativeHeapBlock(new UnsafeHeapBlock(0u, onePastHighestUsedAddress));
				reference2.Add(ref val);
			}
		}
		if (m_UpdatedTriangles.Length != 0)
		{
			for (int i = 0; i < m_UpdatedTriangles.Length; i++)
			{
				NativeHeapBlock val2 = m_UpdatedTriangles[i];
				m_AreaTriangleBuffer.SetData<AreaTriangleData>(m_AreaTriangleData.AsArray(), (int)((NativeHeapBlock)(ref val2)).Begin, (int)((NativeHeapBlock)(ref val2)).Begin, (int)((NativeHeapBlock)(ref val2)).Length);
			}
			m_UpdatedTriangles.Clear();
		}
		if (m_AreaColorBuffer.count != m_AreaColorData.Capacity)
		{
			m_AreaColorBuffer.Release();
			m_AreaColorBuffer = new ComputeBuffer(m_AreaColorData.Capacity, System.Runtime.CompilerServices.Unsafe.SizeOf<AreaColorData>());
		}
		if (m_ColorsUpdated)
		{
			m_ColorsUpdated = false;
			uint onePastHighestUsedAddress2 = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).OnePastHighestUsedAddress;
			if (onePastHighestUsedAddress2 != 0)
			{
				m_AreaColorBuffer.SetData<AreaColorData>(m_AreaColorData.AsArray(), 0, 0, (int)onePastHighestUsedAddress2);
			}
		}
		if (managedBatchData.m_VisibleIndices.count != reference.m_VisibleIndices.Capacity)
		{
			managedBatchData.m_VisibleIndices.Release();
			managedBatchData.m_VisibleIndices = new GraphicsBuffer((Target)16, reference.m_VisibleIndices.Capacity, 4);
		}
		if (reference.m_VisibleIndicesUpdated)
		{
			reference.m_VisibleIndicesUpdated = false;
			if (reference.m_VisibleIndices.Length != 0)
			{
				NativeArray<int> val3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)reference.m_VisibleIndices.Ptr, reference.m_VisibleIndices.Length, (Allocator)1);
				managedBatchData.m_VisibleIndices.SetData<int>(val3, 0, 0, val3.Length);
			}
		}
		buffer = m_AreaTriangleBuffer;
		colors = m_AreaColorBuffer;
		indices = managedBatchData.m_VisibleIndices;
		material = managedBatchData.m_Material;
		bounds = RenderingUtils.ToBounds(reference.m_Bounds);
		count = reference.m_VisibleIndices.Length;
		rendererPriority = managedBatchData.m_RendererPriority;
		return count != 0;
	}

	public NativeList<AreaColorData> GetColorData(out JobHandle dependencies)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_ColorsUpdated = true;
		dependencies = m_DataDependencies;
		return m_AreaColorData;
	}

	public void AddColorWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_DataDependencies = jobHandle;
	}

	public void GetAreaStats(out uint allocatedSize, out uint bufferSize, out uint count)
	{
		((JobHandle)(ref m_DataDependencies)).Complete();
		allocatedSize = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).UsedSpace * GetTriangleSize();
		bufferSize = ((NativeHeapAllocator)(ref m_AreaBufferAllocator)).Size * GetTriangleSize();
		count = (uint)m_AllocationCount.Value;
	}

	private static uint GetTriangleSize()
	{
		return (uint)(System.Runtime.CompilerServices.Unsafe.SizeOf<AreaTriangleData>() + System.Runtime.CompilerServices.Unsafe.SizeOf<AreaColorData>());
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		((JobHandle)(ref m_DataDependencies)).Complete();
		m_UpdatedTriangles.Clear();
		if (!((EntityQuery)(ref m_PrefabQuery)).IsEmptyIgnoreFilter)
		{
			UpdatePrefabs();
		}
		float3 val = m_PrevCameraPosition;
		float3 val2 = m_PrevCameraDirection;
		float4 val3 = m_PrevLodParameters;
		if (m_CameraUpdateSystem.TryGetLODParameters(out var lodParameters))
		{
			val = float3.op_Implicit(((LODParameters)(ref lodParameters)).cameraPosition);
			IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
			val3 = RenderingUtils.CalculateLodParameters(m_BatchDataSystem.GetLevelOfDetail(m_RenderingSystem.frameLod, activeCameraController), lodParameters);
			val2 = m_CameraUpdateSystem.activeViewer.forward;
		}
		BoundsMask visibleMask = BoundsMask.NormalLayers;
		BoundsMask prevVisibleMask = BoundsMask.NormalLayers;
		if (loaded)
		{
			m_PrevCameraPosition = val;
			m_PrevCameraDirection = val2;
			m_PrevLodParameters = val3;
			prevVisibleMask = (BoundsMask)0;
		}
		NativeParallelQueue<CullingAction> val4 = default(NativeParallelQueue<CullingAction>);
		val4._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<AllocationAction> allocationActions = default(NativeQueue<AllocationAction>);
		allocationActions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<int> nodeBuffer = default(NativeArray<int>);
		nodeBuffer._002Ector(512, (Allocator)3, (NativeArrayOptions)0);
		NativeArray<int> subDataBuffer = default(NativeArray<int>);
		subDataBuffer._002Ector(512, (Allocator)3, (NativeArrayOptions)0);
		JobHandle dependencies;
		TreeCullingJob1 treeCullingJob = new TreeCullingJob1
		{
			m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_LodParameters = val3,
			m_PrevLodParameters = m_PrevLodParameters,
			m_CameraPosition = val,
			m_PrevCameraPosition = m_PrevCameraPosition,
			m_CameraDirection = val2,
			m_PrevCameraDirection = m_PrevCameraDirection,
			m_VisibleMask = visibleMask,
			m_PrevVisibleMask = prevVisibleMask,
			m_NodeBuffer = nodeBuffer,
			m_SubDataBuffer = subDataBuffer,
			m_ActionQueue = val4.AsWriter()
		};
		TreeCullingJob2 treeCullingJob2 = new TreeCullingJob2
		{
			m_AreaSearchTree = treeCullingJob.m_AreaSearchTree,
			m_LodParameters = val3,
			m_PrevLodParameters = m_PrevLodParameters,
			m_CameraPosition = val,
			m_PrevCameraPosition = m_PrevCameraPosition,
			m_CameraDirection = val2,
			m_PrevCameraDirection = m_PrevCameraDirection,
			m_VisibleMask = visibleMask,
			m_PrevVisibleMask = prevVisibleMask,
			m_NodeBuffer = nodeBuffer,
			m_SubDataBuffer = subDataBuffer,
			m_ActionQueue = val4.AsWriter()
		};
		QueryCullingJob queryCullingJob = new QueryCullingJob
		{
			m_EntityType = ((ComponentSystemBase)this).GetEntityTypeHandle(),
			m_BatchType = ((ComponentSystemBase)this).GetComponentTypeHandle<Batch>(true),
			m_DeletedType = ((ComponentSystemBase)this).GetComponentTypeHandle<Deleted>(true),
			m_PrefabRefType = ((ComponentSystemBase)this).GetComponentTypeHandle<PrefabRef>(true),
			m_NodeType = ((ComponentSystemBase)this).GetBufferTypeHandle<Node>(true),
			m_TriangleType = ((ComponentSystemBase)this).GetBufferTypeHandle<Triangle>(true),
			m_PrefabAreaGeometryData = ((SystemBase)this).GetComponentLookup<AreaGeometryData>(true),
			m_LodParameters = val3,
			m_CameraPosition = val,
			m_CameraDirection = val2,
			m_VisibleMask = visibleMask,
			m_ActionQueue = val4.AsWriter()
		};
		CullingActionJob cullingActionJob = new CullingActionJob
		{
			m_PrefabRefData = ((SystemBase)this).GetComponentLookup<PrefabRef>(true),
			m_PrefabRenderedAreaData = ((SystemBase)this).GetComponentLookup<RenderedAreaData>(true),
			m_Triangles = ((SystemBase)this).GetBufferLookup<Triangle>(true),
			m_CullingActions = val4.AsReader(),
			m_AllocationActions = allocationActions.AsParallelWriter(),
			m_BatchData = ((SystemBase)this).GetComponentLookup<Batch>(false),
			m_TriangleMetaData = m_TriangleMetaData
		};
		BatchAllocationJob batchAllocationJob = new BatchAllocationJob
		{
			m_BatchData = ((SystemBase)this).GetComponentLookup<Batch>(false),
			m_NativeBatchData = m_NativeBatchData,
			m_TriangleMetaData = m_TriangleMetaData,
			m_AreaTriangleData = m_AreaTriangleData,
			m_AreaColorData = m_AreaColorData,
			m_UpdatedTriangles = m_UpdatedTriangles,
			m_AllocationActions = allocationActions,
			m_AreaBufferAllocator = m_AreaBufferAllocator,
			m_AllocationCount = m_AllocationCount
		};
		TriangleUpdateJob triangleUpdateJob = new TriangleUpdateJob
		{
			m_AreaData = ((SystemBase)this).GetComponentLookup<Area>(true),
			m_OwnerData = ((SystemBase)this).GetComponentLookup<Owner>(true),
			m_TransformData = ((SystemBase)this).GetComponentLookup<Transform>(true),
			m_PrefabRefData = ((SystemBase)this).GetComponentLookup<PrefabRef>(true),
			m_PrefabRenderedAreaData = ((SystemBase)this).GetComponentLookup<RenderedAreaData>(true),
			m_Nodes = ((SystemBase)this).GetBufferLookup<Node>(true),
			m_Triangles = ((SystemBase)this).GetBufferLookup<Triangle>(true),
			m_Expands = ((SystemBase)this).GetBufferLookup<Expand>(true),
			m_CullingActions = val4.AsReader(),
			m_BatchData = ((SystemBase)this).GetComponentLookup<Batch>(false),
			m_TriangleMetaData = m_TriangleMetaData,
			m_AreaTriangleData = m_AreaTriangleData,
			m_NativeBatchData = m_NativeBatchData
		};
		VisibleUpdateJob obj = new VisibleUpdateJob
		{
			m_NativeBatchData = m_NativeBatchData
		};
		JobHandle val5 = IJobExtensions.Schedule<TreeCullingJob1>(treeCullingJob, dependencies);
		JobHandle val6 = IJobParallelForExtensions.Schedule<TreeCullingJob2>(treeCullingJob2, nodeBuffer.Length, 1, val5);
		JobHandle val7 = JobChunkExtensions.ScheduleParallel<QueryCullingJob>(queryCullingJob, m_UpdatedQuery, ((SystemBase)this).Dependency);
		JobHandle val8 = IJobParallelForExtensions.Schedule<CullingActionJob>(cullingActionJob, val4.HashRange, 1, JobHandle.CombineDependencies(val6, val7));
		JobHandle val9 = IJobExtensions.Schedule<BatchAllocationJob>(batchAllocationJob, val8);
		JobHandle val10 = IJobParallelForExtensions.Schedule<TriangleUpdateJob>(triangleUpdateJob, val4.HashRange, 1, val9);
		JobHandle dataDependencies = IJobParallelForExtensions.Schedule<VisibleUpdateJob>(obj, m_ManagedBatchData.Count, 1, val10);
		m_AreaSearchSystem.AddSearchTreeReader(val6);
		val4.Dispose(val10);
		allocationActions.Dispose(val9);
		nodeBuffer.Dispose(val6);
		subDataBuffer.Dispose(val6);
		m_PrevCameraPosition = val;
		m_PrevCameraDirection = val2;
		m_PrevLodParameters = val3;
		((SystemBase)this).Dependency = val10;
		m_DataDependencies = dataDependencies;
	}

	public void EnabledShadersUpdated()
	{
		((JobHandle)(ref m_DataDependencies)).Complete();
		for (int i = 0; i < m_ManagedBatchData.Count; i++)
		{
			ManagedBatchData managedBatchData = m_ManagedBatchData[i];
			ref NativeBatchData reference = ref m_NativeBatchData.ElementAt(i);
			bool flag = m_RenderingSystem.IsShaderEnabled(managedBatchData.m_Material.shader);
			reference.m_VisibleUpdated |= flag != reference.m_IsEnabled;
			reference.m_IsEnabled = flag;
		}
	}

	private void UpdatePrefabs()
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Expected O, but got Unknown
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<AreaGeometryData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			((SystemBase)this).CompleteDependency();
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
				NativeArray<AreaGeometryData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<AreaGeometryData>(ref componentTypeHandle3);
				EntityManager entityManager;
				if (((ArchetypeChunk)(ref val2)).Has<Deleted>(ref componentTypeHandle))
				{
					((JobHandle)(ref m_DataDependencies)).Complete();
					for (int j = 0; j < nativeArray.Length; j++)
					{
						Entity val3 = nativeArray[j];
						entityManager = ((ComponentSystemBase)this).EntityManager;
						RenderedAreaData componentData = ((EntityManager)(ref entityManager)).GetComponentData<RenderedAreaData>(val3);
						if (m_ManagedBatchData.Count <= componentData.m_BatchIndex)
						{
							continue;
						}
						ManagedBatchData managedBatchData = m_ManagedBatchData[componentData.m_BatchIndex];
						NativeBatchData nativeBatchData = m_NativeBatchData[componentData.m_BatchIndex];
						if (!(nativeBatchData.m_Prefab != val3))
						{
							if ((Object)(object)managedBatchData.m_Material != (Object)null)
							{
								Object.Destroy((Object)(object)managedBatchData.m_Material);
							}
							if (managedBatchData.m_VisibleIndices != null)
							{
								managedBatchData.m_VisibleIndices.Release();
							}
							if (nativeBatchData.m_AreaMetaData.IsCreated)
							{
								nativeBatchData.m_AreaMetaData.Dispose();
							}
							if (nativeBatchData.m_VisibleIndices.IsCreated)
							{
								nativeBatchData.m_VisibleIndices.Dispose();
							}
							if (componentData.m_BatchIndex != m_ManagedBatchData.Count - 1)
							{
								ManagedBatchData value = m_ManagedBatchData[m_ManagedBatchData.Count - 1];
								NativeBatchData nativeBatchData2 = m_NativeBatchData[m_ManagedBatchData.Count - 1];
								entityManager = ((ComponentSystemBase)this).EntityManager;
								RenderedAreaData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<RenderedAreaData>(nativeBatchData2.m_Prefab);
								componentData2.m_BatchIndex = componentData.m_BatchIndex;
								entityManager = ((ComponentSystemBase)this).EntityManager;
								((EntityManager)(ref entityManager)).SetComponentData<RenderedAreaData>(nativeBatchData2.m_Prefab, componentData2);
								m_ManagedBatchData[componentData.m_BatchIndex] = value;
								m_NativeBatchData[componentData.m_BatchIndex] = nativeBatchData2;
							}
							m_ManagedBatchData.RemoveAt(m_ManagedBatchData.Count - 1);
							m_NativeBatchData.RemoveAt(m_NativeBatchData.Length - 1);
						}
					}
				}
				else
				{
					for (int k = 0; k < nativeArray.Length; k++)
					{
						Entity val4 = nativeArray[k];
						RenderedArea component = m_PrefabSystem.GetPrefab<AreaPrefab>(nativeArray2[k]).GetComponent<RenderedArea>();
						float minNodeDistance = AreaUtils.GetMinNodeDistance(nativeArray3[k].m_Type);
						float num = minNodeDistance * 2f;
						float num2 = math.clamp(component.m_Roundness, 0.01f, 0.99f) * minNodeDistance;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						RenderedAreaData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<RenderedAreaData>(val4);
						componentData3.m_HeightOffset = num;
						componentData3.m_ExpandAmount = num2 * 0.5f;
						componentData3.m_BatchIndex = m_ManagedBatchData.Count;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<RenderedAreaData>(val4, componentData3);
						ManagedBatchData managedBatchData2 = new ManagedBatchData();
						managedBatchData2.m_Material = new Material(component.m_Material);
						((Object)managedBatchData2.m_Material).name = "Area batch (" + ((Object)component.m_Material).name + ")";
						managedBatchData2.m_Material.renderQueue = component.m_Material.shader.renderQueue;
						managedBatchData2.m_Material.SetVector(m_AreaParameters, new Vector4(num2, num, 0f, 0f));
						managedBatchData2.m_Material.SetFloat(m_DecalLayerMask, math.asfloat((int)component.m_DecalLayerMask));
						managedBatchData2.m_RendererPriority = component.m_RendererPriority;
						NativeBatchData nativeBatchData3 = new NativeBatchData
						{
							m_AreaMetaData = new UnsafeList<AreaMetaData>(10, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)0),
							m_VisibleIndices = new UnsafeList<int>(100, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)0),
							m_Prefab = val4,
							m_IsEnabled = m_RenderingSystem.IsShaderEnabled(managedBatchData2.m_Material.shader)
						};
						managedBatchData2.m_VisibleIndices = new GraphicsBuffer((Target)16, nativeBatchData3.m_VisibleIndices.Capacity, 4);
						m_ManagedBatchData.Add(managedBatchData2);
						m_NativeBatchData.Add(ref nativeBatchData3);
					}
				}
			}
		}
		finally
		{
			val.Dispose();
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
	public AreaBatchSystem()
	{
	}
}
