using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Effects;
using Game.Events;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class PreCullingSystem : GameSystemBase, IPostDeserialize
{
	[Flags]
	private enum QueryFlags
	{
		Unspawned = 1,
		Zones = 2
	}

	[BurstCompile]
	private struct TreeCullingJob1 : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

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

		[NativeDisableParallelForRestriction]
		public NativeArray<int> m_NodeBuffer;

		[NativeDisableParallelForRestriction]
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
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
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
			int num = m_NodeBuffer.Length / 3;
			switch (index)
			{
			case 0:
				m_StaticObjectSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, 3, m_NodeBuffer.GetSubArray(0, num), m_SubDataBuffer.GetSubArray(0, num));
				break;
			case 1:
				m_NetSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, 3, m_NodeBuffer.GetSubArray(num, num), m_SubDataBuffer.GetSubArray(num, num));
				break;
			case 2:
				m_LaneSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, 3, m_NodeBuffer.GetSubArray(num * 2, num), m_SubDataBuffer.GetSubArray(num * 2, num));
				break;
			}
		}
	}

	[BurstCompile]
	private struct TreeCullingJob2 : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_StaticObjectSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_LaneSearchTree;

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
			switch (index * 3 / m_NodeBuffer.Length)
			{
			case 0:
				m_StaticObjectSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, m_SubDataBuffer[index], m_NodeBuffer[index]);
				break;
			case 1:
				m_NetSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, m_SubDataBuffer[index], m_NodeBuffer[index]);
				break;
			case 2:
				m_LaneSearchTree.Iterate<TreeCullingIterator, int>(ref treeCullingIterator, m_SubDataBuffer[index], m_NodeBuffer[index]);
				break;
			}
		}
	}

	private struct TreeCullingIterator : INativeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, int>, IUnsafeQuadTreeIteratorWithSubData<Entity, QuadTreeBoundsXZ, int>
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

		public void Iterate(QuadTreeBoundsXZ bounds, int subData, Entity entity)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
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
							m_Entity = entity,
							m_Flags = ActionFlags.PassedCulling,
							m_UpdateFrame = -1
						});
					}
				}
				return;
			}
			case 2:
			{
				float num = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
				int num2 = RenderingUtils.CalculateLod(num * num, m_PrevLodParameters);
				if ((m_PrevVisibleMask & bounds.m_Mask) != 0 && num2 >= bounds.m_MinLod)
				{
					float num3 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
					int num4 = RenderingUtils.CalculateLod(num3 * num3, m_LodParameters);
					if ((m_VisibleMask & bounds.m_Mask) == 0 || num4 < bounds.m_MaxLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = entity,
							m_Flags = (((m_VisibleMask & bounds.m_Mask) != 0) ? ActionFlags.CrossFade : ((ActionFlags)0)),
							m_UpdateFrame = -1
						});
					}
				}
				return;
			}
			}
			float num9 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
			float num10 = RenderingUtils.CalculateMinDistance(bounds.m_Bounds, m_PrevCameraPosition, m_PrevCameraDirection, m_PrevLodParameters);
			int num11 = RenderingUtils.CalculateLod(num9 * num9, m_LodParameters);
			int num12 = RenderingUtils.CalculateLod(num10 * num10, m_PrevLodParameters);
			bool flag = (m_VisibleMask & bounds.m_Mask) != 0 && num11 >= bounds.m_MinLod;
			bool flag2 = (m_PrevVisibleMask & bounds.m_Mask) != 0 && num12 >= bounds.m_MaxLod;
			if (flag != flag2)
			{
				CullingAction cullingAction = new CullingAction
				{
					m_Entity = entity,
					m_UpdateFrame = -1
				};
				if (flag)
				{
					cullingAction.m_Flags = ActionFlags.PassedCulling;
				}
				else if ((m_VisibleMask & bounds.m_Mask) != 0)
				{
					cullingAction.m_Flags = ActionFlags.CrossFade;
				}
				m_ActionQueue.Enqueue(cullingAction);
			}
		}
	}

	[BurstCompile]
	public struct InitializeCullingJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Updated> m_UpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<BatchesUpdated> m_BatchesUpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> m_OverriddenType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Stack> m_StackType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> m_ObjectMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> m_NodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> m_StartNodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> m_EndNodeGeometryType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> m_OrphanType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> m_UtilityLaneType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Marker> m_NetMarkerType;

		[ReadOnly]
		public ComponentTypeHandle<Block> m_ZoneBlockType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> m_PrefabLaneGeometryData;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> m_PrefabUtilityLaneData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_PrefabCompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> m_PrefabCompositionMeshRef;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> m_PrefabCompositionMeshData;

		[ReadOnly]
		public ComponentLookup<NetData> m_PrefabNetData;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> m_PrefabNetGeometryData;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public bool m_UpdateAll;

		[ReadOnly]
		public bool m_UnspawnedVisible;

		[ReadOnly]
		public bool m_Loaded;

		[ReadOnly]
		public UtilityTypes m_DilatedUtilityTypes;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[NativeDisableParallelForRestriction]
		public NativeList<PreCullingData> m_CullingData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06da: Unknown result type (might be due to invalid IL or missing references)
			//IL_06df: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_0713: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0632: Unknown result type (might be due to invalid IL or missing references)
			//IL_063c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0643: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_0657: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_078d: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ccc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cdd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c23: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0af9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0804: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_0898: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0827: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0908: Unknown result type (might be due to invalid IL or missing references)
			//IL_0912: Unknown result type (might be due to invalid IL or missing references)
			//IL_0928: Unknown result type (might be due to invalid IL or missing references)
			//IL_0932: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CullingInfo> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Updated>(ref m_UpdatedType);
			bool batchesUpdated = ((ArchetypeChunk)(ref chunk)).Has<BatchesUpdated>(ref m_BatchesUpdatedType);
			if (bufferAccessor.Length != 0)
			{
				NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Marker>(ref m_ObjectMarkerType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				bool remove = !m_UnspawnedVisible && ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				Owner owner = default(Owner);
				for (int i = 0; i < nativeArray.Length; i++)
				{
					ref CullingInfo reference = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, i);
					if (m_UpdateAll || flag)
					{
						PrefabRef prefabRef = nativeArray2[i];
						reference.m_Bounds = default(Bounds3);
						if (m_PrefabObjectGeometryData.HasComponent(prefabRef.m_Prefab))
						{
							ObjectGeometryData objectGeometryData = m_PrefabObjectGeometryData[prefabRef.m_Prefab];
							reference.m_Radius = math.length(math.max(-objectGeometryData.m_Bounds.min, objectGeometryData.m_Bounds.max));
							reference.m_Mask = BoundsMask.Debug;
							if (!flag2 || m_EditorMode)
							{
								MeshLayer layers = objectGeometryData.m_Layers;
								CollectionUtils.TryGet<Owner>(nativeArray3, i, ref owner);
								reference.m_Mask |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner, default(Game.Net.UtilityLane), layers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
							}
							reference.m_MinLod = (byte)objectGeometryData.m_MinLod;
						}
						else
						{
							reference.m_Radius = 1f;
							reference.m_Mask = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
							reference.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(2f)));
						}
					}
					SetFlags(ref reference, (int)index, flag, batchesUpdated, remove);
				}
				return;
			}
			NativeArray<Transform> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Node> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<Edge> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			NativeArray<Curve> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Block> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Block>(ref m_ZoneBlockType);
			if (nativeArray4.Length != 0)
			{
				NativeArray<Owner> nativeArray9 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Stack> nativeArray10 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackType);
				bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.Marker>(ref m_ObjectMarkerType) && !((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
				bool flag4 = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
				bool remove2 = !m_UnspawnedVisible && ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
				ObjectGeometryData geometryData = default(ObjectGeometryData);
				StackData stackData = default(StackData);
				Owner owner2 = default(Owner);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					ref CullingInfo reference2 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, j);
					if (m_UpdateAll || flag)
					{
						Transform transform = nativeArray4[j];
						PrefabRef prefabRef2 = nativeArray2[j];
						if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef2.m_Prefab, ref geometryData))
						{
							if (nativeArray10.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef2.m_Prefab, ref stackData))
							{
								Stack stack = nativeArray10[j];
								reference2.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, stack, geometryData, stackData);
								reference2.m_Radius = 0f;
							}
							else
							{
								reference2.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
								reference2.m_Radius = 0f;
							}
							if ((geometryData.m_Flags & Game.Objects.GeometryFlags.HasBase) != Game.Objects.GeometryFlags.None)
							{
								reference2.m_Bounds.min.y = math.min(reference2.m_Bounds.min.y, TerrainUtils.GetHeightRange(ref m_TerrainHeightData, reference2.m_Bounds).min);
							}
							reference2.m_Mask = BoundsMask.Debug;
							if (!flag4 && (!flag3 || m_EditorMode))
							{
								MeshLayer layers2 = geometryData.m_Layers;
								CollectionUtils.TryGet<Owner>(nativeArray9, j, ref owner2);
								reference2.m_Mask |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner2, default(Game.Net.UtilityLane), layers2, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
							}
							reference2.m_MinLod = (byte)geometryData.m_MinLod;
						}
						else
						{
							reference2.m_Bounds = new Bounds3(transform.m_Position - 1f, transform.m_Position + 1f);
							reference2.m_Radius = 0f;
							reference2.m_Mask = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
							reference2.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float3(2f)));
						}
					}
					SetFlags(ref reference2, -1, flag, batchesUpdated, remove2);
				}
			}
			else if (nativeArray5.Length != 0)
			{
				NativeArray<NodeGeometry> nativeArray11 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NodeGeometry>(ref m_NodeGeometryType);
				NativeArray<Orphan> nativeArray12 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Orphan>(ref m_OrphanType);
				bool flag5 = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Marker>(ref m_NetMarkerType);
				NetCompositionMeshData netCompositionMeshData = default(NetCompositionMeshData);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					ref CullingInfo reference3 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, k);
					if (m_UpdateAll || flag)
					{
						if (nativeArray11.Length != 0)
						{
							reference3.m_Bounds = nativeArray11[k].m_Bounds;
							reference3.m_Radius = 0f;
							reference3.m_Mask = BoundsMask.Debug;
							if (nativeArray12.Length != 0)
							{
								Orphan orphan = nativeArray12[k];
								reference3.m_MinLod = (byte)m_PrefabCompositionData[orphan.m_Composition].m_MinLod;
								if (!flag5 || m_EditorMode)
								{
									NetCompositionMeshRef netCompositionMeshRef = m_PrefabCompositionMeshRef[orphan.m_Composition];
									if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef.m_Mesh, ref netCompositionMeshData))
									{
										reference3.m_Mask |= ((netCompositionMeshData.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData.m_DefaultLayers));
									}
								}
							}
							else
							{
								reference3.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
							}
						}
						else
						{
							Node node = nativeArray5[k];
							reference3.m_Bounds = new Bounds3(node.m_Position - 1f, node.m_Position + 1f);
							reference3.m_Radius = 0f;
							reference3.m_Mask = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
							reference3.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
						}
					}
					SetFlags(ref reference3, -1, flag, batchesUpdated, remove: false);
				}
			}
			else if (nativeArray6.Length != 0)
			{
				NativeArray<EdgeGeometry> nativeArray13 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				NativeArray<StartNodeGeometry> nativeArray14 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<StartNodeGeometry>(ref m_StartNodeGeometryType);
				NativeArray<EndNodeGeometry> nativeArray15 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EndNodeGeometry>(ref m_EndNodeGeometryType);
				NativeArray<Composition> nativeArray16 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
				bool flag6 = ((ArchetypeChunk)(ref chunk)).Has<Game.Net.Marker>(ref m_NetMarkerType);
				NetCompositionMeshData netCompositionMeshData2 = default(NetCompositionMeshData);
				NetCompositionMeshData netCompositionMeshData3 = default(NetCompositionMeshData);
				NetCompositionMeshData netCompositionMeshData4 = default(NetCompositionMeshData);
				for (int l = 0; l < nativeArray.Length; l++)
				{
					ref CullingInfo reference4 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, l);
					if (m_UpdateAll || flag)
					{
						if (nativeArray13.Length != 0)
						{
							EdgeGeometry edgeGeometry = nativeArray13[l];
							StartNodeGeometry startNodeGeometry = nativeArray14[l];
							EndNodeGeometry endNodeGeometry = nativeArray15[l];
							Composition composition = nativeArray16[l];
							reference4.m_Bounds = edgeGeometry.m_Bounds | startNodeGeometry.m_Geometry.m_Bounds | endNodeGeometry.m_Geometry.m_Bounds;
							NetCompositionData netCompositionData = m_PrefabCompositionData[composition.m_Edge];
							NetCompositionData netCompositionData2 = m_PrefabCompositionData[composition.m_StartNode];
							NetCompositionData netCompositionData3 = m_PrefabCompositionData[composition.m_EndNode];
							reference4.m_Radius = 0f;
							reference4.m_Mask = BoundsMask.Debug;
							if (!flag6 || m_EditorMode)
							{
								if (math.any(edgeGeometry.m_Start.m_Length + edgeGeometry.m_End.m_Length > 0.1f))
								{
									NetCompositionMeshRef netCompositionMeshRef2 = m_PrefabCompositionMeshRef[composition.m_Edge];
									if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef2.m_Mesh, ref netCompositionMeshData2))
									{
										reference4.m_Mask |= ((netCompositionMeshData2.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData2.m_DefaultLayers));
									}
								}
								if (math.any(startNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(startNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
								{
									NetCompositionMeshRef netCompositionMeshRef3 = m_PrefabCompositionMeshRef[composition.m_StartNode];
									if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef3.m_Mesh, ref netCompositionMeshData3))
									{
										reference4.m_Mask |= ((netCompositionMeshData3.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData3.m_DefaultLayers));
									}
								}
								if (math.any(endNodeGeometry.m_Geometry.m_Left.m_Length > 0.05f) | math.any(endNodeGeometry.m_Geometry.m_Right.m_Length > 0.05f))
								{
									NetCompositionMeshRef netCompositionMeshRef4 = m_PrefabCompositionMeshRef[composition.m_EndNode];
									if (m_PrefabCompositionMeshData.TryGetComponent(netCompositionMeshRef4.m_Mesh, ref netCompositionMeshData4))
									{
										reference4.m_Mask |= ((netCompositionMeshData4.m_DefaultLayers == (MeshLayer)0) ? BoundsMask.NormalLayers : CommonUtils.GetBoundsMask(netCompositionMeshData4.m_DefaultLayers));
									}
								}
							}
							reference4.m_MinLod = (byte)math.min(netCompositionData.m_MinLod, math.min(netCompositionData2.m_MinLod, netCompositionData3.m_MinLod));
						}
						else
						{
							reference4.m_Bounds = MathUtils.Expand(MathUtils.Bounds(nativeArray7[l].m_Bezier), float3.op_Implicit(0.5f));
							reference4.m_Radius = 0f;
							reference4.m_Mask = ((!m_EditorMode) ? BoundsMask.Debug : (BoundsMask.Debug | BoundsMask.NormalLayers));
							reference4.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(2f)));
						}
					}
					SetFlags(ref reference4, -1, flag, batchesUpdated, remove: false);
				}
			}
			else if (nativeArray7.Length != 0)
			{
				NativeArray<Owner> nativeArray17 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
				NativeArray<Game.Net.UtilityLane> nativeArray18 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.UtilityLane>(ref m_UtilityLaneType);
				bool flag7 = ((ArchetypeChunk)(ref chunk)).Has<Overridden>(ref m_OverriddenType);
				Owner owner3 = default(Owner);
				Game.Net.UtilityLane utilityLane = default(Game.Net.UtilityLane);
				UtilityLaneData utilityLaneData = default(UtilityLaneData);
				for (int m = 0; m < nativeArray.Length; m++)
				{
					ref CullingInfo reference5 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, m);
					if (m_UpdateAll || flag)
					{
						Curve curve = nativeArray7[m];
						PrefabRef prefabRef3 = nativeArray2[m];
						if (m_PrefabLaneGeometryData.HasComponent(prefabRef3.m_Prefab))
						{
							NetLaneGeometryData netLaneGeometryData = m_PrefabLaneGeometryData[prefabRef3.m_Prefab];
							reference5.m_Bounds = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), ((float3)(ref netLaneGeometryData.m_Size)).xyx * 0.5f);
							reference5.m_Radius = 0f;
							reference5.m_Mask = BoundsMask.Debug;
							if (!flag7 && curve.m_Length > 0.1f)
							{
								MeshLayer defaultLayers = (m_EditorMode ? netLaneGeometryData.m_EditorLayers : netLaneGeometryData.m_GameLayers);
								CollectionUtils.TryGet<Owner>(nativeArray17, m, ref owner3);
								CollectionUtils.TryGet<Game.Net.UtilityLane>(nativeArray18, m, ref utilityLane);
								reference5.m_Mask |= CommonUtils.GetBoundsMask(Game.Net.SearchSystem.GetLayers(owner3, utilityLane, defaultLayers, ref m_PrefabRefData, ref m_PrefabNetData, ref m_PrefabNetGeometryData));
							}
							int num = netLaneGeometryData.m_MinLod;
							if (m_PrefabUtilityLaneData.TryGetComponent(prefabRef3.m_Prefab, ref utilityLaneData) && (utilityLaneData.m_UtilityTypes & m_DilatedUtilityTypes) != UtilityTypes.None)
							{
								float renderingSize = RenderingUtils.GetRenderingSize(new float2(utilityLaneData.m_VisualCapacity));
								num = math.min(num, RenderingUtils.CalculateLodLimit(renderingSize));
							}
							reference5.m_MinLod = (byte)num;
						}
						else
						{
							reference5.m_Bounds = MathUtils.Expand(MathUtils.Bounds(curve.m_Bezier), float3.op_Implicit(0.5f));
							reference5.m_Radius = 0f;
							reference5.m_Mask = BoundsMask.Debug;
							reference5.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(new float2(1f)));
						}
					}
					SetFlags(ref reference5, -1, flag, batchesUpdated, remove: false);
				}
			}
			else
			{
				if (nativeArray8.Length == 0)
				{
					return;
				}
				for (int n = 0; n < nativeArray.Length; n++)
				{
					ref CullingInfo reference6 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray, n);
					if (m_UpdateAll || flag)
					{
						Block block = nativeArray8[n];
						float3 size = new float3((float)block.m_Size.x, (float)math.cmax(block.m_Size), (float)block.m_Size.y) * 8f;
						reference6.m_Bounds = new Bounds3(block.m_Position, block.m_Position);
						((Bounds3)(ref reference6.m_Bounds)).xz = ZoneUtils.CalculateBounds(block);
						reference6.m_Radius = 0f;
						reference6.m_Mask = BoundsMask.Debug | BoundsMask.NormalLayers;
						reference6.m_MinLod = (byte)RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(size), 0f);
					}
					SetFlags(ref reference6, -1, flag, batchesUpdated, remove: false);
				}
			}
		}

		private void SetFlags(ref CullingInfo cullingInfo, int updateFrame, bool isUpdated, bool batchesUpdated, bool remove)
		{
			if (cullingInfo.m_CullingIndex != 0)
			{
				ref PreCullingData reference = ref m_CullingData.ElementAt(cullingInfo.m_CullingIndex);
				reference.m_UpdateFrame = (sbyte)updateFrame;
				if (isUpdated)
				{
					reference.m_Flags |= PreCullingFlags.Updated;
				}
				if (batchesUpdated)
				{
					reference.m_Flags |= PreCullingFlags.BatchesUpdated;
				}
				if (remove)
				{
					cullingInfo.m_PassedCulling = 0;
					reference.m_Flags &= ~PreCullingFlags.PassedCulling;
					reference.m_Timer = byte.MaxValue;
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct EventCullingJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> m_RentersUpdatedType;

		[ReadOnly]
		public ComponentTypeHandle<ColorUpdated> m_ColorUpdatedType;

		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		public NativeList<PreCullingData> m_CullingData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<RentersUpdated> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<RentersUpdated>(ref m_RentersUpdatedType);
			if (nativeArray.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity property = nativeArray[i].m_Property;
					SetFlags(property);
					AddSubObjects(property);
					AddSubLanes(property);
				}
				return;
			}
			NativeArray<ColorUpdated> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ColorUpdated>(ref m_ColorUpdatedType);
			if (nativeArray2.Length != 0)
			{
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity route = nativeArray2[j].m_Route;
					AddRouteVehicles(route);
				}
			}
		}

		private void AddSubObjects(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
			if (m_SubObjects.TryGetBuffer(owner, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					SetFlags(subObject);
					AddSubObjects(subObject);
					AddSubLanes(subObject);
				}
			}
		}

		private void AddSubLanes(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<Game.Net.SubLane> val = default(DynamicBuffer<Game.Net.SubLane>);
			if (m_SubLanes.TryGetBuffer(owner, ref val))
			{
				for (int i = 0; i < val.Length; i++)
				{
					Entity subLane = val[i].m_SubLane;
					SetFlags(subLane);
				}
			}
		}

		private void AddRouteVehicles(Entity owner)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteVehicle> val = default(DynamicBuffer<RouteVehicle>);
			if (!m_RouteVehicles.TryGetBuffer(owner, ref val))
			{
				return;
			}
			DynamicBuffer<LayoutElement> val2 = default(DynamicBuffer<LayoutElement>);
			for (int i = 0; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (m_LayoutElements.TryGetBuffer(vehicle, ref val2) && val2.Length != 0)
				{
					for (int j = 0; j < val2.Length; j++)
					{
						SetFlags(val2[j].m_Vehicle);
					}
				}
				else
				{
					SetFlags(vehicle);
				}
			}
		}

		private void SetFlags(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CullingInfo cullingInfo = default(CullingInfo);
			if (m_CullingInfoData.TryGetComponent(entity, ref cullingInfo) && cullingInfo.m_CullingIndex != 0)
			{
				m_CullingData.ElementAt(cullingInfo.m_CullingIndex).m_Flags |= PreCullingFlags.ColorsUpdated;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct QueryCullingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public uint m_FrameIndex;

		[ReadOnly]
		public float m_FrameTime;

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
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CullingInfo> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			BufferAccessor<TransformFrame> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TransformFrame>(ref m_TransformFrameType);
			if (bufferAccessor.Length != 0)
			{
				NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				ObjectInterpolateSystem.CalculateUpdateFrames(m_FrameIndex, m_FrameTime, index, out var updateFrame, out var updateFrame2, out var framePosition);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					ref CullingInfo reference = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray2, i);
					if ((m_VisibleMask & reference.m_Mask) == 0)
					{
						if (reference.m_CullingIndex != 0)
						{
							m_ActionQueue.Enqueue(new CullingAction
							{
								m_Entity = nativeArray[i],
								m_UpdateFrame = (sbyte)index
							});
						}
						continue;
					}
					if (reference.m_PassedCulling != 0)
					{
						DynamicBuffer<TransformFrame> val = bufferAccessor[i];
						TransformFrame transformFrame = val[(int)updateFrame];
						TransformFrame transformFrame2 = val[(int)updateFrame2];
						float3 val2 = math.lerp(transformFrame.m_Position, transformFrame2.m_Position, framePosition);
						reference.m_Bounds = new Bounds3(val2 - reference.m_Radius, val2 + reference.m_Radius);
						float num = RenderingUtils.CalculateMinDistance(reference.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
						if (RenderingUtils.CalculateLod(num * num, m_LodParameters) < reference.m_MinLod)
						{
							m_ActionQueue.Enqueue(new CullingAction
							{
								m_Entity = nativeArray[i],
								m_Flags = ActionFlags.CrossFade,
								m_UpdateFrame = (sbyte)index
							});
						}
						continue;
					}
					float3 position = nativeArray3[i].m_Position;
					reference.m_Bounds = new Bounds3(position - reference.m_Radius, position + reference.m_Radius);
					float num2 = math.max(0f, RenderingUtils.CalculateMinDistance(reference.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters) - 277.77777f);
					if (RenderingUtils.CalculateLod(num2 * num2, m_LodParameters) >= reference.m_MinLod)
					{
						DynamicBuffer<TransformFrame> val3 = bufferAccessor[i];
						TransformFrame transformFrame3 = val3[(int)updateFrame];
						TransformFrame transformFrame4 = val3[(int)updateFrame2];
						position = math.lerp(transformFrame3.m_Position, transformFrame4.m_Position, framePosition);
						reference.m_Bounds = new Bounds3(position - reference.m_Radius, position + reference.m_Radius);
						float num3 = RenderingUtils.CalculateMinDistance(reference.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
						if (RenderingUtils.CalculateLod(num3 * num3, m_LodParameters) >= reference.m_MinLod)
						{
							m_ActionQueue.Enqueue(new CullingAction
							{
								m_Entity = nativeArray[i],
								m_Flags = ActionFlags.PassedCulling,
								m_UpdateFrame = (sbyte)index
							});
						}
					}
				}
				return;
			}
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				ref CullingInfo reference2 = ref CollectionUtils.ElementAt<CullingInfo>(nativeArray2, j);
				if ((m_VisibleMask & reference2.m_Mask) == 0)
				{
					if (reference2.m_CullingIndex != 0)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = nativeArray[j],
							m_UpdateFrame = -1
						});
					}
					continue;
				}
				float num4 = RenderingUtils.CalculateMinDistance(reference2.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num5 = RenderingUtils.CalculateLod(num4 * num4, m_LodParameters);
				if (reference2.m_PassedCulling != 0)
				{
					if (num5 < reference2.m_MinLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = nativeArray[j],
							m_Flags = ActionFlags.CrossFade,
							m_UpdateFrame = -1
						});
					}
				}
				else if (num5 >= reference2.m_MinLod)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = nativeArray[j],
						m_Flags = ActionFlags.PassedCulling,
						m_UpdateFrame = -1
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct QueryRemoveJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> m_DeletedType;

		[ReadOnly]
		public ComponentTypeHandle<Applied> m_AppliedType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> m_TransformFrameType;

		public ComponentTypeHandle<CullingInfo> m_CullingInfoType;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CullingInfo> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CullingInfo>(ref m_CullingInfoType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Deleted>(ref m_DeletedType);
			bool flag2 = false;
			if (flag)
			{
				flag2 = ((ArchetypeChunk)(ref chunk)).Has<Applied>(ref m_AppliedType);
			}
			if (((ArchetypeChunk)(ref chunk)).Has<TransformFrame>(ref m_TransformFrameType))
			{
				uint index = ((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index;
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					if (CollectionUtils.ElementAt<CullingInfo>(nativeArray2, i).m_CullingIndex != 0)
					{
						ActionFlags flags = (ActionFlags)0;
						if (flag)
						{
							flags = (flag2 ? (ActionFlags.Deleted | ActionFlags.Applied) : ActionFlags.Deleted);
						}
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = nativeArray[i],
							m_Flags = flags,
							m_UpdateFrame = (sbyte)index
						});
					}
				}
				return;
			}
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				if (CollectionUtils.ElementAt<CullingInfo>(nativeArray2, j).m_CullingIndex != 0)
				{
					ActionFlags flags2 = (ActionFlags)0;
					if (flag)
					{
						flags2 = (flag2 ? (ActionFlags.Deleted | ActionFlags.Applied) : ActionFlags.Deleted);
					}
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = nativeArray[j],
						m_Flags = flags2,
						m_UpdateFrame = -1
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct RelativeCullingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> m_CurrentVehicleType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

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
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<CurrentVehicle> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentVehicle>(ref m_CurrentVehicleType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					Entity entity = nativeArray[i];
					UpdateCulling(entity, nativeArray2[i].m_Vehicle);
				}
				return;
			}
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity entity2 = nativeArray[j];
				UpdateCulling(entity2, nativeArray3[j].m_Owner);
			}
		}

		private void UpdateCulling(Entity entity, Entity parent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			ref CullingInfo valueRW = ref m_CullingInfoData.GetRefRW(entity).ValueRW;
			valueRW.m_Bounds = m_CullingInfoData[parent].m_Bounds;
			if ((m_VisibleMask & valueRW.m_Mask) == 0)
			{
				if (valueRW.m_CullingIndex != 0)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = entity,
						m_UpdateFrame = -1
					});
				}
				return;
			}
			float num = RenderingUtils.CalculateMinDistance(valueRW.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
			int num2 = RenderingUtils.CalculateLod(num * num, m_LodParameters);
			if (valueRW.m_PassedCulling != 0)
			{
				if (num2 < valueRW.m_MinLod)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = entity,
						m_Flags = ActionFlags.CrossFade,
						m_UpdateFrame = -1
					});
				}
			}
			else if (num2 >= valueRW.m_MinLod)
			{
				m_ActionQueue.Enqueue(new CullingAction
				{
					m_Entity = entity,
					m_Flags = ActionFlags.PassedCulling,
					m_UpdateFrame = -1
				});
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct TempCullingJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> m_InterpolatedTransformType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Stack> m_StackType;

		[ReadOnly]
		public ComponentTypeHandle<Static> m_StaticType;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> m_StoppedType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_PrefabObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<StackData> m_PrefabStackData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<InterpolatedTransform>(ref m_InterpolatedTransformType);
			bool flag2 = false;
			bool flag3 = false;
			NativeArray<Transform> val = default(NativeArray<Transform>);
			NativeArray<Stack> val2 = default(NativeArray<Stack>);
			NativeArray<Temp> val3 = default(NativeArray<Temp>);
			NativeArray<PrefabRef> val4 = default(NativeArray<PrefabRef>);
			if (flag)
			{
				flag2 = ((ArchetypeChunk)(ref chunk)).Has<Static>(ref m_StaticType);
				flag3 = ((ArchetypeChunk)(ref chunk)).Has<Stopped>(ref m_StoppedType);
				val = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				val2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Stack>(ref m_StackType);
				val3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
				val4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			}
			CullingInfo cullingInfo = default(CullingInfo);
			ObjectGeometryData geometryData = default(ObjectGeometryData);
			StackData stackData = default(StackData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val5 = nativeArray[i];
				ref CullingInfo valueRW = ref m_CullingInfoData.GetRefRW(val5).ValueRW;
				if (flag)
				{
					Temp temp = val3[i];
					if (temp.m_Original != Entity.Null && (temp.m_Flags & TempFlags.Dragging) == 0 && ((!flag2 && !flag3) || (temp.m_Flags & (TempFlags.Create | TempFlags.Modify)) == 0) && m_CullingInfoData.TryGetComponent(temp.m_Original, ref cullingInfo))
					{
						valueRW.m_Bounds = cullingInfo.m_Bounds;
					}
					else
					{
						Transform transform = val[i];
						PrefabRef prefabRef = val4[i];
						if (m_PrefabObjectGeometryData.TryGetComponent(prefabRef.m_Prefab, ref geometryData))
						{
							if (val2.Length != 0 && m_PrefabStackData.TryGetComponent(prefabRef.m_Prefab, ref stackData))
							{
								Stack stack = val2[i];
								valueRW.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, stack, geometryData, stackData);
							}
							else
							{
								valueRW.m_Bounds = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
							}
							if ((geometryData.m_Flags & Game.Objects.GeometryFlags.HasBase) != Game.Objects.GeometryFlags.None)
							{
								valueRW.m_Bounds.min.y = math.min(valueRW.m_Bounds.min.y, TerrainUtils.GetHeightRange(ref m_TerrainHeightData, valueRW.m_Bounds).min);
							}
						}
						else
						{
							valueRW.m_Bounds = new Bounds3(transform.m_Position - 1f, transform.m_Position + 1f);
						}
					}
				}
				if ((m_VisibleMask & valueRW.m_Mask) == 0)
				{
					if (valueRW.m_CullingIndex != 0)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = nativeArray[i],
							m_UpdateFrame = -1
						});
					}
					continue;
				}
				float num = RenderingUtils.CalculateMinDistance(valueRW.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				int num2 = RenderingUtils.CalculateLod(num * num, m_LodParameters);
				if (valueRW.m_PassedCulling != 0)
				{
					if (num2 < valueRW.m_MinLod)
					{
						m_ActionQueue.Enqueue(new CullingAction
						{
							m_Entity = nativeArray[i],
							m_Flags = ActionFlags.CrossFade,
							m_UpdateFrame = -1
						});
					}
				}
				else if (num2 >= valueRW.m_MinLod)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = nativeArray[i],
						m_Flags = ActionFlags.PassedCulling,
						m_UpdateFrame = -1
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct VerifyVisibleJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentLookup<CullingInfo> m_CullingInfoData;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public BoundsMask m_VisibleMask;

		[ReadOnly]
		public NativeList<PreCullingData> m_CullingData;

		[NativeDisableContainerSafetyRestriction]
		public Writer<CullingAction> m_ActionQueue;

		public void Execute(int index)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			PreCullingData preCullingData = m_CullingData[index];
			if ((preCullingData.m_Flags & PreCullingFlags.FadeContainer) != 0)
			{
				return;
			}
			CullingInfo cullingInfo = m_CullingInfoData[preCullingData.m_Entity];
			if ((m_VisibleMask & cullingInfo.m_Mask) == 0)
			{
				m_ActionQueue.Enqueue(new CullingAction
				{
					m_Entity = preCullingData.m_Entity,
					m_UpdateFrame = preCullingData.m_UpdateFrame
				});
			}
			else if (cullingInfo.m_PassedCulling != 0)
			{
				float num = RenderingUtils.CalculateMinDistance(cullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
				if (RenderingUtils.CalculateLod(num * num, m_LodParameters) < cullingInfo.m_MinLod)
				{
					m_ActionQueue.Enqueue(new CullingAction
					{
						m_Entity = preCullingData.m_Entity,
						m_UpdateFrame = preCullingData.m_UpdateFrame
					});
				}
			}
		}
	}

	[Flags]
	public enum ActionFlags : byte
	{
		PassedCulling = 1,
		CrossFade = 2,
		Deleted = 4,
		Applied = 8
	}

	private struct CullingAction
	{
		public Entity m_Entity;

		public ActionFlags m_Flags;

		public sbyte m_UpdateFrame;

		public override int GetHashCode()
		{
			return ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_Entity)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	private struct OverflowAction
	{
		public int m_DataIndex;

		public Entity m_Entity;

		public sbyte m_UpdateFrame;
	}

	[BurstCompile]
	private struct CullingActionJob : IJobParallelFor
	{
		[ReadOnly]
		public Reader<CullingAction> m_CullingActions;

		public ParallelWriter<OverflowAction> m_OverflowActions;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CullingInfo> m_CullingInfo;

		[NativeDisableParallelForRestriction]
		public NativeList<PreCullingData> m_CullingData;

		[NativeDisableParallelForRestriction]
		public NativeReference<int> m_CullingDataIndex;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<CullingAction> enumerator = m_CullingActions.GetEnumerator(index);
			while (enumerator.MoveNext())
			{
				CullingAction current = enumerator.Current;
				if ((current.m_Flags & ActionFlags.PassedCulling) != 0)
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

		private unsafe void PassedCulling(CullingAction cullingAction)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			ref CullingInfo valueRW = ref m_CullingInfo.GetRefRW(cullingAction.m_Entity).ValueRW;
			valueRW.m_PassedCulling = 1;
			if (valueRW.m_CullingIndex == 0)
			{
				valueRW.m_CullingIndex = Interlocked.Increment(ref UnsafeUtility.AsRef<int>((void*)NativeReferenceUnsafeUtility.GetUnsafePtr<int>(m_CullingDataIndex))) - 1;
				if (valueRW.m_CullingIndex >= m_CullingData.Capacity)
				{
					m_OverflowActions.Enqueue(new OverflowAction
					{
						m_DataIndex = valueRW.m_CullingIndex,
						m_Entity = cullingAction.m_Entity,
						m_UpdateFrame = cullingAction.m_UpdateFrame
					});
				}
				else
				{
					ref PreCullingData reference = ref UnsafeUtility.ArrayElementAsRef<PreCullingData>((void*)NativeListUnsafeUtility.GetUnsafePtr<PreCullingData>(m_CullingData), valueRW.m_CullingIndex);
					reference.m_Entity = cullingAction.m_Entity;
					reference.m_UpdateFrame = cullingAction.m_UpdateFrame;
					reference.m_Flags = PreCullingFlags.PassedCulling | PreCullingFlags.NearCamera | PreCullingFlags.NearCameraUpdated;
					reference.m_Timer = 0;
				}
			}
			else if (valueRW.m_CullingIndex < m_CullingData.Length)
			{
				ref PreCullingData reference2 = ref UnsafeUtility.ArrayElementAsRef<PreCullingData>((void*)NativeListUnsafeUtility.GetUnsafePtr<PreCullingData>(m_CullingData), valueRW.m_CullingIndex);
				reference2.m_Entity = cullingAction.m_Entity;
				reference2.m_UpdateFrame = cullingAction.m_UpdateFrame;
				reference2.m_Flags |= PreCullingFlags.PassedCulling;
				reference2.m_Timer = 0;
				if ((reference2.m_Flags & PreCullingFlags.NearCamera) == 0)
				{
					reference2.m_Flags |= PreCullingFlags.NearCamera | PreCullingFlags.NearCameraUpdated;
				}
			}
		}

		private unsafe void FailedCulling(CullingAction cullingAction)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			ref CullingInfo valueRW = ref m_CullingInfo.GetRefRW(cullingAction.m_Entity).ValueRW;
			valueRW.m_PassedCulling = 0;
			if (valueRW.m_CullingIndex != 0 && valueRW.m_CullingIndex < m_CullingData.Length)
			{
				ref PreCullingData reference = ref UnsafeUtility.ArrayElementAsRef<PreCullingData>((void*)NativeListUnsafeUtility.GetUnsafePtr<PreCullingData>(m_CullingData), valueRW.m_CullingIndex);
				reference.m_UpdateFrame = cullingAction.m_UpdateFrame;
				reference.m_Flags &= ~PreCullingFlags.PassedCulling;
				if ((cullingAction.m_Flags & ActionFlags.Deleted) != 0)
				{
					reference.m_Flags |= PreCullingFlags.Deleted;
				}
				if ((cullingAction.m_Flags & ActionFlags.Applied) != 0)
				{
					reference.m_Flags |= PreCullingFlags.Applied;
				}
				if ((cullingAction.m_Flags & ActionFlags.CrossFade) == 0)
				{
					reference.m_Timer = byte.MaxValue;
				}
			}
		}
	}

	[BurstCompile]
	private struct ResizeCullingDataJob : IJob
	{
		[ReadOnly]
		public NativeReference<int> m_CullingDataIndex;

		public NativeList<PreCullingData> m_CullingData;

		public NativeList<PreCullingData> m_UpdatedData;

		public NativeQueue<OverflowAction> m_OverflowActions;

		public void Execute()
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			m_CullingData.Resize(math.min(m_CullingDataIndex.Value, m_CullingData.Capacity), (NativeArrayOptions)0);
			m_CullingData.Resize(m_CullingDataIndex.Value, (NativeArrayOptions)0);
			m_UpdatedData.Clear();
			if (m_CullingData.Length > m_UpdatedData.Capacity)
			{
				m_UpdatedData.Capacity = m_CullingData.Length;
			}
			OverflowAction overflowAction = default(OverflowAction);
			while (m_OverflowActions.TryDequeue(ref overflowAction))
			{
				ref PreCullingData reference = ref m_CullingData.ElementAt(overflowAction.m_DataIndex);
				reference.m_Entity = overflowAction.m_Entity;
				reference.m_UpdateFrame = overflowAction.m_UpdateFrame;
				reference.m_Flags = PreCullingFlags.PassedCulling | PreCullingFlags.NearCamera | PreCullingFlags.NearCameraUpdated;
				reference.m_Timer = 0;
			}
		}
	}

	[BurstCompile]
	private struct FilterUpdatesJob : IJobParallelForDefer
	{
		[ReadOnly]
		public ComponentLookup<Created> m_CreatedData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<Applied> m_AppliedData;

		[ReadOnly]
		public ComponentLookup<BatchesUpdated> m_BatchesUpdatedData;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> m_InterpolatedTransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Object> m_ObjectData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometry> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Color> m_ObjectColorData;

		[ReadOnly]
		public ComponentLookup<Plant> m_PlantData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Relative> m_RelativeData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Extension> m_ExtensionData;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Node> m_NodeData;

		[ReadOnly]
		public ComponentLookup<Lane> m_LaneData;

		[ReadOnly]
		public ComponentLookup<NodeColor> m_NodeColorData;

		[ReadOnly]
		public ComponentLookup<EdgeColor> m_EdgeColorData;

		[ReadOnly]
		public ComponentLookup<LaneColor> m_LaneColorData;

		[ReadOnly]
		public ComponentLookup<LaneCondition> m_LaneConditionData;

		[ReadOnly]
		public ComponentLookup<Block> m_ZoneData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public BufferLookup<Animated> m_AnimatedData;

		[ReadOnly]
		public BufferLookup<Skeleton> m_SkeletonData;

		[ReadOnly]
		public BufferLookup<Emissive> m_EmissiveData;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColorData;

		[ReadOnly]
		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public BufferLookup<EnabledEffect> m_EffectInstances;

		[ReadOnly]
		public int m_TimerDelta;

		[NativeDisableParallelForRestriction]
		public NativeList<PreCullingData> m_CullingData;

		public ParallelWriter<PreCullingData> m_UpdatedCullingData;

		public void Execute(int index)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			ref PreCullingData reference = ref m_CullingData.ElementAt(index);
			if ((reference.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated)) != 0)
			{
				reference.m_Flags &= ~(PreCullingFlags.Updated | PreCullingFlags.Created | PreCullingFlags.Applied | PreCullingFlags.BatchesUpdated | PreCullingFlags.Temp | PreCullingFlags.Object | PreCullingFlags.Net | PreCullingFlags.Lane | PreCullingFlags.Zone | PreCullingFlags.InfoviewColor | PreCullingFlags.BuildingState | PreCullingFlags.TreeGrowth | PreCullingFlags.LaneCondition | PreCullingFlags.InterpolatedTransform | PreCullingFlags.Animated | PreCullingFlags.Skeleton | PreCullingFlags.Emissive | PreCullingFlags.VehicleLayout | PreCullingFlags.EffectInstances | PreCullingFlags.Relative | PreCullingFlags.SurfaceState | PreCullingFlags.SurfaceDamage | PreCullingFlags.SmoothColor);
				if (m_CreatedData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Updated | PreCullingFlags.Created;
				}
				if ((reference.m_Flags & PreCullingFlags.Updated) != 0 || m_UpdatedData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Updated;
				}
				if (m_AppliedData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Applied;
				}
				if (m_BatchesUpdatedData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.BatchesUpdated;
				}
				if (m_TempData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Temp;
				}
				if (m_EffectInstances.HasBuffer(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.EffectInstances;
				}
				if (m_ObjectData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Object;
					if (m_ObjectGeometryData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.SurfaceState;
					}
					if (m_InterpolatedTransformData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.InterpolatedTransform;
					}
					if (m_AnimatedData.HasBuffer(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.Animated;
					}
					if (m_ObjectColorData.HasComponent(reference.m_Entity) || m_OwnerData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.InfoviewColor;
					}
					if (m_BuildingData.HasComponent(reference.m_Entity) || m_ExtensionData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.BuildingState;
					}
					if (m_PlantData.HasComponent(reference.m_Entity))
					{
						if (m_TreeData.HasComponent(reference.m_Entity))
						{
							reference.m_Flags |= PreCullingFlags.TreeGrowth;
						}
						if (m_MeshColorData.HasBuffer(reference.m_Entity))
						{
							reference.m_Flags |= PreCullingFlags.SmoothColor;
						}
					}
					if (m_SkeletonData.HasBuffer(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.Skeleton;
					}
					if (m_EmissiveData.HasBuffer(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.Emissive;
					}
					if (m_LayoutElements.HasBuffer(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.VehicleLayout;
					}
					if (m_RelativeData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.Relative;
					}
					if (m_DamagedData.HasComponent(reference.m_Entity) || m_OnFireData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.SurfaceDamage;
					}
				}
				else if (m_EdgeData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Net;
					if (m_EdgeColorData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.InfoviewColor;
					}
				}
				else if (m_NodeData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Net;
					if (m_NodeColorData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.InfoviewColor;
					}
				}
				else if (m_LaneData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Lane;
					if (m_PlantData.HasComponent(reference.m_Entity) && m_MeshColorData.HasBuffer(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.SmoothColor;
					}
					if (m_LaneColorData.HasComponent(reference.m_Entity) || m_OwnerData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.InfoviewColor;
					}
					if (m_LaneConditionData.HasComponent(reference.m_Entity))
					{
						reference.m_Flags |= PreCullingFlags.LaneCondition;
					}
				}
				else if (m_ZoneData.HasComponent(reference.m_Entity))
				{
					reference.m_Flags |= PreCullingFlags.Zone;
				}
			}
			if ((reference.m_Flags & PreCullingFlags.PassedCulling) == 0)
			{
				int num = reference.m_Timer + m_TimerDelta;
				if (num >= 255)
				{
					reference.m_Flags &= ~PreCullingFlags.NearCamera;
					reference.m_Flags |= PreCullingFlags.NearCameraUpdated;
					reference.m_Timer = byte.MaxValue;
				}
				else
				{
					reference.m_Timer = (byte)num;
				}
			}
			if ((reference.m_Flags & (PreCullingFlags.NearCameraUpdated | PreCullingFlags.Updated | PreCullingFlags.BatchesUpdated | PreCullingFlags.FadeContainer | PreCullingFlags.ColorsUpdated)) != 0)
			{
				m_UpdatedCullingData.AddNoResize(reference);
			}
		}
	}

	private struct TypeHandle
	{
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Updated> __Game_Common_Updated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BatchesUpdated> __Game_Common_BatchesUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Overridden> __Game_Common_Overridden_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stack> __Game_Objects_Stack_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.Marker> __Game_Objects_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<NodeGeometry> __Game_Net_NodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Orphan> __Game_Net_Orphan_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.UtilityLane> __Game_Net_UtilityLane_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Net.Marker> __Game_Net_Marker_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Block> __Game_Zones_Block_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<TransformFrame> __Game_Objects_TransformFrame_RO_BufferTypeHandle;

		public ComponentTypeHandle<CullingInfo> __Game_Rendering_CullingInfo_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StackData> __Game_Prefabs_StackData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetLaneGeometryData> __Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<UtilityLaneData> __Game_Prefabs_UtilityLaneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshRef> __Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionMeshData> __Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetData> __Game_Prefabs_NetData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetGeometryData> __Game_Prefabs_NetGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<RentersUpdated> __Game_Buildings_RentersUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ColorUpdated> __Game_Routes_ColorUpdated_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Applied> __Game_Common_Applied_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle;

		public ComponentLookup<CullingInfo> __Game_Rendering_CullingInfo_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Static> __Game_Objects_Static_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Stopped> __Game_Objects_Stopped_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Created> __Game_Common_Created_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Applied> __Game_Common_Applied_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BatchesUpdated> __Game_Common_BatchesUpdated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<InterpolatedTransform> __Game_Rendering_InterpolatedTransform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Object> __Game_Objects_Object_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometry> __Game_Objects_ObjectGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Color> __Game_Objects_Color_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Relative> __Game_Objects_Relative_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Extension> __Game_Buildings_Extension_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Node> __Game_Net_Node_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Lane> __Game_Net_Lane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NodeColor> __Game_Net_NodeColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeColor> __Game_Net_EdgeColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneColor> __Game_Net_LaneColor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LaneCondition> __Game_Net_LaneCondition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Animated> __Game_Rendering_Animated_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Skeleton> __Game_Rendering_Skeleton_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Emissive> __Game_Rendering_Emissive_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<EnabledEffect> __Game_Effects_EnabledEffect_RO_BufferLookup;

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
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Common_Updated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Updated>(true);
			__Game_Common_BatchesUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BatchesUpdated>(true);
			__Game_Common_Overridden_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Overridden>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_Stack_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stack>(true);
			__Game_Objects_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.Marker>(true);
			__Game_Objects_OutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_NodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NodeGeometry>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Net_Orphan_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Orphan>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_UtilityLane_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.UtilityLane>(true);
			__Game_Net_Marker_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Marker>(true);
			__Game_Zones_Block_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Block>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_TransformFrame_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TransformFrame>(true);
			__Game_Rendering_CullingInfo_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CullingInfo>(false);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_StackData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StackData>(true);
			__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetLaneGeometryData>(true);
			__Game_Prefabs_UtilityLaneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<UtilityLaneData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshRef>(true);
			__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionMeshData>(true);
			__Game_Prefabs_NetData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetData>(true);
			__Game_Prefabs_NetGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetGeometryData>(true);
			__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RentersUpdated>(true);
			__Game_Routes_ColorUpdated_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ColorUpdated>(true);
			__Game_Rendering_CullingInfo_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Vehicles_LayoutElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Common_Applied_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Applied>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentVehicle>(true);
			__Game_Rendering_CullingInfo_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CullingInfo>(false);
			__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InterpolatedTransform>(true);
			__Game_Objects_Static_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Static>(true);
			__Game_Objects_Stopped_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Stopped>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Created_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Created>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Common_Applied_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Applied>(true);
			__Game_Common_BatchesUpdated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BatchesUpdated>(true);
			__Game_Rendering_InterpolatedTransform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InterpolatedTransform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Objects_Object_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Object>(true);
			__Game_Objects_ObjectGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometry>(true);
			__Game_Objects_Color_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Color>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Relative_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Relative>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_Extension_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extension>(true);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Node_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Node>(true);
			__Game_Net_Lane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Lane>(true);
			__Game_Net_NodeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NodeColor>(true);
			__Game_Net_EdgeColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeColor>(true);
			__Game_Net_LaneColor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneColor>(true);
			__Game_Net_LaneCondition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LaneCondition>(true);
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Rendering_Animated_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Animated>(true);
			__Game_Rendering_Skeleton_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Skeleton>(true);
			__Game_Rendering_Emissive_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Emissive>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Effects_EnabledEffect_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<EnabledEffect>(true);
		}
	}

	private RenderingSystem m_RenderingSystem;

	private UndergroundViewSystem m_UndergroundViewSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	private BatchDataSystem m_BatchDataSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private ToolSystem m_ToolSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_InitializeQuery;

	private EntityQuery m_EventQuery;

	private EntityQuery m_CullingInfoQuery;

	private EntityQuery m_TempQuery;

	private float3 m_PrevCameraPosition;

	private float3 m_PrevCameraDirection;

	private float4 m_PrevLodParameters;

	private BoundsMask m_PrevVisibleMask;

	private QueryFlags m_PrevQueryFlags;

	private Dictionary<QueryFlags, EntityQuery> m_CullingQueries;

	private Dictionary<QueryFlags, EntityQuery> m_RelativeQueries;

	private Dictionary<QueryFlags, EntityQuery> m_RemoveQueries;

	private NativeList<PreCullingData> m_CullingData;

	private NativeList<PreCullingData> m_UpdatedData;

	private Entity m_FadeContainer;

	private JobHandle m_WriteDependencies;

	private JobHandle m_ReadDependencies;

	private bool m_ResetPrevious;

	private bool m_Loaded;

	private TypeHandle __TypeHandle;

	public BoundsMask visibleMask { get; private set; }

	public BoundsMask becameVisible { get; private set; }

	public BoundsMask becameHidden { get; private set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_UndergroundViewSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UndergroundViewSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
		m_BatchDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchDataSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CullingInfo>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<BatchesUpdated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array[0] = val;
		m_InitializeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Common.Event>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<RentersUpdated>(),
			ComponentType.ReadOnly<ColorUpdated>()
		};
		array2[0] = val;
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_CullingInfoQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CullingInfo>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadWrite<CullingInfo>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Deleted>() };
		array3[0] = val;
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		m_CullingQueries = new Dictionary<QueryFlags, EntityQuery>();
		m_RelativeQueries = new Dictionary<QueryFlags, EntityQuery>();
		m_RemoveQueries = new Dictionary<QueryFlags, EntityQuery>();
		m_PrevCameraDirection = math.forward();
		m_PrevLodParameters = float4.op_Implicit(1f);
		m_CullingData = new NativeList<PreCullingData>(10000, AllocatorHandle.op_Implicit((Allocator)4));
		m_UpdatedData = new NativeList<PreCullingData>(10000, AllocatorHandle.op_Implicit((Allocator)4));
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_FadeContainer = ((EntityManager)(ref entityManager)).CreateEntity((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<MeshBatch>(),
			ComponentType.ReadWrite<FadeBatch>()
		});
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_WriteDependencies)).Complete();
		((JobHandle)(ref m_ReadDependencies)).Complete();
		m_CullingData.Dispose();
		m_UpdatedData.Dispose();
		base.OnDestroy();
	}

	public void PostDeserialize(Context context)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_WriteDependencies)).Complete();
		((JobHandle)(ref m_ReadDependencies)).Complete();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).GetBuffer<MeshBatch>(m_FadeContainer, false).Clear();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).GetBuffer<FadeBatch>(m_FadeContainer, false).Clear();
		InitializeCullingData();
		ResetCulling();
		m_Loaded = true;
	}

	public void ResetCulling()
	{
		m_ResetPrevious = true;
	}

	public Entity GetFadeContainer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_FadeContainer;
	}

	public NativeList<PreCullingData> GetCullingData(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_WriteDependencies : JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies));
		return m_CullingData;
	}

	public NativeList<PreCullingData> GetUpdatedData(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_WriteDependencies : JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies));
		return m_UpdatedData;
	}

	public void AddCullingDataReader(JobHandle dependencies)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, dependencies);
	}

	public void AddCullingDataWriter(JobHandle dependencies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = dependencies;
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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0922: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0997: Unknown result type (might be due to invalid IL or missing references)
		//IL_09af: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1009: Unknown result type (might be due to invalid IL or missing references)
		//IL_100e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1043: Unknown result type (might be due to invalid IL or missing references)
		//IL_1048: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_109a: Unknown result type (might be due to invalid IL or missing references)
		//IL_109f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_110e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1113: Unknown result type (might be due to invalid IL or missing references)
		//IL_112b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1130: Unknown result type (might be due to invalid IL or missing references)
		//IL_1148: Unknown result type (might be due to invalid IL or missing references)
		//IL_114d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Unknown result type (might be due to invalid IL or missing references)
		//IL_116a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1182: Unknown result type (might be due to invalid IL or missing references)
		//IL_1187: Unknown result type (might be due to invalid IL or missing references)
		//IL_119f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1204: Unknown result type (might be due to invalid IL or missing references)
		//IL_1209: Unknown result type (might be due to invalid IL or missing references)
		//IL_120e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1212: Unknown result type (might be due to invalid IL or missing references)
		//IL_1214: Unknown result type (might be due to invalid IL or missing references)
		//IL_1219: Unknown result type (might be due to invalid IL or missing references)
		//IL_121c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1223: Unknown result type (might be due to invalid IL or missing references)
		//IL_1225: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1232: Unknown result type (might be due to invalid IL or missing references)
		//IL_123f: Unknown result type (might be due to invalid IL or missing references)
		//IL_124c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1259: Unknown result type (might be due to invalid IL or missing references)
		//IL_1262: Unknown result type (might be due to invalid IL or missing references)
		//IL_1264: Unknown result type (might be due to invalid IL or missing references)
		//IL_126c: Unknown result type (might be due to invalid IL or missing references)
		//IL_126e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1276: Unknown result type (might be due to invalid IL or missing references)
		//IL_1278: Unknown result type (might be due to invalid IL or missing references)
		//IL_1280: Unknown result type (might be due to invalid IL or missing references)
		//IL_1282: Unknown result type (might be due to invalid IL or missing references)
		//IL_128a: Unknown result type (might be due to invalid IL or missing references)
		//IL_128c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1293: Unknown result type (might be due to invalid IL or missing references)
		//IL_1294: Unknown result type (might be due to invalid IL or missing references)
		//IL_129a: Unknown result type (might be due to invalid IL or missing references)
		//IL_129b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da3: Unknown result type (might be due to invalid IL or missing references)
		bool loaded = GetLoaded();
		((JobHandle)(ref m_WriteDependencies)).Complete();
		((JobHandle)(ref m_ReadDependencies)).Complete();
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
		BoundsMask boundsMask = BoundsMask.NormalLayers;
		if (m_UndergroundViewSystem.pipelinesOn)
		{
			boundsMask |= BoundsMask.PipelineLayer;
		}
		if (m_UndergroundViewSystem.subPipelinesOn)
		{
			boundsMask |= BoundsMask.SubPipelineLayer;
		}
		if (m_UndergroundViewSystem.waterwaysOn)
		{
			boundsMask |= BoundsMask.WaterwayLayer;
		}
		if (m_RenderingSystem.markersVisible)
		{
			boundsMask |= BoundsMask.Debug;
		}
		if (m_ResetPrevious)
		{
			m_PrevCameraPosition = val;
			m_PrevCameraDirection = val2;
			m_PrevLodParameters = val3;
			m_PrevVisibleMask = (BoundsMask)0;
			visibleMask = boundsMask;
			becameVisible = boundsMask;
			becameHidden = (BoundsMask)0;
		}
		else
		{
			visibleMask = boundsMask;
			becameVisible = (BoundsMask)((uint)boundsMask & (uint)(ushort)(~(int)m_PrevVisibleMask));
			becameHidden = (BoundsMask)((uint)m_PrevVisibleMask & (uint)(ushort)(~(int)boundsMask));
		}
		int length = m_CullingData.Length;
		NativeParallelQueue<CullingAction> val4 = default(NativeParallelQueue<CullingAction>);
		val4._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeReference<int> cullingDataIndex = default(NativeReference<int>);
		cullingDataIndex._002Ector(length, AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<OverflowAction> overflowActions = default(NativeQueue<OverflowAction>);
		overflowActions._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<int> nodeBuffer = default(NativeArray<int>);
		nodeBuffer._002Ector(1536, (Allocator)3, (NativeArrayOptions)0);
		NativeArray<int> subDataBuffer = default(NativeArray<int>);
		subDataBuffer._002Ector(1536, (Allocator)3, (NativeArrayOptions)0);
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		TreeCullingJob1 treeCullingJob = new TreeCullingJob1
		{
			m_StaticObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies),
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
			m_LaneSearchTree = m_NetSearchSystem.GetLaneSearchTree(readOnly: true, out dependencies3),
			m_LodParameters = val3,
			m_PrevLodParameters = m_PrevLodParameters,
			m_CameraPosition = val,
			m_PrevCameraPosition = m_PrevCameraPosition,
			m_CameraDirection = val2,
			m_PrevCameraDirection = m_PrevCameraDirection,
			m_VisibleMask = boundsMask,
			m_PrevVisibleMask = m_PrevVisibleMask,
			m_NodeBuffer = nodeBuffer,
			m_SubDataBuffer = subDataBuffer,
			m_ActionQueue = val4.AsWriter()
		};
		TreeCullingJob2 obj = new TreeCullingJob2
		{
			m_StaticObjectSearchTree = treeCullingJob.m_StaticObjectSearchTree,
			m_NetSearchTree = treeCullingJob.m_NetSearchTree,
			m_LaneSearchTree = treeCullingJob.m_LaneSearchTree,
			m_LodParameters = val3,
			m_PrevLodParameters = m_PrevLodParameters,
			m_CameraPosition = val,
			m_PrevCameraPosition = m_PrevCameraPosition,
			m_CameraDirection = val2,
			m_PrevCameraDirection = m_PrevCameraDirection,
			m_VisibleMask = boundsMask,
			m_PrevVisibleMask = m_PrevVisibleMask,
			m_NodeBuffer = nodeBuffer,
			m_SubDataBuffer = subDataBuffer,
			m_ActionQueue = val4.AsWriter()
		};
		JobHandle val5 = IJobParallelForExtensions.Schedule<TreeCullingJob1>(treeCullingJob, 3, 1, JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3));
		JobHandle val6 = IJobParallelForExtensions.Schedule<TreeCullingJob2>(obj, nodeBuffer.Length, 1, val5);
		JobHandle.ScheduleBatchedJobs();
		m_BatchMeshSystem.CompleteCaching();
		QueryFlags queryFlags = GetQueryFlags();
		InitializeCullingJob initializeCullingJob = new InitializeCullingJob
		{
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedType = InternalCompilerInterface.GetComponentTypeHandle<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BatchesUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<BatchesUpdated>(ref __TypeHandle.__Game_Common_BatchesUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OverriddenType = InternalCompilerInterface.GetComponentTypeHandle<Overridden>(ref __TypeHandle.__Game_Common_Overridden_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StackType = InternalCompilerInterface.GetComponentTypeHandle<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectMarkerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.Marker>(ref __TypeHandle.__Game_Objects_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<NodeGeometry>(ref __TypeHandle.__Game_Net_NodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StartNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EndNodeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OrphanType = InternalCompilerInterface.GetComponentTypeHandle<Orphan>(ref __TypeHandle.__Game_Net_Orphan_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UtilityLaneType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.UtilityLane>(ref __TypeHandle.__Game_Net_UtilityLane_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetMarkerType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Marker>(ref __TypeHandle.__Game_Net_Marker_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneBlockType = InternalCompilerInterface.GetComponentTypeHandle<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabLaneGeometryData = InternalCompilerInterface.GetComponentLookup<NetLaneGeometryData>(ref __TypeHandle.__Game_Prefabs_NetLaneGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabUtilityLaneData = InternalCompilerInterface.GetComponentLookup<UtilityLaneData>(ref __TypeHandle.__Game_Prefabs_UtilityLaneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionMeshRef = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshRef>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCompositionMeshData = InternalCompilerInterface.GetComponentLookup<NetCompositionMeshData>(ref __TypeHandle.__Game_Prefabs_NetCompositionMeshData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetData = InternalCompilerInterface.GetComponentLookup<NetData>(ref __TypeHandle.__Game_Prefabs_NetData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabNetGeometryData = InternalCompilerInterface.GetComponentLookup<NetGeometryData>(ref __TypeHandle.__Game_Prefabs_NetGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_UpdateAll = loaded,
			m_UnspawnedVisible = m_RenderingSystem.unspawnedVisible,
			m_DilatedUtilityTypes = m_UndergroundViewSystem.utilityTypes,
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_CullingData = m_CullingData
		};
		EventCullingJob eventCullingJob = new EventCullingJob
		{
			m_RentersUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<RentersUpdated>(ref __TypeHandle.__Game_Buildings_RentersUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ColorUpdatedType = InternalCompilerInterface.GetComponentTypeHandle<ColorUpdated>(ref __TypeHandle.__Game_Routes_ColorUpdated_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingData = m_CullingData
		};
		QueryCullingJob queryCullingJob = new QueryCullingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LodParameters = val3,
			m_CameraPosition = val,
			m_CameraDirection = val2,
			m_FrameIndex = m_RenderingSystem.frameIndex,
			m_FrameTime = m_RenderingSystem.frameTime,
			m_VisibleMask = boundsMask,
			m_ActionQueue = val4.AsWriter()
		};
		QueryRemoveJob queryRemoveJob = new QueryRemoveJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedType = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AppliedType = InternalCompilerInterface.GetComponentTypeHandle<Applied>(ref __TypeHandle.__Game_Common_Applied_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformFrameType = InternalCompilerInterface.GetBufferTypeHandle<TransformFrame>(ref __TypeHandle.__Game_Objects_TransformFrame_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoType = InternalCompilerInterface.GetComponentTypeHandle<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ActionQueue = val4.AsWriter()
		};
		RelativeCullingJob relativeCullingJob = new RelativeCullingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleType = InternalCompilerInterface.GetComponentTypeHandle<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodParameters = val3,
			m_CameraPosition = val,
			m_CameraDirection = val2,
			m_VisibleMask = boundsMask,
			m_ActionQueue = val4.AsWriter()
		};
		TempCullingJob obj2 = new TempCullingJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformType = InternalCompilerInterface.GetComponentTypeHandle<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StackType = InternalCompilerInterface.GetComponentTypeHandle<Stack>(ref __TypeHandle.__Game_Objects_Stack_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StaticType = InternalCompilerInterface.GetComponentTypeHandle<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedType = InternalCompilerInterface.GetComponentTypeHandle<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabStackData = InternalCompilerInterface.GetComponentLookup<StackData>(ref __TypeHandle.__Game_Prefabs_StackData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LodParameters = val3,
			m_CameraPosition = val,
			m_CameraDirection = val2,
			m_VisibleMask = boundsMask,
			m_TerrainHeightData = initializeCullingJob.m_TerrainHeightData,
			m_ActionQueue = val4.AsWriter()
		};
		EntityQuery val7 = (loaded ? m_CullingInfoQuery : m_InitializeQuery);
		EntityQuery cullingQuery = GetCullingQuery(queryFlags);
		EntityQuery relativeQuery = GetRelativeQuery(queryFlags);
		EntityQuery removeQuery = GetRemoveQuery(m_PrevQueryFlags & ~queryFlags);
		JobHandle val8 = JobChunkExtensions.ScheduleParallel<InitializeCullingJob>(initializeCullingJob, val7, ((SystemBase)this).Dependency);
		JobHandle val9 = JobChunkExtensions.Schedule<EventCullingJob>(eventCullingJob, m_EventQuery, val8);
		JobHandle val10 = JobChunkExtensions.ScheduleParallel<QueryCullingJob>(queryCullingJob, cullingQuery, val9);
		JobHandle val11 = JobChunkExtensions.ScheduleParallel<QueryRemoveJob>(queryRemoveJob, removeQuery, val10);
		JobHandle val12 = JobChunkExtensions.ScheduleParallel<RelativeCullingJob>(relativeCullingJob, relativeQuery, val11);
		JobHandle val13 = JobChunkExtensions.ScheduleParallel<TempCullingJob>(obj2, m_TempQuery, val12);
		if (m_ResetPrevious || becameHidden != 0)
		{
			JobHandle val14 = IJobParallelForExtensions.Schedule<VerifyVisibleJob>(new VerifyVisibleJob
			{
				m_CullingInfoData = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LodParameters = val3,
				m_CameraPosition = val,
				m_CameraDirection = val2,
				m_VisibleMask = boundsMask,
				m_CullingData = m_CullingData,
				m_ActionQueue = val4.AsWriter()
			}, length, 16, val13);
			m_WriteDependencies = JobHandle.CombineDependencies(val6, val14);
		}
		else
		{
			m_WriteDependencies = JobHandle.CombineDependencies(val6, val13);
		}
		CullingActionJob cullingActionJob = new CullingActionJob
		{
			m_CullingActions = val4.AsReader(),
			m_OverflowActions = overflowActions.AsParallelWriter(),
			m_CullingInfo = InternalCompilerInterface.GetComponentLookup<CullingInfo>(ref __TypeHandle.__Game_Rendering_CullingInfo_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CullingData = m_CullingData,
			m_CullingDataIndex = cullingDataIndex
		};
		ResizeCullingDataJob resizeCullingDataJob = new ResizeCullingDataJob
		{
			m_CullingDataIndex = cullingDataIndex,
			m_CullingData = m_CullingData,
			m_UpdatedData = m_UpdatedData,
			m_OverflowActions = overflowActions
		};
		FilterUpdatesJob obj3 = new FilterUpdatesJob
		{
			m_CreatedData = InternalCompilerInterface.GetComponentLookup<Created>(ref __TypeHandle.__Game_Common_Created_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AppliedData = InternalCompilerInterface.GetComponentLookup<Applied>(ref __TypeHandle.__Game_Common_Applied_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BatchesUpdatedData = InternalCompilerInterface.GetComponentLookup<BatchesUpdated>(ref __TypeHandle.__Game_Common_BatchesUpdated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InterpolatedTransformData = InternalCompilerInterface.GetComponentLookup<InterpolatedTransform>(ref __TypeHandle.__Game_Rendering_InterpolatedTransform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Object>(ref __TypeHandle.__Game_Objects_Object_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometry>(ref __TypeHandle.__Game_Objects_ObjectGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ObjectColorData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Color>(ref __TypeHandle.__Game_Objects_Color_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RelativeData = InternalCompilerInterface.GetComponentLookup<Relative>(ref __TypeHandle.__Game_Objects_Relative_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtensionData = InternalCompilerInterface.GetComponentLookup<Extension>(ref __TypeHandle.__Game_Buildings_Extension_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeData = InternalCompilerInterface.GetComponentLookup<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneData = InternalCompilerInterface.GetComponentLookup<Lane>(ref __TypeHandle.__Game_Net_Lane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NodeColorData = InternalCompilerInterface.GetComponentLookup<NodeColor>(ref __TypeHandle.__Game_Net_NodeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeColorData = InternalCompilerInterface.GetComponentLookup<EdgeColor>(ref __TypeHandle.__Game_Net_EdgeColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneColorData = InternalCompilerInterface.GetComponentLookup<LaneColor>(ref __TypeHandle.__Game_Net_LaneColor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LaneConditionData = InternalCompilerInterface.GetComponentLookup<LaneCondition>(ref __TypeHandle.__Game_Net_LaneCondition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimatedData = InternalCompilerInterface.GetBufferLookup<Animated>(ref __TypeHandle.__Game_Rendering_Animated_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SkeletonData = InternalCompilerInterface.GetBufferLookup<Skeleton>(ref __TypeHandle.__Game_Rendering_Skeleton_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EmissiveData = InternalCompilerInterface.GetBufferLookup<Emissive>(ref __TypeHandle.__Game_Rendering_Emissive_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshColorData = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EffectInstances = InternalCompilerInterface.GetBufferLookup<EnabledEffect>(ref __TypeHandle.__Game_Effects_EnabledEffect_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TimerDelta = m_RenderingSystem.lodTimerDelta,
			m_CullingData = m_CullingData,
			m_UpdatedCullingData = m_UpdatedData.AsParallelWriter()
		};
		JobHandle val15 = IJobParallelForExtensions.Schedule<CullingActionJob>(cullingActionJob, val4.HashRange, 1, m_WriteDependencies);
		JobHandle val16 = IJobExtensions.Schedule<ResizeCullingDataJob>(resizeCullingDataJob, val15);
		JobHandle val17 = IJobParallelForDeferExtensions.Schedule<FilterUpdatesJob, PreCullingData>(obj3, m_CullingData, 16, val16);
		m_ObjectSearchSystem.AddStaticSearchTreeReader(val6);
		m_NetSearchSystem.AddNetSearchTreeReader(val6);
		m_NetSearchSystem.AddLaneSearchTreeReader(val6);
		m_TerrainSystem.AddCPUHeightReader(val13);
		val4.Dispose(val15);
		cullingDataIndex.Dispose(val16);
		overflowActions.Dispose(val16);
		nodeBuffer.Dispose(val6);
		subDataBuffer.Dispose(val6);
		m_PrevCameraPosition = val;
		m_PrevCameraDirection = val2;
		m_PrevLodParameters = val3;
		m_PrevVisibleMask = boundsMask;
		m_PrevQueryFlags = queryFlags;
		m_ResetPrevious = false;
		m_WriteDependencies = val17;
		m_ReadDependencies = default(JobHandle);
		((SystemBase)this).Dependency = val17;
	}

	private void InitializeCullingData()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		m_CullingData.Clear();
		ref NativeList<PreCullingData> reference = ref m_CullingData;
		PreCullingData preCullingData = new PreCullingData
		{
			m_Entity = m_FadeContainer,
			m_Flags = (PreCullingFlags.PassedCulling | PreCullingFlags.NearCamera | PreCullingFlags.FadeContainer)
		};
		reference.Add(ref preCullingData);
	}

	private EntityQuery GetCullingQuery(QueryFlags flags)
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!m_CullingQueries.TryGetValue(flags, out var value))
		{
			List<ComponentType> list = new List<ComponentType>
			{
				ComponentType.ReadOnly<Moving>(),
				ComponentType.ReadOnly<Stopped>(),
				ComponentType.ReadOnly<Updated>()
			};
			List<ComponentType> list2 = new List<ComponentType>
			{
				ComponentType.ReadOnly<Relative>(),
				ComponentType.ReadOnly<Temp>(),
				ComponentType.ReadOnly<Deleted>()
			};
			if ((flags & QueryFlags.Unspawned) == 0)
			{
				list2.Add(ComponentType.ReadOnly<Unspawned>());
			}
			if ((flags & QueryFlags.Zones) != 0)
			{
				list.Add(ComponentType.ReadOnly<Block>());
			}
			else
			{
				list2.Add(ComponentType.ReadOnly<Block>());
			}
			EntityQueryDesc[] array = new EntityQueryDesc[1];
			EntityQueryDesc val = new EntityQueryDesc();
			val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<CullingInfo>() };
			val.Any = list.ToArray();
			val.None = list2.ToArray();
			array[0] = val;
			value = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
			m_CullingQueries.Add(flags, value);
		}
		return value;
	}

	private EntityQuery GetRelativeQuery(QueryFlags flags)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		flags &= QueryFlags.Unspawned;
		if (!m_RelativeQueries.TryGetValue(flags, out var value))
		{
			List<ComponentType> list = new List<ComponentType>
			{
				ComponentType.ReadOnly<Temp>(),
				ComponentType.ReadOnly<Deleted>()
			};
			if ((flags & QueryFlags.Unspawned) == 0)
			{
				list.Add(ComponentType.ReadOnly<Unspawned>());
			}
			EntityQueryDesc[] array = new EntityQueryDesc[1];
			EntityQueryDesc val = new EntityQueryDesc();
			val.All = (ComponentType[])(object)new ComponentType[2]
			{
				ComponentType.ReadOnly<Relative>(),
				ComponentType.ReadWrite<CullingInfo>()
			};
			val.None = list.ToArray();
			array[0] = val;
			value = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
			m_RelativeQueries.Add(flags, value);
		}
		return value;
	}

	private EntityQuery GetRemoveQuery(QueryFlags flags)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!m_RemoveQueries.TryGetValue(flags, out var value))
		{
			List<ComponentType> list = new List<ComponentType> { ComponentType.ReadOnly<Deleted>() };
			if ((flags & QueryFlags.Zones) != 0)
			{
				list.Add(ComponentType.ReadOnly<Block>());
			}
			if ((flags & QueryFlags.Unspawned) != 0)
			{
				list.Add(ComponentType.ReadOnly<Unspawned>());
			}
			EntityQueryDesc[] array = new EntityQueryDesc[1];
			EntityQueryDesc val = new EntityQueryDesc();
			val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<CullingInfo>() };
			val.Any = list.ToArray();
			array[0] = val;
			value = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
			m_RemoveQueries.Add(flags, value);
		}
		return value;
	}

	private QueryFlags GetQueryFlags()
	{
		QueryFlags queryFlags = (QueryFlags)0;
		if (m_RenderingSystem.unspawnedVisible)
		{
			queryFlags |= QueryFlags.Unspawned;
		}
		if (m_ToolSystem.activeTool != null && m_ToolSystem.activeTool.requireZones)
		{
			queryFlags |= QueryFlags.Zones;
		}
		return queryFlags;
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
	public PreCullingSystem()
	{
	}
}
