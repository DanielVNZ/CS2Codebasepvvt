using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Objects;
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
public class CurrentDistrictSystem : GameSystemBase
{
	[BurstCompile]
	private struct FindUpdatedDistrictItemsJob : IJobParallelForDefer
	{
		private struct ObjectIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

			public ParallelWriter<Entity> m_UpdateBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && m_CurrentDistrictData.HasComponent(entity))
				{
					m_UpdateBuffer.Enqueue(entity);
				}
			}
		}

		private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
		{
			public Bounds2 m_Bounds;

			public ComponentLookup<BorderDistrict> m_BorderDistrictData;

			public ParallelWriter<Entity> m_UpdateBuffer;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Bounds) && m_BorderDistrictData.HasComponent(entity))
				{
					m_UpdateBuffer.Enqueue(entity);
				}
			}
		}

		[ReadOnly]
		public NativeArray<Bounds2> m_Bounds;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_ObjectTree;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetTree;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		public ParallelWriter<Entity> m_UpdateBuffer;

		public void Execute(int index)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			ObjectIterator objectIterator = new ObjectIterator
			{
				m_Bounds = m_Bounds[index],
				m_CurrentDistrictData = m_CurrentDistrictData,
				m_UpdateBuffer = m_UpdateBuffer
			};
			m_ObjectTree.Iterate<ObjectIterator>(ref objectIterator, 0);
			NetIterator netIterator = new NetIterator
			{
				m_Bounds = m_Bounds[index],
				m_BorderDistrictData = m_BorderDistrictData,
				m_UpdateBuffer = m_UpdateBuffer
			};
			m_NetTree.Iterate<NetIterator>(ref netIterator, 0);
		}
	}

	[BurstCompile]
	private struct CollectUpdatedDistrictItemsJob : IJob
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

		public NativeQueue<Entity> m_UpdateBuffer;

		public NativeList<Entity> m_UpdateList;

		public void Execute()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			int count = m_UpdateBuffer.Count;
			m_UpdateList.ResizeUninitialized(count);
			for (int i = 0; i < count; i++)
			{
				m_UpdateList[i] = m_UpdateBuffer.Dequeue();
			}
			NativeSortExtension.Sort<Entity, EntityComparer>(m_UpdateList, default(EntityComparer));
			Entity val = Entity.Null;
			int num = 0;
			int num2 = 0;
			while (num < m_UpdateList.Length)
			{
				Entity val2 = m_UpdateList[num++];
				if (val2 != val)
				{
					m_UpdateList[num2++] = val2;
					val = val2;
				}
			}
			if (num2 < m_UpdateList.Length)
			{
				m_UpdateList.RemoveRange(num2, m_UpdateList.Length - num2);
			}
		}
	}

	[BurstCompile]
	private struct FindDistrictParallelJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeArray<Entity> m_UpdateList;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		public ParallelWriter m_CommandBuffer;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Updated> m_UpdatedData;

		[ReadOnly]
		public ComponentLookup<CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> m_PedestrianLaneData;

		[ReadOnly]
		public BufferLookup<SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_UpdateList[index];
			DistrictIterator districtIterator = new DistrictIterator
			{
				m_DistrictData = m_DistrictData,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles
			};
			if (m_CurrentDistrictData.HasComponent(val))
			{
				CurrentDistrict currentDistrict = m_CurrentDistrictData[val];
				Transform transform = m_TransformData[val];
				districtIterator.m_Position = ((float3)(ref transform.m_Position)).xz;
				districtIterator.m_Result = Entity.Null;
				m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
				if (currentDistrict.m_District != districtIterator.m_Result)
				{
					m_CurrentDistrictData[val] = new CurrentDistrict(districtIterator.m_Result);
					CheckChangedDistrict(index, val);
				}
			}
			if (m_BorderDistrictData.HasComponent(val))
			{
				BorderDistrict borderDistrict = m_BorderDistrictData[val];
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[val];
				districtIterator.m_Position = ((float3)(ref edgeGeometry.m_Start.m_Left.d)).xz;
				districtIterator.m_Result = Entity.Null;
				m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
				Entity val2 = districtIterator.m_Result;
				districtIterator.m_Position = ((float3)(ref edgeGeometry.m_Start.m_Right.d)).xz;
				districtIterator.m_Result = Entity.Null;
				m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
				Entity val3 = districtIterator.m_Result;
				if (borderDistrict.m_Left != val2 || borderDistrict.m_Right != val3)
				{
					m_BorderDistrictData[val] = new BorderDistrict(val2, val3);
					CheckChangedDistrict(index, val);
				}
			}
		}

		private void CheckChangedDistrict(int jobIndex, Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if (m_UpdatedData.HasComponent(entity) || !m_SubLanes.HasBuffer(entity))
			{
				return;
			}
			DynamicBuffer<SubLane> val = m_SubLanes[entity];
			for (int i = 0; i < val.Length; i++)
			{
				Entity subLane = val[i].m_SubLane;
				if (m_CarLaneData.HasComponent(subLane) || m_PedestrianLaneData.HasComponent(subLane) || m_ParkingLaneData.HasComponent(subLane))
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(jobIndex, subLane);
				}
			}
		}
	}

	[BurstCompile]
	private struct FindDistrictChunkJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> m_EdgeGeometryType;

		public ComponentTypeHandle<CurrentDistrict> m_CurrentDistrictType;

		public ComponentTypeHandle<BorderDistrict> m_BorderDistrictType;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public BufferLookup<Node> m_Nodes;

		[ReadOnly]
		public BufferLookup<Triangle> m_Triangles;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CurrentDistrict> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentDistrict>(ref m_CurrentDistrictType);
			NativeArray<BorderDistrict> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BorderDistrict>(ref m_BorderDistrictType);
			DistrictIterator districtIterator = new DistrictIterator
			{
				m_DistrictData = m_DistrictData,
				m_Nodes = m_Nodes,
				m_Triangles = m_Triangles
			};
			if (nativeArray.Length != 0)
			{
				NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
				for (int i = 0; i < nativeArray3.Length; i++)
				{
					Transform transform = nativeArray3[i];
					districtIterator.m_Position = ((float3)(ref transform.m_Position)).xz;
					districtIterator.m_Result = Entity.Null;
					m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
					nativeArray[i] = new CurrentDistrict(districtIterator.m_Result);
				}
			}
			if (nativeArray2.Length != 0)
			{
				NativeArray<EdgeGeometry> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<EdgeGeometry>(ref m_EdgeGeometryType);
				for (int j = 0; j < nativeArray4.Length; j++)
				{
					EdgeGeometry edgeGeometry = nativeArray4[j];
					districtIterator.m_Position = ((float3)(ref edgeGeometry.m_Start.m_Left.d)).xz;
					districtIterator.m_Result = Entity.Null;
					m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
					Entity left = districtIterator.m_Result;
					districtIterator.m_Position = ((float3)(ref edgeGeometry.m_Start.m_Right.d)).xz;
					districtIterator.m_Result = Entity.Null;
					m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
					Entity right = districtIterator.m_Result;
					nativeArray2[j] = new BorderDistrict(left, right);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct DistrictIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
	{
		public float2 m_Position;

		public ComponentLookup<District> m_DistrictData;

		public BufferLookup<Node> m_Nodes;

		public BufferLookup<Triangle> m_Triangles;

		public Entity m_Result;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position) && m_DistrictData.HasComponent(areaItem.m_Area))
			{
				DynamicBuffer<Node> nodes = m_Nodes[areaItem.m_Area];
				DynamicBuffer<Triangle> val = m_Triangles[areaItem.m_Area];
				float2 val2 = default(float2);
				if (val.Length > areaItem.m_Triangle && MathUtils.Intersect(AreaUtils.GetTriangle2(nodes, val[areaItem.m_Triangle]), m_Position, ref val2))
				{
					m_Result = areaItem.m_Area;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<District> __Game_Areas_District_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PedestrianLane> __Game_Net_PedestrianLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public ComponentLookup<CurrentDistrict> __Game_Areas_CurrentDistrict_RW_ComponentLookup;

		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RW_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentTypeHandle;

		public ComponentTypeHandle<CurrentDistrict> __Game_Areas_CurrentDistrict_RW_ComponentTypeHandle;

		public ComponentTypeHandle<BorderDistrict> __Game_Areas_BorderDistrict_RW_ComponentTypeHandle;

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
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			__Game_Areas_CurrentDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Areas_District_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<District>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarLane>(true);
			__Game_Net_PedestrianLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PedestrianLane>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkingLane>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<SubLane>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Areas_CurrentDistrict_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentDistrict>(false);
			__Game_Areas_BorderDistrict_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(false);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Net_EdgeGeometry_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EdgeGeometry>(true);
			__Game_Areas_CurrentDistrict_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentDistrict>(false);
			__Game_Areas_BorderDistrict_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BorderDistrict>(false);
		}
	}

	private UpdateCollectSystem m_UpdateCollectSystem;

	private SearchSystem m_AreaSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_CurrentDistrictQuery;

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
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_UpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateCollectSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Updated>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CurrentDistrict>(),
			ComponentType.ReadOnly<BorderDistrict>()
		};
		array[0] = val;
		m_CurrentDistrictQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_CurrentDistrictQuery)).IsEmptyIgnoreFilter || m_UpdateCollectSystem.districtsUpdated)
		{
			if (m_UpdateCollectSystem.districtsUpdated)
			{
				NativeQueue<Entity> updateBuffer = default(NativeQueue<Entity>);
				updateBuffer._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				NativeList<Entity> val = default(NativeList<Entity>);
				val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
				JobHandle dependencies;
				NativeList<Bounds2> updatedDistrictBounds = m_UpdateCollectSystem.GetUpdatedDistrictBounds(out dependencies);
				JobHandle dependencies2;
				JobHandle dependencies3;
				FindUpdatedDistrictItemsJob findUpdatedDistrictItemsJob = new FindUpdatedDistrictItemsJob
				{
					m_Bounds = updatedDistrictBounds.AsDeferredJobArray(),
					m_ObjectTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies2),
					m_NetTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies3),
					m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_UpdateBuffer = updateBuffer.AsParallelWriter()
				};
				CollectUpdatedDistrictItemsJob collectUpdatedDistrictItemsJob = new CollectUpdatedDistrictItemsJob
				{
					m_UpdateBuffer = updateBuffer,
					m_UpdateList = val
				};
				JobHandle dependencies4;
				FindDistrictParallelJob findDistrictParallelJob = new FindDistrictParallelJob
				{
					m_UpdateList = val.AsDeferredJobArray(),
					m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies4)
				};
				EntityCommandBuffer val2 = m_ModificationBarrier.CreateCommandBuffer();
				findDistrictParallelJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
				findDistrictParallelJob.m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_DistrictData = InternalCompilerInterface.GetComponentLookup<District>(ref __TypeHandle.__Game_Areas_District_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_CarLaneData = InternalCompilerInterface.GetComponentLookup<CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_PedestrianLaneData = InternalCompilerInterface.GetComponentLookup<PedestrianLane>(ref __TypeHandle.__Game_Net_PedestrianLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_SubLanes = InternalCompilerInterface.GetBufferLookup<SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_CurrentDistrictData = InternalCompilerInterface.GetComponentLookup<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				findDistrictParallelJob.m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
				FindDistrictParallelJob findDistrictParallelJob2 = findDistrictParallelJob;
				JobHandle val3 = JobHandle.CombineDependencies(dependencies, dependencies2, dependencies3);
				JobHandle val4 = IJobParallelForDeferExtensions.Schedule<FindUpdatedDistrictItemsJob, Bounds2>(findUpdatedDistrictItemsJob, updatedDistrictBounds, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
				JobHandle val5 = IJobExtensions.Schedule<CollectUpdatedDistrictItemsJob>(collectUpdatedDistrictItemsJob, val4);
				JobHandle val6 = IJobParallelForDeferExtensions.Schedule<FindDistrictParallelJob, Entity>(findDistrictParallelJob2, val, 1, JobHandle.CombineDependencies(val5, dependencies4));
				updateBuffer.Dispose(val5);
				val.Dispose(val6);
				m_UpdateCollectSystem.AddDistrictBoundsReader(val4);
				m_ObjectSearchSystem.AddStaticSearchTreeReader(val4);
				m_NetSearchSystem.AddNetSearchTreeReader(val4);
				m_AreaSearchSystem.AddSearchTreeReader(val6);
				((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val6);
				((SystemBase)this).Dependency = val6;
			}
			if (!((EntityQuery)(ref m_CurrentDistrictQuery)).IsEmptyIgnoreFilter)
			{
				JobHandle dependencies5;
				JobHandle val7 = JobChunkExtensions.ScheduleParallel<FindDistrictChunkJob>(new FindDistrictChunkJob
				{
					m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_EdgeGeometryType = InternalCompilerInterface.GetComponentTypeHandle<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_CurrentDistrictType = InternalCompilerInterface.GetComponentTypeHandle<CurrentDistrict>(ref __TypeHandle.__Game_Areas_CurrentDistrict_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_BorderDistrictType = InternalCompilerInterface.GetComponentTypeHandle<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
					m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies5),
					m_DistrictData = InternalCompilerInterface.GetComponentLookup<District>(ref __TypeHandle.__Game_Areas_District_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Nodes = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
					m_Triangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
				}, m_CurrentDistrictQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies5));
				m_AreaSearchSystem.AddSearchTreeReader(val7);
				((SystemBase)this).Dependency = val7;
			}
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
	public CurrentDistrictSystem()
	{
	}
}
